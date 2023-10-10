using AGEERP.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AGEERP.Controllers
{
    [RBAC]
    public class SecurityController : Controller
    {

        private readonly SecurityBL securityBl = new SecurityBL();
        private readonly EmployeeBL employeeBL = new EmployeeBL();

        public ActionResult Index()
        {
            List<UserMenuInfo> menuList = new SecurityBL().GetMenuList(UserInfo.UserId, UserInfo.GroupId);
            return View(menuList);
        }

        #region UserGroup
        //[RBAC]
        public ActionResult UserGroup()
        {
            return View();
        }

        public async Task<ActionResult> UserGroup_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await securityBl.UserGroupsList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> UserGroup_Create([DataSourceRequest] DataSourceRequest request, UsersGroupVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await securityBl.CreateUserGroups(mod);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.GroupId = tbl.GroupId;
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> UserGroup_Update([DataSourceRequest] DataSourceRequest request, UsersGroupVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await securityBl.UpdateUserGroups(mod);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Users
        public async Task<JsonResult> GetGroups()
        {
            var data = await securityBl.ActiveUserGroupsList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Users()
        {
            var grp = await securityBl.ActiveUserGroupsList();
            grp.Insert(0, new UsersGroupVM { GroupId = 0, GroupName = "Select" });
            ViewData["GroupVD"] = grp;
            //var emp = (await securityBl.EmployeeList()).Select(x => new EmployeeVM { EmployeeId = x.EmployeeId, EmployeeName = x.EmployeeName }).OrderBy(x => x.EmployeeName).ToList();
            //emp.Insert(0, new EmployeeVM { EmployeeId = 0, EmployeeName = "Select" });
            //ViewData["EmployeeVD"] = emp;
            return View();
        }

        public async Task<ActionResult> User_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await securityBl.UserList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> User_Create([DataSourceRequest] DataSourceRequest request, UserVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await securityBl.CreateUser(mod);
                if (tbl == null)
                    ModelState.AddModelError("", "Employee or LoginName may already exist");
                else
                    mod.UserID = tbl.UserID;
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> User_Update([DataSourceRequest] DataSourceRequest request, UserVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await securityBl.UpdateUser(mod);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangePassword(ChangePasswordVM mod)
        {
            var msg = await securityBl.ChangePassword(mod, UserInfo.UserId);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Rights
        public ActionResult AndroidRights()
        {
            return View();
        }
        //public async Task<JsonResult> GetUsers()
        //{
        //    var data = await securityBl.EmployeeList();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[RBAC]
        public ActionResult Rights()
        {
            return View();
        }
        public async Task<JsonResult> GetUserOrGroupList(int Id)
        {
            var lst = new List<UserVM>();
            if (Id == 1)
            {
                lst = (await securityBl.ActiveUserGroupsList()).Select(x => new UserVM { UserID = x.GroupId, Username = x.GroupName }).ToList();
                // return Json(lst, JsonRequestBehavior.AllowGet);
            }
            else if (Id == 2)
            {
                lst = (await securityBl.UserList()).Select(x => new UserVM { UserID = x.UserID, Username = x.Username }).ToList();
                //return Json(lst, JsonRequestBehavior.AllowGet);
            }

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetGroupRightsDetails(int id)
        {
            var list = await securityBl.GetGroupRights(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetUserRightsDetails(int id)
        {
            var list = await securityBl.GetUserRights(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> Forms(int id = 0)
        {

            var list = await securityBl.GetForms(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SaveRights(String values, string Id, string TypeId)
        {

            var vList = new List<int>(values.Split(',').Select(s => int.Parse(s)));

            vList.RemoveAt(vList.IndexOf(-1));

            if (await securityBl.SetRights(vList, Convert.ToInt32(Id), Convert.ToInt32(TypeId)))
            {
                DStore.Permissions = null;
                return Json("Saved Sucessfully", JsonRequestBehavior.AllowGet);
            }

            return Json("Nothing Saved", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Fin Rights
        public ActionResult FinRights()
        {
            return View();
        }
        //public async Task<JsonResult> FinGetUserOrGroupList(int Id)
        //{
        //    var lst = new List<UserVM>();
        //    if (Id == 1)
        //    {
        //        lst = (await securityBl.ActiveUserGroupsList()).Select(x => new UserVM { UserID = x.GroupId, Username = x.GroupName }).ToList();
        //        // return Json(lst, JsonRequestBehavior.AllowGet);
        //    }
        //    else if (Id == 2)
        //    {
        //        lst = (await securityBl.UserList()).Select(x => new UserVM { UserID = x.UserID, Username = x.Username }).ToList();
        //        //return Json(lst, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}

        public async Task<JsonResult> FinGetGroupRightsDetails(int id)
        {
            var list = await securityBl.FinGetGroupRights(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> FinGetUserRightsDetails(int id)
        {
            var list = await securityBl.FinGetUserRights(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> FinForms(int id = 0)
        {

            var list = await securityBl.FinGetForms(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> FinSaveRights(String values, string Id, string TypeId)
        {

            var vList = new List<long>(values.Split(',').Select(s => long.Parse(s)));

            vList.RemoveAt(vList.IndexOf(-1));

            if (await securityBl.FinSetRights(vList, Convert.ToInt32(Id), Convert.ToInt32(TypeId)))
            {
                //DStore.Permissions = null;
                return Json("Saved Sucessfully", JsonRequestBehavior.AllowGet);
            }

            return Json("Nothing Saved", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region MenuObject
        public async Task<ActionResult> MenuObject()
        {
            ViewBag.MenuListVD = await securityBl.MenuList();
            return View();
        }
        public async Task<ActionResult> MenuObject_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await securityBl.MenuObjectList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MenuObject_Create([DataSourceRequest] DataSourceRequest request, MenuObjectVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await securityBl.UpdateMenuObject(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
                else
                    DStore.Permissions = null;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MenuObject_Update([DataSourceRequest] DataSourceRequest request, MenuObjectVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await securityBl.UpdateMenuObject(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
                else
                    DStore.Permissions = null;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Mobile Menu

        public ActionResult MobileMenuObject()
        {
            return View();
        }
        public async Task<ActionResult> MobileMenuObject_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await securityBl.GetMobileMenu();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> MobileMenuObjectList()
        {
            var lst = await securityBl.GetMobileMenu();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MobileMenuObject_Create([DataSourceRequest] DataSourceRequest request, MobileMenuVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await securityBl.AddMobileMenu(mod);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }


            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MobileMenuObject_Update([DataSourceRequest] DataSourceRequest request, MobileMenuVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await securityBl.UpdateMobileMenu(mod);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }


            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MobileMenuObject_Delete([DataSourceRequest] DataSourceRequest request, MobileMenuVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await securityBl.DeleteMobileMenu(mod);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult MobileMenuAccess()
        {
            return View();
        }
        public ActionResult MobileMenuAccessObject_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = securityBl.GetMobileMenuAccess();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MobileMenuAccessObject_Create([DataSourceRequest] DataSourceRequest request, MobileMenuAccessVM mod, int[] SelectedActivity)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await securityBl.AddUpdateMobileMenuAccess(mod.GroupId, SelectedActivity);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }


            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MobileMenuAccessObject_Update([DataSourceRequest] DataSourceRequest request, MobileMenuAccessVM mod, int[] SelectedActivity)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await securityBl.AddUpdateMobileMenuAccess(mod.GroupId, SelectedActivity);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }


            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MobileMenuAccessObject_Delete([DataSourceRequest] DataSourceRequest request, MobileMenuAccessVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await securityBl.RemoveMobileMAccess(mod.GroupId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> MobileMenuTest()
        {
            ViewBag.MenuListVD = await securityBl.MenuList();
            return View();
        }
        public JsonResult StartMobileMenuTest()
        {
            MenuObjectTest.IsStarted = true;
            return Json("OK",JsonRequestBehavior.AllowGet);
        }
        public JsonResult StopMobileMenuTest()
        {
            MenuObjectTest.IsStarted = false;
            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SaveMobileMenuTest(int MenuId)
        {
            var msg = await securityBl.SaveMenuObject(MenuId);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult GetMenu()
        {
            var UserId = UserInfo.UserId;
            if (UserId == 0)
            {
                return PartialView("Error");
            }
            List<UserMenuInfo> menuList = securityBl.GetMenuList(UserId, UserInfo.GroupId);
            return PartialView("_Menu", menuList);
        }
        public ContentResult GetUser()
        {
            return Content(UserInfo.Name);
        }

    }
}