using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CampaignTrack_MailScarapper;
using XeroApi;
using DevDefined.OAuth.Consumer;
using System.Security.Cryptography.X509Certificates;
using XeroApi.Model;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            CampaignTrack_MailScarapper.CampaignTrack_MailScarapper cam = new CampaignTrack_MailScarapper.CampaignTrack_MailScarapper();
            cam.ThreadWork();
            Application.Exit();
            Environment.Exit(0);
            Process[] ps = Process.GetProcessesByName("WindowsFormsApplication1");

            foreach (Process p in ps)
                p.Kill();
            /*
            CampaignTrack_MailScarapper.CampaignTrack_MailScarapper cam = new CampaignTrack_MailScarapper.CampaignTrack_MailScarapper();
            cam.ThreadWork();
                
                IOAuthSession session = new XeroApi.OAuth.XeroApiPrivateSession(
                "DPI CampaignTrack",
                "KAMS635LAPADU8CK3HH2EC1KMBRLRJ",
                new X509Certificate2(@"D:\public_privatekey.pfx", "!zfca1999"));

                Repository repository = new Repository(session);
            
                Console.WriteLine("You're connected to " + repository.Organisation.Name);
                Console.ReadLine();
                Invoice inv = new Invoice();

                var cust = repository.Contacts.Where(c => c.ContactStatus == "ACTIVE" && c.IsCustomer == true);
                var contactList = cust.ToList();

                var custs = repository.Items.ToList();
           
                var invoice = repository.Create(
    new Invoice
    {
        Type = "ACCREC",
        Contact = new Contact { Name = "Gunnfreight Pyt Ltd" },
        Date = DateTime.Today,
        DueDate = DateTime.Today.AddDays(6),
        Status = "DRAFT",
        TotalTax=0,
        LineAmountTypes=XeroApi.Model.LineAmountType.Inclusive,
        LineItems = new LineItems
    {
    new LineItem
    {
   
    Description = "Salary for MARCH 2014",
    Quantity = 1,
    UnitAmount =2500,
    TaxType="OUTPUT",
    AccountCode="200"
    }
    }
    });

                if (invoice.ValidationStatus == ValidationStatus.ERROR)
                {
                    foreach (var message in invoice.ValidationErrors)
                    {
                    }
                }
                //write the details for the invoice in the label on the home page
                string str = String.Format("Invoice {0} was raised against {1} on {2} for {3}{4}",
                invoice.InvoiceNumber,
                invoice.Contact.Name, invoice.Date, invoice.Total,
                invoice.CurrencyCode);
         
              */
        }





    }



}


