using AGEERP.CrReports;
using AGEERP.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NotifSystem.Web.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AGEERP.Controllers
{
    [RBAC]
    public class StockController : Controller
    {
        StockBL stockBL = new StockBL();
        SetupBL setupBL = new SetupBL();
        NotificationBL notificationBL = new NotificationBL();
        NotificationHub objNotifHub = new NotificationHub();

        public ActionResult Index()
        {
            List<UserMenuInfo> menuList = new SecurityBL().GetMenuList(UserInfo.UserId, UserInfo.GroupId);
            return View(menuList);
        }

        #region InventoryManager
        public async Task<ActionResult> InventoryManager()
        {
            if (UserInfo.GroupId == 2)
            {
                var lst = await stockBL.StatusList();
                ViewData["StatusVD"] = lst.Where(x => x.StatusID == 1 || x.StatusID == 7).ToList();
            }
            else
            {
                ViewData["StatusVD"] = await stockBL.StatusList();
            }
            return View();
        }
        public async Task<ActionResult> StockStatus_Read([DataSourceRequest] DataSourceRequest request, int CompanyId, int ProductId, int ModelId, int StatusId, int LocationId, string Serial)
        {
            var lst = await stockBL.GetItemStock(CompanyId, ProductId, ModelId, StatusId, LocationId, Serial);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> StockStatus_ReadRow(int LocId, string SrNo)
        {
            var lst = await stockBL.GetItemStockRow(LocId, SrNo);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> StockStatus_Update([DataSourceRequest] DataSourceRequest request, Inv_StoreVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                if (mod.StatusID == 5 && string.IsNullOrEmpty(mod.Remarks))
                {
                    ModelState.AddModelError("", "Please enter remarks");
                }
                else
                {
                    var IsSave = await stockBL.UpdateStockStatus(mod, UserInfo.GroupId, UserId);
                    if (!IsSave)
                    {
                        ModelState.AddModelError("", "Server Error");
                    }
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> StatusList()
        {
            var lst = await stockBL.StatusList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> DamageStatusList()
        {
            var lst = await stockBL.DamageStatusList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Mobile Opening Stock
        public async Task<ActionResult> StockDemand()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MobileStockOpening_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<Inv_StoreVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await stockBL.MobileStockOpeningCreate(mod, UserInfo.UserId);
                if (IsSave)
                {
                    try
                    {
                        string loca = (await setupBL.LocationList(mod.FirstOrDefault().LocId))[0].LocName;
                        var notiLst = notificationBL.PostNotiLoc(5, 0, "New Stock Demand from " + loca, UserInfo.UserId);
                        foreach (var item in notiLst)
                        {
                            objNotifHub.SendNotification(item);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    ModelState.AddModelError("Msg", "Saved Successfully.");
                }
                else
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<ActionResult> EditStockOpening()
        {
            ViewData["SuppVD"] = await setupBL.SupplierList();
            ViewData["SKUVD"] = await setupBL.SKUListAll();
            return View();
        }
        public async Task<ActionResult> StockOpening_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await stockBL.GetStockOpening(LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> StockOpening_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<Inv_OpeningStockVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await stockBL.UpdateStockOpening(mod, UserId);
                if (!IsSave)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public async Task<JsonResult> PostOpeningStock(List<long> TransLst)
        {
            var lst = await stockBL.PostOpeningStock(TransLst, UserInfo.UserId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> IsSrNoExist(string SrNo)
        {
            var lst = await stockBL.IsSrNoExist(SrNo);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ItemHistory
        public async Task<ActionResult> ItemHistory()
        {
            return View();
        }
        public async Task<ActionResult> ItemHistory_Read([DataSourceRequest] DataSourceRequest request, int ItemId)
        {
            var lst = stockBL.GetItemHistory(ItemId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SerialList()
        {
            var lst = await stockBL.SerialNumbList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemInfo(string Serial)
        {
            var obj = await stockBL.ItemDetails(Serial);
            var data = new
            {
                obj.ItemId,
                obj.Itm_Master.SKUCode,
                obj.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                obj.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                obj.Inv_Suppliers.SuppName
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SerialInfo(string Serial)
        {
            var obj = await stockBL.ItemDetails(Serial);
            var data = new
            {
                obj.ItemId,
                obj.Itm_Master.SKUCode,
                obj.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                obj.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                obj.PPrice,
                obj.Inv_Status.MFact,
                obj.Inv_Status.StatusTitle
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region StockAtBranch
        public ActionResult StockAtBranch()
        {
            return View();
        }
        public ActionResult StockAtBranch_Read([DataSourceRequest] DataSourceRequest request, int LocId, int ModelId, int SKUId)
        {
            var lst = stockBL.GetStockAtBranch(LocId, ModelId, SKUId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SKUUpdate

        public async Task<ActionResult> SKUUpdate()
        {
            ViewData["SKUVD"] = await setupBL.SKUListAll();
            return View();
        }
        public async Task<ActionResult> SKUUpdate_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await stockBL.GetSKUUpdateList(LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SKUUpdate_Update([DataSourceRequest] DataSourceRequest request, ItemDetailVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await stockBL.UpdateSKU(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        #endregion
        #region StockManagement
        public ActionResult StockIssuePrint()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> StockIssuePrint(FormCollection frm)
        {
            var trnid = Convert.ToInt32(frm["IssueNo"]);
            var locId = Convert.ToInt32(frm["LocId"]);
            if (locId == 95)
            {
                var mod = await new StockBL().GetIssueDetail(trnid);
                SerialIssuePrint rpt = new SerialIssuePrint();
                rpt.SetDataSource(mod);
                rpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                rpt.Close();
                rpt.Dispose();
            }
            return View();
        }
        public ActionResult StockAdjustment()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }

        public async Task<JsonResult> AdjStype()
        {
            var lst = (await stockBL.GetAdjType()).Select(x => new
            {
                AdjTypeId = x.AdjTypeId,
                AdjType = x.AdjType
            }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> StockAdjustment_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<IssueDetailVM> mod,
            int FLocId, int AdjTypeId, string Remarks)
        {
            if (mod != null && ModelState.IsValid)
            {
                var issueNo = await stockBL.SaveStockAdjusment(mod, FLocId, AdjTypeId, Remarks, UserInfo.UserId);
                if (issueNo > 0)
                {
                    ModelState.AddModelError("Msg", "Save Successfully");
                }
                else
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult StockIssue()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> StockIssue_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<IssueDetailVM> mod,
            int FLocId, int TLocId, DateTime IssueDate, string Remarks)
        {
            if (mod != null && ModelState.IsValid)
            {
                var issueNo = await stockBL.SaveStockIssue(mod, FLocId, TLocId, IssueDate, Remarks, UserInfo.UserId);
                if (issueNo > 0)
                {
                    try
                    {
                        var notiLst = notificationBL.PostNotiLoc(3, TLocId, "Stock Issued. IssueNo " + issueNo, UserInfo.UserId);
                        foreach (var item in notiLst)
                        {
                            objNotifHub.SendNotification(item);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    ModelState.AddModelError("Msg", issueNo.ToString());
                }
                else
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult StockOpening()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> StockOpening_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<Inv_StoreVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await stockBL.StockOpeningCreate(mod, UserInfo.UserId);
                if (IsSave)
                    ModelState.AddModelError("Msg", "Saved Successfully.");
                else
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult StockReceive()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            ViewData["StatusVD"] = new List<SelectListItem>(){
                new SelectListItem { Text = "Not Received",Value = "I"},
                new SelectListItem { Text = "Received",Value = "R"}
            };
            return View();
        }
        public async Task<ActionResult> StockReceive_Read([DataSourceRequest] DataSourceRequest request, int IssueNo, int LocId)
        {
            var lst = await stockBL.GetStockIssue(IssueNo, LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> StockReceive_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<IssueDetailVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var usr = new UsersInfoVM { LocId = UserInfo.LocId, UserId = UserInfo.UserId };
                var issueNo = await stockBL.SaveStockReceive(mod.ToList(), usr);
                if (issueNo > 0)
                {
                    ModelState.AddModelError("Msg", issueNo.ToString());
                }
                else
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> IssueNoForReceive(int LocId)
        {
            var lst = await stockBL.GetIssueNoForReceive(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        #endregion
        public ActionResult DasboardIBStockTransfer()
        {
            return View();
        }
        public async Task<ActionResult> DashboardIBStockTransfer_read([DataSourceRequest] DataSourceRequest request, DateTime FromDate, DateTime ToDate, int LocId, string Sta)
        {
            if (!String.IsNullOrWhiteSpace(FromDate.ToString()) && !String.IsNullOrWhiteSpace(FromDate.ToString()))
            {
                var lst = await stockBL.DashboardIBStockTransfer(FromDate, ToDate.AddDays(1), LocId, Sta);
                return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }

        }

        public ActionResult _LocTag(string LocCode)
        {
            return PartialView(LocCode);
        }

    }
}
