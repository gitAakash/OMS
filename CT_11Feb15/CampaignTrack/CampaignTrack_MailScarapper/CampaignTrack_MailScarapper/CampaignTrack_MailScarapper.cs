using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using CampaignTrackEmailDownloader;
using System.Threading;
using System.IO;

namespace CampaignTrack_MailScarapper
{
    public partial class CampaignTrack_MailScarapper : ServiceBase
    {
        private bool _running = false;
        private bool connectionSuccess = false;
        public CampaignTrack_MailScarapper()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // ThreadWork();
            //
        }

        public void SyncProducts(string XML)
        {

        }
        public void ThreadWork()
        {
            try
            {
                // Replace the following line with your code.
                WriteLog("Start download email process");
                //  Array.ForEach(Directory.GetFiles(System.Configuration.ConfigurationSettings.AppSettings["MailPath"]), File.Delete);
                OpreateData opftp = new OpreateData();
                string strLog = opftp.ProcessData();
                if (strLog == "")
                    WriteLog("End download email process");
                else
                    WriteLog("End download email process " + strLog);
            }
            catch (ThreadAbortException abortException)
            {
                WriteLog("Download email process thread state: " + (string)abortException.ExceptionState + " error (" + abortException.Message + ")");
            }
            catch (Exception ex)
            {
                WriteLog("End download email process with error (" + ex.Message + ")");
            }
            finally
            {
                _running = false;
                connectionSuccess = true;
                WriteLog("_running is made false and connectionSuccess is made true");
            }
        }

        private void WriteLog(string strLog)
        {
            string strDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffffzz");
            string strLogDirectory = System.AppDomain.CurrentDomain.BaseDirectory + System.DateTime.Now.ToString("ddMMyyyy");
            if (!System.IO.Directory.Exists(strLogDirectory))
            {
                System.IO.Directory.CreateDirectory(strLogDirectory);
            }
            string strURL = strLogDirectory + "\\Log.txt";
            if (!File.Exists(strURL))
            {
                File.Create(strURL).Close();
            }
            System.IO.StreamWriter fw = new StreamWriter(strURL, true);
            fw.WriteLine(strDate + " : " + strLog);
            fw.Flush();
            fw.Close();
        }
    }
}
