using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using OrderManagement.Web.Helper.Utilitties;
using WebMatrix.WebData;
using OrderManagement.Web.Filters;
using OrderManagement.Web.Models;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Models.ServiceRepository;
using System.Net.Mail;
using System.Text;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using OrderManagement.Web;
using System.Web.Caching;

namespace OrderManagement.Web.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAccountRepository _repository;
        private readonly IUserRepository _userRepository;

        public AccountController()
        {
            var userrapo = new UserRepository();
            _userService = new UserService(userrapo);

            this._repository = new AccountRepository();
            this._userRepository = new UserRepository();
        }

        public AccountController(IAccountRepository repository, IUserRepository userRepository)
        {
            this._repository = repository;
            this._userRepository = userRepository;
        }

        [HttpGet]
        public ActionResult Login(string ReturnUrl)
        {
             var loginmodel = new LoginModel();

             string DomainName = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            // string DomainName = Request.Url.Host;
            // var CompanylogoInfo = _repository.GetCompanyLogo(DomainName);
            
            //if (CompanylogoInfo.Count > 0)
            // {
            //     foreach (var item in CompanylogoInfo)
            //     {
            //         loginmodel.OrgId = item.OrgId;
            //         loginmodel.OrgName = item.OrgName;
            //         loginmodel.Logolocation = item.Logolocation;
            //         loginmodel.Subdomain = DomainName;
            //     }
            // }
            // else
            // {
            //     loginmodel.OrgId = 0;
            //     loginmodel.OrgName = string.Empty;
            //     if (System.Configuration.ConfigurationManager.AppSettings["DefaultcompanyLogo"] != null)
            //     {
            //         loginmodel.Logolocation = System.Configuration.ConfigurationManager.AppSettings["DefaultcompanyLogo"].ToString();
            //     }
            //     else
            //     {
            //         loginmodel.Logolocation = string.Empty;
            //     }
                
            //    loginmodel.Subdomain = DomainName;
            // }
            ////http://localhost:51985


            loginmodel.OrgId = 0;
            loginmodel.OrgName = "";
            loginmodel.Logolocation =  "~/Images/DPIlogo.jpg";
            loginmodel.Subdomain = "";

         //   loginModel.Logolocation =//item.Logolocation;

            return View(loginmodel);
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel, string ReturnUrl)
        {

            ClearCache();

            if (ModelState.IsValid)
                if (loginModel != null || !string.IsNullOrWhiteSpace(loginModel.EmailId) || !string.IsNullOrWhiteSpace(loginModel.Password))
                {
                    loginModel.Result = (!string.IsNullOrWhiteSpace(loginModel.EmailId) && !string.IsNullOrWhiteSpace(loginModel.Password))
                                        ? _repository.Login(loginModel.EmailId, loginModel.Password, loginModel.Persist)
                                        : UserAuthenticationResult.UnknownUsernameOrPassword;

                    loginModel.LoginAttempt = true;

                    //string DomainName = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    //var CompanylogoInfo = _repository.GetCompanyLogo(DomainName);

                    //if (CompanylogoInfo.Count > 0)
                    //{
                    //    foreach (var item in CompanylogoInfo)
                    //    {
                    //        loginModel.OrgId = item.OrgId;
                    //        loginModel.OrgName = item.OrgName;
                    //        loginModel.Logolocation = item.Logolocation;
                    //        loginModel.Subdomain = DomainName;
                    //    }
                    //}
                    //else
                    //{
                    //    loginModel.OrgId = 0;
                    //    loginModel.OrgName = string.Empty;
                    //    if (System.Configuration.ConfigurationManager.AppSettings["DefaultcompanyLogo"] != null)
                    //    {
                    //        loginModel.Logolocation = System.Configuration.ConfigurationManager.AppSettings["DefaultcompanyLogo"].ToString();
                    //    }
                    //    else
                    //    {
                    //        loginModel.Logolocation = string.Empty;
                    //    }

                    //    loginModel.Subdomain = DomainName;
                    //}



                    loginModel.OrgId = 0;
                    loginModel.OrgName = "";
                    loginModel.Logolocation = "~/Images/DPIlogo.jpg";
                    loginModel.Subdomain = "";

                    if (loginModel.Result == UserAuthenticationResult.Authenticated)
                    {
                        User currentUser = _repository.GetUserByEmailId(loginModel.EmailId);
                        if (currentUser != null)
                        {
                            loginModel.Password = currentUser.Password;
                            loginModel.EmailId = currentUser.EmailAddress;
                            FormsAuthentication.SetAuthCookie(currentUser.Row_Id.ToString(), loginModel.Persist);
                        }

                        HttpContext.Cache["currentloggedinuser"] = new User();
                        HttpContext.Cache.Insert("currentloggedinuser", currentUser);

                        //  if (currentUser != null && currentUser.UserType == 1 || currentUser.UserType == 3)
                        if (currentUser != null)
                        {
                            return RedirectToAction("Index", "Home");
                            // return RedirectToAction("Index", "Scheduler");
                        }
                        else if (currentUser != null && currentUser.UserType == 2)
                        {
                            return RedirectToAction("Index", "Staff");
                        }
                        else
                        {
                            ViewBag.NotAuthorizedUser = "NotAuthorizedUser";
                        }
                        //else if (currentUser != null && currentUser.UserType == 2)
                        //{
                        //    return RedirectToAction("Index", "Staff");
                        //}
                        //else
                        //{
                        //    return RedirectToAction("Index", "Client");
                        //}
                    }
                }
            return View("Login", loginModel);
        }

        [HttpGet]
        public ActionResult UnLock(string ReturnUrl)
        {
            var currentUser = OrderManagement.Web.Helper.Utilitties.UserManager.Current();

            LoginModel loginModel = new LoginModel();
            loginModel.EmailId = currentUser.EmailAddress;
            return View(loginModel);
        }

        [HttpPost]
        public ActionResult UnLock(LoginModel loginModel, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                if (loginModel != null || !string.IsNullOrWhiteSpace(loginModel.EmailId) || !string.IsNullOrWhiteSpace(loginModel.Password))
                {
                    loginModel.Result = (!string.IsNullOrWhiteSpace(loginModel.EmailId) && !string.IsNullOrWhiteSpace(loginModel.Password))
                                        ? _repository.Login(loginModel.EmailId, loginModel.Password, loginModel.Persist)
                                        : UserAuthenticationResult.UnknownUsernameOrPassword;

                    loginModel.LoginAttempt = true;

                    if (loginModel.Result == UserAuthenticationResult.Authenticated)
                    {
                        User currentUser = _repository.GetUserByEmailId(loginModel.EmailId);
                        if (currentUser != null)
                        {
                            loginModel.Password = currentUser.Password;
                            loginModel.EmailId = currentUser.EmailAddress;
                            FormsAuthentication.SetAuthCookie(currentUser.Row_Id.ToString(), loginModel.Persist);
                        }

                        HttpContext.Cache["currentloggedinuser"] = new User();
                        HttpContext.Cache.Insert("currentloggedinuser", currentUser);

                        if (currentUser != null && currentUser.UserType == 1)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if (currentUser != null && currentUser.UserType == 2)
                        {
                            return RedirectToAction("Index", "Staff");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Client");
                        }
                    }
                }
            }
            return View("UnLock", loginModel);
        }

        [HttpGet]
        public ActionResult Register(UserRegistrationModel userRegistrationModel)
        {
            return View(userRegistrationModel);
        }

        [HttpPost]
        public ActionResult Register(string EmailId, string Password, string Firstname, string Lastname)
        {
            UserRegistrationModel userPersistentmodel = new UserRegistrationModel();
            if (ModelState.IsValid)
            {
                var result = _userRepository.CreateUser(EmailId, Password, Firstname, Lastname);

                userPersistentmodel.FirstName = Firstname;
                userPersistentmodel.LastName = Lastname;
                userPersistentmodel.Password = Password;
                userPersistentmodel.Result = result.UserPersistenceResult.ToString();

                if (result.UserPersistenceResult == UserPersistenceResult.UsernameInUse)
                {
                    return RedirectToAction("Register", "Account", userPersistentmodel);
                }
                if (result.UserPersistenceResult == UserPersistenceResult.Successful)
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            return RedirectToAction("Register", "Account", userPersistentmodel);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();



            if (HttpRuntime.Cache["CurrentLoggedInUser"] == null)
            {
                HttpRuntime.Cache.Remove("CurrentLoggedInUser");
            }

            FormsAuthentication.RedirectToLoginPage();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPassword model)
        {
            if (ModelState.ContainsKey("NewPassword"))
                ModelState["NewPassword"].Errors.Clear();

            if (ModelState.IsValid)
            {
                string EmailAddress = model.EmailAddress.ToString();
                User user = _userService.RegistrantUser(EmailAddress);
                if (user != null)
                {
                    ForgotPwd ForgotPassword = EmailClient.SendResetEmail(EmailAddress, "OMSTeam");

                    if (ForgotPassword != null)
                    {
                        using (var OrderMangtDB = new OrderMgntEntities())
                        {
                            //   var userDetails = OrderMangtDB.ForgotPwds.ToList().Where(x => x.ResetURL.Equals(OMS)).FirstOrDefault();
                            OrderMangtDB.ForgotPwds.Add(ForgotPassword);
                            OrderMangtDB.SaveChanges();
                            ForgotPassword objForgotPassword = new Models.ForgotPassword();
                            objForgotPassword.EmailAddress = ForgotPassword.UserID;
                            objForgotPassword.Msgtype = 0;
                            return PartialView("_EmailConfirmation", objForgotPassword);
                        }
                    }
                }
                else
                {
                    ViewBag.EmailNotExistMessage = "User with this Email does not exist.";
                    return View("ForgotPassword");
                }
            }
            return View("ForgotPassword");
        }

        [HttpGet]
        public ActionResult ReturnToLogin()
        {
            FormsAuthentication.RedirectToLoginPage();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult ResetPassword(string OMS)
        {
            using (var OrderMangtDB = new OrderMgntEntities())
            {
                var userDetails = OrderMangtDB.ForgotPwds.ToList().Where(x => x.ResetURL.Equals(OMS)).FirstOrDefault();
                ForgotPassword objForgotPassword = new Models.ForgotPassword();

                if (userDetails.Isused.HasValue)
                {
                    if ((bool)userDetails.Isused)
                    {
                        objForgotPassword.Msgtype = 1;
                        objForgotPassword.ErrorMsg = "This link has been already used. Return to 'log in' page and click forgot password to generate another link.";
                    }
                    else
                    {
                        objForgotPassword.Msgtype = 2;
                        objForgotPassword.UserID = userDetails.UserID.ToString();
                        return PartialView("_EmailConfirmation", objForgotPassword);
                    }
                }
                return PartialView("_EmailConfirmation", objForgotPassword);
            }
        }

        [HttpPost]
        public ActionResult UpdatePassword(ForgotPassword objForgotPassword)
        {
            if (ModelState.ContainsKey("EmailAddress"))
                ModelState["EmailAddress"].Errors.Clear();

            objForgotPassword.Msgtype = 2;
            if (ModelState.IsValid)
            {
                using (var OrderMangtDB = new OrderMgntEntities())
                {
                    var userDetails = OrderMangtDB.Users.ToList().Where(x => x.EmailAddress.Equals(objForgotPassword.UserID)).FirstOrDefault();
                    var ForgotPwds = OrderMangtDB.ForgotPwds.ToList().Where(x => x.UserID.Equals(objForgotPassword.UserID)).FirstOrDefault();
                    if (userDetails != null)
                    {
                        userDetails.Password = Cryptography.Encrypt(objForgotPassword.NewPassword);
                        ForgotPwds.Isused = true;
                        OrderMangtDB.SaveChanges();
                        // Update the entity in the database
                        objForgotPassword.Msgtype = 3;
                    }
                }
            }
            return PartialView("_EmailConfirmation", objForgotPassword);
        }

        private void ClearCache()
        {
            if (HttpRuntime.Cache["CurrentLoggedInUser"] != null)
            {
                HttpRuntime.Cache.Remove("CurrentLoggedInUser");
            }

            if (HttpRuntime.Cache["SelectJobAttachmentFolders"] != null)
            {
                HttpRuntime.Cache.Remove("SelectJobAttachmentFolders");
            }
        }



    }
}



