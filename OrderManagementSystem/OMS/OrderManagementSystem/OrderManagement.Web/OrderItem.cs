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

public partial class OrderItem
{
    public int Row_Id { get; set; }
    public Nullable<int> Order_Id { get; set; }
    public Nullable<int> Xero_Id { get; set; }
    public string Name { get; set; }
    public string ClientPrice { get; set; }
    public string CostPrice { get; set; }
    public string Comments { get; set; }
    public Nullable<System.DateTime> Created { get; set; }
    public string Options { get; set; }

    public virtual Order Order { get; set; }
}
