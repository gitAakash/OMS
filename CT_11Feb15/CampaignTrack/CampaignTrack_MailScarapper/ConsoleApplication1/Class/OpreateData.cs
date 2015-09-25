using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net.Mime;
using OpenPop;
using OpenPop.Mime;
using OpenPop.Mime.Header;
using System.Data.SqlClient;
using System.Xml;
using HtmlAgilityPack;
using System.Globalization;
using System.Threading;
using XeroApi;
using XeroApi.Model;
using DevDefined.OAuth.Consumer;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Linq;
using HtmlRenderer.Entities;

namespace CampaignTrackEmailDownloader
{
    public class OpreateData
    {
        private bool connectionSuccess = false;
        private bool _running = false;
        String[] SplitValueOrderItemId;
        private static string filename = "";
        private static bool completed = false;
        
        private static string FullPath = System.Configuration.ConfigurationSettings.AppSettings["MailPath"];

        private static IOAuthSession XeroAPI()
        {
            IOAuthSession session = new XeroApi.OAuth.XeroApiPrivateSession(
              "DPI CampaignTrack",
              "KAMS635LAPADU8CK3HH2EC1KMBRLRJ",
              new X509Certificate2(@"D:\public_privatekey.pfx", "!zfca1999"));

            Repository repository = new Repository(session);

            var items = repository.Items.ToList();
            XElement xmlitems = new XElement("XeroItems",
                items.Select(i => new XElement("Items", new XAttribute("Code", i.Code), new XAttribute("SalesUnitPrice", i.SalesDetails.UnitPrice == null ? decimal.Zero : i.SalesDetails.UnitPrice), new XAttribute("SalesAccountCode", i.SalesDetails.AccountCode == null ? string.Empty : i.SalesDetails.AccountCode), new XAttribute("SalesTaxType", i.SalesDetails.TaxType == null ? string.Empty : i.SalesDetails.TaxType),
                    i.Description
    )));
            SyncProducts(xmlitems.ToString());

            var contacts = repository.Contacts.ToList();
            XElement xmlcontacts = new XElement("ContactList",
                contacts.Select(i => new XElement("Items", new XAttribute("Name", i.Name))));

            SyncContacts(xmlcontacts.ToString());
            return session;
        }

        private static void StartBrowser(string source)
        {
            /*
            var th = new Thread(() =>
            {
                var webBrowser = new System.Windows.Forms.WebBrowser();
                webBrowser.ScrollBarsEnabled = false;
                webBrowser.DocumentCompleted +=
                    webBrowser_DocumentCompleted;
                webBrowser.DocumentText = source;
                webBrowser.Size = new System.Drawing.Size(1024, 800);
                System.Windows.Forms.Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
             */
            System.Drawing.Image image = HtmlRenderer.HtmlRender.RenderToImage(source, new System.Drawing.Size(1800, 1080));
            image.Save(FullPath + "\\" + filename.Replace("htm", "jpg"));

        }

