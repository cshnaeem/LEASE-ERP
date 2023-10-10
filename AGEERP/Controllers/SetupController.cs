using AGEERP.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using NotifSystem.Web.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AGEERP.Controllers
{
    [RBAC]
    public class SetupController : Controller
    {
        SetupBL setupBL = new SetupBL();
        NotificationBL notificationBL = new NotificationBL();
        NotificationHub objNotifHub = new NotificationHub();
        SMSBL sMSBL = new SMSBL();


        #region CashSaleRate
        public async Task<ActionResult> CashSaleRate()
        {
            ViewData["SKUVD"] = await setupBL.SKUListAll();
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CashSaleRate_Save([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SaleRateVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                if (mod.Where(x => x.EffectiveDate < DateTime.Now.Date).Any())
                {
                    ModelState.AddModelError("", "Effective Date cannot be less than current date");
                }
                else
                {
                    var tbl = await setupBL.CashSaleRateAdd(mod, UserId);
                    if (tbl == null)
                        ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Modifications
        public ActionResult TaxExemption()
        {
            return View();
        }
        public async Task<JsonResult> UpdateExemption(long ItemId, string Remarks, bool Exemption)
        {
            var message = "error";
            var lst = await setupBL.UpdateExemption(UserInfo.UserId, ItemId, Remarks, Exemption);
            if (lst)
            {
                message = "ok";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Modifications()
        {
            return View();
        }
        public async Task<JsonResult> UpdateSKU(int SKUId, string SerialNo, long ItemId, string Remarks, decimal PPrice, decimal MRP)
        {
            var message = "error";
            var lst = await setupBL.UpdateSKU(UserInfo.UserId, SKUId, SerialNo, ItemId, Remarks, PPrice, MRP);
            if (lst)
            {
                message = "ok";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> UpdateMI(long Accno, int Type, int EmpId, string Remarks)
        {
            var message = "error";
            var lst = await setupBL.UpdateMI(UserInfo.UserId, Accno, Type, EmpId, Remarks);
            if (lst)
            {
                message = "ok";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SKUDetails(string Serial)
        {
            var lst = await setupBL.SKUDetails(Serial);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Installment_Update([DataSourceRequest] DataSourceRequest request, InstDetailVM mod, long Accno, string Remarks)
        {
            if (mod != null && ModelState.IsValid)
            {
                var InstId = await new SaleBL().UpdateInstallment(UserInfo.UserId, Accno, Remarks, mod);
                if (InstId)
                {
                    ModelState.AddModelError("Msg", "Success");
                }
                else
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> UpdateAdvance(long Accno, decimal Advance, string Remarks)
        {
            var message = "error";
            var lst = await setupBL.UpdateAdvance(UserInfo.UserId, Accno, Advance, Remarks);
            if (lst)
            {
                message = "ok";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> UpdatePlan(long Accno, decimal InstPrice, decimal ActualAdvance, decimal MonthlyInst, decimal Advance, decimal dAdvance, decimal dInst, long InstPlanId, int Duration, string Remarks)
        {
            var message = "error";
            var lst = await setupBL.UpdatePlan(UserInfo.UserId, Accno, InstPrice, ActualAdvance, MonthlyInst, Advance, dAdvance, dInst, InstPlanId, Duration, Remarks);
            if (lst)
            {
                message = "ok";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SKUByAccno(long Accno)
        {
            var lst = await setupBL.SKUByAccno(Accno);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> EmployeeByRoles(int RoleId, int LocId)
        {
            var lst = await setupBL.EmployeeByRoles(RoleId, LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SMS
        public ActionResult Sms()
        {
            return View();
        }

        //public async Task<FileResult> GetAllSMS()
        //{
        //    var file = await setupBL.GetSMSAll();
        //    return File(file, "text/csv", "SMS" + DateTime.Now.ToString() + ".csv");
        //}
        //[HttpGet]
        //public async Task<JsonResult> SendSMS(int RowId)
        //{
        //    if (RowId > 0)
        //    {
        //        var res = await sMSBL.SendSMS(RowId);
        //        if (res)
        //        {
        //            return Json( new { Msg = "ok" }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            return Json(new { Msg = "failed" }, JsonRequestBehavior.AllowGet);
        //        } 
        //    }
        //    else
        //    {
        //        return Json(new { Msg = "failed" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult SendAllSMS()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<string> SendAllSMS(FormCollection frm)
        {
            int LocId = Convert.ToInt32(frm["LocId"]);
            string Category = frm["Category"];
            string Message = frm["SMS"];
            if (Message.Length > 5 && Category != "" && LocId > 0)
            {
                var result = await setupBL.SendAllSMS(LocId, Category, Message, false);
                if (result)
                {
                    return "ok";
                }
            }
            return "not ok";
        }

        [HttpPost]
        public async Task<string> TestSMS(string TestNo, string Message)
        {
            if (!string.IsNullOrEmpty(TestNo) && !string.IsNullOrEmpty(Message))
            {
                var result = await sMSBL.Send(TestNo, Message);
                return result;
            }
            else
            {
                return "failed";
            }
        }
        public async Task<JsonResult> LoadMessages([DataSourceRequest] DataSourceRequest request, string Category, int LocId, string Message, bool IsUrdu)
        {
            var lst = await setupBL.MessageData(LocId, Category, Message, IsUrdu);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region City
        public ActionResult City()
        {
            return View();
        }
        public async Task<ActionResult> City_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.CityList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> City_Create([DataSourceRequest] DataSourceRequest request, CityVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateCity(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.CityId = tbl.CityId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> City_Update([DataSourceRequest] DataSourceRequest request, CityVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateCity(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> RegionList()
        {
            var lst = await setupBL.RegionList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CityByRegionList(int RegionId)
        {
            var lst = await setupBL.CityList(RegionId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CityList()
        {
            var lst = await setupBL.CityList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //[AcceptVerbs(HttpVerbs.Post)]
        //public async Task<ActionResult> City_Destroy([DataSourceRequest] DataSourceRequest request, CityVM mod)
        //{
        //    if (mod != null)
        //    {
        //        var UserId = UserInfo.UserId;
        //        var IsSave = await setupBL.DestroyCity(mod, UserId);
        //        if (!IsSave)
        //        {
        //            ModelState.AddModelError("", "Server Error");
        //        }
        //    }
        //    return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        //}
        #endregion

        #region CompLocations
        public async Task<ActionResult> CompLocations()
        {
            var loc = await setupBL.LocationList();
            ViewData["LocVD"] = loc.Where(x => x.LocId != 72).Select(x => new LocationVM
            {
                LocId = x.LocId,
                LocName = x.LocCode + " " + x.LocName
            }).ToList();
            return View();
        }
        public async Task<ActionResult> CompLocation_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.CompLocations();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CompLocation_Create([DataSourceRequest] DataSourceRequest request, CompLocationIPVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateCompLocation(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                {
                    mod.RowId = tbl.RowId;
                    mod.Status = tbl.Status;
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CompLocation_Update([DataSourceRequest] DataSourceRequest request, CompLocationIPVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateCompLocation(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public async Task<ActionResult> CompLocation_Destroy([DataSourceRequest] DataSourceRequest request, CompLocationIPVM mod)
        //{
        //    if (mod != null)
        //    {
        //        var UserId = UserInfo.UserId;
        //        var IsSave = await setupBL.DestroyCompLocation(mod, UserId);
        //        if (!IsSave)
        //        {
        //            ModelState.AddModelError("", "Server Error");
        //        }
        //    }
        //    return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        //}
        #endregion

        #region Location
        public async Task<ActionResult> Location()
        {
            ViewData["CityVD"] = await setupBL.CityList();
            ViewData["LocTypeVD"] = await setupBL.LocTypeList();
            var purCenter = (await setupBL.LocationList()).Select(x => new { PurCenterId = x.LocId, PurCenter = x.LocCode }).ToList();
            purCenter.Insert(0, new { PurCenterId = 0, PurCenter = "NA" });
            ViewData["PurCenterVD"] = purCenter;
            return View();
        }
        public async Task<ActionResult> Location_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.LocationDetailList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Location_Create([DataSourceRequest] DataSourceRequest request, LocationDetailVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateLocation(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.LocId = tbl.LocId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Location_Update([DataSourceRequest] DataSourceRequest request, LocationDetailVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateLocation(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Location_Destroy([DataSourceRequest] DataSourceRequest request, LocationDetailVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroyLocation(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> LocationByCityList(int CityId)
        {
            int id = 0;
            var lst = await setupBL.LocationByCityList(CityId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AllLocationNDCityList([DataSourceRequest] DataSourceRequest request, int[] City)
        {
            var lst = await setupBL.AllLocationAndCityList(City);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SuppliersCatList()
        {
            var lst = await setupBL.SupliersCatList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LocationByCityListGH(List<int> CityLst)
        {
            List<LocationVM> _Locs = new List<LocationVM>();
            foreach (var item in _Locs)
            {
                int id = 0;
                var lst = await setupBL.LocationByCityList(id);
                _Locs.AddRange(lst);
            }
            return Json(_Locs, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LocationByCityAllList(int CityId)
        {
            var lst = await setupBL.LocationByCityList(CityId);
            lst.Insert(0, new LocationVM { LocCode = "", LocId = 0, LocName = "All" });
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LocationList()
        {
            var lst = await setupBL.LocationList(UserInfo.LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LocationListA()
        {
            var lst = await setupBL.LocationList(UserInfo.LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LocationListAll()
        {
            var lst = await setupBL.LocationList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LocationListPO()
        {
            var lst = await setupBL.LocationListPO();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LocationListInActive()
        {
            var lst = await setupBL.LocationListInActive();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LocationByType(int LocTypeId)
        {
            var lst = await setupBL.LocationByType(LocTypeId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> PurchaseCenterList()
        {
            var lst = await setupBL.PurchaseCenterList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Company
        public async Task<JsonResult> CompanyList()
        {
            var lst = await setupBL.CompanyList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Company()
        {
            return View();
        }
        public async Task<ActionResult> Company_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.CompanyList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Company_Create([DataSourceRequest] DataSourceRequest request, CompanyVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateCompany(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.ComId = tbl.ComId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Company_Update([DataSourceRequest] DataSourceRequest request, CompanyVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateCompany(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Company_Destroy([DataSourceRequest] DataSourceRequest request, CompanyVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroyCompany(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> CompBySupplierList(int SuppId)
        {
            var lst = await setupBL.CompBySupplierList(SuppId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region Product
        public async Task<JsonResult> ProductList()
        {
            var lst = await setupBL.ProductList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ProductByTypeList(int TypeId)
        {
            var lst = await setupBL.ProductByTypeList(TypeId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Product()
        {
            return View();
        }
        public async Task<ActionResult> Product_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.ProductList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Product_Create([DataSourceRequest] DataSourceRequest request, ProductVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateProduct(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.ProductId = tbl.ProductId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Product_Update([DataSourceRequest] DataSourceRequest request, ProductVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateProduct(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Product_Destroy([DataSourceRequest] DataSourceRequest request, ProductVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroyProduct(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> ProductByCompList(int ComId)
        {
            var lst = await setupBL.ProductByCompList(ComId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Type

        public async Task<JsonResult> TypeListByProductId()
        {
            var lst = await setupBL.TypeListByProductId();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> TypesList()
        {
            var lst = await setupBL.TypeList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> TypeList(int ComId, int ProductId)
        {
            var lst = await setupBL.TypeByCompProdList(ComId, ProductId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> TypeByCompList(int ComId)
        {
            var lst = await setupBL.TypeByCompList(ComId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Type()
        {
            ViewData["CompanyVD"] = await setupBL.CompanyList();
            ViewData["ProductVD"] = await setupBL.ProductList();
            return View();
        }
        public async Task<ActionResult> Type_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.TypeList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Type_Create([DataSourceRequest] DataSourceRequest request, TypeVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateType(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                {
                    mod.TypeId = tbl.TypeId;
                    try
                    {
                        var notiLst = notificationBL.PostNotiLoc(1, 0, "New Type Created " + mod.Name, UserId);
                        foreach (var item in notiLst)
                        {
                            objNotifHub.SendNotification(item);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Type_Update([DataSourceRequest] DataSourceRequest request, TypeVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateType(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Type_Destroy([DataSourceRequest] DataSourceRequest request, TypeVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroyType(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region ProductClass
        public async Task<ActionResult> ProductClass()
        {
            ViewData["ProductsVD"] = await setupBL.ProductList();
            return View();
        }
        public async Task<JsonResult> ProductClassByProductId(int ProductId)
        {
            var lst = await setupBL.ProductClassList(ProductId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ProductClass_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.ProductClassList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ProductClass_Create([DataSourceRequest] DataSourceRequest request, ProductClassVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateProductClass(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else if (tbl.ProductClassId == 0)
                    ModelState.AddModelError("", "Product class Already Exists");
                else
                    mod.ProductClassId = tbl.ProductClassId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ProductClass_Update([DataSourceRequest] DataSourceRequest request, ProductClassVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateProductClass(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ProductClass_Destroy([DataSourceRequest] DataSourceRequest request, ProductClassVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroyProductClass(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Model
        public async Task<JsonResult> ModelByLocList(int LocId)
        {
            var lst = await setupBL.ModelByLocList(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ModelByTypeList(int TypeId)
        {
            var lst = await setupBL.ModelByTypeList(TypeId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ModelByComProList(int ComId, int ProductId)
        {
            var lst = await setupBL.ModelByComProList(ComId, ProductId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ModelByComProByProductClassList(int ComId, int ProductClassId, int ProductId)
        {
            if (ProductClassId != 0)
            {
                var lst = await setupBL.ModelByComProByProductClassList(ComId, ProductClassId, ProductId);
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lst = await setupBL.ModelByComProList(ComId, ProductId);
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> ModelList()
        {
            var lst = await setupBL.ModelList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ModelByLocSearchList(int LocId, string filterModel)
        {
            var lst = await setupBL.ModelByLocList(LocId);
            if (string.IsNullOrEmpty(filterModel))
                return Json(lst, JsonRequestBehavior.AllowGet);
            else
            {
                filterModel = filterModel.ToUpper();
                return Json(lst.Where(x => x.Model.ToUpper().Contains(filterModel) ||
                    x.Product.Contains(filterModel) ||
                    x.Company.Contains(filterModel)), JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> Model()
        {
            ViewData["ProductClassVD"] = await setupBL.ProductClassList();
            ViewData["TypeVD"] = await setupBL.TypeList();
            ViewBag.Status = SelectListVM.StatusDDL;
            return View();
        }
        public async Task<ActionResult> Model_Read([DataSourceRequest] DataSourceRequest request, int status)
        {
            var lst = await setupBL.ModelListByStatus(status);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Model_Create([DataSourceRequest] DataSourceRequest request, ModelVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateModel(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else if (tbl.ModelId == 0)
                    ModelState.AddModelError("", "Model Already Exists");
                else
                    mod.ModelId = tbl.ModelId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Model_Update([DataSourceRequest] DataSourceRequest request, ModelVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateModel(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Model_Destroy([DataSourceRequest] DataSourceRequest request, ModelVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroyModel(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region SKU

        public async Task<JsonResult> SKUByLocList(int LocId)
        {
            var lst = await setupBL.SKUByLocList(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> SKUByLocSearchList(int LocId, string filterModel)
        {
            var lst = await setupBL.SKUByLocList(LocId);
            if (string.IsNullOrEmpty(filterModel))
                return Json(lst, JsonRequestBehavior.AllowGet);
            else
            {
                filterModel = filterModel.ToUpper();
                return Json(lst.Where(x => x.SKUName.ToUpper().Contains(filterModel) ||
                    x.Model.ToUpper().Contains(filterModel) ||
                    x.Product.Contains(filterModel) ||
                    x.Company.Contains(filterModel)), JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> SKUAllByLocSearchList(int LocId, string filterModel)
        {
            var lst = await setupBL.SKUAllByLocList(LocId);
            if (string.IsNullOrEmpty(filterModel))
                return Json(lst, JsonRequestBehavior.AllowGet);
            else
            {
                filterModel = filterModel.ToUpper();
                return Json(lst.Where(x => x.SKUName.ToUpper().Contains(filterModel) ||
                    x.Model.ToUpper().Contains(filterModel) ||
                    x.Product.Contains(filterModel) ||
                    x.Company.Contains(filterModel)), JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> SKU()
        {
            var colLst = await setupBL.ColorList();
            //colLst.Insert(0, new Itm_Color { ColorId = 0, ColorName = "NA" });
            ViewData["ColorVD"] = colLst;
            ViewBag.Status = SelectListVM.StatusDDL;
            return View();
        }
        public async Task<ActionResult> SKU_Read([DataSourceRequest] DataSourceRequest request, int Moodel, int Status)
        {
            var lst = await setupBL.SKUListByStatus(Moodel, Status);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SKU_Create([DataSourceRequest] DataSourceRequest request, SKUVM mod, int Moodel)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                mod.ModelId = Moodel;
                var tbl = await setupBL.CreateSKU(mod, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                {
                    mod = tbl;
                    try
                    {
                        var notiLst = notificationBL.PostNotiLoc(1, 0, "New SKU Created " + mod.SKUName, UserId);
                        foreach (var item in notiLst)
                        {
                            objNotifHub.SendNotification(item);
                        }
                    }
                    catch (Exception)
                    {
                    }

                }

            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SKU_Update([DataSourceRequest] DataSourceRequest request, SKUVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateSKU(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SKU_Destroy([DataSourceRequest] DataSourceRequest request, SKUVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroySKU(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> SKUByModelList(int ModelId)
        {
            var lst = await setupBL.SKUList(ModelId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SerialSkuById(int SkuId)
        {
            var lst = await setupBL.SerailBySKUId(SkuId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SKUByModel4PurchaseList(int ModelId)
        {
            var lst = await setupBL.SKUByModel4PurchaseList(ModelId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SKUListForGRN(long TransId)
        {
            var lst = await setupBL.SKUListForGRN(TransId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SKUPairList(int TypeId)
        {
            var lst = await setupBL.SKUPairList(TypeId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ColorList()
        {
            var data = await setupBL.ColorList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SKUImg(HttpPostedFileBase file, string id)
        {
            //var ext = Path.GetExtension(file.FileName);
            var physicalPath = Path.Combine(Server.MapPath("~/Content/SKUImg"), id.ToString() + ".jpg");
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
            file.SaveAs(physicalPath);
            return Json(new { ImageUrl = physicalPath }, "text/plain");
        }
        #endregion

        #region Color
        public ActionResult Color()
        {
            return View();
        }
        public async Task<ActionResult> Color_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.ColorList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Color_Create([DataSourceRequest] DataSourceRequest request, ColorVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateColor(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else if (tbl.ColorId == 0)
                    ModelState.AddModelError("", "Model Already Exists");
                else
                    mod.ColorId = tbl.ColorId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Color_Update([DataSourceRequest] DataSourceRequest request, ColorVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateColor(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region ItemCashPrice

        public ActionResult ItemCashPrice()
        {
            return View();
        }
        public async Task<ActionResult> ItemCashPrice_Read([DataSourceRequest] DataSourceRequest request, int ComId, int ProductId, int ModelId)
        {
            var lst = await setupBL.SKUSearchList(ComId, ProductId, ModelId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ItemCashPriceList_Read([DataSourceRequest] DataSourceRequest request, int ComId, int ProductId, int ModelId, int[] CityLst, int[] LocLst)
        {
            if (LocLst[0] == 0)
            {
                LocLst = new int[0];
            }
            if (CityLst[0] == 0)
            {
                CityLst = new int[0];
            }
            var lst = await setupBL.SKUCashPriceList(ComId, ProductId, ModelId, CityLst, LocLst);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ItemCashPrice_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ItemCashPriceVM> mod, int[] CityLst, int[] LocLst, DateTime EffectiveDate)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                if (LocLst[0] == 0)
                {
                    LocLst = new int[0];
                }
                if (CityLst[0] == 0)
                {
                    CityLst = new int[0];
                }
                var tbl = await setupBL.CreateItemCashPrice(mod, CityLst, LocLst ?? (new int[0]), EffectiveDate, UserId);
                if (!tbl)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> LocationByCitiesList([Bind(Prefix = "CityLst[]")] int[] CityLst)
        {
            var data = await setupBL.LocationByCitiesList(CityLst);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> LocationByCitiesLst([Bind(Prefix = "CityLst[]")] int[] CityLst)
        {
            var data = await setupBL.LocationByCitiesList(CityLst);
            data.Insert(0, new LocationVM() { LocId = 0, LocCode = "0", LocName = "All" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Branch Staff Strnegth
        /// </summary>
        /// <param name="CityLst"></param>
        /// <returns></returns>
        public async Task<JsonResult> LocationByCitiesLstBS([Bind(Prefix = "CityLst[]")] int[] CityLst)
        {
            var data = await setupBL.LocationByCitiesList(CityLst);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ItemCashPrice_V2

        public async Task<ActionResult> ItemCashPriceV2()
        {
            var city = await setupBL.CityList();
            city.Insert(0, new CityVM { City = "All", CityId = 0 });
            ViewData["CityVD"] = city;
            var loc = await setupBL.LocationList();
            loc.Insert(0, new LocationVM { LocName = "All", LocId = 0 });
            ViewData["LocVD"] = loc;
            return View();
        }
        public async Task<ActionResult> ItemCashPriceV2_Read([DataSourceRequest] DataSourceRequest request, int ComId, int ProductId, int ModelId)
        {
            var lst = await setupBL.SKUSearchListV2(ComId, ProductId, ModelId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ItemCashPriceListV2_Read([DataSourceRequest] DataSourceRequest request, int ComId, int ProductId, int ModelId, int[] CityLst, int[] LocLst)
        {
            if (LocLst[0] == 0)
            {
                LocLst = new int[0];
            }
            if (CityLst[0] == 0)
            {
                CityLst = new int[0];
            }
            var lst = await setupBL.SKUCashPriceList(ComId, ProductId, ModelId, CityLst, LocLst);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ItemCashPriceV2_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SKUPlanVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;

                var tbl = await setupBL.UpdateSkuPlanV2(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region ItemDiscount
        public async Task<ActionResult> ItemDiscount()
        {
            return View();
        }
        public async Task<JsonResult> DiscTypes()
        {
            var lst = await setupBL.DiscList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ItemDiscount_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ItmDiscVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateItemDiscount(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ItemDiscount_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ItmDiscVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.DestroyItemDiscount(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region PPriceUploader
        public async Task<ActionResult> PPriceUploader()
        {
            ViewData["SuppVD"] = await setupBL.SupplierList();
            ViewData["SKUVD"] = await setupBL.SKUListAll();
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> PPriceUploader_Save([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PPriceVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.PPriceUploader(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region ItemPPrice
        public async Task<ActionResult> ItemPPrice()
        {
            return View();
        }
        public async Task<ActionResult> ItemPPrice_Read([DataSourceRequest] DataSourceRequest request, int ComId, int ProductId, int[] SuppId, DateTime FromDate, DateTime ToDate)
        {
            var lst = await setupBL.ItemPPriceList(ComId, ProductId, SuppId, FromDate, ToDate);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ItemPPrice_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PPriceVM> mod, DateTime FromDate, DateTime ToDate, int[] Supplier)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateItemPPrice(mod, FromDate, ToDate, Supplier, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Supplier
        public async Task<ActionResult> Supplier()
        {
            ViewBag.IsMrpTp = SelectListVM.IsMRPTP;
            ViewData["CatVD"] = (await setupBL.SupplierCatList()).Select(x => new { CategoryId = x.CategoryId, CategoryTitle = x.CategoryTitle }).ToList();
            return View();
        }
        public async Task<ActionResult> Supplier_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.SupplierDetailList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Supplier_Create([DataSourceRequest] DataSourceRequest request, SupplierDetailVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateSupplier(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.SuppId = tbl.SuppId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Supplier_Update([DataSourceRequest] DataSourceRequest request, SupplierDetailVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateSupplier(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Supplier_Destroy([DataSourceRequest] DataSourceRequest request, SupplierDetailVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroySupplier(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> SupplierList()
        {
            var lst = await setupBL.SupplierList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        //public async Task<JsonResult> POTypeList()
        //{
        //    OrderBL orderBL = new OrderBL();
        //    var lst = await orderBL.POTypeList();
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}
        public async Task<JsonResult> SupplierMobilesList()
        {
            var lst = await setupBL.SupplierMobilesList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SupplierAppliancesList()
        {
            var lst = await setupBL.SupplierAppliancesList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SupplierLocalList()
        {
            var lst = await setupBL.SupplierLocalList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> PaymentTermsList()
        {
            var lst = (await setupBL.PaymentTermsList()).Select(x => new { PaymentTermId = x.PaymentTermId, Description = x.Description, CreditDays = x.CreditDays }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SupplierCatList()
        {
            var lst = (await setupBL.SupplierCatList()).Select(x => new { CategoryId = x.CategoryId, CategoryTitle = x.CategoryTitle, GLCode = x.GLCode }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SupplierByCatList(int CategoryId)
        {
            var lst = await setupBL.SupplierByCategory(CategoryId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> TaxAppliedList()
        {
            return Json(SelectListVM.IsMRPTP, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SupplierByComList(int ComId)
        {
            var lst = await setupBL.SupplierByComList(ComId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SuppTaxExemption
        public async Task<ActionResult> SuppTaxExemption()
        {
            ViewData["SuppVD"] = await setupBL.SupplierList();
            return View();
        }
        public async Task<ActionResult> SuppTaxExemption_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.SuppTaxExemptionList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SuppTaxExemption_Create([DataSourceRequest] DataSourceRequest request, SuppTaxExemptionVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateSuppTaxExemption(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.RowId = tbl.RowId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SuppTaxExemption_Update([DataSourceRequest] DataSourceRequest request, SuppTaxExemptionVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateSuppTaxExemption(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SuppTaxExemption_Destroy([DataSourceRequest] DataSourceRequest request, SuppTaxExemptionVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroySuppTaxExemption(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region ModelPairPolicy
        public async Task<ActionResult> ModelPairPolicy()
        {
            ViewData["ModelVD"] = await setupBL.ModelList();
            return View();
        }
        public async Task<ActionResult> ModelPairPolicy_Read([DataSourceRequest] DataSourceRequest request, int Model)
        {
            var lst = await setupBL.ModelPairPolicyList(Model);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ModelPairPolicy_Create([DataSourceRequest] DataSourceRequest request, ModelPairPolicyVM mod, int Model)
        {
            if (mod != null && ModelState.IsValid)
            {
                mod.ModelId = Model;
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateModelPairPolicy(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.PolicyId = tbl.PolicyId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ModelPairPolicy_Update([DataSourceRequest] DataSourceRequest request, ModelPairPolicyVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateModelPairPolicy(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ModelPairPolicy_Destroy([DataSourceRequest] DataSourceRequest request, ModelPairPolicyVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroyModelPairPolicy(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region ModelDiscount
        public ActionResult ModelDiscount()
        {
            return View();
        }
        public async Task<JsonResult> DiscountPolicyList(int TypeId)
        {
            var data = (await setupBL.DiscountPolicyList(TypeId)).Select(x => new
            {
                PolicyId = x.PolicyId,
                Policy = x.StartDate.ToString("dd-MM-yy") + " - " + x.EndDate.ToString("dd-MM-yy"),
                StartDate = x.StartDate,
                EndDate = x.EndDate
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> DiscountPolicyModelList(int PolicyId)
        {
            var data = await setupBL.DiscountPolicyModelList(PolicyId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ModelDiscount_Read([DataSourceRequest] DataSourceRequest request, int PolicyId)
        {
            var lst = await setupBL.ModelDiscountList(PolicyId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ModelDiscount_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ModelDiscountSlabVM> mod, int Policy, int[] ModelLst, DateTime StartDate, DateTime EndDate, int TypeId)
        {
            //List<ModelDiscountSlabVM> lst = new List<ModelDiscountSlabVM>();
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateModelDiscount(mod, Policy, ModelLst, StartDate, EndDate, TypeId, UserId);
                if (tbl == 0)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ModelDiscount_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ModelDiscountSlabVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateModelDiscount(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ModelDiscount_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ModelDiscountSlabVM> mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroyModelDiscount(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region TypePlan
        public ActionResult TypePlan()
        {
            return View();
        }
        public async Task<ActionResult> TypePlan_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.TypePlanList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> TypePlan_Create([DataSourceRequest] DataSourceRequest request, TypePlanVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await setupBL.CreateTypePlan(mod, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.PolicyId = tbl.PolicyId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> TypePlan_Update([DataSourceRequest] DataSourceRequest request, TypePlanVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await setupBL.UpdateTypePlan(mod, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> TypePlanRemain_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.TypePlanRemainList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SKUPlan
        public async Task<ActionResult> SKUPlan()
        {
            var city = await setupBL.CityList();
            city.Insert(0, new CityVM { City = "All", CityId = 0 });
            ViewData["CityVD"] = city;
            var loc = await setupBL.LocationList();
            loc.Insert(0, new LocationVM { LocName = "All", LocId = 0 });
            ViewData["LocVD"] = loc;
            return View();
        }
        public async Task<ActionResult> SKUPlan_Read([DataSourceRequest] DataSourceRequest request, int SKUID)
        {
            var lst = await setupBL.SKUPlanList(SKUID);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SKUPlan_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SKUPlanVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await setupBL.CreateSKUPlan(mod, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("Error", "Server Error");
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SKUPlan_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SKUPlanVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await setupBL.DestroySKUPlan(mod, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("Error", "Server Error");
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region SerialPlan
        public async Task<ActionResult> SerialPlan()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SerialPlan_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SerialPlanVM> mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await setupBL.CreateSerialPlan(mod, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("Error", "Server Error");
            }
            return Json(mod.ToDataSourceResult(request, ModelState));
        }
        public async Task<ActionResult> SerialPlan_Read([DataSourceRequest] DataSourceRequest request, int ItemId)
        {
            var lst = await setupBL.SerialPlanRead(ItemId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SusidaryAcco
        public async Task<ActionResult> SubsidaryAccounts()
        {
            ViewData["SubTypes"] = (await setupBL.SubsidaryTypeList()).Select(x => new { SubType = x.SubType, SubTypeId = x.SubTypeId }).ToList();
            return View();
        }

        public async Task<ActionResult> SubsidaryAcc_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.SubsidaryList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SubsidaryAcc_Create([DataSourceRequest] DataSourceRequest request, SubsidaryVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await setupBL.AddSubsidaryAcc(mod);
                if (tbl == 0)
                    ModelState.AddModelError("Error", "Server Error");
            }
            return Json(new { mod }, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SubsidaryAcc_Update([DataSourceRequest] DataSourceRequest request, SubsidaryVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await setupBL.EditSubsidaryAcc(mod);
                if (tbl == 0)
                    ModelState.AddModelError("Error", "Server Error");
            }
            return Json(new { mod }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SusidaryAccoType
        public async Task<ActionResult> SubsidaryAccountTypes()
        {
            ViewData["SubsiCategory"] = (await setupBL.CategoryList()).Select(x => new { Category = x.Category, CatId = x.CatId }).ToList();
            return View();
        }

        public async Task<ActionResult> SubsidaryTypeAcc_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.SubsidaryTypeList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SubsidaryTypeAcc_Create([DataSourceRequest] DataSourceRequest request, SubsidaryTypeVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await setupBL.AddSubsidaryTypeAcc(mod);
                if (tbl == 0)
                    ModelState.AddModelError("Error", "Server Error");
            }
            return Json(new { mod }, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SubsidaryTypeAcc_Update([DataSourceRequest] DataSourceRequest request, SubsidaryTypeVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await setupBL.EditSubsidaryTypeAcc(mod);
                if (tbl == 0)
                    ModelState.AddModelError("Error", "Server Error");
            }
            return Json(new { mod }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GeneralFunctions

        public ActionResult Index()
        {
            List<UserMenuInfo> menuList = new SecurityBL().GetMenuList(UserInfo.UserId, UserInfo.GroupId);
            return View(menuList);
        }
        public async Task<JsonResult> GetUnseenChat(int UserId)
        {
            return Json(notificationBL.GetUnseenChat(UserId), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetChat(int UserId)
        {
            await notificationBL.SeenAllChatMessage(UserId, UserInfo.UserId);
            return Json(await notificationBL.GetChat(UserId, UserInfo.UserId), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetChatUsers(int UserId)
        {
            await notificationBL.SeenAllChatMessage(UserId, UserInfo.UserId);
            return Json(await notificationBL.GetChat(UserId, UserInfo.UserId), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ChatUsers_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await notificationBL.GetChatUsers(UserInfo.UserId);
            var onlineUsers = objNotifHub.GetOnlineUsers();
            UserHubModels usr;
            lst.ForEach(x =>
            {
                x.IsOnline = onlineUsers.TryGetValue(x.UserName, out usr);
            });
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> AllUsers_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await notificationBL.AllChatUsers(UserInfo.UserId);
            var onlineUsers = objNotifHub.GetOnlineUsers();
            UserHubModels usr;
            lst.ForEach(x =>
            {
                x.IsOnline = onlineUsers.TryGetValue(x.UserName, out usr);
            });
            return Json(lst.OrderByDescending(x => x.IsOnline).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SeenChat(long Id)
        {
            return Json(await notificationBL.SeenChatMessage(Id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CustomDropZone_Save(HttpPostedFileBase files)
        {
            var ext = Path.GetExtension(files.FileName);
            string rand = Guid.NewGuid().ToString() + ext;
            string path = "../../Content/ChatImg/" + rand;
            var physicalPath = Path.Combine(Server.MapPath("~/Content/ChatImg"), rand);
            //if (System.IO.File.Exists(physicalPath))
            //{
            //    System.IO.File.Delete(physicalPath);
            //}
            files.SaveAs(physicalPath);
            return Json(path, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetTax()
        {
            var data = await setupBL.GetTax();
            return Json(new { data.GST, data.WHT }, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region ItemDetail

        public async Task<ActionResult> SerialUpdate()
        {
            return View();
        }
        public async Task<ActionResult> ItemDetail()
        {
            return View();
        }
        public async Task<ActionResult> ItemDetail_Read([DataSourceRequest] DataSourceRequest request, string Serial)
        {
            var lst = await setupBL.ItemDetailList(Serial);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ItemDetail_Update([DataSourceRequest] DataSourceRequest request, ItemDetailVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateItemDetail(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Expense
        public async Task<ActionResult> Expense()
        {
            AccountBL accountBL = new AccountBL();
            ViewData["ExpenseForVD"] = SelectListVM.IsShopHead;
            ViewData["COA"] = (await accountBL.SubCodeList()).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            ViewData["ExpenseTypeVD"] = (await setupBL.ExpenseTypeList()).Select(x => new { ExpTypeId = x.Id, ExpType = x.ExpType }).ToList();
            return View();
        }

        public async Task<ActionResult> Expense_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.ExpenseList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Expense_Create([DataSourceRequest] DataSourceRequest request, ExpenseVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await setupBL.CreateExpense(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.ExpHeadId = tbl.ExpHeadId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Expense_Update([DataSourceRequest] DataSourceRequest request, ExpenseVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.UpdateExpense(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Expense_Destroy([DataSourceRequest] DataSourceRequest request, ExpenseVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await setupBL.DestroyExpense(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region ExpenseType

        public ActionResult ExpenseType()
        {
            return View();
        }
        public async Task<JsonResult> GetExpenseTypes()
        {
            var data = await setupBL.ExpenseTypeList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ExpenseType_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await setupBL.ExpenseTypeList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ExpenseType_Create([DataSourceRequest] DataSourceRequest request, ExpenseTypeVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await setupBL.CreateExpenseType(mod);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.Id = tbl.Id;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ExpenseType_Update([DataSourceRequest] DataSourceRequest request, ExpenseTypeVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await setupBL.UpdateExpenseType(mod);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }


        #endregion

        #region UsersFeedback
        public async Task<ActionResult> UsersFeedback()
        {

            return View();
        }


        [HttpPost]
        public async Task<JsonResult> UsersFeedback(Users_FeedbackVM com)
        {
            var result = await setupBL.UserFeedback(com, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Notification
        public ActionResult Notification()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Notification(FormCollection frm)
        {
            var SendBy = UserInfo.UserId;
            var ls = await notificationBL.CustomNotification(Convert.ToInt32(frm["HDeptId"] == "" ? "0" : frm["HDeptId"]), Convert.ToInt32(frm["DeptId"] == "" ? "0" : frm["DeptId"]), Convert.ToInt32(frm["DesgId"] == "" ? "0" : frm["DesgId"]), Convert.ToInt32(frm["EmpId"] == "" ? "0" : frm["EmpId"]), frm["Message"].ToString(), SendBy);
            foreach (var item in ls)
            {
                objNotifHub.SendNotification(item);
            }
            return View();
        }

        public async Task<JsonResult> MarkSeen(long id)
        {
            //NotificationHub objNotifHub = new NotificationHub();
            await notificationBL.SeenMessage(id);
            return Json("True");
        }
        public async Task<JsonResult> ClearAllNoti()
        {
            await notificationBL.ClearAllNoti(UserInfo.UserId);
            return Json("True");
        }
        #endregion

        #region Employee Hierarchy


        public async Task<JsonResult> GeoLocationList(int parentId)
        {
            var lst = await setupBL.GeoLocationListByParentId(parentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ContentResult GetWorkingDate()
        {
            var dt = setupBL.GetWorkingDate(UserInfo.LocId);
            return Content(dt.ToString("dd-MM-yyyy"));
        }

        #region CRM

        #endregion

    
    }
}