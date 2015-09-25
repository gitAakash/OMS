using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace OrderManagement.Web.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    //public class RegisterExternalLoginModel
    //{
    //    [Required]
    //    [Display(Name = "User name")]
    //    public string UserName { get; set; }

    //    public string ExternalLoginData { get; set; }
    //}

    //public class LocalPasswordModel
    //{
    //    [Required]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Current password")]
    //    public string OldPassword { get; set; }

    //    [Required]
    //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "New password")]
    //    public string NewPassword { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirm new password")]
    //    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    //    public string ConfirmPassword { get; set; }
    //}


    public enum UserAuthenticationResult
    {
        UnSuccessful = 0,
        Successful = 1,
        Authenticated = 2,
        UnknownUsernameOrPassword = 3,
        AccountDisabled = 4,
        AccountLockedOut = 5,
        LoggedOut = 6,
        DuplicateUser = 7,
    }
    public class LoginModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter Username")]
        [Display(Name = "User id")]
        public string EmailId { get; set; }
        [Required(ErrorMessage = "Please enter Password")]
        public string Password { get; set; }
        public bool Persist { get; set; }
        public UserAuthenticationResult Result { get; set; }
        public bool LoginAttempt { get; set; }

        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public string Logolocation { get; set; }
        public string Subdomain { get; set; }
        public string ThemeName { get; set; }
        

    }

    public enum UserPersistenceResult
    {
        Successful = 1,
        Failed = 2,
        UsernameInUse = 3,
        NotRequired = 4,
        PasswordNotSupplied = 5,
        PasswordPolicyFail = 6,
        
    }

    public class UserPersistenceInfo
    {
        public int UserId { get; set; }
        public UserPersistenceResult UserPersistenceResult { get; set; }
    }

    public class UserRegistrationModel
    {
        //[Required(ErrorMessage = "First Name is required")]
        [Required]
        [Display(Name = "User name")]
        public string FirstName { get; set; }
      
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string Result { get; set; }
    }
    //public class RegisterModel
    //{
    //    [Required]
    //    [Display(Name = "User name")]
    //    public string UserName { get; set; }

    //    [Required]
    //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Password")]
    //    public string Password { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirm password")]
    //    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //    public string ConfirmPassword { get; set; }
    //}

    //public class ExternalLogin
    //{
    //    public string Provider { get; set; }
    //    public string ProviderDisplayName { get; set; }
    //    public string ProviderUserId { get; set; }
    //}


    public class ForgotPassword
    {
        [Required(ErrorMessage = "Email is Required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Email is not valid")]
        [System.Web.Mvc.Remote("CheckUserName", "User", ErrorMessage = "Already in use!")] 

        public string EmailAddress { get; set; }
        public int Msgtype { get; set; }
        public string ErrorMsg { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string UserID { get; set; }




    }
}
