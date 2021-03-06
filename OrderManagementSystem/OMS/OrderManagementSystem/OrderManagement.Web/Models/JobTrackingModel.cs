﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderManagement.Web.Helper.Utilitties;

namespace OrderManagement.Web.Models
{
    //public class JobTrackingModel1
    //{
    //    public int Row_Id { get; set; }
    //    public int CalendarId { get; set; }
    //    public string OrderId { get; set; }
    //    public string Title { get; set; }
    //    public DateTime MovedOn { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public DateTime EndDate { get; set; }
    //    public string Status { get; set; }
    //    public DateTime Created { get; set; }
    //    public int Org_Id { get; set; }
    //}


    //public class JobTrackingViewModel2
    //{
    //    public string Property { get; set; }
    //    public string FloorPlan { get; set; }
    //    public string Video { get; set; }
    //    public string Copy { get; set; }
    //    public bool FloorStatus { get; set; }
    //    public bool VideoStatus { get; set; }
    //    public bool CopyStatus { get; set; }
    //}

    public class UploadFile
    {
        public UploadFile()
        {
            ContentType = "application/octet-stream";
        }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
    }


    public class JobTrackingModel
    {
        public int JobId { get; set; }
        public string Active { get; set; }
        public string JobTitle { get; set; }
        public string Completion { get; set; }
        public DateTime RequireDate { get; set; }
        public List<TrackJobProductGroup> ProductGroup { get; set; }
        public int OrgId { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public string CreatedByName { get; set; }
        public string Resource { get; set; }
        public string EventTitle { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string StatusColour { get; set; }
        public string JobEventStatus { get; set; }
        public List<string>Email_Notify { get; set; }
        public string Email_Notification { get; set; }
        public string Package { get; set; }
        public int ProductId { get; set; }
        public int? Product_Qty { get; set; }

    }


    public class TrackJobProductGroup
    {
        public int Row_Id { get; set; }
        public string ProductGroupName { get; set; }
        public string CssClass { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Created { get; set; }
        public string Status { get; set; }
        public string JobEventStatusColour { get; set; }
        public int Org_Id { get; set; }
    }


    public class JobCopyModel
    {
        public int RowId { get; set; }
        public int Jobid { get; set; }
        public List<SelectJobCopyType> TypeList { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string TitleWordCount { get; set; }

        [AllowHtml]
        public string JobBody { get; set; }
        public string BodyWordCount { get; set; }
        public string Status { get; set; }
    }
    public class JobImage
    {
        public string ImageURL { get; set; }
        public int Job_Id { get; set; }
        public int Group_Id { get; set; }
        public int Image_Id { get; set; }
        public int TotalImageCount { get; set; }
        public int SelectedImageCount { get; set; }
    }
}