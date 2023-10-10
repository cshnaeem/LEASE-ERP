using AGEERP.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AGEERP.Controllers
{
    [RBAC]
    public class ProcurementController : Controller
    {

        ProcurementBL proBL = new ProcurementBL();
        SetupBL setupBL = new SetupBL();

        public ActionResult Index()
        {
            List<UserMenuInfo> menuList = new SecurityBL().GetMenuList(UserInfo.UserId, UserInfo.GroupId);
            return View(menuList);
        }

        public async Task<JsonResult> UOMList()
        {
            var lst = await proBL.UOMList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        #region Nature
        public async Task<JsonResult> NatureList()
        {
            var lst = await proBL.NatureList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> MTRNatureList()
        {
            var lst = (await proBL.NatureList()).Where(x => x.ItemNatureId < 3).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Nature()
        {
            return View();
        }
        public async Task<ActionResult> Nature_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await proBL.NatureList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Nature_Create([DataSourceRequest] DataSourceRequest request, ItemNatureVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await proBL.CreateNature(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.ItemNatureId = tbl.ItemNatureId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Nature_Update([DataSourceRequest] DataSourceRequest request, ItemNatureVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.UpdateNature(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Nature_Destroy([DataSourceRequest] DataSourceRequest request, ItemNatureVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.DestroyNature(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }



        #endregion

        #region CostType
        public ActionResult CostType()
        {
            return View();
        }
        public async Task<ActionResult> CostType_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await proBL.CostTypeList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CostType_Create([DataSourceRequest] DataSourceRequest request, CostTypeVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await proBL.CreateCostType(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.CostTypeId = tbl.CostTypeId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CostType_Update([DataSourceRequest] DataSourceRequest request, CostTypeVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.UpdateCostType(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CostType_Destroy([DataSourceRequest] DataSourceRequest request, CostTypeVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.DestroyCostType(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Brand
        public async Task<JsonResult> BrandList()
        {
            var lst = await proBL.BrandList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Brand()
        {
            return View();
        }
        public async Task<ActionResult> Brand_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await proBL.BrandList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Brand_Create([DataSourceRequest] DataSourceRequest request, ItemBrandVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await proBL.CreateBrand(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.ItemBrandId = tbl.ItemBrandId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Brand_Update([DataSourceRequest] DataSourceRequest request, ItemBrandVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.UpdateBrand(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Brand_Destroy([DataSourceRequest] DataSourceRequest request, ItemBrandVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.DestroyBrand(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }



        #endregion

        #region Category
        public async Task<JsonResult> CategoryList()
        {
            var lst = await proBL.CategoryList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Category()
        {

            ViewData["NatureVD"] = await proBL.NatureList();
            
            return View();
        }
        public async Task<ActionResult> Category_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await proBL.CategoryList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Category_Create([DataSourceRequest] DataSourceRequest request, ItemCategoryVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await proBL.CreateCategory(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.ItemCategoryId = tbl.ItemCategoryId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Category_Update([DataSourceRequest] DataSourceRequest request, ItemCategoryVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.UpdateCategory(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Category_Destroy([DataSourceRequest] DataSourceRequest request, ItemCategoryVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.DestroyCategory(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }



        #endregion


        #region ItemProduct
        public async Task<JsonResult> ItemProductList()
        {
            var lst = await proBL.ItemProductList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ItemProduct()
        {

            ViewData["CategoryVD"] =await  proBL.CategoryList();
            return View();
        }
        public async Task<ActionResult> ItemProduct_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await proBL.ItemProductList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ItemProduct_Create([DataSourceRequest] DataSourceRequest request, ItemProductVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await proBL.CreateItemProduct(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.ItemProductId = tbl.ItemProductId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ItemProduct_Update([DataSourceRequest] DataSourceRequest request, ItemProductVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.UpdateItemProduct(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ItemProduct_Destroy([DataSourceRequest] DataSourceRequest request, ItemProductVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.DestroyItemProduct(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }


        #endregion

        #region Item
        public async Task<ActionResult> Item()
        {
            var colLst = await proBL.ItemProductList();
            ViewData["ProductVD"] = colLst;
            ViewData["BrandVD"] = await proBL.BrandList();
            ViewData["UOMVD"] = await proBL.UOMList();
            return View();
        }
        public async Task<ActionResult> Item_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await proBL.ItemList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Item_Create([DataSourceRequest] DataSourceRequest request, ItemVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await proBL.CreateItem(mod, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                {
                    mod.ItemId = tbl.ItemId;
                    mod.ItemCode = tbl.ItemCode;
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Item_Update([DataSourceRequest] DataSourceRequest request, ItemVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.UpdateItem(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Item_Destroy([DataSourceRequest] DataSourceRequest request, ItemVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.DestroyItem(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        #endregion
        public async Task<JsonResult> ItemList()
        {
            var lst = await proBL.ItemList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemByItemTypeList(string ItemType)
        {
            var lst = await proBL.ItemByItemTypeList(ItemType);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> MTRFarDetailLst(long MTRId, int LocId)
        {
            var lst = await proBL.MTRFarDetailLst(MTRId,LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemByNatureList(int NatureId)
        {
            var lst = await proBL.ItemByNatureList(NatureId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemSearchList(string filterModel)
        {
            var lst = await proBL.ItemList();
            lst.Where(x => x.Item.Contains(filterModel) || x.ItemProduct.Contains(filterModel) || x.ItemBrand.Contains(filterModel)).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemByLocList(int LocId)
        {
            var lst = await proBL.ItemByLocList(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> MTR()
        {
            ViewBag.SKU = await setupBL.SKUListAll();
            ViewBag.CCCode = await proBL.CCCodeByEmpId(UserInfo.UserId);
            ViewBag.LocId = UserInfo.LocId;
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MTR_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<MTRDetailVM> mod, int LocId, int CCCode, DateTime RequiredDate, int NatureId)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await proBL.SaveMTR(mod,LocId,CCCode,RequiredDate,NatureId, UserInfo.UserId);
                if (result.TransId > 0)
                    ModelState.AddModelError("Msg", result.TransId.ToString());
                else
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> MTRList()
        {
            var lst = await proBL.MTRLst();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> MTRByNatureList(int NatureId)
        {
            var lst = await proBL.MTRLstByNature(NatureId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> MTRReceiving()
        {
            return View();
        }
        public async Task<ActionResult> MTRReceiving_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await proBL.MTRReceiveLst(UserInfo.LocId,UserInfo.UserId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MTRReceiving_Destroy([DataSourceRequest] DataSourceRequest request, MTRVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await proBL.MTRReceiving(mod, UserInfo.UserId);
                if (!result)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<ActionResult> MTRApproval()
        {
            ViewBag.IsEditRight = true;
            return View();
        }
        public async Task<ActionResult> EditMTR(long MTRId)
        {
            ViewBag.MTRId = MTRId;
            return View();
        }
        public async Task<ActionResult> MTRApproval_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await proBL.MTRUnApproveLst();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> POValidation()
        {
            ViewBag.Level = "V";
            return View("POApproval");
        }
        public async Task<ActionResult> POApproval()
        {
            ViewBag.Level = "A";
            return View();
        }
        public async Task<ActionResult> POApproval_Read([DataSourceRequest] DataSourceRequest request, string Level)
        {
            var lst = await proBL.GetPOApproval(Level,UserInfo.UserId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> POApproval_Destroy([DataSourceRequest] DataSourceRequest request, OrderSearchVM mod, string Level)
        {
            if (mod != null && ModelState.IsValid)
            {
                //var UserId = UserInfo.UserId;
                var IsSave = await proBL.POApproval(mod, Level, UserInfo.UserId);
                if (!IsSave)
                    ModelState.AddModelError("", "Server Error");
                else
                    ModelState.AddModelError("Msg", "Saved Successfully");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MTR_Destroy([DataSourceRequest] DataSourceRequest request, MTRVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await proBL.ApproveMTR(mod,UserInfo.UserId);
                if (!result)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<ActionResult> MTR_Read([DataSourceRequest] DataSourceRequest request, long MTR)
        {
            var lst = await proBL.MTRDetailLst(MTR);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> MTR_Update([DataSourceRequest] DataSourceRequest request, MTRDetailVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await proBL.UpdateMTR(mod);
                if (!result)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        public async Task<JsonResult> SupplierList()
        {
            var lst = await proBL.SupplierLst();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CostTypeList()
        {
            var lst = await proBL.CostTypeList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> PO()
        {
            ViewData["LocVD"] = await setupBL.LocationList();
            return View();
        }
        //public async Task<ActionResult> PO_Read([DataSourceRequest] DataSourceRequest request, long MTRId)
        //{
        //    var lst = await proBL.MTRDetailLst(MTRId);
        //    return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> PO_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProPODetailVM> mod, DateTime RequiredDate, string DeliveryAddress, long MTRId, string Remarks, int SuppId, int NatureId)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await proBL.SavePO(mod, RequiredDate,DeliveryAddress,MTRId,Remarks,SuppId,NatureId, UserInfo.UserId);
                if (result.TransId > 0)
                    ModelState.AddModelError("Msg", result.TransId.ToString());
                else
                    ModelState.AddModelError("", result.Msg);
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> POList(int LocId)
        {
            var lst = await proBL.POLst(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemTypeByNature(int NatureId)
        {
            if(NatureId == 1 || NatureId == 3)
            {
                return Json(SelectListVM.ItemTypeSL.Where(x => x.Value == "P").ToList(), JsonRequestBehavior.AllowGet);
            }
            return Json(SelectListVM.ItemTypeSL, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SSI()
        {
            ViewData["LocVD"] = await setupBL.LocationList();
            return View();
        }
        public async Task<ActionResult> GRN_Read([DataSourceRequest] DataSourceRequest request, long POId)
        {
            var lst = await proBL.PODetailLst(POId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> GRN_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProGRNDetailVM> mod, string RefInvNo, string RecvBy, long POId, int LocId, List<long> files)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await proBL.SaveGRN(mod, RefInvNo, RecvBy, POId, LocId, UserInfo.UserId);
                if (result.TransId > 0)
                {
                    await new DocumentBL().UpdateDocRef(files, result.TransId);
                    ModelState.AddModelError("Msg", result.TransId.ToString());
                }
                else
                    ModelState.AddModelError("", result.Msg);
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<ActionResult> SSR()
        {
            ViewData["LocVD"] = await setupBL.LocationList();
            return View();
        }
        public async Task<ActionResult> SSR_Read([DataSourceRequest] DataSourceRequest request, string GRNNo, int LocId)
        {
            var lst = await proBL.GRNReturn(GRNNo, LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SSR_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProGRNDetailVM> mod, string RefInvNo, string RecvBy, string GRNNo, int LocId)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await proBL.SaveGRNReturn(mod, RefInvNo, RecvBy, GRNNo, LocId, UserInfo.UserId);
                if (result.TransId > 0)
                    ModelState.AddModelError("Msg", result.TransId.ToString());
                else
                    ModelState.AddModelError("", result.Msg);
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<ActionResult> SINService()
        {
            //ViewData["LocVD"] = await setupBL.LocationList();
            return View();
        }
        public async Task<ActionResult> SIN()
        {
            ViewData["CostTypeVD"] = await proBL.CostTypeList();
            return View();
        }
        public async Task<ActionResult> SIN_Read([DataSourceRequest] DataSourceRequest request, long MTRId,int LocId)
        {
            var lst = await proBL.MTRDetailLst(MTRId, LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SIN_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SINDetailVM> mod, int CCCode, long MTRId, int LocId)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await proBL.SaveSIN(mod,CCCode,MTRId, LocId, UserInfo.UserId);
                if (result.TransId > 0)
                    ModelState.AddModelError("Msg", result.TransId.ToString());
                else
                    ModelState.AddModelError("", result.Msg);
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        #region SIR
        public async Task<ActionResult> SIR()
        {
            ViewData["CostTypeVD"] = await proBL.CostTypeList();
            return View();
        }
        public async Task<ActionResult> SIR_Read([DataSourceRequest] DataSourceRequest request, string SINNo)
        {
            var lst = await proBL.SIRList(SINNo);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SIR_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SINDetailVM> mod, string SINNo,int LocId)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await proBL.SaveSIR(mod,SINNo, LocId,UserInfo.UserId);
                if (result.TransId > 0)
                    ModelState.AddModelError("Msg", result.TransId.ToString());
                else
                    ModelState.AddModelError("", result.Msg);
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion
        public async Task<ActionResult> SINFar()
        {
            ViewData["CostTypeVD"] = await proBL.CostTypeList();
            return View();
        }
        public async Task<ActionResult> SINFar_Read([DataSourceRequest] DataSourceRequest request, long MTRId, int LocId)
        {
            var lst = await proBL.MTRDetailLst(MTRId, LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SINFar_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SINDetailVM> mod, int CCCode, long MTRId, int LocId)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await proBL.SaveSINFar(mod, CCCode, MTRId, LocId, UserInfo.UserId);
                if (result.TransId > 0)
                    ModelState.AddModelError("Msg", result.TransId.ToString());
                else
                    ModelState.AddModelError("", result.Msg);
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #region Allocation
        public async Task<ActionResult> AssetAllocation(long Id)
        {
            var lst = await proBL.GetAsset(Id);
            return View(lst);
        }
        [HttpPost]
        public async Task<ActionResult> AssetAllocation(FarStoreVM mod)
        {
            string msg = "";
            if (mod.StoreId > 0)
            {
                if(mod.CCCode != mod.ToCCCode || mod.EmpId != mod.ToEmpId)
                {
                    msg = await proBL.SaveAssetAllocation(mod,UserInfo.UserId);
                }
            }
            return Json(msg);
        }
        public async Task<ActionResult> FARDepreciation()
        {
            return View();
        }
        public async Task<ActionResult> FARDepr_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await proBL.CalcDepreciation();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> PostDepreciation()
        {
            var msg = await proBL.PostDepreciation(UserInfo.UserId);
            return Json(msg);
        }
        public async Task<ActionResult> FAR()
        {
            //proBL.farImg();
            return View();
        }
        public async Task<ActionResult> FAR_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await proBL.GetAsset();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult FARImg(HttpPostedFileBase files, string id)
        {
            //var ext = Path.GetExtension(file.FileName);
            var physicalPath = Path.Combine(Server.MapPath("~/Content/FarImg"), id.ToString() + ".jpg");
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Move(physicalPath,physicalPath.Replace(".jpg",DateTime.Now.Ticks.ToString()+".jpg"));
            }
            files.SaveAs(physicalPath);
            return Json(new { ImageUrl = physicalPath }, "text/plain");
        }

        #endregion
        #region Opening Stock
        public async Task<ActionResult> EditStockOpening()
        {
            ViewData["SuppVD"] = await proBL.SupplierLst();
            ViewData["CostTypeVD"] = (await proBL.CostTypeList()).Select(x => new { x.CostTypeId,x.CostType}).ToList();
            //ViewData["ItemVD"] = await proBL.ItemList();
            return View();
        }
        public async Task<ActionResult> StockOpening_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await proBL.GetStockOpening(LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> StockOpening_Update([DataSourceRequest] DataSourceRequest request, FarOpeningVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.UpdateStockOpening(mod, UserId);
                if (!IsSave)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> StockOpening_Destroy([DataSourceRequest] DataSourceRequest request, FarOpeningVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await proBL.DestroyStockOpening(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public async Task<JsonResult> PostOpeningStock(List<long> TransLst)
        {
            var lst = await proBL.PostOpeningStock(TransLst, UserInfo.UserId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}