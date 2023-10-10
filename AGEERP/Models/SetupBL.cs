using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;

namespace AGEERP.Models
{
    public class SetupBL
    {
        AGEEntities db = new AGEEntities();
        SMSBL sMSBL = new SMSBL();

        public async Task<IEnumerable<SaleRateVM>> CashSaleRateAdd(IEnumerable<SaleRateVM> mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var m in mod)
                    {
                        m.EffectiveDate = m.EffectiveDate.Date;
                        DateTime? endDate = null;
                        var count = await db.Itm_SaleRate.Where(x => x.SKUId == m.SKUId && x.EffectiveDate == m.EffectiveDate && x.Status).CountAsync();
                        if (count > 1)
                        {
                            scop.Dispose();
                            return null;
                        }

                        var saleRate = await db.Itm_SaleRate.Where(x => x.SKUId == m.SKUId && x.EffectiveDate == m.EffectiveDate && x.Status).SingleOrDefaultAsync();
                        if (saleRate != null)
                        {
                            saleRate.Status = false;
                            endDate = saleRate.EndDate;
                        }
                        else
                        {
                            saleRate = await db.Itm_SaleRate.Where(x => x.SKUId == m.SKUId && x.EffectiveDate < m.EffectiveDate && x.Status).OrderByDescending(x => x.EffectiveDate).FirstOrDefaultAsync();
                            if (saleRate != null)
                            {
                                saleRate.EndDate = m.EffectiveDate.AddDays(-1);
                            }
                            saleRate = await db.Itm_SaleRate.Where(x => x.SKUId == m.SKUId && x.EffectiveDate > m.EffectiveDate && x.Status).OrderBy(x => x.EffectiveDate).FirstOrDefaultAsync();
                            if (saleRate != null)
                            {
                                endDate = saleRate.EffectiveDate.AddDays(-1);
                            }
                        }
                        var row = new Itm_SaleRate
                        {
                            SKUId = m.SKUId,
                            EffectiveDate = m.EffectiveDate,
                            SM = m.SM,
                            SR = m.SR,
                            SKT = m.SKT,
                            Gala = m.Gala,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            EndDate = endDate,
                            Status = true
                        };
                        db.Itm_SaleRate.Add(row);
                        await db.SaveChangesAsync();
                        m.RowId = row.RowId;
                    }
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

