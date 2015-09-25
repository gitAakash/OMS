using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using OrderManagement.Web.Helper.Utilitties;
namespace OrderManagement.Web.Models
{
    public class NewOrderViewModel : ProjectDetailsViewModel
    {
        //public string PropertyType { get; set; }
        [Required(ErrorMessage = "Please select one Proprty")]
        public EnumHelper.EProprtyTypes ProprtyType { get; set; }
        public bool EProprtyType { get; set; }
        public List<Company> Companylist { get; set; }
        public int UserType { get; set; }  
        
    }

    public class ProjectDetailsViewModel : ProjectRequirements
    {

        public int PropTypeID { get; set; }
        public string CompanyName { get; set; }
        public int CompanyID { get; set; }
       

       
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PropertyAddrs { get; set; }
        public string OfficeContactName { get; set; }
        public string ProjectAddress { get; set; }
        


        public string AgentName { get; set; }
        public string AgentPhone { get; set; }
        public string AgentEmail { get; set; }
        public string RealViewContact { get; set; }

        public string OwnerName { get; set; }
        public string OwnerPhone { get; set; }
        public string TanantName { get; set; }
        public string TanantPhone { get; set; }

        public DateTime PropertyReady { get; set; }
        public string PropertyWebsite { get; set; }

        public int PropRowID { get; set; }
        public int OrderRowID { get; set; }
        public int OrderItemRowID { get; set; }
        public int CompanyOrderRowID { get; set; }
        public int ContactRowID { get; set; }
        public int OrderContactID { get; set; }
        public int OrderSubItemRowID { get; set; }
    
        


    }

    #region Project Requirements Section

    public class ProjectRequirements : AdditionalInstructions
    {
        public string ProjectRequirementsType { get; set; }

        #region Photography Section

        public string DayPhotographyType { get; set; }
        public string Photography { get; set; }

        public string ImageOptions { get; set; }

        public bool DuskPhotograpy { get; set; }
        public string DuskPhotograpyOptions { get; set; }

        public bool PrestigePhotoGraphy { get; set; }
        public string PrestigeDayOption { get; set; }

        public bool RentalPhotoGraphyImage { get; set; }
      

        #endregion

        #region  UAV Drone / Aerial Photography Section

        public bool AerialPhotographyUpTo3 { get; set; }
        public bool AerialPhotographyUpTo5 { get; set; }
        public bool AerialPhotographyHelicopter { get; set; }
        public bool ElevatedPhotography { get; set; }

        #endregion

        #region Floorplan and Landbox Section

        public string OnsiteMeasureDrawType { get; set; }
        public bool OnSiteColor { get; set; }
        public bool Redraw { get; set; }
        public bool RedrawColor { get; set; }
        public string Landbox { get; set; }

        #endregion

        #region Copywriting Section

        public bool OnsiteCopy { get; set; }
        public bool Offsitefrmphotography { get; set; }
        public bool ReWriteAgentOwn { get; set; }
        
        #endregion
        
        #region  Video & Image Section

        public bool PropVideo { get; set; }
        public string VideoAndMusic { get; set; }
        public bool CorpProfileVideos { get; set; }
        public bool ImageTours { get; set; }

        #endregion
    }

    #endregion

    #region   Additional Instructions Section

    public class AdditionalInstructions
    {
        public string KeyInSafeProp { get; set; }
        public string KeyInOffice { get; set; }
        public string SpecialInstruction { get; set; }
        public string DetailedBrief { get; set; }
        

    }
    #endregion



    
}