public class EmailClient
{
    public static string FriendlyPassword()
    {
        string newPassword = Membership.GeneratePassword(8, 0);
        newPassword = System.Text.RegularExpressions.Regex.Replace(newPassword, @"[^a-zA-Z0-9]", m => "9");
        return newPassword;
    }

    public static ForgotPwd SendResetEmail(string UserEmail, string UserName)
    {
        string strSubject = System.Configuration.ConfigurationManager.AppSettings["MailSubject"];

        string encrypted = Encryption.Encrypt(String.Format("{0}&{1}", UserName, DateTime.Now.AddMinutes(50000).Ticks), "OMS");

        string BaseUrl = GetBaseUrl();
        var passwordLink = BaseUrl + "Account/ResetPassword?OMS=" + HttpUtility.UrlEncode(encrypted);
        string strBody = "<p>A request has been recieved to reset your password. If you did not initiate the request, then please ignore this email.</p>";
        strBody += "<p>Please click the following link to reset your password: <a href='" + passwordLink + "'>" + passwordLink + "</a></p>";

        bool hasEmailSent = Email.sendemail(UserEmail, "", strBody, true, false, strSubject);

        ForgotPwd ForgotPwd = new ForgotPwd();
        ForgotPwd.UserID = UserEmail;
        ForgotPwd.ResetURL = encrypted;
        ForgotPwd.Isused = false;
        ForgotPwd.createddate = System.DateTime.Now;

        try
        {
            return ForgotPwd;
            // smtpClient.Send(email);
        }
        catch (Exception ex)
        {
            return new ForgotPwd();
            // ErrorHandler.HandleError(ex, ErrorHandler.Level.Error);
        }
    }

