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
using System.IO;
using System.Configuration;

using CT_SyncGoogleCalendar;
using System.Configuration;

namespace CT_SyncGoogleCalendar_Client
{
    public partial class CT_SyncGoogleCalendar_Client : Form
    {
        Uri baseAddress = new Uri("http://localhost:59484/hello");
        BackgroundWorker BackgroundWorker;
        public CT_SyncGoogleCalendar_Client()
        {
            InitializeComponent();
            this.Visible = false;
            this.Hide();
        }

        private void CT_SyncGoogleCalendar_Load(object sender, EventArgs e)
        {
            BackgroundWorker = new BackgroundWorker();
            BackgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
            BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
            BackgroundWorker.RunWorkerAsync();
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
                using (ServiceHost host = new ServiceHost(typeof(CT_SyncGoogleCalendar.CT_SyncGoogleCalendar), baseAddress))
                {

                    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                    smb.HttpGetEnabled = true;
                    smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                    host.Description.Behaviors.Add(smb);
                    host.Open();
                    //calling Google service 
                    CT_SyncGoogleCalendar.ICT_SyncGoogleCalendar client = new CT_SyncGoogleCalendar.CT_SyncGoogleCalendar();
                    //   client.SyncGoogleCalendars(825);
                    client.SyncGoogleCalendars(Convert.ToInt32(ConfigurationManager.AppSettings["Orgnization_Id"]));
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
