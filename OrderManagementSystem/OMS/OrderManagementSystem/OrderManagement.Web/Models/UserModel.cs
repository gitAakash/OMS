using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class UserModel
    {
        public int Row_Id { get; set; }
        public int OrgId { get; set; }
        public int UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string Role { get; set; }
        public string Group { get; set; }
        public string InternalGroup { get; set; }
        public string Calendar { get; set; }
        public List<UserType> UserTypelist { get; set; }
        public List<Calendar> Calendarlist { get; set; }
        public IList<ColorMaster> Colorlist { get; set; }
        public string ColorCode { get; set; }
        public List<Company> Companylist { get; set; }
        //public List<ProductGroup> ProductGrouplist { get; set; }
        public List<ProductCategories> ProductGrouplist { get; set; }
     //   public string EmailExist { get; set; }
        public string AboutMe { get; set; }
        public bool IsDeleted { get; set; }
        public string MobileNumber { get; set; }
        public string Rating { get; set; }

        public string Notification_Email { get; set; }
        public bool Notification { get; set; }

    }
}