    public static string GetBaseUrl()
    {
        var request = HttpContext.Current.Request;
        var appUrl = HttpRuntime.AppDomainAppVirtualPath;

        if (!string.IsNullOrWhiteSpace(appUrl))
            appUrl += "/";

        var baseUrl = string.Format("{0}:/{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

        return baseUrl;
    }
}

public class Encryption
{
    private const string _defaultKey = "*3ld+43j";

    public static string Encrypt(string toEncrypt, string key)
    {
        var des = new System.Security.Cryptography.DESCryptoServiceProvider();
        var ms = new System.IO.MemoryStream();

        VerifyKey(ref key);

        des.Key = HashKey(key, des.KeySize / 8);
        des.IV = HashKey(key, des.KeySize / 8);
        byte[] inputBytes = Encoding.UTF8.GetBytes(toEncrypt);

        var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
        cs.Write(inputBytes, 0, inputBytes.Length);
        cs.FlushFinalBlock();

        return HttpServerUtility.UrlTokenEncode(ms.ToArray());
    }

    public static string Decrypt(string toDecrypt, string key)
    {
        var des = new System.Security.Cryptography.DESCryptoServiceProvider();
        var ms = new System.IO.MemoryStream();

        VerifyKey(ref key);

        des.Key = HashKey(key, des.KeySize / 8);
        des.IV = HashKey(key, des.KeySize / 8);
        byte[] inputBytes = HttpServerUtility.UrlTokenDecode(toDecrypt);

        var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
        cs.Write(inputBytes, 0, inputBytes.Length);
        cs.FlushFinalBlock();

        var encoding = Encoding.UTF8;
        return encoding.GetString(ms.ToArray());
    }

    /// <summary>
    /// Make sure key is exactly 8 characters
    /// </summary>
    /// <param name="key"></param>
    private static void VerifyKey(ref string key)
    {
        if (string.IsNullOrEmpty(key))
            key = _defaultKey;

        key = key.Length > 8 ? key.Substring(0, 8) : key;

        if (key.Length < 8)
        {
            for (int i = key.Length; i < 8; i++)
            {
                key += _defaultKey[i];
            }
        }
    }

    private static byte[] HashKey(string key, int length)
    {
        var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] hash = sha.ComputeHash(keyBytes);
        byte[] truncateHash = new byte[length];
        Array.Copy(hash, 0, truncateHash, 0, length);
        return truncateHash;
    }
}