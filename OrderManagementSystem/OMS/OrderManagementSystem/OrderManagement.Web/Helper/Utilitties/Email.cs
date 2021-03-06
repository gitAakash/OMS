﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.IO;
using System.Configuration;
using RestSharp;

///<summary>
///Email namespace
///</summary>
namespace OrderManagement.Web.Helper.Utilitties
{
    ///<summary>
    ///Email class to send emails
    ///</summary>
    public static class Email
    {
        ///<summary>
        ///This method is will take different mail parameters and send the mail.
        ///</summary>
        ///<param name="strTo"> To email address</param>
        ///<param name="strCC"> CC email address</param>
        ///<param name="strFrom"> From email address</param>
        ///<param name="strSubject"> Email Subject</param>
        ///<param name="strBody">Email Body</param>
        ///<param name="IsBodyHTML"> Email format is HTML</param>
        ///<param name="hasattachment"> Email has any attachments</param>
        /// <returns>
        /// Return true/false based on mail delivery status</returns>
        public static Boolean sendemail(String strTo, String strCC, string strBody, bool IsBodyHTML, bool hasattachment, string mailSubject)
        {

            string strFrom = System.Configuration.ConfigurationManager.AppSettings["SMTPUserName"];
            //   string strSubject = System.Configuration.ConfigurationManager.AppSettings["MailSubject"];
            string strSubject = mailSubject;
            Array arrToArray;
            char[] splitter = { ';' };
            arrToArray = strTo.Split(splitter);
            Array arrCCArray;
            char[] CCsplitter = { ';' };
            arrCCArray = strCC.Split(CCsplitter);
            MailMessage mm = new MailMessage();
            //
            // mm.From = new MailAddress(Tools.GetConfigValue("SMTPUserName"), strFrom);
            //System.Configuration.ConfigurationManager.AppSettings["PFUserName"];
            mm.From = new MailAddress(strFrom, strFrom);

            mm.Subject = strSubject;
            mm.Body = strBody;
            mm.IsBodyHtml = IsBodyHTML;

            foreach (string s in arrToArray)
            { mm.To.Add(new MailAddress(s)); }

            if (strCC != "")
            {
                foreach (string s in arrCCArray)
                { mm.CC.Add(new MailAddress(s)); }
            }

            SmtpClient smtp = new SmtpClient();

            try
            {

                smtp.Host = ConfigurationManager.AppSettings["SMTPHost"];
                if (ConfigurationManager.AppSettings["SMTPSSL"] == "0")
                    smtp.EnableSsl = false;
                else
                    smtp.EnableSsl = true;
                if (ConfigurationManager.AppSettings["SMTPPortNo"] != "" && ConfigurationManager.AppSettings["SMTPPortNo"] != "0") { smtp.Port = Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPortNo"]); }
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = ConfigurationManager.AppSettings["SMTPUserName"];
                NetworkCred.Password = ConfigurationManager.AppSettings["SMTPPassword"];
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.ServicePoint.MaxIdleTime = 1;
                smtp.Send(mm);
                mm = null;
                smtp = null;
                return true;
            }

            catch (Exception ex)
            {
                // ("Error in sending email on " + DateTime.Now.ToString() + " with message : " + ex.Message);
                mm = null;
                smtp = null;
                return false;

            }

        }

        ///<summary>
        ///This method is used to send the mail using async threads
        ///</summary>
        ///<param name="subject"> Email Subject</param>
        ///<param name="Message">Email Message</param>
        ///<param name="To"> Email To</param>
        ///<param name="CC"> Email CC</param>
        ///<param name="hasattachment"> Email attachment</param>
        //public static void SendMail(String subject, String Message, string To, string CC, bool hasattachment)
        //{

        //    System.Threading.Thread threadSendMails;
        //    threadSendMails = new System.Threading.Thread(delegate() { sendemail(To, CC,ConfigurationManager.AppSettings["email_from"], subject, Message, true, hasattachment); });
        //    threadSendMails.IsBackground = true;
        //    threadSendMails.Start();
        //   // sendemail(To, CC, Tools.GetConfigValue("email_from"), subject, Message, true, hasattachment);
        //}


        public static IRestResponse SendSimpleMessage(String subject, String Message, string To)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2/domains/schedly.com.au/messages/WyJmNzhmNzY0Mzk1IiwgWyI2YTQ0ZGVkYS01ZTQ0LTQwNTUtYjhlMy03YzQ4MGJjNzkxYWYiXSwgIm1haWxndW4iLCAib2xkY29rZSJd");


            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();


            request.AddParameter("from", "dpi.campaigntrack@gmail.com");
            request.AddParameter("to", To);
            // request.AddParameter("cc", "baz@example.com");
            //  request.AddParameter("bcc", "bar@example.com");
            request.AddParameter("subject", "Event testing");
            request.AddParameter("text", "Testing some Mailgun awesomness!");
            request.AddParameter("html", "<html>HTML version of the body</html>");
            //    request.AddFile("attachment", Path.Combine("files", "test.jpg"));
            //  request.AddFile("attachment", Path.Combine("files", "test.txt"));
            request.Method = Method.POST;
            return client.Execute(request);
        }

