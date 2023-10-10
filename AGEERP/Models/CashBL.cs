using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace AGEERP.Models
{
    public class CashBL
    {
        AGEEntities db = new AGEEntities();
        SetupBL setupBL = new SetupBL();

        #region CashReceive

        public async Task<long> SaveCashRecive(CashReceiveVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    Lse_CashReceive tbl = new Lse_CashReceive
                    {
                        Description = mod.Description,
                        IsPosted = false,
                        LocId = mod.LocId,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        CCCode = mod.LocId,
                        WorkingDate = setupBL.GetWorkingDate(mod.LocId),
                        Amount = mod.Amount,
                        Status = "U"
                    };
                    db.Lse_CashReceive.Add(tbl);
                    await db.SaveChangesAsync();
                    if (!String.IsNullOrWhiteSpace(mod.UploadedFiles))
                    {
                        List<long> files = mod.UploadedFiles.Split(',').Select(long.Parse).ToList();
                        var IsSave = await new DocumentBL().UpdateDocRef(files, tbl.TransId);
                        if (!IsSave)
                            scop.Dispose();
                    }

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

        #endregion

        #region CashPayment

        public async Task<long> SaveCashPayment(CashPaymentVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    Lse_CashPayment tbl = new Lse_CashPayment
                    {
                        Description = mod.Description,
                        IsPosted = false,
                        LocId = mod.LocId,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        CCCode = mod.CCCode,
                        WorkingDate = setupBL.GetWorkingDate(mod.LocId),
                        Amount = mod.Amount,
                        Status = "U",
                        AccId = mod.AccId,
                        SubId = mod.SubId
                    };
                    db.Lse_CashPayment.Add(tbl);
                    await db.SaveChangesAsync();
                    if (!String.IsNullOrWhiteSpace(mod.UploadedFiles))
                    {
                        List<long> files = mod.UploadedFiles.Split(',').Select(long.Parse).ToList();
                        var IsSave = await new DocumentBL().UpdateDocRef(files, tbl.TransId);
                        if (!IsSave)
                            scop.Dispose();
                    }

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

        #endregion

        #region CashCollection
        public List<CashCollectionVM> GetCashCollection(int LocId)
        {
            try
            {
                var dt = setupBL.GetWorkingDate(LocId);
                return db.spRep_CashClosing(LocId, dt).Select(x => new CashCollectionVM
                {
                    CashHeadId = x.CashHeadId,
                    CashAmount = (x.Amount ?? 0) * (x.Type == "Cash Payments" ? -1 : 1),
                    CashHead = x.CashHead,
                    CashierDate = dt
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<long> CashCollectionSave(IEnumerable<CashCollectionVM> mod, int LocId, DateTime WorkingDate, string DepositTo, decimal CashDeposit, string CashierRemarks, long CashierId, bool IsVault, string UploadedFiles, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    WorkingDate = setupBL.GetWorkingDate(LocId);
                    Lse_CashCollection mas = new Lse_CashCollection
                    {
                        CashDeposit = CashDeposit,
                        CashierDate = DateTime.Now,
                        CashierId = CashierId,
                        CashierRemarks = CashierRemarks,
                        CashPayable = mod.Sum(x => x.CashAmount),
                        DocDate = WorkingDate,
                        DocType = DepositTo,
                        LocId = LocId,
                        //AccId = mod.FirstOrDefault().AccId,
                        UserId = UserId,
                        Status = true,
                        IsVault = IsVault
                    };
                    mas.Lse_CashCollectionDetail = mod.Select(x => new Lse_CashCollectionDetail
                    {
                        CashAmount = x.CashAmount,
                        CashHeadId = x.CashHeadId,
                        CashHead = x.CashHead
                    }).ToList();
                    db.Lse_CashCollection.Add(mas);
                    await db.SaveChangesAsync();

                    if (!String.IsNullOrWhiteSpace(UploadedFiles))
                    {
                        List<long> files = UploadedFiles.Split(',').Select(long.Parse).ToList();
                        var IsSave = await new DocumentBL().UpdateDocRef(files, mas.DocId);
                        if (!IsSave)
                            scop.Dispose();
                    }
                    scop.Complete();
                    scop.Dispose();
                    return mas.DocId;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<List<CashCollectionListVM>> CashCollectionList(string DepositTo, int LocId, long EmpId, DateTime WorkingDate)
        {
            try
            {
                var dt = DateTime.Now;
                //WorkingDate = setupBL.GetWorkingDate(LocId);
                var lst = new List<CashCollectionListVM>();
                if (DepositTo == "Cashier")
                {
                    lst.AddRange(await (from C in db.Lse_CashCollection
                                        join U in db.Pay_EmpMaster on C.CashierId equals U.EmpId
                                        where C.DocDate == WorkingDate &&
                                               C.Comp_Locations.CashCenter == LocId &&
                                               C.DocType == "Cashier"
                                        && (C.CashierId == EmpId || EmpId == 0)
                                        && !C.IsPosted
                                        && C.Status
                                        //&& C.DocType == "Cashier"
                                        select new CashCollectionListVM
                                        {
                                            CashAmount = C.CashDeposit,
                                            CashPayable = C.CashPayable,
                                            DocType = C.DocType,
                                            CashierId = C.CashierId,
                                            CashierDate = C.CashierDate ?? dt,
                                            DocId = C.DocId,
                                            LocName = C.Comp_Locations.LocCode,
                                            Remarks = C.CashierRemarks,
                                            WorkingDate = C.DocDate,
                                            RecvBy = C.IsVault == true ? "Vault" : "Cashier"
                                        }).ToListAsync());
                }
                if (DepositTo == "Bank")
                {
                    lst.AddRange(await (from C in db.Lse_CashCollection
                                            //join U in db.Fin_Accounts on C.CashierId equals U.AccId
                                        where C.DocDate == WorkingDate &&
                                               C.Comp_Locations.CashCenter == LocId &&
                                                         C.DocType == "Bank"
                                        && (C.CashierId == EmpId || EmpId == 0)
                                        && !C.IsPosted
                                        && C.Status
                                        //&& C.DocType == "Cashier"
                                        select new CashCollectionListVM
                                        {
                                            CashAmount = C.CashDeposit,
                                            CashPayable = C.CashPayable,
                                            DocType = C.DocType,
                                            CashierId = C.CashierId,
                                            CashierDate = C.CashierDate ?? dt,
                                            DocId = C.DocId,
                                            LocName = C.Comp_Locations.LocCode,
                                            //ReceivedAmount = C.CashRcvdByCenter,
                                            //IsReceived = false,
                                            Remarks = C.CashierRemarks,
                                            WorkingDate = C.DocDate,
                                            RecvBy = ""
                                        }).ToListAsync());
                }
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<CashCollectionListVM> CashCollectionListUpdate(CashCollectionListVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_CashCollection.Where(x => x.DocId == mod.DocId).FirstOrDefaultAsync();
                tbl.CashDeposit = mod.CashAmount;
                tbl.CashierId = mod.CashierId;
                tbl.CashCenterUserId = UserId;
                tbl.CashCenterRcvdDate = DateTime.Now;
                tbl.CashierRemarks = mod.Remarks;
                await db.SaveChangesAsync();
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<CashCollectionListVM> CashCollectionListDestroy(CashCollectionListVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_CashCollection.Where(x => x.DocId == mod.DocId).FirstOrDefaultAsync();
                tbl.Status = false;
                tbl.CashCenterUserId = UserId;
                tbl.CashCenterRcvdDate = DateTime.Now;
                await db.SaveChangesAsync();
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Pay_EmpMaster>> GetCashierList(int LocId)
        {
            try
            {
                if (LocId == 72)
                {
                    return await (from E in db.Pay_EmpMaster
                                      //join M in db.Pay_EmpLocationMapping on E.EmpId equals M.EmpId
                                  where E.DesgId == 257 && E.StatusId == "A"
                                  select E).ToListAsync();
                }
                else
                {
                    return await (from E in db.Pay_EmpMaster
                                  join M in db.Pay_EmpLocationMapping on E.EmpId equals M.EmpId
                                  where E.DesgId == 257 && E.StatusId == "A" && M.LocId == LocId && M.Status
                                  select E).ToListAsync();
                }

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<LocationVM>> GetCashCollectionCenterList(int LocId)
        {
            try
            {
                if (LocId == 72)
                {
                    return await db.Comp_Locations.Where(x => (x.LocTypeId == 4 || x.LocTypeId == 3) && x.Status).Select(x => new LocationVM
                    {
                        LocId = x.LocId,
                        LocName = x.LocName,
                        LocCode = x.LocCode
                    }).ToListAsync();
                }
                return await db.Comp_Locations.Where(x => x.LocTypeId == 4 && x.Status && x.LocId == LocId).Select(x => new LocationVM
                {
                    LocId = x.LocId,
                    LocName = x.LocName,
                    LocCode = x.LocCode
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<COAVM>> PaymentHeadList()
        {
            try
            {
                return await (from P in db.Lse_CashPaymentHeads
                              join A in db.Fin_Accounts on P.AccId equals A.AccId
                              join C in db.Fin_AcControls on A.CnId equals C.CnId
                              join G in db.Fin_AcGroups on C.GrId equals G.GrId
                              where P.Status
                              select
                             new COAVM
                             {
                                 Code = G.GrCode,
                                 Id = G.GrId,
                                 Name = G.GrDesc
                             }).Distinct().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<COAVM>> PaymentSubCodeList(string head)
        {
            try
            {
                return await (from P in db.Lse_CashPaymentHeads
                              join A in db.Fin_Accounts on P.AccId equals A.AccId
                              join C in db.Fin_AcControls on A.CnId equals C.CnId
                              join G in db.Fin_AcGroups on C.GrId equals G.GrId
                              where !A.IsLocked && P.Status && G.GrCode == head
                              select
                 new COAVM
                 {
                     Name = A.SubCodeDesc,
                     Code = A.SubCode,
                     Id = A.AccId,
                     IsLocked = P.HasSubsidary
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<CashCollectionListVM>> GetCashCollList(long EmpId)
        {
            try
            {
                var dt = DateTime.Now.Date;
                var lst = new List<CashCollectionListVM>();

                lst.AddRange(await (from C in db.Lse_CashCollection
                                    join U in db.Pay_EmpMaster on C.CashierId equals U.EmpId
                                    where C.DocDate == dt &&
                                           C.DocType == "Cashier"
                                    && (C.CashierId == EmpId)
                                    //&& !C.IsPosted
                                    && C.Status
                                    //&& C.DocType == "Cashier"
                                    select new CashCollectionListVM
                                    {
                                        CashAmount = C.CashDeposit,
                                        CashPayable = C.CashRcvdByCenter,
                                        DocType = C.DocType,
                                        CashierId = C.CashierId,
                                        CashierDate = C.CashierDate ?? dt,
                                        DocId = C.DocId,
                                        LocName = C.Comp_Locations.LocCode,
                                        Remarks = C.CashCenterRemarks,
                                        WorkingDate = C.DocDate,
                                    }).ToListAsync());
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<CashCollectionListVM> SaveCashColl(CashCollectionListVM mod)
        {
            try
            {
                var tbl = await db.Lse_CashCollection.Where(x => x.DocId == mod.DocId).FirstOrDefaultAsync();
                tbl.CashRcvdByCenter = mod.CashAmount;
                tbl.CashCenterUserId = Convert.ToInt32(mod.CashierId);
                tbl.CashCenterRcvdDate = DateTime.Now;
                tbl.CashCenterRemarks = mod.Remarks;
                await db.SaveChangesAsync();
                mod.CashPayable = mod.CashAmount;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<long> GetBankSlipCounter()
        {
            try
            {
                var counter = await db.Comp_Profile.Where(a => a.CompId == 1).FirstOrDefaultAsync();
                if (counter != null)
                {
                    counter.BankSlipCounter = counter.BankSlipCounter + 1;
                    await db.SaveChangesAsync();
                    return counter.BankSlipCounter ?? 0;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> CashRiskAlert()
        {
            try
            {
                var lst = db.spDash_CashInHand().ToList();
                if (lst.Count > 0)
                {
                    string sms = "Following branches exceeding cash limit:";

                    string messageBody = "Dear Concern,<br/>";
                    messageBody += "Following branches exceeding cash limit:<br/><br/>";
                    string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                    string htmlTableEnd = "</table>";
                    string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
                    string htmlHeaderRowEnd = "</tr>";
                    string htmlTrStart = "<tr style=\"color:#555555;\">";
                    string htmlTrEnd = "</tr>";
                    string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                    string htmlTdEnd = "</td>";
                    messageBody += htmlTableStart;
                    messageBody += htmlHeaderRowStart;
                    messageBody += htmlTdStart + "Branch" + htmlTdEnd;
                    messageBody += htmlTdStart + "Exceeding Amount" + htmlTdEnd;
                    messageBody += htmlHeaderRowEnd;
                    foreach (var item in lst)
                    {
                        messageBody = messageBody + htmlTrStart;
                        messageBody = messageBody + htmlTdStart + item.LocCode + htmlTdEnd; //adding student name  
                        messageBody = messageBody + htmlTdStart + item.Diff.ToString("##,###") + htmlTdEnd; //adding DOB  
                        messageBody = messageBody + htmlTrEnd;
                        sms = sms + Environment.NewLine + item.LocCode + "    " + item.Diff.ToString("##,###");
                    }
                    messageBody = messageBody + htmlTableEnd;
                    messageBody += "<br/><br/>Regards,<br/>";
                    messageBody += "Afzal Group of Electronics<br/><br/>";
                    messageBody += "This email is confidential. If you are not the addressee you may not copy, forward, disclose or use any part of it. If you have received this email in error, please delete all copies from your system.<br/>";
                    messageBody += "Internet communications cannot be guaranteed to be timely, secure, error or virus-free. We do not accept liability for any errors or omissions.<br/><br/>";

                    messageBody += "Please do not reply to this message via e-mail. This address is automated, unattended, and cannot help with questions or requests.";

                    var usr = db.spGet_CashRiskAlert(0).ToList();
                    foreach (var item in usr)
                    {
                        await new SMSBL().SendEmail(item.Email, "Cash Risk Branches Alert", messageBody);
                        await new SMSBL().Send(item.Mobile1, sms);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

    }
}