        static void webBrowser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {

            var webBrowser = (System.Windows.Forms.WebBrowser)sender;
            using (System.Drawing.Bitmap bitmap =
                new System.Drawing.Bitmap(webBrowser.Width, webBrowser.Height))
            {
                webBrowser
                    .DrawToBitmap(
                    bitmap,
                    new System.Drawing
                        .Rectangle(0, 0, bitmap.Width, bitmap.Height));
                bitmap.Save(FullPath + "\\" + filename.Replace("htm", "jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        public string GetDataFromServer(ref bool connectionSuccess)
        {
            string strReturn = string.Empty;
            string strLog = string.Empty;


            string PopServer = System.Configuration.ConfigurationSettings.AppSettings["pop_server"];
            int PopPort = 110;
            try
            {
                PopPort = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["pop_port"]);
            }
            catch (Exception ex) { }
            bool PopSsl = false;
            try
            {
                PopSsl = bool.Parse(System.Configuration.ConfigurationSettings.AppSettings["pop_use_ssl"]);
            }
            catch { }
            string strUserName = System.Configuration.ConfigurationSettings.AppSettings["pop_account"];
            string strPass = System.Configuration.ConfigurationSettings.AppSettings["pop_password"];

            bool PopDelete = true;
            try
            {
                PopDelete = bool.Parse(System.Configuration.ConfigurationSettings.AppSettings["pop_delete_after_download"]);
            }
            catch { }

            try
            {
                // strLog = GetDirFileListPop(PopServer, PopPort, PopSsl, strUserName, strPass, PopDelete, ref connectionSuccess);//, ref index);
                List<Message> allMessages = FetchAllMessages(PopServer, PopPort, PopSsl, strUserName, strPass);

                for (int i = 0; i < allMessages.Count; i++)
                {
                    MessageHeader emailHeader = allMessages[i].Headers;
                    List<RfcMailAddress> emailTo = emailHeader.To;
                    string str = "";
                    str = allMessages[i].FindFirstHtmlVersion().GetBodyAsText();
                    filename = System.DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
                    string filepath = System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + filename;
                    System.IO.File.WriteAllText(filepath + ".msg", str);
                    File.Move(filepath + ".msg", filepath + ".htm");
                    str = "";
                    str = str + " " + "From:" + "" + allMessages[i].Headers.From.ToString() + Environment.NewLine;
                    str = str + " " + "SentDate:" + "" + allMessages[i].Headers.DateSent.ToString() + Environment.NewLine;
                    str = str + " " + "Subject:" + "" + allMessages[i].Headers.Subject.ToString() + Environment.NewLine;
                    System.IO.StreamWriter objwriter;
                    objwriter = new System.IO.StreamWriter(System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + filename + ".txt");
                    objwriter.Write(str);
                    objwriter.Close();
                }

            }
            catch (Exception ex)
            {
                strLog = ex.Message + " " + ex.InnerException + " & " + filename;
                WriteLog(strLog);
            }

            return strLog;
        }

        private void WriteLog(string strLog)
        {
            string strDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffffzz");
            string strLogDirectory = System.AppDomain.CurrentDomain.BaseDirectory + System.DateTime.Now.ToString("ddMMyyyy");
            if (!System.IO.Directory.Exists(strLogDirectory))
            {
                System.IO.Directory.CreateDirectory(strLogDirectory);
            }
            string strURL = strLogDirectory + "\\Log.txt";
            if (!File.Exists(strURL))
            {
                File.Create(strURL).Close();
            }
            System.IO.StreamWriter fw = new StreamWriter(strURL, true);
            fw.WriteLine(strDate + " : " + strLog);
            fw.Flush();
            fw.Close();
        }

        private void WriteLog1(string strLog)
        {
            string strDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffffzz");
            string strURL = System.AppDomain.CurrentDomain.BaseDirectory + "Log.txt";
            if (!File.Exists(strURL))
            {
                File.Create(strURL).Close();
            }
            System.IO.StreamWriter fw = new StreamWriter(strURL, true);
            fw.WriteLine(strDate + " : " + strLog);
            fw.Flush();
            fw.Close();
        }



        public static List<Message> FetchAllMessages(string hostname, int port, bool useSsl, string username, string password)
        {
            // The client disconnects from the server when being disposed

            using (OpenPop.Pop3.Pop3Client client = new OpenPop.Pop3.Pop3Client())
            {
                // Connect to the server
                client.Connect(hostname, port, useSsl);

                // Authenticate ourselves towards the server
                client.Authenticate(username, password);

                // Get the number of messages in the inbox
                int messageCount = client.GetMessageCount();

                // We want to download all messages
                List<Message> allMessages = new List<Message>(messageCount);

                // Messages are numbered in the interval: [1, messageCount]
                // Ergo: message numbers are 1-based.
                // Most servers give the latest message the highest number
                for (int i = messageCount; i > 0; i--)
                {
                    allMessages.Add(client.GetMessage(i));
                    //GetDataToSave(client.GetMessage(i));
                    //client.DeleteMessage(i);
                }

                // Now return the fetched messages
                return allMessages;
            }
        }


        private string GetCommaSeperatedTo(List<RfcMailAddress> message)
        {
            string strAllEmails = "";

            foreach (RfcMailAddress tempTo in message)
            {

                if (tempTo.HasValidMailAddress)
                {

                    strAllEmails += tempTo.MailAddress + ",";

                }

            }
            return strAllEmails;

        }
        private string GetCommaSeperatedCC(List<RfcMailAddress> message)
        {
            string strAllEmails = "";

            foreach (RfcMailAddress tempCC in message)
            {

                if (tempCC.HasValidMailAddress)
                {

                    strAllEmails += tempCC.MailAddress + ",";

                }

            }
            return strAllEmails;

        }
        private string GetCommaSeperatedBCC(List<RfcMailAddress> message)
        {
            string strAllEmails = "";

            foreach (RfcMailAddress tempBCC in message)
            {

                if (tempBCC.HasValidMailAddress)
                {

                    strAllEmails += tempBCC.MailAddress + ",";

                }

            }
            return strAllEmails;

        }


        public byte[] ToByteArray(Stream stream)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            for (int totalBytesCopied = 0; totalBytesCopied < stream.Length; )
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            return buffer;
        }

        public string ProcessData()
        {
            string strLog = "";
            XmlDocument xmlDoc = new XmlDocument();
            IOAuthSession session;
            DateTime DueDate = DateTime.Now;
            string strd;
            string Subject;

            try
            {
                session = XeroAPI();
                Repository repository = new Repository(session);
                OpreateData opftp = new OpreateData();
                string strLogs = opftp.GetDataFromServer(ref connectionSuccess);//, ref index);
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                DataSet dsProductNotification = new DataSet();
                Email.Email eml = new Email.Email();
                String[] OrderId = null;
                foreach (string file in Directory.EnumerateFiles(FullPath, "*.htm"))
                {
                    filename = Path.GetFileName(file);
                    // filename = "201404161759591372889.htm";
                    htmlDoc.Load(file);
                    StartBrowser(htmlDoc.DocumentNode.OuterHtml);

                }

                // }
                foreach (string file in Directory.EnumerateFiles(FullPath, "*.htm"))
                {

                    filename = Path.GetFileName(file);
                    // filename = "201404161759591372889.htm";
                    XmlNode rootElement = xmlDoc.CreateElement("Root");
                    xmlDoc.AppendChild(rootElement);
                    htmlDoc.Load(file);
                    htmlDoc.OptionWriteEmptyNodes = true;
                    htmlDoc.OptionOutputAsXml = true;
                    string value = "";
                    HtmlNodeCollection nodesitemheading = htmlDoc.DocumentNode.SelectNodes(".//b");
                    /*This is an order notification from Robin Parker at Marshall White Brighton<br />
                    <i>Relating to 
                     campaign: 11 Lagnicourt Street, Hampton</i> */

                    String[] SplitValues = new String[1000];

                    XmlElement Element = xmlDoc.CreateElement("PrimaryContact");
                    try
                    {
                        SplitValues = Splits(sep(nodesitemheading[0].InnerText, "from "), new char[] { ' ' });
                    }
                    catch
                    {
                        File.Move(file, System.Configuration.ConfigurationSettings.AppSettings["Error"] + "\\" + filename);
                        File.Move(file.Replace("htm", "txt"), System.Configuration.ConfigurationSettings.AppSettings["Error"] + "\\" + filename.Replace("htm", "txt"));
                        xmlDoc.RemoveAll();
                        continue;
                    };

                    string cncl1 = "", cncl2 = "", cncl3 = "";

                    if (nodesitemheading[0].InnerText.Contains("The order listed below was cancelled by"))
                    {


                        IList<string> arrayAsList = (IList<string>)SplitValues;
                        int index;
                        index = arrayAsList.IndexOf("campaign:");
                        if (index == -1)
                            index = arrayAsList.IndexOf("\r\ncampaign:");
                        for (int i = index + 1; i < SplitValues.Length; i++)
                        {
                            if (SplitValues[i].Contains("of")) { break; };
                            cncl1 = cncl1 + " " + SplitValues[i];//email body subject
                            cncl1 = cncl1.Replace("Description", "").Replace("\r\n", "");
                        }

                        IList<string> arrayAsList1 = (IList<string>)SplitValues;
                        int index1 = arrayAsList.IndexOf("being");
                        for (int i = index1 + 2; i < SplitValues.Length; i++)
                        {
                            if (SplitValues[i].Contains("on")) { break; };
                            cncl2 = cncl2 + " " + SplitValues[i];//email body subject
                            cncl2 = cncl2.Replace("Required", "").Replace("\r\n", "");
                        }
                        IList<string> arrayAsList2 = (IList<string>)SplitValues;
                        int index2 = arrayAsList.IndexOf("on");
                        cncl3 = SplitValues[index2 + 1].Replace("Product", "");


                        InsertCancelledOrders(cncl1, cncl2, cncl3, filename);
                        File.Move(file, System.Configuration.ConfigurationSettings.AppSettings["Cancelled"] + "\\" + filename);
                        File.Move(file.Replace("htm", "txt"), System.Configuration.ConfigurationSettings.AppSettings["Cancelled"] + "\\" + filename.Replace("htm", "txt"));
                        xmlDoc.RemoveAll();
                        continue;
                    }

                    string str11 = "", str22 = "", str33 = "";
                    try
                    {



                        str11 = SplitValues[0] + " " + SplitValues[1];//primary contact
                        str22 = SplitValues[3] + " " + SplitValues[4] + " " + SplitValues[5];//company name
                        str22 = str22.Replace("Relating", "");

                        //if (SplitValues[7].Contains("to"))
                        //{
                        //    str33 = "";
                        //    for (int i = 8; i < SplitValues.Length; i++)
                        //    {
                        //        str33 = str33 + " " + SplitValues[i];//email body subject
                        //    }
                        //}
                        //else
                        //    str33 = SplitValues[7] + " " + SplitValues[8] + " " + SplitValues[9] + " " + SplitValues[10] + SplitValues[11];//email body subject

                        IList<string> arrayAsList = (IList<string>)SplitValues;
                        int index;
                        index = arrayAsList.IndexOf("Relating".Replace("\r\n", ""));
                        if (index == -1)
                            index = arrayAsList.IndexOf("\r\ncampaign:");//campaign:
                        if (index == -1)
                            index = arrayAsList.IndexOf("campaign:");//campaign:

                        for (int i = index + 1; i < SplitValues.Length; i++)
                        {
                            if (SplitValues[i].Contains("of")) { break; };
                            str33 = str33 + " " + SplitValues[i];//email body subject
                            str33 = str33.Replace("Description", "").Replace("\r\n", "");
                        }


                        //if (SplitValues[7].Contains("Relating"))
                        //{
                        //    str33 = "";
                        //    for (int i = 10; i < SplitValues.Length; i++)
                        //    {
                        //        str33 = str33 + " " + SplitValues[i];//email body subject
                        //    }
                        //}
                    }
                    catch
                    {
                        File.Move(file, System.Configuration.ConfigurationSettings.AppSettings["Error"] + "\\" + filename);
                        File.Move(file.Replace("htm", "txt"), System.Configuration.ConfigurationSettings.AppSettings["Error"] + "\\" + filename.Replace("htm", "txt"));
                        xmlDoc.RemoveAll();
                        continue;
                    };
                    XmlElement PrimaryContactElement = xmlDoc.CreateElement("PrimaryContact");
                    PrimaryContactElement.InnerText = str11;
                    rootElement.AppendChild(PrimaryContactElement);

                    XmlElement CompanyNameElement = xmlDoc.CreateElement("CompanyName");
                    CompanyNameElement.InnerText = str22;
                    rootElement.AppendChild(CompanyNameElement);

                    XmlElement EmailBodySubjectElement = xmlDoc.CreateElement("EmailBodySubject");
                    EmailBodySubjectElement.InnerText = nodesitemheading[0].InnerText;
                    rootElement.AppendChild(EmailBodySubjectElement);

                    XmlElement PropertyNameElement = xmlDoc.CreateElement("PropertyName");
                    str33 = str33.Replace("campaign:", "");
                    PropertyNameElement.InnerText = str33;
                    rootElement.AppendChild(PropertyNameElement);

                    XmlElement FileNameElement = xmlDoc.CreateElement("FileName");
                    FileNameElement.InnerText = Path.GetFileName(file);
                    rootElement.AppendChild(FileNameElement);
                    string strfile = file.Replace("htm", "txt");
                    string[] lines = File.ReadAllLines(strfile);

                    string From = lines[0].ToString().Replace("From:", "");
                    XmlElement FromElement = xmlDoc.CreateElement("From");
                    FromElement.InnerText = From;
                    rootElement.AppendChild(FromElement);

                    string SentDate = lines[1].ToString().Replace("SentDate:", "");
                    XmlElement SentDateElement = xmlDoc.CreateElement("SentDate");
                    //DateTime objSentDate = Convert.ToDateTime(SentDate,CultureInfo.CreateSpecificCulture("en-US"));
                    DateTime objSentDate = DateTime.Parse(SentDate, CultureInfo.GetCultureInfo("en-gb")); 
                   // SentDateElement.InnerText = objSentDate.Month.ToString().PadLeft(2, '0') + "/" + objSentDate.Day.ToString().PadLeft(2, '0') + "/" + objSentDate.Year + " " + objSentDate.ToLongTimeString();
                    rootElement.AppendChild(SentDateElement);

                    Subject = lines[2].ToString().Replace("Subject:", "");
                    XmlElement SubjectElement = xmlDoc.CreateElement("Subject");
                    SubjectElement.InnerText = Subject;
                    rootElement.AppendChild(SubjectElement);

                    HtmlNodeCollection nodesitemcontacts = htmlDoc.DocumentNode.SelectNodes(".//td[@class='content']");
                    //htmlDoc.DocumentNode.SelectNodes(".//td[@class='content']")[0].ChildNodes
                    foreach (HtmlNode childNode in nodesitemcontacts)
                    {
                        if (childNode.ChildNodes.Count > 0)
                        {
                            foreach (HtmlNode childOfChildNode in childNode.ChildNodes)
                            {
                                if (childOfChildNode.Name == "#text")
                                {
                                    if (childOfChildNode.ChildNodes.Count == 0)
                                    {

                                        if (childOfChildNode.InnerText.Trim().Contains("Property ID") || childOfChildNode.InnerText.Trim().Contains("Property"))
                                        {
                                            //MessageBox.Show(childOfChildofChildNode.InnerText.Trim().Replace("Property ID:", ""));//Property Id
                                            XmlElement PropertyIdElement = xmlDoc.CreateElement("PropertyId");

                                            PropertyIdElement.InnerText = childOfChildNode.InnerText.Trim().Replace("Property ID:", "").Replace("\r\n", "").Replace("Property ID:", "");
                                            rootElement.AppendChild(PropertyIdElement);

                                        }

                                        if (childOfChildNode.InnerText.Trim().Contains("OrderItem"))
                                        {
                                            // MessageBox.Show(childOfChildofChildNode.InnerText.Trim().Replace("OrderItem:", ""));
                                            String[] SplitValue = Splits(childOfChildNode.InnerText.Trim().Replace("OrderItem:", ""), new char[] { ',' });//Order Item Id
                                            OrderId = SplitValue;
                                            SplitValueOrderItemId = Splits(childOfChildNode.InnerText.Trim().Replace("OrderItem:", ""), new char[] { ',' });
                                            XmlElement OrderItemIdElement = xmlDoc.CreateElement("OrderItemId");
                                            for (int i = 0; i < SplitValue.Length; i++)
                                            {
                                                string str = SplitValue[i];
                                                XmlElement IdElement = xmlDoc.CreateElement("Id");
                                                IdElement.InnerText = str.Trim();
                                                OrderItemIdElement.AppendChild(IdElement);
                                            }
                                            rootElement.AppendChild(OrderItemIdElement);
                                        }

                                        if (childOfChildNode.InnerText.Trim().Contains("Sales contacts"))
                                        {
                                            // MessageBox.Show(childOfChildofChildNode.InnerText.Trim().Replace("Sales contacts:", ""), "8");
                                            String[] SplitValue = Splits(childOfChildNode.InnerText.Trim().Replace("Sales contacts:", ""), new char[] { ',' });
                                            for (int i = 0; i < SplitValue.Length; i++)
                                            {
                                                string str = (SplitValue[i].Replace(" on ", ",").Replace("\r\non", ","));
                                                String[] strValue = Splits(str, new char[] { ',' });

                                                string str1 = strValue[0];//Sales contact name
                                                string str2 = strValue[1].Replace("or", "");//Sales contact phone number


                                                XmlElement SalesContactElement = xmlDoc.CreateElement("SalesContact");
                                                SalesContactElement.InnerText = str1.Trim(); ;
                                                rootElement.AppendChild(SalesContactElement);

                                                XmlElement SalesContactNumberElement = xmlDoc.CreateElement("SalesContactNumber");
                                                SalesContactNumberElement.InnerText = str2.Trim();
                                                rootElement.AppendChild(SalesContactNumberElement);

                                            }


                                            HtmlNodeCollection adminemail = htmlDoc.DocumentNode.SelectNodes(".//a[@href]");
                                            string stradmin = adminemail[1].InnerText;//Admin email address


                                        }

                                        if (childOfChildNode.InnerText.Trim().Contains("Admin contact"))
                                        {
                                            // MessageBox.Show(childOfChildofChildNode.InnerText.Trim(), "9");
                                            // String[] SplitValue = Splits(childOfChildNode.InnerText.Trim().Replace("Admin contact:", "").Replace(" on", ","), new char[] { ',' });
                                            String[] SplitValue = Splits(childOfChildNode.InnerText.Trim().Replace("Admin contact:", "").Replace(" on", ","), "or".ToCharArray());

                                            string admindetails = childOfChildNode.InnerText.Trim().Replace("Admin contact:", "");//Admin contact name
                                            string str1;
                                            try
                                            {
                                                // bool string1Matched = Regex.IsMatch(string1, @"\bthe\b", RegexOptions.IgnoreCase); 
                                                str1 = admindetails.Substring(0, admindetails.IndexOf(" or "));
                                                str1 = str1.Substring(0, str1.IndexOf(" on "));//Admin name
                                                string str2;
                                                str2 = admindetails.Substring(0, admindetails.IndexOf(" or "));
                                                str2 = str2.Substring(str2.LastIndexOf(" on ") + 3);//Admin Phone

                                                string str3 = "";
                                                str3 = admindetails.Substring(admindetails.IndexOf(" or ") + 4);
                                                XmlElement AdminContactElement = xmlDoc.CreateElement("AdminContact");
                                                AdminContactElement.InnerText = str1.Trim();
                                                rootElement.AppendChild(AdminContactElement);

                                                XmlElement AdminContactNumberElement = xmlDoc.CreateElement("AdminContactNumber");
                                                AdminContactNumberElement.InnerText = str2.Trim();
                                                rootElement.AppendChild(AdminContactNumberElement);

                                                XmlElement AdminContactEmailElement = xmlDoc.CreateElement("AdminContactEmail");
                                                AdminContactEmailElement.InnerText = str3.Trim();
                                                rootElement.AppendChild(AdminContactEmailElement);
                                            }
                                            catch (Exception ex)
                                            {
                                                strLog = ex.StackTrace.ToString();
                                                WriteLog(strLog + " " + filename);
                                            }


                                        }

                                        if (childOfChildNode.InnerText.Trim().Contains("Supplier:"))
                                        {
                                            //MessageBox.Show(childOfChildofChildNode.InnerText.Trim(), "10");
                                            string str1 = childOfChildNode.InnerText.Replace("Supplier:", "");//Product 

                                            XmlElement SupplierElement = xmlDoc.CreateElement("Supplier");
                                            SupplierElement.InnerText = str1.Trim();
                                            rootElement.AppendChild(SupplierElement);
                                        }

                                        if (childOfChildNode.InnerText.Trim().Contains("Description of product/service being ordered:"))
                                        {
                                            value = "Description of product";
                                            continue;
                                        }

                                        if (value == "Description of product")
                                        {
                                            //MessageBox.Show(childOfChildofChildNode.InnerText.Trim(), "11");
                                            string str = childOfChildNode.InnerText.Replace("Description of product/service being ordered: ", "");//Product Description 
                                            value = "";

                                            XmlElement DescriptionElement = xmlDoc.CreateElement("Description");
                                            DescriptionElement.InnerText = str.Trim();
                                            rootElement.AppendChild(DescriptionElement);
                                        }

                                        if (childOfChildNode.InnerText.Trim().Contains("Required"))
                                        {
                                            // MessageBox.Show(childOfChildofChildNode.InnerText.Trim(), "12");                                          

                                            string RequiredDate = childOfChildNode.InnerText.Replace("Required on", "").Replace("Required \r\non", "").Replace("\r\n", "");//Required Date
                                            DateTime objRequiredDate = DateTime.Parse(RequiredDate, CultureInfo.GetCultureInfo("en-gb"));                                           
                                            XmlElement RequiredDateElement = xmlDoc.CreateElement("RequiredDate");
                                            //RequiredDateElement.InnerText = objRequiredDate.Mo.ToString().PadLeft(2, '0') + "/" + objRequiredDate.Month.ToString().PadLeft(2, '0') + "/" + objRequiredDate.Year;
                                            RequiredDateElement.InnerText = objRequiredDate.ToString();
                                            rootElement.AppendChild(RequiredDateElement);
                                            DueDate = Convert.ToDateTime(RequiredDateElement.InnerText);
                                        }

                                        if (value == "PROPERTY")
                                        {
                                            //    MessageBox.Show(childOfChildofChildNode.InnerText, "2");//Property Comments

                                            XmlElement PropertyDetailsElement = xmlDoc.CreateElement("PropertyDetails");
                                            PropertyDetailsElement.InnerText = childOfChildNode.InnerText;
                                            rootElement.AppendChild(PropertyDetailsElement);

                                            HtmlNodeCollection propertycomments = htmlDoc.DocumentNode.SelectNodes(".//i");
                                            string str = propertycomments[1].InnerText;//Property Comments
                                            value = "";

                                            XmlElement PropertyCommentsElement = xmlDoc.CreateElement("PropertyComments");
                                            PropertyCommentsElement.InnerText = str.Trim();
                                            rootElement.AppendChild(PropertyCommentsElement);

                                        }
                                        if (childOfChildNode.InnerText.Trim() == "PROPERTY")
                                        {
                                            value = "PROPERTY";
                                            continue;
                                        }

                                        //}
                                        // }

                                    }
                                }
                            }
                        }
                    }

                    HtmlNodeCollection Nds = htmlDoc.DocumentNode.SelectNodes(".//table[@cellpadding='3']");
                    int cnt = 1;

                    if (Nds == null)
                    {
                        if (Subject.Contains("cancellation"))
                        {
                            File.Move(file, System.Configuration.ConfigurationSettings.AppSettings["Cancelled"] + "\\" + filename);
                            File.Move(file.Replace("htm", "txt"), System.Configuration.ConfigurationSettings.AppSettings["Cancelled"] + "\\" + filename.Replace("htm", "txt"));

                        }
                    };

                    if (Nds != null)
                    {
                        foreach (HtmlNode childNds in Nds)
                        {

                            if (childNds.ChildNodes.Count > 0)
                            {
                                if (childNds.InnerText.Contains("This is an order"))
                                {
                                    continue;

                                }
                                else
                                {
                                    if (childNds.ChildNodes.Count > 0)
                                    {

                                        foreach (HtmlNode childofChildNds in childNds.ChildNodes)
                                        {
                                            XmlElement OrderItemDetailsElement = xmlDoc.CreateElement("OrderItemDetails");
                                            if (childofChildNds.InnerText.Trim() != "" && childofChildNds.InnerText.Contains("Client PriceCost") == false && childofChildNds.InnerText.Contains("Client \r\nPriceCost") == false && childofChildNds.InnerText.Trim() != "campaigntrack.com.auhelpdesk")
                                            {
                                                string str = childofChildNds.InnerText;
                                                str = str.Replace("$", "|");

                                                String[] SplitValue = Splits(str, new char[] { '|' });
                                                string str1 = SplitValue[0].ToString().Trim();
                                                string str2 = SplitValue[1].ToString().Trim();
                                                string str3 = SplitValue[2].ToString().Trim();
                                                //MessageBox.Show(str1 + ' ' + str2 + ' ' + str3);

                                                //for (int i = 1; i < SplitValue.Length; i++)
                                                //{
                                                //    if (i != 4 && i != 10 && i != 16) { continue; }
                                                //    if (str1 == "   " || str2 == "   " || str3 == "   ") { continue; }
                                                //    string str = SplitValue[i];
                                                //    XmlElement IdElement = xmlDoc.CreateElement("n");
                                                //    //cnt = cnt + 1;
                                                //    IdElement.InnerText = cnt.ToString() + "_" + str1 + "|" + str2 + "|" + str3;
                                                //    OrderItemDetailsElement.AppendChild(IdElement);
                                                //}
                                                if (str1 != "" || str2 != "" || str3 != "")
                                                {
                                                    XmlElement IdElement = xmlDoc.CreateElement("n");
                                                    IdElement.InnerText = cnt.ToString() + "_" + str1 + "|" + str2 + "|" + str3;
                                                    OrderItemDetailsElement.AppendChild(IdElement);
                                                    rootElement.AppendChild(OrderItemDetailsElement);
                                                }


                                                //dsProductNotification = ProductEmailNotification(str1);
                                                //if (dsProductNotification.Tables[0].Rows.Count > 0)
                                                //{

                                                //    //(String subject,String Message,string To,string CC, bool hasattachment)
                                                //    //   eml.SendMail("", "", dsProductNotification.Tables[0].Rows[0]["EmailTo"].ToString(), dsProductNotification.Tables[0].Rows[0]["EmailCC"].ToString(), false);

                                                //}
                                            }


                                        }
                                        { cnt = cnt + 1; }
                                    }

                                }

                            }
                        }
                    }
                    strd = xmlDoc.InnerXml.ToString();
                    strd = strd.Replace("&amp;", "and").Replace("andamp;", " and ");
                    strd = strd.Replace("'", "''");
                    DataSet ds = new DataSet();
                    ds = ExecuteProcess(strd,htmlDoc.DocumentNode.OuterHtml);
                    xmlDoc.RemoveAll();

                    for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                    {
                        dsProductNotification = ProductSchedule((int)ds.Tables[1].Rows[j]["Xero_Id"]);

                        if (dsProductNotification.Tables[0].Rows.Count > 0)
                        {

                            //(String subject,String Message,string To,string CC, bool hasattachment)
                            //   eml.SendMail("", "", dsProductNotification.Tables[0].Rows[0]["EmailTo"].ToString(), dsProductNotification.Tables[0].Rows[0]["EmailCC"].ToString(), false);

                            if (dsProductNotification.Tables[0].Rows[dsProductNotification.Tables[0].Rows.Count - 1]["Value"].ToString().Contains("@"))
                            {
                                // eml.SendMail(Subject, htmlDoc.DocumentNode.OuterHtml, dsProductNotification.Tables[0].Rows[0]["EmailTo"].ToString(), "", false);
                                eml.SendMail(Subject, htmlDoc.DocumentNode.OuterHtml, "campaigntrack.dpi@gmail.com", "", false);

                            }
                            else
                            {
                                DataSet dsRecordtoProcess = new DataSet();
                                DataTable dtRecordtoProcess = new DataTable();
                                dsRecordtoProcess = ReturnDataforCalenderEntry(ds.Tables[1].Rows[j]["OrderId"].ToString());
                                dtRecordtoProcess = dsRecordtoProcess.Tables[0];
                                GoogleCalSvc s = new GoogleCalSvc();
                                if (dtRecordtoProcess.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtRecordtoProcess.Rows.Count; i++)
                                    {

                                        s.CreateEvent(dtRecordtoProcess.Rows[i]["Title"].ToString(), dtRecordtoProcess.Rows[i]["Location"].ToString(), dtRecordtoProcess.Rows[i]["SalesContact"].ToString(), dtRecordtoProcess.Rows[i]["Startdate"].ToString(), dtRecordtoProcess.Rows[i]["Enddate"].ToString(), dtRecordtoProcess.Rows[i]["Comments"].ToString(), dtRecordtoProcess.Rows[i]["ProductDescription"].ToString(), dtRecordtoProcess.Rows[i]["OrderId"].ToString(), dtRecordtoProcess.Rows[i]["RequiredDate"].ToString(), dtRecordtoProcess.Rows[i]["ColorId"].ToString());
                                        //UpdateOrderEvent((int)ds.Tables[1].Rows[j]["row_id1"]);//OrderId
                                        //  s.A1CreateEvent("sac.nan@gmail.com", "abc", "05-12-2013", "06-12-2013", "Test", "def", "ghi");
                                    }
                                }
                            }
                        }
                        UpdateOrderEvent((int)ds.Tables[1].Rows[j]["row_id1"]);//OrderId
                    }



                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        if (ds.Tables[0].Rows[j]["CreateInvoice"].ToString() == "True")
                        {
                            Invoice invoice = new Invoice();
                            invoice.Type = "ACCREC";
                            invoice.Contact = new Contact { Name = ds.Tables[0].Rows[j]["XeroName"].ToString() };
                            invoice.Date = DateTime.Today;
                            invoice.DueDate = DueDate;
                            invoice.Status = "DRAFT";
                            invoice.TotalTax = 0;
                            invoice.HasAttachments = true;
                            invoice.LineAmountTypes = XeroApi.Model.LineAmountType.Inclusive;
                            invoice.Reference = ds.Tables[0].Rows[j]["Property"].ToString();
                            LineItems lineitems = new LineItems();
                            invoice.LineItems = new XeroApi.Model.LineItems();
                            ds.Tables[1].DefaultView.RowFilter = "OrderId='" + ds.Tables[0].Rows[j]["OrderId"].ToString() + "'";
                            for (int i = 0; i < ds.Tables[1].DefaultView.Count; i++)
                            {

                                LineItem lineItem = new LineItem();
                                lineItem.ItemCode = ds.Tables[1].DefaultView[i]["XeroCode"].ToString();
                                lineItem.Description = ds.Tables[1].DefaultView[i]["XeroItemDescription"].ToString();
                                lineItem.Quantity = 1;
                                lineItem.UnitAmount = Convert.ToDecimal(ds.Tables[1].DefaultView[i]["Cost1"]);
                                lineItem.TaxType = ds.Tables[1].DefaultView[i]["SalesTaxType"].ToString().Replace("GST on Income", "OUTPUT");
                                lineItem.AccountCode = ds.Tables[1].DefaultView[i]["SalesAccountCode"].ToString();
                                invoice.LineItems.Add(lineItem);
                            }

                            var inv = repository.Create<XeroApi.Model.Invoice>(invoice);
                            if (invoice.ValidationStatus == ValidationStatus.ERROR)
                            {
                                foreach (var message in invoice.ValidationErrors)
                                {
                                }
                            }
                            try
                            {
                                Guid invoiceId = inv.InvoiceID;
                                string invoiceNo = inv.InvoiceNumber;
                                // System.Threading.Thread.Sleep(5000);
                                string AnyAttachmentFilename = FullPath + "\\" + filename.Replace("htm", "jpg");
                                var SalesInvoice = repository.Invoices.FirstOrDefault(it => it.InvoiceID == invoiceId);
                                var newAttachment = repository.Attachments.Create(SalesInvoice, new FileInfo(AnyAttachmentFilename));
                                InsertProductInvoiceNo((int)ds.Tables[1].Rows[j]["Row_Id"], ds.Tables[0].Rows[j]["OrderId"].ToString(), invoiceNo);
                                ds.Tables[1].DefaultView.RowFilter = string.Empty;

                            }
                            catch (Exception ex)
                            {
                                strLog = ex.StackTrace.ToString();
                                WriteLog(strLog + " " + filename);
                                File.Move(System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\" + filename, System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\Error\\" + filename);
                            };
                        }

                    }
                    System.Threading.Thread.Sleep(30000);
                    System.IO.File.Delete(System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\" + filename);
                    filename = filename.Replace("htm", "txt");
                    System.IO.File.Delete(System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\" + filename);
                    //filename = filename.Replace("txt", "jpg");
                    //System.IO.File.Move(System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\" + filename, System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\Archive\\" + filename);
                }

            }
            catch (Exception ex)
            {
                strLog = ex.StackTrace.ToString();
                WriteLog(strLog + " " + filename);
                File.Move(System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\" + filename, System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\Error\\" + filename);
            }

            return strLog;
        }
        public String[] Splits(string values, char[] character)
        {


            string[] strSplitArr = values.Split(character);
            return strSplitArr;

        }

        public static string sep(string s, string word)
        {
            int l = s.IndexOf(word) + word.Length;
            if (l > 0)
            {
                return s.Substring(l, (s.Length - l));
            }
            return "";

        }




        public static void UpdateOrderEvent(int OrderId)
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


        public static void InsertProductInvoiceNo(int ProductId, string OrderNo, string InvoiceNo)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertProductInvoiceNo", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = ProductId;
                        cmd.Parameters.Add("@OrderNo", SqlDbType.VarChar).Value = OrderNo;
                        cmd.Parameters.Add("@InvoiceNo", SqlDbType.VarChar).Value = InvoiceNo;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        public static void SyncContacts(string XML)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SyncCompany", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@X", SqlDbType.Xml).Value = XML;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        public static void SyncProducts(string XML)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SyncProducts", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@X", SqlDbType.Xml).Value = XML;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        DataSet ProductSchedule(int XeroId)
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


        DataSet ExecuteProcess(string XML,string Html)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                DataSet ds = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Processmail", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@X", SqlDbType.Xml).Value = XML;
                        cmd.Parameters.Add("@Html", SqlDbType.VarChar).Value = Html;

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

        void InsertCancelledOrders(string Service, string Property, string RequiredDate, string FileName)
        {
            string connectionString = null;
            connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertCancelledOrder", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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

        DataSet ReturnDataforCalenderEntry(string OrderId)
        {
            string connectionString = null;
            connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
            DataSet ds = new DataSet();
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

            return ds;
        }
    }
}
