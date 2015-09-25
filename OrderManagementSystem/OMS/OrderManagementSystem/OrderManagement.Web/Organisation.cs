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

public partial class Organisation
{
    public Organisation()
    {
        this.EventTrackings = new HashSet<EventTracking>();
        this.OrderAttachments = new HashSet<OrderAttachment>();
    }

    public int Row_Id { get; set; }
    public string Name { get; set; }
    public Nullable<bool> IsActive { get; set; }
    public Nullable<System.Guid> OrgKey { get; set; }
    public string subdomain { get; set; }
    public string companylogo { get; set; }
    public Nullable<bool> IsDeleted { get; set; }
    public Nullable<System.TimeSpan> BH_StartTime { get; set; }
    public Nullable<System.TimeSpan> BH_EndTime { get; set; }
    public string ThemeName { get; set; }

    public virtual ICollection<EventTracking> EventTrackings { get; set; }
    public virtual ICollection<OrderAttachment> OrderAttachments { get; set; }
}
