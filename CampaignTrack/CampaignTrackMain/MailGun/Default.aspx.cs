using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MailGun
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            ServiceReference2.ICTDPIService ct1 = new ServiceReference2.CTDPIServiceClient();
            ct1.TestMethod();


            var context = HttpContext.Current;
            string reqheaders = string.Empty;
            var actualRequest = context.Request;

            //string strippedText1 = File.ReadAllText(@"C:\Users\swapnil.gade\Desktop\testGunMail.txt");
            //string [] x = strippedText1.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                string sendar =  actualRequest["from"];
                string recipient = actualRequest["recipient"];
                string subject = actualRequest["subject"];
                string htmlBody = actualRequest["body-html"];
                string strippedText = actualRequest["stripped-text"];
                string messageURL = actualRequest["message-url"];
                string strippedHtml = actualRequest["stripped-html"];
                string attachments = actualRequest["attachments"]!=null?actualRequest["attachments"]:string.Empty;
                DateTime timeStamp = new DateTime(1970, 1, 1).AddSeconds(double.Parse(actualRequest["timestamp"]));

                reqheaders = Environment.NewLine + "Sender : " + sendar + Environment.NewLine + "Recipient : " + recipient + Environment.NewLine + "Subject : " + subject +
                    Environment.NewLine + "body-html : " + htmlBody + Environment.NewLine + "Stripped Text : " + strippedHtml + Environment.NewLine + "Message URL : " + messageURL
                    + Environment.NewLine + "stripped-html : " + strippedText + Environment.NewLine + "attachments : " + attachments + Environment.NewLine + timeStamp;

                ServiceReference1.ICTDPIService ct = new ServiceReference1.CTDPIServiceClient();
                ct.ProcessMessages(sendar.ToString(), recipient.ToString(), subject.ToString(), htmlBody.ToString(), strippedHtml.ToString(), messageURL.ToString(), strippedText.ToString(), attachments.ToString(), timeStamp);
            }
            catch (Exception ex)
            {
                reqheaders = reqheaders + ex.Message;
            }


            File.WriteAllText("C://GunMail.txt", reqheaders);
            context.Response.StatusCode = 200;
            context.Response.End();
            return;
            

            

        }
    }
}