using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OAuthTokenForGoogleCalendar
{
    public class OAuthToken
    {
        static CalendarService service;
        static IList<string> scopes = new List<string>();
        public OAuthToken(string clientKey, string secretKey,int orgId)
        {
            SetCalendar(clientKey, secretKey, orgId);
        }

        public void SetCalendar(string clientKey, string secretKey,int orgId)
        {
            string refreshToken = string.Empty;
            try
            {
                scopes.Add(CalendarService.Scope.Calendar);
                UserCredential credential = default(UserCredential);
                DatabaseDataStore _fdsToken;
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    StoredResponse storedResponse = new StoredResponse(refreshToken);
                    _fdsToken = new DatabaseDataStore(storedResponse, orgId);
                }
                else
                {
                    _fdsToken = new DatabaseDataStore(orgId,clientKey,secretKey);
                }
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                  new ClientSecrets
                  {
                      ClientId = clientKey,
                      ClientSecret = secretKey,
                  }, scopes,
                  "Z",
                CancellationToken.None, _fdsToken).Result;
                credential.Token.ExpiresInSeconds = 500000;
                String token = credential.Token.RefreshToken;


                credential.Token.RefreshToken = refreshToken;

                //credential.Token.ExpiresInSeconds = 500000;
                // Create the calendar service using an initializer instance
                BaseClientService.Initializer initializer = new BaseClientService.Initializer();
                initializer.HttpClientInitializer = credential;
                initializer.ApplicationName = "DPI Google Calendar";
                service = new CalendarService(initializer);
            }
            catch (Exception ex)
            {
                //string text = File.ReadAllText("C://Google.txt");
                //text = text + Environment.NewLine + ex.Message;
                //File.WriteAllText("C://Google.txt", text);
            }
        }
    }
}
