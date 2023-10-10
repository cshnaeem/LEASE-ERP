using AGEERP.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AGEERP.Controllers
{
    public class LoginController : Controller
    {
        SecurityBL securityBL = new SecurityBL();

        #region Login
        public ActionResult Index()
        {
            string ip = "";
            try
            {
                ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch (Exception)
            { }
            ViewBag.ip = ip;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login([Bind(Include = "Username, Password")] LoginVM login)
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", "");
            var sess = path;
            string ip = "";
            string com = "";
            try
            {
                ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
                //com = Dns.GetHostEntry(ip).HostName;
            }
            catch (Exception)
            { }
            DateTime loginDate = DateTime.Now;

            var user = await securityBL.LoginUser(login, sess, ip, com, loginDate);
            if (user.msg == "OK")
            {
                FormsAuthenticationTicket tic = new FormsAuthenticationTicket(1,
                    user.UserID.ToString(),
                    DateTime.Now,
                    DateTime.Now.Add(FormsAuthentication.Timeout),
                    false,
                    $"{user.UserID},{user.GroupId},{user.EmployeeId},{sess},{user.LocId},{user.FullName}",
                    FormsAuthentication.FormsCookiePath);
                // Encrypt the ticket.
                var encTicket = FormsAuthentication.Encrypt(tic);
                // Create the cookie.
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                //Response.Cookies.Add(new HttpCookie("CSCS", login.ThemeSel + "," + user.FullName));
                if (user.IsFirstLogin)
                {
                    return RedirectToAction("ChangePassword", "Security");
                }
                if (!string.IsNullOrEmpty(Request.Form["ReturnUrl"]))
                {
                    string[] req = Request.Form["ReturnUrl"].Split('-');
                    return RedirectToAction(req[1], req[0]);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", user.msg);
                login.Password = "";
                return View("Index", login);
            }
        }
        public JsonResult GetThemeList()
        {
            var lst = SelectListVM.UserThemes;
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
        public ActionResult ForgotPassword()
        {
            ViewBag.msg = "";
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ForgotPassword([Bind(Include = "Username, MobileNo")] ForgotPasswordVM mod)
        {
            if (ModelState.IsValid)
            {
                ViewBag.msg = await securityBL.ForgotPassword(mod);
            }
            return View();
        }
        #endregion 
    }
    public static class UserInfo
    {
        #region LoginInfo
        public static int UserId
        {
            get
            {
                UserInfoVM data = new UserInfoVM();
                //if (System.Web.HttpContext.Current.Request.Cookies != null)
                //{logout
                HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                    if (ticket != null)
                    {
                        if (!ticket.Expired)
                        {
                            var tick = ticket.UserData.Split(',');
                            if (tick.Length == 6)
                            {
                                return Convert.ToInt32(tick[0]);
                            }
                        }
                    }
                }
                //}
                return 0;
            }
        }
        public static int GroupId
        {
            get
            {
                UserInfoVM data = new UserInfoVM();
                HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

                    if (ticket != null)
                    {
                        if (!ticket.Expired)
                        {
                            var tick = ticket.UserData.Split(',');
                            if (tick.Length == 6)
                            {
                                return Convert.ToInt32(tick[1]);
                            }
                        }
                    }
                }
                return 0;
            }
        }
        public static int LocId
        {
            get
            {
                UserInfoVM data = new UserInfoVM();
                HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

                    if (ticket != null)
                    {
                        if (!ticket.Expired)
                        {
                            var tick = ticket.UserData.Split(',');
                            if (tick.Length == 6)
                            {
                                return Convert.ToInt32(tick[4]);
                            }
                        }
                    }
                }
                return 0;
            }
        }
        public static string Name
        {
            get
            {
                UserInfoVM data = new UserInfoVM();
                HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

                    if (ticket != null)
                    {
                        if (!ticket.Expired)
                        {
                            var tick = ticket.UserData.Split(',');
                            if (tick.Length == 6)
                            {
                                return tick[5];
                            }
                        }
                    }
                }
                return "";
            }
        }
        #endregion
    }
}