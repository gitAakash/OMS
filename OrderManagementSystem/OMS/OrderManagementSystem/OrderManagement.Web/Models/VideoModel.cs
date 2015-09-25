using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class ClientModel
    {
        public int Row_Id { get; set; }
        public int UpdatedBy { get; set; }
        public int OrgId { get; set; }
        public int UserType { get; set; }
        public string Name { get; set; }
        public string Main_Phone { get; set; }
        public string Main_Email { get; set; }
        public string Main_URL { get; set; }
        public bool IsDeleted { get; set; }
    }

    
    public class VideoModel
    {
        public int Row_Id { get; set; }
        public int UpdatedBy { get; set; }
        public int OrgId { get; set; }
        public int ClientId { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public int FileSize { get; set; }
        public string Files3Location { get; set; }
        public string Reference { get; set; }
        public string HostPrimary { get; set; }
        public string HostPrimaryLink { get; set; }
        public string HostSecondary { get; set; }
        public string HostSecondaryLink { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public bool IsDeleted { get; set; }
      
        public string JobIdOptions { get; set; }

        public IList<JobInfo> JobInfoList { get; set; }

        public int jobId { get; set; }
        public string jobTitle { get; set; }


    }

    public class JsonVideo
    {
        public bool IsDataAvail { get; set; }
    }


    public class JobInfo
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
    }


  


    



}