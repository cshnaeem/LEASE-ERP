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
    public class CashController : Controller
    {
        SetupBL setupBL = new SetupBL();
        CashBL cashBL = new CashBL();
        AccountBL accountBL = new AccountBL();

        #region CashCollection
        public ActionResult Index()
        {
            List<UserMenuInfo> menuList = new SecurityBL().GetMenuList(UserInfo.UserId, UserInfo.GroupId);
            return View(menuList);
        }

        public ActionResult CashDeposit()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        public ActionResult CashCollection()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);

            return View();
        }
        public async Task<ActionResult> CashCollection_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await cashBL.GetCashCollection(LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CashCollection_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CashCollectionVM> mod, int LocId, DateTime WorkingDate, string DepositTo, decimal CashDeposit, string CashierRemarks, long CashierId, bool IsVault, string UploadedFiles)
        {
            if (mod != null && ModelState.IsValid)
            {

                var DocId = await cashBL.CashCollectionSave(mod, LocId, WorkingDate, DepositTo, CashDeposit, CashierRemarks, CashierId, IsVault, UploadedFiles, UserInfo.UserId);
                if (DocId == 0)
                    ModelState.AddModelError("", "Server Error");
                else
                {
                    ModelState.AddModelError("Msg", DocId.ToString());
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
       

        public async Task<ActionResult> BankDepositList()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            ViewData["Bank"] = await accountBL.SubCodeBankList();
            return View();
        }
        public async Task<ActionResult> CashCollectionList()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            ViewData["Cashier"] = (await cashBL.GetCashierList(UserInfo.LocId)).Select(x => new { EmpId = x.EmpId, EmpName = x.EmpName, CNIC = x.CNIC }).ToList();
            return View();
        }
        public async Task<ActionResult> CashCollectionList_Read([DataSourceRequest] DataSourceRequest request, string DepositTo, int LocId, long EmpId, DateTime WorkingDate)
        {
            var lst = await cashBL.CashCollectionList(DepositTo, LocId, EmpId, WorkingDate);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CashCollectionList_Update([DataSourceRequest] DataSourceRequest request, CashCollectionListVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await cashBL.CashCollectionListUpdate(mod, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                {
                    mod = tbl;
                    //ModelState.AddModelError("Msg", "Save Successfully");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CashCollectionList_Destroy([DataSourceRequest] DataSourceRequest request, CashCollectionListVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await cashBL.CashCollectionListDestroy(mod, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                {
                    //mod = tbl;
                    //ModelState.AddModelError("Msg", "Save Successfully");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public async Task<JsonResult> CashCollectionPosting(List<long> TransLst)
        {
            var result = await accountBL.CashTransferVoucher(TransLst, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetMapCashierByLocation(int locid)
        {
            var lst = (await new EmployeeBL().GetMapCashierByLocation(locid)).Select(x => new { EmpId = x.EmpId, EmpName = x.EmpName }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetMapBankByLocation(int locid)
        {
            var lst = (await new EmployeeBL().GetMapBankByLocation(locid)).Select(x => new { BankId = x.BankId, BankName = x.BankName }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CashierList(int LocId)
        {
            var lst = (await cashBL.GetCashierList(LocId)).Select(x => new { EmpId = x.EmpId, EmpName = x.EmpName, CNIC = x.CNIC }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CashCollectionCenterList()
        {
            var lst = await cashBL.GetCashCollectionCenterList(UserInfo.LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CashReceive
        public async Task<ActionResult> CashReceive()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> CashReceive(CashReceiveVM mod)
        {
            if (ModelState.IsValid)
            {
                var transId = await new CashBL().SaveCashRecive(mod, UserInfo.UserId);
                mod.TransId = transId;
                return Json(mod, JsonRequestBehavior.AllowGet);
            }
            return Json(mod, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ExpenseList()
        {
            var lst = await setupBL.ExpenseList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        //public async Task<JsonResult> SaleTypeList()
        //{
        //    var lst = await saleBL.SaleTypeList();
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region CashPayment
        public async Task<JsonResult> PaymentHeadList()
        {
            var lst = await cashBL.PaymentHeadList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> PaymentSubCodeList(string head)
        {
            var lst = await cashBL.PaymentSubCodeList(head);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CashPayment()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> CashPayment(CashPaymentVM mod)
        {
            if (ModelState.IsValid)
            {
                var transId = await new CashBL().SaveCashPayment(mod, UserInfo.UserId);
                mod.TransId = transId;
                return Json(mod, JsonRequestBehavior.AllowGet);
            }
            return Json(mod, JsonRequestBehavior.AllowGet);
        }

        //public async Task<JsonResult> SaleTypeList()
        //{
        //    var lst = await saleBL.SaleTypeList();
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}
        #endregion
    }


}