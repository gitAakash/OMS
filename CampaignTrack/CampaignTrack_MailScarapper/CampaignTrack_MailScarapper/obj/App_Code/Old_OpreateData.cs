using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Mail;
using OpenPop;
using OpenPop.Mime;
using OpenPop.Mime.Header;
using System.Data.SqlClient;
using System.Xml;
using HtmlAgilityPack;
namespace CampaignTrackEmailDownloader
{
    public class OpreateData
    {
        private bool connectionSuccess = false;
        private bool _running = false;
        String[] SplitValueOrderItemId;
        string filename = "";
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
                   /* System.IO.File.WriteAllText(filepath + ".msg", str);
                    File.Move(filepath + ".msg", filepath + ".htm");
                    str = "";
                    str = str + " " + "From:" + "" + allMessages[i].Headers.From.ToString() + Environment.NewLine;
                    str = str + " " + "SentDate:" + "" + allMessages[i].Headers.DateSent.ToString() + Environment.NewLine;
                    str = str + " " + "Subject:" + "" + allMessages[i].Headers.Subject.ToString() + Environment.NewLine;
                    System.IO.StreamWriter objwriter;
                    objwriter = new System.IO.StreamWriter(System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + filename + ".txt");
                    objwriter.Write(str);
                    objwriter.Close();*/
                }

            }
            catch (Exception ex)
            {
                strLog = ex.Message;
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
            try
            {
                OpreateData opftp = new OpreateData();
                string strLogs = opftp.GetDataFromServer(ref connectionSuccess);//, ref index);

                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                XmlDocument xmlDoc = new XmlDocument();

                foreach (string file in Directory.EnumerateFiles(System.Configuration.ConfigurationSettings.AppSettings["MailPath"], "*.htm"))
                {
                    filename = Path.GetFileName(file);
                    XmlNode rootElement = xmlDoc.CreateElement("Root");
                    xmlDoc.AppendChild(rootElement);
                    htmlDoc.Load(file);
                    htmlDoc.OptionWriteEmptyNodes = true;
                    htmlDoc.OptionOutputAsXml = true;
                    string value = "";

                    HtmlNodeCollection nodesitemheading = htmlDoc.DocumentNode.SelectNodes(".//td[@class='content']");

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
                    SentDateElement.InnerText = SentDate;
                    rootElement.AppendChild(SentDateElement);

                    string Subject = lines[2].ToString().Replace("Subject:", "");
                    XmlElement SubjectElement = xmlDoc.CreateElement("Subject");
                    SubjectElement.InnerText = Subject;
                    rootElement.AppendChild(SubjectElement);

                    foreach (HtmlNode childNode in nodesitemheading)
                    {
                        if (childNode.ChildNodes.Count > 0)
                        {
                            foreach (HtmlNode childOfChildNode in childNode.ChildNodes)
                            {
                                if (childOfChildNode.Name == "#text")
                                {
                                    if (childOfChildNode.ChildNodes.Count > 0)
                                    {

                                        foreach (HtmlNode childOfChildofChildNode in childOfChildNode.ChildNodes)
                                        {

                                            if (childOfChildofChildNode.InnerText != "" && childOfChildofChildNode.InnerText != "&nbsp;" && childOfChildofChildNode.InnerText != "\r\n  ")
                                            {

                                                if (childOfChildofChildNode.InnerText.Trim().Contains("This is an order"))
                                                {
                                                    XmlElement Element = xmlDoc.CreateElement("PrimaryContact");
                                                    String[] SplitValue = Splits(sep(childOfChildofChildNode.InnerText, "from "), new char[] { ' ' });
                                                    string str1 = SplitValue[0] + " " + SplitValue[1];//primary contact
                                                    string str2 = SplitValue[3] + " " + SplitValue[4] + " " + SplitValue[5];//company name
                                                    str2 = str2.Replace("Relating", "");
                                                    string str3 = SplitValue[7] + " " + SplitValue[8] + " " + SplitValue[9] + " " + SplitValue[10] + SplitValue[11];//email body subject

                                                    XmlElement PrimaryContactElement = xmlDoc.CreateElement("PrimaryContact");
                                                    PrimaryContactElement.InnerText = str1;
                                                    rootElement.AppendChild(PrimaryContactElement);

                                                    XmlElement CompanyNameElement = xmlDoc.CreateElement("CompanyName");
                                                    CompanyNameElement.InnerText = str2;
                                                    rootElement.AppendChild(CompanyNameElement);

                                                    XmlElement EmailBodySubjectElement = xmlDoc.CreateElement("EmailBodySubject");
                                                    EmailBodySubjectElement.InnerText = str3;
                                                    rootElement.AppendChild(EmailBodySubjectElement);

                                                    XmlElement PropertyNameElement = xmlDoc.CreateElement("PropertyName");
                                                    str3 = str3.Replace("campaign:", "");
                                                    PropertyNameElement.InnerText = str3;
                                                    rootElement.AppendChild(PropertyNameElement);
                                                    //MessageBox.Show(str1 + ' ' + str2);
                                                }

                                                if (childOfChildofChildNode.InnerText.Trim().Contains("Property ID"))
                                                {
                                                    //MessageBox.Show(childOfChildofChildNode.InnerText.Trim().Replace("Property ID:", ""));//Property Id
                                                    XmlElement PropertyIdElement = xmlDoc.CreateElement("PropertyId");
                                                    PropertyIdElement.InnerText = childOfChildofChildNode.InnerText.Trim().Replace("Property ID:", "");
                                                    rootElement.AppendChild(PropertyIdElement);
                                                }

                                                if (childOfChildofChildNode.InnerText.Trim().Contains("OrderItem"))
                                                {
                                                    // MessageBox.Show(childOfChildofChildNode.InnerText.Trim().Replace("OrderItem:", ""));
                                                    String[] SplitValue = Splits(childOfChildofChildNode.InnerText.Trim().Replace("OrderItem:", ""), new char[] { ',' });//Order Item Id
                                                    SplitValueOrderItemId = Splits(childOfChildofChildNode.InnerText.Trim().Replace("OrderItem:", ""), new char[] { ',' });
                                                    XmlElement OrderItemIdElement = xmlDoc.CreateElement("OrderItemId");
                                                    for (int i = 0; i < SplitValue.Length; i++)
                                                    {
                                                        string str = SplitValue[i];
                                                        XmlElement IdElement = xmlDoc.CreateElement("Id");
                                                        IdElement.InnerText = str;
                                                        OrderItemIdElement.AppendChild(IdElement);
                                                    }
                                                    rootElement.AppendChild(OrderItemIdElement);
                                                }

                                                if (childOfChildofChildNode.InnerText.Trim().Contains("Sales contacts"))
                                                {
                                                    // MessageBox.Show(childOfChildofChildNode.InnerText.Trim().Replace("Sales contacts:", ""), "8");
                                                    String[] SplitValue = Splits(childOfChildofChildNode.InnerText.Trim().Replace("Sales contacts:", ""), new char[] { ',' });
                                                    for (int i = 0; i < SplitValue.Length; i++)
                                                    {
                                                        string str = (SplitValue[i].Replace(" on", ","));
                                                        String[] strValue = Splits(str, new char[] { ',' });

                                                        string str1 = strValue[0];//Sales contact name
                                                        string str2 = strValue[1].Replace("or", "");//Sales contact phone number

                                                        XmlElement SalesContactElement = xmlDoc.CreateElement("SalesContact");
                                                        SalesContactElement.InnerText = str1;
                                                        rootElement.AppendChild(SalesContactElement);

                                                        XmlElement SalesContactNumberElement = xmlDoc.CreateElement("SalesContactNumber");
                                                        SalesContactNumberElement.InnerText = str2;
                                                        rootElement.AppendChild(SalesContactNumberElement);
                                                    }


                                                    HtmlNodeCollection adminemail = htmlDoc.DocumentNode.SelectNodes(".//a[@href]");
                                                    string stradmin = adminemail[1].InnerText;//Admin email address

                                                    XmlElement AdminContactEmailElement = xmlDoc.CreateElement("AdminContactEmail");
                                                    AdminContactEmailElement.InnerText = stradmin;
                                                    rootElement.AppendChild(AdminContactEmailElement);
                                                }

                                                if (childOfChildofChildNode.InnerText.Trim().Contains("Admin contact"))
                                                {
                                                    // MessageBox.Show(childOfChildofChildNode.InnerText.Trim(), "9");
                                                    String[] SplitValue = Splits(childOfChildofChildNode.InnerText.Trim().Replace("Admin contact:", "").Replace(" on", ","), new char[] { ',' });

                                                    string str1 = SplitValue[0];//Admin contact name
                                                    string str2 = SplitValue[1].Replace("or", "");//Admin contact phone number

                                                    XmlElement AdminContactElement = xmlDoc.CreateElement("AdminContact");
                                                    AdminContactElement.InnerText = str1;
                                                    rootElement.AppendChild(AdminContactElement);

                                                    XmlElement AdminContactNumberElement = xmlDoc.CreateElement("AdminContactNumber");
                                                    AdminContactNumberElement.InnerText = str2;
                                                    rootElement.AppendChild(AdminContactNumberElement);

                                                }

                                                if (childOfChildofChildNode.InnerText.Trim().Contains("Supplier:"))
                                                {
                                                    //MessageBox.Show(childOfChildofChildNode.InnerText.Trim(), "10");
                                                    string str1 = childOfChildofChildNode.InnerText.Replace("Supplier:", "");//Product 

                                                    XmlElement SupplierElement = xmlDoc.CreateElement("Supplier");
                                                    SupplierElement.InnerText = str1;
                                                    rootElement.AppendChild(SupplierElement);
                                                }

                                                if (childOfChildofChildNode.InnerText.Trim().Contains("Description of product/service being ordered:"))
                                                {
                                                    value = "Description of product";
                                                    continue;
                                                }

                                                if (value == "Description of product")
                                                {
                                                    //MessageBox.Show(childOfChildofChildNode.InnerText.Trim(), "11");
                                                    string str = childOfChildofChildNode.InnerText.Replace("Description of product/service being ordered: ", "");//Product Description 
                                                    value = "";

                                                    XmlElement DescriptionElement = xmlDoc.CreateElement("Description");
                                                    DescriptionElement.InnerText = str;
                                                    rootElement.AppendChild(DescriptionElement);
                                                }

                                                if (childOfChildofChildNode.InnerText.Trim().Contains("Required"))
                                                {
                                                    // MessageBox.Show(childOfChildofChildNode.InnerText.Trim(), "12");
                                                    string str = childOfChildofChildNode.InnerText.Replace("Required on", "");//Required Date

                                                    XmlElement RequiredDateElement = xmlDoc.CreateElement("RequiredDate");
                                                    RequiredDateElement.InnerText = str;
                                                    rootElement.AppendChild(RequiredDateElement);
                                                }

                                                if (value == "PROPERTY")
                                                {
                                                    //    MessageBox.Show(childOfChildofChildNode.InnerText, "2");//Property Comments

                                                    XmlElement PropertyDetailsElement = xmlDoc.CreateElement("PropertyDetails");
                                                    PropertyDetailsElement.InnerText = childOfChildofChildNode.InnerText;
                                                    rootElement.AppendChild(PropertyDetailsElement);

                                                    HtmlNodeCollection propertycomments = htmlDoc.DocumentNode.SelectNodes(".//i");
                                                    string str = propertycomments[1].InnerText;//Property Comments
                                                    value = "";

                                                    XmlElement PropertyCommentsElement = xmlDoc.CreateElement("PropertyComments");
                                                    PropertyCommentsElement.InnerText = str;
                                                    rootElement.AppendChild(PropertyCommentsElement);


                                                }
                                                if (childOfChildofChildNode.InnerText.Trim() == "PROPERTY")
                                                {
                                                    value = "PROPERTY";
                                                    continue;
                                                }

                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }

                    HtmlNodeCollection Nds = htmlDoc.DocumentNode.SelectNodes(".//table[@class='MsoNormalTable']");
                    int cnt = 1;
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
                                        if (childofChildNds.InnerText.Trim() != "" && childofChildNds.InnerText.Trim() != "Client PriceCost Price" && childofChildNds.InnerText.Trim() != "campaigntrack.com.auhelpdesk")
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
                                        }

                                    }
                                    { cnt = cnt + 1; }
                                }

                            }

                        }
                    }
                    string strd = xmlDoc.InnerXml.ToString();
                    strd = strd.Replace("&amp;", "and").Replace("andamp;", " and ");
                    ExecuteProcess(strd);
                    xmlDoc.RemoveAll();
                    System.IO.File.Move(System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\" + filename, System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\Archive\\" + filename);
                    filename = filename.Replace("htm", "txt");
                    System.IO.File.Move(System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\" + filename, System.Configuration.ConfigurationSettings.AppSettings["MailPath"] + "\\Archive\\" + filename);


                    // s.A1CreateEvent("sac.nan@gmail.com", "abc", "03-12-2013", "04-12-2013", "Test", "def", "ghi");
                }

                DataSet dsRecordtoProcess = new DataSet();
                DataTable dtRecordtoProcess = new DataTable();
                dsRecordtoProcess = ReturnDataforCalenderEntry();
                dtRecordtoProcess = dsRecordtoProcess.Tables[0];
                GoogleCalSvc s = new GoogleCalSvc();
                if (dtRecordtoProcess.Rows.Count  > 0)
                {
                    for (int i = 0; i < dtRecordtoProcess.Rows.Count; i++)
                    {
                        s.userName = dtRecordtoProcess.Rows[i]["UserName"].ToString();
                        s.userPassword = dtRecordtoProcess.Rows[i]["Password"].ToString();
                        s.CreateEvent(dtRecordtoProcess.Rows[i]["Name"].ToString(), dtRecordtoProcess.Rows[i]["Startdate"].ToString(), dtRecordtoProcess.Rows[i]["EndDate"].ToString());
                        //  s.A1CreateEvent("sac.nan@gmail.com", "abc", "05-12-2013", "06-12-2013", "Test", "def", "ghi");
                    }
                }
            }
            catch (Exception ex)
            {
                strLog = ex.Message;
                WriteLog(strLog);
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

        void ExecuteProcess(string XML)
        {
            string connectionString = null;
            connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Processmail", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@X", SqlDbType.Xml).Value = XML;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

        }

        DataSet ReturnDataforCalenderEntry()
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
                    adapt.Fill(ds);


                }

            }

            return ds;
        }
    }
}
