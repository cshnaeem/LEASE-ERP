using AGEERP.CrReports;
using NotifSystem.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace AGEERP.Models
{
    public class StockBL
    {
        AGEEntities db = new AGEEntities();
        AccountBL accountBL = new AccountBL();
        SetupBL setupBL = new SetupBL();
        NotificationBL notificationBL = new NotificationBL();
        NotificationHub objNotifHub = new NotificationHub();
        public async Task<bool> MobileStockOpeningCreate(IEnumerable<Inv_StoreVM> model, int UserId)
        {
            try
            {
                var OSMList = new List<Inv_OpeningStockMobile>();
                foreach (var item in model)
                {
                    var itemId = await db.Inv_Store.Where(x => x.SKUId == item.SKUId).MaxAsync(x => (long?)x.ItemId);
                    if (itemId > 0)
                    {
                        var itemDetail = await db.Inv_Store.FirstOrDefaultAsync(x => x.ItemId == itemId);
                        var obj = new Inv_OpeningStockMobile
                        {
                            SKUId = item.SKUId,
                            LocId = item.LocId,
                            SerialNo = item.SerialNo,
                            PPrice = itemDetail.PPrice,
                            StatusID = 1, //InStock
                            SuppId = itemDetail.SuppId,
                            MRP = itemDetail.MRP,
                            Remarks = item.Remarks,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            IsPosted = false,
                            Status = true,
                            Exempted = itemDetail.Exempted,
                            Tax = itemDetail.Tax
                        };
                        OSMList.Add(obj);
                    }
                    else
                    {
                        var obj = new Inv_OpeningStockMobile
                        {
                            SKUId = item.SKUId,
                            LocId = item.LocId,
                            SerialNo = item.SerialNo,
                            PPrice = 0,
                            StatusID = 1, //InStock
                            SuppId = 0,
                            MRP = 0,
                            Remarks = item.Remarks,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            IsPosted = false,
                            Status = true,
                            Exempted = false,
                            Tax = 0
                        };
                        OSMList.Add(obj);
                    }
                }
                db.Inv_OpeningStockMobile.AddRange(OSMList);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<BranchStockVM> GetStockAtBranch(int LocId, int ModelId, int SKUId)
        {
            try
            {
                return db.spGet_SKUWiseStockATBranch(LocId, ModelId, SKUId).Select(x => new BranchStockVM
                {
                    LocCode = x.LocCode,
                    LocName = x.LocName,
                    SKUCode = x.SKUCode,
                    Distance = x.Distance,
                    Qty = x.Qty ?? 0
                }).OrderBy(x => x.Distance).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateStockOpening(IEnumerable<Inv_OpeningStockVM> mod, int UserId)
        {
            try
            {
                foreach (var m in mod)
                {
                    var tbl = await db.Inv_OpeningStockMobile.SingleOrDefaultAsync(x => x.RowId == m.RowId);
                    if (tbl != null)
                    {
                        tbl.MRP = m.MRP;
                        tbl.PPrice = m.PPrice;
                        tbl.SKUId = m.SKUId;
                        tbl.SuppId = m.SuppId;
                        tbl.SerialNo = m.SerialNo;
                        tbl.Remarks = m.Remarks;
                        tbl.Tax = m.Tax;
                        tbl.Exempted = m.Exempted;

                        await db.SaveChangesAsync();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> PostOpeningStock(List<long> TransLst, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    List<Inv_Store> vLst = new List<Inv_Store>();
                    foreach (var v in TransLst)
                    {
                        var item = await db.Inv_OpeningStockMobile.Where(x => x.RowId == v).FirstOrDefaultAsync();
                        if (item.IsPosted)
                            continue;

                        var oldItem = await db.Inv_Store.FirstOrDefaultAsync(x => x.SerialNo == item.SerialNo);
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
                                return false;
                            }
                        }
                        var invstoremodel = new Inv_Store()
                        {
                            LocId = item.LocId,
                            CSerialNo = item.CSerialNo,
                            PPrice = item.PPrice,
                            Qty = 1,
                            Remarks = item.Remarks,
                            SerialNo = item.SerialNo,
                            SKUId = item.SKUId,
                            StatusID = 1,
                            SuppId = item.SuppId,
                            OldSerialNo = item.CSerialNo,
                            TrxDate = DateTime.Now,
                            MRP = item.MRP,
                            Tax = item.Tax,
                            Exempted = item.Exempted
                        };
                        db.Inv_Store.Add(invstoremodel);
                        await db.SaveChangesAsync();

                        var stockopeningmodel = new Inv_OpeningStock()
                        {
                            ItemId = invstoremodel.ItemId,
                            LocId = item.LocId,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            Type = "A",
                            PPrice = item.PPrice
                        };
                        db.Inv_OpeningStock.Add(stockopeningmodel);
                        item.IsPosted = true;
                        await db.SaveChangesAsync();

                        var stHist = await setupBL.CreateStoreHistory(stockopeningmodel.TransDate, invstoremodel.ItemId, invstoremodel.LocId, 1, invstoremodel.MRP, invstoremodel.PPrice, invstoremodel.SKUId, 0, 0, stockopeningmodel.TransDate, "Stock Adjustment", (int)stockopeningmodel.UserId, stockopeningmodel.RowId);
                        if (!stHist)
                        {
                            scop.Dispose();
                        }

                        vLst.Add(invstoremodel);
                    }
                    var res = await accountBL.StockOpeningVoucher(vLst, UserId);
                    if (res)
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
        public async Task<bool> IsSrNoExist(string SrNo)
        {
            try
            {
                bool IsExist = await db.Inv_Store.Where(x => x.SerialNo == SrNo && x.StatusID != 9).AnyAsync();
                if (!IsExist)
                {
                    IsExist = await db.Inv_OpeningStockMobile.Where(x => x.SerialNo == SrNo).AnyAsync();
                }
                return IsExist;
            }
            catch (Exception)
            {
                return true;
            }
        }
        public async Task<List<Inv_OpeningStockVM>> GetStockOpening(int LocId)
        {
            try
            {
                return await (from os in db.Inv_OpeningStockMobile
                              join it in db.Itm_Master on os.SKUId equals it.SKUId
                              where os.LocId == LocId && !os.IsPosted
                              select new Inv_OpeningStockVM
                              {
                                  RowId = os.RowId,
                                  SKUId = os.SKUId,
                                  SerialNo = os.SerialNo,
                                  CSerialNo = os.CSerialNo,
                                  PPrice = os.PPrice,
                                  MRP = os.MRP,
                                  Remarks = os.Remarks,
                                  SKU = it.SKUName,
                                  SuppId = os.SuppId ?? 0,
                                  Exempted = os.Exempted ?? false,
                                  Tax = os.Tax ?? 0
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> SaveStockVerification(List<StockVerificationAVM> mod)
        {
            try
            {
                var lst = new List<Inv_StockVerificationDtl>();
                foreach (var item in mod)
                {
                    if (item.Status == "V" && item.StatusId > 0)
                    {
                        var itm = await db.Inv_Store.Where(x => x.ItemId == item.ItemId).FirstOrDefaultAsync();
                        if (itm != null)
                        {
                            if (itm.StatusID != item.StatusId && (itm.StatusID == 1 || itm.StatusID == 7) && (item.StatusId == 1 || item.StatusId == 7))
                            {
                                Inv_StoreLog obj = new Inv_StoreLog
                                {
                                    LocId = mod[0].LocId,
                                    ItemId = item.ItemId,
                                    FromStatus = itm.StatusID,
                                    ToStatus = item.StatusId,
                                    UserId = mod[0].UserId,
                                    TransDate = DateTime.Now,
                                    Reason = item.Remarks,
                                    IsSerialChange = false
                                };
                                db.Inv_StoreLog.Add(obj);
                                //await db.SaveChangesAsync();
                                itm.StatusID = item.StatusId;
                            }
                        }
                    }
                    lst.Add(new Inv_StockVerificationDtl
                    {
                        ItemId = item.ItemId,
                        SerialNo = item.SerialNo,
                        SKUId = item.SKUId,
                        Status = item.Status,
                        TransDate = item.TransDate,
                        InputType = item.InputType,
                        Remarks = item.Remarks
                    });
                }
                var tbl = new Inv_StockVerification()
                {
                    DocDate = setupBL.GetWorkingDate(mod[0].LocId),
                    LocId = mod[0].LocId,
                    UserId = mod[0].UserId,
                    Inv_StockVerificationDtl = lst
                };
                db.Inv_StockVerification.Add(tbl);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateStockStatus(Inv_StoreVM mod,int GroupId, int UserId)
        {
            try
            {
                var tbl = await db.Inv_Store.SingleOrDefaultAsync(x => x.ItemId == mod.ItemId);
                if (tbl != null)
                {
                    if (tbl.Inv_Status.MFact == 1)
                    {
                        if (GroupId == 2)
                        {
                            if (!((tbl.StatusID == 1 || tbl.StatusID == 7) && (mod.StatusID == 1 || mod.StatusID == 7)))
                            {
                                return false;
                            }
                        }

                        int Status = tbl.StatusID;
                        tbl.StatusID = mod.StatusID;
                        await db.SaveChangesAsync();
                        Inv_StoreLog obj = new Inv_StoreLog
                        {
                            LocId = tbl.LocId,
                            ItemId = mod.ItemId,
                            FromStatus = Status,
                            ToStatus = mod.StatusID,
                            UserId = UserId,
                            TransDate = DateTime.Now,
                            Reason = mod.Remarks,
                            IsSerialChange = false
                        };
                        db.Inv_StoreLog.Add(obj);
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<Inv_StoreVM>> GetItemStock(int CompanyId, int ProductId, int ModelId, int StatusId, int LocationId, string Serial)
        {
            try
            {
                return await db.Inv_Store.Where(x => (x.LocId == LocationId || LocationId == 0)
                && (x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComId == CompanyId || CompanyId == 0)
                && (x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductId == ProductId || ProductId == 0)
                && (x.Itm_Master.Itm_Model.ModelId == ModelId || ModelId == 0)
                && (x.StatusID == StatusId || StatusId == 0)
                && x.Inv_Status.MFact == 1
                                ).Select(x =>
                               new Inv_StoreVM
                               {
                                   ItemId = x.ItemId,
                                   SKU = x.Itm_Master.SKUCode,
                                   Model = x.Itm_Master.Itm_Model.Model,
                                   Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                                   Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                                   SerialNo = x.SerialNo,
                                   StatusID = x.StatusID
                               }).ToListAsync();
               
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Inv_StoreVM>> GetItemStockRow(int LocationId, string serial)
        {
            try
            {
                return await db.Inv_Store.Where(x => x.SerialNo == serial && x.LocId == LocationId && x.Inv_Status.MFact == 1).Select(x =>
                                     new Inv_StoreVM
                                     {
                                         ItemId = x.ItemId,
                                         SKU = x.Itm_Master.SKUCode,
                                         Model = x.Itm_Master.Itm_Model.Model,
                                         Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                                         Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                                         SerialNo = x.SerialNo,
                                         StatusID = x.StatusID
                                     }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<InvStatusVM>> StatusList()
        {
            try
            {
                return await db.Inv_Status.Where(x => x.MFact == 1).Select(x =>
                  new InvStatusVM
                  {
                      StatusID = x.StatusID,
                      StatusTitle = x.StatusTitle
                  }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<InvStatusVM>> DamageStatusList()
        {
            try
            {
                return await db.Inv_Status.Where(x => x.StatusID == 5 || x.StatusID == 6 || x.StatusID == 11).Select(x =>
                  new InvStatusVM
                  {
                      StatusID = x.StatusID,
                      StatusTitle = x.StatusTitle
                  }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Inv_Store> ItemDetails(string SrNo)
        {
            try
            {
                return await db.Inv_Store.Where(x => x.SerialNo == SrNo).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<ItemHistoryVM> GetItemHistory(int ItemId)
        {
            try
            {
                return db.spGet_ItemHistory(ItemId).Select(x =>
                    new ItemHistoryVM
                    {
                        LocName = x.LocName,
                        Date = x.GRNDate,
                        Action = x.Action,
                        Username = x.Username
                    }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Inv_AdjType>> GetAdjType()
        {
            try
            {
                return await db.Inv_AdjType.ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<long> SaveStockAdjusment(IEnumerable<IssueDetailVM> mod, int FLocId, int AdjTypeId, string Remarks, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    Inv_AdjOut mas = new Inv_AdjOut()
                    {
                        WorkingDate = setupBL.GetWorkingDate(FLocId),
                        LocId = FLocId,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        Remarks = Remarks
                    };
                    db.Inv_AdjOut.Add(mas);
                    await db.SaveChangesAsync();
                    //List<long> lst = new List<Inv_AdjOutDetail>();
                    foreach (var v in mod)
                    {
                        if (v.ItemId > 0)
                        {
                            var item = await db.Inv_Store.FindAsync(v.ItemId);
                            if (item.StatusID != 3 && item.StatusID != 12 &&  item.Inv_Status.MFact == 1)
                            {
                                Inv_AdjOutDetail det = new Inv_AdjOutDetail
                                {
                                    TransId = mas.TransId,
                                    ItemId = v.ItemId,
                                    FromStatusId = item.StatusID,
                                    ToStatusId = 12,
                                    PPrice = item.PPrice,
                                    AdjTypeId = AdjTypeId,

                                };
                                item.StatusID = 12;
                                db.Inv_AdjOutDetail.Add(det);
                                await db.SaveChangesAsync();
                                var stHist = await setupBL.CreateStoreHistory(mas.WorkingDate, v.ItemId, mas.LocId, -1, item.MRP, item.PPrice, item.SKUId, 0, 0, mas.TransDate, "Stock Adjustment Out", mas.UserId, det.TransId);
                                if (!stHist)
                                {
                                    scop.Dispose();
                                }
                            }
                        }
                    }

                    var res = await accountBL.StockAdjustmentOutVoucher(mas.TransId, UserId);
                    if (res)
                        scop.Complete();

                    scop.Dispose();
                    return mas.TransId;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<List<IssueRVM>> GetIssueDetail(long transid)
        {
            try
            {
                var iss = await (from item in db.Inv_Issue
                                 join itemdet in db.Inv_IssueDetail on item.TransId equals itemdet.TransId
                                 join loc in db.Comp_Locations on item.ToLocId equals loc.LocId
                                 where item.TransId == transid
                                 select new IssueRVM()
                                 {
                                     LocCode = loc.LocCode
                                 }).ToListAsync();
                return iss;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Inv_StoreVM>> SerialNumbList()
        {
            try
            {
                return await db.Inv_Store.Select(x =>
                 new Inv_StoreVM
                 {
                     ItemId = x.ItemId,
                     SerialNo = x.SerialNo
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<long> SaveStockIssue(IEnumerable<IssueDetailVM> mod, int FLocId, int TLocId, DateTime IssueDate, string Remarks, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    Inv_Issue mas = new Inv_Issue()
                    {
                        FromLocId = FLocId,
                        IssueDate = setupBL.GetWorkingDate(FLocId),
                        ToLocId = TLocId,
                        Status = "I",
                        TransDate = DateTime.Now,
                        Remarks = Remarks,
                        UserId = UserId,
                    };
                    db.Inv_Issue.Add(mas);
                    await db.SaveChangesAsync();
                    int counter = 0;
                    foreach (var v in mod)
                    {
                        if (v.ItemId > 0)
                        {
                            var item = await db.Inv_Store.FindAsync(v.ItemId);
                            if (item.StatusID != 3 && item.Inv_Status.MFact == 1)
                            {
                                item.StatusID = 3;

                                Inv_IssueDetail det = new Inv_IssueDetail
                                {
                                    ItemId = v.ItemId,
                                    Status = "I",
                                    Qty = 1,
                                    TransId = mas.TransId,
                                    PPrice = item.PPrice
                                };
                                db.Inv_IssueDetail.Add(det);
                                await db.SaveChangesAsync();
                                counter++;
                            }
                        }
                    }
                    if (counter > 0)
                    {
                        scop.Complete();
                    }
                    scop.Dispose();
                    return mas.TransId;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<object> GetIssueNoForReceive(int LocId)
        {
            try
            {
                return await (from x in db.Inv_Issue
                              join L in db.Comp_Locations on x.FromLocId equals L.LocId
                              where x.ToLocId == LocId && x.Status == "I"
                              select new { x.TransId, L.LocCode }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<IssueDetailVM>> GetStockIssue(int IssueNo, int LocId)
        {
            try
            {
                return await db.Inv_IssueDetail.Where(x => x.TransId == IssueNo
                && x.Inv_Issue.Status == "I" && x.Inv_Issue.ToLocId == LocId && x.Status == "I").Select(x => new IssueDetailVM
                {
                    Company = x.Inv_Store.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                    Status = x.Status,
                    ItemId = x.ItemId,
                    SKU = x.Inv_Store.Itm_Master.SKUName,
                    Product = x.Inv_Store.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                    Qty = x.Qty,
                    TransDtlId = x.TransDtlId,
                    SrNo = x.Inv_Store.SerialNo,
                    TransId = x.TransId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<long> SaveStockReceive(List<IssueDetailVM> mod, UsersInfoVM Usr)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    List<Inv_IssueDetail> vLst = new List<Inv_IssueDetail>();
                    var mas = await db.Inv_Issue.FindAsync(mod[0].TransId);
                    foreach (var v in mod)
                    {
                        if (v.Status == "R")
                        {
                            var tbl = await db.Inv_IssueDetail.FindAsync(v.TransDtlId);
                            var item = await db.Inv_Store.FindAsync(v.ItemId);
                            if (tbl.Status != "I" || item.StatusID != 3)
                            {
                                scop.Dispose();
                                return 0;
                            }
                            tbl.Status = v.Status;
                            tbl.ReceivedBy = Usr.UserId;
                            tbl.ReceivedDate = DateTime.Now;

                            item.FromLocId = mas.FromLocId;
                            item.LocId = mas.ToLocId;
                            if(mas.ToLocId == 213)
                            {
                                item.StatusID = 11;
                            }
                            else
                            {
                                item.StatusID = 1;
                            }
                            
                            vLst.Add(tbl);

                            var plan = await db.Itm_SKUPlan.Where(x => x.ItemId == v.ItemId && x.Status).FirstOrDefaultAsync();
                            if (plan != null)
                            {
                                plan.LocId = mas.ToLocId;
                            }

                            var stHist = await setupBL.CreateStoreHistory(tbl.ReceivedDate ?? DateTime.Now, item.ItemId, mas.FromLocId, -1, item.MRP, item.PPrice, item.SKUId, 0, 0, tbl.ReceivedDate ?? DateTime.Now, "Stock Out", mas.UserId, mas.TransId);
                            if (!stHist)
                            {
                                scop.Dispose();
                            }
                            stHist = await setupBL.CreateStoreHistory(tbl.ReceivedDate ?? DateTime.Now, item.ItemId, mas.ToLocId, 1, item.MRP, item.PPrice, item.SKUId, 0, 0, tbl.ReceivedDate ?? DateTime.Now, "Stock In", tbl.ReceivedBy ?? 0, mas.TransId);
                            if (!stHist)
                            {
                                scop.Dispose();
                            }
                            await db.SaveChangesAsync();
                        }
                    }
                    var lst = await db.Inv_IssueDetail.Where(x => x.TransId == mas.TransId).ToListAsync();
                    if (!lst.Where(x => x.Status != "R").Any())
                    {
                        mas.Status = "R";
                    }

                    await db.SaveChangesAsync();
                    if (vLst.Sum(x => x.Inv_Store.PPrice) > 0)
                    {
                        var IsPosted = await accountBL.StockReceiveVoucher(vLst, Usr.UserId);
                        if (IsPosted)
                        {
                            scop.Complete();
                        }
                    }
                    else
                    {

                        scop.Complete();
                    }

                    scop.Dispose();
                    return mas.TransId;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<List<StockTransferDashboardVM>> DashboardIBStockTransfer(DateTime FromDate, DateTime ToDate, int LocId, string Sta)
        {
            try
            {

                return await (from item in db.Inv_Issue
                              join frombranch in db.Comp_Locations on item.FromLocId equals frombranch.LocId
                              join tobranch in db.Comp_Locations on item.ToLocId equals tobranch.LocId
                              where (frombranch.LocId == LocId || tobranch.LocId == LocId || LocId == 0) &&
                              (Sta == "All" || item.Status == Sta) && item.TransDate >= FromDate && item.TransDate < ToDate
                              select new StockTransferDashboardVM()
                              {
                                  DocNo = item.TransId,
                                  DocDate = item.TransDate,
                                  FromBranch = frombranch.LocName,
                                  ToBranch = tobranch.LocName,
                                  Remarks = item.Remarks,
                                  Status = item.Status
                              }).ToListAsync();

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> MobileStockOpeningCreate(IEnumerable<InvOpeningStockMobileVM> model)
        {
            try
            {
                foreach (var item in model)
                {
                    var invstoremodel = new Inv_OpeningStockMobile()
                    {
                        LocId = item.LocId,
                        CSerialNo = item.CSerialNo,
                        PPrice = item.PPrice,
                        Remarks = item.Remarks,
                        SerialNo = item.SerialNo,
                        SKUId = item.SKUId,
                        StatusID = item.StatusID,
                        SuppId = item.SuppId,
                        TransDate = DateTime.Now,
                        MRP = item.MRP,
                        UserId = item.UserId,
                        Exempted = false,
                        Tax = 0
                    };
                    db.Inv_OpeningStockMobile.Add(invstoremodel);
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<List<Inv_StoreVM>> StockInTransit(int LocId, string Type)
        {
            try
            {
                if (Type == "F")
                {
                    return await (from item in db.Inv_Issue
                                     join dtl in db.Inv_IssueDetail on item.TransId equals dtl.TransId
                                     join x in db.Inv_Store on dtl.ItemId equals x.ItemId
                                     join frombranch in db.Comp_Locations on item.FromLocId equals frombranch.LocId
                                     join tobranch in db.Comp_Locations on item.ToLocId equals tobranch.LocId
                                     where (frombranch.LocId == LocId) && dtl.Status == "I" && x.StatusID == 3
                                     select new Inv_StoreVM
                                     {
                                         SKUId = x.SKUId,
                                         Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                                         Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                                         Model = x.Itm_Master.Itm_Model.Model,
                                         CSerialNo = x.CSerialNo,
                                         MRP = x.MRP,
                                         ItemId = x.ItemId,
                                         SerialNo = x.SerialNo,
                                         SKU = x.Itm_Master.SKUCode,
                                         Status = x.Inv_Status.StatusTitle,
                                         LocId = x.LocId,
                                         Remarks = frombranch.LocCode + " - " + tobranch.LocCode,
                                         TrxDate = item.TransDate
                                     }).ToListAsync();
                }
                else
                {
                    return await (from item in db.Inv_Issue
                                     join dtl in db.Inv_IssueDetail on item.TransId equals dtl.TransId
                                     join x in db.Inv_Store on dtl.ItemId equals x.ItemId
                                     join frombranch in db.Comp_Locations on item.FromLocId equals frombranch.LocId
                                     join tobranch in db.Comp_Locations on item.ToLocId equals tobranch.LocId
                                     where (tobranch.LocId == LocId) && dtl.Status == "I" && x.StatusID == 3
                                  select new Inv_StoreVM
                                     {
                                         SKUId = x.SKUId,
                                         Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                                         Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                                         Model = x.Itm_Master.Itm_Model.Model,
                                         CSerialNo = x.CSerialNo,
                                         MRP = x.MRP,
                                         ItemId = x.ItemId,
                                         SerialNo = x.SerialNo,
                                         SKU = x.Itm_Master.SKUCode,
                                         Status = x.Inv_Status.StatusTitle,
                                         LocId = x.LocId,
                                         Remarks = frombranch.LocCode + " - " + tobranch.LocCode,
                                         TrxDate = item.TransDate
                                     }).ToListAsync();
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
        public async Task<bool> StockOpeningCreate(IEnumerable<Inv_StoreVM> model, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    List<Inv_Store> vLst = new List<Inv_Store>();
                    foreach (var item in model)
                    {
                        var oldItem = await db.Inv_Store.FirstOrDefaultAsync(x => x.SerialNo == item.SerialNo);
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
                                return false;
                            }
                        }
                        var invstoremodel = new Inv_Store()
                        {
                            ItemId = item.ItemId,
                            LocId = item.LocId,
                            CSerialNo = item.CSerialNo,
                            PPrice = item.PPrice,
                            Qty = 1,
                            Remarks = item.Remarks,
                            SerialNo = item.SerialNo,
                            SKUId = item.SKUId,
                            StatusID = 1,
                            SuppId = item.SuppId,
                            OldSerialNo = item.CSerialNo,
                            TrxDate = DateTime.Now,
                            MRP = item.MRP,
                            Tax = item.Tax,
                            Exempted = item.Exempted
                        };
                        db.Inv_Store.Add(invstoremodel);
                        await db.SaveChangesAsync();

                        var stockopeningmodel = new Inv_OpeningStock()
                        {
                            ItemId = invstoremodel.ItemId,
                            LocId = item.LocId,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            Type = "A",
                            PPrice = item.PPrice
                        };
                        db.Inv_OpeningStock.Add(stockopeningmodel);
                        await db.SaveChangesAsync();

                        var stHist = await setupBL.CreateStoreHistory(stockopeningmodel.TransDate, invstoremodel.ItemId, invstoremodel.LocId, 1, invstoremodel.MRP, invstoremodel.PPrice, invstoremodel.SKUId, 0, 0, stockopeningmodel.TransDate, "Stock Adjustment", (int)stockopeningmodel.UserId, stockopeningmodel.RowId);
                        if (!stHist)
                        {
                            scop.Dispose();
                        }

                        vLst.Add(invstoremodel);

                    }

                    var res = await accountBL.StockOpeningVoucher(vLst, UserId);
                    if (res)
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
        public List<spRep_RecoverOfficerPerSummary_Result> GetRecoverPerformance(DateTime date, int Recid)
        {
            return db.spRep_RecoverOfficerPerSummary(date, Recid).ToList();

        }
        public async Task<List<StockAllVM>> GetStockAll()
        {
            try
            {
                return await (from x in db.Inv_Store
                              join l in db.Comp_Locations on x.LocId equals l.LocId
                              where x.Inv_Status.MFact == 1 && x.Itm_Master.IsPair == false && x.LocId != 191
                              select new StockAllVM
                              {
                                  SKU = x.Itm_Master.SKUCode,
                                  Model = x.Itm_Master.Itm_Model.Model,
                                  Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                                  Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                                  SerialNo = x.SerialNo,
                                  Status = x.Inv_Status.StatusTitle,
                                  LocCode = l.LocCode,
                                  CityCode = l.Comp_City.CityCode,
                                  MRP = x.MRP,
                                  PPrice = x.PPrice,
                                  Supplier = x.Inv_Suppliers.SuppName,
                                  Type = x.Itm_Master.Itm_Model.Itm_Type.TypeName
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ItemDetailVM>> GetSKUUpdateList(int LocId)
        {
            try
            {
                var lst = await (from x in db.Inv_Store
                                join l in db.Comp_Locations on x.LocId equals l.LocId
                                join gd in db.Inv_GRNDetail on x.ItemId equals gd.ItemId
                                join g in db.Inv_GRN on gd.GRNId equals g.GRNId
                                where x.Inv_Status.MFact == 1 && g.Status == "G" && x.LocId == LocId
                                select new ItemDetailVM
                                {
                                    //SKUName = x.Itm_Master.SKUCode,
                                    //Model = x.Itm_Master.Itm_Model.Model,
                                    //Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                                    //Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                                    SerialNo = x.SerialNo,
                                    Status = x.Inv_Status.StatusTitle,
                                    ItemId = x.ItemId,
                                    Location = l.LocCode,
                                    Supplier = x.Inv_Suppliers.SuppName,
                                    //Type = x.Itm_Master.Itm_Model.Itm_Type.TypeName,
                                    SKUId = x.SKUId
                                }).ToListAsync();
                return lst;
                //return db.spGet_SKUWiseStockATBranch(LocId, ModelId, SKUId).Select(x => new BranchStockVM
                //{
                //    LocCode = x.LocCode,
                //    LocName = x.LocName,
                //    SKUCode = x.SKUCode,
                //    Distance = x.Distance,
                //    Qty = x.Qty ?? 0
                //}).OrderBy(x => x.Distance).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateSKU(ItemDetailVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Inv_Store.SingleOrDefaultAsync(x => x.ItemId.Equals(mod.ItemId));
                if (tbl != null)
                {
                    if (tbl.SKUId != mod.SKUId && tbl.Inv_Status.MFact == 1)
                    {
                        var IsExist = await db.Inv_StockAdj.Where(x => x.ItemId == mod.ItemId).AnyAsync();
                        if(IsExist)
                        {
                            return false;
                        }
                        Inv_StockAdj obj = new Inv_StockAdj
                        {
                            LocId = tbl.LocId,
                            ItemId = mod.ItemId,
                            FromSKUId = tbl.SKUId,
                            ToSKUId = mod.SKUId,
                            UserId = UserId,
                            TransDate = DateTime.Now,
                            MRPAdj = 0,
                            PPriceAdj = 0,
                            Status = true,
                            WorkingDate = DateTime.Now.Date,
                            Remarks = mod.Remarks
                        };
                        db.Inv_StockAdj.Add(obj);
                        tbl.SKUId = mod.SKUId;
                        await db.SaveChangesAsync();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}