using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace AGEERP.Models
{
    public class PurchaseBL
    {
        AGEEntities db = new AGEEntities();
        SetupBL setupBL = new SetupBL();
        OrderBL orderBL = new OrderBL();
        AccountBL accountBL = new AccountBL();
        public async Task<long> POPayment(POPaymentVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var inv = await db.Inv_Purchase.FindAsync(mod.POInvId);
                    var pay = await GetInvDetail(mod.POInvId);
                    if (pay.PreBalance >= mod.Amount)
                    {
                        if (pay.PreBalance > mod.Amount)
                        {
                            inv.PaymentStatus = "P";
                        }
                        else if (pay.PreBalance == mod.Amount)
                        {
                            inv.PaymentStatus = "C";
                        }
                        var row = new Inv_POPayment
                        {
                            LocId = mod.LocId,
                            POInvId = mod.POInvId,
                            Amount = mod.Amount,
                            PaidBy = mod.PaidBy,
                            IsPosted = false,
                            TransDate = DateTime.Now,
                            Discount = mod.Discount,
                            WorkingDate = setupBL.GetWorkingDate(mod.LocId),
                            Remarks = mod.Remarks,
                            UserId = UserId
                        };
                        db.Inv_POPayment.Add(row);
                        await db.SaveChangesAsync();
                        scop.Complete();
                        scop.Dispose();
                        return row.TransId;
                    }
                    scop.Dispose();
                    return 0;
                }
                catch (Exception ex)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }

        public async Task<POPaymentVM> GetInvDetail(long PInvId)
        {
            try
            {
                var lst = await db.Inv_PurchaseDetail.Where(x => x.PInvId == PInvId).ToListAsync();
                decimal payment = lst.Sum(x => x.Qty * (x.Rate - x.Discount));
                decimal paid = await db.Inv_POPayment.Where(x => x.POInvId == PInvId).SumAsync(x => (decimal?)(x.Amount + x.Discount)) ?? 0;
                decimal preBalance = payment - paid;
                return new POPaymentVM { Payment = payment, PreBalance = preBalance };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<PurchaseVM>> GetPOInvoiceList(int LocId)
        {
            try
            {
                DateTime startDt = Convert.ToDateTime("2021-07-01");
                return await (from g in db.Inv_GRN
                              join p in db.Inv_PO on g.POId equals p.POId
                              join pr in db.Inv_Purchase on p.POId equals pr.POId
                              where g.LocId == LocId && p.POTypeId == 3 && pr.SuppId == 341 && pr.PaymentStatus != "C"
                              && pr.InvDate >= startDt
                              select new PurchaseVM()
                              {
                                  PInvId = pr.PInvId,
                                  InvNo = pr.InvNo,
                                  Supplier = p.LCSuppName
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Inv_PO>> GetPOForGRN(int LocId)
        {
            try
            {
                var statusLst = new int[] { 3, 4, 5 };
                return await db.Inv_POSchedule.Where(x => statusLst.Contains(x.Inv_PODetail.Inv_PO.Status) &&
                x.LocId == LocId && (x.OrderQty - (x.ReceivedQty)) > 0).Select(x => x.Inv_PODetail.Inv_PO).Distinct().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Inv_PO>> GetPOForGRN(int LocId, bool IsMobile)
        {
            try
            {

                var statusLst = new int[] { 3, 4, 5 };
                if (IsMobile)
                {
                    return await db.Inv_POSchedule.Where(x => x.Inv_PODetail.Inv_PO.POTypeId == 2 && statusLst.Contains(x.Inv_PODetail.Inv_PO.Status) &&
                    x.LocId == LocId && (x.OrderQty - (x.ReceivedQty)) > 0).Select(x => x.Inv_PODetail.Inv_PO).Distinct().ToListAsync();
                }
                else
                {
                    return await db.Inv_POSchedule.Where(x => x.Inv_PODetail.Inv_PO.POTypeId != 2 && statusLst.Contains(x.Inv_PODetail.Inv_PO.Status) &&
                    x.LocId == LocId && (x.OrderQty - (x.ReceivedQty)) > 0).Select(x => x.Inv_PODetail.Inv_PO).Distinct().ToListAsync();
                }

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Inv_PO>> GetPOBySupp(int SuppId)
        {
            try
            {
                var statusLst = new int[] { 5, 6 };
                return await db.Inv_PO.Where(x => statusLst.Contains(x.Status) && x.SuppId == SuppId).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Inv_GRN>> GetGRNBySupp(int SuppId)
        {
            try
            {
                DateTime stDate = Convert.ToDateTime("2021-07-01");
                return await db.Inv_GRN.Where(x => x.SuppId == SuppId && x.Status == "G" && x.GRNDate >= stDate).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<GRNInvoiceDetailVM>> GRNForInv(DateTime fDate, DateTime tDate, int SuppId, int CityId, int LocId, int SuppCatId, string Status)
        {
            try
            {
                var lst = db.spget_GRNForInv(SuppCatId, fDate, tDate, SuppId, CityId, LocId, Status).
                                   Select(x => new GRNInvoiceDetailVM
                                   {
                                       Discount = x.Discount,
                                       DODate = x.DODate,
                                       SKUCode = x.SKUCode,
                                       DONo = x.DONo,
                                       GRNDate = x.GRNDate,
                                       GRNId = x.GRNId,
                                       GRNNo = x.GRNNo,
                                       GST = x.GST,
                                       InvDate = x.InvDate,
                                       InvNo = x.InvNo,
                                       LocCode = x.LocCode,
                                       LocName = x.LocName,
                                       Model = x.Model,
                                       PONo = x.PONo,
                                       Qty = x.Qty,
                                       RcvdBy = x.RcvdBy,
                                       RcvdDate = x.RcvdDate,
                                       SuppName = x.SuppName,
                                       MRP = x.MRP,
                                       TP = x.TP,
                                       RP = x.MRP - x.GST,
                                       WHT = x.WHT,
                                       NetPrice = x.TP + x.WHT - x.Discount,
                                       Amount = (x.TP + x.WHT - x.Discount) * (x.Qty ?? 0),
                                       //TP = x.TP - x.GST,
                                       //WHT = x.WHT,
                                       //NetPrice = x.TP  + x.WHT - x.Discount,
                                       //Amount = (x.TP + x.WHT - x.Discount) * (x.Qty ?? 0),
                                       SuppId = x.SuppId,
                                       IsEdit = x.IsEdit ?? false
                                   }).ToList();

                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<InvForPaymentVM>> InvForPayment(DateTime fDate, DateTime tDate, int SuppId, int LocId, int SuppCatId, string Status)
        {
            try
            {
                var lst = db.spget_InvForPayment(SuppCatId, fDate, tDate, SuppId, LocId, Status).
                                   Select(x => new InvForPaymentVM
                                   {
                                       Discount = x.Discount,
                                       DODate = x.DODate,
                                       DONo = x.DONo,
                                       GRNDate = x.GRNDate,
                                       GRNNo = x.GRNNo,
                                       STax = x.STax,
                                       InvDate = x.InvDate,
                                       InvNo = x.InvNo,
                                       LocCode = x.LocCode,
                                       LocName = x.LocName,
                                       PONo = x.PONo,
                                       Qty = x.Qty,
                                       RcvdBy = x.RcvdBy,
                                       RcvdDate = x.RcvdDate,
                                       SuppName = x.SuppName,
                                       Rate = x.Rate,
                                       WHT = x.WHT,
                                       Amount = ((x.Rate + x.WHT + x.STax - x.Discount) ?? 0) - (x.PaidAmount ?? 0),
                                       PartialPaidAmount = x.PaidAmount ?? 0,
                                       NetPrice = (x.Rate + x.WHT + x.STax - x.Discount) ?? 0,
                                       SuppId = x.SuppId,
                                       DueDate = x.DueDate,
                                       PaymentStatus = x.PaymentStatus,
                                       PInvId = x.PInvId,
                                       PaidAmount = 0
                                   }).OrderBy(x => x.DueDate).ToList();

                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<SuppPaymentVM> GetPaymentAdvice(long AccId)
        {
            try
            {
                return db.spget_PaymentAdvice(true, AccId).Select(x =>
                 new SuppPaymentVM
                 {
                     SuppId = x.SuppId,
                     SuppName = x.SuppName,
                     Proposed = 0,
                     ClosingBalance = x.Balance ?? 0,
                     LocId = 192,
                     Remarks = "",
                     AccId = x.AccId
                 }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SuppPaymentVM>> SupplierPaymentList(int LocId)
        {
            try
            {
                var lst = await (from P in db.Inv_SuppPayment
                                 join S in db.Inv_Suppliers on P.SuppId equals S.SuppId
                                 join A in db.Fin_Accounts on P.AccId equals A.AccId
                                 where P.PaidStatus == "U" && P.LocId == LocId && A.SubDivId == 2
                                 select new SuppPaymentVM
                                 {
                                     AccId = (long)P.AccId,
                                     BankAccId = P.BankAccId ?? 0,
                                     BankAmount = P.BankAmount,
                                     CashAmount = P.CashAmount,
                                     SuppId = P.SuppId,
                                     ClosingBalance = P.ClosingBalance,
                                     Disc = P.Disc,
                                     Instrument = P.Instrument,
                                     InstrumentNo = P.InstrumentNo,
                                     IsWHTPaid = P.IsWHTPaid,
                                     Payment = P.Payment,
                                     SuppName = S.SuppName,
                                     TransId = P.TransId,
                                     WHT = P.WHT,
                                     ChequeDate = P.ChequeDate,
                                     LocId = P.LocId,
                                     Proposed = P.Proposed,
                                     Remarks = P.Remarks,
                                     ChequeNo = P.ChequeNo
                                 }).ToListAsync();
                lst.AddRange(await (from P in db.Inv_SuppPayment
                                    join S in db.Fin_Subsidary on P.SuppId equals S.SubId
                                    join A in db.Fin_Accounts on P.AccId equals A.AccId
                                    where P.PaidStatus == "U" && P.LocId == LocId && A.SubDivId == 1
                                    select new SuppPaymentVM
                                    {
                                        AccId = (long)P.AccId,
                                        BankAccId = P.BankAccId ?? 0,
                                        BankAmount = P.BankAmount,
                                        CashAmount = P.CashAmount,
                                        SuppId = P.SuppId,
                                        ClosingBalance = P.ClosingBalance,
                                        Disc = P.Disc,
                                        Instrument = P.Instrument,
                                        InstrumentNo = P.InstrumentNo,
                                        IsWHTPaid = P.IsWHTPaid,
                                        Payment = P.Payment,
                                        SuppName = S.SubsidaryName,
                                        TransId = P.TransId,
                                        WHT = P.WHT,
                                        ChequeDate = P.ChequeDate,
                                        LocId = P.LocId,
                                        Proposed = P.Proposed,
                                        Remarks = P.Remarks,
                                        ChequeNo = P.ChequeNo
                                    }).ToListAsync());
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SuppPaymentVM>> SupplierPaymentVerificationList()
        {
            try
            {
                var lst = await (from P in db.Inv_SuppPayment
                                 join S in db.Inv_Suppliers on P.SuppId equals S.SuppId
                                 join A in db.Fin_Accounts on P.AccId equals A.AccId
                                 where P.PaidStatus != "C" && P.IsPosted == false && A.SubDivId == 2
                                 select new SuppPaymentVM
                                 {
                                     AccTitle = A.SubCodeDesc,
                                     AccId = (long)P.AccId,
                                     BankAccId = P.BankAccId ?? 0,
                                     BankAmount = P.BankAmount,
                                     CashAmount = P.CashAmount,
                                     SuppId = P.SuppId,
                                     ClosingBalance = P.ClosingBalance,
                                     Disc = P.Disc,
                                     Instrument = P.Instrument,
                                     InstrumentNo = P.InstrumentNo,
                                     IsWHTPaid = P.PaidStatus == "P" ? true : false,
                                     Payment = P.Payment,
                                     SuppName = S.SuppName,
                                     TransId = P.TransId,
                                     WHT = P.WHT,
                                     Proposed = P.Proposed,
                                     LocId = P.LocId,
                                     Remarks = P.Remarks,
                                     ChequeDate = P.ChequeDate,
                                     PaidDate = P.PaidDate ?? DateTime.Now,
                                     ChequeNo = P.ChequeNo
                                 }).ToListAsync();
                lst.AddRange(await (from P in db.Inv_SuppPayment
                                    join S in db.Fin_Subsidary on P.SuppId equals S.SubId
                                    join A in db.Fin_Accounts on P.AccId equals A.AccId
                                    where P.PaidStatus != "C" && P.IsPosted == false && A.SubDivId == 1
                                    select new SuppPaymentVM
                                    {
                                        AccTitle = A.SubCodeDesc,
                                        AccId = (long)P.AccId,
                                        BankAccId = P.BankAccId ?? 0,
                                        BankAmount = P.BankAmount,
                                        CashAmount = P.CashAmount,
                                        SuppId = P.SuppId,
                                        ClosingBalance = P.ClosingBalance,
                                        Disc = P.Disc,
                                        Instrument = P.Instrument,
                                        InstrumentNo = P.InstrumentNo,
                                        IsWHTPaid = P.PaidStatus == "P" ? true : false,
                                        Payment = P.Payment,
                                        SuppName = S.SubsidaryName,
                                        TransId = P.TransId,
                                        WHT = P.WHT,
                                        Proposed = P.Proposed,
                                        LocId = P.LocId,
                                        Remarks = P.Remarks,
                                        ChequeDate = P.ChequeDate,
                                        PaidDate = P.PaidDate ?? DateTime.Now,
                                        ChequeNo = P.ChequeNo
                                    }).ToListAsync());
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Inv_GRN> GetGRNbyId(long GRNId)
        {
            try
            {
                return await db.Inv_GRN.FindAsync(GRNId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<POInvoiceDetailVM>> GRNForInv(long GRNId)
        {
            try
            {
                var lst = await db.Inv_GRNDetail.Where(x => x.GRNId == GRNId).GroupBy(g => g.PODtlId).ToListAsync();
                List<POInvoiceDetailVM> ls = new List<POInvoiceDetailVM>();
                foreach (var x in lst)
                {
                    var rate = await db.Inv_PODetail.FindAsync(x.Key);
                    var tbl = new POInvoiceDetailVM()
                    {
                        Qty = x.Sum(a => a.RcvdQty),
                        Model = x.FirstOrDefault().Inv_Store.Itm_Master.Itm_Model.Model,
                        SKUId = x.FirstOrDefault().Inv_Store.SKUId,
                        SKU = x.FirstOrDefault().Inv_Store.Itm_Master.SKUName,
                        Discount = rate.Discount,
                        GST = rate.GST,
                        IsGiftItem = rate.IsGiftItem,
                        MRP = rate.MRP,
                        TP = rate.TP,
                        RP = rate.MRP - rate.GST,
                        WHT = rate.WHT,
                        NetPrice = rate.TP + rate.WHT - rate.Discount,
                        Amount = (rate.TP + rate.WHT - rate.Discount) * x.Sum(a => a.RcvdQty),
                        RowId = 0,
                        PODtlId = rate.PODtlId
                    };
                    ls.Add(tbl);
                }
                //return lst.Select(x => new POInvoiceDetailVM
                //{
                //    Qty = x.Inv_POSchedule.Sum(a => a.ReceivedQty),
                //    OrderQty = x.Inv_POSchedule.Sum(a => a.OrderQty),
                //    Model = x.Itm_Master.Itm_Model.Model,
                //    SKUId = x.SKUId,
                //    SKU = x.Itm_Master.SKUName,
                //    Discount = x.Discount,
                //    GST = x.GST,
                //    IsGiftItem = x.IsGiftItem,
                //    MRP = x.MRP,
                //    TP = x.TP,
                //    WHT = x.WHT,
                //    NetPrice = x.TP + x.GST + x.WHT - x.Discount,
                //    Amount = (x.TP + x.GST + x.WHT - x.Discount) * x.Inv_POSchedule.Sum(a => a.ReceivedQty)
                //}).ToList();
                return ls;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<POInvoiceDetailVM>> POForInv(long POId)
        {
            try
            {
                var lst = await db.Inv_PODetail.Where(x => x.POId == POId).ToListAsync();
                return lst.Select(x => new POInvoiceDetailVM
                {
                    Qty = x.Inv_POSchedule.Sum(a => a.ReceivedQty),
                    OrderQty = x.Inv_POSchedule.Sum(a => a.OrderQty),
                    Model = x.Itm_Master.Itm_Model.Model,
                    SKUId = x.SKUId,
                    SKU = x.Itm_Master.SKUName,
                    Discount = x.Discount,
                    GST = x.GST,
                    IsGiftItem = x.IsGiftItem,
                    MRP = x.MRP,
                    TP = x.TP - x.GST,
                    WHT = x.WHT,
                    NetPrice = x.TP + x.WHT - x.Discount,
                    Amount = (x.TP + x.WHT - x.Discount) * x.Inv_POSchedule.Sum(a => a.ReceivedQty)
                }).ToList();
                //return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SKUItemVM>> GetSerialFromMobile(int LocId, long POId)
        {
            try
            {
                var lst = await db.Inv_POPocket.Where(x => x.Status && x.POId == POId && x.LocId == LocId).Select(x => new SKUItemVM { SKUId = x.SKUId, Serial = x.SerialNo }).ToListAsync();
                List<SKUItemVM> ls = new List<SKUItemVM>();
                foreach (var v in lst)
                {
                    var isExist = ls.Where(x => x.Serial == v.Serial).Any();
                    if (!isExist)
                    {
                        isExist = await db.Inv_Store.Where(x => x.SerialNo == v.Serial).AnyAsync();
                        if (!isExist)
                        {
                            ls.Add(v);
                        }
                    }
                }
                return ls;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<int> GetSKUByBarcode(string Barcode)
        {
            try
            {
                var SkuId = await db.Itm_Barcode.Where(x => x.Barcode == Barcode).Select(x => x.SKUId).SingleOrDefaultAsync();
                return SkuId;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<List<POPocketVM>> SavePOPocket(List<POPocketVM> mod)
        {
            foreach (var v in mod)
            {
                try
                {
                    var tbl = new Inv_POPocket
                    {
                        LocId = v.LocId,
                        POId = v.POId,
                        SerialNo = v.SerialNo,
                        SKUId = v.SKUId,
                        Status = true,
                        TransDate = DateTime.Now,
                        UserId = v.UserId
                    };
                    db.Inv_POPocket.Add(tbl);
                    await db.SaveChangesAsync();
                    v.TransId = tbl.TransId;
                }
                catch (Exception)
                {

                }
            }
            return mod;
        }
        public async Task<List<PurchaseDetailVM>> OrderDetailForGRN(long OrderNo, int LocId)
        {
            try
            {
                var ord = await db.Inv_POSchedule.Where(x => x.Inv_PODetail.POId == OrderNo && x.LocId == LocId && (x.OrderQty - (x.ReceivedQty)) > 0).ToListAsync();
                List<PurchaseDetailVM> lst = new List<PurchaseDetailVM>();
                foreach (var x in ord)
                {
                    for (int a = 0; a < (x.OrderQty - x.ReceivedQty); a++)
                    {
                        lst.Add(new PurchaseDetailVM
                        {
                            Qty = 1,
                            SrNo = "",
                            Model = x.Inv_PODetail.Itm_Master.Itm_Model.Model,
                            Product = x.Inv_PODetail.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                            Type = x.Inv_PODetail.Itm_Master.Itm_Model.Itm_Type.TypeName,
                            //Discription = x.Inv_PODetail.Itm_Master.Description,
                            TransId = x.POSchId,
                            Company = x.Inv_PODetail.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                            SKUId = x.Inv_PODetail.SKUId,
                            SKU = x.Inv_PODetail.Itm_Master.SKUName,
                            ModelId = x.Inv_PODetail.Itm_Master.ModelId
                        });
                    }
                }
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<PurchaseDetailVM>> GetOrderDetailForGRN(long OrderNo, int LocId)
        {
            try
            {
                var ord = await db.Inv_POSchedule.Where(x => x.Inv_PODetail.POId == OrderNo && x.LocId == LocId && (x.OrderQty - (x.ReceivedQty)) > 0).ToListAsync();
                List<PurchaseDetailVM> lst = new List<PurchaseDetailVM>();
                foreach (var x in ord)
                {
                    for (int a = 0; a < (x.OrderQty - x.ReceivedQty); a++)
                    {
                        var aa = lst.Where(b => b.SKUId == x.Inv_PODetail.SKUId).FirstOrDefault();
                        if (aa == null)
                        {
                            lst.Add(new PurchaseDetailVM
                            {
                                Qty = 1,
                                SrNo = "",
                                Model = x.Inv_PODetail.Itm_Master.Itm_Model.Model,
                                Product = x.Inv_PODetail.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                                Type = x.Inv_PODetail.Itm_Master.Itm_Model.Itm_Type.TypeName,
                                Discription = x.Inv_PODetail.Itm_Master.Description,
                                TransId = x.POSchId,
                                Company = x.Inv_PODetail.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                                SKUId = x.Inv_PODetail.SKUId,
                                SKU = x.Inv_PODetail.Itm_Master.SKUName,
                                ModelId = x.Inv_PODetail.Itm_Master.ModelId
                            });
                        }
                        else
                        {
                            aa.Qty = aa.Qty + 1;
                        }
                    }
                }
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<long> SaveGRN(IEnumerable<PurchaseDetailVM> mod, int LocId, int POId, int SuppId, string InvNo,
            DateTime InvDate, string DONo, DateTime DODate, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var po = await db.Inv_PO.FindAsync(POId);
                    SuppId = po.SuppId;
                    //var lastGRNId = await db.Inv_GRN.MaxAsync(x => x.GRNId);
                    var lastGRN = await db.Inv_GRN.OrderByDescending(x => x.GRNId).Select(x => x.GRNNo).FirstOrDefaultAsync();

                    //string GRNNo = DateTime.Now.ToString("yyMM");
                    //if (lastGRN.Length == 10)
                    //{
                    //    if (lastGRN == null)
                    //    {
                    //        GRNNo = "GRN-" + DateTime.Now.ToString("yyMM");
                    //        GRNNo = GRNNo + "000001";
                    //    }
                    //    else if (lastGRN.Substring(0, 4) == GRNNo)
                    //    {
                    //        GRNNo = "GRN-" + DateTime.Now.ToString("yyMM");
                    //        GRNNo = GRNNo + (Convert.ToInt32(lastGRN.Substring(4, 6)) + 1).ToString("000000");
                    //    }
                    //    else
                    //    {
                    //        GRNNo = "GRN-" + DateTime.Now.ToString("yyMM");
                    //        GRNNo = GRNNo + "000001";
                    //    }
                    //}
                    //else
                    //{
                    string GRNNo = "GRN-" + DateTime.Now.ToString("yyMM");
                    if (lastGRN == null)
                    {
                        GRNNo = GRNNo + "000001";
                    }
                    else if (lastGRN.Substring(0, 8) == GRNNo)
                    {
                        GRNNo = GRNNo + (Convert.ToInt32(lastGRN.Substring(8, 6)) + 1).ToString("000000");
                    }
                    else
                    {
                        GRNNo = GRNNo + "000001";
                    }
                    //}
                    Inv_GRN mas = new Inv_GRN()
                    {

                        GRNDate = setupBL.GetWorkingDate(LocId),
                        DONo = DONo,
                        DODate = DODate,
                        GRNNo = GRNNo,
                        InvDate = InvDate,
                        InvNo = InvNo,
                        LocId = LocId,
                        POId = POId,
                        RcvdBy = UserId,
                        RcvdDate = DateTime.Now,
                        SuppId = SuppId,
                        Status = "G"
                    };
                    db.Inv_GRN.Add(mas);
                    await db.SaveChangesAsync();
                    //List<Inv_PODetail> discLst = new List<Inv_PODetail>();
                    //decimal invAmt = 0;
                    //decimal anuAmt = 0;
                    //decimal biaAmt = 0;
                    //decimal quaAmt = 0;
                    //decimal monAmt = 0;
                    //decimal whtAmt = 0;
                    //decimal gstAmt = 0;
                    
                    foreach (var v in mod)
                    {
                        if (!string.IsNullOrEmpty(v.SrNo))
                        {
                            var poRate = db.Inv_POSchedule.FindAsync(v.TransId).Result.Inv_PODetail;
                            //discLst.Add(poRate);
                            bool isReg = poRate.Inv_PO.Inv_Suppliers.IsReg;
                            //if (poRate.Inv_PO.Inv_Suppliers.CategoryId == 4)
                            //{
                            //    isReg = false;
                            //}
                            var oldItem = await db.Inv_Store.FirstOrDefaultAsync(x => x.SerialNo == v.SrNo);
                            if (oldItem != null)
                            {
                                if (oldItem.StatusID == 9)
                                {
                                    oldItem.SerialNo = oldItem.SerialNo + "/R";
                                    await db.SaveChangesAsync();
                                }
                                else
                                {
                                    scop.Dispose();
                                    return 0;
                                }
                            }

                            var pocketLst = await db.Inv_POPocket.Where(x => x.SerialNo == v.SrNo && x.LocId == LocId && x.POId == POId && x.Status).ToListAsync();
                            pocketLst.ForEach(a => a.Status = false);

                            Inv_Store item = new Inv_Store
                            {
                                LocId = LocId,
                                Qty = v.Qty,
                                SerialNo = v.SrNo,
                                CSerialNo = "",
                                PPrice = poRate.TP - poRate.Discount - poRate.GST,
                                SPrice = 0,
                                SuppId = SuppId,
                                CPrice = 0,
                                SKUId = v.SKUId,
                                StatusID = 1,
                                TrxDate = DateTime.Now,
                                Remarks = v.Remarks,
                                MRP = poRate.MRP,
                                Tax = Math.Round(poRate.MRP > 0 ? (poRate.GST / (poRate.MRP - poRate.GST) * 100) : 0),
                                Exempted = isReg == false ? true : false,
                                WHT = poRate.WHT
                            };
                            db.Inv_Store.Add(item);
                            await db.SaveChangesAsync();

                            Inv_GRNDetail det = new Inv_GRNDetail
                            {
                                ItemId = item.ItemId,
                                PP = poRate.TP - poRate.Discount - poRate.GST,//- poRate.AnnuallyIncentive - poRate.BiannuallyIncentive - poRate.MonthlyIncentive - poRate.QuarterlyIncentive,
                                RcvdQty = 1,
                                SKUStatus = 1,
                                GRNId = mas.GRNId,
                                PODtlId = poRate.PODtlId
                            };
                            db.Inv_GRNDetail.Add(det);
                            await db.SaveChangesAsync();

                            //if (po.Inv_Suppliers.CategoryId == 4)
                            //{
                            //    await setupBL.LocalSKUPlan(LocId, v.SKUId, item.PPrice, mas.GRNNo, item.ItemId, mas.RcvdBy);
                            //}
                            //detialForVoucher.Add(det);

                            //invAmt += (poRate.TP - poRate.Discount);
                            //anuAmt += poRate.AnnuallyIncentive;
                            //biaAmt += poRate.BiannuallyIncentive;
                            //quaAmt += poRate.QuarterlyIncentive;
                            //monAmt += poRate.MonthlyIncentive;
                            //whtAmt += poRate.WHT;
                            //gstAmt += poRate.GST;
                        }
                    }
                    var lst = mod.Where(x => !string.IsNullOrEmpty(x.SrNo)).Select(x => x.TransId).Distinct().ToList();
                    foreach (var v in lst)
                    {
                        var tbl = await db.Inv_POSchedule.FindAsync(v);
                        tbl.ReceivedQty = tbl.ReceivedQty + mod.Where(x => x.TransId == v && !string.IsNullOrEmpty(x.SrNo)).Sum(x => x.Qty);
                        await db.SaveChangesAsync();
                    }

                    var ordQty = po.Inv_PODetail.Sum(x => x.Qty);
                    var recQty = po.Inv_PODetail.Sum(x => x.Inv_POSchedule.Sum(a => a.ReceivedQty));
                    if (ordQty == recQty)
                    {
                        po.Status = 6;
                    }
                    else
                    {
                        po.Status = 5;
                    }
                    await db.SaveChangesAsync();


                    var cat = await db.Inv_Suppliers.Where(x => x.SuppId == mas.SuppId).Select(x => x.CategoryId).FirstOrDefaultAsync();
                    if (mas.SuppId == 341)
                    {
                        mas.IsPosted = true;
                        mas.PostedDate = DateTime.Now;
                        var detialForVoucher = await db.Inv_GRNDetail.Where(x => x.GRNId == mas.GRNId).ToListAsync();
                        
                        //////////////////////////////////Voucher Posting////////////////////////////////////
                        List<VoucherDetailVM> vLst = new List<VoucherDetailVM>();
                        var invAcc = await accountBL.GetAcc(7, cat);
                        var clrAcc = await accountBL.GetAcc(2, cat);
                        var gstAcc = await accountBL.GetSuppAcc(108, cat);
                        var whtAcc = await accountBL.GetSuppAcc(107, cat);
                        var invClr = await accountBL.GetAcc(448);
                        foreach (var v in detialForVoucher)
                        {
                            var poRate = db.Inv_PODetail.FindAsync(v.PODtlId).Result;
                            if (poRate.TP - poRate.Discount - poRate.GST > 0)
                                vLst.Add(new VoucherDetailVM
                                {
                                    AccId = invAcc,
                                    CCCode = LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = poRate.TP - poRate.Discount - poRate.GST, //- anuAmt - biaAmt - quaAmt - monAmt - whtAmt - gstAmt,
                                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                                    PCCode = LocId,
                                    SubId = 0,
                                    RefId = v.ItemId
                                });
                            //if (poRate.GST > 0)
                            //    vLst.Add(new VoucherDetailVM
                            //    {
                            //        AccId = gstAcc,
                            //        CCCode = LocId,
                            //        ChequeNo = "",
                            //        Cr = 0,
                            //        Dr = poRate.GST, //- anuAmt - biaAmt - quaAmt - monAmt - whtAmt - gstAmt,
                            //        Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                            //        PCCode = LocId,
                            //        SubId = SuppId,
                            //        RefId = v.ItemId
                            //    });
                            //if (poRate.WHT > 0)
                            //    vLst.Add(new VoucherDetailVM
                            //    {
                            //        AccId = whtAcc,
                            //        CCCode = LocId,
                            //        ChequeNo = "",
                            //        Cr = 0,
                            //        Dr = poRate.WHT, //- anuAmt - biaAmt - quaAmt - monAmt - whtAmt - gstAmt,
                            //        Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                            //        PCCode = LocId,
                            //        SubId = SuppId,
                            //        RefId = v.ItemId
                            //    });
                            if (poRate.TP - poRate.Discount - poRate.GST > 0)
                                vLst.Add(new VoucherDetailVM
                                {
                                    AccId = clrAcc,
                                    CCCode = LocId,
                                    ChequeNo = "",
                                    Cr = poRate.TP - poRate.Discount - poRate.GST,
                                    Dr = 0,
                                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                                    PCCode = LocId,
                                    SubId = 0,
                                    RefId = v.ItemId
                                });
                        }
                        long vrId = 0;
                        if (vLst.Sum(x => x.Cr) > 0)
                        {
                            vrId = await accountBL.PostAutoVoucher(vLst, "GRN", mas.GRNNo, mas.GRNDate, UserId);
                            if (vrId == 0)
                            {
                                scop.Dispose();
                            }
                            else
                            {
                                if (!await accountBL.PostingLog("GRN", 56, mas.GRNId, vrId))
                                {
                                    scop.Dispose();
                                }
                            }
                        }
                        ////////////////////////Invoice for Local Purchase////////////////////
                        vrId = await SaveLocalPOInvoice(mas.GRNId);
                        if (vrId == 0)
                            scop.Dispose();
                    }
                    //else
                    //{
                    //    foreach (var v in detialForVoucher)
                    //    {
                    //        var poRate = db.Inv_PODetail.FindAsync(v.PODtlId).Result;
                    //        if (poRate.TP - poRate.Discount - poRate.GST > 0)
                    //            vLst.Add(new VoucherDetailVM
                    //            {
                    //                AccId = invClr,
                    //                CCCode = 72,
                    //                ChequeNo = "",
                    //                Cr = 0,
                    //                Dr = poRate.TP - poRate.Discount - poRate.GST, //- anuAmt - biaAmt - quaAmt - monAmt - whtAmt - gstAmt,
                    //                Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                PCCode = 72,
                    //                SubId = 0,
                    //                RefId = v.ItemId
                    //            });
                    //        //if (poRate.GST > 0)
                    //        //    vLst.Add(new VoucherDetailVM
                    //        //    {
                    //        //        AccId = gstAcc,
                    //        //        CCCode = 72,
                    //        //        ChequeNo = "",
                    //        //        Cr = 0,
                    //        //        Dr = poRate.GST, //- anuAmt - biaAmt - quaAmt - monAmt - whtAmt - gstAmt,
                    //        //        Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //        //        PCCode = 72,
                    //        //        SubId = SuppId,
                    //        //        RefId = v.ItemId
                    //        //    });
                    //        //if (poRate.WHT > 0)
                    //        //    vLst.Add(new VoucherDetailVM
                    //        //    {
                    //        //        AccId = whtAcc,
                    //        //        CCCode = 72,
                    //        //        ChequeNo = "",
                    //        //        Cr = 0,
                    //        //        Dr = poRate.WHT, //- anuAmt - biaAmt - quaAmt - monAmt - whtAmt - gstAmt,
                    //        //        Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //        //        PCCode = 72,
                    //        //        SubId = SuppId,
                    //        //        RefId = v.ItemId
                    //        //    });
                    //        if (poRate.TP - poRate.Discount - poRate.GST > 0)
                    //            vLst.Add(new VoucherDetailVM
                    //            {
                    //                AccId = clrAcc,
                    //                CCCode = 72,
                    //                ChequeNo = "",
                    //                Cr = poRate.TP - poRate.Discount - poRate.GST,
                    //                Dr = 0,
                    //                Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                PCCode = 72,
                    //                SubId = 0,
                    //                RefId = v.ItemId
                    //            });


                    //        if (poRate.TP - poRate.Discount - poRate.GST > 0)
                    //        {
                    //            vLst.Add(new VoucherDetailVM
                    //            {
                    //                AccId = invAcc,
                    //                CCCode = LocId,
                    //                ChequeNo = "",
                    //                Cr = 0,
                    //                Dr = poRate.TP - poRate.Discount - poRate.GST,
                    //                Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                PCCode = LocId,
                    //                SubId = 0,
                    //                RefId = v.ItemId
                    //            });
                    //            vLst.Add(new VoucherDetailVM
                    //            {
                    //                AccId = invClr,
                    //                CCCode = LocId,
                    //                ChequeNo = "",
                    //                Cr = poRate.TP - poRate.Discount - poRate.GST,
                    //                Dr = 0,
                    //                Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                PCCode = LocId,
                    //                SubId = 0,
                    //                RefId = v.ItemId
                    //            });
                    //        }
                    //    }
                    //}

                    

                    //vLst = new List<VoucherDetailVM>();

                    //foreach (var v in detialForVoucher)
                    //{
                    //    if (v.PP > 0)
                    //    {
                    //        var prodId = await db.Inv_Store.Where(x => x.ItemId == v.ItemId).Select(x => x.Itm_Master.Itm_Model.Itm_Type.ProductId).FirstOrDefaultAsync();
                    //        var pdiscList = await db.Itm_PDisc.Where(x => x.FromDate <= mas.GRNDate && x.ToDate >= mas.GRNDate && x.Status && x.SuppId == SuppId && x.ProductId == prodId).ToListAsync();
                    //        if (pdiscList.Count > 0)
                    //        {
                    //            var tourAmt = pdiscList.Where(x => x.PDiscTypeId == 1).OrderByDescending(x => x.RowId).Select(x => (decimal?)x.Amount).FirstOrDefault();
                    //            if ((tourAmt ?? 0) > 0)
                    //            {
                    //                var tourAcc = await accountBL.GetSuppAcc(105, supp.CategoryId);
                    //                var tourIncAcc = await accountBL.GetSuppAcc(112, supp.CategoryId);
                    //                vLst.Add(new VoucherDetailVM
                    //                {
                    //                    AccId = tourAcc,
                    //                    CCCode = 0,
                    //                    ChequeNo = "",
                    //                    Cr = 0,
                    //                    Dr = tourAmt ?? 0,
                    //                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                    PCCode = 72,
                    //                    SubId = SuppId,
                    //                    RefId = v.ItemId
                    //                });
                    //                vLst.Add(new VoucherDetailVM
                    //                {
                    //                    AccId = tourIncAcc,
                    //                    CCCode = 0,
                    //                    ChequeNo = "",
                    //                    Cr = tourAmt ?? 0,
                    //                    Dr = 0,
                    //                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                    PCCode = 72,
                    //                    SubId = SuppId,
                    //                    RefId = v.ItemId
                    //                });
                    //            }
                    //            var dealAmt = await db.Itm_PDisc.Where(x => x.PDiscTypeId == 2).OrderByDescending(x => x.RowId).Select(x => (decimal?)x.Amount).FirstOrDefaultAsync();
                    //            if ((dealAmt ?? 0) > 0)
                    //            {
                    //                var dealAcc = await accountBL.GetSuppAcc(102, supp.CategoryId);
                    //                var dealIncAcc = await accountBL.GetSuppAcc(109, supp.CategoryId);
                    //                vLst.Add(new VoucherDetailVM
                    //                {
                    //                    AccId = dealAcc,
                    //                    CCCode = 0,
                    //                    ChequeNo = "",
                    //                    Cr = 0,
                    //                    Dr = dealAmt ?? 0,
                    //                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                    PCCode = 72,
                    //                    SubId = SuppId,
                    //                    RefId = v.ItemId
                    //                });
                    //                vLst.Add(new VoucherDetailVM
                    //                {
                    //                    AccId = dealIncAcc,
                    //                    CCCode = 0,
                    //                    ChequeNo = "",
                    //                    Cr = dealAmt ?? 0,
                    //                    Dr = 0,
                    //                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                    PCCode = 72,
                    //                    SubId = SuppId,
                    //                    RefId = v.ItemId
                    //                });
                    //            }
                    //            var quatAmt = await db.Itm_PDisc.Where(x => x.PDiscTypeId == 3).OrderByDescending(x => x.RowId).Select(x => (decimal?)x.Amount).FirstOrDefaultAsync();
                    //            if ((quatAmt ?? 0) > 0)
                    //            {
                    //                var quatAcc = await accountBL.GetSuppAcc(104, supp.CategoryId);
                    //                var quatIncAcc = await accountBL.GetSuppAcc(111, supp.CategoryId);
                    //                vLst.Add(new VoucherDetailVM
                    //                {
                    //                    AccId = quatAcc,
                    //                    CCCode = 0,
                    //                    ChequeNo = "",
                    //                    Cr = 0,
                    //                    Dr = quatAmt ?? 0,
                    //                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                    PCCode = 72,
                    //                    SubId = SuppId,
                    //                    RefId = v.ItemId
                    //                });
                    //                vLst.Add(new VoucherDetailVM
                    //                {
                    //                    AccId = quatIncAcc,
                    //                    CCCode = 0,
                    //                    ChequeNo = "",
                    //                    Cr = quatAmt ?? 0,
                    //                    Dr = 0,
                    //                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                    PCCode = 72,
                    //                    SubId = SuppId,
                    //                    RefId = v.ItemId
                    //                });
                    //            }
                    //            var annAmt = await db.Itm_PDisc.Where(x => x.PDiscTypeId == 4).OrderByDescending(x => x.RowId).Select(x => (decimal?)x.Amount).FirstOrDefaultAsync();
                    //            if ((annAmt ?? 0) > 0)
                    //            {
                    //                var annAcc = await accountBL.GetSuppAcc(103, supp.CategoryId);
                    //                var annIncAcc = await accountBL.GetSuppAcc(110, supp.CategoryId);
                    //                vLst.Add(new VoucherDetailVM
                    //                {
                    //                    AccId = annAcc,
                    //                    CCCode = 0,
                    //                    ChequeNo = "",
                    //                    Cr = 0,
                    //                    Dr = annAmt ?? 0,
                    //                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                    PCCode = 72,
                    //                    SubId = SuppId,
                    //                    RefId = v.ItemId
                    //                });
                    //                vLst.Add(new VoucherDetailVM
                    //                {
                    //                    AccId = annIncAcc,
                    //                    CCCode = 0,
                    //                    ChequeNo = "",
                    //                    Cr = annAmt ?? 0,
                    //                    Dr = 0,
                    //                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                    PCCode = 72,
                    //                    SubId = SuppId,
                    //                    RefId = v.ItemId
                    //                });
                    //            }
                    //            var focAmt = await db.Itm_PDisc.Where(x => x.PDiscTypeId == 5).OrderByDescending(x => x.RowId).Select(x => (decimal?)x.Amount).FirstOrDefaultAsync();
                    //            if ((focAmt ?? 0) > 0)
                    //            {
                    //                var focAcc = await accountBL.GetSuppAcc(106, supp.CategoryId);
                    //                var focIncAcc = await accountBL.GetSuppAcc(113, supp.CategoryId);
                    //                vLst.Add(new VoucherDetailVM
                    //                {
                    //                    AccId = focAcc,
                    //                    CCCode = 0,
                    //                    ChequeNo = "",
                    //                    Cr = 0,
                    //                    Dr = focAmt ?? 0,
                    //                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                    PCCode = 72,
                    //                    SubId = SuppId,
                    //                    RefId = v.ItemId
                    //                });
                    //                vLst.Add(new VoucherDetailVM
                    //                {
                    //                    AccId = focIncAcc,
                    //                    CCCode = 0,
                    //                    ChequeNo = "",
                    //                    Cr = focAmt ?? 0,
                    //                    Dr = 0,
                    //                    Particulars = "GRN " + mas.GRNNo + " Sr " + v.Inv_Store.SerialNo,
                    //                    PCCode = 72,
                    //                    SubId = SuppId,
                    //                    RefId = v.ItemId
                    //                });
                    //            }
                    //        }
                    //    }
                    //}

                    //if (vLst.Count > 0)
                    //{
                    //    if (vLst.Sum(x => x.Cr) > 0)
                    //    {
                    //        vrId = await accountBL.PostAutoVoucher(vLst, "DBV", mas.GRNNo, mas.GRNDate, UserId);
                    //        if (vrId == 0)
                    //            scop.Dispose();
                    //    }
                    //}
                    
                    ///////////////////////////////////////////////////////////////////////////////////////
                    scop.Complete();
                    scop.Dispose();
                    return mas.GRNId;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<long> PostPurchaseReturn(long PORId, int UserId)
        {
            try
            {
                var pur = await db.Inv_POReturn.FindAsync(PORId);
                var detialForVoucher = await db.Inv_POReturnDtl.Where(x => x.PORId == PORId).ToListAsync();

                List<VoucherDetailVM> vLst = new List<VoucherDetailVM>();
                List<VoucherDetailVM> crvLst = new List<VoucherDetailVM>();
                var supp = await db.Inv_Suppliers.FindAsync(pur.SuppId);
                var invAcc = await accountBL.GetAcc(7, supp.CategoryId);
                //var clrAcc = await accountBL.GetAcc(2, supp.CategoryId);
                var suppAcc = await accountBL.GetSuppAcc(101, supp.CategoryId);
                var gstAcc = await accountBL.GetSuppAcc(108, supp.CategoryId);
                var whtAcc = await accountBL.GetSuppAcc(107, supp.CategoryId);
                var invClr = await accountBL.GetAcc(448);
                var cashInHand = await accountBL.GetAcc(428);

                foreach (var v in detialForVoucher)
                {
                    var store = await db.Inv_Store.FindAsync(v.ItemId);
                    //decimal tax = Math.Round(store.MRP - (store.MRP * 100 / ((store.Tax ?? 0) + 100)));
                    //decimal wht = await orderBL.GetWHT(pur.SuppId, store.MRP, v.TP, tax, 0);

                    if (supp.SuppId == 341)
                    {
                        if (v.TP - (v.STax ?? 0) > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = invAcc,
                                CCCode = pur.LocId,
                                ChequeNo = "",
                                Cr = v.TP - (v.STax ?? 0),
                                Dr = 0,
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = pur.LocId,
                                SubId = 0,
                                RefId = v.ItemId
                            });
                        if ((v.STax ?? 0) > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = gstAcc,
                                CCCode = pur.LocId,
                                ChequeNo = "",
                                Cr = (v.STax ?? 0),
                                Dr = 0, //- anuAmt - biaAmt - quaAmt - monAmt - whtAmt - gstAmt,
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = pur.LocId,
                                SubId = pur.SuppId,
                                RefId = v.ItemId
                            });
                        if (v.WHT > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = whtAcc,
                                CCCode = pur.LocId,
                                ChequeNo = "",
                                Cr = (v.WHT ?? 0),
                                Dr = 0,
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = pur.LocId,
                                SubId = pur.SuppId,
                                RefId = v.ItemId
                            });
                        if (v.TP + v.WHT > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = suppAcc,
                                CCCode = pur.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = v.TP + (v.WHT ?? 0),
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = pur.LocId,
                                SubId = pur.SuppId,
                                RefId = v.ItemId
                            });

                        if (v.TP + v.WHT > 0)
                            crvLst.Add(new VoucherDetailVM
                            {
                                AccId = cashInHand,
                                CCCode = pur.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = v.TP + (v.WHT ?? 0),
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = pur.LocId,
                                SubId = 0
                            });
                        if (v.TP + v.WHT > 0)
                            crvLst.Add(new VoucherDetailVM
                            {
                                AccId = suppAcc,
                                CCCode = pur.LocId,
                                ChequeNo = "",
                                Cr = v.TP + (v.WHT ?? 0),
                                Dr = 0,
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = pur.LocId,
                                SubId = pur.SuppId,
                                RefId = v.ItemId
                            });
                    }
                    else
                    {
                        if (v.TP - (v.STax ?? 0) > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = invClr,
                                CCCode = 72,
                                ChequeNo = "",
                                Cr = v.TP - (v.STax ?? 0),
                                Dr = 0,
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = 72,
                                SubId = 0,
                                RefId = v.ItemId
                            });
                        if ((v.STax ?? 0) > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = gstAcc,
                                CCCode = 72,
                                ChequeNo = "",
                                Cr = (v.STax ?? 0),
                                Dr = 0, //- anuAmt - biaAmt - quaAmt - monAmt - whtAmt - gstAmt,
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = 72,
                                SubId = pur.SuppId,
                                RefId = v.ItemId
                            });
                        if (v.WHT > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = whtAcc,
                                CCCode = 72,
                                ChequeNo = "",
                                Cr = (v.WHT ?? 0),
                                Dr = 0,
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = 72,
                                SubId = pur.SuppId,
                                RefId = v.ItemId
                            });
                        if (v.TP + v.WHT > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = suppAcc,
                                CCCode = 72,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = v.TP + (v.WHT ?? 0),
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = 72,
                                SubId = pur.SuppId,
                                RefId = v.ItemId
                            });

                        if (v.TP - (v.STax ?? 0) > 0)
                        {
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = invClr,
                                CCCode = pur.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = v.TP - (v.STax ?? 0),
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = pur.LocId,
                                SubId = 0,
                                RefId = v.ItemId
                            });
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = invAcc,
                                CCCode = pur.LocId,
                                ChequeNo = "",
                                Cr = v.TP - (v.STax ?? 0),
                                Dr = 0,
                                Particulars = pur.PORNo + " " + store.Itm_Master.SKUCode + " " + store.SerialNo,
                                PCCode = pur.LocId,
                                SubId = 0,
                                RefId = v.ItemId
                            });
                        }
                    }
                }
                long vrId = 0;
                if (vLst.Sum(x => x.Cr) > 0)
                {
                    vrId = await accountBL.PostAutoVoucher(vLst, "PRV", pur.PORNo, pur.PORDate, UserId);
                    if (vrId == 0)
                    {
                        return 0;
                    }
                }
                if (crvLst.Sum(x => x.Cr) > 0)
                {
                    vrId = await accountBL.PostAutoVoucher(crvLst, "CRV", pur.PORNo, pur.PORDate, UserId);
                    if (vrId == 0)
                    {
                        return 0;
                    }
                }
                return vrId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<long> SaveLocalPOInvoice(long GrnId)
        {
            try
            {
                using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var grn = await db.Inv_GRN.FindAsync(GrnId);
                        grn.Status = "I";

                        List<Inv_PurchaseDetail> lst = new List<Inv_PurchaseDetail>();
                        foreach (var v in grn.Inv_GRNDetail)
                        {
                            if (v.RcvdQty > 0)
                            {
                                Inv_PurchaseDetail item = new Inv_PurchaseDetail
                                {
                                    Discount = 0,
                                    IsGiftItem = false,
                                    Qty = v.RcvdQty,
                                    Rate = v.PP,
                                    SKUId = v.Inv_Store.SKUId,
                                    STax = 0,
                                    WHT = 0,
                                    PODtlId = v.PODtlId,
                                    MRP = v.PP
                                };
                                lst.Add(item);


                            }
                        }

                        var lastInvNo = await db.Inv_Purchase.OrderByDescending(x => x.PInvId).Select(x => x.InvNo).FirstOrDefaultAsync();
                        string InvNo = GetNewInvNo(lastInvNo, "INV-");

                        Inv_Purchase mas = new Inv_Purchase()
                        {
                            RefInvDate = DateTime.Now,
                            RefInvNo = 0,
                            POId = grn.POId,
                            SuppId = grn.SuppId,
                            InvDate = setupBL.GetWorkingDate(grn.LocId),
                            IsReturn = false,
                            Purchaser = "",
                            Remarks = "",
                            Status = "P",
                            TransDate = DateTime.Now,
                            UserId = grn.RcvdBy,
                            ApprovedDate = DateTime.Now,
                            ApprovedBy = grn.RcvdBy,
                            CheckedDate = DateTime.Now,
                            CheckedBy = grn.RcvdBy,
                            VerifiedBy = "",
                            InvNo = InvNo,
                            Inv_PurchaseDetail = lst,
                            PaymentStatus = "N",
                            GRNId = grn.GRNId
                        };
                        db.Inv_Purchase.Add(mas);

                        await db.SaveChangesAsync();

                        List<VoucherDetailVM> vLst = new List<VoucherDetailVM>();
                        var suppAcc = await accountBL.GetSuppAcc(101, 4);
                        var clrAcc = await accountBL.GetAcc(2, 4);
                        //var gstAcc = await accountBL.GetSuppAcc(108, 4);
                        //var whtAcc = await accountBL.GetSuppAcc(107, 4);
                        var det = mas.Inv_PurchaseDetail.ToList();
                        var supp = await db.Inv_Suppliers.FindAsync(grn.SuppId);
                        var suppName = grn.SuppId == 341 ? grn.Inv_PO.LCSuppName ?? "" : supp.SuppName;


                        foreach (var v in det)
                        {
                            var itm = await db.Itm_Master.FindAsync(v.SKUId);
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = clrAcc,
                                CCCode = grn.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = (v.Rate - v.Discount) * v.Qty,
                                Particulars = "Local (" + suppName + ") " + itm.SKUCode + " (" + v.Qty.ToString() + "@" + (v.Rate - v.Discount).ToString() + ")",
                                PCCode = grn.LocId,
                                SubId = 0,
                                RefId = v.PInvDtl
                            });
                            //if (v.STax > 0)
                            //    vLst.Add(new VoucherDetailVM
                            //    {
                            //        AccId = gstAcc,
                            //        CCCode = grn.LocId,
                            //        ChequeNo = "",
                            //        Cr = 0,
                            //        Dr = v.STax * v.Qty,
                            //        Particulars = "Local (" + suppName + ") " + itm.SKUCode + " (" + v.Qty.ToString() + "@" + (v.Rate - v.Discount).ToString() + ")",
                            //        PCCode = grn.LocId,
                            //        SubId = grn.SuppId,
                            //        RefId = v.PInvDtl
                            //    });
                            //if (v.WHT > 0)
                            //    vLst.Add(new VoucherDetailVM
                            //    {
                            //        AccId = whtAcc,
                            //        CCCode = grn.LocId,
                            //        ChequeNo = "",
                            //        Cr = 0,
                            //        Dr = v.WHT * v.Qty, //- anuAmt - biaAmt - quaAmt - monAmt - whtAmt - gstAmt,
                            //        Particulars = "Local (" + suppName + ") " + itm.SKUCode + " (" + v.Qty.ToString() + "@" + (v.Rate - v.Discount).ToString() + ")",
                            //        PCCode = grn.LocId,
                            //        SubId = grn.SuppId,
                            //        RefId = v.PInvDtl
                            //    });
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = suppAcc,
                                CCCode = grn.LocId,
                                ChequeNo = "",
                                Cr = (v.Rate - v.Discount/*+v.STax+v.WHT*/) * v.Qty,
                                Dr = 0,
                                Particulars = "Local (" + suppName + ") " + itm.SKUCode + " (" + v.Qty.ToString() + "@" + (v.Rate - v.Discount).ToString() + ")",
                                PCCode = grn.LocId,
                                SubId = grn.SuppId,
                                RefId = v.PInvDtl
                            });
                        }
                        if (vLst.Sum(x => x.Cr) > 0)
                        {
                            var vrId = await accountBL.PostAutoVoucher(vLst, "PIV", mas.InvNo, mas.InvDate, mas.UserId);
                            if (vrId == 0)
                                scop.Dispose();
                        }

                        scop.Complete();
                        scop.Dispose();
                        return mas.PInvId;
                    }
                    catch (Exception)
                    {
                        scop.Dispose();
                        return 0;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string GetNewInvNo(string lastInvNo, string prefix)
        {
            //string InvNo = DateTime.Now.ToString("yyMM");
            //if (lastInvNo.Length == 8)
            //{
            //    if (lastInvNo == null)
            //    {
            //        InvNo = "INV-" + DateTime.Now.ToString("yyMM");
            //        InvNo = InvNo + "000001";
            //    }
            //    else if (lastInvNo.Substring(0, 4) == InvNo)
            //    {
            //        InvNo = "INV-" + DateTime.Now.ToString("yyMM");
            //        InvNo = InvNo + (Convert.ToInt32(lastInvNo.Substring(4, 4)) + 1).ToString("000000");
            //    }
            //    else
            //    {
            //        InvNo = "INV-" + DateTime.Now.ToString("yyMM");
            //        InvNo = InvNo + "000001";
            //    }
            //}
            //else
            //{
            string InvNo = prefix + DateTime.Now.ToString("yyMM");
            if (lastInvNo == null)
            {
                InvNo = InvNo + "000001";
            }
            else if (lastInvNo.Substring(0, 8) == InvNo)
            {
                InvNo = InvNo + (Convert.ToInt32(lastInvNo.Substring(8, 6)) + 1).ToString("000000");
            }
            else
            {
                InvNo = InvNo + "000001";
            }
            //}
            return InvNo;
        }

        public async Task<long> SavePOInv(IEnumerable<POInvoiceDetailVM> mod, int GRNId, string DONo, DateTime DODate, int SuppId, long RefInvNo,
            DateTime RefInvDate, string Remarks, int LocId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var grn = await db.Inv_GRN.FindAsync(GRNId);
                    if (grn.Status == "I")
                        return 0;

                    grn.Status = "I";
                    grn.DONo = DONo;
                    grn.DODate = DODate;

                    List<Inv_PurchaseDetail> lst = new List<Inv_PurchaseDetail>();
                    foreach (var v in mod)
                    {
                        if (v.Qty > 0)
                        {
                            Inv_PurchaseDetail item = new Inv_PurchaseDetail
                            {
                                Discount = v.Discount,
                                IsGiftItem = v.IsGiftItem,
                                Qty = v.Qty,
                                Rate = v.TP - v.GST,
                                SKUId = v.SKUId,
                                STax = v.GST,
                                WHT = v.WHT,
                                PODtlId = v.PODtlId,
                                MRP = v.MRP
                            };
                            lst.Add(item);

                            var store = await (from GD in db.Inv_GRNDetail
                                   join ST in db.Inv_Store on GD.ItemId equals ST.ItemId
                                   where GD.PODtlId == v.PODtlId && GD.GRNId == GRNId
                                   select ST).ToListAsync();

                            foreach (var x in store)
                            {
                                if (x.MRP != v.MRP)
                                {
                                    x.MRP = v.MRP;
                                }
                                if (x.WHT != v.WHT)
                                {
                                    x.WHT = v.WHT;
                                }
                            }
                        }
                    }

                    var lastInvNo = await db.Inv_Purchase.OrderByDescending(x => x.PInvId).Select(x => x.InvNo).FirstOrDefaultAsync();
                    string InvNo = GetNewInvNo(lastInvNo, "INV-");

                    Inv_Purchase mas = new Inv_Purchase()
                    {
                        RefInvDate = RefInvDate,
                        RefInvNo = RefInvNo,
                        POId = grn.POId,
                        SuppId = SuppId,
                        InvDate = setupBL.GetWorkingDate(LocId),
                        IsReturn = false,
                        Purchaser = "",
                        Remarks = Remarks,
                        Status = "U",
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        VerifiedBy = "",
                        InvNo = InvNo,
                        Inv_PurchaseDetail = lst,
                        PaymentStatus = "N",
                        GRNId = GRNId
                    };
                    db.Inv_Purchase.Add(mas);

                    //var po = await db.Inv_PO.FindAsync(POId);
                    //po.Status = 7;

                    await db.SaveChangesAsync();

                    scop.Complete();
                    scop.Dispose();
                    return mas.PInvId;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<bool> PostSupplierPayment(List<long> TransLst, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var v in TransLst)
                    {
                        var tbl = await db.Inv_SuppPayment.FindAsync(v);

                        if (tbl.IsPosted)
                            return false;

                        if (tbl.PaidStatus != "P")
                            return false;

                        tbl.IsPosted = true;
                        tbl.PostedBy = UserId;
                        tbl.PostedDate = DateTime.Now;

                        long suppAcc = (long)tbl.AccId;
                        string SuppName = "";
                        decimal TaxRate = 0;
                        var acc = await db.Fin_Accounts.Where(x => x.AccId == tbl.AccId).FirstOrDefaultAsync();
                        if (acc.SubDivId == 1)
                        {
                            var supp = await db.Fin_Subsidary.Where(x => x.SubId == tbl.SuppId).FirstOrDefaultAsync();
                            SuppName = supp.SubsidaryName;
                            TaxRate = supp.TaxRate ?? 0;
                        }
                        else if (acc.SubDivId == 2)
                        {
                            var supp = await db.Inv_Suppliers.Where(x => x.SuppId == tbl.SuppId).FirstOrDefaultAsync();
                            SuppName = supp.SuppName;
                            TaxRate = supp.TaxRate;
                        }

                        //await db.Inv_Suppliers.FindAsync(tbl.SuppId);
                        //await accountBL.GetSuppAcc(101, supp.CategoryId);

                        if (tbl.CashAmount > 0)
                        {
                            if (tbl.LocId != 72)
                            {
                                var CPVLst = new List<VoucherDetailVM>();
                                var cashInHand = await accountBL.GetAcc(428);
                                var cashClearing = await accountBL.GetAcc(450);

                                CPVLst.Add(new VoucherDetailVM
                                {
                                    AccId = suppAcc,
                                    CCCode = 72,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = tbl.CashAmount,
                                    Particulars = "Paid to " + SuppName,
                                    PCCode = 72,
                                    SubId = tbl.SuppId
                                });

                                CPVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClearing,
                                    CCCode = 72,
                                    ChequeNo = "",
                                    Cr = tbl.CashAmount,
                                    Dr = 0,
                                    Particulars = "Paid to " + SuppName,
                                    PCCode = 72,
                                    SubId = 0
                                });



                                CPVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClearing,
                                    CCCode = tbl.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = tbl.CashAmount,
                                    Particulars = "Paid to " + SuppName,
                                    PCCode = tbl.LocId,
                                    SubId = 0
                                });

                                CPVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = tbl.LocId,
                                    ChequeNo = "",
                                    Cr = tbl.CashAmount,
                                    Dr = 0,
                                    Particulars = "Paid to " + SuppName,
                                    PCCode = tbl.LocId,
                                    SubId = 0
                                });


                                var vrId = await accountBL.PostAutoVoucher(CPVLst, "CPV", tbl.TransId.ToString(), tbl.PaidDate.Value.Date, UserId);
                                if (vrId == 0)
                                {
                                    scop.Dispose();
                                    return false;
                                }
                            }
                            else
                            {
                                var CPVLst = new List<VoucherDetailVM>();
                                var cashInHand = await accountBL.GetAcc(428);
                                var cashClearing = await accountBL.GetAcc(450);

                                CPVLst.Add(new VoucherDetailVM
                                {
                                    AccId = suppAcc,
                                    CCCode = tbl.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = tbl.CashAmount,
                                    Particulars = "Paid to " + SuppName,
                                    PCCode = tbl.LocId,
                                    SubId = tbl.SuppId
                                });
                                CPVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = tbl.LocId,
                                    ChequeNo = "",
                                    Cr = tbl.CashAmount,
                                    Dr = 0,
                                    Particulars = "Paid to " + SuppName,
                                    PCCode = tbl.LocId,
                                    SubId = 0
                                });


                                var vrId = await accountBL.PostAutoVoucher(CPVLst, "CPV", tbl.TransId.ToString(), tbl.PaidDate.Value.Date, UserId);
                                if (vrId == 0)
                                {
                                    scop.Dispose();
                                    return false;
                                }
                            }

                        }
                        if (tbl.BankAmount > 0)
                        {
                            var BPVLst = new List<VoucherDetailVM>();
                            var WHTLst = new List<VoucherDetailVM>();
                            var BankAccId = (long)tbl.BankAccId;

                            BPVLst.Add(new VoucherDetailVM
                            {
                                AccId = suppAcc,
                                CCCode = 72,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = tbl.BankAmount,
                                Particulars = "Paid to " + SuppName + (tbl.Instrument != "" ? " Instrument " : "") + tbl.Instrument + (tbl.Instrument != "" ? " Instrument No" : "") + tbl.InstrumentNo,
                                PCCode = 72,
                                SubId = tbl.SuppId
                            });
                            BPVLst.Add(new VoucherDetailVM
                            {
                                AccId = BankAccId,
                                CCCode = 72,
                                ChequeNo = tbl.ChequeNo,
                                Cr = tbl.BankAmount,
                                Dr = 0,
                                Particulars = "Paid to " + SuppName + (tbl.Instrument != "" ? " Instrument " : "") + tbl.Instrument + (tbl.Instrument != "" ? " Instrument No" : "") + tbl.InstrumentNo,
                                PCCode = 72,
                                SubId = 0
                            });
                            if (acc.SubDivId == 2)
                            {
                                var supp = await db.Inv_Suppliers.FindAsync(tbl.SuppId);
                                var suppArr = new long[] { 20010100010, 20011000010, 20012000010, 20013000010 };
                                decimal taxRate = 0;
                                var tax = await db.Inv_SuppTaxExemption.Where(x => x.SuppId == supp.SuppId
                                && x.FromDate <= tbl.PaidDate.Value && x.ToDate >= tbl.PaidDate.Value && x.Status).FirstOrDefaultAsync();
                                if (tax != null)
                                {
                                    taxRate = tax.TaxRate;
                                }
                                else
                                {
                                    taxRate = supp.TaxRate;
                                }
                                if (suppArr.Contains(suppAcc) && taxRate > 0)
                                {
                                    decimal wht = Math.Round((tbl.BankAmount * taxRate) / (100 - taxRate), 0);

                                    var whtAcc = await accountBL.GetSuppAcc(114, supp.CategoryId);
                                    WHTLst.Add(new VoucherDetailVM
                                    {
                                        AccId = suppAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = wht,
                                        Particulars = "WHT of " + SuppName,
                                        PCCode = 72,
                                        SubId = supp.SuppId
                                    });
                                    WHTLst.Add(new VoucherDetailVM
                                    {
                                        AccId = whtAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = wht,
                                        Dr = 0,
                                        Particulars = "WHT of " + SuppName,
                                        PCCode = 72,
                                        SubId = supp.SuppId
                                    });
                                }
                            }
                            else
                            {
                                if (TaxRate > 0)
                                {
                                    decimal wht = Math.Round((tbl.BankAmount * TaxRate) / (100 - TaxRate), 0);
                                    var whtAcc = await accountBL.GetAcc(465);
                                    WHTLst.Add(new VoucherDetailVM
                                    {
                                        AccId = suppAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = wht,
                                        Particulars = "WHT of " + SuppName,
                                        PCCode = 72,
                                        SubId = tbl.SuppId
                                    });
                                    WHTLst.Add(new VoucherDetailVM
                                    {
                                        AccId = whtAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = wht,
                                        Dr = 0,
                                        Particulars = "WHT of " + SuppName,
                                        PCCode = 72,
                                        SubId = tbl.SuppId
                                    });
                                }
                            }
                            var vrId = await accountBL.PostAutoVoucher(BPVLst, "BPV", tbl.TransId.ToString(), tbl.PaidDate.Value.Date, UserId);
                            if (vrId == 0)
                            {
                                scop.Dispose();
                                return false;
                            }
                            if (WHTLst.Count > 0)
                            {
                                vrId = await accountBL.PostAutoVoucher(WHTLst, "WHT", tbl.TransId.ToString(), tbl.PaidDate.Value.Date, UserId);
                            }
                            if (vrId == 0)
                            {
                                scop.Dispose();
                                return false;
                            }
                        }

                        await db.SaveChangesAsync();
                    }
                    scop.Complete();
                    scop.Dispose();
                    return true;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }
        //public async Task<long> SavePaySupplierPayment(SuppPaymentVM mod,int LocId,int UserId)
        //{
        //    using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {
        //            var tbl = await db.Inv_SuppPayment.FindAsync(mod.TransId);
        //            decimal bankWHT = 0;
        //            decimal cashWHT = 0;
        //            if(mod.BankAmount > 0)
        //            {
        //                tbl.BankAccId = mod.BankAccId;
        //                tbl.BankAmount = mod.BankAmount;
        //            }
        //            if(mod.CashAmount > 0)
        //            {
        //                tbl.CashAmount = mod.Payment - mod.BankAmount;
        //            }
        //            if(tbl.WHT > 0)
        //            {
        //                bankWHT = Math.Round(tbl.BankAmount / (tbl.BankAmount + tbl.CashAmount) * tbl.WHT,0);
        //                cashWHT = Math.Round(tbl.CashAmount / (tbl.BankAmount + tbl.CashAmount) * tbl.WHT, 0);
        //            }
        //            tbl.PaidBy = UserId;
        //            tbl.PaidDate = DateTime.Now;
        //            tbl.PaidStatus = "P";

        //            //var tbl = await db.Inv_POPayment.FindAsync(v.TransId);
        //            //if (tbl.IsPosted)
        //            //    continue;

        //            //tbl.IsPosted = true;
        //            //tbl.PostedBy = UserId;
        //            //tbl.PostedDate = DateTime.Now;

        //            var supp = await db.Inv_Suppliers.FindAsync(tbl.SuppId);
        //            var suppAcc = await accountBL.GetSuppAcc(101, supp.CategoryId);
        //            var wht = await accountBL.GetAcc(457);
        //            var WorkingDate = setupBL.GetWorkingDate(LocId);
        //            if (tbl.CashAmount > 0)
        //            {
        //                var CPVLst = new List<VoucherDetailVM>();
        //                var cashInHand = await accountBL.GetAcc(428);

        //                CPVLst.Add(new VoucherDetailVM
        //                {
        //                    AccId = suppAcc,
        //                    CCCode = 0,
        //                    ChequeNo = "",
        //                    Cr = 0,
        //                    Dr = tbl.CashAmount+cashWHT,
        //                    Particulars = "Paid to "+supp.SuppName,
        //                    PCCode = LocId,
        //                    SubId = tbl.SuppId
        //                });
        //                CPVLst.Add(new VoucherDetailVM
        //                {
        //                    AccId = cashInHand,
        //                    CCCode = 0,
        //                    ChequeNo = "",
        //                    Cr = tbl.CashAmount,
        //                    Dr = 0,
        //                    Particulars = "Paid to " + supp.SuppName,
        //                    PCCode = LocId,
        //                    SubId = 0
        //                });
        //                if(cashWHT > 0)
        //                {
        //                    CPVLst.Add(new VoucherDetailVM
        //                    {
        //                        AccId = wht,
        //                        CCCode = 0,
        //                        ChequeNo = "",
        //                        Cr = cashWHT,
        //                        Dr = 0,
        //                        Particulars = "WHT " + supp.SuppName,
        //                        PCCode = LocId,
        //                        SubId = 0
        //                    });
        //                }

        //                var vrId = await accountBL.PostAutoVoucher(CPVLst, "CPV", tbl.TransId.ToString(), WorkingDate, UserId);
        //                if (vrId == 0)
        //                {
        //                    scop.Dispose();
        //                    return 0;
        //                }


        //                if (tbl.IsWHTPaid && cashWHT > 0)
        //                {
        //                    CPVLst = new List<VoucherDetailVM>();

        //                    CPVLst.Add(new VoucherDetailVM
        //                    {
        //                        AccId = wht,
        //                        CCCode = 0,
        //                        ChequeNo = "",
        //                        Cr = 0,
        //                        Dr = cashWHT,
        //                        Particulars = "WHT Paid " + supp.SuppName,
        //                        PCCode = LocId,
        //                        SubId = tbl.SuppId
        //                    });
        //                    CPVLst.Add(new VoucherDetailVM
        //                    {
        //                        AccId = cashInHand,
        //                        CCCode = 0,
        //                        ChequeNo = "",
        //                        Cr = cashWHT,
        //                        Dr = 0,
        //                        Particulars = "WHT Paid " + supp.SuppName,
        //                        PCCode = LocId,
        //                        SubId = 0
        //                    });


        //                    vrId = await accountBL.PostAutoVoucher(CPVLst, "CPV", tbl.TransId.ToString(), WorkingDate, UserId);
        //                    if (vrId == 0)
        //                    {
        //                        scop.Dispose();
        //                        return 0;
        //                    }

        //                }
        //            }
        //            if (tbl.BankAmount > 0)
        //            {
        //                var BPVLst = new List<VoucherDetailVM>();
        //                var BankAccId = (long)tbl.BankAccId;

        //                BPVLst.Add(new VoucherDetailVM
        //                {
        //                    AccId = suppAcc,
        //                    CCCode = 0,
        //                    ChequeNo = "",
        //                    Cr = 0,
        //                    Dr = tbl.BankAmount,
        //                    Particulars = "Paid to " + supp.SuppName + (tbl.Instrument != "" ? " Instrument " : "") + tbl.Instrument + (tbl.Instrument != "" ? " Instrument No" : "") + tbl.InstrumentNo,
        //                    PCCode = LocId,
        //                    SubId = supp.SuppId
        //                });
        //                BPVLst.Add(new VoucherDetailVM
        //                {
        //                    AccId = BankAccId,
        //                    CCCode = 0,
        //                    ChequeNo = "",
        //                    Cr = tbl.BankAmount,
        //                    Dr = 0,
        //                    Particulars = "Paid to " + supp.SuppName + (tbl.Instrument != "" ? " Instrument " : "") + tbl.Instrument + (tbl.Instrument != "" ? " Instrument No" : "") + tbl.InstrumentNo,
        //                    PCCode = LocId,
        //                    SubId = 0
        //                });
        //                if (bankWHT > 0)
        //                {
        //                    BPVLst.Add(new VoucherDetailVM
        //                    {
        //                        AccId = wht,
        //                        CCCode = 0,
        //                        ChequeNo = "",
        //                        Cr = bankWHT,
        //                        Dr = 0,
        //                        Particulars = "WHT " + supp.SuppName,
        //                        PCCode = LocId,
        //                        SubId = 0
        //                    });
        //                }

        //                var vrId = await accountBL.PostAutoVoucher(BPVLst, "BPV", tbl.TransId.ToString(), WorkingDate, UserId);
        //                if (vrId == 0)
        //                {
        //                    scop.Dispose();
        //                    return 0;
        //                }


        //                if (tbl.IsWHTPaid && bankWHT > 0)
        //                {
        //                    BPVLst = new List<VoucherDetailVM>();
        //                    BPVLst.Add(new VoucherDetailVM
        //                    {
        //                        AccId = wht,
        //                        CCCode = 0,
        //                        ChequeNo = "",
        //                        Cr = 0,
        //                        Dr = bankWHT,
        //                        Particulars = "WHT Paid " + supp.SuppName + (tbl.Instrument != "" ? " Instrument " : "") + tbl.Instrument + (tbl.Instrument != "" ? " Instrument No" : "") + tbl.InstrumentNo,
        //                        PCCode = LocId,
        //                        SubId = tbl.SuppId
        //                    });
        //                    BPVLst.Add(new VoucherDetailVM
        //                    {
        //                        AccId = BankAccId,
        //                        CCCode = 0,
        //                        ChequeNo = "",
        //                        Cr = bankWHT,
        //                        Dr = 0,
        //                        Particulars = "WHT Paid to " + supp.SuppName + (tbl.Instrument != "" ? " Instrument " : "") + tbl.Instrument + (tbl.Instrument != "" ? " Instrument No" : "") + tbl.InstrumentNo,
        //                        PCCode = LocId,
        //                        SubId = 0
        //                    });


        //                    vrId = await accountBL.PostAutoVoucher(BPVLst, "BPV", tbl.TransId.ToString(), WorkingDate, UserId);
        //                    if (vrId == 0)
        //                    {
        //                        scop.Dispose();
        //                        return 0;
        //                    }

        //                }
        //            }

        //            await db.SaveChangesAsync();
        //            scop.Complete();
        //            scop.Dispose();
        //            return tbl.TransId;
        //        }
        //        catch (Exception)
        //        {
        //            scop.Dispose();
        //            return 0;
        //        }
        //    }
        //}
        public async Task<long> SavePaySupplierPayment(SuppPaymentVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var tbl = await db.Inv_SuppPayment.FindAsync(mod.TransId);
                    if (!string.IsNullOrEmpty(tbl.ChequeNo))
                    {
                        var cheq = await db.Fin_BankBookTrans.Where(x => x.AccId == mod.BankAccId && x.ChequeNo == tbl.ChequeNo).FirstOrDefaultAsync();
                        if (cheq != null)
                        {
                            cheq.ChequeType = null;
                            cheq.ChequeDate = null;
                            cheq.Amount = null;
                            cheq.PaymentType = null;
                            cheq.Recipient = null;
                            cheq.ClearDate = null;
                            cheq.InstrumentType = null;
                            cheq.VoidDate = null;
                            cheq.Status = null;
                            cheq.Remarks = null;
                            cheq.UserId = null;
                            cheq.TransDate = null;
                            cheq.InstrumentNo = null;
                        }
                    }
                    if (!string.IsNullOrEmpty(mod.ChequeNo))
                    {
                        var fbt = await db.Fin_BankBookTrans.Where(x => x.AccId == mod.BankAccId && x.ChequeNo == mod.ChequeNo).FirstOrDefaultAsync();
                        if (fbt != null)
                        {
                            fbt.Amount = mod.BankAmount;
                            fbt.ChequeDate = mod.PaidDate;
                            fbt.ChequeNo = mod.ChequeNo;
                            fbt.ChequeType = "Presentable";
                            fbt.PaymentType = "Supplier Payment";
                            fbt.Recipient = mod.SuppName;
                            fbt.InstrumentType = mod.Instrument;
                            fbt.Status = "U";
                            fbt.UserId = UserId;
                            fbt.TransDate = DateTime.Now;
                            fbt.InstrumentNo = mod.InstrumentNo;
                            fbt.Remarks = mod.Remarks;
                        }
                    }
                    //decimal bankWHT = 0;
                    //decimal cashWHT = 0;
                    tbl.BankAmount = mod.BankAmount;
                    tbl.BankAccId = mod.BankAccId;
                    tbl.CashAmount = mod.CashAmount;

                    tbl.Payment = tbl.BankAmount + tbl.CashAmount;
                    tbl.Instrument = mod.Instrument;
                    tbl.InstrumentNo = mod.InstrumentNo;
                    tbl.ChequeNo = mod.ChequeNo;

                    tbl.Remarks = mod.Remarks;

                    //if (tbl.WHT > 0)
                    //{
                    //    bankWHT = Math.Round(tbl.BankAmount / (tbl.BankAmount + tbl.CashAmount) * tbl.WHT, 0);
                    //    cashWHT = Math.Round(tbl.CashAmount / (tbl.BankAmount + tbl.CashAmount) * tbl.WHT, 0);
                    //}
                    tbl.PaidBy = UserId;
                    tbl.PaidDate = mod.PaidDate;
                    tbl.PaidStatus = "P";

                    await db.SaveChangesAsync();
                    scop.Complete();
                    scop.Dispose();
                    return tbl.TransId;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<SuppPaymentVM> EditPaySupplierPayment(SuppPaymentVM mod, int UserId)
        {
            //using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
            try
            {
                var tbl = await db.Inv_SuppPayment.FindAsync(mod.TransId);
                if (tbl.IsPosted)
                {
                    return null;
                }
                if (tbl.ChequeNo == mod.ChequeNo)
                {
                    //var cheq = await db.Fin_BankBookTrans.Where(x => x.AccId == mod.BankAccId && x.Status == null && x.ChequeNo == mod.ChequeNo).FirstOrDefaultAsync();
                    //if (cheq == null)
                    //{
                    //    var chq = await db.Fin_BankBook.Where(x => x.AccId == mod.BankAccId).FirstOrDefaultAsync();
                    //    //chq.CurrentChqNo = (Convert.ToInt64(chq.CurrentChqNo) + 1).ToString();
                    //}
                    //else
                    //{
                    //    return null;
                    //}
                }
                else
                {
                    var cheq = await db.Fin_BankBookTrans.Where(x => x.AccId == mod.BankAccId && x.ChequeNo == tbl.ChequeNo).FirstOrDefaultAsync();
                    if (cheq != null)
                    {
                        cheq.ChequeType = null;
                        cheq.ChequeDate = null;
                        cheq.Amount = null;
                        cheq.PaymentType = null;
                        cheq.Recipient = null;
                        cheq.ClearDate = null;
                        cheq.InstrumentType = null;
                        cheq.VoidDate = null;
                        cheq.Status = null;
                        cheq.Remarks = null;
                        cheq.UserId = null;
                        cheq.TransDate = null;
                        cheq.InstrumentNo = null;
                        await db.SaveChangesAsync();
                    }

                    var fbt = await db.Fin_BankBookTrans.Where(x => x.AccId == mod.BankAccId && x.ChequeNo == mod.ChequeNo).FirstOrDefaultAsync();
                    if (fbt != null)
                    {
                        fbt.Amount = mod.BankAmount;
                        fbt.ChequeDate = mod.PaidDate;
                        fbt.ChequeNo = mod.ChequeNo;
                        fbt.ChequeType = "Presentable";
                        fbt.PaymentType = "Supplier Payment";
                        fbt.Recipient = mod.SuppName;
                        fbt.InstrumentType = mod.Instrument;
                        fbt.Status = "U";
                        fbt.UserId = UserId;
                        fbt.TransDate = DateTime.Now;
                        fbt.InstrumentNo = mod.InstrumentNo;
                        fbt.Remarks = mod.Remarks;
                        await db.SaveChangesAsync();
                    }
                }

                //decimal bankWHT = 0;
                //decimal cashWHT = 0;
                //tbl.AccId = mod.AccId;
                tbl.BankAccId = mod.BankAccId;
                tbl.BankAmount = mod.BankAmount;
                tbl.CashAmount = mod.CashAmount;
                tbl.Instrument = mod.Instrument;
                tbl.InstrumentNo = mod.InstrumentNo;
                tbl.Payment = tbl.CashAmount + tbl.BankAmount;
                tbl.PaidDate = mod.PaidDate;
                tbl.Remarks = mod.Remarks;
                tbl.ChequeNo = mod.ChequeNo;
                //tbl.PaidBy = UserId;
                //tbl.PaidDate = DateTime.Now;
                //tbl.PaidStatus = "P";

                await db.SaveChangesAsync();
                //scop.Complete();
                //scop.Dispose();
                mod.Payment = tbl.Payment;

                

                return mod;
            }
            catch (Exception)
            {
                //scop.Dispose();
                return null;
            }
            //}
        }
        public async Task<long> DeletePaySupplierPayment(SuppPaymentVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Inv_SuppPayment.FindAsync(mod.TransId);
                if (tbl.IsPosted)
                {
                    return 0;
                }
                var cheq = await db.Fin_BankBookTrans.Where(x => x.AccId == tbl.BankAccId && x.ChequeNo == tbl.ChequeNo).FirstOrDefaultAsync();
                if (cheq != null)
                {
                    cheq.ChequeType = null;
                    cheq.ChequeDate = null;
                    cheq.Amount = null;
                    cheq.PaymentType = null;
                    cheq.Recipient = null;
                    cheq.ClearDate = null;
                    cheq.InstrumentType = null;
                    cheq.VoidDate = null;
                    cheq.Status = null;
                    cheq.Remarks = null;
                    cheq.UserId = null;
                    cheq.TransDate = null;
                    cheq.InstrumentNo = null;
                }
                tbl.PaidStatus = "C";
                tbl.PostedBy = UserId;
                tbl.PostedDate = DateTime.Now;
                await db.SaveChangesAsync();
                return tbl.TransId;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<bool> SavePaymentAdvice(IEnumerable<SuppPaymentVM> mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    DateTime workingDate = setupBL.GetWorkingDate(72);
                    foreach (var item in mod)
                    {
                        if (item.BankAmount > 0 || item.CashAmount > 0)
                        {
                            Inv_SuppPayment payment = new Inv_SuppPayment
                            {
                                BankAmount = item.BankAmount,
                                CashAmount = item.CashAmount,
                                BankAccId = item.BankAccId,
                                ClosingBalance = item.ClosingBalance,
                                Disc = 0,
                                IsWHTPaid = false,
                                Payment = 0,
                                Proposed = item.BankAmount + item.CashAmount,
                                SuppId = item.SuppId,
                                TransDate = DateTime.Now,
                                UserId = UserId,
                                WHT = 0,
                                PaidStatus = "U",
                                WorkingDate = workingDate,
                                Remarks = item.Remarks,
                                LocId = item.LocId,
                                AccId = item.AccId
                            };
                            db.Inv_SuppPayment.Add(payment);
                            await db.SaveChangesAsync();
                        }
                    }
                    scop.Complete();
                    scop.Dispose();
                    return true;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }
        public async Task<decimal> GetSuppBalance(long AccId, int SuppId)
        {
            try
            {
                return db.spget_SupplierBalance(AccId, SuppId).Select(x => x.Value).FirstOrDefault();
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<bool> SaveSupplierPayment(IEnumerable<InvForPaymentVM> mod, int SuppId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    DateTime workingDate = setupBL.GetWorkingDate(72);

                    //decimal TotalAmount = 0;
                    //decimal Disc = 0;
                    //decimal WHT = 0;

                    List<Inv_POPayment> payLst = new List<Inv_POPayment>();
                    foreach (var v in mod)
                    {
                        var pur = await db.Inv_Purchase.FindAsync(v.PInvId);
                        if (pur.PaymentStatus == "C")
                        {
                            scop.Dispose();
                            return false;
                        }
                        if (v.Amount == v.PaidAmount)
                        {

                            //Disc += v.Discount ?? 0;
                            //WHT += v.WHT ?? 0;
                            pur.PaymentStatus = "C";
                        }
                        else if (v.Amount > v.PaidAmount && v.PaidAmount > 0)
                        {
                            //Disc += v.PaidAmount / v.Amount * v.Discount ?? 0;
                            //WHT += v.PaidAmount / v.Amount * v.WHT ?? 0;
                            pur.PaymentStatus = "P";
                        }
                        if (v.PaidAmount > 0)
                        {
                            //TotalAmount += v.PaidAmount;
                            Inv_POPayment pay = new Inv_POPayment
                            {
                                Amount = v.PaidAmount,
                                IsPosted = true,
                                LocId = 72,
                                PaidBy = "Auto",
                                POInvId = v.PInvId,
                                PostedBy = UserId,
                                PostedDate = workingDate,
                                Remarks = "",
                                TransDate = DateTime.Now,
                                UserId = UserId,
                                WorkingDate = workingDate,
                            };
                            payLst.Add(pay);
                        }
                    }
                    //var supp = await db.Inv_Suppliers.FindAsync(SuppId);
                    //WHT = (supp.TaxRate / TotalAmount) * 100;
                    //Inv_SuppPayment payment = new Inv_SuppPayment
                    //{
                    //    BankAmount = 0,
                    //    CashAmount = 0,
                    //    ClosingBalance = 0,
                    //    Disc = Math.Round(Disc, 0),
                    //    IsWHTPaid = false,
                    //    Payment = Math.Round(TotalAmount, 0)- Math.Round(WHT, 0),
                    //    SuppId = SuppId,
                    //    TransDate = DateTime.Now,
                    //    UserId = UserId,
                    //    WHT = Math.Round(WHT, 0),
                    //    PaidStatus = "U"
                    //};
                    //db.Inv_SuppPayment.Add(payment);
                    //await db.SaveChangesAsync();

                    //payLst.ForEach(x => x.PaymentId = payment.TransId);

                    db.Inv_POPayment.AddRange(payLst);
                    await db.SaveChangesAsync();
                    scop.Complete();
                    scop.Dispose();
                    return true;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }
        public async Task<long> SavePOInvoice(IEnumerable<POInvoiceDetailVM> mod, int POId, int SuppId, int RefInvNo,
            DateTime RefInvDate, string Remarks, int LocId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    List<Inv_PurchaseDetail> lst = new List<Inv_PurchaseDetail>();
                    foreach (var v in mod)
                    {
                        if (v.Qty > 0)
                        {
                            Inv_PurchaseDetail item = new Inv_PurchaseDetail
                            {
                                Discount = v.Discount,
                                IsGiftItem = v.IsGiftItem,
                                Qty = v.Qty,
                                Rate = v.TP,
                                SKUId = v.SKUId,
                                STax = v.GST,
                                WHT = v.WHT
                            };
                            lst.Add(item);
                        }
                    }

                    //var lastInvId = await db.Inv_Purchase.MaxAsync(x => x.PInvId);
                    var lastInvNo = await db.Inv_Purchase.OrderByDescending(x => x.PInvId).Select(x => x.InvNo).FirstOrDefaultAsync();

                    string InvNo = GetNewInvNo(lastInvNo, "INV-");

                    Inv_Purchase mas = new Inv_Purchase()
                    {
                        RefInvDate = RefInvDate,
                        RefInvNo = RefInvNo,
                        POId = POId,
                        SuppId = SuppId,
                        InvDate = setupBL.GetWorkingDate(LocId),
                        IsReturn = false,
                        Purchaser = "",
                        Remarks = Remarks,
                        Status = "U",
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        VerifiedBy = "",
                        InvNo = InvNo,
                        Inv_PurchaseDetail = lst,
                        PaymentStatus = "N"
                    };
                    db.Inv_Purchase.Add(mas);

                    var po = await db.Inv_PO.FindAsync(POId);
                    po.Status = 7;

                    await db.SaveChangesAsync();

                    var storeLst = mas.Inv_PO.Inv_PODetail.GroupBy(x => x.SKUId).Select(x => new
                    {
                        SKUId = x.Key,
                        Qty = x.Sum(a => a.Qty),
                        TP = x.Sum(a => (a.TP + a.GST + a.WHT) - a.Discount)
                    }).ToList();

                    foreach (var item in storeLst)
                    {
                        var ls = await db.Inv_GRNDetail.Where(x => x.Inv_Store.SKUId == item.SKUId && x.Inv_GRN.POId == POId)
                            .Select(x => x.Inv_Store).ToListAsync();
                        ls.ForEach(x => x.PPrice = Math.Round(item.TP / item.Qty, 0));
                    }
                    await db.SaveChangesAsync();

                    scop.Complete();
                    scop.Dispose();
                    return mas.PInvId;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }

        public async Task<Poresturndata> GetPOReturnData(long ItemId)
        {
            return await db.Inv_Store.Where(x => x.ItemId == ItemId).Select(x =>
            new Poresturndata
            {
                PPrice = x.PPrice + Math.Round(x.MRP - (x.MRP * 100 / ((x.Tax ?? 0) + 100))),
                SuppId = x.SuppId,
                SkuId = x.SKUId,
                SerialNo = x.SerialNo,
                ItemId = x.ItemId

            }).FirstOrDefaultAsync();
        }
        public async Task<bool> IsSrNoExist(string SrNo, int SKUId)
        {
            try
            {
                return await db.Inv_Store.Where(x => x.SerialNo == SrNo && x.StatusID != 9).AnyAsync();
            }
            catch (Exception)
            {
                return true;
            }
        }
        public async Task<bool> SavePurchaseReturn(IEnumerable<POReturnDtlVM> mod, int LocId, int SuppId, string Remarks,string ReturnType,int ReasonId, List<long> files, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var lastPorNo = await db.Inv_POReturn.OrderByDescending(x => x.PORId).Select(x => x.PORNo).FirstOrDefaultAsync();
                    string PORNo = GetNewInvNo(lastPorNo, "PRI-");
                    Inv_POReturn mas = new Inv_POReturn()
                    {
                        LocId = LocId,
                        TransDate = DateTime.Now,
                        Remarks = Remarks,
                        SuppId = SuppId,
                        UserId = UserId,
                        PORDate = setupBL.GetWorkingDate(LocId),
                        Status = "U",
                        PORNo = PORNo,
                        ReturnType = ReturnType,
                        ReasonId = ReasonId
                    };
                    List<Inv_POReturnDtl> lst = new List<Inv_POReturnDtl>();
                    foreach (var v in mod)
                    {
                        if (v.ItemId > 0)
                        {
                            var item = await db.Inv_Store.FindAsync(v.ItemId);
                            if (item.LocId != LocId)
                            {
                                scop.Dispose();
                                return false;
                            }
                            decimal tax = Math.Round(item.MRP - (item.MRP * 100 / ((item.Tax ?? 0) + 100)));
                            v.TP = item.PPrice + tax;
                            decimal wht = await orderBL.GetWHT(SuppId, item.MRP, v.TP, tax, 0);

                            //decimal tax = Math.Round(item.MRP - (item.MRP * 100 / ((item.Tax ?? 0) + 100)));
                            //decimal wht = await orderBL.GetWHT(SuppId, item.MRP, v.TP, tax, 0);

                            var gd = await db.Inv_GRNDetail.Where(x => x.ItemId == v.ItemId).FirstOrDefaultAsync();
                            if (gd != null)
                            {
                                var pod = await db.Inv_PODetail.Where(x => x.PODtlId == gd.PODtlId).FirstOrDefaultAsync();
                                if (pod != null)
                                {
                                    tax = pod.GST;
                                    wht = pod.WHT;
                                    v.TP = pod.TP - pod.Discount;
                                }
                            }

                            if (item.StatusID == 9)
                            {
                                scop.Dispose();
                                return false;
                            }

                            item.StatusID = 9;

                            Inv_POReturnDtl det = new Inv_POReturnDtl
                            {
                                Qty = 1,
                                ItemId = v.ItemId,
                                Remarks = v.Remarks,
                                SKUId = v.SKUId,
                                TP = v.TP,
                                MRP = item.MRP,
                                STax = tax,
                                WHT = wht
                            };
                            lst.Add(det);

                        }
                    }
                    mas.Inv_POReturnDtl = lst;
                    db.Inv_POReturn.Add(mas);
                    await db.SaveChangesAsync();

                    foreach (var v in lst)
                    {
                        var item = await db.Inv_Store.FindAsync(v.ItemId);
                        Inv_StoreHistory tbl = new Inv_StoreHistory
                        {
                            DocDate = mas.PORDate,
                            ItemId = item.ItemId,
                            LocId = item.LocId,
                            MFact = -1,
                            MRP = item.MRP,
                            PPrice = item.PPrice,
                            Qty = 1,
                            SKUId = item.SKUId,
                            SMPrice = 0,
                            SPrice = 0,
                            TransDate = mas.TransDate,
                            Type = "Purchase Return",
                            UserId = UserId,

                        };
                        db.Inv_StoreHistory.Add(tbl);
                    }
                    await db.SaveChangesAsync();

                    if (files != null && files.Count > 0)
                    {
                        await new DocumentBL().UpdateDocRef(files, mas.PORId);
                    }

                    if (lst.Sum(x => x.TP) > 0)
                    {
                        var vrId = await PostPurchaseReturn(mas.PORId, UserId);
                        if (vrId > 0)
                        {
                            scop.Complete();
                        }
                    }
                    else
                    {
                        scop.Complete();
                    }
                    scop.Dispose();
                    return true;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }

        #region PurchaseApproval
        public async Task<List<OrderSearchVM>> GetPurchaseApproval(string Level, int UserId)
        {
            try
            {
                List<OrderSearchVM> lst = new List<OrderSearchVM>();
                var usr = await db.Users_Login.FindAsync(UserId);
                var approval = usr.Users_Group.Users_GroupAccess.Where(x => x.MenuId == 12100000).Any();
                var validate = usr.Users_Group.Users_GroupAccess.Where(x => x.MenuId == 12090000).Any();
                if (Level == "V" && validate)
                {
                    lst = await db.Inv_Purchase.Where(x => x.Status == "U" && x.CheckedBy == null).Select(x =>
                       new OrderSearchVM
                       {
                           PODate = x.InvDate,
                           POId = x.PInvId,
                           PONo = x.InvNo,
                           DeliveryDate = x.DueDate,
                           SuppName = x.Inv_Suppliers.SuppName,
                           Status = x.RevokedBy == null ? "" : "R",
                           FullName = x.Users_Login.FullName,
                           ValidateBy = ""
                       }).ToListAsync();

                }
                else if (Level == "A" && approval)
                {
                    lst = await (from x in db.Inv_Purchase
                                 join v in db.Users_Login on x.CheckedBy equals v.UserID
                                 where x.Status == "U" && x.CheckedBy != null && x.ApprovedBy == null
                                 select
                        new OrderSearchVM
                        {
                            PODate = x.InvDate,
                            POId = x.PInvId,
                            PONo = x.InvNo,
                            DeliveryDate = x.DueDate,
                            SuppName = x.Inv_Suppliers.SuppName,
                            Status = x.RevokedBy == null ? "" : "R",
                            FullName = x.Users_Login.FullName,
                            ValidateBy = v.FullName
                        }).ToListAsync();
                }
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<bool> PurchaseApproval(OrderSearchVM mod, string Level, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var vr = await db.Inv_Purchase.FindAsync(mod.POId);
                    if (Level == "V" && mod.Status == "V")
                    {
                        if (vr.CheckedBy == null)
                        {
                            vr.CheckedBy = UserId;
                            vr.CheckedDate = DateTime.Now;
                            await db.SaveChangesAsync();
                        }
                    }
                    else if (Level == "A" && mod.Status == "A")
                    {
                        if (vr.ApprovedBy == null && vr.CheckedBy != null && vr.Status != "P")
                        {
                            vr.ApprovedBy = UserId;
                            vr.ApprovedDate = DateTime.Now;
                            vr.Status = "P";
                            var det = await db.Inv_PurchaseDetail.Where(x => x.PInvId == vr.PInvId).ToListAsync();

                            if (det.Sum(x => x.Rate) != 0)
                            {
                                List<VoucherDetailVM> vLst = new List<VoucherDetailVM>();
                                var supp = await db.Inv_Suppliers.FindAsync(vr.SuppId);
                                var gstAcc = await accountBL.GetSuppAcc(108, supp.CategoryId);
                                var whtAcc = await accountBL.GetSuppAcc(107, supp.CategoryId);
                                var suppAcc = await accountBL.GetSuppAcc(101, supp.CategoryId);
                                var clrAcc = await accountBL.GetAcc(2, supp.CategoryId);
                                foreach (var v in det)
                                {
                                    vLst.Add(new VoucherDetailVM
                                    {
                                        AccId = clrAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = (v.Rate - v.Discount) * v.Qty,
                                        Particulars = v.Itm_Master.SKUCode + " (" + v.Qty.ToString() + "@" + (v.Rate - v.Discount).ToString() + ")",
                                        PCCode = 72,
                                        SubId = 0,
                                        RefId = v.PInvDtl
                                    });
                                    if (v.STax > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = gstAcc,
                                            CCCode = 72,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = v.STax * v.Qty, //- anuAmt - biaAmt - quaAmt - monAmt - whtAmt - gstAmt,
                                            Particulars = v.Itm_Master.SKUCode + " (" + v.Qty.ToString() + "@" + (v.STax).ToString() + ")",
                                            PCCode = 72,
                                            SubId = vr.SuppId,
                                            RefId = v.PInvDtl
                                        });
                                    if (v.WHT > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = whtAcc,
                                            CCCode = 72,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = v.WHT * v.Qty, //- anuAmt - biaAmt - quaAmt - monAmt - whtAmt - gstAmt,
                                            Particulars = v.Itm_Master.SKUCode + " (" + v.Qty.ToString() + "@" + (v.WHT).ToString() + ")",
                                            PCCode = 72,
                                            SubId = vr.SuppId,
                                            RefId = v.PInvDtl
                                        });
                                    vLst.Add(new VoucherDetailVM
                                    {
                                        AccId = suppAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = (v.Rate - v.Discount + v.STax + v.WHT) * v.Qty,
                                        Dr = 0,
                                        Particulars = v.Itm_Master.SKUCode + " (" + v.Qty.ToString() + "@" + (v.Rate - v.Discount + v.STax + v.WHT).ToString() + ")",
                                        PCCode = 72,
                                        SubId = vr.SuppId,
                                        RefId = v.PInvDtl
                                    });
                                }
                                var vrId = await accountBL.PostAutoVoucher(vLst, "PIV", vr.InvNo, vr.InvDate, vr.UserId);
                                if (vrId > 0)
                                    await db.SaveChangesAsync();
                                else
                                    scop.Dispose();
                            }
                            else
                            {
                                await db.SaveChangesAsync();
                            }
                        }
                    }
                    else if (mod.Status == "R")
                    {
                        if (vr.ApprovedBy == null)
                        {
                            vr.RevokedBy = UserId;
                            vr.RevokedDate = DateTime.Now;
                            vr.Status = "C";
                            await db.SaveChangesAsync();
                        }
                        //if (vr.CheckedBy != null)
                        //{
                        //    vr.CheckedBy = null;
                        //    vr.CheckedDate = null;
                        //}
                    }
                    scop.Complete();
                    scop.Dispose();
                    return true;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }
        #endregion
        public async Task<List<BikeLetterOldVM>> GetBikesOld()
        {
            try
            {
                var lst = await db.Inv_BikeLetter_Old.
                                   Select(x => new BikeLetterOldVM
                                   {
                                       //Amount = x.Amount,
                                       BillDate = x.BillDate,
                                       BillNo = x.BillNo,
                                       CompName = x.CompName,
                                       DoNo = x.DoNo,
                                       EnvelopNo = x.EnvelopNo,
                                       //ItemName = x.ItemName,
                                       LetterNumber = x.LetterNumber,
                                       LetterStatus = x.LetterStatus,
                                       //LocCode = x.LocCode,
                                       LocName = x.LocName,
                                       Model = x.Model,
                                       //PPrice = x.PPrice,
                                       RowId = x.RowId,
                                       //SPrice = x.SPrice,
                                       SrNo = x.SrNo,
                                       SuppName = x.SuppName

                                   }).ToListAsync();

                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> UpdateBikeLetterOld(BikeLetterOldVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Inv_BikeLetter_Old.Where(x => x.RowId == mod.RowId).FirstOrDefaultAsync();
                if (tbl != null)
                {
                    tbl.LetterStatus = mod.LetterStatus;
                    tbl.LetterNumber = mod.LetterNumber;
                    tbl.EnvelopNo = mod.EnvelopNo;
                    tbl.ModificationDate = DateTime.Now;
                    tbl.ModifiedBy = UserId;
                    await db.SaveChangesAsync();
                    return "Success";
                }
                else
                {
                    return "Not Found";
                }
            }
            catch (Exception)
            {
                return "Server Error";
            }
        }
        public async Task<List<BikeRegOldVM>> GetBikesRegOld()
        {
            try
            {
                var lst = await db.Inv_BikeReg_Old.
                                   Select(x => new BikeRegOldVM
                                   {
                                       BillDate = x.BillDate,
                                       BillNo = x.BillNo,
                                       CompName = x.CompName,
                                       ItemName = x.ItemName,
                                       LocCode = x.LocCode,
                                       LocName = x.LocName,
                                       Model = x.Model,
                                       RowId = x.RowId,
                                       SrNo = x.SrNo,
                                       AccNo = x.AccNo,
                                       NumberPlate = x.NumberPlate,
                                       NumberPlateStatus = x.NumberPlateStatus,
                                       RegDocsStatus = x.RegDocsStatus,
                                       RegFeesCharges = x.RegFeesCharges,
                                       RegFeesStatus = x.RegFeesStatus,
                                       Saletype = x.Saletype,
                                       Remarks = x.Remarks
                                   }).ToListAsync();

                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> UpdateBikeRegOld(BikeRegOldVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Inv_BikeReg_Old.Where(x => x.RowId == mod.RowId).FirstOrDefaultAsync();
                if (tbl != null)
                {
                    tbl.NumberPlate = mod.NumberPlate;
                    tbl.NumberPlateStatus = mod.NumberPlateStatus;
                    tbl.RegDocsStatus = mod.RegDocsStatus;
                    tbl.RegFeesCharges = mod.RegFeesCharges;
                    tbl.RegFeesStatus = mod.RegFeesStatus;
                    tbl.Remarks = mod.Remarks;
                    tbl.ModificationDate = DateTime.Now;
                    tbl.ModifiedBy = UserId;
                    await db.SaveChangesAsync();
                    return "Success";
                }
                else
                {
                    return "Not Found";
                }
            }
            catch (Exception)
            {
                return "Server Error";
            }
        }
        public async Task<List<BikeLetterVM>> GetBikes(DateTime FromDate, DateTime ToDate, int SuppId, int TypeId, string SerialNo, string DONo, int LetterStatus)
        {
            try
            {
                var lst = db.spGet_BikeLetter(FromDate, ToDate, SuppId, TypeId, SerialNo, DONo, LetterStatus).
                                   Select(x => new BikeLetterVM
                                   {
                                       LocCode = x.LocCode,
                                       SuppName = x.SuppName,
                                       GRNNo = x.GRNNo,
                                       DONo = x.DONo,
                                       DODate = x.DODate,
                                       GRNDate = x.GRNDate,
                                       TypeName = x.TypeName,
                                       SKUName = x.SKUName,
                                       SerialNo = x.SerialNo,
                                       LetterStatus = x.LetterStatus,
                                       GRNId = x.GRNId
                                   }).ToList();

                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<string> UpdateBikeLetter(string SrNo, string LetterNumber, int EnvelopNo, int UserId)
        {
            try
            {
                long TransId = 0;
                var itemId = await db.Inv_Store.Where(x => x.SerialNo == SrNo).Select(x => x.ItemId).FirstOrDefaultAsync();
                if (itemId == null)
                {
                    return "Serial No not found";
                }
                var IsExist = await db.Inv_BikeLetter.Where(x => x.ItemId == itemId).AnyAsync();
                if (!IsExist)
                {
                    var tbl = new Inv_BikeLetter
                    {
                        ItemId = itemId,
                        LetterNumber = LetterNumber,
                        EnvelopNo = EnvelopNo,
                        LetterStatus = true,
                        TransDate = DateTime.Now,
                        UserId = UserId
                    };
                    db.Inv_BikeLetter.Add(tbl);
                    await db.SaveChangesAsync();
                    TransId = tbl.TransId;

                    return "Success";
                }
                else
                {
                    return "Already Exist";
                }
            }
            catch (Exception)
            {
                return "Server Error";
            }
        }


        public async Task<List<BikeLetterSaleVM>> GetBikesSale(DateTime FromDate, DateTime ToDate, int CityId, int LocId, int TypeId, int SKUId, int SuppId, string SaleType, string SerialNo, string billNo)
        {
            try
            {
                var lst = db.spGet_BikeLetterSale(FromDate, ToDate, CityId, LocId, TypeId, SKUId, SuppId, SaleType, SerialNo, billNo).
                                   Select(x => new BikeLetterSaleVM
                                   {

                                       City = x.City,
                                       LocCode = x.LocCode,
                                       SuppName = x.SuppName,
                                       BillNo = x.BillNo ?? 0,
                                       BillDate = x.BillDate,
                                       SaleType = x.SaleType,
                                       ComName = x.ComName,
                                       TypeName = x.TypeName,
                                       SKUName = x.SKUName,
                                       ItemId = x.ItemId,
                                       SerialNo = x.SerialNo,
                                       LetterStatus = x.LetterStatus,
                                       RefTransId = x.RefTransId,
                                       RegFeeStatus = x.RegFeeStatus ?? false,
                                       RegFeeCharges = x.RegFeeCharges ?? 0,
                                       NumberPlate = x.NumberPlate,
                                       NumberPlateStatus = x.NumberPlateStatus ?? false,
                                       RegDocStatus = x.RegDocStatus ?? false,
                                       Remarks = x.Remarks,
                                       DODate = x.DODate,
                                       DONo = x.DONo,
                                       GRNDate = x.GRNDate,
                                       GRNNo = x.GRNNo,
                                       Status = x.Status
                                   }).ToList();

                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> UpdateBikesSale(BikeLetterSaleVM com, int UserId)
        {
            try
            {
                long RegId = 0;
                var itemId = await db.Inv_Store.Where(x => x.SerialNo == com.SerialNo).Select(x => x.ItemId).FirstOrDefaultAsync();
                if (itemId == null)
                {
                    return "Serial No not found";
                }
                var IsExist = await db.Inv_BikeReg.Where(x => x.ItemId == itemId).AnyAsync();
                if (!IsExist)
                {
                    var tbl = new Inv_BikeReg
                    {
                        ItemId = com.ItemId,
                        SaleType = com.SaleType,
                        RefTransId = com.BillNo,
                        RegFeeStatus = com.RegFeeStatus,
                        RegFeeCharges = com.RegFeeCharges,
                        NumberPlate = com.NumberPlate,
                        NumberPlateStatus = com.NumberPlateStatus,
                        RegDocStatus = com.RegDocStatus,
                        TransferStatus = false,
                        Remarks = com.Remarks,
                        TransDate = DateTime.Now,
                        UserId = UserId
                    };
                    db.Inv_BikeReg.Add(tbl);
                    await db.SaveChangesAsync();
                    RegId = tbl.RegId;

                    return " Save Success";
                }
                else
                {

                    var uib = await db.Inv_BikeReg.FirstOrDefaultAsync(x => x.ItemId == itemId);
                    if (uib != null)
                    {
                        uib.ItemId = com.ItemId;
                        uib.SaleType = com.SaleType;
                        uib.RefTransId = com.BillNo;
                        uib.RegFeeStatus = com.RegFeeStatus;
                        uib.RegFeeCharges = com.RegFeeCharges;
                        uib.NumberPlate = com.NumberPlate;
                        uib.NumberPlateStatus = com.NumberPlateStatus;
                        uib.RegDocStatus = com.RegDocStatus;
                        uib.TransferStatus = false;
                        uib.Remarks = com.Remarks;
                        uib.TransDate = DateTime.Now;
                        uib.UserId = UserId;
                        await db.SaveChangesAsync();
                    };


                    return " Update Success";

                }
            }
            catch (Exception)
            {
                return "Server Error";
            }
        }
    }
}