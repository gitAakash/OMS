using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml;
using XeroApi.Model;

namespace CT_DPIService
{
    public class CTDPIService : ICTDPIService
    {

        #region ICTDPIService Interface implementation

        public void TestMethod()
        {

            Dictionary<string, string> returnValues;
            bool isCancelled;
            string strippedText = "";
            List<OrderItem> orderItems;
            List<MultipleOrder> multipleOrdersList;
            List<SalesContact> salesContacts;
            List<AdminContact> adminContacts;
            bool hasMultipleOrders = false;
            strippedText = File.ReadAllText(@"C:\Users\swapnil.gade\Desktop\2.txt");

            ParseRecievedOrder(out returnValues, out isCancelled, strippedText, out orderItems, out hasMultipleOrders, out multipleOrdersList, out salesContacts, out adminContacts);


            if (!isCancelled)
            {
                if (hasMultipleOrders)
                {
                    //process multiple orders
                    foreach (var ord in multipleOrdersList)
                    {
                        if (returnValues.ContainsKey("OrderId"))
                        {
                            returnValues["OrderId"] = ord.OrderId;
                        }
                        else
                        {
                            returnValues.Add("OrderId", ord.OrderId);
                        }
                        returnValues.Add("ProductDescription", ord.ProductDescription);
                        returnValues.Add("RequiredDate", ord.RequiredDate);
                        ProcessOrder(returnValues, DateTime.Now.ToString(), ord.OrderItems, "Multiple Orders", 1, "", "", salesContacts, adminContacts);
                        returnValues.Remove("OrderId");
                        returnValues.Remove("ProductDescription");
                        returnValues.Remove("RequiredDate");
                    }
                }
                else
                {
                    ProcessOrder(returnValues, DateTime.Now.ToString(), orderItems, "Single Ordr", 1, "", "", salesContacts, adminContacts);
                }
            }
            else // process cancelled order
            {
                List<OrderItem> ordItems = new List<OrderItem>();
                InsertCancelledOrders(returnValues["ProductDescription"], returnValues["PropertyAddress"], returnValues["RequiredDate"], "");
            }

        }

        public int ProcessMessages(string sendar, string recipient, string subject, string htmlBody, string strippedHtml, string messageURL, string strippedText, string attachments, DateTime sentDate)//(string attachments)
        {
            int returnCode = 0;
            try
            {
                string stringSentDate = sentDate.ToString("MM/dd/yyyy hh:mm:ss tt");

                string fileName = System.DateTime.Now.ToString("yyyyMMddHHmmssfffffff");// string.Format("{0}{1}{2}{3}{4}{5}{6}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);

                SaveRecievedMailToFileSystem(sendar, stringSentDate, subject, htmlBody, fileName);


                int emailId = 0;
                if (InsertRecievedMailToInbox(sendar, recipient, subject, htmlBody, sentDate, messageURL, false, 1, 1, strippedText, out emailId))
                {
                    returnCode = 200;

                    Dictionary<string, string> attributeValues;
                    List<OrderItem> orderItems;
                    bool isCancelled = false;
                    List<MultipleOrder> multipleOrdersList;
                    List<SalesContact> salesContacts;
                    List<AdminContact> adminContacts;
                    bool hasMultipleOrders = false;
                    ParseRecievedOrder(out attributeValues, out isCancelled, strippedText, out orderItems, out hasMultipleOrders, out multipleOrdersList, out salesContacts, out adminContacts);


                    if (!isCancelled)
                    {
                        if (hasMultipleOrders)
                        {
                            //process multiple orders
                            foreach (var ord in multipleOrdersList)
                            {
                                if (attributeValues.ContainsKey("OrderId"))
                                {
                                    attributeValues["OrderId"] = ord.OrderId;
                                }
                                else
                                {
                                    attributeValues.Add("OrderId", ord.OrderId);
                                }
                                attributeValues.Add("ProductDescription", ord.ProductDescription);
                                attributeValues.Add("RequiredDate", ord.RequiredDate);
                                ProcessOrder(attributeValues, stringSentDate, ord.OrderItems, subject, emailId, fileName, htmlBody, salesContacts, adminContacts);
                                attributeValues.Remove("OrderId");
                                attributeValues.Remove("ProductDescription");
                                attributeValues.Remove("RequiredDate");
                            }
                        }
                        else
                        {
                            ProcessOrder(attributeValues, stringSentDate, orderItems, subject, emailId, fileName, htmlBody, salesContacts, adminContacts);
                        }
                    }
                    else // process cancelled order
                    {
                        List<OrderItem> ordItems = new List<OrderItem>();
                        InsertCancelledOrders(attributeValues["ProductDescription"], attributeValues["PropertyAddress"], attributeValues["RequiredDate"], fileName);
                    }
                    MoveRecievedFile(fileName, false, isCancelled);
                }
                else
                {
                    //Email eml = new Email();
                    //eml.SendMail("Error inserting email to EmailInbox Table.", "FileName " + fileName + ".htm., Email Url = " + messageURL, "gade.swapnil@gmail.com", "", false);
                    MoveRecievedFile(fileName, true, false);
                    returnCode = 460;
                }
            }
            catch (Exception ex)
            {
                string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
                Email eml = new Email();
                eml.SendMail("Error processing before trying to insert to emailInbox email to EmailInbox Table.", " Email Url = " + messageURL + Environment.NewLine + strLog, "gade.swapnil@gmail.com", "", false);
            }
            return returnCode;

        }

