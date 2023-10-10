using AGEERP.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NotifSystem.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AGEERP.Controllers
{
    [RBAC]
    public class AccountController : Controller
    {
        AccountBL accountBL = new AccountBL();
        SetupBL setupBL = new SetupBL();
        public ActionResult Index()
        {
            List<UserMenuInfo> menuList = new SecurityBL().GetMenuList(UserInfo.UserId, UserInfo.GroupId);
            return View(menuList);
        }

        #region COA
        public ActionResult COA()
        {
            return View();
        }
        public async Task<JsonResult> NewCode(string PCode)
        {
            var lst = await accountBL.GetNewCode(PCode);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> COAList(long ParentId, int Level)
        {
            var lst = (await accountBL.COAList(ParentId, Level)).Select(x => new { Id = x.Id, Code = x.Code, Name = x.Name }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> COA_Read([DataSourceRequest] DataSourceRequest request, long ParentId, int Level)
        {
            var lst = await accountBL.COAList(ParentId, Level);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> COA_Create([DataSourceRequest] DataSourceRequest request, COAVM mod, string ParentId, int Level)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var tbl = await accountBL.CreateCOA(mod, ParentId, Level, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                {
                    mod.Code = tbl.Code;
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> COA_Update([DataSourceRequest] DataSourceRequest request, COAVM mod, int ParentId, int Level)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var IsSave = await accountBL.UpdateCOA(mod, Level, UserInfo.UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> SubCodeBankList()
        {
            var lst = await accountBL.SubCodeBankList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetCheque(long AccId)
        {
            var lst = await accountBL.GetCheque(AccId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SubCodeCashList()
        {
            var lst = await accountBL.SubCodeCashList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CostCenters
        public async Task<ActionResult> CostCenters()
        {
            var loc = await setupBL.LocationList();
            loc.Insert(0, new LocationVM { LocName = "NA", LocId = 0 });
            ViewData["LocVD"] = loc;
            return View();
        }
        public async Task<ActionResult> CostCenters_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await accountBL.CostCentersList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CostCenters_Create([DataSourceRequest] DataSourceRequest request, CostCentersVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await accountBL.CreateCostCenters(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.CCCode = tbl.CCCode;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CostCenters_Update([DataSourceRequest] DataSourceRequest request, CostCentersVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await accountBL.UpdateCostCenters(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CostCenters_Destroy([DataSourceRequest] DataSourceRequest request, CostCentersVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await accountBL.DestroyCostCenters(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        public async Task<JsonResult> CostCentersList()
        {
            var lst = await accountBL.CostCentersList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CostCentersByProfitCenterList(int PCCode)
        {
            var lst = await accountBL.CostCentersByProfitCenterList(PCCode);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ProfitCentersList()
        {
            var lst = await accountBL.ProfitCentersList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ProfitCentersByRegionList([Bind(Prefix = "RegLst[]")] int[] RegLst)
        {
            if (RegLst == null)
            {
                RegLst = new int[0];
            }
            var lst = await accountBL.ProfitCentersByRegList(RegLst.Select(x => (int?)x).ToList());
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region BankBooks
        public async Task<ActionResult> BankBook()
        {
            ViewData["SubCodeBankVD"] = (await accountBL.SubCodeBankList()).Select(x => new { Id = x.Id, SubCode = x.Code, Title = x.Name }).ToList();
            ViewData["AccountTypeSL"] = SelectListVM.BankAccountType;
            return View();
        }
        public async Task<ActionResult> BankBook_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await accountBL.BankBookList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> BankBook_Create([DataSourceRequest] DataSourceRequest request, BankBookVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                mod.AccId = mod.AccId;
                var UserId = UserInfo.UserId;
                var tbl = await accountBL.CreateBankBook(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.TransID = tbl.TransID;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public async Task<ActionResult> BankBook_Update([DataSourceRequest] DataSourceRequest request, BankBookVM mod)
        //{
        //    if (mod != null && ModelState.IsValid)
        //    {
        //        var UserId = UserInfo.UserId;
        //        var IsSave = await accountBL.UpdateBankBook(mod, UserId);
        //        if (!IsSave)
        //        {
        //            ModelState.AddModelError("", "Server Error");
        //        }
        //    }
        //    return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> BankBook_Destroy([DataSourceRequest] DataSourceRequest request, BankBookVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await accountBL.DestroyBankBook(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Voucher

        public async Task<ActionResult> CPV()
        {
            ViewBag.Type = "CPV";
            ViewBag.CYear = (await accountBL.CYear(DateTime.Now.Date)).YrName;
            ViewBag.CPeriod = (await accountBL.CPeriod(DateTime.Now.Date)).PrName;
            var loc = await accountBL.ProfitCentersList();
            //loc.Insert(0, new ProfitCentersVM { PCDesc = "NA", PCCode = 0 });
            ViewData["PCVD"] = loc;
            ViewData["CCVD"] = await accountBL.CostCentersList();
            //ViewBag.COA4 = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return View();
        }
        public async Task<ActionResult> CRV()
        {
            ViewBag.Type = "CRV";
            ViewBag.CYear = (await accountBL.CYear(DateTime.Now.Date)).YrName;
            ViewBag.CPeriod = (await accountBL.CPeriod(DateTime.Now.Date)).PrName;
            var loc = await accountBL.ProfitCentersList();
            //loc.Insert(0, new ProfitCentersVM { PCDesc = "NA", PCCode = 0 });
            ViewData["PCVD"] = loc;
            ViewData["CCVD"] = await accountBL.CostCentersList();
            //ViewBag.COA4 = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return View("CPV");
        }
        public async Task<ActionResult> BPV()
        {
            ViewBag.Type = "BPV";
            ViewBag.CYear = (await accountBL.CYear(DateTime.Now.Date)).YrName;
            ViewBag.CPeriod = (await accountBL.CPeriod(DateTime.Now.Date)).PrName;
            var loc = await accountBL.ProfitCentersList();
            //loc.Insert(0, new ProfitCentersVM { PCDesc = "NA", PCCode = 0 });
            ViewData["PCVD"] = loc;
            ViewData["CCVD"] = await accountBL.CostCentersList();
            //ViewBag.COA4 = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return View();
        }
        public async Task<ActionResult> BRV()
        {
            ViewBag.Type = "BRV";
            ViewBag.CYear = (await accountBL.CYear(DateTime.Now.Date)).YrName;
            ViewBag.CPeriod = (await accountBL.CPeriod(DateTime.Now.Date)).PrName;
            var loc = await accountBL.ProfitCentersList();
            //loc.Insert(0, new ProfitCentersVM { PCDesc = "NA", PCCode = 0 });
            ViewData["PCVD"] = loc;
            ViewData["CCVD"] = await accountBL.CostCentersList();
            //ViewBag.COA4 = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return View();
        }


        public async Task<ActionResult> JV()
        {
            ViewBag.CYear = (await accountBL.CYear(DateTime.Now.Date)).YrName;
            ViewBag.CPeriod = (await accountBL.CPeriod(DateTime.Now.Date)).PrName;
            var loc = await accountBL.ProfitCentersList();
            //loc.Insert(0, new ProfitCentersVM { PCDesc = "NA", PCCode = 0 });
            ViewData["PCVD"] = loc;
            ViewData["CCVD"] = await accountBL.CostCentersList();
            //ViewBag.COA4 = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> VoucherEdit(FormCollection frm)
        {
            long VrId = Convert.ToInt64(frm["VcrId"]);
            var v = await accountBL.GetVoucher(VrId);
            if (v.ApprovedBy != null)
            {
                return Content("You cannot edit approved Voucher");
            }
            ViewBag.CYear = (await accountBL.CYear(v.VrDate)).YrName;
            var prd = await accountBL.CPeriod(v.VrDate);
            if (prd == null)
            {
                return Content("You cannot edit Closed Period Voucher");
            }
            ViewBag.CPeriod = prd.PrName;
            ViewBag.VrId = VrId;
            ViewBag.VrNo = v.VrNo;
            ViewBag.RefDocNo = v.RefDocNo;
            ViewBag.RefDocDate = v.RefDocDate;
            var loc = await accountBL.ProfitCentersList();
            //loc.Insert(0, new ProfitCentersVM { PCDesc = "NA", PCCode = 0 });
            ViewData["PCVD"] = loc;
            ViewData["CCVD"] = await accountBL.CostCentersList();
            ViewData["COA"] = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name, Id = x.Id }).ToList();
            return View();
        }
        public async Task<JsonResult> GetVoucher(long VrId)
        {
            var v = await accountBL.GetVoucher(VrId);
            //var ls = v.Fin_VoucherDetail.OrderBy(x => x.TrxSeqId).ToList();
            //var lst = new List<VoucherDetailVM>();
            //foreach (var x in ls)
            //{
            //    var itm = new VoucherDetailVM
            //    {
            //        CCCode = x.CCCode,
            //        ChequeNo = x.ChequeNo,
            //        Cr = x.Cr,
            //        Dr = x.Dr,
            //        Particulars = x.Particulars,
            //        PCCode = x.PCCode,
            //        SubCode = x.Fin_Accounts.SubCode,
            //        SubCodeDesc = x.Fin_Accounts.SubCodeDesc,
            //        RefId = x.RefId,
            //        SubId = x.SubId ?? 0,
            //        VrDtlId = x.VrDtlId,
            //        AccId = x.AccId,
            //        VrId = x.VrId
            //    };

            //}

            //string code = "";
            //string chequeNo = "";
            //if(v.VrTypeId == "BPV" || v.VrTypeId == "CPV")
            //{
            //    var cr = v.Fin_VoucherDetail.Where(x => x.Cr > 0).FirstOrDefault();
            //    code = cr.SubCode;
            //    chequeNo = cr.ChequeNo;
            //    lst = lst.Where(x => x.Dr > 0).ToList();
            //}
            //else if (v.VrTypeId == "BRV" || v.VrTypeId == "CRV")
            //{
            //    code = v.Fin_VoucherDetail.Where(x => x.Dr > 0).Select(x => x.SubCode).FirstOrDefault();
            //    lst = lst.Where(x => x.Cr > 0).ToList();
            //}
            var data = new { v.VrNo, v.VrId, v.VrDate, v.VrPaidTo, v.RefDocDate, v.RefDocNo };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Voucher_Read([DataSourceRequest] DataSourceRequest request, long VrId)
        {
            var lst = await accountBL.GetVoucherDetail(VrId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Voucher_Update([DataSourceRequest] DataSourceRequest request, VoucherDetailVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var vNo = await accountBL.UpdateVoucher(mod, UserInfo.UserId);
                if (!vNo)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> SubCodeList(string str)
        {
            var lst = await accountBL.SubCodeList(str);//).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> FinSubCodeList(string str)
        {
            var lst = await accountBL.SubCodeListByUser(str, UserInfo.UserId, UserInfo.GroupId);//).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SubsidaryCodeList(long PId, string str)
        {
            //str = str.ToUpper();
            var lst = await accountBL.SubsidiaryList(PId, str);//).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            //lst = lst.Where(x => x.Code.ToUpper().Contains(str) || x.Name.ToUpper().Contains(str)).Take(100).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> VoucherList()
        {
            var lst = (await accountBL.GetVoucherList()).Select(x => new { VrDate = x.VrDate.ToString("dd-MM-yyyy"), x.VrNo, x.Amount, x.VrId, x.VrTypeId, }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> JV_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VoucherDetailVM> mod, string RefDocNo, DateTime RefDocDate, DateTime VoucherDate, List<long> files)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var vNo = await accountBL.CreateJV(mod, VoucherDate, "AJV", RefDocNo, RefDocDate, UserInfo.UserId);
                if (vNo == "")
                    ModelState.AddModelError("", "Server Error");
                else
                {
                    if (files != null && files.Count > 0)
                    {
                        await new DocumentBL().UpdateDocRef(files, Convert.ToInt64(vNo));
                    }
                    ModelState.AddModelError("Msg", vNo);
                }
                //{
                //    mod.Id = tbl.Id;
                //    mod.Code = tbl.Code;
                //}

            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CPV_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VoucherDetailVM> mod, string VType, long CashAcc, string PaidTo, string MasParticular, string RefDocNo, DateTime RefDocDate, DateTime VoucherDate, bool IsMulti)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var vNo = await accountBL.CreateCPV(mod, VoucherDate, VType, CashAcc, PaidTo, MasParticular, RefDocNo, RefDocDate, IsMulti, UserInfo.UserId);
                if (vNo == "")
                    ModelState.AddModelError("", "Server Error");
                else
                    ModelState.AddModelError("Msg", vNo);
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> BPV_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VoucherDetailVM> mod, string VType, long BankAcc, string PaidTo, string MasParticular, string RefDocNo, DateTime RefDocDate, string ChequeNo, DateTime VoucherDate, bool IsMulti, string instrumentno, string instrumenttype)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var vNo = await accountBL.CreateBPV(mod, VoucherDate, VType, BankAcc, PaidTo, MasParticular, RefDocNo, RefDocDate, ChequeNo, IsMulti, UserInfo.UserId, instrumentno, instrumenttype);
                if (vNo == "")
                    ModelState.AddModelError("", "Server Error");
                else
                    ModelState.AddModelError("Msg", vNo);
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region VoucherTypes
        public async Task<ActionResult> VoucherTypes()
        {
            ViewBag.VType = (await accountBL.GetVoucherType()).Select(x => new VoucherTypeVM
            {
                VrTypeDesc = x.VrTypeDesc,
                VrTypeId = x.VrTypeId
            }).ToList();
            return View();
        }
        public async Task<ActionResult> VoucherTypeList()
        {
            var lst = (await accountBL.GetVoucherType()).Select(x => new VoucherTypeVM
            {
                VrTypeDesc = x.VrTypeDesc,
                VrTypeId = x.VrTypeId
            }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Financial Year

        public ActionResult FinancialPeriod()
        {
            ViewData["FinStatus"] = SelectListVM.FinStatus;
            return View();
        }
        public async Task<JsonResult> FinancialYearList()
        {
            var lst = (await accountBL.FinancialYears()).Select(x => new { YrCode = x.YrCode, YrName = x.YrName }).OrderByDescending(x => x.YrCode).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> FinancialPeriod_Read([DataSourceRequest] DataSourceRequest request, int YrCode)
        {
            var lst = await accountBL.FinancialPeriods(YrCode);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> FinancialPeriod_Create([DataSourceRequest] DataSourceRequest request, FinancialPeriodVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;

                var tbl = await accountBL.UpdateFinancialPeriod(mod, UserInfo.UserId);
                if (tbl == false)
                    ModelState.AddModelError("", "Server Error");

            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }






        #endregion

        #region VoucherSearch
        public async Task<ActionResult> VoucherSearch()
        {
            ViewBag.VrStatus = SelectListVM.VrStatusSL;
            ViewBag.UserId = UserInfo.UserId;
            //ViewBag.COA4 = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return View();
        }
        public async Task<ActionResult> VoucherSearch_Read([DataSourceRequest] DataSourceRequest request, DateTime FromDate, DateTime ToDate, string TypeId, string VNo, string Narration, string Code, int PCCode, string ChequeNo, string VStatus, string RefDocNo)
        {
            var lst = await accountBL.GetVoucherSearch(FromDate, ToDate, TypeId, VNo, Narration, Code, PCCode, ChequeNo, VStatus, RefDocNo);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region VoucherApproval
        public async Task<ActionResult> VoucherValidation()
        {
            ViewBag.Level = "V";
            ViewBag.UserId = UserInfo.UserId;
            ViewBag.VrStatus = SelectListVM.VrStatusSL;
            ViewBag.COA4 = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return View("VoucherApproval");
        }
        public async Task<ActionResult> VoucherApproval()
        {
            ViewBag.Level = "A";
            ViewBag.UserId = UserInfo.UserId;
            ViewBag.VrStatus = SelectListVM.VrStatusSL;
            ViewBag.COA4 = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return View();
        }
        public async Task<ActionResult> VoucherApproval_Read([DataSourceRequest] DataSourceRequest request, string Level)
        {
            var UserId = UserInfo.UserId;
            var lst = await accountBL.GetVoucherApproval(Level, UserId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> VoucherApproval_Destroy([DataSourceRequest] DataSourceRequest request, VoucherSearchVM mod, string Level)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var IsSave = await accountBL.VoucherApproval(mod, Level, UserInfo.UserId);
                if (!IsSave)
                    ModelState.AddModelError("", "Server Error");
                else
                    ModelState.AddModelError("Msg", "Saved Successfully");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region PaymentAdvice
        public async Task<ActionResult> PaymentAdvice()
        {
            var lst = await accountBL.SubCodeList();
            var suppArr = new long[] { 20010100010, 20011000010, 20012000010, 20013000010 };
            ViewBag.COA4 = lst.Where(x => suppArr.Contains(x.Id)).Select(x => new { Name = x.Name, Code = x.Code, Id = Convert.ToInt64(x.Code.Replace("-", "")) }).ToList();
            return View();
        }
        public async Task<ActionResult> PaymentAdvice_Read([DataSourceRequest] DataSourceRequest request, bool Reg, long AccId)
        {
            var lst = accountBL.GetPaymentAdvice(Reg, AccId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> PaymentAdvice_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PaymentAdviceDetailVM> mod, bool Reg)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var dNo = await accountBL.CreatePaymentAdvice(mod, Reg, UserInfo.UserId);
                if (dNo == "")
                    ModelState.AddModelError("", "Server Error");
                else
                    ModelState.AddModelError("Msg", dNo);
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region SystemIntegration

        public ActionResult SystemIntegration()
        {
            return View();
        }

        public async Task<ActionResult> SystemIntegration_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await accountBL.SystemInegrationlist();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SystemIntegration_Update([DataSourceRequest] DataSourceRequest request, SystemIntegrationVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var Userid = UserInfo.UserId;
                var isSave = await accountBL.UpdateIntegration(mod, Userid);
                if (!isSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
                else
                {
                    ModelState.AddModelError("Msg", "Seve Successfully");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region VoucherPosting
        public ActionResult SalesVoucherPosting()
        {
            return View();
        }
        public async Task<ActionResult> SalesVoucherPosting_Read([DataSourceRequest] DataSourceRequest request, int LocId, int TransTypeId, DateTime WorkingDate)
        {
            var lst = accountBL.GetSaleForVoucherPosting(LocId, TransTypeId, WorkingDate);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> SalesByType_Read([DataSourceRequest] DataSourceRequest request, int LocId, DateTime WorkingDate, string Type)
        {
            var lst = accountBL.GetSaleByType(WorkingDate, LocId, Type);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> SalesVoucherPosting(List<long> TransLst, int LocId, int TransTypeId, DateTime WorkingDate)
        {
            var lst = accountBL.GetSaleForVoucherPosting(LocId, TransTypeId, WorkingDate);
            lst = lst.Where(x => TransLst.Contains(x.TransId)).ToList();
            var result = await accountBL.SaveSaleVoucherPosting(lst, TransTypeId, WorkingDate, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ExpenseVoucherPosting()
        {
            ViewData["LocVD"] = await setupBL.LocationList();
            ViewData["ExpenseHeadVD"] = (await setupBL.ExpenseList()).Select(x => new ExpenseVM { ExpHeadId = x.ExpHeadId, ExpHead = x.ExpHead }).ToList();
            ViewData["CCVD"] = await accountBL.CostCentersList();
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }

        public async Task<ActionResult> ExpenseVoucherPosting_Read([DataSourceRequest] DataSourceRequest request, int Loc, int ExpHead, DateTime Working)
        {
            var lst = await accountBL.ExpenseVoucherPostingList(Loc, ExpHead, Working);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ExpenseVoucherPosting_Create([DataSourceRequest] DataSourceRequest request, ExpenseTransactionVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var transId = await accountBL.CreateExpenseVoucherPosting(mod, UserId);
                if (transId > 0)
                {
                    mod.TransId = transId;
                }
                else
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ExpenseVoucherPosting_Update([DataSourceRequest] DataSourceRequest request, ExpenseTransactionVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await accountBL.UpdateExpenseVoucherPosting(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<JsonResult> ExpenseVoucherPosting(List<long> TransLst)
        {
            //var lst = accountBL.GetSaleForVoucherPosting(LocId, TransTypeId, WorkingDate);
            //lst = lst.Where(x => TransLst.Contains(x.TransId)).ToList();
            var result = await accountBL.PostExpenseVoucherPosting(TransLst, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ExpenseVoucherPosting_Destroy([DataSourceRequest] DataSourceRequest request, ExpenseTransactionVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await accountBL.DestroyExpenseVoucherPosting(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Other Cash Receive Voucher Posting
        public async Task<ActionResult> CashReceiveVoucherPosting()
        {
            ViewData["LocVD"] = await setupBL.LocationList();
            ViewData["AccVD"] = (await accountBL.SubCodeList()).Select(x => new { Id = x.Id, Code = x.Code, Name = x.Name }).ToList();
            ViewData["CCVD"] = await accountBL.CostCentersList();
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }

        public async Task<ActionResult> CashReceiveVoucherPosting_Read([DataSourceRequest] DataSourceRequest request, int Loc, DateTime Working)
        {
            var lst = await accountBL.CashReceivePostingList(Loc, Working);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CashReceiveVoucherPosting_Update([DataSourceRequest] DataSourceRequest request, CashReceiveVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await accountBL.UpdateCashReceiveVoucherPosting(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<JsonResult> CashReceiveVoucherPosting(List<long> TransLst)
        {
            //var lst = accountBL.GetSaleForVoucherPosting(LocId, TransTypeId, WorkingDate);
            //lst = lst.Where(x => TransLst.Contains(x.TransId)).ToList();
            var result = await accountBL.PostCashReceiveVoucherPosting(TransLst, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CashReceiveVoucherPosting_Destroy([DataSourceRequest] DataSourceRequest request, CashReceiveVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await accountBL.DestroyCashReceiveVoucherPosting(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Other Cash Payment Voucher Posting
        public async Task<ActionResult> CashPaymentVoucherPosting()
        {
            ViewData["LocVD"] = await setupBL.LocationList();
            ViewData["AccVD"] = (await accountBL.SubCodeList()).Select(x => new { Id = x.Id, Code = x.Code, Name = x.Name }).ToList();
            ViewData["CCVD"] = await accountBL.CostCentersList();
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }

        public async Task<ActionResult> CashPaymentVoucherPosting_Read([DataSourceRequest] DataSourceRequest request, int Loc, DateTime Working)
        {
            var lst = await accountBL.CashPaymentPostingList(Loc, Working);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CashPaymentVoucherPosting_Create([DataSourceRequest] DataSourceRequest request, CashPaymentVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await accountBL.CreateCashPaymentVoucherPosting(mod, UserId);
                if (tbl == null)
                {
                    ModelState.AddModelError("", "Server Error");
                }
                else
                {
                    mod.TransId = tbl.TransId;
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CashPaymentVoucherPosting_Update([DataSourceRequest] DataSourceRequest request, CashPaymentVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await accountBL.UpdateCashPaymentVoucherPosting(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<JsonResult> CashPaymentVoucherPosting(List<long> TransLst)
        {
            //var lst = accountBL.GetSaleForVoucherPosting(LocId, TransTypeId, WorkingDate);
            //lst = lst.Where(x => TransLst.Contains(x.TransId)).ToList();
            var result = await accountBL.PostCashPaymentVoucherPosting(TransLst, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CashPaymentVoucherPosting_Destroy([DataSourceRequest] DataSourceRequest request, CashPaymentVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await accountBL.DestroyCashPaymentVoucherPosting(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region CashTransfer
        public ActionResult CashTransfer()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> CashTransfer([Bind] CashTransferVM mod)
        {
            if (ModelState.IsValid)
            {
                var TransId = await accountBL.SaveCashTransfer(mod, UserInfo.UserId);
                if (TransId > 0)
                {
                    try
                    {
                        NotificationBL notificationBL = new NotificationBL();
                        NotificationHub objNotifHub = new NotificationHub();
                        string loca = (await setupBL.LocationList(mod.LocId))[0].LocName;
                        var notiLst = notificationBL.PostNotiLoc(6, mod.ToLocId, "Cash Transfer from " + loca, UserInfo.UserId);
                        foreach (var item in notiLst)
                        {
                            objNotifHub.SendNotification(item);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                mod.TransId = TransId;
            }
            return Json(mod, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CashReceive()
        {
            return View();
        }

        public async Task<ActionResult> CashReceive_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await accountBL.GetCashTransfer(LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CashReceive_Update([DataSourceRequest] DataSourceRequest request, CashTransferVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var tbl = await accountBL.SaveCashReceive(mod, UserInfo.UserId);
                if (!tbl)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region ChequeIssue

        public ActionResult ChequeIssue()
        {
            return View();
        }

        public async Task<JsonResult> CheuqeByAccId(long AccId)
        {
            var chequelst = await accountBL.GetChequeByAccId(AccId);

            return Json(chequelst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ChequeByAccTrans(long AccId, long TransId)
        {
            var chequelst = await accountBL.ChequeByAccTrans(AccId, TransId);
            return Json(chequelst, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Cheque_Add(BankBookTransVM mod)
        {
            if (ModelState.IsValid)
            {
                var status = await accountBL.AddCheque(mod);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult ChequeList()
        {
            ViewData["CStatus"] = SelectListVM.ChequeStatus;
            ViewData["CType"] = SelectListVM.ChequeType;
            return View();
        }

        public async Task<ActionResult> ChequeList_Read([DataSourceRequest] DataSourceRequest request, DateTime StartDate, DateTime EndDate)
        {
            var lst = await accountBL.GetChequeLists(StartDate, EndDate);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ChequeList_Update([DataSourceRequest] DataSourceRequest request, BankBookTransVM mod)
        {
            if (mod != null && mod.Status != null)
            {
                var status = await accountBL.UpdateChequeStatus(mod.TransID, mod.Status, UserInfo.UserId);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region BankStatement
        public async Task<ActionResult> BankStatement()
        {
            ViewData["SubCodeBankVD"] = (await accountBL.SubCodeBankList()).Select(x => new { Id = x.Id, SubCode = x.Code, Title = x.Name }).ToList();
            ViewData["AccountTypeSL"] = SelectListVM.BankAccountType;
            return View();
        }
        public async Task<JsonResult> GetBankDetail(long AccId, DateTime vrDate)
        {
            return Json(await accountBL.BankAccDetail(AccId, vrDate), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> BankStatement_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<FinBRSDetailVM> mod, long AccId, DateTime DocDate)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var tbl = await accountBL.ImportBankStatement(mod.ToList(), AccId, DocDate, UserInfo.UserId);
                if (tbl == false)
                    ModelState.AddModelError("", "Already Exist");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankStatement_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(new[] { "" }.ToDataSourceResult(request, ModelState));
        }
        #endregion
    }
}