using DevDefined.OAuth.Consumer;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using XeroApi;
namespace XeroApiService
{
    
    public class XeroApiService : IXeroApiService
    {
        private Repository repository;

        /// <summary>
        /// Initialize the repository for the Xero API
        /// </summary>
        /// <param name="orgId">organization id to fetch the required credentials</param>
        private void Initialize(string costomerKey, int orgId)
        {
            string applicationName = string.Empty;
            string consumerKey = string.Empty;
            string certificatePath = string.Empty;
            string certificatePassword = string.Empty;
            DataTable dt = GetXeroCredentials(costomerKey,orgId);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0] != null)
                    {
                        applicationName = dt.Rows[0][0].ToString();
                    }
                    if (dt.Rows[0][1] != null)
                    {
                        consumerKey = dt.Rows[0][1].ToString();
                    }
                    if (dt.Rows[0][2] != null)
                    {
                        certificatePath = dt.Rows[0][2].ToString();
                    }
                    if (dt.Rows[0][3] != null)
                    {
                        certificatePassword = dt.Rows[0][3].ToString();
                    }
                }
            }

            if (string.IsNullOrEmpty(applicationName) || string.IsNullOrEmpty(consumerKey) || string.IsNullOrEmpty(certificatePath) || string.IsNullOrEmpty(certificatePassword))
            {
                return;
            }

            IOAuthSession session = new XeroApi.OAuth.XeroApiPrivateSession(
                                    applicationName,
                                    consumerKey,
                                    new X509Certificate2(certificatePath, certificatePassword));

            repository = new Repository(session);
        }

        private DataTable GetXeroCredentials(string costomerKey, int orgId)
        {
            DataTable dt = new DataTable();
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("SelectXeroCredentials", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@customerKey", SqlDbType.NVarChar).Value = costomerKey;
                            cmd.Parameters.Add("@OrgId", SqlDbType.Int).Value = orgId;
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
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
            return dt;
        }

        /// <summary>
        /// Get the contacts from Xero API
        /// </summary>
        /// <param name="orgId">organization id for Initalization</param>
        public void SyncContacts(string costomerKey, int orgId)
        {
            Initialize(costomerKey,orgId);
            var contacts = repository.Contacts.ToList();
            XElement xmlcontacts = new XElement("ContactList",
                contacts.Select(i => new XElement("Items", new XAttribute("Name", i.Name))));
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SyncCompany", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@X", SqlDbType.Xml).Value = xmlcontacts.ToString();
                        cmd.Parameters.Add("@Org_Id", SqlDbType.Int).Value = orgId;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Get products from Xero API
        /// </summary>
        /// <param name="orgId">organization id for Initalization</param>
        public void SyncProducts(string costomerKey, int orgId)
        {
            Initialize(costomerKey, orgId);
            var items = repository.Items.ToList();
            XElement xmlitems = new XElement("XeroItems",
                items.Select(i => new XElement("Items", new XAttribute("Code", i.Code), new XAttribute("SalesUnitPrice", i.SalesDetails.UnitPrice == null ? decimal.Zero : i.SalesDetails.UnitPrice), new XAttribute("SalesAccountCode", i.SalesDetails.AccountCode == null ? string.Empty : i.SalesDetails.AccountCode), new XAttribute("SalesTaxType", i.SalesDetails.TaxType == null ? string.Empty : i.SalesDetails.TaxType),
                    i.Description)));
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SyncProducts", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@X", SqlDbType.Xml).Value = xmlitems.ToString();
                        cmd.Parameters.Add("@OrgId", SqlDbType.Int).Value = orgId;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }


        /// <summary>
        /// Get the employees from Xero API
        /// </summary>
        /// <param name="orgId">organization id for Initalization</param>
        public void SyncEmployees(string costomerKey, int orgId)
        {
            Initialize(costomerKey, orgId);
            var employees = repository.Employees.ToList();

            if (employees.Count > 0)
            {
                XElement xmlEmployees = new XElement("EmployeeList",
                   employees.Select(i => new XElement("Items", new XAttribute("EmployeeID", i.EmployeeID), new XAttribute("FirstName", i.FirstName), new XAttribute("LastName", i.LastName), new XAttribute("Status", i.Status), new XAttribute("UpdatedDateUTC", i.UpdatedDateUTC), new XAttribute("ExternalLink", i.ExternalLink))));
                try
                {
                    string connectionString = null;
                    connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("SyncEmployees", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@X", SqlDbType.Xml).Value = xmlEmployees.ToString();
                            cmd.Parameters.Add("@OrgId", SqlDbType.Int).Value = orgId;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                catch (Exception ex) { throw ex; }
            }
        }

        /// <summary>
        /// Get the users from Xero API
        /// </summary>
        /// <param name="orgId">organization id for Initalization</param>
        public void SyncUsers(string costomerKey, int orgId)
        {
            Initialize(costomerKey, orgId);
            var users = repository.Users.ToList();
            if (users.Count > 0)
            {
                XElement xmlUsers = new XElement("UserList",
                   users.Select(i => new XElement("Items", new XAttribute("FirstName", i.FirstName), new XAttribute("LastName", i.LastName), new XAttribute("OrganisationRole", i.OrganisationRole), new XAttribute("UpdatedDateUTC", i.UpdatedDateUTC), new XAttribute("UserId", i.UserID))));
                try
                {
                    string connectionString = null;
                    connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("SyncUsers", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@X", SqlDbType.Xml).Value = xmlUsers.ToString();
                            cmd.Parameters.Add("@OrgId", SqlDbType.Int).Value = orgId;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                catch (Exception ex) { throw ex; }
            }
            
        }
    }
}
