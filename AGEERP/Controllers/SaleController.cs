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
    public class SaleController : Controller
    {
        SaleBL saleBL = new SaleBL();
        SetupBL setupBL = new SetupBL();
        TaxBL taxBL = new TaxBL();

        public ActionResult Index()
        {
            List<UserMenuInfo> menuList = new SecurityBL().GetMenuList(UserInfo.UserId, UserInfo.GroupId);
            return View(menuList);
        }
        #region CustomerComments
        public async Task<ActionResult> CustomerComments()
        {
            return View();
        }
        public async Task<ActionResult> CustomerComments_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await saleBL.AccountsByLoc(LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CustomerComments_Update([DataSourceRequest] DataSourceRequest request, LseMasterVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await saleBL.CommentUpdate(mod.AccNo, mod.Remarks, UserId);
                if (tbl == false)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Customer
        public ActionResult BlockedCustomers()
        {
            return View();
        }
        public async Task<ActionResult> BlockCustomer_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await saleBL.BlockCustomerList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> BlockCustomer_Create([DataSourceRequest] DataSourceRequest request, BlockCustomerVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await saleBL.CreateBlockCustomer(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.RowId = tbl.RowId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> BlockCustomer_Update([DataSourceRequest] DataSourceRequest request, BlockCustomerVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await saleBL.UpdateBlockCustomer(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> CustomerList()
        {
            var lst = await saleBL.CustomerList();
            lst.RemoveAt(0);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Customer()
        {
            return View();
        }
        public async Task<ActionResult> Customer_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await saleBL.CustomerDetailList();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Customer_Create([DataSourceRequest] DataSourceRequest request, CustomerDetailVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await saleBL.CreateCustomer(mod, UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.CustId = tbl.CustId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Customer_Update([DataSourceRequest] DataSourceRequest request, CustomerDetailVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await saleBL.UpdateCustomer(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Customer_Destroy([DataSourceRequest] DataSourceRequest request, CustomerDetailVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await saleBL.DestroyCustomer(mod, UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region LeaseLock

        public ActionResult LeaseLock()
        {
            return View();
        }

        public async Task<JsonResult> GetAccNo(long accno)
        {
            var accdetail = await saleBL.GetAccountDetail(accno);
            return Json(accdetail, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetInstallmentLog([DataSourceRequest] DataSourceRequest request, long AccNo)
        {
            var lst = await saleBL.GetInstallmentLog(AccNo);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetLockedAccounts([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await saleBL.GetLockedAccounts();
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetExistIntallmentLog([DataSourceRequest] DataSourceRequest request, long AccNo)
        {
            var lst = await saleBL.GetExistLockInstallment(AccNo);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> AccLock(long accno)
        {
            var accdetail = await saleBL.AccountLock(accno);
            return Json(accdetail, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> AccUnLock(long accno)
        {
            var accdetail = await saleBL.AccounUnLock(accno);
            return Json(accdetail, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Installment_Lock(long accno)
        {
            if (accno != null)
            {
                var ms = await saleBL.InstallmentLock(accno);
                if (ms)
                {
                    return Json(new[] { "true" }, JsonRequestBehavior.AllowGet);

                }
                else
                    return Json(new[] { "false" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new[] { "false" }, JsonRequestBehavior.AllowGet);
            }


        }


        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Installment_UnLock(long accno)
        {
            if (accno != null)
            {
                var ms = await saleBL.InstallmentUnLock(accno);
                if (ms)
                {
                    return Json(new[] { "true" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new[] { "false" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new[] { "false" }, JsonRequestBehavior.AllowGet);
            }


        }




        #endregion

        #region CashSale
        public async Task<ActionResult> CashSale()
        {
            //ViewData["CompanyVD"] = await setupBL.CompanyList();
            //ViewData["ProductVD"] = await setupBL.ProductList();
            //ViewData["ModelVD"] = await setupBL.ModelList();
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        public async Task<JsonResult> GetSaleCart(int LocId)
        {
            var lst = await saleBL.GetSaleCart(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> _SaleOrder(int LocId, long OrderId)
        {
            var lst = await saleBL.GetSaleOrderDetail(LocId, OrderId);
            return PartialView(lst);
        }
        public ActionResult PendingInvoices()
        {
            return View();
        }
        public async Task<ActionResult> PendingInvoices_Read([DataSourceRequest] DataSourceRequest request, DateTime FromDate, DateTime ToDate, int LocId)
        {
            var lst = await taxBL.GetPendingInvoices(LocId, FromDate, ToDate);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> PendingInvoices_Update([DataSourceRequest] DataSourceRequest request, CashSaleVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await taxBL.UpdatePendingInvoices(mod, UserInfo.UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public async Task<JsonResult> PendingInvoicesPosting(List<long> TransLst)
        {
            var result = await taxBL.PostPending(TransLst);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CashSaleSearch()
        {
            return View();
        }

        public async Task<ActionResult> CashSale_Read([DataSourceRequest] DataSourceRequest request, DateTime FromDate, DateTime ToDate, int LocId)
        {
            var lst = await saleBL.GetCashSale(LocId, FromDate, ToDate);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CreditSale()
        {
            //ViewData["CompanyVD"] = await setupBL.CompanyList();
            //ViewData["ProductVD"] = await setupBL.ProductList();
            //ViewData["ModelVD"] = await setupBL.ModelList();
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        public async Task<JsonResult> PaymentModeList()
        {
            var lst = (await saleBL.PaymentModeList()).Select(x => new { PaymentModeId = x.PaymentModeId, PaymentMode = x.PaymentMode });
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> EmpByDesgLoc(int DesgId, int LocId)
        {
            var lst = (await saleBL.GetEmpByDesgLoc(DesgId, LocId)).Select(x => new { EmpId = x.EmpId, EmpName = x.EmpName, CNIC = x.CNIC }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetSalesmanList(int DesgId, int LocId)
        {
            var lst = (await saleBL.GetEmpByDesgLoc(DesgId, LocId)).Select(x => new { EmpId = x.EmpId, EmpName = x.EmpName, CNIC = x.CNIC }).ToList();
            lst.Add(new { EmpId = 0, EmpName = "AIC", CNIC = "00000-0000000-0" });
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> EmpByRoleLoc(int RoleId, int LocId)
        {
            var lst = (await saleBL.GetEmpByRoleLoc(RoleId, LocId)).Select(x => new { EmpId = x.EmpId, EmpName = x.EmpName, CNIC = x.CNIC });
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> EmpByCNICRoleLoc(string CNIC, int RoleId, int LocId)
        {
            var x = await saleBL.GetEmpByRoleLoc(CNIC, RoleId, LocId);
            if (x != null)
            {
                var lst = new { EmpId = x.EmpId, EmpName = x.EmpName, CNIC = x.CNIC };
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            return Json("No Data Found", JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ReturnReasonList(string Type)
        {
            var lst = await saleBL.GetReturnReasonList(Type);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemBySrNo(string SrNo, int SKUId, int LocId)
        {
            var lst = await saleBL.GetItemBySrNo(SrNo, SKUId, LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemBySrNo4Advance(string SrNo, int SKUId, int LocId, int PlanId, int Duration)
        {
            var lst = await saleBL.GetItemBySrNo4Advance(SrNo, SKUId, LocId, PlanId, Duration);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> CheckPairing(long[] mod)
        {
            var lst = await saleBL.CheckPairing(mod);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> CheckBikeCashSale(long[] mod)
        {
            var lst = await saleBL.CheckBikeCashSale(mod);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> CheckPairingForProcessing(int[] mod)
        {
            var lst = await saleBL.CheckPairingForProcessing(mod);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> CheckLseCategory(long ItemId, long NewItemId, int CatId)
        {
            var lst = await saleBL.CheckLseCategory(ItemId, NewItemId, CatId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemBySerialNo(string SrNo, int LocId)
        {
            var lst = await saleBL.GetItemBySerialNo(SrNo, LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemBySKULoc(int SKUId, int LocId)
        {
            var pri = await saleBL.GetItemSMBySKULoc(SKUId, LocId);
            var lst = (await saleBL.GetItemBySKULoc(SKUId, LocId)).Select(x => new
            {
                ItemId = x.ItemId,
                SerialNo = x.SerialNo,
                SM = pri,
                MRP = x.MRP,
                //PairId = x.Itm_Master.PairId ?? 0,
                CSerialNo = x.CSerialNo,
                Exempted = x.Exempted,
                SuppId = x.SuppId,
                CategoryId = x.Inv_Suppliers.CategoryId,
                PPrice = x.Inv_Suppliers.CategoryId == 4 ? x.PPrice : 0,
                ProductId = x.Itm_Master.Itm_Model.Itm_Type.ProductId,
                Aging = (DateTime.Now - x.TrxDate).Value.Days,
                IsPair = x.Itm_Master.IsPair

                //SKUName = x.Itm_Master.SKUName,
                //Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                //Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName
            });
            return Json(lst.OrderByDescending(x => x.Aging).ToList(), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SKUByLocListReturn(int LocId, long AccNo)
        {
            var lst = await setupBL.SKUByLocListReturn(LocId, AccNo);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetOnlineCustomer(string MobileNo)
        {
            var lst = await saleBL.GetOnlineCustomer(MobileNo);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemBySKULocReturn(int SKUId, int LocId, long AccNo)
        {
            var pri = await saleBL.GetItemSMBySKULoc(SKUId, LocId);
            var lst = (await saleBL.GetItemBySKULocReturn(SKUId, LocId, AccNo)).Select(x => new
            {
                ItemId = x.ItemId,
                SerialNo = x.SerialNo,
                SM = pri,
                MRP = x.MRP,
                //PairId = x.Itm_Master.PairId ?? 0,
                CSerialNo = x.CSerialNo,
                Exempted = x.Exempted
                //PPrice = x.PPrice

                //SKUName = x.Itm_Master.SKUName,
                //Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                //Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName
            });
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Sale_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SaleDetailVM> mod,
            int LocId, string CustomerName, DateTime InvoiceDate, decimal OrderAdvance, long OrderId,
            DateTime DueDate, decimal ReceiveAmount, decimal CashAmount, decimal Discount, string CustCellNo, string Remarks, int PaymentModeId, long PaymentAccId,
            string CustomerAccountNo, int Salesman, string Address, string CustCNIC, string CustNTN, List<long> files, string AccountTitle, string BankName, string CustType,
            string Email, string BankTransId, string CustomerAccountHolder, string uri)
        {
            if (mod != null && ModelState.IsValid)
            {
                var ms = await saleBL.CheckPairing(mod.Select(x => x.ItemId).ToArray());
                if (ms == "OK")
                {
                    //ms = await saleBL.CheckBikeCashSale(mod.Select(x => x.ItemId).ToArray());
                    //if (ms == "OK")
                    //{
                    var result = await saleBL.SaveSale(mod, LocId, 1, CustomerName, InvoiceDate, DueDate, (ReceiveAmount - CashAmount), CashAmount, Discount, OrderAdvance, CustCellNo, Remarks, PaymentModeId, PaymentAccId, CustomerAccountNo, Salesman, 1, Address, CustCNIC, CustNTN, OrderId, UserInfo.UserId, files, AccountTitle, BankName, CustType, Email, BankTransId, CustomerAccountHolder);
                    if (result.TransId > 0)
                    {
                        if (!string.IsNullOrEmpty(uri))
                        {
                            saleBL.SaveOnlineCustPic(uri, result.TransId.ToString());
                        }
                        await taxBL.PostToFBR(result.TransId, "C");
                        ModelState.AddModelError("Msg", result.TransId.ToString());
                    }
                    else
                        ModelState.AddModelError("", result.Msg);
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("", ms);
                    //}
                }
                else
                {
                    ModelState.AddModelError("", ms);
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CreditSale_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SaleDetailVM> mod,
            int LocId, int TransType, int CustId, DateTime InvoiceDate,
            DateTime DueDate, decimal Advance, decimal Discount, string Remarks, int PaymentModeId, int Salesman)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await saleBL.SaveSale(mod, LocId, CustId, "", InvoiceDate, DueDate, 0, Advance, Discount, 0, "", Remarks, PaymentModeId, 0, "", Salesman, TransType, "", "", "", 0, UserInfo.UserId, null, "", "", "", "", "", "");
                if (result.TransId > 0)
                {
                    await taxBL.PostToFBR(result.TransId, "C");
                    ModelState.AddModelError("Msg", result.TransId.ToString());
                }
                else
                    ModelState.AddModelError("", result.Msg);
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult SaleCreditReceive()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaleCreditReceive(SaleCreditReceiveVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var TransId = await saleBL.CreditSaleSave(mod, UserInfo.UserId);
                if (TransId > 0)
                {
                    mod.TransId = TransId;
                }
            }
            return Json(mod, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CreditInvoiceList()
        {
            var lst = await saleBL.GetCreditInvoiceNo();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetCreditSaleInv(long TransId)
        {
            var x = await saleBL.GetSaleDetail(TransId);
            if (x.TransactionTypeId == 5)
            {
                var data = new
                {
                    TotalPrice = x.Inv_SaleDetail.Sum(a => a.SPrice - a.Discount) - x.Discount,
                    Advance = x.Advance,
                    PreBalance = x.Inv_SaleDetail.Sum(a => a.SPrice - a.Discount) - x.Advance - x.Discount - x.Inv_SaleCreditReceive.Sum(a => a.ReceivedAmount),
                    CustName = x.CustName,
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (x.TransactionTypeId == 11)
            {
                var data = new
                {
                    TotalPrice = x.Inv_SaleDetail.Sum(a => a.SPrice - a.Discount) - x.Discount,
                    Advance = x.Advance,
                    PreBalance = x.Inv_SaleDetail.Sum(a => a.SPrice - a.Discount) - x.Advance - x.Discount - x.Inv_SaleCreditReceive.Sum(a => a.ReceivedAmount),
                    CustName = (await new EmployeeBL().GetEmpNameById(x.CustId)),
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region OrderSale
        public async Task<ActionResult> OrderSale()
        {
            //ViewData["CompanyVD"] = await setupBL.CompanyList();
            //ViewData["ProductVD"] = await setupBL.ProductList();
            //ViewData["ModelVD"] = await setupBL.ModelList();
            return View();
        }

        public async Task<JsonResult> SaleOrderList(int LocId)
        {
            var lst = await saleBL.GetSaleOrderList(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> SaleOrder_Read([DataSourceRequest] DataSourceRequest request, int LocId, long OrderId)
        {
            var lst = await saleBL.GetSaleOrderDetail(LocId, OrderId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SaleOrderReturn(int LocId, long TransId)
        {
            var lst = await saleBL.SaleOrderReturn(LocId, TransId, UserInfo.UserId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Sale_Order_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SaleDetailVM> mod,
           int LocId, string CustomerName, DateTime InvoiceDate,
           DateTime DueDate, decimal Advance, decimal Discount, string CustCellNo, string Remarks, int PaymentModeId, long PaymentAccId, int Salesman, string Address, string CustCNIC)
        {
            if (mod != null && ModelState.IsValid)
            {
                var ms = await saleBL.CheckPairing(mod.Select(x => x.ItemId).ToArray());
                if (ms == "OK")
                {
                    var TransId = await saleBL.SaveSaleOrder(mod, LocId, 1, CustomerName, InvoiceDate, DueDate, 0, Advance, Discount, CustCellNo, Remarks, PaymentModeId, PaymentAccId, Salesman, 13, Address, CustCNIC, UserInfo.UserId);
                    if (TransId > 0)
                        ModelState.AddModelError("Msg", TransId.ToString());
                    else
                        ModelState.AddModelError("", "Server Error");
                }
                else
                {
                    ModelState.AddModelError("", ms);
                }
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Sale Return
        public async Task<ActionResult> SaleReturn()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        public async Task<JsonResult> GetReturnInvoice(int LocId, string BillNo, int TransTypeId)
        {
            var lst = await saleBL.GetReturnInvoice(LocId, BillNo, TransTypeId);
            if (lst != null)
            {
                return Json(new { TransId = lst.TransId, CustId = lst.CustId, CustName = lst.CustName, CustCellNo = lst.CustCellNo }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { TransId = 0, CustId = 0, CustName = "", CustCellNo = "" }, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetSaleTargetType()
        {
            var lst = await saleBL.GetSaleTargetTypeVM();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SKUForReturnList(int LocId, long TransId)
        {
            var lst = await saleBL.GetModelForReturn(LocId, TransId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ItemForReturnList(int SKUId, int LocId, long TransId)
        {
            var lst = (await saleBL.GetItemForReturnList(SKUId, LocId, TransId)).Select(x => new
            {
                ItemId = x.ItemId,
                SerialNo = x.SerialNo,
                SPrice = x.SPrice
            });
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SaleReturn_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SaleDetailVM> mod,
            int LocId, DateTime InvoiceDate, decimal ReceiveAmount, decimal CashAmount, string Remarks, int PaymentModeId, long PaymentAccId, long TransId, string type,int ReasonId)
        {
            if (mod != null && ModelState.IsValid)
            {
                var Id = await saleBL.SaveSaleReturn(mod, LocId, InvoiceDate, ReceiveAmount - CashAmount, CashAmount, Remarks, PaymentModeId, PaymentAccId, TransId, UserInfo.UserId, type, ReasonId);
                if (Id > 0)
                {
                    await taxBL.PostToFBR(Id, "C");
                    ModelState.AddModelError("Msg", Id.ToString());
                }
                else
                    ModelState.AddModelError("", "Server Error");
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Ref Sale Return
        public async Task<ActionResult> RefSaleReturn()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public async Task<ActionResult> SaleReturn_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SaleDetailVM> mod,
        //    int LocId, DateTime InvoiceDate, decimal ReceiveAmount, decimal CashAmount, string Remarks, int PaymentModeId, long PaymentAccId, long TransId, string type)
        //{
        //    if (mod != null && ModelState.IsValid)
        //    {
        //        var Id = await saleBL.SaveSaleReturn(mod, LocId, InvoiceDate, ReceiveAmount - CashAmount, CashAmount, Remarks, PaymentModeId, PaymentAccId, TransId, UserInfo.UserId, type);
        //        if (Id > 0)
        //        {
        //            await taxBL.PostToFBR(Id, "C");
        //            ModelState.AddModelError("Msg", Id.ToString());
        //        }
        //        else
        //            ModelState.AddModelError("", "Server Error");
        //    }

        //    return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        //}
        #endregion

        #region Installment Sale
        public async Task<ActionResult> Processing()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Processing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<LseDetailVM> mod,
            ProcessingVM data)
        {
            if (mod != null && ModelState.IsValid)
            {
                var msg = await saleBL.CheckPairingForProcessing(mod.Select(x => x.SKUId).ToArray());
                if (msg == "OK")
                {
                    var accNo = await saleBL.SaveProcessing(mod, data, UserInfo.UserId);
                    if (accNo > 0)
                        ModelState.AddModelError("Msg", accNo.ToString());
                    else
                        ModelState.AddModelError("", "Server Error");
                }
                else
                    ModelState.AddModelError("", msg);
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        //[HttpPost]
        //public async Task<ActionResult> Processing([Bind(Exclude = "Status")] ProcessingVM mod)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var proc = await saleBL.SaveProcessing(mod, UserInfo.UserId);
        //        //mod.ProcessNo = proc;
        //    }
        //    return Json(mod, JsonRequestBehavior.AllowGet);
        //}
        public async Task<JsonResult> GetLocManagers(int LocId)
        {
            var lst = await saleBL.GetLocManagers(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LseStatusList()
        {
            var lst = await saleBL.LseStatusList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ProceesingList(int LocId)
        {
            //str = str.ToUpper();
            var lst = (await saleBL.GetProceesingList(LocId)).Select(x => new
            {
                ProcessDate = x.ProcessDate.ToString("dd-MM-yyyy"),
                CustName = x.CustName.ToUpper(),
                AccNo = x.AccNo.ToString()
            }).OrderByDescending(x => x.AccNo).ToList();
            //lst = lst.Where(x => x.AccNo.Contains(str) || x.CustName.Contains(str) || x.ProcessDate.Contains(str)).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> ProceesingListVoucher(int LocId)
        {
            //str = str.ToUpper();
            var lst = (await saleBL.GetProceesingListVoucher(LocId)).Select(x => new
            {
                InstDate = x.InstDate.ToString("dd-MM-yyyy"),
                CustName = x.Lse_Master.CustName.ToUpper(),
                AccNo = x.AccNo.ToString(),
                InstId = x.InstId
            }).ToList();
            //lst = lst.Where(x => x.AccNo.Contains(str) || x.CustName.Contains(str) || x.ProcessDate.Contains(str)).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LseCustomerList(int LocId)
        {
            var lst = (await saleBL.GetLseCustomerList(LocId)).Select(x => new
            {
                ProcessDate = x.DeliveryDate.ToString("dd-MM-yyyy"),
                CustName = x.CustName,
                AccNo = x.AccNo
            }).OrderByDescending(x => x.AccNo).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LseProcessingList(int LocId)
        {
            var lst = (await saleBL.GetLseProcessingList(LocId)).Select(x => new
            {
                ProcessDate = x.DeliveryDate.ToString("dd-MM-yyyy"),
                CustName = x.CustName,
                AccNo = x.AccNo
            }).OrderByDescending(x => x.AccNo).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CheckBlock(string str)
        {
            return Json(await saleBL.CheckBlock(str), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ProceesingByNIC(string NIC)
        {
            var lst = await saleBL.GetProceesingByNIC(NIC);
            if (lst != null)
            {
                return Json(new
                {
                    CustName = lst.CustName,
                    FName = lst.FName,
                    Mobile1 = lst.Mobile1,
                    Mobile2 = lst.Mobile2,
                    Status = lst.Status,
                    AccNo = lst.AccNo,
                    ProcessDate = lst.ProcessDate,
                    InqOfficerId = lst.InqOfficerId,
                    MktOfficerId = lst.MktOfficerId,
                    ManagerId = lst.ManagerId
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ProceesingByNo(long AccNo, int LocId)
        {
            var lst = await saleBL.GetProceesingByNo(AccNo, LocId);

            if (lst != null)
            {
                EmployeeBL employeeBL = new EmployeeBL();
                var InqOfficer = await employeeBL.GetEmployeeById(lst.InqOfficerId);
                var MktOfficer = await employeeBL.GetEmployeeById(lst.MktOfficerId);
                var Manager = await employeeBL.GetEmployeeById(lst.ManagerId);
                var SManager = await employeeBL.GetEmployeeById(lst.SManagerId ?? 0);

                bool thumb = await saleBL.IsCustThumbExist(AccNo);
                var mod = lst.Lse_Customer.FirstOrDefault();
                if (mod == null)
                {

                    var data = new
                    {
                        lst.CustName,
                        lst.FName,
                        lst.Mobile1,
                        lst.Mobile2,
                        lst.Status,
                        lst.AccNo,
                        lst.ProcessDate,
                        lst.InqOfficerId,
                        lst.MktOfficerId,
                        lst.ManagerId,
                        lst.SManagerId,
                        lst.NIC,
                        lst.Advance,
                        lst.Duration,
                        lst.InstPrice,
                        lst.MonthlyInst,
                        lst.CategoryId,
                        InqOfficerCNIC = InqOfficer.CNIC,
                        MktOfficerCNIC = MktOfficer.CNIC,
                        ManagerCNIC = Manager.CNIC,
                        SManagerCNIC = SManager.CNIC,
                        InqOfficerName = InqOfficer.EmpName,
                        MktOfficerName = MktOfficer.EmpName,
                        ManagerName = Manager.EmpName,
                        SManagerName = SManager.EmpName,
                        //RM = lst.RMId == null ? "" : employeeBL.GetEmployeeById(lst.RMId ?? 0).Result.EmpName,
                        //SRM = lst.SRMId == null ? "" : employeeBL.GetEmployeeById(lst.SRMId ?? 0).Result.EmpName,
                        //CRC = lst.CRCId == null ? "" : employeeBL.GetEmployeeById(lst.CRCId ?? 0).Result.EmpName,
                        CustId = 0,
                        Affidavit = false,
                        Defaulter = false,
                        Gender = "M",
                        Occupation = "",
                        OffAddress = "",
                        ResAddress = "",
                        ResidentialStatus = "",
                        Salary = 0,
                        Worth = false,
                        WrantyCard = false,
                        Thumb = thumb
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    lst.CustName,
                    lst.FName,
                    lst.Mobile1,
                    lst.Mobile2,
                    lst.Status,
                    lst.AccNo,
                    lst.ProcessDate,
                    lst.InqOfficerId,
                    lst.MktOfficerId,
                    lst.ManagerId,
                    lst.SManagerId,
                    lst.NIC,
                    lst.Advance,
                    lst.Duration,
                    lst.InstPrice,
                    lst.MonthlyInst,
                    lst.CategoryId,
                    InqOfficerCNIC = InqOfficer.CNIC,
                    MktOfficerCNIC = MktOfficer.CNIC,
                    ManagerCNIC = Manager.CNIC,
                    SManagerCNIC = SManager.CNIC,
                    InqOfficerName = InqOfficer.EmpName,
                    MktOfficerName = MktOfficer.EmpName,
                    ManagerName = Manager.EmpName,
                    SManagerName = SManager.EmpName,
                    //RM = lst.RMId == null ? "" : employeeBL.GetEmployeeById(lst.RMId ?? 0).Result.EmpName,
                    //SRM = lst.SRMId == null ? "" : employeeBL.GetEmployeeById(lst.SRMId ?? 0).Result.EmpName,
                    //CRC = lst.CRCId == null ? "" : employeeBL.GetEmployeeById(lst.CRCId ?? 0).Result.EmpName,
                    CustId = mod.CustId,
                    Affidavit = mod.Affidavit,
                    Defaulter = mod.Defaulter,
                    Gender = mod.Gender,
                    Occupation = mod.Occupation,
                    OffAddress = mod.OffAddress,
                    ResAddress = mod.ResAddress,
                    ResidentialStatus = mod.ResidentialStatus,
                    Salary = mod.Salary,
                    Worth = mod.Worth,
                    WrantyCard = mod.WrantyCard,
                    Thumb = thumb
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LseProcessingByNo(long AccNo, int LocId)
        {
            var lst = await saleBL.GetLseProcessingByNo(AccNo, LocId);
            if (lst != null)
            {
                EmployeeBL employeeBL = new EmployeeBL();
                var InqOfficer = await employeeBL.GetEmployeeById(lst.InqOfficerId);
                var MktOfficer = await employeeBL.GetEmployeeById(lst.MktOfficerId);
                var Manager = await employeeBL.GetEmployeeById(lst.ManagerId);
                var SManager = await employeeBL.GetEmployeeById(lst.SManagerId ?? 0);
                string RM = lst.RMId == null ? "" : (await employeeBL.GetEmpNameById(lst.RMId ?? 0));
                string SRM = lst.SRMId == null ? "" : (await employeeBL.GetEmpNameById(lst.SRMId ?? 0));
                string CRC = lst.CRCId == null ? "" : (await employeeBL.GetEmpNameById(lst.CRCId ?? 0));
                var data = new
                {
                    lst.CustName,
                    lst.FName,
                    lst.AccNo,
                    lst.InqOfficerId,
                    lst.MktOfficerId,
                    lst.ManagerId,
                    lst.SManagerId,
                    lst.Advance,
                    lst.Duration,
                    lst.InstPrice,
                    lst.MonthlyInst,
                    lst.CategoryId,
                    RM = RM,
                    SRM = SRM,
                    CRC = CRC,
                    InqOfficerCNIC = InqOfficer.CNIC,
                    MktOfficerCNIC = MktOfficer.CNIC,
                    ManagerCNIC = Manager.CNIC,
                    SManagerCNIC = SManager.CNIC,
                    InqOfficerName = InqOfficer.EmpName,
                    MktOfficerName = MktOfficer.EmpName,
                    ManagerName = Manager.EmpName,
                    SManagerName = SManager.EmpName
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> LseCustomerByNo(long AccNo, int LocId)
        {
            var lst = await saleBL.GetLseCustomerByNo(AccNo, LocId);
            if (lst != null)
            {
                EmployeeBL employeeBL = new EmployeeBL();
                var InqOfficer = await employeeBL.GetEmployeeById(lst.InqOfficerId);
                var MktOfficer = await employeeBL.GetEmployeeById(lst.MktOfficerId);
                var Manager = await employeeBL.GetEmployeeById(lst.ManagerId);
                var SManager = await employeeBL.GetEmployeeById(lst.SManagerId ?? 0);
                string RM = lst.RMId == null ? "" : (await employeeBL.GetEmpNameById(lst.RMId ?? 0));
                string SRM = lst.SRMId == null ? "" : (await employeeBL.GetEmpNameById(lst.SRMId ?? 0));
                string CRC = lst.CRCId == null ? "" : (await employeeBL.GetEmpNameById(lst.CRCId ?? 0));
                var data = new
                {
                    lst.CustName,
                    lst.FName,
                    lst.AccNo,
                    lst.InqOfficerId,
                    lst.MktOfficerId,
                    lst.ManagerId,
                    lst.SManagerId,
                    lst.Advance,
                    lst.Duration,
                    lst.InstPrice,
                    lst.MonthlyInst,
                    lst.CategoryId,
                    RM = RM,
                    SRM = SRM,
                    CRC = CRC,
                    InqOfficerCNIC = InqOfficer.CNIC,
                    MktOfficerCNIC = MktOfficer.CNIC,
                    ManagerCNIC = Manager.CNIC,
                    SManagerCNIC = SManager.CNIC,
                    InqOfficerName = InqOfficer.EmpName,
                    MktOfficerName = MktOfficer.EmpName,
                    ManagerName = Manager.EmpName,
                    SManagerName = SManager.EmpName
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }
        public async Task<JsonResult> InstAccByNo(long AccNo, long InstId)
        {
            var lst = await saleBL.GetAccByNo(AccNo);
            if (lst == null)
                return Json(new { msg = "Account not found" }, JsonRequestBehavior.AllowGet);
            if (lst.IsLock)
                return Json(new { msg = "Account Locked By HO" }, JsonRequestBehavior.AllowGet);
            //if (lst.Status == 3)
            //{
            var inst = lst.Lse_Installment.Where(x => x.InstId == InstId).FirstOrDefault();
            if (inst == null)
            {
                return Json(new { msg = "Installment not found" }, JsonRequestBehavior.AllowGet);
            }
            var workingDate = inst.InstDate;
            var InstPrice = lst.InstPrice;
            var Advance = lst.Advance;
            var ActualAdvance = lst.ActualAdvance;
            var PrevBalance = lst.InstPrice - lst.Advance - (lst.Lse_Installment.Where(x => x.InstId < InstId).Sum(x => (x.InstCharges + x.Discount)));
            var MonthlyInstallment = lst.MonthlyInst;
            var currPeriod = GetMonthDifference(lst.DeliveryDate.Value, workingDate);
            var dur = lst.Duration - 1;
            var Firstdate = Convert.ToDateTime(workingDate.ToString("yyyy-MM-01"));
            if (Firstdate >= Convert.ToDateTime("2020-05-01") && lst.DeliveryDate < Convert.ToDateTime("2020-05-01"))
            {
                currPeriod = GetMonthDifference(lst.DeliveryDate.Value, workingDate.AddMonths(-1));
            }
            var toPaid = lst.ActualAdvance + (lst.MonthlyInst * (dur < currPeriod ? dur : currPeriod));
            var Paid = (lst.Advance + (lst.Lse_Installment.Where(x => x.InstId < InstId).Sum(x => x.InstCharges + x.Discount)));

            var Arrear = toPaid - Paid;
            string Recovery = await new EmployeeBL().GetEmpNameById(inst.RecoveryId ?? 0);
            //var Recov = lst.Lse_Outstand.Where(x => x.OutstandDate.Month == workingDate.Month && x.OutstandDate.Year == workingDate.Year).Select(x => x.Pay_EmpMaster).FirstOrDefault();
            //if (Recov != null)
            //{
            //    Recovery = Recov.EmpName;
            //}
            var prod = await saleBL.GetProductByAcc(AccNo);

            //var dur = lst.AccDate - DateTime.Now.Date

            return Json(new
            {
                InstPrice,
                Advance,
                ActualAdvance,
                PrevBalance,
                MonthlyInstallment,
                Arrear,
                lst.CustName,
                lst.FName,
                Recovery,
                lst.LocId,
                lst.Comp_Locations.LocName,
                msg = "OK",
                lst.Status,
                Company = prod.ComName,
                Product = prod.ProductName,
                SKU = prod.SKUCode,
                Comments = lst.Remarks == "OldData" ? "" : lst.Remarks
                //Company = lst.Lease_Processing.Itm_Model.Itm_Type.Itm_Company.ComName,
                //Product = lst.Lease_Processing.Itm_Model.Itm_Type.Itm_Products.ProductName,
                //lst.Lease_Processing.Itm_Model.Model
            }, JsonRequestBehavior.AllowGet);
            //}
            //return Json(null, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> AccByNo(long AccNo)
        {
            var lst = await saleBL.GetAccByNo(AccNo);
            if (lst == null)
                return Json(new { msg = "Account not found" }, JsonRequestBehavior.AllowGet);
            if (lst.IsLock)
                return Json(new { msg = "Account Locked By HO" }, JsonRequestBehavior.AllowGet);
            //if (lst.Status == 3)
            //{
            var workingDate = setupBL.GetWorkingDate(lst.LocId);
            var InstPrice = lst.InstPrice;
            var Advance = lst.Advance;
            var ActualAdvance = lst.ActualAdvance;
            var PrevBalance = lst.InstPrice - lst.Advance - (lst.Lse_Installment.Sum(x => (x.InstCharges + x.Discount)));
            var MonthlyInstallment = lst.MonthlyInst;
            var currPeriod = GetMonthDifference(lst.DeliveryDate.Value, workingDate);
            var dur = lst.Duration - 1;
            var Firstdate = Convert.ToDateTime(workingDate.ToString("yyyy-MM-01"));
            if (Firstdate >= Convert.ToDateTime("2020-05-01") && lst.DeliveryDate < Convert.ToDateTime("2020-05-01"))
            {
                currPeriod = GetMonthDifference(lst.DeliveryDate.Value, workingDate.AddMonths(-1));
            }
            var toPaid = lst.ActualAdvance + (lst.MonthlyInst * (dur < currPeriod ? dur : currPeriod));
            var Paid = (lst.Advance + (lst.Lse_Installment.Sum(x => x.InstCharges + x.Discount)));

            var Arrear = toPaid - Paid;
            string Recovery = "";
            var Recov = lst.Lse_Outstand.Where(x => x.OutstandDate.Month == workingDate.Month && x.OutstandDate.Year == workingDate.Year).Select(x => x.Pay_EmpMaster).FirstOrDefault();
            if (Recov != null)
            {
                Recovery = Recov.EmpName;
            }
            var prod = await saleBL.GetProductByAcc(AccNo);

            var aicInst = lst.Lse_Installment.Where(x => x.PaidBy == 3).Sum(x => x.InstCharges + x.Discount);

            return Json(new
            {
                InstPrice,
                Advance,
                ActualAdvance,
                PrevBalance,
                MonthlyInstallment,
                Arrear,
                lst.CustName,
                lst.FName,
                Recovery,
                lst.LocId,
                lst.Comp_Locations.LocName,
                msg = "OK",
                lst.Status,
                Company = prod.ComName,
                Product = prod.ProductName,
                SKU = prod.SKUCode,
                Comments = lst.Remarks == "OldData" ? "" : lst.Remarks,
                aicInst = aicInst
                //Company = lst.Lease_Processing.Itm_Model.Itm_Type.Itm_Company.ComName,
                //Product = lst.Lease_Processing.Itm_Model.Itm_Type.Itm_Products.ProductName,
                //lst.Lease_Processing.Itm_Model.Model
            }, JsonRequestBehavior.AllowGet);
            //}
            //return Json(null, JsonRequestBehavior.AllowGet);
        }
        //public async Task<JsonResult> CustomerById(int CustId)
        //{
        //    var lst = await saleBL.GetCustomerById(CustId);
        //    if (lst != null)
        //    {
        //        return Json(new
        //        {
        //            CustName = lst.Lease_Processing.CustName,
        //            SO = lst.Lease_Processing.SO,
        //            Mobile1 = lst.Lease_Processing.Mobile1,
        //            Mobile2 = lst.Lease_Processing.Mobile2,
        //            Status = lst.Lease_Processing.Status,
        //            ProcessNo = lst.ProcessNo,
        //            InqOfficerId = lst.Lease_Processing.InqOfficerId,
        //            InspectorId = lst.Lease_Processing.InspectorId,
        //            MangId = lst.Lease_Processing.MangId,
        //            InstPrice = lst.Lease_Processing.InstPrice,
        //            ActualAdvance = lst.Lease_Processing.ActualAdvance,
        //            CashPrice = lst.Lease_Processing.CashPrice,
        //            Duration = lst.Lease_Processing.Duration,
        //            MonthlyInstallment = lst.Lease_Processing.MonthlyInstallment,
        //            ProcessFee = lst.Lease_Processing.ProcessFee,
        //            ModelId = lst.Lease_Processing.ModelId,
        //            LocId = lst.LocId,
        //            PlanId = lst.Lease_Processing.PlanId
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}
        public async Task<ActionResult> PlanCalculator()
        {
            ViewBag.SKU = await setupBL.SKUListAll();
            return View();
        }
        public async Task<JsonResult> PlanPolicyList(int LocId, int SKUId, bool IsLocal, int Duration)
        {
            var lst = await saleBL.PlanPolicyList(LocId, SKUId, IsLocal, Duration);
            var data = lst.Select(x => new { MarkUp = x.MarkUp, RowId = x.PolicyId }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> InstPriceBySKUAuto(int LocId, int SKUId, decimal Advance, decimal PPrice, bool IsLocal, int PolicyId)
        {
            var x = await saleBL.InstPlanBySKU(LocId, SKUId, Advance, PPrice, IsLocal, PolicyId);
            if (x != null)
            {
                var data = new
                {
                    InstPrice = x.InstPrice,
                    Advance = x.Advance,
                    Inst = x.Inst,
                    //Duration = x.Duration,
                    //RowId = x.RowId,
                    Msg = x.Remarks
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = new
                {
                    InstPrice = 0,
                    Advance = 0,
                    Inst = 0,
                    //Duration = x.Duration,
                    //RowId = x.RowId,
                    Msg = "No Plan Found"
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> InstPriceBySKU(int SKUId, int LocId, int Duration)
        {
            var lst = (await saleBL.GetInstPriceBySKU(SKUId, LocId, Duration)).Select(x => new
            {
                InstPrice = x.InstPrice,
                Advance = x.Advance,
                Inst = x.Inst,
                Duration = x.Duration,
                RowId = x.RowId,
                Status = "Regular",
                PlanName = x.Duration + " Months - Rs. " + x.InstPrice.ToString()
            }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> InstPriceBySerial(int SKUId, int LocId, int Duration, long ItemId)
        {
            var lst = (await saleBL.GetInstPriceBySKU(SKUId, LocId, Duration, ItemId)).Select(x => new
            {
                InstPrice = x.InstPrice ?? 0,
                Advance = x.Advance ?? 0,
                Inst = x.Inst ?? 0,
                Duration = x.Duration ?? 0,
                RowId = x.RowId,
                Status = "Regular",
                PlanType = x.PlanType,
                PlanName = (x.Duration ?? 0) + " Months - Rs. " + (x.InstPrice ?? 0).ToString()
            }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CategoryList()
        {
            var lst = (await saleBL.CategoryList()).Select(x => new
            {
                CategoryId = x.CategoryId,
                Category = x.Category
            }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> LeaseCustomer()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> LeaseCustomer([Bind] LeaseCustomerVM mod)
        {
            if (ModelState.IsValid)
            {
                var CustId = await saleBL.SaveCustomer(mod, UserInfo.UserId);
                if (CustId > 0)
                {
                    mod.CustId = CustId;
                    return Json(mod, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("Guarantor", new { Id = CustId });
                }
            }
            return Json(mod, JsonRequestBehavior.AllowGet);
            //return View(mod);
        }
        public ActionResult _CustEnroll()
        {
            return PartialView();
        }


        public ActionResult CustImg(HttpPostedFileBase file, string id)
        {
            //var ext = Path.GetExtension(file.FileName);
            var physicalPath = Path.Combine(Server.MapPath("~/Content/CustImg"), id.ToString() + ".jpg");
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
            file.SaveAs(physicalPath);
            return Json(new { ImageUrl = physicalPath }, "text/plain");
        }

        #endregion

        #region Guarantor
        //public async Task<ActionResult> Guarantor(int Id)
        //{
        //    if(Id > 0)
        //    {
        //        var cust = await saleBL.GetCustomerById(Id);
        //        if(cust != null)
        //        {
        //            ViewBag.CustId = cust.CustId;
        //            ViewBag.CustName = cust.Lease_Processing.CustName;
        //            ViewBag.SO = cust.Lease_Processing.SO;
        //            ViewBag.Mobile1 = cust.Lease_Processing.Mobile1;
        //            ViewBag.NIC = cust.Lease_Processing.NIC;
        //        }
        //    }
        //    return View();
        //}
        public async Task<ActionResult> Guarantor_Read([DataSourceRequest] DataSourceRequest request, long AcccNo)
        {
            var lst = await saleBL.GuarantorList(AcccNo);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Guarantor_Create([DataSourceRequest] DataSourceRequest request, GuarantorVM mod, long AcccNo)
        {
            if (mod != null && ModelState.IsValid)
            {
                mod.AccNo = AcccNo;
                var tbl = await saleBL.CreateGuarantor(mod, UserInfo.UserId);
                if (tbl == null)
                    ModelState.AddModelError("", "Server Error");
                else
                    mod.GuarantorId = tbl.GuarantorId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Guarantor_Update([DataSourceRequest] DataSourceRequest request, GuarantorVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var IsSave = await saleBL.UpdateGuarantor(mod, UserInfo.UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Guarantor_Destroy([DataSourceRequest] DataSourceRequest request, GuarantorVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await saleBL.DestroyGuarantor(mod, UserInfo.UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Cheque
        public async Task<ActionResult> Cheque_Read([DataSourceRequest] DataSourceRequest request, long AcccNo)
        {
            var lst = await saleBL.ChequeList(AcccNo);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Cheque_Create([DataSourceRequest] DataSourceRequest request, ChequeVM mod, long AcccNo)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                mod.AccNo = AcccNo;
                var tbl = await saleBL.CreateCheque(mod, UserInfo.UserId);
                if (tbl.Msg != "OK")
                    ModelState.AddModelError("", tbl.Msg);
                else
                    mod.ChequeId = tbl.ChequeId;
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Cheque_Update([DataSourceRequest] DataSourceRequest request, ChequeVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var Msg = await saleBL.UpdateCheque(mod, UserInfo.UserId);
                if (Msg != "OK")
                {
                    ModelState.AddModelError("", Msg);
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Cheque_Destroy([DataSourceRequest] DataSourceRequest request, ChequeVM mod)
        {
            if (mod != null)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await saleBL.DestroyCheque(mod, UserInfo.UserId);
                if (!IsSave)
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Advance
        //public async Task<ContentResult> AdvanceTest()
        //{
        //    List<LseDetailVM> det = new List<LseDetailVM>();
        //    //det.Add(new LseDetailVM { DtlId = 0,CSerialNo = "",dAdvance = 0, dInst =0,Discount = 0, InstPlanId = 0, InstPrice =0,ItemId=0,
        //    //PlanType="",Qty=0,SerialNo="",SKUId=0, SKUName="",Status=true,TPrice=0,TransId=0});
        //    ProcessingVM mod = new ProcessingVM();
        //    //{
        //    //    AccNo = 0,
        //    //    ActualAdvance = 0,
        //    //    Advance = 0,
        //    //    CategoryId = 0,
        //    //    CustName = "",
        //    //    Duration = 0,
        //    //    FName = "",
        //    //    InqOfficerId = 0,
        //    //    LocId = 0,
        //    //    InstPrice = 0,
        //    //    ManagerId = 0,
        //    //    MktOfficerId = 0,
        //    //    Mobile1 = "",
        //    //    Mobile2 = "",
        //    //    MonthlyInst = 0,
        //    //    NIC = "",
        //    //    OldAccNo = 0,
        //    //    ProcessAt = "",
        //    //    ProcessFee = 0,
        //    //    SManagerId = 0
        //    //};
        //    var content = await saleBL.SaveAdvanceTest(det,mod,UserInfo.UserId);
        //    return Content(content);
        //}
        public async Task<ActionResult> Advance()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        public async Task<ActionResult> Advance_Read([DataSourceRequest] DataSourceRequest request, long AcccNo)
        {
            var lst = await saleBL.LseDetailList(AcccNo);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Advance_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<LseDetailVM> mod,
            ProcessingVM data)
        {
            if (mod != null && ModelState.IsValid)
            {
                var ms = await saleBL.CheckPairing(mod.Select(x => x.ItemId).ToArray());
                if (ms == "OK")
                {
                    //var accNo = await saleBL.SaveAdvance(mod, data, UserInfo.UserId);
                    //if (accNo > 0)
                    //{
                    //    await taxBL.PostToFBR(accNo, "I");
                    //    ModelState.AddModelError("Msg", accNo.ToString());
                    //}
                    var msg = await saleBL.SaveAdvanceNew(mod, data, UserInfo.UserId);
                    if (msg == "OK")
                    {
                        await taxBL.PostToFBR(data.AccNo, "I");
                        ModelState.AddModelError("Msg", data.AccNo.ToString());
                    }
                    else
                        ModelState.AddModelError("", msg);
                }
                else
                {
                    ModelState.AddModelError("", "Please add pairing Item");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        //[HttpPost]
        //public async Task<ActionResult> Advance([Bind(Exclude = "Status")] AdvanceVM mod)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await saleBL.SaveAdvance(mod, UserInfo.UserId);
        //    }
        //    return View(mod);
        //}

        #endregion

        #region Installment
        public async Task<ActionResult> TodayInstallment_Read([DataSourceRequest] DataSourceRequest request, int LocId, DateTime Date)
        {
            var lst = await saleBL.GetTodayInstallments(LocId, Date);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> InstOtherBranch()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            ViewBag.WrkDte = setupBL.GetWorkingDate(UserInfo.LocId).ToString("MM/dd/yyyy");
            ViewBag.InstPaidBy = await saleBL.PaidByList();
            ViewBag.DiscAllow = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.InstDisc);
            return View();
        }
        public async Task<ActionResult> Installment()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            ViewBag.WrkDte = setupBL.GetWorkingDate(UserInfo.LocId).ToString("MM/dd/yyyy");
            ViewBag.InstPaidBy = await saleBL.PaidByList();
            ViewBag.DiscAllow = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.InstDisc);
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Installment([Bind] InstallmentVM mod)
        {
            if (ModelState.IsValid)
            {
                var inst = await saleBL.SaveInstallment(mod, UserInfo.UserId);
                mod.InstId = inst;

                return Json(mod, JsonRequestBehavior.AllowGet);
            }
            return Json(mod, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Installment_Read([DataSourceRequest] DataSourceRequest request, long AccNo)
        {
            var lst = await saleBL.GetInstByAcc(AccNo);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Installment_Update([DataSourceRequest] DataSourceRequest request, InstDetailVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var InstId = await saleBL.EditInstallment(mod, UserInfo.UserId);
                if (InstId > 0)
                {
                    ModelState.AddModelError("Msg", InstId.ToString());
                }
                else
                {
                    ModelState.AddModelError("", "Server Error");
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<JsonResult> InstExist(long AccNo)
        {
            var data = await saleBL.InstExist(AccNo, UserInfo.LocId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> InstAdj()
        {
            //ViewBag.WorkingDate = setupBL.GetWorkingDate(60);
            //ViewBag.WrkDte = setupBL.GetWorkingDate(UserInfo.LocId).ToString("MM/dd/yyyy");
            //var paidLst = await saleBL.PaidByList();
            //paidLst.RemoveAt(0);
            //var vcr = saleBL.GetClosingVoucher(ViewBag.WorkingDate,60);
            //ViewBag.InstPaidBy = vcr;
            //ViewBag.DiscAllow = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.InstDisc);
            return View();
        }
        public async Task<ActionResult> InstAdj_Read([DataSourceRequest] DataSourceRequest request, int LocId, int PaidBy)
        {
            var lst = await saleBL.GetInstAdjust(LocId, PaidBy);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> InstAdj_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<OutStandVM> mod, int LocId, int PaidBy)
        {
            if (mod != null && ModelState.IsValid)
            {
                var msg = await saleBL.SaveInstAdj(mod.ToList(), LocId, PaidBy, UserInfo.UserId);
                if (msg == "OK")
                {
                    ModelState.AddModelError("Msg", msg);
                }
                else
                {
                    ModelState.AddModelError("", msg);
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public async Task<ActionResult> InstAdjApproval()
        {
            //ViewBag.WorkingDate = setupBL.GetWorkingDate(60);
            //ViewBag.WrkDte = setupBL.GetWorkingDate(UserInfo.LocId).ToString("MM/dd/yyyy");
            //var paidLst = await saleBL.PaidByList();
            //paidLst.RemoveAt(0);
            //var vcr = saleBL.GetClosingVoucher(ViewBag.WorkingDate,60);
            //ViewBag.InstPaidBy = vcr;
            //ViewBag.DiscAllow = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.InstDisc);
            return View();
        }
        public async Task<ActionResult> InstAdjApproval_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await saleBL.GetInstAdjustApproval(LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> InstAdjApproval_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<OutStandVM> mod, int LocId)
        {
            if (mod != null && ModelState.IsValid)
            {
                var msg = await saleBL.SaveInstAdjApproval(mod.ToList(), LocId, UserInfo.UserId);
                if (msg == "OK")
                {
                    ModelState.AddModelError("Msg", msg);
                }
                else
                {
                    ModelState.AddModelError("", msg);
                }
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetClosingVoucherAdj(int LocId)
        {
            var WorkingDate = setupBL.GetWorkingDate(LocId);
            var ls = new int[] { 3, 4, 5, 6 };
            var lst = saleBL.GetClosingVoucher(WorkingDate, LocId).Where(x => ls.Contains(x.Sr)).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region LeaseReturn
        public ActionResult LeaseReturn()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        public async Task<JsonResult> ProductByAcc(long AccNo)
        {
            var obj = await saleBL.GetProductByAcc(AccNo);
            if (obj != null)
            {
                var data = new
                {
                    obj.SKUId,
                    obj.SKUCode,
                    obj.ProductName,
                    obj.ComName,
                    obj.ItemId
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public async Task<ActionResult> SearialNumChange([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<LseDetailVM> mod,
        //ProcessingVM data)
        //{
        //    if (mod != null && ModelState.IsValid)
        //    {
        //        var Trans = await saleBL.SaveSameProductReturn(mod, data, UserInfo.UserId);
        //        if (Trans == true)
        //            ModelState.AddModelError("Msg", "True");
        //        else
        //            ModelState.AddModelError("", "Server Error");
        //    }

        //    return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ProductChange([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<LseDetailVM> mod,
     ProcessingVM data)
        {
            if (mod != null && ModelState.IsValid)
            {
                var result = await saleBL.ProductChange(mod, data, UserInfo.UserId);
                if (result.TransId > 0 && result.AccNo > 0 && result.Msg == "Save Successfully")
                {
                    if (data.Type == "P")
                    {
                        await taxBL.PostToFBR(result.TransId, "R");
                    }
                    await taxBL.PostToFBR(result.AccNo, "I");
                    ModelState.AddModelError("Msg", "True");
                }
                else
                    ModelState.AddModelError("", result.Msg);
            }

            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        //[HttpPost]
        //public async Task<JsonResult> SameProductReturn(long AccNo, int LocId, int SKUId, int ItemId)
        //{
        //    string message;
        //    var obj = await saleBL.SaveSameProductReturn(AccNo, LocId, SKUId, ItemId);
        //    if (obj)
        //    {
        //        message = "success";
        //    }
        //    else
        //    {
        //        message = "Server Error";
        //    }
        //    return Json(message, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public async Task<JsonResult> ProcessPTO(long AccNo, int LocId, int SKUId, decimal InstPrice, string Type, int ReturnTypeId, long ItemId, List<long> files, string Remarks, decimal ReturnAmount, int ReasonId, bool IncExempt, bool AICReturn)
        {
            var result = await saleBL.SavePTO(AccNo, LocId, SKUId, InstPrice, UserInfo.UserId, Type, ReturnTypeId, ItemId, files, Remarks, ReturnAmount, ReasonId, IncExempt, AICReturn);
            if (Type == "P" && result.TransId > 0 && result.Msg == "Save Successfully")
            {
                await taxBL.PostToFBR(result.TransId, "R");
            }
            return Json(result.Msg, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AccountSearch

        public async Task<ActionResult> AccountSearch()
        {
            SecurityBL securityBL = new SecurityBL();
            ViewBag.IsInstRight = await securityBL.HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.IsInstRight);
            ViewBag.Status = SelectListVM.StatusDDL;
            return View();
        }
        public async Task<ActionResult> AccountSearch_Read([DataSourceRequest] DataSourceRequest request, int LocId, int Crit1, string CritVal1, int Crit2, string CritVal2)
        {
            var lst = await saleBL.GetAccountSearch(LocId, Crit1, CritVal1, Crit2, CritVal2);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> AccountSearchMore_Read([DataSourceRequest] DataSourceRequest request, int LocId, DateTime FromDate, DateTime ToDate, int RoleId, int RecoveryId, int Status)
        {
            var lst = await saleBL.GetAccountSearchMore(LocId, FromDate, ToDate, RoleId, RecoveryId, Status);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SearchCriteria()
        {
            var lst = (await saleBL.GetSearchCriteria()).Select(x => new { x.RowId, x.Title }).ToList();
            if (lst != null)
            {
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Audit
        public ActionResult Audit()
        {
            if (Request.RequestContext.RouteData.Values["id"] != null)
            {
                var AccNumber = Request.RequestContext.RouteData.Values["id"].ToString();
                if (!string.IsNullOrEmpty(AccNumber))
                {
                    var AccNo = long.Parse(AccNumber);
                    ViewBag.AccNo = AccNo;
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                ViewBag.AccNo = 0;
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Audit([Bind] LseAuditVM mod)
        {
            if (ModelState.IsValid)
            {
                var inst = await saleBL.SaveAudit(mod, UserInfo.LocId, UserInfo.UserId);
                mod.AuditId = inst.AuditId;
                return Json(mod, JsonRequestBehavior.AllowGet);
            }
            return Json(mod, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> AuditList_Read([DataSourceRequest] DataSourceRequest request, int LocId, DateTime FromDate, DateTime ToDate, string Status, string VStatus)
        {
            var lst = await saleBL.AuditList(LocId, FromDate, ToDate, Status, VStatus);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetAudit(long AccNo)
        {
            try
            {
                var acc = await saleBL.GetAccByNo(AccNo);
                var aud = acc.Lse_Audit.FirstOrDefault();
                if (aud != null)
                {
                    var data = new
                    {
                        acc.LocId,
                        acc.AccNo,
                        acc.CustName,
                        acc.FName,
                        acc.Mobile1,
                        acc.Mobile2,
                        NICStatus = acc.NIC,
                        AffidavitStatus = acc.Lse_Customer.FirstOrDefault() == null ? false : acc.Lse_Customer.FirstOrDefault().Affidavit,
                        NoOfCheques = acc.Lse_Cheque.FirstOrDefault() == null ? 0 : acc.Lse_Cheque.Where(x => x.ChequeStatus).Count(),
                        NoOfGuarantors = acc.Lse_Guarantor.FirstOrDefault() == null ? 0 : acc.Lse_Guarantor.Where(x => x.Status).Count(),

                        aud.Affidavit,
                        aud.AuditId,
                        aud.BMSign,
                        aud.Cheque,
                        aud.Guarantor1,
                        aud.Guarantor2,
                        aud.Mobile,
                        aud.NIC,
                        aud.ObserveDate,
                        aud.ObserveState,
                        aud.Pic,
                        aud.RMSign,
                        aud.Thumb,
                        aud.VerifyStatus,
                        aud.VisitStatus,
                        aud.Completion,
                        aud.ApprovedBy,
                        aud.ApprovedDate,
                        aud.CRCRemarks
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = new
                    {
                        acc.LocId,
                        acc.AccNo,
                        acc.CustName,
                        acc.FName,
                        acc.Mobile1,
                        acc.Mobile2,
                        NICStatus = acc.NIC,
                        AffidavitStatus = acc.Lse_Customer.FirstOrDefault() == null ? false : acc.Lse_Customer.FirstOrDefault().Affidavit,
                        NoOfCheques = acc.Lse_Cheque.FirstOrDefault() == null ? 0 : acc.Lse_Cheque.Count(),
                        NoOfGuarantors = acc.Lse_Guarantor.FirstOrDefault() == null ? 0 : acc.Lse_Guarantor.Count(),
                        AuditId = 0
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region PendingRecovery
        public async Task<ActionResult> PendingRecovery()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View(new PendingRecoveryVM());
        }
        [HttpPost]
        public async Task<ActionResult> PendingRecovery([Bind]PendingRecoveryVM mod)
        {
            if (mod != null)
            {
                var lst = await saleBL.SavePendingRecovery(mod, UserInfo.UserId);
                if (lst == false)
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> GetAccountInfo(long InstId)
        {
            if (InstId > 0)
            {
                var lst = await saleBL.AccountPendingInfo(InstId);
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }


        #endregion

        #region OutStandEdit
        public ActionResult OutStand()
        {
            return View();
        }
        public async Task<JsonResult> LoadOutStand(int LocId, string Category)
        {
            var lst = await saleBL.LoadOutStand(LocId, Category, UserInfo.UserId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetOutStandSummary(int LocId, string Category, DateTime OSMonth)
        {
            var lst = await saleBL.GetOutStandSummary(LocId, Category, OSMonth);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> OutStand_Read([DataSourceRequest] DataSourceRequest request, int LocId, string Category, string Status, string Assign, DateTime OSMonth)
        {
            var lst = await saleBL.GetOutStand(LocId, Category, Status, Assign, OSMonth);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> SaveOutStand(int RecId, long[] AccLst, bool IsAIC)
        {
            var lst = await saleBL.SaveOutStand(RecId, AccLst, IsAIC, UserInfo.UserId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> SaveOutStandAcc(int RecId, long AccNo, int LocId, bool IsAIC)
        {
            var lst = await saleBL.SaveOutStandAcc(RecId, AccNo, LocId, IsAIC, UserInfo.UserId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> SaveOutStandAsLastMonth(int LocId, string Category)
        {
            var lst = await saleBL.SaveOutStandAsLastMonth(LocId, Category, UserInfo.UserId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> SaveOutStandAsInquiry(int LocId, string Category)
        {
            var lst = await saleBL.SaveOutStandAsInquiry(LocId, Category, UserInfo.UserId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region DayOpening
        public async Task<ActionResult> DayOpen()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> DayOpen(DayClosingVM mod)
        {
            var data = await saleBL.DayOpen(mod, UserInfo.UserId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDayOpen(int LocId)
        {
            var data = await saleBL.GetDayOpen(LocId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetWorkingDate(int LocId)
        {
            var lst = await saleBL.GetWorkingDate(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Expense
        public async Task<ActionResult> Expense()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            //ViewData["ExpenseVD"] = await setupBL.ExpenseListByLocation(UserInfo.LocId);
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Expense(ExpenseTransactionVM mod)
        {
            if (ModelState.IsValid)
            {
                var transId = await saleBL.SaveExpense(mod, UserInfo.UserId);
                //mod.TransId = transId;
                return Json(transId, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        //[AcceptVerbs(HttpVerbs.Post)]
        //public async Task<ActionResult> Expense_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExpenseTransactionVM> mod, string UploadedFiles, int LocId, DateTime WorkingDate, string RefBillNo, long TicketId)
        //{
        //    if (mod != null && ModelState.IsValid)
        //    {
        //        var tbl = await saleBL.SaveExpense(mod.ToList(), UploadedFiles, LocId, WorkingDate, RefBillNo, TicketId, UserInfo.UserId);
        //        if (tbl == null)
        //            ModelState.AddModelError("", "Server Error");
        //    }
        //    return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        //}
        public async Task<JsonResult> ExpenseList()
        {

            var lst = await setupBL.ExpenseList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExpenseListBySubCode()
        {

            var lst = setupBL.ExpenseListBySubCode();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> ExpenseListByLocation()
        {
            var lst = await setupBL.ExpenseListByLocation(UserInfo.LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SourceChange
        public async Task<ActionResult> InstallmentSourceChange()
        {
            ViewData["InstallmentSource"] = await saleBL.PaidByList();
            return View();
        }
        public async Task<ActionResult> InstSource_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await saleBL.GetInstallmentSources(LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> InstSource_Update([DataSourceRequest] DataSourceRequest request, InstallmentVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var tbl = await saleBL.UpdateSource(mod.InstId, mod.AccNo, mod.PaidBy, mod.Remarks,UserInfo.UserId);
                if (tbl == false)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region VPNSearch 
        public ActionResult VPNSearch()
        {
            ViewBag.Status = SelectListVM.StatusDDLAll;
            return View();
        }
        public ActionResult VPNSearch_Read([DataSourceRequest] DataSourceRequest request, int Crit1, string CritVal1, int Status)
        {
            var lst = saleBL.GetVPNSearch(UserInfo.LocId, Crit1, CritVal1, Status, UserInfo.UserId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> VPNSearchCriteria()
        {
            var lst = (await saleBL.VPNGetSearchCriteria()).Select(x => new { x.RowId, x.Title }).ToList();
            if (lst != null)
            {
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Customer Editing Rights
        public ActionResult CustomerEditingRights()
        {
            return View();
        }
        public async Task<ActionResult> CERData_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await saleBL.GetAssignAcc(LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CER_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await saleBL.GetLseCER(LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> CER_AddEdit(List<long> mod, int LocId)
        {
            DateTime workdte = setupBL.GetWorkingDate(LocId);
            var lst = await saleBL.AddEditLSECER(mod, workdte, UserInfo.UserId, LocId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CER_Destroy([DataSourceRequest] DataSourceRequest request, List<LSECERVM> mod)
        {
            var lst = await saleBL.LSECERDestroy(mod.FirstOrDefault(), UserInfo.UserId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public async Task<JsonResult> SaleTypeList()
        {
            var lst = await saleBL.SaleTypeList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        #region Complain
        public async Task<ActionResult> Complain()
        {

            return View();
        }


        [HttpPost]
        public async Task<JsonResult> Complain(ComplainVM com)
        {
            var result = await saleBL.RegisterComplain(com, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Closing Voucher

        public async Task<ActionResult> ClosingVoucher()
        {
            ViewBag.ClosingApproval = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.ClosingVoucher);
            ViewBag.LocId = UserInfo.LocId;
            return View();
        }

        //public JsonResult GetClosingVoucherAdj(int LocId, DateTime mon)
        //{
        //    var lst = saleBL.GetClosingVoucherAdj(mon, LocId);
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetClosingVoucherHeader(int LocId, DateTime mon)
        {
            var Voucher = saleBL.GetClosingVoucher(mon, LocId);
            var VoucherAdj = saleBL.GetClosingVoucherAdj(mon, LocId);
            ClosingVoucherDatVM mod = new ClosingVoucherDatVM()
            {

                VoucherAdj = VoucherAdj,
                VoucherDet = Voucher
            };
            return Json(mod, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ClosingVoucherDetail_Read([DataSourceRequest] DataSourceRequest request, int LocId, DateTime mon)
        {
            var mod = saleBL.GetClosingVoucherDist(mon, LocId);
            return Json(mod.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ClosingVoucher_Update([DataSourceRequest] DataSourceRequest request, List<ClosingVoucherDistVM> mod, DateTime mon, int Locid)
        {
            bool Transtatus = false;
            if (mod != null && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;
                var IsSave = await saleBL.AddClosingVoucher(mod, Locid, UserInfo.LocId, mon, UserInfo.UserId);
                Transtatus = IsSave;
            }
            return Json(Transtatus);
        }

        #endregion

        public async Task<JsonResult> SubCodeList()
        {
            var lst = await new AccountBL().SubCodeList();//).Select(x => new { Code = x.Code, Name = x.Name }).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        #region CRCFines
        public async Task<JsonResult> CRCFinePolicy()
        {
            var obj = await saleBL.GetPolicyDetail();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> CrcFines()
        {
            ViewBag.WorkingDate = setupBL.GetWorkingDate(UserInfo.LocId);
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> CrcFines(CrcFinesVM mod)
        {
            if (ModelState.IsValid)
            {
                var result = await saleBL.SaveCrcFines(mod, UserInfo.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(mod, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CrcFines_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = await saleBL.CrcFinesList(UserInfo.UserId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CrcFinesApproval()
        {
            ViewBag.RightsCRC = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.CRCApprovalHead);
            ViewBag.RightsRM = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.CRCApprovalRM);
            return View();
        }

        public ActionResult CrcFinesApproval_Read([DataSourceRequest] DataSourceRequest request)
        {
            var lst = saleBL.CrcFinesListForApproval(UserInfo.UserId, UserInfo.GroupId, 0);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CrcFinesApproval_Approve(int id)
        {
            if (id > 0 && ModelState.IsValid)
            {
                var UserId = UserInfo.UserId;

                var IsSave = await saleBL.ApproveCRCFine(UserInfo.GroupId, UserInfo.UserId, id);
                return Json(IsSave);

            }
            return Json(false);
        }

        #endregion

        #region CancelProcessing
        public ActionResult CancelProcessing()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CancelProcessing(FormCollection frm)
        {
            if (ModelState.IsValid)
            {
                var result = await saleBL.CancelProcessing(Convert.ToInt32(frm["LocId"]), Convert.ToInt64(frm["AcccNo"]), frm["Remarks"], frm["UploadedFiles"], UserInfo.UserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("Invalid", JsonRequestBehavior.AllowGet);
        }
        #endregion

       
    }
}