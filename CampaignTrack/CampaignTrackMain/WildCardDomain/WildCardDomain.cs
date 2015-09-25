using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildCardDomain
{
    public class WildCardDomain
    {
        public WildCardDomain(string website,string port,string newDomain,string websiteName)
        {
            AddHostHeader(website + ":" + port + ":" + newDomain, GetWebSiteId(website, websiteName));//"127.0.0.1:80:user1.domain.com",localhost,OMS
        }
        private string GetWebSiteId(string serverName, string websiteName)
        {
            string result = "-1";

            DirectoryEntry w3svc = new DirectoryEntry(string.Format("IIS://{0}/w3svc", serverName));

            foreach (DirectoryEntry site in w3svc.Children)
            {
                if (site.Properties["ServerComment"] != null)
                {
                    if (site.Properties["ServerComment"].Value != null)
                    {
                        if (string.Compare(site.Properties["ServerComment"].Value.ToString(),
                                                websiteName, true) == 0)
                        {
                            result = site.Name;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private void AddHostHeader(string hostHeader, string websiteID)
        {
            DirectoryEntry site = new DirectoryEntry("IIS://localhost/w3svc/" + websiteID);
            PropertyValueCollection serverBindings = site.Properties["ServerBindings"];

            serverBindings.Add(hostHeader);

            Object[] newList = new Object[serverBindings.Count];
            serverBindings.CopyTo(newList, 0);

            site.Properties["ServerBindings"].Value = newList;
            site.CommitChanges();
        }   
    }
}