        #region Modifications
        public async Task<bool> LogModification(int UserId, string Type, string Details, string Remarks, long RefTransId)
        {
            try
            {
                var mod = new Users_Modification
                {
                    UserId = UserId,
                    ModType = Type,
                    Details = Details,
                    Remarks = Remarks,
                    TransDate = DateTime.Now,
                    RefTransId = RefTransId
                };
                db.Users_Modification.Add(mod);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateExemption(int UserId, long ItemId, string Remarks, bool Exemption)
        {
            try
            {
                var upd = await db.Inv_Store.FirstOrDefaultAsync(x => x.ItemId == ItemId);
                if (upd != null)
                {
                    var obj = new { upd.ItemId, upd.Exempted };
                    upd.Exempted = Exemption;
                    await db.SaveChangesAsync();
                    await LogModification(UserId, "Exemption", obj.ToString(), Remarks, ItemId);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<bool> UpdatePlan(int UserId, long Accno, decimal InstPrice, decimal ActualAdvance, decimal MonthlyInst, decimal Advance, decimal dAdvance, decimal dInst, long InstPlanId, int Duration, string Remarks)
        {
            try
            {
                var upd = await db.Lse_Master.FirstOrDefaultAsync(x => x.AccNo == Accno);
                if (upd != null)
                {
                    var u = new { upd.AccNo, upd.InstPrice, upd.ActualAdvance, upd.MonthlyInst, upd.Advance, upd.Duration };

                    upd.InstPrice = InstPrice;
                    upd.ActualAdvance = ActualAdvance;
                    upd.MonthlyInst = MonthlyInst;
                    upd.Advance = Advance;
                    upd.Duration = Duration;

                    var dtl = await db.Lse_Detail.FirstOrDefaultAsync(x => x.AccNo == Accno && x.InstPrice > 0);
                    if (dtl != null)
                    {
                        //var d = new { LInstPrice = dtl.InstPrice, dtl.dAdvance, dtl.dInst, dtl.InstPlanId };
                        var json = new { u.AccNo, u.InstPrice, u.ActualAdvance, u.MonthlyInst, u.Advance, dInstPrice = dtl.InstPrice, dtl.dAdvance, dtl.dInst, dtl.InstPlanId };

                        dtl.InstPrice = InstPrice;
                        dtl.dAdvance = dAdvance;
                        dtl.dInst = dInst;
                        dtl.InstPlanId = InstPlanId;

                        if (await LogModification(UserId, "Plan", json.ToString(), Remarks, Accno))
                        {
                            await db.SaveChangesAsync();
                            return true;
                        }
                    }
                    return false;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateAdvance(int UserId, long Accno, decimal Advance, string Remarks)
        {
            try
            {
                var upd = await db.Lse_Master.FirstOrDefaultAsync(x => x.AccNo == Accno);
                if (upd != null)
                {
                    var obj = new { upd.AccNo, upd.Advance };
                    upd.Advance = Advance;
                    await db.SaveChangesAsync();
                    await LogModification(UserId, "Advance", obj.ToString(), Remarks, Accno);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateSKU(int UserId, int SKUId, string SerialNo, long ItemId, string Remarks, decimal PPrice, decimal MRP)
        {
            try
            {
                var upd = await db.Inv_Store.FirstOrDefaultAsync(x => x.SerialNo == SerialNo && x.ItemId == ItemId);
                if (upd != null)
                {
                    var obj = new { upd.SerialNo, upd.SKUId, upd.Itm_Master.SKUCode, upd.PPrice, upd.MRP };
                    upd.SKUId = SKUId;
                    upd.PPrice = PPrice;
                    upd.MRP = MRP;
                    await db.SaveChangesAsync();
                    await LogModification(UserId, "SKU", obj.ToString(), Remarks, ItemId);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateMI(int UserId, long Accno, int Type, int EmpId, string Remarks)
        {
            try
            {
                var upd = await db.Lse_Master.FirstOrDefaultAsync(x => x.AccNo == Accno);
                if (upd != null)
                {
                    var obj = new { upd.AccNo, upd.MktOfficerId, upd.InqOfficerId };
                    if (Type == 1)
                    {
                        upd.MktOfficerId = EmpId;
                    }
                    else if (Type == 2)
                    {
                        upd.InqOfficerId = EmpId;
                    }
                    await db.SaveChangesAsync();
                    await LogModification(UserId, "MI", obj.ToString(), Remarks, upd.AccNo);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<EmployeeVM>> EmployeeByRoles(int RoleId, int LocId)
        {
            try
            {
                return await (from E in db.Pay_EmpMaster
                                  //join R in db.Pay_EmpRole on E.EmpId equals R.EmpId
                              join R in db.Pay_DesgRole on E.DesgId equals R.DesgId
                              where R.RoleId == RoleId && E.DeptId == LocId && E.StatusId == "A" //&& R.Status
                              select new EmployeeVM
                              {
                                  EmpId = E.EmpId,
                                  EmpName = E.EmpName,
                                  Mobile1 = E.Mobile1,
                                  CNIC = E.CNIC
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region SMS

        //public async Task<string> GetSMSAll()
        //{
        //    try
        //    {
        //        var filePath = "D:\\SMSFiles\\" + DateTime.Now.Ticks.ToString() + ".csv";
        //        var lst = db.spRep_SMSAll().ToList();
        //        StreamWriter objStreamWriter = File.CreateText(filePath);
        //        foreach (var v in lst)
        //        {
        //            await objStreamWriter.WriteLineAsync(v.Mobile+","+v.SMS);
        //        }
        //        objStreamWriter.Close();
        //        return filePath;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public async Task<bool> SendAllSMS(int LocId, string Category, string Message, bool IsUrdu)
        {
            try
            {
                var WorkingDate = GetWorkingDate(LocId);
                var lst = db.spget_OutStandForSMS(LocId, Category, Message).ToList();
                foreach (var v in lst)
                {
                    if (v.Mobile != null)
                    {
                        //    var sm = await db.Lse_SMS.FirstOrDefaultAsync(x => x.WorkingDate == WorkingDate && x.Category == Category && x.AccNo == v.AccNo && x.MobileNo == v.Mobile);
                        //    if (sm == null)
                        //    {
                        var sen = await sMSBL.Send(v.Mobile, v.SMS);
                        if (sen == "Message Sent")
                        {
                            var tbl = new Lse_SMS
                            {
                                LocId = LocId,
                                Category = Category,
                                AccNo = v.AccNo,
                                MobileNo = v.Mobile,
                                SMS = v.SMS,
                                IsUrdu = IsUrdu,
                                WorkingDate = WorkingDate,
                                TransDate = DateTime.Now,
                                SendDate = DateTime.Now,
                                SendStatus = true,
                                Response = sen
                            };
                            db.Lse_SMS.Add(tbl);
                        }
                        else
                        {
                            var tbl = new Lse_SMS
                            {
                                LocId = LocId,
                                Category = Category,
                                AccNo = v.AccNo,
                                MobileNo = v.Mobile,
                                SMS = v.SMS,
                                IsUrdu = IsUrdu,
                                WorkingDate = WorkingDate,
                                TransDate = DateTime.Now,
                                SendStatus = false,
                                Response = sen
                            };
                            db.Lse_SMS.Add(tbl);
                        }
                        await db.SaveChangesAsync();
                        //}
                        //else
                        //{
                        //    if (!sm.SendStatus)
                        //    {
                        //        var sen = await sMSBL.Send(v.Mobile, v.SMS);
                        //        if (sen == "Message Sent")
                        //        {
                        //            sm.SMS = v.SMS;
                        //            sm.SendDate = DateTime.Now;
                        //            sm.SendStatus = true;
                        //            sm.Response = sen;
                        //            await db.SaveChangesAsync();
                        //        }
                        //    }
                        //}
                    }
                    if (v.Mobile2 != null && v.Mobile2.Length == 13)
                    {
                        //var sm = await db.Lse_SMS.FirstOrDefaultAsync(x => x.WorkingDate == WorkingDate && x.Category == Category && x.AccNo == v.AccNo && x.MobileNo == v.Mobile2);
                        //if (sm == null)
                        //{
                        var sen = await sMSBL.Send(v.Mobile2, v.SMS);
                        if (sen == "Message Sent")
                        {
                            var tbl = new Lse_SMS
                            {
                                LocId = LocId,
                                Category = Category,
                                AccNo = v.AccNo,
                                MobileNo = v.Mobile2,
                                SMS = v.SMS,
                                IsUrdu = IsUrdu,
                                WorkingDate = WorkingDate,
                                TransDate = DateTime.Now,
                                SendDate = DateTime.Now,
                                SendStatus = true,
                                Response = sen
                            };
                            db.Lse_SMS.Add(tbl);
                        }
                        else
                        {
                            var tbl = new Lse_SMS
                            {
                                LocId = LocId,
                                Category = Category,
                                AccNo = v.AccNo,
                                MobileNo = v.Mobile2,
                                SMS = v.SMS,
                                IsUrdu = IsUrdu,
                                WorkingDate = WorkingDate,
                                TransDate = DateTime.Now,
                                SendStatus = false,
                                Response = sen
                            };
                            db.Lse_SMS.Add(tbl);
                        }
                        await db.SaveChangesAsync();
                        //}
                        //else
                        //{
                        //    if (!sm.SendStatus)
                        //    {
                        //        var sen = await sMSBL.Send(v.Mobile2, v.SMS);
                        //        if (sen == "Message Sent")
                        //        {
                        //            sm.SMS = v.SMS;
                        //            sm.SendDate = DateTime.Now;
                        //            sm.SendStatus = true;
                        //            sm.Response = sen;
                        //            await db.SaveChangesAsync();
                        //        }
                        //    }
                        //}
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<LseSmsVM>> MessageData(int LocId, string Category, string Message, bool IsUrdu)
        {
            try
            {
                var WorkingDate = GetWorkingDate(LocId);
                if (db.Lse_SMS.Any(x => x.LocId == LocId && x.WorkingDate == WorkingDate))
                {
                    return await db.Lse_SMS.Where(x => x.LocId == LocId && x.WorkingDate == WorkingDate).Select(x => new LseSmsVM
                    {
                        RowId = x.RowId,
                        LocId = LocId,
                        Category = Category,
                        AccNo = x.AccNo,
                        MobileNo = x.MobileNo,
                        SMS = x.SMS,
                        IsUrdu = IsUrdu,
                        WorkingDate = WorkingDate
                    }).ToListAsync();
                }
                else
                {
                    var LseSMSData = db.spget_OutStandForSMS(LocId, Category, Message).Select(x => new Lse_SMS
                    {
                        LocId = LocId,
                        Category = Category,
                        AccNo = x.AccNo,
                        MobileNo = x.Mobile,
                        SMS = x.SMS,
                        IsUrdu = IsUrdu,
                        WorkingDate = WorkingDate,
                        TransDate = DateTime.Now,
                        SendDate = DateTime.Now,
                        SendStatus = false,
                    }).ToList();
                    db.Lse_SMS.AddRange(LseSMSData);
                    await db.SaveChangesAsync();
                    return await db.Lse_SMS.Where(x => x.LocId == LocId && x.Category == Category && x.WorkingDate == WorkingDate).Select(x => new LseSmsVM
                    {
                        RowId = x.RowId,
                        LocId = LocId,
                        Category = Category,
                        AccNo = x.AccNo,
                        MobileNo = x.MobileNo,
                        SMS = x.SMS,
                        IsUrdu = IsUrdu,
                        WorkingDate = WorkingDate
                    }).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region ProductClass
        public async Task<List<ProductClassVM>> ProductClassList()
        {
            try
            {
                return await db.Itm_ProductClass.Where(x => x.Status).Select(x =>
                    new ProductClassVM
                    {
                        ProductClassId = x.ProductClassId,
                        ProductClass = x.ProductClass,
                        ProductId = x.ProductId,
                    }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ProductClassVM>> ProductClassList(int ProductId)
        {
            try
            {
                return await db.Itm_ProductClass.Where(x => x.Status && x.ProductId == ProductId).Select(x =>
                    new ProductClassVM
                    {
                        ProductClassId = x.ProductClassId,
                        ProductClass = x.ProductClass,
                        ProductId = x.ProductId,
                    }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ProductClassVM> CreateProductClass(ProductClassVM mod, int UserId)
        {
            try
            {
                var IsExist = await db.Itm_ProductClass.Where(x => x.ProductClass == mod.ProductClass.Trim() && x.ProductId == mod.ProductId).AnyAsync();
                if (!IsExist)
                {
                    Itm_ProductClass tbl = new Itm_ProductClass
                    {
                        ProductClass = mod.ProductClass,
                        ProductId = mod.ProductId,
                        Status = true,
                        TransDate = DateTime.Now,
                        UserId = UserId
                    };
                    db.Itm_ProductClass.Add(tbl);
                    await db.SaveChangesAsync();
                    mod.ProductClassId = tbl.ProductClassId;
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateProductClass(ProductClassVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Itm_ProductClass.SingleOrDefaultAsync(x => x.ProductClassId.Equals(mod.ProductClassId));
                if (tbl != null)
                {
                    tbl.ProductClass = mod.ProductClass;
                    tbl.ProductId = mod.ProductId;
                    tbl.TransDate = DateTime.Now;
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
        public async Task<bool> DestroyProductClass(ProductClassVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Itm_ProductClass.SingleOrDefaultAsync(x => x.ProductClassId.Equals(mod.ProductClassId));
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
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
        #region Region
        public async Task<List<RegionVM>> RegionList()
        {
            try
            {
                return await db.Comp_Region.Select(x =>
                new RegionVM
                {
                    Region = x.Region,
                    RegionId = x.RegionId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<CityVM>> CityList(int RegionId)
        {
            try
            {
                return await db.Comp_City.Where(x => x.RegionId == RegionId || RegionId == 0).Select(x =>
                new CityVM
                {
                    CityId = x.CityId,
                    City = x.City,
                    CityCode = x.CityCode
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region City
        public async Task<List<CityVM>> CityList()
        {
            try
            {
                return await db.Comp_City.Select(x =>
                new CityVM
                {
                    CityId = x.CityId,
                    City = x.City,
                    CityCode = x.CityCode
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<CityVM> CreateCity(CityVM mod, int UserId)
        {
            try
            {
                mod.City = mod.City.Trim();
                var IsExist = await db.Comp_City.Where(x => x.City == mod.City || x.CityCode == mod.CityCode).AnyAsync();
                if (!IsExist)
                {
                    Comp_City tbl = new Comp_City
                    {
                        City = mod.City,
                        CityCode = mod.CityCode
                    };
                    db.Comp_City.Add(tbl);
                    await db.SaveChangesAsync();
                    mod.CityId = tbl.CityId;
                    return mod;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCity(CityVM mod, int UserId)
        {
            try
            {
                mod.City = mod.City.Trim();
                var IsExist = await db.Comp_City.Where(x => (x.City == mod.City || x.CityCode == mod.CityCode) && x.CityId != mod.CityId).AnyAsync();
                if (!IsExist)
                {
                    var tbl = await db.Comp_City.SingleOrDefaultAsync(x => x.CityId.Equals(mod.CityId));
                    if (tbl != null)
                    {
                        tbl.City = mod.City;
                        tbl.CityCode = mod.CityCode;
                    }
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

        //public async Task<bool> DestroyCity(CityVM mod, int UserId)
        //{
        //    try
        //    {
        //        var tbl = await db.Comp_City.SingleOrDefaultAsync(x => x.CityId.Equals(mod.CityId));
        //        if (tbl != null)
        //        {
        //            tbl.Status = false;
        //            tbl.UserId = UserId;
        //            tbl.TransDate = DateTime.Now;
        //        }
        //        await db.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        #endregion

        #region Location
        public async Task<List<LocationDetailVM>> LocationDetailList()
        {
            try
            {
                return await db.Comp_Locations.Where(x => x.Status).Select(x =>
                new LocationDetailVM
                {
                    CityId = x.CityId,
                    Address = x.Address,
                    LocId = x.LocId,
                    LocCode = x.LocCode,
                    LocName = x.LocName,
                    LocTypeId = x.LocTypeId,
                    Phone1 = x.Phone1,
                    Phone2 = x.Phone2,
                    Status = x.Status
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<LocationDetailVM> CreateLocation(LocationDetailVM mod, int UserId)
        {
            try
            {
                Comp_Locations tbl = new Comp_Locations
                {
                    Status = true,
                    CityId = mod.CityId,
                    Address = mod.Address,
                    LocCode = mod.LocCode,
                    LocName = mod.LocName,
                    LocTypeId = mod.LocTypeId,
                    Phone1 = mod.Phone1,
                    Phone2 = mod.Phone2
                };
                db.Comp_Locations.Add(tbl);
                await db.SaveChangesAsync();
                mod.LocId = tbl.LocId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateLocation(LocationDetailVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Comp_Locations.SingleOrDefaultAsync(x => x.LocId.Equals(mod.LocId));
                if (tbl != null)
                {
                    tbl.CityId = mod.CityId;
                    tbl.Address = mod.Address;
                    tbl.LocCode = mod.LocCode;
                    tbl.LocName = mod.LocName;
                    tbl.LocTypeId = mod.LocTypeId;
                    tbl.Phone1 = mod.Phone1;
                    tbl.Phone2 = mod.Phone2;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyLocation(LocationDetailVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Comp_Locations.SingleOrDefaultAsync(x => x.LocId.Equals(mod.LocId));
                if (tbl != null)
                {
                    tbl.Status = false;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Comp_LocationType>> LocTypeList()
        {
            try
            {
                return await db.Comp_LocationType.ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<LocationVM>> LocationByCityList(int CityId)
        {
            try
            {
                return await db.Comp_Locations.Where(x => (x.CityId == CityId || CityId == 0) && x.Status).Select(x =>
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


        public async Task<List<LocationVM>> AllLocationAndCityList(int[] City)
        {
            try
            {
                return await db.Comp_Locations.Where(x => x.Status && City.Contains(x.CityId) && x.LocTypeId == 1).Select(x =>
                new LocationVM
                {
                    TransId = 0,
                    CityId = x.CityId,
                    CityName = x.Comp_City.City,
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
        public async Task<List<SupplierCatVM>> SupliersCatList()
        {
            try
            {
                return await db.Inv_SuppliersCat.Select(x =>
                new SupplierCatVM
                {
                    CategoryId = x.CategoryId,
                    CategoryTitle = x.CategoryTitle
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<LocationVM>> LocationListPO()
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
        public async Task<List<LocationVM>> LocationByType(int LocTypeId)
        {
            try
            {
                return await db.Comp_Locations.Where(x => x.Status && x.LocTypeId == LocTypeId).Select(x =>
                new LocationVM
                {
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
        public async Task<List<LocationVM>> LocationList()
        {
            try
            {
                return await db.Comp_Locations.Where(x => x.Status)
                    .Select(x =>
                new LocationVM
                {
                    CityId = x.CityId,
                    LocCode = x.LocCode,
                    LocId = x.LocId,
                    LocName = x.LocName,
                    Lat = x.Lat,
                    Long = x.Lng
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<LocationVM>> LocationListInActive()
        {
            try
            {
                return await db.Comp_Locations.Where(x => !x.Status).Select(x =>
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
        public async Task<List<LocationVM>> PurchaseCenterList()
        {
            try
            {
                return await db.Comp_Locations.Where(x => x.Status && x.LocTypeId == 1 && x.PurchaseCenter == x.LocId).Select(x =>
                new LocationVM
                {
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
        public async Task<List<LocationVM>> LocationList(int LocId)
        {
            try
            {
                if (LocId == 72)
                {
                    return await db.Comp_Locations.Where(x => x.Status)
                        .Select(x =>
                new LocationVM
                {
                    CityId = x.CityId,
                    LocCode = x.LocCode,
                    LocId = x.LocId,
                    LocName = x.LocName
                }).ToListAsync();
                }
                return await db.Comp_Locations.Where(x => x.LocId == LocId).Select(x =>
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
        #endregion
        #region SuppTaxExemption
        public async Task<List<SuppTaxExemptionVM>> SuppTaxExemptionList()
        {
            try
            {
                return await db.Inv_SuppTaxExemption.Where(x => x.Status).Select(x =>
                new SuppTaxExemptionVM
                {
                    RowId = x.RowId,
                    SuppId = x.SuppId,
                    FromDate = x.FromDate,
                    Remarks = x.Remarks,
                    ToDate = x.ToDate,
                    TaxRate = x.TaxRate
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<SuppTaxExemptionVM> CreateSuppTaxExemption(SuppTaxExemptionVM mod, int UserId)
        {
            try
            {
                Inv_SuppTaxExemption tbl = new Inv_SuppTaxExemption
                {
                    SuppId = mod.SuppId,
                    Status = true,
                    TransDate = DateTime.Now,
                    UserId = UserId,
                    FromDate = mod.FromDate,
                    Remarks = mod.Remarks,
                    ToDate = mod.ToDate,
                    TaxRate = mod.TaxRate
                };
                db.Inv_SuppTaxExemption.Add(tbl);
                await db.SaveChangesAsync();
                mod.RowId = tbl.RowId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateSuppTaxExemption(SuppTaxExemptionVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Inv_SuppTaxExemption.SingleOrDefaultAsync(x => x.RowId.Equals(mod.RowId));
                if (tbl != null)
                {
                    tbl.SuppId = mod.SuppId;
                    tbl.FromDate = mod.FromDate;
                    tbl.ToDate = mod.ToDate;
                    tbl.Remarks = mod.Remarks;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
                    tbl.TaxRate = mod.TaxRate;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroySuppTaxExemption(SuppTaxExemptionVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Inv_SuppTaxExemption.SingleOrDefaultAsync(x => x.RowId.Equals(mod.RowId));
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
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
        #region Company
        public async Task<List<CompanyVM>> CompanyList()
        {
            try
            {
                return await db.Itm_Company.Where(x => x.Status).Select(x =>
                new CompanyVM
                {
                    ComId = x.ComId,
                    ComName = x.ComName,
                    ComCode = x.ComCode
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<CompanyVM> CreateCompany(CompanyVM mod, int UserId)
        {
            try
            {
                mod.ComName = mod.ComName.Trim();
                var IsExist = await db.Itm_Company.Where(x => x.ComName == mod.ComName || x.ComCode == mod.ComCode).AnyAsync();
                if (!IsExist)
                {
                    Itm_Company tbl = new Itm_Company
                    {
                        ComName = mod.ComName,
                        ComCode = mod.ComCode,
                        Status = true,
                        TransDate = DateTime.Now,
                        UserId = UserId
                    };
                    db.Itm_Company.Add(tbl);
                    await db.SaveChangesAsync();
                    mod.ComId = tbl.ComId;
                    return mod;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCompany(CompanyVM mod, int UserId)
        {
            try
            {
                mod.ComName = mod.ComName.Trim();
                var IsExist = await db.Itm_Company.Where(x => (x.ComName == mod.ComName || x.ComCode == mod.ComCode) && x.ComId != mod.ComId).AnyAsync();
                if (!IsExist)
                {
                    var tbl = await db.Itm_Company.SingleOrDefaultAsync(x => x.ComId.Equals(mod.ComId));
                    if (tbl != null)
                    {
                        tbl.ComName = mod.ComName;
                        tbl.ComCode = mod.ComCode;
                        tbl.UserId = UserId;
                        tbl.TransDate = DateTime.Now;
                    }
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyCompany(CompanyVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Itm_Company.SingleOrDefaultAsync(x => x.ComId.Equals(mod.ComId));
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<CompanyVM>> CompBySupplierList(int SuppId)
        {
            try
            {
                return await db.Inv_SuppliersMapping.Where(x => x.Status && x.Itm_Company.Status && x.SupplierId == SuppId).Select(x =>
                new CompanyVM
                {
                    ComId = x.Itm_Company.ComId,
                    ComName = x.Itm_Company.ComName,
                    ComCode = x.Itm_Company.ComCode
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Product
        public async Task<List<ProductVM>> ProductByTypeList(int TypeId)
        {
            try
            {
                return await (from P in db.Itm_Products
                              join C in db.Itm_Type on P.ProductId equals C.ProductId
                              where P.Status && C.Status && C.TypeId == TypeId
                              select
                new ProductVM
                {
                    ProductId = P.ProductId,
                    Name = P.ProductName
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ProductVM>> ProductList()
        {
            try
            {
                return await db.Itm_Products.Where(x => x.Status).Select(x =>
                new ProductVM
                {
                    ProductId = x.ProductId,
                    Name = x.ProductName,
                    ProductCode = x.ProductCode,
                    PCTCode = x.PCTCode
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ProductVM> CreateProduct(ProductVM mod, int UserId)
        {
            try
            {
                mod.Name = mod.Name.Trim();
                mod.ProductCode = mod.ProductCode.Trim();
                var IsExist = await db.Itm_Products.Where(x => x.ProductName == mod.Name || x.ProductCode == mod.ProductCode).AnyAsync();
                if (!IsExist)
                {
                    Itm_Products tbl = new Itm_Products
                    {
                        ProductName = mod.Name,
                        ProductCode = mod.ProductCode,
                        PCTCode = mod.PCTCode,
                        Status = true,
                        TransDate = DateTime.Now,
                        UserId = UserId
                    };
                    db.Itm_Products.Add(tbl);
                    await db.SaveChangesAsync();
                    mod.ProductId = tbl.ProductId;
                    return mod;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateProduct(ProductVM mod, int UserId)
        {
            try
            {
                mod.Name = mod.Name.Trim();
                mod.ProductCode = mod.ProductCode.Trim();
                var IsExist = await db.Itm_Products.Where(x => (x.ProductName == mod.Name || x.ProductCode == mod.ProductCode) && x.ProductId != mod.ProductId).AnyAsync();
                if (!IsExist)
                {
                    var tbl = await db.Itm_Products.SingleOrDefaultAsync(x => x.ProductId.Equals(mod.ProductId));
                    if (tbl != null)
                    {
                        tbl.ProductName = mod.Name;
                        tbl.ProductCode = mod.ProductCode;
                        tbl.PCTCode = mod.PCTCode;
                        tbl.Status = true;
                        tbl.TransDate = DateTime.Now;
                        tbl.UserId = UserId;
                    }
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyProduct(ProductVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Itm_Products.SingleOrDefaultAsync(x => x.ProductId.Equals(mod.ProductId));
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
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

        #region Type

        public async Task<List<TypeVM>> TypeListByProductId()
        {
            try
            {
                return await db.Itm_Type.Where(x => x.Status && x.ProductId == 304).Select(x =>
                new TypeVM
                {
                    ProductId = x.ProductId,
                    Name = x.TypeName,
                    ComId = x.ComId,
                    TypeId = x.TypeId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<TypeVM>> TypeList()
        {
            try
            {
                return await db.Itm_Type.Where(x => x.Status).Select(x =>
                new TypeVM
                {
                    ProductId = x.ProductId,
                    Name = x.TypeName,
                    ComId = x.ComId,
                    TypeId = x.TypeId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<TypeVM> CreateType(TypeVM mod, int UserId)
        {
            try
            {
                var IsExist = await db.Itm_Type.Where(x => (x.ProductId == mod.ProductId && x.ComId == mod.ComId)).AnyAsync();
                if (!IsExist)
                {
                    var pro = await db.Itm_Products.FindAsync(mod.ProductId);
                    var com = await db.Itm_Company.FindAsync(mod.ComId);
                    Itm_Type tbl = new Itm_Type
                    {
                        TypeName = com.ComCode + pro.ProductCode,
                        Status = true,
                        ComId = mod.ComId,
                        ProductId = mod.ProductId,
                        TransDate = DateTime.Now,
                        UserId = UserId
                    };
                    db.Itm_Type.Add(tbl);
                    await db.SaveChangesAsync();
                    mod.TypeId = tbl.TypeId;
                    mod.Name = tbl.TypeName;
                    return mod;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateType(TypeVM mod, int UserId)
        {
            try
            {
                mod.Name = mod.Name.Trim();
                //mod.ProductCode = mod.ProductCode.Trim();
                var IsExist = await db.Itm_Type.Where(x => (x.ProductId == mod.ProductId && x.ComId == mod.ComId) && x.ProductId != mod.ProductId).AnyAsync();
                if (!IsExist)
                {
                    var tbl = await db.Itm_Type.SingleOrDefaultAsync(x => x.TypeId.Equals(mod.TypeId));
                    if (tbl != null)
                    {
                        var pro = await db.Itm_Products.FindAsync(mod.ProductId);
                        var com = await db.Itm_Company.FindAsync(mod.ComId);
                        tbl.TypeName = com.ComCode + pro.ProductCode;
                        tbl.ProductId = mod.ProductId;
                        tbl.ComId = mod.ComId;
                        tbl.TransDate = DateTime.Now;
                        tbl.UserId = UserId;
                    }
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyType(TypeVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Itm_Type.SingleOrDefaultAsync(x => x.TypeId.Equals(mod.TypeId));
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<TypeVM>> TypeByCompProdList(int ComId, int ProductId)
        {
            try
            {
                return await db.Itm_Type.Where(x => x.Status && (x.ComId == ComId || ComId == 0)
                && (x.ProductId == ProductId || ProductId == 0)).Select(x =>
                new TypeVM
                {
                    ProductId = x.ProductId,
                    Name = x.TypeName,
                    ComId = x.ComId,
                    TypeId = x.TypeId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ProductVM>> ProductByCompList(int ComId)
        {
            try
            {
                return await (from T in db.Itm_Type
                              join C in db.Itm_Company on T.ComId equals C.ComId
                              join P in db.Itm_Products on T.ProductId equals P.ProductId
                              where T.Status && C.Status && P.Status && T.ComId == ComId
                              select
                 new ProductVM
                 {
                     ProductId = P.ProductId,
                     Name = P.ProductName
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<TypeVM>> TypeByCompList(int ComId)
        {
            try
            {
                return await db.Itm_Type.Where(x => x.Status && x.ComId == ComId).Select(x =>
                new TypeVM
                {
                    ProductId = x.ProductId,
                    Name = x.TypeName,
                    ComId = x.ComId,
                    TypeId = x.TypeId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Model
        public async Task<List<ModelVM>> ModelByLocList(int LocId)
        {
            try
            {
                return await db.Inv_Store.Where(x => x.LocId == LocId && x.Inv_Status.MFact == 1).Select(x =>
                new ModelVM
                {
                    TypeId = x.Itm_Master.Itm_Model.TypeId,
                    Model = x.Itm_Master.Itm_Model.Model,
                    ModelId = x.Itm_Master.Itm_Model.ModelId,
                    Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                    Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName
                }).Distinct().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ModelVM>> ModelByTypeList(int TypeId)
        {
            try
            {
                return await db.Itm_Model.Where(x => x.Status && x.TypeId == TypeId).Select(x =>
                new ModelVM
                {
                    TypeId = x.TypeId,
                    Model = x.Model,
                    ModelId = x.ModelId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ModelVM>> ModelByComProList(int ComId, int ProductId)
        {
            try
            {
                return await (from M in db.Itm_Model
                              join T in db.Itm_Type on M.TypeId equals T.TypeId
                              where T.ComId == ComId && T.ProductId == ProductId && M.Status && T.Status
                              select
                 new ModelVM
                 {
                     TypeId = T.TypeId,
                     Model = M.Model,
                     ModelId = M.ModelId
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ModelVM>> ModelByComProByProductClassList(int ComId, int ClassId, int ProductId)
        {
            try
            {
                return await (from M in db.Itm_Model
                              join T in db.Itm_Type on M.TypeId equals T.TypeId
                              where T.ComId == ComId && T.ProductId == ProductId && M.Status && T.Status && M.ProductClassId == ClassId
                              select
                 new ModelVM
                 {
                     TypeId = T.TypeId,
                     Model = M.Model,
                     ModelId = M.ModelId
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ModelVM>> ModelListByStatus(int status)
        {
            try
            {
                return await db.Itm_Model.Where(x => x.Status == (status == 1 ? true : false)).Select(x =>
                  new ModelVM
                  {
                      TypeId = x.TypeId,
                      Model = x.Model,
                      ModelId = x.ModelId,
                      ProductClassId = x.ProductClassId
                  }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ModelVM>> ModelList()
        {
            try
            {
                return await db.Itm_Model.Where(x => x.Status).Select(x =>
                new ModelVM
                {
                    TypeId = x.TypeId,
                    Model = x.Model,
                    ModelId = x.ModelId,
                    ProductClassId = x.ProductClassId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ModelVM> CreateModel(ModelVM mod, int UserId)
        {
            try
            {
                var IsExist = await db.Itm_Model.Where(x => x.Model == mod.Model.Trim() && x.TypeId == mod.TypeId).AnyAsync();
                if (!IsExist)
                {
                    Itm_Model tbl = new Itm_Model
                    {
                        Status = true,
                        Model = mod.Model,
                        TypeId = mod.TypeId,
                        ProductClassId = mod.ProductClassId,
                        TransDate = DateTime.Now,
                        UserId = UserId
                    };
                    db.Itm_Model.Add(tbl);
                    await db.SaveChangesAsync();
                    mod.ModelId = tbl.ModelId;
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateModel(ModelVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Itm_Model.SingleOrDefaultAsync(x => x.ModelId.Equals(mod.ModelId));
                if (tbl != null)
                {
                    tbl.Model = mod.Model;
                    tbl.TypeId = mod.TypeId;
                    tbl.ProductClassId = mod.ProductClassId;
                    tbl.TransDate = DateTime.Now;
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

        public async Task<bool> DestroyModel(ModelVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Itm_Model.SingleOrDefaultAsync(x => x.ModelId.Equals(mod.ModelId));
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
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

        #region Color


        public async Task<List<ColorVM>> ColorList()
        {
            return await db.Itm_Color.Select(x => new ColorVM
            {
                ColorCode = x.ColorCode,
                ColorId = x.ColorId,
                ColorName = x.ColorName
            }).ToListAsync();
        }
        public async Task<ColorVM> CreateColor(ColorVM mod, int UserId)
        {
            try
            {
                var IsExist = await db.Itm_Color.Where(x => x.ColorName == mod.ColorName.Trim()).AnyAsync();
                if (!IsExist)
                {
                    Itm_Color tbl = new Itm_Color
                    {
                        ColorName = mod.ColorName,
                        ColorCode = mod.ColorCode
                    };
                    db.Itm_Color.Add(tbl);
                    await db.SaveChangesAsync();
                    mod.ColorId = tbl.ColorId;
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateColor(ColorVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Itm_Color.SingleOrDefaultAsync(x => x.ColorId.Equals(mod.ColorId));
                if (tbl != null)
                {
                    tbl.ColorName = mod.ColorName;
                    tbl.ColorCode = mod.ColorCode;
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

        #region SKU
        public async Task<Inv_StoreVM> SKUDetails(string Serial)
        {
            return await (from I in db.Inv_Store
                          join T in db.Itm_Master on I.SKUId equals T.SKUId
                          join S in db.Inv_Status on I.StatusID equals S.StatusID
                          where I.SerialNo == Serial && S.MFact == 1
                          select new Inv_StoreVM
                          {
                              ItemId = I.ItemId,
                              SKUId = I.SKUId,
                              LocId = I.LocId,
                              SerialNo = I.SerialNo,
                              PPrice = I.PPrice,
                              MRP = I.MRP,
                              SKU = T.SKUCode,
                              Model = T.ModelId.ToString(),
                              Status = S.StatusTitle,
                              Exempted = I.Exempted ?? false
                          }).FirstOrDefaultAsync();
        }
        public async Task<SKUVM> SKUByAccno(long Accno)
        {
            try
            {
                return await db.Lse_Detail.Where(x => x.AccNo == Accno).Select(x => new SKUVM
                {
                    SKUId = x.SKUId ?? 0,
                    SKUName = x.Itm_Master.SKUName,
                    Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                    InstPlanId = x.InstPlanId.ToString()
                }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SKUVM>> SKUByLocList(int LocId)
        {
            try
            {

                return await db.Inv_Store.Where(x => x.LocId == LocId && x.Inv_Status.MFact == 1).Select(x =>
                new SKUVM
                {
                    SKUId = x.SKUId,
                    SKUName = x.Itm_Master.SKUName,
                    Model = x.Itm_Master.Itm_Model.Model,
                    ModelId = x.Itm_Master.Itm_Model.ModelId,
                    Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                    Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName
                }).Distinct().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SKUVM>> SKUByLocListReturn(int LocId, long AccNo)
        {
            try
            {
                var itms = await db.Lse_Detail.Where(x => x.AccNo == AccNo).Select(x => x.ItemId).ToListAsync();
                return await db.Inv_Store.Where(x => (x.LocId == LocId && x.Inv_Status.MFact == 1) || itms.Contains(x.ItemId)).Select(x =>
                new SKUVM
                {
                    SKUId = x.SKUId,
                    SKUName = x.Itm_Master.SKUName,
                    Model = x.Itm_Master.Itm_Model.Model,
                    ModelId = x.Itm_Master.Itm_Model.ModelId,
                    Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                    Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName
                }).Distinct().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SKUVM>> SKUListAll()
        {
            try
            {
                return await db.Itm_Master.Where(x => x.ActiveStatus && x.Itm_Model.Status).Select(x =>
                  new SKUVM
                  {
                      SKUId = x.SKUId,
                      SKUName = x.SKUName,
                      Company = x.Itm_Model.Itm_Type.Itm_Company.ComName,
                      Product = x.Itm_Model.Itm_Type.Itm_Products.ProductName
                  }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Inv_StoreVM>> SerailBySKUId(int skuid)
        {
            try
            {
                return await db.Inv_Store.Where(x => (x.StatusID == 1 || x.StatusID == 7) && x.SKUId == skuid).Select(x =>
                  new Inv_StoreVM
                  {
                      SerialNo = x.SerialNo,
                      ItemId = x.ItemId

                  }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SKUVM>> SKUAllByLocList(int LocId)
        {
            try
            {
                var lst = await db.Itm_Master.Where(x => x.ActiveStatus == true && x.AvailableForSale == true && x.Itm_Model.Status).Select(x =>
                  new SKUVM
                  {
                      SKUId = x.SKUId,
                      SKUName = x.SKUName,
                      Model = x.Itm_Model.Model,
                      ModelId = x.Itm_Model.ModelId,
                      Company = x.Itm_Model.Itm_Type.Itm_Company.ComName,
                      Product = x.Itm_Model.Itm_Type.Itm_Products.ProductName
                  }).ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SKUVM>> SKUListForGRN(long TransId)
        {
            try
            {
                var po = await db.Inv_POSchedule.Where(x => x.POSchId == TransId).Select(x => new { x.Inv_PODetail.SKUId, x.Inv_PODetail.Itm_Master.ModelId, x.Inv_PODetail.Inv_PO.PODate, x.Inv_PODetail.MRP, x.Inv_PODetail.Inv_PO.POTypeId }).FirstOrDefaultAsync();
                //var sku = await db.Itm_Master.FindAsync(po.);
                if (po.POTypeId == 3)
                {
                    return await db.Itm_Master.Where(x => x.ActiveStatus == true && x.SKUId == po.SKUId).Select(x =>
                new SKUVM
                {
                    SKUCode = x.SKUCode,
                    SKUId = x.SKUId,
                    SKUName = x.SKUName
                }).ToListAsync();
                }
                var lst = await (from P in db.Itm_PPrice
                                 join I in db.Itm_Master on P.SKUId equals I.SKUId
                                 where I.ModelId == po.ModelId && P.FromDate <= po.PODate && P.ToDate >= po.PODate && P.MRP == po.MRP
                                 select I.SKUId).ToListAsync();

                return await db.Itm_Master.Where(x => x.ActiveStatus == true && lst.Contains(x.SKUId)).Select(x =>
                new SKUVM
                {
                    SKUCode = x.SKUCode,
                    SKUId = x.SKUId,
                    SKUName = x.SKUName
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<SKUVM>> SKUListByStatus(int ModelId, int status)
        {
            try
            {
                return db.Itm_Master.Where(x => x.ModelId == ModelId && x.ActiveStatus == (status == 1 ? true : false)).Include("Itm_SKUPair").Include("Itm_Barcode").ToListAsync().Result.Select(x =>
                new SKUVM
                {
                    ModelId = ModelId,
                    ColorId = x.ColorId,
                    SKUCode = x.SKUCode,
                    SKUId = x.SKUId,
                    SKUName = x.SKUName,
                    Specs = x.OtherSpecs,
                    AvailableForPurchase = x.AvailableForPurchase,
                    AvailableForSale = x.AvailableForSale,
                    IsPaired = x.IsPair,
                    PairLst = x.Itm_SKUPair.Select(a => a.PairId).ToList(),
                    Description = x.Description,
                    BarcodeValue = x.BarcodeValue,
                    Capacity = x.Capacity ?? 0,
                    Barcodes = string.Join(",", x.Itm_Barcode.Select(a => a.Barcode))
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SKUVM>> SKUByModel4PurchaseList(int ModelId)
        {
            try
            {
                return await db.Itm_Master.Where(x => x.ModelId == ModelId && x.ActiveStatus && x.AvailableForPurchase).Select(x =>
                new SKUVM
                {
                    ModelId = ModelId,
                    ColorId = x.ColorId,
                    SKUCode = x.SKUCode,
                    SKUId = x.SKUId,
                    SKUName = x.SKUName,
                    Specs = x.OtherSpecs,
                    AvailableForPurchase = x.AvailableForPurchase,
                    AvailableForSale = x.AvailableForSale,
                    IsPaired = x.IsPair,
                    PairLst = x.Itm_SKUPair.Select(a => a.PairId).ToList(),
                    Description = x.Description,
                    BarcodeValue = x.BarcodeValue,
                    Capacity = x.Capacity ?? 0
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SKUVM>> SKUList(int ModelId)
        {
            try
            {


                return await db.Itm_Master.Where(x => x.ModelId == ModelId && x.ActiveStatus).Select(x =>
                new SKUVM
                {
                    ModelId = ModelId,
                    ColorId = x.ColorId,
                    SKUCode = x.SKUCode,
                    SKUId = x.SKUId,
                    SKUName = x.SKUName,
                    Specs = x.OtherSpecs,
                    AvailableForPurchase = x.AvailableForPurchase,
                    AvailableForSale = x.AvailableForSale,
                    IsPaired = x.IsPair,
                    PairLst = x.Itm_SKUPair.Select(a => a.PairId).ToList(),
                    Description = x.Description,
                    BarcodeValue = x.BarcodeValue,
                    Capacity = x.Capacity ?? 0
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SKUVM>> SKUPairList(int TypeId)
        {
            try
            {


                return await db.Itm_Master.Where(x => x.Itm_Model.TypeId == TypeId &&
                x.IsPair && x.ActiveStatus == true).Select(x =>
                new SKUVM
                {
                    ModelId = x.ModelId,
                    ColorId = x.ColorId,
                    SKUCode = x.SKUCode,
                    SKUId = x.SKUId,
                    SKUName = x.SKUName,
                    Specs = x.OtherSpecs,
                    AvailableForPurchase = x.AvailableForPurchase,
                    AvailableForSale = x.AvailableForSale,
                    IsPaired = x.IsPair,
                    PairLst = x.Itm_SKUPair.Select(a => a.PairId).ToList(),
                    Description = x.Description,
                    BarcodeValue = x.BarcodeValue
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<SKUVM> CreateSKU(SKUVM mod, int UserId)
        {
            try
            {
                var IsExist = await db.Itm_Master.Where(x => (x.ModelId == mod.ModelId && x.ColorId == mod.ColorId && x.OtherSpecs == mod.Specs)).AnyAsync();
                if (!IsExist)
                {
                    var model = (await db.Itm_Model.FindAsync(mod.ModelId));

                    mod.SKUCode = model.Itm_Type.TypeName + "_" + model.Model;

                    if (mod.ColorId > 0)
                    {
                        var col = await db.Itm_Color.FindAsync(mod.ColorId);
                        mod.SKUCode = mod.SKUCode + "_" + col.ColorCode;
                    }
                    if (!string.IsNullOrEmpty(mod.Specs))
                    {
                        mod.SKUCode = mod.SKUCode + "_" + mod.Specs;
                    }

                    Itm_Master tbl = new Itm_Master
                    {
                        SKUName = mod.SKUCode,
                        ActiveStatus = true,
                        BarcodeValue = mod.BarcodeValue,
                        ColorId = mod.ColorId,
                        DefinedBy = UserId,
                        DefinedDate = DateTime.Now,
                        Description = mod.Description,
                        ModelId = mod.ModelId,
                        OtherSpecs = mod.Specs,
                        //PairId = mod.PairId,
                        WarrantyId = 0,
                        SKUCode = mod.SKUCode,
                        AvailableForSale = mod.AvailableForSale,
                        AvailableForPurchase = mod.AvailableForPurchase,
                        IsPair = mod.IsPaired,
                        Capacity = mod.Capacity
                    };
                    db.Itm_Master.Add(tbl);
                    await db.SaveChangesAsync();
                    if (!string.IsNullOrEmpty(mod.Barcodes))
                    {
                        string[] arr = mod.Barcodes.Split(',');
                        foreach (var v in arr)
                        {
                            db.Itm_Barcode.Add(new Itm_Barcode
                            {
                                Barcode = v,
                                SKUId = tbl.SKUId
                            });
                            await db.SaveChangesAsync();
                        }
                    }

                    if (mod.PairLst == null)
                    {
                        mod.PairLst = new List<int>();
                    }
                    foreach (var item in mod.PairLst)
                    {
                        db.Itm_SKUPair.Add(new Itm_SKUPair { PairId = item, SKUId = tbl.SKUId });
                    }
                    await db.SaveChangesAsync();
                    mod.SKUId = tbl.SKUId;
                    mod.SKUName = tbl.SKUName;

                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateSKU(SKUVM mod, int UserId)
        {
            try
            {
                var IsExist = await db.Itm_Master.Where(x => (x.ModelId == mod.ModelId && x.ColorId == mod.ColorId && x.OtherSpecs == mod.Specs) && x.SKUId != mod.SKUId).AnyAsync();
                if (!IsExist)
                {
                    var tbl = await db.Itm_Master.SingleOrDefaultAsync(x => x.SKUId == mod.SKUId);
                    if (tbl != null)
                    {
                        var model = (await db.Itm_Model.FindAsync(mod.ModelId));

                        mod.SKUCode = model.Itm_Type.TypeName + "_" + model.Model;

                        if (mod.ColorId > 0)
                        {
                            var col = await db.Itm_Color.FindAsync(mod.ColorId);
                            mod.SKUCode = mod.SKUCode + "_" + col.ColorCode;
                        }
                        if (!string.IsNullOrEmpty(mod.Specs))
                        {
                            mod.SKUCode = mod.SKUCode + "_" + mod.Specs;
                        }
                        tbl.SKUName = mod.SKUCode;
                        tbl.BarcodeValue = mod.BarcodeValue;
                        tbl.ColorId = mod.ColorId;
                        tbl.Description = mod.Description;
                        tbl.OtherSpecs = mod.Specs;
                        //tbl.PairId = mod.PairId;
                        tbl.SKUCode = mod.SKUCode;
                        tbl.DefinedBy = UserId;
                        tbl.DefinedDate = DateTime.Now;
                        tbl.AvailableForSale = mod.AvailableForSale;
                        tbl.AvailableForPurchase = mod.AvailableForPurchase;
                        tbl.IsPair = mod.IsPaired;
                        tbl.Capacity = mod.Capacity;
                        var pairLst = tbl.Itm_SKUPair.Select(x => x.PairId).ToList();

                        if (!string.IsNullOrEmpty(mod.Barcodes))
                        {
                            string[] arr = mod.Barcodes.Split(',');
                            foreach (var v in arr)
                            {
                                var barcode = await db.Itm_Barcode.Where(x => x.SKUId == mod.SKUId && x.Barcode == v).FirstOrDefaultAsync();
                                if (barcode == null)
                                {
                                    db.Itm_Barcode.Add(new Itm_Barcode
                                    {
                                        Barcode = v,
                                        SKUId = tbl.SKUId
                                    });
                                    await db.SaveChangesAsync();
                                }
                            }
                        }

                        if (mod.PairLst == null)
                        {
                            mod.PairLst = new List<int>();
                        }
                        var remLst = pairLst.Except(mod.PairLst);
                        foreach (var item in remLst)
                        {
                            var itm = await db.Itm_SKUPair.Where(x => x.SKUId == mod.SKUId && x.PairId == item).FirstOrDefaultAsync();
                            db.Itm_SKUPair.Remove(itm);
                        }
                        var newLst = mod.PairLst.Except(pairLst);
                        foreach (var item in newLst)
                        {
                            db.Itm_SKUPair.Add(new Itm_SKUPair { PairId = item, SKUId = tbl.SKUId });
                        }
                    }
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroySKU(SKUVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Itm_Master.SingleOrDefaultAsync(x => x.SKUId.Equals(mod.SKUId));
                if (tbl != null)
                {
                    tbl.ActiveStatus = false;
                    tbl.DefinedBy = UserId;
                    tbl.DefinedDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //public async Task<IEnumerable<SKUVM>> CreateSKU(IEnumerable<SKUVM> mod,int ModelId, int UserId)
        //{
        //    try
        //    {
        //        foreach (var item in mod)
        //        {
        //            var tbl = await db.Itm_Master.Where(x => x.ModelId == item.ModelId && x.ColorId == item.ColorId
        //            && x.OtherSpecs == item.Specs && x.SKUName == item.SKUName).FirstOrDefaultAsync();
        //            if(tbl == null)
        //            {
        //                tbl = new Itm_Master
        //                {
        //                    ActiveStatus = true,
        //                    SKUName = item.SKUName,
        //                    ColorId = item.ColorId,
        //                    DefinedBy = UserId,
        //                    DefinedDate = DateTime.Now,
        //                    ModelId = ModelId,
        //                    OtherSpecs = item.Specs,
        //                    SKUCode = item.SKUName
        //                };
        //                db.Itm_Master.Add(tbl);
        //                await db.SaveChangesAsync();

        //            }
        //            else
        //            {
        //                if(!tbl.ActiveStatus)
        //                {
        //                    tbl.ActiveStatus = true;
        //                    await db.SaveChangesAsync();
        //                }
        //            }
        //            item.SKUId = tbl.SKUId;
        //        }
        //        var skuLst = mod.Select(x => x.SKUId);
        //        var remLst = await db.Itm_Master.Where(x => x.ModelId == ModelId && x.ActiveStatus && !skuLst.Contains(x.SKUId)).ToListAsync();
        //        remLst.ForEach(x => x.ActiveStatus = false);
        //        return mod;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        #endregion

        #region ItemCashPrice

        public async Task<List<ItemCashPriceVM>> SKUSearchList(int ComId, int ProductId, int ModelId)
        {
            var lst = await db.Itm_Master.Where(x => (x.ModelId == ModelId || ModelId == 0) &&
            (x.Itm_Model.Itm_Type.ComId == ComId || ComId == 0) &&
            (x.Itm_Model.Itm_Type.ProductId == ProductId || ProductId == 0)).Select(x =>
            new ItemCashPriceVM
            {
                CashPrice = 0,
                //MRP = 0,
                Model = x.Itm_Model.Model,
                SKU = x.SKUCode,
                SKUId = x.SKUId,
                Type = x.Itm_Model.Itm_Type.TypeName
            }).ToListAsync();
            return lst;
        }
        public async Task<List<ItemCashPriceVM>> SKUCashPriceList(int ComId, int ProductId, int ModelId, int[] CityLst, int[] LocLst)
        {
            var dt = DateTime.Now.Date.AddDays(1);
            var lst = await (from I in db.Itm_Master
                             where (I.ModelId == ModelId || ModelId == 0) &&
             (I.Itm_Model.Itm_Type.ComId == ComId || ComId == 0) &&
             (I.Itm_Model.Itm_Type.ProductId == ProductId || ProductId == 0) &&
             I.ActiveStatus
                             select
                             new ItemCashPriceVM
                             {
                                 City = "All",
                                 Location = "All",
                                 CashPrice = 0,
                                 //MRP = 0,
                                 Model = I.Itm_Model.Model,
                                 SKU = I.SKUCode,
                                 SKUId = I.SKUId,
                                 Type = I.Itm_Model.Itm_Type.TypeName
                             }).ToListAsync();

            foreach (var v in lst)
            {
                if (LocLst.Length > 0)
                {
                    foreach (var l in LocLst)
                    {
                        var tbl = await db.Itm_SPrice.Where(x => x.SKUId == v.SKUId && x.LocId == l && x.Status && x.EffectedFrom <= dt).OrderByDescending(x => x.RowId).FirstOrDefaultAsync();
                        if (tbl != null)
                        {
                            var loc = db.Comp_Locations.Find(l);
                            v.City = loc.Comp_City.City;
                            v.Location = loc.LocName;
                            v.CashPrice = tbl.CashPrice;
                            break;
                        }
                    }
                }
                else if (CityLst.Length > 0)
                {
                    foreach (var c in CityLst)
                    {
                        var tbl = await db.Itm_SPrice.Where(x => x.SKUId == v.SKUId && x.CityId == c && x.LocId == 0 && x.Status && x.EffectedFrom <= dt).OrderByDescending(x => x.RowId).FirstOrDefaultAsync();
                        if (tbl != null)
                        {
                            var cit = db.Comp_City.Find(c);
                            v.City = cit.City;
                            v.Location = "All";
                            v.CashPrice = tbl.CashPrice;
                            break;
                        }
                    }
                }
                else
                {
                    var tbl = await db.Itm_SPrice.Where(x => x.SKUId == v.SKUId && x.CityId == 0 && x.LocId == 0 && x.Status && x.EffectedFrom <= dt).OrderByDescending(x => x.RowId).FirstOrDefaultAsync();
                    if (tbl != null)
                    {
                        v.City = "All";
                        v.Location = "All";
                        v.CashPrice = tbl.CashPrice;

                    }
                }
            }
            return lst;
        }
        public async Task<bool> CreateItemCashPrice(IEnumerable<ItemCashPriceVM> mod, int[] CityLst, int[] LocLst, DateTime EffectiveDate, int UserId)
        {
            try
            {
                foreach (var v in mod)
                {
                    if (v.CashPrice > 0)
                    {
                        if (LocLst.Length > 0)
                        {
                            foreach (var l in LocLst)
                            {
                                //var tbl = await db.Itm_SPrice.Where(x => x.SKUId == v.SKUId && x.LocId == l && x.Status).FirstOrDefaultAsync();
                                //if (tbl != null)
                                //{
                                //    tbl.Status = false;
                                //}
                                var newtbl = new Itm_SPrice
                                {
                                    CashPrice = v.CashPrice,
                                    CityId = db.Comp_Locations.Find(l).CityId,
                                    EffectedFrom = EffectiveDate,
                                    LocId = l,
                                    SKUId = v.SKUId,
                                    Status = true,
                                    TransDate = DateTime.Now,
                                    UserId = UserId
                                };
                                db.Itm_SPrice.Add(newtbl);

                            }
                        }
                        else if (CityLst.Length > 0)
                        {
                            foreach (var c in CityLst)
                            {
                                //var tbl = await db.Itm_SPrice.Where(x => x.SKUId == v.SKUId && x.CityId == c && x.Status).FirstOrDefaultAsync();
                                //if (tbl != null)
                                //{
                                //    tbl.Status = false;
                                //}
                                var newtbl = new Itm_SPrice
                                {
                                    CashPrice = v.CashPrice,
                                    CityId = c,
                                    EffectedFrom = EffectiveDate,
                                    LocId = 0,
                                    SKUId = v.SKUId,
                                    Status = true,
                                    TransDate = DateTime.Now,
                                    UserId = UserId
                                };
                                db.Itm_SPrice.Add(newtbl);
                            }
                        }
                        else
                        {
                            //var tbl = await db.Itm_SPrice.Where(x => x.SKUId == v.SKUId && x.CityId == 0 && x.Status).FirstOrDefaultAsync();
                            //if (tbl != null)
                            //{
                            //    tbl.Status = false;
                            //}
                            var newtbl = new Itm_SPrice
                            {
                                CashPrice = v.CashPrice,
                                CityId = 0,
                                EffectedFrom = EffectiveDate,
                                LocId = 0,
                                SKUId = v.SKUId,
                                Status = true,
                                TransDate = DateTime.Now,
                                UserId = UserId
                            };
                            db.Itm_SPrice.Add(newtbl);
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
        public async Task<List<LocationVM>> LocationByCitiesList(int[] CityLst)
        {
            try
            {
                return await db.Comp_Locations.Where(x => x.Status && CityLst.Contains(x.CityId)).Select(x =>
                new LocationVM
                {
                    LocId = x.LocId,
                    LocName = x.LocName
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region ItemCashPriceV2

        public async Task<List<SKUPlanVM>> SKUSearchListV2(int ComId, int ProductId, int ModelId)
        {
            var lst = await (from master in db.Itm_Master
                             join skuplan in db.Itm_SKUPlan on master.SKUId equals skuplan.SKUId
                             where (master.ModelId == ModelId || ModelId == 0) && (master.Itm_Model.Itm_Type.ComId == ComId || ComId == 0)
                             && skuplan.Status == true
                             && (master.Itm_Model.Itm_Type.ProductId == ProductId || ProductId == 0)
                             select new SKUPlanVM()
                             {
                                 Advance = skuplan.Advance,
                                 CityId = skuplan.CityId,
                                 Duration = skuplan.Duration,
                                 SKUId = skuplan.SKUId,
                                 EffectedDate = skuplan.EffectedDate,
                                 Inst = skuplan.Inst,
                                 InstPrice = skuplan.InstPrice,
                                 BasePrice = skuplan.BasePrice,
                                 LocId = skuplan.LocId,
                                 RowId = skuplan.RowId,
                                 Model = master.Itm_Model.Model,
                                 SKU = master.SKUCode,
                                 Type = master.Itm_Model.Itm_Type.TypeName
                             }).ToListAsync();
            return lst;
        }

        public async Task<IEnumerable<SKUPlanVM>> UpdateSkuPlanV2(IEnumerable<SKUPlanVM> mod, int UserId)
        {
            try
            {
                foreach (var v in mod)
                {
                    var tbl = await db.Itm_SKUPlan.Where(x => x.CityId == v.CityId && x.LocId == v.LocId && x.SKUId == v.SKUId && x.Duration == v.Duration && x.Status).FirstOrDefaultAsync();
                    if (tbl != null)
                    {
                        if (v.InstPrice != tbl.InstPrice)
                        {
                            var nwtbl = new Itm_SKUPlan()
                            {
                                Advance = v.Advance,
                                CityId = v.CityId,
                                Duration = v.Duration,
                                SKUId = v.SKUId,
                                EffectedDate = v.EffectedDate,
                                Inst = v.Inst,
                                InstPrice = v.InstPrice,
                                BasePrice = v.BasePrice,
                                LocId = v.LocId,
                                Status = true,
                                TransDate = DateTime.Now,
                                UserId = UserId
                            };
                            db.Itm_SKUPlan.Add(nwtbl);
                            await db.SaveChangesAsync();
                            v.RowId = nwtbl.RowId;
                        }
                    }
                    else
                    {
                        var newtbl = new Itm_SKUPlan()
                        {
                            Advance = v.Advance,
                            CityId = v.CityId,
                            Duration = v.Duration,
                            SKUId = v.SKUId,
                            EffectedDate = v.EffectedDate,
                            Inst = v.Inst,
                            InstPrice = v.InstPrice,
                            BasePrice = v.BasePrice,
                            LocId = v.LocId,
                            Status = true,
                            TransDate = DateTime.Now,
                            UserId = UserId
                        };
                        db.Itm_SKUPlan.Add(newtbl);
                        await db.SaveChangesAsync();
                        v.RowId = newtbl.RowId;
                    }
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }


        #endregion

        #region Supplier
        public async Task<List<SupplierVM>> SupplierList()
        {
            try
            {
                return await db.Inv_Suppliers.Where(x => x.Status).Select(x =>
                new SupplierVM
                {
                    SuppId = x.SuppId,
                    SuppName = x.SuppName,
                    CategoryId = x.CategoryId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SupplierVM>> SupplierMobilesList()
        {
            try
            {
                return await db.Inv_Suppliers.Where(x => x.Status && x.CategoryId == 2).Select(x =>
                new SupplierVM
                {
                    SuppId = x.SuppId,
                    SuppName = x.SuppName
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SupplierVM>> SupplierAppliancesList()
        {
            try
            {
                return await db.Inv_Suppliers.Where(x => x.Status && (x.CategoryId == 1 || x.CategoryId == 3)).Select(x =>
                new SupplierVM
                {
                    SuppId = x.SuppId,
                    SuppName = x.SuppName
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SupplierVM>> SupplierLocalList()
        {
            try
            {
                return await db.Inv_Suppliers.Where(x => x.Status && (x.CategoryId == 1 || x.CategoryId == 3)).Select(x =>
                new SupplierVM
                {
                    SuppId = x.SuppId,
                    SuppName = x.SuppName
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Comp_PaymentTerms>> PaymentTermsList()
        {
            try
            {
                return await db.Comp_PaymentTerms.ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Inv_SuppliersCat>> SupplierCatList()
        {
            try
            {
                return await db.Inv_SuppliersCat.ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<SupplierDetailVM>> SupplierDetailList()
        {
            try
            {
                return await db.Inv_Suppliers.Where(x => x.Status).Select(x =>
                new SupplierDetailVM
                {
                    SuppId = x.SuppId,
                    SuppName = x.SuppName,
                    Address = x.Address,
                    CategoryId = x.CategoryId,
                    PaymentTermId = x.PaymentTermId,
                    Email = x.Email,
                    GLCode = x.GLCode,
                    ContactPerson = x.ContactPerson,
                    Mobile = x.Mobile,
                    NTN = x.NTN,
                    PhoneOff = x.PhoneOff,
                    PhoneRes = x.PhoneRes,
                    STRN = x.STRN,
                    TaxRate = x.TaxRate,
                    GST = x.GST ?? 0,
                    WHT = x.WHT ?? 0,
                    TaxAppliedOn = x.TaxAppliedOn,
                    CompanyLst = x.Inv_SuppliersMapping.Where(a => a.Status).Select(a => a.ComId).ToList(),
                    CityLst = x.Inv_SuppliersCity.Select(a => a.CityId).ToList(),
                    IsReg = x.IsReg,
                    CompanyName = x.CompanyName,
                    RegisteredAddress = x.RegisteredAddress,
                    RegisteredName = x.ChequeName
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<SupplierDetailVM> CreateSupplier(SupplierDetailVM mod, int UserId)
        {
            try
            {
                Inv_Suppliers tbl = new Inv_Suppliers
                {
                    Status = true,
                    SuppName = mod.SuppName,
                    TransDate = DateTime.Now,
                    UserID = UserId,
                    Address = mod.Address,
                    ContactPerson = mod.ContactPerson,
                    Mobile = mod.Mobile,
                    NTN = mod.NTN,
                    PhoneOff = mod.PhoneOff,
                    PhoneRes = mod.PhoneRes,
                    STRN = mod.STRN,
                    TaxRate = mod.TaxRate,
                    GLCode = mod.GLCode,
                    CategoryId = mod.CategoryId,
                    Email = mod.Email,
                    PaymentTermId = mod.PaymentTermId,
                    WHT = mod.WHT,
                    GST = mod.GST,
                    TaxAppliedOn = mod.TaxAppliedOn,
                    IsReg = mod.IsReg,
                    ChequeName = mod.RegisteredName,
                    RegisteredAddress = mod.RegisteredAddress,
                    CompanyName = mod.CompanyName
                };
                if (mod.CompanyLst != null)
                {
                    List<Inv_SuppliersMapping> mapp = new List<Inv_SuppliersMapping>();

                    foreach (var item in mod.CompanyLst)
                    {
                        mapp.Add(new Inv_SuppliersMapping
                        {
                            ComId = item,
                            LogDate = DateTime.Now,
                            Status = true,
                            UserId = UserId
                        });
                    }
                    tbl.Inv_SuppliersMapping = mapp;
                }
                if (mod.CityLst != null)
                {
                    List<Inv_SuppliersCity> city = new List<Inv_SuppliersCity>();
                    foreach (var item in mod.CityLst)
                    {
                        city.Add(new Inv_SuppliersCity
                        {
                            CityId = item
                        });
                    }
                    tbl.Inv_SuppliersCity = city;
                }
                db.Inv_Suppliers.Add(tbl);
                await db.SaveChangesAsync();
                mod.SuppId = tbl.SuppId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateSupplier(SupplierDetailVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Inv_Suppliers.SingleOrDefaultAsync(x => x.SuppId.Equals(mod.SuppId));
                if (tbl != null)
                {
                    tbl.SuppName = mod.SuppName;
                    tbl.TransDate = DateTime.Now;
                    tbl.UserID = UserId;
                    tbl.Address = mod.Address;
                    tbl.CategoryId = mod.CategoryId;
                    tbl.ContactPerson = mod.ContactPerson;
                    tbl.Mobile = mod.Mobile;
                    tbl.Email = mod.Email;
                    tbl.GLCode = mod.GLCode;
                    tbl.PaymentTermId = mod.PaymentTermId;
                    tbl.NTN = mod.NTN;
                    tbl.PhoneOff = mod.PhoneOff;
                    tbl.PhoneRes = mod.PhoneRes;
                    tbl.STRN = mod.STRN;
                    tbl.TaxRate = mod.TaxRate;
                    tbl.WHT = mod.WHT;
                    tbl.GST = mod.GST;
                    tbl.TaxAppliedOn = mod.TaxAppliedOn;
                    tbl.IsReg = mod.IsReg;
                    tbl.ChequeName = mod.RegisteredName;
                    tbl.RegisteredAddress = mod.RegisteredAddress;
                    tbl.CompanyName = mod.CompanyName;

                    if (mod.CompanyLst == null)
                    {
                        mod.CompanyLst = new List<int>();
                    }
                    var compLst = await db.Inv_SuppliersMapping.Where(x => x.SupplierId == mod.SuppId && !mod.CompanyLst.Contains(x.ComId) && x.Status).ToListAsync();
                    foreach (var item in compLst)
                    {
                        item.LogDate = DateTime.Now;
                        item.Status = false;
                        item.UserId = UserId;
                    }


                    foreach (var item in mod.CompanyLst)
                    {
                        var comp = await db.Inv_SuppliersMapping.Where(x => x.SupplierId == mod.SuppId && x.ComId == item && x.Status).FirstOrDefaultAsync();
                        if (comp == null)
                        {
                            db.Inv_SuppliersMapping.Add(new Inv_SuppliersMapping
                            {
                                ComId = item,
                                LogDate = DateTime.Now,
                                Status = true,
                                UserId = UserId,
                                SupplierId = mod.SuppId
                            });
                        }
                    }

                    if (mod.CityLst == null)
                    {
                        mod.CityLst = new List<int>();
                    }
                    var cityLst = await db.Inv_SuppliersCity.Where(x => x.SuppId == mod.SuppId && !mod.CityLst.Contains(x.CityId)).ToListAsync();
                    db.Inv_SuppliersCity.RemoveRange(cityLst);

                    foreach (var item in mod.CityLst)
                    {
                        var city = await db.Inv_SuppliersCity.Where(x => x.SuppId == mod.SuppId && x.CityId == item).FirstOrDefaultAsync();
                        if (city == null)
                        {
                            db.Inv_SuppliersCity.Add(new Inv_SuppliersCity
                            {
                                CityId = item,
                                SuppId = mod.SuppId
                            });
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

        public async Task<bool> DestroySupplier(SupplierDetailVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Inv_Suppliers.SingleOrDefaultAsync(x => x.SuppId.Equals(mod.SuppId));
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.UserID = UserId;
                    tbl.TransDate = DateTime.Now;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<SupplierVM>> SupplierByCategory(int CategoryId)
        {
            try
            {
                return await (from S in db.Inv_Suppliers
                              where S.Status && S.CategoryId == CategoryId
                              select
                 new SupplierVM
                 {
                     SuppId = S.SuppId,
                     SuppName = S.SuppName
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SupplierVM>> SupplierByComList(int ComId)
        {
            try
            {
                return await (from S in db.Inv_Suppliers
                              join M in db.Inv_SuppliersMapping on S.SuppId equals M.SupplierId
                              where S.Status && M.Status && M.ComId == ComId
                              select
                 new SupplierVM
                 {
                     SuppId = S.SuppId,
                     SuppName = S.SuppName
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region ItemDiscount
        public async Task<List<DiscTypeVM>> DiscList()
        {
            try
            {
                return await db.Itm_PDiscType.Where(x => x.Status).Select(x =>
                new DiscTypeVM
                {
                    PDiscTypeId = x.PDiscTypeId,
                    DiscType = x.DiscType
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<IEnumerable<ItmDiscVM>> CreateItemDiscount(IEnumerable<ItmDiscVM> mod, int UserId)
        {
            try
            {
                foreach (var m in mod)
                {
                    var row = new Itm_PDisc
                    {
                        SuppId = m.SuppId,
                        ProductId = m.ProductId,
                        PDiscTypeId = m.PDiscTypeId,
                        Amount = m.Amount,
                        FromDate = m.FromDate,
                        ToDate = m.ToDate,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        Status = true
                    };
                    db.Itm_PDisc.Add(row);
                    await db.SaveChangesAsync();
                    m.RowId = row.RowId;
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<IEnumerable<ItmDiscVM>> DestroyItemDiscount(IEnumerable<ItmDiscVM> mod, int UserId)
        {
            try
            {
                foreach (var m in mod)
                {
                    var tbl = await db.Itm_PDisc.FindAsync(m.RowId);
                    tbl.Status = false;
                    tbl.TransDate = DateTime.Now;
                    tbl.UserId = UserId;
                    await db.SaveChangesAsync();
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region ModelPairPolicy
        public async Task<List<ModelPairPolicyVM>> ModelPairPolicyList(int ModelId)
        {
            try
            {
                return await db.Itm_PairPolicy.Where(x => x.Status && x.ModelId == ModelId).Select(x =>
                new ModelPairPolicyVM
                {
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    ExtraCharges = x.ExtraCharges,
                    ModelId = x.ModelId,
                    PolicyId = x.PolicyId,
                    RatioGiftQty = x.RatioGiftQty,
                    RatioPurQty = x.RatioPurQty,
                    Remarks = x.Remarks,
                    FOCModelId = x.FOCModelId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ModelPairPolicyVM> CreateModelPairPolicy(ModelPairPolicyVM mod, int UserId)
        {
            try
            {
                Itm_PairPolicy tbl = new Itm_PairPolicy
                {
                    FromDate = mod.FromDate,
                    ToDate = mod.ToDate,
                    ExtraCharges = mod.ExtraCharges,
                    ModelId = mod.ModelId,
                    PolicyId = mod.PolicyId,
                    RatioGiftQty = mod.RatioGiftQty,
                    RatioPurQty = mod.RatioPurQty,
                    Remarks = mod.Remarks,
                    Status = true,
                    TransDate = DateTime.Now,
                    UserId = UserId,
                    FOCModelId = mod.FOCModelId
                };
                db.Itm_PairPolicy.Add(tbl);
                await db.SaveChangesAsync();
                mod.PolicyId = tbl.PolicyId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateModelPairPolicy(ModelPairPolicyVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Itm_PairPolicy.SingleOrDefaultAsync(x => x.PolicyId == mod.PolicyId);
                if (tbl != null)
                {
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
                    tbl.RatioPurQty = mod.RatioPurQty;
                    tbl.RatioGiftQty = mod.RatioGiftQty;
                    tbl.FromDate = mod.FromDate;
                    tbl.ToDate = mod.ToDate;
                    tbl.ExtraCharges = mod.ExtraCharges;
                    tbl.Remarks = mod.Remarks;
                    tbl.FOCModelId = mod.FOCModelId;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DestroyModelPairPolicy(ModelPairPolicyVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Itm_PairPolicy.SingleOrDefaultAsync(x => x.PolicyId == mod.PolicyId);
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
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

        #region ModelDiscount

        public async Task<List<ModelDiscountVM>> DiscountPolicyList(int TypeId)
        {
            try
            {
                return await db.Itm_DiscountPolicy.Where(x => x.Status && x.TypeId == TypeId).Select(x =>
                new ModelDiscountVM
                {
                    EndDate = x.EndDate,
                    PolicyId = x.PolicyId,
                    StartDate = x.StartDate
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<int>> DiscountPolicyModelList(int PolicyId)
        {
            try
            {
                return await db.Itm_DiscountPolicyModel.Where(x => x.PolicyId == PolicyId).Select(x =>
                x.ModelId).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ModelDiscountSlabVM>> ModelDiscountList(int PolicyId)
        {
            try
            {
                return await db.Itm_DiscountPolicySlab.Where(x => x.PolicyId == PolicyId && x.Status).Select(x =>
                new ModelDiscountSlabVM
                {
                    ExtraQtyRate = x.ExtraQtyRate,
                    IncentiveAmt = x.IncentiveAmt,
                    MaxSlabQty = x.MaxSlabQty,
                    MinSlabQty = x.MinSlabQty,
                    PolicyId = x.PolicyId,
                    RowId = x.RowId,
                    Tour = x.Tour
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<int> CreateModelDiscount(IEnumerable<ModelDiscountSlabVM> mod, int PolicyId, int[] ModelLst, DateTime StartDate, DateTime EndDate, int TypeId, int UserId)
        {
            try
            {
                if (PolicyId > 0)
                {
                    var policy = await db.Itm_DiscountPolicy.FindAsync(PolicyId);
                    foreach (var v in mod)
                    {
                        policy.Itm_DiscountPolicySlab.Add(new Itm_DiscountPolicySlab
                        {
                            ExtraQtyRate = v.ExtraQtyRate,
                            IncentiveAmt = v.IncentiveAmt,
                            MaxSlabQty = v.MaxSlabQty,
                            MinSlabQty = v.MinSlabQty,
                            PolicyId = v.PolicyId,
                            Status = true,
                            Tour = v.Tour
                        });
                    }
                    await db.SaveChangesAsync();
                    //policy.Itm_DiscountPolicyModel = ModelLst.Select(x =>
                    //new Itm_DiscountPolicyModel
                    //{
                    //    ModelId = x,
                    //    Status = true,
                    //    PolicyId = PolicyId
                    //}).ToList();

                    //var remLst = policy.Itm_DiscountPolicyModel.Where(x => !ModelLst.Contains(x.ModelId)).ToList();
                    //remLst.ForEach(x => x.Status = false);
                    //var updLst = policy.Itm_DiscountPolicyModel.Where(x => ModelLst.Contains(x.ModelId) && !x.Status).ToList();
                    //updLst.ForEach(x => x.Status = true);
                    //var newLst = ModelLst.Where(a => policy.Itm_DiscountPolicyModel.Select(x => x.ModelId).ToList().Contains(a)).Select(x => 
                    //new Itm_DiscountPolicyModel { ModelId = x})
                }
                else
                {
                    Itm_DiscountPolicy tbl = new Itm_DiscountPolicy
                    {
                        EndDate = EndDate,
                        StartDate = StartDate,
                        Status = true,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        TypeId = TypeId
                    };
                    tbl.Itm_DiscountPolicyModel = ModelLst.Select(x =>
                    new Itm_DiscountPolicyModel
                    {
                        ModelId = x,
                        Status = true
                    }).ToList();
                    tbl.Itm_DiscountPolicySlab = mod.Select(v => new Itm_DiscountPolicySlab
                    {
                        ExtraQtyRate = v.ExtraQtyRate,
                        IncentiveAmt = v.IncentiveAmt,
                        MaxSlabQty = v.MaxSlabQty,
                        MinSlabQty = v.MinSlabQty,
                        PolicyId = v.PolicyId,
                        Status = true,
                        Tour = v.Tour
                    }).ToList();

                    db.Itm_DiscountPolicy.Add(tbl);
                    await db.SaveChangesAsync();
                    PolicyId = tbl.PolicyId;
                }
                return PolicyId;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<bool> UpdateModelDiscount(IEnumerable<ModelDiscountSlabVM> mod, int UserId)
        {
            try
            {
                foreach (var v in mod)
                {
                    var tbl = await db.Itm_DiscountPolicySlab.FindAsync(v.RowId);
                    tbl.ExtraQtyRate = v.ExtraQtyRate;
                    tbl.IncentiveAmt = v.IncentiveAmt;
                    tbl.MaxSlabQty = v.MaxSlabQty;
                    tbl.MinSlabQty = v.MinSlabQty;
                    tbl.Tour = v.Tour;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DestroyModelDiscount(IEnumerable<ModelDiscountSlabVM> mod, int UserId)
        {
            try
            {
                foreach (var v in mod)
                {
                    var tbl = await db.Itm_DiscountPolicySlab.FindAsync(v.RowId);
                    if (tbl != null)
                    {
                        tbl.Status = false;
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

        #region PPrice
        public async Task<IEnumerable<PPriceVM>> PPriceUploader(IEnumerable<PPriceVM> mod, int UserId)
        {
            try
            {
                foreach (var m in mod)
                {
                    var tbl = await db.Itm_PPrice.Where(x => x.SKUId == m.SKUId && x.SuppId == m.SuppId && x.Status
                    && x.FromDate >= m.FromDate && x.ToDate <= m.ToDate).FirstOrDefaultAsync();
                    if (tbl != null)
                    {
                        tbl.Status = false;
                    }
                    if (m.MRP > 0 || m.TP > 0)
                    {
                        var newtbl = new Itm_PPrice
                        {
                            SuppId = m.SuppId,
                            SKUId = m.SKUId,
                            TP = m.TP,
                            MRP = m.MRP,
                            Discount = m.Discount,
                            FromDate = m.FromDate,
                            ToDate = m.ToDate,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            Status = true,
                            AnnuallyIncentive = m.AnnuallyIncentive,
                            BiannuallyIncentive = m.BiannuallyIncentive,
                            MonthlyIncentive = m.MonthlyIncentive,
                            QuarterlyIncentive = m.QuarterlyIncentive,
                            Remarks = m.Remarks
                        };
                        db.Itm_PPrice.Add(newtbl);
                        await db.SaveChangesAsync();
                        m.RowId = newtbl.RowId;
                    }
                    else
                    {
                        await db.SaveChangesAsync();
                    }
                }
                return mod;
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return null;
            }
        }
        public async Task<IEnumerable<PPriceVM>> CreateItemPPrice(IEnumerable<PPriceVM> mod, DateTime FromDate, DateTime ToDate, int[] SuppId, int UserId)
        {
            try
            {
                foreach (var v in mod)
                {
                    foreach (var s in SuppId)
                    {
                        var tbl = await db.Itm_PPrice.Where(x => x.SKUId == v.SKUId && x.SuppId == s && x.Status
                        && x.FromDate >= FromDate && x.ToDate <= ToDate).FirstOrDefaultAsync();
                        if (tbl != null)
                        {
                            tbl.Status = false;
                        }
                        if (v.MRP > 0 || v.TP > 0)
                        {
                            var newtbl = new Itm_PPrice
                            {
                                SKUId = v.SKUId,
                                Discount = v.Discount,
                                FromDate = FromDate,
                                TP = v.TP,
                                MRP = v.MRP,
                                SuppId = s,
                                ToDate = ToDate,
                                Status = true,
                                TransDate = DateTime.Now,
                                UserId = UserId,
                                AnnuallyIncentive = v.AnnuallyIncentive,
                                BiannuallyIncentive = v.BiannuallyIncentive,
                                MonthlyIncentive = v.MonthlyIncentive,
                                QuarterlyIncentive = v.QuarterlyIncentive,
                                Remarks = v.Remarks
                            };
                            db.Itm_PPrice.Add(newtbl);
                            await db.SaveChangesAsync();
                            v.RowId = newtbl.RowId;
                        }
                        else
                        {
                            await db.SaveChangesAsync();
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

        public async Task<List<PPriceVM>> ItemPPriceList(int ComId, int ProductId, int[] SuppId, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return await db.Itm_PPrice.Where(x => x.Status && x.Itm_Master.Itm_Model.Itm_Type.ComId == ComId && x.Itm_Master.Itm_Model.Itm_Type.ProductId == ProductId && SuppId.Contains(x.SuppId) && x.FromDate >= FromDate && x.ToDate <= ToDate).Select(v =>
               new PPriceVM
               {
                   SKUId = v.SKUId,
                   Discount = v.Discount,
                   TP = v.TP,
                   MRP = v.MRP,
                   AnnuallyIncentive = v.AnnuallyIncentive,
                   BiannuallyIncentive = v.BiannuallyIncentive,
                   MonthlyIncentive = v.MonthlyIncentive,
                   QuarterlyIncentive = v.QuarterlyIncentive,
                   RowId = v.RowId,
                   SKU = v.Itm_Master.SKUName,
                   Supplier = v.Inv_Suppliers.SuppName,
                   Remarks = v.Remarks
               }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        //public async Task<PPriceVM> CreatePPrice(PPriceVM mod, int UserId)
        //{
        //    try
        //    {
        //        Itm_PPrice tbl = new Itm_PPrice
        //        {
        //            SKUId = mod.SKUId,
        //            Discount = mod.Discount,
        //            FromDate = mod.FromDate,
        //            TP = mod.TP,
        //            MRP = mod.MRP,
        //            RowId = mod.RowId,
        //            SuppId = mod.SuppId,
        //            ToDate = mod.ToDate,
        //            Status = true,
        //            TransDate = DateTime.Now,
        //            UserId = UserId
        //        };
        //        db.Itm_PPrice.Add(tbl);
        //        await db.SaveChangesAsync();
        //        mod.RowId = tbl.RowId;
        //        return mod;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        //public async Task<bool> UpdatePPrice(PPriceVM mod, int UserId)
        //{
        //    try
        //    {
        //        var tbl = await db.Itm_PPrice.SingleOrDefaultAsync(x => x.RowId == mod.RowId);
        //        if (tbl != null)
        //        {
        //            tbl.UserId = UserId;
        //            tbl.TransDate = DateTime.Now;
        //            tbl.Discount = mod.Discount;
        //            tbl.FromDate = mod.FromDate;
        //            tbl.TP = mod.TP;
        //            tbl.MRP = mod.MRP;
        //            tbl.SuppId = mod.SuppId;
        //            tbl.ToDate = mod.ToDate;
        //        }
        //        await db.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        //public async Task<bool> DestroyPPrice(PPriceVM mod, int UserId)
        //{
        //    try
        //    {
        //        var tbl = await db.Itm_PPrice.SingleOrDefaultAsync(x => x.RowId == mod.RowId);
        //        if (tbl != null)
        //        {
        //            tbl.Status = false;
        //            tbl.UserId = UserId;
        //            tbl.TransDate = DateTime.Now;
        //        }
        //        await db.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        #endregion

        #region TypePlan
        public async Task<List<TypePlanVM>> TypePlanList()
        {
            try
            {
                var lst = await db.Itm_TypePlanMaster.Where(x => x.Status).Select(x => new TypePlanVM
                {
                    //Duration = x.Key.Duration,
                    //IsLocal = x.Key.IsLocal,
                    //MarkUp = x.Key.MarkUp,
                    //MaxAdvance = x.Key.MaxAdvance,
                    //MinAdvance = x.Key.MinAdvance,
                    //RegFee = x.Key.RegFee,
                    //Loc = x.Select(a => a.LocId).ToList(),
                    //Type = x.Select(a => a.TypeId).ToList(),
                    Loc = x.Itm_TypePlan.Where(a => a.Status).Select(a => a.LocId).Distinct().ToList(),
                    Type = x.Itm_TypePlan.Where(a => a.Status).Select(a => a.TypeId).Distinct().ToList(),
                    Title = x.Title,
                    EffectiveDate = x.EffectiveFrom,
                    PolicyId = x.PolicyId,
                    EndDate = x.EndDate
                }).ToListAsync();


                foreach (var item in lst)
                {
                    var plan = await db.Itm_TypePlan.Where(x => x.PolicyId == item.PolicyId && x.Status).FirstOrDefaultAsync();
                    item.Duration = plan.Duration;
                    item.IsLocal = plan.IsLocal;
                    item.MarkUp = plan.MarkUp;
                    item.MaxAdvance = plan.MaxAdvance;
                    item.MinAdvance = plan.MinAdvance;
                    item.RegFee = plan.RegFee;

                    //item.Loc = await db.Itm_TypePlan.Where(x => x.PolicyId == item.PolicyId && x.Status).Select(x => x.LocId).Distinct().ToListAsync();
                    //item.Type = await db.Itm_TypePlan.Where(x => x.PolicyId == item.PolicyId && x.Status).Select(x => x.TypeId).Distinct().ToListAsync();
                }
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<TypePlanVM> CreateTypePlan(TypePlanVM mod, int UserId)
        {
            try
            {
                var policyExists = await db.Itm_TypePlan.Where(x => x.Duration == mod.Duration &&
                            x.IsLocal == mod.IsLocal &&
                            x.MarkUp == mod.MarkUp &&
                            x.MaxAdvance == mod.MaxAdvance &&
                            x.MinAdvance == mod.MinAdvance &&
                            x.RegFee == mod.RegFee &&
                            x.Itm_TypePlanMaster.EffectiveFrom == mod.EffectiveDate &&
                            x.Status).AnyAsync();
                if (policyExists)
                {
                    return null;
                }
                Itm_TypePlanMaster tbl = new Itm_TypePlanMaster()
                {
                    CreatedBy = UserId,
                    CreatedDate = DateTime.Now,
                    EffectiveFrom = mod.EffectiveDate,
                    Status = true,
                    Title = mod.Title,
                    EndDate = mod.EndDate
                };
                db.Itm_TypePlanMaster.Add(tbl);
                await db.SaveChangesAsync();

                foreach (var l in mod.Loc)
                {
                    foreach (var t in mod.Type)
                    {
                        Itm_TypePlan plan = new Itm_TypePlan()
                        {
                            Duration = mod.Duration,
                            IsLocal = mod.IsLocal,
                            MarkUp = mod.MarkUp,
                            LocId = l,
                            MaxAdvance = mod.MaxAdvance,
                            MinAdvance = mod.MinAdvance,
                            RegFee = mod.RegFee,
                            Status = true,
                            TypeId = t,
                            PolicyId = tbl.PolicyId
                        };
                        db.Itm_TypePlan.Add(plan);
                    }
                }
                await db.SaveChangesAsync();
                mod.PolicyId = tbl.PolicyId;

                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<TypePlanVM> UpdateTypePlan(TypePlanVM mod, int UserId)
        {
            try
            {
                var mas = await db.Itm_TypePlanMaster.FindAsync(mod.PolicyId);
                mas.ModifiedBy = UserId;
                mas.ModifiedDate = DateTime.Now;
                mas.Title = mod.Title;
                mas.EffectiveFrom = mod.EffectiveDate;
                mas.EndDate = mod.EndDate;

                var rem = await db.Itm_TypePlan.Where(x => x.PolicyId == mod.PolicyId && x.Status && (!mod.Loc.Contains(x.LocId) || !mod.Type.Contains(x.TypeId))).ToListAsync();
                rem.ForEach(x => x.Status = false);
                await db.SaveChangesAsync();

                foreach (var l in mod.Loc)
                {
                    foreach (var t in mod.Type)
                    {
                        var tbl = await db.Itm_TypePlan.Where(x => x.PolicyId == mod.PolicyId && x.LocId == l && x.TypeId == t).FirstOrDefaultAsync();
                        if (tbl == null)
                        {
                            tbl = new Itm_TypePlan
                            {
                                Status = true,
                                TypeId = t,
                                MaxAdvance = mod.MaxAdvance,
                                Duration = mod.Duration,
                                MarkUp = mod.MarkUp,
                                MinAdvance = mod.MinAdvance,
                                IsLocal = mod.IsLocal,
                                LocId = l,
                                RegFee = mod.RegFee,
                                PolicyId = mod.PolicyId
                            };
                            db.Itm_TypePlan.Add(tbl);
                        }
                        else
                        {
                            tbl.Duration = mod.Duration;
                            tbl.IsLocal = mod.IsLocal;
                            tbl.MarkUp = mod.MarkUp;
                            tbl.MaxAdvance = mod.MaxAdvance;
                            tbl.MinAdvance = mod.MinAdvance;
                            tbl.RegFee = mod.RegFee;
                            tbl.Status = true;
                        }

                    }

                }
                await db.SaveChangesAsync();

                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<TypePlanVM>> TypePlanRemainList()
        {
            try
            {
                var typeLst = await db.Itm_TypePlan.Select(x => x.TypeId).Distinct().ToListAsync();
                return await db.Itm_Type.Where(x => x.Status && !typeLst.Contains(x.TypeId)).Select(x => new TypePlanVM { Title = x.TypeName }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region SupSubsidaryAccount


        public async Task<List<SubsidaryVM>> SubsidaryList()
        {
            try
            {
                return await db.Fin_Subsidary.Select(x => new SubsidaryVM()
                {
                    SubsidaryName = x.SubsidaryName,
                    SubsidaryCode = x.SubsidaryCode,
                    SubCode = x.SubCode,
                    OldCode = x.OldCode,
                    SubId = x.SubId,
                    SubTypeId = x.SubTypeId
                }).ToListAsync();

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<long> AddSubsidaryAcc(SubsidaryVM mod)
        {
            try
            {
                var accid = Convert.ToInt64(mod.SubCode.Replace("-", ""));
                var finexs = db.Fin_SubsidaryType.Where(x => x.SubTypeId == mod.SubTypeId).FirstOrDefault();

                Fin_Subsidary fis = new Fin_Subsidary()
                {
                    SubsidaryName = mod.SubsidaryName,
                    SubCode = mod.SubCode,
                    SubsidaryCode = "",
                    SubTypeId = mod.SubTypeId,
                    OldCode = mod.OldCode,
                    AccId = accid,
                };
                db.Fin_Subsidary.Add(fis);
                await db.SaveChangesAsync();
                fis.SubsidaryCode = finexs.Abbr + "-" + fis.SubId;
                await db.SaveChangesAsync();
                return fis.SubId;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public async Task<long> EditSubsidaryAcc(SubsidaryVM mod)
        {
            try
            {
                var finex = db.Fin_Subsidary.Where(x => x.SubId == mod.SubId).FirstOrDefault();
                var accid = Convert.ToInt64(mod.SubCode.Replace("-", ""));
                if (finex != null)
                {
                    finex.SubsidaryName = mod.SubsidaryName;
                    finex.SubCode = mod.SubCode;
                    finex.SubTypeId = mod.SubTypeId;
                    finex.OldCode = mod.OldCode;

                    finex.AccId = accid;
                    await db.SaveChangesAsync();
                    finex.SubsidaryCode = finex.Fin_SubsidaryType.Abbr + "-" + finex.SubId;
                    await db.SaveChangesAsync();
                    return mod.SubId;
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

        #region SupSubsidaryAccountType
        public async Task<List<SubsiCategoryVM>> CategoryList()
        {
            return await db.Fin_Category.Select(x => new SubsiCategoryVM()
            {
                Category = x.Category,
                CatId = x.CatId
            }).ToListAsync();

        }
        public async Task<List<SubsidaryTypeVM>> SubsidaryTypeList()
        {
            return await db.Fin_SubsidaryType.Select(x => new SubsidaryTypeVM()
            {
                SubType = x.SubType,
                SubTypeId = x.SubTypeId
            }).ToListAsync();

        }


        public async Task<long> AddSubsidaryTypeAcc(SubsidaryTypeVM mod)
        {
            try
            {
                var accid = Convert.ToInt64(mod.AccCode.Replace("-", ""));
                Fin_SubsidaryType fis = new Fin_SubsidaryType()
                {
                    SubType = mod.SubType,
                    AccCode = mod.AccCode,
                    CatId = mod.CatId,
                    Abbr = mod.Abbr
                };
                db.Fin_SubsidaryType.Add(fis);
                await db.SaveChangesAsync();
                return fis.SubTypeId;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public async Task<long> EditSubsidaryTypeAcc(SubsidaryTypeVM mod)
        {
            try
            {
                var finex = db.Fin_SubsidaryType.Where(x => x.SubTypeId == mod.SubTypeId).FirstOrDefault();
                var accid = Convert.ToInt64(mod.AccCode.Replace("-", ""));
                if (finex != null)
                {
                    finex.SubType = mod.SubType;
                    finex.AccCode = mod.AccCode;
                    finex.CatId = mod.CatId;
                    finex.Abbr = mod.Abbr;
                    await db.SaveChangesAsync();
                    return mod.SubTypeId;
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

        #region SKUPlan

        public async Task<List<SKUPlanVM>> SKUPlanList(int SKUId)
        {
            try
            {
                var lst = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.Status)
                    .Select(x => new SKUPlanVM
                    {
                        Advance = x.Advance,
                        CityId = x.CityId,
                        Duration = x.Duration,
                        SKUId = x.SKUId,
                        EffectedDate = x.EffectedDate,
                        Inst = x.Inst,
                        InstPrice = x.InstPrice,
                        BasePrice = x.BasePrice,
                        LocId = x.LocId,
                        RowId = x.RowId
                    }).ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<SKUPlanVM>> CreateSKUPlan(IEnumerable<SKUPlanVM> mod, int UserId)
        {
            try
            {
                foreach (var v in mod)
                {
                    //var tbl = await db.Itm_SKUPlan.Where(x => x.CityId == v.CityId && x.LocId == v.LocId && x.SKUId == v.SKUId && x.Status && x.Duration == v.Duration).FirstOrDefaultAsync();
                    //if (tbl != null)
                    //{
                    //    tbl.Status = false;
                    //}


                    var newtbl = new Itm_SKUPlan()
                    {
                        Advance = v.Advance,
                        CityId = v.CityId,
                        Duration = v.Duration,
                        SKUId = v.SKUId,
                        EffectedDate = v.EffectedDate,
                        Inst = v.Inst,
                        InstPrice = v.InstPrice,
                        BasePrice = v.BasePrice,
                        LocId = v.LocId,
                        Status = true,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        Type = v.LocId > 0 ? "S" : "R"
                    };
                    db.Itm_SKUPlan.Add(newtbl);
                    await db.SaveChangesAsync();
                    v.RowId = newtbl.RowId;
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<SKUPlanVM>> DestroySKUPlan(IEnumerable<SKUPlanVM> mod, int UserId)
        {
            try
            {
                foreach (var v in mod)
                {
                    var tbl = await db.Itm_SKUPlan.Where(x => x.RowId == v.RowId).FirstOrDefaultAsync();
                    if (tbl != null)
                    {
                        tbl.Status = false;
                        tbl.ModifiedDate = DateTime.Now;
                        tbl.ModifiedBy = UserId;
                    }
                    await db.SaveChangesAsync();
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> LocalSKUPlan(int LocId, int SKUId, decimal PPrice, string GRNNo, long ItemId, int UserId)
        {
            try
            {
                if (PPrice > 0)
                {
                    var sku = await db.Itm_Master.FindAsync(SKUId);
                    if (sku.Itm_Model.TypeId == 1020)
                    {
                        return false;
                    }

                    if (sku.Itm_Model.Itm_Type.ProductId == 371)
                    {
                        var loc = await db.Comp_Locations.FindAsync(LocId);
                        var Duration = 6;
                        var MarkUp = 35;
                        var Advance = Math.Round(((PPrice * MarkUp / 100) + PPrice) / Duration);
                        var adv = Advance % 25;
                        adv = adv > 0 ? 25 - adv : 0;
                        Advance = Advance + adv;
                        var InstPrice = Advance * Duration;

                        var oldPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.Status && x.Duration == Duration
                        && (x.CityId == loc.CityId) && x.LocId == 0).OrderByDescending(x => x.RowId).FirstOrDefaultAsync();
                        if (oldPlan != null)
                        {
                            if (oldPlan.InstPrice >= InstPrice)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            oldPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.Status && x.Duration == Duration
                            && x.CityId == 0 && x.LocId == 0).OrderByDescending(x => x.RowId).FirstOrDefaultAsync();
                            if (oldPlan != null)
                            {
                                if (oldPlan.InstPrice >= InstPrice)
                                {
                                    return false;
                                }
                            }
                        }

                        var tbl = new Itm_SerialPlan()
                        {
                            Advance = Advance,
                            Duration = Duration,
                            EffectedDate = GetWorkingDate(LocId),
                            Inst = Advance,
                            InstPrice = InstPrice,
                            BasePrice = PPrice,
                            Status = true,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            Remarks = "Auto Plan for " + GRNNo,
                            ItemId = ItemId
                        };
                        db.Itm_SerialPlan.Add(tbl);
                        await db.SaveChangesAsync();
                        //return false;
                    }
                    //else
                    {
                        var plan = await db.Itm_TypePlan.Where(x => x.LocId == LocId && x.TypeId == sku.Itm_Model.TypeId && x.IsLocal && x.Status).OrderBy(x => x.MarkUp).FirstOrDefaultAsync();

                        var loc = await db.Comp_Locations.FindAsync(LocId);
                        var Advance = Math.Round(((PPrice * plan.MarkUp / 100) + PPrice) / plan.Duration);
                        var adv = Advance % 25;
                        adv = adv > 0 ? 25 - adv : 0;
                        Advance = Advance + adv;
                        var InstPrice = (Advance * plan.Duration);

                        var oldPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.Status && x.Duration == plan.Duration
                        && (x.CityId == loc.CityId) && x.LocId == 0).OrderByDescending(x => x.RowId).FirstOrDefaultAsync();
                        if (oldPlan != null)
                        {
                            if (oldPlan.InstPrice >= InstPrice)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            oldPlan = await db.Itm_SKUPlan.Where(x => x.SKUId == SKUId && x.Status && x.Duration == plan.Duration
                            && x.CityId == 0 && x.LocId == 0).OrderByDescending(x => x.RowId).FirstOrDefaultAsync();
                            if (oldPlan != null)
                            {
                                if (oldPlan.InstPrice >= InstPrice)
                                {
                                    return false;
                                }
                            }
                        }



                        var tbl = new Itm_SerialPlan()
                        {
                            Advance = Advance + plan.RegFee,
                            Duration = plan.Duration,
                            EffectedDate = GetWorkingDate(LocId),
                            Inst = Advance,
                            InstPrice = InstPrice + plan.RegFee,
                            BasePrice = PPrice,
                            Status = true,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            Remarks = "Auto Plan for " + GRNNo,
                            ItemId = ItemId
                        };
                        db.Itm_SerialPlan.Add(tbl);
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

        #endregion

        #region SerialPlan
        public async Task<List<SerialPlanVM>> SerialPlanRead(int ItemId)
        {
            try
            {
                var lst = await (from x in db.Itm_SerialPlan
                                 join I in db.Inv_Store on x.ItemId equals I.ItemId
                                 where x.ItemId == ItemId && x.Status
                                 select new SerialPlanVM
                                 {
                                     ItemId = x.ItemId,
                                     Advance = x.Advance,
                                     Duration = x.Duration,
                                     EffectedDate = x.EffectedDate,
                                     Inst = x.Inst,
                                     InstPrice = x.InstPrice,
                                     BasePrice = x.BasePrice,
                                     RowId = x.RowId,
                                     SerialNo = I.SerialNo,
                                     SKU = I.Itm_Master.SKUCode,
                                     Remarks = x.Remarks
                                 }).ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<IEnumerable<SerialPlanVM>> CreateSerialPlan(IEnumerable<SerialPlanVM> mod, int UserId)
        {
            try
            {
                foreach (var v in mod)
                {
                    var itm = await db.Inv_Store.Where(x => x.SerialNo == v.SerialNo).FirstOrDefaultAsync();
                    await db.Itm_SerialPlan.Where(x => x.ItemId == itm.ItemId && x.Status).ForEachAsync(x => { x.Status = false; });
                    var row = new Itm_SerialPlan()
                    {
                        ItemId = v.ItemId,
                        Advance = v.Advance,
                        Duration = v.Duration,
                        EffectedDate = v.EffectedDate,
                        Inst = v.Inst,
                        InstPrice = v.InstPrice,
                        BasePrice = v.BasePrice,
                        Remarks = v.Remarks,
                        Status = true,
                        TransDate = DateTime.Now,
                        UserId = UserId
                    };
                    db.Itm_SerialPlan.Add(row);
                    await db.SaveChangesAsync();
                    v.RowId = row.RowId;
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region SEND SMS / SEND EMAIl
        //public async Task<bool> SendSMS(string number, string msg)
        //{
        //    var url = "http://172.16.1.100/api/MSG/Send?number=" + number + "&msg=" + msg;
        //    var client = new HttpClient();
        //    var content = await client.GetStringAsync(url);
        //    return true;
        //}

        //public async Task<bool> SendEmail()
        //{

        //}
        #endregion

        #region Expense
        public async Task<List<ExpenseVM>> ExpenseList()
        {
            try
            {
                return await db.Lse_ExpenseHead.Where(x => x.Status).Select(x =>
                new ExpenseVM
                {
                    ExpHeadId = x.ExpHeadId,
                    ExpHead = x.ExpHead,
                    GLCode = x.GLCode,
                    MaxLimit = x.MaxLimit,
                    ExpTypeId = x.ExpTypeId,
                    ExpFor = x.ExpFor
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ExpenseVM> ExpenseListBySubCode()
        {
            try
            {
                return db.Lse_ExpenseHead.Where(x => x.Status).Select(x =>
               new ExpenseVM
               {
                   ExpHeadId = x.ExpHeadId,
                   ExpHead = x.ExpHead,
                   GLCode = x.GLCode,
                   MaxLimit = x.MaxLimit,
                   ExpTypeId = x.ExpTypeId,
                   ExpFor = x.ExpFor

               }).ToList().Select(x => new ExpenseVM()
               {
                   ExpHeadId = x.ExpHeadId,
                   ExpHead = x.ExpHead,
                   GLCode = x.GLCode,
                   MaxLimit = x.MaxLimit,
                   ExpTypeId = x.ExpTypeId,
                   ExpFor = x.ExpFor,
                   //SubCode = String.Format("{0:##-##-##-#####}", Convert.ToInt32(x.GLCode))
                   SubCode = Convert.ToInt64(x.GLCode).ToString("##-##-##-#####")

               }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ExpenseVM>> ExpenseListByLocation(int locid)
        {
            try
            {
                if (locid == 72)
                {
                    return await db.Lse_ExpenseHead.Where(x => x.Status).Select(x =>
                    new ExpenseVM
                    {
                        ExpHeadId = x.ExpHeadId,
                        ExpHead = x.ExpHead,
                        GLCode = x.GLCode,
                        MaxLimit = x.MaxLimit,
                        ExpTypeId = x.ExpTypeId,
                        ExpFor = x.ExpFor
                    }).ToListAsync();
                }
                else
                {
                    return await db.Lse_ExpenseHead.Where(x => x.ExpFor == "B" && x.Status).Select(x =>
                      new ExpenseVM
                      {
                          ExpHeadId = x.ExpHeadId,
                          ExpHead = x.ExpHead,
                          GLCode = x.GLCode,
                          MaxLimit = x.MaxLimit,
                          ExpTypeId = x.ExpTypeId,
                          ExpFor = x.ExpFor
                      }).ToListAsync();

                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ExpenseVM> CreateExpense(ExpenseVM mod, int UserId)
        {
            try
            {
                mod.ExpHead = mod.ExpHead.Trim();
                Lse_ExpenseHead tbl = new Lse_ExpenseHead
                {
                    ExpHead = mod.ExpHead,
                    GLCode = mod.GLCode,
                    MaxLimit = mod.MaxLimit,
                    Status = true,
                    TransDate = DateTime.Now,
                    UserId = UserId,
                    ExpTypeId = mod.ExpTypeId,
                    ExpFor = mod.ExpFor
                };
                db.Lse_ExpenseHead.Add(tbl);
                await db.SaveChangesAsync();
                mod.ExpHeadId = tbl.ExpHeadId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateExpense(ExpenseVM mod, int UserId)
        {
            try
            {
                mod.ExpHead = mod.ExpHead.Trim();
                var tbl = await db.Lse_ExpenseHead.SingleOrDefaultAsync(x => x.ExpHeadId.Equals(mod.ExpHeadId));
                if (tbl != null)
                {
                    tbl.ExpHead = mod.ExpHead;
                    tbl.GLCode = mod.GLCode;
                    tbl.MaxLimit = mod.MaxLimit;
                    tbl.UserId = UserId;
                    tbl.ExpTypeId = mod.ExpTypeId;
                    tbl.TransDate = DateTime.Now;
                    tbl.ExpTypeId = mod.ExpTypeId;
                    tbl.ExpFor = mod.ExpFor;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyExpense(ExpenseVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Lse_ExpenseHead.SingleOrDefaultAsync(x => x.ExpHeadId.Equals(mod.ExpHeadId));
                if (tbl != null)
                {
                    tbl.Status = false;
                    tbl.UserId = UserId;
                    tbl.TransDate = DateTime.Now;
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

        #region ExpenseType
        public async Task<List<ExpenseTypeVM>> ExpenseTypeList()
        {
            try
            {
                return await db.Lse_ExpType.Select(x =>
                new ExpenseTypeVM
                {
                    Id = x.ExpTypeId,
                    ExpType = x.ExpType
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ExpenseTypeVM> CreateExpenseType(ExpenseTypeVM mod)
        {
            try
            {
                Lse_ExpType tbl = new Lse_ExpType
                {
                    ExpType = mod.ExpType
                };
                db.Lse_ExpType.Add(tbl);
                await db.SaveChangesAsync();
                mod.Id = tbl.ExpTypeId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateExpenseType(ExpenseTypeVM mod)
        {
            try
            {
                var tbl = await db.Lse_ExpType.Where(x => x.ExpTypeId == mod.Id).FirstOrDefaultAsync();
                if (tbl != null)
                {
                    tbl.ExpType = mod.ExpType;
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

        public async Task<Comp_Tax> GetTax()
        {
            try
            {
                return await db.Comp_Tax.FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ItemDetailVM>> ItemDetailList(string Serial)
        {
            try
            {
                var lst = await (from ID in db.Inv_Store
                                 join S in db.Itm_Master on ID.SKUId equals S.SKUId
                                 join M in db.Itm_Model on S.ModelId equals M.ModelId
                                 join T in db.Itm_Type on M.TypeId equals T.TypeId
                                 join P in db.Itm_Products on T.ProductId equals P.ProductId
                                 join C in db.Itm_Company on T.ComId equals C.ComId
                                 join L in db.Comp_Locations on ID.LocId equals L.LocId
                                 join ST in db.Inv_Status on ID.StatusID equals ST.StatusID
                                 join SP in db.Inv_Suppliers on ID.SuppId equals SP.SuppId
                                 where ID.SerialNo.Contains(Serial)
                                 select new ItemDetailVM
                                 {
                                     Company = C.ComName,
                                     SKUName = S.SKUName,
                                     ItemId = ID.ItemId,
                                     Location = L.LocName,
                                     Model = M.Model,
                                     Product = P.ProductName,
                                     SerialNo = ID.SerialNo,
                                     Status = ST.StatusTitle,
                                     Supplier = SP.SuppName,
                                     Type = T.TypeName,
                                     Remarks = ""
                                 }).ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateItemDetail(ItemDetailVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Inv_Store.SingleOrDefaultAsync(x => x.ItemId.Equals(mod.ItemId));
                if (tbl != null)
                {
                    Inv_StoreLog obj = new Inv_StoreLog
                    {
                        LocId = tbl.LocId,
                        ItemId = mod.ItemId,
                        FromStatus = tbl.StatusID,
                        ToStatus = tbl.StatusID,
                        UserId = UserId,
                        TransDate = DateTime.Now,
                        Reason = tbl.SerialNo + " to " + mod.SerialNo + " (" + mod.Remarks + ")",
                        IsSerialChange = true
                    };
                    db.Inv_StoreLog.Add(obj);
                    tbl.SerialNo = mod.SerialNo;
                    await db.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetNext(string LastNo)
        {
            string NewNo = DateTime.Now.ToString("yyMM");
            if (LastNo.Substring(0, 4) == NewNo)
            {
                NewNo = NewNo + (Convert.ToInt32(LastNo.Substring(4, 4)) + 1).ToString("0000");
            }
            else
            {
                NewNo = NewNo + "0001";
            }
            return NewNo;
        }

        public async Task<int?> GetMobileAppVersion()
        {
            try
            {
                return await db.Comp_Profile.Select(x => x.MobileAppVersion).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region CompLocations
        public async Task<List<CompLocationIPVM>> CompLocations()
        {
            try
            {
                return await (from comp in db.Comp_Locations
                              join compip in db.Comp_LocationIP on comp.LocId equals compip.LocId
                              where compip.Type == "P" && comp.LocTypeId != 3 && compip.Status
                              select new CompLocationIPVM()
                              {
                                  IP = compip.IP,
                                  LocCode = comp.LocCode,
                                  RowId = compip.RowId,
                                  LocId = comp.LocId,
                                  LocName = comp.LocName,
                                  ToIP = compip.ToIP,
                                  Status = compip.Status
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<CompLocationIPVM> CreateCompLocation(CompLocationIPVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Comp_LocationIP.Where(x => x.LocId == mod.LocId && x.Type == "P" && !x.Status).FirstOrDefaultAsync();
                if (tbl == null)
                {
                    tbl = new Comp_LocationIP
                    {
                        IP = mod.IP,
                        LocId = mod.LocId,
                        Status = true,
                        ToIP = mod.IP,
                        Type = "P"
                    };
                    db.Comp_LocationIP.Add(tbl);
                }
                else
                {
                    tbl.IP = mod.IP;
                    tbl.ToIP = mod.IP;
                    tbl.Status = mod.Status;
                }
                await db.SaveChangesAsync();
                mod.RowId = tbl.RowId;
                mod.Status = tbl.Status;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCompLocation(CompLocationIPVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Comp_LocationIP.Where(x => x.RowId == mod.RowId).FirstOrDefaultAsync();
                if (tbl != null)
                {
                    tbl.IP = mod.IP;
                    tbl.ToIP = mod.IP;
                    tbl.Status = mod.Status;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public async Task<bool> DestroyCompLocation(CompLocationIPVM mod, int UserId)
        //{
        //    try
        //    {
        //        var tbl = await db.Comp_LocationIP.SingleOrDefaultAsync(x => x.RowId == mod.RowId);
        //        if (tbl != null)
        //        {
        //            tbl.Status = false;
        //        }
        //        await db.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        #endregion

        #region UsersFeedback
        public async Task<Users_FeedbackVM> UserFeedback(Users_FeedbackVM Com, int UserId)
        {
            try
            {
                Users_Feedback tbl = new Users_Feedback
                {

                    Title = Com.Title,
                    Description = Com.Description,
                    MobileNo = Com.MobileNo,
                    TransDate = DateTime.Now,
                    UserId = UserId,

                };
                db.Users_Feedback.Add(tbl);
                await db.SaveChangesAsync();
                Com.RowId = tbl.RowId;
                return Com;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region BackEndMobile Policy
        public async Task<List<BackendIncBasisVM>> GetBackEndIncBasis()
        {
            return await db.Fin_BackendIncBasis.Select(x => new BackendIncBasisVM()
            {
                IncBase = x.IncBase,
                IncBaseId = x.IncBaseId
            }).ToListAsync();
        }


        public async Task<List<BackendIncTypesVM>> GetBackEndIncType()
        {
            return await db.Fin_BackendIncTypes.Select(x => new BackendIncTypesVM()
            {
                IncType = x.IncType,
                IncTypeId = x.IncTypeId
            }).ToListAsync();
        }

        #endregion
        public DateTime GetWorkingDate(int LocId)
        {
            try
            {
                return db.Comp_Locations.Where(x => x.LocId == LocId).Select(x => (DateTime)x.WorkingDate).FirstOrDefault();
            }
            catch (Exception)
            {
                return DateTime.Now.Date;
            }
        }

        public async Task<bool> CreateStoreHistory(DateTime DocDate, long ItemId, int LocId, int MFact, decimal MRP, decimal PPrice,
            int SKUId, decimal SMPrice, decimal SPrice, DateTime TransDate, string Type, int UserId, long RefId)
        {
            try
            {
                Inv_StoreHistory tbl = new Inv_StoreHistory
                {
                    DocDate = DocDate,
                    ItemId = ItemId,
                    LocId = LocId,
                    MFact = MFact,
                    MRP = MRP,
                    PPrice = PPrice,
                    Qty = 1,
                    SKUId = SKUId,
                    SMPrice = SMPrice,
                    SPrice = SPrice,
                    TransDate = TransDate,
                    Type = Type,
                    UserId = UserId,
                    RefId = RefId
                };
                db.Inv_StoreHistory.Add(tbl);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<POTypeVM>> POTypeList()
        {
            try
            {
                return await db.Inv_POType.Select(x =>
                new POTypeVM
                {
                    POTypeId = x.POTypeId,
                    POType = x.POType
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #region Employee Hierarchy
        public async Task<List<GeoLocationVM>> GeoLocationListByParentId(int parentId)
        {
            try
            {
                return await db.Comp_GeoLocation.Where(x => x.ParentId == parentId).Select(x =>
                new GeoLocationVM
                {
                    GTitle = x.GTitle,
                    ParentId = x.ParentId,
                    GLevel = x.GLevel,
                    GeoId = x.GeoId
                }).Distinct().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region
        public async Task<List<Crm_CategoryVM>> CrmCategoryList()
        {
            try
            {
                return await db.Crm_Category.Where(x => x.Status == true).Select(x =>
                new Crm_CategoryVM
                {
                    CategoryId = x.CategoryId,
                    Category = x.Category,
                    Status = x.Status
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Crm_CategoryVM>> CrmCategoryMappedList(int UserId)
        {
            try
            {
                return await (from M in db.Crm_ComplainMapping
                              join C in db.Crm_Category on M.CategoryId equals C.CategoryId
                              where M.EmpId == UserId
                              select
                 new Crm_CategoryVM
                 {
                     CategoryId = C.CategoryId,
                     Category = C.Category,
                     Status = C.Status
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Crm_CategoryVM>> CrmMCategoryList()
        {
            try
            {
                return await db.Crm_MCategory.Select(x =>
                new Crm_CategoryVM
                {
                    CategoryId = x.MCategoryId,
                    Category = x.MCategory
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

       

    }
}