        public bool DeletedMail(string MailUid)
        {
            return true;
        }

        #endregion

        #region Save & Move files to file system

        private void MoveRecievedFile(string fileName, bool isFailed, bool isCancelled)
        {
            string mailPath = System.Configuration.ConfigurationSettings.AppSettings["MailPath"];
            if (isFailed)
            {
                string errorPath = System.Configuration.ConfigurationSettings.AppSettings["Error"];
                File.Move(mailPath + fileName + ".htm", errorPath + fileName + ".htm");
            }
            else if (!isFailed && !isCancelled)
            {
                string archivePath = System.Configuration.ConfigurationSettings.AppSettings["Archive"];
                File.Move(mailPath + fileName + ".htm", archivePath + fileName + ".htm");
            }
            else
            {
                string cancelledPath = System.Configuration.ConfigurationSettings.AppSettings["Cancelled"];
                File.Move(mailPath + fileName + ".htm", cancelledPath + fileName + ".htm");
            }
        }

        private void SaveRecievedMailToFileSystem(string sendar, string sentDate, string subject, string htmlBody, string fileName)
        {
            string mailPath = System.Configuration.ConfigurationSettings.AppSettings["MailPath"];
            File.WriteAllText(mailPath + fileName + ".htm", htmlBody);
            File.WriteAllText(mailPath + fileName + ".txt", sendar + Environment.NewLine + sentDate + Environment.NewLine + subject);
        }

        #endregion

        #region Process Orders

        private void ProcessOrder(Dictionary<string, string> attributeValues, string sentDate, List<OrderItem> ordItems, string subject, int emailId, string fileName, string htmlText, List<SalesContact> salesContact, List<AdminContact>
            adminContact)
        {
            Mail mail = new Mail { Subject = subject, OrgId = 825, SentDate = sentDate, Created = DateTime.Now, FileName = fileName + ".htm" };
            CompanyMail companyMail = new CompanyMail { ComanyName = attributeValues["CompanyName"], Created = DateTime.Now };
            Order order = new Order { Description = attributeValues["ProductDescription"], OrderId = attributeValues["OrderId"], OrderTypeId = 1, PropertyId = attributeValues["PropertyId"], RequiredDate = DateTime.Parse(attributeValues["RequiredDate"]) };
            InsertOrderIntoDatabase(mail, companyMail, order, ordItems, emailId, fileName, htmlText, salesContact, adminContact,attributeValues["PropertyAddress"]);
            #region test code
            //if (!InsertOrderIntoDatabase(mail, companyMail, order, ordItems, emailId, fileName,htmlText,salesContact,adminContact))
            //{
            //    Email eml = new Email();
            //    eml.SendMail("Error inserting order details to database", "FileName " + fileName + ".htm.", "gade.swapnil@gmail.com", "", false);
            //}
            #endregion
        }

        #endregion

        #region Parsing of the stripped Text to Dictionary

