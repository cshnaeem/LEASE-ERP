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
    public class OrderController : Controller
    {
        OrderBL orderBL = new OrderBL();
        SetupBL setupBL = new SetupBL();
        NotificationBL notificationBL = new NotificationBL();
        NotificationHub objNotifHub = new NotificationHub();


        #region Dashboard

        public ActionResult OrderManagerDashboard()
        {
            return View();
        }

        public ActionResult OrderManagerDashboard_Read([DataSourceRequest] DataSourceRequest request, DateTime FromDate, DateTime ToDate, int RegionId, int POStatusId, int CategoryId, int CompanyId, int SupplierId, int ProductId)
        {
            var lst = orderBL.GetOrderManagerDasboard(FromDate, ToDate, RegionId, POStatusId, CategoryId, CompanyId, SupplierId, ProductId).ToList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> PoStatusUpdate(int PoId, int PoStatus)
        {
            var lst = await orderBL.UpdatePOStatus(PoId, PoStatus, UserInfo.UserId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region OrderPlanCity
        public async Task<ActionResult> OrderPlanCity()
        {
            var CityList = await setupBL.CityList();
            return View(CityList);
        }
        public async Task<ActionResult> OrderPlanCity_Read([DataSourceRequest] DataSourceRequest request, int ComId, int ProductId, int[] ModelLst)
        {
            var lst = await orderBL.GetPOPlanCityList(ComId, ProductId, ModelLst.ToList());
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> OrderPlanCity_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<POPlanCityDetailVM> mod, int ComId, string Remarks)
        {
            if (mod != null && ModelState.IsValid)
            {
                var TransId = await orderBL.CreateOrderPlanCity(mod, ComId, Remarks, UserInfo.UserId);
                if (TransId > 0)
                    ModelState.AddModelError("Msg", TransId.ToString());
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region OrderPlan
        public ActionResult OrderPlan()
        {
            return View();
        }
        public async Task<ActionResult> OrderPlan_Read([DataSourceRequest] DataSourceRequest request, int[] ModelLst, int[] CityLst, DateTime FromDate, DateTime ToDate)
        {
            var lst = orderBL.GetModelSaleStock(ModelLst, CityLst, FromDate, ToDate);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> OrderPlan_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<POPlanDetailVM> mod, int[] CityLst, DateTime FromDate, DateTime ToDate, int SuppId, string Remarks)
        {
            if (mod != null && ModelState.IsValid)
            {
                var TransId = await orderBL.CreateOrderPlan(mod, CityLst, SuppId, Remarks, FromDate, ToDate, UserInfo.UserId);
                if (TransId > 0)
                    ModelState.AddModelError("Msg", "Saved Successfully. PO No. " + TransId);
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region OrderPlanMobile
        public async Task<ActionResult> PreOrderMobile()
        {
            return View();
        }
        public async Task<PartialViewResult> _PreOrderMobile(int[] ModelLst)
        {
            ModelLst = ModelLst.OrderBy(x => x).ToArray();
            var lst = await orderBL.GetModelList(ModelLst);
            return PartialView(lst);
        }
        public async Task<ActionResult> PreOrderMobile_Read([DataSourceRequest] DataSourceRequest request, int[] ModelLst, DateTime FromDate, DateTime ToDate, string LocType)
        {
            ModelLst = ModelLst.OrderBy(x => x).ToArray();
            var lst = await orderBL.GetModelSaleStockMobile(ModelLst, FromDate, ToDate, LocType);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> PreOrderMobile_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PreOrderMobileVM> mod, DateTime FromDate, DateTime ToDate, int SuppId, string Remarks)
        {
            if (mod != null && ModelState.IsValid)
            {
                var TransId = await orderBL.SavePreOrderMobile(mod, SuppId, Remarks, FromDate, ToDate, UserInfo.UserId);
                if (TransId > 0)
                    ModelState.AddModelError("Msg", "Saved Successfully. PO Plan No. " + TransId);
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region PurchaseOrder
        public async Task<ActionResult> PurchaseOrder()
        {
            ViewBag.PolicySL = SelectListVM.PolicySL;
            return View();
        }
        public async Task<JsonResult> PaymentTermsList()
        {
            return Json(orderBL.PaymentTermsList(), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> PoTypeList()
        {
            return Json(await orderBL.POTypeList(), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> POList(int POTypeId)
        {
            var ord = (await orderBL.POList(POTypeId)).Select(x => new { x.POId, x.PONo, PODate = x.PODate.ToString("dd-MM-yyyy"), SuppName = x.Inv_Suppliers.SuppName }).OrderByDescending(x => x.POId).ToList();
            return Json(ord, JsonRequestBehavior.AllowGet);
        }
        //public async Task<JsonResult> GetPO(long POId)
        //{
        //    var ord = await orderBL.GetPO(POId);
        //    var data = new
        //    {
        //        ord.POId,
        //        ord.PONo,
        //        ord.SuppId,
        //        ord.Remarks,
        //        ord.POTypeId,
        //        ord.PolicyType,
        //        ord.PODate,
        //        ord.PlanId,
        //        ord.PaymentTerm,
        //        ord.Discount,
        //        ord.DeliveryDate,
        //        CityId = ord.Inv_POMapping.Select(x => x.CityId).ToArray(),
        //        lst = ord.Inv_PODetail.Select(x => new PODetailVM
        //        {
        //            SKUId = x.SKUId,
        //            Qty = x.Qty,
        //            Discount = x.Discount,
        //            GST = x.GST,
        //            TP = x.TP,
        //            MRP = x.MRP,
        //            WHT = x.WHT,
        //            SKU = x.Itm_Master.SKUName,
        //            PODtlId = x.PODtlId,
        //            NetPrice = x.GST + x.WHT + x.TP - x.Discount,
        //            Amount = (x.GST + x.WHT + x.TP - x.Discount) * x.Qty,
        //            Model = x.Itm_Master.Itm_Model.Model,
        //            ModelId = x.Itm_Master.ModelId
        //        })
        //    };
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        public async Task<JsonResult> GetPlanCityList()
        {
            var lst = await orderBL.GetPlanCityList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetPlanList(int SuppId)
        {
            var lst = await orderBL.GetPlanList(SuppId);
            return Json(lst.Select(x => new { PlanId = x }), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SupplierByPlanList(int PlanId)
        {
            var lst = await orderBL.SupplierByPlanList(PlanId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> AddToOrder(int SKUId, int SuppId, int Qty, int CityId)
        {
            var data = await orderBL.AddToOrder(SKUId, SuppId, Qty, CityId, 1);
            data.NetPrice = data.TP - data.Discount;
            data.Amount = (data.TP - data.Discount) * data.Qty;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> AddToOrderMobile(int SKUId, int SuppId, int Qty, int LocId)
        {
            var data = await orderBL.AddToOrder(SKUId, SuppId, Qty, LocId, 2);
            data.NetPrice = data.TP - data.Discount;
            data.Amount = (data.TP - data.Discount) * data.Qty;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ApplyDiscountPolicy(IEnumerable<PODetailVM> dat)
        {
            return Json(await orderBL.ApplyDiscountPolicy(dat), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> PurchaseOrderPolicy_Read([DataSourceRequest] DataSourceRequest request, IEnumerable<PODetailVM> dat)
        {
            var lst = await orderBL.ApplyPairPolicy(dat);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> PurchaseOrder_Read([DataSourceRequest] DataSourceRequest request, int PlanId, int SuppId)
        {
            var lst = await orderBL.GetPOPlan(PlanId, SuppId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> PurchaseOrder_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PODetailVM> mod, int[] CityId, int SuppId, DateTime DueDate, string Remarks, int PaymentTerm, int PlanId, string PolicyType)
        {
            if (mod != null)
            {
                var TransId = await orderBL.CreatePurchaseOrder(mod, CityId, SuppId, DueDate, Remarks, PaymentTerm, PlanId, PolicyType, UserInfo.UserId);
                if (TransId > 0)
                {
                    try
                    {
                        var ord = await orderBL.GetPOById(TransId);
                        var notiLst = notificationBL.PostNotiLoc(12, 0, "New " + ord.PONo + " Supplier " + ord.Inv_Suppliers.SuppName, UserInfo.UserId);
                        foreach (var item in notiLst)
                        {
                            objNotifHub.SendNotification(item);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    ModelState.AddModelError("Msg", TransId.ToString());
                }
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(mod.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> PurchaseOrderEdit(long POId)
        {
            var po = await orderBL.GetPOById(POId);
            if (po != null)
            {
                ViewBag.POId = po.POId;
                ViewBag.SuppId = po.SuppId.ToString();
                ViewBag.PONo = po.PONo;
                ViewData["CityVD"] = await setupBL.CityList();
            }
            ViewBag.PolicySL = SelectListVM.PolicySL;
            return View();
        }
        public async Task<ActionResult> PurchaseOrderEdit_Read([DataSourceRequest] DataSourceRequest request, long POId)
        {
            var lst = await orderBL.GetPODetailById(POId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> PurchaseOrderEdit_Edit([DataSourceRequest] DataSourceRequest request, PODetailVM LstPoDtl)
        {
            var lst = await orderBL.UpdatePO(LstPoDtl);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> PurchaseOrderMobile()
        {
            return View();
        }
        public async Task<ActionResult> PurchaseOrderMobileEdit(int PlanId)
        {
            if (PlanId > 0)
            {
                var plan = await orderBL.GetPOPlanMobileById(PlanId);
                if (plan == null)
                {
                    return RedirectToAction("PurchaseOrderMobile");
                }
                else
                {
                    ViewBag.SuppId = plan.SuppId;
                    ViewBag.PoId = PlanId;
                    return View();

                }

            }
            else
            {
                return RedirectToAction("PurchaseOrderMobile");
            }
        }
        public async Task<ActionResult> PurchaseOrderMobile_Read([DataSourceRequest] DataSourceRequest request, int PlanId)
        {
            var lst = await orderBL.GetPOPlanMobile(PlanId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> PurchaseOrderMobileEdit_Read([DataSourceRequest] DataSourceRequest request, int PoId)
        {
            var lst = await orderBL.GetPOEdit(PoId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> PurchaseOrderMobile_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PODetailVM> mod, int SuppId, DateTime DueDate, string Remarks, int PaymentTerm, int PlanId)
        {
            if (mod != null)
            {
                var TransId = await orderBL.CreatePurchaseOrderMobile(mod, SuppId, DueDate, Remarks, PaymentTerm, PlanId, UserInfo.UserId);
                if (TransId > 0)
                    ModelState.AddModelError("Msg", TransId.ToString());
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> PurchaseOrderMobile_Edit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PODetailVM> mod, int SuppId, DateTime DueDate, string Remarks, int PaymentTerm, int PlanId)
        {
            if (mod != null)
            {
                var TransId = await orderBL.EditPurchaseOrderMobile(mod, SuppId, DueDate, Remarks, PaymentTerm, PlanId, UserInfo.UserId);
                if (TransId)
                    ModelState.AddModelError("Msg", TransId.ToString());
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region OrderSchedule

        public async Task<ActionResult> OrderSchedule()
        {
            ViewBag.POId = "";
            ViewData["LocVD"] = await setupBL.LocationList();
            return View();
        }
        public async Task<ActionResult> OrderSchedules(string id)
        {
            ViewBag.POId = id;
            ViewData["LocVD"] = await setupBL.LocationList();
            return View("OrderSchedule");
        }

        public async Task<JsonResult> CityByPO(long POId)
        {
            var lst = await orderBL.GetCityByPOList(POId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SKUByPOCity(long POId, int CityId)
        {
            var lst = await orderBL.SKUByPOCity(POId, CityId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetPOSummary(long POId)
        {
            var lst = await orderBL.GetPOSummary(POId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetSchedule(long POId)
        {
            var lst = await orderBL.GetSchedule(POId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> OrderSchedule_Read([DataSourceRequest] DataSourceRequest request, long SchMasterId)
        {
            var lst = await orderBL.GetScheduleDetail(SchMasterId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> OrderSchedule_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<OrderScheduleVM> mod, long SchMasterId, DateTime DeliveryDate)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await orderBL.CreateOrderSchedule(mod, SchMasterId, DeliveryDate, UserInfo.UserId);
                long TransId = 0;
                if (long.TryParse(IsSave, out TransId))
                {
                    ModelState.AddModelError("Msg", "Saved Successfully");
                    ModelState.AddModelError("TransId", TransId.ToString());
                }
                else
                    ModelState.AddModelError("", IsSave);
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        //public async Task<ActionResult> OrderSchedule(int PODtlId)
        //{
        //    var mod = await orderBL.GetOrderDetailRow(PODtlId);
        //    if (mod != null)
        //    {
        //        ViewBag.SKU = mod.SKU;
        //        ViewBag.Model = mod.Model;
        //        ViewBag.OrderQty = mod.Qty.ToString();
        //    }
        //    ViewBag.PODtlId = PODtlId;
        //    return PartialView("_OrderSchedule");
        //}
        //public async Task<ActionResult> OrderSchedule_Read([DataSourceRequest] DataSourceRequest request, int PODtlId, DateTime FromDate, DateTime ToDate)
        //{
        //    var lst = orderBL.GetSaleStockForSchedule(PODtlId, FromDate, ToDate);
        //    return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}
        //[AcceptVerbs(HttpVerbs.Post)]
        //public async Task<ActionResult> OrderSchedule_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<OrderScheduleVM> mod)
        //{
        //    if (mod != null && ModelState.IsValid)
        //    {
        //        var IsSave = await orderBL.CreateOrderSchedule(mod, UserInfo.UserId);
        //        if (IsSave)
        //        {
        //            ModelState.AddModelError("Msg", "Saved Successfully.");
        //        }
        //        else
        //            ModelState.AddModelError("", "Server Error");
        //    }

        //    return Json(mod.ToDataSourceResult(request, ModelState));
        //}
        #endregion
        #region OrdePlanSearch 
        public ActionResult OrderPlanSearch()
        {
            return View();
        }
        public async Task<ActionResult> OrderPlanSearch_Read([DataSourceRequest] DataSourceRequest request, DateTime FromDate, DateTime ToDate)
        {
            var lst = orderBL.OrderPlanSearch(FromDate, ToDate);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region OrderSearch
        public async Task<ActionResult> OrderSearch()
        {
            SecurityBL securityBL = new SecurityBL();
            ViewBag.IsEditRight = await securityBL.HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.IsOrderEditRight);
            ViewBag.IsScheduleRight = await securityBL.HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.OrderSchedule);
            return View();
        }
        public ActionResult OrderSearch_Read([DataSourceRequest] DataSourceRequest request, DateTime FromDate, DateTime ToDate, int Status, int SuppId, string PONo, string POInvNo)
        {
            var lst = orderBL.OrderSearch(FromDate, ToDate, Status, SuppId, PONo, POInvNo);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> POStatusList()
        {
            var mod = await orderBL.POStatusList();
            return Json(mod, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> OrderDetail(long Id)
        {
            var mod = await orderBL.GetOrder(Id);
            return View(mod);
        }
        public async Task<JsonResult> SendOrderEmail(long Id)
        {
            var ord = await orderBL.GetPOById(Id);
            var msg = await new SMSBL().SendPOEmail("New " + ord.PONo + " Supplier " + ord.Inv_Suppliers.SuppName, Id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> OrderDetail_Read([DataSourceRequest] DataSourceRequest request, int POId)
        {
            var lst = await orderBL.OrderDetail(POId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region POApproval
        public async Task<ActionResult> POValidation()
        {
            ViewBag.Level = "V";
            //ViewBag.VrStatus = SelectListVM.VrStatusSL;
            //ViewBag.COA4 = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return View("POApproval");
        }
        public async Task<ActionResult> POApproval()
        {
            ViewBag.Level = "A";
            //ViewBag.VrStatus = SelectListVM.VrStatusSL;
            //ViewBag.COA4 = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return View();
        }
        public async Task<ActionResult> POApproval_Read([DataSourceRequest] DataSourceRequest request, string Level)
        {
            var UserId = UserInfo.UserId;
            var lst = await orderBL.GetPOApproval(Level, UserId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> POApproval_Destroy([DataSourceRequest] DataSourceRequest request, OrderSearchVM mod, string Level)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var IsSave = await orderBL.POApproval(mod, Level, UserInfo.UserId);
                if (!IsSave)
                    ModelState.AddModelError("", "Server Error");
                else
                    ModelState.AddModelError("Msg", "Saved Successfully");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region PurchaseRequest
        public async Task<ActionResult> PR()
        {
            return View();
        }

        public async Task<ActionResult> AddPR()
        {
            return View();
        }
        public async Task<ActionResult> PRDetailModel(long id)
        {
            return PartialView("_GridPRDetail", await orderBL.GetPurchaseRequestById(id));
        }
        public async Task<ActionResult> PR_Read([DataSourceRequest] DataSourceRequest request, string Level)
        {
            var lst = await orderBL.GetPurchaseRequest();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> PR_Create([DataSourceRequest] DataSourceRequest request, PRVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var IsSave = await orderBL.CreatePurchaseRequest(mod, UserInfo.UserId);
                if (!IsSave)
                    ModelState.AddModelError("", "Server Error");
                else
                    ModelState.AddModelError("Msg", "Saved Successfully");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        #endregion
        #region LocalPurchase
        public async Task<ActionResult> LocalPO()
        {
            ViewData["SKUVD"] = await setupBL.SKUListAll();
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> LocalPO_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PODetailVM> mod, string Remarks, int SuppId, string SupName, string MobileNo, string Address, long prid, List<long> files)
        {
            if (mod != null)
            {
                var TransId = await orderBL.CreateLocalPurchaseOrder(mod, Remarks, SuppId, SupName, MobileNo, Address, UserInfo.UserId, prid, files);
                if (TransId > 0)
                {
                    ModelState.AddModelError("Msg", TransId.ToString());
                }
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(mod.ToDataSourceResult(request, ModelState));
        }

        #endregion
        #region PurchaseRequest
        public async Task<ActionResult> PRBranch()
        {
            return View();
        }

        public async Task<ActionResult> AddPRBranch(int? id)
        {
            if (id != null)
            {
                var mod = await orderBL.GetPurchaseRequestById(Convert.ToInt32(id));
                return View(mod);
            }
            else
            {
                return View(new PRVM());
            }

        }
        //public async Task<ActionResult> PRDetailModelBranch(long id)
        //{
        //    return PartialView("_GridPRDetail", await orderBL.GetPurchaseRequestById(id));
        //}
        public async Task<ActionResult> PRBranch_Read([DataSourceRequest] DataSourceRequest request, string Level)
        {
            var lst = await orderBL.GetPurchaseRequestBranch(UserInfo.LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> PRBranchtbl_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var lst = await orderBL.GetPurchaseRequestBranchtbl(id);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CancelPR(int id)
        {
            if (id > 0)
            {
                var lst = await orderBL.CancelPR(id, UserInfo.UserId);
                if (lst == true)
                {
                    return Json("true", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("false", JsonRequestBehavior.AllowGet);

                }

            }
            else
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> GetPOBList()
        {
            var lst = (await orderBL.GetPOBList(UserInfo.LocId)).Select(x => new { SuppName = x.LCSuppName, RowId = x.PrId, Location = x.location, PrNo = x.PrNo }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetPR(int PrId)
        {
            var lst = await orderBL.GetPurchaseRequest(PrId);
            if (lst != null)
            {
                lst.DocCount = await orderBL.GetLocalPODocuments(PrId);
            }
            else
            {
                lst.DocCount = 0;
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> PRBranch_Create([DataSourceRequest] DataSourceRequest request, PRVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var IsSave = await orderBL.CreatePurchaseRequestBranch(mod, UserInfo.UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
                else
                {
                    ModelState.AddModelError("Msg", "Saved Successfully");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        #endregion
    }
}