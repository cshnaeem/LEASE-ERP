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
    public class CRMController : Controller
    {

        CRMBL crmBL = new CRMBL();
        SetupBL setupBL = new SetupBL();

        #region Tickets
        public ActionResult Index()
        {
            List<UserMenuInfo> menuList = new SecurityBL().GetMenuList(UserInfo.UserId, UserInfo.GroupId);
            return View(menuList);
        }
        public async Task<ActionResult> Tickets()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            ViewData["CategoryVD"] = await setupBL.CrmCategoryList();
            ViewData["StatusVD"] = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "O", Text = "Open" },
            new SelectListItem(){ Value = "H", Text = "Hold" }
        };
            ViewData["PriorityVD"] = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "Normal", Text = "Normal" },
            new SelectListItem(){ Value = "High", Text = "High" }
        };
            return View();
        }
        public async Task<ActionResult> Ticket_Read([DataSourceRequest] DataSourceRequest request, int CityId, int LocId, int CategoryId)
        {
            var lst = await crmBL.GetTickets(CityId, LocId, CategoryId, UserInfo.UserId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Ticket_Update([DataSourceRequest] DataSourceRequest request, Crm_TicketVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await crmBL.UpdateTicket(mod, UserInfo.UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult BranchTickets()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        public async Task<ActionResult> BranchTicket_Read([DataSourceRequest] DataSourceRequest request, int LocId, int CategoryId)
        {
            List<Crm_TicketVM> lst = new List<Crm_TicketVM>();
            if (LocId > 0)
            {
                lst = await crmBL.GetTickets(0, LocId, CategoryId);
            }
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Complain
        public ActionResult AddTicket()
        {

            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AddTicket(Crm_TicketVM mod)
        {


            //mod.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            var TicketId = await crmBL.AddTickets(mod, UserInfo.UserId);
            mod.TicketId = TicketId;
            return Json(mod, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _TicketResponse(int TicketId)
        {
            ViewBag.TicketId = TicketId;
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return PartialView();
        }
        public async Task<JsonResult> AddResponse(int TicketId, string Response)
        {
            var result = await crmBL.AddResponse(TicketId, Response, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetResponseList(int TicketId)
        {
            var lst = await crmBL.GetResponseList(TicketId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetTicketsList(int TicketId)
        {
            var lst = await crmBL.GetTicketsByTicketId(TicketId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetTicketsByLoc(int LocId)
        {
            var lst = await crmBL.GetTicketsByLoc(LocId);
            return Json(lst.Select(x => new { x.Category, x.Status, x.TicketId, x.Complain, Date = x.TransDate.ToString("dd-MM-yyyy") }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CloseTicket(int TicketId)
        {
            var result = await crmBL.CloseTicket(TicketId, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public async Task<JsonResult> HoldTicket(int TicketId)
        //{
        //    var result = await crmBL.HoldTicket(TicketId, UserInfo.UserId);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //public async Task<JsonResult> OpenTicket(int TicketId)
        //{
        //    var result = await crmBL.OpenTicket(TicketId, UserInfo.UserId);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        /*  [AcceptVerbs(HttpVerbs.Post)]
          public async Task<ActionResult> Ticket_Update([DataSourceRequest] DataSourceRequest request, BikeLetterSaleVM mod)
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
          }*/

        public async Task<JsonResult> CrmCategoryList()
        {
            var lst = await setupBL.CrmCategoryList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CrmCategoryMapppedList()
        {
            var lst = await setupBL.CrmCategoryMappedList(UserInfo.UserId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CrmMCategoryList()
        {
            var lst = await setupBL.CrmMCategoryList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Inventory Complain
        public ActionResult InvComplain()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> InvComplain(InvCompalinVM mod)
        {
            var IsSave = await crmBL.UpdateInvComplain(mod, UserInfo.UserId);
            return Json(IsSave, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DamageStockTickets()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            ViewData["CategoryVD"] = await setupBL.CrmCategoryList();
            ViewData["StatusVD"] = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "O", Text = "Open" },
            new SelectListItem(){ Value = "H", Text = "Hold" }
        };
            ViewData["PriorityVD"] = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "Normal", Text = "Normal" },
            new SelectListItem(){ Value = "High", Text = "High" }
        };
            return View();
        }

        public async Task<ActionResult> DamageStockTicket_Read([DataSourceRequest] DataSourceRequest request, int CityId, int LocId, DateTime FromDate, DateTime ToDate)
        {
            var lst = await crmBL.GetDamageStockTickets(CityId, LocId, UserInfo.UserId, FromDate, ToDate);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}