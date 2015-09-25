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

public partial class User
{
    public User()
    {
        this.UserProductGroups = new HashSet<UserProductGroup>();
        this.Attachments = new HashSet<Attachment>();
    }

    public int Row_Id { get; set; }
    public Nullable<int> OrgId { get; set; }
    public Nullable<int> UserType { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public Nullable<bool> IsActive { get; set; }
    public Nullable<System.DateTime> Created { get; set; }
    public Nullable<System.DateTime> Updated { get; set; }
    public Nullable<int> CompanyId { get; set; }
    public string AboutMe { get; set; }
    public Nullable<bool> IsDeleted { get; set; }
    public string MobileNumber { get; set; }
    public string Rating { get; set; }
    public string Notification_Email { get; set; }
    public Nullable<bool> Notification { get; set; }

    public virtual Company Company { get; set; }
    public virtual ICollection<UserProductGroup> UserProductGroups { get; set; }
    public virtual UserType UserType1 { get; set; }
    public virtual ICollection<Attachment> Attachments { get; set; }
}
