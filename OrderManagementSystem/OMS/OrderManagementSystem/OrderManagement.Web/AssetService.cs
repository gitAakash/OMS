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

public partial class AssetService
{
    public int Row_Id { get; set; }
    public Nullable<int> AssetId { get; set; }
    public Nullable<int> AssignedtoComp { get; set; }
    public Nullable<int> AssignedtoTech { get; set; }
    public Nullable<System.DateTime> ActivityStartDate { get; set; }
    public string Type { get; set; }
    public Nullable<decimal> EstimatedEffort { get; set; }
    public Nullable<decimal> ActualEffort { get; set; }
    public string Description { get; set; }
    public string Notes { get; set; }
    public string Resolution { get; set; }
    public Nullable<System.DateTime> ReleaseDateTime { get; set; }
}
