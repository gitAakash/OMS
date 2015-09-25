using System;
using System.Collections.Generic;
using System.Text;
using CampaignTrack_MailScarapper;

namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {
            CampaignTrack_MailScarapper.CampaignTrack_MailScarapper cam = new CampaignTrack_MailScarapper.CampaignTrack_MailScarapper();
            cam.ThreadWork();
        }
    }
}
