using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;


namespace OrderManagement.Web.Helper.Utilitties
{
    public class EnumHelper
    {
        [AttributeUsage(AttributeTargets.Field)]
        public class EnumDisplayNameAttribute : Attribute
        {
            private readonly string _entityName;
            public EnumDisplayNameAttribute(string entityName)
            {
                _entityName = entityName;

            }
            public string EntityName
            {
                get { return _entityName; }
            }


        }


        public enum EProductGroup
        {
            // [EnumDisplayName("Down")]
            //  Down=1,
            //  [EnumDisplayName("Dusk")]
            //  Dusk=2
            [EnumDisplayName("Photography")]
            Photography,
            [EnumDisplayName("Video Services")]
            VideoServices,
            [EnumDisplayName("Copy Writing")]
            Copywriting,
            [EnumDisplayName("Floor Plan")]
            Floorplan,

        }

        public enum EContactType
        {
            Admin,
            Landlord,
            Sales,
            Tenant

        }



        public enum EProprtyTypes
        {
           
            Residential_Property = 0,

            Commercial_Property = 1,
           
            Commercial_PhotoGraphy = 2
        }

     

        public enum EWebOrderType
        {
            Selectable,
            Quantity
        }

        public enum EStatus
        {
            Active,
            Inactive
        }


        public enum EHosts
        {
            Youtube,
            Vimeo,
            [EnumDisplayName("File System")]
            Filesystem
        }


    }
}