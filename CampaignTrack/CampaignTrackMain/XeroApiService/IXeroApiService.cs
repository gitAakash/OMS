using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace XeroApiService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IXeroApiService
    {

        /// <summary>
        /// Get the contacts from Xero API
        /// </summary>
        /// <param name="orgId">organization id for Initalization</param>
        [OperationContract]
        void SyncContacts(string costomerKey, int orgId);

        /// <summary>
        /// Get the products from Xero API
        /// </summary>
        /// <param name="orgId">organization id for Initalization</param>
        [OperationContract]
        void SyncProducts(string costomerKey, int orgId);

        /// <summary>
        /// Get the employees from Xero API
        /// </summary>
        /// <param name="orgId">organization id for Initalization</param>
        [OperationContract]
        void SyncEmployees(string costomerKey, int orgId);

        /// <summary>
        /// Get the users from Xero API
        /// </summary>
        /// <param name="orgId">organization id for Initalization</param>
        [OperationContract]
        void SyncUsers(string costomerKey, int orgId);
    }
}
