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
    public class PurchaseController : Controller
    {
        PurchaseBL purchaseBL = new PurchaseBL();
        OrderBL orderBL = new OrderBL();
        SetupBL setupBL = new SetupBL();
        NotificationBL notificationBL = new NotificationBL();
        NotificationHub objNotifHub = new NotificationHub();

        #region GRN Mobile
        public async Task<ActionResult> GRNMobile()
        {
            ViewData["SKUVD"] = await setupBL.SKUListAll();
            return View();
        }
        #endregion
        #region GRN
        public ActionResult Index()
        {
            List<UserMenuInfo> menuList = new SecurityBL().GetMenuList(UserInfo.UserId, UserInfo.GroupId);
            return View(menuList);
        }
        public async Task<ActionResult> GRN()
        {
            ViewData["SKUVD"] = await setupBL.SKUListAll();
            return View();
        }
        public async Task<JsonResult> SKUByBarcode(string Barcode)
        {
            var lst = await purchaseBL.GetSKUByBarcode(Barcode);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SerialFromMobile(int LocId, long POId)
        {
            var lst = await purchaseBL.GetSerialFromMobile(LocId, POId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> POForGRNList(int LocId)
        {
            var lst = (await purchaseBL.GetPOForGRN(LocId)).Select(x => new { POId = x.POId, PONo = x.PONo, SuppId = x.SuppId, SuppName = x.Inv_Suppliers.SuppName }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> POMobileForGRNList(int LocId)
        {
            var lst = (await purchaseBL.GetPOForGRN(LocId, true)).Select(x => new { POId = x.POId, PONo = x.PONo, SuppId = x.SuppId, SuppName = x.Inv_Suppliers.SuppName }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GRNBySupp(int SuppId)
        {
            var lst = (await purchaseBL.GetGRNBySupp(SuppId)).Select(x => new { GRNId = x.GRNId, GRNNo = x.GRNNo }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> POBySupp(int SuppId)
        {
            var lst = (await purchaseBL.GetPOBySupp(SuppId)).Select(x => new { POId = x.POId, PONo = x.PONo }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetOrder(long OrderNo)
        {
            var lst = await orderBL.GetOrder(OrderNo);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GRN_Read([DataSourceRequest] DataSourceRequest request, long OrderNo, int LocId)
        {
            var lst = await purchaseBL.OrderDetailForGRN(OrderNo, LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> GRN_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PurchaseDetailVM> mod,
            int LocId, int POId, int SuppId, string InvNo, DateTime InvDate,
            string DONo, DateTime DODate, List<long> files)
        {
            if (mod != null && ModelState.IsValid)
            {
                var GRNId = await purchaseBL.SaveGRN(mod, LocId, POId, SuppId, InvNo, InvDate, DONo, DODate, UserInfo.UserId);
                if (GRNId > 0)
                {
                    var upDoc = await new DocumentBL().UpdateDocRef(files, GRNId);
                    ModelState.AddModelError("Msg", GRNId.ToString());
                }
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> IsSrNoExist(string SrNo, int SKUId)
        {
            var lst = await purchaseBL.IsSrNoExist(SrNo, SKUId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region POInvoice New
        public async Task<ActionResult> POI()
        {
            ViewBag.InvoiceRights = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.InvoiceRights);
            return View();
        }
        public async Task<ActionResult> POI_Read([DataSourceRequest] DataSourceRequest request, DateTime fDate, DateTime tDate, int SuppId, int CityId, int LocId, int SuppCatId, string Status)
        {
            var lst = await purchaseBL.GRNForInv(fDate, tDate, SuppId, CityId, LocId, SuppCatId, Status);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GRNById(long GRNId)
        {
            var grn = await purchaseBL.GetGRNbyId(GRNId);
            var loc = await setupBL.LocationList(grn.LocId);
            var data = new { grn.Inv_PO.PONo, grn.Inv_PO.PODate, grn.DONo, grn.DODate, loc[0].LocName };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetWHT(int SuppId, decimal MRP, decimal TP, decimal GST, decimal Discount)
        {
            var data = await orderBL.GetWHT(SuppId, MRP, TP, GST, Discount);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> POInv()
        {
            ViewBag.GRNId = "";
            ViewBag.SuppId = "";
            return View();
        }
        [ActionName("POInvG")]
        public async Task<ActionResult> POInv(string GRNId)
        {
            var grn = await purchaseBL.GetGRNbyId(Convert.ToInt64(GRNId));
            if (grn.Status == "G")
            {
                ViewBag.GRNId = GRNId;
                ViewBag.SuppId = grn.SuppId.ToString();
            }
            else
            {
                ViewBag.GRNId = "";
                ViewBag.SuppId = "";
            }
            return View("POInv");
        }
        public async Task<ActionResult> POInv_Read([DataSourceRequest] DataSourceRequest request, long GRNId)
        {
            var lst = await purchaseBL.GRNForInv(GRNId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> POInv_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<POInvoiceDetailVM> mod,
            int GRNId, string DONo, DateTime DODate, int SuppId, long RefInvNo, DateTime RefInvDate, string Remarks)
        {
            if (mod != null && ModelState.IsValid)
            {
                var InvId = await purchaseBL.SavePOInv(mod, GRNId, DONo, DODate, SuppId, RefInvNo, RefInvDate, Remarks, UserInfo.LocId, UserInfo.UserId);
                if (InvId > 0)
                    ModelState.AddModelError("Msg", InvId.ToString());
                else
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region POInvoice
        public ActionResult POInvoice()
        {
            return View();
        }
        public async Task<ActionResult> POInvoice_Read([DataSourceRequest] DataSourceRequest request, long POId)
        {
            var lst = await purchaseBL.POForInv(POId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> POInvoice_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<POInvoiceDetailVM> mod,
            int POId, int SuppId, int RefInvNo, DateTime RefInvDate, string Remarks)
        {
            if (mod != null && ModelState.IsValid)
            {
                var InvId = await purchaseBL.SavePOInvoice(mod, POId, SuppId, RefInvNo, RefInvDate, Remarks, UserInfo.LocId, UserInfo.UserId);
                if (InvId > 0)
                    ModelState.AddModelError("Msg", InvId.ToString());
                else
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region InvoicePayment
        public async Task<ActionResult> InvoicePayment()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        public async Task<ActionResult> InvoicePayment_Read([DataSourceRequest] DataSourceRequest request, DateTime fDate, DateTime tDate, int SuppId, int LocId, int SuppCatId, string Status)
        {
            var lst = await purchaseBL.InvForPayment(fDate, tDate, SuppId, LocId, SuppCatId, Status);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> InvoicePayment_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<InvForPaymentVM> mod,
            int SuppId)
        {
            if (mod != null && ModelState.IsValid)
            {
                var InvId = await purchaseBL.SaveSupplierPayment(mod, SuppId, UserInfo.UserId);
                if (InvId)
                    ModelState.AddModelError("Msg", "Save Successfully");
                else
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> GetSuppBalance(long AccId, int SuppId)
        {
            var data = await purchaseBL.GetSuppBalance(AccId, SuppId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Payments
        public async Task<ActionResult> Payments()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            var lst = await new AccountBL().SubCodeList();
            var suppArr = new long[] { 20010100010, 20011000010, 20012000010, 20013000010, 24010100010, 20100100010 };
            ViewBag.COA4 = lst.Where(x => suppArr.Contains(x.Id)).Select(x => new { Name = x.Name, Code = x.Code, Id = Convert.ToInt64(x.Code.Replace("-", "")) }).ToList();
            ViewData["LocVD"] = await new CashBL().GetCashCollectionCenterList(UserInfo.LocId);

            var bankArr = new long[] { 46010100040, 46010100070 };
            var bankLst = await new AccountBL().SubCodeBankList();
            bankLst = bankLst.Where(x => bankArr.Contains(x.Id)).ToList();
            bankLst.Insert(0, new COAVM { Id = 0, Name = "NA" });
            ViewData["Bank"] = bankLst;
            return View();
        }
        public async Task<ActionResult> Payments_Read([DataSourceRequest] DataSourceRequest request, long AccId)
        {
            var lst = purchaseBL.GetPaymentAdvice(AccId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Payments_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SuppPaymentVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await purchaseBL.SavePaymentAdvice(mod, UserInfo.UserId);
                if (!IsSave)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region SupplierPayment
        public async Task<ActionResult> SupplierPayment()
        {
            //ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            var bankArr = new long[] { 46010100040, 46010100070 };
            var bankLst = await new AccountBL().SubCodeBankList();
            bankLst = bankLst.Where(x => bankArr.Contains(x.Id)).ToList();
            bankLst.Insert(0, new COAVM { Id = 0, Name = "NA" });
            ViewData["Bank"] = bankLst;
            return View();
        }
        public async Task<ActionResult> SupplierPayment_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await purchaseBL.SupplierPaymentList(LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SupplierPayment_Create([DataSourceRequest] DataSourceRequest request, SuppPaymentVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var InvId = await purchaseBL.SavePaySupplierPayment(mod, UserInfo.UserId);
                if (InvId == 0)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region SupplierPaymentVerification
        public async Task<ActionResult> SupplierPaymentVerification()
        {
            ViewData["LocVD"] = await new CashBL().GetCashCollectionCenterList(UserInfo.LocId);
            return View();
        }
        public async Task<ActionResult> SupplierPaymentVerification_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await purchaseBL.SupplierPaymentVerificationList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SupplierPaymentVerification_Update([DataSourceRequest] DataSourceRequest request, SuppPaymentVM mod,
            int SuppId)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await purchaseBL.EditPaySupplierPayment(mod, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod = tbl;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SupplierPaymentVerification_Destroy([DataSourceRequest] DataSourceRequest request, SuppPaymentVM mod,
            int SuppId)
        {
            if (mod != null && ModelState.IsValid)
            {
                var InvId = await purchaseBL.DeletePaySupplierPayment(mod, UserInfo.UserId);
                if (InvId == 0)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public async Task<JsonResult> SupplierPaymentVoucherPosting(List<long> TransLst)
        {
            var result = await purchaseBL.PostSupplierPayment(TransLst, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region PurchasePayment
        public ActionResult PurchasePayment()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PurchasePayment(POPaymentVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var TransId = await purchaseBL.POPayment(mod, UserInfo.UserId);
                if (TransId > 0)
                {
                    mod.TransId = TransId;
                }
            }
            return Json(mod, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> InvoiceList(int LocId)
        {
            var lst = await purchaseBL.GetPOInvoiceList(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetInvDetails(long POInvId)
        {
            var lst = await purchaseBL.GetInvDetail(POInvId);
            var data = new { lst.Payment, lst.PreBalance };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region PurchseReturn
        public ActionResult PurchaseReturn()
        {
            return View();
        }
        public async Task<JsonResult> GetPOReturnData(long ItemId)
        {
            var UserObj = await new PurchaseBL().GetPOReturnData(ItemId);
            return Json(UserObj, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> PurchaseReturn_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<POReturnDtlVM> mod,
            int LocId, int SuppId, string MRemarks, string ReturnType, int ReasonId, List<long> files)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await purchaseBL.SavePurchaseReturn(mod, LocId, SuppId, MRemarks, ReturnType, ReasonId, files, UserInfo.UserId);
                if (IsSave)
                    ModelState.AddModelError("Msg", "Saved Successfully.");
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region PurchaseApproval
        public ActionResult PurchaseValidation()
        {
            ViewBag.Level = "V";
            return View("PurchaseApproval");
        }
        public ActionResult PurchaseApproval()
        {
            ViewBag.Level = "A";
            return View();
        }
        public async Task<ActionResult> PurchaseApproval_Read([DataSourceRequest] DataSourceRequest request, string Level)
        {
            var UserId = UserInfo.UserId;
            var lst = await purchaseBL.GetPurchaseApproval(Level, UserId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> PurchaseApproval_Destroy([DataSourceRequest] DataSourceRequest request, OrderSearchVM mod, string Level)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await purchaseBL.PurchaseApproval(mod, Level, UserInfo.UserId);
                if (!IsSave)
                    ModelState.AddModelError("", "Server Error");
                else
                    ModelState.AddModelError("Msg", "Saved Successfully");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region BikeLetter
        public ActionResult BikeLetter()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }

        public async Task<ActionResult> BikeLetter_Read([DataSourceRequest] DataSourceRequest request, DateTime FromDate, DateTime ToDate, int SuppId, int TypeId, string SerialNo, string DONo, int LetterStatus)
        {
            var lst = await purchaseBL.GetBikes(FromDate, ToDate, SuppId, TypeId, SerialNo, DONo, LetterStatus);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> BikeLetter_Update(string SrNo, string LetterNumber, int EnvelopNo)
        {
            var result = await purchaseBL.UpdateBikeLetter(SrNo, LetterNumber, EnvelopNo, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult BikeLetterSale()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }

        public async Task<ActionResult> BikeLetterSale_Read([DataSourceRequest] DataSourceRequest request, DateTime FromDate, DateTime ToDate, int CityId, int LocId, int Type_Id, int SKU_Id, int Supp_Id, string Sale_Type, string Serial_No, string Bill_No)
        {
            var lst = await purchaseBL.GetBikesSale(FromDate, ToDate, CityId, LocId, Type_Id, SKU_Id, Supp_Id, Sale_Type, Serial_No, Bill_No);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> BikeLetterSale_Update([DataSourceRequest] DataSourceRequest request, BikeLetterSaleVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var msg = await purchaseBL.UpdateBikesSale(mod, UserInfo.UserId);
                if (msg == "Success")
                {
                    ModelState.AddModelError("Msg", "Success");
                }
                else
                {
                    ModelState.AddModelError("", msg);
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult BikeLetterOld()
        {
            ViewData["StatusVD"] = new List<SelectListItem> {
                new SelectListItem{Text = "NONE",Value="NONE"},
                new SelectListItem{Text = "Yes",Value="Yes"},
                new SelectListItem{Text = "No",Value="No"}
            };
            return View();
        }

        public async Task<ActionResult> BikeLetterOld_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await purchaseBL.GetBikesOld();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> BikeLetterOld_Update([DataSourceRequest] DataSourceRequest request, BikeLetterOldVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var msg = await purchaseBL.UpdateBikeLetterOld(mod, UserInfo.UserId);
                if (msg == "Success")
                {
                    ModelState.AddModelError("Msg", "Success");
                }
                else
                {
                    ModelState.AddModelError("", msg);
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult BikeLetterSaleOld()
        {
            ViewData["StatusVD"] = new List<SelectListItem> {
                new SelectListItem{Text = "NONE",Value="NONE"},
                new SelectListItem{Text = "Yes",Value="Yes"},
                new SelectListItem{Text = "No",Value="No"}
            };
            return View();
        }

        public async Task<ActionResult> BikeLetterSaleOld_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await purchaseBL.GetBikesRegOld();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> BikeLetterSaleOld_Update([DataSourceRequest] DataSourceRequest request, BikeRegOldVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var msg = await purchaseBL.UpdateBikeRegOld(mod, UserInfo.UserId);
                if (msg == "Success")
                {
                    ModelState.AddModelError("Msg", "Success");
                }
                else
                {
                    ModelState.AddModelError("", msg);
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion


    }
}