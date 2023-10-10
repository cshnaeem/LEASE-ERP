using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace AGEERP.Models
{
    public class OrderBL
    {
        AGEEntities db = new AGEEntities();
        SetupBL setupBL = new SetupBL();
        NotificationBL notificationBL = new NotificationBL();



        #region POPlanCity
        public async Task<List<POPlanCityDetailVM>> GetPOPlanCityList(int CompanyId, int ProductId, List<int> ModelLst)
        {
            try
            {
                if (ModelLst[0] == 0)
                {
                    return await db.Itm_Master.Where(x => x.Itm_Model.Itm_Type.ComId == CompanyId
                    && (x.Itm_Model.Itm_Type.ProductId == ProductId || ProductId == 0) && x.AvailableForPurchase)
                        .Select(x => new POPlanCityDetailVM
                        {
                            SkuId = x.SKUId,
                            SKU = x.SKUCode
                        }).ToListAsync();
                }
                return await db.Itm_Master.Where(x => ModelLst.Contains(x.ModelId) && x.AvailableForPurchase)
                    .Select(x => new POPlanCityDetailVM
                    {
                        SkuId = x.SKUId,
                        SKU = x.SKUCode
                    }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
        #region POPlanCity
        public async Task<long> CreateOrderPlanCity(IEnumerable<POPlanCityDetailVM> mod, int ComId, string Remarks, int UserId)
        {
            try
            {
                var cityLst = db.Comp_City.ToList();
                List<Inv_POPlanCityDetail> det = new List<Inv_POPlanCityDetail>();
                foreach (var x in mod)
                {
                    for (int i = 0; i < cityLst.Count; i++)
                    {
                        var city = GetVal(x, "City" + (i + 1));
                        if (city > 0)
                        {
                            det.Add(new Inv_POPlanCityDetail
                            {
                                Qty = city,
                                SKUId = x.SkuId,
                                CityId = cityLst[i].CityId
                            });
                        }
                    }
                }
                var comp = await db.Itm_Company.FindAsync(ComId);
                var lastPO = await db.Inv_POPlanCity.Where(x => x.ComId == ComId).OrderByDescending(x => x.PlanId).Select(x => x.PlanNo).FirstOrDefaultAsync();
                string planNo = comp.ComCode + DateTime.Now.ToString("yyMMdd");
                if (lastPO == null)
                {
                    planNo = planNo + "01";
                }
                else if (lastPO.Substring(0, planNo.Length) == planNo)
                {
                    planNo = planNo + (Convert.ToInt32(lastPO.Substring(planNo.Length, 2)) + 1).ToString("00");
                }
                else
                {
                    planNo = planNo + "01";
                }


                Inv_POPlanCity ord = new Inv_POPlanCity()
                {
                    ComId = ComId,
                    TransDate = DateTime.Now,
                    UserId = UserId,
                    Remarks = Remarks,
                    Status = "P",
                    PlanNo = planNo,
                    Inv_POPlanCityDetail = det
                };

                db.Inv_POPlanCity.Add(ord);
                await db.SaveChangesAsync();
                return ord.PlanId;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion
        #region POPlan

        public List<POPlanDetailVM> GetModelSaleStock(int[] ModelLst, int[] CityLst, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                List<POPlanDetailVM> lst = new List<POPlanDetailVM>();
                foreach (var c in CityLst)
                {
                    foreach (var m in ModelLst)
                    {
                        lst.AddRange(db.spget_ModelSaleStock(m, c, FromDate, ToDate).Select(x => new POPlanDetailVM
                        {
                            SKU = x.SKUName,
                            SkuId = x.SKUId,
                            Qty = 0,
                            SaleQty = x.Sale,
                            StockQty = x.Stock,
                            PendingQty = x.Pending,
                            CityId = c
                        }).ToList());
                    }
                }
                return lst.GroupBy(x => new { x.CityId, x.SkuId }).Select(x => new POPlanDetailVM
                {
                    SKU = x.Max(a => a.SKU),
                    SkuId = x.Key.SkuId,
                    CityId = x.Key.CityId,
                    City = db.Comp_City.Find(x.Key.CityId).City,
                    Qty = 0,
                    SaleQty = x.Sum(a => a.SaleQty),
                    StockQty = x.Sum(a => a.StockQty),
                    PendingQty = x.Sum(a => a.PendingQty)
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }



        public async Task<long> CreateOrderPlan(IEnumerable<POPlanDetailVM> mod, int[] CityLst, int SuppId, string Remarks, DateTime FromDate, DateTime ToDate, int UserId)
        {
            try
            {
                List<Inv_POPlanDetail> det = new List<Inv_POPlanDetail>();
                foreach (var x in mod)
                {
                    det.Add(new Inv_POPlanDetail
                    {
                        Pending = x.PendingQty,
                        Qty = x.Qty,
                        Sale = x.SaleQty,
                        SKUId = x.SkuId,
                        Stock = x.StockQty,
                        LocId = x.CityId
                    });
                }

                Inv_POPlan ord = new Inv_POPlan()
                {
                    SuppId = SuppId,
                    TransDate = DateTime.Now,
                    UserId = UserId,
                    Remarks = Remarks,
                    Status = "P",
                    Inv_POPlanDetail = det,
                    FromDate = FromDate,
                    POTypeId = 1,
                    ToDate = ToDate
                };

                db.Inv_POPlan.Add(ord);
                await db.SaveChangesAsync();
                foreach (var c in CityLst)
                {
                    db.Inv_POPlanMapping.Add(new Inv_POPlanMapping
                    {
                        CityId = c,
                        POPlanId = ord.PlanId
                    });
                }
                await db.SaveChangesAsync();
                return ord.PlanId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion
        #region POPlanMobile
        public async Task<List<PreOrderMobileVM>> GetModelSaleStockMobile(int[] ModelLst, DateTime FromDate, DateTime ToDate, string LocType)
        {
            try
            {
                List<PreOrderMobileVM> lst = new List<PreOrderMobileVM>();
                var lsst = await GetModelList(ModelLst);
                for (int i = 0; i < lsst.Count; i++)
                {
                    var ls = db.spget_ModelSaleStockMobile(lsst[i].ModelId, FromDate, ToDate, LocType).ToList();
                    if (i == 0)
                    {
                        foreach (var item in ls)
                        {
                            lst.Add(new PreOrderMobileVM
                            {
                                City = item.City,
                                LocId = item.LocId,
                                LocName = item.LocName,
                                Branches = item.Branches ?? 0,
                                Model1 = item.SKUName,
                                ModelId1 = item.SKUId,
                                Pending1 = item.Pending,
                                Sale1 = item.Sale,
                                Stock1 = item.Stock,
                                Order1 = 0
                            });
                        }
                    }
                    else
                    {
                        foreach (var item in ls)
                        {
                            var tbl = lst.Where(x => x.LocId == item.LocId).FirstOrDefault();
                            SetVal(tbl, item.SKUName, "Model" + (i + 1));
                            SetVal(tbl, item.SKUId, "ModelId" + (i + 1));
                            SetVal(tbl, item.Pending, "Pending" + (i + 1));
                            SetVal(tbl, item.Sale, "Sale" + (i + 1));
                            SetVal(tbl, item.Stock, "Stock" + (i + 1));
                            SetVal(tbl, 0, "Order" + (i + 1));
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
        public async Task<long> SavePreOrderMobile(IEnumerable<PreOrderMobileVM> mod, int SuppId, string Remarks, DateTime FromDate, DateTime ToDate, int UserId)
        {
            try
            {
                List<Inv_POPlanDetail> lst = new List<Inv_POPlanDetail>();
                for (int i = 0; i < 10; i++)
                {
                    foreach (var v in mod)
                    {
                        var skuId = GetVal(v, "ModelId" + (i + 1));
                        if (skuId > 0)
                        {
                            var qty = GetVal(v, "Order" + (i + 1));
                            if (qty > 0)
                            {
                                lst.Add(new Inv_POPlanDetail
                                {
                                    LocId = v.LocId,
                                    Pending = GetVal(v, "Pending" + (i + 1)),
                                    Qty = GetVal(v, "Order" + (i + 1)),
                                    Sale = GetVal(v, "Sale" + (i + 1)),
                                    SKUId = GetVal(v, "ModelId" + (i + 1)),
                                    Stock = GetVal(v, "Stock" + (i + 1))
                                });
                            }
                        }
                        else
                            break;
                    }
                }
                if (lst.Count > 0)
                {
                    Inv_POPlan ord = new Inv_POPlan()
                    {
                        SuppId = SuppId,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        Remarks = Remarks,
                        Status = "P",
                        Inv_POPlanDetail = lst,
                        FromDate = FromDate,
                        POTypeId = 2,
                        ToDate = ToDate
                    };
                    db.Inv_POPlan.Add(ord);
                    await db.SaveChangesAsync();
                    return ord.PlanId;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<List<ModelVM>> GetModelList(int[] ModelLst)
        {
            try
            {
                return await db.Itm_Master.Where(x => ModelLst.Contains(x.ModelId) && x.AvailableForPurchase).Select(x => new ModelVM
                {
                    Model = x.SKUName,
                    ModelId = x.SKUId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<POPlanVM>> GetPlanCityList()
        {
            try
            {
                //var comLst = await db.Inv_SuppliersMapping.Where(x => x.SupplierId == SuppId && x.Status).Select(x => x.ComId).ToListAsync();
                var lst = await db.Inv_POPlanCity.Where(x => x.Status == "P").Select(x => new POPlanVM { PlanId = x.PlanId, PlanNo = x.PlanNo }).OrderByDescending(x => x.PlanId).ToListAsync();
                lst.Insert(0, new POPlanVM { PlanId = 0, PlanNo = "No Plan" });
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<long>> GetPlanList(int SuppId)
        {
            try
            {
                //var comLst = await db.Inv_SuppliersMapping.Where(x => x.SupplierId == SuppId && x.Status).Select(x => x.ComId).ToListAsync();
                return await db.Inv_POPlan.Where(x => x.SuppId == SuppId && x.Status == "P").Select(x => x.PlanId).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SupplierVM>> SupplierByPlanList(int PlanId)
        {
            try
            {
                if (PlanId > 0)
                {
                    var plan = await db.Inv_POPlanCity.FindAsync(PlanId);
                    var suppLst = await db.Inv_SuppliersMapping.Where(x => x.ComId == plan.ComId && x.Status).Select(x => x.SupplierId).ToListAsync();
                    return await db.Inv_Suppliers.Where(x => suppLst.Contains(x.SuppId) && x.Status).Select(x => new SupplierVM { SuppId = x.SuppId, SuppName = x.SuppName }).ToListAsync();
                }
                return await db.Inv_Suppliers.Where(x => x.Status).Select(x => new SupplierVM { SuppId = x.SuppId, SuppName = x.SuppName }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
        #region PO

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
        public async Task<List<Inv_PO>> POList(int POTypeId)
        {
            try
            {
                return await db.Inv_PO.Where(x => x.POTypeId == POTypeId && x.Status >= 3 && x.Status <= 5).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        //public async Task<Inv_PO> GetPO(long POId)
        //{
        //    try
        //    {
        //        return await db.Inv_PO.FindAsync(POId);
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public async Task<decimal> ApplyDiscountPolicy(IEnumerable<PODetailVM> mod)
        {
            try
            {
                var cDate = DateTime.Now.Date; //setupBL.GetWorkingDate(UserInfo.LocId);
                decimal Amt = 0;
                List<POGiftVM> lst = new List<POGiftVM>();
                var ls = (from M in mod
                          join DD in db.Itm_DiscountPolicyModel on M.ModelId equals DD.ModelId
                          join D in db.Itm_DiscountPolicy on DD.PolicyId equals D.PolicyId
                          where D.StartDate <= cDate && D.EndDate >= cDate && D.Status
                          select D).Distinct().ToList();
                foreach (var item in ls)
                {
                    var modl = item.Itm_DiscountPolicyModel.Select(x => x.ModelId).ToList();
                    var totqty = mod.Where(x => modl.Contains(x.ModelId)).Sum(x => x.Qty);
                    foreach (var v in item.Itm_DiscountPolicySlab)
                    {
                        if (totqty >= v.MinSlabQty && totqty <= v.MaxSlabQty)
                        {
                            Amt = Amt + v.IncentiveAmt;
                            Amt = Amt + (v.ExtraQtyRate * (totqty - v.MinSlabQty));
                        }
                    }
                }
                //foreach (var v in mod)
                //{
                //    var sku = await db.Itm_Master.FindAsync(v.SKUId);
                //    var foc = await db.Itm_DiscountPolicy.Where(x => x.ModelId == sku.ModelId && x.EffectedDate < DateTime.Now && x.Status).FirstOrDefaultAsync();
                //    if (foc != null)
                //    {
                //        var qty = (int)Math.Floor(((decimal)v.Qty / foc.RatioPurQty) * foc.RatioGiftQty);
                //        if (qty > 0)
                //        {
                //            lst.Add(new POGiftVM
                //            {
                //                Qty = qty,
                //                SKUId = v.SKUId,
                //                SKU = sku.SKUName,
                //                Model = sku.Itm_Model.Model,
                //                ExtraCharges = foc.ExtraCharges,
                //                Amount = qty * foc.ExtraCharges
                //            });
                //        }
                //    }
                //}
                return Amt;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<List<POGiftVM>> ApplyPairPolicy(IEnumerable<PODetailVM> mod)
        {
            try
            {
                DateTime cDate = DateTime.Now.Date; //setupBL.GetWorkingDate(UserInfo.LocId);
                List<POGiftVM> lst = new List<POGiftVM>();
                //var flst = mod.GroupBy(x => x.SKUId).ToList();
                foreach (var v in mod)
                {
                    var sku = await db.Itm_Master.FindAsync(v.SKUId);
                    var foc = await db.Itm_PairPolicy.Where(x => x.ModelId == sku.ModelId && x.FromDate <= cDate && x.ToDate >= cDate && x.Status).OrderByDescending(x => x.PolicyId).FirstOrDefaultAsync();
                    if (foc != null)
                    {
                        if (foc.ModelId != foc.FOCModelId)
                        {
                            sku = await db.Itm_Master.Where(x => x.ModelId == foc.FOCModelId).FirstOrDefaultAsync();
                        }

                        var qty = (int)Math.Floor(((decimal)v.Qty / foc.RatioPurQty) * foc.RatioGiftQty);
                        decimal amt = 0;//(v.TP * (decimal)foc.RatioPurQty)/((decimal)foc.RatioPurQty + (decimal)foc.RatioGiftQty);
                        if (qty > 0)
                        {
                            lst.Add(new POGiftVM
                            {
                                Qty = qty,
                                SKUId = v.SKUId,
                                SKU = sku.SKUName,
                                Model = sku.Itm_Model.Model,
                                ExtraCharges = foc.ExtraCharges,
                                Amount = amt,
                                PolicyId = foc.PolicyId,
                                CityId = v.CityId,
                                City = v.City
                            });
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
        public async Task<decimal> GetWHT(int SuppId, decimal MRP, decimal TP, decimal GST, decimal Discount)
        {
            try
            {
                decimal WHT = 0;
                var supp = await db.Inv_Suppliers.FindAsync(SuppId);
                switch (supp.TaxAppliedOn)
                {
                    case "T":
                        {
                            WHT = Math.Round((TP - GST) * (supp.WHT ?? 0) / 100);
                            break;
                        }
                    case "M":
                        {
                            WHT = Math.Round((MRP) * (supp.WHT ?? 0) / 100);
                            break;
                        }
                    case "T-D":
                        {
                            WHT = Math.Round((TP - GST - Discount) * (supp.WHT ?? 0) / 100);
                            break;
                        }
                    case "M-D":
                        {
                            WHT = Math.Round((MRP - Discount) * (supp.WHT ?? 0) / 100);
                            break;
                        }
                    case "R":
                        {
                            WHT = Math.Round((MRP - GST) * (supp.WHT ?? 0) / 100);
                            break;
                        }
                    case "R-D":
                        {
                            WHT = Math.Round((MRP - GST - Discount) * (supp.WHT ?? 0) / 100);
                            break;
                        }
                    case "D-D":
                        {
                            WHT = Math.Round((TP - Discount) * (supp.WHT ?? 0) / 100);
                            break;
                        }
                    case "D":
                        {
                            WHT = Math.Round((TP) * (supp.WHT ?? 0) / 100);
                            break;
                        }
                }
                return WHT;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<PODetailVM> AddToOrder(int SKUId, int SuppId, int Qty, int CityId, int TypeId)
        {
            try
            {
                //db.Database.CommandTimeout = 7200;
                var cDate = DateTime.Now.Date; //setupBL.GetWorkingDate(UserInfo.LocId);
                PODetailVM mod = new PODetailVM();
                var sku = await db.Itm_Master.Where(x => x.SKUId == SKUId).Select(x => new { x.IsPair, x.SKUName, x.Itm_Model.Model, x.ModelId }).FirstOrDefaultAsync();
                var gst = await db.Inv_Suppliers.Where(x => x.SuppId == SuppId).Select(x => x.GST).FirstOrDefaultAsync();
                //var tax = await db.Comp_Tax.FirstOrDefaultAsync();
                var pPrice = await db.Itm_PPrice.Where(x => x.SKUId == SKUId &&
                x.FromDate <= cDate && x.ToDate >= cDate && x.Status && x.SuppId == SuppId).FirstOrDefaultAsync();

                mod.SKUId = SKUId;
                mod.IsPair = sku.IsPair;
                mod.SKU = sku.SKUName;
                mod.Model = sku.Model;
                mod.Qty = Qty;
                mod.ModelId = sku.ModelId;
                mod.CityId = CityId;
                if (CityId > 0)
                {
                    if (TypeId == 1)
                    {
                        var city = await db.Comp_City.Where(x => x.CityId == CityId).Select(x => x.City).FirstOrDefaultAsync();
                        mod.City = city;
                    }
                    else
                    {
                        var city = await db.Comp_Locations.Where(x => x.LocId == CityId).Select(x => x.LocName).FirstOrDefaultAsync();
                        mod.City = city;
                    }

                }
                if (pPrice != null)
                {

                    mod.Discount = Math.Round(pPrice.Discount);
                    mod.MRP = Math.Round(pPrice.MRP);
                    mod.GST = Math.Round(pPrice.MRP - (pPrice.MRP * 100 / ((gst ?? 0) + 100))); //Math.Round(pPrice.MRP * (supp.GST ?? 0)/100);
                    mod.WHT = await GetWHT(SuppId, pPrice.MRP, pPrice.TP, mod.GST, pPrice.Discount);
                    //if (supp.TaxAppliedOn == "T")
                    //{
                    //    mod.WHT = Math.Round((pPrice.TP) * (supp.WHT ?? 0) / 100);
                    //}
                    //else if (supp.TaxAppliedOn == "M")
                    //{
                    //    mod.WHT = Math.Round((pPrice.MRP) * (supp.WHT ?? 0) / 100);
                    //}
                    //else if (supp.TaxAppliedOn == "T-D")
                    //{
                    //    mod.WHT = Math.Round((pPrice.TP - pPrice.Discount) * (supp.WHT ?? 0) / 100);
                    //}
                    //else if (supp.TaxAppliedOn == "M-D")
                    //{
                    //    mod.WHT = Math.Round((pPrice.MRP - pPrice.Discount) * (supp.WHT ?? 0) / 100);
                    //}
                    //else if (supp.TaxAppliedOn == "R")
                    //{
                    //    mod.WHT = Math.Round((pPrice.MRP - mod.GST) * (supp.WHT ?? 0) / 100);
                    //}
                    //else if (supp.TaxAppliedOn == "R-D")
                    //{
                    //    mod.WHT = Math.Round((pPrice.MRP - mod.GST - pPrice.Discount) * (supp.WHT ?? 0) / 100);
                    //}

                    mod.TP = Math.Round(pPrice.TP);
                    mod.NetPrice = Math.Round(mod.WHT + mod.TP - mod.Discount);
                    mod.Amount = Math.Round(mod.NetPrice * mod.Qty);
                    mod.AnnuallyIncentive = pPrice.AnnuallyIncentive;
                    mod.BiannuallyIncentive = pPrice.BiannuallyIncentive;
                    mod.MonthlyIncentive = pPrice.MonthlyIncentive;
                    mod.QuarterlyIncentive = pPrice.QuarterlyIncentive;
                }

                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<PODetailVM>> GetPOPlan(int PlanId, int SuppId)
        {
            try
            {
                var lst = new List<PODetailVM>();
                var cityLst = await db.Inv_SuppliersCity.Where(x => x.SuppId == SuppId).Select(x => x.CityId).ToListAsync();
                var POLst = await db.Inv_POPlanCityDetail.Where(x => x.PlanId == PlanId && cityLst.Contains(x.CityId)).ToListAsync();
                //var suppId = POLst[0].Inv_POPlanCity.SuppId;
                var pLst = POLst.GroupBy(x => new { x.CityId, x.SKUId }).Select(x => new PODetailVM
                {
                    SKUId = x.Key.SKUId,
                    CityId = x.Key.CityId,
                    Qty = x.Sum(a => a.Qty)

                }).ToList();
                foreach (var v in pLst)
                {
                    lst.Add(await AddToOrder(v.SKUId, SuppId, v.Qty, v.CityId, 1));
                }
                lst.ForEach(x => { x.NetPrice = x.TP - x.Discount; x.Amount = (x.TP - x.Discount) * x.Qty; });
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Inv_PO> GetPOById(long POId)
        {
            try
            {
                return await db.Inv_PO.Where(x => x.POId == POId).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<PODetailVM>> GetPODetailById(long POId)
        {
            try
            {
                return await db.Inv_PODetail.Where(x => x.POId == POId).Select(x =>
                new PODetailVM
                {
                    //AnnuallyIncentive = x.AnnuallyIncentive,
                    //BiannuallyIncentive = x.BiannuallyIncentive,
                    CityId = x.CityId,
                    //Discount = x.Discount,
                    //GST = x.GST,
                    //ModelId = x.Itm_Master.ModelId,
                    //MonthlyIncentive = x.MonthlyIncentive,
                    MRP = x.MRP,
                    PODtlId = x.PODtlId,
                    //IsPair = x.Itm_Master.IsPair,
                    Model = x.Itm_Master.Itm_Model.Model,
                    Qty = x.Qty,
                    //QuarterlyIncentive = x.QuarterlyIncentive,
                    //PP = x.PP,
                    SKU = x.Itm_Master.SKUCode,
                    SKUId = x.SKUId,
                    Amount = x.Inv_POSchedule.Sum(a => (decimal?)a.ReceivedQty) ?? 0
                    //TP = x.TP,
                    //WHT = x.WHT,
                    //NetPrice = x.TP - x.Discount,
                    //Amount = (x.TP - x.Discount) * x.Qty
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdatePO(PODetailVM item)
        {
            try
            {
                var podet = await db.Inv_PODetail.Where(x => x.PODtlId == item.PODtlId).FirstOrDefaultAsync();
                var recv = await db.Inv_POSchedule.Where(x => x.PODtlId == item.PODtlId).SumAsync(x => (int?)x.ReceivedQty) ?? 0;
                podet.CityId = item.CityId;
                if (item.Qty >= recv)
                {
                    podet.Qty = item.Qty;
                }
                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        //public async Task<List<PODetailVM>> GetPOPlan(int PlanId)
        //{
        //    try
        //    {
        //        var lst = new List<PODetailVM>();
        //        var POLst = await db.Inv_POPlanDetail.Where(x => x.PlanId == PlanId).ToListAsync();
        //        var suppId = POLst[0].Inv_POPlan.SuppId;
        //        var pLst = POLst.GroupBy(x => new { x.LocId, x.SKUId }).Select(x => new PODetailVM
        //        {
        //            SKUId = x.Key.SKUId,
        //            CityId = x.Key.LocId,
        //            Qty = x.Sum(a => a.Qty)

        //        }).ToList();
        //        foreach (var v in pLst)
        //        {
        //            lst.Add(await AddToOrder(v.SKUId, suppId, v.Qty, v.CityId, 1));
        //        }
        //        lst.ForEach(x => { x.NetPrice = x.TP - x.Discount; x.Amount = (x.TP - x.Discount) * x.Qty; });
        //        return lst;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        public async Task<Inv_PO> GetPOPlanMobileById(int PlanId)
        {
            try
            {

                return await db.Inv_PO.Where(x => x.POId == PlanId && x.Status == 1).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }



        public async Task<List<PODetailVM>> GetPOPlanMobile(int PlanId)
        {
            try
            {
                var lst = new List<PODetailVM>();
                var POLst = await db.Inv_POPlanDetail.Where(x => x.PlanId == PlanId).ToListAsync();
                var suppId = POLst[0].Inv_POPlan.SuppId;
                var pLst = POLst.GroupBy(x => new { x.LocId, x.SKUId }).Select(x => new PODetailVM
                {
                    SKUId = x.Key.SKUId,
                    CityId = x.Key.LocId,
                    Qty = x.Sum(a => a.Qty)

                }).ToList();
                foreach (var v in pLst)
                {
                    var ord = await AddToOrder(v.SKUId, suppId, v.Qty, v.CityId, 2);

                    lst.Add(ord);
                }
                lst.ForEach(x => { x.NetPrice = x.TP - x.Discount; x.Amount = (x.TP - x.Discount) * x.Qty; });
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<PODetailVM>> GetPOEdit(int PoId)
        {
            try
            {
                var lst = new List<PODetailVM>();
                var po = await db.Inv_PO.Where(x => x.POId == PoId).FirstOrDefaultAsync();
                var POLst = await db.Inv_PODetail.Where(x => x.POId == PoId).ToListAsync();
                var suppId = po.SuppId;
                var pLst = POLst.GroupBy(x => new { x.CityId, x.SKUId, x.PODtlId }).Select(x => new PODetailVM
                {
                    SKUId = x.Key.SKUId,
                    CityId = x.Key.CityId,
                    PODtlId = x.Key.PODtlId,
                    Qty = x.Sum(a => a.Qty)

                }).ToList();
                foreach (var v in pLst)
                {
                    var ord = await AddToOrder(v.SKUId, suppId, v.Qty, v.CityId, 2);
                    ord.PODtlId = v.PODtlId;
                    lst.Add(ord);
                }
                lst.ForEach(x => { x.NetPrice = x.TP - x.Discount; x.Amount = (x.TP - x.Discount) * x.Qty; });
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
        #region POApproval
        public async Task<List<OrderSearchVM>> GetPOApproval(string Level, int UserId)
        {
            try
            {
                List<OrderSearchVM> lst = new List<OrderSearchVM>();
                var usr = await db.Users_Login.FindAsync(UserId);
                //var approval = usr.Users_Group.Users_GroupAccess.Where(x => x.MenuId == 12100000).Any();
                var approval = usr.Users_Group.Users_GroupAccess.Where(x => x.MenuId == 12070000).Any();

                //var validate = usr.Users_Group.Users_GroupAccess.Where(x => x.MenuId == 12090000).Any();
                var validate = usr.Users_Group.Users_GroupAccess.Where(x => x.MenuId == 12060000).Any();

                if (Level == "V" && validate)
                {
                    lst = await db.Inv_PO.Where(x => x.Status == 1 && x.CheckedBy == null).Select(x =>
                       new OrderSearchVM
                       {
                           PODate = x.PODate,
                           POId = x.POId,
                           PONo = x.PONo,
                           DeliveryDate = x.DeliveryDate,
                           SuppName = x.Inv_Suppliers.SuppName,
                           Status = x.RevokedBy == null ? "" : "R",
                           FullName = x.Users_Login.FullName,
                           ValidateBy = ""
                       }).ToListAsync();

                }
                else if (Level == "A" && approval)
                {
                    lst = await (from x in db.Inv_PO
                                 join v in db.Users_Login on x.CheckedBy equals v.UserID
                                 where x.Status == 2 && x.CheckedBy != null && x.ApprovedBy == null
                                 select
                        new OrderSearchVM
                        {
                            PODate = x.PODate,
                            POId = x.POId,
                            PONo = x.PONo,
                            DeliveryDate = x.DeliveryDate,
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

        public async Task<bool> POApproval(OrderSearchVM mod, string Level, int UserId)
        {
            try
            {
                var vr = await db.Inv_PO.FindAsync(mod.POId);
                if (Level == "V" && mod.Status == "V")
                {
                    if (vr.CheckedBy == null)
                    {
                        vr.CheckedBy = UserId;
                        vr.CheckedDate = DateTime.Now;
                        vr.Status = 2;
                    }
                }
                else if (Level == "A" && mod.Status == "A")
                {
                    if (vr.ApprovedBy == null && vr.CheckedBy != null)
                    {
                        vr.ApprovedBy = UserId;
                        vr.ApprovedDate = DateTime.Now;
                        vr.Status = 3;
                    }
                }
                else if (mod.Status == "R")
                {
                    if (vr.ApprovedBy == null)
                    {
                        vr.RevokedBy = UserId;
                        vr.RevokedDate = DateTime.Now;
                        vr.Status = 8;
                    }
                    //if (vr.CheckedBy != null)
                    //{
                    //    vr.CheckedBy = null;
                    //    vr.CheckedDate = null;
                    //}
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
        #region PRRequest
        public async Task<List<PRVM>> GetPurchaseRequest()
        {
            return await (from item in db.Inv_PR
                          join loc in db.Comp_Locations on item.LocId equals loc.LocId
                          where item.Status == true
                          select new PRVM()
                          {
                              PRDate = item.PRDate,
                              PrNo = item.PRNo,
                              TransDate = item.TransDate,
                              PrId = item.PRId,
                              location = loc.LocName
                          }).ToListAsync();
        }

        public async Task<PRVM> GetPurchaseRequestById(long id)
        {

            var _MainReq = await (from item in db.Inv_PR
                                  join loc in db.Comp_Locations on item.LocId equals loc.LocId
                                  where item.Status == true && item.PRId == id
                                  select new PRVM()
                                  {
                                      PRDate = item.PRDate,
                                      PrNo = item.PRNo,
                                      TransDate = item.TransDate,
                                      PrId = item.PRId,
                                      LocId = item.LocId,
                                      LCSuppName = item.LCSuppName,
                                      LCSuppMob = item.LCSuppMob,
                                      LCSuppAddress = item.LCSuppAddress,
                                      location = loc.LocName
                                  }).FirstOrDefaultAsync();

            //var _ChildReq = await (from itemdetails in db.Inv_PRDetail
            //                       join sku in db.Itm_Master on itemdetails.SKUId equals sku.SKUId
            //                       where itemdetails.PRId == id
            //                       select new PRVM_Detail()
            //                       {
            //                           PRId = itemdetails.PRId,
            //                           SKU = sku.SKUName,
            //                           Qty = itemdetails.Qty,
            //                           Remarks = itemdetails.Remarks
            //                       }).ToListAsync();

            //_MainReq.PRDetail = _ChildReq;

            return _MainReq;

        }



        public async Task<bool> CreatePurchaseRequest(PRVM model, long Userid)
        {
            var lastPR = await db.Inv_PR.Where(x => x.LocId == model.LocId).OrderByDescending(x => x.PRId).Select(x => x.PRNo).FirstOrDefaultAsync();
            string planNo = model.LocId.ToString("000") + DateTime.Now.ToString("yyMM");
            //string str = lastPR.Substring(0, 5);

            if (lastPR == null)
            {
                planNo = planNo + "001";
            }
            else if (lastPR.Substring(0, 7) == planNo)
            {
                planNo = (Convert.ToInt32(lastPR.Substring(7, 3)) + 1).ToString("000");
            }
            else
            {
                planNo = planNo + "001";
            }

            if (model != null)
            {
                Inv_PR request = new Inv_PR()
                {
                    LocId = model.LocId,
                    PRDate = model.PRDate,
                    PRNo = planNo,
                    Status = true,
                    TransDate = DateTime.Now,
                    UserId = Userid
                };
                db.Inv_PR.Add(request);
                await db.SaveChangesAsync();

                foreach (var item in model.PRDetail)
                {
                    Inv_PRDetail DetailCh = new Inv_PRDetail();
                    DetailCh.PRId = request.PRId;
                    DetailCh.SKUId = item.SKUId;
                    DetailCh.Qty = item.Qty;
                    DetailCh.Remarks = item.Remarks;
                    db.Inv_PRDetail.Add(DetailCh);
                    await db.SaveChangesAsync();
                }
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion
        #region PRRequestBranch
        public async Task<List<PRVM>> GetPurchaseRequestBranch(int LocId)
        {
            return await (from item in db.Inv_PR
                          join loc in db.Comp_Locations on item.LocId equals loc.LocId
                          where item.PRStatus == "P" && (item.LocId == LocId || LocId == 72)
                          select new PRVM()
                          {
                              PRDate = item.PRDate,
                              PrNo = item.PRNo,
                              TransDate = item.TransDate,
                              PrId = item.PRId,
                              location = loc.LocName,
                              PrStatus = "Pending",
                              LCSuppName = item.LCSuppName,
                              LCSuppMob = item.LCSuppMob,
                              LCSuppAddress = item.LCSuppAddress
                          }).ToListAsync();
        }

        public async Task<bool> CancelPR(int id, int UserId)
        {
            try
            {
                if (id > 0)
                {
                    var pr = await db.Inv_PR.Where(x => x.PRId == id).FirstOrDefaultAsync();
                    if (pr != null && pr.PRStatus != "C")
                    {
                        pr.PRStatus = "C";
                        pr.ModifiedBy = UserId;
                        pr.ModifiedDate = DateTime.Now;
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
        public async Task<List<PRVM_Detail>> GetPurchaseRequestBranchtbl(int id)
        {
            return await (from itemdetails in db.Inv_PRDetail
                          join sku in db.Itm_Master on itemdetails.SKUId equals sku.SKUId into yG
                          from y1 in yG.DefaultIfEmpty()
                          where itemdetails.PRId == id
                          select new PRVM_Detail()
                          {
                              PRId = itemdetails.PRId,
                              SKU = y1.SKUName,
                              SKUId = itemdetails.SKUId,
                              Price = itemdetails.Price,
                              Qty = itemdetails.Qty,
                              Remarks = itemdetails.Remarks
                          }).ToListAsync();
        }

        public async Task<PRVM> GetPurchaseRequest(long id)
        {
            var _MainReq = await (from item in db.Inv_PR
                                  join loc in db.Comp_Locations on item.LocId equals loc.LocId
                                  where item.Status == true && item.PRId == id
                                  select new PRVM()
                                  {
                                      PRDate = item.PRDate,
                                      PrNo = item.PRNo,
                                      TransDate = item.TransDate,
                                      PrId = item.PRId,
                                      LocId = item.LocId,
                                      LCSuppName = item.LCSuppName,
                                      LCSuppMob = item.LCSuppMob,
                                      LCSuppAddress = item.LCSuppAddress,
                                      location = loc.LocName
                                  }).FirstOrDefaultAsync();

            var _ChildReq = await (from itemdetails in db.Inv_PRDetail
                                   join sku in db.Itm_Master on itemdetails.SKUId equals sku.SKUId into yG
                                   from y1 in yG.DefaultIfEmpty()
                                   where itemdetails.PRId == id
                                   select new PRVM_Detail()
                                   {
                                       //PRId = itemdetails.PRId,
                                       SKU = y1.SKUName,
                                       Model = y1.Itm_Model.Model,
                                       ModelId = y1.Itm_Model.ModelId,
                                       SKUId = itemdetails.SKUId,
                                       Price = itemdetails.Price,
                                       Qty = itemdetails.Qty,
                                       Remarks = itemdetails.Remarks,
                                       LocId = _MainReq.LocId,
                                       LocName = _MainReq.location
                                   }).ToListAsync();
            _MainReq.PRDetail = _ChildReq;
            return _MainReq;
        }

        public async Task<PRVM> GetPurchaseRequestByIdBranch(long id)
        {

            var _MainReq = await (from item in db.Inv_PR
                                  join loc in db.Comp_Locations on item.LocId equals loc.LocId
                                  where item.Status == true
                                  select new PRVM()
                                  {
                                      PRDate = item.PRDate,
                                      PrNo = item.PRNo,
                                      TransDate = item.TransDate,
                                      PrId = item.PRId,
                                      location = loc.LocName
                                  }).FirstOrDefaultAsync();

            var _ChildReq = await (from itemdetails in db.Inv_PRDetail
                                   join sku in db.Itm_Master on itemdetails.SKUId equals sku.SKUId
                                   where itemdetails.PRId == id
                                   select new PRVM_Detail()
                                   {
                                       PRId = itemdetails.PRId,
                                       SKU = sku.SKUName,
                                       Qty = itemdetails.Qty,
                                       Remarks = itemdetails.Remarks
                                   }).ToListAsync();

            _MainReq.PRDetail = _ChildReq;

            return _MainReq;

        }

        public async Task<List<PRVM>> GetPOBList(int LocId)
        {
            try
            {
                return await (from item in db.Inv_PR
                              join dets in db.Comp_Locations on item.LocId equals dets.LocId
                              where item.PRStatus == "P" && (item.LocId == LocId || LocId == 72)
                              select new PRVM()
                              {
                                  LCSuppName = item.LCSuppName,
                                  location = dets.LocName,
                                  PrNo = item.PRNo,
                                  PrId = item.PRId
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CreatePurchaseRequestBranch(PRVM model, long Userid)
        {
            try
            {
                if (model.PrId == 0)
                {
                    var lastPR = await db.Inv_PR.Where(x => x.LocId == model.LocId).OrderByDescending(x => x.PRId).Select(x => x.PRNo).FirstOrDefaultAsync();
                    string si = "PR-";

                    //string str = lastPR.Substring(0, 5);

                    string planNo = String.Format("{0:yyMM}", DateTime.Now) + String.Format("{0:000}", model.LocId);

                    if (lastPR == null)
                    {
                        planNo = si + planNo + "0001";
                    }
                    else if (lastPR.Substring(3, 7) == planNo)
                    {
                        planNo = si + planNo + String.Format("{0:0000}", Convert.ToInt32(lastPR.Substring(10, 4)) + 1);
                    }
                    else
                    {
                        planNo = si + planNo + "0001";
                    }

                    if (model != null)
                    {
                        Inv_PR request = new Inv_PR()
                        {
                            LocId = model.LocId,
                            PRDate = DateTime.Now,
                            PRNo = planNo,
                            Status = true,
                            LCSuppName = model.LCSuppName,
                            LCSuppAddress = model.LCSuppAddress,
                            LCSuppMob = model.LCSuppMob,
                            TransDate = DateTime.Now,
                            UserId = Userid,
                            PRStatus = "P"
                        };
                        db.Inv_PR.Add(request);
                        await db.SaveChangesAsync();

                        foreach (var item in model.PRDetail)
                        {
                            Inv_PRDetail DetailCh = new Inv_PRDetail();
                            DetailCh.PRId = request.PRId;
                            DetailCh.SKUId = item.SKUId;
                            DetailCh.Qty = item.Qty;
                            DetailCh.Remarks = item.Remarks;
                            DetailCh.Price = item.Price;
                            db.Inv_PRDetail.Add(DetailCh);
                            await db.SaveChangesAsync();
                        }

                        await new DocumentBL().UpdateDocRef(model.files, request.PRId);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    var pr = await db.Inv_PR.Where(x => x.PRId == model.PrId).FirstOrDefaultAsync();
                    pr.LocId = model.LocId;
                    pr.LCSuppName = model.LCSuppName;
                    pr.LCSuppAddress = model.LCSuppAddress;
                    pr.LCSuppMob = model.LCSuppMob;


                    var prdet = db.Inv_PRDetail.Where(x => x.PRId == model.PrId).ToList();
                    foreach (var item in prdet)
                    {
                        db.Inv_PRDetail.Remove(item);
                        await db.SaveChangesAsync();
                    }


                    foreach (var item in model.PRDetail)
                    {
                        Inv_PRDetail DetailCh = new Inv_PRDetail();
                        DetailCh.PRId = model.PrId;
                        DetailCh.SKUId = item.SKUId;
                        DetailCh.Qty = item.Qty;
                        DetailCh.Remarks = item.Remarks;
                        DetailCh.Price = item.Price;
                        db.Inv_PRDetail.Add(DetailCh);
                        await db.SaveChangesAsync();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }

        }
        #endregion
        #region GeneralFunctions

        public List<OrderManagerDashboardVM> GetOrderManagerDasboard(DateTime FromDate, DateTime ToDate, int RegionId, int PoStatusId, int CategoryId, int CompanyId, int SupplierId, int ProductId)
        {
            return db.spget_OrderManagerDashboard(FromDate, ToDate, RegionId, CategoryId, CompanyId, SupplierId, PoStatusId, ProductId).Select(x => new OrderManagerDashboardVM()
            {
                ComName = x.ComName,
                Pending = x.Pending,
                PODate = x.PODate,
                POId = x.POId,
                PONo = x.PONo,
                POStatus = x.POStatus,
                POStatusId = x.POStatusId,
                Recived = x.Recived,
                SupplierName = x.SupplierName
            }).ToList();
        }

        public async Task<bool> UpdatePOStatus(int poid, int status, int UserId)
        {
            var po = await db.Inv_PO.Where(x => x.POId == poid).FirstOrDefaultAsync();
            if (po != null)
            {
                try
                {
                    po.Status = status;
                    po.ModifiedBy = UserId;
                    po.ModifiedDate = DateTime.Now;
                    await db.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region OrderManagement
        public async Task<dynamic> GetPOSummary(long POId)
        {
            try
            {
                int ord = await db.Inv_PODetail.Where(x => x.POId == POId).SumAsync(x => x.Qty);
                int sch = await db.Inv_POSchedule.Where(x => x.Inv_PODetail.POId == POId).SumAsync(x => (int?)x.OrderQty) ?? 0;
                int recv = await db.Inv_POSchedule.Where(x => x.Inv_PODetail.POId == POId).SumAsync(x => (int?)x.ReceivedQty) ?? 0;
                //var lst = await db.Inv_PODetail.Where(x => x.POId == POId).Select(x => new { Qty = x.Qty, ReceivedQty = x.Inv_POSchedule.Sum(a => a.ReceivedQty) }).ToListAsync();
                var data = new { ord = ord, sch = sch, recv = recv };
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ScheduleMasterVM>> GetSchedule(long POId)
        {
            try
            {
                return await db.Inv_POSchedule.Where(x => x.Inv_PODetail.POId == POId).Select(x => new ScheduleMasterVM { SchMasterId = x.SchMasterId ?? 0 }).Distinct().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<OrderScheduleVM>> GetScheduleDetail(long SchMasterId)
        {
            try
            {
                return await db.Inv_POSchedule.Where(x => x.SchMasterId == SchMasterId).Select(x => new OrderScheduleVM
                {
                    Qty = x.OrderQty,
                    LocId = x.LocId,
                    PODtlId = x.PODtlId,
                    SKUId = x.Inv_PODetail.SKUId,
                    SKU = x.Inv_PODetail.Itm_Master.SKUCode,
                    PendingQty = x.ReceivedQty
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<OrderScheduleVM> GetSaleStockForSchedule(int PODtlId, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return db.spget_SaleStockForSchedule(PODtlId, FromDate, ToDate).Select(x => new OrderScheduleVM
                {
                    Model = x.Model,
                    ModelId = x.ModelId,
                    Qty = x.OrderQty,
                    SaleQty = x.Sale,
                    StockQty = x.Stock,
                    PendingQty = x.Pending,
                    LocId = x.LocId,
                    LocName = x.LocName,
                    PODtlId = PODtlId,
                    SKU = x.SKUName,
                    SKUId = x.SKUId
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> IsValidSchedule(IEnumerable<OrderScheduleVM> mod, long SchMasterId)
        {
            try
            {
                var lst = mod.GroupBy(x => x.PODtlId).Select(x => new { x.Key, Qty = x.Sum(a => a.Qty) }).ToList();
                foreach (var x in lst)
                {
                    var ls = await db.Inv_PODetail.Where(a => a.PODtlId == x.Key).SumAsync(a => (int?)a.Qty) ?? 0;
                    var sch = await db.Inv_POSchedule.Where(a => a.PODtlId == x.Key && a.SchMasterId != SchMasterId).SumAsync(a => (int?)a.OrderQty) ?? 0;
                    if (x.Qty + sch > ls)
                    {
                        return "Qty should be less than Order Qty";
                    }
                }
                return "OK";
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        public async Task<string> CreateOrderSchedule(IEnumerable<OrderScheduleVM> mod, long SchMasterId, DateTime DeliveryDate, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    List<Inv_PODetail> det = new List<Inv_PODetail>();
                    if (SchMasterId > 0)
                    {
                        var msg = await IsValidSchedule(mod, SchMasterId);
                        if (msg != "OK")
                        {
                            return msg;
                        }
                        foreach (var x in mod)
                        {
                            var pod = await db.Inv_PODetail.Where(a => a.PODtlId == x.PODtlId).Select(a => new { a.Inv_PO.PONo, a.Itm_Master.SKUCode, a.PODtlId, OrderQty = a.Inv_POSchedule.Sum(b => (int?)b.OrderQty) ?? 0, Qty = a.Qty }).FirstOrDefaultAsync();
                            //if (x.Qty < ord.ReceivedQty)
                            //{
                            //    return "Qty should be greater than Recv Qty";
                            //}
                            //if (x.Qty > ord.OrderQty)
                            //{
                            //    return "Qty should be less than Total Order Qty";
                            //}
                            var sch = await db.Inv_POSchedule.Where(a => a.PODtlId == x.PODtlId && a.LocId == x.LocId && a.SchMasterId == SchMasterId).FirstOrDefaultAsync();
                            if (sch != null)
                            {
                                if (x.Qty < sch.ReceivedQty)
                                {
                                    return "Qty should be greater than Recv Qty";
                                }
                                if (sch.OrderQty != x.Qty)
                                {
                                    Inv_POScheduleLog log = new Inv_POScheduleLog
                                    {
                                        LocId = sch.LocId,
                                        OrderBy = sch.OrderBy,
                                        OrderDate = sch.OrderDate,
                                        OrderQty = sch.OrderQty,
                                        PODtlId = sch.PODtlId,
                                        POSchId = sch.POSchId
                                    };
                                    db.Inv_POScheduleLog.Add(log);

                                    if (x.Qty > 0)
                                        notificationBL.PostNotiLoc(9, x.LocId, "PO Scheduled. PONo: " + sch.Inv_PODetail.Inv_PO.PONo + " SKU: " + sch.Inv_PODetail.Itm_Master.SKUCode + " Qty:" + x.Qty, UserId);

                                    sch.OrderQty = x.Qty;
                                    sch.OrderBy = UserId;
                                    sch.OrderDate = DateTime.Now;
                                }
                            }
                            else
                            {
                                if (x.Qty > pod.Qty - pod.OrderQty)
                                {
                                    return "Qty should be less than Order Qty";
                                }
                                if (x.Qty > 0)
                                {
                                    //var pod = await db.Inv_PODetail.FindAsync(x.PODtlId);
                                    notificationBL.PostNotiLoc(9, x.LocId, "PO Scheduled. PONo: " + pod.PONo + " SKU: " + pod.SKUCode + " Qty:" + x.Qty, UserId);
                                    Inv_POSchedule tbl = new Inv_POSchedule()
                                    {
                                        LocId = x.LocId,
                                        OrderQty = x.Qty,
                                        PODtlId = x.PODtlId,
                                        OrderBy = UserId,
                                        OrderDate = DateTime.Now,
                                        SchMasterId = SchMasterId
                                    };
                                    db.Inv_POSchedule.Add(tbl);
                                }
                            }
                            await db.SaveChangesAsync();
                        }

                    }
                    else
                    {
                        var poDtlId = mod.FirstOrDefault().PODtlId;
                        var poId = await db.Inv_PODetail.Where(x => x.PODtlId == poDtlId).Select(x => x.POId).FirstOrDefaultAsync();
                        Inv_POSchMaster mas = new Inv_POSchMaster()
                        {
                            DeliveryDate = DeliveryDate,
                            SchDate = DateTime.Now.Date,
                            POId = poId
                        };
                        db.Inv_POSchMaster.Add(mas);
                        await db.SaveChangesAsync();

                        foreach (var x in mod)
                        {
                            var pod = await db.Inv_PODetail.Where(a => a.PODtlId == x.PODtlId).Select(a => new { a.Inv_PO.PONo, a.Itm_Master.SKUCode, a.PODtlId, OrderQty = a.Inv_POSchedule.Sum(b => (int?)b.OrderQty) ?? 0, Qty = a.Qty }).FirstOrDefaultAsync();
                            //if (x.Qty < ord.ReceivedQty)
                            //{
                            //    return "Qty should be greater than Recv Qty";
                            //}
                            //if (x.Qty > (ord.Qty - ord.OrderQty))
                            //{
                            //    return "Qty should be less than Total Order Qty";
                            //}
                            if (x.Qty > pod.Qty - pod.OrderQty)
                            {
                                return "Qty should be less than Order Qty";
                            }
                            if (x.Qty > 0)
                            {
                                //var pod = await db.Inv_PODetail.FindAsync(x.PODtlId);
                                notificationBL.PostNotiLoc(9, x.LocId, "PO Scheduled. PONo: " + pod.PONo + " SKU: " + pod.SKUCode + " Qty:" + x.Qty, UserId);
                                Inv_POSchedule tbl = new Inv_POSchedule()
                                {
                                    LocId = x.LocId,
                                    OrderQty = x.Qty,
                                    PODtlId = x.PODtlId,
                                    OrderBy = UserId,
                                    OrderDate = DateTime.Now,
                                    SchMasterId = mas.SchMasterId
                                };
                                db.Inv_POSchedule.Add(tbl);
                            }
                        }
                        SchMasterId = mas.SchMasterId;
                    }
                    await db.SaveChangesAsync();
                    scop.Complete();
                    scop.Dispose();
                    return SchMasterId.ToString();
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return "Error";
                }
            }
        }


        public async Task<int> GetLocalPODocuments(int id)
        {
            var docs = await db.Comp_Documents.Where(x => x.RefObjId == id).ToListAsync();
            if (docs != null)
            {
                return docs.Count();
            }
            else
            {
                return 0;
            }
        }
        public string GetNewPONo(string lastPO)
        {

            string PONo = "PO-" + DateTime.Now.ToString("yyMM");
            if (lastPO == null)
            {
                PONo = PONo + "000001";
            }
            else if (lastPO.Substring(0, 7) == PONo)
            {
                PONo = PONo + (Convert.ToInt32(lastPO.Substring(7, 6)) + 1).ToString("000000");
            }
            else
            {
                PONo = PONo + "000001";
            }
            //}
            return PONo;
        }

        public async Task<long> CreatePurchaseOrder(IEnumerable<PODetailVM> mod, int[] CityId, int SuppId, DateTime DueDate, string Remarks, int PaymentTerm, int PlanId, string PolicyType, int UserId)
        {
            try
            {
                DateTime cDate = DateTime.Now.Date; //setupBL.GetWorkingDate(UserInfo.LocId);
                int? DiscPolicyId = null;
                int? PairPolicyId = null;

                List<Inv_PODetail> det = new List<Inv_PODetail>();
                foreach (var x in mod)
                {
                    var order = await AddToOrder(x.SKUId, SuppId, x.Qty, x.CityId, 1);
                    if (PolicyType == "D")
                    {
                        DiscPolicyId = await (from DD in db.Itm_DiscountPolicyModel
                                              where DD.Itm_DiscountPolicy.StartDate <= cDate
                                              && DD.Itm_DiscountPolicy.EndDate >= cDate
                                              && DD.Itm_DiscountPolicy.Status
                                              && DD.ModelId == x.ModelId
                                              select DD.PolicyId).OrderByDescending(DD => DD).FirstOrDefaultAsync();
                    }
                    else
                    {
                        PairPolicyId = await db.Itm_PairPolicy.Where(DD => DD.FromDate <= cDate && DD.ToDate >= cDate
                                          && DD.Status
                                          && DD.ModelId == x.ModelId)
                                          .Select(DD => DD.PolicyId).OrderByDescending(DD => DD).FirstOrDefaultAsync();
                    }
                    det.Add(new Inv_PODetail
                    {
                        SKUId = x.SKUId,
                        Qty = x.Qty,
                        Discount = order.Discount,
                        GST = order.GST,
                        TP = order.TP,
                        MRP = order.MRP,
                        WHT = order.WHT,
                        DiscPolicyId = DiscPolicyId,
                        PairPolicyId = PairPolicyId,
                        ExtraCharges = 0,
                        IsGiftItem = false,
                        CityId = x.CityId,
                        AnnuallyIncentive = order.AnnuallyIncentive,
                        QuarterlyIncentive = order.QuarterlyIncentive,
                        MonthlyIncentive = order.MonthlyIncentive,
                        BiannuallyIncentive = order.BiannuallyIncentive
                    });
                }
                decimal Disc = 0;
                if (PolicyType == "D")
                {
                    //Disc = await ApplyDiscountPolicy(mod);
                }
                else
                {

                    var lst = await ApplyPairPolicy(mod);
                    foreach (var v in lst)
                    {
                        var order = await AddToOrder(v.SKUId, SuppId, v.Qty, v.CityId, 1);
                        det.Add(new Inv_PODetail()
                        {
                            SKUId = v.SKUId,
                            Qty = v.Qty,
                            Discount = 0,
                            ExtraCharges = 0,//v.ExtraCharges,
                            GST = 0,
                            IsGiftItem = true,
                            MRP = order.MRP,
                            TP = 0,
                            WHT = 0,
                            PP = 0,
                            CityId = v.CityId
                        });
                    }

                }

                var lastPO = await db.Inv_PO.OrderByDescending(x => x.POId).Select(x => x.PONo).FirstOrDefaultAsync();
                string PONo = GetNewPONo(lastPO);

                Inv_PO ord = new Inv_PO()
                {
                    SuppId = SuppId,
                    TransDate = DateTime.Now,
                    UserId = UserId,
                    DeliveryDate = DueDate,
                    PODate = DateTime.Now.Date, //setupBL.GetWorkingDate(UserInfo.LocId),
                    Remarks = Remarks,
                    Status = 1,
                    PaymentTerm = PaymentTerm,
                    POTypeId = 1,
                    PONo = PONo,
                    PolicyType = PolicyType,
                    PlanId = PlanId,
                    Discount = Disc,
                    Inv_PODetail = det,
                    //Inv_POMapping = map
                };
                db.Inv_PO.Add(ord);




                await db.SaveChangesAsync();

                return ord.POId;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public async Task<long> SaveLocalPurchase(IEnumerable<PODetailVM> mod, string Remarks, int SuppId, string SupName, string MobileNo, string Address, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    DateTime cDate = DateTime.Now.Date; //setupBL.GetWorkingDate(UserInfo.LocId);
                    List<Inv_PODetail> det = new List<Inv_PODetail>();

                    foreach (var x in mod)
                    {
                        List<Inv_POSchedule> sch = new List<Inv_POSchedule>();
                        sch.Add(new Inv_POSchedule
                        {
                            LocId = x.CityId,
                            OrderBy = UserId,
                            OrderDate = DateTime.Now.Date, //setupBL.GetWorkingDate(UserInfo.LocId),
                            OrderQty = x.Qty,
                            ReceivedQty = 0
                        });
                        det.Add(new Inv_PODetail
                        {
                            SKUId = x.SKUId,
                            Qty = x.Qty,
                            Discount = 0,
                            GST = 0,
                            TP = x.PP,
                            PP = x.PP,
                            MRP = x.MRP,
                            WHT = 0,
                            ExtraCharges = 0,
                            IsGiftItem = false,
                            CityId = x.CityId,
                            Inv_POSchedule = sch
                        });
                    }

                    var lastPO = await db.Inv_PO.OrderByDescending(x => x.POId).Select(x => x.PONo).FirstOrDefaultAsync();
                    string PONo = GetNewPONo(lastPO);
                    Inv_PO ord = new Inv_PO()
                    {
                        SuppId = SuppId,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        DeliveryDate = DateTime.Now,
                        PODate = DateTime.Now.Date, //setupBL.GetWorkingDate(UserInfo.LocId),
                        Remarks = Remarks,
                        Status = 4,
                        PaymentTerm = 11,
                        POTypeId = 3,
                        PONo = PONo,
                        PolicyType = "N",
                        PlanId = 0,
                        Discount = 0,
                        ApprovedBy = UserId,
                        ApprovedDate = DateTime.Now,
                        CheckedBy = UserId,
                        CheckedDate = DateTime.Now,
                        LCSuppAddress = Address,
                        LCSuppMobile = MobileNo,
                        LCSuppName = SupName,
                        Inv_PODetail = det,
                    };
                    db.Inv_PO.Add(ord);
                    await db.SaveChangesAsync();

                    Inv_POSchMaster mas = new Inv_POSchMaster()
                    {
                        DeliveryDate = DateTime.Now.Date,
                        SchDate = DateTime.Now.Date,
                        POId = ord.POId
                    };
                    db.Inv_POSchMaster.Add(mas);
                    await db.SaveChangesAsync();

                    var schLst = await db.Inv_POSchedule.Where(x => x.Inv_PODetail.POId == ord.POId).ToListAsync();
                    schLst.ForEach(x => x.SchMasterId = mas.SchMasterId);
                    await db.SaveChangesAsync();

                    scop.Complete();
                    scop.Dispose();
                    return ord.POId;
                }
                catch (Exception ex)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }

        public async Task<long> CreateLocalPurchaseOrder(IEnumerable<PODetailVM> mod, string Remarks, int SuppId, string SupName, string MobileNo, string Address, int UserId, long prid, List<long> str)
        {
            try
            {
                var PRStats = await db.Inv_PR.Where(x => x.PRId == prid).FirstOrDefaultAsync();
                if (PRStats != null && PRStats.PRStatus == "P")
                {
                    var TransId = await SaveLocalPurchase(mod, Remarks, SuppId, SupName, MobileNo, Address, UserId);
                    if (TransId > 0)
                    {
                        PRStats.PRStatus = "A";
                        PRStats.POId = TransId;
                        PRStats.ModifiedBy = UserId;
                        PRStats.ModifiedDate = DateTime.Now;
                        await db.SaveChangesAsync();
                        await new DocumentBL().UpdateDocRef(str, TransId);
                        return TransId;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    var TransId = await SaveLocalPurchase(mod, Remarks, SuppId, SupName, MobileNo, Address, UserId);
                    if (TransId > 0)
                    {
                        await new DocumentBL().UpdateDocRef(str, TransId);
                        return TransId;
                    }
                    else
                    {
                        return 0;
                    }
                }

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> EditPurchaseOrderMobile(IEnumerable<PODetailVM> mod, int SuppId, DateTime DueDate, string Remarks, int PaymentTerm, int PlanId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    List<Inv_POSchedule> _ExistPoMob_PoSchedule_Lst_Del = new List<Inv_POSchedule>();
                    var _ExistPoMob_PODetail = await db.Inv_PODetail.Where(x => x.POId == PlanId).ToListAsync();
                    //var _ExistPoMob_POSchMaster = await db.Inv_POSchMaster.Where(x => x.POId == PlanId).ToListAsync();

                    var _PoMob_ToBeDeleted = _ExistPoMob_PODetail.Where(y => !mod.Any(z => z.PODtlId == y.PODtlId)).ToList();
                    foreach (var item in _PoMob_ToBeDeleted)
                    {
                        Inv_POSchedule Po_Secdule = await db.Inv_POSchedule.Where(x => x.PODtlId == item.PODtlId).FirstOrDefaultAsync();
                        if (Po_Secdule != null)
                        {
                            _ExistPoMob_PoSchedule_Lst_Del.Add(Po_Secdule);
                        }
                    }
                    var _NewPO_ToBeAdded = mod.Where(x => x.PODtlId == 0).Select(x => new Inv_PODetail()
                    {
                        POId = PlanId,
                        SKUId = x.SKUId,
                        Qty = x.Qty,
                        DiscPolicyId = x.PolicyId,
                        PP = x.PP,
                        TP = x.TP,
                        MRP = x.MRP,
                        Discount = x.Discount,
                        GST = x.GST,
                        WHT = x.WHT,
                        CityId = x.CityId
                    }).ToList();

                    db.Inv_PODetail.RemoveRange(_PoMob_ToBeDeleted);
                    db.Inv_POSchedule.RemoveRange(_ExistPoMob_PoSchedule_Lst_Del);
                    db.Inv_PODetail.AddRange(_NewPO_ToBeAdded);
                    await db.SaveChangesAsync();
                    scop.Complete();
                    scop.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }

        public async Task<long> CreatePurchaseOrderMobile(IEnumerable<PODetailVM> mod, int SuppId, DateTime DueDate, string Remarks, int PaymentTerm, int PlanId, int UserId)
        {
            db.Database.CommandTimeout = 7200;
            List<Inv_PODetail> det = new List<Inv_PODetail>();
            foreach (var x in mod)
            {
                List<Inv_POSchedule> sch = new List<Inv_POSchedule>();
                //var planLst = db.Inv_POPlanDetail.Where(a => a.SKUId == x.SKUId && a.PlanId == PlanId);
                //int qty = 0;
                //foreach (var v in planLst)
                //{
                //    qty = qty + v.Qty;
                //    if(qty <= x.Qty)
                //    {
                sch.Add(new Inv_POSchedule
                {
                    LocId = x.CityId,
                    OrderBy = UserId,
                    OrderDate = DateTime.Now.Date, //setupBL.GetWorkingDate(UserInfo.LocId),
                    OrderQty = x.Qty,
                    ReceivedQty = 0
                });
                //    }
                //}
                var order = await AddToOrder(x.SKUId, SuppId, x.Qty, x.CityId, 2);
                det.Add(new Inv_PODetail
                {
                    SKUId = x.SKUId,
                    Qty = x.Qty,
                    Discount = order.Discount,
                    GST = order.GST,
                    TP = order.TP,
                    MRP = order.MRP,
                    WHT = order.WHT,
                    ExtraCharges = 0,
                    IsGiftItem = false,
                    Inv_POSchedule = sch,
                    CityId = x.CityId
                });
            }
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    DateTime cDate = DateTime.Now.Date; //setupBL.GetWorkingDate(UserInfo.LocId);

                    var lastPO = await db.Inv_PO.OrderByDescending(x => x.POId).Select(x => x.PONo).FirstOrDefaultAsync();
                    string PONo = GetNewPONo(lastPO);

                    Inv_PO ord = new Inv_PO()
                    {
                        SuppId = SuppId,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        DeliveryDate = DueDate,
                        PODate = DateTime.Now.Date, //setupBL.GetWorkingDate(UserInfo.LocId),
                        Remarks = Remarks,
                        Status = 1,
                        PaymentTerm = PaymentTerm,
                        POTypeId = 2,
                        PONo = PONo,
                        PolicyType = "N",
                        PlanId = PlanId,
                        Discount = 0,
                        Inv_PODetail = det
                    };
                    db.Inv_PO.Add(ord);
                    await db.SaveChangesAsync();

                    Inv_POSchMaster mas = new Inv_POSchMaster()
                    {
                        DeliveryDate = DateTime.Now.Date,
                        SchDate = DateTime.Now.Date,
                        POId = ord.POId
                    };
                    db.Inv_POSchMaster.Add(mas);
                    await db.SaveChangesAsync();

                    var schLst = await db.Inv_POSchedule.Where(x => x.Inv_PODetail.POId == ord.POId).ToListAsync();
                    schLst.ForEach(x => x.SchMasterId = mas.SchMasterId);
                    await db.SaveChangesAsync();

                    scop.Complete();
                    scop.Dispose();
                    return ord.POId;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public List<OrderSearchVM> OrderSearch(DateTime fromDate, DateTime toDate, int status, int suppId, string PONo, string POInvNo)
        {
            try
            {
                return db.spget_OrderSearch(fromDate, toDate, status, suppId, PONo, POInvNo)
                    .Select(x => new OrderSearchVM
                    {
                        FullName = x.FullName,
                        Remarks = x.Remarks,
                        Status = x.Status,
                        SuppName = x.SuppName,
                        DeliveryDate = x.DeliveryDate,
                        PODate = x.PODate,
                        POId = x.POId,
                        PInvId = x.PInvId ?? 0,
                        POTypeId = x.POTypeId,
                        PONo = x.PONo
                    }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<OrderSearchVM> OrderPlanSearch(DateTime fromDate, DateTime toDate)
        {
            try
            {
                List<OrderSearchVM> lst = new List<OrderSearchVM>();
                var poplan = (from item in db.Inv_POPlanCity
                              join comp in db.Itm_Company on item.ComId equals comp.ComId
                              where item.TransDate >= fromDate && item.TransDate <= toDate && item.Status == "P"
                              select new OrderSearchVM()
                              {
                                  SuppName = comp.ComName,
                                  PODate = item.TransDate,
                                  POId = item.PlanId,
                                  PONo = item.PlanNo
                              }).ToList();



                var poplanmob = (from item in db.Inv_POPlan
                                 join comp in db.Inv_Suppliers on item.SuppId equals comp.SuppId
                                 where item.TransDate >= fromDate && item.TransDate <= toDate && item.POTypeId == 2
                                 select new OrderSearchVM()
                                 {
                                     SuppName = comp.SuppName,
                                     PODate = item.TransDate,
                                     POId = item.PlanId,
                                     PONo = item.PlanId.ToString()
                                 }).ToList();

                lst.AddRange(poplan);
                lst.AddRange(poplanmob);

                return lst;

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<POStatusVM>> POStatusList()
        {
            try
            {
                return await db.Inv_POStatus.Select(x => new POStatusVM { Status = x.Status, StatusId = x.StatusId }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<POTypeVM>> POTypeList()
        {
            try
            {
                return await db.Inv_POType.Select(x => new POTypeVM { POType = x.POType, POTypeId = x.POTypeId }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<OrderSearchVM> GetOrder(long Id)
        {
            try
            {
                var po = await db.Inv_PO.Where(x => x.POId == Id).FirstOrDefaultAsync();
                if (po.Status >= 3 && po.Status <= 6)
                {
                    var ord = new OrderSearchVM()
                    {
                        FullName = po.Users_Login.FullName,
                        Remarks = po.Remarks,
                        Status = po.Inv_POStatus.Status,
                        SuppName = po.Inv_Suppliers.SuppName,
                        DeliveryDate = po.DeliveryDate,
                        PODate = po.PODate,
                        POId = po.POId,
                        SuppId = po.SuppId,
                        PONo = po.PONo
                    };
                    ord.FromDate = DateTime.Now.Date.AddMonths(-1);
                    ord.ToDate = DateTime.Now.Date;
                    return ord;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<PODetailVM> GetOrderDetailRow(int Id)
        {
            try
            {
                var det = await db.Inv_PODetail.Where(x => x.PODtlId == Id).FirstOrDefaultAsync();
                return new PODetailVM
                {
                    Discount = det.Discount,
                    Qty = det.Qty,
                    GST = det.GST,
                    ModelId = det.Itm_Master.ModelId,
                    Model = det.Itm_Master.Itm_Model.Model,
                    MRP = det.MRP,
                    PODtlId = det.PODtlId,
                    SKU = det.Itm_Master.SKUName,
                    SKUId = det.SKUId,
                    TP = det.TP,
                    WHT = det.WHT,
                    CityId = det.CityId,
                    City = db.Comp_City.Find(det.CityId).City
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<PODetailVM>> OrderDetail(int POId)
        {
            try
            {
                return await db.Inv_PODetail.Where(x => x.POId == POId).Select(x => new PODetailVM
                {
                    Discount = x.Discount,
                    Qty = x.Qty,
                    GST = x.GST,
                    ModelId = x.Itm_Master.ModelId,
                    Model = x.Itm_Master.Itm_Model.Model,
                    MRP = x.MRP,
                    PODtlId = x.PODtlId,
                    SKU = x.Itm_Master.SKUName,
                    SKUId = x.SKUId,
                    TP = x.TP,
                    WHT = x.WHT,
                    NetPrice = x.TP - x.WHT - x.Discount - x.GST,
                    Amount = (x.TP - x.WHT - x.Discount - x.GST) * x.Qty
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<CityVM>> GetCityByPOList(long POId)
        {
            try
            {
                return await (from POD in db.Inv_PODetail
                              join C in db.Comp_City on POD.CityId equals C.CityId
                              where POD.POId == POId && POD.Inv_PO.POTypeId == 1
                              select new CityVM { City = C.City, CityId = C.CityId }).Distinct().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SchSKUVM>> SKUByPOCity(long POId, int CityId)
        {
            try
            {
                return await db.Inv_PODetail.Where(x => x.POId == POId && (x.CityId == CityId || CityId == 0)).Select(x => new SchSKUVM
                {
                    SKUId = x.SKUId,
                    SKUCode = x.Itm_Master.SKUCode,
                    Company = x.Itm_Master.Itm_Model.Itm_Type.Itm_Company.ComName,
                    Product = x.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName,
                    PODtlId = x.PODtlId,
                    OrderQty = x.Qty,
                    RecvQty = x.Inv_POSchedule.Sum(a => (int?)a.ReceivedQty) ?? 0,
                    SchQty = x.Inv_POSchedule.Sum(a => (int?)a.OrderQty) ?? 0
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<OrderScheduleVM> GetPODetail(long PODtlId)
        {
            try
            {
                var lst = await db.Inv_PODetail.Where(x => x.PODtlId == PODtlId).FirstOrDefaultAsync();
                OrderScheduleVM mod = new OrderScheduleVM()
                {
                    Qty = lst.Qty,
                    PendingQty = lst.Inv_POSchedule.Sum(x => x.OrderQty),
                    StockQty = lst.Inv_POSchedule.Sum(x => x.ReceivedQty)
                };

                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}