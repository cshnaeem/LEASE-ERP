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
    public class TargetController : Controller
    {
        TargetBL targetBL = new TargetBL();
        public ActionResult SaleTarget()
        {
            return View();
        }
        public async Task<ActionResult> SaleTarget_Read([DataSourceRequest] DataSourceRequest request, DateTime month, string TypeId)
        {
            var lst = await targetBL.GetSaleTarget(month, TypeId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SaleTarget_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SaleTargetVM> mod, string TypeId)
        {
            var lst = new List<SaleTargetVM>();
            if (mod != null && ModelState.IsValid)
            {
                var target = await targetBL.CreateSaleTarget(mod, UserInfo.UserId, TypeId);
                if (target != null)
                {
                    lst.AddRange(target);
                    ModelState.AddModelError("Msg", "Saved Successfully.");
                }
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(new[] { lst }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SaleTarget_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SaleTargetVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await targetBL.UpdateSaleTarget(mod, UserInfo.UserId);
                if (IsSave)
                    ModelState.AddModelError("Msg", "Saved Successfully.");
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        #region Incentive
        public ActionResult Incentive()
        {
            return View();
        }

        public async Task<ActionResult> Incentive_Read([DataSourceRequest] DataSourceRequest request, DateTime month, int PTypeID)
        {
            var lst = await targetBL.GetEmpPerformance(month, PTypeID);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Incentive_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<IncentiveVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await targetBL.UpdateIncentive(mod, 1);
                if (IsSave)
                    ModelState.AddModelError("Msg", "Saved Successfully.");
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region ProcessPerfromance
        public ActionResult ProcessPerformance()
        {
            return View();
        }
        //[HttpPost]
        public async Task<ActionResult> ProcessPerformance_Read([DataSourceRequest] DataSourceRequest request, int PerformanceTypeId, DateTime ToDate)
        {
            var UserId = UserInfo.UserId;
            var lst = await targetBL.ProcessPerformance(PerformanceTypeId, ToDate, UserId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


        }
        public async Task<JsonResult> GetPerformanceTypeList()
        {
            var lst = await targetBL.PerformanceTypeList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}