        private static void ParseRecievedOrder(out Dictionary<string, string> returnValues, out bool isCancelled, string strippedText, out List<OrderItem> orderItems, out bool hasMultipleOrderes, out List<MultipleOrder> multipleOrderList, out List<SalesContact> salesContacts, out List<AdminContact> adminContacts)
        {
            returnValues = new Dictionary<string, string>();
            orderItems = new List<OrderItem>();
            multipleOrderList = new List<MultipleOrder>();
            string preCustomerName = "Campaign Track     (http://www.campaigntrack.com.au) This is an order notification from ";
            string customerName = "";
            string companyName = "";
            string propertyAddress = "";
            string propertyId = "";
            string orderId = "";
            string supplier = "";
            string productDescription = "";
            string requiredDate = "";
            string postCustomerName = "Relating to campaign:";
            string postPropertyAddress = " Property ID:";
            string postPropertyId = " OrderItem:";
            string postOrderId = " Sales contacts:";
            string postSalesContact = " Admin contact:";
            string postAdminContact = " Supplier:";
            string postSupplier = " Description of product/service being ordered:";
            string postProductDescription = " Required on ";
            string postRequiredDate = " Client Price Cost Price ";
            string postOrderDetails = " Click here to view your order.";

            string cancelPreCustomerCompanyName = "Campaign Track     (http://www.campaigntrack.com.au) The order listed below was cancelled by ";
            string cancelPostPropertyAddress = " Description of product/service being cancelled:";
            string tempString = string.Empty;

            isCancelled = false;

            strippedText = strippedText.Replace("\n", "");

            if (strippedText.Contains("The order listed below was cancelled"))
            {
                isCancelled = true;
                hasMultipleOrderes = false;
                salesContacts = new List<SalesContact>();
                adminContacts = new List<AdminContact>();

                tempString = GetTempString(cancelPreCustomerCompanyName, postCustomerName, strippedText);
                //remove this from actula text
                strippedText.Replace(tempString, "");
                tempString = tempString.Replace(cancelPreCustomerCompanyName, "");

                //get Customer and company name
                customerName = tempString.Replace(" at ", ",").Split(',')[0];
                returnValues.Add("CustomerName", customerName);
                companyName = tempString.Replace(" at ", ",").Split(',')[1];
                returnValues.Add("CompanyName", companyName);

                //get Property Address
                tempString = GetTempString(postCustomerName, cancelPostPropertyAddress, strippedText);
                strippedText.Replace(tempString, "");
                propertyAddress = tempString.Replace(postCustomerName, "");
                returnValues.Add("PropertyAddress", propertyAddress);

                //Product Description
                tempString = GetTempString(cancelPostPropertyAddress, postProductDescription, strippedText);
                strippedText.Replace(tempString, "");
                productDescription = tempString.Replace(cancelPostPropertyAddress, "");
                returnValues.Add("ProductDescription", productDescription);

                //Required Date
                tempString = GetTempString(postProductDescription, postRequiredDate, strippedText);
                strippedText.Replace(tempString, "");
                requiredDate = tempString.Replace(postProductDescription, "").Trim();
                if (requiredDate.Contains(" "))
                {
                    var test = requiredDate.Split(' ');
                    DateTime testdate = new DateTime();
                    if (DateTime.TryParse(test[0].ToString(), out testdate))
                        returnValues.Add("RequiredDate", test[0].ToString());
                }
                else
                {
                    returnValues.Add("RequiredDate", requiredDate);
                }
                return;
            }

            //get customer and company information
            tempString = GetTempString(preCustomerName, postCustomerName, strippedText);
            //remove this from actula text
            strippedText.Replace(tempString, "");
            tempString = tempString.Replace(preCustomerName, "");
            //get Customer and company name
            customerName = tempString.Replace(" at ", ",").Split(',')[0];
            returnValues.Add("CustomerName", customerName);
            companyName = tempString.Replace(" at ", ",").Split(',')[1];
            returnValues.Add("CompanyName", companyName);

            //get Property Address
            tempString = GetTempString(postCustomerName, postPropertyAddress, strippedText);
            strippedText.Replace(tempString, "");
            propertyAddress = tempString.Replace(postCustomerName, "");
            returnValues.Add("PropertyAddress", propertyAddress);

            //get Property ID
            tempString = GetTempString(postPropertyAddress, postPropertyId, strippedText);
            strippedText.Replace(tempString, "");
            propertyId = tempString.Replace(postPropertyAddress, "");
            returnValues.Add("PropertyId", propertyId);

            //get Order Id
            tempString = GetTempString(postPropertyId, postOrderId, strippedText);
            strippedText.Replace(tempString, "");
            hasMultipleOrderes = false;
            List<string> orderIds = new List<string>();
            if (tempString.Contains(","))
            {
                hasMultipleOrderes = true;
                tempString = tempString.Replace(postPropertyId, "").Trim();
                orderIds = tempString.Split(',').ToList();
            }
            else
            {
                orderId = tempString.Replace(postPropertyId, "");
            }

            returnValues.Add("OrderId", orderId);


            //get sales Contact
            tempString = GetTempString(postOrderId, postSalesContact, strippedText);
            strippedText.Replace(tempString, "");
            tempString = tempString.Replace(postPropertyId, "");
            salesContacts = new List<SalesContact>();
            if (tempString.Contains(","))
            {
                var list = tempString.Split(',').ToList();
                foreach (var contact in list)
                {
                    string tempContact = contact;
                    if (tempContact.Contains(postPropertyId))
                    {
                        tempContact = tempContact.Replace(postOrderId, "");
                    }
                    string salesExecutiveName = tempContact.Substring(0, tempContact.IndexOf(" on "));
                    string salesContactNumber1 = tempContact.Substring(tempContact.IndexOf(" on "), tempContact.IndexOf(" or") - tempContact.IndexOf(" on ")).Replace(" on ", "");
                    string salesContactNumber2 = tempContact.Substring(tempContact.IndexOf(" or"), tempContact.Length - tempContact.IndexOf(" or")).Replace(" or", "");
                    SalesContact salesContact = new SalesContact { SalesContactName = salesExecutiveName, SalesContactNumber1 = salesContactNumber1, SalesContactNumber2 = salesContactNumber2 };
                    salesContacts.Add(salesContact);
                }
            }
            else
            {
                string tempContact = tempString;
                if (tempContact.Contains(postPropertyId))
                {
                    tempContact = tempContact.Replace(postOrderId, "");
                }
                string salesExecutiveName = tempContact.Substring(0, tempContact.IndexOf(" on "));
                string salesContactNumber1 = tempContact.Substring(tempContact.IndexOf(" on "), tempContact.IndexOf(" or ") - tempContact.IndexOf(" on ")).Replace(" on ", "");
                string salesContactNumber2 = tempContact.Substring(tempContact.IndexOf(" or "), tempContact.Length - tempContact.IndexOf(" or ")).Replace(" or ", "");
                SalesContact salesContact = new SalesContact { SalesContactName = salesExecutiveName, SalesContactNumber1 = salesContactNumber1, SalesContactNumber2 = salesContactNumber2 };
                salesContacts.Add(salesContact);
            }

            strippedText.Replace(tempString, "");


            //get admin Contact
            tempString = GetTempString(postSalesContact, postAdminContact, strippedText);
            strippedText.Replace(tempString, "");
            tempString = tempString.Replace(postPropertyId, "");
            adminContacts = new List<AdminContact>();
            if (tempString.Contains(","))
            {
                var list = tempString.Split(',').ToList();
                foreach (var contact in list)
                {
                    string tempContact = contact;
                    if (tempContact.Contains(postSalesContact))
                    {
                        tempContact = tempContact.Replace(postSalesContact, "");
                    }
                    string adminName = tempContact.Substring(0, tempContact.IndexOf(" on "));
                    string adminEmail = tempContact.Substring(tempContact.IndexOf(" on "), tempContact.IndexOf(" or") - tempContact.IndexOf(" on ")).Replace(" on ", "");
                    string adminPhone = tempContact.Substring(tempContact.IndexOf(" or"), tempContact.Length - tempContact.IndexOf(" or")).Replace(" or", "");
                    AdminContact adminContact = new AdminContact { Name = adminName, Email = adminEmail, Phone = adminPhone };
                    adminContacts.Add(adminContact);
                }
            }
            else
            {
                string tempContact = tempString;
                if (tempContact.Contains(postSalesContact))
                {
                    tempContact = tempContact.Replace(postSalesContact, "");
                }
                string adminName = tempContact.Substring(0, tempContact.IndexOf(" on "));
                string adminPhone = tempContact.Substring(tempContact.IndexOf(" on "), tempContact.IndexOf(" or") - tempContact.IndexOf(" on ")).Replace(" on ", "");
                string adminEmail = tempContact.Substring(tempContact.IndexOf(" or"), tempContact.Length - tempContact.IndexOf(" or")).Replace(" or", "");
                AdminContact adminContact = new AdminContact { Name = adminName.Trim(), Email = adminEmail.Trim(), Phone = adminPhone.Trim() };
                adminContacts.Add(adminContact);
            }

            strippedText.Replace(tempString, "");

            //Sapplier Details
            tempString = GetTempString(postAdminContact, postSupplier, strippedText);
            strippedText.Replace(tempString, "");
            supplier = tempString.Replace(postAdminContact, "");
            returnValues.Add("Supplier", supplier);

            if (hasMultipleOrderes)
            {
                multipleOrderList = new List<MultipleOrder>();
                for (int ord = 0; ord < orderIds.Count; ord++)
                {
                    List<OrderItem> multipleOrderItems = new List<OrderItem>();
                    //Product Description
                    tempString = GetTempString(postSupplier, postProductDescription, strippedText);
                    strippedText.Replace(tempString, "");
                    productDescription = tempString.Replace(postSupplier, "");
                    //returnValues.Add("ProductDescription", productDescription);

                    //Required Date
                    tempString = GetTempString(postProductDescription, postRequiredDate, strippedText);
                    strippedText.Replace(tempString, "");
                    requiredDate = tempString.Replace(postProductDescription, "");
                    //returnValues.Add("RequiredDate", requiredDate);

                    //Order details
                    tempString = GetTempString(postRequiredDate, postOrderDetails, strippedText);
                    strippedText.Replace(tempString, "");
                    string tempOrderDetails = tempString.Replace(postRequiredDate, "");
                    var tempOrderItems = tempOrderDetails.Substring(0, tempOrderDetails.IndexOf(" Total ")).Split('$').ToList();
                    List<string> tempOrderItemReplace = new List<string>();
                    int count = tempOrderItems.Count();
                    if (count > 3)
                    {
                        for (int x = 0; x < count; x++)
                        {
                            if (x % 2 == 0 && x != 0)
                            {
                                decimal tempValue;
                                if (!Decimal.TryParse(tempOrderItems.ElementAt(x), out tempValue))
                                {
                                    var tempListValue = tempOrderItems.ElementAt(x);
                                    string tempValueofDecimal = tempListValue.Substring(0, tempListValue.IndexOf(" "));
                                    string tempItemDescription = tempListValue.Replace(tempValueofDecimal, "");
                                    tempOrderItems.Insert(x, tempValueofDecimal);
                                    tempOrderItems.RemoveAt(x + 1);
                                    tempOrderItems.Insert(x + 1, tempItemDescription.Trim());
                                }
                            }
                        }
                    }

                    int i = 1;

                    OrderItem orderItem = new OrderItem();
                    foreach (var value in tempOrderItems)
                    {
                        if (i == 1)
                        {
                            orderItem.Name = value;
                        }
                        else if (i == 2)
                        {
                            orderItem.ClientPrice = value;
                        }
                        else if (i == 3)
                        {
                            orderItem.CostPrice = value;
                            multipleOrderItems.Add(orderItem);
                            orderItem = new OrderItem();
                            i = 0;
                        }
                        i++;
                    }

                    MultipleOrder multOrder = new MultipleOrder { OrderId = orderIds.ToArray()[ord], ProductDescription = productDescription, RequiredDate = requiredDate, OrderItems = multipleOrderItems };
                    multipleOrderList.Add(multOrder);
                }
            }
            else
            {
                //Product Description
                tempString = GetTempString(postSupplier, postProductDescription, strippedText);
                strippedText.Replace(tempString, "");
                productDescription = tempString.Replace(postSupplier, "");
                returnValues.Add("ProductDescription", productDescription);

                //Required Date
                tempString = GetTempString(postProductDescription, postRequiredDate, strippedText);
                strippedText.Replace(tempString, "");
                requiredDate = tempString.Replace(postProductDescription, "");
                returnValues.Add("RequiredDate", requiredDate);

                //Order details
                tempString = GetTempString(postRequiredDate, postOrderDetails, strippedText);
                strippedText.Replace(tempString, "");
                string tempOrderDetails = tempString.Replace(postRequiredDate, "");
                var tempOrderItems = tempOrderDetails.Substring(0, tempOrderDetails.IndexOf(" Total ")).Split('$').ToList();
                List<string> tempOrderItemReplace = new List<string>();
                int count = tempOrderItems.Count();
                if (count > 3)
                {
                    for (int x = 0; x < count; x++)
                    {
                        if (x % 2 == 0 && x != 0)
                        {
                            decimal tempValue;
                            if (!Decimal.TryParse(tempOrderItems.ElementAt(x), out tempValue))
                            {
                                var tempListValue = tempOrderItems.ElementAt(x);
                                string tempValueofDecimal = tempListValue.Substring(0, tempListValue.IndexOf(" "));
                                string tempItemDescription = tempListValue.Replace(tempValueofDecimal, "");
                                tempOrderItems.Insert(x, tempValueofDecimal);
                                tempOrderItems.RemoveAt(x + 1);
                                tempOrderItems.Insert(x + 1, tempItemDescription.Trim());
                            }
                        }
                    }
                }

                int i = 1;

                OrderItem orderItem = new OrderItem();
                foreach (var value in tempOrderItems)
                {
                    if (i == 1)
                    {
                        orderItem.Name = value;
                    }
                    else if (i == 2)
                    {
                        orderItem.ClientPrice = value;
                    }
                    else if (i == 3)
                    {
                        orderItem.CostPrice = value;
                        orderItems.Add(orderItem);
                        orderItem = new OrderItem();
                        i = 0;
                    }
                    i++;
                }
            }
        }

