using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;

namespace AGEERP.Models
{
    public class SaleBL
    {
        AGEEntities db = new AGEEntities();
        AccountBL accountBL = new AccountBL();
        SetupBL setupBL = new SetupBL();

        #region CustomerComments
        public async Task<List<LseMasterVM>> AccountsByLoc(int LocId)
        {
            try
            {
                return await db.Lse_Master.Where(x => x.LocId == LocId && x.Status == 3).Select(x => new LseMasterVM
                {
                    AccNo = x.AccNo,
                    DeliveryDate = (DateTime)x.DeliveryDate,
                    CustName = x.CustName,
                    Mobile1 = x.Mobile1,
                    NIC = x.NIC,
                    Remarks = x.Remarks
                }).ToListAsync();
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<bool> CommentUpdate(long Accno, string Remarks, int UserId)
        {
            try
            {
                var acc = await db.Lse_Master.Where(x => x.AccNo == Accno).FirstOrDefaultAsync();
                if (acc != null)
                {
                    acc.Remarks = Remarks;
                    await db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        #endregion

        #region LeaseReturn
        public async Task<SKUItemVM> GetProductByAcc(long AccNo)
        {
            try
            {
                return await db.Lse_Detail.Where(x => x.AccNo == AccNo && x.InstPrice > 0).Select(x => new SKUItemVM()
                {
                    SKUId = x.SKUId ?? 0,
                    SKUCode = x.SKUId == null ? x.OldModel : x.Itm_Master.SKUCode,
                    ComName = x.SKUId == null ? x.OldCompany : x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                    ProductName = x.SKUId == null ? x.OldProduct : x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                    ItemId = x.ItemId
                }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SaleTargetTypeVM>> GetSaleTargetTypeVM()
        {
            try
            {
                return db.Lse_SaleTargetTypes.Select(x => new SaleTargetTypeVM()
                {
                    TargetType = x.TargetType,
                    TargetTypeId = x.TargetTypeId
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        //public async Task<bool> SaveSameProductReturn(IEnumerable<LseDetailVM> mod, ProcessingVM data, int UserId)
        //{
        //    try
        //    {
        //        //Get the Current Working Date
        //        var workingdte = await GetWorkingDate(data.LocId);
        //        foreach (var item in mod)
        //        {
        //            var OldItem = await db.Inv_Store.Where(x => x.SerialNo == item.SerialNo).FirstOrDefaultAsync();
        //            var LseDetail = await db.Lse_Detail.Where(x => x.AccNo == data.AccNo && x.ItemId == OldItem.ItemId).FirstOrDefaultAsync();
        //            LseDetail.IsReturned = true;

        //            await db.SaveChangesAsync();


        //            var NewItem = await db.Inv_Store.Where(x => x.SerialNo == item.CSerialNo).FirstOrDefaultAsync();
        //            var NewRec = LseDetail;
        //            NewRec.IsReturned = false;
        //            NewRec.ItemId = NewItem.ItemId;

        //            db.Lse_Detail.Add(NewRec);

        //            NewItem.StatusID = 2;
        //            OldItem.StatusID = (data.Type == "P" ? 1 : 10);
        //            await db.SaveChangesAsync();


        //            Lse_Return ret = new Lse_Return()
        //            {
        //                AccNo = data.AccNo,
        //                Amount = item.InstPrice,
        //                IsPosted = false,
        //                WorkingDate = Convert.ToDateTime(workingdte),
        //                UserId = UserId,
        //                TransDate = DateTime.Now,
        //                ItemType = data.Type,
        //                ReturnTypeId = data.ReturnTypeId,
        //                LocId = data.LocId,
        //                ItemId = OldItem.ItemId
        //            };
        //            db.Lse_Return.Add(ret);
        //            await db.SaveChangesAsync();
        //        }
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        public async Task<decimal> GetOutStandAmount(long AccNo)
        {
            try
            {
                var lsemaster = await db.Lse_Master.Where(x => x.AccNo == AccNo).FirstOrDefaultAsync();
                //Getting the Total Installment Paid
                var TotalInstallment = await db.Lse_Installment.Where(x => x.AccNo == AccNo).ToListAsync();
                //Getting the Installment Amount
                var InstallmetPrice = lsemaster.InstPrice;
                //Getting the Total Advance Paid
                var AdvancePaid = lsemaster.Advance;

                var TotalAmt = (InstallmetPrice - AdvancePaid) - TotalInstallment.Sum(x => x.InstCharges + x.Discount);
                return TotalAmt;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public async Task<ResultVM> ProductChange(IEnumerable<LseDetailVM> mod, ProcessingVM data, int UserId)
        {
            ResultVM result = new ResultVM();
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //Get the Current Working Date
                    DateTime workingdte = Convert.ToDateTime(await GetWorkingDate(data.LocId));

                    //Getting the Current Lease Record
                    var CurrentAcc = db.Lse_Master.Where(x => x.AccNo == data.AccNo && x.LocId == data.LocId).FirstOrDefault();
                    if (CurrentAcc != null)
                    {
                        if (CurrentAcc.Type == "R")
                        {
                            scop.Dispose();
                            result.Msg = "Already Returned";
                            return result;
                        }
                        //if (data.Type == "P")
                        //{
                        //    var instExist = await db.Lse_Installment.AnyAsync(x => x.AccNo == data.AccNo);
                        //    if (instExist)
                        //    {
                        //        scop.Dispose();
                        //        result.Msg = "Installment Exists cannot be Packed Return";
                        //        return result;
                        //    }
                        //}

                        List<Lse_Detail> lsedet = new List<Lse_Detail>();

                        //Close the Current Lease Account And Open the New Account
                        CurrentAcc.CloseDate = workingdte;
                        CurrentAcc.CloseTransDate = DateTime.Now;
                        CurrentAcc.Status = 4;
                        CurrentAcc.Type = "R";
                        await db.SaveChangesAsync();

                        if (data.AICReturn)
                        {
                            var instLst = await db.Lse_Installment.Where(x => x.AccNo == data.AccNo && x.PaidBy == 3).ToListAsync();
                            foreach (var item in instLst)
                            {
                                db.Lse_Installment.Add(new Lse_Installment
                                {
                                    InstDate = workingdte,
                                    AccNo = item.AccNo,
                                    Discount = item.Discount,
                                    Fine = item.Fine,
                                    FineType = item.FineType,
                                    InstCharges = item.InstCharges * -1,
                                    RecoveryId = item.RecoveryId,
                                    Remarks = item.Remarks,
                                    LocId = item.LocId,
                                    UserId = UserId,
                                    TransDate = DateTime.Now,
                                    PaidBy = item.PaidBy,
                                    IsAIC = item.IsAIC
                                });
                                DateTime eom = new DateTime(workingdte.Year, workingdte.Month, DateTime.DaysInMonth(workingdte.Year, workingdte.Month));
                                db.Pay_AICManager.Add(new Pay_AICManager
                                {
                                    AicMonth = eom,
                                    Amount = item.InstCharges,
                                    DefinedBy = UserId,
                                    DefinedDate = DateTime.Now,
                                    IsPosted = false,
                                    LocId = item.LocId,
                                    Remarks = "AIC Returned"
                                });
                            }
                            await db.SaveChangesAsync();
                        }

                        var outstand = await db.Lse_Outstand.Where(x => x.AccNo == data.AccNo && x.OutstandDate.Month == workingdte.Month && x.OutstandDate.Year == workingdte.Year).FirstOrDefaultAsync();
                        if (outstand != null)
                        {
                            outstand.RecvAmt = outstand.OutstandAmt;
                            outstand.InstDate = workingdte;
                            outstand.Status = "C";
                        }
                        var outstandAmt = await GetOutStandAmount(data.AccNo);
                        if ((CurrentAcc.InstPrice - outstandAmt) < data.ReturnAmount)
                        {
                            scop.Dispose();
                            result.Msg = "Return Amount should be less than Returnable Amount";
                            return result;
                        }
                        if (data.Type != "P")
                        {
                            outstandAmt = outstandAmt + data.ReturnAmount;
                        }
                        //Get the Current Lease Detail Records By Account # and Update the Item Returned For Packing Status Id = 1 & for Opened Status Id=10
                        var lsedetail = await db.Lse_Detail.Where(x => x.AccNo == data.AccNo).ToListAsync();
                        foreach (var item in lsedetail)
                        {
                            item.IsReturned = true;
                            var Invstr = await db.Inv_Store.Where(x => x.ItemId == item.ItemId).FirstOrDefaultAsync();
                            Invstr.IncExempt = data.IncExempt;
                            if (data.Type == "P")
                            {
                                Invstr.StatusID = 1;
                            }
                            else
                            {
                                Invstr.StatusID = 10;
                                if (!Invstr.Itm_Master.IsPair)
                                {
                                    //var outstandAmt = await GetOutStandAmount(data.AccNo);
                                    Invstr.PPrice = outstandAmt;
                                    Invstr.MRP = outstandAmt;
                                }
                                Invstr.Exempted = true;
                                Invstr.Tax = 0;
                            }
                            await db.SaveChangesAsync();
                        }

                        //Creating the New Lease Account in Master While Copying the Information for Previous Object

                        Lse_Master lsemaster = new Lse_Master
                        {
                            ProcessTransDate = DateTime.Now,
                            FName = CurrentAcc.FName,
                            ManagerId = CurrentAcc.ManagerId,
                            MktOfficerId = CurrentAcc.MktOfficerId,
                            Advance = data.Advance,
                            CustName = CurrentAcc.CustName,
                            Duration = data.Duration,
                            InqOfficerId = CurrentAcc.InqOfficerId,
                            SManagerId = CurrentAcc.SManagerId,
                            RMId = CurrentAcc.RMId,
                            SRMId = CurrentAcc.SRMId,
                            InstPrice = data.InstPrice,
                            LocId = data.LocId,
                            OldAccNo = CurrentAcc.OldAccNo,
                            Mobile1 = CurrentAcc.Mobile1,
                            Mobile2 = CurrentAcc.Mobile2,
                            DeliveryDate = Convert.ToDateTime(workingdte),
                            DeliveryTransDate = DateTime.Now,
                            //ModelId = mod.ModelId,
                            MonthlyInst = Math.Round((data.InstPrice - data.ActualAdvance) / (data.Duration - 1)),
                            ActualAdvance = data.ActualAdvance,
                            NIC = CurrentAcc.NIC,
                            ProcessAt = CurrentAcc.ProcessAt,
                            ProcessDate = Convert.ToDateTime(workingdte),
                            ProcessFee = data.ProcessFee,
                            Remarks = CurrentAcc.Remarks,
                            Status = 3,
                            UserId = UserId,
                            CategoryId = data.CategoryId,
                            Type = "N"
                            //PlanId = mod.Duration

                        };
                        //lsemaster.AccNo = 0;
                        db.Lse_Master.Add(lsemaster);
                        await db.SaveChangesAsync();

                        //Creating the New Lease Details 
                        foreach (var v in mod)
                        {
                            var item = await db.Inv_Store.FindAsync(v.ItemId);
                            int prevStatus = item.StatusID;
                            item.SPrice = v.InstPrice;
                            item.StatusID = 2;
                            decimal basePrice = 0;
                            if (v.PlanType == "S")
                            {
                                var pri = await db.Itm_SerialPlan.FirstOrDefaultAsync(x => x.RowId == v.InstPlanId);
                                basePrice = pri == null ? 0 : pri.BasePrice;
                            }
                            else if (v.PlanType == "M")
                            {
                                var pri = await db.Itm_SKUPlan.FirstOrDefaultAsync(x => x.RowId == v.InstPlanId);
                                basePrice = pri == null ? 0 : pri.BasePrice;
                            }
                            var prii = await db.Itm_SKUPlan.FindAsync(v.InstPlanId);

                            db.Lse_Detail.Add(new Lse_Detail
                            {
                                AccNo = lsemaster.AccNo,
                                IsReturned = false,
                                InstPrice = v.InstPrice,
                                Qty = 1,
                                SKUId = item.SKUId,
                                Status = true,
                                ItemId = item.ItemId,
                                Discount = v.Discount,
                                MRP = item.MRP,
                                SM = basePrice,
                                PPrice = item.PPrice,
                                InstPlanId = v.InstPlanId,
                                dAdvance = v.dAdvance,
                                dInst = v.dInst,
                                Tax = Math.Round(item.MRP - (item.MRP * 100 / ((item.Inv_Suppliers.GST ?? 0) + 100))),
                                dDate = Convert.ToDateTime(workingdte),
                                PlanType = v.PlanType,
                                PrevStatus = prevStatus
                            });
                            if (prii != null)
                            {
                                if (prii.Type == "S" || prii.Type == "A")
                                {
                                    prii.ModifiedBy = UserId;
                                    prii.ModifiedDate = DateTime.Now;
                                    prii.Status = false;
                                    prii.Remarks = (prii.Remarks ?? "") + " Auto Close";
                                }
                            }
                            //if (v.PlanType == "S" && v.InstPlanId > 0)
                            //{
                            //    var plan = await db.Itm_SerialPlan.FirstOrDefaultAsync(x => x.RowId == v.InstPlanId);
                            //    if (plan != null)
                            //    {
                            //        plan.ModifiedBy = UserId;
                            //        plan.ModifiedDate = DateTime.Now;
                            //        plan.Status = false;
                            //        plan.Remarks = (plan.Remarks ?? "") + " Auto Close";
                            //    }
                            //}
                        }
                        await db.SaveChangesAsync();

                        //Getting the Previous Customer Mapped to the Lse Master
                        var PrevCustomer = await db.Lse_Customer.Where(x => x.AccNo == data.AccNo).FirstOrDefaultAsync();
                        //Updaing the Previous Lse Customer to the New Lse Master
                        if (PrevCustomer != null)
                        {
                            PrevCustomer.AccNo = lsemaster.AccNo;
                            //PrevCustomer.CustId = (long?)null;
                            //db.Lse_Customer.Add(PrevCustomer);
                            await db.SaveChangesAsync();
                        }

                        //Getting the Previous Cheques Mapped to the Lse Master
                        var PrevCheque = await db.Lse_Cheque.Where(x => x.AccNo == data.AccNo).ToListAsync();
                        //Updaing the Previous Lse Cheque to the New Lse Master
                        if (PrevCheque.Count > 0)
                        {
                            foreach (var item in PrevCheque)
                            {
                                item.AccNo = lsemaster.AccNo;
                                //item.ChequeId = 0;
                                //db.Lse_Cheque.Add(item);
                            }
                            await db.SaveChangesAsync();
                        }
                        //Getting the Gurrantor Mapped to the Lse Master
                        var PrevGurantor = await db.Lse_Guarantor.Where(x => x.AccNo == data.AccNo).ToListAsync();
                        //Updaing the Previous Lse Gurrantor to the New Lse Master
                        if (PrevGurantor.Count > 0)
                        {
                            foreach (var item in PrevGurantor)
                            {
                                item.AccNo = lsemaster.AccNo;
                                //item.GuarantorId = 0;
                                //db.Lse_Guarantor.Add(item);
                            }
                            await db.SaveChangesAsync();
                        }

                        //Getting the Customer Thumb to the Lse Master
                        var CustomerThumb = await db.Lse_CustomerTemplate.Where(x => x.AccNo == data.AccNo).FirstOrDefaultAsync();
                        //Updaing the Previous Lse Gurrantor to the New Lse Master
                        if (CustomerThumb != null)
                        {
                            CustomerThumb.AccNo = lsemaster.AccNo;
                            //CustomerThumb.RowId = 0;
                            //db.Lse_CustomerTemplate.Add(CustomerThumb);
                            await db.SaveChangesAsync();
                        }

                        //Getting the Lse Customer Documents
                        var CustomerDocs = await db.Comp_Documents.Where(x => x.RefObjId == data.AccNo && x.DocTypeId == 3).ToListAsync();
                        if (CustomerDocs.Count > 0)
                        {
                            foreach (var item in CustomerDocs)
                            {
                                item.RefObjId = lsemaster.AccNo;
                                item.Modified = DateTime.Now;
                                //item.DocId = 0;
                                //db.Comp_Documents.Add(item);
                            }
                            await db.SaveChangesAsync();
                        }

                        //Add Transaction to Lease Return
                        Lse_Return ret = new Lse_Return()
                        {
                            AccNo = data.AccNo,
                            Amount = (data.Type == "P" ? CurrentAcc.InstPrice : outstandAmt),
                            IsPosted = false,
                            WorkingDate = Convert.ToDateTime(workingdte),
                            UserId = UserId,
                            TransDate = DateTime.Now,
                            ItemType = data.Type,
                            ReturnTypeId = data.ReturnTypeId,
                            LocId = data.LocId,
                            NewAccNo = lsemaster.AccNo,
                            Remarks = data.Remarks,
                            ReturnAmount = data.ReturnAmount,
                            ReasonId = data.ReasonId,
                            AICReturn = data.AICReturn,
                            IncExempt = data.IncExempt
                        };
                        db.Lse_Return.Add(ret);
                        await db.SaveChangesAsync();

                        if (data.files != null && data.files.Count > 0)
                        {
                            await new DocumentBL().UpdateDocRef(data.files, ret.TransId);
                        }

                        scop.Complete();
                        scop.Dispose();
                        result.TransId = ret.TransId;
                        result.AccNo = lsemaster.AccNo;
                        result.Msg = "Save Successfully";
                        return result;
                    }
                    else
                    {
                        result.Msg = "Account not found";
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    result.Msg = "Error";
                    return result;
                }
            }
        }
        public async Task<ResultVM> SavePTO(long AccNo, int LocId, int SKUId, decimal InstPrice, int UserId, string Type, int ReturnTypeId, long ItemId, List<long> files, string Remarks, decimal ReturnAmount, int ReasonId, bool IncExempt, bool AICReturn)
        {
            ResultVM result = new ResultVM();
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    //Get the Current Working Date
                    DateTime workingdte = Convert.ToDateTime(await GetWorkingDate(LocId));
                    var CurrentAcc = db.Lse_Master.Where(x => x.AccNo == AccNo && x.LocId == LocId).FirstOrDefault();
                    if (CurrentAcc != null)
                    {
                        if (CurrentAcc.Type == "R")
                        {
                            scop.Dispose();
                            result.Msg = "Already Returned";
                            return result;
                        }
                        //if (Type == "P")
                        //{
                        //    var instExist = await db.Lse_Installment.AnyAsync(x => x.AccNo == AccNo);
                        //    if (instExist)
                        //    {
                        //        scop.Dispose();
                        //        result.Msg = "Installment Exists cannot be Packed Return";
                        //        return result;
                        //    }
                        //}
                        CurrentAcc.CloseDate = workingdte;
                        CurrentAcc.CloseTransDate = DateTime.Now;
                        CurrentAcc.Status = 4;
                        CurrentAcc.Type = "R";
                        await db.SaveChangesAsync();

                        //var Invstr = await db.Inv_Store.Where(x => x.ItemId == ItemId).ToListAsync();
                        //foreach (var item in Invstr)
                        //{
                        //    if (item != null)
                        //    {
                        //        if (Type == "P")
                        //        {
                        //            item.StatusID = 1;
                        //        }
                        //        else
                        //        {
                        //            item.StatusID = 10;
                        //            var outstandAmt = await GetOutStandAmount(AccNo);
                        //            item.PPrice = outstandAmt;
                        //            item.MRP = outstandAmt;
                        //            item.Exempted = true;
                        //            item.Tax = 0;
                        //        }
                        //        await db.SaveChangesAsync();
                        //    }
                        //}

                        //var instLst = await db.Lse_Installment.Where(x => x.AccNo == AccNo && x.PaidBy == 3).ToListAsync();
                        //foreach (var x in instLst)
                        //{
                        //    db.Lse_Installment.Add(new Lse_Installment
                        //    {
                        //        AccNo = x.AccNo,
                        //        Discount = x.Discount,
                        //        Fine = x.Fine,
                        //        FineType = x.FineType,
                        //        InstCharges = x.InstCharges * -1,
                        //        InstDate = workingdte,
                        //        IsAIC = x.IsAIC,
                        //        IsLock = x.IsLock,
                        //        LocId = x.LocId,
                        //        PaidBy = x.PaidBy,
                        //        RecoveryId = x.RecoveryId,
                        //        Remarks = x.Remarks,
                        //        TransDate = DateTime.Now,
                        //        UserId = UserId,
                        //        IsPosted = false
                        //    });
                        //}
                        //await db.SaveChangesAsync();
                        //if (Type != "P")
                        //{
                        if (AICReturn)
                        {
                            var instLst = await db.Lse_Installment.Where(x => x.AccNo == AccNo && x.PaidBy == 3).ToListAsync();
                            foreach (var item in instLst)
                            {
                                db.Lse_Installment.Add(new Lse_Installment
                                {
                                    InstDate = workingdte,
                                    AccNo = item.AccNo,
                                    Discount = item.Discount,
                                    Fine = item.Fine,
                                    FineType = item.FineType,
                                    InstCharges = item.InstCharges * -1,
                                    RecoveryId = item.RecoveryId,
                                    Remarks = item.Remarks,
                                    LocId = item.LocId,
                                    UserId = UserId,
                                    TransDate = DateTime.Now,
                                    PaidBy = item.PaidBy,
                                    IsAIC = item.IsAIC
                                });
                                DateTime eom = new DateTime(workingdte.Year, workingdte.Month, DateTime.DaysInMonth(workingdte.Year, workingdte.Month));
                                db.Pay_AICManager.Add(new Pay_AICManager
                                {
                                    AicMonth = eom,
                                    Amount = item.InstCharges,
                                    DefinedBy = UserId,
                                    DefinedDate = DateTime.Now,
                                    IsPosted = false,
                                    LocId = item.LocId,
                                    Remarks = "AIC Returned"
                                });
                            }
                            await db.SaveChangesAsync();
                        }

                        var outstand = await db.Lse_Outstand.Where(x => x.AccNo == AccNo && x.OutstandDate.Month == workingdte.Month && x.OutstandDate.Year == workingdte.Year).FirstOrDefaultAsync();
                        if (outstand != null)
                        {
                            outstand.RecvAmt = outstand.OutstandAmt;
                            outstand.InstDate = workingdte;
                            outstand.Status = "C";
                        }
                        var outstandAmt = await GetOutStandAmount(AccNo);
                        if (CurrentAcc.InstPrice - outstandAmt < ReturnAmount)
                        {
                            scop.Dispose();
                            result.Msg = "Return Amount should be less than Returnable Amount";
                            return result;
                        }

                        if (Type != "P")
                        {
                            outstandAmt = outstandAmt + ReturnAmount;
                        }
                        var lsedetail = await db.Lse_Detail.Where(x => x.AccNo == AccNo).ToListAsync();
                        foreach (var item in lsedetail)
                        {
                            item.IsReturned = true;
                            var Invstr = await db.Inv_Store.Where(x => x.ItemId == item.ItemId).FirstOrDefaultAsync();
                            Invstr.IncExempt = IncExempt;
                            if (Type == "P")
                            {
                                Invstr.StatusID = 1;
                            }
                            else
                            {
                                Invstr.StatusID = 10;
                                if (!Invstr.Itm_Master.IsPair)
                                {
                                    //var outstandAmt = await GetOutStandAmount(AccNo);
                                    Invstr.PPrice = outstandAmt;
                                    Invstr.MRP = outstandAmt;
                                }
                                Invstr.Exempted = true;
                                Invstr.Tax = 0;
                            }
                            await db.SaveChangesAsync();
                        }

                        //Add Transaction to Lease Return
                        Lse_Return ret = new Lse_Return()
                        {
                            AccNo = AccNo,
                            Amount = (Type == "P" ? CurrentAcc.InstPrice : outstandAmt),
                            ReturnAmount = ReturnAmount,
                            IsPosted = false,
                            WorkingDate = workingdte,
                            UserId = UserId,
                            TransDate = DateTime.Now,
                            ItemType = Type,
                            ReturnTypeId = ReturnTypeId,
                            LocId = LocId,
                            Remarks = Remarks,
                            ReasonId = ReasonId,
                            AICReturn = AICReturn,
                            IncExempt = IncExempt
                        };
                        db.Lse_Return.Add(ret);
                        await db.SaveChangesAsync();

                        if (files != null && files.Count > 0)
                        {
                            await new DocumentBL().UpdateDocRef(files, ret.TransId);
                        }

                        scop.Complete();
                        scop.Dispose();
                        result.TransId = ret.TransId;
                        result.Msg = "Save Successfully";
                        return result;
                    }
                    else
                    {
                        result.Msg = "Account not found";
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    result.Msg = "Error";
                    return result;
                }
            }
        }

        public async Task<List<ReasonVM>> GetReturnReasonList(string Type)
        {
            try
            {
                return await db.Lse_ReturnReason.Where(x => x.Status && x.RType == Type).Select(x => new ReasonVM { Reason = x.Reason, ReasonId = x.ReasonId }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region InstallmentLock
        public async Task<bool> InstallmentLock(long inst)
        {
            try
            {
                var Inst = await db.Lse_Installment.Where(x => x.InstId == inst).FirstOrDefaultAsync();
                Inst.IsLock = true;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }

        public async Task<bool> InstallmentUnLock(long inst)
        {
            try
            {
                var Inst = await db.Lse_Installment.Where(x => x.InstId == inst).FirstOrDefaultAsync();
                Inst.IsLock = false;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }

        public async Task<bool> AccountLock(long AccNo)
        {
            try
            {
                var Inst = await db.Lse_Master.Where(x => x.AccNo == AccNo).FirstOrDefaultAsync();
                Inst.IsLock = true;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }

        public async Task<bool> AccounUnLock(long AccNo)
        {
            try
            {
                var Inst = await db.Lse_Master.Where(x => x.AccNo == AccNo).FirstOrDefaultAsync();
                Inst.IsLock = false;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        public async Task<AccountsVM> GetAccountDetail(long accno)
        {
            try
            {
                return await db.Lse_Master.Where(x => x.AccNo == accno).Select(x => new AccountsVM()
                {
                    IsLocked = x.IsLock
                }).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<List<LseAccountVM>> GetLockedAccounts()
        {
            try
            {
                return await db.Lse_Master.Where(x => x.IsLock).Select(x => new LseAccountVM()
                {
                    AccNo = x.AccNo,
                    InstPrice = x.InstPrice,
                    IsLocked = x.IsLock
                }).ToListAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<InstallmentVM>> GetExistLockInstallment(long accno)
        {
            try
            {
                var Installments = await db.Lse_Installment.Where(x => x.AccNo == accno && x.IsLock).Select(x => new InstallmentVM()
                {
                    AccNo = x.AccNo,
                    InstCharges = x.InstCharges,
                    InstId = x.InstId,
                    IsLock = x.IsLock
                }).ToListAsync();
                return Installments;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<SelectListItem>> PaidByList()
        {
            try
            {
                return await db.Lse_PaidBy.Where(x => x.Status).Select(x => new SelectListItem()
                {
                    Text = x.Title,
                    Value = x.RowId.ToString()
                }).ToListAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<InstallmentVM>> GetInstallmentLog(long accno)
        {
            try
            {
                var Installments = await db.Lse_Installment.Where(x => x.AccNo == accno).Select(x => new InstallmentVM()
                {
                    AccNo = x.AccNo,
                    InstCharges = x.InstCharges,
                    InstId = x.InstId,
                    IsLock = x.IsLock,
                    InstDate = x.InstDate
                }).ToListAsync();
                return Installments;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        #endregion

        #region Customer
        public async Task<List<CustomerVM>> CustomerList()
        {
            try
            {
                return await db.Inv_Customers.Where(x => x.Status).Select(x =>
                new CustomerVM
                {
                    CustId = x.CustId,
                    CustName = x.CustName
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> CheckBlock(string str)
        {
            try
            {
                var tbl = await db.Lse_BlockCustomer.Where(x => (x.CNIC == str || x.MobileNo1 == str || x.MobileNo2 == str) && x.Status).Select(x => x.CustomerName).FirstOrDefaultAsync();
                if (tbl != null)
                {
                    return tbl + " Blocked";
                }
                else
                {
                    return "OK";
                }
            }
            catch (Exception)
            {
                return "Error";
            }
        }
        public async Task<BlockCustomerVM> CreateBlockCustomer(BlockCustomerVM mod, int UserId)
        {
            try
            {
                var obj = new Lse_BlockCustomer
                {
                    CNIC = mod.CNIC,
                    CustomerName = mod.CustomerName,
                    MobileNo1 = mod.MobileNo1,
                    MobileNo2 = mod.MobileNo2,
                    CareOf = mod.CareOf,
                    Remarks = mod.Remarks,
                    City = mod.City,
                    Location = mod.Location,
                    Status = mod.Status,
                    TransDate = DateTime.Now
                };
                db.Lse_BlockCustomer.Add(obj);
                await db.SaveChangesAsync();
                mod.RowId = obj.RowId;
                return mod;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return null;
            }
        }
        public async Task<bool> UpdateBlockCustomer(BlockCustomerVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_BlockCustomer.SingleOrDefaultAsync(x => x.RowId.Equals(mod.RowId));
                if (tbl != null)
                {
                    tbl.CNIC = mod.CNIC;
                    tbl.CustomerName = mod.CustomerName;
                    tbl.MobileNo1 = mod.MobileNo1;
                    tbl.MobileNo1 = mod.MobileNo1;
                    tbl.CareOf = mod.CareOf;
                    tbl.City = mod.City;
                    tbl.Location = mod.Location;
                    tbl.TransDate = DateTime.Now;
                    tbl.Status = mod.Status;
                    tbl.Remarks = mod.Remarks;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        public async Task<List<BlockCustomerVM>> BlockCustomerList()
        {
            try
            {
                return await db.Lse_BlockCustomer.Select(x =>
                new BlockCustomerVM
                {
                    RowId = x.RowId,
                    CustomerName = x.CustomerName,
                    CNIC = x.CNIC,
                    MobileNo1 = x.MobileNo1,
                    MobileNo2 = x.MobileNo2,
                    CareOf = x.CareOf,
                    Remarks = x.Remarks,
                    City = x.City,
                    Location = x.Location,
                    Status = x.Status
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<CustomerDetailVM>> CustomerDetailList()
        {
            try
            {
                return await db.Inv_Customers.Where(x => x.Status).Select(x =>
                new CustomerDetailVM
                {
                    CustId = x.CustId,
                    CustName = x.CustName,
                    Address = x.Address,
                    Email = x.Email,
                    GLCode = x.GLCode,
                    Mobile = x.Mobile,
                    AccountTitle = x.AccountTitle,
                    BankName = x.BankName,
                    BusinessRelation = x.BusinessRelation,
                    ChequeNo = x.ChequeNo,
                    DaysLimit = x.DaysLimit,
                    Remarks = x.Remarks,
                    CNIC = x.CNIC,
                    NTN = x.NTN
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<CustomerDetailVM> CreateCustomer(CustomerDetailVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    Inv_Customers tbl = new Inv_Customers
                    {
                        Status = true,
                        CustName = mod.CustName,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        Address = mod.Address,
                        Email = mod.Email,
                        Mobile = mod.Mobile,
                        GLCode = mod.GLCode,
                        AccountTitle = mod.AccountTitle,
                        BankName = mod.BankName,
                        BusinessRelation = mod.BusinessRelation,
                        ChequeNo = mod.ChequeNo,
                        DaysLimit = mod.DaysLimit,
                        Remarks = mod.Remarks,
                        CNIC = mod.CNIC,
                        NTN = mod.NTN
                    };
                    db.Inv_Customers.Add(tbl);
                    await db.SaveChangesAsync();


                    var accId = await accountBL.GetAcc(451);
                    Fin_Subsidary sub = new Fin_Subsidary
                    {
                        AccId = accId,
                        SubsidaryName = mod.CustName,
                        SubCode = accId.ToString("00-00-00-00000"),
                        SubTypeId = 461,
                        SubsidaryCode = "CRED-" + tbl.CustId.ToString()
                    };

                    db.Fin_Subsidary.Add(sub);
                    await db.SaveChangesAsync();

                    sub.SubsidaryCode = "CRED-" + sub.SubId.ToString();
                    tbl.GLCode = sub.SubId.ToString();

                    await db.SaveChangesAsync();

                    mod.CustId = tbl.CustId;
                    scop.Complete();
                    scop.Dispose();
                    return mod;
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    return null;
                }
            }
        }

        public async Task<List<CreditInvoiceVM>> GetCreditInvoiceNo()
        {
            try
            {
                var lst = await db.Inv_Sale.Where(x => x.TransactionTypeId == 5
                && (x.Inv_SaleDetail.Sum(a => (decimal?)(a.SPrice - a.Discount)) ?? 0 - x.Advance - x.Discount - x.Inv_SaleCreditReceive.Sum(a => (decimal?)a.ReceivedAmount) ?? 0) > 0
                ).Select(x => new CreditInvoiceVM
                {
                    BillNo = x.BillNo,
                    CustId = x.CustId,
                    CustName = x.CustName,
                    TransId = x.TransId

                }).ToListAsync();
                lst.AddRange(await (from S in db.Inv_Sale
                                    join E in db.Pay_EmpMaster on S.CustId equals E.EmpId
                                    where S.TransactionTypeId == 11
                                    && (S.Inv_SaleDetail.Sum(a => (decimal?)(a.SPrice - a.Discount)) ?? 0 - S.Advance - S.Discount - S.Inv_SaleCreditReceive.Sum(a => (decimal?)a.ReceivedAmount) ?? 0) > 0
                                    select new CreditInvoiceVM
                                    {
                                        BillNo = S.BillNo,
                                        CustId = S.CustId,
                                        CustName = E.EmpName,
                                        TransId = S.TransId
                                    }).ToListAsync());
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<long> CreditSaleSave(SaleCreditReceiveVM mod, int UserId)
        {
            try
            {
                var cus = await GetSaleDetail(mod.SaleId);
                Inv_SaleCreditReceive tbl = new Inv_SaleCreditReceive
                {
                    LocId = mod.LocId,
                    TransDate = DateTime.Now,
                    Remarks = mod.Remarks,
                    CustId = cus.CustId,
                    ReceivedAmount = mod.ReceivedAmount,
                    SaleId = mod.SaleId,
                    WorkingDate = setupBL.GetWorkingDate(mod.LocId),
                    UserId = UserId,
                    ReceivedFrom = mod.ReceivedFrom
                };
                db.Inv_SaleCreditReceive.Add(tbl);
                await db.SaveChangesAsync();
                return tbl.TransId;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return 0;
            }
        }

        public async Task<Inv_Sale> GetSaleDetail(long transid)
        {
            try
            {
                var lst = await db.Inv_Sale.FindAsync(transid);
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCustomer(CustomerDetailVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Inv_Customers.SingleOrDefaultAsync(x => x.CustId.Equals(mod.CustId));
                if (tbl != null)
                {

                    tbl.TransDate = DateTime.Now;
                    tbl.UserId = UserId;
                    tbl.Address = mod.Address;
                    tbl.Email = mod.Email;
                    tbl.Mobile = mod.Mobile;
                    //tbl.GLCode = mod.GLCode;
                    tbl.AccountTitle = mod.AccountTitle;
                    tbl.BankName = mod.BankName;
                    tbl.BusinessRelation = mod.BusinessRelation;
                    tbl.ChequeNo = mod.ChequeNo;
                    tbl.DaysLimit = mod.DaysLimit;
                    tbl.Remarks = mod.Remarks;
                    tbl.NTN = mod.NTN;
                    tbl.CNIC = mod.CNIC;

                    if (mod.CustName != tbl.CustName)
                    {
                        tbl.CustName = mod.CustName;
                        var sub = await db.Fin_Subsidary.FindAsync(Convert.ToInt64(tbl.GLCode));
                        sub.SubsidaryName = mod.CustName;
                    }
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }

        public async Task<bool> DestroyCustomer(CustomerDetailVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Inv_Customers.SingleOrDefaultAsync(x => x.CustId.Equals(mod.CustId));
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        #endregion

        #region Cash/Credit Sale
        public async Task<List<TransactionTypeVM>> SaleTypeList()
        {
            try
            {
                return await db.Comp_TransactionType.Where(x => x.TransactionTypeId != 4).Select(x => new TransactionTypeVM
                {
                    TransactionTypeId = x.TransactionTypeId,
                    TransactionType = x.TransactionType
                }).ToListAsync();
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<string> GetInvoiceNo(int LocId)
        {
            try
            {
                var docNo = await db.Inv_Sale.Where(x => x.LocId == LocId).OrderByDescending(x => x.TransId).Select(x => x.BillNo).FirstOrDefaultAsync();
                if (docNo != null)
                {
                    if (docNo.Substring(0, 4) == DateTime.Now.ToString("yyMM"))
                    {
                        docNo = DateTime.Now.ToString("yyMM") + LocId.ToString("000") + (Convert.ToInt32(docNo.Substring(7, 5)) + 1).ToString("00000");
                    }
                    else
                    {
                        docNo = DateTime.Now.ToString("yyMM") + LocId.ToString("000") + "00001";
                    }
                }
                else
                {
                    docNo = DateTime.Now.ToString("yyMM") + LocId.ToString("000") + "00001";
                }
                return docNo;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        public async Task<string> GetOrderNo(int LocId)
        {
            try
            {
                var docNo = await db.Inv_SaleOrder.Where(x => x.LocId == LocId).OrderByDescending(x => x.TransId).Select(x => x.BillNo).FirstOrDefaultAsync();
                if (docNo != null)
                {
                    if (docNo.Substring(4, 4) == DateTime.Now.ToString("yyMM"))
                    {
                        docNo = "ORD-" + DateTime.Now.ToString("yyMM") + LocId.ToString("000") + (Convert.ToInt32(docNo.Substring(11, 5)) + 1).ToString("00000");
                    }
                    else
                    {
                        docNo = "ORD-" + DateTime.Now.ToString("yyMM") + LocId.ToString("000") + "00001";
                    }
                }
                else
                {
                    docNo = "ORD-" + DateTime.Now.ToString("yyMM") + LocId.ToString("000") + "00001";
                }
                return docNo;
            }
            catch (Exception ex)
            {

                return "";
            }
        }
        public string IsFBRInvoice(long TransId, string Type)
        {
            try
            {
                if (Type == "C")
                {
                    var inv = db.Inv_Sale.Find(TransId);
                    if (inv.TInvoiceNo == null)

                    //if (!(db.spget_Taxable(TransId, Type).FirstOrDefault().Value))
                    {
                        return "Not Taxable";
                    }
                    else
                    {
                        if (inv.Inv_SaleDetail.Where(x => x.SPrice > x.MRP).Any())
                        {
                            return "Above MRP";
                        }
                        else
                        {
                            return "OK";
                        }
                    }
                }
                else if (Type == "I")
                {
                    var inv = db.Lse_Master.Find(TransId);
                    if (inv.TInvoiceNo == null)
                    //if (!(db.spget_Taxable(TransId, Type).FirstOrDefault().Value))
                    {
                        return "Not Taxable";
                    }
                    else
                    {
                        return "OK";
                    }
                }
                else if (Type == "R")
                {
                    var inv = db.Lse_Return.Find(TransId);
                    if (inv.FBRInvoiceNo == null)
                    //if (!(db.spget_Taxable(TransId, Type).FirstOrDefault().Value))
                    {
                        return "Not Taxable";
                    }
                    else
                    {
                        return "OK";
                    }
                }
                return "Error";
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        public async Task<List<CashSaleVM>> GetCashSale(int locid, DateTime fromdate, DateTime todate)
        {
            try
            {
                var lst = db.Inv_Sale.Where(x => x.LocId == locid && x.BillDate >= fromdate.Date && x.BillDate <= todate.Date && x.FBRInvoiceNo != null
                ).ToList().Select(x => new CashSaleVM()
                {
                    TransId = x.TransId,
                    BillNo = x.BillNo,
                    Amount = x.Inv_SaleDetail.Sum(y => y.SPrice) - x.Inv_SaleDetail.Sum(y => y.Discount),
                    Customer = x.CustName,
                    FBR = x.FBRInvoiceNo,
                    IsReturn = GetReturnType(x.TransactionTypeId),
                    USIN = x.TInvoiceNo,
                    CNIC = x.CustCNIC,
                    Mobile = x.CustCellNo,
                    Type = "Cash"
                }).ToList();
                var ls = db.Lse_Master.Where(x => x.LocId == locid && x.DeliveryDate >= fromdate.Date && x.DeliveryDate <= todate.Date && x.FBRInvoiceNo != null
                ).ToList().Select(x => new CashSaleVM()
                {
                    TransId = x.AccNo,
                    BillNo = x.AccNo.ToString(),
                    Amount = x.InstPrice,
                    Customer = x.CustName,
                    FBR = x.FBRInvoiceNo,
                    IsReturn = "Inst Sale",
                    USIN = x.TInvoiceNo,
                    CNIC = x.NIC,
                    Mobile = x.Mobile1,
                    Type = "Lease"
                }).ToList();
                lst.AddRange(ls);
                var lsr = db.Lse_Return.Where(x => x.LocId == locid && x.WorkingDate >= fromdate.Date && x.WorkingDate <= todate.Date && x.FBRInvoiceNo != null
                ).ToList().Select(x => new CashSaleVM()
                {
                    TransId = x.TransId,
                    BillNo = x.AccNo.ToString(),
                    Amount = x.Lse_Master.InstPrice,
                    Customer = x.Lse_Master.CustName,
                    FBR = x.FBRInvoiceNo,
                    IsReturn = "Inst Sale Return",
                    USIN = x.TInvoiceNo,
                    CNIC = x.Lse_Master.NIC,
                    Mobile = x.Lse_Master.Mobile1,
                    Type = "LeaseReturn"
                }).ToList();
                lst.AddRange(lsr);
                return lst;
            }
            catch (Exception ex)
            {

                return null;
            }
            //var invsale = await (from item in db.Inv_Sale
            //                     join invsaledet in db.Inv_SaleDetail on item.TransId equals invsaledet.TransId
            //                     where item.LocId == locid && item.BillDate >= fromdate.Date && item.BillDate <= todate.Date
            //                     select new CashSaleVM()
            //                     {
            //                         TransId = item.TransId,
            //                         BillNo = item.BillNo,
            //                         Amount = item.ReceiveAmount,
            //                         Customer = item.CustName,
            //                         FBR = item.FBRInvoiceNo,
            //                         USIN = item.TInvoiceNo,
            //                         CNIC = item.CustCNIC,
            //                         Mobile = item.CustCellNo
            //                     }).ToListAsync()

        }

        public string GetReturnType(int typeid)
        {
            if (typeid == 2)
            {
                return "Cash Sale Return";
            }
            else if (typeid == 6)
            {
                return "Credit Sale Return";
            }
            else
            {
                return "";
            }
        }
        public async Task<ResultVM> SaveSale(IEnumerable<SaleDetailVM> mod, int LocId, int CustId, string CustomerName, DateTime InvoiceDate,
            DateTime DueDate, decimal ReceiveAmount, decimal Advance, decimal Discount, decimal OrderAdvance, string CustCellNo, string Remarks, int PaymentModeId, long PaymentAccId, string CustomerAccountNo, int Salesman, int TransTypeId,
            string Address, string CustCNIC, string CustNTN, long OrderId, int UserId, List<long> files, string AccountTitle, string BankName, string CustType, string Email, string BankTransId, string CustomerAccountHolder)
        {
            ResultVM result = new ResultVM();
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (TransTypeId == 5)
                    {
                        var tbl = await db.Inv_Customers.FindAsync(CustId);
                        if (tbl.Mobile.Length < 11)
                        {
                            result.Msg = "Mobile No Required";
                            return result;
                        }
                        if (tbl.CNIC.Length < 13)
                        {
                            result.Msg = "CNIC Required";
                            return result;
                        }
                        CustCellNo = tbl.Mobile;
                        CustomerName = tbl.CustName;
                        Address = tbl.Address;
                        CustCNIC = tbl.CNIC;
                        CustNTN = tbl.NTN;
                    }
                    else if (TransTypeId == 11)
                    {
                        var tbl = await db.Pay_EmpMaster.FindAsync(CustId);
                        if (tbl.Mobile1.Length < 11)
                        {
                            result.Msg = "Mobile No Required";
                            return result;
                        }
                        if (tbl.CNIC.Length < 13)
                        {
                            result.Msg = "CNIC Required";
                            return result;
                        }
                        CustCellNo = tbl.Mobile1;
                        CustomerName = tbl.EmpName;
                        Address = tbl.Address;
                        CustCNIC = tbl.CNIC;
                    }

                    //if(PaymentModeId > 1)
                    //{
                    //    var cust = await db.Inv_Customers.Where(x => x.Mobile == CustCellNo && x.CustType == "O").FirstOrDefaultAsync();
                    //    if (cust == null)
                    //    {
                    //        cust = new Inv_Customers
                    //        {
                    //            AccountTitle = AccountTitle,
                    //            Address = Address,
                    //            BankName = BankName,
                    //            BusinessRelation = CustType,
                    //            ChequeNo = CustomerAccountNo,
                    //            CNIC = CustCNIC,
                    //            CustName = CustomerName,
                    //            CustType = "O",
                    //            DaysLimit = 0,
                    //            Email = Email,
                    //            Mobile = CustCellNo,
                    //            NTN = CustNTN,
                    //            Remarks = Remarks,
                    //            Status = true,
                    //            TransDate = DateTime.Now,
                    //            UserId = UserId
                    //        };
                    //        db.Inv_Customers.Add(cust);
                    //        await db.SaveChangesAsync();
                    //        SaveOnlineCustPic(uri, cust.CustId.ToString());
                    //    }
                    //    else
                    //    {
                    //        cust.AccountTitle = AccountTitle;
                    //        cust.Address = Address;
                    //        cust.BankName = BankName;
                    //        cust.BusinessRelation = CustType;
                    //        cust.ChequeNo = CustomerAccountNo;
                    //        cust.CNIC = CustCNIC;
                    //        cust.CustName = CustomerName;
                    //        cust.CustType = "O";
                    //        cust.DaysLimit = 0;
                    //        cust.Email = Email;
                    //        cust.Mobile = CustCellNo;
                    //        cust.NTN = CustNTN;
                    //        cust.Remarks = Remarks;
                    //    }
                    //}

                    if (OrderId > 0)
                    {
                        var ord = await db.Inv_SaleOrder.FindAsync(OrderId);
                        if (ord.Status != "O" || ord.Advance != OrderAdvance || ord.TransactionTypeId != 13)
                        {
                            result.Msg = "Invalid Order";
                            return result;
                        }
                        else
                        {
                            ord.Status = "S";
                        }
                    }
                    else
                    {
                        OrderAdvance = 0;
                    }

                    InvoiceDate = await GetWorkingDate(LocId) ?? DateTime.Now.Date;
                    string invNo = await GetInvoiceNo(LocId);
                    Inv_Sale mas = new Inv_Sale()
                    {
                        BillDate = InvoiceDate,
                        DueDate = DueDate,
                        LocId = LocId,
                        TransDate = DateTime.Now,
                        Remarks = Remarks,
                        UserId = UserId,
                        Advance = Advance,
                        OrderAdvance = OrderAdvance,
                        BillNo = invNo,
                        CustId = CustId,
                        Discount = 0,
                        ReceiveAmount = ReceiveAmount,
                        CustName = CustomerName,
                        CustCellNo = CustCellNo,
                        PaymentModeId = PaymentModeId,
                        TransactionTypeId = TransTypeId,
                        Salesman = Salesman,
                        Address = Address,
                        CustCNIC = CustCNIC,
                        CustNTN = CustNTN,
                        PaymentAccId = (PaymentModeId == 1 ? (long?)null : PaymentAccId),
                        OrderId = OrderId,
                        CustAccountNo = CustomerAccountNo,
                        BankTransId = BankTransId,
                        CustAccountHolder = CustomerAccountHolder,
                        CustAccountTitle = AccountTitle,
                        CustBankName = BankName,
                        CustType = CustType,
                        Email = Email
                    };
                    db.Inv_Sale.Add(mas);
                    await db.SaveChangesAsync();
                    //List<Inv_SaleDetail> lst = new List<Inv_SaleDetail>();
                    foreach (var v in mod)
                    {
                        if (v.ItemId > 0)
                        {
                            var item = await db.Inv_Store.FindAsync(v.ItemId);
                            int PrevStatus = item.StatusID;
                            if (item.Inv_Status.MFact == -1)
                            {
                                result.Msg = "Serial Not in Stock";
                                return result;
                            }
                            item.SPrice = v.SPrice;
                            item.StatusID = 2;
                            //await db.SaveChangesAsync();

                            var pri = await GetItemSMBySKULoc(v.SKUId, LocId);
                            decimal mrp = item.MRP;
                            if (v.SPrice > item.MRP)
                            {
                                mrp = v.SPrice;
                            }

                            Inv_SaleDetail det = new Inv_SaleDetail
                            {
                                ItemId = v.ItemId,
                                PPrice = item.PPrice,
                                Qty = 1,
                                SPrice = v.SPrice,
                                TransId = mas.TransId,
                                IsReturned = false,
                                Discount = v.Discount,
                                MRP = mrp,
                                PrevStatus = PrevStatus,
                                SM = (item.Inv_Suppliers.CategoryId == 4 ? item.PPrice : pri),
                                Tax = (item.Exempted ?? false) ? 0: Math.Round(mrp - (mrp * 100 / ((item.Tax ?? 0) + 100))) //Math.Round(item.MRP - (item.MRP * 100/((item.Inv_Suppliers.GST ?? 0) +100)))
                            };
                            db.Inv_SaleDetail.Add(det);
                            await db.SaveChangesAsync();

                            if (v.SPrice < (item.Inv_Suppliers.CategoryId == 4 ? item.PPrice : pri))
                            {
                                Inv_SaleDiscount disc = new Inv_SaleDiscount
                                {
                                    CareOf = v.CareOf,
                                    Disc = (item.Inv_Suppliers.CategoryId == 4 ? item.PPrice : pri) - v.SPrice,
                                    Reason = v.Reason,
                                    TransDtlId = det.TransDtlId
                                };
                                db.Inv_SaleDiscount.Add(disc);
                                await db.SaveChangesAsync();
                            }
                            //var stHist = await setupBL.CreateStoreHistory(mas.BillDate, item.ItemId, item.LocId, -1, item.MRP, item.PPrice, item.SKUId, det.SM, det.SPrice, mas.TransDate, (TransTypeId==1?"Cash Sale": TransTypeId == 5 ? "Reference Sale" : TransTypeId == 11 ? "Employee Sale" : ""), mas.UserId);
                            //if (!stHist)
                            //{
                            //    scop.Dispose();
                            //}
                        }
                    }

                    //mas.Inv_SaleDetail = lst;
                    //db.Inv_Sale.Add(mas);
                    //await db.SaveChangesAsync();
                    var lst = await db.Inv_SaleDetail.Where(x => x.TransId == mas.TransId).ToListAsync();
                    foreach (var v in lst)
                    {
                        var item = await db.Inv_Store.FindAsync(v.ItemId);
                        decimal mrp = item.MRP;
                        if (v.SPrice > item.MRP)
                        {
                            mrp = v.SPrice;
                        }
                        Inv_StoreHistory tbl = new Inv_StoreHistory
                        {
                            DocDate = mas.BillDate,
                            ItemId = item.ItemId,
                            LocId = item.LocId,
                            MFact = -1,
                            MRP = mrp,
                            PPrice = item.PPrice,
                            Qty = 1,
                            SKUId = item.SKUId,
                            SMPrice = v.SM,
                            SPrice = v.SPrice,
                            TransDate = mas.TransDate,
                            Type = (TransTypeId == 1 ? "Cash Sale" : TransTypeId == 5 ? "Reference Sale" : TransTypeId == 11 ? "Employee Sale" : ""),
                            UserId = UserId,
                            RefId = mas.TransId
                        };
                        db.Inv_StoreHistory.Add(tbl);
                        var cart = await db.Inv_SaleCart.Where(x => x.ItemId == v.ItemId && x.Status == "T").ToListAsync();
                        cart.ForEach(x => x.Status = "S");
                    }
                    await db.SaveChangesAsync();
                    //if (IsCash)
                    //{
                    //    var CrCode = await accountBL.GetCodeByLoc(60102, LocId);
                    //    await accountBL.PostAutoVoucher(2, UserId, LocId, mas.TransId.ToString(), "AutoVoucher", 5070100001, CrCode, mod.Sum(x => x.SPrice), "Cash Againt CashSale", "Cash Againt CashSale");
                    //}
                    //else
                    //{
                    //    var DrCode = (await db.Inv_Customers.FindAsync(CustId)).GLCode ?? 0;
                    //    var CrCode = await accountBL.GetCodeByLoc(50102, LocId);
                    //    await accountBL.PostAutoVoucher(2, UserId, LocId, mas.TransId.ToString(), "AutoVoucher", DrCode, CrCode, mod.Sum(x => x.SPrice), "Credit Sale", "Credit Sale");
                    //}

                    if (files != null && files.Count > 0)
                    {
                        await new DocumentBL().UpdateDocRef(files, mas.TransId);
                    }
                    scop.Complete();
                    scop.Dispose();
                    result.Msg = "Save Successfully";
                    result.TransId = mas.TransId;
                    return result;
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    result.Msg = "Server Error";
                    return result;
                }
            }
        }
        public async Task<List<Inv_StoreVM>> GetSaleCart(int LocId)
        {
            try
            {
                var lst = await (from C in db.Inv_SaleCart
                                 join S in db.Inv_Store on C.ItemId equals S.ItemId
                                 where C.LocId == LocId && S.LocId == LocId && S.Inv_Status.MFact == 1
                                 && C.Status == "T"
                                 select new Inv_StoreVM
                                 {
                                     //SKUId = S.SKUId,
                                     //Company = S.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                                     //Product = S.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                                     //Model = S.Itm_Master.Itm_Model.Model,
                                     //CSerialNo = S.CSerialNo,
                                     MRP = C.Rate,
                                     ItemId = S.ItemId,
                                     SerialNo = S.SerialNo,
                                     SKU = S.Itm_Master.SKUCode,
                                     //Status = S.Inv_Status.StatusTitle,
                                     //LocId = S.LocId
                                 }).ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<SaleCartVM> SaveSaleCart(SaleCartVM mod)
        {
            try
            {
                Inv_SaleCart tbl = new Inv_SaleCart
                {
                    ItemId = mod.ItemId,
                    LocId = mod.LocId,
                    Rate = mod.Rate,
                    SalesmanId = mod.SalesmanId,
                    Status = "T",
                    TransDate = DateTime.Now
                };
                db.Inv_SaleCart.Add(tbl);
                await db.SaveChangesAsync();
                mod.RowId = tbl.RowId;
                return mod;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<dynamic> GetSaleOrderList(int LocId)
        {
            try
            {
                var lst = await db.Inv_SaleOrder.Where(x => x.LocId == LocId && x.Status == "O").Select(x => new { x.BillNo, x.BillDate, x.Advance, x.TransId, x.CustCellNo, x.CustCNIC, x.CustName, x.Address, x.Remarks, x.DueDate, x.Salesman }).ToListAsync();
                return lst;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<long> SaleOrderReturn(int LocId, long TransId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var x = await db.Inv_SaleOrder.Where(a => a.LocId == LocId && a.TransId == TransId && a.Status == "O" && a.TransactionTypeId == 13).FirstOrDefaultAsync();
                    if (x == null)
                    {
                        scop.Dispose();
                        return 0;
                    }
                    x.Status = "C";
                    DateTime InvoiceDate = await GetWorkingDate(LocId) ?? DateTime.Now.Date;
                    string invNo = await GetOrderNo(LocId);
                    Inv_SaleOrder mas = new Inv_SaleOrder()
                    {
                        BillDate = InvoiceDate,
                        DueDate = x.DueDate,
                        LocId = LocId,
                        TransDate = DateTime.Now,
                        Remarks = x.Remarks,
                        UserId = UserId,
                        Advance = x.Advance,
                        BillNo = invNo,
                        CustId = x.CustId,
                        Discount = x.Discount,
                        CustName = x.CustName,
                        CustCellNo = x.CustCellNo,
                        PaymentModeId = x.PaymentModeId,
                        TransactionTypeId = 14,
                        Salesman = x.Salesman,
                        Address = x.Address,
                        CustCNIC = x.CustCNIC,
                        Status = "R",
                        PaymentAccId = x.PaymentAccId,
                        RefSaleId = x.TransId
                    };
                    List<Inv_SaleOrderDetail> lst = new List<Inv_SaleOrderDetail>();
                    foreach (var v in x.Inv_SaleOrderDetail)
                    {
                        if (v.SKUId > 0)
                        {
                            Inv_SaleOrderDetail det = new Inv_SaleOrderDetail
                            {
                                Qty = 1,
                                SPrice = v.SPrice,
                                TransId = mas.TransId,
                                Discount = v.Discount,
                                SKUId = v.SKUId
                            };
                            lst.Add(det);
                        }
                    }

                    mas.Inv_SaleOrderDetail = lst;
                    db.Inv_SaleOrder.Add(mas);
                    await db.SaveChangesAsync();

                    scop.Complete();
                    scop.Dispose();
                    return mas.TransId;
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<List<SaleDetailVM>> GetSaleOrderDetail(int LocId, long TransId)
        {
            try
            {
                var lst = await (from x in db.Inv_SaleOrderDetail
                                 join I in db.Itm_Master on x.SKUId equals I.SKUId
                                 where x.TransId == TransId && x.Inv_SaleOrder.LocId == LocId
                                 select new SaleDetailVM
                                 {
                                     Discount = x.Discount,
                                     ItemId = 0,
                                     SKUId = x.SKUId,
                                     SPrice = x.SPrice,
                                     TPrice = x.SPrice - x.Discount,
                                     SKUName = I.SKUCode,
                                     Qty = x.Qty
                                 }).ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<long> SaveSaleOrder(IEnumerable<SaleDetailVM> mod, int LocId, int CustId, string CustomerName, DateTime InvoiceDate,
    DateTime DueDate, decimal ReceiveAmount, decimal Advance, decimal Discount, string CustCellNo, string Remarks, int PaymentModeId, long PaymentAccId, int Salesman, int TransTypeId,
    string Address, string CustCNIC, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    InvoiceDate = await GetWorkingDate(LocId) ?? DateTime.Now.Date;
                    string invNo = await GetOrderNo(LocId);
                    Inv_SaleOrder mas = new Inv_SaleOrder()
                    {
                        BillDate = InvoiceDate,
                        DueDate = DueDate,
                        LocId = LocId,
                        TransDate = DateTime.Now,
                        Remarks = Remarks,
                        UserId = UserId,
                        Advance = Advance,
                        BillNo = invNo,
                        CustId = CustId,
                        Discount = Discount,
                        //ReceiveAmount = ReceiveAmount,
                        CustName = CustomerName,
                        CustCellNo = CustCellNo,
                        PaymentModeId = PaymentModeId,
                        TransactionTypeId = TransTypeId,
                        Salesman = Salesman,
                        Address = Address,
                        CustCNIC = CustCNIC,
                        Status = "O",
                        PaymentAccId = (PaymentModeId == 1 ? (long?)null : PaymentAccId),
                    };

                    //await db.SaveChangesAsync();
                    List<Inv_SaleOrderDetail> lst = new List<Inv_SaleOrderDetail>();
                    foreach (var v in mod)
                    {
                        if (v.SKUId > 0)
                        {
                            //var item = await db.Inv_Store.FindAsync(v.ItemId);
                            //item.SPrice = v.SPrice;
                            //item.StatusID = 2;
                            //await db.SaveChangesAsync();

                            //var pri = await GetItemSMBySKULoc(v.SKUId, LocId);

                            Inv_SaleOrderDetail det = new Inv_SaleOrderDetail
                            {
                                //ItemId = v.ItemId,
                                //PPrice = 0,
                                Qty = 1,
                                SPrice = v.SPrice,
                                TransId = mas.TransId,
                                //IsReturned = false,
                                Discount = v.Discount,
                                SKUId = v.SKUId
                                //MRP = 0,
                                //SM = 0,
                                //Tax = 0
                            };
                            lst.Add(det);
                        }
                    }

                    mas.Inv_SaleOrderDetail = lst;
                    db.Inv_SaleOrder.Add(mas);
                    await db.SaveChangesAsync();

                    scop.Complete();
                    scop.Dispose();
                    return mas.TransId;
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<object> GetItemBySrNo4Advance(string SrNo, int SKUId, int LocId, int PlanId, int Duration)
        {
            try
            {
                var item = await db.Inv_Store.Where(x => x.SerialNo == SrNo && x.SKUId == SKUId && x.LocId == LocId && x.Inv_Status.MFact == 1).FirstOrDefaultAsync();
                if (item != null)
                {
                    if (item.Inv_Suppliers.CategoryId == 4 && item.Itm_Master.Itm_Model.Itm_Type.ProductId != 371 && Duration == 6)
                    {
                        return new
                        {
                            ItemId = 0,
                            CSerialNo = "",
                            SerialNo = "",
                            Msg = "6 Month Plan only allow for Battery"
                        };
                    }
                    var plan = (await GetInstPriceBySKU(SKUId, LocId, Duration, item.ItemId)).FirstOrDefault();
                    if (plan.RowId != PlanId)
                    {
                        return new
                        {
                            ItemId = 0,
                            CSerialNo = "",
                            SerialNo = "",
                            Msg = "Old Plan"
                        };
                    }
                    return new
                    {
                        ItemId = item.ItemId,
                        CSerialNo = item.CSerialNo,
                        SerialNo = item.SerialNo,
                        Msg = ""
                    };
                }
                return new
                {
                    ItemId = 0,
                    CSerialNo = "",
                    SerialNo = "",
                    Msg = "Serial Not Found"
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<object> GetItemBySrNo(string SrNo, int SKUId, int LocId)
        {
            try
            {
                return await db.Inv_Store.Where(x => x.SerialNo == SrNo && x.SKUId == SKUId && x.LocId == LocId && x.Inv_Status.MFact == 1).Select(x =>
                new
                {
                    ItemId = x.ItemId,
                    CSerialNo = x.CSerialNo,
                    SerialNo = x.SerialNo
                }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> CheckPairingForProcessing(int[] mod)
        {
            try
            {
                var ls = await db.Itm_Master.Where(x => mod.Contains(x.SKUId) && (x.Itm_Model.Itm_Type.ProductId == 341 || x.Itm_Model.Itm_Type.ProductId == 342)).Select(x => new SKUPairVM { SKUId = x.SKUId, AvailableForSale = false, Type = x.IsPair }).ToListAsync();
                foreach (var item in ls)
                {

                    if (item.Type == false && !item.AvailableForSale)
                    {
                        item.AvailableForSale = true;
                        var itm = (from L in ls
                                   join P in db.Itm_SKUPair on L.SKUId equals P.PairId
                                   where P.SKUId == item.SKUId && L.Type == true && !L.AvailableForSale
                                   select L).FirstOrDefault();
                        if (itm != null)
                        {
                            itm.AvailableForSale = true;
                        }
                        else
                        {
                            return "Please add Pairing Item";
                        }
                    }
                    else if (item.Type == true && !item.AvailableForSale)
                    {
                        item.AvailableForSale = true;
                        var itm = (from L in ls
                                   join P in db.Itm_SKUPair on L.SKUId equals P.SKUId
                                   where P.PairId == item.SKUId && L.Type == false && !L.AvailableForSale
                                   select L).FirstOrDefault();
                        if (itm != null)
                        {
                            itm.AvailableForSale = true;
                        }
                        else
                        {
                            return "Please add Pairing Item";
                        }
                    }
                }
                return "OK";
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool SaveOnlineCustPic(string uri, string id)
        {
            try
            {
                uri = uri.Replace("data:image/jpeg;base64,", "");
                string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~\\Content\\OnlineCustImg\\") + id.ToString() + ".jpg";
                if (File.Exists(filePath))
                {
                    string newFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~\\Content\\OnlineCustImg\\") + id.ToString() + "_" + DateTime.Now.Ticks.ToString() + ".jpg";
                    File.Move(filePath, newFilePath);
                }
                var img = new AttendanceBL().Base64StringToBitmap(uri);
                img = new Bitmap(img, new Size(200, 200));
                img.Save(filePath);
                //System.IO.File.WriteAllBytes(filePath, imgByte);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<string> CheckBikeCashSale(long[] mod)
        {
            try
            {
                foreach (var aa in mod)
                {
                    var type = await db.Inv_Store.Where(x => x.ItemId == aa).Select(x => x.Itm_Master.Itm_Model.TypeId).FirstOrDefaultAsync();
                    if (type == 878)
                    {
                        return "United Bike Cash Sale Temporarily Blocked by Management";
                    }
                    else if (type == 1065)
                    {
                        return "RoadPrince Bike Cash Sale Temporarily Blocked by Management";
                    }
                }
                return "OK";
            }
            catch (Exception)
            {
                return "Server Error";
            }
        }

        public async Task<string> CheckPairing(long[] mod)
        {
            try
            {

                var ls = await db.Inv_Store.Where(x => mod.Contains(x.ItemId) && (x.Itm_Master.Itm_Model.Itm_Type.ProductId == 341 || x.Itm_Master.Itm_Model.Itm_Type.ProductId == 342)).Select(x => new SKUPairVM { SKUId = x.SKUId, AvailableForSale = false, Type = x.Itm_Master.IsPair }).ToListAsync();
                foreach (var item in ls)
                {
                    if (item.Type == false && !item.AvailableForSale)
                    {
                        item.AvailableForSale = true;
                        var itm = (from L in ls
                                   join P in db.Itm_SKUPair on L.SKUId equals P.PairId
                                   where P.SKUId == item.SKUId && L.Type == true && !L.AvailableForSale
                                   select L).FirstOrDefault();
                        if (itm != null)
                        {
                            itm.AvailableForSale = true;
                        }
                        else
                        {
                            return "Please add Pairing Item";
                        }
                    }
                    else if (item.Type == true && !item.AvailableForSale)
                    {
                        item.AvailableForSale = true;
                        var itm = (from L in ls
                                   join P in db.Itm_SKUPair on L.SKUId equals P.SKUId
                                   where P.PairId == item.SKUId && L.Type == false && !L.AvailableForSale
                                   select L).FirstOrDefault();
                        if (itm != null)
                        {
                            itm.AvailableForSale = true;
                        }
                        else
                        {
                            return "Please add Pairing Item";
                        }
                    }
                }
                return "OK";
            }
            catch (Exception)
            {
                return null;
            }
        }
        //public async Task<string> CheckPairingForProcessing(int[] mod)
        //{
        //    try
        //    {
        //        var lst = await db.Itm_Master.Where(x => mod.Contains(x.SKUId) && x.PairId > 0).Select(x => new SKUVM { SKUId = x.SKUId, PairId = x.PairId ?? 0, AvailableForSale = false }).ToListAsync();
        //        foreach (var item in lst)
        //        {
        //            if (item.SKUId == item.PairId && !item.AvailableForSale)
        //            {
        //                item.AvailableForSale = true;
        //                var itm = lst.Where(x => x.PairId == item.PairId && x.SKUId != x.PairId && !x.AvailableForSale).FirstOrDefault();
        //                if (itm != null)
        //                {
        //                    itm.AvailableForSale = true;
        //                }
        //                else
        //                {
        //                    return "Please add Pairing Item";
        //                }
        //            }
        //            else if (item.SKUId != item.PairId && !item.AvailableForSale)
        //            {
        //                item.AvailableForSale = true;
        //                var itm = lst.Where(x => x.PairId == item.PairId && x.SKUId == x.PairId && !x.AvailableForSale).FirstOrDefault();
        //                if (itm != null)
        //                {
        //                    itm.AvailableForSale = true;
        //                }
        //                else
        //                {
        //                    return "Please add Pairing Item";
        //                }
        //            }
        //        }
        //        return "OK";
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        //public async Task<string> CheckPairing(long[] mod)
        //{
        //    try
        //    {
        //        var lst = await db.Inv_Store.Where(x => mod.Contains(x.ItemId) && x.Itm_Master.PairId > 0).Select(x => new SKUVM { SKUId = x.SKUId, PairId = x.Itm_Master.PairId ?? 0, AvailableForSale = false }).ToListAsync();
        //        foreach (var item in lst)
        //        {
        //            if (item.SKUId == item.PairId && !item.AvailableForSale)
        //            {
        //                item.AvailableForSale = true;
        //                var itm = lst.Where(x => x.PairId == item.PairId && x.SKUId != x.PairId && !x.AvailableForSale).FirstOrDefault();
        //                if (itm != null)
        //                {
        //                    itm.AvailableForSale = true;
        //                }
        //                else
        //                {
        //                    return "Please add Pairing Item";
        //                }
        //            }
        //            else if (item.SKUId != item.PairId && !item.AvailableForSale)
        //            {
        //                item.AvailableForSale = true;
        //                var itm = lst.Where(x => x.PairId == item.PairId && x.SKUId == x.PairId && !x.AvailableForSale).FirstOrDefault();
        //                if (itm != null)
        //                {
        //                    itm.AvailableForSale = true;
        //                }
        //                else
        //                {
        //                    return "Please add Pairing Item";
        //                }
        //            }
        //        }
        //        return "OK";
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public async Task<string> CheckLseCategory(long ItemId, long NewItemId, int CatId)
        {
            try
            {
                if (CatId == 1)
                {
                    if (ItemId > 0)
                    {
                        var itm = await db.Itm_Master.Where(x => x.SKUId == ItemId).FirstOrDefaultAsync();
                        if (itm.Itm_Model.Itm_Type.ProductId != 341 && itm.Itm_Model.Itm_Type.ProductId != 342)
                        {
                            return "You can only add one item in General Category";
                        }
                        else
                        {
                            var newItm = await db.Itm_SKUPair.Where(x => (x.SKUId == NewItemId && x.PairId == ItemId) ||
                            (x.SKUId == ItemId && x.PairId == NewItemId)).AnyAsync();
                            if (newItm)
                            {
                                return "OK";
                            }
                        }
                        return "Please add the pairing Item";
                    }
                    return "OK";
                }
                else if (CatId == 2)
                {
                    var itm = await db.Itm_Master.Where(x => x.SKUId == NewItemId).FirstOrDefaultAsync();
                    //var prod = itm.Itm_Model.Itm_Type.ProductId;
                    if (itm.Itm_Model.Itm_Type.ProductId == 347)
                    {
                        if (ItemId > 0)
                        {
                            var newItm = await db.Itm_Master.Where(x => x.SKUId == ItemId).FirstOrDefaultAsync();
                            if (newItm.Itm_Model.Itm_Type.ProductId != 371)
                            {
                                return "You can only add single UPS/Battery";
                            }
                        }
                        return "OK";
                    }
                    else if (itm.Itm_Model.Itm_Type.ProductId == 371)
                    {
                        if (ItemId > 0)
                        {
                            var newItm = await db.Itm_Master.Where(x => x.SKUId == ItemId).FirstOrDefaultAsync();
                            if (newItm.Itm_Model.Itm_Type.ProductId != 347)
                            {
                                return "You can only add single UPS/Battery";
                            }
                        }
                        return "OK";
                    }
                    else
                        return "You can only add single UPS/Battery";
                }
                else if (CatId == 3)
                {
                    var itm = await db.Itm_Master.Where(x => x.SKUId == NewItemId).FirstOrDefaultAsync();
                    //var prod = itm.Itm_Model.Itm_Type.ProductId;
                    if (itm.Itm_Model.Itm_Type.ProductId == 322)
                    {
                        if (ItemId > 0)
                        {
                            var newItm = await db.Itm_Master.Where(x => x.SKUId == ItemId).FirstOrDefaultAsync();
                            if (newItm.Itm_Model.Itm_Type.ProductId != 349)
                            {
                                return "You can only add single WashingMachine/Dryer";
                            }
                        }
                        return "OK";
                    }
                    else if (itm.Itm_Model.Itm_Type.ProductId == 349)
                    {
                        if (ItemId > 0)
                        {
                            var newItm = await db.Itm_Master.Where(x => x.SKUId == ItemId).FirstOrDefaultAsync();
                            if (newItm.Itm_Model.Itm_Type.ProductId != 322)
                            {
                                return "You can only add single WashingMachine/Dryer";
                            }
                        }
                        return "OK";
                    }
                    else
                        return "You can only add single WashingMachine/Dryer";
                }
                return "OK";
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Inv_StoreVM>> GetItemBySrNo(int LocId, string SrNo)
        {
            try
            {
                //int[] sta = new int[] { 1, 5, 6, 7 };
                return await db.Inv_Store.Where(x => x.LocId == LocId && x.SerialNo == SrNo && x.Inv_Status.MFact == 1).Select(x =>
                new Inv_StoreVM
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
                    Description = x.Itm_Master.Description
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<object> GetItemBySerialNo(string SrNo, int LocId)
        {
            try
            {
                return await db.Inv_Store.Where(x => x.SerialNo == SrNo && x.LocId == LocId && x.Inv_Status.MFact == 1).Select(x =>
                new
                {
                    ItemId = x.ItemId,
                    SkuId = x.SKUId,
                    SerialNo = x.SerialNo
                }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Sale Return
        public async Task<List<SKUVM>> GetModelForReturn(int LocId, long TransId)
        {
            try
            {
                var lst = await (from S in db.Inv_Sale
                                 join D in db.Inv_SaleDetail on S.TransId equals D.TransId
                                 join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                                 where S.LocId == LocId && S.TransId == TransId && !D.IsReturned
                                 select new SKUVM
                                 {
                                     SKUId = ST.SKUId,
                                     SKUName = ST.Itm_Master.SKUName,
                                     Model = ST.Itm_Master.Itm_Model.Model,
                                     ModelId = ST.Itm_Master.Itm_Model.ModelId,
                                     Company = ST.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                                     Product = ST.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName
                                 }).Distinct().ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Inv_Store>> GetItemForReturnList(int SKUId, int LocId, long TransId)
        {
            try
            {
                var lst = await (from S in db.Inv_Sale
                                 join D in db.Inv_SaleDetail on S.TransId equals D.TransId
                                 join I in db.Inv_Store on D.ItemId equals I.ItemId
                                 where S.LocId == LocId && S.TransId == TransId && I.SKUId == SKUId && !D.IsReturned
                                 select I).ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Inv_Sale> GetReturnInvoice(int LocId, string BillNo, int TransTypeId)
        {
            try
            {
                if (TransTypeId == 1)
                {
                    return await db.Inv_Sale.Where(x => x.LocId == LocId && x.BillNo == BillNo && x.TransactionTypeId == TransTypeId).FirstOrDefaultAsync();
                }
                else
                {
                    int[] sl = new int[] { 5, 11 };
                    return await db.Inv_Sale.Where(x => x.LocId == LocId && x.BillNo == BillNo && sl.Contains(x.TransactionTypeId)).FirstOrDefaultAsync();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<long> SaveSaleReturn(IEnumerable<SaleDetailVM> mod, int LocId, DateTime InvoiceDate,
           decimal ReceiveAmount, decimal CashAmount, string Remarks, int PaymentModeId, long PaymentAccId, long TransId, int UserId, string Type, int ReasonId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var OldBill = await db.Inv_Sale.FindAsync(TransId);

                    var invNo = await GetInvoiceNo(LocId);
                    InvoiceDate = await GetWorkingDate(LocId) ?? DateTime.Now.Date;
                    Inv_Sale mas = new Inv_Sale()
                    {
                        BillDate = InvoiceDate,
                        TransactionTypeId = OldBill.CustId == 1 ? 2 : 6,
                        LocId = LocId,
                        TransDate = DateTime.Now,
                        Remarks = Remarks,
                        UserId = UserId,
                        BillNo = invNo,
                        CustId = OldBill.CustId,
                        ReceiveAmount = ReceiveAmount,
                        PaymentModeId = PaymentModeId,
                        Advance = CashAmount,
                        Discount = 0,
                        CustName = OldBill.CustName,
                        CustCellNo = OldBill.CustCellNo,
                        Address = OldBill.Address,
                        CustCNIC = OldBill.CustCNIC,
                        CustNTN = OldBill.CustNTN,
                        Salesman = OldBill.Salesman,
                        RefSaleId = OldBill.TransId,
                        ItemType = Type,
                        PaymentAccId = (PaymentModeId == 1 ? (long?)null : PaymentAccId),
                        ReasonId = ReasonId
                    };

                    List<Inv_SaleDetail> lst = new List<Inv_SaleDetail>();
                    foreach (var v in mod)
                    {
                        if (v.ItemId > 0)
                        {
                            var item = await db.Inv_Store.FindAsync(v.ItemId);
                            int PrevStatus = item.StatusID;
                            if (item.StatusID == 1 || item.StatusID == 10)
                            {
                                return 0;
                            }

                            //item.SPrice = v.SPrice;
                            //item.StatusID = 1;
                            item.StatusID = (Type == "P" ? 1 : 10);
                            item.Exempted = item.Exempted == true ? true : (Type == "P" ? false : true);
                            var OldDetail = OldBill.Inv_SaleDetail.Where(x => x.ItemId == v.ItemId).FirstOrDefault();
                            OldDetail.IsReturned = true;

                            Inv_SaleDetail det = new Inv_SaleDetail
                            {
                                ItemId = v.ItemId,
                                PPrice = item.PPrice,
                                Qty = 1,
                                SPrice = v.SPrice,
                                TransId = mas.TransId,
                                IsReturned = false,
                                Discount = 0,
                                MRP = OldDetail.MRP,
                                SM = OldDetail.SM,
                                Tax = (item.Exempted ?? false) ? 0 : OldDetail.Tax,
                                PrevStatus = PrevStatus
                            };
                            lst.Add(det);
                            //var stHist = await setupBL.CreateStoreHistory(mas.BillDate, item.ItemId, item.LocId, 1, item.MRP, item.PPrice, item.SKUId, det.SM, det.SPrice, mas.TransDate, (mas.TransactionTypeId == 2 ? "Cash Sale Return" : mas.TransactionTypeId == 6 ? "Reference Sale Return" : ""), mas.UserId);
                            //if (!stHist)
                            //{
                            //    scop.Dispose();
                            //}
                        }
                    }

                    mas.Inv_SaleDetail = lst;
                    db.Inv_Sale.Add(mas);
                    await db.SaveChangesAsync();

                    foreach (var v in lst)
                    {
                        var item = await db.Inv_Store.FindAsync(v.ItemId);
                        Inv_StoreHistory tbl = new Inv_StoreHistory
                        {
                            DocDate = mas.BillDate,
                            ItemId = item.ItemId,
                            LocId = item.LocId,
                            MFact = 1,
                            MRP = item.MRP,
                            PPrice = item.PPrice,
                            Qty = 1,
                            SKUId = item.SKUId,
                            SMPrice = v.SM,
                            SPrice = v.SPrice,
                            TransDate = mas.TransDate,
                            Type = (mas.TransactionTypeId == 2 ? "Cash Sale Return" : mas.TransactionTypeId == 6 ? "Reference Sale Return" : ""),
                            UserId = UserId,
                            RefId = mas.TransId
                        };
                        db.Inv_StoreHistory.Add(tbl);
                    }
                    await db.SaveChangesAsync();
                    //var DrCode = await accountBL.GetCodeByLoc(60105, LocId);
                    //await accountBL.PostAutoVoucher(3, UserId, LocId, mas.TransId.ToString(), "AutoVoucher", DrCode, 5070100001, mod.Sum(x => x.SPrice), "Return Againt CashSale", "Return Againt CashSale");
                    scop.Complete();
                    scop.Dispose();
                    return mas.TransId;
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    return 0;
                }
            }
        }
        #endregion

        #region Processing
        public async Task<long> SaveProcessing(IEnumerable<LseDetailVM> det, ProcessingVM mod, int UserId)
        {
            try
            {
                //var plan = await db.Itm_SKUPlan.FindAsync(mod.Pl);
                //if (mod.Advance < plan.Advance)
                //    return 0;


                Lse_Master tbl = new Lse_Master
                {
                    ProcessTransDate = DateTime.Now,
                    FName = mod.FName,
                    ManagerId = mod.ManagerId,
                    MktOfficerId = mod.MktOfficerId,
                    Advance = mod.Advance,
                    CustName = mod.CustName,
                    Duration = mod.Duration,
                    InqOfficerId = mod.InqOfficerId,
                    SManagerId = mod.SManagerId,
                    InstPrice = mod.InstPrice,
                    LocId = mod.LocId,
                    Mobile1 = mod.Mobile1,
                    Mobile2 = mod.Mobile2,
                    //ModelId = mod.ModelId,
                    MonthlyInst = Math.Round((mod.InstPrice - mod.Advance) / (mod.Duration - 1)),
                    NIC = mod.NIC,
                    ProcessAt = mod.ProcessAt,
                    ProcessDate = setupBL.GetWorkingDate(mod.LocId),
                    ProcessFee = mod.ProcessFee,
                    Remarks = mod.Remarks,
                    Status = 1,
                    UserId = UserId,
                    CategoryId = mod.CategoryId,
                    Type = "N"
                    //PlanId = mod.Duration

                };
                foreach (var item in det)
                {
                    tbl.Lse_Detail.Add(new Lse_Detail
                    {
                        InstPrice = item.InstPrice,
                        Qty = 1,
                        SKUId = item.SKUId,
                        Status = true,
                        InstPlanId = item.InstPlanId,
                        dAdvance = item.dAdvance,
                        dInst = item.dInst,
                        PlanType = item.PlanType
                    });
                }

                //var managers = db.spget_LocManagers(mod.LocId).FirstOrDefault();
                var managers = await db.Pay_EmpHierarchy.Where(x => x.LocId == mod.LocId).FirstOrDefaultAsync();
                if (managers != null)
                {
                    tbl.RMId = managers.RMId;
                    tbl.SRMId = managers.SRMId;
                    tbl.CRCId = managers.AuditorId;
                    tbl.BDMId = managers.BDMId;
                }

                db.Lse_Master.Add(tbl);
                await db.SaveChangesAsync();
                return tbl.AccNo;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return 0;
            }
        }
        public async Task<LocManagersVM> GetLocManagers(int LocId)
        {
            try
            {
                var lst = await db.Pay_EmpHierarchy.Where(x => x.LocId == LocId).FirstOrDefaultAsync();
                LocManagersVM mod = new LocManagersVM();
                if (lst != null)
                {
                    if (lst.RMId > 0)
                    {
                        mod.RMId = lst.RMId;
                        mod.RM = lst.RM; //db.Pay_EmpMaster.FindAsync(lst.RM).Result.EmpName;
                    }
                    if (lst.SRMId > 0)
                    {
                        mod.SRMId = lst.SRMId;
                        mod.SRM = lst.SRM; //db.Pay_EmpMaster.FindAsync(lst.SRM).Result.EmpName;
                    }
                    if (lst.AuditorId > 0)
                    {
                        mod.CRCId = lst.AuditorId;
                        mod.CRC = lst.Auditor; //db.Pay_EmpMaster.FindAsync(lst.CRC).Result.EmpName;
                    }
                    if (lst.BDMId > 0)
                    {
                        mod.BDMId = lst.BDMId;
                        mod.BDM = lst.BDM; //db.Pay_EmpMaster.FindAsync(lst.CRC).Result.EmpName;
                    }
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Lse_Master> GetProceesingByNIC(string NIC)
        {
            try
            {
                return await db.Lse_Master.Where(x => x.NIC == NIC).OrderByDescending(x => x.AccNo).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<LseStatusVM>> LseStatusList()
        {
            try
            {
                return await db.Lse_Status.Select(x => new LseStatusVM
                {
                    Status = x.Status,
                    StatusId = x.StatusId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Lease Customer

        public bool ConvertURItoImage(string uri, string id)
        {
            try
            {
                uri = uri.Replace("data:image/jpeg;base64,", "");
                string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~\\Content\\CustImg\\") + id.ToString() + ".jpg";
                if (File.Exists(filePath))
                {
                    string newFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~\\Content\\CustImg\\") + id.ToString() + "_" + DateTime.Now.Ticks.ToString() + ".jpg";
                    File.Move(filePath, newFilePath);
                }
                var img = new AttendanceBL().Base64StringToBitmap(uri);
                img = new Bitmap(img, new Size(200, 200));
                img.Save(filePath);
                //System.IO.File.WriteAllBytes(filePath, imgByte);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<string> CancelProcessing(int LocId, long AccNo, string Remarks, string UploadedFiles, int UserId)
        {
            try
            {
                var acc = await db.Lse_Master.Where(x => x.LocId == LocId && x.AccNo == AccNo).FirstOrDefaultAsync();
                if (acc != null)
                {
                    if (acc.Status == 1 || acc.Status == 2)
                    {
                        acc.Status = 7;
                        acc.DeliveryTransDate = DateTime.Now;
                        acc.Remarks = Remarks;
                        acc.UserId = UserId;


                        if (!String.IsNullOrWhiteSpace(UploadedFiles))
                        {
                            List<long> files = UploadedFiles.Split(',').Select(long.Parse).ToList();
                            var IsSave = await new DocumentBL().UpdateDocRef(files, AccNo);
                            if (!IsSave)
                                return "Invalid File";
                        }
                        await db.SaveChangesAsync();
                        return "Ok";
                    }
                }
                return "Invalid Account";
            }
            catch (Exception)
            {
                return "Error";
            }
        }
        public async Task<long> SaveCustomer(LeaseCustomerVM mod, int UserId)
        {
            try
            {
                var proc = await db.Lse_Master.FindAsync(mod.AccNo);
                if (proc.Status == 1 || proc.Status == 2)
                {
                    proc.CustName = mod.CustName;
                    proc.Mobile1 = mod.Mobile1;
                    proc.Mobile2 = mod.Mobile2;
                    proc.NIC = mod.NIC;
                    proc.FName = mod.FName;
                    proc.InqOfficerId = mod.InqOfficerId;
                    proc.MktOfficerId = mod.MktOfficerId;
                    proc.ManagerId = mod.ManagerId;
                    proc.SManagerId = mod.SManagerId;
                    proc.Status = 2;
                }
                else if (proc.DeliveryDate == DateTime.Now.Date)
                {
                    proc.InqOfficerId = mod.InqOfficerId;
                    proc.MktOfficerId = mod.MktOfficerId;
                    proc.ManagerId = mod.ManagerId;
                    proc.SManagerId = mod.SManagerId;
                    proc.Mobile1 = mod.Mobile1;
                    proc.Mobile2 = mod.Mobile2;
                }
                if (proc.Status == 1 || proc.Status == 2 || proc.Status == 3)
                {
                    var tbl = await db.Lse_Customer.Where(x => x.AccNo == mod.AccNo).FirstOrDefaultAsync();
                    if (tbl == null)
                    {
                        tbl = new Lse_Customer()
                        {
                            Remarks = mod.Remarks,
                            Status = true,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            Affidavit = mod.Affidavit,
                            AuditStatus = false,
                            Defaulter = mod.Defaulter,
                            Gender = mod.Gender,
                            Occupation = mod.Occupation,
                            OffAddress = mod.OffAddress,
                            ResAddress = mod.ResAddress,
                            ResidentialStatus = mod.ResidentialStatus,
                            Salary = mod.Salary,
                            Worth = mod.Worth,
                            WrantyCard = mod.WrantyCard,
                            AccNo = mod.AccNo,
                            CustomerDate = setupBL.GetWorkingDate(proc.LocId),
                            PTO = mod.PTO,
                            SearchStatus = mod.SearchStatus,

                        };

                        tbl.Thumb = await db.Lse_CustomerTemplate.Where(x => x.AccNo == mod.AccNo).AnyAsync();
                        if (!string.IsNullOrEmpty(mod.uri))
                        {
                            tbl.Pic = true;
                            ConvertURItoImage(mod.uri, tbl.AccNo.ToString());
                        }
                        else
                        {
                            tbl.Pic = false;
                        }
                        if (!string.IsNullOrEmpty(mod.uriG))
                        {
                            tbl.Pic = true;
                            ConvertURItoImage(mod.uriG, "G" + tbl.AccNo.ToString());
                        }
                        db.Lse_Customer.Add(tbl);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        tbl.Remarks = mod.Remarks;
                        tbl.TransDate = DateTime.Now;
                        tbl.UserId = UserId;
                        tbl.Affidavit = mod.Affidavit;
                        tbl.Defaulter = mod.Defaulter;
                        tbl.Gender = mod.Gender;
                        tbl.Occupation = mod.Occupation;
                        tbl.OffAddress = mod.OffAddress;
                        tbl.ResAddress = mod.ResAddress;
                        tbl.ResidentialStatus = mod.ResidentialStatus;
                        tbl.Salary = mod.Salary;
                        tbl.Worth = mod.Worth;
                        tbl.WrantyCard = mod.WrantyCard;
                        tbl.PTO = mod.PTO;
                        tbl.SearchStatus = mod.SearchStatus;

                        if (!string.IsNullOrEmpty(mod.uri))
                        {
                            tbl.Pic = true;
                            ConvertURItoImage(mod.uri, tbl.AccNo.ToString());
                        }
                        if (!string.IsNullOrEmpty(mod.uriG))
                        {
                            tbl.Pic = true;
                            ConvertURItoImage(mod.uriG, "G" + tbl.AccNo.ToString());
                        }
                        await db.SaveChangesAsync();
                    }
                    return tbl.CustId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return 0;
            }
        }
        public async Task<Lse_Master> GetProceesingByNo(long AccNo, int LocId)
        {
            try
            {
                var workingDate = setupBL.GetWorkingDate(LocId);
                DateTime dt = workingDate.AddDays(-3);
                var mas = await db.Lse_Master.Where(x => x.AccNo == AccNo).FirstOrDefaultAsync();
                if (mas != null)
                {
                    if ((mas.Status == 1 || mas.Status == 2 || mas.DeliveryDate > dt) && mas.LocId == LocId)
                        return mas;
                    else
                    {
                        var IsAllow = await db.Lse_CER.Where(x => x.LocId == LocId && x.Status && x.WorkingDate == workingDate && x.AccNo == AccNo).AnyAsync();
                        if (IsAllow)
                            return mas;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Lse_Master> GetLseCustomerByNo(long AccNo, int LocId)
        {
            try
            {
                return await db.Lse_Master.Where(x => x.AccNo == AccNo && x.LocId == LocId && x.Status == 2).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Lse_Master> GetLseProcessingByNo(long AccNo, int LocId)
        {
            try
            {
                return await db.Lse_Master.Where(x => x.AccNo == AccNo && x.LocId == LocId && x.Status <= 2).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Lse_Master> GetAccByNo(long AccNo)
        {
            try
            {
                return await db.Lse_Master.Include(x => x.Lse_Outstand).Include(x => x.Lse_Installment)
                    //.Include(x => x.Lse_Customer)
                    //.Include(x => x.Lse_Guarantor).Include(x => x.Lse_Cheque).Include(x => x.Lse_Audit)
                    .Where(x => x.AccNo == AccNo).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> IsCustThumbExist(long AccNo)
        {
            try
            {
                return await db.Lse_CustomerTemplate.Where(x => x.AccNo == AccNo).AnyAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SaveCustTemplate(long AccNo, string template, int UserId)
        {
            try
            {
                Lse_CustomerTemplate tbl = await db.Lse_CustomerTemplate.FirstOrDefaultAsync(x => x.AccNo == AccNo);
                if (tbl == null)
                {
                    tbl = new Lse_CustomerTemplate();
                    tbl.AccNo = AccNo;
                    tbl.Status = true;
                    tbl.Template = template;
                    tbl.TransDate = DateTime.Now;
                    tbl.UserId = UserId;
                    db.Lse_CustomerTemplate.Add(tbl);
                }
                else
                {
                    tbl.Status = true;
                    tbl.Template = template;
                    tbl.TransDate = DateTime.Now;
                    tbl.UserId = UserId;
                }
                var customer = await db.Lse_Customer.Where(x => x.AccNo == AccNo).FirstOrDefaultAsync();
                if (customer != null)
                {
                    customer.Thumb = true;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        public async Task<List<spGet_ProcessingList_Result>> GetProceesingList(int LocId)
        {
            try
            {
                //var workingDate = setupBL.GetWorkingDate(LocId);
                //DateTime pt = workingDate.AddDays(-10);
                //DateTime dt = workingDate.AddDays(-3);
                //var lst = await db.Lse_CER.Where(x => x.LocId == LocId && x.Status && x.WorkingDate == workingDate).Select(x => x.AccNo).ToListAsync();
                //return await db.Lse_Master.Where(x => x.LocId == LocId && ((((x.Status == 1 || x.Status == 2) && x.ProcessDate > pt) || x.DeliveryDate > dt) || lst.Contains(x.AccNo))).Select(x => new LseMasterVM { DeliveryDate = x.ProcessDate, AccNo = x.AccNo, CustName = x.CustName }).ToListAsync();
                return db.spGet_ProcessingList(LocId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Lse_Installment>> GetProceesingListVoucher(int LocId)
        {
            try
            {
                var workingDate = setupBL.GetWorkingDate(LocId);
                DateTime dt = workingDate.AddMonths(-1);
                return await (from item in db.Lse_Master
                              join itm in db.Lse_Installment on item.AccNo equals itm.AccNo
                              where itm.PaidBy != 1 && item.LocId == LocId && itm.InstDate.Month == dt.Month && itm.InstDate.Year == dt.Year
                              select itm).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<LseMasterVM>> GetLseCustomerList(int LocId)
        {
            try
            {
                var dt = DateTime.Now.Date.AddDays(-10);
                return await db.Lse_Master.Where(x => x.LocId == LocId && x.Status == 2 && x.ProcessDate >= dt).Select(x => new LseMasterVM { DeliveryDate = x.ProcessDate, AccNo = x.AccNo, CustName = x.CustName }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<LseMasterVM>> GetLseProcessingList(int LocId)
        {
            try
            {
                var dt = DateTime.Now.Date.AddDays(-10);
                return await db.Lse_Master.Where(x => x.LocId == LocId && x.Status <= 2 && x.ProcessDate >= dt).Select(x => new LseMasterVM { DeliveryDate = x.ProcessDate, AccNo = x.AccNo, CustName = x.CustName }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Guarantor
        public async Task<List<GuarantorVM>> GuarantorList(long AccNo)
        {
            try
            {
                return await db.Lse_Guarantor.Where(x => x.Status && x.AccNo == AccNo).Select(x =>
                new GuarantorVM
                {
                    GuarantorId = x.GuarantorId,
                    AccNo = x.AccNo,
                    GRelation = x.GRelation,
                    Name = x.Name,
                    CNIC = x.NIC,
                    Occupation = x.Occupation,
                    OffAddress = x.OffAddress,
                    ResAddress = x.ResAddress,
                    TelOff = x.TelOff,
                    TelRes = x.TelRes,
                    FName = x.FName
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<GuarantorVM> CreateGuarantor(GuarantorVM mod, int UserId)
        {
            try
            {
                Lse_Guarantor tbl = new Lse_Guarantor
                {
                    AccNo = mod.AccNo,
                    GRelation = mod.GRelation,
                    Name = mod.Name,
                    NIC = mod.CNIC,
                    Occupation = mod.Occupation,
                    OffAddress = mod.OffAddress,
                    ResAddress = mod.ResAddress,
                    TelOff = mod.TelOff,
                    TelRes = mod.TelRes,
                    Status = true,
                    UserId = UserId,
                    FName = mod.FName,
                    TransDate = DateTime.Now
                };
                db.Lse_Guarantor.Add(tbl);
                await db.SaveChangesAsync();
                mod.GuarantorId = tbl.GuarantorId;
                return mod;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return null;
            }
        }

        public async Task<bool> UpdateGuarantor(GuarantorVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_Guarantor.SingleOrDefaultAsync(x => x.GuarantorId.Equals(mod.GuarantorId));
                if (tbl != null)
                {
                    tbl.GRelation = mod.GRelation;
                    tbl.Name = mod.Name;
                    tbl.NIC = mod.CNIC;
                    tbl.Occupation = mod.Occupation;
                    tbl.OffAddress = mod.OffAddress;
                    tbl.ResAddress = mod.ResAddress;
                    tbl.FName = mod.FName;
                    if (string.IsNullOrEmpty(tbl.TelOff))
                    {
                        tbl.TelOff = mod.TelOff;
                    }
                    if (string.IsNullOrEmpty(tbl.TelRes))
                    {
                        tbl.TelRes = mod.TelRes;
                    }
                    tbl.TransDate = DateTime.Now;
                    tbl.UserId = UserId;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }

        public async Task<bool> DestroyGuarantor(GuarantorVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_Guarantor.SingleOrDefaultAsync(x => x.GuarantorId.Equals(mod.GuarantorId));
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        #endregion

        #region Cheque
        public async Task<List<ChequeVM>> ChequeList(long AccNo)
        {
            try
            {
                return await db.Lse_Cheque.Where(x => x.ChequeStatus && x.AccNo == AccNo).Select(x =>
                new ChequeVM
                {
                    AccNo = x.AccNo,
                    ChequeAmount = x.ChequeAmount,
                    ChequeNo = x.ChequeNo,
                    ChequeType = x.ChequeType,
                    ChequeId = x.ChequeId,
                    AccountTitle = x.AccountTitle,
                    BankName = x.BankName,
                    BranchName = x.BranchName,
                    ChequeIssueTo = x.ChequeIssueTo,
                    ChequeReturnTo = x.ChequeReturnTo,
                    BankAccNo = x.BankAccNo,
                    ChequeBy = x.ChequeBy,
                    ChequeCNIC = x.ChequeCNIC
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ChequeVM> CreateCheque(ChequeVM mod, int UserId)
        {
            try
            {
                var NoOfCheq = await db.Lse_Cheque.Where(x => x.ChequeCNIC == mod.ChequeCNIC && x.AccNo != mod.AccNo && x.ChequeStatus && x.Lse_Master.Status <= 3).Select(x => x.AccNo).Distinct().CountAsync();
                if (NoOfCheq > 2)
                {
                    mod.Msg = "This ChequeCNIC cannot use in more than 2 Open Accounts";
                    return mod;
                }
                Lse_Cheque tbl = new Lse_Cheque
                {
                    AccNo = mod.AccNo,
                    ChequeAmount = mod.ChequeAmount,
                    ChequeNo = mod.ChequeNo,
                    ChequeType = mod.ChequeType,
                    ChequeStatus = true,
                    IsChequeReturn = false,
                    AccountTitle = mod.AccountTitle,
                    BankName = mod.BankName,
                    BranchName = mod.BranchName,
                    ChequeIssueTo = mod.ChequeIssueTo,
                    ChequeReturnTo = mod.ChequeReturnTo,
                    BankAccNo = mod.BankAccNo,
                    ChequeBy = mod.ChequeBy,
                    ChequeCNIC = mod.ChequeCNIC
                };
                db.Lse_Cheque.Add(tbl);
                await db.SaveChangesAsync();
                mod.ChequeId = tbl.ChequeId;
                mod.Msg = "OK";
                return mod;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                mod.Msg = "Error";
                return mod;
            }
        }

        public async Task<string> UpdateCheque(ChequeVM mod, int UserId)
        {
            try
            {
                var NoOfCheq = await db.Lse_Cheque.Where(x => x.ChequeCNIC == mod.ChequeCNIC && x.AccNo != mod.AccNo && x.ChequeStatus && x.Lse_Master.Status <= 3).Select(x => x.AccNo).Distinct().CountAsync();
                if (NoOfCheq >= 2)
                {
                    return "This ChequeCNIC cannot use in more than 2 Open Accounts";
                }
                var tbl = await db.Lse_Cheque.SingleOrDefaultAsync(x => x.ChequeId.Equals(mod.ChequeId));
                if (tbl != null)
                {
                    tbl.ChequeAmount = mod.ChequeAmount;
                    tbl.ChequeNo = mod.ChequeNo;
                    tbl.ChequeType = mod.ChequeType;
                    tbl.AccountTitle = mod.AccountTitle;
                    tbl.BankName = mod.BankName;
                    tbl.BranchName = mod.BranchName;
                    tbl.ChequeIssueTo = mod.ChequeIssueTo;
                    tbl.ChequeReturnTo = mod.ChequeReturnTo;
                    tbl.BankAccNo = mod.BankAccNo;
                    tbl.ChequeCNIC = mod.ChequeCNIC;
                    tbl.ChequeBy = mod.ChequeBy;
                }
                await db.SaveChangesAsync();
                return "OK";
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return "Error";
            }
        }

        public async Task<bool> DestroyCheque(ChequeVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_Cheque.SingleOrDefaultAsync(x => x.ChequeId.Equals(mod.ChequeId));
                if (tbl != null)
                {
                    tbl.ChequeStatus = false;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        #endregion

        #region Audit
        public LseAuditVM AuditRecord(long AccNo)
        {
            return db.Lse_Audit.Where(a => a.AccNo == AccNo).Select(x => new LseAuditVM()
            {
                AccNo = x.AccNo,
                AuditDate = x.AuditDate,
                Guarantor1 = x.Guarantor1,
                Guarantor2 = x.Guarantor2,
                Mobile = x.Mobile,
                NIC = x.NIC,
                Cheque = x.Cheque,
                Pic = x.Pic,
                Thumb = x.Thumb,
                Affidavit = x.Affidavit,
                Completion = x.Completion,
                BMSign = x.BMSign,
                RMSign = x.RMSign,
                VisitStatus = x.VisitStatus,
                ObserveState = x.ObserveState,
                ObserveDate = x.ObserveDate,
                VerifyStatus = x.VerifyStatus,
                CRCRemarks = x.CRCRemarks,
                ApprovedBy = x.ApprovedBy,
                ApprovedDate = x.ApprovedDate,
                FakeGuarantee = x.FakeGuarantee ?? false,
                HomeFake = x.HomeFake ?? false,
                HomeRental = x.HomeRental ?? false,
                InvolvementCase = x.InvolvementCase ?? false,
                LoseGuarantee = x.LoseGuarantee ?? false,
                ManageGuarantee = x.ManageGuarantee ?? false,
                OfficialFake = x.OfficialFake ?? false,
                PhotoCHQ = x.PhotoCHQ ?? false,
                PTOCase = x.PTOCase ?? false,
                WithoutVerification = x.WithoutVerification ?? false,
                WrongCase = x.WrongCase ?? false,
                WrongProduct = x.WrongProduct ?? false,
                WrongPTO = x.WrongPTO ?? false
            }).FirstOrDefault();
        }
        public async Task<List<LseAuditVM>> AuditList(int LocId, DateTime FromDate, DateTime ToDate, string Status, string VStatus)
        {
            try
            {
                var lst = await (from M in db.Lse_Master
                                 join D in db.Lse_Detail on M.AccNo equals D.AccNo
                                 join I in db.Itm_Master on D.SKUId equals I.SKUId into IS
                                 from I in IS.DefaultIfEmpty()
                                 join A in db.Lse_Audit on M.AccNo equals A.AccNo into AS
                                 from A in AS.DefaultIfEmpty()
                                 where
                                 M.LocId == LocId && (M.Status == 3 || M.Status == 4) && (D.SKUId == null || !I.IsPair) &&
                                 M.DeliveryDate >= FromDate && M.DeliveryDate <= ToDate
                                 group new { D, I } by new { A, M } into G
                                 select new LseAuditVM()
                                 {
                                     AccNo = G.Key.M.AccNo,
                                     AuditDate = G.Key.M.DeliveryDate,
                                     CustName = G.Key.M.CustName,
                                     ComName = G.Max(x => x.D.SKUId != null ? x.I.Itm_Model.Itm_Type.Itm_Company.ComName : x.D.OldCompany),
                                     ProductName = G.Max(x => x.D.SKUId != null ? x.I.Itm_Model.Itm_Type.Itm_Products.ProductName : x.D.OldProduct),
                                     Model = G.Max(x => x.D.SKUId != null ? x.I.Itm_Model.Model : x.D.OldModel),
                                     InstPrice = G.Key.M.InstPrice,
                                     Status = G.Key.A == null ? false : G.Key.A.Status,
                                     CRCRemarks = G.Key.A == null ? "" : G.Key.A.CRCRemarks,
                                     ApprovedBy = G.Key.A == null ? "" : G.Key.A.ApprovedBy,
                                     VerifyStatus = G.Key.A == null ? false : G.Key.A.VerifyStatus
                                 }).ToListAsync();
                lst = lst.Where(x => ((Status == "true" && x.Status) || (Status == "false" && !x.Status) || (Status == "all"))
                && ((VStatus == "true" && x.VerifyStatus) || (VStatus == "false" && !x.VerifyStatus) || (VStatus == "all"))
                ).ToList();
                return lst;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
        public async Task<LseAuditAVM> GetAudit(long AccNo)
        {
            try
            {
                var acc = await db.Lse_Master.FindAsync(AccNo);
                var aud = acc.Lse_Audit.FirstOrDefault();
                if (aud != null)
                {
                    var data = new LseAuditAVM
                    {
                        LocId = acc.LocId,
                        AccNo = acc.AccNo,
                        CustName = acc.CustName,
                        FName = acc.FName,
                        Mobile1 = acc.Mobile1,
                        Mobile2 = acc.Mobile2,
                        NICStatus = acc.NIC,
                        AffidavitStatus = acc.Lse_Customer.FirstOrDefault() == null ? false : acc.Lse_Customer.FirstOrDefault().Affidavit,
                        NoOfCheques = acc.Lse_Cheque.FirstOrDefault() == null ? 0 : acc.Lse_Cheque.Where(x => x.ChequeStatus).Count(),
                        NoOfGuarantors = acc.Lse_Guarantor.FirstOrDefault() == null ? 0 : acc.Lse_Guarantor.Where(x => x.Status).Count(),

                        Affidavit = aud.Affidavit,
                        AuditId = aud.AuditId,
                        BMSign = aud.BMSign,
                        Cheque = aud.Cheque,
                        Guarantor1 = aud.Guarantor1,
                        Guarantor2 = aud.Guarantor2,
                        Mobile = aud.Mobile,
                        NIC = aud.NIC,
                        ObserveDate = aud.ObserveDate,
                        ObserveState = aud.ObserveState,
                        Pic = aud.Pic,
                        RMSign = aud.RMSign,
                        Thumb = aud.Thumb,
                        VerifyStatus = aud.VerifyStatus,
                        VisitStatus = aud.VisitStatus,
                        Completion = aud.Completion,
                        ApprovedBy = aud.ApprovedBy,
                        ApprovedDate = aud.ApprovedDate,
                        CRCRemarks = aud.CRCRemarks,
                        FakeGuarantee = aud.FakeGuarantee ?? false,
                        HomeFake = aud.HomeFake ?? false,
                        HomeRental = aud.HomeRental ?? false,
                        InvolvementCase = aud.InvolvementCase ?? false,
                        LoseGuarantee = aud.LoseGuarantee ?? false,
                        ManageGuarantee = aud.ManageGuarantee ?? false,
                        OfficialFake = aud.OfficialFake ?? false,
                        PhotoCHQ = aud.PhotoCHQ ?? false,
                        PTOCase = aud.PTOCase ?? false,
                        WithoutVerification = aud.WithoutVerification ?? false,
                        WrongCase = aud.WrongCase ?? false,
                        WrongProduct = aud.WrongProduct ?? false,
                        WrongPTO = aud.WrongPTO ?? false
                    };
                    return data;
                }
                else
                {
                    var data = new LseAuditAVM
                    {
                        LocId = acc.LocId,
                        AccNo = acc.AccNo,
                        CustName = acc.CustName,
                        FName = acc.FName,
                        Mobile1 = acc.Mobile1,
                        Mobile2 = acc.Mobile2,
                        NICStatus = acc.NIC,
                        AffidavitStatus = acc.Lse_Customer.FirstOrDefault() == null ? false : acc.Lse_Customer.FirstOrDefault().Affidavit,
                        NoOfCheques = acc.Lse_Cheque.FirstOrDefault() == null ? 0 : acc.Lse_Cheque.Where(x => x.ChequeStatus).Count(),
                        NoOfGuarantors = acc.Lse_Guarantor.FirstOrDefault() == null ? 0 : acc.Lse_Guarantor.Where(x => x.Status).Count(),
                        AuditId = 0
                    };
                    return data;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<LseAuditVM> SaveAudit(LseAuditVM mod, int LocId, int UserId)
        {
            try
            {
                var tbl = await db.Lse_Audit.Where(x => x.AccNo == mod.AccNo).FirstOrDefaultAsync();
                if (tbl != null)
                {
                    Lse_AuditLog log = new Lse_AuditLog()
                    {
                        Affidavit = tbl.Affidavit,
                        AuditDate = tbl.AuditDate,
                        BMSign = tbl.BMSign,
                        Cheque = tbl.Cheque,
                        Completion = tbl.Completion,
                        CRCRemarks = tbl.CRCRemarks,
                        Guarantor1 = tbl.Guarantor1,
                        Guarantor2 = tbl.Guarantor2,
                        Mobile = tbl.Mobile,
                        NIC = tbl.NIC,
                        ObserveDate = tbl.ObserveDate,
                        ObserveState = tbl.ObserveState,
                        Pic = tbl.Pic,
                        RMSign = tbl.RMSign,
                        Status = tbl.Status,
                        Thumb = tbl.Thumb,
                        VerifyStatus = tbl.VerifyStatus,
                        VisitStatus = tbl.VisitStatus,
                        ApprovedBy = tbl.ApprovedBy,
                        ApprovedDate = tbl.ApprovedDate,
                        AccNo = tbl.AccNo,
                        AuditId = tbl.AuditId,
                        UserId = tbl.UserId,
                        TransDate = tbl.TransDate,
                        FakeGuarantee = tbl.FakeGuarantee,
                        HomeFake = tbl.HomeFake,
                        HomeRental = tbl.HomeRental,
                        InvolvementCase = tbl.InvolvementCase,
                        LoseGuarantee = tbl.LoseGuarantee,
                        ManageGuarantee = tbl.ManageGuarantee,
                        OfficialFake = tbl.OfficialFake,
                        PhotoCHQ = tbl.PhotoCHQ,
                        PTOCase = tbl.PTOCase,
                        WithoutVerification = tbl.WithoutVerification,
                        WrongCase = tbl.WrongCase,
                        WrongProduct = tbl.WrongProduct,
                        WrongPTO = tbl.WrongPTO
                    };
                    db.Lse_AuditLog.Add(log);

                    tbl.Affidavit = mod.Affidavit;
                    //tbl.AuditDate = setupBL.GetWorkingDate(LocId);
                    tbl.BMSign = mod.BMSign;
                    tbl.Cheque = mod.Cheque;
                    tbl.Completion = mod.Completion;
                    tbl.CRCRemarks = mod.CRCRemarks;
                    tbl.Guarantor1 = mod.Guarantor1;
                    tbl.Guarantor2 = mod.Guarantor2;
                    tbl.Mobile = mod.Mobile;
                    tbl.NIC = mod.NIC;
                    tbl.ObserveDate = mod.ObserveDate;
                    tbl.ObserveState = mod.ObserveState;
                    tbl.Pic = mod.Pic;
                    tbl.RMSign = mod.RMSign;
                    tbl.Status = true;
                    tbl.Thumb = mod.Thumb;
                    tbl.VerifyStatus = mod.VerifyStatus;
                    tbl.VisitStatus = mod.VisitStatus;
                    tbl.ApprovedBy = mod.ApprovedBy;
                    tbl.ApprovedDate = mod.ApprovedDate;
                    tbl.TransDate = DateTime.Now;
                    tbl.UserId = UserId;
                    tbl.FakeGuarantee = mod.FakeGuarantee;
                    tbl.HomeFake = mod.HomeFake;
                    tbl.HomeRental = mod.HomeRental;
                    tbl.InvolvementCase = mod.InvolvementCase;
                    tbl.LoseGuarantee = mod.LoseGuarantee;
                    tbl.ManageGuarantee = mod.ManageGuarantee;
                    tbl.OfficialFake = mod.OfficialFake;
                    tbl.PhotoCHQ = mod.PhotoCHQ;
                    tbl.PTOCase = mod.PTOCase;
                    tbl.WithoutVerification = mod.WithoutVerification;
                    tbl.WrongCase = mod.WrongCase;
                    tbl.WrongProduct = mod.WrongProduct;
                    tbl.WrongPTO = mod.WrongPTO;
                }
                else
                {
                    tbl = new Lse_Audit
                    {
                        AccNo = mod.AccNo,
                        Affidavit = mod.Affidavit,
                        AuditDate = setupBL.GetWorkingDate(LocId),
                        BMSign = mod.BMSign,
                        Cheque = mod.Cheque,
                        Completion = mod.Completion,
                        CRCRemarks = mod.CRCRemarks,
                        Guarantor1 = mod.Guarantor1,
                        Guarantor2 = mod.Guarantor2,
                        Mobile = mod.Mobile,
                        NIC = mod.NIC,
                        ObserveDate = mod.ObserveDate,
                        ObserveState = mod.ObserveState,
                        Pic = mod.Pic,
                        RMSign = mod.RMSign,
                        Status = true,
                        Thumb = mod.Thumb,
                        VerifyStatus = mod.VerifyStatus,
                        VisitStatus = mod.VisitStatus,
                        ApprovedBy = mod.ApprovedBy,
                        ApprovedDate = mod.ApprovedDate,
                        UserId = UserId,
                        TransDate = DateTime.Now,
                        FakeGuarantee = mod.FakeGuarantee,
                        HomeFake = mod.HomeFake,
                        HomeRental = mod.HomeRental,
                        InvolvementCase = mod.InvolvementCase,
                        LoseGuarantee = mod.LoseGuarantee,
                        ManageGuarantee = mod.ManageGuarantee,
                        OfficialFake = mod.OfficialFake,
                        PhotoCHQ = mod.PhotoCHQ,
                        PTOCase = mod.PTOCase,
                        WithoutVerification = mod.WithoutVerification,
                        WrongCase = mod.WrongCase,
                        WrongProduct = mod.WrongProduct,
                        WrongPTO = mod.WrongPTO
                    };
                    db.Lse_Audit.Add(tbl);
                }
                await db.SaveChangesAsync();
                mod.AuditId = tbl.AuditId;
                return mod;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return null;
            }
        }
        public async Task<List<LseAuditSummaryVM>> GetAuditCRCSummary(DateTime FromDate, DateTime ToDate, int SAuditorId)
        {
            try
            {
                //string auditorId = SAuditorId.ToString();
                var lst = await (from L in db.Pay_EmpHierarchy
                                 join M in db.Lse_Master on L.LocId equals M.LocId
                                 join A in db.Lse_Audit on M.AccNo equals A.AccNo into AS
                                 from A in AS.DefaultIfEmpty()
                                 where
                                 (L.SAuditorId == SAuditorId || L.RAuditorId == SAuditorId || L.CRCHeadId == SAuditorId || L.SAssAuditorId == SAuditorId ||
                                 L.BDMId == SAuditorId || L.RMId == SAuditorId || L.SRMId == SAuditorId || L.SSRMId == SAuditorId || L.DGMId == SAuditorId) && (M.Status == 3 || M.Status == 4) &&
                                 M.DeliveryDate >= FromDate && M.DeliveryDate <= ToDate
                                 select new
                                 {
                                     M.LocId,
                                     M.Comp_Locations.LocCode,
                                     M.Comp_Locations.LocName,
                                     L.AuditorId,
                                     L.Auditor,
                                     Status = A == null ? false : A.Status,
                                     VerifyStatus = A == null ? false : A.VerifyStatus
                                 }).ToListAsync();
                //join M in db.Lse_Master on L.LocId equals M.LocId
                //                 join A in db.Lse_Audit on M.AccNo equals A.AccNo into AS
                //                 from A in AS.DefaultIfEmpty()
                //                 where M.DeliveryDate >= FromDate && M.DeliveryDate <= ToDate && L.SAuditorId == auditorId
                //           group A by new { M.LocId,M.Comp_Locations.LocCode,M.Comp_Locations.LocName,L.AuditorId,L.Auditor} into g
                var ls = lst.GroupBy(x => new { x.LocId, x.LocCode, x.LocName, x.AuditorId, x.Auditor }).Select(
                               g => new LseAuditSummaryVM()
                               {
                                   LocId = g.Key.LocId,
                                   LocCode = g.Key.LocCode,
                                   LocName = g.Key.LocName,
                                   AuditorId = g.Key.AuditorId == null ? "" : g.Key.AuditorId.ToString(),
                                   Audit = g.Sum(x => !x.Status && !x.VerifyStatus ? 1 : 0),
                                   Auditor = g.Key.Auditor,
                                   Pending = g.Sum(x => x.Status && !x.VerifyStatus ? 1 : 0),
                                   Verified = g.Sum(x => x.VerifyStatus && x.Status ? 1 : 0)
                               }).ToList();

                return ls;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
        public async Task<List<LseAuditVM>> GetAuditCRC(DateTime FromDate, DateTime ToDate, int LocId)
        {
            try
            {
                DateTime defaultDate = Convert.ToDateTime("01-01-1900");
                var lst = await (from A in db.Lse_Master //on AD.AccNo equals A.AccNo
                                 join D in db.Lse_Detail on A.AccNo equals D.AccNo
                                 join I in db.Itm_Master on D.SKUId equals I.SKUId into IS
                                 from I in IS.DefaultIfEmpty()
                                 join AD in db.Lse_Audit on A.AccNo equals AD.AccNo into AIS
                                 from AD in AIS.DefaultIfEmpty()
                                 where A.LocId == LocId && A.DeliveryDate >= FromDate && A.DeliveryDate <= ToDate && (D.SKUId == null || !I.IsPair)
                                 select new LseAuditVM
                                 {
                                     AccNo = A.AccNo,
                                     Cheque = AD == null ? false : AD.Cheque,
                                     Affidavit = AD == null ? false : AD.Affidavit,
                                     AuditDate = AD == null ? defaultDate : AD.AuditDate,
                                     BMSign = AD == null ? false : AD.BMSign,
                                     Completion = AD == null ? 0 : AD.Completion,
                                     VisitStatus = AD == null ? false : AD.VisitStatus,
                                     AuditId = AD == null ? 0 : AD.AuditId,
                                     Guarantor1 = AD == null ? false : AD.Guarantor1,
                                     Guarantor2 = AD == null ? false : AD.Guarantor2,
                                     Mobile = AD == null ? false : AD.Mobile,
                                     NIC = AD == null ? false : AD.NIC,
                                     Pic = AD == null ? false : AD.Pic,
                                     RMSign = AD == null ? false : AD.RMSign,
                                     Thumb = AD == null ? false : AD.Thumb,
                                     VerifyStatus = AD == null ? false : AD.VerifyStatus,
                                     CRCRemarks = AD == null ? "" : AD.CRCRemarks,
                                     CustName = A.CustName,
                                     InstPrice = A.InstPrice,
                                     Status = AD == null ? false : AD.Status,
                                     ComName = D.SKUId != null ? I.Itm_Model.Itm_Type.Itm_Company.ComName : D.OldCompany,
                                     ProductName = D.SKUId != null ? I.Itm_Model.Itm_Type.Itm_Products.ProductName : D.OldProduct,
                                     Model = (D.SKUId != null ? I.Itm_Model.Model : D.OldModel),
                                     FakeGuarantee = AD == null ? false : AD.FakeGuarantee ?? false,
                                     HomeFake = AD == null ? false : AD.HomeFake ?? false,
                                     HomeRental = AD == null ? false : AD.HomeRental ?? false,
                                     InvolvementCase = AD == null ? false : AD.InvolvementCase ?? false,
                                     LoseGuarantee = AD == null ? false : AD.LoseGuarantee ?? false,
                                     ManageGuarantee = AD == null ? false : AD.ManageGuarantee ?? false,
                                     OfficialFake = AD == null ? false : AD.OfficialFake ?? false,
                                     PhotoCHQ = AD == null ? false : AD.PhotoCHQ ?? false,
                                     PTOCase = AD == null ? false : AD.PTOCase ?? false,
                                     WithoutVerification = AD == null ? false : AD.WithoutVerification ?? false,
                                     WrongCase = AD == null ? false : AD.WrongCase ?? false,
                                     WrongProduct = AD == null ? false : AD.WrongProduct ?? false,
                                     WrongPTO = AD == null ? false : AD.WrongPTO ?? false
                                 }).ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region PendingRecovery

        public async Task<PendingRecoveryVM> AccountPendingInfo(long InstId)
        {
            try
            {


                PendingRecoveryVM PenRec = new PendingRecoveryVM();
                if (InstId > 0)
                {
                    var _Installments = await db.Lse_Installment.Where(x => x.InstId == InstId).FirstOrDefaultAsync();
                    if (_Installments != null)
                    {
                        var _PendingRecoveryInst = db.Lse_PendingRecovery.Where(x => x.InstId == InstId).Sum(x => (decimal?)x.Amount) ?? 0;
                        var _RemaningToBePaid = (_Installments.InstCharges) - _PendingRecoveryInst;
                        PenRec.InstId = _Installments.InstId;
                        PenRec.MobileNumber = _Installments.Lse_Master.Mobile1;
                        PenRec.AccName = _Installments.Lse_Master.CustName;
                        PenRec.RemaningAmt = _RemaningToBePaid;
                        PenRec.AccNo = _Installments.AccNo;
                        PenRec.CNIC = _Installments.Lse_Master.NIC;
                        return PenRec;
                    }
                    else
                    {
                        return PenRec;
                    }
                }
                else
                {
                    return PenRec;
                }
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return null;
            }
        }

        public async Task<bool> SavePendingRecovery(PendingRecoveryVM mod, int userid)
        {
            try
            {


                if (mod != null)
                {
                    var inst = await AccountPendingInfo(mod.InstId);
                    if (inst.PaidAmt <= inst.RemaningAmt)
                    {
                        Lse_PendingRecovery data = new Lse_PendingRecovery()
                        {
                            Amount = mod.PaidAmt,
                            InstId = mod.InstId,
                            TransDate = DateTime.Now,
                            UserId = userid
                        };
                        db.Lse_PendingRecovery.Add(data);
                        await db.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }

        #endregion

        #region MobileAPILeaseCustomer
        public async Task<bool> UpdateLseCustomer(long AccNo, string ResAd, string OffAd)
        {
            try
            {
                var lseCustomer = await db.Lse_Customer.FirstOrDefaultAsync(x => x.AccNo == AccNo);
                if (lseCustomer != null)
                {
                    lseCustomer.ResAddress = ResAd;
                    lseCustomer.OffAddress = OffAd;
                    lseCustomer.TransDate = DateTime.Now;
                    await db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        public async Task<List<SKUVM>> GetSKUList()
        {
            try
            {
                return await (from x in db.Itm_Master
                              join S in db.Inv_Store on x.SKUId equals S.SKUId
                              where S.Inv_Status.MFact == 1
                              group x by new
                              {
                                  x.SKUId,
                                  x.Itm_Color.ColorName,
                                  x.Itm_Model.Itm_Type.Itm_Company.ComName,
                                  x.Itm_Model.Itm_Type.Itm_Products.ProductName,
                                  x.SKUCode,
                                  x.Itm_Model.Model,
                                  x.OtherSpecs,
                                  x.Description
                              } into g
                              select
                              new SKUVM
                              {
                                  SKUId = g.Key.SKUId,
                                  Color = g.Key.ColorName,
                                  Company = g.Key.ComName,
                                  Product = g.Key.ProductName,
                                  SKUCode = g.Key.SKUCode,
                                  Model = g.Key.Model,
                                  Specs = g.Key.OtherSpecs,
                                  Description = g.Key.Description
                              }).ToListAsync();

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Inv_StoreVM>> GetCurrentStockList(int LocId)
        {
            try
            {
                //int[] sta = new int[] { 1, 5, 6, 7 };
                return await db.Inv_Store.Where(x => x.LocId == LocId && x.Inv_Status.MFact == 1).Select(x =>
                new Inv_StoreVM
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
                    LocId = x.LocId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<LeaseCustomerVM>> GetLeaseCustomer(int LocId, int Status)
        {
            return await (from customer in db.Lse_Customer
                          join invcust in db.Lse_Master on customer.AccNo equals invcust.AccNo
                          //join invout in db.Lse_Outstand on customer.AccNo equals invout.AccNo
                          where customer.Status == true && invcust.Status == Status && invcust.LocId == LocId
                          //&& invout.OutstandDate.Month == DateTime.Now.Month && invout.OutstandDate.Year == DateTime.Now.Year
                          select new LeaseCustomerVM()
                          {
                              AccNo = customer.AccNo,
                              CustName = invcust.CustName,
                              OffAddress = customer.OffAddress,
                              ResAddress = customer.ResAddress,
                              Mobile1 = invcust.Mobile1,
                              Mobile2 = invcust.Mobile2
                          })
                         .ToListAsync();
            //return customers.Where(x => x.OutstandDate.Month == DateTime.Now.Month).Select(x => new LeaseCustomerVM()
            //{
            //    AccNo = x.AccNo,
            //    CustName = x.CustName,
            //    OffAddress = x.OffAddress,
            //    ResAddress = x.ResAddress,
            //    Mobile1 = x.Mobile1,
            //    Mobile2 = x.Mobile2
            //}).ToList();
        }
        public async Task<List<OutStandVM>> GetLeaseOutstand(int LocId, string Category, string Status)
        {
            var lst = await (from O in db.Lse_Outstand
                             join E in db.Pay_EmpMaster on O.RecoveryId equals E.EmpId into OS
                             from E in OS.DefaultIfEmpty()
                             where O.LocId == LocId && O.Category == Category
                             && O.OutstandDate.Month == DateTime.Now.Month && O.OutstandDate.Year == DateTime.Now.Year
                             && (O.Status == Status || Status == "")
                             //&& ((!O.RecoveryId.HasValue && Assign == "U") || (O.RecoveryId.HasValue && Assign == "A") || Assign == "")
                             select new OutStandVM
                             {
                                 AccNo = O.AccNo,
                                 OldAccNo = O.Lse_Master.OldAccNo ?? 0,
                                 Category = O.Category,
                                 OutStandAmt = O.OutstandAmt,
                                 OutStandDate = O.OutstandDate,
                                 RecoveryId = O.RecoveryId ?? 0,
                                 Status = O.Status,
                                 TransId = O.TransId,
                                 Customer = O.Lse_Master.CustName,
                                 Inst = O.Lse_Master.MonthlyInst,
                                 RecvAmt = O.RecvAmt ?? 0,
                                 Remaning = O.OutstandAmt - (O.RecvAmt ?? 0),
                                 Recovery = O.IsAIC ? "AIC (Office)" : E.EmpName
                             }).OrderBy(x => x.AccNo).ToListAsync();
            return lst;
        }
        public async Task<int> GetLocByIp(string IP)
        {
            try
            {
                string ip = IP.Substring(0, IP.LastIndexOf('.'));
                int lastIp = Convert.ToInt32(IP.Substring(IP.LastIndexOf('.') + 1));
                var locLst = await db.Comp_LocationIP.Where(x => x.Status).ToListAsync();
                var loc = locLst.Where(x => x.IP.Substring(0, x.IP.LastIndexOf('.')) == ip &&
               Convert.ToInt32(x.IP.Substring(x.IP.LastIndexOf('.') + 1)) <= lastIp && Convert.ToInt32(x.ToIP.Substring(x.ToIP.LastIndexOf('.') + 1)) >= lastIp).FirstOrDefault();
                if (loc != null)
                {
                    return loc.LocId;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<int> GetUtilityVersion()
        {
            try
            {
                return (await db.Comp_Profile.FirstOrDefaultAsync()).UtilityVersion ?? 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<List<LeaseCustomerVM>> GetLeaseCustomer(int RecoveryId, string Category)
        {
            try
            {


                //var customers = await (from customer in db.Lse_Customer
                //                       join invcust in db.Lse_Master on customer.AccNo equals invcust.AccNo
                //                       join invout in db.Lse_Outstand on customer.AccNo equals invout.AccNo
                //                       where customer.Status == true && invout.RecoveryId == RecoveryId

                var LocId = await db.Pay_EmpMaster.Where(x => x.EmpId == RecoveryId).Select(x => x.DeptId).SingleAsync();
                return db.spRep_OutstandingReportForMobile(LocId, DateTime.Now.Date, Category, RecoveryId).Select(x => new LeaseCustomerVM()
                {
                    AccNo = x.AccNo,
                    CustName = x.CustName,
                    OffAddress = "",
                    ResAddress = x.Address,
                    Mobile1 = x.Mobile1,
                    Mobile2 = x.Mobile2,
                    OutstandDate = DateTime.Now.Date,
                    Remarks = x.Remarks
                }).ToList();
                //return customers.Where(x => x.OutstandDate.Month == DateTime.Now.Month).Select(x => new LeaseCustomerVM()
                //{
                //    AccNo = x.AccNo,
                //    CustName = x.CustName,
                //    OffAddress = x.OffAddress,
                //    ResAddress = x.ResAddress,
                //    Mobile1 = x.Mobile1,
                //    Mobile2 = x.Mobile2
                //}).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateOutstandRemarks(long AccNo, string Remarks)
        {
            try
            {
                var outstand = await db.Lse_Outstand.Where(x => x.AccNo == AccNo
                && x.OutstandDate.Month == DateTime.Now.Month
                && x.OutstandDate.Year == DateTime.Now.Year).SingleOrDefaultAsync();
                if (outstand != null)
                {
                    outstand.Remarks = Remarks;
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        public async Task<List<MCustomerPurchaseVM>> GetCustomerPurchaseHistory(long accno)
        {
            return await (from lsedetail in db.Lse_Detail
                          join itemmaster in db.Itm_Master on lsedetail.SKUId equals itemmaster.SKUId
                          join itemmodel in db.Itm_Model on itemmaster.ModelId equals itemmodel.ModelId
                          join itemtype in db.Itm_Type on itemmodel.TypeId equals itemtype.TypeId
                          join itemproducts in db.Itm_Products on itemtype.ProductId equals itemproducts.ProductId
                          join itemcompany in db.Itm_Company on itemtype.ComId equals itemcompany.ComId
                          where lsedetail.AccNo == accno
                          select new MCustomerPurchaseVM()
                          {
                              Company = itemcompany.ComName,
                              Model = itemmodel.Model,
                              Product = itemproducts.ProductName,
                              SKU = itemmaster.SKUName,
                              InstallmentPrice = lsedetail.InstPrice

                          }).ToListAsync();
        }
        public async Task<List<MLatLng>> GetLatLng(long accno)
        {
            return await (from item in db.Lse_LatLng
                          where item.AccNo == accno && item.Status
                          select new MLatLng()
                          {
                              Lat = item.Lat,
                              Lng = item.Lng,
                              AccNo = item.AccNo,
                              Type = item.Type
                          }).ToListAsync();
        }
        public async Task<List<MGurantorVM>> GetCustomerGurantors(long accno)
        {
            return await (from item in db.Lse_Guarantor
                          where item.AccNo == accno && item.Status
                          select new MGurantorVM()
                          {
                              Name = item.Name,
                              FName = item.FName,
                              TelRes = item.TelRes,
                              TelOff = item.TelOff,
                              NIC = item.NIC,
                              Occupation = item.Occupation,
                              GRelation = item.GRelation,
                              OffAddress = item.OffAddress,
                              ResAddress = item.ResAddress
                          }).ToListAsync();
        }

        public async Task<List<MCustomerInstallmentsVM>> GetCustomerInstallments(long accno)
        {
            return await (from lseinstallment in db.Lse_Installment
                          join empmaster in db.Pay_EmpMaster on lseinstallment.RecoveryId equals empmaster.EmpId
                          where lseinstallment.AccNo == accno
                          group lseinstallment by new { AccNo = lseinstallment.AccNo, InstDate = lseinstallment.InstDate, Discount = lseinstallment.Discount, Fine = lseinstallment.Fine, FineType = lseinstallment.FineType, RcvdAmnt = lseinstallment.InstCharges, RecoveryOfficer = empmaster.EmpName } into custinstalls
                          select new MCustomerInstallmentsVM()
                          {
                              Date = custinstalls.Key.InstDate,
                              RcvdAmnt = custinstalls.Sum(x => x.InstCharges),
                              Installment = custinstalls.Key.RcvdAmnt,
                              Discount = custinstalls.Key.Discount,
                              Fine = custinstalls.Key.Fine,
                              FineType = custinstalls.Key.FineType,
                              RecoveryOfficer = custinstalls.Key.RecoveryOfficer

                          }).ToListAsync();
        }
        public async Task<string> GetLeaseCustomerName(long Accno)
        {
            try
            {
                return await db.Lse_Master.Where(x => x.AccNo == Accno).Select(x => x.CustName).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public async Task<MCustomerLedgerVM> GetLeaseCustomer(long Accno)
        {
            try
            {
                var CustomerInfo = new MCustomerLedgerVM();
                CustomerInfo.CustomerInstallments = await GetInstByAcc(Accno);
                CustomerInfo.CustomerInfo = (await new ReportBL().GetCustomerInfo(Accno)).FirstOrDefault();
                CustomerInfo.CustomerGuarantor = await GetCustomerGurantors(Accno);
                CustomerInfo.LatLng = await GetLatLng(Accno);
                return CustomerInfo;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public async Task<bool> UpdateLatLng(long AccNo, string Type, decimal Lat, decimal Lng)
        {
            try
            {
                var latlng = await db.Lse_LatLng.Where(x => x.AccNo == AccNo && x.Type == Type && x.Status).FirstOrDefaultAsync();
                if (latlng != null)
                {
                    latlng.Status = false;
                }
                var ll = new Lse_LatLng()
                {
                    AccNo = AccNo,
                    Lat = Lat,
                    Lng = Lng,
                    Status = true,
                    TransDate = DateTime.Now,
                    Type = Type
                };
                db.Lse_LatLng.Add(ll);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }

        }
        #endregion

        #region ClosingVoucher

        public List<ClosingVoucherVM> GetClosingVoucher(DateTime month, int locid)
        {
            try
            {
                return db.spPay_Closing_Voucher(month, locid).Select(x => new ClosingVoucherVM()
                {
                    Amount = x.Amount,
                    Head = x.Head,
                    Sr = x.Sr
                }).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public List<ClosingVoucherAdjVM> GetClosingVoucherAdj(DateTime month, int locid)
        {
            try
            {
                return db.spPay_Closing_Voucher_Adj(month, locid).Select(x => new ClosingVoucherAdjVM()
                {
                    Sr = x.Sr,
                    InstCharges = x.InstCharges,
                    PaidBy = x.PaidBy
                }).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //public List<Pay_SalarySheet_VD> GetClosingVoucherList(int locid, DateTime mon)
        //{
        //    return db.Pay_SalarySheet_VD.Where(x => x.DSMonth.Month == mon.Month && x.DSMonth.Year == mon.Year && x.LocId == locid).Distinct().ToList();
        //}
        public List<ClosingVoucherDistVM> GetCVExisting(List<ClosingVoucherDistVM> mon, DateTime month, int locid)
        {
            foreach (var item in mon)
            {
                var ExistVal = db.Pay_SalarySheet_VD.Where(x => x.DSMonth.Month == month.Month
                                                       && x.DSMonth.Year == month.Year
                                                       && x.EmpId == item.EmpId
                                                       && x.DesgId == item.DesgId
                                                       && x.LocId == locid).FirstOrDefault();
                if (ExistVal != null)
                {
                    item.ClosingAdv = ExistVal.ClosingAdv;
                    if (ExistVal.ApprovedAdv == null)
                    {
                        item.ApprovedAdv = ExistVal.ClosingAdv;
                    }
                    else
                    {
                        item.ApprovedAdv = ExistVal.ApprovedAdv;
                        item.ApprovedBy = ExistVal.ApprovedBy;
                        item.ApprovedDate = ExistVal.ApprovedDate;
                    }



                    item.DsId = ExistVal.DSId;
                }
            }
            return mon;
        }

        public List<ClosingVoucherDistVM> GetClosingVoucherDist(DateTime month, int locid)
        {
            try
            {
                var lst = db.spPay_Closing_Voucher_Dist(month, locid).Select(x => new ClosingVoucherDistVM()
                {

                    ClosingAdv = x.ClosingAdv,
                    CNIC = x.CNIC,
                    DeptId = x.DeptId,
                    DesgId = x.DesgId,
                    DesgName = x.DesgName,
                    EmpId = x.EmpId,
                    EmpLedgerBal = x.EmpLedgerBal,
                    EmpName = x.EmpName,
                    EmpStatus = x.EmpStatus,
                    NetSalary = x.NetSalary,
                    Outstand = x.Outstand,
                    SortOrder = x.SortOrder
                }).ToList();

                var FLst = GetCVExisting(lst, month, locid);
                return FLst;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> AddClosingVoucher(List<ClosingVoucherDistVM> mod, int LocId, int LcId, DateTime mon, int UserId)
        {
            try
            {
                foreach (var item in mod)
                {
                    //if (item.ClosingAdv <= item.NetSalary && item.ClosingAdv > 0)
                    if (item.ClosingAdv >= 0)
                    {
                        if (item.DsId == 0)
                        {
                            if (LcId == 72)
                            {
                                Pay_SalarySheet_VD md = new Pay_SalarySheet_VD()
                                {
                                    ClosingAdv = item.ClosingAdv,
                                    DesgId = item.DesgId,
                                    EmpId = item.EmpId,
                                    LedgerBal = item.EmpLedgerBal,
                                    LocId = LocId,
                                    Outstand = item.Outstand,
                                    Salary = Convert.ToDecimal(item.NetSalary),
                                    CreatedBy = UserId,
                                    CreatedDate = DateTime.Now,
                                    DSMonth = mon,
                                    ApprovedAdv = item.ApprovedAdv,
                                    ApprovedBy = UserId,
                                    ApprovedDate = DateTime.Now
                                };
                                db.Pay_SalarySheet_VD.Add(md);
                            }
                            else
                            {
                                Pay_SalarySheet_VD md = new Pay_SalarySheet_VD()
                                {
                                    ClosingAdv = item.ClosingAdv,
                                    DesgId = item.DesgId,
                                    EmpId = item.EmpId,
                                    LedgerBal = item.EmpLedgerBal,
                                    LocId = LocId,
                                    Outstand = item.Outstand,
                                    Salary = Convert.ToDecimal(item.NetSalary),
                                    CreatedBy = UserId,
                                    CreatedDate = DateTime.Now,
                                    DSMonth = mon
                                };
                                db.Pay_SalarySheet_VD.Add(md);
                            }
                        }
                        else
                        {
                            var Exist = await db.Pay_SalarySheet_VD.Where(x => x.DSId == item.DsId).FirstOrDefaultAsync();
                            if (Exist != null)
                            {
                                Exist.ClosingAdv = item.ClosingAdv;
                                if (item.ApprovedAdv != null && LcId == 72)
                                {
                                    Exist.ApprovedAdv = item.ApprovedAdv;
                                    Exist.ApprovedBy = UserId;
                                    Exist.ApprovedDate = DateTime.Now;
                                }
                            }
                        }

                    }
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }

        #endregion

        #region SourceChange

        public async Task<List<InstallmentVM>> GetInstallmentSources(int locid)
        {
            var now = DateTime.Now;
            var first = new DateTime(now.Year, now.Month, 1);
            var last = first.AddMonths(1).AddDays(-1);
            var AllInst = await db.Lse_Installment.Where(x => x.InstDate >= first && x.InstDate <= last && x.LocId == locid).Select(x => new InstallmentVM()
            {
                InstId = x.InstId,
                PaidBy = x.PaidBy,
                InstCharges = x.InstCharges,
                InstDate = x.InstDate,
                AccNo = x.AccNo,
                Remarks = x.Remarks
            }).ToListAsync();
            return AllInst;
        }

        public async Task<bool> UpdateSource(long Instid, long accno, int PaidBy, string Remark, int UserId)
        {
            try
            {
                var inst = await db.Lse_Installment.Where(x => x.InstId == Instid).FirstOrDefaultAsync();
                if (inst != null)
                {
                    db.Lse_InstallmentLog.Add(new Lse_InstallmentLog
                    {
                        AccNo = inst.AccNo,
                        Discount = inst.Discount,
                        InstCharges = inst.InstCharges,
                        InstDate = inst.InstDate,
                        InstId = inst.InstId,
                        LocId = inst.LocId,
                        Remarks = inst.Remarks,
                        TransDate = inst.TransDate,
                        UserId = inst.UserId,
                        PaidBy = inst.PaidBy
                    });
                    inst.PaidBy = PaidBy;
                    inst.Remarks = Remark;
                    inst.TransDate = DateTime.Now;
                    inst.UserId = UserId;
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        #endregion

        public bool IsAccLocked(long AccNo)
        {
            try
            {
                return db.Lse_Master.Where(x => x.AccNo == AccNo).Select(x => x.IsLock).FirstOrDefault();
            }
            catch (Exception)
            {
                return true;
            }
        }

        #region VPNSearch
        public List<VPNSearchVM> GetVPNSearch(int LocId, int Crit1, string CritVal1, int Status, int UserId)
        {
            try
            {
                return db.spget_AccountSearchAll(LocId, Status, Crit1, CritVal1, UserId).Select(x => new
                    VPNSearchVM
                {
                    AccNo = x.AccNo ?? 0,
                    Customer = x.Customer,
                    DeliveryDate = x.DeliveryDate ?? DateTime.Now.Date,
                    FName = x.FName,
                    InstPrice = x.InstPrice,
                    Mobile1 = x.Mobile1,
                    Mobile2 = x.Mobile2,
                    MonthlyInst = x.MonthlyInst,
                    NIC = x.NIC,
                    Status = x.Status,
                    Type = x.Type,
                    LocCode = x.LocCode
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<CashSaleSummarySaleManWiseVM> GetCashSaleSummarySaleManWise(int locid, DateTime FromDate, DateTime Todate)
        {
            return db.spRep_SalemanWiseCashSaleSummary(locid, FromDate, Todate).Select(x => new CashSaleSummarySaleManWiseVM()
            {
                LocName = x.LocName,
                Qty = x.Qty,
                RetQty = x.RetQty,
                RetValue = x.RetValue,
                SaleVale = x.SaleVale,
                Saleman = x.Saleman,
                SalemanId = x.SalemanId,
            }).ToList();
        }
        public async Task<bool> GetBlockCustomer(int Criteria, string CriteriaValue)
        {
            try
            {
                if (Criteria == 1)
                {
                    var cust = await db.Lse_BlockCustomer.Where(x => x.CNIC == CriteriaValue && x.Status == true).FirstOrDefaultAsync();
                    if (cust != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (Criteria == 2)
                {
                    var cust = await db.Lse_BlockCustomer.Where(x => x.MobileNo1 == CriteriaValue || x.MobileNo2 == CriteriaValue && x.Status == true).FirstOrDefaultAsync();
                    if (cust != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
        public async Task<List<Lse_SearchCriteria>> VPNGetSearchCriteria()
        {
            try
            {
                return await db.Lse_SearchCriteria.OrderBy(x => x.SortOrder).Where(x => x.RowId == 42 || x.RowId == 49 || x.RowId == 74 || x.RowId == 41 || x.RowId == 48 || x.RowId == 75 || x.RowId == 77).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }





        #endregion

        #region Customer Editing Rights
        public async Task<List<LSECERVM>> GetAssignAcc(int LocId)
        {
            try
            {
                var dt = DateTime.Now.Date;
                return await (from item in db.Lse_CER
                              join lsemas in db.Lse_Master on item.AccNo equals lsemas.AccNo
                              where item.LocId == LocId
                              && item.WorkingDate == dt
                              && item.Status
                              select new LSECERVM()
                              {
                                  AccNo = item.AccNo,
                                  Customer = lsemas.CustName
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<LSECERVM>> GetLseCER(int LocId)
        {
            try
            {
                return await (from item in db.Lse_Master
                                  //join lsemas in db.Lse_CER on item.AccNo equals lsemas.AccNo into ps
                                  //from p in ps.DefaultIfEmpty()
                              where item.Status == 3 && item.LocId == LocId
                              select new LSECERVM()
                              {
                                  AccNo = item.AccNo
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddEditLSECER(List<long> modlst, DateTime WorkingdDate, int UserId, int LocId)
        {
            try
            {
                foreach (var item in modlst)
                {
                    var IsExistAccNo = await db.Lse_CER.Where(x => x.AccNo == item && x.WorkingDate == WorkingdDate && x.LocId == LocId && x.Status).FirstOrDefaultAsync();
                    if (IsExistAccNo == null)
                    {
                        //    IsExistAccNo.Status = true;
                        //    await db.SaveChangesAsync();
                        //}
                        //else
                        //{
                        Lse_CER mod = new Lse_CER()
                        {
                            AccNo = item,
                            LocId = LocId,
                            Status = true,
                            TransDate = DateTime.Now,
                            WorkingDate = WorkingdDate,
                            UserId = UserId
                        };
                        db.Lse_CER.Add(mod);
                        await db.SaveChangesAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }

        public async Task<bool> LSECERDestroy(LSECERVM mod, int UserId)
        {
            try
            {
                var ExstCER = await db.Lse_CER.Where(x => x.AccNo == mod.AccNo).FirstOrDefaultAsync();
                if (ExstCER != null)
                {
                    ExstCER.Status = false;
                    ExstCER.UserId = UserId;
                    ExstCER.TransDate = DateTime.Now;
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

        public async Task<bool> LSECERAddRemove(LSECERVM mod)
        {
            try
            {
                DateTime workdte = setupBL.GetWorkingDate(mod.LocId);
                if (mod.Status == true)
                {
                    var IsExistAccNo = await db.Lse_CER.Where(x => x.AccNo == mod.AccNo && x.WorkingDate == workdte && x.LocId == mod.LocId && x.Status).FirstOrDefaultAsync();
                    if (IsExistAccNo == null)
                    {
                        Lse_CER tbl = new Lse_CER()
                        {
                            AccNo = mod.AccNo,
                            LocId = mod.LocId,
                            Status = true,
                            TransDate = DateTime.Now,
                            WorkingDate = workdte,
                            UserId = mod.UserId
                        };
                        db.Lse_CER.Add(tbl);
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
                else
                {
                    var ExstCER = await db.Lse_CER.Where(x => x.AccNo == mod.AccNo && x.WorkingDate == workdte && x.LocId == mod.LocId && x.Status).FirstOrDefaultAsync();
                    if (ExstCER != null)
                    {
                        ExstCER.Status = false;
                        ExstCER.UserId = mod.UserId;
                        ExstCER.TransDate = DateTime.Now;
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion
        public async Task<List<LseDetailVM>> LseDetailList(long AccNo)
        {
            try
            {
                var lst = await db.Lse_Detail.Where(x => x.Status && x.AccNo == AccNo && x.IsReturned == false).Select(x =>
                 new LseDetailVM
                 {
                     //AccNo = x.AccNo,
                     DtlId = x.DtlId,
                     InstPrice = x.InstPrice,
                     ItemId = x.ItemId ?? 0,
                     Qty = x.Qty,
                     SKUId = x.SKUId ?? 0,
                     SKUName = x.Itm_Master.SKUName,
                     SerialNo = "",
                     Discount = x.Discount,
                     TPrice = x.InstPrice,
                     InstPlanId = x.InstPlanId ?? 0,
                     dAdvance = x.dAdvance ?? 0,
                     dInst = x.dInst ?? 0,
                     PlanType = x.PlanType,
                     Status = x.Itm_Master.Itm_Model.Itm_Type.ProductId == 307
                 }).ToListAsync();
                foreach (var item in lst)
                {
                    if (item.InstPlanId > 0)
                    {
                        if (item.PlanType == "S")
                        {
                            var plan = await db.Itm_SerialPlan.FindAsync(item.InstPlanId);
                            item.dAdvance = plan.Advance;
                            item.dInst = plan.Inst;
                        }
                        else
                        {
                            var plan = await db.Itm_SKUPlan.FindAsync(item.InstPlanId);
                            item.dAdvance = plan.Advance;
                            item.dInst = plan.Inst;
                        }

                    }
                    if (item.ItemId > 0)
                    {
                        item.SerialNo = db.Inv_Store.FindAsync(item.ItemId).Result.SerialNo;
                    }
                }
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> SaveAdvanceNew(IEnumerable<LseDetailVM> det, ProcessingVM mod, int UserId)
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("DtlId", typeof(long));
                dataTable.Columns.Add("AccNo", typeof(long));
                dataTable.Columns.Add("SKUId", typeof(int));
                dataTable.Columns.Add("ItemId", typeof(long));
                dataTable.Columns.Add("Qty", typeof(int));
                dataTable.Columns.Add("Discount", typeof(decimal));
                dataTable.Columns.Add("InstPrice", typeof(decimal));
                dataTable.Columns.Add("Status", typeof(bool));
                dataTable.Columns.Add("dAdvance", typeof(decimal));
                dataTable.Columns.Add("dInst", typeof(decimal));
                dataTable.Columns.Add("InstPlanId", typeof(long));
                dataTable.Columns.Add("PlanType", typeof(string));

                string reason = "";
                foreach (var item in det)
                {
                    if (item.CSerialNo != null)
                    {
                        reason = item.CSerialNo;
                    }
                    dataTable.Rows.Add(item.DtlId, mod.AccNo, item.SKUId, item.ItemId, item.Qty, item.Discount, item.InstPrice, item.Status
                        , item.dAdvance, item.dInst, item.InstPlanId, item.PlanType);
                }


                List<SqlParameter> Parameters = new List<SqlParameter>();

                Parameters.Add(new SqlParameter("udt", SqlDbType.Structured) { Value = dataTable, TypeName = "dbo.LseDetailTY" });
                Parameters.Add(new SqlParameter("AccNo", SqlDbType.BigInt) { Value = mod.AccNo });
                Parameters.Add(new SqlParameter("LocId", SqlDbType.Int) { Value = mod.LocId });
                Parameters.Add(new SqlParameter("CategoryId", SqlDbType.Int) { Value = mod.CategoryId });
                //Parameters.Add(new SqlParameter("ProcessAt", SqlDbType.VarChar) { Value = mod.ProcessAt ?? "" });
                Parameters.Add(new SqlParameter("ProcessFee", SqlDbType.Decimal) { Value = mod.ProcessFee });
                //Parameters.Add(new SqlParameter("CustName", SqlDbType.VarChar) { Value = mod.CustName });
                //Parameters.Add(new SqlParameter("FName", SqlDbType.VarChar) { Value = mod.FName });
                //Parameters.Add(new SqlParameter("Mobile1", SqlDbType.VarChar) { Value = mod.Mobile1 });
                //Parameters.Add(new SqlParameter("Mobile2", SqlDbType.VarChar) { Value = mod.Mobile2 });
                //Parameters.Add(new SqlParameter("NIC", SqlDbType.VarChar) { Value = mod.NIC });
                Parameters.Add(new SqlParameter("InstPrice", SqlDbType.Decimal) { Value = mod.InstPrice });
                Parameters.Add(new SqlParameter("ActualAdvance", SqlDbType.Decimal) { Value = mod.ActualAdvance });
                Parameters.Add(new SqlParameter("Advance", SqlDbType.Decimal) { Value = mod.Advance });
                Parameters.Add(new SqlParameter("MonthlyInst", SqlDbType.Decimal) { Value = mod.MonthlyInst });
                Parameters.Add(new SqlParameter("Duration", SqlDbType.Int) { Value = mod.Duration });
                Parameters.Add(new SqlParameter("MktOfficerId", SqlDbType.Int) { Value = mod.MktOfficerId });
                Parameters.Add(new SqlParameter("InqOfficerId", SqlDbType.Int) { Value = mod.InqOfficerId });
                Parameters.Add(new SqlParameter("ManagerId", SqlDbType.Int) { Value = mod.ManagerId });
                Parameters.Add(new SqlParameter("SManager", SqlDbType.Int) { Value = mod.SManagerId });
                Parameters.Add(new SqlParameter("OldAccNo", SqlDbType.Int) { Value = mod.OldAccNo });
                Parameters.Add(new SqlParameter("UserId", SqlDbType.Int) { Value = UserId });
                Parameters.Add(new SqlParameter("OrderId", SqlDbType.BigInt) { Value = mod.OrderId });
                Parameters.Add(new SqlParameter("OrderAmount", SqlDbType.Decimal) { Value = mod.OrderAmount });
                Parameters.Add(new SqlParameter("Reason", SqlDbType.VarChar) { Value = reason });
                //db.Database.ExecuteSqlCommand("Select * from @AName", Parameters.ToArray());
                //List<LseDetailTY> param = new List<LseDetailTY>();
                //db.Database.CommandTimeout = 3600;
                var r = db.Database.SqlQuery<string>("exec spSet_SaveAdvance @udt,@AccNo,@LocId,@CategoryId," +
                    //"@ProcessAt," +
                    "@ProcessFee," +
                    //"@CustName,@FName,@Mobile1,@Mobile2,@NIC," +
                    "@InstPrice,@ActualAdvance,@Advance,@MonthlyInst,@Duration,@MktOfficerId," +
                    "@InqOfficerId,@ManagerId,@SManager,@OldAccNo,@UserId,@OrderId,@OrderAmount,@Reason", Parameters.ToArray()).FirstOrDefault();
                //var ls = db.spSet_SaveAdvance()

                return r.ToString();
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        public async Task<long> SaveAdvance(IEnumerable<LseDetailVM> det, ProcessingVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var tbl = await db.Lse_Master.Where(x => x.AccNo == mod.AccNo && x.LocId == mod.LocId).SingleAsync();
                    if (tbl.Status >= 3)
                    {
                        scop.Dispose();
                        return 0;
                    }
                    if (mod.InstPrice == 0 || mod.ManagerId == 0 || mod.MktOfficerId == 0 || mod.Advance == 0 || mod.ActualAdvance == 0 || mod.Duration == 0 || mod.InqOfficerId == 0 || mod.SManagerId == 0)
                    {
                        scop.Dispose();
                        return 0;
                    }

                    tbl.ManagerId = mod.ManagerId;
                    tbl.SManagerId = mod.SManagerId;
                    tbl.MktOfficerId = mod.MktOfficerId;
                    tbl.Advance = mod.Advance;
                    tbl.ActualAdvance = mod.ActualAdvance;
                    tbl.Duration = mod.Duration;
                    tbl.InqOfficerId = mod.InqOfficerId;
                    tbl.InstPrice = mod.InstPrice;
                    tbl.MonthlyInst = Math.Round((mod.InstPrice - mod.ActualAdvance) / (mod.Duration - 1));
                    tbl.ProcessFee = mod.ProcessFee;
                    tbl.Remarks = mod.Remarks;
                    tbl.Status = 3;
                    tbl.UserId = UserId;
                    tbl.CategoryId = mod.CategoryId;
                    tbl.DeliveryDate = setupBL.GetWorkingDate(mod.LocId);
                    tbl.DeliveryTransDate = DateTime.Now;
                    tbl.OldAccNo = mod.OldAccNo;
                    tbl.Type = "N";

                    if (tbl.Advance < tbl.ActualAdvance || tbl.ActualAdvance < tbl.MonthlyInst)
                    {
                        scop.Dispose();
                        return 0;
                    }


                    var lastSerial = await db.Lse_Master.Where(x => x.LocId == mod.LocId && x.DeliveryDate != null).MaxAsync(x => x.AccSr) ?? 0;
                    tbl.AccSr = lastSerial + 1;

                    var detailIds = det.Select(x => x.DtlId).ToList();
                    var dlst = await db.Lse_Detail.Where(x => x.AccNo == mod.AccNo && !detailIds.Contains(x.DtlId)).ToListAsync();
                    foreach (var v in dlst)
                    {
                        db.Lse_Detail.Remove(v);
                    }

                    foreach (var v in det)
                    {
                        //if (v.ItemId > 0)
                        //{
                        var item = await db.Inv_Store.FindAsync(v.ItemId);
                        int prevStatus = item.StatusID;
                        if (item.Inv_Status.MFact == -1)
                        {
                            scop.Dispose();
                            return 0;
                        }
                        item.SPrice = v.InstPrice;
                        item.StatusID = 2;

                        decimal basePrice = 0;
                        if (v.PlanType == "S")
                        {
                            var pri = await db.Itm_SerialPlan.FirstOrDefaultAsync(x => x.RowId == v.InstPlanId);
                            basePrice = pri == null ? 0 : pri.BasePrice;
                        }
                        else if (v.PlanType == "M")
                        {
                            var pri = await db.Itm_SKUPlan.FirstOrDefaultAsync(x => x.RowId == v.InstPlanId);
                            basePrice = pri == null ? 0 : pri.BasePrice;
                        }




                        var inst = db.Lse_Detail.Where(x => x.DtlId == v.DtlId && x.AccNo == mod.AccNo).FirstOrDefault();
                        if (inst != null)
                        {
                            inst.SKUId = item.SKUId;
                            inst.ItemId = item.ItemId;
                            inst.InstPrice = v.InstPrice;
                            inst.Status = true;
                            inst.Discount = v.Discount;
                            inst.MRP = item.MRP;
                            inst.SM = basePrice;
                            inst.PPrice = item.PPrice;
                            inst.InstPlanId = v.InstPlanId;
                            inst.dAdvance = v.dAdvance;
                            inst.dInst = v.dInst;
                            inst.Tax = (item.Exempted ?? false) ? 0 : Math.Round(item.MRP - (item.MRP * 100 / ((item.Tax ?? 0) + 100)));
                            inst.dDate = tbl.DeliveryDate;
                            inst.PlanType = v.PlanType;
                            inst.PrevStatus = prevStatus;
                        }
                        else
                        {
                            db.Lse_Detail.Add(new Lse_Detail
                            {
                                AccNo = mod.AccNo,
                                IsReturned = false,
                                InstPrice = v.InstPrice,
                                Qty = 1,
                                SKUId = item.SKUId,
                                Status = true,
                                ItemId = item.ItemId,
                                Discount = v.Discount,
                                MRP = item.MRP,
                                SM = basePrice,
                                PPrice = item.PPrice,
                                InstPlanId = v.InstPlanId,
                                dAdvance = v.dAdvance,
                                dInst = v.dInst,
                                Tax = (item.Exempted ?? false) ? 0 : Math.Round(item.MRP - (item.MRP * 100 / ((item.Tax ?? 0) + 100))),
                                dDate = tbl.DeliveryDate,
                                PlanType = v.PlanType,
                                PrevStatus = prevStatus
                            });
                        }
                        //var stHist = await setupBL.CreateStoreHistory((DateTime)tbl.DeliveryDate, item.ItemId, item.LocId, -1, item.MRP, item.PPrice, item.SKUId, (pri == null ? 0 : pri.BasePrice), v.InstPrice, (DateTime)tbl.DeliveryTransDate, "Installment Sale", tbl.UserId);
                        //if (!stHist)
                        //{
                        //    scop.Dispose();
                        //    return 0;
                        //}
                        //}
                    }

                    await db.SaveChangesAsync();

                    var lst = await db.Lse_Detail.Where(x => x.AccNo == mod.AccNo).ToListAsync();
                    foreach (var v in lst)
                    {
                        var item = await db.Inv_Store.FindAsync(v.ItemId);
                        Inv_StoreHistory stoHis = new Inv_StoreHistory
                        {
                            DocDate = (DateTime)tbl.DeliveryDate,
                            ItemId = item.ItemId,
                            LocId = item.LocId,
                            MFact = -1,
                            MRP = item.MRP,
                            PPrice = item.PPrice,
                            Qty = 1,
                            SKUId = item.SKUId,
                            SMPrice = v.SM,
                            SPrice = v.InstPrice,
                            TransDate = (DateTime)tbl.DeliveryTransDate,
                            Type = "Installment Sale",
                            UserId = UserId,
                            RefId = tbl.AccNo
                        };
                        db.Inv_StoreHistory.Add(stoHis);

                        //var prii = await db.Itm_SKUPlan.FindAsync(v.InstPlanId);
                        //if (prii != null)
                        //{
                        //    if (prii.Type == "S" || prii.Type == "A")
                        //    {
                        //        prii.ModifiedBy = UserId;
                        //        prii.ModifiedDate = DateTime.Now;
                        //        prii.Status = false;
                        //        prii.Remarks = (prii.Remarks ?? "") + " Auto Close";
                        //    }
                        //}
                        //if (v.PlanType == "S" && v.InstPlanId > 0)
                        //{
                        //    var plan = await db.Itm_SerialPlan.FirstOrDefaultAsync(x => x.RowId == v.InstPlanId);
                        //    if (plan != null)
                        //    {
                        //        plan.ModifiedBy = UserId;
                        //        plan.ModifiedDate = DateTime.Now;
                        //        plan.Status = false;
                        //        plan.Remarks = (plan.Remarks ?? "") + " Auto Close";
                        //    }
                        //}
                    }
                    await db.SaveChangesAsync();



                    scop.Complete();
                    scop.Dispose();
                    return tbl.AccNo;
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    return 0;
                }
            }
        }

       
        public async Task<bool> UpdateInstallment(int UserId, long Accno, string Remarks, InstDetailVM mod)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var upd = await db.Lse_Installment.FirstOrDefaultAsync(x => x.InstId == mod.TransId && x.AccNo == Accno);
                    if (upd != null)
                    {
                        var obj = new { upd.AccNo, upd.InstCharges, upd.Discount, upd.Fine, upd.FineType };

                        Lse_InstallmentLog log = new Lse_InstallmentLog
                        {
                            AccNo = upd.AccNo,
                            Discount = upd.Discount,
                            InstCharges = upd.InstCharges,
                            InstDate = upd.InstDate,
                            InstId = upd.InstId,
                            LocId = upd.LocId,
                            Remarks = upd.Remarks,
                            TransDate = upd.TransDate,
                            UserId = upd.UserId,
                            PaidBy = upd.PaidBy
                        };
                        db.Lse_InstallmentLog.Add(log);

                        upd.InstCharges = 0;
                        upd.Discount = 0;
                        //upd.Fine = mod.Fine;
                        //upd.FineType = mod.FineType;
                        await db.SaveChangesAsync();
                        if(!await UpdateOutStand(Accno, upd.InstDate, upd.InstId))
                        {
                            scop.Dispose();
                            return false;
                        }

                        var acc = await db.Lse_Master.FindAsync(mod.AccNo);
                        var totRecv = await db.Lse_Installment.Where(x => x.AccNo == mod.AccNo).SumAsync(x => (decimal?)(x.InstCharges + x.Discount)) ?? 0;
                        totRecv = totRecv + acc.Advance;
                        if (totRecv < acc.InstPrice && acc.Status == 4)
                        {
                            acc.Status = 3;
                            acc.CloseDate = null;
                            acc.CloseTransDate = null;
                            await db.SaveChangesAsync();
                        }

                        if (!await setupBL.LogModification(UserId, "Installment", obj.ToString(), Remarks, upd.InstId))
                        {
                            scop.Dispose();
                            return false;
                        }
                        if (upd.IsPosted)
                        {
                            if(await new AccountBL().InstallmentAdjustmentVoucher(mod.TransId, obj.InstCharges, obj.Discount, UserId))
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
                    return false;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }
        public async Task<bool> UpdateOutStand(long AccNo, DateTime WorkingDate, long InstId)
        {
            try
            {
                var Recovery = db.Lse_Outstand.Where(x => x.AccNo == AccNo && x.OutstandDate.Month == WorkingDate.Month && x.OutstandDate.Year == WorkingDate.Year).FirstOrDefault();
                if (Recovery != null)
                {
                    var TotalRecv = db.Lse_Installment.Where(x => x.AccNo == AccNo && x.InstDate.Month == WorkingDate.Month && x.InstDate.Year == WorkingDate.Year).Sum(x => (decimal?)(x.InstCharges + x.Discount)) ?? 0;
                    if (Recovery.OutstandAmt <= TotalRecv)
                    {
                        Recovery.RecvAmt = Recovery.OutstandAmt;
                        Recovery.Status = "C";
                        Recovery.InstId = InstId;
                        Recovery.InstDate = WorkingDate;
                    }
                    else
                    {
                        Recovery.RecvAmt = TotalRecv;
                        Recovery.Status = "P";
                        Recovery.InstId = InstId;
                        Recovery.InstDate = WorkingDate;
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



        #region Installment
        public async Task<long> EditInstallment(InstDetailVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var acc = await db.Lse_Master.FindAsync(mod.AccNo);
                    var workingDate = setupBL.GetWorkingDate(acc.LocId);
                    var InstP = await db.Lse_Installment.Where(x => x.InstId == mod.TransId && x.InstDate == workingDate && x.IsPosted == false).FirstOrDefaultAsync();
                    if (InstP != null)
                    {
                        var totRecv = db.Lse_Installment.Where(x => x.AccNo == mod.AccNo && x.InstId != mod.TransId).Sum(x => (decimal?)(x.InstCharges + x.Discount)) ?? 0;
                        totRecv = totRecv + acc.Advance + mod.Discount + mod.InstCharges;
                        if (totRecv > acc.InstPrice)
                        {
                            scop.Dispose();
                            return 0;
                        }

                        //var TotalRecv = db.Lse_Installment.Where(x => x.AccNo == mod.AccNo && x.InstDate.Month == DateTime.Now.Month && x.InstDate.Year == DateTime.Now.Year && x.InstId != mod.TransId).Sum(x => (decimal?)(x.InstCharges + x.Discount)) ?? 0;
                        //TotalRecv = (TotalRecv + acc.Advance) - acc.InstPrice;

                        //var Recovery = db.Lse_Outstand.Where(x => x.AccNo == mod.AccNo && x.OutstandDate.Month == DateTime.Now.Month && x.OutstandDate.Year == DateTime.Now.Year).FirstOrDefault();
                        //if (Recovery != null)
                        //{
                        //    if (Recovery.OutstandAmt < TotalRecv + mod.InstCharges + mod.Discount)
                        //    {
                        //        Recovery.RecvAmt = Recovery.OutstandAmt;
                        //    }
                        //    else
                        //    {
                        //        Recovery.RecvAmt = TotalRecv + mod.InstCharges + mod.Discount;

                        //    }
                        //    if (Recovery.RecvAmt >= Recovery.OutstandAmt)
                        //    {
                        //        Recovery.Status = "C";
                        //    }
                        //    else if (Recovery.RecvAmt < Recovery.OutstandAmt)
                        //    {
                        //        Recovery.Status = "P";
                        //    }
                        //}

                        if (InstP.InstCharges != mod.InstCharges || InstP.Discount != mod.Discount)
                        {
                            Lse_InstallmentLog log = new Lse_InstallmentLog
                            {
                                AccNo = InstP.AccNo,
                                Discount = InstP.Discount,
                                InstCharges = InstP.InstCharges,
                                InstDate = InstP.InstDate,
                                InstId = InstP.InstId,
                                LocId = InstP.LocId,
                                Remarks = InstP.Remarks,
                                TransDate = InstP.TransDate,
                                UserId = InstP.UserId,
                                PaidBy = InstP.PaidBy
                            };
                            db.Lse_InstallmentLog.Add(log);
                        }


                        InstP.InstCharges = mod.InstCharges;
                        InstP.Discount = mod.Discount;
                        InstP.TransDate = DateTime.Now;
                        InstP.UserId = UserId;
                        await db.SaveChangesAsync();

                        if ((await UpdateOutStand(mod.AccNo, workingDate, InstP.InstId)) == false)
                        {
                            scop.Dispose();
                            return 0;
                        }

                        totRecv = db.Lse_Installment.Where(x => x.AccNo == mod.AccNo).Sum(x => (decimal?)(x.InstCharges + x.Discount)) ?? 0;
                        totRecv = totRecv + acc.Advance;
                        if (totRecv < acc.InstPrice && acc.Status == 4)
                        {
                            acc.Status = 3;
                            acc.CloseDate = null;
                            acc.CloseTransDate = null;

                        }
                        if (totRecv >= acc.InstPrice)
                        {
                            acc.Status = 4;
                            acc.CloseDate = workingDate;
                            acc.CloseTransDate = DateTime.Now;
                        }
                        await db.SaveChangesAsync();

                        scop.Complete();
                        scop.Dispose();
                        return InstP.InstId;
                    }
                    else
                    {
                        scop.Dispose();
                        return 0;
                    }
                }
                catch (Exception e)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<long> SaveInstallment(InstallmentVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var acc = await db.Lse_Master.FindAsync(mod.AccNo);
                    if (acc != null)
                    {
                        if (acc.Status == 3)
                        {
                            var totRecv = db.Lse_Installment.Where(x => x.AccNo == mod.AccNo).Sum(x => (decimal?)(x.InstCharges + x.Discount)) ?? 0;
                            totRecv = totRecv + acc.Advance + mod.Discount + mod.InstCharges;
                            if (totRecv > acc.InstPrice)
                            {
                                scop.Dispose();
                                return 0;
                            }
                            var workingDate = setupBL.GetWorkingDate(mod.LocId);
                            var Recovery = db.Lse_Outstand.Where(x => x.AccNo == mod.AccNo && x.OutstandDate.Month == workingDate.Month && x.OutstandDate.Year == workingDate.Year).Select(x => new { x.RecoveryId, x.IsAIC }).FirstOrDefault();
                            //if (Recovery != null)
                            //{

                            //    var TotalRecv = db.Lse_Installment.Where(x => x.AccNo == mod.AccNo && x.InstDate.Month == DateTime.Now.Month && x.InstDate.Year == DateTime.Now.Year).Sum(x => (decimal?)(x.InstCharges + x.Discount)) ?? 0;

                            //    if (Recovery.OutstandAmt < TotalRecv + mod.InstCharges + mod.Discount)
                            //    {
                            //        Recovery.RecvAmt = Recovery.OutstandAmt;
                            //    }
                            //    else
                            //    {
                            //        Recovery.RecvAmt = TotalRecv + mod.InstCharges + mod.Discount;

                            //    }
                            //    if (Recovery.RecvAmt >= Recovery.OutstandAmt)
                            //    {
                            //        Recovery.Status = "C";
                            //    }
                            //    else if (Recovery.RecvAmt < Recovery.OutstandAmt)
                            //    {
                            //        Recovery.Status = "P";
                            //    }
                            //}
                            Lse_Installment tbl = new Lse_Installment
                            {
                                InstDate = workingDate,
                                AccNo = mod.AccNo,
                                Discount = mod.Discount,
                                Fine = mod.Fine,
                                FineType = mod.FineType,
                                InstCharges = mod.InstCharges,
                                RecoveryId = Recovery != null ? Recovery.RecoveryId : null,
                                Remarks = mod.Remarks,
                                LocId = mod.LocId,
                                UserId = UserId,
                                TransDate = DateTime.Now,
                                PaidBy = mod.PaidBy,
                                IsAIC = Recovery != null ? Recovery.IsAIC : false
                            };
                            db.Lse_Installment.Add(tbl);
                            await db.SaveChangesAsync();

                            if ((await UpdateOutStand(mod.AccNo, workingDate, tbl.InstId)) == false)
                            {
                                scop.Dispose();
                                return 0;
                            }

                            totRecv = db.Lse_Installment.Where(x => x.AccNo == mod.AccNo).Sum(x => (decimal?)(x.InstCharges + x.Discount)) ?? 0;
                            totRecv = totRecv + acc.Advance;
                            if (totRecv >= acc.InstPrice)
                            {
                                acc.Status = 4;
                                acc.CloseDate = setupBL.GetWorkingDate(mod.LocId);
                                acc.CloseTransDate = DateTime.Now;
                                await db.SaveChangesAsync();
                            }
                            //if (Recovery != null)
                            //{
                            //    Recovery.InstId = tbl.InstId;
                            //    Recovery.InstDate = tbl.InstDate;
                            //    await db.SaveChangesAsync();
                            //}
                            scop.Complete();
                            scop.Dispose();
                            return tbl.InstId;

                        }
                    }
                    scop.Dispose();
                    return 0;
                }
                catch (Exception ex)
                {
                    scop.Dispose();
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    return 0;
                }
            }
        }
        public async Task<List<OutStandVM>> GetInstAdjust(int LocId, int PaidBy)
        {
            try
            {
                var WorkingDate = setupBL.GetWorkingDate(LocId);
                //var lst = new List<OutStandVM>();
                //if (OSMonth.Month == DateTime.Now.Month && OSMonth.Year == DateTime.Now.Year)
                //{
                var ls = await db.Lse_InstAdj.Where(O => O.WorkingDate.Month == WorkingDate.Month && O.WorkingDate.Year == WorkingDate.Year && O.LocId == LocId).ToListAsync();
                var lst = await (from O in db.Lse_Outstand
                                     //join E in db.Lse_InstAdj on O.AccNo equals E.AccNo into OS
                                     //from E in OS.DefaultIfEmpty()
                                 where O.LocId == LocId
                                 && O.OutstandDate.Month == WorkingDate.Month && O.OutstandDate.Year == WorkingDate.Year
                                 //&& (O.Status == "I")
                                 select new OutStandVM
                                 {
                                     AccNo = O.AccNo,
                                     OldAccNo = 0,
                                     Category = O.Category,
                                     OutStandAmt = O.OutstandAmt,
                                     OutStandDate = O.OutstandDate,
                                     RecoveryId = O.RecoveryId ?? 0,
                                     Status = O.Status,
                                     TransId = O.TransId,
                                     Customer = O.Lse_Master.CustName,
                                     Inst = O.Lse_Master.MonthlyInst,
                                     RecvAmt = O.RecvAmt ?? 0,
                                     Remaning = O.OutstandAmt - (O.RecvAmt ?? 0),
                                     //Recovery = O.IsAIC ? "AIC (Office)" : E.EmpName
                                 }).OrderBy(x => x.AccNo).ToListAsync();
                //var l = lst.Where(x => x.Status == "I" && !ls.Where(a => a.Amount > 0 && a.PaidBy != PaidBy).Select(a => a.AccNo).ToList().Contains(x.AccNo)).ToList();
                //foreach (var v in l)
                //{
                //    v.RecvAmt = ls.Where(x => x.AccNo == v.AccNo && x.PaidBy == PaidBy).Select(x => (decimal?)x.Amount).FirstOrDefault() ?? 0;
                //    v.Remaning = v.OutStandAmt - v.RecvAmt;
                //}
                //l.AddRange(lst.Where(x => ls.Where(a => a.PaidBy == PaidBy && a.Amount > 0).Select(a => a.AccNo).ToList().Contains(x.AccNo)).ToList());
                //}
                lst = (from O in lst
                       join E in ls on O.AccNo equals E.AccNo into OS
                       from E in OS.DefaultIfEmpty()
                       select new OutStandVM
                       {
                           AccNo = O.AccNo,
                           OldAccNo = E == null ? 0 : E.PaidBy,
                           Category = O.Category,
                           OutStandAmt = O.OutStandAmt,
                           OutStandDate = O.OutStandDate,
                           RecoveryId = O.RecoveryId,
                           Status = O.Status,
                           TransId = O.TransId,
                           Customer = O.Customer,
                           Inst = O.Inst,
                           RecvAmt = (E == null ? 0 : E.Amount),
                           Remaning = O.OutStandAmt - (E == null ? 0 : E.Amount),
                           IsCheck = (E == null ? 0 : E.Amount) > 0 ? true : false,
                           Recovery = E == null ? "" : (E.ApprovedBy == null ? "" : "A")
                           //Recovery = O.IsAIC ? "AIC (Office)" : E.EmpName
                       }).Where(x => (x.OldAccNo == PaidBy && x.RecvAmt > 0) || (x.Status == "I" && x.OldAccNo == 0)).OrderBy(x => x.AccNo).ToList();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> SaveInstAdj(List<OutStandVM> mod, int LocId, int PaidBy, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var WorkingDate = setupBL.GetWorkingDate(LocId);
                    foreach (var v in mod)
                    {
                        if (v.RecvAmt > 0)
                        {
                            var os = await (from O in db.Lse_Outstand
                                            where O.AccNo == v.AccNo && O.LocId == LocId
                                            && O.OutstandDate.Month == WorkingDate.Month && O.OutstandDate.Year == WorkingDate.Year
                                            select O).SingleOrDefaultAsync();
                            if (os != null)
                            {
                                if (os.RecvAmt == 0)
                                {
                                    var tbl = await db.Lse_InstAdj.Where(x => x.AccNo == v.AccNo && x.LocId == LocId && x.WorkingDate.Month == WorkingDate.Month
                                    && x.WorkingDate.Year == WorkingDate.Year).FirstOrDefaultAsync();
                                    if (tbl != null)
                                    {
                                        if (tbl.Amount != v.RecvAmt)
                                        {
                                            tbl.Amount = v.RecvAmt;
                                            tbl.PaidBy = PaidBy;
                                        }
                                    }
                                    else
                                    {
                                        tbl = new Lse_InstAdj
                                        {
                                            AccNo = v.AccNo,
                                            Amount = v.RecvAmt,
                                            ApprovedBy = null,
                                            ApprovedDate = null,
                                            LocId = os.LocId,
                                            OSId = os.TransId,
                                            PaidBy = PaidBy,
                                            TransDate = DateTime.Now,
                                            UserId = UserId,
                                            WorkingDate = WorkingDate
                                        };
                                        db.Lse_InstAdj.Add(tbl);
                                        //await db.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                        else
                        {
                            var tbl = await db.Lse_InstAdj.Where(x => x.AccNo == v.AccNo && x.LocId == LocId && x.WorkingDate.Month == WorkingDate.Month
                                    && x.WorkingDate.Year == WorkingDate.Year).FirstOrDefaultAsync();
                            if (tbl != null)
                            {
                                if (tbl.Amount != 0)
                                {
                                    tbl.Amount = 0;
                                    tbl.PaidBy = 0;
                                    //await db.SaveChangesAsync();
                                }
                            }
                        }

                    }
                    await db.SaveChangesAsync();
                    var vcr = GetClosingVoucher(WorkingDate, LocId);
                    var lst = await db.Lse_InstAdj.Where(x => x.LocId == LocId && x.WorkingDate.Month == WorkingDate.Month
                                    && x.WorkingDate.Year == WorkingDate.Year && x.PaidBy == PaidBy).ToListAsync();
                    if (lst.Sum(a => a.Amount) > vcr.Where(a => a.Sr == PaidBy).Sum(a => a.Amount))
                    {
                        scop.Dispose();
                        return "Exceeding Limit";
                    }
                    else
                    {
                        scop.Complete();
                        scop.Dispose();
                        return "OK";
                    }
                }
                catch (Exception ex)
                {
                    scop.Dispose();
                    return "Server Error";
                }
            }
        }
        public async Task<List<OutStandVM>> GetInstAdjustApproval(int LocId)
        {
            try
            {
                var WorkingDate = setupBL.GetWorkingDate(LocId);
                return await (from A in db.Lse_InstAdj
                              join O in db.Lse_Outstand on A.OSId equals O.TransId
                              join P in db.Lse_PaidBy on A.PaidBy equals P.RowId
                              where A.LocId == LocId && A.WorkingDate.Month == WorkingDate.Month && A.WorkingDate.Year == WorkingDate.Year
                              && A.ApprovedBy == null && O.Status == "I"
                              select new OutStandVM
                              {
                                  AccNo = A.AccNo,
                                  Customer = O.Lse_Master.CustName,
                                  Category = P.Title,
                                  OutStandAmt = O.OutstandAmt,
                                  RecvAmt = O.RecvAmt ?? 0 + A.Amount,
                                  IsCheck = false,
                                  TransId = A.RowId,
                                  RecoveryId = A.PaidBy
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> SaveInstAdjApproval(List<OutStandVM> mod, int LocId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var WorkingDate = setupBL.GetWorkingDate(LocId);
                    var ls = new int[] { 3, 4, 5, 6 };
                    var closingvcr = GetClosingVoucher(WorkingDate.Date, LocId).Where(x => ls.Contains(x.Sr)).ToList();
                    var closingadj = GetClosingVoucherAdj(WorkingDate.Date, LocId);
                    foreach (var v in closingvcr)
                    {
                        var vcr = mod.Where(x => x.IsCheck && x.RecoveryId == v.Sr).ToList();
                        var adj = closingadj.Where(x => x.Sr == v.Sr).Sum(x => x.InstCharges);
                        if (v.Amount - vcr.Sum(x => x.RecvAmt) - adj >= 0)
                        {
                            foreach (var i in vcr)
                            {
                                var ad = await db.Lse_InstAdj.Where(x => x.AccNo == i.AccNo && x.WorkingDate.Month == WorkingDate.Month &&
                                x.WorkingDate.Year == WorkingDate.Year).FirstOrDefaultAsync();
                                ad.ApprovedBy = UserId;
                                ad.ApprovedDate = DateTime.Now;
                                var instId = await SaveInstallment(new InstallmentVM
                                {
                                    AccNo = i.AccNo,
                                    Discount = 0,
                                    Fine = 0,
                                    FineType = "",
                                    InstCharges = i.RecvAmt,
                                    LocId = LocId,
                                    PaidBy = i.RecoveryId,
                                    Remarks = ""
                                }, UserId);
                                if (instId > 0)
                                {
                                    await db.SaveChangesAsync();
                                }
                                else
                                {
                                    scop.Dispose();
                                }
                            }
                        }
                        else
                        {
                            scop.Dispose();
                            return "Limit Exceeding";
                        }
                    }
                    scop.Complete();
                    scop.Dispose();

                    return "OK";
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return "Error";
                }
            }
        }
        //public async Task<Lease_Advance> GetAccountByNo(long AccNo)
        //{
        //    try
        //    {
        //        return await db.Lease_Advance.Where(x => x.AccNo == AccNo).FirstOrDefaultAsync();
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public async Task<bool> InstExist(long AccNo, int LocId)
        {
            try
            {
                var workingDate = setupBL.GetWorkingDate(LocId);
                var fromDate = new DateTime(workingDate.Year, workingDate.Month, 1);
                var toDate = fromDate.AddMonths(1);
                return await db.Lse_Installment.Where(x => x.AccNo == AccNo && x.InstDate >= fromDate && x.InstDate < toDate).AnyAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<Lse_Category>> CategoryList()
        {
            try
            {
                return await db.Lse_Category.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<TypePlanVM>> PlanPolicyList(int LocId, int SKUId, bool IsLocal, int Duration)
        {
            try
            {
                var dt = await GetWorkingDate(LocId) ?? DateTime.Now.Date;
                var typeId = await db.Itm_Master.Where(x => x.SKUId == SKUId).Select(x => x.Itm_Model.TypeId).FirstAsync();

                return await (from TP in db.Itm_TypePlan
                              where TP.LocId == LocId && TP.IsLocal == IsLocal && TP.Duration == Duration && TP.TypeId == typeId
                              && TP.Status && TP.Itm_TypePlanMaster.EffectiveFrom <= dt && (TP.Itm_TypePlanMaster.EndDate >= dt || TP.Itm_TypePlanMaster.EndDate == null) && TP.Itm_TypePlanMaster.Status
                              select new TypePlanVM { PolicyId = TP.RowId, MarkUp = TP.MarkUp }).ToListAsync();

                //if (mas.Count > 0)
                //{
                //    var policyId = mas.OrderByDescending(x => x.Itm_TypePlanMaster.EffectiveFrom).Select(x => x.PolicyId).FirstOrDefault();
                //    return mas.Where(x => x.PolicyId == policyId).Select(x => new TypePlanVM { PolicyId = x.RowId, MarkUp = x.MarkUp }).ToList();
                //}
                //return new List<TypePlanVM>();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<spget_InstPlanExtraAdvance_Result> InstPlanBySKU(int LocId, int SKUId, decimal Advance, decimal PPrice, bool IsLocal, int PolicyId)
        {
            try
            {
                return db.spget_InstPlanExtraAdvance(LocId, SKUId, Advance, PPrice, PolicyId, IsLocal).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Itm_SKUPlan> GetInstPlanBySKU(int LocId, int SKUId, decimal Advance, decimal PPrice, bool IsLocal, int PolicyId)
        {
            try
            {
                DateTime dt = DateTime.Now.Date;
                var plan = new Itm_SKUPlan();
                if (IsLocal)
                {
                    var policy = await db.Itm_PlanPolicy.Where(x => x.LocId == LocId && x.RowId == PolicyId).FirstAsync();
                    var Duration = policy.Duration;
                    decimal MarkUp = policy.MarkUp;
                    decimal MinAdvance = policy.MinAdvance;
                    decimal MaxAdvance = policy.MaxAdvance;

                    var InstPrice = (PPrice * MarkUp / 100) + PPrice;

                    var addv = Math.Round(InstPrice / Duration);
                    var addvmin = (PPrice * MinAdvance / 100);
                    var addvmax = (PPrice * MaxAdvance / 100);

                    var adv = addv % 25;
                    adv = adv > 0 ? 25 - adv : 0;
                    addv = addv + adv;
                    if (Advance == 0)
                    {
                        plan.Advance = addv;
                        plan.InstPrice = addv * Duration;
                        plan.Inst = addv;
                        plan.Remarks = "";
                        if (SKUId > 0)
                        {
                            var cityId = await db.Comp_Locations.Where(x => x.LocId == LocId).Select(x => x.CityId).FirstOrDefaultAsync();
                            var compPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.LocId == 0 && x.Duration == Duration && x.CityId == cityId && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync();
                            if (compPlan == null)
                            {
                                compPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.LocId == 0 && x.Duration == Duration && x.CityId == 0 && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync();
                            }
                            if (compPlan != null)
                            {
                                if (plan.InstPrice < compPlan.InstPrice)
                                {
                                    plan.Remarks = "InstPrice less than Company Plan";
                                    plan.InstPrice = compPlan.InstPrice;
                                    plan.Advance = compPlan.Advance;
                                    plan.Inst = compPlan.Inst;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Advance >= addvmin && Advance <= addvmax)
                        {
                            plan.Advance = Advance;
                            plan.InstPrice = ((PPrice - plan.Advance) * MarkUp / 100) + (PPrice - plan.Advance) + plan.Advance;
                            plan.Inst = (plan.InstPrice - plan.Advance) / (Duration - 1);
                            var inst = plan.Inst % 25;
                            inst = inst > 0 ? 25 - inst : 0;
                            plan.Inst = plan.Inst + inst;
                            plan.InstPrice = plan.Advance + (plan.Inst * (Duration - 1));
                            plan.Remarks = "";
                            //if (SKUId > 0)
                            //{
                            //    var cityId = await db.Comp_Locations.Where(x => x.LocId == LocId).Select(x => x.CityId).FirstOrDefaultAsync();
                            //    var compPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.LocId == 0 && x.Duration == Duration && x.CityId == cityId && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync();
                            //    if (compPlan == null)
                            //    {
                            //        compPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.LocId == 0 && x.Duration == Duration && x.CityId == 0 && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync();
                            //    }
                            //    if (compPlan != null)
                            //    {
                            //        if (plan.InstPrice < compPlan.InstPrice)
                            //        {
                            //            plan.Remarks = "InstPrice less than Company Plan";
                            //            plan.InstPrice = compPlan.InstPrice;
                            //            plan.Advance = compPlan.Advance;
                            //            plan.Inst = compPlan.Inst;
                            //        }
                            //    }
                            //}
                        }
                        else if (Advance > addvmax)
                        {
                            plan.Remarks = "Advance exceeding 50%";
                            plan.Advance = addvmax;
                            var inst = plan.Advance % 25;
                            inst = inst > 0 ? 25 - inst : 0;
                            plan.Advance = plan.Advance + inst;
                            plan.InstPrice = ((PPrice - plan.Advance) * MarkUp / 100) + (PPrice - plan.Advance) + plan.Advance;
                            plan.Inst = (plan.InstPrice - plan.Advance) / (Duration - 1);
                            inst = plan.Inst % 25;
                            inst = inst > 0 ? 25 - inst : 0;
                            plan.Inst = plan.Inst + inst;
                            plan.InstPrice = plan.Advance + (plan.Inst * (Duration - 1));
                        }
                        else
                        {
                            plan.Remarks = "Advance below 30%";
                            plan.Advance = addv;
                            plan.InstPrice = plan.Advance * Duration;
                            plan.Inst = addv;
                        }
                    }
                    plan.Duration = Duration;
                }
                else
                {
                    var policy = await db.Itm_PlanPolicy.Where(x => x.LocId == LocId && x.RowId == PolicyId).FirstAsync();
                    var tbl = (await GetInstPriceBySKU(SKUId, LocId, policy.Duration, 0)).FirstOrDefault();
                    if (tbl == null)
                    {
                        plan.Remarks = "Plan Not Found";
                        plan.Advance = 0;
                        plan.InstPrice = 0;
                        plan.Inst = 0;
                        return plan;
                    }
                    var skuPlan = await db.Itm_SKUPlan.FindAsync(tbl.RowId);
                    var Duration = policy.Duration;
                    decimal MarkUp = policy.MarkUp;
                    decimal MinAdvance = policy.MinAdvance;
                    decimal MaxAdvance = policy.MaxAdvance;
                    PPrice = skuPlan.BasePrice;

                    var InstPrice = (PPrice * MarkUp / 100) + PPrice;

                    var addv = Math.Round(InstPrice / Duration);
                    var addvmin = (PPrice * MinAdvance / 100);
                    var addvmax = (PPrice * MaxAdvance / 100);

                    var adv = addv % 25;
                    adv = adv > 0 ? 25 - adv : 0;
                    addv = addv + adv;
                    if (Advance == 0)
                    {
                        plan.Advance = addv;
                        plan.InstPrice = addv * Duration;
                        plan.Inst = addv;
                        plan.Remarks = "";
                        //if (SKUId > 0)
                        //{
                        //    var cityId = await db.Comp_Locations.Where(x => x.LocId == LocId).Select(x => x.CityId).FirstOrDefaultAsync();
                        //    var compPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.LocId == 0 && x.Duration == Duration && x.CityId == cityId && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync();
                        //    if (compPlan == null)
                        //    {
                        //        compPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.LocId == 0 && x.Duration == Duration && x.CityId == 0 && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync();
                        //    }
                        //    if (compPlan != null)
                        //    {
                        //        if (plan.InstPrice < compPlan.InstPrice)
                        //        {
                        //            plan.Remarks = "InstPrice less than Company Plan";
                        //            plan.InstPrice = compPlan.InstPrice;
                        //            plan.Advance = compPlan.Advance;
                        //            plan.Inst = compPlan.Inst;
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        if (Advance >= addvmin && Advance <= addvmax)
                        {
                            plan.Advance = Advance;
                            plan.InstPrice = ((PPrice - plan.Advance) * MarkUp / 100) + (PPrice - plan.Advance) + plan.Advance;
                            plan.Inst = (plan.InstPrice - plan.Advance) / (Duration - 1);
                            var inst = plan.Inst % 25;
                            inst = inst > 0 ? 25 - inst : 0;
                            plan.Inst = plan.Inst + inst;
                            plan.InstPrice = plan.Advance + (plan.Inst * (Duration - 1));
                            plan.Remarks = "";
                            //if (SKUId > 0)
                            //{
                            //    var cityId = await db.Comp_Locations.Where(x => x.LocId == LocId).Select(x => x.CityId).FirstOrDefaultAsync();
                            //    var compPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.LocId == 0 && x.Duration == Duration && x.CityId == cityId && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync();
                            //    if (compPlan == null)
                            //    {
                            //        compPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.LocId == 0 && x.Duration == Duration && x.CityId == 0 && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync();
                            //    }
                            //    if (compPlan != null)
                            //    {
                            //        if (plan.InstPrice < compPlan.InstPrice)
                            //        {
                            //            plan.Remarks = "InstPrice less than Company Plan";
                            //            plan.InstPrice = compPlan.InstPrice;
                            //            plan.Advance = compPlan.Advance;
                            //            plan.Inst = compPlan.Inst;
                            //        }
                            //    }
                            //}
                        }
                        else if (Advance > addvmax)
                        {
                            plan.Remarks = "Advance exceeding 50%";
                            plan.Advance = addvmax;
                            var inst = plan.Advance % 25;
                            inst = inst > 0 ? 25 - inst : 0;
                            plan.Advance = plan.Advance + inst;
                            plan.InstPrice = ((PPrice - plan.Advance) * MarkUp / 100) + (PPrice - plan.Advance) + plan.Advance;
                            plan.Inst = (plan.InstPrice - plan.Advance) / (Duration - 1);
                            inst = plan.Inst % 25;
                            inst = inst > 0 ? 25 - inst : 0;
                            plan.Inst = plan.Inst + inst;
                            plan.InstPrice = plan.Advance + (plan.Inst * (Duration - 1));
                        }
                        else
                        {
                            plan.Remarks = "Advance below 30%";
                            plan.Advance = addv;
                            plan.InstPrice = plan.Advance * Duration;
                            plan.Inst = addv;
                        }
                    }
                    plan.Duration = Duration;
                    //if (tbl != null)
                    //{
                    //    var type = await db.Itm_Master.Where(x => x.SKUId == SKUId).Select(x => x.Itm_Model.TypeId).FirstOrDefaultAsync();
                    //    var typePlan = await db.Itm_TypePlan.Where(x => x.TypeId == type && x.Status && x.Duration == tbl.Duration).FirstOrDefaultAsync();
                    //    if (typePlan == null)
                    //    {
                    //        return plan;
                    //    }

                    //    var Duration = tbl.Duration;
                    //    var InstPrice = (tbl.BasePrice * typePlan.MarkUp / 100) + tbl.BasePrice;

                    //    var addv = Math.Round(InstPrice / Duration);
                    //    var addvmin = (tbl.BasePrice * typePlan.MinAdvance / 100);
                    //    var addvmax = (tbl.BasePrice * typePlan.MaxAdvance / 100);

                    //    var adv = addv % 25;
                    //    adv = adv > 0 ? 25 - adv : 0;
                    //    addv = addv + adv;
                    //    if (Advance == 0)
                    //    {
                    //        plan.Advance = addv;
                    //        plan.InstPrice = addv * Duration;
                    //        plan.Inst = addv;
                    //    }
                    //    else
                    //    {
                    //        //if(Advance < addv)
                    //        //{
                    //        //    plan.Advance = addv;
                    //        //    plan.InstPrice = plan.Advance * 12;
                    //        //    plan.Inst = addv;
                    //        //}
                    //        //else 
                    //        if (Advance >= addvmin && Advance <= addvmax)
                    //        {
                    //            plan.Advance = Advance;
                    //            plan.InstPrice = ((tbl.BasePrice - plan.Advance) * typePlan.MarkUp / 100) + (tbl.BasePrice - plan.Advance) + plan.Advance;
                    //            plan.Inst = (plan.InstPrice - plan.Advance) / (Duration - 1);
                    //            var inst = plan.Inst % 25;
                    //            inst = inst > 0 ? 25 - inst : 0;
                    //            plan.Inst = plan.Inst + inst;
                    //            plan.InstPrice = plan.Advance + (plan.Inst * (Duration - 1));
                    //        }
                    //        else if (Advance > addvmax)
                    //        {
                    //            plan.Advance = addvmax;
                    //            var inst = plan.Advance % 25;
                    //            inst = inst > 0 ? 25 - inst : 0;
                    //            plan.Advance = plan.Advance + inst;
                    //            plan.InstPrice = ((tbl.BasePrice - plan.Advance) * typePlan.MarkUp / 100) + (tbl.BasePrice - plan.Advance) + plan.Advance;
                    //            plan.Inst = (plan.InstPrice - plan.Advance) / (Duration - 1);
                    //            inst = plan.Inst % 25;
                    //            inst = inst > 0 ? 25 - inst : 0;
                    //            plan.Inst = plan.Inst + inst;
                    //            plan.InstPrice = plan.Advance + (plan.Inst * (Duration - 1));
                    //            plan.Remarks = "Advance exceeding 50%";
                    //        }
                    //        else
                    //        {
                    //            plan.Advance = addv;
                    //            plan.InstPrice = plan.Advance * Duration;
                    //            plan.Inst = addv;
                    //            plan.Remarks = "Advance below 30%";
                    //        }



                    //        //plan.InstPrice = InstPrice;
                    //        //plan.Advance = Advance;
                    //        //plan.InstPrice = Advance * Duration;
                    //        plan.Duration = Duration;
                    //        //plan.Inst = Advance;
                    //    }
                    //}
                }

                return plan;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Itm_SKUPlan>> GetInstPriceBySKU(int SKUId, int LocId, int Duration)
        {
            try
            {
                List<Itm_SKUPlan> lst = new List<Itm_SKUPlan>();
                var sku = db.Itm_Master.FindAsync(SKUId).Result;
                DateTime dt = setupBL.GetWorkingDate(LocId);
                if (sku.IsPair)
                {
                    //if (sku.SKUId != sku.PairId)
                    //{
                    lst.Add(new Itm_SKUPlan
                    {
                        Advance = 0,
                        InstPrice = 0,
                        Inst = 0,
                        Duration = Duration,
                        RowId = 0
                    });
                    return lst;
                    //}
                }
                else if (sku.Itm_Model.TypeId == 1020)
                {
                    lst = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.Duration == Duration && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).ToListAsync();
                    return lst;
                }
                else if (sku.Itm_Model.ModelId == 28821 || sku.Itm_Model.ModelId == 27148)
                {
                    lst.Add(await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.Duration == Duration && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync());
                    return lst;
                }
                else
                {

                    var CityId = await db.Comp_Locations.Where(x => x.LocId == LocId).Select(x => x.CityId).FirstOrDefaultAsync();
                    var tbl = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.LocId == LocId && x.Duration == Duration && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync();
                    if (tbl != null)
                    {
                        lst.Add(tbl);
                        return lst;
                    }
                    tbl = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.LocId == 0 && x.Duration == Duration && x.CityId == CityId && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync();
                    if (tbl != null)
                    {
                        lst.Add(tbl);
                        return lst;
                    }
                    tbl = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.LocId == 0 && x.Duration == Duration && x.CityId == 0 && x.Status && x.EffectedDate <= dt).OrderByDescending(x => x.EffectedDate).FirstOrDefaultAsync();
                    if (tbl != null)
                    {
                        lst.Add(tbl);
                        return lst;
                    }
                }
                //var typeId = sku.Itm_Model.TypeId;
                //var pri = await GetItemSMBySKULoc(SKUId, LocId);
                //var ls = await db.Itm_TypePlan.Where(x => x.TypeId == typeId && x.Status).FirstOrDefaultAsync();
                //if (ls != null)
                //{
                //    var per = (ls.MaxPercent - ls.MinPercent) / (ls.MaxDuration - ls.MinDuration);
                //    if (Duration >= ls.MinDuration && Duration <= ls.MaxDuration)
                //    {
                //        var plan = new Itm_SKUPlan();
                //        plan.InstPrice = pri.CashPrice + pri.CashPrice * ((Duration * per) / 100);
                //        plan.Advance = Math.Round((plan.InstPrice / Duration) / 10) * 10;
                //        plan.InstPrice = plan.Advance * Duration;
                //        plan.Duration = Duration;
                //        plan.Inst = plan.Advance;
                //        plan.RowId = ls.RowId;
                //        plan.Status = false;
                //        lst.Add(plan);
                //    }
                //}
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<spget_InstPlan_V1_Result>> GetInstPriceBySKU(int SKUId, int LocId, int Duration, long ItemId)
        {
            try
            {
                var lst = db.spget_InstPlan_V1(SKUId, LocId, Duration, ItemId).ToList();
                //    .Select(x => new Itm_SKUPlan 
                //{
                //    Advance = x.Advance ?? 0,
                //    InstPrice = x.InstPrice ?? 0,
                //    Inst = x.Inst ?? 0,
                //    RowId = x.RowId,
                //    Duration = x.Duration ?? 0,
                //    PlanType = x.PlanType
                //}).ToList();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<InstallmentDetailVM>> GetTodayInstallments(int LocId, DateTime Date)
        {
            try
            {
                var selectedDate = Date.Date;
                return await (from I in db.Lse_Installment
                              join M in db.Lse_Master on I.AccNo equals M.AccNo
                              where I.LocId == LocId && I.InstDate == selectedDate
                              select new InstallmentDetailVM
                              {
                                  TransId = I.InstId,
                                  AccNo = I.AccNo,
                                  CustName = M.CustName,
                                  //FName = M.FName,
                                  //Recovery = 
                                  //Company =
                                  //Product =
                                  //SKU =
                                  //ActualAdvance = M.ActualAdvance ?? 0,
                                  //TotalPrice = M.InstPrice,
                                  //Advance = M.Advance,
                                  ////PreBalance =
                                  //ActualInstallment = M.MonthlyInst,
                                  //rearInstallment = 
                                  InstCharges = I.InstCharges,
                                  Discount = I.Discount,
                                  //Balance =
                                  FineType = I.FineType,
                                  Fine = I.Fine,
                                  PaidBy = I.PaidBy,
                                  //Comments =
                                  Remarks = I.Remarks,
                              }).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<InstDetailVM>> GetInstByAcc(long AccNo)
        {
            try
            {
                var proc = await db.Lse_Master.FindAsync(AccNo);
                var PreBalance = proc.InstPrice - proc.Advance;
                var lst = await db.Lse_Installment.Where(x => x.AccNo == AccNo && !x.IsLock).OrderBy(x => x.InstDate).ToListAsync();
                List<InstDetailVM> InstDetLst = new List<InstDetailVM>();
                foreach (var x in lst)
                {
                    InstDetLst.Add(new InstDetailVM
                    {
                        AccNo = x.AccNo,
                        ActualInstallment = x.InstCharges,
                        Discount = x.Discount,
                        Fine = x.Fine,
                        FineType = x.FineType,
                        InstallDate = x.InstDate,
                        InstCharges = x.InstCharges,
                        Remarks = (x.PaidBy == 1 ? "" : "Voucher ") + (x.Remarks ?? ""),
                        //Remarks = x.Remarks,
                        TransId = x.InstId,
                        RecoveryOff = x.RecoveryId == null ? "" : await db.Pay_EmpMaster.Where(a => a.EmpId == x.RecoveryId).Select(a => a.EmpName).FirstOrDefaultAsync(),
                        PreBalance = PreBalance,
                        Balance = PreBalance - x.InstCharges - x.Discount,
                        PaidBy = x.PaidBy
                    });
                    PreBalance = PreBalance - x.InstCharges - x.Discount;
                }
                return InstDetLst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        public async Task<List<Inv_Store>> GetItemBySKULoc(int SKUId, int LocId)
        {
            try
            {
                return await db.Inv_Store.Where(x => x.SKUId == SKUId && x.LocId == LocId && x.Inv_Status.MFact == 1).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Inv_Store>> GetItemBySKULocReturn(int SKUId, int LocId, long AccNo)
        {
            try
            {
                var itms = await db.Lse_Detail.Where(x => x.AccNo == AccNo).Select(x => x.ItemId).ToListAsync();
                return await db.Inv_Store.Where(x => (x.SKUId == SKUId && x.LocId == LocId && x.Inv_Status.MFact == 1) || itms.Contains(x.ItemId)).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public decimal GetItemSMBySKULoc(int SKUId, int LocId)
        {
            try
            {
                return db.spget_SKUCashRate(LocId, SKUId).Select(x => x).FirstOrDefault() ?? 0;
               
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<CustomerDetailVM> GetOnlineCustomer(string MobileNo)
        {
            try
            {
                return await db.Inv_Sale.Where(x => x.CustCellNo == MobileNo).OrderByDescending(x => x.TransId)
                    .Select(x => new CustomerDetailVM
                    {
                        AccountTitle = x.CustAccountTitle,
                        Address = x.Address,
                        BankName = x.CustBankName,
                        BusinessRelation = x.CustType,
                        CNIC = x.CustCNIC,
                        CustId = x.CustId,
                        CustName = x.CustName,
                        Email = x.Email,
                        Mobile = x.CustCellNo,
                        NTN = x.CustNTN,
                        ChequeNo = x.CustAccountNo,
                        CustAccountHolder = x.CustAccountHolder,
                        CustType = x.CustType,
                        AccNo = x.TransId
                    }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Inv_PaymentMode>> PaymentModeList()
        {
            try
            {
                return await db.Inv_PaymentMode.Where(x => x.PaymentModeId != 3).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Pay_EmpMaster>> GetEmpByDesgLoc(int DesgId, int LocId)
        {
            try
            {
                return await db.Pay_EmpMaster.Where(x => x.StatusId == "A" && (x.DesgId == DesgId || DesgId == 0) && x.DeptId == LocId).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Pay_EmpMaster>> GetEmpByRoleLoc(int RoleId, int LocId)
        {
            try
            {
                if (RoleId == 4 || RoleId == 2)
                {
                    return await (from E in db.Pay_EmpMaster
                                      //join R in db.Pay_EmpRole on E.EmpId equals R.EmpId
                                  join R in db.Pay_DesgRole on E.DesgId equals R.DesgId
                                  where R.RoleId == RoleId && E.DeptId == LocId && E.StatusId == "A" //&& R.Status
                                  select E).ToListAsync();
                }
                return await (from E in db.Pay_EmpMaster
                              where E.DeptId == LocId && E.StatusId == "A"
                              select E).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Pay_EmpMaster> GetEmpByRoleLoc(string CNIC, int RoleId, int LocId)
        {
            try
            {
                if (RoleId == 4 || RoleId == 2)
                {
                    return await (from E in db.Pay_EmpMaster
                                      //join R in db.Pay_EmpRole on E.EmpId equals R.EmpId
                                  join R in db.Pay_DesgRole on E.DesgId equals R.DesgId
                                  where R.RoleId == RoleId && E.DeptId == LocId && E.StatusId == "A" //&& R.Status 
                                  && E.CNIC == CNIC
                                  select E).FirstOrDefaultAsync();
                }
                return await (from E in db.Pay_EmpMaster
                              where E.DeptId == LocId && E.StatusId == "A" && E.CNIC == CNIC
                              select E).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region AccountSearch
        public async Task<List<AccountSearchVM>> GetAccountSearchMore(int LocId, DateTime FromDate, DateTime ToDate, int RoleId, int RecoveryId, int Status)
        {
            try
            {
                var data = db.spget_AccSearchByInqMktMgr(LocId, FromDate, ToDate, RoleId, RecoveryId, Status).Select(x => new
                    AccountSearchVM
                {
                    AccNo = x.AccNo,
                    DeliveryDate = x.DeliveryDate,
                    Customer = x.CustName,
                    FName = x.FName,
                    Mobile1 = x.Mobile1,
                    NIC = x.NIC,
                    Model = x.Model,
                    SerialNo = x.SerialNo,
                    SKU = x.SKUName
                }).ToList();

                return data;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
        public async Task<List<AccountSearchVM>> GetAccountSearch(int LocId, int Crit1, string CritVal1, int Crit2, string CritVal2)
        {
            try
            {
                return db.spget_AccountSearch(LocId, Crit1, CritVal1, Crit2, CritVal2).Select(x => new
                    AccountSearchVM
                {
                    DeliveryDate = x.DeliveryDate,
                    AccNo = x.AccNo,
                    Customer = x.Customer,
                    Mobile1 = x.Mobile1,
                    MonthlyInst = x.MonthlyInst,
                    NIC = x.NIC,
                    FName = x.FName,
                    InstPrice = x.InstPrice,
                    Status = x.Status
                    //Company = x.Company,
                    //Product = x.Product,
                    //SKU = x.Model
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Lse_SearchCriteria>> GetSearchCriteria()
        {
            try
            {
                return await db.Lse_SearchCriteria.Where(x => x.Status).OrderBy(x => x.SortOrder).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region OutStand
        public async Task<bool> LoadAllOutStand(int M)
        {
            try
            {
                int UserId = 100003;
                var dt = DateTime.Now.Date;
                int fromLoc = (50 * M) + 1;
                int toLoc = (50 * M) + 50;

                var loc = await db.Comp_Locations.Where(x => x.Status == true && x.LocTypeId == 1 && x.LocId >= fromLoc && x.LocId <= toLoc).Select(x => x.LocId).ToListAsync();
                foreach (var v in loc)
                {
                    var FExists = await db.Lse_Outstand.Where(x => x.LocId == v && x.Category == "F" && x.OutstandDate.Month == dt.Month && x.OutstandDate.Year == dt.Year).AnyAsync();
                    if (!FExists)
                    {
                        await LoadOutStand(v, "F", UserId);
                    }
                    var RExists = await db.Lse_Outstand.Where(x => x.LocId == v && x.Category == "R" && x.OutstandDate.Month == dt.Month && x.OutstandDate.Year == dt.Year).AnyAsync();
                    if (!RExists)
                    {
                        await LoadOutStand(v, "R", UserId);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<bool> LoadOutStand(int LocId, string Category, int UserId)
        {
            try
            {
                AGEEntities data = new AGEEntities();
                var dt = setupBL.GetWorkingDate(LocId);
                dt = new DateTime(dt.Year, dt.Month, 1);
                var lst = await data.Lse_Outstand.Where(x => x.LocId == LocId && x.Category == Category && x.OutstandDate.Month == DateTime.Now.Month && x.OutstandDate.Year == DateTime.Now.Year).ToListAsync();
                var newOS = data.spget_OutStandNew(LocId, Category).ToList();
                foreach (var v in newOS)
                {
                    var tbl = lst.Where(x => x.AccNo == v.AccNo).FirstOrDefault();
                    if (tbl == null)
                    {
                        if (v.DueAmt > 0)
                        {
                            tbl = new Lse_Outstand
                            {
                                AccNo = v.AccNo,
                                Category = Category,
                                LocId = LocId,
                                OutstandAmt = v.DueAmt,
                                OutstandDate = dt,
                                Status = v.RecvAmt == 0 ? "I" : (v.RecvAmt >= v.DueAmt ? "C" : "P"),
                                TransDate = DateTime.Now,
                                UserId = UserId,
                                RecvAmt = v.RecvAmt > v.DueAmt ? v.DueAmt : v.RecvAmt,
                                InstDate = v.InstallDate,
                                InstId = v.InstId
                            };
                            data.Lse_Outstand.Add(tbl);
                        }
                    }
                    else
                    {
                        tbl.OutstandAmt = v.DueAmt;
                        tbl.RecvAmt = v.RecvAmt > v.DueAmt ? v.DueAmt : v.RecvAmt;
                        tbl.Status = v.RecvAmt == 0 ? "I" : (v.RecvAmt >= v.DueAmt ? "C" : "P");
                        tbl.InstDate = v.InstallDate;
                        tbl.InstId = v.InstId;
                    }
                }

                var remLst = lst.Where(x => !(newOS.Select(a => a.AccNo).ToList()).Contains(x.AccNo)).ToList();
                if (remLst.Count > 0)
                    data.Lse_Outstand.RemoveRange(remLst);

                await data.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<dynamic> GetOutStandSummary(int LocId, string Category, DateTime OSMonth)
        {
            try
            {

                var lst = await db.Lse_Outstand.Where(x => x.LocId == LocId && x.OutstandDate.Month == OSMonth.Month && x.OutstandDate.Year == OSMonth.Year && x.Category == Category).ToListAsync();
                var data = new
                {
                    IssueQty = lst.Count(),
                    IssueAmt = lst.Sum(x => x.OutstandAmt),
                    ClearQty = lst.Where(x => x.Status == "C").Count(),
                    ClearAmt = lst.Sum(x => x.RecvAmt),
                    PartialQty = lst.Where(x => x.Status == "P").Count(),
                    PartialAmt = lst.Where(x => x.Status == "P").Sum(x => x.OutstandAmt - x.RecvAmt),
                    PendingQty = lst.Where(x => x.Status == "I").Count(),
                    PendingAmt = lst.Where(x => x.Status == "I").Sum(x => x.OutstandAmt)
                };
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<OutStandVM>> GetOutStand(int LocId, string Category, string Status, string Assign, DateTime OSMonth)
        {
            try
            {
                if (OSMonth.Month == DateTime.Now.Month && OSMonth.Year == DateTime.Now.Year)
                {
                    var lst = await (from O in db.Lse_Outstand
                                     join E in db.Pay_EmpMaster on O.RecoveryId equals E.EmpId into OS
                                     from E in OS.DefaultIfEmpty()
                                     where O.LocId == LocId && O.Category == Category
                                     && O.OutstandDate.Month == DateTime.Now.Month && O.OutstandDate.Year == DateTime.Now.Year
                                     && (O.Status == Status || Status == "")
                                     && ((!O.RecoveryId.HasValue && Assign == "U") || (O.RecoveryId.HasValue && Assign == "A") || Assign == "")
                                     select new OutStandVM
                                     {
                                         AccNo = O.AccNo,
                                         OldAccNo = O.Lse_Master.OldAccNo ?? 0,
                                         Category = O.Category,
                                         OutStandAmt = O.OutstandAmt,
                                         OutStandDate = O.OutstandDate,
                                         RecoveryId = O.RecoveryId ?? 0,
                                         Status = O.Status,
                                         TransId = O.TransId,
                                         Customer = O.Lse_Master.CustName,
                                         Inst = O.Lse_Master.MonthlyInst,
                                         RecvAmt = O.RecvAmt ?? 0,
                                         Remaning = O.OutstandAmt - (O.RecvAmt ?? 0),
                                         Recovery = O.IsAIC ? "AIC (Office)" : E.EmpName
                                     }).OrderBy(x => x.AccNo).ToListAsync();
                    return lst;
                }
                else
                {
                    var lst = await (from O in db.Lse_Outstand
                                     join MOS in db.Lse_MOS on O.AccNo equals MOS.AccNo
                                     join E in db.Pay_EmpMaster on O.RecoveryId equals E.EmpId into OS
                                     from E in OS.DefaultIfEmpty()
                                     where O.LocId == LocId && O.Category == Category
                                     && O.OutstandDate.Month == MOS.Month.Month && O.OutstandDate.Year == MOS.Month.Year
                                     && O.OutstandDate.Month == OSMonth.Month && O.OutstandDate.Year == OSMonth.Year
                                     && (O.Status == Status || Status == "")
                                     && ((!O.RecoveryId.HasValue && Assign == "U") || (O.RecoveryId.HasValue && Assign == "A") || Assign == "")
                                     && MOS.ActDue > 0
                                     select new OutStandVM
                                     {
                                         AccNo = O.AccNo,
                                         OldAccNo = O.Lse_Master.OldAccNo ?? 0,
                                         Category = O.Category,
                                         OutStandAmt = O.OutstandAmt,
                                         OutStandDate = O.OutstandDate,
                                         RecoveryId = O.RecoveryId ?? 0,
                                         Status = O.Status,
                                         TransId = O.TransId,
                                         Customer = O.Lse_Master.CustName,
                                         Inst = O.Lse_Master.MonthlyInst,
                                         RecvAmt = O.RecvAmt ?? 0,
                                         Remaning = O.OutstandAmt - (O.RecvAmt ?? 0),
                                         Recovery = E.EmpName
                                     }).OrderBy(x => x.AccNo).ToListAsync();
                    return lst;
                }

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> SaveOutStand(int RecId, long[] AccLst, bool IsAIC, int UserId)
        {
            try
            {
                foreach (var v in AccLst)
                {
                    var tbl = await db.Lse_Outstand.FindAsync(v);
                    if (IsAIC)
                    {
                        if ((tbl.Status == "I" || tbl.Status == "P") && tbl.IsAIC != IsAIC)
                        {
                            Lse_OutstandLog log = new Lse_OutstandLog
                            {
                                AccNo = tbl.AccNo,
                                Category = tbl.Category,
                                RecoveryId = tbl.RecoveryId,
                                Status = tbl.Status,
                                InstDate = tbl.InstDate,
                                InstId = tbl.InstId,
                                LocId = tbl.LocId,
                                OutstandAmt = tbl.OutstandAmt,
                                OutstandDate = tbl.OutstandDate,
                                RecvAmt = tbl.RecvAmt,
                                TransDate = tbl.TransDate,
                                UserId = tbl.UserId
                            };
                            db.Lse_OutstandLog.Add(log);

                            tbl.RecoveryId = null;
                            tbl.IsAIC = true;
                            tbl.UserId = UserId;
                            tbl.TransDate = DateTime.Now;

                        }
                    }
                    else
                    {
                        if ((tbl.Status == "I" || tbl.Status == "P") && tbl.RecoveryId != RecId)
                        {
                            if ((tbl.RecoveryId ?? 0) > 0 || tbl.IsAIC)
                            {
                                Lse_OutstandLog log = new Lse_OutstandLog
                                {
                                    AccNo = tbl.AccNo,
                                    Category = tbl.Category,
                                    RecoveryId = tbl.RecoveryId,
                                    Status = tbl.Status,
                                    InstDate = tbl.InstDate,
                                    InstId = tbl.InstId,
                                    LocId = tbl.LocId,
                                    OutstandAmt = tbl.OutstandAmt,
                                    OutstandDate = tbl.OutstandDate,
                                    RecvAmt = tbl.RecvAmt,
                                    TransDate = tbl.TransDate,
                                    UserId = tbl.UserId
                                };
                                db.Lse_OutstandLog.Add(log);
                            }
                            tbl.RecoveryId = RecId;
                            tbl.IsAIC = false;
                            tbl.UserId = UserId;
                            tbl.TransDate = DateTime.Now;

                        }
                    }
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }


        public async Task<bool> SaveOutStandAcc(int RecId, long AccNo, int LocId, bool IsAIC, int UserId)
        {
            try
            {
                var accno = db.Lse_Master.Where(x => x.AccNo == AccNo && x.LocId == LocId).FirstOrDefault();
                var wrldte = setupBL.GetWorkingDate(LocId);
                if (accno != null)
                {
                    var tbl = await db.Lse_Outstand.Where(x => x.AccNo == AccNo
                    && x.OutstandDate.Month == wrldte.Date.Month
                    && x.OutstandDate.Year == wrldte.Date.Year).FirstOrDefaultAsync();
                    if (tbl != null)
                    {
                        if (IsAIC)
                        {
                            if ((tbl.Status == "I" || tbl.Status == "P") && tbl.IsAIC != IsAIC)
                            {
                                Lse_OutstandLog log = new Lse_OutstandLog
                                {
                                    AccNo = tbl.AccNo,
                                    Category = tbl.Category,
                                    RecoveryId = tbl.RecoveryId,
                                    Status = tbl.Status,
                                    InstDate = tbl.InstDate,
                                    InstId = tbl.InstId,
                                    LocId = tbl.LocId,
                                    OutstandAmt = tbl.OutstandAmt,
                                    OutstandDate = tbl.OutstandDate,
                                    RecvAmt = tbl.RecvAmt,
                                    TransDate = tbl.TransDate,
                                    UserId = tbl.UserId
                                };
                                db.Lse_OutstandLog.Add(log);

                                tbl.RecoveryId = null;
                                tbl.IsAIC = true;
                                tbl.UserId = UserId;
                                tbl.TransDate = DateTime.Now;

                            }
                        }
                        else
                        {
                            if ((tbl.Status == "I" || tbl.Status == "P") && tbl.RecoveryId != RecId)
                            {
                                if ((tbl.RecoveryId ?? 0) > 0 || tbl.IsAIC)
                                {
                                    Lse_OutstandLog log = new Lse_OutstandLog
                                    {
                                        AccNo = tbl.AccNo,
                                        Category = tbl.Category,
                                        RecoveryId = tbl.RecoveryId,
                                        Status = tbl.Status,
                                        InstDate = tbl.InstDate,
                                        InstId = tbl.InstId,
                                        LocId = tbl.LocId,
                                        OutstandAmt = tbl.OutstandAmt,
                                        OutstandDate = tbl.OutstandDate,
                                        RecvAmt = tbl.RecvAmt,
                                        TransDate = tbl.TransDate,
                                        UserId = tbl.UserId
                                    };
                                    db.Lse_OutstandLog.Add(log);
                                }
                                tbl.RecoveryId = RecId;
                                tbl.IsAIC = false;
                                tbl.UserId = UserId;
                                tbl.TransDate = DateTime.Now;

                            }
                        }
                        await db.SaveChangesAsync();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }

        public async Task<bool> SaveOutStandAsLastMonth(int LocId, string Category, int UserId)
        {
            try
            {
                var lst = await (from O in db.Lse_Outstand
                                 where O.LocId == LocId && O.Category == Category
                                 && O.OutstandDate.Month == DateTime.Now.Month && O.OutstandDate.Year == DateTime.Now.Year
                                 && (O.Status == "I" || O.Status == "P")
                                 select O).ToListAsync();
                var month = DateTime.Now.AddMonths(-1).Month;
                var year = DateTime.Now.AddMonths(-1).Year;
                foreach (var v in lst)
                {
                    var last = await db.Lse_Outstand.Where(O => O.AccNo == v.AccNo && O.OutstandDate.Month == month && O.OutstandDate.Year == year).FirstOrDefaultAsync();
                    if (last != null)
                    {
                        if (last.RecoveryId > 0)
                        {
                            var emp = await db.Pay_EmpMaster.Where(x => x.EmpId == last.RecoveryId && x.DeptId == LocId && x.StatusId == "A").FirstOrDefaultAsync();
                            if (emp != null)
                            {
                                //if (emp.Pay_EmpRole.Where(x => x.RoleId == 3).Any())
                                //{
                                if ((v.Status == "I" || v.Status == "P") && v.RecoveryId != last.RecoveryId)
                                {
                                    if ((v.RecoveryId ?? 0) > 0 || v.IsAIC)
                                    {
                                        Lse_OutstandLog log = new Lse_OutstandLog
                                        {
                                            AccNo = v.AccNo,
                                            Category = v.Category,
                                            RecoveryId = v.RecoveryId,
                                            Status = v.Status,
                                            InstDate = v.InstDate,
                                            InstId = v.InstId,
                                            LocId = v.LocId,
                                            OutstandAmt = v.OutstandAmt,
                                            OutstandDate = v.OutstandDate,
                                            RecvAmt = v.RecvAmt,
                                            TransDate = v.TransDate,
                                            UserId = v.UserId
                                        };
                                        db.Lse_OutstandLog.Add(log);
                                    }

                                    v.RecoveryId = last.RecoveryId;
                                    v.UserId = UserId;
                                    v.TransDate = DateTime.Now;
                                }
                                //}
                            }
                        }
                    }
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        public async Task<bool> SaveOutStandAsInquiry(int LocId, string Category, int UserId)
        {
            try
            {
                var lst = await (from O in db.Lse_Outstand
                                 where O.LocId == LocId && O.Category == Category
                                 && O.OutstandDate.Month == DateTime.Now.Month && O.OutstandDate.Year == DateTime.Now.Year
                                 && (O.Status == "I" || O.Status == "P")
                                 select O).ToListAsync();

                foreach (var v in lst)
                {
                    var inq = v.Lse_Master.InqOfficerId;
                    var emp = await db.Pay_EmpMaster.Where(x => x.EmpId == inq && x.DeptId == LocId && x.StatusId == "A").FirstOrDefaultAsync();
                    if (emp != null)
                    {
                        //if (emp.Pay_EmpRole.Where(x => x.RoleId == 3).Any())
                        //{
                        if ((v.Status == "I" || v.Status == "P") && v.RecoveryId != inq)
                        {
                            if ((v.RecoveryId ?? 0) > 0 || v.IsAIC)
                            {
                                Lse_OutstandLog log = new Lse_OutstandLog
                                {
                                    AccNo = v.AccNo,
                                    Category = v.Category,
                                    RecoveryId = v.RecoveryId,
                                    Status = v.Status,
                                    InstDate = v.InstDate,
                                    InstId = v.InstId,
                                    LocId = v.LocId,
                                    OutstandAmt = v.OutstandAmt,
                                    OutstandDate = v.OutstandDate,
                                    RecvAmt = v.RecvAmt,
                                    TransDate = v.TransDate,
                                    UserId = v.UserId
                                };
                                db.Lse_OutstandLog.Add(log);
                            }

                            v.RecoveryId = inq;
                            v.UserId = UserId;
                            v.TransDate = DateTime.Now;
                        }
                        //}
                    }
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return false;
            }
        }
        #endregion

        #region DayOpenClose
        public async Task<DateTime?> GetWorkingDate(int LocId)
        {
            try
            {
                return db.Comp_Locations.Where(x => x.LocId == LocId).Select(x => (DateTime)x.WorkingDate).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<DayClosingVM> GetDayOpen(int LocId)
        {
            try
            {
                DayClosingVM mod = new DayClosingVM();
                var workingDate = (await GetWorkingDate(LocId)) ?? DateTime.Now.Date;
                var dayOpen = await db.Lse_DayClosing.Where(x => x.WorkingDate == workingDate && x.LocId == LocId).FirstOrDefaultAsync();
                if (dayOpen != null)
                {
                    //if (dayOpen.DayCloseDate == null)
                    //{
                    //    mod.OpeningCash = dayOpen.OpeningCash;
                    //    mod.DayStartDate = dayOpen.DayStartDate;
                    //    mod.WorkingDate = dayOpen.WorkingDate;
                    //    mod.Status = "C";
                    //}
                    //else
                    //{
                    mod.WorkingDate = dayOpen.WorkingDate;
                    mod.Status = "A";
                    //}
                }
                else
                {

                    //mod.OpeningCash = (await new CashBL().GetCashCollection(LocId)).Where(x => x.CashHeadId == 1).Select(x => x.CashAmount).FirstOrDefault();
                    mod.DayStartDate = workingDate;
                    mod.WorkingDate = workingDate;
                    mod.Status = "C";
                };
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<DayClosingVM> DayOpen(DayClosingVM mod, int UserId)
        {
            try
            {
                var workingDate = await GetWorkingDate(mod.LocId) ?? DateTime.Now.Date;
                if (workingDate != mod.WorkingDate)
                    return null;

                var IsExist = await db.Lse_DayClosing.Where(x => x.WorkingDate == workingDate && x.LocId == mod.LocId).AnyAsync();
                if (IsExist)
                    return null;

                //var cashLst = await new CashBL().GetCashCollection(mod.LocId);
                decimal? vaultCash = 0;
                var loctypeId = await db.Comp_Locations.Where(x => x.LocId == mod.LocId).Select(x => x.LocTypeId).FirstOrDefaultAsync();
                if (loctypeId == 4)
                {
                    vaultCash = db.spRep_DayCloseingCash(mod.LocId, workingDate).Select(x => x.VaultCash).FirstOrDefault();
                }



                Lse_DayClosing tbl = new Lse_DayClosing();
                //if (mod.Status == "O")
                //{
                tbl.LocId = mod.LocId;
                tbl.OpeningCash = 0;// cashLst.Where(x => x.CashHeadId == 1).Select(x => x.CashAmount).FirstOrDefault();
                tbl.DayStartDate = workingDate;
                tbl.WorkingDate = mod.WorkingDate;
                tbl.UserId = UserId;
                tbl.TransDate = DateTime.Now;
                tbl.SysClosingCash = 0;// cashLst.Sum(x => x.CashAmount);
                tbl.ClosingCash = mod.ClosingCash;
                tbl.VaultCash = vaultCash;
                tbl.DayCloseDate = DateTime.Now;
                db.Lse_DayClosing.Add(tbl);
                //}
                //else if (mod.Status == "C")
                //{
                //    tbl = await db.Lse_DayClosing.Where(x => x.WorkingDate == workingDate && x.LocId == mod.LocId && x.DayCloseDate == null).FirstOrDefaultAsync();
                //    tbl.DayCloseDate = DateTime.Now;
                //    tbl.ClosingCash = mod.ClosingCash;

                //    var cl = db.spget_DayClose(workingDate, mod.LocId).FirstOrDefault();
                //    tbl.SysClosingCash = cl.SysClosingCash ?? 0;
                //tbl.TotalTrans = cl.TotalTrans;
                //tbl.CashSaleTrans = cl.CashSaleTrans;
                //tbl.CardSaleTrans = cl.CardSaleTrans;
                //tbl.CreditSaleTrans = cl.CreditSaleTrans;
                //tbl.ProcessingTrans = cl.ProcessingTrans;
                //tbl.InstSaleTrans = cl.InstSaleTrans;
                //tbl.InquiryTrans = cl.InquiryTrans;
                //tbl.RecoveryTrans = cl.RecoveryTrans;
                //tbl.InstCollectionTrans = cl.InstCollectionTrans;
                //tbl.SaleReturnTrans = cl.SaleReturnTrans;
                //tbl.SaleReturn = cl.SaleReturn;
                //tbl.CreditSaleReturnTrans = cl.CreditSaleReturnTrans;
                //tbl.CreditSaleReturn = cl.CreditSaleReturn;
                //tbl.CashSales = cl.CashSales;
                //tbl.CardSales = cl.CardSales ?? 0;
                //tbl.CreditSales = cl.CreditSales;
                //tbl.CreditAdvance = cl.CreditAdvance;
                //tbl.InstSale = cl.InstSale;
                //tbl.AdvanceRcvd = cl.AdvanceRcvd;
                //tbl.InstCollection = cl.InstCollection;
                //tbl.TodayOpenAcs = cl.TodayOpenAcs;
                //tbl.TodayClosedAcs = cl.TodayClosedAcs;
                //tbl.TotalOpenAcs = cl.TotalOpenAcs;
                //tbl.TotalClosedAcs = cl.TotalClosedAcs;
                //tbl.FOutstand = cl.FOutstand;
                //tbl.ROutstand = cl.ROutstand;
                //tbl.FOutstandRecovered = cl.FOutstandRecovered;
                //tbl.ROutstandRecovered = cl.ROutstandRecovered;
                //tbl.FOutstandTrans = cl.FOutstandTrans;
                //tbl.ROutstandTrans = cl.ROutstandTrans;
                //tbl.FOutstandRecoveredTrans = cl.FOutstandRecoveredTrans;
                //tbl.ROutstandRecoveredTrans = cl.ROutstandRecoveredTrans;
                //tbl.Discount = cl.Discount;
                //tbl.Expenses = cl.Expenses;
                //tbl.ProcessingFee = cl.ProcessingFee;
                //tbl.FineFee = cl.FineFee;
                //tbl.OtherRA = cl.OtherRA;
                //tbl.OtherPA = cl.OtherPA;
                //}
                await db.SaveChangesAsync();
                mod.TransId = tbl.TransId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Expense
        public async Task<List<ExpenseTransactionVM>> SaveExpense(List<ExpenseTransactionVM> lst, string UploadedFiles, int LocId, DateTime WorkingDate, string RefBillNo, long TicketId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var mod in lst)
                    {
                        Lse_ExpenseTransaction tbl = new Lse_ExpenseTransaction
                        {
                            Description = mod.Description,
                            ExpHeadId = mod.ExpHeadId,
                            LocId = LocId,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            CCCode = LocId,
                            WorkingDate = WorkingDate,
                            Amount = mod.Amount,
                            Status = "U",
                            TicketId = TicketId,
                            RefBillNo = RefBillNo
                        };
                        db.Lse_ExpenseTransaction.Add(tbl);
                        await db.SaveChangesAsync();
                        mod.TransId = tbl.TransId;
                    }
                    if (!String.IsNullOrWhiteSpace(UploadedFiles))
                    {
                        {
                            List<long> files = UploadedFiles.Split(',').Select(long.Parse).ToList();
                            var IsSave = await new DocumentBL().UpdateDocRef(files, lst[0].TransId);
                            if (!IsSave)
                                scop.Dispose();
                        }

                        for (int i = 1; i < lst.Count(); i++)
                        {
                            List<long> files = UploadedFiles.Split(',').Select(long.Parse).ToList();
                            var IsSave = await new DocumentBL().UpdateDocRef(files, lst[i].TransId);
                            if (!IsSave)
                                scop.Dispose();
                        }
                    }

                    scop.Complete();
                    scop.Dispose();
                    return lst;
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    return null;
                }
            }
        }
        public async Task<long> SaveExpense(ExpenseTransactionVM mod, int UserId)
        {
            mod.WorkingDate = setupBL.GetWorkingDate(mod.LocId);
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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
                        CCCode = mod.LocId,
                        WorkingDate = mod.WorkingDate,
                        Amount = mod.Amount,
                        Status = "U",
                        TicketId = mod.TicketId,
                        RefBillNo = mod.RefBillNo
                    };
                    db.Lse_ExpenseTransaction.Add(tbl);
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
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    return 0;
                }
            }
        }
        #endregion

        #region Complain
        public async Task<ComplainVM> RegisterComplain(ComplainVM Com, int UserId)
        {
            try
            {

                Inv_Complain tbl = new Inv_Complain
                {
                    ComplainDate = DateTime.Now,
                    ItemId = 0,
                    SKUId = 0,
                    SrNo = Com.SrNo,
                    SaleType = Com.SaleType,
                    TransId = Com.TransId,
                    Customer = Com.Customer,
                    CustomerMobile = Com.CustomerMobile,
                    Complain = Com.Complain,
                    Communicator = Com.Communicator,
                    CommunicatorMobile = Com.CommunicatorMobile,
                    CComplainNo = Com.CComplainNo,
                    Action = Com.Action,
                    Status = "O",
                    TransDate = DateTime.Now,
                    UserId = UserId,

                };
                db.Inv_Complain.Add(tbl);
                await db.SaveChangesAsync();
                return Com;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion


        public async Task<List<LSECRCFinePolicy>> GetPolicyDetail()
        {
            return await (from item in db.LSE_CrcFinePolicy
                              // join itemdet in db.LSE_CrcFinePolicyDtl on item.PolicyId equals itemdet.PolicyId
                              //join desg in db.Pay_Designation on itemdet.DesgId equals desg.DesgId
                          where item.Status == true
                          select new LSECRCFinePolicy()
                          {
                              PolicyCode = item.PolicyCode,
                              ShortDesc = item.ShortDesc,
                              PolicyDetail = item.PolicyDetail,
                              PolicyId = item.PolicyId,
                              //  Amount = itemdet.FineAmt,
                              //DesgName = desg.DesgName
                          }).ToListAsync();
        }


        public async Task<CrcFinesVM> SaveCrcFines(CrcFinesVM Com, int UserId)
        {
            try
            {


                Lse_CrcFines tbl = new Lse_CrcFines
                {
                    AccountNo = Com.AccountNo,
                    FineToEmp = Com.FineToEmp,
                    FineAmt = Com.FineAmt,
                    CRC = UserId,
                    FineReason = Com.FineReason,
                    FineDate = DateTime.Now,
                    TransDate = DateTime.Now,
                    PolicyId = Com.PolicyId,
                    LocId = Com.LocId

                };
                db.Lse_CrcFines.Add(tbl);
                await db.SaveChangesAsync();
                Com.Id = tbl.FineId;
                return Com;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return null;
            }
        }


        public async Task<List<CrcFinesVM>> CrcFinesList(int UserId)
        {
            try
            {
                var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                return await (from x in db.Lse_CrcFines
                              join d in db.Pay_EmpMaster on x.FineToEmp equals d.EmpId
                              join s in db.Pay_Department on d.DeptId equals s.DeptId
                              join e in db.Pay_Designation on d.DesgId equals e.DesgId
                              where x.CRC == UserId && x.FineDate >= dt
                              select new CrcFinesVM
                              {
                                  AccountNo = x.AccountNo,
                                  FineToEmp = x.FineToEmp,
                                  FineAmt = x.FineAmt,
                                  CRC = x.CRC,
                                  FineReason = x.FineReason,
                                  FineDate = x.FineDate,
                                  EmpName = d.EmpName,
                                  DesgName = e.DesgName

                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<CrcFinesVM> CrcFinesListForApproval(int UserId, int GroupId, int LocId)
        {
            try
            {
                return db.spPay_CRCFines(UserId, LocId).Select(x => new CrcFinesVM()
                {
                    AccountNo = x.AccountNo,
                    ApprovedBy = x.ApprovedBy,
                    ApprovedByName = x.ApprovedByName,
                    ApprovedDate = x.ApprovedDate,
                    CRC = x.CRC,
                    DesgName = x.DesgName,
                    EmpName = x.EmpName,
                    FineAmt = x.FineAmt,
                    FineDate = x.FineDate,
                    FineReason = x.FineReason,
                    FineToEmp = x.FineToEmp,
                    Id = x.Id,
                    RMApprovalTransDate = x.RMApprovalTransDate,
                    RMApprovedBy = x.RMApprovedBy,
                    RMApprovedName = x.RMApprovedName,
                    DeptName = x.DeptName
                }).ToList();
              

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> RejectCRCFine(int UserId, long FineId)
        {
            try
            {
                var crcapproe = await db.Lse_CrcFines.Where(x => x.FineId == FineId && x.ApprovedBy == null).FirstOrDefaultAsync();
                if (crcapproe != null)
                {
                    crcapproe.RejectedBy = UserId;
                    crcapproe.RejectedDate = DateTime.Now;
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

        public async Task<bool> ApproveCRCFine(int GroupId, int UserId, long FineId)
        {
            try
            {

                //Senior Auditor , Senior Auditor ISB,RWP
                if (GroupId == 1056 || GroupId == 1072)
                {
                    var crcapproe = await db.Lse_CrcFines.Where(x => x.FineId == FineId && x.RMApproval != null && x.ApprovedBy == null).FirstOrDefaultAsync();
                    if (crcapproe != null)
                    {
                        crcapproe.ApprovedBy = UserId;
                        crcapproe.ApprovedDate = DateTime.Now;
                        await db.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //RM , SRM , BDM
                else if (GroupId == 4 || GroupId == 5 || GroupId == 1060)
                {
                    var crcapproe = await db.Lse_CrcFines.Where(x => x.FineId == FineId && x.RMApproval == null).FirstOrDefaultAsync();
                    if (crcapproe != null)
                    {
                        crcapproe.RMApproval = UserId;
                        crcapproe.RMApprovalDate = DateTime.Now;
                        await db.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<bool> WriteLog(string trace, string strMessage)
        {
            try
            {
                FileStream objFilestream = new FileStream("D:\\Web\\logs.txt", FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                await objStreamWriter.WriteLineAsync(DateTime.Now.ToString() + "--" + trace + "--" + strMessage);
                objStreamWriter.Close();
                objFilestream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}