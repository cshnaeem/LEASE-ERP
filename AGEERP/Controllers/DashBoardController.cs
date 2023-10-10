using AGEERP.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AGEERP.Controllers
{
    [RBAC]
    public class DashBoardController : Controller
    {
        DashboardBL dashboardBL = new DashboardBL();

        #region HODashboards
        public ActionResult SaleComparison()
        {
            return View();
        }

        public async Task<ActionResult> HRDashBoards()
        {
            ViewBag.VTitle = "HR";
            var lst = await dashboardBL.GetDashboard(1);
            return View("Dashboard", lst);
        }
        public async Task<ActionResult> Sales()
        {
            ViewBag.VTitle = "Sales";
            var lst = await dashboardBL.GetDashboard(2);
            return View("Dashboard", lst);
        }
        public async Task<ActionResult> Cash()
        {
            ViewBag.VTitle = "Cash";
            var lst = await dashboardBL.GetDashboard(3);
            return View("Dashboard", lst);
        }
        public async Task<ActionResult> Stock()
        {
            ViewBag.VTitle = "Stock";
            var lst = await dashboardBL.GetDashboard(4);
            return View("Dashboard", lst);
        }
        public async Task<ActionResult> Implementation()
        {
            ViewBag.VTitle = "Implementation";
            var lst = await dashboardBL.GetDashboard(5);
            return View("Dashboard", lst);
        }
        public async Task<ActionResult> CRM()
        {
            ViewBag.VTitle = "CRM";
            var lst = await dashboardBL.GetDashboard(7);
            return View("Dashboard", lst);
        }
        public async Task<JsonResult> GetDash(string DashId)
        {
            var url = await dashboardBL.GetDash(DashId);
            return Json(url, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetDashboardCloud()
        {
            var dashbl = await dashboardBL.GetDashboardCloud();
            return Json(dashbl, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> StockComparison()
        {
            ViewBag.dat = await (new StockBL().GetStockAll());
            return View();
        }
        public async Task<ActionResult> StockComparison_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json((await (new StockBL()).GetStockAll()).ToDataSourceResult(request));
        }

        public async Task<JsonResult> GetHDashboard(int LocId)
        {
            var lst = await dashboardBL.GetHDashboard(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetTwoMonthSale(int LocId)
        {
            var lst = await dashboardBL.GetLocDashboard(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}