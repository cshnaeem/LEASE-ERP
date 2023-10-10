using AGEERP.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AGEERP.Controllers
{
    [RBAC]
    public class HomeController : Controller
    {
        DashboardBL dashboardBL = new DashboardBL();

        #region Dashboard
        public ActionResult Index()
        {
            var UserId = UserInfo.UserId;
            var locId = UserInfo.LocId;
            ViewBag.LocId = locId;
            if (UserId > 0)
            {
                var isBranch = dashboardBL.IsBranch(locId);
                ViewBag.isBranch = isBranch;
                if (isBranch)
                {
                    return View("Index");
                }
                else
                {
                    List<UserMenuInfo> menuList = new SecurityBL().GetMenuList(UserInfo.UserId, UserInfo.GroupId);
                    return View("Index2", menuList);
                }
            }
            return RedirectToAction("Logout", "Login");
        }
        public ActionResult _BranchDB(int LocId)
        {
            var lst = dashboardBL.GetBranchDashboard(LocId);
            ViewBag.tdCashSale = lst.tdCashSale;
            ViewBag.mnCashSale = lst.mnCashSale;
            ViewBag.tdInstSale = lst.tdInstSale;
            ViewBag.mnInstSale = lst.mnInstSale;
            ViewBag.tmCashSale = lst.tmCashSale;
            ViewBag.tmInstSale = lst.tmInstSale;
            ViewBag.policy = lst.policy;
            ViewBag.recovery = lst.recovery;
            ViewBag.pendingGrn = lst.pendingGrn;
            ViewBag.pendingStockReceive = lst.pendingStockReceive;
            ViewBag.pendingStockIssue = lst.pendingStockIssue;
            ViewBag.pendingCashReceive = lst.pendingCashReceive;
            ViewBag.pendingCashIssue = lst.pendingCashIssue;
            return PartialView();
        }

        public ActionResult Tutorial()
        {
            return View();
        }

        public async Task<ActionResult> Chat(string id)
        {
            ViewBag.fullName = await new EmployeeBL().GetEmpNameById(Convert.ToInt32(id));
            ViewBag.usr = id;
            return View();
        }
        public ActionResult WhatsAGE()
        {
            return View();
        }
        public ActionResult MainChat()
        {
            return View();
        }
        public ActionResult HelpDesk()
        {
            return View();
        }

        #endregion

    }

}