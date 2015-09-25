using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using Sync_XeroProducts_XeroCompanies;
using System.IO;
namespace CT_SyncXeroProductsAndXeroCompanies_Client
{
    public partial class CT_SyncProductsAndCompanies : Form
    {
        public CT_SyncProductsAndCompanies()
        {
            InitializeComponent();
            this.Visible = false;
            this.Hide();
        }
        Uri baseAddress = new Uri("http://localhost:52314/hello");
        BackgroundWorker BackgroundWorker;
        private void CT_SyncProductsAndCompanies_Load(object sender, EventArgs e)
        {
            BackgroundWorker = new BackgroundWorker();
            BackgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
            BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
            BackgroundWorker.RunWorkerAsync();
            //Run();
            //Application.Exit();
            //Environment.Exit(0);
        }

        void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            Application.Exit();
            Environment.Exit(0);
        }

        void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Run();
        }

        private void Run()
        {
            try
            {
                using (ServiceHost host = new ServiceHost(typeof(Sync_XeroProducts_XeroCompanies.Sync_XeroProducts_XeroCompanies), baseAddress))
                {

                    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                    smb.HttpGetEnabled = true;
                    smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                    host.Description.Behaviors.Add(smb);
                    host.Open();

                    Sync_XeroProducts_XeroCompanies.ISync_XeroProducts_XeroCompanies client = new Sync_XeroProducts_XeroCompanies.Sync_XeroProducts_XeroCompanies();
                    client.SyncXeroProductsAndCompanies();
                    host.Close();
                }
            }
            catch (Exception ex)
            {
                File.WriteAllLines("D://testLog.txt", new string[] { ex.Message });
            }
        }
    }
}
