using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CT_DPIService
{
    [ServiceContract]
    public interface ICTDPIService
    {
        [OperationContract]
        int ProcessMessages(string sendar , string recipient , string subject , string htmlBody , string strippedHtml , string messageURL , string strippedText , string attachments , DateTime sentDate);
        [OperationContract]
        bool DeletedMail(string MailUid);
        [OperationContract]
        void TestMethod();

    }    
}