        public static IRestResponse SendEmailFromMailGunServer(String subject, string HtmlBody, string MailTo,string MailFrom)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "schedly.com.au", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            //request.AddParameter("from", "Excited User ms <dpi.campaigntrack@gmail.com>");
            request.AddParameter("from", "Campaigntrack <" + MailFrom + ">");
           // request.AddParameter("to", "bar@example.com");
            request.AddParameter("to", MailTo);
            request.AddParameter("subject", subject);
           //request.AddParameter("text", "Testing some Mailgun awesomness!");
            request.AddParameter("html", HtmlBody);
            request.Method = Method.POST;
            return client.Execute(request);
        }

        public static IRestResponse SendEmailFromMailGunServer(String subject, string HtmlBody, List<EmailTo> MailTo, List<EmailCC> MailCC, List<EmailBCC> MailBCC, string MailFrom)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "schedly.com.au", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            //request.AddParameter("from", "Excited User ms <dpi.campaigntrack@gmail.com>");
          
            // request.AddParameter("to", "bar@example.com");

            request.AddParameter("from", "Campaigntrack <" + MailFrom + ">");

            foreach (var item in MailTo)
            {
                request.AddParameter("to", item.Email_Id);  
            }

            foreach (var item in MailCC)
            {
                request.AddParameter("cc", item.Email_Id);
            }

            foreach (var item in MailBCC)
            {
                request.AddParameter("bcc", item.Email_Id);
            }


            // request.AddParameter("cc", "baz@example.com");
            //  request.AddParameter("bcc", "bar@example.com");

          
            request.AddParameter("subject", subject);
            //request.AddParameter("text", "Testing some Mailgun awesomness!");
            request.AddParameter("html", HtmlBody);
            request.Method = Method.POST;
          //  return null;
           return client.Execute(request);
        }

        public static IRestResponse SendEmailFromMailGunServer(String subject, string HtmlBody, List<EmailTo> MailTo, List<EmailCC> MailCC, List<EmailBCC> MailBCC, string MailFrom, string FromEmailDisplayAsText)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "schedly.com.au", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            //request.AddParameter("from", "Excited User ms <dpi.campaigntrack@gmail.com>");

            // request.AddParameter("to", "bar@example.com");

            request.AddParameter("from", FromEmailDisplayAsText + " <" + MailFrom + ">");

            foreach (var item in MailTo)
            {
                request.AddParameter("to", item.Email_Id);
            }

            foreach (var item in MailCC)
            {
                request.AddParameter("cc", item.Email_Id);
            }

            foreach (var item in MailBCC)
            {
                request.AddParameter("bcc", item.Email_Id);
            }


            // request.AddParameter("cc", "baz@example.com");
            //  request.AddParameter("bcc", "bar@example.com");


            request.AddParameter("subject", subject);
            //request.AddParameter("text", "Testing some Mailgun awesomness!");
            request.AddParameter("html", HtmlBody);
            request.Method = Method.POST;
            //  return null;
            return client.Execute(request);
        }


     
    }

    public class EmailTo
    {
        public string Email_Id { get; set; }

    }

    public class EmailCC
    {
        public string Email_Id { get; set; }

    }

    public class EmailBCC
    {
        public string Email_Id { get; set; }

    }

    /// <summary>
    ///  This class is used assiging Event data and access these data in the scheduler cshtml page in loading textboxes 
    /// </summary>
    public class EventData
    {
        public string EmailContent { get; set; }
        public string EmailFrom { get; set; }
        public string EmailSubject { get; set; }
        public string EmailFromDisplay { get; set; }
        public string EmailCC { get; set; }
        public string EmailBCC { get; set; }
        public string TO{ get; set; }
    }



    public static class SaveEmail
    {

        public static int InsertEmailsToEmailInbox(string To_email, string from_email, string Cc_email, string Bcc_email, string Subject, string MessageBody, string FolderName)
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                OrderMgntEntities db = null;

                using (db = new OrderMgntEntities())
                {
                    if (currentUser.OrgId != null)
                    {
                        int orgid = currentUser.OrgId.Value;
                        var userid = currentUser.Row_Id;
                        var userType = currentUser.UserType;
                        string userTypeName = currentUser.UserType1.Name;
                        int? compamyid = null;

                        if (currentUser.UserType == 3)
                        {
                            compamyid = currentUser.CompanyId;
                        }

                        return db.InsertEmailsToEmailInbox(To_email, from_email, Cc_email, Bcc_email, Subject, MessageBody, orgid, userid, userType.ToString(), compamyid, FolderName);
                    }
                }
            }

            return 0;
        }

    }


}
