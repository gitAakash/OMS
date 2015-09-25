using Google.Apis.Json;
using Google.Apis.Util.Store;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NotificationService
{
    public class DatabaseDataStore : IDataStore
    {
        public StoredResponse _storedResponse { get; set; }
        public bool IsRefeshTokenNull { get; set; }
        public int OrgId { get; set; }

        public DatabaseDataStore(StoredResponse pResponse,int orgId)
        {
            _storedResponse = pResponse;
            orgId = OrgId;
        }

        public DatabaseDataStore(int orgId)
        {
            _storedResponse = new StoredResponse();
            IsRefeshTokenNull = true;
        }

        public System.Threading.Tasks.Task ClearAsync()
        {
            this._storedResponse = new StoredResponse();
            return TaskEx.Delay(0);
        }

        public System.Threading.Tasks.Task DeleteAsync<T>(string key)
        {
            this._storedResponse = new StoredResponse();
            return TaskEx.Delay(0);
        }

        public System.Threading.Tasks.Task<T> GetAsync<T>(string key)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            try
            {
                string JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(this._storedResponse);
                tcs.SetResult(Google.Apis.Json.NewtonsoftJsonSerializer.Instance.Deserialize<T>(JsonData));
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return tcs.Task;            
        }

        public System.Threading.Tasks.Task StoreAsync<T>(string key, T value)
        {
            var serialized = NewtonsoftJsonSerializer.Instance.Serialize(value);
            JObject jObject = JObject.Parse(serialized);
            // storing access token
            var val = jObject.SelectToken("access_token");
            if (val != null)
            {
                this._storedResponse.access_token = (string)val;
            }
            // storing token type
            val = jObject.SelectToken("token_type");
            if (val != null)
            {
                this._storedResponse.token_type = (string)val;
            }
            val = jObject.SelectToken("expires_in");
            if (val != null)
            {
                this._storedResponse.expires_in = (long?)val;
            }
            val = jObject.SelectToken("refresh_token");
            if (val != null)
            {
                this._storedResponse.refresh_token = (string)val;
                if (IsRefeshTokenNull)
                {
 
                }
            }
            val = jObject.SelectToken("Issued");
            if (val != null)
            {
                this._storedResponse.Issued = (string)val;
            }
            return TaskEx.Delay(0);            
        }

        public void UpdateRefreshToken(int orgId, string refreshToken)// string clientKey, string clientSecretKey,string refreshToken)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("UpdateCalendarRefreshToken", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@refreshToken", SqlDbType.VarChar).Value = refreshToken;
                            cmd.Parameters.Add("@orgId", SqlDbType.Int).Value = orgId;
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
    }

    public class StoredResponse 
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long? expires_in { get; set; }
        public string refresh_token { get; set; }
        public string Issued { get; set; }

        public StoredResponse(string pRefreshToken)
        {
            this.refresh_token = pRefreshToken;
            this.Issued = DateTime.MinValue.ToString();
        }
        public StoredResponse()
        {
            this.Issued = DateTime.MinValue.ToString();
        }
    }
}