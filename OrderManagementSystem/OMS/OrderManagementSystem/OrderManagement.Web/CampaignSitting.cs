//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrderManagement.Web
{
    using System;
    using System.Collections.Generic;
    
    public partial class CampaignSitting
    {
        public int Row_Id { get; set; }
        public string AssetProxUDID { get; set; }
        public string ReceiverUDID { get; set; }
        public string Event { get; set; }
        public Nullable<System.DateTime> EventDatetime { get; set; }
        public Nullable<System.DateTime> DwellTime { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Nullable<decimal> SignalMeasure { get; set; }
        public Nullable<decimal> PowerMeasure { get; set; }
        public Nullable<decimal> TempMeasure { get; set; }
        public string ReceiverDevice { get; set; }
    }
}