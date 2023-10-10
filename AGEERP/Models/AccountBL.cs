using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace AGEERP.Models
{
    public class AccountBL
    {
        AGEEntities db = new AGEEntities();
        SetupBL setupBL = new SetupBL();

        #region COA
        public async Task<string> GetNewCode(string ParentCode)
        {
            try
            {
                var pId = Convert.ToInt64(ParentCode);
                switch (ParentCode.Length)
                {
                    case 1:
                        {
                            int cl = Convert.ToInt32(ParentCode);
                            var par = await db.Fin_AcGroups.Where(x => x.ClsCode == cl).OrderByDescending(x => x.GrCode).FirstOrDefaultAsync();
                            if (par == null)
                            {
                                return cl.ToString("00") + "-01";
                            }
                            else
                            {
                                int n = Convert.ToInt32(par.GrCode.Substring(3, 2)) + 1;
                                return cl.ToString("00") + "-" + n.ToString("00");
                            }

                        }
                    case 3:
                        {
                            var par = await db.Fin_AcControls.Where(x => x.GrId == pId).OrderByDescending(x => x.CnCode).FirstOrDefaultAsync();
                            if (par == null)
                            {
                                return pId.ToString("00-00") + "-01";
                            }
                            else
                            {
                                int n = Convert.ToInt32(par.CnCode.Substring(6, 2)) + 1;
                                return pId.ToString("00-00") + "-" + n.ToString("00");
                            }
                        }
                    case 5:
                        {
                            var par = await db.Fin_Accounts.Where(x => x.CnId == pId).OrderByDescending(x => x.SubCode).FirstOrDefaultAsync();
                            if (par == null)
                            {
                                return pId.ToString("00-00-00") + "-00001";
                            }
                            else
                            {
                                int n = Convert.ToInt32(par.SubCode.Substring(9, 5)) + 1;
                                return pId.ToString("00-00-00") + "-" + n.ToString("00000");
                            }
                        }
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
        public List<COAVM> SubsidiaryList(long PId, string str)
        {
            try
            {
                return db.spget_Fin_SubsidaryList(PId, str).Select(x =>
                     new COAVM
                     {
                         PCode = PId.ToString(),
                         Name = x.SubsidaryName,
                         Code = x.SubsidaryCode,
                         Id = x.SubId ?? 0,
                         PId = PId,
                     }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<COAVM>> COAList(long PId, int Level)
        {
            try
            {
                if (Level == 1)
                {
                    return await db.Fin_AcClasses.Select(x =>
                     new COAVM
                     {
                         PCode = "",
                         Name = x.ClsDesc,
                         Code = x.ClsCode.ToString(),
                         Id = x.ClsCode,
                         PId = 0
                     }).ToListAsync();
                }
                else if (Level == 2)
                {
                    int parent = Convert.ToInt32(PId);
                    return await db.Fin_AcGroups.Where(x => x.ClsCode == parent).Select(x =>
                     new COAVM
                     {
                         PCode = x.ClsCode.ToString(),
                         Name = x.GrDesc,
                         Code = x.GrCode,
                         Id = x.GrId,
                         PId = PId
                     }).ToListAsync();
                }
                else if (Level == 3)
                {
                    return await db.Fin_AcControls.Where(x => x.GrId == PId).Select(x =>
                     new COAVM
                     {
                         PCode = x.GrCode,
                         Name = x.CnDesc,
                         Code = x.CnCode,
                         Id = x.CnId,
                         PId = PId
                     }).ToListAsync();
                }
                else if (Level == 4)
                {
                    return await db.Fin_Accounts.Where(x => x.CnId == PId).Select(x =>
                     new COAVM
                     {
                         PCode = x.CnCode,
                         Name = x.SubCodeDesc,
                         Code = x.SubCode,
                         Id = x.AccId,
                         PId = PId,
                         SubDivId = x.SubDivId,
                         IsLocked = x.IsLocked,
                         Remarks = x.Remarks
                     }).ToListAsync();
                }
                else if (Level == 5)
                {
                    return db.spget_Fin_SubsidaryList(PId, "").Select(x =>
                     new COAVM
                     {
                         PCode = PId.ToString(),
                         Name = x.SubsidaryName,
                         Code = x.SubsidaryCode,
                         Id = x.SubId ?? 0,
                         PId = PId,
                     }).ToList();
                }
                else
                {
                    return new List<COAVM>();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<COAVM> CreateCOA(COAVM mod, string PId, int Level, int UserId)
        {
            try
            {

                switch (Level)
                {
                    case 1:

                        break;
                    case 2:
                        {
                            //int Id = 0;
                            //int acc = 0;
                            //int parent = Convert.ToInt32(PId);
                            //acc = db.Fin_AcGroups.Where(x => x.ClsCode == parent).Max(x => (int?)x.Id) ?? 0;
                            //if (acc == 0)
                            //{
                            //    Id = (PId * 100) + 1;
                            //}
                            //else
                            //{
                            //    Id = acc + 1;
                            //}
                            //string newCode = await GetNewCode(PId);
                            int parent = Convert.ToInt32(PId);
                            Fin_AcGroups tbl = new Fin_AcGroups
                            {
                                GrCode = mod.Code,
                                GrDesc = mod.Name,
                                ClsCode = parent,
                                GrId = Convert.ToInt32(mod.Code.Replace("-", ""))
                            };
                            db.Fin_AcGroups.Add(tbl);
                            await db.SaveChangesAsync();
                            mod.Code = tbl.GrCode;
                        }
                        break;
                    case 3:
                        {
                            //int Id = 0;
                            //int acc = 0;
                            //acc = db.Fin_COA_THREE.Where(x => x.PId == PId).Max(x => (int?)x.Id) ?? 0;
                            //if (acc == 0)
                            //{
                            //    Id = (PId * 100) + 1;
                            //}
                            //else
                            //{
                            //    Id = acc + 1;
                            //}
                            //Fin_COA_THREE tbl = new Fin_COA_THREE
                            //{
                            //    Id = Id,
                            //    Code = Id.ToString("0#-##-##"),
                            //    Name = mod.Name,
                            //    PId = PId
                            //};
                            //db.Fin_COA_THREE.Add(tbl);
                            //await db.SaveChangesAsync();
                            //mod.Id = tbl.Id;
                            //mod.Code = tbl.Code;

                            //string newCode = await GetNewCode(PId);
                            int parent = Convert.ToInt32(PId.Replace("-", ""));
                            Fin_AcControls tbl = new Fin_AcControls
                            {
                                CnCode = mod.Code,
                                CnDesc = mod.Name,
                                GrCode = PId,
                                GrId = parent,
                                CnId = Convert.ToInt32(mod.Code.Replace("-", ""))
                            };
                            db.Fin_AcControls.Add(tbl);
                            await db.SaveChangesAsync();
                            mod.Code = tbl.CnCode;
                        }
                        break;
                    case 4:
                        {
                            //string newCode = await GetNewCode(PId);
                            int parent = Convert.ToInt32(PId.Replace("-", ""));
                            Fin_Accounts tbl = new Fin_Accounts
                            {
                                CnCode = PId,
                                DefinedBy = UserId,
                                DefinitionDate = DateTime.Now,
                                IsLocked = mod.IsLocked,
                                Remarks = mod.Remarks,
                                SubCode = mod.Code,
                                SubCodeDesc = mod.Name,
                                CnId = parent,
                                AccId = Convert.ToInt64(mod.Code.Replace("-", "")),
                                SubDivId = mod.SubDivId
                            };
                            db.Fin_Accounts.Add(tbl);
                            await db.SaveChangesAsync();
                            mod.Code = tbl.SubCode;
                        }
                        break;
                    default:
                        break;
                }

                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateCOA(COAVM mod, int Level, int UserId)
        {
            try
            {
                switch (Level)
                {
                    case 1:

                        break;
                    case 2:
                        {
                            var tbl = await db.Fin_AcGroups.FindAsync(mod.Id);
                            if (tbl != null)
                            {
                                tbl.GrDesc = mod.Name;
                            }
                        }
                        break;
                    case 3:
                        {
                            var tbl = await db.Fin_AcControls.FindAsync(mod.Id);
                            if (tbl != null)
                            {
                                tbl.CnDesc = mod.Name;
                            }
                        }
                        break;
                    case 4:
                        {
                            var tbl = await db.Fin_Accounts.FindAsync(mod.Id);
                            if (tbl != null)
                            {
                                tbl.SubCodeDesc = mod.Name;
                                tbl.Remarks = mod.Remarks;
                                tbl.IsLocked = mod.IsLocked;
                                tbl.SubDivId = mod.SubDivId;
                            }
                        }
                        break;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<Fin_VoucherTypes>> GetVoucherType()
        {
            try
            {
                return await db.Fin_VoucherTypes.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<ProfitCentersVM>> ProfitCentersByRegList(List<int?> RegLst)
        {
            try
            {
                if (RegLst.Count == 0)
                {
                    return await db.Fin_ProfitCenters.Where(x => x.Status).Select(x =>
                new ProfitCentersVM
                {
                    LocCode = x.LocCode,
                    PCCode = x.PCCode,
                    PCDesc = x.PCDesc
                }).ToListAsync();
                }
                else
                {
                    return await db.Fin_ProfitCenters.Where(x => x.Status && RegLst.Contains(x.RegionId)).Select(x =>
                new ProfitCentersVM
                {
                    LocCode = x.LocCode,
                    PCCode = x.PCCode,
                    PCDesc = x.PCDesc
                }).ToListAsync();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ProfitCentersVM>> ProfitCentersList()
        {
            try
            {
                return await db.Fin_ProfitCenters.Where(x => x.Status).Select(x =>
                new ProfitCentersVM
                {
                    LocCode = x.LocCode,
                    PCCode = x.PCCode,
                    PCDesc = x.PCDesc
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region CostCenters
        public async Task<List<CostCentersVM>> CostCentersList()
        {
            try
            {
                return await db.Fin_CostCenters.Where(x => x.Status && x.CCCode > 0).Select(x =>
                new CostCentersVM
                {
                    CCCode = x.CCCode,
                    CCDesc = x.CCDesc,
                    LocId = x.LocId ?? 0
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<CostCentersVM> CreateCostCenters(CostCentersVM mod, int UserId)
        {
            try
            {
                mod.CCDesc = mod.CCDesc.Trim();
                Fin_CostCenters tbl = new Fin_CostCenters
                {
                    CCDesc = mod.CCDesc,
                    DefinedBy = UserId,
                    DefinitionDate = DateTime.Now,
                    LocId = mod.LocId == 0 ? (int?)null : mod.LocId,
                    Status = true
                };
                db.Fin_CostCenters.Add(tbl);
                await db.SaveChangesAsync();
                mod.CCCode = tbl.CCCode;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCostCenters(CostCentersVM mod, int UserId)
        {
            try
            {
                mod.CCDesc = mod.CCDesc.Trim();
                var tbl = await db.Fin_CostCenters.FindAsync(mod.CCCode);
                if (tbl != null)
                {
                    tbl.CCDesc = mod.CCDesc;
                    tbl.LocId = mod.LocId == 0 ? (int?)null : mod.LocId;
                    tbl.DefinedBy = UserId;
                    tbl.DefinitionDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyCostCenters(CostCentersVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Fin_CostCenters.FindAsync(mod.CCCode);
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.DefinedBy = UserId;
                    tbl.DefinitionDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<CostCentersVM>> CostCentersByProfitCenterList(int PCCode)
        {
            try
            {
                return await db.Fin_CostCenters.Where(x => x.Status && x.CCCode > 0 && x.PCCode == PCCode)
                    .Select(x =>
                new CostCentersVM
                {
                    CCCode = x.CCCode,
                    CCDesc = x.CCDesc,
                    LocId = x.LocId ?? 0
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region BankBook
        public async Task<List<BankBookVM>> BankBookList()
        {
            try
            {
                return await db.Fin_BankBook.Where(x => x.ActiveStatus).Select(x =>
                new BankBookVM
                {
                    CurrentChqNo = x.CurrentChqNo,
                    EndChqNo = x.EndChqNo,
                    StartChqNo = x.StartChqNo,
                    AccId = x.AccId,
                    TransID = x.TransID,
                    AccountType = x.AccountType,
                    BankAccNo = x.BankAccNo,
                    BankName = x.BankName
                    //SubCodeDesc = ""//x.Fin_Accounts.SubCodeDesc          To be discussed
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<BankBookVM> CreateBankBook(BankBookVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //var previousBook = await db.Fin_BankBook.Where(x => x.AccId == mod.AccId && x.ActiveStatus).FirstOrDefaultAsync();
                    //if (previousBook != null)
                    //{
                    //    previousBook.ActiveStatus = false;
                    //}
                    Fin_BankBook tbl = new Fin_BankBook
                    {
                        CurrentChqNo = mod.StartChqNo,
                        EndChqNo = mod.EndChqNo,
                        StartChqNo = mod.StartChqNo,
                        AccountType = mod.AccountType,
                        AccId = mod.AccId,
                        DefinedBy = UserId,
                        DefinitionDate = DateTime.Now,
                        ActiveStatus = true,
                        BankAccNo = mod.BankAccNo,
                        BankName = mod.BankName
                    };
                    db.Fin_BankBook.Add(tbl);
                    await db.SaveChangesAsync();

                    long SRange = Convert.ToInt64(mod.StartChqNo);
                    long ERange = Convert.ToInt64(mod.EndChqNo);
                    int CYear = (await this.CYear(DateTime.Now.Date)).YrCode;
                    int CPeriod = (await this.CPeriod(DateTime.Now.Date)).PrCode;

                    for (long i = SRange; i <= ERange; i++)
                    {
                        Fin_BankBookTrans fbbt = new Fin_BankBookTrans()
                        {
                            VrId = 0,
                            YrCode = CYear,
                            PrCode = CPeriod,
                            AccId = mod.AccId,
                            ChequeNo = i.ToString(),
                            BankBookId = tbl.TransID
                        };
                        db.Fin_BankBookTrans.Add(fbbt);
                    }
                    await db.SaveChangesAsync();
                    mod.TransID = tbl.TransID;
                    scop.Complete();
                    scop.Dispose();
                    return mod;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return null;
                }
            }
        }

        public async Task<bool> UpdateBankBook(BankBookVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Fin_BankBook.FindAsync(mod.TransID);
                if (tbl != null)
                {
                    tbl.CurrentChqNo = mod.CurrentChqNo;
                    tbl.EndChqNo = mod.EndChqNo;
                    tbl.StartChqNo = mod.StartChqNo;
                    tbl.AccountType = mod.AccountType;
                    tbl.AccId = mod.AccId;
                    tbl.DefinedBy = UserId;
                    tbl.DefinitionDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyBankBook(BankBookVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Fin_BankBook.FindAsync(mod.TransID);
                if (tbl != null)
                {
                    tbl.ActiveStatus = false;
                    tbl.DefinedBy = UserId;
                    tbl.DefinitionDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region BankReconciliation
        //public async Task<List<FinBRSDetailVM>> GetBankReconciliation(long AccId, DateTime DocDate)
        //{
        //    try
        //    {
        //        return await db.Fin_BRSDetail.Where(x => x.Fin_BRS.AccId == AccId && x.Fin_BRS.DocDate == DocDate)
        //            .Select(x => new FinBRSDetailVM
        //            {
        //                Balance = x.Balance,
        //                ChequeNo = x.ChequeNo,
        //                CreditAmount = x.CreditAmount,
        //                DebitAmount = x.DebitAmount,
        //                DocDtlId = x.DocDtlId,
        //                DocId = x.DocId,
        //                Narration = x.Narration,
        //                Status = x.Status,
        //                TransactionDate = x.TransactionDate,
        //                VrDtlId = x.VrDtlId ?? 0
        //            }).ToListAsync();
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public async Task<bool> ImportBankStatement(List<FinBRSDetailVM> bdm, long AccId, DateTime DocDate, int UserId)
        {
            try
            {
                var bankSta = await db.Fin_BRS.Where(x => x.DocDate == DocDate && x.AccId == AccId).FirstOrDefaultAsync();
                if (bankSta == null)
                {
                    Fin_BRS br = new Fin_BRS()
                    {
                        AccId = AccId,
                        DocDate = DocDate,
                        TransDate = DateTime.Now,
                        UserId = UserId
                    };
                    db.Fin_BRS.Add(br);
                    await db.SaveChangesAsync();

                    foreach (var item in bdm)
                    {
                        Fin_BRSDetail md = new Fin_BRSDetail()
                        {
                            Balance = item.Balance,
                            ChequeNo = item.ChequeNo ?? "",
                            CreditAmount = item.CreditAmount,
                            DebitAmount = item.DebitAmount,
                            DocId = br.DocId,
                            Narration = item.Narration,
                            TransactionDate = item.TransactionDate,
                            Status = item.Status,
                            VrDtlId = item.VrDtlId
                        };
                        db.Fin_BRSDetail.Add(md);
                        await db.SaveChangesAsync();
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<List<VoucherDetailVM>> BankAccDetail(long AccId, DateTime VrDate)
        {
            try
            {
                return await db.Fin_VoucherDetail.Where(x => x.AccId == AccId && x.Fin_Voucher.VrStatus == "P" && x.Fin_Voucher.VrDate == VrDate)
                    .Select(x => new VoucherDetailVM { VrDtlId = x.VrDtlId, AccId = x.AccId, Cr = x.Cr, Dr = x.Dr, Particulars = x.Particulars, ChequeNo = x.ChequeNo, TrxSeqId = 0 }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Vouchers

        public async Task<string> GetCheque(long AccId)
        {
            try
            {
                var acc = await db.Fin_BankBook.Where(x => x.AccId == AccId && x.ActiveStatus).Select(x => x.CurrentChqNo).FirstOrDefaultAsync();
                if (acc != null)
                {
                    return acc;
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
        public async Task<List<COAVM>> SubCodeBankList()
        {
            try
            {
                return db.spget_BankSubCode().Select(x =>
                new COAVM
                {
                    Name = x.SubCodeDesc,
                    Code = x.SubCode,
                    Id = x.AccId
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<COAVM>> SubCodeCashList()
        {
            try
            {
                return db.spget_CashSubCode().Select(x =>
                new COAVM
                {
                    Name = x.SubCodeDesc,
                    Code = x.SubCode,
                    Id = x.AccId
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<COAVM>> SubCodeList()
        {
            try
            {
                return await db.Fin_Accounts.Where(x => !x.IsLocked).Select(x =>
                new COAVM
                {
                    Name = x.SubCodeDesc,
                    Code = x.SubCode,
                    Id = x.AccId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<COAVM>> SubCodeList(string str)
        {
            try
            {
                string[] strarr = str.Split('%');
                if (strarr.Length > 1)
                {
                    string str1 = strarr[0];
                    string str2 = strarr.Length > 1 ? strarr[1] : "";
                    return await db.Fin_Accounts.Where(x => !x.IsLocked && (x.SubCodeDesc.Contains(str1) && x.SubCodeDesc.Contains(str2))).Select(x =>
                new COAVM
                {
                    Name = x.SubCodeDesc,
                    Code = x.SubCode,
                    Id = x.AccId
                }).ToListAsync();
                }
                else
                {
                    return await db.Fin_Accounts.Where(x => !x.IsLocked && (x.SubCode.Contains(str) || x.SubCodeDesc.Contains(str))).Select(x =>
                new COAVM
                {
                    Name = x.SubCodeDesc,
                    Code = x.SubCode,
                    Id = x.AccId
                }).ToListAsync();
                }

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<COAVM>> SubCodeListByUser(string str, int UserId, int GroupId)
        {
            try
            {
                var lst = new List<COAVM>();
                string[] strarr = str.Split('%');
                if (strarr.Length > 1)
                {
                    string str1 = strarr[0];
                    string str2 = strarr.Length > 1 ? strarr[1] : "";
                    lst = await db.Fin_Accounts.Where(x => !x.IsLocked && (x.SubCodeDesc.Contains(str1) && x.SubCodeDesc.Contains(str2))).Select(x =>
                new COAVM
                {
                    Name = x.SubCodeDesc,
                    Code = x.SubCode,
                    Id = x.AccId
                }).ToListAsync();
                }
                else
                {
                    lst = await db.Fin_Accounts.Where(x => !x.IsLocked && (x.SubCode.Contains(str) || x.SubCodeDesc.Contains(str))).Select(x =>
                new COAVM
                {
                    Name = x.SubCodeDesc,
                    Code = x.SubCode,
                    Id = x.AccId
                }).ToListAsync();
                }

                var rts = await db.Fin_GroupAccess.Where(x => x.GroupId == GroupId).Select(x => x.AccId).ToListAsync();
                rts.AddRange(await db.Fin_UserAccess.Where(x => x.UserId == UserId).Select(x => x.AccId).ToListAsync());
                rts = rts.Distinct().ToList();
                return lst.Where(x => rts.Contains(x.Id)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Fin_Year>> FinancialYears()
        {
            try
            {
                return await db.Fin_Year.Where(x => x.YrStatus == "O").ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<FinancialPeriodVM>> FinancialPeriods(int YrCode)
        {
            try
            {
                //var yrCode = await CYear(DateTime.Now);
                return await db.Fin_Period.Where(x => x.YrCode == YrCode && x.Fin_Year.YrStatus == "O"
                ).Select(x => new FinancialPeriodVM
                {
                    PrCode = x.PrCode,
                    YrCode = x.YrCode,
                    PrEnd = x.PrEnd,
                    PrName = x.PrName,
                    PrStart = x.PrStart,
                    PrStatus = x.PrStatus
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateFinancialPeriod(FinancialPeriodVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Fin_Period.Where(x => x.PrCode == mod.PrCode && x.YrCode == mod.YrCode && x.Fin_Year.YrStatus == "O").FirstOrDefaultAsync();
                if (tbl != null)
                {
                    Fin_PeriodLog log = new Fin_PeriodLog
                    {
                        PrCode = tbl.PrCode,
                        PrStatus = tbl.PrStatus,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        YrCode = tbl.YrCode
                    };
                    db.Fin_PeriodLog.Add(log);
                    tbl.PrStatus = mod.PrStatus;
                    await db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<Fin_Year> CYear(DateTime dt)
        {
            try
            {
                return await db.Fin_Year.Where(x => x.YrStatus == "O" && x.YrStart <= dt && x.YrEnd >= dt
                ).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        //public async Task<Fin_Period> CPeriod()
        //{
        //    try
        //    {
        //        return await db.Fin_Period.Where(x => x.PrStatus == "O"
        //        //&& x.PrStart <= dt && x.PrEnd >= dt
        //        ).FirstOrDefaultAsync();
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public async Task<Fin_Period> CPeriod(DateTime dt)
        {
            try
            {
                return await db.Fin_Period.Where(x => x.PrStatus == "O" && x.PrStart <= dt && x.PrEnd >= dt).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> VCounter(string VType, int CPeriod, int CYear, DateTime dt)
        {
            try
            {
                string VNo = "";
                long counter = 0;
                var VoucherType = await db.Fin_VoucherTypes.Where(x => x.VrTypeId == VType).FirstOrDefaultAsync();
                if (VoucherType.VtNumGeneration == "A")
                {
                    if (VoucherType.VtNumInitialization == "M")
                    {
                        var vcount = await db.Fin_VoucherCounter.Where(x => x.VrTypeID == VType && x.PrCode == CPeriod && x.YrCode == CYear).OrderByDescending(x => x.CounterID).FirstOrDefaultAsync();
                        if (vcount == null)
                        {
                            vcount = new Fin_VoucherCounter
                            {
                                Day = 1,
                                DayCounter = 1,
                                MonthCounter = 1,
                                PrCode = CPeriod,
                                YrCode = CYear,
                                YearCounter = 1,
                                VrTypeID = VType
                            };
                            db.Fin_VoucherCounter.Add(vcount);
                        }
                        else
                        {
                            vcount.MonthCounter = vcount.MonthCounter + 1;
                        }
                        counter = vcount.MonthCounter;
                        VNo = VType + "-" + dt.ToString("yyMM") + "-" + counter.ToString("000000");
                    }
                    else if (VoucherType.VtNumInitialization == "Y")
                    {
                        var vcount = await db.Fin_VoucherCounter.Where(x => x.VrTypeID == VType && x.YrCode == CYear).OrderByDescending(x => x.CounterID).FirstOrDefaultAsync();
                        if (vcount == null)
                        {
                            vcount = new Fin_VoucherCounter
                            {
                                Day = 1,
                                DayCounter = 1,
                                MonthCounter = 1,
                                PrCode = CPeriod,
                                YrCode = CYear,
                                YearCounter = 1,
                                VrTypeID = VType
                            };
                            db.Fin_VoucherCounter.Add(vcount);
                        }
                        else
                        {
                            vcount.YearCounter = vcount.YearCounter + 1;
                        }
                        counter = vcount.MonthCounter;
                        VNo = VType + "-" + dt.ToString("yy") + "-" + counter.ToString("000000");
                    }
                    await db.SaveChangesAsync();

                }
                return VNo;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public int? getSubDivId(string SubsidaryCode)
        {
            int? SubDivId = null;
            if (SubsidaryCode != null)
            {
                switch (SubsidaryCode.Substring(0, 4))
                {
                    case "SUPP":
                        SubDivId = 2;
                        break;
                    case "CUST":
                        SubDivId = 3;
                        break;
                    case "EMPL":
                        SubDivId = 4;
                        break;
                    default:
                        SubDivId = 1;
                        break;
                }
            }
            return SubDivId;
        }
        public async Task<List<VoucherDetailVM>> GetVoucherDetail(long VrId)
        {
            try
            {
                var ls = await db.Fin_VoucherDetail.Where(x => x.VrId == VrId).OrderBy(x => x.TrxSeqId).ToListAsync();
                var lst = new List<VoucherDetailVM>();
                foreach (var x in ls)
                {
                    var itm = new VoucherDetailVM
                    {
                        CCCode = x.CCCode,
                        ChequeNo = x.ChequeNo,
                        Cr = x.Cr,
                        Dr = x.Dr,
                        Particulars = x.Particulars,
                        PCCode = x.PCCode,
                        SubCode = x.Fin_Accounts.SubCode,
                        SubCodeDesc = x.Fin_Accounts.SubCodeDesc,
                        RefId = x.RefId,
                        SubId = x.SubId ?? 0,
                        VrDtlId = x.VrDtlId,
                        AccId = x.AccId,
                        VrId = x.VrId
                    };
                    if (x.SubId > 0)
                    {
                        switch (x.Fin_Accounts.SubDivId)
                        {
                            case 2:
                                {
                                    var sub = await db.Inv_Suppliers.Where(a => a.SuppId == x.SubId).Select(a => a.SuppName).FirstOrDefaultAsync();
                                    itm.SubsidaryCode = "SUPP-" + x.SubId;
                                    itm.SubsidaryDesc = sub;
                                    break;
                                }
                            case 3:
                                {
                                    var sub = await db.Lse_Master.Where(a => a.AccNo == x.SubId).Select(a => a.CustName).FirstOrDefaultAsync();
                                    itm.SubsidaryCode = "CUST-" + x.SubId;
                                    itm.SubsidaryDesc = sub;
                                    break;
                                }
                            case 4:
                                {
                                    var sub = await db.Pay_EmpMaster.Where(a => a.EmpId == x.SubId).Select(a => new { a.EmpName, a.CNIC }).FirstOrDefaultAsync();
                                    itm.SubsidaryCode = "EMPL-" + sub.CNIC;
                                    itm.SubsidaryDesc = sub.EmpName;
                                    break;
                                }
                            default:
                                {
                                    var sub = await db.Fin_Subsidary.Where(a => a.SubId == x.SubId).Select(a => new { a.SubsidaryCode, a.SubsidaryName }).FirstOrDefaultAsync();
                                    itm.SubsidaryCode = sub.SubsidaryCode;
                                    itm.SubsidaryDesc = sub.SubsidaryName;
                                    break;
                                }
                        }
                    }
                    lst.Add(itm);
                }
                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> UpdateVoucher(VoucherDetailVM mod, int UserId)
        {
            try
            {
                var mas = db.Fin_Voucher.Where(x => x.VrId == mod.VrId).FirstOrDefault();
                mas.ModifiedBy = UserId;
                mas.ModifiedDate = DateTime.Now;

                var det = db.Fin_VoucherDetail.Where(x => x.VrDtlId == mod.VrDtlId).FirstOrDefault();
                det.Particulars = mod.Particulars;
                det.SubId = mod.SubId == 0 ? (long?)null : mod.SubId;

                await db.SaveChangesAsync();

                if (mod.SubId > 0)
                {
                    switch (det.Fin_Accounts.SubDivId)
                    {
                        case 2:
                            {
                                var sub = await db.Inv_Suppliers.Where(a => a.SuppId == mod.SubId).Select(a => a.SuppName).FirstOrDefaultAsync();
                                mod.SubsidaryCode = "SUPP-" + mod.SubId;
                                mod.SubsidaryDesc = sub;
                                break;
                            }
                        case 3:
                            {
                                var sub = await db.Lse_Master.Where(a => a.AccNo == mod.SubId).Select(a => a.CustName).FirstOrDefaultAsync();
                                mod.SubsidaryCode = "CUST-" + mod.SubId;
                                mod.SubsidaryDesc = sub;
                                break;
                            }
                        case 4:
                            {
                                var sub = await db.Pay_EmpMaster.Where(a => a.EmpId == mod.SubId).Select(a => new { a.EmpName, a.CNIC }).FirstOrDefaultAsync();
                                mod.SubsidaryCode = "EMPL-" + sub.CNIC;
                                mod.SubsidaryDesc = sub.EmpName;
                                break;
                            }
                        default:
                            {
                                var sub = await db.Fin_Subsidary.Where(a => a.SubId == mod.SubId).Select(a => new { a.SubsidaryCode, a.SubsidaryName }).FirstOrDefaultAsync();
                                mod.SubsidaryCode = sub.SubsidaryCode;
                                mod.SubsidaryDesc = sub.SubsidaryName;
                                break;
                            }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<string> CreateJV(IEnumerable<VoucherDetailVM> mod, DateTime VoucherDate, string VType, string RefDocNo, DateTime RefDocDate, int UserId)
        {
            try
            {
                if (mod.Sum(x => x.Cr) != mod.Sum(x => x.Dr))
                    return "";

                foreach (var item in mod)
                {
                    var Acc = await db.Fin_Accounts.FindAsync(Convert.ToInt64(item.AccId));
                    if (Acc.HasSub && item.SubId == 0)
                    {
                        return "Subsidiary Required for " + Acc.SubCode;
                    }
                    if (Acc.IsHOGL && item.CCCode != 72)
                    {
                        return Acc.SubCode + " should be in HO Cost Center";
                    }
                }

                int CYear = (await this.CYear(VoucherDate)).YrCode;
                int CPeriod = (await this.CPeriod(VoucherDate)).PrCode;
                string VNo = await this.VCounter(VType, CPeriod, CYear, VoucherDate);

                var clearingAcc = await GetAcc(459);

                Fin_Voucher mas = new Fin_Voucher()
                {
                    CreatedBy = UserId,
                    CreatedDate = DateTime.Now,
                    PrCode = CPeriod,
                    RefDocDate = RefDocDate,
                    RefDocNo = RefDocNo,
                    VrDate = VoucherDate,
                    VrNo = VNo,
                    VrStatus = "U",
                    VrTypeId = VType,
                    YrCode = CYear
                };
                List<Fin_VoucherDetail> lst = new List<Fin_VoucherDetail>();
                int i = 1;
                foreach (var v in mod)
                {
                    int pcCode = await db.Fin_CostCenters.Where(x => x.CCCode == v.CCCode).Select(x => x.PCCode).FirstOrDefaultAsync() ?? 0;
                    if (v.PCCode == pcCode || v.CCCode == 0)
                    {
                        lst.Add(new Fin_VoucherDetail
                        {
                            Cr = v.Cr,
                            Dr = v.Dr,
                            CCCode = v.CCCode,
                            Particulars = v.Particulars,
                            PCCode = v.PCCode,
                            AccId = v.AccId,
                            SubId = v.SubId == 0 ? (long?)null : v.SubId,
                            SubDivId = getSubDivId(v.SubsidaryCode),
                            TrxSeqId = i++
                        });
                    }
                    else
                    {
                        lst.Add(new Fin_VoucherDetail
                        {
                            Cr = v.Cr,
                            Dr = v.Dr,
                            CCCode = v.CCCode,
                            Particulars = v.Particulars,
                            PCCode = pcCode,
                            AccId = v.AccId,
                            SubId = v.SubId == 0 ? (long?)null : v.SubId,
                            SubDivId = getSubDivId(v.SubsidaryCode),
                            TrxSeqId = i++
                        });
                    }
                }

                var pc = lst.Select(x => x.PCCode).Distinct().ToList();
                foreach (var v in pc)
                {
                    var dif = lst.Where(x => x.PCCode == v).Sum(x => x.Dr - x.Cr);
                    if (dif != 0)
                    {
                        lst.Add(new Fin_VoucherDetail
                        {
                            Cr = dif > 0 ? dif : 0,
                            Dr = dif < 0 ? (dif * -1) : 0,
                            CCCode = v,
                            Particulars = "Clearing JV",
                            PCCode = v,
                            AccId = clearingAcc,
                            SubId = 0,
                            TrxSeqId = i++
                        });
                    }
                }

                //if(VType == "CPV")
                //{
                //    lst.Add(new Fin_VoucherDetail
                //    {
                //        Cr = v.Cr,
                //        Dr = v.Dr,
                //        CCCode = v.CCCode,
                //        Particulars = v.Particulars,
                //        PCCode = v.PCCode,
                //        SubCode = v.SubCode,
                //        TrxSeqId = i++
                //    });
                //}


                mas.Fin_VoucherDetail = lst;
                db.Fin_Voucher.Add(mas);
                await db.SaveChangesAsync();

                return mas.VrId.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task<string> CreateCPV(IEnumerable<VoucherDetailVM> mod, DateTime VoucherDate, string VType, long CashAcc, string PaidTo, string MasParticular, string RefDocNo, DateTime RefDocDate, bool IsMulti, int UserId)
        {
            try
            {
                foreach (var item in mod)
                {
                    var Acc = await db.Fin_Accounts.FindAsync(Convert.ToInt64(item.AccId));
                    if (Acc.HasSub && item.SubId == 0)
                    {
                        return "Subsidiary Required for " + Acc.SubCode;
                    }
                    if (Acc.IsHOGL && item.CCCode != 72)
                    {
                        return Acc.SubCode + " should be in HO Cost Center";
                    }
                }
                int CYear = (await this.CYear(VoucherDate)).YrCode;
                int CPeriod = (await this.CPeriod(VoucherDate)).PrCode;
                string VNo = await this.VCounter(VType, CPeriod, CYear, VoucherDate);

                Fin_Voucher mas = new Fin_Voucher()
                {
                    CreatedBy = UserId,
                    CreatedDate = DateTime.Now,
                    PrCode = CPeriod,
                    RefDocDate = RefDocDate,
                    RefDocNo = RefDocNo,
                    VrDate = VoucherDate,
                    VrNo = VNo,
                    VrStatus = "U",
                    VrTypeId = VType,
                    YrCode = CYear,
                    VrPaidTo = PaidTo
                };
                List<Fin_VoucherDetail> lst = new List<Fin_VoucherDetail>();
                var ls = mod.GroupBy(x => new { x.PCCode, x.CCCode }).Select(x => new Fin_VoucherDetail
                {
                    PCCode = x.Key.PCCode,
                    CCCode = x.Key.CCCode,
                    Dr = x.Sum(a => VType == "CPV" ? a.Dr : a.Cr)
                }).ToList();

                int i = 1;
                foreach (var m in ls)
                {
                    var lss = mod.Where(x => x.PCCode == m.PCCode && x.CCCode == m.CCCode).ToList();
                    foreach (var v in lss)
                    {
                        if (IsMulti)
                        {
                            lst.Add(new Fin_VoucherDetail
                            {
                                Cr = VType == "CPV" ? v.Dr : 0,
                                Dr = VType == "CPV" ? 0 : v.Cr,
                                CCCode = v.CCCode,
                                Particulars = v.Particulars,
                                PCCode = v.PCCode,
                                AccId = CashAcc,
                                TrxSeqId = i++
                            });
                        }
                        lst.Add(new Fin_VoucherDetail
                        {
                            Cr = VType == "CPV" ? 0 : v.Cr,
                            Dr = VType == "CPV" ? v.Dr : 0,
                            CCCode = v.CCCode,
                            Particulars = v.Particulars,
                            PCCode = v.PCCode,
                            AccId = v.AccId,
                            SubId = v.SubId == 0 ? (long?)null : v.SubId,
                            SubDivId = getSubDivId(v.SubsidaryCode),
                            TrxSeqId = i++
                        });
                    }
                    if (!IsMulti)
                    {
                        lst.Add(new Fin_VoucherDetail
                        {
                            Cr = VType == "CPV" ? m.Dr : 0,
                            Dr = VType == "CPV" ? 0 : m.Dr,
                            CCCode = m.CCCode,
                            Particulars = MasParticular,
                            PCCode = m.PCCode,
                            AccId = CashAcc,
                            TrxSeqId = i++
                        });
                    }
                }

                mas.Fin_VoucherDetail = lst;
                db.Fin_Voucher.Add(mas);
                await db.SaveChangesAsync();

                return mas.VrId.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }



        public async Task<string> CreateBPV(IEnumerable<VoucherDetailVM> mod, DateTime VoucherDate, string VType, long BankAcc, string PaidTo, string MasParticular, string RefDocNo, DateTime RefDocDate, string ChequeNo, bool IsMulti, int UserId, string instrumentno, string instrumenttype)
        {
            try
            {
                if (VType == "BPV" && IsMulti == false && ChequeNo != "")
                {
                    var cheq = await db.Fin_BankBookTrans.Where(x => x.AccId == BankAcc && x.ChequeNo == ChequeNo).FirstOrDefaultAsync();
                    if (cheq.Status != null)
                    {
                        //    var chq = await db.Fin_BankBook.Where(x => x.AccId == BankAcc).FirstOrDefaultAsync();
                        //    chq.CurrentChqNo = (Convert.ToInt64(chq.CurrentChqNo) + 1).ToString();
                        //}
                        //else
                        //{
                        return "Duplicate Cheque Entry";
                    }
                }
                foreach (var item in mod)
                {
                    var Acc = await db.Fin_Accounts.FindAsync(Convert.ToInt64(item.AccId));
                    if (Acc.HasSub && item.SubId == 0)
                    {
                        return "Subsidiary Required for " + Acc.SubCode;
                    }
                    if (Acc.IsHOGL && item.CCCode != 72)
                    {
                        return Acc.SubCode + " should be in HO Cost Center";
                    }
                }

                int CYear = (await this.CYear(VoucherDate)).YrCode;
                int CPeriod = (await this.CPeriod(VoucherDate)).PrCode;
                string VNo = await this.VCounter(VType, CPeriod, CYear, VoucherDate);

                Fin_Voucher mas = new Fin_Voucher()
                {
                    CreatedBy = UserId,
                    CreatedDate = DateTime.Now,
                    PrCode = CPeriod,
                    RefDocDate = RefDocDate,
                    RefDocNo = RefDocNo,
                    VrDate = VoucherDate,
                    VrNo = VNo,
                    VrStatus = "U",
                    VrTypeId = VType,
                    YrCode = CYear,
                    VrPaidTo = PaidTo
                };
                List<Fin_VoucherDetail> lst = new List<Fin_VoucherDetail>();


                int i = 1;
                //foreach (var m in ls)
                //{
                //    var lss = mod.Where(x => x.PCCode == m.PCCode && x.CCCode == m.CCCode).ToList();


                foreach (var v in mod)
                {
                    if (IsMulti)
                    {
                        lst.Add(new Fin_VoucherDetail
                        {
                            Cr = VType == "BPV" ? v.Dr : 0,
                            Dr = VType == "BPV" ? 0 : v.Cr,
                            CCCode = v.CCCode,
                            Particulars = v.Particulars,
                            PCCode = 72,
                            AccId = BankAcc,
                            TrxSeqId = i++,
                            ChequeNo = VType == "BPV" ? ChequeNo : null,
                            Instrument = instrumenttype,
                            InstrumentNo = instrumentno
                        });
                    }
                    lst.Add(new Fin_VoucherDetail
                    {
                        Cr = VType == "BPV" ? 0 : v.Cr,
                        Dr = VType == "BPV" ? v.Dr : 0,
                        CCCode = v.CCCode,
                        Particulars = v.Particulars,
                        PCCode = v.PCCode,
                        AccId = v.AccId,
                        SubId = v.SubId == 0 ? (long?)null : v.SubId,
                        SubDivId = getSubDivId(v.SubsidaryCode),
                        ChequeNo = VType == "BPV" ? null : v.ChequeNo,
                        TrxSeqId = i++
                    });
                }

                //}
                if (!IsMulti)
                {
                    var m = mod.GroupBy(x => x.VrId).Select(x => new Fin_VoucherDetail
                    {
                        PCCode = 72,// x.FirstOrDefault().PCCode,
                        CCCode = 72, //x.FirstOrDefault().CCCode,
                        Dr = x.Sum(a => VType == "BPV" ? a.Dr : a.Cr)
                    }).FirstOrDefault();



                    lst.Add(new Fin_VoucherDetail
                    {
                        Cr = VType == "BPV" ? m.Dr : 0,
                        Dr = VType == "BPV" ? 0 : m.Dr,
                        CCCode = m.CCCode,
                        Particulars = MasParticular,
                        PCCode = m.PCCode,
                        AccId = BankAcc,
                        TrxSeqId = i++,
                        ChequeNo = VType == "BPV" ? ChequeNo : null,
                    });
                }

                var clearingAcc = await GetAcc(459);
                var pc = lst.Select(x => x.PCCode).Distinct().ToList();
                foreach (var v in pc)
                {
                    var dif = lst.Where(x => x.PCCode == v).Sum(x => x.Dr - x.Cr);
                    if (dif != 0)
                    {
                        lst.Add(new Fin_VoucherDetail
                        {
                            Cr = dif > 0 ? dif : 0,
                            Dr = dif < 0 ? (dif * -1) : 0,
                            CCCode = v,
                            Particulars = "Clearing " + VType,
                            PCCode = v,
                            AccId = clearingAcc,
                            SubId = 0,
                            TrxSeqId = i++
                        });
                    }
                }

                if (VType == "BPV" && IsMulti == false)
                {
                    //mas.Fin_BankBookTrans.Add(new Fin_BankBookTrans()
                    //{
                    //    ChequeNo = ChequeNo,
                    //    PrCode = CPeriod,
                    //    AccId = BankAcc,
                    //    YrCode = CYear
                    //});

                    var fbt = await db.Fin_BankBookTrans.Where(x => x.AccId == BankAcc && x.ChequeNo == ChequeNo && x.Status == null).FirstOrDefaultAsync();
                    if (fbt != null)
                    {
                        fbt.Amount = mod.Sum(x => x.Dr);
                        fbt.ChequeDate = RefDocDate;
                        fbt.ChequeNo = ChequeNo;
                        fbt.ChequeType = "Presentable";
                        fbt.PaymentType = "BPV";
                        fbt.Recipient = PaidTo;
                        fbt.Status = "U";
                        fbt.UserId = UserId;
                        fbt.TransDate = DateTime.Now;
                        fbt.InstrumentNo = instrumentno;
                        fbt.InstrumentType = instrumenttype;
                        fbt.Remarks = MasParticular;
                        await db.SaveChangesAsync();
                    }



                }

                mas.Fin_VoucherDetail = lst;
                db.Fin_Voucher.Add(mas);
                await db.SaveChangesAsync();

                return mas.VrId.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
        public async Task<List<VoucherSearchVM>> GetVoucherSearch(DateTime FromDate, DateTime ToDate, string TypeId, string VNo, string Narration, string Code, int PCCode, string ChequeNo, string VStatus, string RefDocNo)
        {
            try
            {
                //return db.spget_VoucherSearch(FromDate, ToDate, TypeId, VNo, Narration, Code, PCCode, ChequeNo, VStatus, RefDocNo).Select(x =>
                //   new VoucherSearchVM
                //   {
                //       VrDate = x.VrDate,
                //       VrId = x.VrId,
                //       VrNo = x.VrNo,
                //       VrTypeId = x.VrTypeId,
                //       Amount = x.Amount ?? 0,
                //       UserId = x.CreatedBy
                //   }).ToList();

                return new List<VoucherSearchVM>();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> VoucherApproval(VoucherSearchVM mod, string Level, int UserId)
        {
            try
            {
                var vr = await db.Fin_Voucher.FindAsync(mod.VrId);
                if (Level == "V" && mod.Status == "V")
                {
                    if (vr.CheckedBy == null)
                    {
                        vr.CheckedBy = UserId;
                        vr.CheckedDate = DateTime.Now;
                    }
                }
                else if (Level == "A" && mod.Status == "A")
                {
                    if (vr.ApprovedBy == null && vr.CheckedBy != null)
                    {
                        vr.ApprovedBy = UserId;
                        vr.ApprovedDate = DateTime.Now;
                        vr.VrStatus = "P";
                    }
                }
                else if (mod.Status == "R")
                {
                    if (vr.ApprovedBy == null)
                    {
                        vr.RevokedBy = UserId;
                        vr.RevokedDate = DateTime.Now;
                    }
                    if (vr.CheckedBy != null)
                    {
                        vr.CheckedBy = null;
                        vr.CheckedDate = null;
                    }
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<Fin_Voucher> GetVoucher(long VrId)
        {
            try
            {
                return await db.Fin_Voucher.FindAsync(VrId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<VoucherSearchVM>> GetVoucherList()
        {
            try
            {
                var lst = await db.Fin_Voucher.Where(x => x.CheckedBy == null).Select(x =>
                       new VoucherSearchVM
                       {
                           VrDate = x.VrDate,
                           VrId = x.VrId,
                           VrNo = x.VrNo,
                           VrTypeId = x.VrTypeId,
                           Amount = x.Fin_VoucherDetail.Sum(a => a.Cr)
                       }).ToListAsync();
                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<VoucherSearchVM>> GetVoucherApproval(string Level, int UserId)
        {
            try
            {
                List<VoucherSearchVM> lst = new List<VoucherSearchVM>();
                var usr = await db.Users_Login.FindAsync(UserId);
                var approval = usr.Users_Group.Users_GroupAccess.Where(x => x.MenuId == 15120000).Any();
                var validate = usr.Users_Group.Users_GroupAccess.Where(x => x.MenuId == 15110000).Any();
                if (Level == "V" && validate)
                {
                    lst = await (from x in db.Fin_Voucher
                                 join C in db.Users_Login on x.CreatedBy equals C.UserID
                                 where x.VrStatus == "U" && x.CheckedBy == null && x.RevokedBy == null
                                 select
                        new VoucherSearchVM
                        {
                            VrDate = x.VrDate,
                            VrId = x.VrId,
                            VrNo = x.VrNo,
                            VrTypeId = x.VrTypeId,
                            Amount = x.Fin_VoucherDetail.Sum(a => a.Cr),
                            Status = x.RevokedBy == null ? "" : "R",
                            CreateBy = C.FullName,
                            ValidateBy = ""
                        }).ToListAsync();

                }
                else if (Level == "A" && approval)
                {
                    lst = await (from x in db.Fin_Voucher
                                 join C in db.Users_Login on x.CreatedBy equals C.UserID
                                 join CH in db.Users_Login on x.CheckedBy equals CH.UserID
                                 where x.VrStatus == "U" && x.CheckedBy != null && x.ApprovedBy == null && x.RevokedBy == null
                                 select
                        new VoucherSearchVM
                        {
                            VrDate = x.VrDate,
                            VrId = x.VrId,
                            VrNo = x.VrNo,
                            VrTypeId = x.VrTypeId,
                            Amount = x.Fin_VoucherDetail.Sum(a => a.Cr),
                            CreateBy = C.FullName,
                            ValidateBy = CH.FullName
                        }).ToListAsync();

                }
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region PaymentAdvice
        public List<PaymentAdviceDetailVM> GetPaymentAdvice(bool Reg, long AccId)
        {
            return db.spget_PaymentAdvice(Reg, AccId).Select(x =>
             new PaymentAdviceDetailVM
             {
                 ChequeName = "",
                 ChequeNo = "",
                 LedgerClosingBal = x.Balance ?? 0,
                 SubCode = x.SubCode,
                 SubCodeDesc = x.SubCodeDesc,
                 TaxRate = x.TaxRate,
                 AdviceDtlID = 0,
                 PaymentAmount = 0,
                 CheqValue = 0,
                 TaxAmount = 0,
                 SuppId = x.SuppId,
                 Supplier = x.SuppName
             }).ToList();
        }

        public async Task<string> CreatePaymentAdvice(IEnumerable<PaymentAdviceDetailVM> mod, bool Reg, int UserId)
        {
            try
            {
                var lst = mod.Select(x => new Fin_PaymentAdviceDetail
                {
                    ChequeName = x.ChequeName,
                    ChequeNo = x.ChequeNo,
                    CheqValue = x.CheqValue,
                    isMerged = false,
                    LedgerClosingBal = x.LedgerClosingBal,
                    MergeValue = 0,
                    PaymentAmount = x.PaymentAmount,
                    SubCode = x.SubCode,
                    TaxAmount = x.TaxAmount,
                    TaxRate = x.TaxRate
                }).ToList();

                var docNo = await db.Fin_PaymentAdvice.OrderByDescending(x => x.AdviceID).Select(x => x.DocumentNo).FirstOrDefaultAsync();
                if (docNo != null)
                {
                    if (docNo.Substring(0, 4) == DateTime.Now.ToString("yyMM"))
                    {
                        docNo = DateTime.Now.ToString("yyMM") + (Convert.ToInt32(docNo.Substring(4, 4)) + 1).ToString("0000");
                    }
                    else
                    {
                        docNo = DateTime.Now.ToString("yyMM") + "0001";
                    }
                }
                else
                {
                    docNo = DateTime.Now.ToString("yyMM") + "0001";
                }

                var mas = new Fin_PaymentAdvice
                {
                    DocumentNo = docNo,
                    DocumentType = Reg ? "Registered" : "Non Registered",
                    Locked4Validation = false,
                    PostStatus = "U",
                    PreparedBy = UserId,
                    PreparationDate = DateTime.Now,
                    Fin_PaymentAdviceDetail = lst
                };
                db.Fin_PaymentAdvice.Add(mas);
                await db.SaveChangesAsync();
                return docNo;
            }
            catch (Exception)
            {
                return "";
            }
        }

        #endregion

        #region SystemInegrationlist
        public async Task<List<SystemIntegrationVM>> SystemInegrationlist()
        {
            try
            {
                return await db.Comp_SystemIntegration.Select(a =>
               new SystemIntegrationVM
               {
                   GLCode = a.GLCode,
                   TransDescription = a.TransDescription,
                   TransId = a.TransId
               }).ToListAsync();
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<bool> UpdateIntegration(SystemIntegrationVM mob, int userid)
        {
            try
            {
                var tbl = await db.Comp_SystemIntegration.SingleOrDefaultAsync(x => x.TransId.Equals(mob.TransId));
                if (tbl != null)
                {
                    tbl.GLCode = mob.GLCode;
                    tbl.ModifiedBy = userid;
                    tbl.ModifiedDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        #endregion

        #region SaleVoucherPosting
        public async Task<bool> AutoVoucherPosting()
        {
            try
            {
                //if(dt == DateTime.Now.Date)
                //return false;

                DateTime workingDate = DateTime.Now.Date.AddDays(-1);
                //DateTime fromDate = Convert.ToDateTime("2021-10-01");
                var TransTypeLst = new int[] { 1, 2, 5, 11, 6, 7, 8, 9, 13, 14, 3, 15 };
                //var loc = await db.Comp_Locations.Where(x => x.Status == true && x.LocTypeId == 1).ToListAsync();
                foreach (var tType in TransTypeLst)
                {
                    if (tType == 1)
                    {
                        var loc = await db.Inv_Sale.Where(x => x.TransactionTypeId == 1 && !x.IsPosted && x.BillDate == workingDate).Select(x => new { x.LocId, WorkingDate = x.BillDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    else if (tType == 2)
                    {
                        var loc = await db.Inv_Sale.Where(x => x.TransactionTypeId == 2 && !x.IsPosted && x.BillDate == workingDate).Select(x => new { x.LocId, WorkingDate = x.BillDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    else if (tType == 5)
                    {
                        var loc = await db.Inv_Sale.Where(x => x.TransactionTypeId == 5 && !x.IsPosted && x.BillDate == workingDate).Select(x => new { x.LocId, WorkingDate = x.BillDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    else if (tType == 11)
                    {
                        var loc = await db.Inv_Sale.Where(x => x.TransactionTypeId == 11 && !x.IsPosted && x.BillDate == workingDate).Select(x => new { x.LocId, WorkingDate = x.BillDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    else if (tType == 6)
                    {
                        var loc = await db.Inv_Sale.Where(x => x.TransactionTypeId == 6 && !x.IsPosted && x.BillDate == workingDate).Select(x => new { x.LocId, WorkingDate = x.BillDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    else if (tType == 7)
                    {
                        var loc = await db.Lse_Master.Where(x => !x.IsPosted && x.DeliveryDate == workingDate).Select(x => new { x.LocId, WorkingDate = (DateTime)x.DeliveryDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    else if (tType == 8)
                    {
                        var loc = await db.Lse_Return.Where(x => !x.IsPosted && x.WorkingDate == workingDate).Select(x => new { x.LocId, WorkingDate = x.WorkingDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    else if (tType == 9)
                    {
                        var loc = await db.Lse_Installment.Where(x => !x.IsPosted && x.InstDate == workingDate).Select(x => new { x.LocId, WorkingDate = x.InstDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    else if (tType == 13)
                    {
                        var loc = await db.Inv_SaleOrder.Where(x => x.TransactionTypeId == 13 && !x.IsPosted && x.BillDate == workingDate).Select(x => new { x.LocId, WorkingDate = x.BillDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    else if (tType == 14)
                    {
                        var loc = await db.Inv_SaleOrder.Where(x => x.TransactionTypeId == 14 && !x.IsPosted && x.BillDate == workingDate).Select(x => new { x.LocId, WorkingDate = x.BillDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    else if (tType == 3)
                    {
                        var loc = await (from PP in db.Inv_POPayment
                                         join P in db.Inv_Purchase on PP.POInvId equals P.PInvId
                                         join PO in db.Inv_PO on P.POId equals PO.POId
                                         where PO.POTypeId == 3 && !PP.IsPosted && PP.WorkingDate == workingDate
                                         select new { PP.LocId, WorkingDate = PP.WorkingDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    else if (tType == 15)
                    {
                        var loc = await db.Inv_SaleCreditReceive.Where(x => !x.IsPosted && x.WorkingDate == workingDate).Select(x => new { x.LocId, WorkingDate = x.WorkingDate }).Distinct().ToListAsync();
                        foreach (var l in loc)
                        {
                            var lst = GetSaleForVoucherPosting(l.LocId, tType, l.WorkingDate);
                            await SaveSaleVoucherPosting(lst, tType, l.WorkingDate, 100003);
                        }
                    }
                    //else if (tType == 20)
                    //{
                    //    DateTime fDt = Convert.ToDateTime("2021-03-01");
                    //    DateTime tDt = Convert.ToDateTime("2021-06-17");
                    //    var loc = await db.Lse_ExpenseTransaction.Where(x => !x.IsPosted && x.Comp_Locations.CityId == 17 && x.WorkingDate >= fDt && x.WorkingDate <= tDt && x.ExpHeadId != 87).Select(x => new { x.LocId, WorkingDate = x.WorkingDate }).Distinct().ToListAsync();
                    //    foreach (var l in loc)
                    //    {
                    //        var lst = await ExpenseVoucherPostingList(l.LocId, 0, l.WorkingDate);
                    //        var ls = lst.Where(x => x.ExpHeadId != 87).Select(x => x.TransId).ToList();
                    //        await PostExpenseVoucherPosting(ls, 100003);
                    //    }
                    //}
                    //else if (tType == 21)
                    //{
                    //    DateTime fDt = Convert.ToDateTime("2021-03-01");
                    //    DateTime tDt = Convert.ToDateTime("2021-06-17");
                    //    var loc = await db.Lse_CashReceive.Where(x => x.Status != "C" && !x.IsPosted && x.LocId >= 155 && x.LocId <= 183 && x.WorkingDate >= fDt && x.WorkingDate <= tDt).Select(x => new { x.LocId, WorkingDate = x.WorkingDate }).Distinct().ToListAsync();
                    //    foreach (var l in loc)
                    //    {
                    //        var lst = await CashReceivePostingList(l.LocId, l.WorkingDate);
                    //        var ls = lst.Select(x => x.TransId).ToList();
                    //        await PostCashReceiveVoucherPosting(ls, 100003);
                    //    }
                    //}
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<SaleForVoucherPostingVM> GetSaleForVoucherPosting(int LocId, int TransId, DateTime WorkingDate)
        {
            try
            {
                db = new AGEEntities();
                return db.spget_SaleForVoucherPosting_V1(LocId, TransId, WorkingDate).Select(x => new
                 SaleForVoucherPostingVM
                {
                    LocId = x.LocId,
                    TransId = x.TransId,
                    BillDate = x.BillDate,
                    BillNo = x.BillNo,
                    CustCellNo = x.CustCellNo,
                    CustId = x.CustId ?? 0,
                    CustName = x.CustName,
                    Discount = x.Discount,
                    SPrice = x.SPrice,
                    Advance = x.Advance,
                    PPrice = x.PPrice
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<SaleByTypeVM> GetSaleByType(DateTime WorkingDate, int LocId, string Type)
        {
            try
            {
                return db.spget_SaleByType(WorkingDate, LocId, Type).Select(x => new
                 SaleByTypeVM
                {
                    BillNo = x.BillNo,
                    SerialNo = x.SerialNo,
                    SKUCode = x.SKUCode,
                    SPrice = x.SPrice,
                    TransactionType = x.TransactionType,
                    TransId = x.TransId
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<long> GetAcc(int TransId)
        {
            try
            {
                var str = await db.Comp_SystemIntegration.Where(x => x.TransId == TransId).Select(x => x.GLCode).FirstOrDefaultAsync();
                if (!str.IsNullOrWhiteSpace())
                {
                    return Convert.ToInt64(str);
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<long> GetSuppAcc(int TypeId, int CategoryId)
        {
            try
            {
                var str = await db.Inv_SuppCatGL.Where(x => x.TypeId == TypeId && x.CategoryId == CategoryId).Select(x => x.GLCode).FirstOrDefaultAsync();
                return str;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<long> GetAcc(int TypeId, int CategoryId)
        {
            try
            {
                var str = await db.Comp_SystemIntegration.Where(x => x.TransTypeId == TypeId && x.CategoryId == CategoryId).Select(x => x.GLCode).FirstOrDefaultAsync();
                if (!str.IsNullOrWhiteSpace())
                {
                    return Convert.ToInt64(str);
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<bool> InstallmentAdjustmentVoucher(long InstId, decimal SPrice, decimal Discount, int UserId)
        {
            db = new AGEEntities();
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    long instDiscountAcc = await GetAcc(446);
                    long InstAcc = await GetAcc(405);
                    var cashInHand = await GetAcc(428);
                    var clrAcc = await GetAcc(450);

                    List<VoucherDetailVM> vLst = new List<VoucherDetailVM>();

                    var tbl = await db.Lse_Installment.FindAsync(InstId);

                    if (tbl.LocId != tbl.Lse_Master.LocId)
                    {
                        if (SPrice > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = cashInHand,
                                CCCode = tbl.LocId,
                                ChequeNo = "",
                                Cr = SPrice,
                                Dr = 0,
                                Particulars = "Installment Adjustment.",
                                PCCode = tbl.LocId,
                                SubId = 0
                            });
                        if (SPrice > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = clrAcc,
                                CCCode = tbl.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = SPrice,
                                Particulars = "Installment Adjustment.",
                                PCCode = tbl.LocId,
                                SubId = 0
                            });
                        if (SPrice > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = clrAcc,
                                CCCode = tbl.LocId,
                                ChequeNo = "",
                                Cr = SPrice,
                                Dr = 0,
                                Particulars = "Installment Adjustment.",
                                PCCode = tbl.Lse_Master.LocId,
                                SubId = 0
                            });
                        if (SPrice > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = InstAcc,
                                CCCode = tbl.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = SPrice,
                                Particulars = "Installment Adjustment. AccNo " + tbl.AccNo.ToString() + " RecvNo " + tbl.InstId.ToString(),
                                PCCode = tbl.Lse_Master.LocId,
                                SubId = tbl.AccNo,
                                SubsidaryCode = "CUST-" + tbl.AccNo.ToString(),
                                RefId = InstId
                            });
                    }
                    else
                    {
                        if (SPrice > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = cashInHand,
                                CCCode = tbl.LocId,
                                ChequeNo = "",
                                Cr = SPrice,
                                Dr = 0,
                                Particulars = "Installment Adjustment.",
                                PCCode = tbl.LocId,
                                SubId = 0
                            });
                        if (SPrice > 0)
                            vLst.Add(new VoucherDetailVM
                            {
                                AccId = InstAcc,
                                CCCode = tbl.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = SPrice,
                                Particulars = "Installment Adjustment. AccNo " + tbl.AccNo.ToString() + " RecvNo " + tbl.InstId.ToString(),
                                PCCode = tbl.LocId,
                                SubId = tbl.AccNo,
                                SubsidaryCode = "CUST-" + tbl.AccNo.ToString(),
                                RefId = tbl.InstId
                            });
                    }
                    if (Discount > 0)
                        vLst.Add(new VoucherDetailVM
                        {
                            AccId = instDiscountAcc,
                            CCCode = tbl.LocId,
                            ChequeNo = "",
                            Cr = Discount,
                            Dr = 0,
                            Particulars = "Installment Adjustment.",
                            PCCode = tbl.Lse_Master.LocId,
                            SubId = 0
                        });
                    if (Discount > 0)
                        vLst.Add(new VoucherDetailVM
                        {
                            AccId = InstAcc,
                            CCCode = tbl.LocId,
                            ChequeNo = "",
                            Cr = 0,
                            Dr = Discount,
                            Particulars = "Installment Discount Adjustment. AccNo " + tbl.AccNo.ToString() + " RecvNo " + tbl.InstId.ToString(),
                            PCCode = tbl.Lse_Master.LocId,
                            SubId = tbl.AccNo,
                            SubsidaryCode = "CUST-" + tbl.AccNo,
                            RefId = tbl.InstId
                        });

                    var vrId = await PostAutoVoucher(vLst, "IRV", "", tbl.InstDate, UserId);
                    if (vrId > 0)
                    {
                        await db.SaveChangesAsync();
                        scop.Complete();
                        scop.Dispose();
                        return true;
                    }
                    else
                    {
                        scop.Dispose();
                        return false;
                    }
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }

        public async Task<bool> SaveSaleVoucherPosting(IEnumerable<SaleForVoucherPostingVM> mod, int TransTypeId, DateTime WorkingDate, int UserId)
        {
            try
            {
                //var accLst = await db.Comp_SystemIntegration.ToListAsync();

                if (TransTypeId == 1)
                {
                    foreach (var v in mod)
                    {
                        db = new AGEEntities();
                        List<VoucherDetailVM> CSVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> STVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> MCVLst = new List<VoucherDetailVM>();
                        var instSaleLst = await (from M in db.Inv_Sale
                                                 join D in db.Inv_SaleDetail on M.TransId equals D.TransId
                                                 join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                                                 join SP in db.Inv_Suppliers on ST.SuppId equals SP.SuppId
                                                 where M.TransId == v.TransId
                                                 select new { M.PaymentModeId, OrderAdvance = M.OrderAdvance ?? 0, M.OrderId, SP.CategoryId, ST.ItemId, ST.Exempted, ST.SerialNo, D.MRP, D.SM, D.SPrice, D.PPrice, D.Tax }).ToListAsync();
                        ///////////////////////////////////////Cash Sale Entry////////////////////////////////////////////////
                        var cashInHand = await GetAcc(428);
                        if (instSaleLst[0].OrderId > 0 && instSaleLst[0].OrderAdvance > 0)
                        {
                            var ordAdvanceAcc = await GetAcc(462);
                            var ordId = instSaleLst[0].OrderId ?? 0;
                            //decimal ordAdvance = instSaleLst[0].OrderAdvance;
                            var ordNo = await db.Inv_SaleOrder.Where(x => x.TransId == ordId).Select(x => x.BillNo).FirstOrDefaultAsync();
                            CSVLst.Add(new VoucherDetailVM
                            {
                                AccId = ordAdvanceAcc,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = instSaleLst[0].OrderAdvance,
                                Particulars = "Cash Sale." + v.BillNo + " Order No " + ordNo,
                                PCCode = v.LocId,
                                SubId = 0
                            });
                        }
                        if (instSaleLst[0].PaymentModeId == 1)
                        {
                            if (v.SPrice - (instSaleLst[0].OrderAdvance) > 0)
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.SPrice - (instSaleLst[0].OrderAdvance),
                                    Particulars = "Cash Sale." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });
                        }
                        else
                        {
                            //var cashInHand = await GetAcc(428);
                            if (v.Advance > 0)
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.Advance,
                                    Particulars = "Cash Sale." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });

                            if (v.SPrice - v.Advance - instSaleLst[0].OrderAdvance > 0)
                            {
                                var cashClr = await GetAcc(450);
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClr,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.SPrice - v.Advance - instSaleLst[0].OrderAdvance,
                                    Particulars = "Cash Sale." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClr,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = v.SPrice - v.Advance - instSaleLst[0].OrderAdvance,
                                    Dr = 0,
                                    Particulars = "Cash Sale." + v.BillNo,
                                    PCCode = 72,
                                    SubId = 0
                                });
                                var htvDebtor = await GetAcc(463);
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = htvDebtor,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.SPrice - v.Advance - instSaleLst[0].OrderAdvance,
                                    Particulars = "Cash Sale." + v.BillNo,
                                    PCCode = 72,
                                    SubId = 0
                                });
                            }

                        }

                        var cashSaleNormalDiscount = await GetAcc(430);
                        decimal disc = instSaleLst.Sum(x => ((x.MRP - x.SPrice) > 0 ? (x.MRP - x.SPrice) : 0));
                        if (disc > 0)
                            CSVLst.Add(new VoucherDetailVM
                            {
                                AccId = cashSaleNormalDiscount,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = disc,
                                Particulars = "Cash Sale." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = 0
                            });

                        //var cashSaleSpecialDiscount = await GetAcc(431);
                        //if (instSaleLst.Sum(x => x.SM - x.SPrice) > 0)
                        //{
                        //    if (instSaleLst.Sum(x => x.SM - x.MRP) > 0)
                        //        CSVLst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = cashSaleSpecialDiscount,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = 0,
                        //            Dr = instSaleLst.Sum(x => x.SM - x.MRP),
                        //            Particulars = "Cash Sale." + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0
                        //        });
                        //    if (instSaleLst.Sum(x => x.MRP - x.SM) > 0)
                        //        CSVLst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = cashSaleSpecialDiscount,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = instSaleLst.Sum(x => x.MRP - x.SM),
                        //            Dr = 0,
                        //            Particulars = "Cash Sale." + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0
                        //        });
                        //}


                        var cashSaleMarginAboveMRP = await GetAcc(436);
                        decimal mrg = instSaleLst.Sum(x => ((x.SPrice - x.MRP) > 0 ? (x.SPrice - x.MRP) : 0));
                        if (mrg > 0)
                            CSVLst.Add(new VoucherDetailVM
                            {
                                AccId = cashSaleMarginAboveMRP,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = mrg,
                                Dr = 0,
                                Particulars = "Cash Sale." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = 0
                            });
                        //var cashSaleMarginAboveBP = await GetAcc(437);
                        //if (instSaleLst.Sum(x => x.SPrice - x.SM) > 0)
                        //    CSVLst.Add(new VoucherDetailVM
                        //    {
                        //        AccId = cashSaleMarginAboveBP,
                        //        CCCode = v.LocId,
                        //        ChequeNo = "",
                        //        Cr = instSaleLst.Sum(x => x.SPrice - x.SM),
                        //        Dr = 0,
                        //        Particulars = "Cash Sale." + v.BillNo,
                        //        PCCode = v.LocId,
                        //        SubId = 0
                        //    });
                        foreach (var item in instSaleLst)
                        {
                            var cashSaleRetail = await GetAcc(8, item.CategoryId);
                            if (item.MRP > 0)
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashSaleRetail,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.MRP,
                                    Dr = 0,
                                    Particulars = "Cash Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });

                            ////////////////////////////////////////Sales Tax/////////////////////////////////

                            if (item.Tax > 0 && item.CategoryId != 4 && item.Exempted == false)
                            {
                                STVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(9, item.CategoryId),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = item.Tax ?? 0,
                                    Particulars = "Cash Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                                STVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(419),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.Tax ?? 0,
                                    Dr = 0,
                                    Particulars = "Cash Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                            }
                            //////////////////////////////////Consumption////////////////////////////////////////
                            //long consumptionAcc = accLst.Where(x => x.TransTypeId == 6 && x.CategoryId == item.CategoryId).Select(x => Convert.ToInt64(x.GLCode)).FirstOrDefault();
                            //long inventoryAcc = accLst.Where(x => x.TransTypeId == 7 && x.CategoryId == item.CategoryId).Select(x => Convert.ToInt64(x.GLCode)).FirstOrDefault();
                            if (item.PPrice > 0)
                            {
                                MCVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(14, item.CategoryId),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = item.PPrice,
                                    Particulars = "Cash Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                                MCVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(7, item.CategoryId),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.PPrice,
                                    Dr = 0,
                                    Particulars = "Cash Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                            }
                        }
                        using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                var tbl = await db.Inv_Sale.FindAsync(v.TransId);
                                if (tbl.IsPosted)
                                    continue;

                                var vrId = await PostAutoVoucher(CSVLst, "CSV", v.BillNo, WorkingDate, UserId);
                                if (vrId > 0)
                                {
                                    if (!await PostingLog("CSV", TransTypeId, tbl.TransId, vrId))
                                    {
                                        scop.Dispose();
                                        continue;
                                    }
                                }
                                else
                                {
                                    scop.Dispose();
                                    continue;
                                }
                                if (STVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(STVLst, "STV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("STV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                        continue;
                                    }
                                }
                                if (MCVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(MCVLst, "MCV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("MCV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                        continue;
                                    }
                                }
                                tbl.IsPosted = true;
                                tbl.PostedBy = UserId;
                                tbl.PostedDate = DateTime.Now;
                                await db.SaveChangesAsync();
                                scop.Complete();
                                scop.Dispose();
                            }
                            catch (Exception)
                            {
                                scop.Dispose();
                            }
                        }
                    }


                }
                else if (TransTypeId == 2)
                {
                    foreach (var v in mod)
                    {
                        db = new AGEEntities();
                        List<VoucherDetailVM> SRILst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> STVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> MCVLst = new List<VoucherDetailVM>();
                        var instSaleLst = await (from M in db.Inv_Sale
                                                 join D in db.Inv_SaleDetail on M.TransId equals D.TransId
                                                 join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                                                 join SP in db.Inv_Suppliers on ST.SuppId equals SP.SuppId
                                                 where M.TransId == v.TransId
                                                 select new { M.ItemType, M.RefSaleId, M.PaymentModeId, SP.CategoryId, ST.ItemId, ST.Exempted, ST.SerialNo, D.MRP, D.SM, D.SPrice, D.PPrice, D.Tax }).ToListAsync();

                        var oldTrans = await db.Inv_Sale.FindAsync(instSaleLst[0].RefSaleId);
                        if (instSaleLst[0].ItemType == "O")
                        {
                            ///////////////////////////////////////Cash Sale Return OLD////////////////////////////////////////////////
                            if (instSaleLst[0].PaymentModeId == 1)
                            {
                                var cashInHand = await GetAcc(428);
                                if (v.SPrice > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashInHand,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = v.SPrice,
                                        Dr = 0,
                                        Particulars = "Cash Sale Return." + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0
                                    });
                            }
                            else
                            {
                                var cashInHand = await GetAcc(428);
                                if (v.Advance > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashInHand,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = v.Advance,
                                        Dr = 0,
                                        Particulars = "Cash Sale Return." + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0
                                    });

                                if (v.SPrice - v.Advance > 0)
                                {
                                    var cashClr = await GetAcc(450);
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashClr,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = v.SPrice - v.Advance,
                                        Dr = 0,
                                        Particulars = "Cash Sale Return." + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0
                                    });
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashClr,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = v.SPrice - v.Advance,
                                        Particulars = "Cash Sale Return." + v.BillNo,
                                        PCCode = 72,
                                        SubId = 0
                                    });
                                    var htvDebtor = await GetAcc(463);
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = htvDebtor,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = v.SPrice - v.Advance,
                                        Dr = 0,
                                        Particulars = "Cash Sale Return." + v.BillNo,
                                        PCCode = 72,
                                        SubId = 0
                                    });
                                }
                            }
                            foreach (var item in instSaleLst)
                            {
                                var cashSaleRetail = await GetAcc(10, item.CategoryId);
                                if (item.SPrice > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashSaleRetail,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = item.SPrice,
                                        Particulars = "Cash Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                //////////////////////////////////Consumption////////////////////////////////////////
                                if (item.PPrice > 0)
                                {
                                    MCVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(14, item.CategoryId),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = item.PPrice,
                                        Dr = 0,
                                        Particulars = "Cash Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                    MCVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(7, item.CategoryId),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = item.PPrice,
                                        Particulars = "Cash Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                }
                            }
                        }
                        else if (instSaleLst[0].ItemType == "P")
                        {
                            ///////////////////////////////////////Cash Sale Return NEW PACKED////////////////////////////////////////////////

                            if (instSaleLst[0].PaymentModeId == 1)
                            {
                                var cashInHand = await GetAcc(428);
                                if (v.SPrice > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashInHand,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = v.SPrice,
                                        Dr = 0,
                                        Particulars = "Cash Sale Return." + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0
                                    });
                            }
                            else
                            {
                                var cashInHand = await GetAcc(428);
                                if (v.Advance > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashInHand,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = v.Advance,
                                        Dr = 0,
                                        Particulars = "Cash Sale Return." + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0
                                    });

                                if (v.SPrice - v.Advance > 0)
                                {
                                    var cashClr = await GetAcc(450);
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashClr,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = v.SPrice - v.Advance,
                                        Dr = 0,
                                        Particulars = "Cash Sale Return." + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0
                                    });
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashClr,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = v.SPrice - v.Advance,
                                        Particulars = "Cash Sale Return." + v.BillNo,
                                        PCCode = 72,
                                        SubId = 0
                                    });
                                    var htvDebtor = await GetAcc(463);
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = htvDebtor,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = v.SPrice - v.Advance,
                                        Dr = 0,
                                        Particulars = "Cash Sale Return." + v.BillNo,
                                        PCCode = 72,
                                        SubId = 0
                                    });
                                }
                            }
                            var cashSaleNormalDiscount = await GetAcc(430);
                            decimal disc = instSaleLst.Sum(x => ((x.MRP - x.SPrice) > 0 ? (x.MRP - x.SPrice) : 0));
                            if (disc > 0)
                                SRILst.Add(new VoucherDetailVM
                                {
                                    AccId = cashSaleNormalDiscount,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = disc,
                                    Dr = 0,
                                    Particulars = "Cash Sale Return." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });

                            var cashSaleMarginAboveMRP = await GetAcc(436);
                            decimal mrg = instSaleLst.Sum(x => ((x.SPrice - x.MRP) > 0 ? (x.SPrice - x.MRP) : 0));
                            if (mrg > 0)
                                SRILst.Add(new VoucherDetailVM
                                {
                                    AccId = cashSaleMarginAboveMRP,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = mrg,
                                    Particulars = "Cash Sale Return." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });

                            foreach (var item in instSaleLst)
                            {
                                var cashSaleRetail = await GetAcc(10, item.CategoryId);
                                if (item.MRP > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashSaleRetail,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = item.MRP,
                                        Particulars = "Cash Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });

                                ////////////////////////////////////////Sales Tax/////////////////////////////////

                                if (item.Tax > 0 && item.CategoryId != 4 && item.Exempted == false)
                                {
                                    STVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(9, item.CategoryId),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = item.Tax ?? 0,
                                        Dr = 0,
                                        Particulars = "Cash Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                    STVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(419),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = item.Tax ?? 0,
                                        Particulars = "Cash Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                }
                                //////////////////////////////////Consumption////////////////////////////////////////
                                if (item.PPrice > 0)
                                {
                                    MCVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(14, item.CategoryId),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = item.PPrice,
                                        Dr = 0,
                                        Particulars = "Cash Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                    MCVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(7, item.CategoryId),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = item.PPrice,
                                        Particulars = "Cash Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                }
                            }
                        }
                        using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                var tbl = await db.Inv_Sale.FindAsync(v.TransId);
                                if (tbl.IsPosted)
                                    continue;

                                var vrId = await PostAutoVoucher(SRILst, "SRI", v.BillNo, WorkingDate, UserId);
                                if (vrId > 0)
                                {
                                    if (!await PostingLog("SRI", TransTypeId, tbl.TransId, vrId))
                                    {
                                        scop.Dispose();
                                    }
                                }
                                else
                                {
                                    scop.Dispose();
                                }
                                if (STVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(STVLst, "STV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("STV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                if (MCVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(MCVLst, "MCV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("MCV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                tbl.IsPosted = true;
                                tbl.PostedBy = UserId;
                                tbl.PostedDate = DateTime.Now;
                                await db.SaveChangesAsync();
                                scop.Complete();
                                scop.Dispose();
                            }
                            catch (Exception)
                            {
                                scop.Dispose();
                            }
                        }
                    }
                }
                else if (TransTypeId == 5)
                {
                    foreach (var v in mod)
                    {
                        db = new AGEEntities();
                        List<VoucherDetailVM> CSVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> STVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> MCVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> CRVLst = new List<VoucherDetailVM>();
                        var instSaleLst = await (from M in db.Inv_Sale
                                                 join D in db.Inv_SaleDetail on M.TransId equals D.TransId
                                                 join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                                                 join SP in db.Inv_Suppliers on ST.SuppId equals SP.SuppId
                                                 where M.TransId == v.TransId
                                                 select new { M.PaymentModeId, SP.CategoryId, ST.ItemId, ST.Exempted, ST.SerialNo, D.MRP, D.SM, D.SPrice, D.PPrice, D.Tax }).ToListAsync();
                        ///////////////////////////////////////Cash Sale Entry////////////////////////////////////////////////

                        var custAcc = Convert.ToInt64((await db.Inv_Customers.FindAsync(v.CustId)).GLCode);

                        if ((v.SPrice) > 0)
                        {
                            var refSale = await GetAcc(451);
                            CSVLst.Add(new VoucherDetailVM
                            {
                                AccId = refSale,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = v.SPrice,
                                Particulars = "Credit Sale." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = custAcc
                            });
                        }

                        var cashSaleNormalDiscount = await GetAcc(430);
                        decimal disc = instSaleLst.Sum(x => ((x.MRP - x.SPrice) > 0 ? (x.MRP - x.SPrice) : 0));
                        if (disc > 0)
                            CSVLst.Add(new VoucherDetailVM
                            {
                                AccId = cashSaleNormalDiscount,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = disc,
                                Particulars = "Credit Sale." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = 0
                            });

                        //var cashSaleSpecialDiscount = await GetAcc(431);
                        //if (instSaleLst.Sum(x => x.SM - x.SPrice) > 0)
                        //{
                        //    if (instSaleLst.Sum(x => x.SM - x.MRP) > 0)
                        //        CSVLst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = cashSaleSpecialDiscount,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = 0,
                        //            Dr = instSaleLst.Sum(x => x.SM - x.MRP),
                        //            Particulars = "Cash Sale." + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0
                        //        });
                        //    if (instSaleLst.Sum(x => x.MRP - x.SM) > 0)
                        //        CSVLst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = cashSaleSpecialDiscount,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = instSaleLst.Sum(x => x.MRP - x.SM),
                        //            Dr = 0,
                        //            Particulars = "Cash Sale." + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0
                        //        });
                        //}


                        var cashSaleMarginAboveMRP = await GetAcc(436);
                        decimal mrg = instSaleLst.Sum(x => ((x.SPrice - x.MRP) > 0 ? (x.SPrice - x.MRP) : 0));
                        if (mrg > 0)
                            CSVLst.Add(new VoucherDetailVM
                            {
                                AccId = cashSaleMarginAboveMRP,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = mrg,
                                Dr = 0,
                                Particulars = "Credit Sale." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = 0
                            });
                        //var cashSaleMarginAboveBP = await GetAcc(437);
                        //if (instSaleLst.Sum(x => x.SPrice - x.SM) > 0)
                        //    CSVLst.Add(new VoucherDetailVM
                        //    {
                        //        AccId = cashSaleMarginAboveBP,
                        //        CCCode = v.LocId,
                        //        ChequeNo = "",
                        //        Cr = instSaleLst.Sum(x => x.SPrice - x.SM),
                        //        Dr = 0,
                        //        Particulars = "Cash Sale." + v.BillNo,
                        //        PCCode = v.LocId,
                        //        SubId = 0
                        //    });
                        foreach (var item in instSaleLst)
                        {
                            var cashSaleRetail = await GetAcc(8, item.CategoryId);
                            if (item.MRP > 0)
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashSaleRetail,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.MRP,
                                    Dr = 0,
                                    Particulars = "Credit Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });

                            ////////////////////////////////////////Sales Tax/////////////////////////////////

                            if (item.Tax > 0 && item.CategoryId != 4 && item.Exempted == false)
                            {
                                STVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(9, item.CategoryId),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = item.Tax ?? 0,
                                    Particulars = "Credit Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                                STVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(419),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.Tax ?? 0,
                                    Dr = 0,
                                    Particulars = "Credit Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                            }
                            //////////////////////////////////Consumption////////////////////////////////////////
                            //long consumptionAcc = accLst.Where(x => x.TransTypeId == 6 && x.CategoryId == item.CategoryId).Select(x => Convert.ToInt64(x.GLCode)).FirstOrDefault();
                            //long inventoryAcc = accLst.Where(x => x.TransTypeId == 7 && x.CategoryId == item.CategoryId).Select(x => Convert.ToInt64(x.GLCode)).FirstOrDefault();
                            if (item.PPrice > 0)
                            {
                                MCVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(14, item.CategoryId),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = item.PPrice,
                                    Particulars = "Credit Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                                MCVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(7, item.CategoryId),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.PPrice,
                                    Dr = 0,
                                    Particulars = "Credit Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                            }
                        }
                        //////////////////////////////////////Advance Receive/////////////////////////

                        if (instSaleLst[0].PaymentModeId == 1)
                        {
                            var cashInHand = await GetAcc(428);
                            if (v.Advance > 0)
                                CRVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.Advance,
                                    Particulars = "Credit Sale." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });
                        }
                        else
                        {
                            //var cashInHand = await GetAcc(428);
                            var htvDebtor = await GetAcc(463);
                            if (v.Advance > 0)
                                CRVLst.Add(new VoucherDetailVM
                                {
                                    AccId = htvDebtor,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.Advance,
                                    Particulars = "Credit Sale." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });
                        }
                        if ((v.Advance) > 0)
                        {
                            var refSale = await GetAcc(451);
                            CRVLst.Add(new VoucherDetailVM
                            {
                                AccId = refSale,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = v.Advance,
                                Dr = 0,
                                Particulars = "Credit Sale." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = custAcc
                            });
                        }

                        using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                var tbl = await db.Inv_Sale.FindAsync(v.TransId);
                                if (tbl.IsPosted)
                                    continue;

                                var vrId = await PostAutoVoucher(CSVLst, "CSV", v.BillNo, WorkingDate, UserId);
                                if (vrId > 0)
                                {
                                    if (!await PostingLog("CSV", TransTypeId, tbl.TransId, vrId))
                                    {
                                        scop.Dispose();
                                    }
                                }
                                else
                                {
                                    scop.Dispose();
                                }
                                if (STVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(STVLst, "STV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("STV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                if (MCVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(MCVLst, "MCV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("MCV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                if (CRVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(CRVLst, "CRV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("CRV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                tbl.IsPosted = true;
                                tbl.PostedBy = UserId;
                                tbl.PostedDate = DateTime.Now;
                                await db.SaveChangesAsync();
                                scop.Complete();
                                scop.Dispose();
                            }
                            catch (Exception)
                            {
                                scop.Dispose();
                            }
                        }
                    }


                }
                else if (TransTypeId == 6)
                {
                    foreach (var v in mod)
                    {
                        db = new AGEEntities();
                        List<VoucherDetailVM> SRILst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> STVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> MCVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> CPVLst = new List<VoucherDetailVM>();
                        var instSaleLst = await (from M in db.Inv_Sale
                                                 join D in db.Inv_SaleDetail on M.TransId equals D.TransId
                                                 join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                                                 join SP in db.Inv_Suppliers on ST.SuppId equals SP.SuppId
                                                 where M.TransId == v.TransId
                                                 select new { M.ItemType, M.PaymentModeId, M.RefSaleId, SP.CategoryId, ST.ItemId, ST.Exempted, ST.SerialNo, D.MRP, D.SM, D.SPrice, D.PPrice, D.Tax }).ToListAsync();

                        var oldTrans = await db.Inv_Sale.FindAsync(instSaleLst[0].RefSaleId);
                        long refSale = 0;
                        long custAcc = 0;
                        //string customer = "";
                        if (oldTrans.TransactionTypeId == 5)
                        {
                            refSale = await GetAcc(451);
                            custAcc = Convert.ToInt64((await db.Inv_Customers.FindAsync(v.CustId)).GLCode);
                            //customer = "CUST-" + v.CustId.ToString();
                        }
                        else
                        {
                            refSale = await GetAcc(452);
                            custAcc = v.CustId;
                            //customer = "EMPL-" + v.CustId.ToString();
                        }
                        if (instSaleLst[0].ItemType == "O")
                        {
                            ///////////////////////////////////////Cash Sale Return OLD////////////////////////////////////////////////
                            //var refSale = await GetAcc(451);
                            if (v.SPrice > 0)
                                SRILst.Add(new VoucherDetailVM
                                {
                                    AccId = refSale,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = v.SPrice,
                                    Dr = 0,
                                    Particulars = "Credit Sale Return." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = custAcc
                                });
                            foreach (var item in instSaleLst)
                            {
                                var cashSaleRetail = await GetAcc(10, item.CategoryId);
                                if (item.SPrice > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashSaleRetail,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = item.SPrice,
                                        Particulars = "Credit Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                //////////////////////////////////Consumption////////////////////////////////////////
                                if (item.PPrice > 0)
                                {
                                    MCVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(14, item.CategoryId),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = item.PPrice,
                                        Dr = 0,
                                        Particulars = "Credit Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                    MCVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(7, item.CategoryId),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = item.PPrice,
                                        Particulars = "Credit Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                }
                                if (v.Advance > 0)
                                {
                                    if (instSaleLst[0].PaymentModeId == 1)
                                    {
                                        var cashInHand = await GetAcc(428);
                                        CPVLst.Add(new VoucherDetailVM
                                        {
                                            AccId = cashInHand,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = v.Advance,
                                            Dr = 0,
                                            Particulars = "Credit Sale Return." + v.BillNo,
                                            PCCode = v.LocId,
                                            SubId = 0
                                        });
                                    }
                                    else
                                    {
                                        var htvDebtor = await GetAcc(463);
                                        CPVLst.Add(new VoucherDetailVM
                                        {
                                            AccId = htvDebtor,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = v.Advance,
                                            Dr = 0,
                                            Particulars = "Credit Sale Return." + v.BillNo,
                                            PCCode = v.LocId,
                                            SubId = 0
                                        });
                                    }
                                    CPVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = refSale,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = v.Advance,
                                        Particulars = "Credit Sale Return." + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = custAcc
                                    });
                                }
                            }
                        }
                        else if (instSaleLst[0].ItemType == "P")
                        {
                            ///////////////////////////////////////Credit Sale Return NEW PACKED////////////////////////////////////////////////

                            //var refSale = await GetAcc(451);
                            if (v.SPrice > 0)
                                SRILst.Add(new VoucherDetailVM
                                {
                                    AccId = refSale,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = v.SPrice,
                                    Dr = 0,
                                    Particulars = "Credit Sale Return." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = custAcc
                                });
                            var cashSaleNormalDiscount = await GetAcc(430);
                            decimal disc = instSaleLst.Sum(x => ((x.MRP - x.SPrice) > 0 ? (x.MRP - x.SPrice) : 0));
                            if (disc > 0)
                                SRILst.Add(new VoucherDetailVM
                                {
                                    AccId = cashSaleNormalDiscount,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = disc,
                                    Dr = 0,
                                    Particulars = "Credit Sale Return." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });

                            var cashSaleMarginAboveMRP = await GetAcc(436);
                            decimal mrg = instSaleLst.Sum(x => ((x.SPrice - x.MRP) > 0 ? (x.SPrice - x.MRP) : 0));
                            if (mrg > 0)
                                SRILst.Add(new VoucherDetailVM
                                {
                                    AccId = cashSaleMarginAboveMRP,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = mrg,
                                    Particulars = "Credit Sale Return." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });

                            foreach (var item in instSaleLst)
                            {
                                var cashSaleRetail = await GetAcc(10, item.CategoryId);
                                if (item.MRP > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashSaleRetail,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = item.MRP,
                                        Particulars = "Credit Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });

                                ////////////////////////////////////////Sales Tax/////////////////////////////////

                                if (item.Tax > 0 && item.CategoryId != 4 && item.Exempted == false)
                                {
                                    STVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(9, item.CategoryId),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = item.Tax ?? 0,
                                        Dr = 0,
                                        Particulars = "Credit Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                    STVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(419),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = item.Tax ?? 0,
                                        Particulars = "Credit Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                }
                                //////////////////////////////////Consumption////////////////////////////////////////
                                if (item.PPrice > 0)
                                {
                                    MCVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(14, item.CategoryId),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = item.PPrice,
                                        Dr = 0,
                                        Particulars = "Credit Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                    MCVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = await GetAcc(7, item.CategoryId),
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = item.PPrice,
                                        Particulars = "Credit Sale Return." + v.BillNo + " Sr " + item.SerialNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = item.ItemId
                                    });
                                }
                            }
                            if (v.Advance > 0)
                            {
                                if (instSaleLst[0].PaymentModeId == 1)
                                {
                                    var cashInHand = await GetAcc(428);
                                    CPVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashInHand,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = v.Advance,
                                        Dr = 0,
                                        Particulars = "Credit Sale Return." + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0
                                    });
                                }
                                else
                                {
                                    var htvDebtor = await GetAcc(463);
                                    CPVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = htvDebtor,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = v.Advance,
                                        Dr = 0,
                                        Particulars = "Credit Sale Return." + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0
                                    });
                                }
                                CPVLst.Add(new VoucherDetailVM
                                {
                                    AccId = refSale,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.Advance,
                                    Particulars = "Credit Sale Return." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = custAcc
                                });
                            }
                        }
                        using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                var tbl = await db.Inv_Sale.FindAsync(v.TransId);
                                if (tbl.IsPosted)
                                    continue;

                                var vrId = await PostAutoVoucher(SRILst, "SRI", v.BillNo, WorkingDate, UserId);
                                if (vrId > 0)
                                {
                                    if (!await PostingLog("SRI", TransTypeId, tbl.TransId, vrId))
                                    {
                                        scop.Dispose();
                                    }
                                }
                                else
                                {
                                    scop.Dispose();
                                }
                                if (STVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(STVLst, "STV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("STV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                if (MCVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(MCVLst, "MCV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("MCV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                if (CPVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(CPVLst, "CPV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("CPV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                tbl.IsPosted = true;
                                tbl.PostedBy = UserId;
                                tbl.PostedDate = DateTime.Now;
                                await db.SaveChangesAsync();
                                scop.Complete();
                                scop.Dispose();
                            }
                            catch (Exception)
                            {
                                scop.Dispose();
                            }
                        }
                    }
                }
                else if (TransTypeId == 7)
                {
                    long instDiscountAcc = await GetAcc(406);
                    long InstAcc = await GetAcc(405);
                    //long processFeeAcc = await GetAcc(415);
                    var cashInHand = await GetAcc(428);

                    foreach (var v in mod)
                    {
                        db = new AGEEntities();
                        List<VoucherDetailVM> ISVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> STVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> MCVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> CRVLst = new List<VoucherDetailVM>();
                        var instSaleLst = await (from M in db.Lse_Master
                                                 join D in db.Lse_Detail on M.AccNo equals D.AccNo
                                                 join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                                                 join SP in db.Inv_Suppliers on ST.SuppId equals SP.SuppId
                                                 where D.AccNo == v.TransId
                                                 select new { OrderId = M.OrderId ?? 0, OrderAdvance = M.OrderAdvance ?? 0, SP.CategoryId, ST.ItemId, ST.Exempted, ST.SerialNo, ST.CSerialNo, D.MRP, D.SM, D.InstPrice, SP.GST, ST.PPrice, M.Advance, M.ProcessFee, D.Tax }).ToListAsync();
                        ///////////////////////////////Insallment Sale///////////////////////////////////////
                        if (instSaleLst.Sum(x => x.InstPrice) > 0)
                            ISVLst.Add(new VoucherDetailVM
                            {
                                AccId = InstAcc,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = instSaleLst.Sum(x => x.InstPrice),
                                Particulars = "Installment Sale. " + v.BillNo,
                                PCCode = v.LocId,
                                SubId = v.CustId,
                                SubsidaryCode = "CUST-" + v.CustId,
                                RefId = v.TransId
                            });
                        decimal disc = instSaleLst.Sum(x => ((x.MRP - x.InstPrice) > 0 ? (x.MRP - x.InstPrice) : 0));
                        if (disc > 0)
                            ISVLst.Add(new VoucherDetailVM
                            {
                                AccId = instDiscountAcc,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = disc,
                                Particulars = "Installment Sale. " + v.BillNo,
                                PCCode = v.LocId,
                                SubId = 0,
                                RefId = v.TransId
                            });

                        //var instSpecialDiscount = await GetAcc(429);
                        //if (instSaleLst.Sum(x => x.SM - x.InstPrice) > 0)
                        //{
                        //    if (instSaleLst.Sum(x => x.SM - x.MRP) > 0)
                        //        ISVLst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = instSpecialDiscount,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = 0,
                        //            Dr = instSaleLst.Sum(x => x.SM - x.MRP),
                        //            Particulars = "Cash Sale." + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0,
                        //            RefId = v.TransId
                        //        });
                        //    if (instSaleLst.Sum(x => x.MRP - x.SM) > 0)
                        //        ISVLst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = instSpecialDiscount,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = instSaleLst.Sum(x => x.MRP - x.SM),
                        //            Dr = 0,
                        //            Particulars = "Cash Sale." + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0,
                        //            RefId = v.TransId
                        //        });
                        //}

                        foreach (var item in instSaleLst)
                        {
                            //decimal margin = (item.InstPrice - item.SM);
                            if (item.MRP > 0)
                            {
                                long instSaleAcc = await GetAcc(3, item.CategoryId);
                                ISVLst.Add(new VoucherDetailVM
                                {
                                    AccId = instSaleAcc,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.MRP,
                                    Dr = 0,
                                    Particulars = "Installment Sale. " + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                            }
                            //if (item.InstPrice - item.SM > 0)
                            //{
                            //    long marginAcc = await GetAcc(4,item.CategoryId);
                            //    ISVLst.Add(new VoucherDetailVM
                            //    {
                            //        AccId = marginAcc,
                            //        CCCode = v.LocId,
                            //        ChequeNo = "",
                            //        Cr = item.InstPrice - item.SM,
                            //        Dr = 0,
                            //        Particulars = "Installment Sale. " + v.BillNo + " Sr " + item.SerialNo ,
                            //        PCCode = v.LocId,
                            //        SubId = 0,
                            //        RefId = item.ItemId
                            //    });
                            //}
                        }
                        var instSaleMarginAboveMRP = await GetAcc(447);
                        decimal mrg = instSaleLst.Sum(x => ((x.InstPrice - x.MRP) > 0 ? (x.InstPrice - x.MRP) : 0));
                        if (mrg > 0)
                            ISVLst.Add(new VoucherDetailVM
                            {
                                AccId = instSaleMarginAboveMRP,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = mrg,
                                Dr = 0,
                                Particulars = "Installment Sale." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = 0,
                                RefId = v.TransId
                            });
                        ///////////////////////////////////Sales Tax///////////////////////////////////////////
                        long outputSalesTax = await GetAcc(419);
                        foreach (var item in instSaleLst)
                        {
                            long outputTax = await GetAcc(5, item.CategoryId);

                            //decimal tax = item.MRP / ((item.GST) ?? 0 + 100) * (item.GST ?? 0);
                            if (item.Tax > 0 && item.CategoryId != 4 && item.Exempted == false)
                            {
                                STVLst.Add(new VoucherDetailVM
                                {
                                    AccId = outputTax,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = item.Tax,
                                    Particulars = "Installment Sale. " + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                                STVLst.Add(new VoucherDetailVM
                                {
                                    AccId = outputSalesTax,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.Tax,
                                    Dr = 0,
                                    Particulars = "Installment Sale. " + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                            }
                        }
                        //////////////////////////////////Consumption////////////////////////////////////////

                        foreach (var item in instSaleLst)
                        {
                            long consumptionAcc = await GetAcc(6, item.CategoryId);
                            long inventoryAcc = await GetAcc(7, item.CategoryId);
                            if (item.PPrice > 0)
                            {
                                MCVLst.Add(new VoucherDetailVM
                                {
                                    AccId = consumptionAcc,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = item.PPrice,
                                    Particulars = "Installment Sale. " + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                                MCVLst.Add(new VoucherDetailVM
                                {
                                    AccId = inventoryAcc,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.PPrice,
                                    Dr = 0,
                                    Particulars = "Installment Sale. " + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                            }
                        }
                        ///////////////////////////////Advance Received/////////////////////////////////////////////
                        if (instSaleLst[0].OrderId > 0 && instSaleLst[0].OrderAdvance > 0)
                        {
                            var ordAdvanceAcc = await GetAcc(462);
                            var ordId = instSaleLst[0].OrderId;
                            //decimal ordAdvance = instSaleLst[0].OrderAdvance;
                            var ordNo = await db.Inv_SaleOrder.Where(x => x.TransId == ordId).Select(x => x.BillNo).FirstOrDefaultAsync();
                            CRVLst.Add(new VoucherDetailVM
                            {
                                AccId = ordAdvanceAcc,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = instSaleLst[0].OrderAdvance,
                                Particulars = "Installment Sale." + v.BillNo + " Order No " + ordNo,
                                PCCode = v.LocId,
                                SubId = 0
                            });
                        }
                        if (instSaleLst[0].Advance - instSaleLst[0].OrderAdvance > 0)
                            CRVLst.Add(new VoucherDetailVM
                            {
                                AccId = cashInHand,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = instSaleLst[0].Advance - instSaleLst[0].OrderAdvance,
                                Particulars = "Installment Sale. Advance Received. BillNo" + v.BillNo,
                                PCCode = v.LocId,
                                SubId = 0,
                                RefId = v.TransId
                            });
                        //if (instSaleLst[0].ProcessFee > 0)
                        //    CRVLst.Add(new VoucherDetailVM
                        //    {
                        //        AccId = processFeeAcc,
                        //        CCCode = v.LocId,
                        //        ChequeNo = "",
                        //        Cr = 0,
                        //        Dr = instSaleLst[0].ProcessFee,
                        //        Particulars = "Auto voucher posted. " + v.BillNo,
                        //        PCCode = v.LocId,
                        //        SubId = 0,
                        //        RefId = v.TransId
                        //    });
                        if (instSaleLst[0].Advance /*+ instSaleLst[0].ProcessFee*/ > 0)
                            CRVLst.Add(new VoucherDetailVM
                            {
                                AccId = InstAcc,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = instSaleLst[0].Advance /*+ instSaleLst[0].ProcessFee*/,
                                Dr = 0,
                                Particulars = "Installment Sale. Advance Received. BillNo" + v.BillNo,
                                PCCode = v.LocId,
                                SubId = v.CustId,
                                SubsidaryCode = "CUST-" + v.CustId,
                                RefId = v.TransId
                            });

                        using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                var tbl = await db.Lse_Master.FindAsync(v.TransId);
                                if (tbl.IsPosted)
                                    continue;

                                var vrId = await PostAutoVoucher(ISVLst, "ISV", v.BillNo, WorkingDate, UserId);
                                if (vrId > 0)
                                {
                                    if (!await PostingLog("ISV", TransTypeId, tbl.AccNo, vrId))
                                    {
                                        scop.Dispose();
                                    }
                                }
                                else
                                {
                                    scop.Dispose();
                                }
                                if (STVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(STVLst, "STV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("STV", TransTypeId, tbl.AccNo, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                if (MCVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(MCVLst, "MCV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("MCV", TransTypeId, tbl.AccNo, vrId))
                                        {
                                            scop.Dispose();
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                if (CRVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(CRVLst, "CRV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("CRV", TransTypeId, tbl.AccNo, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                tbl.IsPosted = true;
                                tbl.PostedBy = UserId;
                                tbl.PostedDate = DateTime.Now;
                                await db.SaveChangesAsync();
                                scop.Complete();
                                scop.Dispose();
                            }
                            catch (Exception)
                            {
                                scop.Dispose();
                            }
                        }

                    }
                }
                else if (TransTypeId == 8)
                {
                    long instDiscountAcc = await GetAcc(406);
                    long InstAcc = await GetAcc(405);
                    //long processFeeAcc = await GetAcc(415);
                    var cashInHand = await GetAcc(428);
                    //var cashInHand = await GetAcc(428);

                    foreach (var v in mod)
                    {
                        db = new AGEEntities();
                        var saleReturn = await db.Lse_Return.FindAsync(v.TransId);
                        //if (saleReturn.ReturnTypeId == 1)
                        //{
                        //    List<VoucherDetailVM> SRILst = new List<VoucherDetailVM>();
                        //    List<VoucherDetailVM> STVRLst = new List<VoucherDetailVM>();
                        //    List<VoucherDetailVM> MCVRLst = new List<VoucherDetailVM>();
                        //    //List<VoucherDetailVM> CRVLst = new List<VoucherDetailVM>();

                        //    var instSaleLst = await (from M in db.Lse_Master
                        //                             join D in db.Lse_Detail on M.AccNo equals D.AccNo
                        //                             join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                        //                             join SP in db.Inv_Suppliers on ST.SuppId equals SP.SuppId
                        //                             where M.AccNo == saleReturn.AccNo && D.IsReturned == true && !ST.Itm_Master.IsPair
                        //                             select new { SP.CategoryId, ST.ItemId, ST.SerialNo, ST.CSerialNo, D.MRP, D.SM, D.InstPrice, SP.GST, ST.PPrice, M.Advance, M.ProcessFee, D.Tax }).ToListAsync();

                        //    ///////////////////////////////Insallment Sale/////////////////////////////////////////////
                        //    if (instSaleLst.Sum(x => x.InstPrice) > 0)
                        //        SRILst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = InstAcc,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = instSaleLst.Sum(x => x.InstPrice),
                        //            Dr = 0,
                        //            Particulars = "Installment Sale Return. " + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = v.CustId,
                        //            SubsidaryCode = "CUST-" + v.CustId,
                        //            RefId = v.TransId
                        //        });
                        //    if (instSaleLst.Sum(x => x.MRP - x.InstPrice) > 0)
                        //        SRILst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = instDiscountAcc,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = instSaleLst.Sum(x => x.MRP - x.InstPrice),
                        //            Dr = 0,
                        //            Particulars = "Installment Sale Return. " + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0,
                        //            RefId = v.TransId
                        //        });

                        //    foreach (var item in instSaleLst)
                        //    {
                        //        //decimal margin = (item.InstPrice - item.SM);
                        //        if (item.MRP > 0)
                        //        {
                        //            long instSaleAcc = await GetAcc(11, item.CategoryId);
                        //            SRILst.Add(new VoucherDetailVM
                        //            {
                        //                AccId = instSaleAcc,
                        //                CCCode = v.LocId,
                        //                ChequeNo = "",
                        //                Cr = 0,
                        //                Dr = item.MRP,
                        //                Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                        //                PCCode = v.LocId,
                        //                SubId = 0,
                        //                RefId = item.ItemId
                        //            });
                        //        }

                        //    }
                        //    var instSaleMarginAboveMRP = await GetAcc(447);
                        //    if (instSaleLst.Sum(x => x.InstPrice - x.MRP) > 0)
                        //        SRILst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = instSaleMarginAboveMRP,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = 0,
                        //            Dr = instSaleLst.Sum(x => x.InstPrice - x.MRP),
                        //            Particulars = "Installment Sale Return." + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0,
                        //            RefId = v.TransId
                        //        });
                        //    ///////////////////////////////////Sales Tax///////////////////////////////////////////
                        //    long outputSalesTax = await GetAcc(419);
                        //    foreach (var item in instSaleLst)
                        //    {
                        //        long outputTax = await GetAcc(5, item.CategoryId);
                        //        if (item.Tax > 0)
                        //        {
                        //            STVRLst.Add(new VoucherDetailVM
                        //            {
                        //                AccId = outputTax,
                        //                CCCode = v.LocId,
                        //                ChequeNo = "",
                        //                Cr = item.Tax,
                        //                Dr = 0,
                        //                Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                        //                PCCode = v.LocId,
                        //                SubId = 0,
                        //                RefId = item.ItemId
                        //            });
                        //            STVRLst.Add(new VoucherDetailVM
                        //            {
                        //                AccId = outputSalesTax,
                        //                CCCode = v.LocId,
                        //                ChequeNo = "",
                        //                Cr = 0,
                        //                Dr = item.Tax,
                        //                Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                        //                PCCode = v.LocId,
                        //                SubId = 0,
                        //                RefId = item.ItemId
                        //            });
                        //        }
                        //    }
                        //    //////////////////////////////////Consumption////////////////////////////////////////

                        //    foreach (var item in instSaleLst)
                        //    {
                        //        long consumptionAcc = await GetAcc(6, item.CategoryId);
                        //        long inventoryAcc = await GetAcc(7, item.CategoryId);
                        //        if (item.PPrice > 0)
                        //        {
                        //            MCVRLst.Add(new VoucherDetailVM
                        //            {
                        //                AccId = consumptionAcc,
                        //                CCCode = v.LocId,
                        //                ChequeNo = "",
                        //                Cr = item.PPrice,
                        //                Dr = 0,
                        //                Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                        //                PCCode = v.LocId,
                        //                SubId = 0,
                        //                RefId = item.ItemId
                        //            });
                        //            MCVRLst.Add(new VoucherDetailVM
                        //            {
                        //                AccId = inventoryAcc,
                        //                CCCode = v.LocId,
                        //                ChequeNo = "",
                        //                Cr = 0,
                        //                Dr = item.PPrice,
                        //                Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                        //                PCCode = v.LocId,
                        //                SubId = 0,
                        //                RefId = item.ItemId
                        //            });
                        //        }
                        //    }

                        //    List<VoucherDetailVM> ISVLst = new List<VoucherDetailVM>();
                        //    List<VoucherDetailVM> STVLst = new List<VoucherDetailVM>();
                        //    List<VoucherDetailVM> MCVLst = new List<VoucherDetailVM>();
                        //    //CRVLst = new List<VoucherDetailVM>();

                        //    instSaleLst = await (from M in db.Lse_Master
                        //                         join D in db.Lse_Detail on M.AccNo equals D.AccNo
                        //                         join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                        //                         join SP in db.Inv_Suppliers on ST.SuppId equals SP.SuppId
                        //                         where D.AccNo == saleReturn.AccNo && D.IsReturned == false && !ST.Itm_Master.IsPair
                        //                         select new { SP.CategoryId, ST.ItemId, ST.SerialNo, ST.CSerialNo, D.MRP, D.SM, D.InstPrice, SP.GST, ST.PPrice, M.Advance, M.ProcessFee, D.Tax }).ToListAsync();
                        //    ///////////////////////////////Insallment Sale/////////////////////////////////////////////
                        //    if (instSaleLst.Sum(x => x.InstPrice) > 0)
                        //        ISVLst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = InstAcc,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = 0,
                        //            Dr = instSaleLst.Sum(x => x.InstPrice),
                        //            Particulars = "Installment Sale. " + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = v.CustId,
                        //            SubsidaryCode = "CUST-" + v.CustId,
                        //            RefId = v.TransId
                        //        });
                        //    if (instSaleLst.Sum(x => x.MRP - x.InstPrice) > 0)
                        //        ISVLst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = instDiscountAcc,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = 0,
                        //            Dr = instSaleLst.Sum(x => x.MRP - x.InstPrice),
                        //            Particulars = "Installment Sale. " + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0,
                        //            RefId = v.TransId
                        //        });

                        //    foreach (var item in instSaleLst)
                        //    {
                        //        //decimal margin = (item.InstPrice - item.SM);
                        //        if (item.MRP > 0)
                        //        {
                        //            long instSaleAcc = await GetAcc(3, item.CategoryId);
                        //            ISVLst.Add(new VoucherDetailVM
                        //            {
                        //                AccId = instSaleAcc,
                        //                CCCode = v.LocId,
                        //                ChequeNo = "",
                        //                Cr = item.MRP,
                        //                Dr = 0,
                        //                Particulars = "Installment Sale. " + v.BillNo + " Sr " + item.SerialNo,
                        //                PCCode = v.LocId,
                        //                SubId = 0,
                        //                RefId = item.ItemId
                        //            });
                        //        }

                        //    }
                        //    instSaleMarginAboveMRP = await GetAcc(447);
                        //    if (instSaleLst.Sum(x => x.InstPrice - x.MRP) > 0)
                        //        ISVLst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = instSaleMarginAboveMRP,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = instSaleLst.Sum(x => x.InstPrice - x.MRP),
                        //            Dr = 0,
                        //            Particulars = "Installment Sale." + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0,
                        //            RefId = v.TransId
                        //        });
                        //    ///////////////////////////////////Sales Tax///////////////////////////////////////////
                        //    outputSalesTax = await GetAcc(419);
                        //    foreach (var item in instSaleLst)
                        //    {
                        //        long outputTax = await GetAcc(5, item.CategoryId);
                        //        if (item.Tax > 0)
                        //        {
                        //            STVLst.Add(new VoucherDetailVM
                        //            {
                        //                AccId = outputTax,
                        //                CCCode = v.LocId,
                        //                ChequeNo = "",
                        //                Cr = 0,
                        //                Dr = item.Tax,
                        //                Particulars = "Installment Sale. " + v.BillNo + " Sr " + item.SerialNo,
                        //                PCCode = v.LocId,
                        //                SubId = 0,
                        //                RefId = item.ItemId
                        //            });
                        //            STVLst.Add(new VoucherDetailVM
                        //            {
                        //                AccId = outputSalesTax,
                        //                CCCode = v.LocId,
                        //                ChequeNo = "",
                        //                Cr = item.Tax,
                        //                Dr = 0,
                        //                Particulars = "Installment Sale. " + v.BillNo + " Sr " + item.SerialNo,
                        //                PCCode = v.LocId,
                        //                SubId = 0,
                        //                RefId = item.ItemId
                        //            });
                        //        }
                        //    }
                        //    //////////////////////////////////Consumption////////////////////////////////////////

                        //    foreach (var item in instSaleLst)
                        //    {
                        //        long consumptionAcc = await GetAcc(6, item.CategoryId);
                        //        long inventoryAcc = await GetAcc(7, item.CategoryId);
                        //        if (item.PPrice > 0)
                        //        {
                        //            MCVLst.Add(new VoucherDetailVM
                        //            {
                        //                AccId = consumptionAcc,
                        //                CCCode = v.LocId,
                        //                ChequeNo = "",
                        //                Cr = 0,
                        //                Dr = item.PPrice,
                        //                Particulars = "Installment Sale. " + v.BillNo + " Sr " + item.SerialNo,
                        //                PCCode = v.LocId,
                        //                SubId = 0,
                        //                RefId = item.ItemId
                        //            });
                        //            MCVLst.Add(new VoucherDetailVM
                        //            {
                        //                AccId = inventoryAcc,
                        //                CCCode = v.LocId,
                        //                ChequeNo = "",
                        //                Cr = item.PPrice,
                        //                Dr = 0,
                        //                Particulars = "Installment Sale. " + v.BillNo + " Sr " + item.SerialNo,
                        //                PCCode = v.LocId,
                        //                SubId = 0,
                        //                RefId = item.ItemId
                        //            });
                        //        }
                        //    }
                        //    using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        //    {
                        //        try
                        //        {
                        //            var tbl = await db.Lse_Return.FindAsync(v.TransId);
                        //            if (tbl.IsPosted)
                        //                continue;

                        //            var vrId = await PostAutoVoucher(SRILst, "SRI", v.BillNo, WorkingDate, UserId);
                        //            if (vrId == 0)
                        //            {
                        //                scop.Dispose();
                        //            }
                        //            if (STVRLst.Count > 0)
                        //            {
                        //                vrId = await PostAutoVoucher(STVRLst, "STV", v.BillNo, WorkingDate, UserId);
                        //                if (vrId == 0)
                        //                {
                        //                    scop.Dispose();
                        //                }
                        //            }
                        //            if (MCVRLst.Count > 0)
                        //            {
                        //                vrId = await PostAutoVoucher(MCVRLst, "MCV", v.BillNo, WorkingDate, UserId);
                        //                if (vrId == 0)
                        //                {
                        //                    scop.Dispose();
                        //                }
                        //            }
                        //            vrId = await PostAutoVoucher(ISVLst, "ISV", v.BillNo, WorkingDate, UserId);
                        //            if (vrId == 0)
                        //            {
                        //                scop.Dispose();
                        //            }
                        //            if (STVLst.Count > 0)
                        //            {
                        //                vrId = await PostAutoVoucher(STVLst, "STV", v.BillNo, WorkingDate, UserId);
                        //                if (vrId == 0)
                        //                {
                        //                    scop.Dispose();
                        //                }
                        //            }
                        //            if (MCVLst.Count > 0)
                        //            {
                        //                vrId = await PostAutoVoucher(MCVLst, "MCV", v.BillNo, WorkingDate, UserId);
                        //                if (vrId == 0)
                        //                {
                        //                    scop.Dispose();
                        //                }
                        //            }
                        //            tbl.IsPosted = true;
                        //            tbl.PostedBy = UserId;
                        //            tbl.PostedDate = DateTime.Now;
                        //            await db.SaveChangesAsync();
                        //            scop.Complete();
                        //            scop.Dispose();
                        //        }
                        //        catch (Exception)
                        //        {
                        //            scop.Dispose();
                        //        }
                        //    }
                        //}
                        if (saleReturn.ReturnTypeId == 2 || saleReturn.ReturnTypeId == 3)
                        {
                            if (saleReturn.ItemType == "P")
                            {
                                List<VoucherDetailVM> SRILst = new List<VoucherDetailVM>();
                                List<VoucherDetailVM> STVRLst = new List<VoucherDetailVM>();
                                List<VoucherDetailVM> MCVRLst = new List<VoucherDetailVM>();
                                List<VoucherDetailVM> CPVLst = new List<VoucherDetailVM>();

                                var instSaleLst = await (from R in db.Lse_Return
                                                         join M in db.Lse_Master on R.AccNo equals M.AccNo
                                                         join D in db.Lse_Detail on M.AccNo equals D.AccNo
                                                         join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                                                         join SP in db.Inv_Suppliers on ST.SuppId equals SP.SuppId
                                                         where M.AccNo == saleReturn.AccNo && M.Type == "R"
                                                         select new { SP.CategoryId, ST.ItemId, ST.Exempted, ST.SerialNo, ST.CSerialNo, D.MRP, D.SM, D.InstPrice, SP.GST, ST.PPrice, M.Advance, M.ProcessFee, D.Tax }).ToListAsync();
                                ///////////////////////////////Insallment Sale/////////////////////////////////////////////
                                if (instSaleLst.Sum(x => x.InstPrice) > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = InstAcc,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = instSaleLst.Sum(x => x.InstPrice),
                                        Dr = 0,
                                        Particulars = "Installment Sale Return. " + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = v.CustId,
                                        SubsidaryCode = "CUST-" + v.CustId,
                                        RefId = v.TransId
                                    });
                                decimal disc = instSaleLst.Sum(x => ((x.MRP - x.InstPrice) > 0 ? (x.MRP - x.InstPrice) : 0));
                                if (disc > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = instDiscountAcc,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = disc,
                                        Dr = 0,
                                        Particulars = "Installment Sale Return. " + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = v.TransId
                                    });

                                foreach (var item in instSaleLst)
                                {
                                    //decimal margin = (item.InstPrice - item.SM);
                                    if (item.MRP > 0)
                                    {
                                        long instSaleAcc = await GetAcc(11, item.CategoryId);
                                        SRILst.Add(new VoucherDetailVM
                                        {
                                            AccId = instSaleAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = item.MRP,
                                            Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                                            PCCode = v.LocId,
                                            SubId = 0,
                                            RefId = item.ItemId
                                        });
                                    }

                                }
                                var instSaleMarginAboveMRP = await GetAcc(447);
                                decimal mrg = instSaleLst.Sum(x => ((x.InstPrice - x.MRP) > 0 ? (x.InstPrice - x.MRP) : 0));
                                if (mrg > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = instSaleMarginAboveMRP,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = mrg,
                                        Particulars = "Installment Sale Return." + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = v.TransId
                                    });
                                ///////////////////////////////////Sales Tax///////////////////////////////////////////
                                long outputSalesTax = await GetAcc(419);
                                foreach (var item in instSaleLst)
                                {
                                    long outputTax = await GetAcc(5, item.CategoryId);
                                    if (item.Tax > 0 && item.CategoryId != 4 && item.Exempted == false)
                                    {
                                        STVRLst.Add(new VoucherDetailVM
                                        {
                                            AccId = outputTax,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = item.Tax,
                                            Dr = 0,
                                            Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                                            PCCode = v.LocId,
                                            SubId = 0,
                                            RefId = item.ItemId
                                        });
                                        STVRLst.Add(new VoucherDetailVM
                                        {
                                            AccId = outputSalesTax,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = item.Tax,
                                            Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                                            PCCode = v.LocId,
                                            SubId = 0,
                                            RefId = item.ItemId
                                        });
                                    }
                                }
                                //////////////////////////////////Consumption////////////////////////////////////////

                                foreach (var item in instSaleLst)
                                {
                                    long consumptionAcc = await GetAcc(6, item.CategoryId);
                                    long inventoryAcc = await GetAcc(7, item.CategoryId);
                                    if (item.PPrice > 0)
                                    {
                                        MCVRLst.Add(new VoucherDetailVM
                                        {
                                            AccId = consumptionAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = item.PPrice,
                                            Dr = 0,
                                            Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                                            PCCode = v.LocId,
                                            SubId = 0,
                                            RefId = item.ItemId
                                        });
                                        MCVRLst.Add(new VoucherDetailVM
                                        {
                                            AccId = inventoryAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = item.PPrice,
                                            Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                                            PCCode = v.LocId,
                                            SubId = 0,
                                            RefId = item.ItemId
                                        });
                                    }
                                }
                                ///////////////////////////////Advance Received/////////////////////////////////////////////
                                ///
                                var outStandAmt = await new SaleBL().GetOutStandAmount(v.CustId);
                                var retAmt = instSaleLst.Sum(x => x.InstPrice) - outStandAmt;

                                if ((saleReturn.ReturnAmount ?? 0) > 0)
                                    CPVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashInHand,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = (saleReturn.ReturnAmount ?? 0),
                                        Dr = 0,
                                        Particulars = "Installment Sale Return. BillNo" + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = v.TransId
                                    });
                                if (retAmt - (saleReturn.ReturnAmount ?? 0) > 0)
                                {
                                    long otherIncomeAcc = await GetAcc(470);
                                    CPVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = otherIncomeAcc,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = retAmt - (saleReturn.ReturnAmount ?? 0),
                                        Dr = 0,
                                        Particulars = "Installment Sale Return. BillNo" + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = v.TransId
                                    });
                                }
                                //if (instSaleLst[0].ProcessFee > 0)
                                //    CPVLst.Add(new VoucherDetailVM
                                //    {
                                //        AccId = processFeeAcc,
                                //        CCCode = v.LocId,
                                //        ChequeNo = "",
                                //        Cr = instSaleLst[0].ProcessFee,
                                //        Dr = 0,
                                //        Particulars = "Installment Sale Return. ProcessFee Received. BillNo" + v.BillNo,
                                //        PCCode = v.LocId,
                                //        SubId = 0,
                                //        RefId = v.TransId
                                //    });
                                if (retAmt > 0)
                                    CPVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = InstAcc,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = retAmt,
                                        Particulars = "Installment Sale Return. BillNo" + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = v.CustId,
                                        SubsidaryCode = "CUST-" + v.CustId,
                                        RefId = v.TransId
                                    });

                                using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                                {
                                    try
                                    {
                                        var tbl = await db.Lse_Return.FindAsync(v.TransId);
                                        if (tbl.IsPosted)
                                            continue;

                                        var vrId = await PostAutoVoucher(SRILst, "SRI", v.BillNo, WorkingDate, UserId);
                                        if (vrId > 0)
                                        {
                                            if (!await PostingLog("SRI", TransTypeId, tbl.TransId, vrId))
                                            {
                                                scop.Dispose();
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            scop.Dispose();
                                            break;
                                        }
                                        if (STVRLst.Count > 0)
                                        {
                                            vrId = await PostAutoVoucher(STVRLst, "STV", v.BillNo, WorkingDate, UserId);
                                            if (vrId > 0)
                                            {
                                                if (!await PostingLog("STV", TransTypeId, tbl.TransId, vrId))
                                                {
                                                    scop.Dispose();
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                scop.Dispose();
                                                break;
                                            }
                                        }
                                        if (MCVRLst.Count > 0)
                                        {
                                            vrId = await PostAutoVoucher(MCVRLst, "MCV", v.BillNo, WorkingDate, UserId);
                                            if (vrId > 0)
                                            {
                                                if (!await PostingLog("MCV", TransTypeId, tbl.TransId, vrId))
                                                {
                                                    scop.Dispose();
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                scop.Dispose();
                                                break;
                                            }
                                        }
                                        if (CPVLst.Count > 0)
                                        {
                                            vrId = await PostAutoVoucher(CPVLst, "CPV", v.BillNo, WorkingDate, UserId);
                                            if (vrId > 0)
                                            {
                                                if (!await PostingLog("CPV", TransTypeId, tbl.TransId, vrId))
                                                {
                                                    scop.Dispose();
                                                }
                                            }
                                            else
                                            {
                                                scop.Dispose();
                                            }
                                        }
                                        //vrId = await PostAutoVoucher(ISVLst, "ISV", v.BillNo, WorkingDate, UserId);
                                        //if (vrId == 0)
                                        //{
                                        //    scop.Dispose();
                                        //}
                                        //if (STVLst.Count > 0)
                                        //{
                                        //    vrId = await PostAutoVoucher(STVLst, "STV", v.BillNo, WorkingDate, UserId);
                                        //    if (vrId == 0)
                                        //    {
                                        //        scop.Dispose();
                                        //    }
                                        //}
                                        //if (MCVLst.Count > 0)
                                        //{
                                        //    vrId = await PostAutoVoucher(MCVLst, "MCV", v.BillNo, WorkingDate, UserId);
                                        //    if (vrId == 0)
                                        //    {
                                        //        scop.Dispose();
                                        //    }
                                        //}
                                        tbl.IsPosted = true;
                                        tbl.PostedBy = UserId;
                                        tbl.PostedDate = DateTime.Now;
                                        await db.SaveChangesAsync();
                                        scop.Complete();
                                        scop.Dispose();
                                    }
                                    catch (Exception)
                                    {
                                        scop.Dispose();
                                    }
                                }
                            }
                            else if (saleReturn.ItemType == "O")
                            {
                                List<VoucherDetailVM> SRILst = new List<VoucherDetailVM>();
                                List<VoucherDetailVM> CPVLst = new List<VoucherDetailVM>();
                                List<VoucherDetailVM> MCVRLst = new List<VoucherDetailVM>();

                                var instSaleLst = await (from R in db.Lse_Return
                                                         join M in db.Lse_Master on R.AccNo equals M.AccNo
                                                         join D in db.Lse_Detail on M.AccNo equals D.AccNo
                                                         join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                                                         join SP in db.Inv_Suppliers on ST.SuppId equals SP.SuppId
                                                         where M.AccNo == saleReturn.AccNo && M.Type == "R"
                                                         select new { SP.CategoryId, ST.ItemId, ST.Exempted, ST.SerialNo, ST.CSerialNo, D.MRP, D.SM, D.InstPrice, SP.GST, ST.PPrice, M.Advance, M.ProcessFee, D.Tax }).ToListAsync();
                                ///////////////////////////////Insallment Sale/////////////////////////////////////////////
                                if (saleReturn.Amount > 0)
                                    SRILst.Add(new VoucherDetailVM
                                    {
                                        AccId = InstAcc,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = saleReturn.Amount,
                                        Dr = 0,
                                        Particulars = "Installment Sale Return. " + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = v.CustId,
                                        SubsidaryCode = "CUST-" + v.CustId,
                                        RefId = v.TransId
                                    });
                                foreach (var item in instSaleLst)
                                {
                                    //decimal margin = (item.InstPrice - item.SM);
                                    if (item.MRP > 0)
                                    {
                                        long instSaleAcc = await GetAcc(11, item.CategoryId);
                                        SRILst.Add(new VoucherDetailVM
                                        {
                                            AccId = instSaleAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = saleReturn.Amount,
                                            Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                                            PCCode = v.LocId,
                                            SubId = 0,
                                            RefId = item.ItemId
                                        });
                                    }
                                }
                                //////////////////////////////////Consumption////////////////////////////////////////
                                foreach (var item in instSaleLst)
                                {
                                    long consumptionAcc = await GetAcc(6, item.CategoryId);
                                    long inventoryAcc = await GetAcc(7, item.CategoryId);
                                    if (item.PPrice > 0)
                                    {
                                        MCVRLst.Add(new VoucherDetailVM
                                        {
                                            AccId = consumptionAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = saleReturn.Amount,
                                            Dr = 0,
                                            Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                                            PCCode = v.LocId,
                                            SubId = 0,
                                            RefId = item.ItemId
                                        });
                                        MCVRLst.Add(new VoucherDetailVM
                                        {
                                            AccId = inventoryAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = saleReturn.Amount,
                                            Particulars = "Installment Sale Return. " + v.BillNo + " Sr " + item.SerialNo,
                                            PCCode = v.LocId,
                                            SubId = 0,
                                            RefId = item.ItemId
                                        });
                                    }
                                }

                                if ((saleReturn.ReturnAmount ?? 0) > 0)
                                    CPVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashInHand,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = (saleReturn.ReturnAmount ?? 0),
                                        Dr = 0,
                                        Particulars = "Installment Sale Return. BillNo" + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = 0,
                                        RefId = v.TransId
                                    });

                                if ((saleReturn.ReturnAmount ?? 0) > 0)
                                    CPVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = InstAcc,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = (saleReturn.ReturnAmount ?? 0),
                                        Particulars = "Installment Sale Return. BillNo" + v.BillNo,
                                        PCCode = v.LocId,
                                        SubId = v.CustId,
                                        SubsidaryCode = "CUST-" + v.CustId,
                                        RefId = v.TransId
                                    });

                                using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                                {
                                    try
                                    {
                                        var tbl = await db.Lse_Return.FindAsync(v.TransId);
                                        if (tbl.IsPosted)
                                            continue;

                                        var vrId = await PostAutoVoucher(SRILst, "SRI", v.BillNo, WorkingDate, UserId);
                                        if (vrId > 0)
                                        {
                                            if (!await PostingLog("SRI", TransTypeId, tbl.TransId, vrId))
                                            {
                                                scop.Dispose();
                                            }
                                        }
                                        else
                                        {
                                            scop.Dispose();
                                        }
                                        if (MCVRLst.Count > 0)
                                        {
                                            vrId = await PostAutoVoucher(MCVRLst, "MCV", v.BillNo, WorkingDate, UserId);
                                            if (vrId > 0)
                                            {
                                                if (!await PostingLog("MCV", TransTypeId, tbl.TransId, vrId))
                                                {
                                                    scop.Dispose();
                                                }
                                            }
                                            else
                                            {
                                                scop.Dispose();
                                            }
                                        }
                                        if (CPVLst.Count > 0)
                                        {
                                            vrId = await PostAutoVoucher(CPVLst, "CPV", v.BillNo, WorkingDate, UserId);
                                            if (vrId > 0)
                                            {
                                                if (!await PostingLog("CPV", TransTypeId, tbl.TransId, vrId))
                                                {
                                                    scop.Dispose();
                                                }
                                            }
                                            else
                                            {
                                                scop.Dispose();
                                            }
                                        }

                                        tbl.IsPosted = true;
                                        tbl.PostedBy = UserId;
                                        tbl.PostedDate = DateTime.Now;
                                        await db.SaveChangesAsync();
                                        scop.Complete();
                                        scop.Dispose();
                                    }
                                    catch (Exception)
                                    {
                                        scop.Dispose();
                                    }
                                }
                            }
                        }
                    }
                }
                else if (TransTypeId == 9)
                {
                    db = new AGEEntities();
                    using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        try
                        {
                            long instDiscountAcc = await GetAcc(446);
                            long InstAcc = await GetAcc(405);
                            var cashInHand = await GetAcc(428);
                            var clrAcc = await GetAcc(450);

                            List<VoucherDetailVM> vLst = new List<VoucherDetailVM>();
                            foreach (var v in mod)
                            {
                                var tbl = await db.Lse_Installment.FindAsync(v.TransId);
                                if (tbl.IsPosted)
                                    continue;


                                tbl.IsPosted = true;
                                tbl.PostedBy = UserId;
                                tbl.PostedDate = DateTime.Now;

                                if (tbl.LocId != tbl.Lse_Master.LocId)
                                {
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = cashInHand,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = v.SPrice,
                                            Particulars = "Installment Received.",
                                            PCCode = tbl.LocId,
                                            SubId = 0
                                        });
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = clrAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = v.SPrice,
                                            Dr = 0,
                                            Particulars = "Installment Received.",
                                            PCCode = tbl.LocId,
                                            SubId = 0
                                        });
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = clrAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = v.SPrice,
                                            Particulars = "Installment Received.",
                                            PCCode = tbl.Lse_Master.LocId,
                                            SubId = 0
                                        });
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = InstAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = v.SPrice,
                                            Dr = 0,
                                            Particulars = "Installment Received. AccNo " + v.CustId.ToString() + " RecvNo " + v.TransId.ToString(),
                                            PCCode = tbl.Lse_Master.LocId,
                                            SubId = v.CustId,
                                            SubsidaryCode = "CUST-" + v.CustId,
                                            RefId = v.TransId
                                        });
                                }
                                else
                                {
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = cashInHand,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = v.SPrice,
                                            Particulars = "Installment Received.",
                                            PCCode = v.LocId,
                                            SubId = 0
                                        });
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = InstAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = v.SPrice,
                                            Dr = 0,
                                            Particulars = "Installment Received. AccNo " + v.CustId.ToString() + " RecvNo " + v.TransId.ToString(),
                                            PCCode = v.LocId,
                                            SubId = v.CustId,
                                            SubsidaryCode = "CUST-" + v.CustId,
                                            RefId = v.TransId
                                        });
                                }
                                if (v.Discount > 0)
                                    vLst.Add(new VoucherDetailVM
                                    {
                                        AccId = instDiscountAcc,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = v.Discount,
                                        Particulars = "Installment Received.",
                                        PCCode = tbl.Lse_Master.LocId,
                                        SubId = 0
                                    });
                                if (v.Discount > 0)
                                    vLst.Add(new VoucherDetailVM
                                    {
                                        AccId = InstAcc,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = v.Discount,
                                        Dr = 0,
                                        Particulars = "Installment Discount Allowed. AccNo " + v.CustId.ToString() + " RecvNo " + v.TransId.ToString(),
                                        PCCode = tbl.Lse_Master.LocId,
                                        SubId = v.CustId,
                                        SubsidaryCode = "CUST-" + v.CustId,
                                        RefId = v.TransId
                                    });


                            }
                            vLst = vLst.GroupBy(x => new { x.AccId, x.CCCode, x.ChequeNo, x.Particulars, x.PCCode, x.SubId, x.RefId }).Select(x =>
                                   new VoucherDetailVM
                                   {
                                       AccId = x.Key.AccId,
                                       CCCode = x.Key.CCCode,
                                       ChequeNo = x.Key.ChequeNo,
                                       Cr = x.Sum(a => a.Cr),
                                       Dr = x.Sum(a => a.Dr),
                                       Particulars = x.Key.Particulars,
                                       PCCode = x.Key.PCCode,
                                       SubId = x.Key.SubId,
                                       RefId = x.Key.RefId,
                                   }).ToList();
                            var vrId = await PostAutoVoucher(vLst, "IRV", "", WorkingDate, UserId);
                            if (vrId > 0)
                            {
                                foreach (var item in mod)
                                {
                                    if (!await PostingLog("IRV", TransTypeId, item.TransId, vrId))
                                    {
                                        scop.Dispose();
                                    }
                                }
                                await db.SaveChangesAsync();
                                scop.Complete();
                                scop.Dispose();
                            }
                            else
                            {
                                scop.Dispose();
                            }
                        }
                        catch (Exception)
                        {
                            scop.Dispose();
                        }
                    }
                }
                else if (TransTypeId == 3)
                {
                    foreach (var v in mod)
                    {
                        db = new AGEEntities();
                        using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                var tbl = await db.Inv_POPayment.FindAsync(v.TransId);
                                if (tbl.IsPosted)
                                    continue;

                                var suppId = await db.Inv_Purchase.Where(x => x.PInvId == tbl.POInvId).Select(x => x.SuppId).FirstOrDefaultAsync();
                                tbl.IsPosted = true;
                                tbl.PostedBy = UserId;
                                tbl.PostedDate = DateTime.Now;

                                var CPVLst = new List<VoucherDetailVM>();
                                var suppAcc = await GetSuppAcc(101, 4);
                                var localDiscAcc = await GetAcc(458);
                                var cashInHand = await GetAcc(428);

                                if ((tbl.Amount + tbl.Discount) > 0)
                                    CPVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = suppAcc,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = tbl.Amount + tbl.Discount,
                                        Particulars = "Local PO Invoice No " + v.BillNo + " Supplier " + v.CustName,
                                        PCCode = v.LocId,
                                        SubId = suppId
                                    });
                                if (tbl.Amount > 0)
                                    CPVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = cashInHand,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = tbl.Amount,
                                        Dr = 0,
                                        Particulars = "Local PO Invoice No " + v.BillNo + " Supplier " + v.CustName,
                                        PCCode = v.LocId,
                                        SubId = 0
                                    });
                                if (tbl.Discount > 0)
                                    CPVLst.Add(new VoucherDetailVM
                                    {
                                        AccId = localDiscAcc,
                                        CCCode = v.LocId,
                                        ChequeNo = "",
                                        Cr = tbl.Discount,
                                        Dr = 0,
                                        Particulars = "Local PO Invoice No " + v.BillNo + " Supplier " + v.CustName,
                                        PCCode = v.LocId,
                                        SubId = suppId
                                    });


                                var vrId = await PostAutoVoucher(CPVLst, "CPV", v.BillNo, WorkingDate, UserId);
                                if (vrId > 0)
                                {
                                    if (!await PostingLog("CPV", TransTypeId, tbl.TransId, vrId))
                                    {
                                        scop.Dispose();
                                    }
                                    else
                                    {
                                        await db.SaveChangesAsync();
                                        scop.Complete();
                                        scop.Dispose();
                                    }
                                }
                                else
                                {
                                    scop.Dispose();
                                }
                            }
                            catch (Exception)
                            {
                                scop.Dispose();
                            }
                        }
                    }
                }
                else if (TransTypeId == 11)
                {
                    foreach (var v in mod)
                    {
                        db = new AGEEntities();
                        List<VoucherDetailVM> CSVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> STVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> MCVLst = new List<VoucherDetailVM>();
                        List<VoucherDetailVM> CRVLst = new List<VoucherDetailVM>();
                        var instSaleLst = await (from M in db.Inv_Sale
                                                 join D in db.Inv_SaleDetail on M.TransId equals D.TransId
                                                 join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                                                 join SP in db.Inv_Suppliers on ST.SuppId equals SP.SuppId
                                                 where M.TransId == v.TransId
                                                 select new { M.PaymentModeId, SP.CategoryId, ST.ItemId, ST.Exempted, ST.SerialNo, D.MRP, D.SM, D.SPrice, D.PPrice, D.Tax }).ToListAsync();
                        ///////////////////////////////////////Cash Sale Entry////////////////////////////////////////////////

                        //var custAcc = Convert.ToInt64((await db.Inv_Customers.FindAsync(v.CustId)).GLCode);

                        if ((v.SPrice) > 0)
                        {
                            var empSale = await GetAcc(452);
                            CSVLst.Add(new VoucherDetailVM
                            {
                                AccId = empSale,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = v.SPrice,
                                Particulars = "Credit Sale." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = v.CustId
                            });
                        }

                        var cashSaleNormalDiscount = await GetAcc(430);
                        decimal disc = instSaleLst.Sum(x => ((x.MRP - x.SPrice) > 0 ? (x.MRP - x.SPrice) : 0));
                        if (disc > 0)
                            CSVLst.Add(new VoucherDetailVM
                            {
                                AccId = cashSaleNormalDiscount,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = disc,
                                Particulars = "Credit Sale." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = 0
                            });

                        //var cashSaleSpecialDiscount = await GetAcc(431);
                        //if (instSaleLst.Sum(x => x.SM - x.SPrice) > 0)
                        //{
                        //    if (instSaleLst.Sum(x => x.SM - x.MRP) > 0)
                        //        CSVLst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = cashSaleSpecialDiscount,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = 0,
                        //            Dr = instSaleLst.Sum(x => x.SM - x.MRP),
                        //            Particulars = "Cash Sale." + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0
                        //        });
                        //    if (instSaleLst.Sum(x => x.MRP - x.SM) > 0)
                        //        CSVLst.Add(new VoucherDetailVM
                        //        {
                        //            AccId = cashSaleSpecialDiscount,
                        //            CCCode = v.LocId,
                        //            ChequeNo = "",
                        //            Cr = instSaleLst.Sum(x => x.MRP - x.SM),
                        //            Dr = 0,
                        //            Particulars = "Cash Sale." + v.BillNo,
                        //            PCCode = v.LocId,
                        //            SubId = 0
                        //        });
                        //}


                        var cashSaleMarginAboveMRP = await GetAcc(436);
                        decimal mrg = instSaleLst.Sum(x => ((x.SPrice - x.MRP) > 0 ? (x.SPrice - x.MRP) : 0));
                        if (mrg > 0)
                            CSVLst.Add(new VoucherDetailVM
                            {
                                AccId = cashSaleMarginAboveMRP,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = mrg,
                                Dr = 0,
                                Particulars = "Credit Sale." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = 0
                            });
                        //var cashSaleMarginAboveBP = await GetAcc(437);
                        //if (instSaleLst.Sum(x => x.SPrice - x.SM) > 0)
                        //    CSVLst.Add(new VoucherDetailVM
                        //    {
                        //        AccId = cashSaleMarginAboveBP,
                        //        CCCode = v.LocId,
                        //        ChequeNo = "",
                        //        Cr = instSaleLst.Sum(x => x.SPrice - x.SM),
                        //        Dr = 0,
                        //        Particulars = "Cash Sale." + v.BillNo,
                        //        PCCode = v.LocId,
                        //        SubId = 0
                        //    });
                        foreach (var item in instSaleLst)
                        {
                            var cashSaleRetail = await GetAcc(8, item.CategoryId);
                            if (item.MRP > 0)
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashSaleRetail,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.MRP,
                                    Dr = 0,
                                    Particulars = "Credit Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });

                            ////////////////////////////////////////Sales Tax/////////////////////////////////

                            if (item.Tax > 0 && item.CategoryId != 4 && item.Exempted == false)
                            {
                                STVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(9, item.CategoryId),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = item.Tax ?? 0,
                                    Particulars = "Credit Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                                STVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(419),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.Tax ?? 0,
                                    Dr = 0,
                                    Particulars = "Credit Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                            }
                            //////////////////////////////////Consumption////////////////////////////////////////
                            //long consumptionAcc = accLst.Where(x => x.TransTypeId == 6 && x.CategoryId == item.CategoryId).Select(x => Convert.ToInt64(x.GLCode)).FirstOrDefault();
                            //long inventoryAcc = accLst.Where(x => x.TransTypeId == 7 && x.CategoryId == item.CategoryId).Select(x => Convert.ToInt64(x.GLCode)).FirstOrDefault();
                            if (item.PPrice > 0)
                            {
                                MCVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(14, item.CategoryId),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = item.PPrice,
                                    Particulars = "Credit Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                                MCVLst.Add(new VoucherDetailVM
                                {
                                    AccId = await GetAcc(7, item.CategoryId),
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = item.PPrice,
                                    Dr = 0,
                                    Particulars = "Credit Sale." + v.BillNo + " Sr " + item.SerialNo,
                                    PCCode = v.LocId,
                                    SubId = 0,
                                    RefId = item.ItemId
                                });
                            }
                        }
                        //////////////////////////////////////Advance Receive/////////////////////////

                        if (instSaleLst[0].PaymentModeId == 1)
                        {
                            var cashInHand = await GetAcc(428);
                            if (v.Advance > 0)
                                CRVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.Advance,
                                    Particulars = "Credit Sale." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });
                        }
                        else
                        {
                            //var cashInHand = await GetAcc(428);
                            var htvDebtor = await GetAcc(463);
                            if (v.Advance > 0)
                                CRVLst.Add(new VoucherDetailVM
                                {
                                    AccId = htvDebtor,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.Advance,
                                    Particulars = "Credit Sale." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });
                        }
                        if ((v.Advance) > 0)
                        {
                            var empSale = await GetAcc(452);
                            CRVLst.Add(new VoucherDetailVM
                            {
                                AccId = empSale,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = v.Advance,
                                Dr = 0,
                                Particulars = "Credit Sale." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = v.CustId
                            });
                        }

                        using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                var tbl = await db.Inv_Sale.FindAsync(v.TransId);
                                if (tbl.IsPosted)
                                    continue;

                                var vrId = await PostAutoVoucher(CSVLst, "CSV", v.BillNo, WorkingDate, UserId);
                                if (vrId > 0)
                                {
                                    if (!await PostingLog("CSV", TransTypeId, tbl.TransId, vrId))
                                    {
                                        scop.Dispose();
                                    }
                                }
                                else
                                {
                                    scop.Dispose();
                                }
                                if (STVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(STVLst, "STV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("STV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                if (MCVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(MCVLst, "MCV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("MCV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                if (CRVLst.Count > 0)
                                {
                                    vrId = await PostAutoVoucher(CRVLst, "CRV", v.BillNo, WorkingDate, UserId);
                                    if (vrId > 0)
                                    {
                                        if (!await PostingLog("CRV", TransTypeId, tbl.TransId, vrId))
                                        {
                                            scop.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        scop.Dispose();
                                    }
                                }
                                tbl.IsPosted = true;
                                tbl.PostedBy = UserId;
                                tbl.PostedDate = DateTime.Now;
                                await db.SaveChangesAsync();
                                scop.Complete();
                                scop.Dispose();
                            }
                            catch (Exception)
                            {
                                scop.Dispose();
                            }
                        }
                    }


                }
                else if (TransTypeId == 13)
                {
                    foreach (var v in mod)
                    {
                        db = new AGEEntities();
                        List<VoucherDetailVM> CSVLst = new List<VoucherDetailVM>();
                        var ord = await db.Inv_SaleOrder.Where(x => x.TransId == v.TransId).FirstOrDefaultAsync();
                        ///////////////////////////////////////Cash Sale Entry////////////////////////////////////////////////
                        var cashInHand = await GetAcc(428);
                        if (v.Advance > 0)
                        {
                            var ordAdvanceAcc = await GetAcc(462);
                            //decimal ordAdvance = instSaleLst[0].OrderAdvance;

                            CSVLst.Add(new VoucherDetailVM
                            {
                                AccId = ordAdvanceAcc,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = v.Advance,
                                Dr = 0,
                                Particulars = "Sale Order." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = 0
                            });
                        }
                        if (ord.PaymentModeId == 1)
                        {
                            if (v.Advance > 0)
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.Advance,
                                    Particulars = "Sale Order." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });
                        }
                        else
                        {
                            //var cashInHand = await GetAcc(428);
                            //if (v.Advance > 0)
                            //    CSVLst.Add(new VoucherDetailVM
                            //    {
                            //        AccId = cashInHand,
                            //        CCCode = v.LocId,
                            //        ChequeNo = "",
                            //        Cr = 0,
                            //        Dr = v.Advance,
                            //        Particulars = "Cash Sale." + v.BillNo,
                            //        PCCode = v.LocId,
                            //        SubId = 0
                            //    });

                            if (v.Advance > 0)
                            {
                                var cashClr = await GetAcc(450);
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClr,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.Advance,
                                    Particulars = "Sale Order." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClr,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = v.Advance,
                                    Dr = 0,
                                    Particulars = "Sale Order." + v.BillNo,
                                    PCCode = 72,
                                    SubId = 0
                                });
                                var htvDebtor = await GetAcc(463);
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = htvDebtor,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.Advance,
                                    Particulars = "Sale Order." + v.BillNo,
                                    PCCode = 72,
                                    SubId = 0
                                });
                            }

                        }

                        using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                var tbl = await db.Inv_SaleOrder.FindAsync(v.TransId);
                                if (tbl.IsPosted)
                                    continue;

                                var vrId = await PostAutoVoucher(CSVLst, "CRV", v.BillNo, WorkingDate, UserId);
                                if (vrId > 0)
                                {
                                    if (!await PostingLog("CRV", TransTypeId, tbl.TransId, vrId))
                                    {
                                        scop.Dispose();
                                        continue;
                                    }
                                }
                                else
                                {
                                    scop.Dispose();
                                    continue;
                                }
                                tbl.IsPosted = true;
                                tbl.PostedBy = UserId;
                                tbl.PostedDate = DateTime.Now;
                                await db.SaveChangesAsync();
                                scop.Complete();
                                scop.Dispose();
                            }
                            catch (Exception)
                            {
                                scop.Dispose();
                            }
                        }
                    }


                }
                else if (TransTypeId == 14)
                {
                    foreach (var v in mod)
                    {
                        db = new AGEEntities();
                        List<VoucherDetailVM> CSVLst = new List<VoucherDetailVM>();
                        var ord = await db.Inv_SaleOrder.Where(x => x.TransId == v.TransId).FirstOrDefaultAsync();
                        ///////////////////////////////////////Cash Sale Entry////////////////////////////////////////////////
                        var cashInHand = await GetAcc(428);
                        if (v.Advance > 0)
                        {
                            var ordAdvanceAcc = await GetAcc(462);
                            //decimal ordAdvance = instSaleLst[0].OrderAdvance;

                            CSVLst.Add(new VoucherDetailVM
                            {
                                AccId = ordAdvanceAcc,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = v.Advance,
                                Particulars = "Sale Order Return." + v.BillNo,
                                PCCode = v.LocId,
                                SubId = 0
                            });
                        }
                        if (ord.PaymentModeId == 1)
                        {
                            if (v.Advance > 0)
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = v.Advance,
                                    Dr = 0,
                                    Particulars = "Sale Order Return." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });
                        }
                        else
                        {
                            //var cashInHand = await GetAcc(428);
                            //if (v.Advance > 0)
                            //    CSVLst.Add(new VoucherDetailVM
                            //    {
                            //        AccId = cashInHand,
                            //        CCCode = v.LocId,
                            //        ChequeNo = "",
                            //        Cr = 0,
                            //        Dr = v.Advance,
                            //        Particulars = "Cash Sale." + v.BillNo,
                            //        PCCode = v.LocId,
                            //        SubId = 0
                            //    });

                            if (v.Advance > 0)
                            {
                                var cashClr = await GetAcc(450);
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClr,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = v.Advance,
                                    Dr = 0,
                                    Particulars = "Sale Order Return." + v.BillNo,
                                    PCCode = v.LocId,
                                    SubId = 0
                                });
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClr,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.Advance,
                                    Particulars = "Sale Order Return." + v.BillNo,
                                    PCCode = 72,
                                    SubId = 0
                                });
                                var htvDebtor = await GetAcc(463);
                                CSVLst.Add(new VoucherDetailVM
                                {
                                    AccId = htvDebtor,
                                    CCCode = v.LocId,
                                    ChequeNo = "",
                                    Cr = v.Advance,
                                    Dr = 0,
                                    Particulars = "Sale Order Return." + v.BillNo,
                                    PCCode = 72,
                                    SubId = 0
                                });
                            }

                        }

                        using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                var tbl = await db.Inv_SaleOrder.FindAsync(v.TransId);
                                if (tbl.IsPosted)
                                    continue;

                                var vrId = await PostAutoVoucher(CSVLst, "CPV", v.BillNo, WorkingDate, UserId);
                                if (vrId > 0)
                                {
                                    if (!await PostingLog("CPV", TransTypeId, tbl.TransId, vrId))
                                    {
                                        scop.Dispose();
                                        continue;
                                    }
                                }
                                else
                                {
                                    scop.Dispose();
                                    continue;
                                }
                                tbl.IsPosted = true;
                                tbl.PostedBy = UserId;
                                tbl.PostedDate = DateTime.Now;
                                await db.SaveChangesAsync();
                                scop.Complete();
                                scop.Dispose();
                            }
                            catch (Exception)
                            {
                                scop.Dispose();
                            }
                        }
                    }


                }
                else if (TransTypeId == 15)
                {
                    db = new AGEEntities();
                    using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        try
                        {
                            //long instDiscountAcc = await GetAcc(446);

                            var cashInHand = await GetAcc(428);
                            var clrAcc = await GetAcc(450);

                            List<VoucherDetailVM> vLst = new List<VoucherDetailVM>();
                            foreach (var v in mod)
                            {
                                var tbl = await db.Inv_SaleCreditReceive.FindAsync(v.TransId);
                                if (tbl.IsPosted)
                                    continue;


                                tbl.IsPosted = true;
                                tbl.PostedBy = UserId;
                                tbl.PostedDate = DateTime.Now;

                                long refSale = 0;
                                long custAcc = 0;
                                string customer = "";
                                if (tbl.Inv_Sale.TransactionTypeId == 5)
                                {
                                    refSale = await GetAcc(451);
                                    custAcc = Convert.ToInt64((await db.Inv_Customers.FindAsync(v.CustId)).GLCode);
                                    customer = "CUST-" + v.CustId.ToString();
                                }
                                else if (tbl.Inv_Sale.TransactionTypeId == 11)
                                {
                                    refSale = await GetAcc(452);
                                    custAcc = v.CustId;
                                    customer = "EMPL-" + v.CustId.ToString();
                                }

                                if (tbl.LocId != tbl.Inv_Sale.LocId)
                                {
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = cashInHand,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = v.SPrice,
                                            Particulars = "Credit Received.",
                                            PCCode = tbl.LocId,
                                            SubId = 0
                                        });
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = clrAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = v.SPrice,
                                            Dr = 0,
                                            Particulars = "Credit Received.",
                                            PCCode = tbl.LocId,
                                            SubId = 0
                                        });
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = clrAcc,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = v.SPrice,
                                            Particulars = "Credit Received.",
                                            PCCode = tbl.Inv_Sale.LocId,
                                            SubId = 0
                                        });
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = refSale,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = v.SPrice,
                                            Dr = 0,
                                            Particulars = "Credit Received. " + customer + " RecvNo " + v.TransId.ToString(),
                                            PCCode = tbl.Inv_Sale.LocId,
                                            SubId = custAcc,
                                            RefId = v.TransId
                                        });
                                }
                                else
                                {
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = cashInHand,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = v.SPrice,
                                            Particulars = "Credit Received.",
                                            PCCode = v.LocId,
                                            SubId = 0
                                        });
                                    if (v.SPrice > 0)
                                        vLst.Add(new VoucherDetailVM
                                        {
                                            AccId = refSale,
                                            CCCode = v.LocId,
                                            ChequeNo = "",
                                            Cr = v.SPrice,
                                            Dr = 0,
                                            Particulars = "Credit Received. " + customer + " RecvNo " + v.TransId.ToString(),
                                            PCCode = v.LocId,
                                            SubId = custAcc,
                                            RefId = v.TransId
                                        });
                                }


                            }
                            vLst = vLst.GroupBy(x => new { x.AccId, x.CCCode, x.ChequeNo, x.Particulars, x.PCCode, x.SubId, x.RefId }).Select(x =>
                                   new VoucherDetailVM
                                   {
                                       AccId = x.Key.AccId,
                                       CCCode = x.Key.CCCode,
                                       ChequeNo = x.Key.ChequeNo,
                                       Cr = x.Sum(a => a.Cr),
                                       Dr = x.Sum(a => a.Dr),
                                       Particulars = x.Key.Particulars,
                                       PCCode = x.Key.PCCode,
                                       SubId = x.Key.SubId,
                                       RefId = x.Key.RefId,
                                   }).ToList();
                            var vrId = await PostAutoVoucher(vLst, "CRV", "", WorkingDate, UserId);
                            if (vrId > 0)
                            {
                                foreach (var item in mod)
                                {
                                    if (!await PostingLog("CRV", TransTypeId, item.TransId, vrId))
                                    {
                                        scop.Dispose();
                                    }
                                }
                                await db.SaveChangesAsync();
                                scop.Complete();
                                scop.Dispose();
                            }
                            else
                            {
                                scop.Dispose();
                            }
                        }
                        catch (Exception)
                        {
                            scop.Dispose();
                        }
                    }
                }
                return true;
                //var tbl = await db.Fin_AutoVoucherLog.Where(x => x.LocId == v && x.WorkingDate == WorkingDate).FirstOrDefaultAsync();
                //if (tbl == null)
                //{
                //    tbl = new Fin_AutoVoucherLog
                //    {
                //        LocId = v,
                //        WorkingDate = WorkingDate
                //    };
                //    db.Fin_AutoVoucherLog.Add(tbl);
                //}
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Expense Voucher Posting
        public async Task<List<ExpenseTransactionVM>> ExpenseVoucherPostingList(int Loc, int ExpHead, DateTime WorkingDate)
        {
            try
            {
                return await db.Lse_ExpenseTransaction.Where(x => (x.LocId == Loc || Loc == 0) &&
                (x.ExpHeadId == ExpHead || ExpHead == 0) && x.WorkingDate == WorkingDate && x.Status != "C").Select(x =>
                new ExpenseTransactionVM
                {
                    Amount = x.Amount,
                    CCCode = x.CCCode ?? 0,
                    WorkingDate = x.WorkingDate,
                    Description = x.Description,
                    LocId = x.LocId,
                    ExpHeadId = x.ExpHeadId,
                    TransId = x.TransId,
                    IsPosted = x.IsPosted,
                    SubId = x.SubId ?? 0,
                    RefBillNo = x.RefBillNo
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<long> CreateExpenseVoucherPosting(ExpenseTransactionVM mod, int UserId)
        {
            try
            {
                Lse_ExpenseTransaction tbl = new Lse_ExpenseTransaction
                {
                    Description = mod.Description,
                    ExpHeadId = mod.ExpHeadId,
                    LocId = mod.LocId,
                    TransDate = DateTime.Now,
                    UserId = UserId,
                    CCCode = mod.CCCode,
                    WorkingDate = mod.WorkingDate,
                    Amount = mod.Amount,
                    SubId = mod.SubId,
                    Status = "U",
                    RefBillNo = mod.RefBillNo
                };
                db.Lse_ExpenseTransaction.Add(tbl);
                await db.SaveChangesAsync();

                return tbl.TransId;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public async Task<bool> UpdateExpenseVoucherPosting(ExpenseTransactionVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_ExpenseTransaction.FindAsync(mod.TransId);
                if (tbl != null)
                {
                    if (tbl.IsPosted)
                        return
                            false;

                    tbl.Amount = mod.Amount;
                    tbl.Description = mod.Description;
                    tbl.CCCode = mod.CCCode;
                    tbl.ExpHeadId = mod.ExpHeadId;
                    tbl.ModifiedBy = UserId;
                    tbl.ModifiedDate = DateTime.Now;
                    tbl.SubId = mod.SubId;
                    tbl.RefBillNo = mod.RefBillNo;
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DestroyExpenseVoucherPosting(ExpenseTransactionVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_ExpenseTransaction.FindAsync(mod.TransId);
                if (tbl != null)
                {
                    if (tbl.IsPosted)
                        return
                            false;

                    tbl.Status = "C";
                    tbl.ModifiedBy = UserId;
                    tbl.ModifiedDate = DateTime.Now;
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> PostExpenseVoucherPosting(List<long> mod, int UserId)
        {
            try
            {
                DateTime? workingDate = null;
                List<VoucherDetailVM> lst = new List<VoucherDetailVM>();
                var cashInHand = await GetAcc(428);
                var clearingAcc = await GetAcc(459);
                foreach (var v in mod)
                {
                    var tbl = await db.Lse_ExpenseTransaction.FindAsync(v);
                    if (tbl.IsPosted)
                        continue;

                    var Acc = await db.Fin_Accounts.FindAsync(Convert.ToInt64(tbl.Lse_ExpenseHead.GLCode));
                    if (Acc.HasSub && (tbl.SubId ?? 0) == 0)
                    {
                        return false;
                    }
                    if (Acc.IsHOGL && tbl.CCCode != 72)
                    {
                        return false;
                    }
                    //if(tbl.LocId == 72 || tbl.LocId == 95)
                    //{
                    //    cashInHand = await GetAcc(461);
                    //}
                    //else
                    //{
                    //cashInHand = await GetAcc(428);
                    //}

                    //tbl.PostedBy = UserId;
                    //tbl.PostedDate = DateTime.Now;
                    //tbl.IsPosted = true;
                    //tbl.Status = "P";
                    var pcCode = (await db.Fin_CostCenters.FindAsync(tbl.CCCode)).PCCode ?? 0;
                    if (workingDate != null)
                    {
                        if (workingDate != tbl.WorkingDate)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        workingDate = tbl.WorkingDate;
                    }

                    if (tbl.LocId == pcCode)
                    {
                        lst.Add(new VoucherDetailVM
                        {
                            AccId = Convert.ToInt64(tbl.Lse_ExpenseHead.GLCode),
                            CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                            ChequeNo = "",
                            Cr = 0,
                            Dr = tbl.Amount,
                            Particulars = tbl.Description,
                            PCCode = tbl.LocId,
                            SubId = tbl.SubId ?? 0,
                            RefId = tbl.TransId
                        });

                        lst.Add(new VoucherDetailVM
                        {
                            AccId = cashInHand,
                            CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                            ChequeNo = "",
                            Cr = tbl.Amount,
                            Dr = 0,
                            Particulars = tbl.Description,
                            PCCode = tbl.LocId,
                            SubId = 0
                        });
                    }
                    else if (tbl.CCCode != tbl.LocId)
                    {
                        lst.Add(new VoucherDetailVM
                        {
                            AccId = clearingAcc,
                            CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                            ChequeNo = "",
                            Cr = 0,
                            Dr = tbl.Amount,
                            Particulars = tbl.Description,
                            PCCode = tbl.LocId,
                            SubId = 0
                        });
                        lst.Add(new VoucherDetailVM
                        {
                            AccId = cashInHand,
                            CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                            ChequeNo = "",
                            Cr = tbl.Amount,
                            Dr = 0,
                            Particulars = tbl.Description,
                            PCCode = tbl.LocId,
                            SubId = 0
                        });

                        lst.Add(new VoucherDetailVM
                        {
                            AccId = Convert.ToInt64(tbl.Lse_ExpenseHead.GLCode),
                            CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                            ChequeNo = "",
                            Cr = 0,
                            Dr = tbl.Amount,
                            Particulars = tbl.Description,
                            PCCode = pcCode,
                            SubId = tbl.SubId ?? 0,
                            RefId = tbl.TransId
                        });

                        lst.Add(new VoucherDetailVM
                        {
                            AccId = clearingAcc,
                            CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                            ChequeNo = "",
                            Cr = tbl.Amount,
                            Dr = 0,
                            Particulars = tbl.Description,
                            PCCode = pcCode,
                            SubId = 0
                        });
                    }

                }
                //var vLst = lst.Select(x => x.PCCode).Distinct().ToList();
                //foreach (var v in vLst)
                //{

                //    var vouc = lst.Where(x => x.PCCode == v).ToList();
                //    var vvLst = vouc;
                //    vouc.Add(new VoucherDetailVM
                //    {
                //        AccId = cashInHand,
                //        CCCode = v.LocId,
                //        ChequeNo = "",
                //        Cr = vouc.Sum(x => x.Dr),
                //        Dr = 0,
                //        Particulars = "Expense Paid",
                //        PCCode = v,
                //        SubId = 0
                //    });
                using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var vrId = await PostAutoVoucher(lst, "EXP", "", (DateTime)workingDate, UserId);
                        if (vrId == 0)
                        {
                            scop.Dispose();
                            return false;
                        }
                        else
                        {
                            var vrNo = await db.Fin_Voucher.Where(x => x.VrId == vrId).Select(x => x.VrNo).FirstOrDefaultAsync();
                            foreach (var a in lst)
                            {
                                if (a.RefId > 0)
                                {
                                    var tbl = await db.Lse_ExpenseTransaction.FindAsync(a.RefId);
                                    tbl.PostedBy = UserId;
                                    tbl.PostedDate = DateTime.Now;
                                    tbl.IsPosted = true;
                                    tbl.Status = "P";
                                    tbl.VrNo = vrNo;

                                    db.Fin_VoucherPostingLog.Add(new Fin_VoucherPostingLog { TransId = tbl.TransId, TransTypeId = 50, VrTypeId = "EXP", VrId = vrId });
                                }
                            }
                            await db.SaveChangesAsync();
                        }
                        //}


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
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Cash Receive Voucher Posting
        public async Task<List<CashReceiveVM>> CashReceivePostingList(int Loc, DateTime WorkingDate)
        {
            try
            {
                return await db.Lse_CashReceive.Where(x => (x.LocId == Loc || Loc == 0) &&
                 x.Status != "C" && x.WorkingDate == WorkingDate).Select(x =>
                new CashReceiveVM
                {
                    Amount = x.Amount,
                    CCCode = x.CCCode ?? 0,
                    Description = x.Description,
                    LocId = x.LocId,
                    AccId = x.AccId ?? 0,
                    TransId = x.TransId,
                    IsPosted = x.IsPosted,
                    SubId = x.SubId ?? 0
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCashReceiveVoucherPosting(CashReceiveVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_CashReceive.FindAsync(mod.TransId);
                if (tbl != null)
                {
                    if (tbl.IsPosted)
                        return
                            false;

                    tbl.Amount = mod.Amount;
                    tbl.Description = mod.Description;
                    tbl.CCCode = mod.CCCode;
                    tbl.AccId = mod.AccId;
                    tbl.ModifiedBy = UserId;
                    tbl.ModifiedDate = DateTime.Now;
                    tbl.SubId = mod.SubId;
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DestroyCashReceiveVoucherPosting(CashReceiveVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_CashReceive.FindAsync(mod.TransId);
                if (tbl != null)
                {
                    if (tbl.IsPosted)
                        return false;

                    tbl.Status = "C";
                    tbl.ModifiedBy = UserId;
                    tbl.ModifiedDate = DateTime.Now;
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> PostCashReceiveVoucherPosting(List<long> mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    DateTime? workingDate = null;
                    List<VoucherDetailVM> lst = new List<VoucherDetailVM>();
                    var cashInHand = await GetAcc(428);
                    var clearingAcc = await GetAcc(459);
                    foreach (var v in mod)
                    {
                        var tbl = await db.Lse_CashReceive.FindAsync(v);
                        if (tbl.IsPosted)
                            continue;

                        var Acc = await db.Fin_Accounts.FindAsync(Convert.ToInt64(tbl.AccId));
                        if (Acc.HasSub && (tbl.SubId ?? 0) == 0)
                        {
                            scop.Dispose();
                            return false;
                        }
                        if (Acc.IsHOGL && tbl.CCCode != 72)
                        {
                            scop.Dispose();
                            return false;
                        }

                        if (tbl.AccId > 0)
                        {
                            tbl.PostedBy = UserId;
                            tbl.PostedDate = DateTime.Now;
                            tbl.IsPosted = true;
                            tbl.Status = "P";
                            if (workingDate != null)
                            {
                                if (workingDate != tbl.WorkingDate)
                                {
                                    scop.Dispose();
                                    return false;
                                }
                            }
                            else
                            {
                                workingDate = tbl.WorkingDate;
                            }

                            var pcCode = (await db.Fin_CostCenters.FindAsync(tbl.CCCode)).PCCode ?? 0;
                            if (pcCode == tbl.LocId)
                            {
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = tbl.AccId ?? 0,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = tbl.Amount,
                                    Dr = 0,
                                    Particulars = tbl.Description,
                                    PCCode = tbl.LocId,
                                    SubId = tbl.SubId ?? 0,
                                    RefId = tbl.TransId
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = tbl.Amount,
                                    Particulars = "Cash Receive Posting",
                                    PCCode = tbl.LocId,
                                    SubId = 0
                                });
                            }
                            else
                            {
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = tbl.AccId ?? 0,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = tbl.Amount,
                                    Dr = 0,
                                    Particulars = tbl.Description,
                                    PCCode = pcCode,
                                    SubId = tbl.SubId ?? 0,
                                    RefId = tbl.TransId
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = clearingAcc,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = tbl.Amount,
                                    Particulars = tbl.Description,
                                    PCCode = pcCode,
                                    SubId = 0
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = clearingAcc,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = tbl.Amount,
                                    Dr = 0,
                                    Particulars = tbl.Description,
                                    PCCode = tbl.LocId,
                                    SubId = 0
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = tbl.Amount,
                                    Particulars = tbl.Description,
                                    PCCode = tbl.LocId,
                                    SubId = 0
                                });
                            }
                        }
                    }
                    var vrId = await PostAutoVoucher(lst, "CRV", "", (DateTime)workingDate, UserId);
                    if (vrId == 0)
                    {
                        scop.Dispose();
                    }
                    else
                    {
                        foreach (var v in mod)
                        {
                            db.Fin_VoucherPostingLog.Add(new Fin_VoucherPostingLog { TransId = v, TransTypeId = 52, VrTypeId = "CRV", VrId = vrId });
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
        #endregion

        #region Cash Payment Voucher Posting
        public async Task<List<CashPaymentVM>> CashPaymentPostingList(int Loc, DateTime WorkingDate)
        {
            try
            {
                return await db.Lse_CashPayment.Where(x => (x.LocId == Loc || Loc == 0) &&
                 x.Status != "C" && x.IsPosted == false && x.WorkingDate == WorkingDate).Select(x =>
                new CashPaymentVM
                {
                    Amount = x.Amount,
                    CCCode = x.CCCode ?? 0,
                    Description = x.Description,
                    LocId = x.LocId,
                    AccId = x.AccId ?? 0,
                    TransId = x.TransId,
                    IsPosted = x.IsPosted,
                    SubId = x.SubId ?? 0,
                    WorkingDate = x.WorkingDate
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<CashPaymentVM> CreateCashPaymentVoucherPosting(CashPaymentVM mod, int UserId)
        {
            try
            {
                var tbl = new Lse_CashPayment();
                tbl.Amount = mod.Amount;
                tbl.Description = mod.Description;
                tbl.CCCode = mod.CCCode;
                tbl.AccId = mod.AccId;
                tbl.SubId = mod.SubId;
                tbl.ModifiedBy = UserId;
                tbl.ModifiedDate = DateTime.Now;
                tbl.IsPosted = false;
                tbl.LocId = mod.LocId;
                tbl.Status = "U";
                tbl.TransDate = DateTime.Now;
                tbl.UserId = UserId;
                tbl.WorkingDate = mod.WorkingDate;
                db.Lse_CashPayment.Add(tbl);
                await db.SaveChangesAsync();
                mod.TransId = tbl.TransId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateCashPaymentVoucherPosting(CashPaymentVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_CashPayment.FindAsync(mod.TransId);
                if (tbl != null)
                {
                    if (tbl.IsPosted)
                        return
                            false;

                    tbl.Amount = mod.Amount;
                    tbl.Description = mod.Description;
                    tbl.CCCode = mod.CCCode;
                    tbl.AccId = mod.AccId;
                    tbl.SubId = mod.SubId;
                    tbl.ModifiedBy = UserId;
                    tbl.ModifiedDate = DateTime.Now;
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DestroyCashPaymentVoucherPosting(CashPaymentVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_CashPayment.FindAsync(mod.TransId);
                if (tbl != null)
                {
                    if (tbl.IsPosted)
                        return false;

                    tbl.Status = "C";
                    tbl.ModifiedBy = UserId;
                    tbl.ModifiedDate = DateTime.Now;
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> PostCashPaymentVoucherPosting(List<long> mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    DateTime? workingDate = null;
                    List<VoucherDetailVM> lst = new List<VoucherDetailVM>();
                    var cashInHand = await GetAcc(428);
                    var clearingAcc = await GetAcc(459);
                    foreach (var v in mod)
                    {
                        var tbl = await db.Lse_CashPayment.FindAsync(v);
                        if (tbl.IsPosted)
                            continue;

                        var Acc = await db.Fin_Accounts.FindAsync(Convert.ToInt64(tbl.AccId));
                        if (Acc.HasSub && (tbl.SubId ?? 0) == 0)
                        {
                            scop.Dispose();
                            return false;
                        }
                        if (Acc.IsHOGL && tbl.CCCode != 72)
                        {
                            scop.Dispose();
                            return false;
                        }
                        //if (tbl.LocId == 72 || tbl.LocId == 95)
                        //{
                        //    cashInHand = await GetAcc(461);
                        //}
                        //else
                        //{
                        //cashInHand = await GetAcc(428);
                        //}
                        var pcCode = (await db.Fin_CostCenters.FindAsync(tbl.CCCode)).PCCode ?? 0;
                        if (tbl.AccId > 0)
                        {
                            tbl.PostedBy = UserId;
                            tbl.PostedDate = DateTime.Now;
                            tbl.IsPosted = true;
                            tbl.Status = "P";
                            if (workingDate != null)
                            {
                                if (workingDate != tbl.WorkingDate)
                                {
                                    scop.Dispose();
                                    return false;
                                }
                            }
                            else
                            {
                                workingDate = tbl.WorkingDate;
                            }
                            //workingDate = tbl.WorkingDate;
                            if (tbl.LocId == pcCode)
                            {
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = tbl.AccId ?? 0,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = tbl.Amount,
                                    Particulars = tbl.Description,
                                    PCCode = tbl.LocId,
                                    SubId = tbl.SubId ?? 0,
                                    RefId = tbl.TransId
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = tbl.Amount,
                                    Dr = 0,
                                    Particulars = tbl.Description,
                                    PCCode = tbl.LocId,
                                    SubId = 0
                                });
                            }
                            else if (tbl.CCCode != tbl.LocId)
                            {
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = clearingAcc,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = tbl.Amount,
                                    Particulars = tbl.Description,
                                    PCCode = tbl.LocId,
                                    SubId = 0
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = tbl.Amount,
                                    Dr = 0,
                                    Particulars = tbl.Description,
                                    PCCode = tbl.LocId,
                                    SubId = 0
                                });

                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = tbl.AccId ?? 0,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = tbl.Amount,
                                    Particulars = tbl.Description,
                                    PCCode = pcCode,
                                    SubId = tbl.SubId ?? 0,
                                    RefId = tbl.TransId
                                });

                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = clearingAcc,
                                    CCCode = Convert.ToInt32(tbl.CCCode ?? 0),
                                    ChequeNo = "",
                                    Cr = tbl.Amount,
                                    Dr = 0,
                                    Particulars = tbl.Description,
                                    PCCode = pcCode,
                                    SubId = 0
                                });
                            }
                        }
                    }
                    //var vLst = lst.Select(x => x.PCCode).Distinct().ToList();
                    //foreach (var v in vLst)
                    //{
                    //    var vouc = lst.Where(x => x.PCCode == v).ToList();
                    //    vouc.Add(new VoucherDetailVM
                    //    {
                    //        AccId = cashInHand,
                    //        CCCode = v.LocId,
                    //        ChequeNo = "",
                    //        Cr = vouc.Sum(x => x.Dr),
                    //        Dr = 0,
                    //        Particulars = "Cash Payment Posting",
                    //        PCCode = v,
                    //        SubId = 0
                    //    });
                    var vrId = await PostAutoVoucher(lst, "CPV", "", (DateTime)workingDate, UserId);
                    if (vrId == 0)
                    {
                        scop.Dispose();
                    }
                    else
                    {
                        foreach (var v in mod)
                        {
                            db.Fin_VoucherPostingLog.Add(new Fin_VoucherPostingLog { TransId = v, TransTypeId = 53, VrTypeId = "CPV", VrId = vrId });
                        }
                        await db.SaveChangesAsync();
                    }
                    //}

                    //await db.SaveChangesAsync();
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

        #region Cheque

        //Change Cheuq Status   
        public async Task<List<BankBookTransVM>> GetChequeLists(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var chq = await (from item in db.Fin_BankBookTrans
                                 where item.ChequeDate >= StartDate && item.ChequeDate <= EndDate
                                 select new BankBookTransVM()
                                 {
                                     TransID = item.TransID,
                                     ChequeNo = item.ChequeNo,
                                     ChequeDate = item.ChequeDate,
                                     Amount = item.Amount,
                                     PaymentType = item.PaymentType,
                                     Recipient = item.Recipient,
                                     ClearDate = item.ClearDate,
                                     ChequeType = item.ChequeType,
                                     VoidDate = item.VoidDate,
                                     Status = item.Status,
                                     Remarks = item.Remarks,
                                     InstrumentNo = item.InstrumentNo
                                 }).ToListAsync();
                return chq;

            }
            catch (Exception e)
            {
                return new List<BankBookTransVM>();
            }
        }

        public async Task<List<ChequeVM>> GetChequeByAccId(long AccId)
        {
            return await db.Fin_BankBookTrans.Where(x => x.AccId == AccId && x.VrId == 0 && (x.ChequeNo != null || x.ChequeNo != "") && x.TransDate == null).Select(x => new ChequeVM() { ChequeNo = x.ChequeNo }).ToListAsync();
        }
        public async Task<List<ChequeVM>> ChequeByAccTrans(long AccId, long TransId)
        {
            try
            {
                var lst = await db.Fin_BankBookTrans.Where(x => x.AccId == AccId && x.Status == null).Select(x => new ChequeVM() { ChequeNo = x.ChequeNo }).ToListAsync();
                var cheq = await db.Inv_SuppPayment.Where(x => x.TransId == TransId && x.BankAccId == AccId).Select(x => x.ChequeNo).FirstOrDefaultAsync();
                if (cheq != null)
                {
                    lst.Add(new ChequeVM { ChequeNo = cheq });
                }
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateChequeStatus(long TransId, string Status, int UserId)
        {
            try
            {
                var chq = await db.Fin_BankBookTrans.Where(x => x.TransID == TransId).FirstOrDefaultAsync();
                chq.Status = Status;
                if (Status == "C")
                {
                    chq.ClearDate = DateTime.Now;
                }
                if (Status == "V")
                {
                    chq.VoidDate = DateTime.Now;
                }
                chq.TransDate = DateTime.Now;
                chq.UserId = UserId;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        //Add Security Cheuqe
        public async Task<bool> AddCheque(BankBookTransVM mod)
        {
            try
            {

                //var cheq = await db.Fin_BankBook.Where(x => x.AccId == mod.AccId).FirstOrDefaultAsync();
                //if (cheq != null)
                //{
                //    if (cheq.CurrentChqNo != mod.ChequeNum)
                //        return false;
                //    else
                //    {
                //        cheq.CurrentChqNo = (Convert.ToInt64(cheq.CurrentChqNo) + 1).ToString();

                //    }
                //}
                //await db.SaveChangesAsync();

                var fbt = await db.Fin_BankBookTrans.Where(x => x.AccId == mod.AccId && x.ChequeNo == mod.ChequeNum).FirstOrDefaultAsync();

                if (fbt != null)
                {
                    fbt.Amount = mod.Amount;
                    fbt.ChequeDate = mod.ChequeDate;
                    fbt.ChequeNo = mod.ChequeNum;
                    fbt.ChequeType = mod.ChequeType;
                    fbt.PaymentType = "Security Cheque";
                    fbt.InstrumentType = mod.InstrumentType;
                    fbt.Recipient = mod.Recipient;
                    fbt.Status = "U";
                    fbt.UserId = mod.UserId;
                    fbt.TransDate = DateTime.Now;
                    fbt.InstrumentNo = mod.InstrumentNo;
                    fbt.Remarks = mod.Remarks;
                    await db.SaveChangesAsync();

                    var bankbook = await db.Fin_BankBook.Where(x => x.AccId == mod.AccId).FirstOrDefaultAsync();
                    bankbook.CurrentChqNo = mod.ChequeNum;
                    await db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion

        #region MiscVouchers
        public async Task<bool> StockReceiveVoucher(List<Inv_IssueDetail> vLst, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    List<VoucherDetailVM> lst = new List<VoucherDetailVM>();
                    var invClearing = await GetAcc(448);
                    var iss = vLst[0].Inv_Issue;
                    var fromLoc = await db.Comp_Locations.Where(x => x.LocId == iss.FromLocId).Select(x => x.LocCode).FirstOrDefaultAsync();
                    var toLoc = await db.Comp_Locations.Where(x => x.LocId == iss.ToLocId).Select(x => x.LocCode).FirstOrDefaultAsync();

                    foreach (var a in vLst)
                    {
                        var v = await db.Inv_IssueDetail.FindAsync(a.TransDtlId);
                        var invAcc = await GetAcc(7, v.Inv_Store.Inv_Suppliers.CategoryId);

                        lst.Add(new VoucherDetailVM
                        {
                            AccId = invAcc,
                            CCCode = v.Inv_Issue.FromLocId,
                            ChequeNo = "",
                            Cr = v.Inv_Store.PPrice,
                            Dr = 0,
                            Particulars = "Stock Transfer " + fromLoc + "-" + toLoc + ". " + iss.TransId.ToString() + " Sr " + v.Inv_Store.SerialNo,
                            PCCode = v.Inv_Issue.FromLocId,
                            SubId = 0,
                            RefId = v.ItemId
                        });
                        lst.Add(new VoucherDetailVM
                        {
                            AccId = invClearing,
                            CCCode = v.Inv_Issue.FromLocId,
                            ChequeNo = "",
                            Cr = 0,
                            Dr = v.Inv_Store.PPrice,
                            Particulars = "Stock Transfer " + fromLoc + "-" + toLoc + ". " + iss.TransId.ToString(),
                            PCCode = v.Inv_Issue.FromLocId,
                            SubId = 0,
                            RefId = 0
                        });
                        ////////////////////////////////////////////////////////////////
                        lst.Add(new VoucherDetailVM
                        {
                            AccId = invAcc,
                            CCCode = v.Inv_Issue.ToLocId,
                            ChequeNo = "",
                            Cr = 0,
                            Dr = v.Inv_Store.PPrice,
                            Particulars = "Stock Transfer " + fromLoc + "-" + toLoc + ". " + iss.TransId.ToString() + " Sr " + v.Inv_Store.SerialNo,
                            PCCode = v.Inv_Issue.ToLocId,
                            SubId = 0,
                            RefId = v.ItemId
                        });
                        lst.Add(new VoucherDetailVM
                        {
                            AccId = invClearing,
                            CCCode = v.Inv_Issue.ToLocId,
                            ChequeNo = "",
                            Cr = v.Inv_Store.PPrice,
                            Dr = 0,
                            Particulars = "Stock Transfer " + fromLoc + "-" + toLoc + ". " + iss.TransId.ToString(),
                            PCCode = v.Inv_Issue.ToLocId,
                            SubId = 0,
                            RefId = 0
                        });
                    }
                    lst = lst.GroupBy(x => new { x.AccId, x.CCCode, x.ChequeNo, x.Particulars, x.PCCode, x.SubId, x.RefId }).Select(x =>
                               new VoucherDetailVM
                               {
                                   AccId = x.Key.AccId,
                                   CCCode = x.Key.CCCode,
                                   ChequeNo = x.Key.ChequeNo,
                                   Cr = x.Sum(a => a.Cr),
                                   Dr = x.Sum(a => a.Dr),
                                   Particulars = x.Key.Particulars,
                                   PCCode = x.Key.PCCode,
                                   SubId = x.Key.SubId,
                                   RefId = x.Key.RefId,
                               }).ToList();
                    var vrId = await PostAutoVoucher(lst, "STR", iss.TransId.ToString(), setupBL.GetWorkingDate(iss.ToLocId), UserId);
                    if (vrId > 0)
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
        public async Task<bool> StockAdjustmentOutVoucher(long TransId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    List<VoucherDetailVM> lst = new List<VoucherDetailVM>();
                    //var stockAdjustment = await GetAcc(464);
                    var vLst = await db.Inv_AdjOutDetail.Where(x => x.TransId == TransId).ToListAsync();
                    foreach (var a in vLst)
                    {
                        var v = await db.Inv_Store.FindAsync(a.ItemId);
                        if (v.PPrice > 0)
                        {
                            var invAcc = await GetAcc(7, v.Inv_Suppliers.CategoryId);
                            lst.Add(new VoucherDetailVM
                            {
                                AccId = invAcc,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = v.PPrice,
                                Dr = 0,
                                Particulars = "Stock Adjustment Out" + " Sr " + v.SerialNo,
                                PCCode = v.LocId,
                                SubId = 0,
                                RefId = v.ItemId
                            });
                            lst.Add(new VoucherDetailVM
                            {
                                AccId = (long)a.Inv_AdjType.AccId,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = v.PPrice,
                                Particulars = "Stock Adjustment Out",
                                PCCode = v.LocId,
                                SubId = 0,
                                RefId = 0
                            });
                        }
                    }
                    lst = lst.GroupBy(x => new { x.AccId, x.CCCode, x.ChequeNo, x.Particulars, x.PCCode, x.SubId, x.RefId }).Select(x =>
                               new VoucherDetailVM
                               {
                                   AccId = x.Key.AccId,
                                   CCCode = x.Key.CCCode,
                                   ChequeNo = x.Key.ChequeNo,
                                   Cr = x.Sum(a => a.Cr),
                                   Dr = x.Sum(a => a.Dr),
                                   Particulars = x.Key.Particulars,
                                   PCCode = x.Key.PCCode,
                                   SubId = x.Key.SubId,
                                   RefId = x.Key.RefId,
                               }).ToList();

                    if (lst.Sum(x => x.Cr) > 0)
                    {
                        var vrId = await PostAutoVoucher(lst, "IAV", TransId.ToString(), DateTime.Now.Date, UserId);
                        if (vrId == 0)
                        {
                            scop.Dispose();
                            return false;
                        }
                        else
                        {
                            if (await PostingLog("IAV", 54, TransId, vrId))
                            {
                                scop.Complete();
                            }
                        }
                        scop.Dispose();
                        return true;
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public async Task<bool> StockOpeningVoucher(List<Inv_Store> vLst, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    List<VoucherDetailVM> lst = new List<VoucherDetailVM>();
                    var stockAdjustment = await GetAcc(464);
                    foreach (var a in vLst)
                    {
                        var v = await db.Inv_Store.FindAsync(a.ItemId);
                        if (v.PPrice > 0)
                        {
                            var invAcc = await GetAcc(7, v.Inv_Suppliers.CategoryId);
                            lst.Add(new VoucherDetailVM
                            {
                                AccId = invAcc,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = v.PPrice,
                                Particulars = "Stock Adjustment " + " Sr " + v.SerialNo + (string.IsNullOrEmpty(v.CSerialNo) ? "" : " (" + v.CSerialNo + ")"),
                                PCCode = v.LocId,
                                SubId = 0,
                                RefId = v.ItemId
                            });
                            lst.Add(new VoucherDetailVM
                            {
                                AccId = stockAdjustment,
                                CCCode = v.LocId,
                                ChequeNo = "",
                                Cr = v.PPrice,
                                Dr = 0,
                                Particulars = "Stock Adjustment",
                                PCCode = v.LocId,
                                SubId = 0,
                                RefId = 0
                            });
                        }
                    }
                    lst = lst.GroupBy(x => new { x.AccId, x.CCCode, x.ChequeNo, x.Particulars, x.PCCode, x.SubId, x.RefId }).Select(x =>
                               new VoucherDetailVM
                               {
                                   AccId = x.Key.AccId,
                                   CCCode = x.Key.CCCode,
                                   ChequeNo = x.Key.ChequeNo,
                                   Cr = x.Sum(a => a.Cr),
                                   Dr = x.Sum(a => a.Dr),
                                   Particulars = x.Key.Particulars,
                                   PCCode = x.Key.PCCode,
                                   SubId = x.Key.SubId,
                                   RefId = x.Key.RefId,
                               }).ToList();

                    if (lst.Sum(x => x.Cr) > 0)
                    {
                        var vrId = await PostAutoVoucher(lst, "INV", "", DateTime.Now.Date, UserId);
                        if (vrId == 0)
                        {
                            scop.Dispose();
                        }
                        else
                        {
                            scop.Complete();
                            scop.Dispose();
                        }
                    }
                    else
                    {
                        scop.Complete();
                        scop.Dispose();
                    }

                    return true;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }
        public async Task<bool> CashTransferVoucher(List<long> TransLst, int UserId)
        {
            foreach (var v in TransLst)
            {
                using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var mod = await db.Lse_CashCollection.FindAsync(v);
                        if (mod.IsPosted)
                            continue;

                        List<VoucherDetailVM> lst = new List<VoucherDetailVM>();
                        var cashClearing = await GetAcc(450);
                        var cashInHand = await GetAcc(428);

                        if (mod.DocType == "Cashier")
                        {
                            var cashCollectionCenter = await db.Comp_Locations.Where(x => x.LocId == mod.LocId).Select(x => x.CashCenter).SingleOrDefaultAsync();
                            //var cashier = await db.Pay_EmpMaster.SingleOrDefaultAsync(x => x.EmpId == mod.CashierId);
                            if (mod.CashDeposit > 0)
                            {

                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClearing,
                                    CCCode = mod.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = mod.CashDeposit,
                                    Particulars = "Cash Deposit from " + mod.Comp_Locations.LocName + " to Cashier",
                                    PCCode = mod.LocId,
                                    SubId = 0,
                                    RefId = 0
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = mod.LocId,
                                    ChequeNo = "",
                                    Cr = mod.CashDeposit,
                                    Dr = 0,
                                    Particulars = "Cash Deposit from " + mod.Comp_Locations.LocName + " to Cashier",
                                    PCCode = mod.LocId,
                                    SubId = 0,
                                    RefId = 0
                                });

                                //////////////////////////////////////////////////////////////////////////////
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = cashCollectionCenter ?? 0,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = mod.CashDeposit,
                                    Particulars = "Cash Deposit from " + mod.Comp_Locations.LocName + " to Cashier",
                                    PCCode = cashCollectionCenter ?? 0,
                                    SubId = 0,
                                    RefId = 0
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClearing,
                                    CCCode = cashCollectionCenter ?? 0,
                                    ChequeNo = "",
                                    Cr = mod.CashDeposit,
                                    Dr = 0,
                                    Particulars = "Cash Deposit from " + mod.Comp_Locations.LocName + " to Cashier",
                                    PCCode = cashCollectionCenter ?? 0,
                                    SubId = 0,
                                    RefId = 0
                                });
                                var vrId = await PostAutoVoucher(lst, "CTR", mod.DocId.ToString(), mod.DocDate, UserId);
                                if (vrId > 0)
                                {
                                    mod.IsPosted = true;
                                    mod.PostedDate = DateTime.Now;
                                    mod.PostedBy = UserId;
                                    await db.SaveChangesAsync();
                                    if (await PostingLog("CTR", 51, mod.DocId, vrId))
                                    {
                                        scop.Complete();
                                    }
                                }
                                scop.Dispose();
                            }
                        }
                        else if (mod.DocType == "Bank")
                        {
                            if (mod.CashDeposit > 0)
                            {
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = mod.CashierId,
                                    CCCode = 72,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = mod.CashDeposit,
                                    Particulars = "Cash Deposit from " + mod.Comp_Locations.LocName + " to Bank",
                                    PCCode = 72,
                                    SubId = 0,
                                    RefId = 0
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClearing,
                                    CCCode = 72,
                                    ChequeNo = "",
                                    Cr = mod.CashDeposit,
                                    Dr = 0,
                                    Particulars = "Cash Deposit from " + mod.Comp_Locations.LocName + " to Bank",
                                    PCCode = 72,
                                    SubId = 0,
                                    RefId = 0
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClearing,
                                    CCCode = mod.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = mod.CashDeposit,
                                    Particulars = "Cash Deposit from " + mod.Comp_Locations.LocName + " to Bank",
                                    PCCode = mod.LocId,
                                    SubId = 0,
                                    RefId = 0
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = mod.LocId,
                                    ChequeNo = "",
                                    Cr = mod.CashDeposit,
                                    Dr = 0,
                                    Particulars = "Cash Deposit from " + mod.Comp_Locations.LocName + " to Bank",
                                    PCCode = mod.LocId,
                                    SubId = 0,
                                    RefId = 0
                                });

                                var vrId = await PostAutoVoucher(lst, "CPV", mod.DocId.ToString(), mod.DocDate, UserId);
                                if (vrId > 0)
                                {
                                    mod.IsPosted = true;
                                    mod.PostedDate = DateTime.Now;
                                    mod.PostedBy = UserId;
                                    await db.SaveChangesAsync();

                                    if (await PostingLog("CPV", 51, mod.DocId, vrId))
                                    {
                                        scop.Complete();
                                    }
                                }
                                scop.Dispose();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        scop.Dispose();
                    }
                }
            }


            return true;

        }
        #endregion

        #region CashTransfer
        public async Task<long> SaveCashTransfer(CashTransferVM mod, int UserId)
        {
            try
            {
                Lse_CashTransfer tbl = new Lse_CashTransfer();
                tbl.LocId = mod.LocId;
                tbl.ToLocId = mod.ToLocId;
                tbl.WorkingDate = mod.WorkingDate;
                tbl.TransferedCash = mod.TransferedCash;
                tbl.Status = "T";
                tbl.TransDate = DateTime.Now;
                tbl.UserId = UserId;
                tbl.Remarks = mod.Remarks;
                db.Lse_CashTransfer.Add(tbl);
                await db.SaveChangesAsync();
                return tbl.TransId;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<List<CashTransferVM>> GetCashTransfer(int LocId)
        {
            try
            {
                return await (from C in db.Lse_CashTransfer
                              join L in db.Comp_Locations on C.LocId equals L.LocId
                              where C.ToLocId == LocId && C.Status == "T"
                              select
                 new CashTransferVM
                 {
                     TransId = C.TransId,
                     Location = L.LocName,
                     TransferedCash = C.TransferedCash,
                     Remarks = C.Remarks,
                     Status = C.Status
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> SaveCashReceive(CashTransferVM item, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (item.Status == "R" || item.Status == "C")
                    {
                        var tbl = await db.Lse_CashTransfer.FindAsync(item.TransId);
                        if (tbl.Status == "T")
                        {
                            tbl.ReceivedBy = UserId;
                            tbl.ReceivedDate = DateTime.Now;
                            tbl.Status = item.Status;
                            if (item.Status == "C")
                            {
                                await db.SaveChangesAsync();
                                scop.Complete();
                            }
                            else
                            {
                                var fLoc = await db.Comp_Locations.FindAsync(tbl.LocId);
                                var tLoc = await db.Comp_Locations.FindAsync(tbl.ToLocId);
                                var cashClearing = await GetAcc(450);
                                var cashInHand = await GetAcc(428);

                                List<VoucherDetailVM> lst = new List<VoucherDetailVM>();
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClearing,
                                    CCCode = tbl.LocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = tbl.TransferedCash,
                                    Particulars = "Cash Deposit from " + fLoc.LocName + " to " + tLoc.LocName,
                                    PCCode = tbl.LocId,
                                    SubId = 0,
                                    RefId = 0
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = tbl.LocId,
                                    ChequeNo = "",
                                    Cr = tbl.TransferedCash,
                                    Dr = 0,
                                    Particulars = "Cash Deposit from " + fLoc.LocName + " to " + tLoc.LocName,
                                    PCCode = tbl.LocId,
                                    SubId = 0,
                                    RefId = 0
                                });

                                //////////////////////////////////////////////////////////////////////////////
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashInHand,
                                    CCCode = tbl.ToLocId,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = tbl.TransferedCash,
                                    Particulars = "Cash Deposit from " + fLoc.LocName + " to " + tLoc.LocName,
                                    PCCode = tbl.ToLocId,
                                    SubId = 0,
                                    RefId = 0
                                });
                                lst.Add(new VoucherDetailVM
                                {
                                    AccId = cashClearing,
                                    CCCode = tbl.ToLocId,
                                    ChequeNo = "",
                                    Cr = tbl.TransferedCash,
                                    Dr = 0,
                                    Particulars = "Cash Deposit from " + fLoc.LocName + " to " + tLoc.LocName,
                                    PCCode = tbl.ToLocId,
                                    SubId = 0,
                                    RefId = 0
                                });
                                var vrId = await PostAutoVoucher(lst, "CTR", tbl.TransId.ToString(), DateTime.Now.Date, UserId);
                                if (vrId > 0)
                                {
                                    tbl.IsPosted = true;
                                    tbl.PostedDate = DateTime.Now;
                                    tbl.PostedBy = UserId;
                                    await db.SaveChangesAsync();
                                    if (await PostingLog("CTR", 55, tbl.TransId, vrId))
                                    {
                                        scop.Complete();
                                    }
                                }
                                scop.Dispose();
                            }

                        }
                    }
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

        #region PostingVouchers
        public async Task<bool> PostingLog(string VrTypeId, int TransTypeId, long TransId, long VrId)
        {
            try
            {
                db.Fin_VoucherPostingLog.Add(new Fin_VoucherPostingLog
                {
                    VrTypeId = VrTypeId,
                    TransId = TransId,
                    TransTypeId = TransTypeId,
                    VrId = VrId
                });
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<long> PostAutoVoucher(IEnumerable<VoucherDetailVM> mod, string VType, string RefDocNo, DateTime RefDocDate, int UserId)
        {
            try
            {
                if (mod.Sum(x => x.Cr) != mod.Sum(x => x.Dr))
                    return 0;

                int CYear = (await this.CYear(RefDocDate)).YrCode;
                int CPeriod = (await this.CPeriod(RefDocDate)).PrCode;
                string VNo = await this.VCounter(VType, CPeriod, CYear, RefDocDate);
                if (VNo == "")
                {
                    return 0;
                }



                Fin_Voucher mas = new Fin_Voucher()
                {
                    CreatedBy = UserId,
                    CreatedDate = DateTime.Now,
                    CheckedBy = UserId,
                    CheckedDate = DateTime.Now,
                    ApprovedBy = UserId,
                    ApprovedDate = DateTime.Now,
                    PrCode = CPeriod,
                    RefDocDate = RefDocDate,
                    RefDocNo = RefDocNo,
                    VrDate = RefDocDate,
                    VrNo = VNo,
                    VrStatus = "P",
                    VrTypeId = VType,
                    YrCode = CYear,
                    Remarks = "Auto Voucher"
                };
                List<Fin_VoucherDetail> lst = new List<Fin_VoucherDetail>();
                int i = 1;
                foreach (var v in mod)
                {
                    if (v.Cr > 0 || v.Dr > 0)
                    {
                        lst.Add(new Fin_VoucherDetail
                        {
                            Cr = v.Cr,
                            Dr = v.Dr,
                            CCCode = v.CCCode,
                            Particulars = v.Particulars,
                            PCCode = v.PCCode,
                            AccId = v.AccId,
                            SubId = v.SubId == 0 ? (long?)null : v.SubId,
                            SubDivId = getSubDivId(v.SubsidaryCode),
                            RefId = v.RefId,
                            TrxSeqId = i++
                        });
                    }
                }

                var pc = lst.Select(x => x.PCCode).Distinct().ToList();
                foreach (var v in pc)
                {
                    var dif = lst.Where(x => x.PCCode == v).Sum(x => x.Dr - x.Cr);
                    if (dif != 0)
                    {
                        var clearingAcc = await GetAcc(459);
                        lst.Add(new Fin_VoucherDetail
                        {
                            Cr = dif > 0 ? dif : 0,
                            Dr = dif < 0 ? (dif * -1) : 0,
                            CCCode = v,
                            Particulars = "Clearing",
                            PCCode = v,
                            AccId = clearingAcc,
                            SubId = 0,
                            TrxSeqId = i++
                        });
                    }
                }

                if (lst.Count() == 0)
                {
                    return 0;
                }

                mas.Fin_VoucherDetail = lst;
                db.Fin_Voucher.Add(mas);
                await db.SaveChangesAsync();
                return mas.VrId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        #endregion
    }
}