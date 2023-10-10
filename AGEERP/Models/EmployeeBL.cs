using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace AGEERP.Models
{
    public class EmployeeBL
    {
        private AGEEntities db = new AGEEntities();

        #region BasicSalaryPolicy
        public async Task<List<BasicSalaryPolicyVM>> GetBasciSalaryPolicy()
        {
            try
            {
                return await (from bsp in db.Pay_BasicSalaryPolicy
                              join desg in db.Pay_Designation on bsp.DesgId equals desg.DesgId
                              join slab in db.Pay_BasicSalarySlab on bsp.SlabId equals slab.SlabId
                              select new { bsp.PolicyId, bsp.PolicyTitle, bsp.EffectiveDate, desg.DesgName, slab.SlabTitle }).Select(x =>
                                   new BasicSalaryPolicyVM
                                   {
                                       PolicyId = x.PolicyId,
                                       PolicyTitle = x.PolicyTitle,
                                       EffectiveDate = x.EffectiveDate,
                                       Designation = x.DesgName,
                                       SlabTitle = x.SlabTitle
                                   }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<BasicSalarySlabVM>> SlabList()
        {
            try
            {
                return await db.Pay_BasicSalarySlab.Where(x => x.Status).Select(x =>
                  new BasicSalarySlabVM
                  {
                      SlabId = x.SlabId,
                      SlabTitle = x.SlabTitle
                  }).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
        #region BranchMonthlyClosing
        public List<BranchMonthlyClosingVM> BranchMonthlyClosing(DateTime ClosingMonth, int CityId, int LocId)
        {
            db.Database.CommandTimeout = 3600;
            return db.spPay_Branches_Month_Closing_Summary(ClosingMonth, CityId, LocId).Select(x => new BranchMonthlyClosingVM()
            {
                AIC_Adj = x.AIC_Adj,
                AIC_CMonth = x.AIC_CMonth,
                CityCode = x.CityCode,
                INC_Adj = x.INC_Adj,
                INC_Adj_BM = x.INC_Adj_BM,
                LocCode = x.LocCode,
                LocId = x.LocId,
                Outstand_F = x.Outstand_F,
                Outstand_R = x.Outstand_R,
                Salary = x.Salary,
                Sale_Target_BM = x.Sale_Target_BM,
                Sale_Target_BR = x.Sale_Target_BR,
                STAFF_Adj = x.STAFF_Adj,
                IsClosed = x.IsClosed
            }).ToList();
        }

        public string BranchMonthlyClosingPosting(List<BranchMonthlyClosingVM> mod, DateTime ClosingMonth, int UserId)
        {
            if (mod != null)
            {
                foreach (var item in mod)
                {
                    if (item.IsClosed == null)
                    {
                        db.spPay_Post_Branch_Closing_Voucher(ClosingMonth, item.LocId, UserId);
                    }
                }
                return "Data Posting Completed";
            }
            else
            {
                return "No Data Present";
            }
        }


        #endregion
        #region ProductIncentivePolicy
        public async Task<List<ProductIncPolicyVM>> GetProductIncPolicy(DateTime FDate, DateTime TDate)
        {
            return await (from item in db.Pay_ProductIncPolicy
                          join user in db.Pay_EmpMaster on item.DefinedBy equals user.EmpId
                          where item.PolicyStatus == true && item.PolicyStartDate >= FDate && item.PolicyExpiryDate <= TDate
                          select new ProductIncPolicyVM()
                          {
                              PolicyId = item.PolicyId,
                              SaleType = item.SaleType,
                              PolicyTitle = item.PolicyTitle,
                              PolicyStartDate = item.PolicyStartDate,
                              PolicyExpiryDate = item.PolicyExpiryDate,
                              PolicyStatus = item.PolicyStatus,
                              Remarks = item.Remarks,
                              Cities = db.Pay_ProductIncPolicyLocations.Where(x => x.PolicyId == item.PolicyId).Select(x => x.CityId).ToList(),
                              Locations = db.Pay_ProductIncPolicyLocations.Where(x => x.PolicyId == item.PolicyId).Select(x => x.LocId).ToList()
                          }).ToListAsync();
        }

        public async Task<ProductIncPolicyVM> AddProducIncPolicy(ProductIncPolicyVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (mod.PolicyId > 0)
                    {
                        var policy = await db.Pay_ProductIncPolicy.Where(x => x.PolicyId == mod.PolicyId).FirstOrDefaultAsync();
                        policy.SaleType = mod.SaleType;
                        policy.PolicyTitle = mod.PolicyTitle;
                        policy.PolicyStartDate = mod.PolicyStartDate;
                        policy.PolicyExpiryDate = mod.PolicyExpiryDate;
                        policy.Remarks = mod.Remarks;
                        policy.ModifiedBy = UserId;
                        policy.PolicyStatus = mod.PolicyStatus;

                        policy.ModifiedDate = DateTime.Now;
                        await db.SaveChangesAsync();

                        var citilocs = db.Pay_ProductIncPolicyLocations.Where(x => x.PolicyId == policy.PolicyId).ToList();
                        db.Pay_ProductIncPolicyLocations.RemoveRange(citilocs);
                        await db.SaveChangesAsync();

                        if (mod.Locations.FirstOrDefault() == 0)
                        {
                            foreach (var item in mod.Cities)
                            {
                                Pay_ProductIncPolicyLocations md = new Pay_ProductIncPolicyLocations();
                                md.PolicyId = policy.PolicyId;
                                md.LocId = 0;
                                md.CityId = item;
                                db.Pay_ProductIncPolicyLocations.Add(md);
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            foreach (var item in mod.Locations)
                            {
                                var city = await db.Comp_Locations.Where(x => x.LocId == item).FirstOrDefaultAsync();
                                Pay_ProductIncPolicyLocations md = new Pay_ProductIncPolicyLocations();
                                md.PolicyId = policy.PolicyId;
                                md.LocId = item;
                                md.CityId = city.CityId;
                                db.Pay_ProductIncPolicyLocations.Add(md);
                                await db.SaveChangesAsync();
                            }
                        }

                        mod.PolicyId = policy.PolicyId;
                    }
                    else
                    {
                        Pay_ProductIncPolicy md = new Pay_ProductIncPolicy();
                        md.SaleType = mod.SaleType;
                        md.PolicyTitle = mod.PolicyTitle;
                        md.PolicyStartDate = mod.PolicyStartDate;
                        md.PolicyExpiryDate = mod.PolicyExpiryDate;
                        md.PolicyStatus = true;
                        md.DefinedBy = UserId;
                        md.Remarks = mod.Remarks;
                        md.DefinedDate = DateTime.Now;
                        db.Pay_ProductIncPolicy.Add(md);
                        await db.SaveChangesAsync();

                        if (mod.Locations.FirstOrDefault() == 0)
                        {
                            foreach (var item in mod.Cities)
                            {
                                Pay_ProductIncPolicyLocations mds = new Pay_ProductIncPolicyLocations();
                                mds.PolicyId = md.PolicyId;
                                mds.LocId = 0;
                                mds.CityId = item;
                                db.Pay_ProductIncPolicyLocations.Add(mds);
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            foreach (var item in mod.Locations)
                            {
                                var city = await db.Comp_Locations.Where(x => x.LocId == item).FirstOrDefaultAsync();
                                Pay_ProductIncPolicyLocations mds = new Pay_ProductIncPolicyLocations();
                                mds.PolicyId = md.PolicyId;
                                mds.LocId = item;
                                mds.CityId = city.CityId;
                                db.Pay_ProductIncPolicyLocations.Add(mds);
                                await db.SaveChangesAsync();
                            }
                        }
                        mod.PolicyId = md.PolicyId;
                    }

                    scop.Complete();
                    scop.Dispose();
                    return mod;
                }
                catch (Exception ex)
                {
                    scop.Dispose();
                    return null;
                }
            }
        }

        public async Task<bool> CopyProductPolicyDateRange(DateTime FromDate, DateTime ToDate, DateTime CopyFromDate, DateTime CopyToDate, int UserId)
        {
            try
            {
                var oldPolicylst = await db.Pay_ProductIncPolicy.Where(x => x.PolicyStartDate >= FromDate && x.PolicyExpiryDate <= ToDate).ToListAsync();
                foreach (var item in oldPolicylst)
                {
                    var oldPolicy = await db.Pay_ProductIncPolicy.Where(x => x.PolicyId == item.PolicyId).FirstOrDefaultAsync();
                    var policy = new Pay_ProductIncPolicy();
                    policy.PolicyTitle = oldPolicy.PolicyTitle;
                    policy.PolicyStartDate = CopyFromDate;
                    policy.PolicyExpiryDate = CopyToDate;
                    policy.DefinedDate = DateTime.Now;
                    policy.DefinedBy = UserId;
                    policy.PolicyStatus = true;
                    policy.SaleType = oldPolicy.SaleType;
                    policy.Remarks = oldPolicy.Remarks;
                    db.Pay_ProductIncPolicy.Add(policy);
                    await db.SaveChangesAsync();

                    var existdet = await db.Pay_ProductIncPolicyDetail.Where(x => x.PolicyId == item.PolicyId).ToListAsync();



                    foreach (var items in existdet)
                    {
                        Pay_ProductIncPolicyDetail md = new Pay_ProductIncPolicyDetail();
                        md.PolicyId = policy.PolicyId;
                        md.TypeId = items.TypeId;
                        md.ModelId = items.ModelId;
                        md.SkuId = items.SkuId;
                        md.SerialNo = items.SerialNo;
                        md.Condition = items.Condition;
                        md.SalePrice = items.SalePrice;
                        md.IncAmount = items.IncAmount;
                        md.IncQty = 1;
                        db.Pay_ProductIncPolicyDetail.Add(md);
                    }
                    await db.SaveChangesAsync();


                    var citilocs = db.Pay_ProductIncPolicyLocations.Where(x => x.PolicyId == item.PolicyId).ToList();

                    foreach (var itemss in citilocs)
                    {
                        Pay_ProductIncPolicyLocations md = new Pay_ProductIncPolicyLocations();
                        md.PolicyId = policy.PolicyId;
                        md.LocId = itemss.LocId;
                        md.CityId = itemss.CityId;
                        db.Pay_ProductIncPolicyLocations.Add(md);
                        await db.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<bool> CopyProductPolicyFromPrev(int id, string PolicyTitle, DateTime startdate, DateTime EndDate, int UserId)
        {
            try
            {
                var oldPolicy = await db.Pay_ProductIncPolicy.Where(x => x.PolicyId == id).FirstOrDefaultAsync();
                var policy = new Pay_ProductIncPolicy();
                policy.PolicyTitle = PolicyTitle;
                policy.PolicyStartDate = startdate;
                policy.PolicyExpiryDate = EndDate;
                policy.DefinedDate = DateTime.Now;
                policy.DefinedBy = UserId;
                policy.PolicyStatus = true;
                policy.SaleType = oldPolicy.SaleType;
                policy.Remarks = oldPolicy.Remarks;
                db.Pay_ProductIncPolicy.Add(policy);
                await db.SaveChangesAsync();

                var existdet = await db.Pay_ProductIncPolicyDetail.Where(x => x.PolicyId == id).ToListAsync();



                foreach (var item in existdet)
                {
                    Pay_ProductIncPolicyDetail md = new Pay_ProductIncPolicyDetail();
                    md.PolicyId = policy.PolicyId;
                    md.TypeId = item.TypeId;
                    md.ModelId = item.ModelId;
                    md.SkuId = item.SkuId;
                    md.SerialNo = item.SerialNo;
                    md.Condition = item.Condition;
                    md.SalePrice = item.SalePrice;
                    md.IncAmount = item.IncAmount;
                    md.IncQty = 1;
                    db.Pay_ProductIncPolicyDetail.Add(md);
                }
                await db.SaveChangesAsync();


                var citilocs = db.Pay_ProductIncPolicyLocations.Where(x => x.PolicyId == id).ToList();

                foreach (var item in citilocs)
                {
                    Pay_ProductIncPolicyLocations md = new Pay_ProductIncPolicyLocations();
                    md.PolicyId = policy.PolicyId;
                    md.LocId = item.LocId;
                    md.CityId = item.CityId;
                    db.Pay_ProductIncPolicyLocations.Add(md);
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<bool> DeletePolicy(int id)
        {
            try
            {
                var policy = await db.Pay_ProductIncPolicy.Where(x => x.PolicyId == id).FirstOrDefaultAsync();
                policy.PolicyStatus = false;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<List<ProductIncPolicyVM>> GetProductIncPolicyById(int id)
        {
            return await (from item in db.Pay_ProductIncPolicy
                          where item.PolicyStatus == true && item.PolicyId == id
                          select new ProductIncPolicyVM()
                          {
                              PolicyId = item.PolicyId,
                              SaleType = item.SaleType,
                              PolicyTitle = item.PolicyTitle,
                              PolicyStartDate = item.PolicyStartDate,
                              PolicyExpiryDate = item.PolicyExpiryDate,
                              PolicyStatus = item.PolicyStatus,
                              Remarks = item.Remarks,
                              Cities = db.Pay_ProductIncPolicyLocations.Where(x => x.PolicyId == item.PolicyId).Select(x => x.CityId).ToList(),
                              Locations = db.Pay_ProductIncPolicyLocations.Where(x => x.PolicyId == item.PolicyId).Select(x => x.LocId).ToList()
                          }).ToListAsync();
        }

        public async Task<List<ProductIncPolicyDetailVM>> GetProductIncDetails(int PolicyId)
        {
            return await (from item in db.Pay_ProductIncPolicyDetail
                          join type in db.Itm_Type on item.TypeId equals type.TypeId
                          where item.PolicyId == PolicyId
                          select new ProductIncPolicyDetailVM()
                          {
                              CompanyId = type.ComId,
                              ProductId = type.ProductId,
                              ModelId = item.ModelId,
                              ProductClassId = item.ProductClassId,
                              SkuId = item.SkuId,
                              IncAmount = item.IncAmount,
                              IncQty = item.IncQty,
                              SerialNo = item.SerialNo,
                              PolicyDtlId = item.PolicyDtlId,
                              Condition = item.Condition,
                              SalePrice = item.SalePrice,
                              TypeId = item.TypeId,
                              PolicyId = item.PolicyId
                          }).ToListAsync();
        }

        public async Task<bool> AddProductPolicyDetail(List<ProductIncPolicyDetailVM> mod)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var PolicyId = mod.FirstOrDefault().PolicyId;
                    var existdet = await db.Pay_ProductIncPolicyDetail.Where(x => x.PolicyId == PolicyId).ToListAsync();
                    db.Pay_ProductIncPolicyDetail.RemoveRange(existdet);
                    await db.SaveChangesAsync();

                    foreach (var item in mod)
                    {
                        var tyoe = await db.Itm_Type.Where(x => x.ComId == item.CompanyId && x.ProductId == item.ProductId && x.Status).FirstOrDefaultAsync();
                        Pay_ProductIncPolicyDetail md = new Pay_ProductIncPolicyDetail();
                        md.PolicyId = PolicyId;
                        md.TypeId = tyoe.TypeId;
                        md.ModelId = item.ModelId;
                        md.SkuId = item.SkuId;
                        md.SerialNo = item.SerialNo;
                        md.Condition = item.Condition;
                        md.ProductClassId = item.ProductClassId;
                        md.SalePrice = item.SalePrice;
                        md.IncAmount = item.IncAmount;
                        md.IncQty = 1;
                        db.Pay_ProductIncPolicyDetail.Add(md);
                    }
                    await db.SaveChangesAsync();
                    scop.Complete();
                    scop.Dispose();

                    return true;
                }
                catch (Exception e)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }

        public async Task<bool> DeletePorductIncDetail(ProductIncPolicyDetailVM mod)
        {
            try
            {
                var existproddetail = await db.Pay_ProductIncPolicyDetail.Where(x => x.PolicyDtlId == mod.PolicyDtlId).FirstOrDefaultAsync();
                db.Pay_ProductIncPolicyDetail.Remove(existproddetail);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion
        #region ProductIncentiveApproval

        public List<ProductIncPeriod> GetProductIncPeriod(DateTime Month)
        {
            return db.Pay_ProductIncCalendar.Where(x => x.Month.Month == Month.Month && x.Month.Year == Month.Year).ToList().Select(x => new ProductIncPeriod()
            {
                FromDate = x.FromDate.ToString("dd/MM/yyyy"),
                Month = x.Month.ToString("dd/MM/yyyy"),
                RowId = x.RowId,
                ToDate = x.ToDate.ToString("dd/MM/yyyy"),
                Week = x.Week
            }).OrderBy(x => x.Week).ToList();
        }

        public List<ProductIncApprovalVM> ProcessIncentiveApproval(DateTime Month, int WeekId, int CityId)
        {
            try
            {
                var Period = db.Pay_ProductIncCalendar.Where(x => x.Month.Month == Month.Month && x.Month.Year == Month.Year && x.RowId == WeekId).FirstOrDefault();

                var lst = db.spPay_GetProductIncApproval(Period.FromDate, Period.ToDate, CityId).Select(x => new ProductIncApprovalVM()
                {
                    CashInc = x.CashInc,
                    InstInc = x.InstInc,
                    LocName = x.LocName,
                    RowId = x.RowId,
                    Salesman = x.Salesman,
                    LocId = x.LocId,
                    EmpName = x.EmpName,
                    TotalInc = x.TotalInc,
                    ApprovedBy = x.ApprovedBy,
                    ApprovedDate = x.ApprovedDate

                }).ToList();
                return lst;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> ProductApproveIncentive(IEnumerable<ProductIncApprovalVM> mod, int UserId, int PeriodId, int CityId)
        {
            try
            {
                db.Database.CommandTimeout = 3600;
                var Period = await db.Pay_ProductIncCalendar.Where(x => x.RowId == PeriodId).FirstOrDefaultAsync();
                Pay_ProductInc process = new Pay_ProductInc();
                using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        process = await db.Pay_ProductInc.Where(x => x.FromDate >= Period.FromDate && x.ToDate <= Period.ToDate && x.CityId == CityId).FirstOrDefaultAsync();
                        //int processid = 0;
                        foreach (var item in mod)
                        {
                            if (item.selectable == true)
                            {
                                var InstallmentInc = await db.Pay_ProductIncInst.Where(x => x.Salesman == item.Salesman && x.LocId == item.LocId && x.BillDate >= Period.FromDate && x.BillDate <= Period.ToDate).ToListAsync();
                                var CashInc = await db.Pay_ProductIncCash.Where(x => x.Salesman == item.Salesman && x.LocId == item.LocId && x.BillDate >= Period.FromDate && x.BillDate <= Period.ToDate).ToListAsync();

                                foreach (var itminst in InstallmentInc)
                                {
                                    //var inst = await db.Pay_ProductIncInst.Where(x => x.RowId == itminst.RowId).ToListAsync();
                                    itminst.ApprovedBy = UserId;
                                    itminst.ApprovedDate = DateTime.Now;
                                    await db.SaveChangesAsync();
                                }

                                foreach (var itmcash in CashInc)
                                {
                                    //var cash = await db.Pay_ProductIncInst.Where(x => x.RowId == itmcash.RowId).ToListAsync();
                                    itmcash.ApprovedBy = UserId;
                                    itmcash.ApprovedDate = DateTime.Now;
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        var proc = await db.Pay_ProductInc.Where(x => x.ProcessId == process.ProcessId).FirstOrDefaultAsync();
                        proc.IsLocked = true;
                        await db.SaveChangesAsync();

                        db.spPay_Post_Product_Incentives(Period.FromDate, Period.ToDate, CityId, UserId);

                        scop.Complete();
                        scop.Dispose();

                        return true;
                    }
                    catch (Exception e)
                    {
                        scop.Dispose();
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion
        #region ProductIncentiveCalc

        public bool ProcessIncentiveCalc(int PeriodId, int CityId, int UserId)
        {
            try
            {
                var calendar = db.Pay_ProductIncCalendar.Where(x => x.RowId == PeriodId).FirstOrDefault();
                var lockcheck = db.Pay_ProductInc.Where(x => x.FromDate >= calendar.FromDate && x.ToDate <= calendar.ToDate && x.CityId == CityId && x.IsLocked == true).ToList();
                if (lockcheck.Count == 0)
                {
                    var process = db.Pay_ProductInc.Where(x => x.FromDate >= calendar.FromDate && x.ToDate <= calendar.ToDate && x.CityId == CityId).FirstOrDefault();
                    if (process != null)
                    {
                        var productinc = process.ProcessId;
                        var prodins = db.Pay_ProductIncInst.Where(x => x.ProcessId == productinc).ToList();
                        var prodcash = db.Pay_ProductIncCash.Where(x => x.ProcessId == productinc).ToList();
                        db.Pay_ProductIncInst.RemoveRange(prodins);
                        db.Pay_ProductIncCash.RemoveRange(prodcash);
                        db.Pay_ProductInc.Remove(process);
                        db.SaveChanges();
                    }

                    db.Database.CommandTimeout = 3600;
                    db.spProductIncentiveCalc(calendar.FromDate, calendar.ToDate, CityId, UserId);
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

        public async Task<List<ProductIncVM>> GetProductIncentive()
        {

            return await (from item in db.Pay_ProductInc
                          join city in db.Comp_City on item.CityId equals city.CityId
                          join users in db.Pay_EmpMaster on item.ProcessBy equals users.EmpId
                          select new ProductIncVM()
                          {
                              CityId = item.CityId,
                              ProcessBy = item.ProcessBy,
                              City = city.City,
                              FromDate = item.FromDate,
                              ProcessDate = item.ProcessDate,
                              ToDate = item.ToDate,
                              TransactionDate = item.TransactionDate,
                              ProcessId = item.ProcessId,
                              User = users.EmpName
                          }).ToListAsync();
        }

        public async Task<bool> AddIMEI(List<IMEIUploaderVM> mod, int UserId)
        {
            try
            {
                foreach (var item in mod)
                {
                    var isexist = await db.Pay_ProductIncIMEI.Where(x => x.IMEI == item.IMEI).FirstOrDefaultAsync();
                    if (isexist == null)
                    {
                        Pay_ProductIncIMEI md = new Pay_ProductIncIMEI()
                        {
                            IMEI = item.IMEI,
                            isActive = true,
                            LoadBy = UserId,
                            LoadDate = DateTime.Now
                        };
                        db.Pay_ProductIncIMEI.Add(md);
                        await db.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public Pay_ProductInc GetProductProcessId(int CityId, DateTime StartDate, DateTime EndDate)
        {
            return db.Pay_ProductInc.Where(x => x.CityId == CityId && x.FromDate >= StartDate && x.ToDate <= EndDate).FirstOrDefault();
        }
        public ProductIncTime GetProductIncTime(int PeriodId)
        {
            var calendar = db.Pay_ProductIncCalendar.Where(x => x.RowId == PeriodId).FirstOrDefault();
            ProductIncTime prodinc = new ProductIncTime();
            prodinc.StartDate = calendar.FromDate;
            prodinc.EndDate = calendar.ToDate;
            return prodinc;
        }
        public List<ProductIncDetailVM> GetProductIncentiveDetail(int ProcessId)
        {
            return db.spPay_GetProductIncentiveDetail(ProcessId).Select(x => new ProductIncDetailVM()
            {
                BillDate = x.BillDate,
                BillNo = x.BillNo,
                ComName = x.ComName,
                DesgName = x.DesgName,
                EmpName = x.EmpName,
                FromDate = x.FromDate,
                Incentive = x.Incentive,
                LocCode = x.LocCode,
                LocId = x.LocId,
                Model = x.Model,
                PolicyTitle = x.PolicyTitle,
                Salesman = x.Salesman,
                SerialNo = x.SerialNo,
                SKUCode = x.SKUCode,
                SPrice = x.SPrice,
                ToDate = x.ToDate,
                SType = x.SType
            }).ToList();
        }


        #endregion
        #region GeoLocation
        public async Task<bool> CreateGeoLocation(GeoLocationVM mod, int UserId)
        {
            try
            {
                //var IsGeoExist = db.Comp_GeoLocation.Where(x => x.GeoId == mod.GeoId).FirstOrDefault();
                //if (IsGeoExist == null)
                //{
                Comp_GeoLocation obj = new Comp_GeoLocation
                {
                    //GeoId = mod.GeoId,
                    GTitle = mod.GTitle,
                    ParentId = mod.ParentId,
                    GLevel = mod.GLevel,
                    Status = 1
                };
                db.Comp_GeoLocation.Add(obj);
                await db.SaveChangesAsync();

                var mapping = new Comp_LocationsMapping()
                {
                    EmpId = mod.EmpId,
                    GeoId = obj.GeoId,
                    MappedBy = UserId,
                    MappedDate = DateTime.Now,
                    MappingStatus = true
                };
                db.Comp_LocationsMapping.Add(mapping);
                db.SaveChanges();

                return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateGeoLocation(GeoLocationVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Comp_GeoLocation.FindAsync(mod.GeoId);
                tbl.GTitle = mod.GTitle;
                tbl.ParentId = mod.ParentId;
                tbl.GLevel = mod.GLevel;
                await db.SaveChangesAsync();
                var ExistMapping = db.Comp_LocationsMapping.Where(x => x.GeoId == mod.GeoId && x.MappingStatus).FirstOrDefault();
                if (ExistMapping != null)
                {
                    if (ExistMapping.EmpId != mod.EmpId)
                    {
                        ExistMapping.MappingStatus = false;
                        await db.SaveChangesAsync();
                        var mapping = new Comp_LocationsMapping()
                        {
                            EmpId = mod.EmpId,
                            GeoId = mod.GeoId,
                            MappedBy = UserId,
                            MappedDate = DateTime.Now,
                            MappingStatus = true
                        };
                        db.Comp_LocationsMapping.Add(mapping);
                        await db.SaveChangesAsync();
                    }

                }
                else
                {
                    var mapping = new Comp_LocationsMapping()
                    {
                        EmpId = mod.EmpId,
                        GeoId = mod.GeoId,
                        MappedBy = UserId,
                        MappedDate = DateTime.Now,
                        MappingStatus = true
                    };
                    db.Comp_LocationsMapping.Add(mapping);
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<GeoLocationVM>> GeoLocationRead(int GId)
        {
            try
            {
                var lsts = await (from gp in db.Comp_GeoLocation
                                  join gl in db.Comp_GeoLocation on gp.ParentId equals gl.GeoId
                                  where (gp.ParentId == GId || GId == 0)
                                  select new GeoLocationVM
                                  {
                                      GeoId = gp.GeoId,
                                      GTitle = gp.GTitle,
                                      ParentId = gp.ParentId,
                                      GLvl = "",
                                      GLevel = gp.GLevel,
                                      ParentTitle = gl.GTitle,
                                      EmployeeName = "",

                                  }).ToListAsync();

                var lst = lsts.Select(x => new GeoLocationVM()
                {
                    GeoId = x.GeoId,
                    GTitle = x.GTitle,
                    ParentId = x.ParentId,
                    GLvl = x.GLevel == null ? "" : SelectListVM.RZL.Where(p => p.Value == x.GLevel).First().Text,
                    GLevel = x.GLevel,
                    ParentTitle = x.GTitle,
                    EmployeeName = ""
                }).ToList();
                foreach (var v in lst)
                {
                    v.EmpId = (await db.Comp_LocationsMapping.Where(x => x.GeoId == v.GeoId && x.MappingStatus).Select(x => (int?)x.EmpId).FirstOrDefaultAsync()) ?? 0;
                    if (v.EmpId != 0)
                    {
                        //var emp = (await db.Pay_EmpMaster.Where(x => x.EmpId == v.EmpId).FirstOrDefaultAsync());
                        //v.EmployeeName = emp.EmpName;
                        //v.DesgId = emp.DesgId;
                        //v.DesgName = emp.Pay_Designation.DesgName;

                        var emp = (await db.Pay_EmpMaster.Where(x => x.EmpId == v.EmpId).FirstOrDefaultAsync());
                        v.EmployeeName = emp == null ? "" : emp.EmpName;
                        v.DesgId = emp.DesgId;
                        v.DesgName = emp == null ? "" : emp.Pay_Designation.DesgName;

                    }
                }
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
        #region Designation Setup

        /// <summary>
        /// List the Designations
        /// </summary>
        /// <returns></returns>
        public async Task<List<DesignationSectionVM>> SectionList()
        {
            try
            {
                return await db.Pay_DesignationSection.Select(x =>
               new DesignationSectionVM
               {
                   SectionId = x.SectionId,
                   SectionTitle = x.SectionTitle
               }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// List the Designations
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task<List<DesignationVM>> DesignationList()
        {
            try
            {
                return await db.Pay_Designation.Select(x =>
               new DesignationVM
               {
                   DesgId = x.DesgId,
                   DesgName = x.DesgName,
                   ReportingTo = x.ReportingTo,
                   SectionId = x.SectionId == 0 ? 0 : x.SectionId,
                   Status = x.Status
               }).Where(x => x.Status).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<DesignationVM> BSDesignationList()
        {
            try
            {
                return db.spget_BSDesgList().Select(x => new DesignationVM() { DesgId = x.DesgId, DesgName = x.DesgName }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<DesignationVM>> DesgFieldList()
        {
            try
            {
                return db.spget_FieldDesgList().Select(x =>
               new DesignationVM
               {
                   DesgId = x.DesgId,
                   DesgName = x.DesgName
                   //ReportingTo = x.ReportingTo,
                   //SectionId = x.SectionId == 0 ? 0 : x.SectionId,
                   //Status = x.Status
               }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// List the Designations
        /// </summary>
        /// <returns></returns>
        public async Task<DesignationVM> DesignationById(int DesgId)
        {
            try
            {
                return await db.Pay_Designation.Select(x =>
               new DesignationVM
               {
                   DesgId = x.DesgId,
                   DesgName = x.DesgName,
                   ReportingTo = x.ReportingTo,
                   SectionId = x.SectionId,
                   Status = x.Status
               }).Where(x => x.Status && x.DesgId == DesgId).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void CreateDesignationLog(int desgid, int OldDesgId, int EmpId)
        {

        }



        /// <summary>
        /// If Designation Id is Passed Greated Than 0 will be update all otherwise 0 Will a a new Designation
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<int> CreateUpdateDesignation(DesignationVM mod, int UserId)
        {
            try
            {

                if (mod.DesgId > 0)
                {
                    var desg = await db.Pay_Designation.FindAsync(mod.DesgId);
                    desg.DesgName = mod.DesgName;
                    desg.Status = mod.Status;
                    desg.SectionId = mod.SectionId == 0 ? 0 : Convert.ToInt32(mod.SectionId);
                    desg.ReportingTo = mod.ReportingTo == null ? 0 : Convert.ToInt32(mod.ReportingTo);
                    await db.SaveChangesAsync();
                }
                else
                {
                    var IsExist = db.Pay_Designation.Where(x => x.DesgName == mod.DesgName).FirstOrDefault();
                    if (IsExist == null)
                    {
                        Pay_Designation tbl = new Pay_Designation
                        {
                            DesgName = mod.DesgName,
                            Status = true,
                            SectionId = mod.SectionId == 0 ? 0 : Convert.ToInt32(mod.SectionId),
                            ReportingTo = mod.ReportingTo == null ? 0 : Convert.ToInt32(mod.ReportingTo)
                        };
                        db.Pay_Designation.Add(tbl);
                        await db.SaveChangesAsync();
                        mod.DesgId = tbl.DesgId;
                    }
                    else
                    {
                        IsExist.Status = true;
                        await db.SaveChangesAsync();
                        mod.DesgId = IsExist.DesgId;
                    }
                }
                return mod.DesgId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Soft Delete - Update the Status to false in Designation
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<bool> DestroyDesignation(DesignationVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pay_Designation.FindAsync(mod.DesgId);
                tbl.Status = false;
                tbl.DesgName = tbl.DesgName;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Designation Setup
        #region HDepartment

        public async Task<List<HDepartmentVM>> HDepartmentList()
        {
            try
            {
                return await db.Pay_HDepartment.Select(x =>
               new HDepartmentVM
               {
                   HDeptName = x.HDeptName,
                   HDeptId = x.HDeptId
               }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion HDepartment
        #region Departments

        /// <summary>
        /// List the Departments
        /// </summary>
        /// <returns></returns>
        public async Task<List<DepartmentVM>> DepartmentList()
        {
            try
            {
                return await db.Pay_Department.Select(x =>
               new DepartmentVM
               {
                   DeptId = x.DeptId,
                   DeptName = x.DeptName,
                   HDepId = x.HDeptId,
                   LocId = x.LocId == 0 ? 0 : x.LocId,
                   status = x.Status
               }).Where(x => x.status).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// If Department Id is Passed Greated Than 0 will be update all otherwise 0 Will a a new Department
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<int> CreateUpdateDepartment(DepartmentVM mod)
        {
            try
            {

                if (mod.DeptId > 0)
                {
                    var desg = await db.Pay_Department.FindAsync(mod.DeptId);

                    desg.DeptName = mod.DeptName;
                    desg.LocId = mod.LocId == 0 ? 0 : Convert.ToInt32(mod.LocId);
                    desg.HDeptId = mod.HDepId;
                    desg.Status = mod.status;
                    await db.SaveChangesAsync();
                }
                else
                {
                    var IsExist = db.Pay_Department.Where(x => x.DeptName == mod.DeptName).FirstOrDefault();
                    int depId = db.Pay_Department.Max(x => x.DeptId + 1);

                    if (IsExist == null)
                    {
                        Pay_Department tbl = new Pay_Department
                        {
                            DeptName = mod.DeptName,
                            HDeptId = mod.HDepId,
                            LocId = mod.LocId == 0 ? 0 : Convert.ToInt32(mod.LocId),
                            Status = true,
                            DeptId = depId
                        };
                        db.Pay_Department.Add(tbl);

                        await db.SaveChangesAsync();
                        mod.DeptId = tbl.DeptId;
                    }
                }
                return mod.DeptId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Soft Delete - Update the Status to false in Department
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<bool> DestroyDepartment(DepartmentVM mod)
        {
            try
            {
                var tbl = await db.Pay_Department.FindAsync(mod.DeptId);
                tbl.Status = false;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Departments
        #region Location

        public async Task<List<LocationVM>> Locations()
        {
            try
            {

                return await db.Comp_Locations.Select(x =>
               new LocationVM
               {
                   CityId = x.CityId,
                   LocCode = x.LocCode,
                   LocId = x.LocId,
                   LocName = x.LocName
               }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion Location
        #region Shifts

        /// <summary>
        /// List the Shift
        /// </summary>
        /// <returns></returns>
        public List<ShiftVM> ShiftList()
        {
            try
            {
                //   return db.Pay_Shift.ToList().Select(x =>
                //new ShiftVM
                //{
                //    ShiftTitle = x.ShiftTitle,
                //    StartTime = DateTime.ParseExact(x.StartTime.ToString(), "HH:mm:ss", null).ToString("hh:mm tt"),
                //    EndTime = DateTime.ParseExact(x.EndTime.ToString(), "HH:mm:ss", null).ToString("hh:mm tt"),
                //    ShiftId = x.ShiftId,
                //    status = x.Status
                //}).Where(x => x.status).ToList();
                return db.Pay_Shift.ToList().Select(x =>
              new ShiftVM
              {
                  ShiftTitle = x.ShiftTitle,
                  StartTime = Convert.ToDateTime(x.StartTime.ToString()),
                  EndTime = Convert.ToDateTime(x.EndTime.ToString()),
                  ShiftId = x.ShiftId,
                  status = x.Status,
                  GraceTime = x.GraceTime
              }).Where(x => x.status).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// If Shift Id is Passed Greated Than 0 will be update all otherwise 0 Will a a new Shift
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<int> CreateUpdateShift(ShiftVM mod)
        {
            try
            {

                if (mod.ShiftId > 0)
                {
                    var desg = await db.Pay_Shift.FindAsync(mod.ShiftId);
                    desg.ShiftTitle = mod.ShiftTitle;
                    desg.StartTime = Convert.ToDateTime(mod.StartTime).TimeOfDay;
                    desg.EndTime = Convert.ToDateTime(mod.EndTime).TimeOfDay;
                    desg.Status = mod.status;
                    desg.GraceTime = mod.GraceTime;
                    await db.SaveChangesAsync();
                }
                else
                {
                    var IsExist = db.Pay_Shift.Where(x => x.ShiftTitle == mod.ShiftTitle).FirstOrDefault();
                    if (IsExist == null)
                    {
                        Pay_Shift tbl = new Pay_Shift
                        {
                            ShiftTitle = mod.ShiftTitle,
                            StartTime = Convert.ToDateTime(mod.StartTime).TimeOfDay,
                            EndTime = Convert.ToDateTime(mod.EndTime).TimeOfDay,
                            GraceTime = mod.GraceTime,
                            Status = true
                        };
                        db.Pay_Shift.Add(tbl);

                        await db.SaveChangesAsync();
                        mod.ShiftId = tbl.ShiftId;
                    }
                }
                return mod.ShiftId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Soft Delete - Update the Status to false in Shift
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<bool> DestroyShift(ShiftVM mod)
        {
            try
            {
                var tbl = await db.Pay_Shift.FindAsync(mod.ShiftId);
                tbl.Status = false;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Shifts
        #region AppointmentType



        public List<AppointmentTypeVM> AppointmentTypeList()
        {
            try
            {
                return db.Pay_EmpAppointmentTypes.ToList().Select(x =>
              new AppointmentTypeVM
              {
                  AppointmentType = x.AppointmentType,
                  AppointmentTypeId = x.AppointmentTypeId,
                  Status = x.Status
              }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// List the AppointmentType
        /// </summary>
        /// <returns></returns>
        public List<AppointmentTypeVM> AppointmentType(int status)
        {
            try
            {
                return db.Pay_EmpAppointmentTypes.ToList().Select(x =>
              new AppointmentTypeVM
              {
                  AppointmentType = x.AppointmentType,
                  AppointmentTypeId = x.AppointmentTypeId,
                  Status = x.Status
              }).Where(x => x.Status == (status == 1) ? true : false).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// If Shift Id is Passed Greated Than 0 will be update all otherwise 0 Will a a new Shift
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<bool> CreateUpdateAppointmentType(AppointmentTypeVM mod)
        {
            try
            {

                if (mod.AppointmentTypeId > 0)
                {
                    var desg = await db.Pay_EmpAppointmentTypes.FindAsync(mod.AppointmentTypeId);
                    desg.AppointmentType = mod.AppointmentType;
                    desg.AppointmentTypeId = mod.AppointmentTypeId;
                    await db.SaveChangesAsync();
                }
                else
                {
                    var IsExist = db.Pay_EmpAppointmentTypes.Where(x => x.AppointmentTypeId == mod.AppointmentTypeId).FirstOrDefault();
                    if (IsExist == null)
                    {
                        Pay_EmpAppointmentTypes tbl = new Pay_EmpAppointmentTypes
                        {
                            AppointmentType = mod.AppointmentType,
                            AppointmentTypeId = mod.AppointmentTypeId,
                            Status = true
                        };
                        db.Pay_EmpAppointmentTypes.Add(tbl);

                        await db.SaveChangesAsync();
                        mod.AppointmentTypeId = tbl.AppointmentTypeId;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Soft Delete - Update the Status to false in Shift
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<bool> DestroyAppointmentType(AppointmentTypeVM mod)
        {
            try
            {
                var tbl = await db.Pay_EmpAppointmentTypes.Where(x => x.AppointmentTypeId == mod.AppointmentTypeId).FirstOrDefaultAsync();
                tbl.Status = false;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Shifts
        #region Employee Profile,Education,Experience,Qualification,Salary,Cheque
        public async Task<List<LocationVM>> LocationList()
        {
            return await db.Comp_GeoLocation.Where(x => x.GLevel == "3" && x.Status == 1)
                    .Select(x => new LocationVM
                    {
                        LocId = x.GeoId,
                        LocName = x.GTitle
                    }).ToListAsync();
        }
        public async Task<List<EmpRoleVM>> GetRoles()
        {
            return await db.Pay_Role.Where(x => x.Status).Select(x => new EmpRoleVM()
            {
                EROle = x.RoleId,
                RoleName = x.Role
            }).ToListAsync();
        }


        public async Task<int> GetNearToExpireCNICEmps(int HDeptId, int DeptId, int DesgIdSel, string StatusId)
        {
            var emps = await (from x in db.Pay_EmpMaster
                              join d in db.Pay_Department on x.DeptId equals d.DeptId
                              join dd in db.Pay_Designation on x.DesgId equals dd.DesgId
                              join s in db.Pay_EmpReportingHierarchy on x.EmpId equals s.EmpId into ds
                              from s in ds.DefaultIfEmpty()
                              where
                               (d.HDeptId == HDeptId || HDeptId == 0)
                              && (d.DeptId == DeptId || DeptId == 0)
                              && DbFunctions.TruncateTime(x.CNICExpireDate.Value) < DbFunctions.TruncateTime(DateTime.Now)
                              && (dd.DesgId == DesgIdSel || DesgIdSel == 0)
                              select x).ToListAsync();
            return emps.Count();
        }

        public async Task<int> GetTotalActiveEmployees(int HDeptId, int DeptId, int DesgIdSel, string StatusId)
        {
            var emps = await (from x in db.Pay_EmpMaster
                              join d in db.Pay_Department on x.DeptId equals d.DeptId
                              join dd in db.Pay_Designation on x.DesgId equals dd.DesgId
                              join s in db.Pay_EmpReportingHierarchy on x.EmpId equals s.EmpId into ds
                              from s in ds.DefaultIfEmpty()
                              where x.StatusId == "A"
                              && (d.HDeptId == HDeptId || HDeptId == 0)
                              && (d.DeptId == DeptId || DeptId == 0)
                              && (dd.DesgId == DesgIdSel || DesgIdSel == 0)
                              select x).ToListAsync();
            return emps.Count();
        }

        public async Task<int> GetPendingEmployees(int HDeptId, int DeptId, int DesgIdSel, string StatusId)
        {
            var emps = await (from x in db.Pay_EmpMaster
                              join d in db.Pay_Department on x.DeptId equals d.DeptId
                              join dd in db.Pay_Designation on x.DesgId equals dd.DesgId
                              join s in db.Pay_EmpReportingHierarchy on x.EmpId equals s.EmpId into ds
                              from s in ds.DefaultIfEmpty()
                              where
                                (d.HDeptId == HDeptId || HDeptId == 0)
                             && (d.DeptId == DeptId || DeptId == 0)
                             && (dd.DesgId == DesgIdSel || DesgIdSel == 0)
                             && x.ApprovedBy == null
                             && x.StatusId == "A"
                              select x).ToListAsync();
            return emps.Count();
        }


        public async Task<List<EmployeeVM>> EmployeeList(int HDeptId, int DeptId, int DesgIdSel, string StatusId, string PenEmp)
        {
            try
            {
                var lst = await (from x in db.Pay_EmpMaster
                                 join d in db.Pay_Department on x.DeptId equals d.DeptId
                                 join dd in db.Pay_Designation on x.DesgId equals dd.DesgId
                                 join s in db.Pay_EmpReportingHierarchy on x.EmpId equals s.EmpId into ds
                                 from s in ds.DefaultIfEmpty()
                                 where (x.StatusId == StatusId || StatusId == "0")
                                 && (d.HDeptId == HDeptId || HDeptId == 0)
                                 && (d.DeptId == DeptId || DeptId == 0)
                                 && (dd.DesgId == DesgIdSel || DesgIdSel == 0)
                                 && (PenEmp == "P" ? x.ApprovedBy == null : x.ApprovedBy != null || PenEmp == "0")
                                 select new EmployeeVM
                                 {
                                     Address = x.Address,
                                     BloodGrp = x.BloodGroup,
                                     CNIC = x.CNIC,
                                     DeptId = x.DeptId,
                                     DesgId = x.DesgId,
                                     DOJ = x.DOJ,
                                     DOB = x.DOB,
                                     Email = x.Email,
                                     EmpId = x.EmpId,
                                     EmpName = x.EmpName,
                                     Gender = x.Gender,
                                     MaritalStatus = x.MaritalStatus,
                                     Mobile1 = x.Mobile1,
                                     Mobile2 = x.Mobile2,
                                     Religion = x.Religion,
                                     TransDate = x.TransDate,
                                     UserId = x.UserId,
                                     FName = x.FName,
                                     RefAddress = x.RefAddress,
                                     RefCNIC = x.RefCNIC,
                                     RefContactNo = x.RefContactNo,
                                     RefPerson = x.RefPerson,
                                     Remarks = x.Remarks,
                                     ShiftId = x.ShiftId,
                                     Status = x.StatusId,
                                     RptTo = (s == null ? 0 : s.ReportingTo),
                                     HDeptId = d.HDeptId,
                                     PayScaleId = x.PayScaleId,
                                     WeeklyHoliday = x.WeeklyHoliday,
                                     JobType = x.JobType,
                                     AppointmentTypeId = x.AppointmentTypeId,
                                     Weekend = x.Weekend,
                                     JoinDate = x.DOJ,
                                     DeptName = d.DeptName,
                                     DesgName = dd.DesgName,
                                     ApprovedBy = x.ApprovedBy,
                                     ApprovedDate = x.ApprovedDate,
                                     //EmpRolesIds = x.Pay_EmpRole.Where(y => y.EmpId == x.EmpId).Select(x => x.RoleId).ToList()
                                     //Color = String.Format("#{0:X6}", new Random().Next(0x1000000))
                                 }).ToListAsync();
                var random = new Random();
                lst.ForEach(x => x.Color = String.Format("#{0:X6}", random.Next(0x1000000)));
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<decimal> GetEmployeeBasicSalary(int id)
        {
            try
            {
                EmployeeCalculatedSalaryVM _SalaryObj = new EmployeeCalculatedSalaryVM();
                _SalaryObj.EmployeeId = id;
                _SalaryObj.id = 0;
                var empsal = await db.Pay_EmpSalary.Where(x => x.EmpId == id).ToListAsync();
                var basicsal = empsal.Where(x => x.EmpId == id && x.SalTypeId == "B").FirstOrDefault();
                var existsalinc = empsal.Where(x => x.EmpId == id && x.SalTypeId == "I").ToList();
                var existsaldec = empsal.Where(x => x.EmpId == id && x.SalTypeId == "D").ToList();
                if (basicsal != null)
                {
                    decimal exstincsal = existsalinc.Count > 0 ? existsalinc.Sum(x => x.Salary) : 0;
                    decimal exstdecsal = existsaldec.Count > 0 ? existsaldec.Sum(x => x.Salary) : 0;
                    var BS = (basicsal.Salary + exstincsal) - exstdecsal;
                    return BS;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<string> GetEmpNameById(int id)
        {
            try
            {
                return await db.Pay_EmpMaster.Where(x => x.EmpId == id).Select(x => x.EmpName).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return "";
            }
        }
        public async Task<EmployeeVM> GetEmployeeById(int id)
        {
            try
            {
                if (id > 0)
                {
                    var emp = await (from x in db.Pay_EmpMaster
                                         //join d in db.Pay_Department on x.DeptId equals d.DeptId
                                         //join dss in db.Pay_Designation on x.DesgId equals dss.DesgId
                                         //join s in db.Pay_EmpReportingHierarchy on x.EmpId equals s.EmpId into ds
                                         //from s in ds.DefaultIfEmpty()
                                     where x.EmpId == id
                                     select new EmployeeVM
                                     {
                                         Address = x.Address,
                                         BloodGrp = x.BloodGroup,
                                         CNIC = x.CNIC,
                                         DeptId = x.DeptId,
                                         DesgId = x.DesgId,
                                         DOJ = x.DOJ,
                                         DOB = x.DOB,
                                         Email = x.Email,
                                         EmpId = x.EmpId,
                                         EmpName = x.EmpName,
                                         Gender = x.Gender,
                                         MaritalStatus = x.MaritalStatus,
                                         Mobile1 = x.Mobile1,
                                         Mobile2 = x.Mobile2,
                                         Religion = x.Religion,
                                         TransDate = x.TransDate,
                                         UserId = x.UserId,
                                         FName = x.FName,
                                         RefAddress = x.RefAddress,
                                         RefCNIC = x.RefCNIC,
                                         RefContactNo = x.RefContactNo,
                                         RefPerson = x.RefPerson,
                                         Remarks = x.Remarks,
                                         ShiftId = x.ShiftId,
                                         Status = x.StatusId,
                                         RptTo = db.Pay_EmpReportingHierarchy.Where(a => a.EmpId == x.EmpId).Select(a => (int?)a.ReportingTo).FirstOrDefault() ?? 0,
                                         HDeptId = x.Pay_Department.HDeptId,
                                         PayScaleId = x.PayScaleId,
                                         WeeklyHoliday = x.WeeklyHoliday,
                                         JobType = x.JobType,
                                         JoinDate = x.DOJ,
                                         CNICExpireDate = x.CNICExpireDate,
                                         Weekend = x.Weekend,
                                         AppointmentTypeId = x.AppointmentTypeId,
                                         DeptName = x.Pay_Department.DeptName,
                                         DesgName = x.Pay_Designation.DesgName,
                                         //EmpRolesIds = db.Pay_DesgRole.Where(a => a.DesgId == x.DesgId).Select(a => a.RoleId).ToList(),
                                         BlockSalary = x.BlockSalary ?? false,
                                         IsFinalized = x.IsFinalized ?? false,
                                         IsFaceAllow = x.IsFaceAllow ?? false,
                                         ApprovedBy = x.ApprovedBy,
                                         ApprovedDate = x.ApprovedDate,
                                         MSalary = x.MSalary ?? false,
                                         AStatus = x.AttendanceStatus == true ? "A" : "B"
                                         //Color = String.Format("#{0:X6}", new Random().Next(0x1000000))
                                     }).FirstOrDefaultAsync();

                    emp.BasicSalary = await GetEmployeeBasicSalary(id);
                    return emp;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<EmployeeVM> GetEmployeeByCNIC(string CNIC)
        {
            try
            {
                if (CNIC != null)
                {
                    var emp = await (from x in db.Pay_EmpMaster
                                         //join d in db.Pay_Department on x.DeptId equals d.DeptId
                                         //join dss in db.Pay_Designation on x.DesgId equals dss.DesgId
                                         //join s in db.Pay_EmpReportingHierarchy on x.EmpId equals s.EmpId into ds
                                         //from s in ds.DefaultIfEmpty()
                                     where x.CNIC == CNIC
                                     select new EmployeeVM
                                     {
                                         Address = x.Address,
                                         BloodGrp = x.BloodGroup,
                                         CNIC = x.CNIC,
                                         DeptId = x.DeptId,
                                         DesgId = x.DesgId,
                                         DOJ = x.DOJ,
                                         DOB = x.DOB,
                                         Email = x.Email,
                                         EmpId = x.EmpId,
                                         EmpName = x.EmpName,
                                         Gender = x.Gender,
                                         MaritalStatus = x.MaritalStatus,
                                         Mobile1 = x.Mobile1,
                                         Mobile2 = x.Mobile2,
                                         Religion = x.Religion,
                                         TransDate = x.TransDate,
                                         UserId = x.UserId,
                                         FName = x.FName,
                                         RefAddress = x.RefAddress,
                                         RefCNIC = x.RefCNIC,
                                         RefContactNo = x.RefContactNo,
                                         RefPerson = x.RefPerson,
                                         Remarks = x.Remarks,
                                         ShiftId = x.ShiftId,
                                         Status = x.StatusId,
                                         RptTo = db.Pay_EmpReportingHierarchy.Where(a => a.EmpId == x.EmpId).Select(a => (int?)a.ReportingTo).FirstOrDefault() ?? 0,
                                         HDeptId = x.Pay_Department.HDeptId,
                                         PayScaleId = x.PayScaleId,
                                         WeeklyHoliday = x.WeeklyHoliday,
                                         JobType = x.JobType,
                                         JoinDate = x.DOJ,
                                         Weekend = x.Weekend,
                                         AppointmentTypeId = x.AppointmentTypeId,
                                         DeptName = x.Pay_Department.DeptName,
                                         DesgName = x.Pay_Designation.DesgName,
                                         //EmpRolesIds = x.Pay_EmpRole.Where(a => a.Status == true).Select(a => a.RoleId).ToList(),
                                         BlockSalary = x.BlockSalary ?? false,
                                         IsFinalized = x.IsFinalized ?? false,
                                         IsFaceAllow = x.IsFaceAllow ?? false,
                                         MSalary = x.MSalary ?? false,
                                         AStatus = x.AttendanceStatus == true ? "A" : "B"
                                         //Color = String.Format("#{0:X6}", new Random().Next(0x1000000))
                                     }).FirstOrDefaultAsync();

                    //  emp.BasicSalary = GetEmployeeBasicSalary(id);
                    return emp;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<SEmployeeVM>> GetEmployeeListMin()
        {
            return await db.Pay_EmpMaster.Where(x => x.StatusId == "A")
                    .Select(x => new SEmployeeVM
                    {
                        EmpId = x.EmpId,
                        EmpName = x.EmpName,
                        CNIC = x.CNIC
                    }).ToListAsync();
        }

        public async Task<List<EmployeeVM>> EmployeeListRptTo(int userid)
        {
            try
            {
                return await (from x in db.Pay_EmpMaster
                              join d in db.Pay_Department on x.DeptId equals d.DeptId
                              join s in db.Pay_EmpReportingHierarchy on x.EmpId equals s.EmpId into ds
                              from s in ds.DefaultIfEmpty()
                              where x.StatusId == "A" && (s.ReportingTo == userid || x.EmpId == userid)
                              select new EmployeeVM
                              {
                                  Address = x.Address,
                                  BloodGrp = x.BloodGroup,
                                  CNIC = x.CNIC,
                                  DeptId = x.DeptId,
                                  DesgId = x.DesgId,
                                  DOJ = x.DOJ,
                                  Email = x.Email,
                                  EmpId = x.EmpId,
                                  EmpName = x.EmpName,
                                  Gender = x.Gender,
                                  MaritalStatus = x.MaritalStatus,
                                  Mobile1 = x.Mobile1,
                                  Mobile2 = x.Mobile2,
                                  Religion = x.Religion,
                                  TransDate = x.TransDate,
                                  UserId = x.UserId,
                                  FName = x.FName,
                                  RefAddress = x.RefAddress,
                                  RefCNIC = x.RefCNIC,
                                  RefContactNo = x.RefContactNo,
                                  RefPerson = x.RefPerson,
                                  Remarks = x.Remarks,
                                  ShiftId = x.ShiftId,
                                  Status = x.StatusId,
                                  RptTo = s.ReportingTo,
                                  PayScaleId = x.PayScaleId
                              }).Distinct().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<EmployeeVM>> EmployeeListRptToLeave(int userid, int groupid, int locid)
        {
            if (groupid == 1037)
            {
                return await (from x in db.Pay_EmpMaster
                              join d in db.Pay_Department on x.DeptId equals d.DeptId
                              where x.StatusId == "A"
                              select new EmployeeVM
                              {
                                  Address = x.Address,
                                  BloodGrp = x.BloodGroup,
                                  CNIC = x.CNIC,
                                  DeptId = x.DeptId,
                                  DesgId = x.DesgId,
                                  DOJ = x.DOJ,
                                  Email = x.Email,
                                  EmpId = x.EmpId,
                                  EmpName = x.EmpName,
                                  Gender = x.Gender,
                                  MaritalStatus = x.MaritalStatus,
                                  Mobile1 = x.Mobile1,
                                  Mobile2 = x.Mobile2,
                                  Religion = x.Religion,
                                  TransDate = x.TransDate,
                                  UserId = x.UserId,
                                  FName = x.FName,
                                  RefAddress = x.RefAddress,
                                  RefCNIC = x.RefCNIC,
                                  DeptName = x.Pay_Department.DeptName,
                                  DesgName = x.Pay_Designation.DesgName,
                                  RefContactNo = x.RefContactNo,
                                  RefPerson = x.RefPerson,
                                  Remarks = x.Remarks,
                                  ShiftId = x.ShiftId,
                                  Status = x.StatusId,
                                  PayScaleId = x.PayScaleId
                              }).Distinct().ToListAsync();
            }
            else if (groupid == 200)
            {
                return await (from x in db.Pay_EmpMaster
                              join d in db.Pay_Department on x.DeptId equals d.DeptId
                              where x.StatusId == "A" && x.LocId == locid
                              select new EmployeeVM
                              {
                                  Address = x.Address,
                                  BloodGrp = x.BloodGroup,
                                  CNIC = x.CNIC,
                                  DeptId = x.DeptId,
                                  DesgId = x.DesgId,
                                  DOJ = x.DOJ,
                                  Email = x.Email,
                                  EmpId = x.EmpId,
                                  EmpName = x.EmpName,
                                  Gender = x.Gender,
                                  MaritalStatus = x.MaritalStatus,
                                  Mobile1 = x.Mobile1,
                                  Mobile2 = x.Mobile2,
                                  Religion = x.Religion,
                                  TransDate = x.TransDate,
                                  UserId = x.UserId,
                                  FName = x.FName,
                                  RefAddress = x.RefAddress,
                                  RefCNIC = x.RefCNIC,
                                  DeptName = x.Pay_Department.DeptName,
                                  DesgName = x.Pay_Designation.DesgName,
                                  RefContactNo = x.RefContactNo,
                                  RefPerson = x.RefPerson,
                                  Remarks = x.Remarks,
                                  ShiftId = x.ShiftId,
                                  Status = x.StatusId,
                                  PayScaleId = x.PayScaleId
                              }).Distinct().ToListAsync();
            }
            else if (groupid == 1035 && userid == 112)
            {
                return await (from x in db.Pay_EmpMaster
                              join d in db.Pay_Department on x.DeptId equals d.DeptId
                              where x.StatusId == "A" && d.HDeptId == 4
                              select new EmployeeVM
                              {
                                  Address = x.Address,
                                  BloodGrp = x.BloodGroup,
                                  CNIC = x.CNIC,
                                  DeptId = x.DeptId,
                                  DesgId = x.DesgId,
                                  DOJ = x.DOJ,
                                  Email = x.Email,
                                  EmpId = x.EmpId,
                                  EmpName = x.EmpName,
                                  Gender = x.Gender,
                                  MaritalStatus = x.MaritalStatus,
                                  Mobile1 = x.Mobile1,
                                  Mobile2 = x.Mobile2,
                                  Religion = x.Religion,
                                  TransDate = x.TransDate,
                                  UserId = x.UserId,
                                  FName = x.FName,
                                  RefAddress = x.RefAddress,
                                  RefCNIC = x.RefCNIC,
                                  DeptName = x.Pay_Department.DeptName,
                                  DesgName = x.Pay_Designation.DesgName,
                                  RefContactNo = x.RefContactNo,
                                  RefPerson = x.RefPerson,
                                  Remarks = x.Remarks,
                                  ShiftId = x.ShiftId,
                                  Status = x.StatusId,
                                  PayScaleId = x.PayScaleId
                              }).Distinct().ToListAsync();
            }
            else if (userid == 432)
            {
                return await (from x in db.Pay_EmpMaster
                              join d in db.Pay_Department on x.DeptId equals d.DeptId
                              where x.StatusId == "A" && d.DeptId == 1038
                              select new EmployeeVM
                              {
                                  Address = x.Address,
                                  BloodGrp = x.BloodGroup,
                                  CNIC = x.CNIC,
                                  DeptId = x.DeptId,
                                  DesgId = x.DesgId,
                                  DOJ = x.DOJ,
                                  Email = x.Email,
                                  EmpId = x.EmpId,
                                  EmpName = x.EmpName,
                                  Gender = x.Gender,
                                  MaritalStatus = x.MaritalStatus,
                                  Mobile1 = x.Mobile1,
                                  Mobile2 = x.Mobile2,
                                  Religion = x.Religion,
                                  TransDate = x.TransDate,
                                  UserId = x.UserId,
                                  FName = x.FName,
                                  RefAddress = x.RefAddress,
                                  RefCNIC = x.RefCNIC,
                                  DeptName = x.Pay_Department.DeptName,
                                  DesgName = x.Pay_Designation.DesgName,
                                  RefContactNo = x.RefContactNo,
                                  RefPerson = x.RefPerson,
                                  Remarks = x.Remarks,
                                  ShiftId = x.ShiftId,
                                  Status = x.StatusId,
                                  PayScaleId = x.PayScaleId
                              }).Distinct().ToListAsync();
            }
            else
            {
                try
                {
                    return await (from x in db.Pay_EmpMaster
                                  join d in db.Pay_Department on x.DeptId equals d.DeptId
                                  join s in db.Pay_EmpReportingHierarchy on x.EmpId equals s.EmpId into ds
                                  from s in ds.DefaultIfEmpty()
                                  where x.StatusId == "A" && (s.ReportingTo == userid || x.EmpId == userid)
                                  select new EmployeeVM
                                  {
                                      Address = x.Address,
                                      BloodGrp = x.BloodGroup,
                                      CNIC = x.CNIC,
                                      DeptId = x.DeptId,
                                      DesgId = x.DesgId,
                                      DOJ = x.DOJ,
                                      Email = x.Email,
                                      EmpId = x.EmpId,
                                      EmpName = x.EmpName,
                                      Gender = x.Gender,
                                      MaritalStatus = x.MaritalStatus,
                                      Mobile1 = x.Mobile1,
                                      Mobile2 = x.Mobile2,
                                      Religion = x.Religion,
                                      TransDate = x.TransDate,
                                      UserId = x.UserId,
                                      FName = x.FName,
                                      RefAddress = x.RefAddress,
                                      RefCNIC = x.RefCNIC,
                                      RefContactNo = x.RefContactNo,
                                      RefPerson = x.RefPerson,
                                      Remarks = x.Remarks,
                                      ShiftId = x.ShiftId,
                                      DeptName = x.Pay_Department.DeptName,
                                      DesgName = x.Pay_Designation.DesgName,
                                      Status = x.StatusId,
                                      RptTo = s.ReportingTo,
                                      PayScaleId = x.PayScaleId
                                  }).Distinct().ToListAsync();
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }
        public async Task<List<GeoLocationVM>> GetGeoLocation(int GeoId)
        {
            try
            {
                //{
                //    var lst = await (from gp in db.Comp_GeoLocation
                //                     join gl in db.Comp_GeoLocation on gp.GeoId equals gl.ParentId
                //                     where gl.GeoId == GeoId
                //                     select new GeoLocationVM
                //                     {
                //                         GeoId = gl.GeoId,
                //                         GTitle = gl.GTitle,
                //                         ParentId = gp.ParentId,
                //                         GLevel = gl.GLevel == "1" ? "Region" : gl.GLevel == "2" ? "Zone" : "Location",
                //                         ParentTitle = gp.GTitle
                //                     }).ToListAsync();
                //var lsts = await (from gp in db.Comp_GeoLocation
                //                  join gl in db.Comp_GeoLocation on gp.GeoId equals gl.ParentId
                //                  where gl.GeoId == GeoId
                //                  select new GeoLocationVM
                //                  {
                //                      GeoId = gl.GeoId,
                //                      GTitle = gl.GTitle,
                //                      ParentId = gp.ParentId,
                //                      GLvl = "",
                //                      GLevel = gl.GLevel,
                //                      ParentTitle = gp.GTitle,
                //                      EmployeeName = ""
                //                  }).ToListAsync();

                var lsts = await (from gp in db.Comp_GeoLocation
                                      //join gl in db.Comp_GeoLocation on gp.ParentId equals gl.GeoId
                                  where (gp.GeoId == GeoId)
                                  select new GeoLocationVM
                                  {
                                      GeoId = gp.GeoId,
                                      GTitle = gp.GTitle,
                                      ParentId = gp.ParentId,
                                      GLvl = "",
                                      GLevel = gp.GLevel,
                                      EmployeeName = ""
                                  }).ToListAsync();

                var lst = lsts.Select(x => new GeoLocationVM()
                {
                    GeoId = x.GeoId,
                    GTitle = x.GTitle,
                    ParentId = x.ParentId,
                    GLvl = SelectListVM.RZL.Where(p => p.Value == x.GLevel).First().Text,
                    GLevel = x.GLevel,
                    ParentTitle = x.GTitle,
                    EmployeeName = ""
                }).ToList();
                foreach (var v in lst)
                {
                    v.EmpId = (await db.Comp_LocationsMapping.Where(x => x.GeoId == v.GeoId && x.MappingStatus).Select(x => (int?)x.EmpId).FirstOrDefaultAsync()) ?? 0;
                    if (v.EmpId != 0)
                    {
                        //v.EmployeeName = (await db.Pay_EmpMaster.Where(x => x.EmpId == v.EmpId).Select(x => x.EmpName).FirstOrDefaultAsync()) ?? "";
                        var emp = db.Pay_EmpMaster.Where(x => x.EmpId == v.EmpId).FirstOrDefault();
                        v.EmployeeName = emp == null ? "" : emp.EmpName;
                        v.DesgName = emp == null ? "" : emp.Pay_Designation.DesgName;


                    }
                }
                return lst;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetColor(string Glevel)
        {
            if (Glevel == "0")
            {
                return "#004aff";
            }
            else if (Glevel == "1")
            {
                return "#cf3737";
            }
            else if (Glevel == "2")
            {
                return "#ffbe33";
            }
            else if (Glevel == "3")
            {
                return "#25a0da";
            }
            else if (Glevel == "4")
            {
                return "#49a046";
            }
            else
            {
                return "#cf3737";
            }
        }
        //public List<OrganizaOrgano> GetOrganos(int EmpId, int Level)
        //{
        //    try
        //    {
        //        if (Level == 1)
        //        {
        //            var lst = (from f in db.Pay_EmpHierarchy
        //                       //where f.GMId == GeoId
        //                       orderby f.GMId ascending
        //                       select f).ToList().Select(f => new OrganizaOrgano
        //                       {

        //                           id = f.GMId ?? 0,
        //                           Name = f.GM,
        //                           items = GetOrganos(f.GMId ?? 0,Level+1),
        //                           Color = GetColor("1")
        //                       }).ToList();
        //        }
        //        else if (Level == 2)
        //        {
        //            var lst = (from f in db.Pay_EmpHierarchy
        //                           //where f.GMId == GeoId
        //                       orderby f.SSRMId ascending
        //                       select f).ToList().Select(f => new OrganizaOrgano
        //                       {

        //                           id = f.SSRMId ?? 0,
        //                           Name = f.SSRM,
        //                           items = GetOrganos(f.SSRMId ?? 0, Level + 1),
        //                           Color = GetColor("2")
        //                       }).ToList();
        //        }
        //        else if (Level == 3)
        //        {
        //            var lst = (from f in db.Pay_EmpHierarchy
        //                           //where f.GMId == GeoId
        //                       orderby f.SRMId ascending
        //                       select f).ToList().Select(f => new OrganizaOrgano
        //                       {

        //                           id = f.SRMId ?? 0,
        //                           Name = f.SRM,
        //                           items = GetOrganos(f.SRMId ?? 0, Level + 1),
        //                           Color = GetColor("3")
        //                       }).ToList();
        //        }
        //        else if (Level == 4)
        //        {
        //            var lst = (from f in db.Pay_EmpHierarchy
        //                           //where f.GMId == GeoId
        //                       orderby f.BDMId ascending
        //                       select f).ToList().Select(f => new OrganizaOrgano
        //                       {

        //                           id = f.BDMId ?? 0,
        //                           Name = f.BDM,
        //                           items = GetOrganos(f.BDMId ?? 0, Level + 1),
        //                           Color = GetColor("2")
        //                       }).ToList();
        //        }
        //        else if (Level == 5)
        //        {
        //            var lst = (from f in db.Pay_EmpHierarchy
        //                           //where f.GMId == GeoId
        //                       orderby f.SSRMId ascending
        //                       select f).ToList().Select(f => new OrganizaOrgano
        //                       {

        //                           id = f.SSRMId ?? 0,
        //                           Name = f.SSRM,
        //                           items = GetOrganos(f.SSRMId ?? 0, Level + 1),
        //                           Color = GetColor("2")
        //                       }).ToList();
        //        }
        //        //var lst = (from f in db.Comp_GeoLocation
        //        //           where f.ParentId == GeoId
        //        //           orderby f.GeoId ascending
        //        //           select f).ToList().Select(f => new OrganizaOrgano
        //        //           {

        //        //               id = f.GeoId,
        //        //               Name = f.GTitle,
        //        //               items = GetOrganos(f.GeoId),
        //        //               Color = GetColor(f.GLevel)

        //        //           }).ToList();
        //        foreach (var v in lst)
        //        {
        //            int EmpId = db.Comp_LocationsMapping.Where(x => x.GeoId == v.id && x.MappingStatus).Select(x => (int?)x.EmpId).FirstOrDefault() ?? 0;
        //            if (EmpId != 0)
        //            {
        //                var emp = db.Pay_EmpMaster.Where(x => x.EmpId == EmpId).FirstOrDefault();
        //                v.EmployeeName = emp == null ? "" : emp.EmpName;
        //                v.DesgName = emp == null ? "" : emp.Pay_Designation.DesgName;
        //                v.Name = v.Name + "\n" + v.EmployeeName + "\n" + v.DesgName;


        //            }

        //        }
        //        return lst;

        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public List<OrganizaOrgano> GetOrganoH(int GeoId, int Level)
        {
            try
            {

                var lst = (from f in db.Pay_EmpReportingHierarchy
                           join e in db.Pay_EmpMaster on f.EmpId equals e.EmpId
                           where f.ReportingTo == GeoId && e.StatusId == "A" && e.Pay_Department.HDeptId == 1
                           orderby f.EmpId ascending
                           select new { f, e }).ToList().Select(f => new OrganizaOrgano
                           {
                               id = f.f.EmpId,
                               Name = f.e.EmpName,
                               items = GetOrganoH(f.f.EmpId, Level + 1),
                               DesgName = f.e.Pay_Designation.DesgName,
                               EmployeeName = f.e.EmpName,
                               Color = GetColor(Level.ToString())
                           }).ToList();
                //foreach (var v in lst)
                //{
                //    int EmpId = db.Comp_LocationsMapping.Where(x => x.GeoId == v.id && x.MappingStatus).Select(x => (int?)x.EmpId).FirstOrDefault() ?? 0;
                //    if (EmpId != 0)
                //    {
                //        var emp = db.Pay_EmpMaster.Where(x => x.EmpId == EmpId).FirstOrDefault();
                //        v.EmployeeName = emp == null ? "" : emp.EmpName;
                //        v.DesgName = emp == null ? "" : emp.Pay_Designation.DesgName;
                //        v.Name = v.Name + "\n" + v.EmployeeName + "\n" + v.DesgName;


                //    }

                //}
                return lst;

            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<OrganizaOrgano> GetOrgano(int GeoId)
        {
            try
            {

                var lst = (from f in db.Comp_GeoLocation
                           where f.ParentId == GeoId
                           orderby f.GeoId ascending
                           select f).ToList().Select(f => new OrganizaOrgano
                           {

                               id = f.GeoId,
                               Name = f.GTitle,
                               items = GetOrgano(f.GeoId),
                               Color = GetColor(f.GLevel)

                           }).ToList();
                foreach (var v in lst)
                {
                    int EmpId = db.Comp_LocationsMapping.Where(x => x.GeoId == v.id && x.MappingStatus).Select(x => (int?)x.EmpId).FirstOrDefault() ?? 0;
                    if (EmpId != 0)
                    {
                        var emp = db.Pay_EmpMaster.Where(x => x.EmpId == EmpId).FirstOrDefault();
                        v.EmployeeName = emp == null ? "" : emp.EmpName;
                        v.DesgName = emp == null ? "" : emp.Pay_Designation.DesgName;
                        v.Name = v.Name + "\n" + v.EmployeeName + "\n" + v.DesgName;


                    }

                }
                return lst;

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<LocationVM>> GetLocationByZ(string RZID)
        {
            try
            {
                var locations = await (from item in db.Comp_GeoLocation
                                       where item.GLevel == RZID && item.Status == 1
                                       select new LocationVM { LocId = item.GeoId, LocName = item.GTitle }).ToListAsync();
                return locations;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<LocationVM>> GetLocationList(int EmpId)
        {
            try
            {
                //var locations = await db.Comp_Locations.ToListAsync();
                //var EmpLocations = await (from item in db.Comp_Locations
                //                    join emps in db.Pay_EmpLocationMapping on item.LocId equals emps.LocId
                //                    select new LocationVM { LocId = item.LocId, LocName = item.LocName }).ToListAsync();

                var emplocations = await (from item in db.Comp_Locations
                                          join empitem in db.Pay_EmpLocationMapping on item.LocId equals empitem.LocId
                                          where empitem.EmpId == EmpId
                                          select new LocationVM { LocId = item.LocId, LocName = item.LocName }).ToListAsync();
                //.Select(x => new LocationVM { LocId = x.LocId, LocName = x.LocName }).ToList();
                var locations = await (from item in db.Comp_Locations
                                       where !db.Pay_EmpLocationMapping.Any(f => f.LocId == item.LocId && f.EmpId == EmpId && f.Status)
                                       select new LocationVM { LocId = item.LocId, LocName = item.LocName }).ToListAsync();
                //locations.Union(emplocations);
                return locations;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<bool> AddUpdateEmpRole(List<int> roleids, int empid)
        {
            try
            {

                //var emproles = db.Pay_EmpRole.Where(x => x.EmpId == empid && x.Status == true).ToList();
                ////Roles Deleted by the User
                //if (roleids == null)
                //{

                //    var itemm = emproles.Where(p => roleids == null || emproles.Any(p2 => p2.RoleId == p.RoleId && p.EmpId == empid)).ToList();
                //    if (itemm.Count > 0)
                //    {
                //        foreach (var emps in itemm)
                //        {
                //            emps.Status = false;
                //            await db.SaveChangesAsync();
                //        }
                //    }
                //    return true;
                //}
                ////HashSet<Pay_EmpRole> sentIDs = new HashSet<int>(emproles.Select(s => s.RoleId)).ToList();
                //var items = emproles.Where(x => !emproles.Any(y => y.RoleId == x.RoleId && y.EmpId == empid)).ToList();
                ////var items = emproles.Where(p => roleids == null || !emproles.Any(p2 => p2.RoleId == p.RoleId && p.EmpId == empid)).ToList();
                //if (items.Count > 0)
                //{
                //    foreach (var emps in items)
                //    {
                //        emps.Status = false;
                //        await db.SaveChangesAsync();
                //    }
                //}

                //foreach (var item in roleids)
                //{
                //    var itm = Convert.ToInt32(item);
                //    var itemroleexist = db.Pay_EmpRole.Where(x => x.EmpId == empid && x.RoleId == itm).FirstOrDefault();
                //    if (itemroleexist == null)
                //    {
                //        Pay_EmpRole _EmpRole = new Pay_EmpRole()
                //        {
                //            EmpId = empid,
                //            Status = true,
                //            RoleId = itm
                //        };
                //        db.Pay_EmpRole.Add(_EmpRole);
                //        await db.SaveChangesAsync();
                //    }
                //    else
                //    {
                //        itemroleexist.Status = true;
                //        await db.SaveChangesAsync();
                //    }
                //}
                //var emproles = await db.Pay_EmpRole.Where(x => x.EmpId == empid).ToListAsync();
                //db.Pay_EmpRole.RemoveRange(emproles);
                //await db.SaveChangesAsync();

                //foreach (var item in roleids)
                //{
                //    Pay_EmpRole _EmpRole = new Pay_EmpRole()
                //    {
                //        EmpId = empid,
                //        Status = true,
                //        RoleId = item
                //    };
                //    db.Pay_EmpRole.Add(_EmpRole);
                //    await db.SaveChangesAsync();
                //}
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<int> GetHeadCountByDesgId(int DesgId, int DeptId)
        {
            try
            {
                var emp = await db.Pay_BranchStaffStrength.Where(x => x.DeptId == DeptId && x.DesgId == DesgId).FirstOrDefaultAsync();
                if (emp != null)
                {
                    return emp.ApprovedStrength;
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public async Task<int> GetExisitingHeadCountByDesgId(int DesgId, int DeptId)
        {
            try
            {
                var EmpStrength = await db.Pay_EmpMaster.Where(x => x.DesgId == DesgId && x.DeptId == DeptId && x.StatusId == "A").ToListAsync();
                if (EmpStrength != null)
                {
                    return EmpStrength.Count;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                return 0;
            }

        }

        /// <summary>
        /// Employee Save with Head Count Validation
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<int> ValidateAndSaveEmployee(EmployeeVM mod, int UserId)
        {
            try
            {
                var GetBranchStrength = await GetHeadCountByDesgId(mod.DesgId, mod.DeptId);
                var ExistEmpStrength = await GetExisitingHeadCountByDesgId(mod.DesgId, mod.DeptId);


                if (mod.HDeptId != 2)
                {
                    return await SaveEmployee(mod, UserId);
                }
                // No Limit
                //else if (GetBranchStrength == 0)
                //{
                //    return await SaveEmployee(mod, UserId);
                //}
                else if (mod.EmpId > 0)
                {
                    var empstatus = await GetEmployeeById(mod.EmpId);
                    if ((empstatus.Status != "A" && empstatus.Status != mod.Status) || empstatus.DeptId != mod.DeptId)
                    {
                        if (ExistEmpStrength < GetBranchStrength)
                        {
                            return await SaveEmployee(mod, UserId);
                        }
                    }
                    else
                    {
                        return await SaveEmployee(mod, UserId);
                    }
                }
                //Get Existing Count From Database and Verify with the Limit Allocated
                else if (ExistEmpStrength < GetBranchStrength)
                {
                    return await SaveEmployee(mod, UserId);
                }
                //Will Give Error in All Other Cases

                return -1;

            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public async Task<bool> SaveEmployeeStatusLog(PayEmpStatusLogVM mod)
        {
            try
            {
                Pay_EmpStatusLog dat = new Pay_EmpStatusLog();
                dat.DesgId = mod.DesgId;
                dat.EmpId = mod.EmpId;
                dat.LocId = mod.LocId;
                dat.StatusId = mod.StatusId;
                dat.TransDate = DateTime.Now;
                dat.UserId = mod.UserId;
                dat.AttendanceStatus = mod.AttendanceStatus;
                dat.FaceAllow = mod.FaceAllow;
                dat.Finalized = mod.Finalized;
                dat.BlockSalary = mod.BlockSalary;
                dat.ManualSalary = mod.ManualSalary;
                dat.Remarks = mod.Remarks;
                db.Pay_EmpStatusLog.Add(dat);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Employee Save Without Head Count Validation
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<int> SaveEmployee(EmployeeVM mod, int UserId)
        {
            try
            {
                if (mod.EmpId > 0)
                {
                    var emp = await db.Pay_EmpMaster.FindAsync(mod.EmpId);
                    if (emp.DeptId != mod.DeptId)
                    {
                        var tbldept = new Pay_DepartmentLog
                        {
                            EmpId = mod.EmpId,
                            DeptId = mod.DeptId,
                            UserId = UserId,
                            TransDate = DateTime.Now
                        };
                        db.Pay_DepartmentLog.Add(tbldept);
                        await db.SaveChangesAsync();
                    }
                    if (emp.DesgId != mod.DesgId)
                    {
                        var tbldesg = new Pay_DesignationLog
                        {
                            EmpId = mod.EmpId,
                            DesgId = mod.DesgId,
                            UserId = UserId,
                            TransDate = DateTime.Now
                        };
                        db.Pay_DesignationLog.Add(tbldesg);
                        await db.SaveChangesAsync();
                    }
                    if (emp.StatusId != mod.Status || emp.AttendanceStatus != (mod.AStatus == "A" ? true : false) || emp.IsFaceAllow != mod.IsFaceAllow || emp.IsFinalized != mod.IsFinalized || emp.IsFinalized != mod.IsFinalized || emp.MSalary != mod.MSalary)
                    {
                        await SaveEmployeeStatusLog(new PayEmpStatusLogVM()
                        {
                            DesgId = mod.DesgId,
                            StatusId = mod.Status,
                            EmpId = Convert.ToInt32(mod.EmpId),
                            LocId = Convert.ToInt32(mod.DeptId),
                            UserId = UserId,
                            AttendanceStatus = mod.AStatus == "A" ? true : false,
                            BlockSalary = mod.BlockSalary,
                            FaceAllow = mod.IsFaceAllow,
                            ManualSalary = mod.MSalary,
                            Finalized = mod.IsFinalized
                        });
                    }

                    if (mod.Status == "I" && emp.StatusId == "A")
                    {
                        await new SecurityBL().MakeUserInActive(emp.EmpId);
                    }


                    emp.Address = mod.Address;
                    emp.BloodGroup = mod.BloodGrp;
                    emp.CNIC = mod.CNIC;
                    if (mod.HDeptId == 1)
                    {
                        mod.LocId = 72;
                    }
                    else
                    {
                        mod.LocId = mod.DeptId;
                    }
                    emp.DeptId = mod.DeptId;
                    emp.DesgId = mod.DesgId;
                    emp.DOJ = Convert.ToDateTime(mod.DOJ);
                    emp.Email = mod.Email;
                    emp.EmpId = Convert.ToInt32(mod.EmpId);
                    emp.EmpName = mod.EmpName;
                    emp.Gender = mod.Gender;
                    emp.MaritalStatus = mod.MaritalStatus;
                    emp.Mobile1 = mod.Mobile1;
                    emp.Mobile2 = mod.Mobile2;
                    emp.Religion = mod.Religion;
                    //emp.TransDate = DateTime.Now;
                    emp.UserId = UserId;
                    emp.DOB = mod.DOB;
                    emp.StatusId = mod.Status;
                    emp.FName = mod.FName;
                    emp.RefAddress = mod.RefAddress;
                    emp.RefCNIC = mod.RefCNIC;
                    emp.RefContactNo = mod.RefContactNo;
                    emp.RefPerson = mod.RefPerson;
                    emp.Remarks = mod.Remarks;
                    emp.ShiftId = mod.ShiftId;
                    emp.PayScaleId = mod.PayScaleId;
                    emp.JobType = mod.JobType;
                    emp.Weekend = mod.Weekend;
                    emp.CNICExpireDate = mod.CNICExpireDate;
                    //emp.ApprovedBy = UserId;
                    //emp.ApprovedDate = DateTime.Now;
                    emp.WeeklyHoliday = mod.WeeklyHoliday;
                    emp.AppointmentTypeId = mod.AppointmentTypeId;
                    emp.AttendanceStatus = mod.AStatus == "A" ? true : false;
                    emp.IsFaceAllow = mod.IsFaceAllow;
                    emp.IsFinalized = mod.IsFinalized;
                    emp.BlockSalary = mod.BlockSalary;
                    emp.ModifiedDate = DateTime.Now;
                    emp.ModifiedBy = UserId;

                    emp.MSalary = mod.MSalary;
                    await db.SaveChangesAsync();
                    if (mod.RptTo > 0)
                    {
                        EmployeeHirarchyAllocationAddUpdate(new EmployeeHirarchyAllocationVM()
                        {
                            EmpId = emp.EmpId,
                            ReportingTo = mod.RptTo
                        }, UserId);
                    }

                    //await AddUpdateEmpRole(mod.EmpRolesIds, emp.EmpId);


                    //var BSalaryExist = IsBasicSalaryExist(emp.EmpId);

                    //if (mod.BasicSalary > 0 && BSalaryExist == false)
                    //{
                    //    List<EmployeeCalculatedSalaryVM> lstsal = new List<EmployeeCalculatedSalaryVM>();
                    //    EmployeeCalculatedSalaryVM emps = new EmployeeCalculatedSalaryVM()
                    //    {
                    //        GrossSalary = mod.BasicSalary,
                    //        EffectiveDate = DateTime.Now,
                    //        Remarks = "Basic Salary - ER",
                    //        EmployeeId = Convert.ToInt32(mod.EmpId),
                    //        SalaryType = "B"
                    //    };
                    //    lstsal.Add(emps);
                    //    await CreateEmployeeSalary(lstsal, UserId);
                    //}




                }
                else
                {

                    var empfound = await db.Pay_EmpMaster.Where(x => x.CNIC == mod.CNIC).FirstOrDefaultAsync();
                    if (empfound == null)
                    {
                        int locid = 0;
                        if (mod.HDeptId == 1)
                        {
                            locid = 72;
                        }
                        else
                        {
                            locid = mod.DeptId;
                        }
                        Pay_EmpMaster emp = new Pay_EmpMaster()
                        {
                            Address = mod.Address,
                            BloodGroup = mod.BloodGrp,
                            CNIC = mod.CNIC,
                            DeptId = mod.DeptId,
                            DesgId = mod.DesgId,
                            DOJ = Convert.ToDateTime(mod.DOJ),
                            Email = mod.Email,
                            EmpId = Convert.ToInt32(mod.EmpId),
                            EmpName = mod.EmpName,
                            Gender = mod.Gender,
                            MaritalStatus = mod.MaritalStatus,
                            Mobile1 = mod.Mobile1,
                            StatusId = mod.Status,
                            Mobile2 = mod.Mobile2,
                            Religion = mod.Religion,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            DOB = mod.DOB,
                            FName = mod.FName,
                            RefAddress = mod.RefAddress,
                            RefCNIC = mod.RefCNIC,
                            RefContactNo = mod.RefContactNo,
                            CNICExpireDate = mod.CNICExpireDate,
                            RefPerson = mod.RefPerson,
                            Remarks = mod.Remarks,
                            ShiftId = mod.ShiftId,
                            PayScaleId = mod.PayScaleId,
                            LocId = locid,
                            ApprovedBy = UserId,
                            ApprovedDate = DateTime.Now,
                            AppointmentTypeId = mod.AppointmentTypeId,
                            Weekend = mod.Weekend,
                            AttendanceStatus = mod.AStatus == "A" ? true : false,
                            WeeklyHoliday = mod.WeeklyHoliday,
                            IsFaceAllow = mod.IsFaceAllow,
                            IsFinalized = mod.IsFinalized,
                            BlockSalary = mod.BlockSalary,
                            MSalary = mod.MSalary
                        };
                        db.Pay_EmpMaster.Add(emp);

                        await db.SaveChangesAsync();
                        mod.EmpId = emp.EmpId;


                        if (mod.RptTo != null)
                        {
                            EmployeeHirarchyAllocationAddUpdate(new EmployeeHirarchyAllocationVM()
                            {
                                EmpId = emp.EmpId,
                                ReportingTo = mod.RptTo
                            }, UserId);
                        }

                        //await AddUpdateEmpRole(mod.EmpRolesIds, emp.EmpId);

                        if (mod.BasicSalary > 0)
                        {
                            List<EmployeeCalculatedSalaryVM> lstsal = new List<EmployeeCalculatedSalaryVM>();
                            EmployeeCalculatedSalaryVM emps = new EmployeeCalculatedSalaryVM()
                            {
                                GrossSalary = mod.BasicSalary,
                                EffectiveDate = DateTime.Now,
                                Remarks = "Basic Salary - ER",
                                EmployeeId = Convert.ToInt32(mod.EmpId),
                                SalaryType = "B"
                            };
                            lstsal.Add(emps);
                            await CreateEmployeeSalary(lstsal, UserId, "Basic Salary - ER", DateTime.Now);
                        }
                    }
                    else
                    {
                        return 001;
                    }

                }

                return Convert.ToInt32(mod.EmpId);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<List<EducationVM>> EducationList(int EmpId)
        {
            try
            {
                return await db.Pay_Education.Where(x => x.Status && x.EmpId == EmpId).Select(x =>
                new EducationVM
                {
                    EmpId = x.EmpId,
                    FromDate = x.FromDate,
                    Institute = x.Institute,
                    QualificationId = x.QualificationId,
                    RowId = x.EducationId,
                    ToDate = x.ToDate
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<EducationVM> CreateEducation(EducationVM mod, int UserId)
        {
            try
            {
                Pay_Education tbl = new Pay_Education
                {
                    EmpId = mod.EmpId,
                    FromDate = mod.FromDate,
                    Institute = mod.Institute,
                    QualificationId = mod.QualificationId,
                    Status = true,
                    ToDate = mod.ToDate,
                    TransDate = DateTime.Now,
                    UserId = UserId
                };
                db.Pay_Education.Add(tbl);
                await db.SaveChangesAsync();
                mod.RowId = tbl.EmpId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateEducation(EducationVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pay_Education.FindAsync(mod.RowId);
                tbl.EmpId = mod.EmpId;
                tbl.FromDate = mod.FromDate;
                tbl.Institute = mod.Institute;
                tbl.QualificationId = mod.QualificationId;
                tbl.TransDate = DateTime.Now;
                tbl.UserId = UserId;
                tbl.ToDate = mod.ToDate;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyEducation(EducationVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pay_Education.FindAsync();
                tbl.Status = false;
                tbl.TransDate = DateTime.Now;
                tbl.UserId = UserId;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<ExperienceVM>> ExperienceList(int EmpId)
        {
            try
            {
                return await db.Pay_Experience.Where(x => x.Status && x.EmpId == EmpId).Select(x =>
                 new ExperienceVM
                 {
                     EmpId = x.EmpId,
                     FromDate = x.FromDate,
                     Company = x.Company,
                     Designation = x.Designation,
                     RowId = x.EmpId,
                     ToDate = x.ToDate
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ExperienceVM> CreateExperience(ExperienceVM mod, int UserId)
        {
            try
            {
                Pay_Experience tbl = new Pay_Experience
                {
                    EmpId = mod.EmpId,
                    FromDate = mod.FromDate,
                    Company = mod.Company,
                    Designation = mod.Designation,
                    Status = true,
                    ToDate = mod.ToDate,
                    TransDate = DateTime.Now,
                    UserId = UserId
                };
                db.Pay_Experience.Add(tbl);
                await db.SaveChangesAsync();
                mod.RowId = tbl.EmpId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateExperience(ExperienceVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pay_Experience.FindAsync(mod.RowId);
                tbl.EmpId = mod.EmpId;
                tbl.FromDate = mod.FromDate;
                tbl.Company = mod.Company;
                tbl.Designation = mod.Designation;
                tbl.ToDate = mod.ToDate;
                tbl.TransDate = DateTime.Now;
                tbl.UserId = UserId;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyExperience(ExperienceVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pay_Experience.FindAsync();
                tbl.Status = false;
                tbl.TransDate = DateTime.Now;
                tbl.UserId = UserId;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Pay_Qualification>> QualificationList()
        {
            try
            {
                return await db.Pay_Qualification.Where(x => x.Status).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Qualification_VM> CreateQualification(Qualification_VM mod, int UserId)
        {
            try
            {
                Pay_Qualification tbl = new Pay_Qualification
                {
                    QualificationTitle = mod.QualificationTitle,
                    Status = mod.Status
                };
                db.Pay_Qualification.Add(tbl);
                await db.SaveChangesAsync();
                mod.QualificationId = tbl.QualificationId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateQualification(Qualification_VM mod, int UserId)
        {
            try
            {
                Pay_Qualification tbl = new Pay_Qualification
                {
                    QualificationId = mod.QualificationId,
                    QualificationTitle = mod.QualificationTitle,
                    Status = mod.Status
                };
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyQualification(Qualification_VM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pay_Qualification.FindAsync();
                tbl.Status = false;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<Pay_EmpMaster>> EmpListByDesigMapping(int desgid)
        {
            List<Pay_EmpMaster> _Employees = new List<Pay_EmpMaster>();
            try
            {
                var reportingdesg = db.Pay_Designation.Where(x => x.DesgId == desgid).FirstOrDefault();
                if (reportingdesg != null)
                {
                    var oprDesg = new int[] { 253, 261, 268, 285, 291, 391, 191, 401, 290, 252, 197 };
                    if (oprDesg.Contains(reportingdesg.DesgId))
                        _Employees = await db.Pay_EmpMaster.Where(x => oprDesg.Contains(x.DesgId) && x.StatusId == "A").ToListAsync();
                    else
                        _Employees = await db.Pay_EmpMaster.Where(x => x.DesgId == reportingdesg.ReportingTo && x.StatusId == "A").ToListAsync();
                }
                return _Employees;
            }
            catch (Exception)
            {
                return _Employees;
            }
        }
        public async Task<List<EmployeeVM>> EmpListByDesg(int desgid)
        {
            try
            {
                var lst = await db.Pay_EmpMaster.Where(x => x.DesgId == desgid && x.StatusId == "A").Select(x =>
                new EmployeeVM { EmpId = x.EmpId, EmpName = x.EmpId.ToString() + " " + x.EmpName }).ToListAsync();
                lst.Insert(0, new EmployeeVM { EmpId = 0, EmpName = "NA" });
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<EmployeeVM>> GetMapCashierByLocation(int locid)
        {
            try
            {
                var loc = await db.Comp_Locations.Where(x => x.LocId == locid).FirstOrDefaultAsync();

                if (loc != null)
                {
                    return await (from item in db.Pay_EmpMaster
                                  join emploc in db.Pay_EmpLocationMapping on item.EmpId equals emploc.EmpId
                                  where item.DesgId == 257 && emploc.LocId == loc.CashCenter
                                  select new EmployeeVM()
                                  {
                                      EmpId = item.EmpId,
                                      EmpName = item.EmpName
                                  }).ToListAsync();
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<BankVM>> GetMapBankByLocation(int locid)
        {
            try
            {
                var loc = await db.Comp_Locations.Where(x => x.LocId == locid).FirstOrDefaultAsync();
                if (loc != null)
                {
                    return await (from item in db.Fin_Accounts
                                  join bank in db.Fin_BankMapping on item.AccId equals bank.BankAcc
                                  where bank.LocId == loc.CashCenter
                                  select new BankVM()
                                  {
                                      BankId = item.AccId,
                                      BankName = item.SubCodeDesc
                                  }).ToListAsync();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void EmployeeHirarchyAllocationAddUpdate(EmployeeHirarchyAllocationVM mod, int userid)
        {
            try
            {
                var EmpHirarExist = db.Pay_EmpReportingHierarchy.Where(x => x.EmpId == mod.EmpId).FirstOrDefault();
                if (EmpHirarExist == null)
                {
                    Pay_EmpReportingHierarchy EmpHirar = new Pay_EmpReportingHierarchy()
                    {
                        DefinedBy = userid,
                        DefinedDate = DateTime.Now,
                        EmpId = Convert.ToInt32(mod.EmpId),
                        ReportingTo = Convert.ToInt32(mod.ReportingTo)
                    };
                    db.Pay_EmpReportingHierarchy.Add(EmpHirar);
                    db.SaveChanges();
                }
                else
                {
                    EmpHirarExist.EmpId = Convert.ToInt32(mod.EmpId);
                    EmpHirarExist.ModifiedBy = userid;
                    EmpHirarExist.ModifiedDate = DateTime.Now;
                    EmpHirarExist.ReportingTo = Convert.ToInt32(mod.ReportingTo);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

        }
        public async Task<List<PayEmpSalaryVM>> GetEmployeeSalaryLog(long EmpId)
        {
            try
            {
                return await (from emp in db.Pay_EmpMaster
                              join empsale in db.Pay_SalarySheet_E on emp.EmpId equals empsale.EmpId
                              join empsal in db.Pay_SalarySheet on empsale.SheetId equals empsal.SheetId
                              where emp.EmpId == EmpId
                              select new PayEmpSalaryVM()
                              {
                                  EmpId = emp.EmpId,
                                  CNIC = emp.CNIC,
                                  department = emp.Pay_Department.DeptName,
                                  BankSalary = empsale.BankSalary,
                                  BasicSalary = empsale.BasicSalary,
                                  Month = empsal.SalaryMonth
                              }).ToListAsync();
            }
            catch (Exception e)
            {
                return new List<PayEmpSalaryVM>();
            }
        }

        public async Task<List<PayEmpChequeVM>> ChequeList(long empid)
        {
            try
            {
                return await db.Pay_EmpCheque.Where(x => x.EmpId == empid && x.ChequeStatus).Select(x => new PayEmpChequeVM()
                {

                    BankName = x.BankName,
                    ChequeAmount = x.ChequeAmount,
                    ChequeId = x.ChequeId,
                    ChequeNo = x.ChequeNo,
                    ChequeStatus = x.ChequeStatus,
                    ChequeType = x.ChequeType,
                    EmpId = x.EmpId,
                    IsChequeReturn = x.IsChequeReturn

                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<PayEmpChequeVM> CreateEmpCheque(PayEmpChequeVM mod, int UserId)
        {
            try
            {
                Pay_EmpCheque tbl = new Pay_EmpCheque
                {
                    BankName = mod.BankName,
                    ChequeAmount = mod.ChequeAmount,
                    ChequeNo = mod.ChequeNo,
                    ChequeStatus = true,
                    ChequeType = mod.ChequeType,
                    EmpId = mod.EmpId,
                    IsChequeReturn = mod.IsChequeReturn
                };
                db.Pay_EmpCheque.Add(tbl);
                await db.SaveChangesAsync();
                mod.ChequeId = tbl.ChequeId;
                return mod;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCheque(PayEmpChequeVM mod, int UserId)
        {
            try
            {
                var empchq = await db.Pay_EmpCheque.Where(x => x.ChequeId == mod.ChequeId).FirstOrDefaultAsync();
                empchq.BankName = mod.BankName;
                empchq.ChequeAmount = mod.ChequeAmount;
                empchq.ChequeNo = mod.ChequeNo;
                empchq.ChequeType = mod.ChequeType;
                empchq.IsChequeReturn = mod.IsChequeReturn;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyCheque(PayEmpChequeVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pay_EmpCheque.Where(x => x.ChequeId == mod.ChequeId).FirstOrDefaultAsync();
                tbl.ChequeStatus = false;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        #endregion Employee Profile,Education,Experience,Qualification
        #region Employee Promotion/Demotion

        public int GetAttendanceCount(int EmpId)
        {
            var attcount = db.Pay_AttendanceFinal.Where(x => x.EmpId == EmpId).Count();
            return attcount;
        }

        public async Task<int> SaveEmployeePromotion(EmpPromotionVM mod, int UserId)
        {
            try
            {
                //  var currentemp = await GetEmployeeDetailById(mod.EmpId);
                var GetBranchStrength = await GetHeadCountByDesgId(mod.NewDesgId, Convert.ToInt32(mod.NewDeptId));
                var ExistEmpStrength = await GetExisitingHeadCountByDesgId(mod.NewDesgId, Convert.ToInt32(mod.NewDeptId));
                var Dept = await db.Pay_Department.Where(x => x.DeptId == mod.NewDeptId).FirstOrDefaultAsync();
                var AttCount = GetAttendanceCount(mod.EmpId);

                if (AttCount == 0) return 2;

                //Only for Branches 
                if (Dept.HDeptId == 2)
                {
                    if (ExistEmpStrength < GetBranchStrength)
                    {
                        var currentemp = await db.Pay_EmpMaster.Where(x => x.EmpId == mod.EmpId).FirstOrDefaultAsync();
                        var OldDesgId = currentemp.DesgId;
                        var OldDeptId = currentemp.DeptId;

                        if (mod.NewDesgId != currentemp.DesgId)
                        {
                            currentemp.DesgId = mod.NewDesgId;
                            currentemp.DeptId = Convert.ToInt32(mod.NewDeptId);
                            var OldSalary = await GetEmployeeBasicSalary(currentemp.EmpId);
                            Pay_EmpPromotions obj = new Pay_EmpPromotions()
                            {
                                ApprovedBy = UserId,
                                ApprovedDate = DateTime.Now,
                                CurrentDesgId = OldDesgId,
                                CurrentSalary = OldSalary,
                                DefinedBy = UserId,
                                DefinedDate = DateTime.Now,
                                DeptId = OldDeptId,
                                EffectiveFrom = mod.EffectiveFrom,
                                EmpId = mod.EmpId,
                                NewDesgId = mod.NewDesgId,
                                NewSalary = mod.NewSalary,
                                Reason = mod.Reason,
                                TransType = mod.TransType,
                                NewDeptId = mod.NewDeptId
                            };
                            db.Pay_EmpPromotions.Add(obj);
                            await db.SaveChangesAsync();
                            if (OldSalary > 0)
                            {
                                var sal = await db.Pay_EmpSalary.Where(x => x.EmpId == mod.EmpId && x.SalTypeId == "B").FirstOrDefaultAsync();
                                if (sal != null)
                                {
                                    if (mod.TransType == "P")
                                    {
                                        var incrementamnt = mod.NewSalary - OldSalary;
                                        if (incrementamnt > 0)
                                        {
                                            Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
                                            {
                                                EmpId = mod.EmpId,
                                                EffectiveFrom = mod.EffectiveFrom,
                                                Remarks = mod.Reason,
                                                UserId = UserId,
                                                Salary = incrementamnt,
                                                SalTypeId = "I",
                                                TransDate = DateTime.Now
                                            };
                                            db.Pay_EmpSalary.Add(_EmployeeSalary);
                                            await db.SaveChangesAsync();
                                        }
                                    }
                                    else if (mod.TransType == "D")
                                    {
                                        var incrementamnt = OldSalary - mod.NewSalary;
                                        if (incrementamnt > 0)
                                        {
                                            Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
                                            {
                                                EmpId = mod.EmpId,
                                                EffectiveFrom = mod.EffectiveFrom,
                                                Remarks = mod.Reason,
                                                UserId = UserId,
                                                Salary = incrementamnt,
                                                SalTypeId = "D",
                                                TransDate = DateTime.Now
                                            };
                                            db.Pay_EmpSalary.Add(_EmployeeSalary);
                                            await db.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var sal = await db.Pay_EmpSalary.Where(x => x.EmpId == mod.EmpId && x.SalTypeId == "B").FirstOrDefaultAsync();
                                if (sal == null)
                                {
                                    Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
                                    {
                                        EmpId = mod.EmpId,
                                        EffectiveFrom = mod.EffectiveFrom,
                                        Remarks = mod.Reason,
                                        UserId = UserId,
                                        Salary = mod.NewSalary,
                                        SalTypeId = "B",
                                        TransDate = DateTime.Now
                                    };
                                    db.Pay_EmpSalary.Add(_EmployeeSalary);
                                    await db.SaveChangesAsync();
                                }
                            }
                            if (OldDesgId != mod.NewDesgId)
                            {
                                await UpdateDesginationLog(mod.EmpId);
                                await AddDesignationLog(mod.EmpId, UserId, mod.NewDesgId);
                            }

                            if (OldDeptId != mod.NewDeptId)
                            {
                                await UpdateDepartmentLog(mod.EmpId);
                                await AddDepartmentLog(mod.EmpId, UserId, mod.NewDesgId);
                            }

                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 3;
                    }
                }
                else
                {
                    var currentemp = await db.Pay_EmpMaster.Where(x => x.EmpId == mod.EmpId).FirstOrDefaultAsync();
                    var OldDesgId = currentemp.DesgId;
                    var OldDeptId = currentemp.DeptId;

                    if (mod.NewDesgId != currentemp.DesgId)
                    {
                        currentemp.DesgId = mod.NewDesgId;
                        currentemp.DeptId = Convert.ToInt32(mod.NewDeptId);
                        var OldSalary = await GetEmployeeBasicSalary(currentemp.EmpId);
                        Pay_EmpPromotions obj = new Pay_EmpPromotions()
                        {
                            ApprovedBy = UserId,
                            ApprovedDate = DateTime.Now,
                            CurrentDesgId = OldDesgId,
                            CurrentSalary = OldSalary,
                            DefinedBy = UserId,
                            DefinedDate = DateTime.Now,
                            DeptId = OldDeptId,
                            EffectiveFrom = mod.EffectiveFrom,
                            EmpId = mod.EmpId,
                            NewDesgId = mod.NewDesgId,
                            NewSalary = mod.NewSalary,
                            Reason = mod.Reason,
                            TransType = mod.TransType,
                            NewDeptId = mod.NewDeptId
                        };
                        db.Pay_EmpPromotions.Add(obj);
                        await db.SaveChangesAsync();
                        if (OldSalary > 0)
                        {
                            var sal = await db.Pay_EmpSalary.Where(x => x.EmpId == mod.EmpId && x.SalTypeId == "B").FirstOrDefaultAsync();
                            if (sal != null)
                            {
                                if (mod.TransType == "P")
                                {
                                    var incrementamnt = mod.NewSalary - OldSalary;
                                    if (incrementamnt > 0)
                                    {
                                        Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
                                        {
                                            EmpId = mod.EmpId,
                                            EffectiveFrom = mod.EffectiveFrom,
                                            Remarks = mod.Reason,
                                            UserId = UserId,
                                            Salary = incrementamnt,
                                            SalTypeId = "I",
                                            TransDate = DateTime.Now
                                        };
                                        db.Pay_EmpSalary.Add(_EmployeeSalary);
                                        await db.SaveChangesAsync();
                                    }
                                }
                                else if (mod.TransType == "D")
                                {
                                    var incrementamnt = OldSalary - mod.NewSalary;
                                    if (incrementamnt > 0)
                                    {
                                        Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
                                        {
                                            EmpId = mod.EmpId,
                                            EffectiveFrom = mod.EffectiveFrom,
                                            Remarks = mod.Reason,
                                            UserId = UserId,
                                            Salary = incrementamnt,
                                            SalTypeId = "D",
                                            TransDate = DateTime.Now
                                        };
                                        db.Pay_EmpSalary.Add(_EmployeeSalary);
                                        await db.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                        else
                        {
                            var sal = await db.Pay_EmpSalary.Where(x => x.EmpId == mod.EmpId && x.SalTypeId == "B").FirstOrDefaultAsync();
                            if (sal == null)
                            {
                                Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
                                {
                                    EmpId = mod.EmpId,
                                    EffectiveFrom = mod.EffectiveFrom,
                                    Remarks = mod.Reason,
                                    UserId = UserId,
                                    Salary = mod.NewSalary,
                                    SalTypeId = "B",
                                    TransDate = DateTime.Now
                                };
                                db.Pay_EmpSalary.Add(_EmployeeSalary);
                                await db.SaveChangesAsync();
                            }
                        }
                        if (OldDesgId != mod.NewDesgId)
                        {
                            await UpdateDesginationLog(mod.EmpId);
                            await AddDesignationLog(mod.EmpId, UserId, mod.NewDesgId);
                        }

                        if (OldDeptId != mod.NewDeptId)
                        {
                            await UpdateDepartmentLog(mod.EmpId);
                            await AddDepartmentLog(mod.EmpId, UserId, mod.NewDesgId);
                        }

                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }

            }
            catch (Exception e)
            {
                return 0;
            }
        }
        #endregion
        #region DesignationLog/DepartmentLog
        public async Task<Pay_DesignationLog> UpdateDesginationLog(int EmpId)
        {
            try
            {
                var dsg = db.Pay_DesignationLog.Where(x => x.EmpId == EmpId).OrderByDescending(x => x.TransId).FirstOrDefault();
                if (dsg != null)
                {
                    dsg.ToDate = DateTime.Now;
                    dsg.TransDate = DateTime.Now;
                    await db.SaveChangesAsync();
                    return dsg;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {

                return null;
            }

        }

        public async Task<bool> AddDesignationLog(int EmpId, int UserId, int DesgId)
        {
            try
            {
                Pay_DesignationLog desg = new Pay_DesignationLog()
                {
                    DesgId = DesgId,
                    EmpId = EmpId,
                    FromDate = DateTime.Now,
                    TransDate = DateTime.Now,
                    UserId = UserId
                };
                db.Pay_DesignationLog.Add(desg);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<Pay_DepartmentLog> UpdateDepartmentLog(int EmpId)
        {
            var dep = db.Pay_DepartmentLog.Where(x => x.EmpId == EmpId).OrderByDescending(x => x.TransId).FirstOrDefault();
            if (dep != null)
            {
                dep.EndDate = DateTime.Now;
                dep.TransDate = DateTime.Now;
                await db.SaveChangesAsync();
                return dep;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> AddDepartmentLog(int EmpId, int UserId, int DeptId)
        {
            try
            {
                Pay_DepartmentLog dept = new Pay_DepartmentLog()
                {
                    DeptId = DeptId,
                    EmpId = EmpId,
                    StartDate = DateTime.Now,
                    TransDate = DateTime.Now,
                    UserId = UserId
                };
                db.Pay_DepartmentLog.Add(dept);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion
        #region EmployeeBranchRegistration
        public async Task<List<BranchEmployeeVM>> BranchEmployeeList(int LocId)
        {
            try
            {
                var lst = await (from x in db.Pay_EmpMaster
                                 join d in db.Pay_Department on x.DeptId equals d.DeptId
                                 join dd in db.Pay_Designation on x.DesgId equals dd.DesgId
                                 where x.StatusId == "A" && x.ApprovedBy == null
                                 && d.DeptId == LocId
                                 //&& (dd.DesgId == DesgIdSel || DesgIdSel == 0)
                                 select new BranchEmployeeVM
                                 {
                                     Address = x.Address,
                                     BloodGrp = x.BloodGroup,
                                     CNIC = x.CNIC,
                                     DeptId = x.DeptId,
                                     DesgId = x.DesgId,
                                     DOJ = x.DOJ,
                                     DOB = x.DOB,
                                     Email = x.Email,
                                     EmpId = x.EmpId,
                                     EmpName = x.EmpName,
                                     Gender = x.Gender,
                                     MaritalStatus = x.MaritalStatus,
                                     Mobile1 = x.Mobile1,
                                     Mobile2 = x.Mobile2,
                                     Religion = x.Religion,
                                     TransDate = x.TransDate,
                                     UserId = x.UserId,
                                     FName = x.FName,
                                     RefAddress = x.RefAddress,
                                     RefCNIC = x.RefCNIC,
                                     RefContactNo = x.RefContactNo,
                                     RefPerson = x.RefPerson,
                                     Remarks = x.Remarks,
                                     ShiftId = x.ShiftId,
                                     Status = x.StatusId,
                                     //  RptTo = (s == null ? 0 : s.ReportingTo),
                                     HDeptId = d.HDeptId,
                                     PayScaleId = x.PayScaleId,
                                     WeeklyHoliday = x.WeeklyHoliday,
                                     JobType = x.JobType,
                                     AppointmentTypeId = x.AppointmentTypeId,
                                     Weekend = x.Weekend,
                                     JoinDate = x.DOJ,
                                     DeptName = d.DeptName,
                                     DesgName = dd.DesgName,
                                     //  EmpRolesIds = x.Pay_EmpRole.Where(y => y.EmpId == x.EmpId).Select(x => x.RoleId).ToList()
                                     //Color = String.Format("#{0:X6}", new Random().Next(0x1000000))
                                 }).ToListAsync();
                //var random = new Random();
                //lst.ForEach(x => x.Color = String.Format("#{0:X6}", random.Next(0x1000000)));
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// Employee Save with Head Count Validation
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<int> ValidateAndSaveBranchEmployee(EmployeeVM mod, int GroupId, int UserId, int locid)
        {
            try
            {
                var GetBranchStrength = await GetHeadCountByDesgId(mod.DesgId, locid);
                var ExistEmpStrength = await GetExisitingHeadCountByDesgId(mod.DesgId, locid);

                // No Limit
                //if (GetBranchStrength == 0)
                //{
                //    return await SaveBranchEmployee(mod, UserId, locid);
                //}

                if (GroupId == 1079)
                {
                    mod.DeptId = 1038;
                    mod.LocId = 1038;
                    locid = 1038;
                    return await SaveBranchEmployee(mod, UserId, locid);
                }

                if (mod.EmpId > 0)
                {
                    var empstatus = await GetEmployeeById(mod.EmpId);
                    if (empstatus.Status != "A" || empstatus.DeptId != mod.DeptId)
                    {
                        if (ExistEmpStrength < GetBranchStrength)
                        {
                            return await SaveBranchEmployee(mod, UserId, locid);
                        }
                    }
                    else
                    {
                        return await SaveBranchEmployee(mod, UserId, locid);
                    }
                }
                //Get Existing Count From Database and Verify with the Limit Allocated
                else if (ExistEmpStrength < GetBranchStrength)
                {
                    return await SaveBranchEmployee(mod, UserId, locid);
                }
                //Will Give Error in All Other Cases

                return -1;

            }
            catch (Exception e)
            {
                return -1;
            }
        }
        public async Task<int> SaveBranchEmployee(EmployeeVM mod, int UserId, int locid)
        {
            try
            {

                if (mod.EmpId > 0)
                {
                    var emp = await db.Pay_EmpMaster.FindAsync(mod.EmpId);
                    emp.Address = mod.Address;
                    emp.BloodGroup = mod.BloodGrp;
                    emp.CNIC = mod.CNIC;
                    //emp.DeptId = locid;
                    emp.LocId = locid;
                    emp.DesgId = mod.DesgId;
                    emp.DOJ = DateTime.Now;
                    emp.Email = mod.Email;
                    //emp.EmpId = Convert.ToInt32(mod.EmpId);
                    emp.EmpName = mod.EmpName;
                    emp.Gender = mod.Gender;
                    emp.MaritalStatus = mod.MaritalStatus;
                    emp.Mobile1 = mod.Mobile1;
                    emp.Mobile2 = mod.Mobile2;
                    emp.Religion = mod.Religion;
                    emp.TransDate = DateTime.Now;
                    emp.UserId = UserId;
                    emp.DOB = mod.DOB;
                    //emp.StatusId = mod.Status;
                    emp.FName = mod.FName;
                    emp.AttendanceStatus = true;
                    //emp.RefAddress = mod.RefAddress;
                    //emp.RefCNIC = mod.RefCNIC;
                    //emp.RefContactNo = mod.RefContactNo;
                    //emp.RefPerson = mod.RefPerson;
                    //emp.Remarks = mod.Remarks;
                    emp.ShiftId = mod.ShiftId;
                    //emp.PayScaleId = mod.PayScaleId;
                    //emp.JobType = mod.JobType;
                    //emp.Weekend = mod.Weekend;
                    //emp.WeeklyHoliday = mod.WeeklyHoliday;
                    //emp.AppointmentTypeId = mod.AppointmentTypeId;
                    await db.SaveChangesAsync();
                }
                else
                {
                    var empfound = await db.Pay_EmpMaster.Where(x => x.CNIC == mod.CNIC).FirstOrDefaultAsync();
                    if (empfound == null)
                    {
                        Pay_EmpMaster emp = new Pay_EmpMaster()
                        {
                            Address = mod.Address,
                            BloodGroup = mod.BloodGrp,
                            CNIC = mod.CNIC,
                            DeptId = locid,
                            DesgId = mod.DesgId,
                            DOJ = DateTime.Now,
                            Email = mod.Email,
                            //EmpId = Convert.ToInt32(mod.EmpId),
                            EmpName = mod.EmpName,
                            Gender = mod.Gender,
                            MaritalStatus = mod.MaritalStatus,
                            Mobile1 = mod.Mobile1,
                            StatusId = "A",
                            AttendanceStatus = true,
                            Mobile2 = mod.Mobile2,
                            Religion = mod.Religion,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            DOB = mod.DOB,
                            FName = mod.FName,
                            //RefAddress = mod.RefAddress,
                            //RefCNIC = mod.RefCNIC,
                            //RefContactNo = mod.RefContactNo,
                            //RefPerson = mod.RefPerson,
                            //Remarks = mod.Remarks,
                            ShiftId = mod.ShiftId,
                            //PayScaleId = mod.PayScaleId,
                            //AppointmentTypeId = mod.AppointmentTypeId,
                            //Weekend = mod.Weekend,
                            //WeeklyHoliday = mod.WeeklyHoliday
                        };
                        db.Pay_EmpMaster.Add(emp);
                        await db.SaveChangesAsync();
                        mod.EmpId = emp.EmpId;
                    }
                    else
                    {
                        return 001;
                    }

                }

                return Convert.ToInt32(mod.EmpId);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }



        #endregion
        #region Allowance Setup

        /// <summary>
        /// List the Allowance
        /// </summary>
        /// <returns></returns>
        public List<AllowanceVM> AllowanceList()
        {
            try
            {
                return db.Pay_Allowance.ToList().Select(x =>
             new AllowanceVM
             {
                 AlwId = x.AlwId,
                 AlwName = x.AlwName,
                 AlwType = x.AlwType.ToString(),
                 Amount = x.Amount,
                 Status = x.Status
             }).Where(x => x.Status == true).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// If Allowance Id is Passed Greated Than 0 will be update all otherwise 0 Will a a new Allowance
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<int> CreateUpdateAllowance(AllowanceVM mod)
        {
            try
            {
                if (mod.AlwId > 0)
                {
                    var allw = await db.Pay_Allowance.FindAsync(mod.AlwId);
                    allw.AlwName = mod.AlwName;
                    allw.AlwType = mod.AlwType.ToString();
                    allw.Amount = mod.Amount;
                    allw.Status = true;
                    await db.SaveChangesAsync();
                }
                else
                {
                    var IsExist = db.Pay_Allowance.Where(x => x.AlwName == mod.AlwName).FirstOrDefault();
                    if (IsExist == null)
                    {
                        Pay_Allowance tbl = new Pay_Allowance
                        {
                            AlwName = mod.AlwName,
                            AlwType = mod.AlwType.ToString(),
                            Amount = mod.Amount,
                            Status = true
                        };
                        db.Pay_Allowance.Add(tbl);

                        await db.SaveChangesAsync();
                        mod.AlwId = tbl.AlwId;
                    }
                    else
                    {
                        IsExist.AlwType = mod.AlwType.ToString();
                        IsExist.Amount = mod.Amount;
                        IsExist.Status = true;
                        await db.SaveChangesAsync();
                        return IsExist.AlwId;
                    }
                }
                return mod.AlwId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Soft Delete - Update the Status to false in Allowance
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<bool> DestroyAllowance(AllowanceVM mod)
        {
            try
            {
                var tbl = await db.Pay_Allowance.FindAsync(mod.AlwId);
                tbl.Status = false;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<AllowanceTypeVM> GetAllowanceTypes()
        {
            var lst = new List<AllowanceTypeVM>();
            AllowanceTypeVM _allowancew = new AllowanceTypeVM()
            {
                AlwType = "M",
                AlwName = "Monthly"
            };
            lst.Add(_allowancew);
            AllowanceTypeVM _allowancey = new AllowanceTypeVM()
            {
                AlwType = "D",
                AlwName = "Daily"
            };
            lst.Add(_allowancey);
            return lst.ToList();
        }

        #endregion Allowance Setup
        #region Deduction Setup
        /// <summary>
        /// List the Deduction
        /// </summary>
        /// <returns></returns>
        public List<DeductionVM> DeductionList()
        {
            try
            {
                return db.Pay_Deduction.ToList().Select(x =>
             new DeductionVM
             {
                 DedId = x.DedId,
                 DedName = x.DedName,
                 Amount = x.Amount,
                 Status = x.Status
             }).Where(x => x.Status == true).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// If Deduction Id is Passed Greated Than 0 will be update all otherwise 0 Will a a new Deduction
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<int> CreateUpdateDeduction(DeductionVM mod)
        {
            try
            {
                if (mod.DedId > 0)
                {
                    var allw = await db.Pay_Deduction.FindAsync(mod.DedId);
                    allw.DedName = mod.DedName;
                    allw.Amount = mod.Amount;
                    allw.DedId = mod.DedId;
                    allw.Status = true;
                    await db.SaveChangesAsync();
                }
                else
                {
                    var IsExist = db.Pay_Deduction.Where(x => x.DedName == mod.DedName).FirstOrDefault();
                    if (IsExist == null)
                    {
                        Pay_Deduction tbl = new Pay_Deduction
                        {
                            DedName = mod.DedName,
                            Amount = mod.Amount,
                            Status = true
                        };
                        db.Pay_Deduction.Add(tbl);

                        await db.SaveChangesAsync();
                        mod.DedId = tbl.DedId;
                    }
                    else
                    {
                        IsExist.Amount = mod.Amount;
                        IsExist.Status = true;
                        await db.SaveChangesAsync();
                        return IsExist.DedId;
                    }
                }
                return mod.DedId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Soft Delete - Update the Status to false in Deduction
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<bool> DestroyDeduction(DeductionVM mod)
        {
            try
            {
                var tbl = await db.Pay_Deduction.FindAsync(mod.DedId);
                tbl.Status = false;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Deduction Setup
        #region Allowance Allocation
        public async Task<long> CreateVariableAllowance(VariableAllowanceVM mod, int UserId)
        {
            try
            {

                var obj = new Pay_AllowanceTrans()
                {
                    EmpId = mod.EmpId,
                    AlwId = mod.AlwId,
                    Amount = mod.Amount,
                    Remarks = mod.Remarks,
                    EffectiveDate = mod.EffectiveDate,
                    UserId = UserId,
                    TransDate = DateTime.Now,
                    Status = true,
                    TransType = "A"
                };
                db.Pay_AllowanceTrans.Add(obj);
                await db.SaveChangesAsync();
                return obj.TransId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<List<VariableAllowanceVM>> VariableAllowanceList()
        {
            try
            {
                var Month = DateTime.Now.Month;
                var Year = DateTime.Now.Year;
                return await (from x in db.Pay_AllowanceTrans
                              join d in db.Pay_EmpMaster on x.EmpId equals d.EmpId
                              where x.Status == true && (x.EffectiveDate.Month == Month && x.EffectiveDate.Year == Year)
                              select new VariableAllowanceVM
                              {
                                  TransId = x.TransId,
                                  EmpId = x.EmpId,
                                  EmployeeName = d.EmpName,
                                  CNIC = d.CNIC,
                                  Amount = x.Amount,
                                  Remarks = x.Remarks,
                                  EffectiveDate = x.EffectiveDate,
                                  UserId = x.UserId,
                                  TransDate = x.TransDate,
                                  Status = x.Status
                              }).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<bool> UpdateVAStatus(int id)
        {
            try
            {
                if (id > 0)
                {
                    var pr = await db.Pay_AllowanceTrans.Where(x => x.TransId == id).FirstOrDefaultAsync();
                    if (pr != null)
                    {
                        pr.Status = false;
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
            catch (Exception e)
            {
                return false;
            }
        }
        /// <summary>
        /// List the Allowance
        /// </summary>
        /// <returns></returns>
        public List<AllowanceAllocationVM> GetAllowanceListByType(string type, int id)
        {
            var allowances = db.Pay_Allowance.Select(x => new AllowanceAllocationVM()
            {
                AlwId = x.AlwId,
                AlwName = x.AlwName,
                Status = x.Status
            }).Where(x => x.Status).ToList();
            if (type == "1")
            {
                var allocationassigned = (from pa in db.Pay_Allowance
                                          join paa in db.Pay_AllownaceAllocation
                                          on pa.AlwId equals paa.AlwId
                                          where pa.Status == true & paa.EmpId == id
                                          select paa.AlwId).ToList();
                allowances.Where(x => allocationassigned.Contains(x.AlwId)).ToList().ForEach(x => x.IsSelected = true);

                return allowances;
            }
            else if (type == "2")
            {
                var allocationassigned = (from pa in db.Pay_Allowance
                                          join paa in db.Pay_AllownaceAllocation
                                          on pa.AlwId equals paa.AlwId
                                          where pa.Status == true & paa.DesgId == id
                                          select paa.AlwId).ToList();
                allowances.Where(x => allocationassigned.Contains(x.AlwId)).ToList().ForEach(x => x.IsSelected = true);

                return allowances;
            }
            else if (type == "3")
            {
                var allocationassigned = (from pa in db.Pay_Allowance
                                          join paa in db.Pay_AllownaceAllocation
                                          on pa.AlwId equals paa.AlwId
                                          where pa.Status == true & paa.DeptId == id
                                          select paa.AlwId).ToList();
                allowances.Where(x => allocationassigned.Contains(x.AlwId)).ToList().ForEach(x => x.IsSelected = true);

                return allowances;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// If Allowance Id is Passed Greated Than 0 will be update all otherwise 0 Will a a new Allowance
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<bool> CreateUpdateAllowanceAllocation(List<AllowanceAllocationVM> lst, int typeid, int transacid, int UserId)
        {
            try
            {
                if (typeid != 0)
                {

                    if (typeid == 1)
                    {
                        var EmpAllowances = db.Pay_AllownaceAllocation.Where(x => x.EmpId == transacid);
                        db.Pay_AllownaceAllocation.RemoveRange(EmpAllowances);
                        db.SaveChanges();
                    }
                    else if (typeid == 2)
                    {
                        var DesgAllowances = db.Pay_AllownaceAllocation.Where(x => x.DesgId == transacid);
                        db.Pay_AllownaceAllocation.RemoveRange(DesgAllowances);
                        db.SaveChanges();
                    }
                    else if (typeid == 3)
                    {
                        var DeptAllowances = db.Pay_AllownaceAllocation.Where(x => x.DeptId == transacid);
                        db.Pay_AllownaceAllocation.RemoveRange(DeptAllowances);
                        db.SaveChanges();
                    }
                    foreach (var mods in lst)
                    {
                        if (mods.TypeId == 1)
                        {
                            Pay_AllownaceAllocation tbl = new Pay_AllownaceAllocation
                            {
                                AlwId = mods.AlwId,
                                DeptId = mods.DeptId,
                                DesgId = mods.DesgId,
                                EmpId = mods.EmpId,
                                TransDate = DateTime.Now,
                                AllocId = mods.AllocId,
                                UserId = UserId,
                                Status = true
                            };
                            db.Pay_AllownaceAllocation.Add(tbl);
                            await db.SaveChangesAsync();
                        }
                        else if (mods.TypeId == 2)
                        {
                            Pay_AllownaceAllocation tbl = new Pay_AllownaceAllocation
                            {
                                AlwId = mods.AlwId,
                                DeptId = mods.DeptId,
                                DesgId = mods.DesgId,
                                EmpId = mods.EmpId,
                                TransDate = DateTime.Now,
                                AllocId = mods.AllocId,
                                UserId = UserId,
                                Status = true
                            };
                            db.Pay_AllownaceAllocation.Add(tbl);
                            await db.SaveChangesAsync();
                        }
                        else if (mods.TypeId == 3)
                        {
                            Pay_AllownaceAllocation tbl = new Pay_AllownaceAllocation
                            {
                                AlwId = mods.AlwId,
                                DeptId = mods.DeptId,
                                DesgId = mods.DesgId,
                                EmpId = mods.EmpId,
                                TransDate = DateTime.Now,
                                AllocId = mods.AllocId,
                                UserId = UserId,
                                Status = true
                            };
                            db.Pay_AllownaceAllocation.Add(tbl);
                            await db.SaveChangesAsync();
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Allowance Allocation
        #region Deduction Allocation
        public async Task<long> CreateVariableDeduction(VariableDeductionVM mod, int UserId)
        {
            try
            {

                var obj = new Pay_DeductionTrans()
                {
                    EmpId = mod.EmpId,
                    DedId = mod.DedId,
                    Amount = mod.Amount,
                    Remarks = mod.Remarks,
                    EffectiveDate = mod.EffectiveDate,
                    UserId = UserId,
                    TransDate = DateTime.Now,
                    Status = true
                };
                db.Pay_DeductionTrans.Add(obj);
                await db.SaveChangesAsync();
                return obj.TransId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<List<VariableDeductionVM>> VariableDeductionList()
        {
            try
            {

                var Month = DateTime.Now.Month;
                var Year = DateTime.Now.Year;
                return await (from x in db.Pay_DeductionTrans
                              join d in db.Pay_EmpMaster on x.EmpId equals d.EmpId
                              where x.Status == true && (x.EffectiveDate.Month == Month && x.EffectiveDate.Year == Year)
                              select new VariableDeductionVM
                              {
                                  TransId = x.TransId,
                                  EmpId = x.EmpId,
                                  DedId = x.DedId,
                                  EmployeeName = d.EmpName,
                                  CNIC = d.CNIC,
                                  Amount = x.Amount,
                                  Remarks = x.Remarks,
                                  EffectiveDate = x.EffectiveDate,
                                  UserId = x.UserId,
                                  TransDate = x.TransDate,
                                  Status = x.Status
                              }).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<bool> UpdateVDStatus(int id)
        {
            try
            {
                if (id > 0)
                {
                    var pr = await db.Pay_DeductionTrans.Where(x => x.TransId == id).FirstOrDefaultAsync();
                    if (pr != null)
                    {
                        pr.Status = false;
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
            catch (Exception e)
            {
                return false;
            }
        }
        /// <summary>
        /// List the Allowance
        /// </summary>
        /// <returns></returns>
        public List<DeductionAllocationVM> GetDeductionListByType(string type, int id)
        {
            var deductions = db.Pay_Deduction.Select(x => new DeductionAllocationVM()
            {
                DedId = x.DedId,
                DedName = x.DedName,
                Status = x.Status
            }).Where(x => x.Status).ToList();
            if (type == "1")
            {
                var allocationassigned = (from pa in db.Pay_Deduction
                                          join paa in db.Pay_DeductionAllocation
                                          on pa.DedId equals paa.DedId
                                          where pa.Status == true & paa.EmpId == id
                                          select paa.DedId).ToList();
                deductions.Where(x => allocationassigned.Contains(x.DedId)).ToList().ForEach(x => x.IsSelected = true);

                return deductions;
            }
            else if (type == "2")
            {
                var allocationassigned = (from pa in db.Pay_Deduction
                                          join paa in db.Pay_DeductionAllocation
                                          on pa.DedId equals paa.DedId
                                          where pa.Status == true & paa.DesgId == id
                                          select paa.DedId).ToList();
                deductions.Where(x => allocationassigned.Contains(x.DedId)).ToList().ForEach(x => x.IsSelected = true);

                return deductions;
            }
            else if (type == "3")
            {
                var allocationassigned = (from pa in db.Pay_Deduction
                                          join paa in db.Pay_DeductionAllocation
                                          on pa.DedId equals paa.DedId
                                          where pa.Status == true & paa.DeptId == id
                                          select paa.DedId).ToList();
                deductions.Where(x => allocationassigned.Contains(x.DedId)).ToList().ForEach(x => x.IsSelected = true);

                return deductions;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// If Allowance Id is Passed Greated Than 0 will be update all otherwise 0 Will a a new Allowance
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<bool> CreateUpdateDeductionAllocation(List<DeductionAllocationVM> lst, int typeid, int transacid, int UserId)
        {
            try
            {
                if (typeid != 0)
                {

                    if (typeid == 1)
                    {
                        var EmpAllowances = db.Pay_DeductionAllocation.Where(x => x.EmpId == transacid);
                        db.Pay_DeductionAllocation.RemoveRange(EmpAllowances);
                        db.SaveChanges();
                    }
                    else if (typeid == 2)
                    {
                        var DesgAllowances = db.Pay_DeductionAllocation.Where(x => x.DesgId == transacid);
                        db.Pay_DeductionAllocation.RemoveRange(DesgAllowances);
                        db.SaveChanges();
                    }
                    else if (typeid == 3)
                    {
                        var DeptAllowances = db.Pay_DeductionAllocation.Where(x => x.DeptId == transacid);
                        db.Pay_DeductionAllocation.RemoveRange(DeptAllowances);
                        db.SaveChanges();
                    }
                    foreach (var mods in lst)
                    {
                        if (mods.TypeId == 1)
                        {
                            Pay_DeductionAllocation tbl = new Pay_DeductionAllocation
                            {
                                DedId = mods.DedId,
                                DeptId = mods.DeptId,
                                DesgId = mods.DesgId,
                                EmpId = mods.EmpId,
                                TransDate = DateTime.Now,
                                AllocId = mods.AllocId,
                                UserId = UserId,
                                Status = true
                            };
                            db.Pay_DeductionAllocation.Add(tbl);
                            await db.SaveChangesAsync();
                        }
                        else if (mods.TypeId == 2)
                        {
                            Pay_DeductionAllocation tbl = new Pay_DeductionAllocation
                            {
                                DedId = mods.DedId,
                                DeptId = mods.DeptId,
                                DesgId = mods.DesgId,
                                EmpId = mods.EmpId,
                                TransDate = DateTime.Now,
                                AllocId = mods.AllocId,
                                UserId = UserId,
                                Status = true
                            };
                            db.Pay_DeductionAllocation.Add(tbl);
                            await db.SaveChangesAsync();
                        }
                        else if (mods.TypeId == 3)
                        {
                            Pay_DeductionAllocation tbl = new Pay_DeductionAllocation
                            {
                                DedId = mods.DedId,
                                DeptId = mods.DeptId,
                                DesgId = mods.DesgId,
                                EmpId = mods.EmpId,
                                TransDate = DateTime.Now,
                                AllocId = mods.AllocId,
                                UserId = UserId,
                                Status = true
                            };
                            db.Pay_DeductionAllocation.Add(tbl);
                            await db.SaveChangesAsync();
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Allowance Allocation
        #region Employee Leave

        public List<LeaveTypeVM> GetLeaveTypes(int LocId)
        {
            if (LocId != 72)
            {
                return db.Pay_LeaveTypes.Where(x => x.LeaveTypeId != "R").Select(x => new LeaveTypeVM
                {
                    LeaveType = x.LeaveType,
                    LeaveTypeId = x.LeaveTypeId,
                    DisplayOrder = x.DisplayOrder
                }).ToList();
            }
            return db.Pay_LeaveTypes.Select(x => new LeaveTypeVM
            {
                LeaveType = x.LeaveType,
                LeaveTypeId = x.LeaveTypeId,
                DisplayOrder = x.DisplayOrder
            }).ToList();
        }

        public List<LeaveCategoryVM> GetLeaveCategory()
        {
            return db.Pay_LeaveCategory.Select(x => new LeaveCategoryVM
            {
                LeaveCat = x.LeaveCat,
                LeaveCatId = x.LeaveCatId,
                DisplayOrder = x.DisplayOrder
            }).ToList();
        }

        /// <summary>
        /// List the EmployeeLeave
        /// </summary>
        /// <returns></returns>
        public List<EmployeeLeaveVM> EmployeeLeaveList(int UserId, int GroupId, DateTime FDate, DateTime TDate, int DesgId, int HDeptId, int DeptId, int EmpId)
        {
            try
            {
                var Emps = GetReportingToUsers(UserId).Select(x => x.EmpId).ToList();
                Emps.Add(UserId);
                if (GroupId == 1037 || GroupId == 10)
                {
                    return (from x in db.Pay_EmpLeave
                            join d in db.Pay_EmpMaster on x.EmpId equals d.EmpId
                            join s in db.Pay_Department on d.DeptId equals s.DeptId
                            join e in db.Pay_Designation on d.DesgId equals e.DesgId
                            where x.LeaveFromDate >= FDate.Date && x.LeaveToDate <= TDate.Date
                            && (e.DesgId == DesgId || DesgId == 0)
                            && (d.DeptId == DeptId || DeptId == 0)
                            && (s.HDeptId == HDeptId || HDeptId == 0)
                            && (x.EmpId == EmpId || EmpId == 0)
                            select new EmployeeLeaveVM
                            {
                                EmpId = x.EmpId,
                                LeaveStatus = x.LeaveStatus,
                                ApprovedBy = x.ApprovedBy,
                                ApprovedByHR = x.ApprovedByHR,
                                ApprovedDate = x.ApprovedDate,
                                ApprovedDateHR = x.ApprovedDateHR,
                                LeaveCatId = x.LeaveCatId,
                                EmpName = d.EmpName,
                                LeaveFromDate = x.LeaveFromDate,
                                LeaveId = x.LeaveId,
                                LeaveReason = x.LeaveReason,
                                LeaveToDate = x.LeaveToDate,
                                LeaveTypeId = x.LeaveTypeId,
                                Remarks = x.Remarks,
                                RemarksByHR = x.RemarksByHR,
                                TransDate = x.TransDate,
                                UserId = x.UserId,
                                Department = s.DeptName,
                                Designation = e.DesgName
                            }).ToList();
                }
                else
                {
                    return (from x in db.Pay_EmpLeave
                            join d in db.Pay_EmpMaster on x.EmpId equals d.EmpId
                            join s in db.Pay_Department on d.DeptId equals s.DeptId
                            join e in db.Pay_Designation on d.DesgId equals e.DesgId
                            where x.LeaveFromDate >= FDate.Date && x.LeaveToDate <= TDate.Date
                            && Emps.Contains(d.EmpId)
                            && (e.DesgId == DesgId || DesgId == 0)
                            && (d.DeptId == DeptId || DeptId == 0)
                            && (s.HDeptId == HDeptId || HDeptId == 0)
                            && (x.EmpId == EmpId || EmpId == 0)
                            select new EmployeeLeaveVM
                            {
                                EmpId = x.EmpId,
                                LeaveStatus = x.LeaveStatus,
                                ApprovedBy = x.ApprovedBy,
                                ApprovedByHR = x.ApprovedByHR,
                                ApprovedDate = x.ApprovedDate,
                                ApprovedDateHR = x.ApprovedDateHR,
                                LeaveCatId = x.LeaveCatId,
                                EmpName = d.EmpName,
                                LeaveFromDate = x.LeaveFromDate,
                                LeaveId = x.LeaveId,
                                LeaveReason = x.LeaveReason,
                                LeaveToDate = x.LeaveToDate,
                                LeaveTypeId = x.LeaveTypeId,
                                Remarks = x.Remarks,
                                RemarksByHR = x.RemarksByHR,
                                TransDate = x.TransDate,
                                UserId = x.UserId,
                                Department = s.DeptName,
                                Designation = e.DesgName
                            }).ToList();
                }

            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<EmployeeLeaveVM> BranchEmployeeLeaveList(int LocId, DateTime FDate, DateTime TDate)
        {
            try
            {

                return (from x in db.Pay_EmpLeave
                        join d in db.Pay_EmpMaster on x.EmpId equals d.EmpId
                        join s in db.Pay_Department on d.DeptId equals s.DeptId
                        join e in db.Pay_Designation on d.DesgId equals e.DesgId
                        where LocId == d.DeptId && (x.LeaveFromDate >= FDate.Date && x.LeaveToDate <= TDate.Date)
                        select new EmployeeLeaveVM
                        {
                            EmpId = x.EmpId,
                            LeaveStatus = x.LeaveStatus,
                            ApprovedBy = x.ApprovedBy,
                            ApprovedByHR = x.ApprovedByHR,
                            ApprovedDate = x.ApprovedDate,
                            ApprovedDateHR = x.ApprovedDateHR,
                            LeaveCatId = x.LeaveCatId,
                            EmpName = d.EmpName,
                            LeaveFromDate = x.LeaveFromDate,
                            LeaveId = x.LeaveId,
                            LeaveReason = x.LeaveReason,
                            LeaveToDate = x.LeaveToDate,
                            LeaveTypeId = x.LeaveTypeId,
                            Remarks = x.Remarks,
                            RemarksByHR = x.RemarksByHR,
                            TransDate = x.TransDate,
                            UserId = x.UserId,
                            Department = s.DeptName,
                            Designation = e.DesgName
                        }).ToList();

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<EmployeeLeaveVM> GetEmployeeLeave(long id)
        {
            var empleave = await (from x in db.Pay_EmpLeave
                                  join d in db.Pay_EmpMaster on x.EmpId equals d.EmpId
                                  join s in db.Pay_Department on d.DeptId equals s.DeptId
                                  join e in db.Pay_Designation on d.DesgId equals e.DesgId
                                  //join u in db.Users_Login on x.UserId equals u.em
                                  where x.LeaveId == id
                                  select new EmployeeLeaveVM
                                  {
                                      EmpId = x.EmpId,
                                      LeaveStatus = x.LeaveStatus,
                                      ApprovedBy = x.ApprovedBy,
                                      ApprovedByHR = x.ApprovedByHR,
                                      ApprovedDate = x.ApprovedDate,
                                      ApprovedDateHR = x.ApprovedDateHR,
                                      LeaveCatId = x.LeaveCatId,
                                      EmpName = d.EmpName,
                                      LeaveFromDate = x.LeaveFromDate,
                                      LeaveId = x.LeaveId,
                                      LeaveReason = x.LeaveReason,
                                      LeaveToDate = x.LeaveToDate,
                                      LeaveTypeId = x.LeaveTypeId,
                                      Remarks = x.Remarks,
                                      RemarksByHR = x.RemarksByHR,
                                      TransDate = x.TransDate,
                                      UserId = x.UserId,
                                      Department = s.DeptName,
                                      Designation = e.DesgName
                                  }).FirstOrDefaultAsync();
            if (empleave.ApprovedBy != null)
            {
                var empd = Convert.ToInt32(empleave.ApprovedBy);
                var Emps = await GetEmployeeById(empd);
                empleave.ApprovedByName = Emps.EmpName;
            }
            if (empleave.ApprovedByHR != null)
            {
                var empd = Convert.ToInt32(empleave.ApprovedByHR);
                var Emps = await GetEmployeeById(empd);
                empleave.ApprovedByHRName = Emps.EmpName;
            }
            return empleave;
        }


        public async Task<bool> MarkLeaveFinal(int EmpId, DateTime date, int UserId, string StatusId)
        {
            try
            {
                var emp = db.Pay_AttendanceFinal.Where(x => x.EmpId == EmpId
                && x.AttendanceDate.Month == date.Month
                && x.AttendanceDate.Day == date.Day
                && x.AttendanceDate.Year == date.Year).FirstOrDefault();

                if (emp == null)
                {
                    Pay_AttendanceFinal mod = new Pay_AttendanceFinal()
                    {
                        EmpId = EmpId,
                        AttendanceDate = date,
                        StatusId = StatusId,
                        UserId = UserId,
                        AttnType = "B",
                        Reason = "Import From EmpLeave",
                        TransDate = DateTime.Now,
                        WorkingHours = 0
                    };
                    db.Pay_AttendanceFinal.Add(mod);
                    await db.SaveChangesAsync();
                }
                else
                {
                    if (emp.StatusId == "A")
                    {
                        emp.StatusId = StatusId;
                        emp.TransDate = DateTime.Now;
                        emp.Reason = "Import From EmpLeave";
                        emp.AttnType = "B";
                        emp.UserId = UserId;
                        await db.SaveChangesAsync();
                    }

                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// If EmployeeLeave Id is Passed Greated Than 0 will be update all otherwise 0 Will a a new EmployeeLeave
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<long> CreateUpdateEmployeeLeave(EmployeeLeaveVM mod, int UserId)
        {
            try
            {

                if (mod.LeaveId > 0)
                {
                    var empleave = await db.Pay_EmpLeave.FindAsync(mod.LeaveId);

                    empleave.ApprovedBy = mod.ApprovedBy;
                    empleave.ApprovedByHR = mod.ApprovedByHR;
                    empleave.ApprovedDate = mod.ApprovedDate;
                    empleave.ApprovedDateHR = mod.ApprovedDateHR;
                    //empleave.EmpId = mod.EmpId;
                    //empleave.LeaveCatId = mod.LeaveCatId;
                    //empleave.LeaveFromDate = mod.LeaveFromDate;
                    //empleave.LeaveId = mod.LeaveId;
                    //empleave.LeaveReason = mod.LeaveReason;
                    empleave.LeaveStatus = mod.LeaveStatus;
                    //empleave.LeaveToDate = mod.LeaveToDate;
                    //empleave.LeaveTypeId = mod.LeaveTypeId;
                    empleave.Remarks = mod.Remarks;
                    empleave.RemarksByHR = mod.RemarksByHR;
                    empleave.TransDate = DateTime.Now;
                    empleave.UserId = mod.UserId;
                    await db.SaveChangesAsync();

                    if ((mod.LeaveCatId == "O" || mod.LeaveCatId == "N") && mod.LeaveStatus == "A" && mod.LeaveStatus != "R")
                    {
                        DateTime frmdte = mod.LeaveFromDate;
                        DateTime todte = mod.LeaveToDate;
                        while (frmdte <= todte)
                        {
                            await new EmployeeBL().MarkLeaveFinal(empleave.EmpId, frmdte, UserId, mod.LeaveTypeId);
                            frmdte = frmdte.AddDays(1);
                        }

                    }

                }
                else
                {
                    var graceprd = await db.Pay_LeaveTypes.FirstOrDefaultAsync();
                    int dys = Convert.ToInt32(graceprd.GraceDays);
                    var datelim = DateTime.Now.Date.AddDays(-dys);
                    if (mod.LeaveToDate.Date >= datelim.Date)
                    {

                        var IsLeaveAlreadyExist = db.Pay_EmpLeave.Where(x => x.LeaveFromDate >= mod.LeaveFromDate && x.LeaveToDate <= mod.LeaveToDate && x.EmpId == mod.EmpId).ToList();
                        if (IsLeaveAlreadyExist.Count() >= 1)
                        {
                            return 2;
                        }
                        Pay_EmpLeave tbl = new Pay_EmpLeave
                        {
                            ApprovedBy = mod.ApprovedBy,
                            ApprovedByHR = mod.ApprovedByHR,
                            EmpId = mod.EmpId,
                            ApprovedDate = mod.ApprovedDate,
                            ApprovedDateHR = mod.ApprovedDateHR,
                            LeaveCatId = mod.LeaveCatId,
                            LeaveFromDate = mod.LeaveFromDate,
                            LeaveReason = mod.LeaveReason,
                            LeaveStatus = mod.LeaveStatus,
                            LeaveToDate = mod.LeaveToDate,
                            LeaveTypeId = mod.LeaveTypeId,
                            Remarks = mod.Remarks,
                            RemarksByHR = mod.RemarksByHR,
                            TransDate = DateTime.Now,
                            UserId = mod.UserId
                        };
                        db.Pay_EmpLeave.Add(tbl);
                        await db.SaveChangesAsync();
                        mod.LeaveId = tbl.LeaveId;
                    }
                    else
                    {
                        return 3;
                    }
                }
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion Employee Leave
        #region Employee Leave Roster
        public List<LeaveRosterVM> GetLeavesRoster(int empid, DateTime startdate, DateTime enddate)
        {
            var Emps = GetReportingToUsers(empid);
            Emps.Add(new EmployeeVM() { EmpId = empid });
            List<LeaveRosterVM> _LeaveRoster = new List<LeaveRosterVM>();
            foreach (var item in Emps)
            {

                var _Lrv = (from ite in db.Pay_LeaveRoster
                            join emps in db.Pay_EmpMaster on ite.EmpId equals emps.EmpId
                            where ite.EmpId == item.EmpId && ite.LeaveDate >= startdate && ite.LeaveDate <= enddate && ite.Status == true
                            select new LeaveRosterVM()
                            {
                                EmpId = ite.EmpId,
                                Start = ite.LeaveDate,
                                End = ite.LeaveDate,
                                IsAllDay = true,
                                Title = emps.EmpName,
                                TransId = ite.TransId,
                                TransDate = ite.TransDate,
                                UserId = ite.UserId,
                                Color = "#f58a8a"
                            }).ToList();
                _LeaveRoster.AddRange(_Lrv);
                //if (_Lrv != null)
                //_LeaveRoster.Add(_Lrv);
                ////_LeaveRoster = db.Pay_LeaveRoster.Where(x => x.EmpId == item.EmpId && x.LeaveDate >= startdate && x.LeaveDate <= enddate).ToList().Select(x => new LeaveRosterVM()
                //{
                //    EmpId = x.EmpId,
                //    Start = x.LeaveDate,
                //    End = x.LeaveDate,
                //    IsAllDay = true,
                //    Title = "Leave",
                //    TransId = x.TransId,
                //    TransDate = x.TransDate,
                //    UserId = x.UserId,
                //    employees = new int[] { x.EmpId }
                //}).ToList();
            }
            return _LeaveRoster;
        }

        public List<EmployeeVM> GetReportingToUsers(int userid)
        {
            return db.Pay_EmpReportingHierarchy.Where(x => x.ReportingTo == userid)
                 .Select(x => new EmployeeVM()
                 {
                     EmpId = x.EmpId
                 }).ToList();
        }

        public bool CreateandUpdateLeavesRoster(LeaveRosterVM mod, int UserId)
        {
            try
            {
                if (mod.TransId > 0)
                {
                    var leave = db.Pay_LeaveRoster.Where(x => x.TransId == mod.TransId).FirstOrDefault();
                    leave.EmpId = mod.EmpId;
                    leave.LeaveDate = mod.Start;
                    leave.TransDate = DateTime.Now;
                    leave.UserId = UserId;
                    leave.Status = true;
                    db.SaveChanges();
                }
                else
                {
                    if (DateTime.Now.Date <= mod.Start)
                    {
                        var IsLeaveAlreadyExist = db.Pay_LeaveRoster.Where(x => x.LeaveDate == mod.Start && x.EmpId == mod.EmpId && x.Status == true).FirstOrDefault();
                        if (IsLeaveAlreadyExist == null)
                        {
                            Pay_LeaveRoster _LeaveRoster = new Pay_LeaveRoster()
                            {
                                EmpId = mod.EmpId,
                                LeaveDate = mod.Start,
                                UserId = UserId,
                                TransDate = DateTime.Now,
                                Status = true
                            };
                            db.Pay_LeaveRoster.Add(_LeaveRoster);
                            db.SaveChanges();
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
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool DeleteLeaveRoster(long leaveid)
        {
            try
            {
                var leaave = db.Pay_LeaveRoster.Where(x => x.TransId == leaveid && x.ApprovedBy == null).FirstOrDefault();
                if (leaave != null)
                {
                    leaave.Status = false;
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<List<LeaveRosterVM>> GetEmployeeLeaveList(int userid, int groupid)
        {
            var emps = GetReportingToUsers(userid);
            List<LeaveRosterVM> _empleaves = new List<LeaveRosterVM>();
            bool HasApprovalRight = await new SecurityBL().HasApprovalRight(userid, groupid, (int)RightMenuApproval.LeaveRosterApproval);
            emps.Add(new EmployeeVM() { EmpId = userid });
            foreach (var item in emps)
            {
                var empdat = await (from leave in db.Pay_LeaveRoster
                                    join employee in db.Pay_EmpMaster on leave.EmpId equals employee.EmpId
                                    where leave.EmpId == item.EmpId && leave.Status == true && leave.ApprovedBy == null
                                    select new LeaveRosterVM()
                                    {
                                        EmpId = leave.EmpId,
                                        empname = employee.EmpName,
                                        TransDate = leave.TransDate,
                                        ApprovedBy = leave.ApprovedBy,
                                        ApprovedDate = leave.ApprovedDate,
                                        HasApprovalRights = HasApprovalRight,
                                        TransId = leave.TransId

                                    }).ToListAsync();
                _empleaves.AddRange(empdat);
            }
            return _empleaves;
        }

        #endregion
        #region Employee Role Setup

        #endregion
        #region Empoyee Location Mapping
        public async Task<List<EmployeeGeoLocationMappingVM>> EmployeeLocationMappingList(int Designation)
        {
            try
            {
                var loc = await (from M in db.Comp_LocationsMapping
                                 join L in db.Comp_GeoLocation on M.GeoId equals L.GeoId
                                 where M.MappingStatus
                                 select new { M.EmpId, M.GeoId, L.GTitle }).ToListAsync();

                var emp = await (from E in db.Pay_EmpMaster
                                 where E.StatusId == "A" && (E.DesgId == Designation || Designation == 0)
                                 select new
                                 {
                                     E.EmpId,
                                     E.EmpName,
                                     E.CNIC
                                 }).ToListAsync();

                var data = (from E in emp
                            select new EmployeeGeoLocationMappingVM()
                            {
                                EmpId = E.EmpId,
                                EmployeeName = E.EmpName,
                                CNIC = E.CNIC,
                                LocationName = loc.Where(e => e.EmpId == E.EmpId).Select(x => x.GTitle).FirstOrDefault() ?? "",
                                GeoId = loc.Where(e => e.EmpId == E.EmpId).Select(x => x.GeoId).FirstOrDefault()
                            }).ToList();
                return data;

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> CreateUpdateEmployeeLocationMapping(EmployeeGeoLocationMappingVM mod, int UserId, int empid)
        {
            try
            {
                var trans = db.Comp_LocationsMapping.Where(X => X.EmpId == mod.EmpId).FirstOrDefault();
                if (trans == null)
                {
                    Comp_LocationsMapping obj = new Comp_LocationsMapping
                    {
                        GeoId = mod.GeoId,
                        EmpId = empid,
                        MappingStatus = true,
                        MappedBy = UserId,
                        MappedDate = DateTime.Now
                    };
                    db.Comp_LocationsMapping.Add(obj);
                    await db.SaveChangesAsync();
                }
                else
                {
                    if (trans.GeoId != mod.GeoId)
                    {
                        trans.MappingStatus = false;
                        Comp_LocationsMapping obj = new Comp_LocationsMapping
                        {
                            GeoId = mod.GeoId,
                            EmpId = empid,
                            MappingStatus = true,
                            MappedBy = UserId,
                            MappedDate = DateTime.Now
                        };
                        db.Comp_LocationsMapping.Add(obj);
                        await db.SaveChangesAsync();
                    }
                }
                //var trans = db.Pay_EmpLocationMapping.Where(X => X.EmpId == mod.EmpId).ToList();
                ////Items which are removed by the User
                //var items = trans.Where(p => mod.LocId == null || !mod.LocId.Any(p2 => p.LocId == p2 && p.EmpId == empid)).ToList();
                //if (items != null)
                //{
                //    foreach (var emps in items)
                //    {
                //        emps.ModifiedBy = UserId;
                //        emps.ModifiedDate = DateTime.Now;
                //        emps.Status = false;
                //        await db.SaveChangesAsync();
                //    }
                //}

                //foreach (var item in mod.LocId)
                //{
                //    var emplocexist = db.Pay_EmpLocationMapping.Where(x => x.LocId == item
                //    && x.EmpId == mod.EmpId).FirstOrDefault();
                //    if (emplocexist == null)
                //    {
                //        Pay_EmpLocationMapping _EmpLoc = new Pay_EmpLocationMapping
                //        {
                //            DefinedBy = UserId,
                //            DefinedDate = DateTime.Now,
                //            EmpId = mod.EmpId,
                //            LocId = item,
                //            Status = true
                //        };
                //        db.Pay_EmpLocationMapping.Add(_EmpLoc);
                //        await db.SaveChangesAsync();

                //    }
                //    else
                //    {
                //        emplocexist.Status = true;
                //        emplocexist.ModifiedBy = UserId;
                //        emplocexist.ModifiedDate = DateTime.Now;
                //        await db.SaveChangesAsync();
                //    }
                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<List<GeoLocationVM>> GeoParentList(int GLevel)
        {
            string ParentGlvl = (GLevel - 1).ToString();
            return await db.Comp_GeoLocation.Where(x => x.Status == 1 && x.GLevel == ParentGlvl).Select(x =>
                  new GeoLocationVM
                  {
                      GeoId = x.GeoId,
                      GTitle = x.GTitle
                  }).ToListAsync();
        }
        public async Task<EmployeeVM> GetEmployeeDetailById(int EmpId)
        {
            var emp = await db.Pay_EmpMaster.Where(x => x.EmpId == EmpId).FirstOrDefaultAsync();
            if (emp != null)
            {
                return new EmployeeVM()
                {
                    DOJ = emp.DOJ,
                    DesgName = emp.Pay_Designation.DesgName,
                    DeptName = emp.Pay_Department.DeptName,
                    DeptId = emp.DeptId,
                    DesgId = emp.DesgId,
                    CurrentSalary = await GetEmployeeBasicSalary(emp.EmpId)
                };
            }
            else
            {
                return null;
            }

        }
        public async Task<List<SEmployeeVM>> GetEmpployeeByTypeId(int id, int typeid, int hid)
        {

            if (typeid == 3)
            {
                return await db.Pay_EmpMaster.Where(x => x.DeptId == id && x.StatusId == "A").Select(x =>
                    new SEmployeeVM()
                    {
                        EmpId = x.EmpId,
                        EmpName = x.EmpName
                    }).ToListAsync();
            }
            else if (typeid == 2)
            {
                return await db.Pay_EmpMaster.Where(x => x.DesgId == id && x.StatusId == "A" && x.Pay_Department.HDeptId == hid).Select(x =>
                    new SEmployeeVM()
                    {
                        EmpId = x.EmpId,
                        EmpName = x.EmpName,
                        DeptName = x.Pay_Department.DeptName,
                        DesgName = x.Pay_Designation.DesgName

                    }).ToListAsync();
            }
            else
            {
                return await db.Pay_EmpMaster.Where(x => x.StatusId == "A").Select(x =>
                      new SEmployeeVM()
                      {
                          EmpId = x.EmpId,
                          EmpName = x.EmpName
                      }).ToListAsync();
            }

        }
        #endregion
        #region Employee Loan

        /// <summary>
        /// List the EmployeeLoanList
        /// </summary>
        /// <returns></returns>
        public List<LoanTypesVM> GetLoanTypes()
        {
            try
            {
                return db.Pay_LoanTypes.Select(x => new LoanTypesVM()
                {
                    LoanType = x.LoanType,
                    LoanTypeId = x.LoanTypeId
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// List the EmployeeLoanList
        /// </summary>
        /// <returns></returns>
        public List<EmployeeLoanListVM> EmployeeLoanList()
        {
            try
            {
                return (from item in db.Pay_EmpLoan
                        join loantype in db.Pay_LoanTypes on item.LoanTypeID equals loantype.LoanTypeId
                        join user in db.Pay_EmpMaster on item.EmpId equals user.EmpId into ps
                        join user in db.Pay_EmpMaster on item.ApprovedBy equals user.EmpId into em
                        from empmast in ps.DefaultIfEmpty()
                        from appmas in em.DefaultIfEmpty()
                        where item.Status == true
                        select new EmployeeLoanListVM
                        {
                            LoanTypeID = item.LoanTypeID,
                            LoanType = loantype.LoanType,
                            Inst = item.Inst,
                            Status = item.Status,
                            EmpName = empmast.EmpName,
                            CNIC = empmast.CNIC,
                            EmpId = item.EmpId,
                            LoanAmt = item.LoanAmt,
                            AutoDeduct = item.AutoDeduct,
                            IssueDate = item.IssueDate,
                            DedStartDate = item.DedStartDate,
                            LoanId = item.LoanId,
                            ApproveBy = appmas.EmpName,
                            ApprovedBy = appmas.ApprovedBy,
                            Approve = item.ApprovedBy == null ? false : true,
                            LoanBal = item.LoanBal,
                            Remarks = item.Remarks,
                            AdvTypeID = item.AdvTypeID,
                            AdvType = item.AdvTypeID == 1 ? "Employee Advance" : "Closing Advance"
                        }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// If EmployeeLoan Id is Passed Greated Than 0 will be update all otherwise 0 Will a a new EmployeeLoan
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<EmployeeLoanListVM> CreateUpdateEmployeeLoan(EmployeeLoanListVM mod, int UserId)
        {
            try
            {
                if (mod.LoanId > 0)
                {
                    var emploan = await db.Pay_EmpLoan.FindAsync(mod.LoanId);
                    if (mod.Approve == true && emploan.ApprovedBy == null)
                    {
                        emploan.ApprovedBy = UserId;
                        emploan.Remarks = mod.Remarks;
                        emploan.ApprovedDate = DateTime.Now;
                    }
                    //emploan.LoanAmt = mod.LoanAmt;
                    emploan.Inst = mod.Inst;
                    //emploan.LoanBal = mod.LoanAmt;
                    emploan.Remarks = mod.Remarks;
                    emploan.DedStartDate = mod.DedStartDate;
                    emploan.AutoDeduct = mod.AutoDeduct;
                    emploan.AdvTypeID = mod.AdvTypeID;

                    await db.SaveChangesAsync();
                }
                else
                {
                    Pay_EmpLoan tbl = new Pay_EmpLoan()
                    {
                        LoanTypeID = mod.LoanTypeID,
                        AutoDeduct = mod.AutoDeduct,
                        DedStartDate = mod.DedStartDate,
                        EmpId = mod.EmpId,
                        Inst = mod.Inst,
                        IssueDate = mod.IssueDate,
                        Status = true,
                        Remarks = mod.Remarks,
                        LoanBal = mod.LoanAmt,
                        TransDate = DateTime.Now,
                        LoanAmt = mod.LoanAmt,
                        UserId = UserId,
                        AdvTypeID = mod.AdvTypeID
                    };
                    db.Pay_EmpLoan.Add(tbl);

                    await db.SaveChangesAsync();
                    mod.LoanId = tbl.LoanId;
                    var emp = await GetEmployeeById(mod.EmpId);
                    mod.EmpName = emp.EmpName;
                    mod.CNIC = emp.CNIC;
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Soft Delete - Update the Status to false in EmployeeLoan
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<bool> DestroyEmployeeLoan(EmployeeLoanListVM mod)
        {
            try
            {
                var tbl = await db.Pay_EmpLoan.FindAsync(mod.LoanId);
                tbl.Status = false;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
        #region Employee Fine
        public async Task<List<EmpFineVM>> GetEmployeeFineList(int userid, int groupid, int menuid)
        {
            bool HasApprovalRight = await new SecurityBL().HasApprovalRight(userid, groupid, menuid);
            var empfnelst = (from empfnemaster in db.Pay_EmpFine
                             join users in db.Users_Login on empfnemaster.ApprovedBy equals users.UserID into ps
                             from p in ps.DefaultIfEmpty()
                                 //join empfnsmasterdtl in db.Pay_EmpFineDetail on empfnemaster.DocId equals empfnsmasterdtl.DocId
                                 //join empmaster in db.s on empfnsmasterdtl.EmpId equals empmaster.EmpId
                             select new EmpFineVM()
                             {
                                 DocId = empfnemaster.DocId,
                                 ApprovedBy = empfnemaster.ApprovedBy,
                                 ApprovedDate = empfnemaster.ApprovedDate,
                                 Remarks = empfnemaster.Remarks,
                                 DocDate = empfnemaster.DocDate,
                                 FineDate = empfnemaster.FineDate,
                                 CreatedBy = empfnemaster.CheckedBy,
                                 CreatedDate = empfnemaster.CreatedDate,
                                 ApprovedName = p.FullName,
                                 HasLeaveApprovalRights = HasApprovalRight
                             }).ToList();
            return empfnelst;
        }

        public List<EmpFineDetailVM> GetEmployeeFineListByDocId(int docid)
        {
            var empfnelst = (from empdet in db.Pay_EmpFineDetail
                             join emp in db.Pay_EmpMaster on empdet.EmpId equals emp.EmpId
                             //join empfnsmasterdtl in db.Pay_EmpFineDetail on empfnemaster.DocId equals empfnsmasterdtl.DocId
                             //join empmaster in db.s on empfnsmasterdtl.EmpId equals empmaster.EmpId
                             where empdet.DocId == docid
                             select new EmpFineDetailVM()
                             {
                                 DocDtlId = empdet.DocDtlId,
                                 DocId = empdet.DocId,
                                 EmpId = empdet.EmpId,
                                 EmployeeName = emp.EmpName,
                                 FineActual = empdet.FineActual,
                                 FineApproved = empdet.FineApproved,
                                 FineTypeId = empdet.FineTypeId,
                                 Remarks = empdet.Remarks
                             }).ToList();
            return empfnelst;
        }

        public async Task<List<FineTypesVM>> GetFineTypes()
        {
            try
            {
                return await db.Pay_EmpFineType.Select(x => new FineTypesVM()
                {
                    FineType = x.FineType,
                    FineTypeId = x.FineTypeId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> CreateUpdateEmployeeFine(EmpFineVM mod, int UserId)
        {
            try
            {
                if (mod.DocId > 0)
                {
                    var empfine = await db.Pay_EmpFine.FindAsync(mod.DocId);
                    if (mod.Approve == true && empfine.ApprovedBy == null)
                    {
                        empfine.ApprovedBy = UserId;
                        empfine.ApprovedDate = DateTime.Now;
                    }
                    empfine.Remarks = mod.Remarks;
                    foreach (var item in mod.EmpFineDetail)
                    {
                        var empfnddtl = await db.Pay_EmpFineDetail.FindAsync(item.DocDtlId);
                        empfnddtl.FineActual = item.FineActual;
                        empfnddtl.FineTypeId = item.FineTypeId;
                        empfnddtl.Remarks = item.Remarks;
                        empfnddtl.EmpId = item.EmpId;
                        if (mod.Approve == true && empfine.ApprovedBy == null)
                        {
                            empfnddtl.FineApproved = item.FineApproved;
                        }
                        else if (mod.Approve == false && empfine.ApprovedBy == null)
                        {
                            empfnddtl.FineApproved = 0;
                        }
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    Pay_EmpFine tbl = new Pay_EmpFine()
                    {
                        CreatedDate = DateTime.Now,
                        CreatedBy = UserId,
                        DocDate = DateTime.Now,
                        FineDate = mod.FineDate,
                        Remarks = mod.Remarks
                    };
                    db.Pay_EmpFine.Add(tbl);
                    await db.SaveChangesAsync();

                    foreach (var item in mod.EmpFineDetail)
                    {
                        Pay_EmpFineDetail EmpFneDetail = new Pay_EmpFineDetail()
                        {
                            EmpId = item.EmpId,
                            Remarks = item.Remarks,
                            FineTypeId = item.FineTypeId,
                            FineActual = item.FineActual,
                            DocId = tbl.DocId
                        };
                        db.Pay_EmpFineDetail.Add(EmpFneDetail);
                        await db.SaveChangesAsync();
                    }
                    mod.DocId = tbl.DocId;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<bool> ApproveDoc(int docid, int userid)
        {
            try
            {
                var empfine = await db.Pay_EmpFine.FindAsync(docid);
                empfine.ApprovedBy = userid;
                empfine.ApprovedDate = DateTime.Now;
                await db.SaveChangesAsync();

                var empdetfne = await db.Pay_EmpFineDetail.Where(x => x.DocId == docid).ToListAsync();
                foreach (var item in empdetfne)
                {
                    item.FineApproved = item.FineActual;
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<bool> EmpLeaveRosterApprove(int transid, int userid)
        {
            try
            {
                var empfine = await db.Pay_LeaveRoster.FindAsync(transid);
                empfine.ApprovedBy = userid;
                empfine.ApprovedDate = DateTime.Now;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> CreateUpdateEmployeeDetailFine(List<EmpFineDetailVM> mod)
        {
            try
            {
                foreach (var item in mod)
                {
                    if (item.DocDtlId > 0)
                    {
                        var empfine = await db.Pay_EmpFineDetail.FindAsync(item.DocDtlId);
                        empfine.EmpId = item.EmpId;
                        empfine.FineActual = item.FineActual;
                        empfine.FineTypeId = item.FineTypeId;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        Pay_EmpFineDetail tbl = new Pay_EmpFineDetail()
                        {
                            DocId = item.DocId,
                            EmpId = item.EmpId,
                            FineActual = item.FineActual,
                            FineTypeId = item.FineTypeId,
                            Remarks = item.Remarks
                        };
                        db.Pay_EmpFineDetail.Add(tbl);
                        await db.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DestroyEmployeeFineDetail(long id)
        {
            if (id > 0)
            {
                try
                {
                    var emp = db.Pay_EmpFineDetail.Where(x => x.DocDtlId == id).FirstOrDefault();
                    db.Pay_EmpFineDetail.Remove(emp);
                    await db.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        #endregion
        #region Employee Salary
        public bool IsBasicSalaryExist(int id)
        {
            var bs = db.Pay_EmpSalary.Where(x => x.EmpId == id && x.SalTypeId == "B").FirstOrDefault();
            if (bs != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<EmployeeVM>> GetEmpployeeByTypeId(int DeptId, int DesgId, DateTime? FromDate, DateTime? ToDate)
        {
            List<int> ExDesg = new List<int>() { 192, 221, 282, 379 };
            if (FromDate == null || ToDate == null)
            {
                var emp = await db.Pay_EmpMaster.Where(x => x.StatusId == "A" && (DeptId == 0 || x.DeptId == DeptId) && (DesgId == 0 || x.DesgId == DesgId) && !ExDesg.Contains(x.DesgId)).Select(x =>
                          new EmployeeVM()
                          {
                              EmpId = x.EmpId,
                              EmpName = x.EmpName,
                              DeptId = x.DeptId,
                              DesgId = x.DesgId,
                              DesgName = x.Pay_Designation.DesgName,
                              DeptName = x.Pay_Department.DeptName,
                              CNIC = x.CNIC
                          }).ToListAsync();
                foreach (var item in emp)
                {
                    item.BasicSalary = await GetEmployeeBasicSalary(item.EmpId);
                }
                return emp;
            }
            else
            {
                var emp = await db.Pay_EmpMaster.Where(x => x.DOJ >= FromDate && x.DOJ <= ToDate && x.StatusId == "A" && (DeptId == 0 || x.DeptId == DeptId) && (DesgId == 0 || x.DesgId == DesgId) && !ExDesg.Contains(x.DesgId)).Select(x =>
                           new EmployeeVM()
                           {
                               EmpId = x.EmpId,
                               EmpName = x.EmpName,
                               DeptId = x.DeptId,
                               DesgId = x.DesgId,
                               CNIC = x.CNIC,
                               DesgName = x.Pay_Designation.DesgName,
                               DeptName = x.Pay_Department.DeptName
                           }).ToListAsync();
                foreach (var item in emp)
                {
                    item.BasicSalary = await GetEmployeeBasicSalary(item.EmpId);
                }
                return emp;
            }
        }

        public async Task<bool> CreateEmployeeBasicSalary(EmployeeCalculatedSalaryVM mod, int UserId)
        {
            try
            {
                Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
                {
                    EmpId = mod.EmployeeId,
                    EffectiveFrom = mod.EffectiveDate,
                    Remarks = mod.Remarks,
                    UserId = UserId,
                    Salary = mod.GrossSalary,
                    SalTypeId = "B",
                    TransDate = DateTime.Now
                };
                db.Pay_EmpSalary.Add(_EmployeeSalary);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> CreateEmployeeSalary(List<EmployeeCalculatedSalaryVM> mod, int UserId, string Remarks, DateTime EffectiveDate)
        {
            try
            {

                foreach (var item in mod)
                {
                    var empbs = await db.Pay_EmpSalary.Where(x => x.EmpId == item.EmployeeId && x.SalTypeId == "B").FirstOrDefaultAsync();
                    if (item.EmployeeId > 0)
                    {
                        if (item.SalaryType == "B" && empbs == null)
                        {
                            Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
                            {
                                EmpId = item.EmployeeId,
                                EffectiveFrom = EffectiveDate,
                                Remarks = Remarks,
                                UserId = UserId,
                                Salary = item.GrossSalary,
                                SalTypeId = item.SalaryType,
                                TransDate = DateTime.Now
                            };
                            db.Pay_EmpSalary.Add(_EmployeeSalary);
                            await db.SaveChangesAsync();
                        }
                        else if (item.SalaryType == "I" && empbs != null)
                        {

                            Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
                            {
                                EmpId = item.EmployeeId,
                                EffectiveFrom = EffectiveDate,
                                Remarks = Remarks,
                                UserId = UserId,
                                Salary = item.IncrementAmount,
                                SalTypeId = item.SalaryType,
                                TransDate = DateTime.Now
                            };
                            db.Pay_EmpSalary.Add(_EmployeeSalary);
                            await db.SaveChangesAsync();
                        }
                        else if (item.SalaryType == "D" && empbs != null)
                        {
                            Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
                            {
                                EmpId = item.EmployeeId,
                                EffectiveFrom = EffectiveDate,
                                Remarks = Remarks,
                                UserId = UserId,
                                Salary = item.IncrementAmount,
                                SalTypeId = item.SalaryType,
                                TransDate = DateTime.Now
                            };
                            db.Pay_EmpSalary.Add(_EmployeeSalary);
                            await db.SaveChangesAsync();
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


        //public async Task<bool> CreateEmployeeSalary(List<EmployeeCalculatedSalaryVM> mod, int UserId)
        //{
        //    try
        //    {
        //        foreach (var item in mod)
        //        {
        //            if (item.EmployeeId > 0)
        //            {
        //                if (item.SalaryType == "B")
        //                {
        //                    Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
        //                    {
        //                        EmpId = item.EmployeeId,
        //                        EffectiveFrom = item.EffectiveDate,
        //                        Remarks = item.Remarks,
        //                        UserId = UserId,
        //                        Salary = item.GrossSalary,
        //                        SalTypeId = item.SalaryType,
        //                        TransDate = DateTime.Now
        //                    };
        //                    db.Pay_EmpSalary.Add(_EmployeeSalary);
        //                    await db.SaveChangesAsync();
        //                }
        //                else if (item.SalaryType == "I")
        //                {
        //                    Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
        //                    {
        //                        EmpId = item.EmployeeId,
        //                        EffectiveFrom = item.EffectiveDate,
        //                        Remarks = item.Remarks,
        //                        UserId = UserId,
        //                        Salary = item.IncrementAmount,
        //                        SalTypeId = item.SalaryType,
        //                        TransDate = DateTime.Now
        //                    };
        //                    db.Pay_EmpSalary.Add(_EmployeeSalary);
        //                    await db.SaveChangesAsync();
        //                }
        //                else if (item.SalaryType == "D")
        //                {
        //                    Pay_EmpSalary _EmployeeSalary = new Pay_EmpSalary()
        //                    {
        //                        EmpId = item.EmployeeId,
        //                        EffectiveFrom = item.EffectiveDate,
        //                        Remarks = item.Remarks,
        //                        UserId = UserId,
        //                        Salary = item.IncrementAmount,
        //                        SalTypeId = item.SalaryType,
        //                        TransDate = DateTime.Now
        //                    };
        //                    db.Pay_EmpSalary.Add(_EmployeeSalary);
        //                    await db.SaveChangesAsync();
        //                }
        //            }

        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public async Task<List<SalaryTypeVM>> GetSalaryTypes(int id)
        {
            //var _SalaryTypes = await (from item in db.Pay_EmpSalary
            //                    join emp in db.Pay_EmpMaster on item.EmpId equals emp.EmpId
            //                    join saltype in db.Pay_SalaryTypes on item.SalTypeId equals saltype.SalTypeId
            //                    where emp.EmpId == id
            //                    select new SalaryTypeVM { SalTypeId = saltype.SalTypeId, SalTypeDesc = saltype.SalTypeDesc }).ToListAsync();
            return await db.Pay_SalaryTypes.Where(x => x.Status).Select(x => new SalaryTypeVM()
            {
                SalTypeId = x.SalTypeId,
                SalTypeDesc = x.SalTypeDesc
            }).ToListAsync();
        }

        public async Task<List<EmployeeCalculatedSalaryVM>> CalculateEmployeesSalary(List<EmployeeVM> mod, string SalaryType, bool IsPercentage, bool IsAmount)
        {
            List<EmployeeCalculatedSalaryVM> _Salary = new List<EmployeeCalculatedSalaryVM>();

            foreach (var item in mod)
            {
                var MasSal = await db.Pay_EmpSalary.Where(x => x.EmpId == item.EmpId).ToListAsync();
                var BSal = MasSal.Where(x => x.EmpId == item.EmpId && x.SalTypeId == "B").FirstOrDefault();
                var EISal = MasSal.Where(x => x.EmpId == item.EmpId && x.SalTypeId == "I").ToList();
                var EDSal = MasSal.Where(x => x.EmpId == item.EmpId && x.SalTypeId == "D").ToList();
                var EmpDetail = await db.Pay_EmpMaster.Where(x => x.EmpId == item.EmpId).FirstOrDefaultAsync();
                if (SalaryType == "B")
                {
                    if (BSal == null)
                    {
                        EmployeeCalculatedSalaryVM _SalaryObj = new EmployeeCalculatedSalaryVM();
                        _SalaryObj.EmployeeId = item.EmpId;
                        _SalaryObj.id = 0;
                        _SalaryObj.EmployeeName = EmpDetail.EmpName;
                        _SalaryObj.Department = EmpDetail.Pay_Department.DeptName;
                        _SalaryObj.CNIC = EmpDetail.CNIC;
                        _SalaryObj.BasicSalary = item.NewAmount;
                        _SalaryObj.IncrementAmount = 0;
                        _SalaryObj.GrossSalary = item.NewAmount;
                        _SalaryObj.SalaryType = SalaryType;
                        _Salary.Add(_SalaryObj);
                    }
                    else
                    {
                        return new List<EmployeeCalculatedSalaryVM>();
                    }
                }
                else if (SalaryType == "I")
                {
                    EmployeeCalculatedSalaryVM _SalaryObj = new EmployeeCalculatedSalaryVM();
                    _SalaryObj.EmployeeId = item.EmpId;
                    _SalaryObj.id = 0;
                    _SalaryObj.EmployeeName = EmpDetail.EmpName;
                    _SalaryObj.Department = EmpDetail.Pay_Department.DeptName;
                    _SalaryObj.CNIC = EmpDetail.CNIC;
                    if (IsPercentage == true)
                    {
                        var basicsal = BSal;
                        var existsalinc = EISal;
                        var existsaldec = EDSal;
                        if (basicsal != null)
                        {
                            var BS = basicsal.Salary + ((existsalinc.Count > 0 ? existsalinc.Sum(x => x.Salary) : 0) - (existsaldec.Count > 0 ? existsaldec.Sum(x => x.Salary) : 0));
                            var incrementamount = BS * (item.NewAmount / 100);
                            _SalaryObj.BasicSalary = BS;
                            _SalaryObj.IncrementAmount = incrementamount;
                            _SalaryObj.GrossSalary = _SalaryObj.BasicSalary + _SalaryObj.IncrementAmount;
                            _SalaryObj.SalaryType = SalaryType;

                        }
                    }
                    else if (IsAmount == true)
                    {
                        var basicsal = BSal;
                        var existsalinc = EISal;
                        var existsaldec = EDSal;
                        if (basicsal != null)
                        {
                            var BS = basicsal.Salary + ((existsalinc.Count > 0 ? existsalinc.Sum(x => x.Salary) : 0) - (existsaldec.Count > 0 ? existsaldec.Sum(x => x.Salary) : 0));
                            _SalaryObj.BasicSalary = BS;
                            _SalaryObj.IncrementAmount = item.NewAmount;
                            _SalaryObj.GrossSalary = _SalaryObj.BasicSalary + _SalaryObj.IncrementAmount;
                            _SalaryObj.SalaryType = SalaryType;

                        }
                    }
                    _Salary.Add(_SalaryObj);
                }
                else if (SalaryType == "D")
                {
                    EmployeeCalculatedSalaryVM _SalaryObj = new EmployeeCalculatedSalaryVM();
                    _SalaryObj.EmployeeId = item.EmpId;
                    _SalaryObj.id = 0;
                    _SalaryObj.EmployeeName = EmpDetail.EmpName;
                    _SalaryObj.Department = EmpDetail.Pay_Department.DeptName;
                    _SalaryObj.CNIC = EmpDetail.CNIC;
                    var basicsal = BSal;
                    var existsalinc = EISal;
                    var existsaldec = EDSal;
                    if (basicsal != null)
                    {
                        decimal exstincsal = existsalinc.Count > 0 ? existsalinc.Sum(x => x.Salary) : 0;
                        decimal exstdecsal = existsaldec.Count > 0 ? existsaldec.Sum(x => x.Salary) : 0;
                        var BS = (basicsal.Salary + exstincsal) - exstdecsal;
                        _SalaryObj.BasicSalary = BS;
                        _SalaryObj.IncrementAmount = item.NewAmount;
                        _SalaryObj.GrossSalary = _SalaryObj.BasicSalary - _SalaryObj.IncrementAmount;
                        _SalaryObj.SalaryType = SalaryType;

                    }
                    //var existsal = db.Pay_EmpSalary.Where(x => x.EmpId == item).FirstOrDefault();
                    //if (existsal != null)
                    //{
                    //    _SalaryObj.BasicSalary = existsal.Salary;
                    //    _SalaryObj.IncrementAmount = mod.Amount;
                    //    _SalaryObj.GrossSalary = existsal.Salary - _SalaryObj.IncrementAmount;
                    //}
                    _Salary.Add(_SalaryObj);
                }
            }
            return _Salary;
        }
        //public List<EmployeeCalculatedSalaryVM> CalculateEmployeesSalary(EmployeeSalaryVM mod)
        //{
        //    List<EmployeeCalculatedSalaryVM> _Salary = new List<EmployeeCalculatedSalaryVM>();

        //    foreach (var item in mod.EmployeeId)
        //    {
        //        var MasSal = db.Pay_EmpSalary.Where(x => x.EmpId == item).ToList();
        //        var BSal = MasSal.Where(x => x.EmpId == item && x.SalTypeId == "B").FirstOrDefault();
        //        var EISal = MasSal.Where(x => x.EmpId == item && x.SalTypeId == "I").ToList();
        //        var EDSal = MasSal.Where(x => x.EmpId == item && x.SalTypeId == "D").ToList();
        //        var EmpDetail = db.Pay_EmpMaster.Where(x => x.EmpId == item).FirstOrDefault();
        //        if (mod.SalaryType == "B")
        //        {
        //            if (BSal == null)
        //            {
        //                EmployeeCalculatedSalaryVM _SalaryObj = new EmployeeCalculatedSalaryVM();
        //                _SalaryObj.EmployeeId = item;
        //                _SalaryObj.id = 0;
        //                _SalaryObj.EmployeeName = EmpDetail.EmpName;
        //                _SalaryObj.Department = EmpDetail.Pay_Department.DeptName;
        //                _SalaryObj.CNIC = EmpDetail.CNIC;
        //                _SalaryObj.BasicSalary = mod.Amount;
        //                _SalaryObj.IncrementAmount = 0;
        //                _SalaryObj.GrossSalary = mod.Amount;
        //                _Salary.Add(_SalaryObj);
        //            }
        //            else
        //            {
        //                return new List<EmployeeCalculatedSalaryVM>();
        //            }
        //        }
        //        else if (mod.SalaryType == "I")
        //        {
        //            EmployeeCalculatedSalaryVM _SalaryObj = new EmployeeCalculatedSalaryVM();
        //            _SalaryObj.EmployeeId = item;
        //            _SalaryObj.id = 0;
        //            _SalaryObj.EmployeeName = EmpDetail.EmpName;
        //            _SalaryObj.Department = EmpDetail.Pay_Department.DeptName;
        //            _SalaryObj.CNIC = EmpDetail.CNIC;
        //            if (mod.IsPercentage == true)
        //            {
        //                var basicsal = BSal;
        //                var existsalinc = EISal;
        //                var existsaldec = EDSal;
        //                if (basicsal != null)
        //                {
        //                    var BS = basicsal.Salary + ((existsalinc.Count > 0 ? existsalinc.Sum(x => x.Salary) : 0) - (existsaldec.Count > 0 ? existsaldec.Sum(x => x.Salary) : 0));
        //                    var incrementamount = BS * (mod.Amount / 100);
        //                    _SalaryObj.BasicSalary = BS;
        //                    _SalaryObj.IncrementAmount = incrementamount;
        //                    _SalaryObj.GrossSalary = _SalaryObj.BasicSalary + _SalaryObj.IncrementAmount;
        //                }
        //            }
        //            else if (mod.IsAmount == true)
        //            {
        //                var basicsal = BSal;
        //                var existsalinc = EISal;
        //                var existsaldec = EDSal;
        //                if (basicsal != null)
        //                {
        //                    var BS = basicsal.Salary + ((existsalinc.Count > 0 ? existsalinc.Sum(x => x.Salary) : 0) - (existsaldec.Count > 0 ? existsaldec.Sum(x => x.Salary) : 0));
        //                    _SalaryObj.BasicSalary = BS;
        //                    _SalaryObj.IncrementAmount = mod.Amount;
        //                    _SalaryObj.GrossSalary = _SalaryObj.BasicSalary + _SalaryObj.IncrementAmount;
        //                }
        //            }
        //            _Salary.Add(_SalaryObj);
        //        }
        //        else if (mod.SalaryType == "D")
        //        {
        //            EmployeeCalculatedSalaryVM _SalaryObj = new EmployeeCalculatedSalaryVM();
        //            _SalaryObj.EmployeeId = item;
        //            _SalaryObj.id = 0;
        //            _SalaryObj.EmployeeName = EmpDetail.EmpName;
        //            _SalaryObj.Department = EmpDetail.Pay_Department.DeptName;
        //            _SalaryObj.CNIC = EmpDetail.CNIC;
        //            var basicsal = BSal;
        //            var existsalinc = EISal;
        //            var existsaldec = EDSal;
        //            if (basicsal != null)
        //            {
        //                decimal exstincsal = existsalinc.Count > 0 ? existsalinc.Sum(x => x.Salary) : 0;
        //                decimal exstdecsal = existsaldec.Count > 0 ? existsaldec.Sum(x => x.Salary) : 0;
        //                var BS = (basicsal.Salary + exstincsal) - exstdecsal;
        //                _SalaryObj.BasicSalary = BS;
        //                _SalaryObj.IncrementAmount = mod.Amount;
        //                _SalaryObj.GrossSalary = _SalaryObj.BasicSalary - _SalaryObj.IncrementAmount;
        //            }
        //            //var existsal = db.Pay_EmpSalary.Where(x => x.EmpId == item).FirstOrDefault();
        //            //if (existsal != null)
        //            //{
        //            //    _SalaryObj.BasicSalary = existsal.Salary;
        //            //    _SalaryObj.IncrementAmount = mod.Amount;
        //            //    _SalaryObj.GrossSalary = existsal.Salary - _SalaryObj.IncrementAmount;
        //            //}
        //            _Salary.Add(_SalaryObj);
        //        }
        //    }
        //    return _Salary;
        //}

        #endregion
        #region BranchStrength
        public List<BranchStrengthVM> GetBranchStrength()
        {
            try
            {
                List<BranchStrengthVM> lst = new List<BranchStrengthVM>();
                var lt = db.spget_PayBranchStrength().ToList();
                var lsst = lt.Select(x => new { x.DesgId, x.DesgName }).Distinct().ToList();
                for (int i = 0; i < lsst.Count; i++)
                {
                    var ls = lt.Where(x => x.DesgId == lsst[i].DesgId).ToList();
                    if (i == 0)
                    {
                        foreach (var item in ls)
                        {
                            lst.Add(new BranchStrengthVM
                            {
                                DeptId = item.DeptId,
                                DeptName = item.DeptName,
                                ApprovedStrength1 = item.ApprovedStrength,
                                DesgId1 = item.DesgId,
                                DesgName1 = item.DesgName
                            });
                        }
                    }
                    else
                    {
                        foreach (var item in ls)
                        {
                            var tbl = lst.Where(x => x.DeptId == item.DeptId).FirstOrDefault();
                            SetVal(tbl, item.ApprovedStrength, "ApprovedStrength" + (i + 1));
                            SetVal(tbl, item.DesgId, "DesgId" + (i + 1));
                            SetVal(tbl, item.DesgName, "DesgName" + (i + 1));
                        }
                    }
                }
                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> SaveBranchStrength(BranchStaffStrengthAEVM mod, int UserId)
        {
            try
            {
                foreach (var item in mod.LocId)
                {
                    if (mod.StaffStrength >= 0)
                    {
                        var tbl = await db.Pay_BranchStaffStrength.Where(x => x.DesgId == mod.DesgId && x.DeptId == item).FirstOrDefaultAsync();
                        if (tbl != null)
                        {
                            if (tbl.ApprovedStrength != mod.StaffStrength)
                            {
                                tbl.ApprovedStrength = mod.StaffStrength;
                                tbl.TransDate = DateTime.Now;
                                tbl.UserId = UserId;
                            }
                        }
                        else
                        {
                            tbl = new Pay_BranchStaffStrength();
                            tbl.ApprovedStrength = mod.StaffStrength;
                            tbl.TransDate = DateTime.Now;
                            tbl.UserId = UserId;
                            tbl.DeptId = item;
                            tbl.DesgId = mod.DesgId;
                            db.Pay_BranchStaffStrength.Add(tbl);
                        }
                    }
                }

                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// Old FUnction
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<List<BranchStrengthVM>> SaveBranchStrength_Old(IEnumerable<BranchStrengthVM> mod, int UserId)
        {
            try
            {
                for (int i = 0; i < 11; i++)
                {
                    foreach (var v in mod)
                    {
                        var DesgId = GetVal(v, "DesgId" + (i + 1));
                        if (DesgId > 0)
                        {
                            var strength = GetVal(v, "ApprovedStrength" + (i + 1));
                            if (strength >= 0)
                            {
                                var tbl = await db.Pay_BranchStaffStrength.Where(x => x.DesgId == DesgId && x.DeptId == v.DeptId).FirstOrDefaultAsync();
                                if (tbl != null)
                                {
                                    if (tbl.ApprovedStrength != strength)
                                    {
                                        tbl.ApprovedStrength = strength;
                                        tbl.TransDate = DateTime.Now;
                                        tbl.UserId = UserId;
                                    }
                                }
                                else
                                {
                                    tbl = new Pay_BranchStaffStrength();
                                    tbl.ApprovedStrength = strength;
                                    tbl.TransDate = DateTime.Now;
                                    tbl.UserId = UserId;
                                    tbl.DeptId = v.DeptId;
                                    tbl.DesgId = DesgId;
                                    db.Pay_BranchStaffStrength.Add(tbl);
                                }
                            }
                        }
                        else
                            break;
                    }
                }
                await db.SaveChangesAsync();
                return mod.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void SetVal(object obj, object val, string Name)
        {
            Type t = obj.GetType();
            PropertyInfo I = t.GetProperty(Name);
            I.SetValue(obj, val);
        }
        public int GetVal(object obj, string Name)
        {
            Type t = obj.GetType();
            PropertyInfo I = t.GetProperty(Name);
            return (int)I.GetValue(obj);
        }
        #endregion
        #region Attendance Finalization

        public async Task<List<AttendanceStatusVM>> GetAttendanceStatus()
        {
            var status = await db.Pay_AttendanceStatus.Select(x => new AttendanceStatusVM()
            {
                StatusId = x.StatusId,
                Status = x.Status
            }).ToListAsync();
            return status;
        }

        public async Task<bool> AttendanceExceptionUpdate(IEnumerable<AttendanceExceptionVM> attendanceexceps, DateTime Attdate, int userid)
        {
            try
            {
                var att = db.Pay_Attendance.Where(x => x.AttnDate == Attdate).ToList();
                att.ForEach(x => x.IsVerified = true);
                db.SaveChanges();
                foreach (var item in attendanceexceps)
                {
                    if (item.Status == "P")
                    {
                        item.AutoDescription = "Present";
                    }
                    else if (item.Status == "A")
                    {
                        item.AutoDescription = "Absent";
                    }
                    else if (item.Status == "L")
                    {
                        item.AutoDescription = "Leave";
                    }
                    Pay_AttendanceFinal _Atttendance = new Pay_AttendanceFinal()
                    {
                        AttendanceDate = Convert.ToDateTime(item.Attendancedt),
                        AttnType = item.AttnType,
                        WorkingHours = Convert.ToInt32(item.WorkingHours),
                        EmpId = Convert.ToInt32(item.EmpId),
                        Reason = item.AutoDescription,
                        StatusId = item.Status,
                        UserId = userid,
                        TransDate = DateTime.Now
                    };

                    db.Pay_AttendanceFinal.Add(_Atttendance);
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public List<AttendanceExceptionVM> GetAttendanceException(DateTime WorkingDt, int? HDeptId, int? DeptId, int? Userid)
        {
            try
            {
                List<AttendanceExceptionVM> lst = new List<AttendanceExceptionVM>();
                var lt = db.spMark_FinalAttendance(WorkingDt, HDeptId, DeptId, Userid).Select(x => new AttendanceExceptionVM()
                {
                    AttnId = 0,
                    Attendancedt = x.AttendanceDt,
                    AttnType = x.AttnType,
                    AutoDescription = x.AutoDescription,
                    DeptName = x.DeptName,
                    EmpId = x.EmpId,
                    EmpName = x.EmpName,
                    Status = x.Status,
                    //InTime = x.InTime,
                    //OutTime = x.OutTime,
                    InTime = (x.InTime.HasValue ? x.InTime.Value.ToString("HH:mm tt") : ""),
                    OutTime = (x.OutTime.HasValue ? x.OutTime.Value.ToString("HH:mm tt") : ""),
                    TransDate = x.TransDate,
                    WorkingHours = x.WorkingHours
                }).ToList();

                return lt;
            }
            catch (Exception)
            {

                throw;
            }
        }


        #endregion
        #region Manual Attendance
        public async Task<AttendanceVM> AddManualAttendance(AttendanceVM mod)
        {
            // var emp = db.Pay_AttendanceFinal.Where(x => x.EmpId == mod.EmpId && x.AttendanceDate == mod.AttendanceDate).FirstOrDefault();
            if (mod.Todate >= mod.AttendanceDate)
            {
                var dt = mod.AttendanceDate;
                while (dt <= mod.Todate)
                {
                    var emps = db.Pay_AttendanceFinal.Where(x => x.EmpId == mod.EmpId && x.AttendanceDate == dt).FirstOrDefault();
                    if (emps != null)
                    {
                        emps.StatusId = mod.StatusId;
                        emps.Reason = mod.Reason + " Manual Attendance";
                        emps.TransDate = DateTime.Now;
                        emps.UserId = mod.UserId;
                        await db.SaveChangesAsync();
                        mod.TransId = emps.RowId;
                    }
                    else
                    {
                        Pay_AttendanceFinal _AttFinal = new Pay_AttendanceFinal()
                        {
                            AttendanceDate = dt,
                            AttnType = mod.AttnType,
                            EmpId = mod.EmpId,
                            Reason = mod.Reason + " Manual Attendance",
                            WorkingHours = 0,
                            StatusId = mod.StatusId,
                            TransDate = DateTime.Now,
                            UserId = mod.UserId
                        };
                        db.Pay_AttendanceFinal.Add(_AttFinal);
                        await db.SaveChangesAsync();
                        mod.TransId = _AttFinal.RowId;
                    }
                    dt = dt.AddDays(1);

                }
                return mod;
            }
            else
            {
                return null;
            }
        }

        #endregion
        #region Employee PayScale

        /// <summary>
        /// List the PayScaleList
        /// </summary>
        /// <returns></returns>
        public List<PayScaleVM> EmpPayScaleList()
        {
            try
            {
                return db.Pay_PayScales.ToList().Select(x =>
              new PayScaleVM
              {
                  BPS = x.BPS,
                  Increment = x.Increment,
                  MaxSalary = x.MaxSalary,
                  MinSalary = x.MinSalary,
                  Stages = x.Stages
              }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// If Shift Id is Passed Greated Than 0 will be update all otherwise 0 Will a a new Shift
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public async Task<int> CreateUpdatePayScale(PayScaleVM mod)
        {
            try
            {

                if (mod.BPS > 0)
                {
                    var pyscale = await db.Pay_PayScales.FindAsync(mod.BPS);
                    pyscale.BPS = mod.BPS;
                    pyscale.Increment = mod.Increment;
                    pyscale.MaxSalary = mod.MaxSalary;
                    pyscale.MinSalary = mod.MinSalary;
                    pyscale.Stages = mod.Stages;
                    await db.SaveChangesAsync();
                }
                else
                {

                    Pay_PayScales tbl = new Pay_PayScales
                    {
                        BPS = mod.BPS,
                        Increment = mod.Increment,
                        MaxSalary = mod.MaxSalary,
                        MinSalary = mod.MinSalary,
                        Stages = mod.Stages
                    };
                    db.Pay_PayScales.Add(tbl);

                    await db.SaveChangesAsync();
                    mod.BPS = tbl.BPS;

                }
                return mod.BPS;
            }
            catch (Exception)
            {
                return 0;
            }
        }



        #endregion Shifts
        #region Salary Test Process
        /// <summary>
        /// List the PayScaleList
        /// </summary>
        /// <returns></returns>
        public int TestSalaryProcess(SalaryTestProcessVM mod)
        {
            try
            {
                db.Database.CommandTimeout = 3600;
                if (mod.pHDeptId == 1)
                {
                    return db.spPay_SalaryProcess_HO_Test(mod.pSalaryMonth, mod.pHDeptId, mod.pUserId);
                }
                else if (mod.pHDeptId == 2)
                {

                    return db.spPay_SalaryProcess_Showrooms_Test(mod.pSalaryMonth, mod.pHDeptId, mod.pUserId);
                }
                else if (mod.pHDeptId == 3)
                {
                    return db.spPay_SalaryProcess_Field_Test(mod.pSalaryMonth, mod.pHDeptId, mod.pUserId);
                }
                else if (mod.pHDeptId == 4)
                {
                    return db.spPay_CharityDonation_Process(mod.pSalaryMonth, mod.pUserId, "T");
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion
        #region Salary Final Process
        /// <summary>
        /// List the PayScaleList
        /// </summary>
        /// <returns></returns>
        public int FinalSalaryProcess(SalaryTestProcessVM mod)
        {
            try
            {
                if (mod.pHDeptId == 1)
                {
                    return db.spPay_SalaryProcess_HO_Final(mod.pSalaryMonth, mod.pHDeptId, mod.pUserId);
                }
                else if (mod.pHDeptId == 2)
                {
                    return db.spPay_SalaryProcess_Showrooms_Final(mod.pSalaryMonth, mod.pHDeptId, mod.pUserId);
                }
                else if (mod.pHDeptId == 3)
                {
                    return db.spPay_SalaryProcess_Field_Final(mod.pSalaryMonth, mod.pHDeptId, mod.pUserId);
                }
                else if (mod.pHDeptId == 4)
                {
                    return db.spPay_CharityDonation_Process(mod.pSalaryMonth, mod.pUserId, "F");
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion
        #region Empoyee Location Mapping
        public async Task<List<EmployeeLocationMappingVM>> EmployeeLocationMapping1List(int DesgId)
        {
            try
            {
                var lst = await db.Pay_EmpMaster.Where(x => x.StatusId == "A" && (x.DesgId == DesgId || DesgId == 0)).ToListAsync();
                var loc = await db.Pay_EmpLocationMapping.Select(x => new { x.LocId, x.Comp_Locations.LocName, x.EmpId, x.Status }).Where(x => x.Status).ToListAsync();
                //join empmaster in db.Pay_EmpMaster on emp.EmpId equals empmaster.EmpId
                var data = (from emp in lst
                                //let L = loc
                            select new EmployeeLocationMappingVM()
                            {
                                EmpId = emp.EmpId,
                                EmployeeName = emp.EmpName,
                                CNIC = emp.CNIC,
                                LocationName = String.Join(",", loc.Where(x => x.EmpId == emp.EmpId).Select(x => x.LocName)),
                                LocId = loc.Where(x => x.EmpId == emp.EmpId).Select(x => x.LocId).ToList()
                            }).ToList();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> CreateUpdateEmployeeLocationMapping1(EmployeeLocationMappingVM mod, int UserId, int empid)
        {
            try
            {
                var trans = db.Pay_EmpLocationMapping.Where(X => X.EmpId == mod.EmpId).ToList();


                //Items which are removed by the User
                var items = trans.Where(p => mod.LocId == null || !mod.LocId.Any(p2 => p.LocId == p2 && p.EmpId == empid)).ToList();
                if (items != null)
                {
                    foreach (var emps in items)
                    {
                        emps.ModifiedBy = UserId;
                        emps.ModifiedDate = DateTime.Now;
                        emps.Status = false;
                        await db.SaveChangesAsync();
                    }
                }


                foreach (var item in mod.LocId)
                {
                    var emplocexist = db.Pay_EmpLocationMapping.Where(x => x.LocId == item
                    && x.EmpId == mod.EmpId).FirstOrDefault();
                    if (emplocexist == null)
                    {
                        Pay_EmpLocationMapping _EmpLoc = new Pay_EmpLocationMapping
                        {
                            DefinedBy = UserId,
                            DefinedDate = DateTime.Now,
                            EmpId = mod.EmpId,
                            LocId = item,
                            Status = true
                        };
                        db.Pay_EmpLocationMapping.Add(_EmpLoc);
                        await db.SaveChangesAsync();

                    }
                    else
                    {
                        emplocexist.Status = true;
                        emplocexist.ModifiedBy = UserId;
                        emplocexist.ModifiedDate = DateTime.Now;
                        await db.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region EmployeeJoinLog

        public async Task<bool> EmployeeJoinLogCreate(EmpJoiningLogVM mod, int UserId)
        {
            try
            {
                Pay_EmpJoiningLog tbl = new Pay_EmpJoiningLog
                {
                    EmpId = mod.EmpId,
                    JoinDate = mod.JoinDate,
                    JoinReason = mod.JoinReason,
                    Status = "A",
                    Remarks = "Rejoin",
                    UserId = UserId,
                    TransDate = DateTime.Now
                };

                db.Pay_EmpJoiningLog.Add(tbl);
                await db.SaveChangesAsync();
                mod.JoinId = tbl.JoinId;
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployee(EmployeeVM mod, int UserId)
        {
            try
            {
                if (mod.EmpId > 0)
                {
                    var tbl = await db.Pay_EmpMaster.FirstOrDefaultAsync(x => x.EmpId == mod.EmpId);
                    if (tbl != null)
                    {
                        tbl.DeptId = mod.DeptId;
                        tbl.DesgId = mod.DesgId;
                        tbl.DOJ = mod.JoinDate == null ? new DateTime() : Convert.ToDateTime(mod.JoinDate);
                        tbl.UserId = UserId;
                        tbl.StatusId = "A";
                        tbl.AttendanceStatus = true;
                        tbl.TransDate = DateTime.Now;
                    }
                    await db.SaveChangesAsync();

                    if (tbl.DeptId != mod.DeptId)
                    {
                        var tbldept = new Pay_DepartmentLog
                        {
                            EmpId = mod.EmpId,
                            DeptId = mod.DeptId,
                            UserId = UserId,
                            TransDate = DateTime.Now
                        };
                        db.Pay_DepartmentLog.Add(tbldept);
                        await db.SaveChangesAsync();
                    }
                    if (tbl.DesgId != mod.DesgId)
                    {
                        var tbldesg = new Pay_DesignationLog
                        {
                            EmpId = mod.EmpId,
                            DesgId = mod.DesgId,
                            UserId = UserId,
                            TransDate = DateTime.Now
                        };
                        db.Pay_DesignationLog.Add(tbldesg);
                        await db.SaveChangesAsync();
                    }
                    var ISBasicSalaryExist = IsBasicSalaryExist(Convert.ToInt32(mod.EmpId));
                    if (mod.NewSalary > 0 && ISBasicSalaryExist == true)
                    {
                        var SalaryAmt = await EmpLastSalary(Convert.ToInt32(mod.EmpId));
                        decimal FinalSalary = 0;
                        if (SalaryAmt > mod.NewSalary)
                        {
                            FinalSalary = SalaryAmt - mod.NewSalary;
                        }
                        else
                        {
                            FinalSalary = mod.NewSalary - SalaryAmt;
                        }

                        var tblSalary = new Pay_EmpSalary
                        {
                            EmpId = mod.EmpId,
                            SalTypeId = mod.NewSalary > await EmpLastSalary(mod.EmpId) ? "I" : "D",
                            Salary = FinalSalary,
                            UserId = UserId,
                            EffectiveFrom = mod.JoinDate,
                            TransDate = DateTime.Now
                        };
                        db.Pay_EmpSalary.Add(tblSalary);
                        await db.SaveChangesAsync();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<bool> UpdateEmployeeJoinLog(FinalSettlementVM mod, int UserId)
        {
            try
            {
                //var EmpJoinLog = await db.Pay_EmpJoiningLog.Where(x => x.EmpId == mod.EmpId).FirstOrDefaultAsync();
                //if (EmpJoinLog != null)
                //{
                var emp = await db.Pay_EmpMaster.Where(x => x.EmpId == mod.EmpId).FirstOrDefaultAsync();

                Pay_EmpJoiningLog EmpJoinMod = new Pay_EmpJoiningLog();
                EmpJoinMod.FinalDate = mod.FinalDate;
                EmpJoinMod.FinalReason = mod.Reason;
                EmpJoinMod.Status = mod.Status;
                EmpJoinMod.JoinDate = Convert.ToDateTime(emp.DOJ);
                EmpJoinMod.UserId = UserId;
                EmpJoinMod.EmpId = mod.EmpId;
                EmpJoinMod.TransDate = DateTime.Now;
                db.Pay_EmpJoiningLog.Add(EmpJoinMod);
                await db.SaveChangesAsync();
                return true;
                //}

            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateEmployeeStatus(FinalSettlementVM mod, int UserId)
        {
            try
            {
                var Emp = await db.Pay_EmpMaster.Where(x => x.EmpId == mod.EmpId).FirstOrDefaultAsync();
                if (Emp != null)
                {
                    Emp.StatusId = mod.Status;
                    Emp.ModifiedBy = UserId;
                    Emp.ModifiedDate = DateTime.Now;
                    Emp.AttendanceStatus = false;
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




        #endregion
        #region EmployeePerformance

        public async Task<List<EmpPerformanceTypeVM>> EmployeePerformanceTypeList()
        {
            return await db.Pay_EmpPerformanceType.Select(x => new EmpPerformanceTypeVM()
            {
                PerformanceTypeId = x.PerformanceTypeId,
                PerformanceType = x.PerformanceType
            }).ToListAsync();
        }
        public async Task<List<EmpPerformanceTypeVM>> StaffIncentiveList()
        {
            var empperform = await (from item in db.Pay_EmpPerformance
                                    join pertype in db.Pay_EmpPerformanceType on item.PerformanceTypeId equals pertype.PerformanceTypeId
                                    join proccessby in db.Users_Login on item.ProcessedBy equals proccessby.UserID
                                    join validatedby in db.Users_Login on item.ValidatedBy equals validatedby.UserID
                                    join approvedby in db.Users_Login on item.ApprovedBy equals approvedby.UserID
                                    select new EmpPerformanceTypeVM()
                                    {
                                        TransId = item.TransId,
                                        PerformanceMonth = item.PerformanceMonth,
                                        PerformanceType = pertype.PerformanceType,
                                        ProcessedByName = proccessby.FullName,
                                        ProcessedDate = item.ProcessedDate,
                                        ValidatedByName = validatedby.FullName,
                                        ValidatedDate = item.ValidatedDate,
                                        ApprovedByName = approvedby.FullName,
                                        ApprovedDate = item.ApprovedDate
                                    }).ToListAsync();

            return empperform;
        }

        public async Task<bool> UpdateApprovedPerfomaceValue(long transid, decimal approvedvalue, string remarks, int userid)
        {
            if (transid > 0)
            {
                var rec = await db.Pay_EmpPerformanceDetail.Where(x => x.TransId == transid).Where(x => x.TransId == transid).FirstOrDefaultAsync();
                rec.ApprovedValue = approvedvalue;
                rec.Remarks = remarks;
                var MasterRec = await db.Pay_EmpPerformance.Where(x => x.TransId == rec.TransId).FirstOrDefaultAsync();
                MasterRec.ApprovedBy = userid;
                MasterRec.ApprovedDate = DateTime.Now;
                await db.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<EmpPerformanceTypeVM> GetEmpPerformance(int TransId)
        {
            var empperformance = (from item in db.Pay_EmpPerformance
                                  join itemdet in db.Pay_EmpPerformanceDetail on item.TransId equals itemdet.TransId
                                  join emps in db.Pay_EmpMaster on itemdet.EmpId equals emps.EmpId
                                  join loc in db.Comp_Locations on itemdet.LocId equals loc.LocId
                                  where item.TransId == TransId
                                  select new EmpPerformanceTypeVM()
                                  {
                                      TransId = item.TransId,
                                      TransDtlId = itemdet.TransDtlId,
                                      EmpId = itemdet.EmpId,
                                      EmpName = emps.EmpName,
                                      Designation = emps.Pay_Designation.DesgName,
                                      CNIC = emps.CNIC,
                                      AchQty = itemdet.AchQty,
                                      AchValue = itemdet.AchValue,
                                      TargetQty = itemdet.TargetQty,
                                      TargetValue = itemdet.TargetValue,
                                      IncentiveValue = itemdet.IncentiveValue,
                                      IncentivePercent = itemdet.IncentivePercent,
                                      ApprovedValue = itemdet.ApprovedValue,
                                      Remarks = itemdet.Remarks

                                  }).ToList().Select(x => new EmpPerformanceTypeVM()
                                  {

                                      TransId = x.TransId,
                                      TransDtlId = x.TransDtlId,
                                      EmpId = x.EmpId,
                                      EmpName = x.EmpName,
                                      Designation = x.Designation,
                                      CNIC = x.CNIC,
                                      AchQty = x.AchQty,
                                      AchValue = x.AchValue,
                                      TargetQty = x.TargetQty,
                                      TargetValue = x.TargetValue,
                                      IncentiveValue = x.IncentiveValue,
                                      IncentivePercent = x.IncentivePercent,
                                      ApprovedValue = x.IncentiveValue,
                                      Remarks = x.Remarks,
                                      HeaderOne = GetPerformanceHeader(x.PerformanceTypeId).HeaderOne,
                                      HeaderTwo = GetPerformanceHeader(x.PerformanceTypeId).HeaderTwo
                                  }).ToList();
            return empperformance;
        }

        public HeadersVM GetPerformanceHeader(int id)
        {
            HeadersVM _HeaderVM = new HeadersVM();
            if (id == 1)
            {
                _HeaderVM.HeaderOne = "Maketing Sale";
                _HeaderVM.HeaderTwo = "";
            }
            if (id == 2)
            {
                _HeaderVM.HeaderOne = "Inquiry Target";
                _HeaderVM.HeaderTwo = "Inquiry Achieved";
            }
            if (id == 3)
            {
                _HeaderVM.HeaderOne = "Recovery Target";
                _HeaderVM.HeaderTwo = "Recovery Achieved";
            }
            if (id == 4)
            {
                _HeaderVM.HeaderOne = "Recovery Target";
                _HeaderVM.HeaderTwo = "Recovery Achieved";
            }
            if (id == 5)
            {
                _HeaderVM.HeaderOne = "Sale Target";
                _HeaderVM.HeaderTwo = "Sale Target Achieved";
            }
            if (id == 6)
            {
                _HeaderVM.HeaderOne = "Sale Target";
                _HeaderVM.HeaderTwo = "Sale Target Achieved";
            }
            return _HeaderVM;
        }
        #endregion
        #region GeoHirInsSales
        //public bool GeoHirInsSaleUpdate(GeoHirVM mod, int UserId)
        //{
        //var geoloc = db.Comp_GeoLocation.Where(x => x.GeoId == mod.GeoId).FirstOrDefault();
        //geoloc.GTitle = mod.GTitle;
        //db.SaveChanges();
        //var comlocmap = db.Comp_LocationsMapping.Where(x => x.EmpId == mod.EmpId).ToList();
        //foreach (var item in comlocmap)
        //{

        //}
        //}
        public bool GeoHirInsSale(GeoHirVM mod, int UserId)
        {
            try
            {
                var GeoMaxId = db.Comp_GeoLocation.OrderByDescending(x => x.GeoId).FirstOrDefault().GeoId + 1;
                Comp_GeoLocation grpmap = new Comp_GeoLocation()
                {
                    GTitle = mod.GTitle,
                    ParentId = mod.ParentId,
                    GLevel = mod.GLevel,
                    GeoId = GeoMaxId,
                    Status = 1
                };
                db.Comp_GeoLocation.Add(grpmap);
                db.SaveChanges();

                var mapping = new Comp_LocationsMapping()
                {
                    EmpId = mod.EmpId,
                    GeoId = GeoMaxId,
                    MappedBy = UserId,
                    MappedDate = DateTime.Now,
                    MappingStatus = true
                };
                db.Comp_LocationsMapping.Add(mapping);
                db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion
        #region SalarySlab

        public async Task<List<BasicSalarySlabVM>> GetBasicSalarySlabsList()
        {
            return await (from item in db.Pay_BasicSalarySlab
                          where item.Status == true
                          select new BasicSalarySlabVM()
                          {
                              SlabId = item.SlabId,
                              SlabTitle = item.SlabTitle,
                              Status = item.Status
                          }).ToListAsync();
        }
        public async Task<List<BasicSalarySlabVM>> GetBasicSalarySlabs(int status)
        {
            return await (from item in db.Pay_BasicSalarySlab
                          join user in db.Pay_EmpMaster on item.DefinedBy equals user.EmpId into ps
                          from p in ps.DefaultIfEmpty()
                          where item.Status == (status == 1 ? true : false)
                          select new BasicSalarySlabVM()
                          {
                              SlabId = item.SlabId,
                              SlabTitle = item.SlabTitle,
                              Status = item.Status,
                              Defined = p.EmpName,
                              DefinedDate = item.DefinedDate
                          }).ToListAsync();
        }
        public async Task<BasicSalarySlabVM> GetBasicSalarySlab(int id)
        {
            return await (from item in db.Pay_BasicSalarySlab
                          where item.SlabId == id
                          select new BasicSalarySlabVM()
                          {
                              SlabId = item.SlabId,
                              SlabTitle = item.SlabTitle,
                              Status = item.Status,
                              StatusId = item.Status == true ? "1" : "2",
                              DefinedBy = item.DefinedBy,
                              DefinedDate = item.DefinedDate
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<BasicSalarySlabDtlVM>> GetBasicSalarySlabDetail(int SlabId)
        {
            return await (from item in db.Pay_BasicSalarySlabDtl
                          where item.SlabId == SlabId
                          select new BasicSalarySlabDtlVM()
                          {
                              TransId = item.TransId,
                              SlabStart = item.SlabStart,
                              SlabEnd = item.SlabEnd,
                              BasicSalary = item.BasicSalary
                          }).ToListAsync();
        }
        public async Task<bool> AddUpdateBasicSalarySlab(List<BasicSalarySlabDtlVM> mod, int UserId, int SlabId, string SlabTitle, string Status)
        {
            try
            {
                if (mod != null)
                {
                    if (SlabId > 0)
                    {
                        var slab = await db.Pay_BasicSalarySlab.Where(x => x.SlabId == SlabId).FirstOrDefaultAsync();
                        if (slab != null)
                        {
                            slab.SlabTitle = SlabTitle;
                            slab.Status = Status == "1" ? true : false;
                            slab.DefinedBy = UserId;
                            slab.DefinedDate = DateTime.Now;

                            var DtlSlab = await db.Pay_BasicSalarySlabDtl.Where(x => x.SlabId == SlabId).ToListAsync();
                            db.Pay_BasicSalarySlabDtl.RemoveRange(DtlSlab);
                            await db.SaveChangesAsync();

                            foreach (var item in mod)
                            {
                                Pay_BasicSalarySlabDtl SalarySlbD = new Pay_BasicSalarySlabDtl();
                                SalarySlbD.SlabStart = item.SlabStart;
                                SalarySlbD.SlabEnd = item.SlabEnd;
                                SalarySlbD.BasicSalary = item.BasicSalary;
                                SalarySlbD.SlabId = slab.SlabId;
                                db.Pay_BasicSalarySlabDtl.Add(SalarySlbD);
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

                        Pay_BasicSalarySlab SalarySalb = new Pay_BasicSalarySlab()
                        {
                            SlabTitle = SlabTitle,
                            DefinedBy = UserId,
                            DefinedDate = DateTime.Now,
                            Status = true
                        };
                        db.Pay_BasicSalarySlab.Add(SalarySalb);
                        await db.SaveChangesAsync();

                        foreach (var item in mod)
                        {
                            Pay_BasicSalarySlabDtl SalarySlbD = new Pay_BasicSalarySlabDtl();
                            SalarySlbD.SlabStart = item.SlabStart;
                            SalarySlbD.SlabEnd = item.SlabEnd;
                            SalarySlbD.BasicSalary = item.BasicSalary;
                            SalarySlbD.SlabId = SalarySalb.SlabId;
                            db.Pay_BasicSalarySlabDtl.Add(SalarySlbD);
                        }
                        await db.SaveChangesAsync();
                        return true;
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
        #endregion
        #region Salary Policy
        public async Task<List<BasicSalaryPolicyNVM>> GetBasicSalaryPolicy(int status)
        {
            return await (from item in db.Pay_BasicSalaryPolicy
                          join user in db.Pay_EmpMaster on item.DefinedBy equals user.EmpId into ps
                          from p in ps.DefaultIfEmpty()
                          where item.Status == (status == 1 ? true : false)
                          select new BasicSalaryPolicyNVM()
                          {
                              PolicyId = item.PolicyId,
                              PolicyTitle = item.PolicyTitle,
                              Status = item.Status,
                              Defined = p.EmpName,
                              EffectiveDate = item.EffectiveDate,
                              DefinedDate = item.DefinedDate,
                              Remarks = item.Remarks
                          }).ToListAsync();
        }

        public async Task<BasicSalaryPolicyNVM> GetBasicSalaryPolicyById(int id)
        {
            var lst = await (from item in db.Pay_BasicSalaryPolicy
                             where item.PolicyId == id
                             select new BasicSalaryPolicyNVM()
                             {
                                 PolicyId = item.PolicyId,
                                 PolicyTitle = item.PolicyTitle,
                                 Status = item.Status,
                                 StatusId = item.Status == true ? "1" : "2",
                                 DefinedBy = item.DefinedBy,
                                 DefinedDate = item.DefinedDate,
                                 DesgId = item.DesgId,
                                 SlabId = item.SlabId,
                                 PerformanceTypeId = item.PerformanceTypeId,
                                 EffectiveDate = item.EffectiveDate,
                                 Remarks = item.Remarks

                             }).FirstOrDefaultAsync();
            lst.CCtyLst = db.Pay_BasicSalaryPolicyLocations.Where(x => x.PolicyId == lst.PolicyId).Select(x => x.CityId).ToArray();
            lst.CLcLst = db.Pay_BasicSalaryPolicyLocations.Where(x => x.PolicyId == lst.PolicyId).Select(x => x.LocId).ToArray();
            return lst;
        }

        public async Task<bool> AddUpdateBasicSalaryPolicy(int[] citylst, int[] loclst, int DesgId, int PolicyId, int UserId, int SlabId, int PerformanceId, DateTime EffectiveDate, string remarks, string Status)
        {
            try
            {
                if (citylst.Count() > 0 && loclst.Count() > 0)
                {
                    if (PolicyId > 0)
                    {
                        var policy = await db.Pay_BasicSalaryPolicy.Where(x => x.PolicyId == PolicyId).FirstOrDefaultAsync();
                        if (policy != null)
                        {
                            var Desg = await DesignationById(DesgId);
                            policy.PolicyTitle = "Basic Salary Policy " + Desg.DesgName;
                            policy.DesgId = DesgId;
                            policy.SlabId = SlabId;
                            policy.PerformanceTypeId = PerformanceId;
                            policy.EffectiveDate = EffectiveDate;
                            policy.Status = Status == "1" ? true : false;
                            policy.DefinedBy = UserId;
                            policy.DefinedDate = DateTime.Now;
                            policy.Remarks = remarks;
                            await db.SaveChangesAsync();

                            var DtlSlab = await db.Pay_BasicSalaryPolicyLocations.Where(x => x.PolicyId == PolicyId).ToListAsync();
                            db.Pay_BasicSalaryPolicyLocations.RemoveRange(DtlSlab);
                            await db.SaveChangesAsync();
                            await AddBSPCL(PolicyId, citylst, loclst);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        var Desg = await DesignationById(DesgId);
                        Pay_BasicSalaryPolicy SalarySalb = new Pay_BasicSalaryPolicy()
                        {
                            PolicyTitle = "Basic Salary Policy " + Desg.DesgName,
                            DesgId = DesgId,
                            SlabId = SlabId,
                            PerformanceTypeId = PerformanceId,
                            EffectiveDate = EffectiveDate,
                            DefinedBy = UserId,
                            DefinedDate = DateTime.Now,
                            Status = true,
                            Remarks = remarks
                        };
                        db.Pay_BasicSalaryPolicy.Add(SalarySalb);
                        await db.SaveChangesAsync();

                        await AddBSPCL(SalarySalb.PolicyId, citylst, loclst);
                        return true;
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

        public async Task<int> AddBSPCL(int PolicyId, int[] Cities, int[] Locations)
        {
            try
            {
                int bspId = PolicyId;
                if (Cities.Length > 0 && Cities[0] > 0)
                {
                    if (Locations.Length > 0 && Locations[0] > 0)
                    {
                        var bsplList = new List<Pay_BasicSalaryPolicyLocations>();
                        var LCTs = new List<int>();
                        foreach (var l in Locations)
                        {
                            var cityID = db.Comp_Locations.First(x => x.LocId == l).CityId;
                            if (Cities.Contains(cityID))
                            {
                                LCTs.Add(cityID);
                            }
                            var bspl = new Pay_BasicSalaryPolicyLocations
                            {
                                PolicyId = bspId,
                                LocId = l,
                                CityId = cityID
                            };
                            bsplList.Add(bspl);
                        }
                        db.Pay_BasicSalaryPolicyLocations.AddRange(bsplList);
                        await db.SaveChangesAsync();

                        var CitiesOnly = Cities.Except(LCTs);
                        AddBSP(CitiesOnly.ToArray(), bspId);
                    }
                    else
                    {
                        AddBSP(Cities, bspId);
                    }
                }
                return bspId;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public bool AddBSP(int[] Cities, int bspId)
        {
            try
            {
                var bsplList = new List<Pay_BasicSalaryPolicyLocations>();
                foreach (var c in Cities)
                {
                    var bspl = new Pay_BasicSalaryPolicyLocations
                    {
                        PolicyId = bspId,
                        LocId = 0,
                        CityId = c
                    };
                    bsplList.Add(bspl);
                }
                db.Pay_BasicSalaryPolicyLocations.AddRange(bsplList);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
        #region Employee Incentives Policy
        public async Task<List<PayIncentiveVM>> GetEmployeeIncentives(int status)
        {


            return await (from item in db.Pay_IncentivePolicy
                          join user in db.Pay_EmpMaster on item.DefinedBy equals user.EmpId into ps
                          from p in ps.DefaultIfEmpty()
                          where item.Status == (status == 1 ? true : false)
                          select new PayIncentiveVM()
                          {
                              PolicyId = item.PolicyId,
                              PolicyTitle = item.PolicyTitle,
                              Status = item.Status,
                              IncVal = item.IncVal,
                              IncPer = item.IncPer,
                              AlwId = item.AlwId,
                              DefinedBy = item.DefinedBy,
                              Defined = p.EmpName,
                              DefinedDate = item.DefinedDate
                          }).ToListAsync();
        }

        public async Task<PayIncentiveVM> GetEmployeeIncentiveById(int id)
        {
            var lst = await (from item in db.Pay_IncentivePolicy
                             where item.PolicyId == id
                             select new PayIncentiveVM()
                             {
                                 PolicyId = item.PolicyId,
                                 PolicyTitle = item.PolicyTitle,
                                 Status = item.Status,
                                 StatusId = item.Status == true ? "1" : "2",
                                 DefinedBy = item.DefinedBy,
                                 DefinedDate = item.DefinedDate,
                                 DesgId = item.DesgId,
                                 AlwId = item.AlwId,
                                 IncVal = item.IncVal,
                                 IncPer = item.IncPer,
                                 selectable = true

                             }).FirstOrDefaultAsync();
            lst.CCtyLst = db.Pay_IncentivePolicyLocations.Where(x => x.PolicyId == lst.PolicyId).Select(x => x.CityId).ToArray();
            lst.CLcLst = db.Pay_IncentivePolicyLocations.Where(x => x.PolicyId == lst.PolicyId).Select(x => x.LocId).ToArray();
            return lst;
        }

        public async Task<bool> AddUpdateEmployeeIncentive(int[] citylst, int[] loclst, int DesgId, int PolicyId, int UserId, int AlwId, decimal IncVal, decimal IncPer, string Status)
        {
            try
            {
                if (citylst.Count() > 0 && loclst.Count() > 0)
                {

                    if (PolicyId > 0)
                    {
                        var policy = await db.Pay_IncentivePolicy.Where(x => x.PolicyId == PolicyId).FirstOrDefaultAsync();
                        if (policy != null)
                        {
                            var Desg = await DesignationById(DesgId);
                            policy.PolicyTitle = Desg.DesgName + " Incentive Policy";
                            policy.DesgId = DesgId;
                            policy.AlwId = AlwId;
                            policy.IncVal = IncVal;
                            policy.IncPer = IncPer;
                            policy.Status = Status == "1" ? true : false;
                            policy.DefinedBy = UserId;
                            policy.DefinedDate = DateTime.Now;
                            await db.SaveChangesAsync();

                            var DtlSlab = await db.Pay_IncentivePolicyLocations.Where(x => x.PolicyId == PolicyId).ToListAsync();
                            db.Pay_IncentivePolicyLocations.RemoveRange(DtlSlab);
                            await db.SaveChangesAsync();
                            await AddBSPCLINCPOL(PolicyId, citylst, loclst);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        var ExistPolicy = await db.Pay_IncentivePolicy.Where(x => x.DesgId == DesgId && x.AlwId == AlwId).FirstOrDefaultAsync();
                        if (ExistPolicy == null)
                        {
                            var Desg = await DesignationById(DesgId);
                            Pay_IncentivePolicy SalarySalb = new Pay_IncentivePolicy()
                            {
                                PolicyTitle = "Basic Salary Policy " + Desg.DesgName,
                                DesgId = DesgId,
                                AlwId = AlwId,
                                IncVal = IncVal,
                                IncPer = IncPer,
                                DefinedBy = UserId,
                                DefinedDate = DateTime.Now,
                                Status = true
                            };
                            db.Pay_IncentivePolicy.Add(SalarySalb);
                            await db.SaveChangesAsync();

                            await AddBSPCLINCPOL(SalarySalb.PolicyId, citylst, loclst);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
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

        public async Task<int> AddBSPCLINCPOL(int PolicyId, int[] Cities, int[] Locations)
        {
            try
            {
                int bspId = PolicyId;
                if (Cities.Length > 0 && Cities[0] > 0)
                {
                    if (Locations.Length > 0 && Locations[0] > 0)
                    {
                        var bsplList = new List<Pay_IncentivePolicyLocations>();
                        var LCTs = new List<int>();
                        foreach (var l in Locations)
                        {
                            var cityID = db.Comp_Locations.First(x => x.LocId == l).CityId;
                            if (Cities.Contains(cityID))
                            {
                                LCTs.Add(cityID);
                            }
                            var bspl = new Pay_IncentivePolicyLocations
                            {
                                PolicyId = bspId,
                                LocId = l,
                                CityId = cityID
                            };
                            bsplList.Add(bspl);
                        }
                        db.Pay_IncentivePolicyLocations.AddRange(bsplList);
                        await db.SaveChangesAsync();

                        var CitiesOnly = Cities.Except(LCTs);
                        AddBSPINCPOL(CitiesOnly.ToArray(), bspId);
                    }
                    else
                    {
                        AddBSPINCPOL(Cities, bspId);
                    }
                }
                return bspId;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public bool AddBSPINCPOL(int[] Cities, int bspId)
        {
            try
            {
                var bsplList = new List<Pay_IncentivePolicyLocations>();
                foreach (var c in Cities)
                {
                    var bspl = new Pay_IncentivePolicyLocations
                    {
                        PolicyId = bspId,
                        LocId = 0,
                        CityId = c
                    };
                    bsplList.Add(bspl);
                }
                db.Pay_IncentivePolicyLocations.AddRange(bsplList);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
        #region Employee Bank Salary


        public async Task<List<EmployeeBankSalary>> GetEmployeeSalaryByDept(int deptid, int desgid, DateTime? FDate, DateTime? TDate)
        {
            if (FDate != null && TDate != null)
            {
                var empbanksal = await (from empdet in db.Pay_EmpMaster
                                            //   join empsal in db.Pay_EmpSalary on empdet.EmpId equals empsal.EmpId
                                        join empbsal in db.Pay_EmpBankSalary on empdet.EmpId equals empbsal.EmpId into es
                                        join bankacc in db.Pay_EmpBank on empdet.EmpId equals bankacc.EmpId into ds
                                        join dept in db.Pay_Department on empdet.DeptId equals dept.DeptId
                                        join desg in db.Pay_Designation on empdet.DesgId equals desg.DesgId
                                        where (deptid == 0 || dept.DeptId == deptid)
                                        && (desgid == 0 || desg.DesgId == desgid)
                                        && empdet.StatusId == "A"
                                        //   && empsal.Salary > 0
                                        //  && empsal.SalTypeId == "B"
                                        && empdet.DOJ >= FDate
                                        && empdet.DOJ <= TDate
                                        from ps in ds.DefaultIfEmpty()
                                        from ss in es.DefaultIfEmpty()
                                        select new EmployeeBankSalary()
                                        {
                                            TransId = empdet.EmpId,
                                            BankSalary = ss.BankSalary == null ? 0 : ss.BankSalary,
                                            //      FixedSalary = empsal.Salary,
                                            CNIC = empdet.CNIC,
                                            Designation = desg.DesgName,
                                            EmpId = empdet.EmpId,
                                            EmpName = empdet.EmpName,
                                            AccountNumber = ps.AccountNumber,
                                            Bank = ps.Bank,
                                            StartRange = ss.StartRange == null ? 0 : ss.StartRange,
                                            EndRange = ss.EndRange == null ? 0 : ss.EndRange,
                                            IsFullBank = ss.IsFullBank == null ? false : ss.IsFullBank,
                                            AccountTitle = ps.AccountTitle == null ? " " : ps.AccountTitle
                                        }).ToListAsync();
                return empbanksal;
            }
            else
            {
                var empbanksal = await (from empdet in db.Pay_EmpMaster
                                            // join empsal in db.Pay_EmpSalary on empdet.EmpId equals empsal.EmpId
                                        join empbsal in db.Pay_EmpBankSalary on empdet.EmpId equals empbsal.EmpId into es
                                        join bankacc in db.Pay_EmpBank on empdet.EmpId equals bankacc.EmpId into ds
                                        join dept in db.Pay_Department on empdet.DeptId equals dept.DeptId
                                        join desg in db.Pay_Designation on empdet.DesgId equals desg.DesgId
                                        where (deptid == 0 || dept.DeptId == deptid)
                                        && (desgid == 0 || desg.DesgId == desgid)
                                         && empdet.StatusId == "A"
                                        //  && empsal.Salary > 0
                                        // && empsal.SalTypeId == "B"
                                        from ps in ds.DefaultIfEmpty()
                                        from ss in es.DefaultIfEmpty()
                                        select new EmployeeBankSalary()
                                        {
                                            TransId = empdet.EmpId,
                                            BankSalary = ss.BankSalary,
                                            //  FixedSalary = empsal.Salary,
                                            CNIC = empdet.CNIC,
                                            Designation = desg.DesgName,
                                            EmpId = empdet.EmpId,
                                            EmpName = empdet.EmpName,
                                            AccountNumber = ps.AccountNumber,
                                            Bank = ps.Bank,
                                            StartRange = ss.StartRange == null ? 0 : ss.StartRange,
                                            EndRange = ss.EndRange == null ? 0 : ss.EndRange,
                                            IsFullBank = ss.IsFullBank == null ? false : ss.IsFullBank,
                                            AccountTitle = ps.AccountTitle == null ? " " : ps.AccountTitle
                                        }).ToListAsync();
                return empbanksal;
            }
        }


        public async Task<bool> CreateUpdateEmployeeBankSalary(List<EmployeeBankSalary> mod, int UserId)
        {
            try
            {
                if (mod.Count > 0)
                {
                    foreach (var item in mod)
                    {
                        // var empsalary = db.Pay_EmpSalary.Where(x => x.EmpId == item.EmpId && x.SalTypeId == "B").FirstOrDefault();
                        var empsalary = await db.Pay_EmpBankSalary.Where(x => x.EmpId == item.EmpId).FirstOrDefaultAsync();
                        if (empsalary != null && item.AccountNumber != "")
                        {
                            empsalary.BankSalary = item.BankSalary;
                            empsalary.DefinedBy = UserId;
                            empsalary.DefinedDate = DateTime.Now;
                            empsalary.StartRange = item.StartRange;
                            empsalary.EndRange = item.EndRange;
                            empsalary.IsFullBank = item.IsFullBank;
                            await db.SaveChangesAsync();

                        }
                        else
                        {
                            Pay_EmpBankSalary ebs = new Pay_EmpBankSalary()
                            {
                                BankSalary = item.BankSalary,
                                DefinedBy = UserId,
                                DefinedDate = DateTime.Now,
                                EmpId = item.EmpId,
                                EndRange = item.EndRange,
                                IsFullBank = item.IsFullBank,
                                StartRange = item.StartRange
                            };
                            db.Pay_EmpBankSalary.Add(ebs);
                            await db.SaveChangesAsync();

                        }

                        if (!String.IsNullOrWhiteSpace(item.AccountNumber))
                        {

                            var IsAlreadyExist = await db.Pay_EmpBank.Where(x => x.EmpId == item.EmpId).FirstOrDefaultAsync();
                            if (IsAlreadyExist == null)
                            {
                                var emp = await new EmployeeBL().GetEmployeeById(item.EmpId);
                                Pay_EmpBank peb = new Pay_EmpBank()
                                {
                                    AccountNumber = item.AccountNumber,
                                    AccountStatus = "Active",
                                    DefinedDate = DateTime.Now,
                                    DefinedBy = UserId,
                                    AccountTitle = emp.EmpName.ToUpper(),
                                    Bank = item.Bank,
                                    Branch = "N/A",
                                    EmpId = item.EmpId
                                };
                                db.Pay_EmpBank.Add(peb);
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                var emp = await new EmployeeBL().GetEmployeeById(item.EmpId);
                                IsAlreadyExist.AccountNumber = item.AccountNumber;
                                IsAlreadyExist.AccountStatus = "Active";
                                IsAlreadyExist.DefinedDate = DateTime.Now;
                                IsAlreadyExist.DefinedBy = UserId;
                                IsAlreadyExist.AccountTitle = emp.EmpName.ToUpper();
                                IsAlreadyExist.Bank = item.Bank;
                                await db.SaveChangesAsync();
                            }

                        }
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


        #endregion
        #region SalaryDisbursmentControlPanel


        //public async Task<Salarydi>
        public async Task<SalaryControlPanelVM> GetSalaryDisbursementById(int id)
        {
            return await db.Pay_SalaryDisbursementControlPanel.Where(x => x.PanelId == id)
                .Select(x => new SalaryControlPanelVM()
                {
                    AllowDisbursement = x.AllowDisbursement,
                    PanelId = x.PanelId,
                    AllowFinalized = x.AllowFinalized,
                    CityId = x.CityId,
                    DeptId = x.DeptId,
                    DefinedBy = x.DefinedBy,
                    DefinedDate = x.DefinedDate,
                    DesignationId = x.DesignationId,
                    DisbursementEndDate = x.DisbursementEndDate,
                    DisbursementStartDate = x.DisbursementStartDate,
                    HDeptId = x.HDeptId,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                    SalaryMonth = x.SalaryMonth,
                    SectionId = x.SectionId,
                    DisbursementTypeId = x.DisbursementTypeId
                }).FirstOrDefaultAsync();
        }
        public List<SalaryControlPanelListVM> GetSalaryDisbursment(DateTime startdate, DateTime enddate)
        {
            var SalDis = db.spPay_SalaryDisbursementControlPanel().Where(x => x.DisbursementStartDate >= startdate.Date && x.DisbursementEndDate <= enddate.Date).Select(x => new SalaryControlPanelListVM()
            {
                AllowDisbursement = x.AllowDisbursement,
                AllowFinalized = x.AllowFinalized,
                City = x.City,
                Department = x.Department,
                Designation = x.Designation,
                DisbursementEndDate = x.DisbursementEndDate,
                DisbursementStartDate = x.DisbursementStartDate,
                HDeptName = x.HDeptName,
                PanelId = x.PanelId,
                SalaryMonth = x.SalaryMonth,
                Section = x.Section
            }).ToList();
            return SalDis;
        }

        public async Task<bool> AddSalaryDisbursment(SalaryControlPanelVM mod, int Userid)
        {
            try
            {
                Pay_SalaryDisbursementControlPanel md = new Pay_SalaryDisbursementControlPanel()
                {
                    DisbursementStartDate = mod.DisbursementStartDate,
                    AllowDisbursement = mod.AllowDisbursement,
                    AllowFinalized = mod.AllowFinalized,
                    DefinedBy = Userid,
                    DefinedDate = DateTime.Now,
                    DeptId = mod.DeptId == null ? 0 : Convert.ToInt32(mod.DeptId),
                    DesignationId = mod.DesignationId == null ? 0 : Convert.ToInt32(mod.DesignationId),
                    HDeptId = mod.HDeptId,
                    DisbursementEndDate = mod.DisbursementEndDate,
                    SalaryMonth = mod.SalaryMonth,
                    CityId = mod.CityId == null ? 0 : Convert.ToInt32(mod.CityId),
                    DisbursementTypeId = mod.DisbursementTypeId,
                    SectionId = mod.SectionId == null ? 0 : Convert.ToInt32(mod.SectionId)
                };
                db.Pay_SalaryDisbursementControlPanel.Add(md);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> EditSalaryDisbursment(SalaryControlPanelVM mod, int Userid)
        {
            try
            {
                var saldis = db.Pay_SalaryDisbursementControlPanel.Where(x => x.PanelId == mod.PanelId).FirstOrDefault();
                saldis.DisbursementStartDate = mod.DisbursementStartDate;
                saldis.AllowDisbursement = mod.AllowDisbursement;
                saldis.AllowFinalized = mod.AllowFinalized;
                saldis.ModifiedBy = Userid;
                saldis.ModifiedDate = DateTime.Now;
                saldis.DeptId = mod.DeptId == null ? 0 : Convert.ToInt32(mod.DeptId);
                saldis.DesignationId = mod.DesignationId == null ? 0 : Convert.ToInt32(mod.DesignationId);
                saldis.HDeptId = mod.HDeptId;
                saldis.CityId = mod.CityId == null ? 0 : Convert.ToInt32(mod.CityId);
                saldis.DisbursementEndDate = mod.DisbursementEndDate;
                saldis.SalaryMonth = mod.SalaryMonth;
                saldis.DisbursementTypeId = mod.DisbursementTypeId;
                saldis.SectionId = mod.SectionId == null ? 0 : Convert.ToInt32(mod.SectionId);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }



        #endregion
        #region BankLetter

        public List<BankLetterVM> GetBankLetterInc()
        {
            return db.Pay_BankSalaryTransfer.Where(x => x.BLStatus == "P").ToList().Select(x => new BankLetterVM()
            {
                BlId = x.BLId,
                BLTitle = x.BLTitle,
                SalaryMonth = x.SalaryMonth,
            }).OrderBy(x => x.SalaryMonth).ToList();
        }


        public async Task<List<BankLetterPostingVM>> GetBankLetterPosting(DateTime SalaryMonth, string status)
        {
            try
            {
                return await (from item in db.Pay_BankSalaryTransfer
                              join prepared in db.Pay_EmpMaster on item.DefinedBy equals prepared.EmpId into pr
                              from p in pr.DefaultIfEmpty()
                              join posted in db.Pay_EmpMaster on item.PostedBy equals posted.EmpId into pp
                              from pe in pp.DefaultIfEmpty()
                              where DbFunctions.TruncateTime(item.SalaryMonth) == DbFunctions.TruncateTime(SalaryMonth.Date)
                              && (item.BLStatus == status || status == "0")
                              select new BankLetterPostingVM()
                              {
                                  LetterTitle = item.BLTitle,

                                  BlId = item.BLId,
                                  PreparedBy = p.EmpName,
                                  PreparedDate = item.DefinedDate,
                                  PostedBy = pe.EmpName,
                                  PostedDate = item.PostedDate,
                                  Status = item.BLStatus
                              }).ToListAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<BankLetterVM>> GetBankLetterDetail(int blid)
        {
            try
            {
                return await (from item in db.Pay_BankSalaryTransferDetail
                              join prepared in db.Pay_EmpMaster on item.EmpId equals prepared.EmpId
                              join desg in db.Pay_Designation on prepared.DesgId equals desg.DesgId
                              join dept in db.Pay_Department on prepared.DeptId equals dept.DeptId
                              where item.BLId == blid && item.IsDeleted == false
                              select new BankLetterVM()
                              {
                                  EmpId = prepared.EmpId,
                                  EmpName = prepared.EmpName,
                                  BankSalary = item.BankSalary,
                                  DesgName = desg.DesgName,
                                  DeptName = dept.DeptName
                              }).ToListAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> EditBankLetter(int blid, int EMpid)
        {
            try
            {
                var bklett = await db.Pay_BankSalaryTransferDetail.Where(x => x.BLId == blid && x.EmpId == EMpid && x.IsDeleted == false).FirstOrDefaultAsync();
                if (bklett != null)
                {
                    bklett.IsDeleted = true;
                    await db.SaveChangesAsync();
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<BankLetterVM> GetBankLetter(int HdeptId, int DeptId, int CityId, int DesgId, decimal FromSalary, decimal ToSalary, string EmpStatus, DateTime SalMon)
        {
            List<BankLetterVM> banklet = new List<BankLetterVM>();
            var bnkletter = db.spPay_GetData_4_Bank_Letter(SalMon, HdeptId, DeptId, CityId, DesgId, EmpStatus, FromSalary, ToSalary).ToList();
            foreach (var item in bnkletter)
            {
                BankLetterVM BnkLett = new BankLetterVM()
                {
                    AccountNumber = item.AccountNumber,
                    Bank = item.Bank,
                    BankSalary = item.BankSalary,
                    CNIC = item.CNIC,
                    DeptName = item.DeptName,
                    DesgName = item.DesgName,
                    DOJ = item.DOJ,
                    EmpId = item.EmpId,
                    EmpName = item.EmpName,
                    EmpStatus = item.EmpStatus,
                    SortOrder = item.SortOrder,
                    SheetDtlId = item.SheetDtlId
                };
                banklet.Add(BnkLett);
            }
            return banklet;
        }
        public List<BankLetterVM> GetBankLetterInfo(List<BankLetterUploader> mod, DateTime Month)
        {
            List<BankLetterVM> md = new List<BankLetterVM>();
            var emps = mod.Select(x => x.EmpId);
            var EmpIds = String.Join(",", emps);
            var bankletterdat = db.spPay_GetData_4_Bank_Letter_Uploader(Month, EmpIds);
            foreach (var item in bankletterdat)
            {
                BankLetterVM mds = new BankLetterVM();
                mds.SheetDtlId = item.SheetDtlId;
                mds.EmpId = item.EmpId;
                mds.EmpName = item.EmpName;
                mds.CNIC = item.CNIC;
                mds.DesgName = item.DesgName;
                mds.EmpStatus = item.EmpStatus;
                mds.DOJ = item.DOJ;
                mds.DeptName = item.DeptName;
                mds.BankSalary = item.BankSalary;
                mds.AccountNumber = item.AccountNumber;
                mds.Bank = item.Bank;
                mds.SortOrder = item.SortOrder;
                mds.e = item.EmpId;
                mds.b = item.BankSalary;
                mds.s = item.SheetDtlId;
                md.Add(mds);
            }
            return md;
        }
        public async Task<bool> SaveBankLetter(string BLTitle, DateTime SalMon, List<BankLetterVM> BankLetVM, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    Pay_BankSalaryTransfer mod = new Pay_BankSalaryTransfer()
                    {
                        BLTitle = BLTitle,
                        SalaryMonth = SalMon,
                        BLStatus = "D",
                        DefinedBy = UserId,
                        DefinedDate = DateTime.Now
                    };
                    db.Pay_BankSalaryTransfer.Add(mod);
                    await db.SaveChangesAsync();

                    foreach (var item in BankLetVM)
                    {
                        Pay_BankSalaryTransferDetail ModDet = new Pay_BankSalaryTransferDetail()
                        {
                            BLId = mod.BLId,
                            EmpId = item.e,
                            BankSalary = item.b,
                            SheetDtlId = item.s,
                            IsDeleted = false
                        };
                        db.Pay_BankSalaryTransferDetail.Add(ModDet);
                        await db.SaveChangesAsync();
                    }
                    scop.Complete();
                    scop.Dispose();
                    return true;
                }
                catch (Exception e)
                {
                    scop.Dispose();
                    return false;

                }
            }
        }

        public List<string> BankLetterVoucherPosting(List<int> mod, string bankcode, int UserId, DateTime postdate)
        {
            List<string> _Str = new List<string>();
            string str = "";
            if (mod != null)
            {
                foreach (var item in mod)
                {
                    str = db.spPay_Post_Bank_Salary(item, bankcode, UserId, postdate).FirstOrDefault();
                    if (str != "Voucher Posted Successfully!")
                    {
                        _Str.Add(str + "<br>");
                    }
                    else
                    {
                        _Str.Add(str + "<br>");
                    }
                }
                return _Str;
            }
            else
            {
                str = "No Data Present";
                _Str.Add(str);
                return _Str;
            }
        }
        #endregion
        #region Employee Incentive
        public List<PaySalaryIncentiveVM> GetSalaryIncentive(DateTime month, int DisbursementTypeId, int CityId)
        {
            return db.spPay_GetTargetIncentives_4_Approval(month, DisbursementTypeId, CityId).Select(x => new PaySalaryIncentiveVM()
            {
                EmpId = x.EmpId,
                TIId = x.TIId,
                IncAmount = x.IncAmount,
                IncPayable = x.IncPayable,
                UsedInClosing = x.UsedInClosing,
                Designation = x.DesgName,
                Employee = x.EmpName,
                Location = x.LocCode,
                selectable = true,
                ApprovedPayable = x.ApprovedPayable,
                ApprovedDate = x.ApprovedDate,
                DisbursementType = x.DisbursementType,
                AllowDisbursement = x.AllowDisbursement

            }).ToList();
        }

        public async Task<bool> AllowIncDisbursement(int TiId)
        {
            try
            {
                var Inc = await db.Pay_SalarySheet_TI.Where(x => x.TIId == TiId && x.ApprovedBy != null).FirstOrDefaultAsync();
                Inc.AllowDisbursement = true;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ApproveIncentive(IEnumerable<PaySalaryIncentiveVM> mod, int UserId, int TiType, DateTime Month)
        {
            try
            {
                foreach (var item in mod)
                {
                    if (item.selectable == true)
                    {
                        var Inc = db.Pay_SalarySheet_TI.Where(x => x.TIId == item.TIId).FirstOrDefault();
                        if (Inc.ApprovedBy == null)
                        {
                            Inc.ApprovedBy = UserId;
                            Inc.ApprovedIncPayable = Convert.ToDecimal(item.ApprovedPayable);
                            Inc.ApprovedDate = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                }
                //if (TiType == 3)
                //{
                //    db.spPay_Post_Target_Incentives(Month, UserId, TiType);
                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        #endregion
        #region GeneralFunctions
        public async Task<List<EmployeeVM>> EmpByDeptList(int DeptId)
        {
            try
            {
                return await db.Pay_EmpMaster.Where(x => x.StatusId == "A" && x.DeptId == DeptId)
                    .Select(x => new EmployeeVM
                    {
                        EmpId = x.EmpId,
                        EmpName = x.EmpName,
                        CNIC = x.CNIC,
                        DeptName = x.Pay_Department.DeptName,
                        DesgName = x.Pay_Designation.DesgName
                    }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<EmpListCRCVM> EmpListByPolicy(long accno, int PolicyId, int LocId)
        {
            if (accno > 0 || PolicyId > 0 || LocId > 0)
            {
                return db.spGet_EmpForFine(accno, PolicyId, LocId)
                        .Select(x => new EmpListCRCVM
                        {
                            EmpId = x.EmpId,
                            EmpName = x.EmpName,
                            DesgName = x.DesgName,
                            Amount = x.FineAmt
                        }).ToList();
            }
            else
            {
                return new List<EmpListCRCVM>();
            }
        }
        public async Task<List<EmployeeVM>> EmpList(int LocId)
        {
            try
            {
                var loc = await db.Comp_Locations.FindAsync(LocId);
                if (loc.LocTypeId == 1)
                {
                    var empLst = db.spget_Pay_EmpTemplateByLoc(LocId);
                    return await db.Pay_EmpMaster.Where(x => empLst.Contains(x.EmpId))
                    .Select(x => new EmployeeVM
                    {
                        EmpId = x.EmpId,
                        EmpName = x.EmpName,
                        CNIC = x.CNIC,
                        DeptName = x.Pay_Department.DeptName,
                        DesgName = x.Pay_Designation.DesgName,
                        DesgId = x.DesgId
                    }).ToListAsync();
                }
                return db.Pay_EmpMaster.Where(x => x.StatusId == "A").ToList()
                    .Select(x => new EmployeeVM
                    {
                        EmpId = x.EmpId,
                        EmpName = x.EmpName,
                        CNIC = x.CNIC,
                        DeptName = x.Pay_Department.DeptName,
                        DesgName = x.Pay_Designation.DesgName,
                        DesgId = x.DesgId
                    }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public async Task<List<EmployeeVM>> EmpListIALst(int LocId)
        {
            try
            {
                var loc = await db.Comp_Locations.FindAsync(LocId);
                if (loc.LocTypeId == 1)
                {
                    var empLst = db.spget_Pay_EmpTemplateByLoc(LocId);
                    return await db.Pay_EmpMaster.Where(x => empLst.Contains(x.EmpId))
                    .Select(x => new EmployeeVM
                    {
                        EmpId = x.EmpId,
                        EmpName = x.EmpName,
                        CNIC = x.CNIC,
                        DeptName = x.Pay_Department.DeptName,
                        DesgName = x.Pay_Designation.DesgName,
                        DesgId = x.DesgId,
                        Status = x.StatusId
                    }).ToListAsync();
                }

                var CurrDate = DateTime.Now.AddMonths(-1);
                var salesemp = await db.Pay_EmpJoiningLog.Where(x => x.FinalDate > CurrDate && (new List<string> { "I", "T", "R" }).Contains(x.Status)).Select(x => x.EmpId).ToListAsync();

                return db.Pay_EmpMaster.Where(x => x.StatusId == "A" || salesemp.Contains(x.EmpId)).ToList()
                    .Select(x => new EmployeeVM
                    {
                        EmpId = x.EmpId,
                        EmpName = x.EmpName,
                        CNIC = x.CNIC,
                        DeptName = x.Pay_Department.DeptName,
                        DesgName = x.Pay_Designation.DesgName,
                        DesgId = x.DesgId,
                        Status = x.StatusId
                    }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<List<EmployeeVM>> EmpListActive()
        {
            try
            {
                return await db.Pay_EmpMaster.Where(x => x.StatusId == "A" && x.DeptId == 1014)
                   .Select(x => new EmployeeVM
                   {
                       EmpId = x.EmpId,
                       EmpName = x.EmpName,
                       CNIC = x.CNIC,
                       DeptName = x.Pay_Department.DeptName,
                       DesgName = x.Pay_Designation.DesgName
                   }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<DesignationSectionVM>> GetSectionByDesignation(int desgid)
        {
            try
            {
                return await (from item in db.Pay_Designation
                              join section in db.Pay_DesignationSection on item.SectionId equals section.SectionId into ps
                              from p in ps.DefaultIfEmpty()
                              where item.DesgId == desgid || desgid == 0
                              select new DesignationSectionVM()
                              {
                                  SectionId = p.SectionId,
                                  SectionTitle = p.SectionTitle
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<List<EmployeeVM>> EmpTranListActive()
        {
            try
            {
                return await db.Pay_EmpMaster.Where(x => x.StatusId == "A")
                   .Select(x => new EmployeeVM
                   {
                       EmpId = x.EmpId,
                       EmpName = x.EmpName,
                       CNIC = x.CNIC,
                       LocId = x.LocId,
                       DeptName = x.Pay_Department.DeptName,
                       DesgName = x.Pay_Designation.DesgName
                   }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<EmployeeVM>> EmpTranListActiveByLoc(int LocId)
        {
            try
            {
                return await db.Pay_EmpMaster.Where(x => x.StatusId == "A" && x.LocId == LocId)
                   .Select(x => new EmployeeVM
                   {
                       EmpId = x.EmpId,
                       EmpName = x.EmpName,
                       CNIC = x.CNIC,
                       LocId = x.LocId,
                       DeptName = x.Pay_Department.DeptName,
                       DesgName = x.Pay_Designation.DesgName
                   }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<EmployeeVM>> EmpListInActive(int LocId)
        {
            try
            {
                string[] ls = new[] { "I", "R", "T" };
                if (LocId == 72)
                {
                    var lst = await (from x in db.Pay_EmpMaster
                                     join item in db.Pay_Department on x.DeptId equals item.DeptId
                                     join j in db.Pay_EmpJoiningLog on x.EmpId equals j.EmpId into ps
                                     from j in ps.DefaultIfEmpty()
                                     where ls.Contains(x.StatusId)
                                     select new EmployeeVM
                                     {
                                         EmpId = x.EmpId,
                                         EmpName = x.EmpName,
                                         CNIC = x.CNIC,
                                         DeptId = x.DeptId,
                                         DesgId = x.DesgId,
                                         DeptName = x.Pay_Department.DeptName,
                                         DesgName = x.Pay_Designation.DesgName,
                                         Status = x.StatusId,
                                         FinalReason = j.FinalReason
                                     }).Distinct().ToListAsync();
                    return lst;
                }
                else
                {
                    var lst = await (from x in db.Pay_EmpMaster
                                     join item in db.Pay_Department on x.DeptId equals item.DeptId
                                     join j in db.Pay_EmpJoiningLog on x.EmpId equals j.EmpId into ps
                                     from j in ps.DefaultIfEmpty()
                                     where ls.Contains(x.StatusId) && item.LocId != 72
                                     select new EmployeeVM
                                     {
                                         EmpId = x.EmpId,
                                         EmpName = x.EmpName,
                                         CNIC = x.CNIC,
                                         DeptId = x.DeptId,
                                         DesgId = x.DesgId,
                                         DeptName = x.Pay_Department.DeptName,
                                         DesgName = x.Pay_Designation.DesgName,
                                         Status = x.StatusId,
                                         FinalReason = j.FinalReason
                                     }).Distinct().ToListAsync();
                    return lst;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<Pay_Department>> GetDeptList()
        {
            try
            {
                return await db.Pay_Department.Where(x => x.Status).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Pay_Department> GetDeptById(int Id)
        {
            try
            {
                return await db.Pay_Department.Where(x => x.DeptId == Id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Pay_Department>> DeptByLocList(int LocId)
        {
            try
            {
                return await db.Pay_Department.Where(x => x.Status && x.LocId == LocId).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Pay_Department>> DeptByHDeptList(int HDeptId)
        {
            try
            {
                return await db.Pay_Department.Where(x => x.Status && x.HDeptId == HDeptId).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<LocationVM>> DeptByHDeptListCP(int HDeptId, int CityId)
        {
            try
            {
                if (HDeptId == 1)
                {
                    return await (from p in db.Pay_Department
                                  join c in db.Comp_Locations on p.LocId equals c.LocId
                                  where c.LocId == 72
                                  select new LocationVM()
                                  {
                                      LocId = p.DeptId,
                                      LocCode = c.LocCode,
                                      LocName = p.DeptName
                                  }).ToListAsync();
                }
                else
                {
                    return await (from p in db.Pay_Department
                                  join c in db.Comp_Locations on p.LocId equals c.LocId
                                  where c.CityId == CityId || CityId == 0
                                  select new LocationVM()
                                  {
                                      LocId = c.LocId,
                                      LocCode = c.LocCode,
                                      LocName = p.DeptName
                                  }).ToListAsync();
                }

                //db.Pay_Department.Where(x => x.Status && x.HDeptId == HDeptId).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }



        public async Task<int> EmpTransferLog(EmpTransferLogVM mod, int UserId)
        {
            try
            {
                int GetBranchStrength = 0;
                int ExistEmpStrength = 0;
                if (mod.SEmpId == 0)
                {
                    var tbl1 = await db.Pay_EmpMaster.FirstOrDefaultAsync(x => x.EmpId == mod.EmpId);
                    GetBranchStrength = await GetHeadCountByDesgId(tbl1.DesgId, Convert.ToInt32(mod.TLocId));
                    ExistEmpStrength = await GetExisitingHeadCountByDesgId(tbl1.DesgId, Convert.ToInt32(mod.TLocId));
                }

                var Dept = await db.Pay_Department.Where(x => x.DeptId == mod.TLocId).FirstOrDefaultAsync();
                var AttCount = GetAttendanceCount(mod.EmpId);

                if (AttCount == 0) return 2;
                if (mod.EmpId == mod.SEmpId) return 0;

                //For Branches Only Strength will be checked
                if (Dept.HDeptId == 2)
                {
                    //if (mod.TrasferType == "T")
                    //{

                    //    if (mod.FLocId > 0 && mod.TLocId > 0)
                    //    {
                    //        Pay_EmpTransferLog tbl = new Pay_EmpTransferLog
                    //        {
                    //            EmpId = mod.EmpId,
                    //            FLocId = mod.FLocId,
                    //            TLocId = mod.TLocId,
                    //            FDate = mod.FDate,
                    //            TDate = mod.TDate,
                    //            TReason = mod.TReason,
                    //            TrasferType = mod.TrasferType,
                    //            TransDate = DateTime.Now,
                    //            UserId = UserId
                    //        };
                    //        db.Pay_EmpTransferLog.Add(tbl);
                    //        await db.SaveChangesAsync();
                    //        mod.RowId = tbl.RowId;
                    //        return 1;

                    //    }
                    //    else
                    //    {
                    //        return 0;
                    //    }
                    //}
                    //else
                    //{
                    if (mod.SEmpId > 0)
                    {
                        if (mod.FLocId > 0 && mod.TLocId > 0)
                        {
                            var tbl1 = await db.Pay_EmpMaster.FirstOrDefaultAsync(x => x.EmpId == mod.EmpId);
                            //if (mod.TrasferType == "P")
                            //{
                            //var tbl1 = await db.Pay_EmpMaster.FirstOrDefaultAsync(x => x.EmpId == mod.EmpId);
                            if (tbl1 != null)
                            {
                                tbl1.LocId = mod.TLocId;
                                tbl1.DeptId = mod.TLocId;

                            }
                            await db.SaveChangesAsync();

                            var SEmp = await db.Pay_EmpMaster.FirstOrDefaultAsync(x => x.EmpId == mod.SEmpId);
                            if (tbl1 != null)
                            {
                                SEmp.LocId = mod.FLocId;
                                SEmp.DeptId = mod.FLocId;
                            }
                            //}

                            Pay_EmpTransferLog tbl = new Pay_EmpTransferLog
                            {
                                EmpId = mod.EmpId,
                                FLocId = mod.FLocId,
                                TLocId = mod.TLocId,
                                FDate = mod.FDate,
                                TDate = mod.TDate,
                                TReason = mod.TReason,
                                TrasferType = mod.TrasferType,
                                TransDate = DateTime.Now,
                                UserId = UserId
                            };
                            db.Pay_EmpTransferLog.Add(tbl);
                            await db.SaveChangesAsync();

                            Pay_EmpTransferLog tblone = new Pay_EmpTransferLog
                            {
                                EmpId = mod.SEmpId,
                                FLocId = mod.TLocId,
                                TLocId = mod.FLocId,
                                FDate = mod.FDate,
                                TDate = mod.TDate,
                                TReason = mod.TReason,
                                TrasferType = mod.TrasferType,
                                TransDate = DateTime.Now,
                                UserId = UserId
                            };
                            db.Pay_EmpTransferLog.Add(tblone);
                            await db.SaveChangesAsync();
                            mod.RowId = tbl.RowId;
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }


                    }
                    else if (ExistEmpStrength < GetBranchStrength)
                    {
                        if (mod.FLocId > 0 && mod.TLocId > 0)
                        {
                            //if (mod.TrasferType == "P")
                            //{
                            //var tbl1 = await db.Pay_EmpMaster.FirstOrDefaultAsync(x => x.EmpId == mod.EmpId);
                            var tbl1 = await db.Pay_EmpMaster.FirstOrDefaultAsync(x => x.EmpId == mod.EmpId);
                            if (tbl1 != null)
                            {
                                tbl1.LocId = mod.TLocId;
                                tbl1.DeptId = mod.TLocId;

                            }
                            await db.SaveChangesAsync();
                            //}

                            Pay_EmpTransferLog tbl = new Pay_EmpTransferLog
                            {
                                EmpId = mod.EmpId,
                                FLocId = mod.FLocId,
                                TLocId = mod.TLocId,
                                FDate = mod.FDate,
                                TDate = mod.TDate,
                                TReason = mod.TReason,
                                TrasferType = mod.TrasferType,
                                TransDate = DateTime.Now,
                                UserId = UserId
                            };
                            db.Pay_EmpTransferLog.Add(tbl);
                            await db.SaveChangesAsync();
                            mod.RowId = tbl.RowId;
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 3;
                    }
                    //}

                }
                else
                {
                    return 0;
                }

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<List<Pay_AttendanceFinal>> GetTotalPresentDays(int EmpId, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var presentdays = await db.Pay_AttendanceFinal.Where(x => x.AttendanceDate >= StartDate && x.AttendanceDate <= EndDate && x.EmpId == EmpId && (x.StatusId == "P" || x.StatusId == "R")).ToListAsync();
                if (presentdays.Count() >= 0)
                {
                    return presentdays;
                }
                else
                {
                    return new List<Pay_AttendanceFinal>();
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #region Holidays
        public async Task<List<HolidayVM>> GetHolidays()
        {
            return await db.Pay_Holidays.Where(x => x.Status).Select(x => new HolidayVM()
            {
                Holiday = x.Holiday,
                RowId = x.RowId
            }).ToListAsync();
        }

        public async Task<List<LocVM>> GetLocationsByCity(List<int> Cities)
        {
            return await db.Comp_Locations.Where(x => Cities.Contains(x.CityId)).Where(x => x.Status).Select(x => new LocVM()
            {
                LocId = x.LocId,
                LocName = x.LocName
            }).ToListAsync();
        }

        public async Task<bool> AddHolidays(IEnumerable<LocVM> Locs, DateTime StartDate, DateTime EndDate, string Remarks, int HolidayId, int UserId)
        {
            try
            {
                if (StartDate < DateTime.Now.Date && EndDate > DateTime.Now.Date)
                {
                    //Previous Dates Holidays
                    Pay_HolidayMaster hmp = new Pay_HolidayMaster()
                    {
                        StartDate = StartDate,
                        EndDate = DateTime.Now.AddDays(-1),
                        Remarks = Remarks,
                        TransDate = DateTime.Now,
                        Type = "M",
                        UserId = UserId,
                        HolidayId = HolidayId,
                        IsProcessed = false
                    };
                    db.Pay_HolidayMaster.Add(hmp);
                    await db.SaveChangesAsync();

                    foreach (var item in Locs)
                    {
                        if (item.selectable == true)
                        {
                            Pay_HolidayDetail hd = new Pay_HolidayDetail()
                            {
                                HId = hmp.RowId,
                                LocId = item.LocId
                            };
                            db.Pay_HolidayDetail.Add(hd);
                        }
                    }
                    db.spPay_ProcessManualHolidays(hmp.RowId);

                    //Next Days Holidays
                    Pay_HolidayMaster hmn = new Pay_HolidayMaster()
                    {
                        StartDate = DateTime.Now,
                        EndDate = EndDate,
                        Remarks = Remarks,
                        TransDate = DateTime.Now,
                        Type = "P",
                        UserId = UserId,
                        HolidayId = HolidayId,
                        IsProcessed = false
                    };
                    db.Pay_HolidayMaster.Add(hmn);
                    await db.SaveChangesAsync();

                    foreach (var item in Locs)
                    {
                        Pay_HolidayDetail hd = new Pay_HolidayDetail()
                        {
                            HId = hmn.RowId,
                            LocId = item.LocId
                        };
                        db.Pay_HolidayDetail.Add(hd);
                    }
                    await db.SaveChangesAsync();

                }
                else if (StartDate < DateTime.Now.Date && EndDate < DateTime.Now.Date)
                {
                    Pay_HolidayMaster hmp = new Pay_HolidayMaster()
                    {
                        StartDate = StartDate,
                        EndDate = EndDate,
                        Remarks = Remarks,
                        TransDate = DateTime.Now,
                        Type = "M",
                        UserId = UserId,
                        HolidayId = HolidayId,
                        IsProcessed = false
                    };
                    db.Pay_HolidayMaster.Add(hmp);
                    await db.SaveChangesAsync();

                    foreach (var item in Locs)
                    {
                        if (item.selectable == true)
                        {
                            Pay_HolidayDetail hd = new Pay_HolidayDetail()
                            {
                                HId = hmp.RowId,
                                LocId = item.LocId
                            };
                            db.Pay_HolidayDetail.Add(hd);
                        }
                    }
                    await db.SaveChangesAsync();
                    db.spPay_ProcessManualHolidays(hmp.RowId);
                }
                else if (StartDate >= DateTime.Now.Date && EndDate >= DateTime.Now.Date)
                {
                    Pay_HolidayMaster hmn = new Pay_HolidayMaster()
                    {
                        StartDate = DateTime.Now,
                        EndDate = EndDate,
                        Remarks = Remarks,
                        TransDate = DateTime.Now,
                        Type = "P",
                        UserId = UserId,
                        HolidayId = HolidayId,
                        IsProcessed = false
                    };
                    db.Pay_HolidayMaster.Add(hmn);
                    await db.SaveChangesAsync();

                    foreach (var item in Locs)
                    {
                        Pay_HolidayDetail hd = new Pay_HolidayDetail()
                        {
                            HId = hmn.RowId,
                            LocId = item.LocId
                        };
                        db.Pay_HolidayDetail.Add(hd);
                    }
                    await db.SaveChangesAsync();
                }
                else
                {

                    //List<Pay_EmpMaster> emps = new List<Pay_EmpMaster>();
                    //foreach (var item in mod.LocId)
                    //{
                    //    var employees = await db.Pay_EmpMaster.Where(x => x.LocId == item).ToListAsync();
                    //    emps.AddRange(employees);
                    //}

                    //foreach (var item in emps)
                    //{
                    //    var attendance = await db.Pay_AttendanceFinal.Where(x => x.AttendanceDate >= hm.StartDate && x.AttendanceDate <= hm.EndDate).ToListAsync();

                    //}
                }
                return true;
            }
            catch (Exception e)
            {
                return false;

            }
        }


        #endregion

        public async Task<List<EmpHierarchyVM>> GetEmployeeHierarchy()
        {
            try
            {
                var lst = await db.Pay_EmpHierarchy.
                                   Select(x => new EmpHierarchyVM
                                   {
                                       Auditor = x.Auditor,
                                       AuditorId = x.AuditorId ?? 0,
                                       BDM = x.BDM,
                                       BDMId = x.BDMId ?? 0,
                                       CashHead = x.CashHead,
                                       CashHeadId = x.CashHeadId ?? 0,
                                       CRCHead = x.CRCHead,
                                       CRCHeadId = x.CRCHeadId ?? 0,
                                       GM = x.GM,
                                       GMId = x.GMId ?? 0,
                                       LocId = x.LocId,
                                       RAuditor = x.RAuditor,
                                       RAuditorId = x.RAuditorId ?? 0,
                                       RegionalCashHead = x.RegionalCashHead,
                                       RegionalCashHeadId = x.RegionalCashHeadId ?? 0,
                                       RM = x.RM,
                                       RMId = x.RMId ?? 0,
                                       SAuditor = x.SAuditor,
                                       SAuditorId = x.SAuditorId ?? 0,
                                       SRM = x.SRM,
                                       SRMId = x.SRMId ?? 0,
                                       SSRM = x.SSRM,
                                       SSRMId = x.SSRMId ?? 0,
                                       CashSaleCoordinatorId = x.CashSaleCoordinatorId ?? 0,
                                       CashSaleCoordinator = x.CashSaleCoordinator,
                                       DGMId = x.DGMId ?? 0,
                                       DGM = x.DGM
                                   }).ToListAsync();

                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateEmployeeHierarchy(EmpHierarchyVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pay_EmpHierarchy.Where(x => x.LocId == mod.LocId).FirstOrDefaultAsync();
                if ((tbl.AuditorId ?? 0) != mod.AuditorId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.AuditorId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 252, Remarks = "Hierarchy Change" });
                    if (mod.AuditorId != 0)
                    {
                        tbl.AuditorId = mod.AuditorId;
                        tbl.Auditor = await GetEmpNameById(mod.AuditorId);
                    }
                    else
                    {
                        tbl.AuditorId = null;
                        tbl.Auditor = "Vacant";
                    }
                }
                if ((tbl.BDMId ?? 0) != mod.BDMId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.BDMId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 253, Remarks = "Hierarchy Change" });
                    if (mod.BDMId != 0)
                    {
                        tbl.BDMId = mod.BDMId;
                        tbl.BDM = await GetEmpNameById(mod.BDMId);
                    }
                    else
                    {
                        tbl.BDMId = null;
                        tbl.BDM = "Vacant";
                    }
                }
                if ((tbl.CashHeadId ?? 0) != mod.CashHeadId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.CashHeadId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 256, Remarks = "Hierarchy Change" });
                    if (mod.CashHeadId != 0)
                    {
                        tbl.CashHeadId = mod.CashHeadId;
                        tbl.CashHead = await GetEmpNameById(mod.CashHeadId);
                    }
                    else
                    {
                        tbl.CashHeadId = null;
                        tbl.CashHead = "Vacant";
                    }
                }
                if ((tbl.CRCHeadId ?? 0) != mod.CRCHeadId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.CRCHeadId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 197, Remarks = "Hierarchy Change" });
                    if (mod.CRCHeadId != 0)
                    {
                        tbl.CRCHeadId = mod.CRCHeadId;
                        tbl.CRCHead = await GetEmpNameById(mod.CRCHeadId);
                    }
                    else
                    {
                        tbl.CRCHeadId = null;
                        tbl.CRCHead = "Vacant";
                    }
                }
                if ((tbl.GMId ?? 0) != mod.GMId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.GMId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 268, Remarks = "Hierarchy Change" });
                    if (mod.GMId != 0)
                    {
                        tbl.GMId = mod.GMId;
                        tbl.GM = await GetEmpNameById(mod.GMId);
                    }
                    else
                    {
                        tbl.GMId = null;
                        tbl.GM = "Vacant";
                    }
                }
                if ((tbl.DGMId ?? 0) != mod.DGMId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.DGMId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 261, Remarks = "Hierarchy Change" });
                    if (mod.DGMId != 0)
                    {
                        tbl.DGMId = mod.DGMId;
                        tbl.DGM = await GetEmpNameById(mod.DGMId);
                    }
                    else
                    {
                        tbl.DGMId = null;
                        tbl.DGM = "Vacant";
                    }
                }
                if ((tbl.RAuditorId ?? 0) != mod.RAuditorId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.RAuditorId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 401, Remarks = "Hierarchy Change" });
                    if (mod.RAuditorId != 0)
                    {
                        tbl.RAuditorId = mod.RAuditorId;
                        tbl.RAuditor = await GetEmpNameById(mod.RAuditorId);
                    }
                    else
                    {
                        tbl.RAuditorId = null;
                        tbl.RAuditor = "Vacant";
                    }
                }
                if ((tbl.RegionalCashHeadId ?? 0) != mod.RegionalCashHeadId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.RegionalCashHeadId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 283, Remarks = "Hierarchy Change" });
                    if (mod.RegionalCashHeadId != 0)
                    {
                        tbl.RegionalCashHeadId = mod.RegionalCashHeadId;
                        tbl.RegionalCashHead = await GetEmpNameById(mod.RegionalCashHeadId);
                    }
                    else
                    {
                        tbl.RegionalCashHeadId = null;
                        tbl.RegionalCashHead = "Vacant";
                    }
                }
                if ((tbl.RMId ?? 0) != mod.RMId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.RMId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 285, Remarks = "Hierarchy Change" });
                    if (mod.RMId != 0)
                    {
                        tbl.RMId = mod.RMId;
                        tbl.RM = await GetEmpNameById(mod.RMId);
                    }
                    else
                    {
                        tbl.RMId = null;
                        tbl.RM = "Vacant";
                    }
                }
                if ((tbl.SAuditorId ?? 0) != mod.SAuditorId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.SAuditorId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 290, Remarks = "Hierarchy Change" });
                    if (mod.SAuditorId != 0)
                    {
                        tbl.SAuditorId = mod.SAuditorId;
                        tbl.SAuditor = await GetEmpNameById(mod.SAuditorId);
                    }
                    else
                    {
                        tbl.SAuditorId = null;
                        tbl.SAuditor = "Vacant";
                    }
                }
                if ((tbl.SRMId ?? 0) != mod.SRMId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.SRMId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 291, Remarks = "Hierarchy Change" });
                    if (mod.SRMId != 0)
                    {
                        tbl.SRMId = mod.SRMId;
                        tbl.SRM = await GetEmpNameById(mod.SRMId);
                    }
                    else
                    {
                        tbl.SRMId = null;
                        tbl.SRM = "Vacant";
                    }
                }
                if ((tbl.SSRMId ?? 0) != mod.SSRMId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.SSRMId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 391, Remarks = "Hierarchy Change" });
                    if (mod.SSRMId != 0)
                    {
                        tbl.SSRMId = mod.SSRMId;
                        tbl.SSRM = await GetEmpNameById(mod.SSRMId);
                    }
                    else
                    {
                        tbl.SSRMId = null;
                        tbl.SSRM = "Vacant";
                    }
                }
                if ((tbl.CashSaleCoordinatorId ?? 0) != mod.CashSaleCoordinatorId)
                {
                    await SaveEmployeeStatusLog(new PayEmpStatusLogVM { EmpId = tbl.CashSaleCoordinatorId ?? 0, LocId = tbl.LocId, UserId = UserId, DesgId = 380, Remarks = "Hierarchy Change" });
                    if (mod.CashSaleCoordinatorId != 0)
                    {
                        tbl.CashSaleCoordinatorId = mod.CashSaleCoordinatorId;
                        tbl.CashSaleCoordinator = await GetEmpNameById(mod.CashSaleCoordinatorId);
                    }
                    else
                    {
                        tbl.CashSaleCoordinatorId = null;
                        tbl.CashSaleCoordinator = "Vacant";
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
        #endregion
        #region Gift Salary

        public List<DonationVM> GetDonationsForApproval(DateTime Month)
        {
            List<DonationVM> dlst = new List<DonationVM>();
            var donations = db.spGetDonationsForApproval(Month).Select(x => new DonationVM()
            {
                Amount = x.Amount,
                CDId = x.CDId,
                PersonId = x.PersonId,
                PersonName = x.PersonName,
                ApprovedBy = x.ApprovedBy,
                CashierId = x.CashierId,
                CDDtlId = x.CDDtlId
            }).ToList();
            return donations;
        }

        public async Task<List<EmployeeVM>> GetCashierList()
        {
            return await db.Pay_EmpMaster.Where(x => x.DesgId == 257 && x.StatusId == "A")
                .Select(x => new EmployeeVM()
                {
                    EmpName = x.EmpName,
                    EmpId = x.EmpId
                }).ToListAsync();
        }

        public async Task<bool> ApproveDonations(List<DonationVM> lst, int UserId)
        {
            try
            {
                foreach (var aitem in lst)
                {
                    if (aitem.CashierId > 0)
                    {
                        List<DonationVM> dlst = new List<DonationVM>();
                        var donations = await db.Pay_CharityDonationTransDtl.Where(item => item.CDId == aitem.CDId && item.CDDtlId == aitem.CDDtlId && item.PersonId == aitem.PersonId && item.ApprovedBy == null).FirstOrDefaultAsync();
                        donations.ApprovedBy = UserId;
                        donations.ApprovedDate = DateTime.Now;
                        await db.SaveChangesAsync();

                        var person = await db.Pay_CharityDonation.Where(x => x.PersonId == aitem.PersonId).FirstOrDefaultAsync();
                        if (person.CashierId != aitem.CashierId)
                        {
                            person.CashierId = aitem.CashierId;
                            await db.SaveChangesAsync();
                        }

                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<object> GetDonationDetail(int PersonId, DateTime Month)
        {
            var donation = await (from item in db.Pay_CharityDonationTrans
                                  join itemdetl in db.Pay_CharityDonationTransDtl on item.CDId equals itemdetl.CDId
                                  join dodetail in db.Pay_CharityDonation on itemdetl.PersonId equals dodetail.PersonId
                                  where item.CDMonth.Month == Month.Month && item.CDMonth.Year == Month.Year && item.ProcessType == "F" && itemdetl.ApprovedBy != null && itemdetl.PersonId == PersonId && itemdetl.PaidBy == null
                                  select new { Name = dodetail.PersonName, Amount = (itemdetl.Cash == 0 ? (itemdetl.Bank - itemdetl.Tax) : itemdetl.Cash  }).FirstOrDefaultAsync();
            return donation;
        }

        public async Task<List<DonationVM>> GetDonationListByCahier(DateTime Month, int CashierId)
        {
            var donation = await (from item in db.Pay_CharityDonationTrans
                                  join itemdetl in db.Pay_CharityDonationTransDtl on item.CDId equals itemdetl.CDId
                                  join dodetail in db.Pay_CharityDonation on itemdetl.PersonId equals dodetail.PersonId
                                  where item.CDMonth.Month == Month.Month && item.CDMonth.Year == Month.Year && item.ProcessType == "F" && itemdetl.ApprovedBy != null && itemdetl.PaidBy == null && dodetail.CashierId == CashierId
                                  select new DonationVM()
                                  {
                                      Amount = dodetail.Amount,
                                      PersonName = dodetail.PersonName,
                                      PersonId = dodetail.PersonId
                                  }).ToListAsync();
            return donation;
        }

        public async Task<bool> PayDonation(int PersonId, DateTime Month, int UserId, List<long> files)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var donation = await (from item in db.Pay_CharityDonationTrans
                                          join itemdetl in db.Pay_CharityDonationTransDtl on item.CDId equals itemdetl.CDId
                                          join dodetail in db.Pay_CharityDonation on itemdetl.PersonId equals dodetail.PersonId
                                          where item.CDMonth.Month == Month.Month && item.CDMonth.Year == Month.Year && item.ProcessType == "F" && itemdetl.ApprovedBy != null && itemdetl.PersonId == PersonId && itemdetl.PaidBy == null && dodetail.CashierId == UserId
                                          select itemdetl).FirstOrDefaultAsync();
                    donation.PaidBy = UserId;
                    donation.PaidDate = DateTime.Now;
                    await db.SaveChangesAsync();



                    scop.Complete();
                    scop.Dispose();

                    if (files != null && files.Count > 0)
                    {
                        await new DocumentBL().UpdateDocRef(files, donation.CDDtlId);
                    }
                    db.spPay_Post_CharityDonation(donation.CDDtlId);
                    return true;
                }
                catch (Exception e)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }

        public async Task<List<BranchLeaveVM>> GetBranchLeaves(int UserId, int LocId, DateTime date)
        {
            try
            {
                return db.spGet_BranchLeaves(UserId, LocId, date).Select(x => new BranchLeaveVM()
                {
                    DeptName = x.DeptName,
                    EmpId = x.EmpId,
                    LeaveId = x.LeaveId,
                    EmpName = x.EmpName,
                    LeaveFromDate = x.LeaveFromDate,
                    LeaveReason = x.LeaveReason,
                    LeaveToDate = x.LeaveToDate,
                    LeaveType = x.LeaveType,
                    DesgName = x.DesgName
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> ApproveRejectLeave(int LeaveId, bool IsApproved, int UserId, string remarks)
        {

            var leave = await db.Pay_EmpLeave.Where(x => x.LeaveId == LeaveId).FirstOrDefaultAsync();
            try
            {
                if (leave.LeaveStatus == "P")
                {
                    if (IsApproved == true)
                    {
                        leave.ApprovedBy = UserId;
                        leave.ApprovedDate = DateTime.Now;
                        leave.LeaveStatus = "P";
                        leave.Remarks = remarks;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        leave.LeaveStatus = "R";
                        leave.Remarks = remarks;
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



        #endregion
        #region Charity
        public async Task<List<CharityDonationVM>> CharityList(int status)
        {
            try
            {
                return await (from x in db.Pay_CharityDonation
                              join emp in db.Pay_EmpMaster on x.CashierId equals emp.EmpId
                              join loc in db.Comp_Locations on x.CCC equals loc.LocId
                              where x.Status == (status == 1 ? "A" : "I")

                              select new CharityDonationVM
                              {
                                  PersonId = x.PersonId,
                                  PersonName = x.PersonName,
                                  CNIC = x.CNIC,
                                  ContactNo = x.ContactNo,
                                  Reference = x.Reference,
                                  Amount = x.Amount,
                                  Tax = x.Tax,
                                  BankTransfer = x.BankTransfer,
                                  BankName = x.BankName,
                                  AccountNo = x.AccountNo,
                                  CashierId = x.CashierId ?? 0,
                                  Cashier = emp.EmpName,
                                  CCC = x.CCC ?? 0,
                                  CashCenter = loc.LocName,
                                  Status = x.Status,
                                  UserId = x.UserId,
                                  TransDate = x.TransDate,
                                  Remarks = x.Remarks,
                                  GLCode = x.GLCode
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<CharityDonationVM> CreateCharity(CharityDonationVM mod, int UserId)
        {
            try
            {
                Pay_CharityDonation tbl = new Pay_CharityDonation
                {
                    PersonName = mod.PersonName,
                    CNIC = mod.CNIC,
                    ContactNo = mod.ContactNo,
                    Reference = mod.Reference,
                    Amount = mod.Amount,
                    Tax = mod.Tax,
                    BankTransfer = mod.BankTransfer,
                    BankName = mod.BankName,
                    AccountNo = mod.AccountNo,
                    CCC = mod.CCC,
                    CashierId = mod.CashierId,
                    Status = mod.Status,
                    UserId = UserId,
                    TransDate = DateTime.Now,
                    Remarks = mod.Remarks,
                    GLCode = mod.GLCode
                };
                db.Pay_CharityDonation.Add(tbl);
                await db.SaveChangesAsync();
                mod.PersonId = tbl.PersonId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCharity(CharityDonationVM mod, int UserId)
        {
            try
            {

                var tbl = await db.Pay_CharityDonation.SingleOrDefaultAsync(x => x.PersonId.Equals(mod.PersonId));
                if (tbl != null)
                {

                    tbl.PersonName = mod.PersonName;
                    tbl.CNIC = mod.CNIC;
                    tbl.ContactNo = mod.ContactNo;
                    tbl.Reference = mod.Reference;
                    tbl.Amount = mod.Amount;
                    tbl.Tax = mod.Tax;
                    tbl.BankTransfer = mod.BankTransfer;
                    tbl.BankName = mod.BankName;
                    tbl.AccountNo = mod.AccountNo;
                    tbl.CCC = mod.CCC;
                    tbl.CashierId = mod.CashierId;
                    tbl.Status = mod.Status;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
                    tbl.Remarks = mod.Remarks;
                    tbl.GLCode = mod.GLCode;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyCharity(CharityDonationVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pay_CharityDonation.SingleOrDefaultAsync(x => x.PersonId.Equals(mod.PersonId));
                if (tbl != null)
                {
                    tbl.Status = "InActive";
                    tbl.UserId = UserId;

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
        #region BackEndPolicy
        public async Task<List<BankEndIncpolicyVM>> GetBackEndPolicyList(DateTime StartDate, DateTime EndDate)
        {
            return await db.Fin_BackendIncPolicy.Where(x => x.FromDate >= StartDate && x.ToDate <= EndDate).Select(x => new BankEndIncpolicyVM()
            {
                PolicyId = x.PolicyId,
                PolicyTitle = x.PolicyTitle,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                Status = x.Status,
                SupId = x.SupId
            }).ToListAsync();
        }

        public async Task<List<BackEndIncPolicyDtlVM>> GetBackEndPolicyDtlList(int PolicyId)
        {
            return await (from item in db.Fin_BackendIncPolicyDtl
                          join itm in db.Itm_Type on item.TypeId equals itm.TypeId
                          where item.PolicyId == PolicyId
                          select new BackEndIncPolicyDtlVM()
                          {
                              CompId = itm.ComId,
                              TypeId = itm.TypeId,
                              ProdId = itm.ProductId,
                              ModelId = item.ModelId,
                              SkuId = item.SkuId,
                              BasicTarget = item.BasicTarget,
                              IncPercent = item.IncPercent,
                              SalesmanInc = item.SalesmanInc,
                              LowerPortion = item.LowerPortion,
                              SpecialInc = item.SpecialInc,
                              PriceType = item.PriceType,
                              SerialNo = item.SerialNo == "0" ? "All" : item.SerialNo
                          }).ToListAsync();
        }


        public async Task<bool> AddEditBackEndPolicy(int PolicyId, string PolicyTitle, DateTime FromDate, DateTime ToDate, int SupId, string IncTypeId, int IncBaseId, string Status, IEnumerable<BackEndIncPolicyDtlVM> lstmd, int UserId)
        {
            if (PolicyId == 0)
            {
                try
                {
                    Fin_BackendIncPolicy md = new Fin_BackendIncPolicy()
                    {
                        PolicyTitle = PolicyTitle,
                        FromDate = FromDate,
                        ToDate = ToDate,
                        SupId = SupId,
                        IncTypeId = IncTypeId,
                        IncBaseId = IncBaseId,
                        Status = Status,
                        DefinedBy = UserId,
                        DefinedDate = DateTime.Now,

                    };
                    db.Fin_BackendIncPolicy.Add(md);
                    await db.SaveChangesAsync();
                    var lst = lstmd.ToList();
                    foreach (var item in lstmd)
                    {
                        var type = await db.Itm_Type.Where(x => x.ComId == item.CompId && x.ProductId == item.ProdId).FirstOrDefaultAsync();
                        Fin_BackendIncPolicyDtl mds = new Fin_BackendIncPolicyDtl()
                        {
                            PolicyId = md.PolicyId,
                            TypeId = type.TypeId,
                            ModelId = item.ModelId == 0 ? 0 : item.ModelId,
                            SkuId = item.SkuId == 0 ? 0 : item.SkuId,
                            BasicTarget = item.BasicTarget,
                            IncPercent = item.IncPercent,
                            SalesmanInc = item.SalesmanInc,
                            LowerPortion = item.LowerPortion,
                            SpecialInc = item.SpecialInc,
                            PriceType = item.PriceType,
                            SerialNo = item.SerialNo == "All" ? "0" : item.SerialNo

                        };
                        db.Fin_BackendIncPolicyDtl.Add(mds);
                        await db.SaveChangesAsync();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    var policy = await db.Fin_BackendIncPolicy.Where(x => x.PolicyId == PolicyId).FirstOrDefaultAsync();
                    policy.PolicyTitle = PolicyTitle;
                    policy.FromDate = FromDate;
                    policy.ToDate = ToDate;
                    policy.SupId = SupId;
                    policy.IncTypeId = IncTypeId;
                    policy.IncBaseId = IncBaseId;
                    policy.Status = Status;
                    policy.ModifiedBy = UserId;
                    policy.ModifiedDate = DateTime.Now;

                    await db.SaveChangesAsync();

                    var poldtllst = await db.Fin_BackendIncPolicyDtl.Where(x => x.PolicyId == PolicyId).ToListAsync();
                    db.Fin_BackendIncPolicyDtl.RemoveRange(poldtllst);
                    var lst = lstmd.ToList();
                    foreach (var item in lst)
                    {
                        var type = await db.Itm_Type.Where(x => x.ComId == item.CompId && x.ProductId == item.ProdId).FirstOrDefaultAsync();
                        Fin_BackendIncPolicyDtl mds = new Fin_BackendIncPolicyDtl()
                        {
                            PolicyId = policy.PolicyId,
                            TypeId = type.TypeId,
                            ModelId = item.ModelId == 0 ? 0 : item.ModelId,
                            SkuId = item.SkuId == 0 ? 0 : item.SkuId,
                            BasicTarget = item.BasicTarget,
                            IncPercent = item.IncPercent,
                            SalesmanInc = item.SalesmanInc,
                            LowerPortion = item.LowerPortion,
                            ModifiedBy = UserId,
                            SpecialInc = item.SpecialInc,
                            PriceType = item.PriceType,
                            ModifiedDate = DateTime.Now,
                            SerialNo = item.SerialNo == "All" ? "0" : item.SerialNo
                        };
                        db.Fin_BackendIncPolicyDtl.Add(mds);
                        await db.SaveChangesAsync();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }


        public async Task<BackEndPolicyMobileVM> GetBackEndPolicy(int PolicyId)
        {
            try
            {
                return await db.Fin_BackendIncPolicy.Where(x => x.PolicyId == PolicyId).Select(x => new BackEndPolicyMobileVM()
                {
                    StartDate = x.FromDate,
                    EndDate = x.ToDate,
                    IncBaseId = x.IncBaseId,
                    IncType = x.IncTypeId,
                    PolicyTitle = x.PolicyTitle,
                    Status = x.Status,
                    SupId = x.SupId,
                    PolicyId = x.PolicyId
                }).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<BankEndIncpolicyVM>> GetBackEndPolicyBySupplier(DateTime FromDate, DateTime ToDate, int SupplierId)
        {
            try
            {
                var pol = await db.Fin_BackendIncPolicy.Where(x => x.FromDate >= FromDate && x.ToDate <= ToDate && x.SupId == SupplierId && x.Status == "A").Select(x => new BankEndIncpolicyVM()
                {
                    PolicyId = x.PolicyId,
                    PolicyTitle = x.PolicyTitle,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    IncBaseId = x.IncBaseId,
                    IncTypeId = x.IncTypeId
                }).ToListAsync();
                return pol;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public bool BackEndMobileCalc(List<int> translst, DateTime FromDate, DateTime ToDate, int RegionId, int UserId)
        {
            try
            {
                db.Database.CommandTimeout = 3600;
                foreach (var item in translst)
                {

                    // List<SqlParameter> Parameters = new List<SqlParameter>();
                    //Parameters.Add(new SqlParameter("FromDate", SqlDbType.Date) { Value = FromDate });
                    //Parameters.Add(new SqlParameter("ToDate", SqlDbType.Date) { Value = ToDate });
                    //Parameters.Add(new SqlParameter("PolicyId", SqlDbType.Int) { Value = item });
                    //Parameters.Add(new SqlParameter("RegionId", SqlDbType.Int) { Value = RegionId });
                    //Parameters.Add(new SqlParameter("UserId", SqlDbType.Int) { Value = UserId });
                    //var r = db.Database.SqlQuery<string>("exec spFin_BackendIncCalc @FromDate,@ToDate,@PolicyId,@RegionId,@UserId", Parameters.ToArray());

                    db.spFin_BackendIncCalc(FromDate, ToDate, item, RegionId, UserId);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendWishEmail()
        {
            try
            {
                var dt = DateTime.Now.Date;
                {
                    var lst = await (from E in db.Pay_EmpMaster
                                     join ES in db.Pay_WishEmailTemplate on E.DesgId equals ES.DesgId
                                     where E.StatusId == "A" && E.DOJ.Value.Month == dt.Month
                                     && E.DOJ.Value.Year == dt.Year && ES.Event == "W"
                                     && E.DOJ < dt
                                     select new { E.EmpId, E.EmpName, E.DOJ, ES.Email }).ToListAsync();
                    //string msgg = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\workanni.txt"));
                    foreach (var item in lst)
                    {
                        var img = Convert.ToBase64String(File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\EmpImg\" + item.EmpId.ToString() + ".jpg")));
                        string year = ToOrdinal(dt.Year - item.DOJ.Value.Year);
                        string msg = item.Email;
                        msg = msg.Replace("[image]", img);
                        msg = msg.Replace("[EmpName]", item.EmpName);
                        msg = msg.Replace("[Years]", year);
                        await new SMSBL().SendEmail("abubakar.ashraf@afzalelectronics.com.pk", "Work Anniversary", msg);
                    }
                }
                {
                    var lst = await (from E in db.Pay_EmpMaster
                                     join ES in db.Pay_WishEmailTemplate on 0 equals ES.DesgId
                                     where E.StatusId == "A" && E.DOB.Value.Month == dt.Month
                                     && E.DOB.Value.Year == dt.Year && ES.Event == "B"
                                     //&& E.DOB > dt
                                     select new { E.EmpId, E.EmpName, ES.Email }).ToListAsync();
                    //string msgg = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\workanni.txt"));
                    foreach (var item in lst)
                    {
                        var img = Convert.ToBase64String(File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\EmpImg\" + item.EmpId.ToString() + ".jpg")));
                        //string year = ToOrdinal(dt.Year - item.DOB.Value.Year);
                        string msg = item.Email;
                        msg = msg.Replace("[image]", img);
                        msg = msg.Replace("[EmpName]", item.EmpName);
                        //msg = msg.Replace("[Years]", year);
                        await new SMSBL().SendEmail("abubakar.ashraf@afzalelectronics.com.pk", "Birthday Wishes", msg);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string ToOrdinal(long number)
        {
            if (number < 0) return number.ToString();
            long rem = number % 100;
            if (rem >= 11 && rem <= 13) return number + "th";

            switch (number % 10)
            {
                case 1:
                    return number + "st";
                case 2:
                    return number + "nd";
                case 3:
                    return number + "rd";
                default:
                    return number + "th";
            }
        }
        #endregion
        #region CRCFine


        public async Task<List<CRCPolicyVM>> GetCRCPolicy()
        {
            return await db.LSE_CrcFinePolicy.Where(x => x.Status == true).Select(x => new CRCPolicyVM()
            {
                ApprovedBy = x.ApprovedBy,
                Status = x.Status,
                ApprovedDate = x.ApprovedDate,
                DefinedBy = x.DefinedBy,
                DefinedDate = x.DefinedDate,
                EffectiveFrom = x.EffectiveFrom,
                PolicyCode = x.PolicyCode,
                PolicyDetail = x.PolicyDetail,
                PolicyId = x.PolicyId,
                ShortDesc = x.ShortDesc
            }).ToListAsync();
        }

        public async Task<bool> AddCRCPolicy(CRCPolicyVM mod, int UserId)
        {
            try
            {
                if (mod.PolicyId == 0)
                {
                    LSE_CrcFinePolicy md = new LSE_CrcFinePolicy();
                    md.PolicyCode = mod.PolicyCode;
                    md.PolicyDetail = mod.PolicyDetail;
                    md.ShortDesc = mod.ShortDesc;
                    md.EffectiveFrom = mod.EffectiveFrom.Date;
                    md.DefinedBy = UserId;
                    md.DefinedDate = DateTime.Now;
                    md.Status = true;
                    db.LSE_CrcFinePolicy.Add(md);
                    await db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    var md = await db.LSE_CrcFinePolicy.Where(x => x.PolicyId == mod.PolicyId).FirstOrDefaultAsync();
                    md.PolicyDetail = mod.PolicyDetail;
                    md.PolicyCode = mod.PolicyCode;
                    md.ShortDesc = mod.ShortDesc;
                    md.EffectiveFrom = mod.EffectiveFrom.Date;
                    md.DefinedBy = UserId;
                    md.Status = md.Status;
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<CRCPolicyVM> GetPolicyDetailById(int id, int empid)
        {
            var emp = await db.Pay_EmpMaster.Where(x => x.EmpId == empid).FirstOrDefaultAsync();
            return await (from item in db.LSE_CrcFinePolicy
                          join poldtl in db.LSE_CrcFinePolicyDtl on item.PolicyId equals poldtl.PolicyId
                          join desg in db.Pay_Designation on poldtl.DesgId equals desg.DesgId
                          where item.PolicyId == id && poldtl.PolicyId == id && poldtl.DesgId == emp.DesgId
                          select new CRCPolicyVM()
                          {
                              Designation = desg.DesgName,
                              FineAmount = poldtl.FineAmt,
                              PolicyCode = item.PolicyCode,
                              PolicyDetail = item.PolicyDetail
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<CRCPolicyDtlVM>> GetPolicyDtl(int policyid)
        {
            return await db.LSE_CrcFinePolicyDtl.Where(x => x.PolicyId == policyid).Select(x => new CRCPolicyDtlVM()
            {
                DesgId = x.DesgId,
                FineAmt = x.FineAmt,
                PolicyDtlId = x.PolicyDtlId,
                PolicyId = x.PolicyId
            }).ToListAsync();
        }

        public async Task<bool> AECRCPolicyDtl(List<CRCPolicyDtlVM> mod, int UserId)
        {
            try
            {
                var policyid = mod.FirstOrDefault().PolicyId;
                var pol = await db.LSE_CrcFinePolicyDtl.Where(x => x.PolicyId == policyid).ToListAsync();
                if (pol.Count > 0)
                {
                    db.LSE_CrcFinePolicyDtl.RemoveRange(pol);
                    await db.SaveChangesAsync();
                }
                foreach (var item in mod)
                {
                    LSE_CrcFinePolicyDtl lsemd = new LSE_CrcFinePolicyDtl()
                    {
                        DesgId = item.DesgId,
                        FineAmt = item.FineAmt,
                        PolicyDtlId = item.PolicyDtlId,
                        PolicyId = policyid
                    };
                    db.LSE_CrcFinePolicyDtl.Add(lsemd);
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion
        #region GeneralFunction
        public async Task<decimal> EmpLastSalary(int EmpId)
        {
            return await GetEmployeeBasicSalary(EmpId);
        }

        public async Task<List<SEmployeeVM>> GetCRCList()
        {
            try
            {
                return await db.Pay_EmpMaster.Where(x => x.StatusId == "A" && x.DeptId == 1007).Select(x => new SEmployeeVM()
                {
                    EmpId = x.EmpId,
                    EmpName = x.EmpName
                }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<GMVM>> GMList()
        {
            try
            {
                return await db.Pay_EmpMaster.Where(x => x.DesgId == 268).Select(x =>
                    new GMVM
                    {
                        GMId = x.EmpId,
                        GMName = x.EmpName
                    }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CheckToBeApproved(int id)
        {
            var emp = await db.Pay_EmpMaster.Where(x => x.EmpId == id && x.ApprovedBy == null && x.StatusId == "I").FirstOrDefaultAsync();
            if (emp != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> ApproveEmpoyee(int EmpId, int UserId)
        {
            try
            {
                var empmas = await db.Pay_EmpMaster.Where(x => x.EmpId == EmpId && x.ApprovedBy == null).FirstOrDefaultAsync();
                empmas.ApprovedBy = UserId;
                empmas.ApprovedDate = DateTime.Now;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool UpdateEmpHirarchy()
        {
            try
            {
                var ex = db.spPay_UpdateEmpHierarchy();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion
    }
}