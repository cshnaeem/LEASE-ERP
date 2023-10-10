using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AGEERP.Models
{
    public class TargetBL
    {
        AGEEntities db = new AGEEntities();
        public async Task<List<SaleTargetVM>> GetSaleTarget(DateTime Month, string TypeId)
        {
            try
            {
                List<SaleTargetVM> lst = new List<SaleTargetVM>();
                var target = await db.Lse_SaleTarget.Where(T => T.TargetMonth == Month.Month && T.TargetYear == Month.Year && T.TargetTypeId == TypeId).ToListAsync();
                var location = await db.Comp_Locations.Where(x => x.Status && x.LocTypeId == 1 && x.LocId != 191).ToListAsync();
                foreach (var L in location)
                {
                    var T = target.Where(x => x.LocId == L.LocId).FirstOrDefault();
                    lst.Add(new SaleTargetVM
                    {
                        City = L.Comp_City.City,
                        LocCode = L.LocCode,
                        LocId = L.LocId,
                        LocName = L.LocName,
                        Target = T != null ? T.Target : (decimal)0,
                        TargetMonth = Month.Month,
                        TargetPDA = T != null ? T.TargetPDA : (decimal)0,
                        TargetYear = Month.Year,
                        TargetId = T != null ? T.TargetId : 0
                    });
                }
              
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<IEnumerable<SaleTargetVM>> CreateSaleTarget(IEnumerable<SaleTargetVM> mod, int UserId, string TypeId)
        {
            try
            {
                foreach (var v in mod)
                {
                    if (v.Target > 0)
                    {
                        var target = await db.Lse_SaleTarget.Where(x => x.TargetMonth == v.TargetMonth
                        && x.TargetYear == v.TargetYear && x.LocId == v.LocId && x.TargetTypeId == TypeId).FirstOrDefaultAsync();
                        if (target == null)
                        {
                            target = new Lse_SaleTarget
                            {
                                LocId = v.LocId,
                                Target = v.Target,
                                TargetMonth = v.TargetMonth,
                                TargetPDA = Math.Round(v.Target / DateTime.DaysInMonth(v.TargetYear, v.TargetMonth)),
                                TargetYear = v.TargetYear,
                                TransDate = DateTime.Now,
                                UserId = UserId,
                                TargetTypeId = TypeId
                            };
                            db.Lse_SaleTarget.Add(target);
                            await db.SaveChangesAsync();
                            v.TargetId = target.TargetId;
                        }
                    }
                }

                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateSaleTarget(IEnumerable<SaleTargetVM> mod, int UserId)
        {
            try
            {
                foreach (var v in mod)
                {
                    var target = await db.Lse_SaleTarget.Where(x => x.TargetMonth == v.TargetMonth
                    && x.TargetYear == v.TargetYear && x.LocId == v.LocId).FirstOrDefaultAsync();
                    if (target != null)
                    {
                        if (target.Target != v.Target)
                        {
                            Lse_SaleTargetLog log = new Lse_SaleTargetLog
                            {
                                ActualTarget = target.Target,
                                LocId = target.LocId,
                                ModifiedBy = UserId,
                                ModifiedDate = DateTime.Now,
                                ModifiedTarget = v.Target,
                                TargetId = target.TargetId,
                                TargetMonth = target.TargetMonth,
                                TargetYear = target.TargetYear
                            };
                            db.Lse_SaleTargetLog.Add(log);
                            target.Target = v.Target;
                            target.TargetPDA = Math.Round(v.Target / DateTime.DaysInMonth(v.TargetYear, v.TargetMonth));
                        }
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

        #region Incentive
        public async Task<List<IncentiveVM>> GetEmpPerformance(DateTime Month, int PType)
        {
            try
            {
                var lst = await (from P in db.Pay_EmpPerformance
                                 join PD in db.Pay_EmpPerformanceDetail on P.TransId equals PD.TransId
                                 join E in db.Pay_EmpMaster on PD.EmpId equals E.EmpId
                                 join D in db.Pay_Designation on E.DesgId equals D.DesgId
                                 join L in db.Comp_Locations on PD.LocId equals L.LocId
                                 where P.PerformanceMonth == Month && P.PerformanceTypeId == PType
                                 select new IncentiveVM
                                 {
                                     AchQty = PD.AchQty,
                                     EmpId = PD.EmpId,
                                     EmpName = E.EmpName,
                                     AchValue = PD.AchValue,
                                     IncentivePercent = PD.IncentivePercent,
                                     IncentiveValue = PD.IncentiveValue,
                                     CNIC = E.CNIC,
                                     LocId = PD.LocId,
                                     LocName = L.LocName,
                                     Remarks = PD.Remarks,
                                     DesgName = D.DesgName,
                                     RowId = PD.TransDtlId
                                 }).ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateIncentive(IEnumerable<IncentiveVM> mod, int UserId)
        {
            try
            {
                foreach (var v in mod)
                {
                    var target = await db.Pay_EmpPerformanceDetail.Where(x => x.TransDtlId == v.RowId
                    && x.EmpId == v.EmpId && x.LocId == v.LocId).FirstOrDefaultAsync();
                    if (target != null)
                    {
                        if (target.IncentiveValue != v.IncentiveValue || target.Remarks != v.Remarks)
                        {
                            target.IncentiveValue = v.IncentiveValue;
                            target.Remarks = v.Remarks;
                        }
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
        #region ProcessPerformance
        public async Task<List<PerformanceTypeVM>> PerformanceTypeList()
        {
            try
            {
                return await db.Pay_EmpPerformanceType.Select(x =>
                new PerformanceTypeVM
                {
                    PerformanceTypeId = x.PerformanceTypeId,
                    PerformanceType = x.PerformanceType
                }).ToListAsync();
            }
            catch (Exception)
            {

                return null;
            }
        }
        public async Task<List<ProcessPerformanceVM>> ProcessPerformance(int PTypeId, DateTime date, int userid)
        {
            try
            {
                return db.spget_PayProcessPerformance(date, PTypeId, userid).Select(x =>
                  new ProcessPerformanceVM
                  {
                      TransId = x.TransId,
                      PerformanceMonth = x.PerformanceMonth,
                      PerformanceType = x.PerformanceType,
                      EmpName = x.EmpName,
                      LocName = x.LocName,
                      TargetValue = x.TargetValue,
                      TargetQty = x.TargetQty,
                      AchQty = x.AchQty,
                      AchValue = x.AchValue,
                      IncentivePercent = x.IncentivePercent,
                      IncentiveValue = x.IncentiveValue

                  }).ToList();
                //List<spget_PayProcessPerformance_Result> IsSave = db.spget_PayProcessPerformance(date, PTypeId, userid).ToList();
                //return IsSave;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}