        private static string GetTempString(string preCustomerName, string postCustomerName, string strippedText)
        {
            return strippedText.Substring(strippedText.IndexOf(preCustomerName), (strippedText.IndexOf(postCustomerName)) - strippedText.IndexOf(preCustomerName));
        }

        #endregion

        #region Select/Insert/update values from database

        private List<TemplateAttributes> GetTemplateAtributes(int templateId)
        {
            List<TemplateAttributes> templateAttributes = new List<TemplateAttributes>();
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("TemplateAttributes", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@TemplateId", SqlDbType.Int).Value = templateId;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    templateAttributes.Add(new TemplateAttributes { AttributeName = row[0].ToString(), ReplacementText = row[1].ToString(), LineNumber = int.Parse(row[2].ToString()) });

                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            return templateAttributes;

        }

        private bool InsertRecievedMailToInbox(string fromMail, string toMail, string subject, string messageBody, DateTime sentDate, string emailUid, bool isDeleted, int folderId, int folderOrder, string strippedText, out int emailId)
        {
            emailId = 0;
            bool success = false;
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertMailToInbox", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@From_Email", SqlDbType.VarChar).Value = fromMail;
                        cmd.Parameters.Add("@To_Email", SqlDbType.VarChar).Value = toMail;
                        cmd.Parameters.Add("@Subject", SqlDbType.VarChar).Value = subject;
                        cmd.Parameters.Add("@Message_Body", SqlDbType.VarChar).Value = messageBody;
                        cmd.Parameters.Add("@Date_Sent", SqlDbType.DateTime).Value = sentDate;
                        cmd.Parameters.Add("@Email_Uid", SqlDbType.VarChar).Value = emailUid;
                        cmd.Parameters.Add("@IsDeleted", SqlDbType.Bit).Value = isDeleted;
                        cmd.Parameters.Add("@Folder_ID", SqlDbType.Int).Value = folderId;
                        cmd.Parameters.Add("@Folder_Order", SqlDbType.Int).Value = folderOrder;
                        cmd.Parameters.Add("@StrippedText", SqlDbType.NVarChar).Value = strippedText;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                emailId = int.Parse(dt.Rows[0][0].ToString());
                            }
                        }
                        if (emailId != 0)
                        {
                            success = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
                Email eml = new Email();
                eml.SendMail("Error processing before trying to insert to emailInbox email to EmailInbox Table.", strLog, "gade.swapnil@gmail.com", "", false);
                success = false;
            }

