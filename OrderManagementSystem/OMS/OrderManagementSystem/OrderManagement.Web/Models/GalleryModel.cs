using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class GalleryModel
    {
        public IList<SelectJobAttachmentFolders> GalleryFolders { get; set; }
        public IList<TagInfo> TagList { get; set; }
        public IList<SelectJobAttachments> SelectJobAttachments { get; set; }
        public IList<GetJobAttachments> JobAttachmentlist { get; set; }

        public int NumberOfPages { get; set; }
        public int CurrentPage { get; set; }

        //public IList<FolderRecordsCounts> FolderRecordsCountList { get; set; }
        public IList<GetFoldersAttachmentCount> FolderRecordsCountList { get; set; }
        public IList<GetUserCommentsbyJobId> GetUserCommentsbyJobIdList { get; set; }
        
      //  IList<ProductCategories> GetProductgroupBySp(int? orgId, int? parentId);
    }



    public class JobAttachmentModel
    {
        public List<SelectJobAttachments> JobAttachmentlst { get; set; }
        public int NumberOfPages { get; set; }
        public int CurrentPage { get; set; }
    }

    public class TagInfo
    {
        public int Row_Id { get; set; }
        public string TagName { get; set; }
        public string Folder { get; set; }
        public string Chkchecked { get; set; }
    }

    public class TagListByRowId
    {
        public int Row_Id { get; set; }
        public string TagName { get; set; }
        public string Folder { get; set; }
        public string Chkchecked { get; set; }
        //IList<GetJobAttachments> JobAttachmentlist { get; set; }
    }

    [Serializable]
    public class GalleryInputModel 
    {
        public int Row_Id { get; set; }
        public string TagSelected { get; set; }
        public string FileName { get; set; }
        public int Job_id { get; set; }
        public string Message { get; set; }
    }


    public class JsonInfo
    {
        public string NewFileName { get; set; }
        public string OldFileName { get; set; }
        public string FileNotSaveReason { get; set; }
        public bool IsTagUpdated { get; set; }
        public bool IsFileNameUpdated { get; set; }
        public string Tags { get; set; }
        public bool Save { get; set; }
    }

    public class ImgCountInfo
    {
        public int SelectedImgCount { get; set; }
        public int TotalImgCount { get; set; }

    }

    public class FolderRecordsCounts
    {
        public int Count { get; set; }
        public string FolderName { get; set; }
    }


}