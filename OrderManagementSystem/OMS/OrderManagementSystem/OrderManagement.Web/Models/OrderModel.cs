using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class NewOrderModel
    {
        public List<Company> Companylist { get; set; }
        public int Companyid { get; set; }
        public int UserType { get; set; }  

        public string Phone { get; set; }
        public string Email { get; set; }
        public string PropertyAddress { get; set; }
        public string OfficeContactName { get; set; }
        public string ProjectAddress { get; set; }

        public List<RealViewContact> RealViewContactlist { get; set; }

        public string AgentName { get; set; }
        public string AgentPhone { get; set; }
        public string AgentEmail { get; set; }
        public string RealViewContact { get; set; }

        public string OwnerName { get; set; }
        public string OwnerPhone { get; set; }
        public string TenantName { get; set; }
        public string TenantPhone { get; set; }

        public DateTime PropertyReady { get; set; }
        public string PropertyWebsite { get; set; }

        public List<NewSelectOrderableProducts> ProductGoruplist { get; set; }
        public int ProductGorupId { get; set; }

        public Photography Photography { get; set; }

        public string KeyInSafeProp { get; set; }
        public string KeyInOffice { get; set; }
        public string SpecialInstruction { get; set; }
        public string DetailedBrief { get; set; }


        public List<ContactTypes> contactTypes { get; set; }
        

        public List<OrderableProductsDayPhotographyOpt2> orderableProductsDayPhoto2 { get; set; }
        public List<OrderableProductsDayPhotographyOpt5> orderableProductsDayPhoto5 { get; set; }
        public List<OrderableProductsDayPhotographyOpt8> orderableProductsDayPhoto8 { get; set; }

        public List<OrderableProductsDuskPhotography> orderableProductsduskPhoto { get; set; }
        public List<OrderableProductsDuskPhotographyOpt8> orderableProductsDuskPhoto8 { get; set; }

        public List<OrderableProductsPrestigeDayPhoto12> orderableProductsPrestigeDayPhoto12 { get; set; }
        public List<OrderableProductsPrestigeDuskPhoto12> orderableProductsPrestigeDuskPhoto12 { get; set; }

        public List<OrderableProductsRentalPhoto5> orderableProductsRentalPhoto5 { get; set; }

        public List<OrderableProductsRentalPhoto10> orderableProductsRentalPhoto10 { get; set; }

        public List<OrderableProductsUAVDrone> orderableProductsUAVDrone { get; set; }

        public List<OrderableProductsCopyWriting> orderableProductsCopyWriting { get; set; }

        public List<OrderableProductsVideoAndImage> orderableProductsVideoAndImage { get; set; }

        public List<LongitudeAndLatitude> longitudeAndLatitude { get; set; }

        public List<Keys> keys { get; set; }




        public List<ProjectPrimaryProductGroup> projectPrimaryProductGroup { get; set; }

        


        //--------------------------------------------
        public List<PrimaryProductGrp> primaryProductGrp { get; set; }
        public List<SecondaryProductGroup> secondaryProductGroup { get; set; }


      public IList<PrimaryGroup> primaryGroup { get; set; }
    }


    public class NewSelectOrderableProducts :SelectOrderableProducts
    {
     public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string CompanyName { get; set; }
        public string WebName { get; set; }
        public string WebDescription { get; set; }
        public string WebType { get; set; }
        public string WebOptions { get; set; }
        public Nullable<int> WebOptionMin { get; set; }
        public Nullable<int> WebOptionMax { get; set; }
        public Nullable<decimal> SalesUnitPrice { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string datarelativediv{ get; set; }

    }






    public class ContactTypes
    {
        public int ContactTypeId { get; set; }
        public string ContactType { get; set; }
        public string Relativediv { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string cssClass { get; set; }

    }


    public class ProjectPrimaryProductGroup
    {

        public string PrimaryProductGroup { get; set; }
      

    }


    public class LongitudeAndLatitude
    {
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
    public class OrderableProductsDayPhotographyOpt2 
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string WebName { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string cssClass { get; set; }
    }

    public class OrderableProductsDayPhotographyOpt5
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string WebName { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string cssClass { get; set; }
    }

    public class OrderableProductsDayPhotographyOpt8
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string WebName { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string cssClass { get; set; }
    }

    public class OrderableProductsDuskPhotographyOpt8
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string WebName { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string cssClass { get; set; }
    }

    public class OrderableProductsDuskPhotography
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string WebName { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string cssClass { get; set; }
    }

    public class OrderableProductsPrestigeDayPhoto12
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string WebName { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string cssClass { get; set; }
    }

    public class OrderableProductsPrestigeDuskPhoto12
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string WebName { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string cssClass { get; set; }
    }

    public class OrderableProductsRentalPhoto5
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string WebName { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string cssClass { get; set; }
    }
    
    public class OrderableProductsRentalPhoto10
     {
         public int Row_Id { get; set; }
         public string PrimaryProductGroup { get; set; }
         public string WebName { get; set; }
         public bool CheckedStatus { get; set; }
         public string ChkboxName { get; set; }
         public string cssClass { get; set; }
     }

    public class OrderableProductsUAVDrone
     {
         public int Row_Id { get; set; }
         public string PrimaryProductGroup { get; set; }
         public string WebName { get; set; }
         public bool CheckedStatus { get; set; }
         public string ChkboxName { get; set; }
         public string cssClass { get; set; }
         public string LabelText { get; set; }
     }

    public class OrderableProductsCopyWriting
     {
         public int Row_Id { get; set; }
         public string PrimaryProductGroup { get; set; }
         public string WebName { get; set; }
         public bool CheckedStatus { get; set; }
         public string ChkboxName { get; set; }
         public string cssClass { get; set; }
         public string LabelText { get; set; }
     }

    public class OrderableProductsVideoAndImage
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string WebName { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string cssClass { get; set; }
        public string LabelText { get; set; }
    }

    public class Keys
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string WebName { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string cssClass { get; set; }
        public string LabelText { get; set; }
    }


    public class PrimaryProductGrp : SelectOrderableProducts
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string CompanyName { get; set; }
        public string WebName { get; set; }
        public string WebDescription { get; set; }
        public string WebType { get; set; }
        public string WebOptions { get; set; }
        public Nullable<int> WebOptionMin { get; set; }
        public Nullable<int> WebOptionMax { get; set; }
        public Nullable<decimal> SalesUnitPrice { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string datarelativediv { get; set; }

    }

    public class SecondaryProductGroup : SelectOrderableProducts
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string CompanyName { get; set; }
        public string WebName { get; set; }
        public string WebDescription { get; set; }
        public string WebType { get; set; }
        public string WebOptions { get; set; }
        public Nullable<int> WebOptionMin { get; set; }
        public Nullable<int> WebOptionMax { get; set; }
        public Nullable<decimal> SalesUnitPrice { get; set; }
        public bool CheckedStatus { get; set; }
        public string ChkboxName { get; set; }
        public string datarelativediv { get; set; }

    }


    public class PrimaryGroup : CommonProperty
    {
        public string PrimaryProductGroupName { get; set; }
        public IList<SecondaryGroup> secondaryProductGroup { get; set; }
        public PrimaryGroup()
        {
            secondaryProductGroup = new List<SecondaryGroup>();
        }
    }

    public class SecondaryGroup : CommonProperty
    {
        public string SecondaryProductGroup { get; set; }
        public IList<ThirdGroup> thirdProductGroup { get; set; }
        public SecondaryGroup()
        {
            thirdProductGroup = new List<ThirdGroup>();

        }
    }

    public class ThirdGroup : CommonProperty
    {
        public string ThirdProductGroup { get; set; }

        public IList<WebOption> webOption { get; set; }
        public ThirdGroup()
        {
            webOption = new List<WebOption>();
        }
    }

    public class WebOption : CommonProperty
    {
        public string Option { get; set; }

    }


public class CommonProperty
{
    public int Row_Id { get; set; }
    public bool ischecked { get; set; }
    public string CompanyName { get; set; }
    public string WebName { get; set; }
    public string WebDescription { get; set; }
    public string WebType { get; set; }
    public string WebOptions { get; set; }
    public int WebOptionMin { get; set; }
    public int WebOptionMax { get; set; }
    public decimal SalesUnitPrice { get; set; }
  
}





















  public class orderableProducts 
    {
        public List<OrderableProductsDayPhotographyOpt2> ProductsDayPhotographyOpt2 { get; set; }
    }


    public class RealViewContact
    {
        public int RealViewContactId { get; set; }
        public string RealViewContactName { get; set; }
     
    }

    public class Photography
    {
        #region Photography Section


        public string PhotographySec { get; set; }
        public bool DayPhotographyType { get; set; }
        public string ImageOptions { get; set; }

        public bool DuskPhotograpy { get; set; }
        public string DuskPhotograpyOptions { get; set; }

        public bool PrestigePhotoGraphy { get; set; }
        public string PrestigeDayOption { get; set; }

        public bool RentalPhotoGraphy { get; set; }
        public string RentalPhotoGraphyImage { get; set; }


        #endregion

    }



    
}