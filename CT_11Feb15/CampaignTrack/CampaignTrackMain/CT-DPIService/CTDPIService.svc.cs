using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Script.Serialization;
using System.Xml;
using DevDefined.OAuth.Consumer;
using XeroApi;
using XeroApi.Model;
using RestSharp;
using Newtonsoft.Json;

namespace CT_DPIService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
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
            strippedText = File.ReadAllText(@"C:\Users\swapnil.gade\Desktop\1204.txt");

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
                        ProcessOrder(returnValues, DateTime.Now, ord.OrderItems, "Multiple Orders", 1, "", "", salesContacts, adminContacts);
                        returnValues.Remove("OrderId");
                        returnValues.Remove("ProductDescription");
                        returnValues.Remove("RequiredDate");
                    }
                }
                else
                {
                    ProcessOrder(returnValues, DateTime.Now, orderItems, "Single Ordr", 1, "", "", salesContacts, adminContacts);
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
                sentDate = sentDate.ToLocalTime();
                string stringSentDate = sentDate.ToString("MM/dd/yyyy hh:mm:ss tt");

                string fileName = System.DateTime.Now.ToString("yyyyMMddHHmmssfffffff");// string.Format("{0}{1}{2}{3}{4}{5}{6}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);               

                // check duplicate emailUid before process the order

                if (!CheckDuplicateMailToInbox(messageURL))
                {
                    int emailId = 0;
                    if (InsertRecievedMailToInbox(sendar, recipient, subject, htmlBody, sentDate, messageURL, false, 1, 1, strippedText, out emailId))
                    {
                        fileName = System.DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
                        SaveRecievedMailToFileSystem(sendar, stringSentDate, subject, htmlBody, fileName);
                        returnCode = ProcessOrder(subject, htmlBody, strippedText, returnCode, sentDate, emailId, fileName);
                    }
                    else
                    {
                        //Email eml = new Email();
                        //eml.SendMail("Error inserting email to EmailInbox Table.", "FileName " + fileName + ".htm., Email Url = " + messageURL, "gade.swapnil@gmail.com", "", false);
                        MoveRecievedFile(fileName, true, false);
                        returnCode = 460;
                    }
                }
            }
            catch (Exception ex)
            {
                string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
                string supportEmail = System.Configuration.ConfigurationSettings.AppSettings["supportEmail"];
                string supportCCEmail = System.Configuration.ConfigurationSettings.AppSettings["supportCCEmail"];
                Email eml = new Email();
                eml.SendMail("Error processing before trying to insert to emailInbox email to EmailInbox Table.", " Email Url = " + messageURL + Environment.NewLine + strLog, supportEmail, supportCCEmail, false);
            }
            return returnCode;

        }
        Invoice invoice = null;

        private int ProcessOrder(string subject, string htmlBody, string strippedText, int returnCode, DateTime stringSentDate,
            int emailId, string fileName)
        {
            returnCode = 200;

            Dictionary<string, string> attributeValues;
            List<OrderItem> orderItems;
            bool isCancelled = false;
            List<MultipleOrder> multipleOrdersList;
            List<SalesContact> salesContacts;
            List<AdminContact> adminContacts;
            bool hasMultipleOrders = false;

            ParseRecievedOrder(out attributeValues, out isCancelled, strippedText, out orderItems, out hasMultipleOrders,
                out multipleOrdersList, out salesContacts, out adminContacts);
            invoice = null;

            if (!isCancelled)
            {

                if (hasMultipleOrders)
                {
                    //process multiple orders

                    foreach (var ord in multipleOrdersList)
                    {
                        if (!exceptionOccured)
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
                            ProcessOrder(attributeValues, stringSentDate, ord.OrderItems, subject, emailId, fileName,
                                htmlBody,
                                salesContacts, adminContacts);
                            attributeValues.Remove("OrderId");
                            attributeValues.Remove("ProductDescription");
                            attributeValues.Remove("RequiredDate");
                        }
                    }


                    if (!exceptionOccured)
                    {
                        if (createInvoice)
                        {
                            try
                            {
                                IOAuthSession session = new XeroApi.OAuth.XeroApiPrivateSession(
                                    "CT-DPI", // Photography",
                                    "N2KFABNCYXNHCGNU7GDVRL3AOUYMS2", //"ZFRY6DJBFEMVYJYBFKJFYDVDJZUXYV",
                                    new X509Certificate2(@"D:\CampaignTrack\Certificate\public_privatekey.pfx",
                                        "zerofootprint"));
                                Repository repository = new Repository(session);
                                var inv = repository.Create<XeroApi.Model.Invoice>(invoice);
                                if (invoice.ValidationStatus == ValidationStatus.ERROR)
                                {
                                    foreach (var message in invoice.ValidationErrors)
                                    {
                                    }
                                }
                                try
                                {
                                    if (inv.InvoiceNumber != null)
                                    {
                                        Guid invoiceId = inv.InvoiceID;
                                        string invoiceNo = inv.InvoiceNumber;
                                        // System.Threading.Thread.Sleep(5000);
                                        string jpegFileName =
                                            System.Configuration.ConfigurationSettings.AppSettings["MailPath"] +
                                            fileName +
                                            ".jpg";
                                        //string AnyAttachmentFilename = fileName;
                                        var SalesInvoice =
                                            repository.Invoices.FirstOrDefault(it => it.InvoiceID == invoiceId);
                                        if (File.Exists(jpegFileName))
                                        {
                                            var newAttachment = repository.Attachments.Create(SalesInvoice,
                                                new FileInfo(jpegFileName));
                                        }
                                        InsertPropertyInvoice(propertyId, invoiceNo);
                                    }

                                }
                                catch
                                {

                                }
                            }
                            catch (Exception sqlError)
                            {
                                Email eml = new Email();
                                string supportEmail =
                                    System.Configuration.ConfigurationSettings.AppSettings["supportEmail"];
                                string supportCCEmail =
                                    System.Configuration.ConfigurationSettings.AppSettings["supportCCEmail"];
                                string strLog = sqlError.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" +
                                                sqlError.Message.ToString();
                                UpdateFailedOrder(emailId);
                                eml.SendMail("Error while creating invoice at Xero : " + fileName + ".htm",
                                    "EmailInbox table Row_Id = " + emailId + Environment.NewLine + "Sentdate : " +
                                    stringSentDate + Environment.NewLine + "Error :- " + strLog, supportEmail,
                                    supportCCEmail, false);

                            }
                        }
                    }
                }
                else
                {
                    ProcessOrder(attributeValues, stringSentDate, orderItems, subject, emailId, fileName, htmlBody,
                        salesContacts, adminContacts);
                    if (!exceptionOccured)
                    {
                        if (createInvoice)
                        {
                            try
                            {
                                IOAuthSession session = new XeroApi.OAuth.XeroApiPrivateSession(
                                    "CT-DPI",
                                    "N2KFABNCYXNHCGNU7GDVRL3AOUYMS2",
                                    new X509Certificate2(@"D:\CampaignTrack\Certificate\public_privatekey.pfx",
                                        "zerofootprint"));
                                Repository repository = new Repository(session);
                                var inv = repository.Create<XeroApi.Model.Invoice>(invoice);
                                if (invoice.ValidationStatus == ValidationStatus.ERROR)
                                {
                                    foreach (var message in invoice.ValidationErrors)
                                    {
                                    }
                                }
                                try
                                {
                                    if (inv.InvoiceNumber != null)
                                    {
                                        Guid invoiceId = inv.InvoiceID;
                                        string invoiceNo = inv.InvoiceNumber;
                                        // System.Threading.Thread.Sleep(5000);
                                        string jpegFileName =
                                            System.Configuration.ConfigurationSettings.AppSettings["MailPath"] +
                                            fileName +
                                            ".jpg";
                                        var SalesInvoice =
                                            repository.Invoices.FirstOrDefault(it => it.InvoiceID == invoiceId);
                                        if (File.Exists(jpegFileName))
                                        {
                                            var newAttachment = repository.Attachments.Create(SalesInvoice,
                                                new FileInfo(jpegFileName));
                                        }
                                        InsertPropertyInvoice(propertyId, invoiceNo);
                                    }
                                }
                                catch (Exception sqlError)
                                {

                                }

                            }
                            catch (Exception sqlError)
                            {
                                Email eml = new Email();
                                string supportEmail =
                                    System.Configuration.ConfigurationSettings.AppSettings["supportEmail"];
                                string supportCCEmail =
                                    System.Configuration.ConfigurationSettings.AppSettings["supportCCEmail"];
                                string strLog = sqlError.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" +
                                                sqlError.Message.ToString();
                                UpdateFailedOrder(emailId);
                                eml.SendMail("Error while creating invoice at Xero : " + fileName + ".htm",
                                    "EmailInbox table Row_Id = " + emailId + Environment.NewLine + "Sentdate : " +
                                    stringSentDate + Environment.NewLine + "Error :- " + strLog, supportEmail,
                                    supportCCEmail, false);

                            }
                        }
                    }
                }
            }
            else // process cancelled order
            {
                List<OrderItem> ordItems = new List<OrderItem>();
                InsertCancelledOrders(attributeValues["ProductDescription"], attributeValues["PropertyAddress"],
                    attributeValues["RequiredDate"], fileName);
            }
            MoveRecievedFile(fileName, false, isCancelled);
            return returnCode;
        }

        ///<summary>
        ///Make Invoice entry against a property
        ///</summary>
        ///<param name="PropertyId">Property Id</param>
        ///<param name="InvoiceNo">Invoice No</param>
        public static void InsertPropertyInvoice(int PropertyId, string InvoiceNo)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertPropertyInvoiceNo", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PropertyId", SqlDbType.Int).Value = PropertyId;
                        cmd.Parameters.Add("@InvoiceNo", SqlDbType.VarChar).Value = InvoiceNo;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }


        public bool DeletedMail(string MailUid)
        {
            GetMessage();
            //ProcessMessages("Campaigntrack@campaigntrack.com.au,Campaigntrack@campaigntrack.com.au", "campaigntrack.dpi@schedly.com.au", "151 Dow St, Port Melbourne (Marshall White Albert Park)", "", "", "https://api.mailgun.net/v2/domains/schedly.com.au/messages/WyJmNzhmNzY0Mzk1IiwgWyI2YTQ0ZGVkYS01ZTQ0LTQwNTUtYjhlMy03YzQ4MGJjNzkxYWYiXSwgIm1haWxndW4iLCAib2xkY29rZSJd", "", "", DateTime.Now);
            return true;
        }

        public void ProcessFailedMessages()
        {
            string fileName = System.DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
            DataTable dt = GetFailedMessages();
            foreach (DataRow row in dt.Rows)
            {
                int row_Id = int.Parse(row["Row_Id"].ToString());
                UpdateFailedOrderToProcessed(row_Id);
                DateTime dt123 = Convert.ToDateTime(row["DATE_SENT"].ToString());
                SaveRecievedMailToFileSystem(row["FROM_EMAIL"].ToString(), dt123.ToString(), row["SUBJECT"].ToString(), row["MESSAGE_BODY"].ToString(), fileName);
                ProcessOrder(row["SUBJECT"].ToString(), row["MESSAGE_BODY"].ToString(), row["StrippedText"].ToString(),
                    0, dt123, row_Id, fileName);
            }
        }

        private void UpdateFailedOrderToProcessed(int row_id)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateFailedOrderToProcessed", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Row_Id", SqlDbType.Int).Value = row_id;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public void GetMessage()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2/domains/schedly.com.au/messages/WyI0YjgyMTNiMTgxIiwgWyJhMWM1YTczNy1jN2NlLTQ0MDYtODgxMy04OGY0ZGU1NWMzY2EiXSwgIm1haWxndW4iLCAib2RpbiJd");
            //  client.BaseUrl = new Uri("https://api.mailgun.net/v2/domains/schedly.com.au/messages/WyJmNzhmNzY0Mzk1IiwgWyI2YTQ0ZGVkYS01ZTQ0LTQwNTUtYjhlMy03YzQ4MGJjNzkxYWYiXSwgIm1haWxndW4iLCAib2xkY29rZSJd");


            // 
            client.Authenticator = new HttpBasicAuthenticator(
                "api", "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            //request.AddParameter("domain",
            //                    "schedly.com.au", ParameterType.UrlSegment);
            //request.Resource = "{domain}/messages/WyJmNzhmNzY0Mzk1IiwgWyI2YTQ0ZGVkYS01ZTQ0LTQwNTUtYjhlMy03YzQ4MGJjNzkxYWYiXSwgIm1haWxndW4iLCAib2xkY29rZSJd";

            request.Method = Method.GET;
            var test = client.Execute(request);
            var test123 = JsonConvert.DeserializeObject<dynamic>(test.Content);
            //var body = test123["Stripped-Text"];
            var body = test123["stripped-text"];
            //return test;
        }


        #endregion

        #region Save & Move files to file system

        private void MoveRecievedFile(string fileName, bool isFailed, bool isCancelled)
        {
            try
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
            catch
            {



            }

        }

        private void SaveRecievedMailToFileSystem(string sendar, string sentDate, string subject, string htmlBody, string fileName)
        {
            try
            {
                string mailPath = System.Configuration.ConfigurationSettings.AppSettings["MailPath"];
                File.WriteAllText(mailPath + fileName + ".htm", htmlBody);
                File.WriteAllText(mailPath + fileName + ".txt", sendar + Environment.NewLine + sentDate + Environment.NewLine + subject);
                SaveImage(fileName);
            }
            catch
            { }
        }

        #endregion

        #region Process Orders

        private void ProcessOrder(Dictionary<string, string> attributeValues, DateTime sentDate, List<OrderItem> ordItems, string subject, int emailId, string fileName, string htmlText, List<SalesContact> salesContact, List<AdminContact>
            adminContact)
        {
            Mail mail = new Mail { Subject = subject, OrgId = 825, SentDate = sentDate.ToString(), Created = DateTime.Now, FileName = fileName + ".htm" };
            CompanyMail companyMail = new CompanyMail { ComanyName = attributeValues["CompanyName"], Created = DateTime.Now };
            var requiredOnDate = attributeValues["RequiredDate"].Split('/');
            var day = int.Parse(requiredOnDate[0]);
            var month = int.Parse(requiredOnDate[1]);
            var year = int.Parse(requiredOnDate[2]);
            Order order = new Order { Description = attributeValues["ProductDescription"], OrderId = attributeValues["OrderId"], OrderTypeId = 1, PropertyId = attributeValues["PropertyId"], RequiredDate = new DateTime(year, month, day) };
            InsertOrderIntoDatabase(mail, companyMail, order, ordItems, emailId, fileName, htmlText, salesContact, adminContact, attributeValues["PropertyAddress"], sentDate);
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

        private static bool isCompanyMail = false;
        private static void ParseRecievedOrder(out Dictionary<string, string> returnValues, out bool isCancelled, string strippedText, out List<OrderItem> orderItems, out bool hasMultipleOrderes, out List<MultipleOrder> multipleOrderList, out List<SalesContact> salesContacts, out List<AdminContact> adminContacts)
        {
            isCompanyMail = false;
            returnValues = new Dictionary<string, string>();
            orderItems = new List<OrderItem>();
            multipleOrderList = new List<MultipleOrder>();
            // string preCustomerName = "Campaign Track     (http://www.campaigntrack.com.au) This is an order notification from ";
            string preCustomerName = "Campaign Track     (http://www.campaigntrack.com.au) This is an order notification from ";
            string preCustomerNameWithOutUrl = "Campaign Track  This is an order notification from ";
            string preCustomerNameForNewLitho = "New Litho   This is an order notification from ";
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

            string cancelPreCustomerCompanyNameWithOutUrl = "Campaign Track  The order listed below was cancelled by ";

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




                bool match = false;
                match = ExactMatch(strippedText, cancelPreCustomerCompanyName);

                if (match)
                {
                    tempString = GetTempString(cancelPreCustomerCompanyName, postCustomerName, strippedText);
                    //remove this from actula text
                    strippedText.Replace(tempString, "");
                    tempString = tempString.Replace(cancelPreCustomerCompanyName, "");
                }

                else
                {

                    /////////////////////////////////

                    match = ExactMatch(strippedText, cancelPreCustomerCompanyNameWithOutUrl);

                    if (match)
                    {
                        tempString = GetTempString(cancelPreCustomerCompanyNameWithOutUrl, postCustomerName, strippedText);
                        //remove this from actula text
                        strippedText.Replace(tempString, "");
                        tempString = tempString.Replace(cancelPreCustomerCompanyNameWithOutUrl, "");
                    }

                    else
                    {
                        match = ExactMatch(strippedText, cancelPreCustomerCompanyNameWithOutUrl);

                        if (match)
                        {
                            tempString = GetTempString(cancelPreCustomerCompanyNameWithOutUrl, postCustomerName, strippedText);
                        }
                        else
                        {
                            // Remove Extra spaces from the string and check it again in the stripeed text 
                            cancelPreCustomerCompanyNameWithOutUrl = RemoveExtraSpaces(cancelPreCustomerCompanyNameWithOutUrl);
                            tempString = GetTempString(cancelPreCustomerCompanyNameWithOutUrl, postCustomerName, strippedText);

                        }
                        //remove this from actula text
                        strippedText.Replace(tempString, "");
                        tempString = tempString.Replace(cancelPreCustomerCompanyNameWithOutUrl, "");
                    }

                    ////////////////////////////////  old one ///////////////////////

                    //match = ExactMatch(strippedText, cancelPreCustomerCompanyNameWithOutUrl);
                    //tempString = GetTempString(cancelPreCustomerCompanyNameWithOutUrl, postCustomerName, strippedText);
                    ////remove this from actula text
                    //strippedText.Replace(tempString, "");
                    //tempString = tempString.Replace(cancelPreCustomerCompanyNameWithOutUrl, "");

                    //////////////////////////////// End old one ///////////////////////
                }




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

                    else
                    {
                        string dateString = "20/05/2015"; // <-- Valid
                        string format = "dd/mmm/yyyy";
                        DateTime dateTime;
                        if (DateTime.TryParseExact(test[0].ToString(), format, CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out dateTime))
                        {
                            returnValues.Add("RequiredDate", test[0].ToString());
                        }

                    }
                }
                else
                {
                    returnValues.Add("RequiredDate", requiredDate);
                }
                return;
            }

            if (strippedText.StartsWith("New Litho"))
            {
                //get customer and company information
                tempString = GetTempString(preCustomerNameForNewLitho, postCustomerName, strippedText);
                //remove this from actula text
                strippedText.Replace(tempString, "");
                tempString = tempString.Replace(preCustomerNameForNewLitho, "");
                isCompanyMail = true;
            }

            else
            {
                //get customer and company information


                bool match = false;
                match = ExactMatch(strippedText, preCustomerName);

                if (match)
                {
                    tempString = GetTempString(preCustomerName, postCustomerName, strippedText);
                    //remove this from actula text
                    strippedText.Replace(tempString, "");
                    tempString = tempString.Replace(preCustomerName, "");
                }

                else
                {
                    match = ExactMatch(strippedText, preCustomerNameWithOutUrl);

                    if (match)
                    {
                        tempString = GetTempString(preCustomerNameWithOutUrl, postCustomerName, strippedText);
                    }
                    else
                    {
                        // Remove Extra spaces from the string and check it again in the stripeed text 
                        preCustomerNameWithOutUrl = RemoveExtraSpaces(preCustomerNameWithOutUrl);
                        tempString = GetTempString(preCustomerNameWithOutUrl, postCustomerName, strippedText);

                    }
                    //remove this from actula text
                    strippedText.Replace(tempString, "");
                    tempString = tempString.Replace(preCustomerNameWithOutUrl, "");
                }
            }

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
            tempString = tempString.Replace(postOrderId, "");
            salesContacts = new List<SalesContact>();
            if (tempString.Contains(","))
            {
                var list = tempString.Split(',').ToList();
                foreach (var contact in list)
                {
                    string tempContact = contact;
                    if (tempContact.Contains(postOrderId))
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
                if (tempContact.Contains(postOrderId))
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
                for (int ord = 1; ord <= orderIds.Count; ord++)
                {
                    List<OrderItem> multipleOrderItems = new List<OrderItem>();
                    //Product Description
                    tempString = GetTempString(postSupplier, postProductDescription, strippedText);
                    strippedText = ReplaceFirst(strippedText, tempString, "");// strippedText.Replace(tempString, "");
                    productDescription = tempString.Replace(postSupplier, "");
                    //returnValues.Add("ProductDescription", productDescription);

                    //Required Date
                    tempString = GetTempString(postProductDescription, postRequiredDate, strippedText);
                    strippedText = ReplaceFirst(strippedText, tempString, ""); //strippedText.Replace(tempString, "");
                    requiredDate = tempString.Replace(postProductDescription, "");
                    //returnValues.Add("RequiredDate", requiredDate);
                    if (ord < orderIds.Count)
                    {
                        //Order details
                        tempString = GetTempString(postRequiredDate, postSupplier, strippedText);

                    }
                    else
                    {
                        tempString = GetTempString(postRequiredDate, postOrderDetails, strippedText);
                    }
                    strippedText = ReplaceFirst(strippedText, tempString, ""); //strippedText.Replace(tempString, "");
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
                            if (value.Equals("0.00 Supplier Instructions - Property Ready: Monday 29th JuneContact: Dom dominika.runkowska@marshallwhite.com.au 98324774Access: Via vendorVendor: Michael Kroger (contact Dom)Property: 4bed / 2.5bath house "))
                            {
                                string value1 = value.Replace(" Supplier Instructions - Property Ready: Monday 29th JuneContact: Dom dominika.runkowska@marshallwhite.com.au 98324774Access: Via vendorVendor: Michael Kroger (contact Dom)Property: 4bed / 2.5bath house ", string.Empty);
                                orderItem.CostPrice = value1;
                            }
                            else
                            {
                                orderItem.CostPrice = value;
                            }

                            multipleOrderItems.Add(orderItem);
                            orderItem = new OrderItem();
                            i = 0;
                        }
                        i++;
                    }

                    MultipleOrder multOrder = new MultipleOrder { OrderId = orderIds.ToArray()[ord - 1], ProductDescription = productDescription, RequiredDate = requiredDate, OrderItems = multipleOrderItems };
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

                //var dollorCount = tempOrderDetails.ToArray().Where(x => x == '$').Count();

                ////decimal test3 = dollorCount/2;
                ////int test2=0;
                //if (dollorCount % 2 !=0)
                //{
                //    int dollorIndex= tempOrderDetails.IndexOf('$');
                //    tempOrderDetails = tempOrderDetails.Remove(dollorIndex,1);

                //}   

                tempOrderDetails = tempOrderDetails.Replace("$54.00 incl GST", "54.00 incl GST");

                tempOrderDetails = tempOrderDetails.Replace("Adjustment: Supplier - As per Jess B $175.00 $2.30 $2.30 ", string.Empty);

                // replace the content as this is not a part of order - row_id- 2071


                var tempOrderItems = tempOrderDetails.Substring(0, tempOrderDetails.IndexOf(" Total ")).Split('$').ToList();
                List<string> tempOrderItemReplace = new List<string>();
                int count = tempOrderItems.Count();
                if (count > 2)
                {
                    decimal test;
                    if (!Decimal.TryParse(tempOrderItems[2], out test))
                    {
                        var testpriceIssue = tempOrderItems[2].Split(' ');
                        tempOrderItems.Insert(2, testpriceIssue[0].Trim());
                        tempOrderItems[3] = tempOrderItems[3].Replace(testpriceIssue[0].Trim(), string.Empty);
                    }
                    if (tempOrderItems.Count > 5)
                    {
                        if (!Decimal.TryParse(tempOrderItems[5], out test))
                        {
                            var testpriceIssue = tempOrderItems[5].Split(' ');
                            tempOrderItems.Insert(5, testpriceIssue[0].Trim());
                            tempOrderItems[6] = tempOrderItems[6].Replace(testpriceIssue[0].Trim(), string.Empty);
                        }
                    }
                    if (tempOrderItems.Count > 8)
                    {
                        if (!Decimal.TryParse(tempOrderItems[8], out test))
                        {
                            var testpriceIssue = tempOrderItems[8].Split(' ');
                            tempOrderItems.Insert(8, testpriceIssue[0].Trim());
                            tempOrderItems[9] = tempOrderItems[9].Replace(testpriceIssue[0].Trim(), string.Empty);
                        }
                    }
                    if (tempOrderItems.Count > 11)
                    {
                        if (!Decimal.TryParse(tempOrderItems[11], out test))
                        {
                            var testpriceIssue = tempOrderItems[11].Split(' ');
                            tempOrderItems.Insert(11, testpriceIssue[0].Trim());
                            tempOrderItems[12] = tempOrderItems[12].Replace(testpriceIssue[0].Trim(), string.Empty);
                        }
                    }
                    if (tempOrderItems.Count > 14)
                    {
                        if (!Decimal.TryParse(tempOrderItems[14], out test))
                        {
                            var testpriceIssue = tempOrderItems[14].Split(' ');
                            tempOrderItems.Insert(14, testpriceIssue[0]);
                            tempOrderItems[15] = tempOrderItems[15].Replace(testpriceIssue[0], string.Empty);
                        }
                    }
                    if (tempOrderItems.Count > 17)
                    {
                        if (!Decimal.TryParse(tempOrderItems[17], out test))
                        {
                            var testpriceIssue = tempOrderItems[17].Split(' ');
                            tempOrderItems.Insert(17, testpriceIssue[0]);
                            tempOrderItems[18] = tempOrderItems[18].Replace(testpriceIssue[0], string.Empty);
                        }
                    }
                    if (tempOrderItems.Count > 20)
                    {
                        if (!Decimal.TryParse(tempOrderItems[20], out test))
                        {
                            var testpriceIssue = tempOrderItems[20].Split(' ');
                            tempOrderItems.Insert(20, testpriceIssue[0]);
                            tempOrderItems[21] = tempOrderItems[21].Replace(testpriceIssue[0], string.Empty);
                        }
                    }
                    if (tempOrderItems.Count > 23)
                    {
                        if (!Decimal.TryParse(tempOrderItems[23], out test))
                        {
                            var testpriceIssue = tempOrderItems[23].Split(' ');
                            tempOrderItems.Insert(23, testpriceIssue[0]);
                            tempOrderItems[24] = tempOrderItems[24].Replace(testpriceIssue[0], string.Empty);
                        }
                    }
                    if (tempOrderItems.Count > 26)
                    {
                        if (!Decimal.TryParse(tempOrderItems[26], out test))
                        {
                            var testpriceIssue = tempOrderItems[26].Split(' ');
                            tempOrderItems.Insert(26, testpriceIssue[0]);
                            tempOrderItems[27] = tempOrderItems[27].Replace(testpriceIssue[0], string.Empty);
                        }
                    }
                    //if (!Decimal.TryParse(tempOrderItems[9], out test))
                    //{
                    //    var testpriceIssue = tempOrderItems[9].Split(' ');
                    //    tempOrderItems.Insert(9, testpriceIssue[0]);
                    //}
                    //for (int x = 0; x < count; x++)
                    //{
                    //    if (x % 2 == 0 && x != 0)
                    //    {
                    //        decimal tempValue;
                    //        if (!Decimal.TryParse(tempOrderItems.ElementAt(x).Trim(), out tempValue))
                    //        {
                    //            var tempListValue = tempOrderItems.ElementAt(x).Trim();
                    //            string tempValueofDecimal = tempListValue.Trim().Substring(0, tempListValue.IndexOf(" "));
                    //            string tempItemDescription = tempListValue;
                    //            if (!string.IsNullOrEmpty(tempValueofDecimal))
                    //            {
                    //                tempItemDescription = tempListValue.Replace(tempValueofDecimal, "");
                    //            }
                    //            tempOrderItems.Insert(x, tempValueofDecimal);
                    //            tempOrderItems.RemoveAt(x + 1);
                    //            tempOrderItems.Insert(x + 1, tempItemDescription.Trim());
                    //        }
                    //    }
                    //}
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



                        int l = value.Trim().IndexOf(" ");
                        if (l > 0)
                        {
                            string CPrice = string.Empty;
                            CPrice = value.Substring(0, l);
                            orderItem.ClientPrice = CPrice;
                        }
                        else
                        {
                            orderItem.ClientPrice = value;
                        }
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

        private static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        private static string GetTempString(string preCustomerName, string postCustomerName, string strippedText)
        {
            return strippedText.Substring(strippedText.IndexOf(preCustomerName), (strippedText.IndexOf(postCustomerName)) - strippedText.IndexOf(preCustomerName));
        }

        #endregion

        #region Select/Insert/update values from database

        private DataTable GetFailedMessages()
        {
            DataTable dataTable = new DataTable();

            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SelectFailedOrders", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dataTable);
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

            return dataTable;
        }

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
                        cmd.Parameters.Add("@Date_Sent", SqlDbType.DateTime).Value = sentDate.ToLocalTime();
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
                string supportEmail = System.Configuration.ConfigurationSettings.AppSettings["supportEmail"];
                string supportCCEmail = System.Configuration.ConfigurationSettings.AppSettings["supportCCEmail"];
                Email eml = new Email();
                eml.SendMail("Error processing before trying to insert to emailInbox email to EmailInbox Table.", strLog, supportEmail, supportCCEmail, false);
                success = false;
            }

            return success;

        }


        private bool CheckDuplicateMailToInbox(string emailUid)
        {
            int RowCount = 0;
            bool success = false;
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("CheckDuplicateMailToInbox", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@MsgURl", SqlDbType.VarChar).Value = emailUid;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                RowCount = int.Parse(dt.Rows[0][0].ToString());
                            }
                        }
                        if (RowCount > 1)
                        {
                            success = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
                //string supportEmail = System.Configuration.ConfigurationSettings.AppSettings["supportEmail"];
                //string supportCCEmail = System.Configuration.ConfigurationSettings.AppSettings["supportCCEmail"];
                //Email eml = new Email();
                //eml.SendMail("Error processing before trying to insert to emailInbox email to EmailInbox Table.", strLog, supportEmail, supportCCEmail, false);
                success = false;
            }

            return success;

        }


        private int propertyId = 0;
        private bool createInvoice = false;
        private bool exceptionOccured = false;
        private bool InsertOrderIntoDatabase(Mail mail, CompanyMail companyMail, Order order, List<OrderItem> orderItems, int emailId, string fileName, string htmlText, List<SalesContact> salesContacts, List<AdminContact> adminContacts, string propertyAddress, DateTime sentDate)
        {
            string year = "", month = "", day = "";
            Email eml = new Email();
            bool status = false;
            int mailId = 0, CompanyMailId = 0, orderId = 0;
            int companyId = 0, userId = 0, calendarId = 0;
            string XeroCompanyName = string.Empty;
            int adminId = 0;
            int salesId = 0;
            SqlConnection db = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["DbConn"]);

            try
            {
                db.Open();

                mailId = InsertMail(mail, db);

                order.MailId = mailId;

                InsertComapnyMail(companyMail, db, mailId, order.PropertyId, propertyAddress);

                // insert sales contact
                foreach (var salesContact in salesContacts)
                {
                    InsertContact(companyMail.ComanyName, salesContact.SalesContactName, salesContact.SalesContactNumber1, "Sales", out salesId);
                }

                // insert admin contact
                foreach (var adminContact in adminContacts)
                {

                    InsertContact(companyMail.ComanyName, adminContact.Name, adminContact.Email, "Admin", out adminId);
                }

                orderId = InsertOrder(order, db);
                Dictionary<int, XeroProductInfo> xeroCodeCostDictionary = new Dictionary<int, XeroProductInfo>();
                List<int> xeroIds = InsertOrderItems(orderItems, db, orderId, out xeroCodeCostDictionary);

                GetCompanyIdPropertyIdUserIdCalendarId(companyMail.ComanyName, order.PropertyId, "campaigntrack.dpi@gmail.com", out companyId, out propertyId, out userId, out calendarId, out createInvoice, out XeroCompanyName);//campaigntrack.dpi@gmail.com

                if (propertyId != 0 && !string.IsNullOrEmpty(propertyAddress.Trim()))
                {
                    ProprtyCoordinates(propertyId, propertyAddress.Trim());
                }

                string companyCode = GetCompanyCode(companyMail.ComanyName);
                if (string.IsNullOrEmpty(companyCode))
                {
                    RollbackOrder(mailId, CompanyMailId, orderId);
                    string supportEmail = System.Configuration.ConfigurationSettings.AppSettings["supportEmail"];
                    string supportCCEmail = System.Configuration.ConfigurationSettings.AppSettings["supportCCEmail"];
                    string strLog = "~~~~~~~~~~~~No Company Match Found : Company Name : " + companyMail.ComanyName + "~~~~~~~~~~~:";
                    UpdateFailedOrder(emailId);
                    MoveRecievedFile(fileName, true, false);
                    eml.SendMail("No Company Match Found FileName : " + fileName + ".htm", "EmailInbox table Row_Id = " + emailId + Environment.NewLine + "Sentdate : " + sentDate + Environment.NewLine + "Error :- " + strLog, supportEmail, supportCCEmail, false);
                    status = false;
                    exceptionOccured = true;
                    return false;
                }

                bool eventCreated = false;

                if (!isCompanyMail && xeroIds.Count == 0)
                {
                    RollbackOrder(mailId, CompanyMailId, orderId);
                    string supportEmail = System.Configuration.ConfigurationSettings.AppSettings["supportEmail"];
                    string supportCCEmail = System.Configuration.ConfigurationSettings.AppSettings["supportCCEmail"];
                    string strLog = "~~~~~~~~~~~~No Product Match Found : Company Name : " + companyMail.ComanyName + "~~~~~~~~~~~:";
                    UpdateFailedOrder(emailId);
                    MoveRecievedFile(fileName, true, false);
                    eml.SendMail("No Product Match Found FileName : " + fileName + ".htm", "EmailInbox table Row_Id = " + emailId + Environment.NewLine + "Sentdate : " + sentDate + Environment.NewLine + "Error :- " + strLog, supportEmail, supportCCEmail, false);
                    status = false;
                    exceptionOccured = true;
                    return false;
                }
                if (!isCompanyMail)
                {
                    foreach (var xeroId in xeroIds)
                    {
                        if (xeroId != null)
                        {
                            //if (!eventCreated)
                            //{
                            var dsProductNotification = ProductSchedule(xeroId);
                            if (dsProductNotification.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow row in dsProductNotification.Tables[0].Rows)
                                {
                                    string productGroupName = string.Empty;
                                    string productGroupId = string.Empty;
                                    try
                                    {
                                        if (row["ProductGroupId"] != null)
                                        {
                                            productGroupId = row["ProductGroupId"].ToString();
                                        }
                                        if (row["ProductGroupName"] != null)
                                        {
                                            productGroupName = row["ProductGroupName"].ToString();
                                        }
                                    }
                                    catch
                                    {
                                        productGroupId = string.Empty;
                                        productGroupName = string.Empty;
                                    }



                                    if (row["Value"].ToString().Contains("@"))
                                    {
                                        string dpiEmail = System.Configuration.ConfigurationSettings.AppSettings["dpiEmail"];
                                        string dpiCCEmail = System.Configuration.ConfigurationSettings.AppSettings["dpiCCEmail"];
                                        // need to comment below code while you debug this form local machine Manoj
                                        eml.SendMail(mail.Subject, htmlText, dpiEmail, dpiCCEmail, false);
                                        eventCreated = true;

                                    }
                                    else
                                    {
                                        GoogleService.IGoogleNotificationService client = new GoogleService.GoogleNotificationServiceClient();
                                        string orderItemDetails = string.Empty;
                                        if (!string.IsNullOrEmpty(productGroupId))
                                        {
                                            if (!string.IsNullOrEmpty(productGroupName))
                                            {
                                                orderItemDetails =
                                                    "Order Item (Please do not delete or update Order Item Id) :" +
                                                    order.OrderId + ", " + productGroupId + "-" + productGroupName;
                                            }
                                            else
                                            {
                                                orderItemDetails =
                                                    "Order Item (Please do not delete or update Order Item Id) :" +
                                                    order.OrderId + ", " + productGroupId;
                                            }
                                        }
                                        else
                                        {
                                            orderItemDetails = "Order Item (Please do not delete or update Order Item Id) :" + order.OrderId;
                                        }
                                        string salesContactDetails = salesContacts.FirstOrDefault().SalesContactName + " on " + salesContacts.FirstOrDefault().SalesContactNumber1 + " or " + salesContacts.FirstOrDefault().SalesContactNumber2;

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

                                        string title = companyCode + ":" + propertyAddress + ":" + row["Title"].ToString() ?? order.Description;
                                        string colorId = row["ColorId"].ToString();

                                        //GetStartDate(mail.SentDate, out year, out month, out day);
                                        var startDate = sentDate;// GetStartDate(mail.SentDate);


                                        //DateTime startDate = Convert.ToDateTime(mail.SentDate);


                                        if (startDate.TimeOfDay.Minutes < 15 && startDate.TimeOfDay.Minutes > 0)
                                        {
                                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, 00, 00);
                                        }
                                        else if (startDate.TimeOfDay.Minutes < 30 && startDate.TimeOfDay.Minutes > 15)
                                        {
                                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, 30, 00);
                                        }

                                        else if (startDate.TimeOfDay.Minutes < 45 && startDate.TimeOfDay.Minutes > 30)
                                        {
                                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, 30, 00);
                                        }
                                        else if (startDate.TimeOfDay.Minutes < 45)
                                        {
                                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour + 1, 00, 00);
                                        }
                                        int intervalMinutes = int.Parse(row["Value"].ToString());

                                        // need to change campaigntrack.dpi@gmail.com into dpi.campaigntrack@gmail.com below code while you debug this form local machine 
                                        var returnEventId = client.CreateEvent(title, propertyAddress + ",Australia", salesContactDetails, startDate, startDate.AddMinutes(intervalMinutes), order.Description, order.Description, order.OrderId, order.RequiredDate.ToString(), colorId, "campaigntrack.dpi@gmail.com", description, string.Empty, false);
                                        // var returnEventId = client.CreateEvent(title, propertyAddress + ",Australia", salesContactDetails, startDate, startDate.AddMinutes(intervalMinutes), order.Description, order.Description, order.OrderId, order.RequiredDate.ToString(), colorId, "dpi.campaigntrack@gmail.com", description, string.Empty, false);
                                        if (!string.IsNullOrEmpty(returnEventId))
                                        {
                                            eventCreated = true;
                                            int eventId = GetEventId(returnEventId.Split(',')[0]);
                                            int jobId = CreateJob(825, orderId, userId, propertyId, order.RequiredDate, mailId, companyId, "Not Cancelled", "", adminId);
                                            CreateEvent(825, jobId, userId, calendarId, eventId, int.Parse(productGroupId), "Not Cancelled", "");
                                        }
                                        else
                                        {
                                            throw new Exception("Google service has problem");
                                        }
                                    }
                                    UpdateOrderEvent(orderId);//Order Row_Id
                                }
                                //string productGroupName = string.Empty;
                                //string productGroupId = string.Empty;
                                //try
                                //{
                                //    if (dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["ProductGroupId"] != null)
                                //    {
                                //        productGroupId = dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["ProductGroupId"].ToString();
                                //    }
                                //    if (dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["ProductGroupName"] != null)
                                //    {
                                //        productGroupName = dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["ProductGroupName"].ToString();
                                //    }
                                //}
                                //catch
                                //{
                                //    productGroupId = string.Empty;
                                //    productGroupName = string.Empty;
                                //}



                                //if (dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["Value"].ToString().Contains("@"))
                                //{
                                //    string dpiEmail = System.Configuration.ConfigurationSettings.AppSettings["dpiEmail"];
                                //    string dpiCCEmail = System.Configuration.ConfigurationSettings.AppSettings["dpiCCEmail"];
                                //    //eml.SendMail(mail.Subject, htmlText, dpiEmail, dpiCCEmail, false);
                                //    eventCreated = true;

                                //}
                                //else
                                //{
                                //    GoogleService.IGoogleNotificationService client = new GoogleService.GoogleNotificationServiceClient();
                                //    string orderItemDetails = string.Empty;
                                //    if (!string.IsNullOrEmpty(productGroupId))
                                //    {
                                //        orderItemDetails = "Order Item (Please do not delete or update Order Item Id) :" + order.OrderId + ", " + productGroupId + "-" + productGroupName;
                                //    }
                                //    else
                                //    {
                                //        orderItemDetails = "Order Item (Please do not delete or update Order Item Id) :" + order.OrderId;
                                //    }
                                //    string salesContactDetails = salesContacts.FirstOrDefault().SalesContactName + " on " + salesContacts.FirstOrDefault().SalesContactNumber1 + " or " + salesContacts.FirstOrDefault().SalesContactNumber2;

                                //    string orderItemDescription = string.Empty;
                                //    foreach (var item in orderItems)
                                //    {
                                //        if (string.IsNullOrEmpty(orderItemDescription))
                                //        {
                                //            orderItemDescription = item.Name;
                                //        }
                                //        else
                                //        {
                                //            orderItemDescription = orderItemDescription + Environment.NewLine + Environment.NewLine + item.Name;
                                //        }
                                //    }

                                //    string description = "SalesContact : " + salesContactDetails + " " + Environment.NewLine + Environment.NewLine
                                //                       + "Required On :" + Convert.ToDateTime(order.RequiredDate).ToShortDateString() + Environment.NewLine + Environment.NewLine
                                //                       + "Product Description :" + orderItemDescription + Environment.NewLine + Environment.NewLine
                                //        //+ "Supplier Instructions :" + dtRecordtoProcess.Rows[i]["Comments"].ToString().Replace("Supplier Instructions", "") + Environment.NewLine 
                                //                       + orderItemDetails;
                                //    string companyCode = GetCompanyCode(companyMail.ComanyName);

                                //    string title = companyCode + ":" + propertyAddress + ":" + dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["Title"].ToString() ?? order.Description;
                                //    string colorId = dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["ColorId"].ToString();

                                //    //GetStartDate(mail.SentDate, out year, out month, out day);
                                //    var startDate = sentDate;// GetStartDate(mail.SentDate);


                                //    //DateTime startDate = Convert.ToDateTime(mail.SentDate);


                                //    if (startDate.TimeOfDay.Minutes < 15 && startDate.TimeOfDay.Minutes > 0)
                                //    {
                                //        startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, 00, 00);
                                //    }
                                //    else if (startDate.TimeOfDay.Minutes < 30 && startDate.TimeOfDay.Minutes > 15)
                                //    {
                                //        startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, 30, 00);
                                //    }

                                //    else if (startDate.TimeOfDay.Minutes < 45 && startDate.TimeOfDay.Minutes > 30)
                                //    {
                                //        startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, 30, 00);
                                //    }
                                //    else if (startDate.TimeOfDay.Minutes < 45)
                                //    {
                                //        startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour + 1, 00, 00);
                                //    }
                                //    int intervalMinutes = int.Parse(dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["Value"].ToString());
                                //    //var returnEventId = client.CreateEvent(title, propertyAddress + ",Australia", salesContactDetails, startDate, startDate.AddMinutes(intervalMinutes), order.Description, order.Description, order.OrderId, order.RequiredDate.ToString(), colorId, "campaigntrack.dpi@gmail.com", description, string.Empty, false);
                                //    //if (!string.IsNullOrEmpty(returnEventId))
                                //    //{
                                //    //    eventCreated = true;
                                //    //    int eventId = GetEventId(returnEventId.Split(',')[0]);
                                //    //    int jobId = CreateJob(825, orderId, userId, propertyId, order.RequiredDate, mailId, companyId, "Not Cancelled", "");
                                //    //    CreateEvent(825, jobId, userId, calendarId, eventId, int.Parse(productGroupId), "Not Cancelled", "");
                                //    //}
                                //    //else
                                //    //{
                                //    //    throw new Exception("Google service has problem");
                                //    //}
                                //}
                                //UpdateOrderEvent(orderId);//Order Row_Id
                            }

                            //}
                        }
                    }
                }
                AddItemsToXeroInvoice(propertyAddress, xeroIds, XeroCompanyName, xeroCodeCostDictionary);
                status = true;
            }
            catch (Exception sqlError)
            {
                RollbackOrder(mailId, CompanyMailId, orderId);
                string supportEmail = System.Configuration.ConfigurationSettings.AppSettings["supportEmail"];
                string supportCCEmail = System.Configuration.ConfigurationSettings.AppSettings["supportCCEmail"];
                string strLog = sqlError.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + sqlError.Message.ToString();
                UpdateFailedOrder(emailId);
                MoveRecievedFile(fileName, true, false);
                eml.SendMail("Error in parsing file.. FileName : " + fileName + ".htm", "EmailInbox table Row_Id = " + emailId + Environment.NewLine + "Sentdate : " + sentDate + Environment.NewLine + "Error :- " + strLog, supportEmail, supportCCEmail, false);
                status = false;
                exceptionOccured = true;
            }
            if (db.State == ConnectionState.Open)
                db.Close();
            return status;
        }

        private int GetEventId(string returnEventId)
        {
            int eventId = 0;
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetEventId", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@EventId", SqlDbType.VarChar).Value = returnEventId;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt != null)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                if (row["Row_Id"] != null)
                                    eventId = int.Parse(row["Row_Id"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }
            return eventId;
        }

        private void GetCompanyIdPropertyIdUserIdCalendarId(string comanyName, string propertyAddress, string calendar, out int companyId, out int propertyId, out int userId, out int calendarId, out bool createInvoice, out string xeroCompanyName)
        {
            companyId = 0;
            propertyId = 0;
            calendarId = 0;
            userId = 0;
            createInvoice = false;
            xeroCompanyName = string.Empty;
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetCompanyIdPropertyIdUserIdCalendarId", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CompanyName", SqlDbType.VarChar).Value = comanyName.Trim();
                        cmd.Parameters.Add("@Property_Id", SqlDbType.VarChar).Value = propertyAddress.Trim();
                        cmd.Parameters.Add("@Calendar", SqlDbType.VarChar).Value = calendar;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt != null)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                if (row["CompanyId"] != null)
                                    companyId = int.Parse(row["CompanyID"].ToString());
                                if (row["PropertyId"] != null)
                                    propertyId = int.Parse(row["PropertyId"].ToString());
                                if (row["CalendarId"] != null)
                                    calendarId = int.Parse(row["CalendarId"].ToString());
                                if (row["UserId"] != null)
                                    userId = int.Parse(row["UserId"].ToString());
                                if (row["CreateInvoice"] != null)
                                {
                                    if (!string.IsNullOrEmpty(row["CreateInvoice"].ToString()))
                                        createInvoice = bool.Parse(row["CreateInvoice"].ToString());
                                }
                                if (row["XeroCompanyName"] != null)
                                    xeroCompanyName = row["XeroCompanyName"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private int CreateJob(int orgId, int orderId, int userId, int propertyId, DateTime requiredDate, int mailId, int companyId, string status, string keys, int adminId)
        {
            int jobId = 0;
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertJob", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Org_Id", SqlDbType.Int).Value = orgId;
                        cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                        cmd.Parameters.Add("@user_id", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@Property_Id", SqlDbType.Int).Value = propertyId;
                        cmd.Parameters.Add("@RequiredDate", SqlDbType.DateTime).Value = requiredDate;
                        cmd.Parameters.Add("@Mail_Id", SqlDbType.Int).Value = mailId;
                        cmd.Parameters.Add("@company_id", SqlDbType.Int).Value = companyId;
                        cmd.Parameters.Add("@Status", SqlDbType.NVarChar).Value = status;
                        cmd.Parameters.Add("@Keys", SqlDbType.NVarChar).Value = keys;
                        cmd.Parameters.Add("@AdminId", SqlDbType.Int).Value = adminId;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0][0] != null)
                            {
                                if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                                    jobId = int.Parse(dt.Rows[0][0].ToString());
                            }
                        }
                        //con.Open();
                        //cmd.ExecuteNonQuery();
                        //con.Close();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
            return jobId;
        }

        private void CreateEvent(int orgId, int jobId, int userId, int calendarId, int eventId, int productGroup, string status, string keys)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertJobEvent", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Org_Id", SqlDbType.Int).Value = orgId;
                        cmd.Parameters.Add("@Job_Id", SqlDbType.Int).Value = jobId;
                        cmd.Parameters.Add("@user_id", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@Calendar_Id", SqlDbType.Int).Value = calendarId;
                        cmd.Parameters.Add("@Event_Id", SqlDbType.Int).Value = eventId;
                        cmd.Parameters.Add("@ProductGroup_Id", SqlDbType.NVarChar).Value = productGroup;
                        cmd.Parameters.Add("@Status", SqlDbType.NVarChar).Value = status;
                        cmd.Parameters.Add("@Keys", SqlDbType.NVarChar).Value = keys;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }


        }

        private static void GetStartDate(string sentDate, out string year, out string month, out string day)
        {
            string date = sentDate;
            var split = date.Split('/');
            day = split[0];
            month = split[1];
            year = split[2].Split(' ')[0];
            string hour = split[2].Split(' ')[1].Split(':')[0];
            string minute = split[2].Split(' ')[1].Split(':')[1];
            //DateTime startDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), 00);
            //return startDate;
        }
        private static DateTime GetStartDate(string sentDate)
        {
            string date = sentDate;
            var split = date.Split('/');
            string day = split[0];
            string month = split[1];
            string year = split[2].Split(' ')[0];
            string hour = split[2].Split(' ')[1].Split(':')[0];
            string minute = split[2].Split(' ')[1].Split(':')[1];
            DateTime startDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), 00);
            return startDate;
        }

        //InsertContact

        private void InsertContact(string companyName, string contactName, string contactValue, string contactType, out int row_id)
        {
            row_id = 0;
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertContact", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CompanyName", SqlDbType.VarChar).Value = companyName;
                        cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = contactName;
                        cmd.Parameters.Add("@Value", SqlDbType.VarChar).Value = contactValue;
                        cmd.Parameters.Add("@ContactType", SqlDbType.VarChar).Value = contactType;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                row_id = int.Parse(dt.Rows[0][0].ToString());
                            }

                        }
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        private void UpdateFailedOrder(int emailInboxRowId)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateFailedOrder", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@emailInboxRowId", SqlDbType.Int).Value = emailInboxRowId;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        private string GetCompanyCode(string companyName)
        {
            string companyCode = string.Empty;
            DataTable dt = new DataTable();

            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetCompanyCode", con))
                    {
                        con.Open();
                        SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CompanyName", SqlDbType.VarChar).Value = companyName;
                        adapt.Fill(dt);
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                if (dt.Rows[0][0] != null)
                                {
                                    companyCode = dt.Rows[0][0].ToString();
                                }
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            { }
            return companyCode;
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

        private List<int> InsertOrderItems(List<OrderItem> orderItems, SqlConnection db, int orderId, out Dictionary<int, XeroProductInfo> xeroCodeCostDictionary)
        {
            List<int> xeroIds = new List<int>();
            xeroCodeCostDictionary = new Dictionary<int, XeroProductInfo>();
            foreach (OrderItem orderItem in orderItems)
            {
                var orderItemCommand = new SqlCommand("InsertOrderItem", db);//, transaction);
                orderItemCommand.CommandType = System.Data.CommandType.StoredProcedure;
                orderItemCommand.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = orderItem.Name.Trim().Replace("\n", "").Replace("&", "and");
                orderItemCommand.Parameters.Add("@ClientPrice", System.Data.SqlDbType.NVarChar).Value = orderItem.ClientPrice.Replace("$", "");
                orderItemCommand.Parameters.Add("@CostPrice", System.Data.SqlDbType.NVarChar).Value = orderItem.CostPrice.Replace("$", "");
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
                        {
                            if (int.TryParse(dt.Rows[0][0].ToString(), out no))
                            {
                                xeroIds.Add(no);
                                if (xeroCodeCostDictionary.Keys.Contains(no))
                                {
                                    xeroCodeCostDictionary[no].Count = xeroCodeCostDictionary[no].Count + 1;
                                }
                                else
                                {

                                    xeroCodeCostDictionary.Add(no, new XeroProductInfo { SalesAccountCode = dt.Rows[0][3].ToString(), SalesTaxType = dt.Rows[0][2].ToString(), Code = dt.Rows[0][1].ToString(), Cost = orderItem.ClientPrice, Name = orderItem.Name.Trim().Replace("\n", "").Replace("&", "and"), Count = 1 });
                                }

                            }
                        }
                    }
                }
            }
            return xeroIds;
        }
        public class XeroProductInfo
        {
            public string Name { get; set; }
            public string Cost { get; set; }
            public string Code { get; set; }
            public string SalesTaxType { get; set; }
            public string SalesAccountCode { get; set; }
            public int Count { get; set; }
        }
        private void InsertComapnyMail(CompanyMail companyMail, SqlConnection db, int mailId, string propertyId, string propertyAddress)
        {
            var CompanyMailCommand = new SqlCommand("InsertCompanyMail", db);//, transaction);
            CompanyMailCommand.CommandType = System.Data.CommandType.StoredProcedure;
            CompanyMailCommand.Parameters.Add("@CompanyName", System.Data.SqlDbType.NVarChar).Value = companyMail.ComanyName;
            CompanyMailCommand.Parameters.Add("@MailId", System.Data.SqlDbType.Int).Value = companyMail.MailId = mailId;
            CompanyMailCommand.Parameters.Add("@Property_Id", System.Data.SqlDbType.NVarChar).Value = propertyId.Trim();
            CompanyMailCommand.Parameters.Add("@Property", System.Data.SqlDbType.NVarChar).Value = propertyAddress.Trim();
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

        private void AddItemsToXeroInvoice(string propertyAddress, List<int> xeroCodes, string companyName, Dictionary<int, XeroProductInfo> xeroProductCostDictionary)
        {
            //bool invoiceCreated = false;

            if (invoice == null)
            {
                invoice = new Invoice(); invoice.Type = "ACCREC";
                invoice.Contact = new Contact { Name = companyName.Trim() };
                invoice.Date = DateTime.Today;
                invoice.DueDate = DateTime.Today.AddDays(30);
                invoice.Status = "DRAFT";
                invoice.TotalTax = 0;
                invoice.HasAttachments = true;
                invoice.LineAmountTypes = XeroApi.Model.LineAmountType.Inclusive;
                invoice.Reference = propertyAddress.Trim();
                LineItems lineitems = new LineItems();
                invoice.LineItems = new XeroApi.Model.LineItems();
            }
            if (createInvoice)
            {
                for (int i = 0; i < xeroCodes.Count; i++)
                {
                    LineItem lineItem = new LineItem();
                    lineItem.ItemCode = xeroProductCostDictionary[xeroCodes[i]].Code;
                    lineItem.Description = xeroProductCostDictionary[xeroCodes[i]].Name;
                    lineItem.Quantity = xeroProductCostDictionary[xeroCodes[i]].Count;
                    lineItem.UnitAmount = Convert.ToDecimal(xeroProductCostDictionary[xeroCodes[i]].Cost);
                    lineItem.TaxType = xeroProductCostDictionary[xeroCodes[i]].SalesTaxType.Replace("GST on Income", "OUTPUT");
                    lineItem.AccountCode = xeroProductCostDictionary[xeroCodes[i]].SalesAccountCode;
                    invoice.LineItems.Add(lineItem);
                }
            }
        }

        private void SaveImage(string fileName)
        {
            string jpegFileName = System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + fileName +
                                  ".jpg";
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.Load(System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + fileName + ".htm");
            System.Drawing.Image image = HtmlRenderer.HtmlRender.RenderToImage(htmlDoc.DocumentNode.OuterHtml, new System.Drawing.Size(1800, 1080));
            image.Save(jpegFileName);
        }


        public static bool ExactMatch(string input, string match)
        {
            return Regex.IsMatch(input, string.Format(@"\b{0}\b", Regex.Escape(match)));
        }



        public static string RemoveExtraSpaces(string content)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"[ ]{2,}", options);
            content = regex.Replace(content, @" ");
            return content;
        }




        #endregion

        #region Property Coordinates

        private void ProprtyCoordinates(int propertyId, string location)
        {
            try
            {
                GoogleService.IGoogleNotificationService google = new GoogleService.GoogleNotificationServiceClient();
                var coordinates = google.GetCoordinates(location);
                if (coordinates != null)
                {
                    if (coordinates.Latitude != null && coordinates.Longitude != null)
                    {
                        InsertPropertyCoordinates(propertyId, coordinates.Latitude, coordinates.Longitude);
                    }
                }
            }
            catch
            { }
        }
        private void InsertPropertyCoordinates(int propertyId, string latitude, string longitude)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("InsertProperyCoordinates", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@PropertId", SqlDbType.Int).Value = propertyId;
                            cmd.Parameters.Add("@latitude", SqlDbType.VarChar).Value = latitude;
                            cmd.Parameters.Add("@longitude", SqlDbType.VarChar).Value = longitude;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    catch (Exception ex)
                    { }
                    finally
                    {
                        SqlConnection.ClearPool(con);
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }

                    }

                }
            }
            catch (Exception ex) { throw ex; }

        }

        #endregion
    }
}