            return success;

        }

        private bool InsertOrderIntoDatabase(Mail mail, CompanyMail companyMail, Order order, List<OrderItem> orderItems, int emailId, string fileName, string htmlText, List<SalesContact> salesContacts, List<AdminContact> adminContacts,string propertyAddress)
        {
            Email eml = new Email();
            bool status = false;
            int mailId = 0, CompanyMailId = 0, orderId = 0;
            SqlConnection db = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["DbConn"]);

            try
            {
                db.Open();

                mailId = InsertMail(mail, db);

                order.MailId = mailId;

                InsertComapnyMail(companyMail, db, mailId);

                orderId = InsertOrder(order, db);

                List<int> xeroIds = InsertOrderItems(orderItems, db, orderId);

                bool eventCreated = false;

                foreach (var xeroId in xeroIds)
                {
                    if (xeroId != null)
                    {
                        if (!eventCreated)
                        {
                            var dsProductNotification = ProductSchedule(xeroId);
                            if (dsProductNotification.Tables[0].Rows.Count > 0)
                            {
                                string productGroupName = string.Empty;
                                string productGroupId = string.Empty;
                                try
                                {
                                    if (dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["ProductGroupId"] != null)
                                    {
                                        productGroupId = dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["ProductGroupId"].ToString();
                                    }
                                    if (dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["ProductGroupName"] != null)
                                    {
                                        productGroupName = dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["ProductGroupName"].ToString();
                                    }
                                }
                                catch
                                {
                                    productGroupId = string.Empty;
                                    productGroupName = string.Empty;
                                }



                                if (dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["Value"].ToString().Contains("@"))
                                {
                                    eml.SendMail(mail.Subject, htmlText, "gade.swapnil@gmail.com", "", false);
                                    eventCreated = true;

                                }
                                else
                                {
                                    GoogleService.IGoogleNotificationService client = new GoogleService.GoogleNotificationServiceClient();
                                    string orderItemDetails = string.Empty;
                                    if (!string.IsNullOrEmpty(productGroupId))
                                    {
                                        orderItemDetails = "Order Item (Please do not delete or update Order Item Id) :" + order.OrderId + ", " + productGroupId + "-" + productGroupName;
                                    }
                                    else
                                    {
                                        orderItemDetails = "Order Item (Please do not delete or update Order Item Id) :" + order.OrderId;
                                    }
                                    string salesContactDetails = salesContacts.FirstOrDefault().SalesContactName + " on " + salesContacts.FirstOrDefault().SalesContactNumber1 + " or " + salesContacts.FirstOrDefault().SalesContactNumber2;
                                    //foreach (SalesContact salesContact in salesContacts)
                                    //{
                                    //    if (string.IsNullOrEmpty(salesContactDetails))
                                    //    {
                                    //        salesContactDetails = salesContact.SalesContactName + " on " + salesContact.SalesContactNumber1 + " or " + salesContact.SalesContactNumber2;
                                    //    }
                                    //    else
                                    //    {
                                    //        salesContactDetails = salesContactDetails + Environment.NewLine + salesContact.SalesContactName + " on " + salesContact.SalesContactNumber1 + " or " + salesContact.SalesContactNumber2;
                                    //    }
                                    //}

                                    string orderItemDescription = string.Empty;
                                    foreach (var item in orderItems)
                                    {
                                        if (string.IsNullOrEmpty(orderItemDescription))
                                        {
                                            orderItemDescription = item.Name;
                                        }
                                        else
                                        {
                                            orderItemDescription = orderItemDescription + Environment.NewLine + Environment.NewLine + item.Name;
                                        }
                                    }

                                    string description = "SalesContact : " + salesContactDetails + " " + Environment.NewLine + Environment.NewLine
                                                       + "Required On :" + Convert.ToDateTime(order.RequiredDate).ToShortDateString() + Environment.NewLine + Environment.NewLine
                                                       + "Product Description :" + orderItemDescription + Environment.NewLine + Environment.NewLine
                                        //+ "Supplier Instructions :" + dtRecordtoProcess.Rows[i]["Comments"].ToString().Replace("Supplier Instructions", "") + Environment.NewLine 
                                                       + orderItemDetails;
                                    DateTime startDate = Convert.ToDateTime(mail.SentDate).AddHours(10);
                                    int intervalMinutes = int.Parse(dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["Value"].ToString());
                                    client.CreateEvent(dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["Title"].ToString(), propertyAddress + ",Australia", salesContactDetails, startDate, startDate.AddMinutes(intervalMinutes), order.Description, order.Description, order.OrderId, order.RequiredDate.ToString(), "8", "dpi.campaigntrack@gmail.com", description, string.Empty, false);
                                    eventCreated = true;

                                }
                            }
                            UpdateOrderEvent(orderId);//Order Row_Id
                        }
                    }
                }

                status = true;
            }
            catch (SqlException sqlError)
            {
                RollbackOrder(mailId, CompanyMailId, orderId);
                string strLog = sqlError.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + sqlError.Message.ToString();
                eml.SendMail("Error in parsing file.. FileName : " + fileName + ".htm", "EmailInbox table Row_Id = " + emailId + Environment.NewLine + "Error :- " + strLog, "gade.swapnil@gmail.com", "", false);
                status = false;
            }
            if (db.State == ConnectionState.Open)
                db.Close();
            return status;
        }

        private void UpdateOrderEvent(int OrderId)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateOrderEvent", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = OrderId;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        private DataSet ReturnDataforCalenderEntry(string OrderId)
        {
            DataSet ds = new DataSet();
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetDataforCalenderEntry", con))
                    {
                        con.Open();
                        SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@OrderId", SqlDbType.VarChar).Value = OrderId;
                        adapt.Fill(ds);


                    }

                }
            }
            catch (Exception ex)
            { }
            return ds;
        }

        private DataSet ProductSchedule(int XeroId)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                DataSet ds = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetProductSchedules", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@XeroId", SqlDbType.Int).Value = XeroId;

                        con.Open();
                        SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                        adapt.Fill(ds);
                        con.Close();
                    }
                }
                return ds;
            }
            catch (Exception ex) { throw ex; }

        }

        private void RollbackOrder(int mailId, int CompanyMailId, int orderId)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("DeleteUnprocessedOrder", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@MailId", SqlDbType.Int).Value = mailId;
                        cmd.Parameters.Add("@TemplateId", SqlDbType.Int).Value = CompanyMailId;
                        cmd.Parameters.Add("@CompanyMailId", SqlDbType.Int).Value = orderId;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
            }

        }

        private List<int> InsertOrderItems(List<OrderItem> orderItems, SqlConnection db, int orderId)
        {
            List<int> xeroIds = new List<int>();
            foreach (OrderItem orderItem in orderItems)
            {
                var orderItemCommand = new SqlCommand("InsertOrderItem", db);//, transaction);
                orderItemCommand.CommandType = System.Data.CommandType.StoredProcedure;
                orderItemCommand.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = orderItem.Name.Trim().Replace("\n", "").Replace("&", "and");
                orderItemCommand.Parameters.Add("@ClientPrice", System.Data.SqlDbType.NVarChar).Value = orderItem.ClientPrice.Replace("$", "");
                orderItemCommand.Parameters.Add("@CostPrice", System.Data.SqlDbType.Decimal).Value = Convert.ToDecimal(orderItem.CostPrice.Replace("$", ""));
                orderItemCommand.Parameters.Add("@OrderId", System.Data.SqlDbType.Int).Value = orderId;
                SqlDataAdapter da = new SqlDataAdapter(orderItemCommand);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        int no = 0;
                        if (dt.Rows[0][0] != null)
                            if (int.TryParse(dt.Rows[0][0].ToString(), out no))
                                xeroIds.Add(no);
                    }
                }
            }
            return xeroIds;
        }

        private static void InsertComapnyMail(CompanyMail companyMail, SqlConnection db, int mailId)
        {
            var CompanyMailCommand = new SqlCommand("InsertCompanyMail", db);//, transaction);
            CompanyMailCommand.CommandType = System.Data.CommandType.StoredProcedure;
            CompanyMailCommand.Parameters.Add("@CompanyName", System.Data.SqlDbType.NVarChar).Value = companyMail.ComanyName;
            CompanyMailCommand.Parameters.Add("@MailId", System.Data.SqlDbType.Int).Value = companyMail.MailId = mailId;
            CompanyMailCommand.ExecuteNonQuery();
        }

        private int InsertMail(Mail mail, SqlConnection db)
        {
            int mailId = 0;
            var mailCommand = new SqlCommand("InsertMail", db);//, transaction);
            mailCommand.CommandType = System.Data.CommandType.StoredProcedure;
            mailCommand.Parameters.Add("@FileName", System.Data.SqlDbType.NVarChar).Value = mail.FileName;
            mailCommand.Parameters.Add("@Subject", System.Data.SqlDbType.NVarChar).Value = mail.Subject;
            mailCommand.Parameters.Add("@SentDate", System.Data.SqlDbType.NVarChar).Value = mail.SentDate;
            mailCommand.Parameters.Add("@OrgId", System.Data.SqlDbType.Int).Value = mail.OrgId;
            SqlDataAdapter da = new SqlDataAdapter(mailCommand);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    mailId = int.Parse(dt.Rows[0][0].ToString());
                }
            }
            //var mailId = mailCommand.ExecuteScalar();//.ExecuteNonQuery();
            return mailId;// int.Parse(mailId.ToString());
        }

        private int InsertOrder(Order order, SqlConnection db)
        {
            int orderId = 0;
            var orderCommand = new SqlCommand("InsertOrder", db);//, transaction);
            orderCommand.CommandType = System.Data.CommandType.StoredProcedure;
            orderCommand.Parameters.Add("@Property_Id", System.Data.SqlDbType.NVarChar).Value = order.PropertyId;
            orderCommand.Parameters.Add("@OrderId", System.Data.SqlDbType.NVarChar).Value = order.OrderId;
            orderCommand.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar).Value = order.Description;
            orderCommand.Parameters.Add("@Mail_Id", System.Data.SqlDbType.Int).Value = order.MailId;
            orderCommand.Parameters.Add("@OrderType_Id", System.Data.SqlDbType.Int).Value = order.OrderTypeId;
            orderCommand.Parameters.Add("@RequiredDate", System.Data.SqlDbType.DateTime).Value = order.RequiredDate;

            SqlDataAdapter da = new SqlDataAdapter(orderCommand);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    orderId = int.Parse(dt.Rows[0][0].ToString());
                }
            }

            return orderId;
        }

        private void InsertCancelledOrders(string Service, string Property, string RequiredDate, string FileName)
        {
            string connectionString = null;
            connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertCancelledOrder", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add("@OrgId", SqlDbType.Int).Value = orgId;
                    cmd.Parameters.Add("@Service", SqlDbType.VarChar).Value = Service;
                    cmd.Parameters.Add("@Property", SqlDbType.VarChar).Value = Property;
                    cmd.Parameters.Add("@RequiredDate", SqlDbType.VarChar).Value = RequiredDate;
                    cmd.Parameters.Add("@FileName", SqlDbType.VarChar).Value = FileName;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

        }

        #endregion

        #region Models

        public class SalesContact
        {
            public string SalesContactName { get; set; }
            public string SalesContactNumber1 { get; set; }
            public string SalesContactNumber2 { get; set; }
        }
        public class AdminContact
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }
        public class Mail
        {
            public string FileName { get; set; }
            public string Subject { get; set; }
            public string SentDate { get; set; }
            public DateTime Created { get; set; }
            public int OrgId { get; set; }
        }
        public class CompanyMail
        {
            public string ComanyName { get; set; }
            public int MailId { get; set; }
            public DateTime Created { get; set; }
        }
        public class Order
        {
            public int OrderTypeId { get; set; }
            public string PropertyId { get; set; }
            public int MailId { get; set; }
            public string OrderId { get; set; }
            public DateTime RequiredDate { get; set; }
            public string Description { get; set; }
            public DateTime Created { get; set; }
            public string Status { get; set; }
            public string Keys { get; set; }
            public string SpecialInstructions { get; set; }
        }
        public class OrderItem
        {
            public int OrderId { get; set; }
            public int XeroId { get; set; }
            public string Name { get; set; }
            public string ClientPrice { get; set; }
            public string CostPrice { get; set; }
            public string Comments { get; set; }
            public DateTime Created { get; set; }
            public string Options { get; set; }
        }
        public class TemplateAttributes
        {
            public string AttributeName { get; set; }
            public string ReplacementText { get; set; }
            public int LineNumber { get; set; }
        }
        public class Attachment
        {
            public string Url { get; set; }
            public string ContentType { get; set; }
            public string Name { get; set; }
            public string Size { get; set; }
        }
        public class MultipleOrder
        {
            public string OrderId { get; set; }
            public string ProductDescription { get; set; }
            public string RequiredDate { get; set; }
            public List<OrderItem> OrderItems { get; set; }
        }

        #endregion

        #region Create Xero Invoice

        private bool CreateXeroInvoice()
        {
            bool invoiceCreated = false;


            Invoice invoice = new Invoice();
            invoice.Type = "ACCREC";
            //invoice.Contact = new Contact { Name = ds.Tables[0].Rows[0]["XeroName"].ToString() };
            invoice.Date = DateTime.Today;
            invoice.DueDate = DateTime.Today.AddDays(30);
            invoice.Status = "DRAFT";
            invoice.TotalTax = 0;
            invoice.HasAttachments = true;
            invoice.LineAmountTypes = XeroApi.Model.LineAmountType.Inclusive;
            //invoice.Reference = ds.Tables[0].Rows[0]["Property"].ToString();
            LineItems lineitems = new LineItems();
            invoice.LineItems = new XeroApi.Model.LineItems();
            //for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
            //{
            //    if (ds.Tables[0].Rows[j]["CreateInvoice"].ToString() == "True")
            //    {
            //        ds.Tables[1].DefaultView.RowFilter = "OrderId='" + ds.Tables[0].Rows[j]["OrderId"].ToString() + "'";
            //        for (int i = 0; i < ds.Tables[1].DefaultView.Count; i++)
            //        {

            //            LineItem lineItem = new LineItem();
            //            lineItem.ItemCode = ds.Tables[1].DefaultView[i]["XeroCode"].ToString();
            //            lineItem.Description = ds.Tables[1].DefaultView[i]["XeroItemDescription"].ToString();
            //            lineItem.Quantity = 1;
            //            lineItem.UnitAmount = Convert.ToDecimal(ds.Tables[1].DefaultView[i]["Cost1"]);
            //            lineItem.TaxType = ds.Tables[1].DefaultView[i]["SalesTaxType"].ToString().Replace("GST on Income", "OUTPUT");
            //            lineItem.AccountCode = ds.Tables[1].DefaultView[i]["SalesAccountCode"].ToString();
            //            invoice.LineItems.Add(lineItem);
            //        }

            //    }

            //}

            //var inv = repository.Create<XeroApi.Model.Invoice>(invoice);

            return invoiceCreated;
        }

        #endregion
    }
}
