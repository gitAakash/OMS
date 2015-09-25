using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.IO;

///<summary>
///Email namespace
///</summary>
namespace Email
{
    ///<summary>
    ///Email class to send emails
    ///</summary>
    public class Email
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
        public Boolean sendemail(String strTo, String strCC, string strFrom, string strSubject, string strBody, bool IsBodyHTML, bool hasattachment)
        {

            Array arrToArray;
            char[] splitter = { ';' };
            arrToArray = strTo.Split(splitter);
            Array arrCCArray;
            char[] CCsplitter = { ';' };
            arrCCArray = strCC.Split(CCsplitter);
            MailMessage mm = new MailMessage();
            mm.From = new MailAddress(Tools.GetConfigValue("SMTPUserName"), strFrom);
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

                smtp.Host = Tools.GetConfigValue("SMTPHost");
                if (Tools.GetConfigValue("SMTPSSL") == "0")
                    smtp.EnableSsl = false;
                else
                    smtp.EnableSsl = true;
                if (Tools.GetConfigValue("SMTPPortNo") != "" && Tools.GetConfigValue("SMTPPortNo") != "0") { smtp.Port = Convert.ToInt16(Tools.GetConfigValue("SMTPPortNo")); }
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = Tools.GetConfigValue("SMTPUserName");
                NetworkCred.Password = Tools.GetConfigValue("SMTPPassword");
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
                Tools.WriteInfo("Error in sending email on " + DateTime.Now.ToString() + " with message : " + ex.Message);
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
        public void SendMail(String subject, String Message, string To, string CC, bool hasattachment)
        {

            System.Threading.Thread threadSendMails;
            threadSendMails = new System.Threading.Thread(delegate() { sendemail(To, CC, Tools.GetConfigValue("email_from"), subject, Message, true, hasattachment); });
            threadSendMails.IsBackground = true;
            threadSendMails.Start();
           // sendemail(To, CC, Tools.GetConfigValue("email_from"), subject, Message, true, hasattachment);
        }


    }
}
