using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.IO;

namespace Email
{
    public class Email
    {
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
        public void SendMail(String subject, String Message, string To, string CC, bool hasattachment)
        {

            System.Threading.Thread threadSendMails;
            threadSendMails = new System.Threading.Thread(delegate() { sendemail(To, CC, Tools.GetConfigValue("email_from"), subject, Message, true, hasattachment); });
            threadSendMails.IsBackground = true;
            threadSendMails.Start();
            //sendemail(To, CC, Tools.GetConfigValue("email_from"), "Error occured parsing file ", Message, true, hasattachment);
        }


    }
}
