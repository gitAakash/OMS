//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

public partial class Video
{
    public int Row_Id { get; set; }
    public Nullable<System.DateTime> Created { get; set; }
    public Nullable<int> CreatedBy { get; set; }
    public Nullable<System.DateTime> Updated { get; set; }
    public Nullable<System.DateTime> UpdatedBy { get; set; }
    public Nullable<int> Org_Id { get; set; }
    public Nullable<int> Client_Id { get; set; }
    public string Title { get; set; }
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public Nullable<int> FileSize { get; set; }
    public string File_s3_Location { get; set; }
    public string Reference { get; set; }
    public string Host_Primary { get; set; }
    public string Host_Primary_Link { get; set; }
    public string Host_Secondary { get; set; }
    public string Host_Secondary_Link { get; set; }
    public string Comments { get; set; }
    public string Status { get; set; }
    public Nullable<bool> IsDeleted { get; set; }
    public Nullable<int> Job_Id { get; set; }
}
