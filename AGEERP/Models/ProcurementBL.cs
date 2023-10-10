using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace AGEERP.Models
{
    public class ProcurementBL
    {
        AGEEntities db = new AGEEntities();
        SetupBL setupBL = new SetupBL();

     

        #region Nature
        public async Task<List<ItemNatureVM>> NatureList()
        {
            try
            {
                return await db.Pro_ItemNature.Where(x => x.Status).Select(x =>
                new ItemNatureVM
                {
                    ItemNature = x.ItemNature,
                    ItemNatureId = x.ItemNatureId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ItemNatureVM> CreateNature(ItemNatureVM mod, int UserId)
        {
            try
            {
                Pro_ItemNature tbl = new Pro_ItemNature
                {
                    ItemNature = mod.ItemNature,
                    Status = true
                };
                db.Pro_ItemNature.Add(tbl);
                await db.SaveChangesAsync();
                mod.ItemNatureId = tbl.ItemNatureId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateNature(ItemNatureVM mod, int UserId)
        {
            try
            {

                var tbl = await db.Pro_ItemNature.SingleOrDefaultAsync(x => x.ItemNatureId.Equals(mod.ItemNatureId));
                if (tbl != null)
                {
                    tbl.ItemNature = mod.ItemNature;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyNature(ItemNatureVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pro_ItemNature.SingleOrDefaultAsync(x => x.ItemNatureId.Equals(mod.ItemNatureId));
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
        #endregion
        #region CostType
        public async Task<List<CostTypeVM>> CostTypeList()
        {
            try
            {
                return await db.Fin_CostType.Where(x => x.Status).Select(x =>
                new CostTypeVM
                {
                    CostType = x.CostType,
                    CostTypeId = x.CostTypeId,
                    CapitalHOGL = x.CapitalHOGL,
                    CapitalSHGL = x.CapitalSHGL,
                    RevenueHOGL = x.RevenueHOGL,
                    RevenueSHGL = x.RevenueSHGL,
                    IsService = false,
                    AccDeprHOGL = x.AccDeprHOGL,
                    DeprSHGL = x.DeprSHGL,
                    AccDeprSHGL = x.AccDeprSHGL,
                    DeprHOGL = x.DeprHOGL,
                    DeprRate = x.DeprRate
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<CostTypeVM> CreateCostType(CostTypeVM mod, int UserId)
        {
            try
            {
                Fin_CostType tbl = new Fin_CostType
                {
                    CostType = mod.CostType,
                    Status = true,
                    CapitalHOGL = mod.CapitalHOGL,
                    CapitalSHGL = mod.CapitalSHGL,
                    RevenueHOGL = mod.RevenueHOGL,
                    RevenueSHGL = mod.RevenueSHGL,
                    IsService = false,
                    AccDeprHOGL = mod.AccDeprHOGL,
                    CostTypeId = mod.CostTypeId,
                    DeprHOGL = mod.DeprHOGL,
                    DeprRate = mod.DeprRate,
                    AccDeprSHGL = mod.AccDeprSHGL,
                    DeprSHGL = mod.DeprSHGL
                };
                db.Fin_CostType.Add(tbl);
                await db.SaveChangesAsync();
                mod.CostTypeId = tbl.CostTypeId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCostType(CostTypeVM mod, int UserId)
        {
            try
            {

                var tbl = await db.Fin_CostType.SingleOrDefaultAsync(x => x.CostTypeId.Equals(mod.CostTypeId));
                if (tbl != null)
                {
                    tbl.CostType = mod.CostType;
                    tbl.CapitalHOGL = mod.CapitalHOGL;
                    tbl.CapitalSHGL = mod.CapitalSHGL;
                    tbl.RevenueHOGL = mod.RevenueHOGL;
                    tbl.RevenueSHGL = mod.RevenueSHGL;
                    tbl.DeprRate = mod.DeprRate;
                    tbl.DeprHOGL = mod.DeprHOGL;
                    tbl.AccDeprHOGL = mod.AccDeprHOGL;
                    tbl.AccDeprSHGL = mod.AccDeprSHGL;
                    tbl.DeprSHGL = mod.DeprSHGL;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyCostType(CostTypeVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Fin_CostType.SingleOrDefaultAsync(x => x.CostTypeId.Equals(mod.CostTypeId));
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
        #endregion
        #region Brand
        public async Task<List<ItemBrandVM>> BrandList()
        {
            try
            {
                return await db.Pro_ItemBrand.Where(x => x.Status).Select(x =>
                new ItemBrandVM
                {
                    ItemBrand = x.ItemBrand,
                    ItemBrandId = x.ItemBrandId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ItemBrandVM> CreateBrand(ItemBrandVM mod, int UserId)
        {
            try
            {
                Pro_ItemBrand tbl = new Pro_ItemBrand
                {
                    ItemBrand = mod.ItemBrand,
                    Status = true
                };
                db.Pro_ItemBrand.Add(tbl);
                await db.SaveChangesAsync();
                mod.ItemBrandId = tbl.ItemBrandId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateBrand(ItemBrandVM mod, int UserId)
        {
            try
            {

                var tbl = await db.Pro_ItemBrand.SingleOrDefaultAsync(x => x.ItemBrandId.Equals(mod.ItemBrandId));
                if (tbl != null)
                {
                    tbl.ItemBrand = mod.ItemBrand;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyBrand(ItemBrandVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pro_ItemBrand.SingleOrDefaultAsync(x => x.ItemBrandId.Equals(mod.ItemBrandId));
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
        #endregion
        #region Category
        public async Task<List<ItemCategoryVM>> CategoryList()
        {
            try
            {
                return await db.Pro_ItemCategory.Where(x => x.Status).Select(x =>
                new ItemCategoryVM
                {
                    ItemCategory = x.ItemCategory,
                    ItemCategoryId = x.ItemCategoryId,
                    ItemNatureId = x.ItemNatureId,
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ItemCategoryVM> CreateCategory(ItemCategoryVM mod, int UserId)
        {
            try
            {
                Pro_ItemCategory tbl = new Pro_ItemCategory
                {
                    ItemCategory = mod.ItemCategory,
                    ItemNatureId = mod.ItemNatureId,
                    Status = true
                };
                db.Pro_ItemCategory.Add(tbl);
                await db.SaveChangesAsync();
                mod.ItemCategoryId = tbl.ItemCategoryId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCategory(ItemCategoryVM mod, int UserId)
        {
            try
            {

                var tbl = await db.Pro_ItemCategory.SingleOrDefaultAsync(x => x.ItemCategoryId.Equals(mod.ItemCategoryId));
                if (tbl != null)
                {
                    tbl.ItemCategory = mod.ItemCategory;
                    tbl.ItemNatureId = mod.ItemNatureId;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyCategory(ItemCategoryVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pro_ItemCategory.SingleOrDefaultAsync(x => x.ItemCategoryId.Equals(mod.ItemCategoryId));
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
        #endregion
        #region ItemProduct
        public async Task<List<ItemProductVM>> ItemProductList()
        {
            try
            {
                return await db.Pro_ItemProduct.Where(x => x.Status).Select(x =>
                new ItemProductVM
                {
                    ItemCategoryId = x.ItemCategoryId,
                    ItemProduct = x.ItemProduct,
                    ItemProductId = x.ItemProductId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ItemProductVM> CreateItemProduct(ItemProductVM mod, int UserId)
        {
            try
            {
                Pro_ItemProduct tbl = new Pro_ItemProduct
                {
                    Status = true,
                    ItemProduct = mod.ItemProduct,
                    ItemCategoryId = mod.ItemCategoryId
                };
                db.Pro_ItemProduct.Add(tbl);
                await db.SaveChangesAsync();
                mod.ItemProductId = tbl.ItemProductId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateItemProduct(ItemProductVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pro_ItemProduct.SingleOrDefaultAsync(x => x.ItemProductId.Equals(mod.ItemProductId));
                if (tbl != null)
                {
                    tbl.ItemProduct = mod.ItemProduct;
                    tbl.ItemCategoryId = mod.ItemCategoryId;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyItemProduct(ItemProductVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pro_ItemProduct.SingleOrDefaultAsync(x => x.ItemProductId.Equals(mod.ItemProductId));
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

        #endregion
        #region Item
        public async Task<List<ItemVM>> ItemByItemTypeList(string ItemType)
        {
            try
            {
                if (ItemType == "P")
                {
                    var lst = await db.Pro_Item.Where(x => x.Status).Select(x =>
                    new ItemVM
                    {
                        Item = x.Item,
                        ItemId = x.ItemId,
                        ItemCode = x.ItemCode
                    }).ToListAsync();
                    return lst;
                }
                else
                {
                    var lst = await db.Itm_Master.Where(x => x.ActiveStatus).Select(x =>
                    new ItemVM
                    {
                        Item = x.SKUCode,
                        ItemId = x.SKUId,
                        ItemCode = x.SKUCode
                    }).ToListAsync();
                    return lst;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ItemVM>> ItemList()
        {
            try
            {
                var lst = await db.Pro_Item.Where(x => x.Status).Select(x =>
                new ItemVM
                {
                    ItemProductId = x.ItemProductId,
                    Item = x.Item,
                    ItemId = x.ItemId,
                    ItemBrandId = x.ItemBrandId,
                    ItemCode = x.ItemCode,
                    ItemBrand = x.Pro_ItemBrand.ItemBrand,
                    ItemProduct = x.Pro_ItemProduct.ItemProduct,
                    Spec = x.Spec,
                    UOMId = x.UOMId
                }).ToListAsync();
                //var lst1 = await db.Itm_Master.Where(x => x.ActiveStatus).Select(x =>
                //new ItemVM
                //{
                //    ItemProductId = 2,
                //    Item = x.SKUCode,
                //    ItemId = x.SKUId,
                //    ItemBrandId = 0,
                //    ItemCode = x.SKUCode,
                //    ItemBrand = x.Itm_Model.Itm_Type.Itm_Company.ComName,
                //    ItemProduct = x.Itm_Model.Itm_Type.Itm_Products.ProductName,
                //    Spec = x.OtherSpecs,
                //    UOMId = 1
                //}).ToListAsync();
                //lst.AddRange(lst1);
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ItemVM>> ItemByNatureList(int NatureId)
        {
            try
            {
                var lst = await db.Pro_Item.Where(x => x.Status && x.Pro_ItemProduct.Pro_ItemCategory.ItemNatureId == NatureId).Select(x =>
                new ItemVM
                {
                    ItemProductId = 1,
                    Item = x.Item,
                    ItemId = x.ItemId,
                    ItemBrandId = 0,
                    ItemCode = x.ItemCode,
                    ItemBrand = x.Pro_ItemBrand.ItemBrand,
                    ItemProduct = x.Pro_ItemProduct.ItemProduct,
                    Spec = x.Spec,
                    UOMId = x.UOMId,
                    UOM = x.Itm_UOM.UOM
                }).ToListAsync();
                //var lst1 = await db.Itm_Master.Where(x => x.ActiveStatus).Select(x =>
                //new ItemVM
                //{
                //    ItemProductId = 2,
                //    Item = x.SKUCode,
                //    ItemId = x.SKUId,
                //    ItemBrandId = 0,
                //    ItemCode = x.SKUCode,
                //    ItemBrand = x.Itm_Model.Itm_Type.Itm_Company.ComName,
                //    ItemProduct = x.Itm_Model.Itm_Type.Itm_Products.ProductName,
                //    Spec = x.OtherSpecs,
                //    UOMId = 1
                //}).ToListAsync();
                //lst.AddRange(lst1);
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ItemVM>> ItemAllList()
        {
            try
            {
                var lst = await db.Pro_Item.Where(x => x.Status && x.Pro_ItemProduct.Pro_ItemCategory.ItemNatureId == 2).Select(x =>
                new ItemVM
                {
                    ItemProductId = 1,
                    Item = x.Item,
                    ItemId = x.ItemId,
                    ItemBrandId = 0,
                    ItemCode = x.ItemCode,
                    ItemBrand = x.Pro_ItemBrand.ItemBrand,
                    ItemProduct = x.Pro_ItemProduct.ItemProduct,
                    Spec = x.Spec,
                    UOMId = x.UOMId
                }).ToListAsync();
                var lst1 = await db.Itm_Master.Where(x => x.ActiveStatus).Select(x =>
                new ItemVM
                {
                    ItemProductId = 2,
                    Item = x.SKUCode,
                    ItemId = x.SKUId,
                    ItemBrandId = 0,
                    ItemCode = x.SKUCode,
                    ItemBrand = x.Itm_Model.Itm_Type.Itm_Company.ComName,
                    ItemProduct = x.Itm_Model.Itm_Type.Itm_Products.ProductName,
                    Spec = x.OtherSpecs,
                    UOMId = 1
                }).ToListAsync();
                lst.AddRange(lst1);
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ItemVM> CreateItem(ItemVM mod, int UserId)
        {
            try
            {
                Pro_Item tbl = new Pro_Item
                {
                    Status = true,
                    Item = mod.Item,
                    ItemProductId = mod.ItemProductId,
                    ItemBrandId = mod.ItemBrandId,
                    UOMId = mod.UOMId,
                    Spec = mod.Spec,
                    ItemCode = "",
                    TransDate = DateTime.Now,
                    UserId = UserId
                };
                db.Pro_Item.Add(tbl);
                await db.SaveChangesAsync();
                mod.ItemId = tbl.ItemId;
                mod.ItemCode = tbl.ItemCode;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateItem(ItemVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pro_Item.SingleOrDefaultAsync(x => x.ItemId.Equals(mod.ItemId));
                if (tbl != null)
                {
                    tbl.Item = mod.Item;
                    tbl.ItemProductId = mod.ItemProductId;
                    tbl.UserId = UserId;
                    tbl.UOMId = mod.UOMId;
                    tbl.TransDate = DateTime.Now;
                    tbl.Spec = mod.Spec;
                    tbl.ItemCode = mod.ItemCode;
                    tbl.ItemBrandId = mod.ItemBrandId;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DestroyItem(ItemVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Pro_Item.SingleOrDefaultAsync(x => x.ItemId.Equals(mod.ItemId));
                if (tbl != null)
                {
                    tbl.Status = false;
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

        #endregion
        #region MTR
        public async Task<string> GetMTRNo()
        {
            try
            {
                var docNo = await db.Pro_MTR.OrderByDescending(x => x.MTRId).Select(x => x.MTRNo).FirstOrDefaultAsync();
                string pre = "MTR-" + DateTime.Now.ToString("yyMM");
                if (docNo != null)
                {
                    if (docNo.Substring(0, 8) == pre)
                    {
                        docNo = pre + (Convert.ToInt32(docNo.Substring(8, 4)) + 1).ToString("0000");
                    }
                    else
                    {
                        docNo = pre + "0001";
                    }
                }
                else
                {
                    docNo = pre + "0001";
                }
                return docNo;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public async Task<ResultVM> SaveMTR(IEnumerable<MTRDetailVM> mod, int LocId, int CCCode, DateTime RequiredDate, int NatureId, int UserId)
        {
            ResultVM result = new ResultVM();
            try
            {
                var invNo = await GetMTRNo();
                DateTime workingDate = setupBL.GetWorkingDate(LocId);

                List<Pro_MTRDetail> det = new List<Pro_MTRDetail>();
                foreach (var v in mod)
                {
                    det.Add(new Pro_MTRDetail
                    {
                        EstPrice = v.EstPrice,
                        ItemId = v.ItemId,
                        Qty = v.Qty,
                        Remarks = v.Remarks,
                        ItemType = v.ItemType
                    });
                }
                Pro_MTR mas = new Pro_MTR
                {
                    LocId = LocId,
                    MTRDate = workingDate,
                    MTRNo = invNo,
                    RequiredDate = RequiredDate,
                    Status = "O",
                    TransDate = DateTime.Now,
                    UserId = UserId,
                    Pro_MTRDetail = det,
                    CCCode = CCCode,
                    NatureId = NatureId
                };
                db.Pro_MTR.Add(mas);
                await db.SaveChangesAsync();
                result.TransId = mas.MTRId;
                result.Msg = "OK";
                return result;
            }
            catch (Exception)
            {
                result.Msg = "Error";
                return result;
            }
        }

        public async Task<bool> UpdateMTR(MTRDetailVM mod)
        {
            try
            {
                var mtr = await db.Pro_MTR.FindAsync(mod.MTRId);
                if(mtr.ApprovedBy == null)
                {
                    var mtrDetail = await db.Pro_MTRDetail.FindAsync(mod.MTRDtlId);
                    mtrDetail.ItemType = mod.ItemType;
                    mtrDetail.ItemId = mod.ItemId;
                    mtrDetail.Qty = mod.Qty;
                    mtrDetail.EstPrice = mod.EstPrice;
                    mtrDetail.Remarks = mod.Remarks;
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
        public async Task<bool> ApproveMTR(MTRVM mod,int UserId)
        {
            try
            {
                var mtr = await db.Pro_MTR.FindAsync(mod.MTRId);
                if (mtr.ApprovedBy == null)
                {
                    if(mod.Status == "A")
                    {
                        mtr.Status = "A";
                        mtr.ApprovedBy = UserId;
                        mtr.ApprovedDate = DateTime.Now;
                    }
                    else if (mod.Status == "R")
                    {
                        mtr.Status = "R";
                        mtr.ApprovedBy = UserId;
                        mtr.ApprovedDate = DateTime.Now;
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
        public async Task<bool> MTRReceiving(MTRVM mod, int UserId)
        {
            try
            {
                var mtr = await db.Pro_MTR.FindAsync(mod.MTRId);
                if (mtr.Status == "C")
                {
                    mtr.Status = "K";
                    mtr.ReceivedBy = UserId;
                    mtr.ReceivedDate = DateTime.Now;
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
        #region PO
        public async Task<List<MTRVM>> MTRLst()
        {
            try
            {
                return await (from x in db.Pro_MTR
                             join C in db.Fin_CostCenters on x.CCCode equals C.CCCode
                             where  x.Status == "A"
                             select new MTRVM { MTRNo = x.MTRNo, MTRId = x.MTRId,CCCode = x.CCCode,CostCenter = C.CCDesc }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<MTRVM>> MTRLstByNature(int NatureId)
        {
            try
            {
                return await (from x in db.Pro_MTR
                              join C in db.Fin_CostCenters on x.CCCode equals C.CCCode
                              where x.Status == "A" && x.NatureId == NatureId
                              select new MTRVM { MTRNo = x.MTRNo, MTRId = x.MTRId, CCCode = x.CCCode, CostCenter = C.CCDesc }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<MTRVM>> MTRUnApproveLst()
        {
            try
            {
                return (await (from x in db.Pro_MTR
                              join C in db.Fin_CostCenters on x.CCCode equals C.CCCode
                              where x.Status == "O"
                              select new { MTRNo = x.MTRNo, MTRId = x.MTRId, CCCode = x.CCCode, CostCenter = C.CCDesc,MTRDate = x.MTRDate }).ToListAsync()).
                              Select(x => new MTRVM { MTRNo = x.MTRNo, MTRId = x.MTRId, CCCode = x.CCCode, CostCenter = x.CostCenter, MTRDate = x.MTRDate.ToString("dd/MM/yyyy") }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<OrderSearchVM>> GetPOApproval(string Level, int UserId)
        {
            try
            {
                List<OrderSearchVM> lst = new List<OrderSearchVM>();
                var usr = await db.Users_Login.FindAsync(UserId);
                var approval = usr.Users_Group.Users_GroupAccess.Where(x => x.MenuId == 23150000).Any();
                var validate = usr.Users_Group.Users_GroupAccess.Where(x => x.MenuId == 23140000).Any();

                if (Level == "V" && validate)
                {
                    lst = await (from x in db.Pro_PO
                                 join c in db.Users_Login on x.UserId equals c.UserID
                                 //join v in db.Users_Login on x.CheckedBy equals v.UserID
                                 join s in db.Fin_Subsidary on x.SuppId equals s.SubId
                                 where x.Status == "O" && x.CheckedBy == null 
                                 select
                       new OrderSearchVM
                       {
                           PODate = x.PODate,
                           POId = x.POId,
                           PONo = x.PONo,
                           SuppName = s.SubsidaryName,
                           Status = x.RevokedBy == null ? "" : "R",
                           FullName = c.FullName,
                           ValidateBy = ""
                       }).ToListAsync();
                }
                else if (Level == "A" && approval)
                {
                    lst = await (from x in db.Pro_PO
                                 join c in db.Users_Login on x.UserId equals c.UserID
                                 join v in db.Users_Login on x.CheckedBy equals v.UserID
                                 join s in db.Fin_Subsidary  on x.SuppId equals s.SubId
                                 where x.Status == "V" && x.CheckedBy != null && x.ApprovedBy == null
                                 select
                        new OrderSearchVM
                        {
                            PODate = x.PODate,
                            POId = x.POId,
                            PONo = x.PONo,
                            SuppName = s.SubsidaryName,
                            Status = x.RevokedBy == null ? "" : "R",
                            FullName = c.FullName,
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
                var vr = await db.Pro_PO.FindAsync(mod.POId);
                if (Level == "V" && mod.Status == "V")
                {
                    if (vr.CheckedBy == null)
                    {
                        vr.CheckedBy = UserId;
                        vr.CheckedDate = DateTime.Now;
                        vr.Status = mod.Status;
                    }
                }
                else if (Level == "A" && mod.Status == "A")
                {
                    if (vr.ApprovedBy == null && vr.CheckedBy != null)
                    {
                        vr.ApprovedBy = UserId;
                        vr.ApprovedDate = DateTime.Now;
                        vr.Status = mod.Status;
                    }
                }
                else if (mod.Status == "R")
                {
                    if (vr.ApprovedBy == null)
                    {
                        vr.RevokedBy = UserId;
                        vr.RevokedDate = DateTime.Now;
                        vr.Status = mod.Status;
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
        public async Task<int> CCCodeByEmpId(int UserId)
        {
            try
            {
                return await db.Pay_EmpMaster.Where(x => x.EmpId == UserId).Select(x => x.Pay_Department.CCCode).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<List<MTRVM>> MTRReceiveLst(int LocId, int UserId)
        {
            try
            {
                var cc = await db.Pay_EmpMaster.Where(x => x.EmpId == UserId).Select(x => x.Pay_Department.CCCode).FirstOrDefaultAsync();
                return (await (from x in db.Pro_MTR
                               join C in db.Fin_CostCenters on x.CCCode equals C.CCCode
                               where x.Status != "K" && x.Status != "R" && x.LocId == LocId && x.CCCode == cc
                               select new { MTRNo = x.MTRNo, MTRId = x.MTRId, CCCode = x.CCCode, CostCenter = C.CCDesc, MTRDate = x.MTRDate,Status = x.Status }).ToListAsync()).
                              Select(x => new MTRVM { MTRNo = x.MTRNo, MTRId = x.MTRId, CCCode = x.CCCode, CostCenter = x.CostCenter, MTRDate = x.MTRDate.ToString("dd/MM/yyyy"), Status = x.Status }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<MTRDetailVM>> MTRDetailLst(long MTRId)
        {
            try
            {
                var lst = await (from x in db.Pro_MTRDetail
                                 join I in db.Pro_MTR on x.MTRId equals I.MTRId
                                 join P in db.Pro_Item on x.ItemId equals P.ItemId
                                 where x.ItemType == "P" && 
                                 x.MTRId == MTRId && I.Status == "O"
                                 select
                 new MTRDetailVM
                 {
                     ItemId = x.ItemId,
                     Item = P.Item,
                     Remarks = x.Remarks,
                     ItemType = x.ItemType,
                     MTRDtlId = x.MTRDtlId,
                     EstPrice = x.EstPrice,
                     MTRId = x.MTRId,
                     Qty = x.Qty
                 }).ToListAsync();
                lst.AddRange(await (from x in db.Pro_MTRDetail
                                 join I in db.Pro_MTR on x.MTRId equals I.MTRId
                                 join P in db.Itm_Master on x.ItemId equals P.SKUId
                                 where x.ItemType == "S" &&
                                 x.MTRId == MTRId && I.Status == "O"
                                 select
                 new MTRDetailVM
                 {
                     ItemId = x.ItemId,
                     Item = P.SKUCode,
                     Remarks = x.Remarks,
                     ItemType = x.ItemType,
                     MTRDtlId = x.MTRDtlId,
                     EstPrice = x.EstPrice,
                     MTRId = x.MTRId,
                     Qty = x.Qty
                 }).ToListAsync());
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SINDetailVM>> MTRDetailLst(long MTRId, int LocId)
        {
            try
            {
                var lst = await (from x in db.Pro_MTRDetail
                                 join I in db.Pro_Item on x.ItemId equals I.ItemId
                                 where x.ItemType == "P" && I.Pro_ItemProduct.Pro_ItemCategory.ItemNatureId == 1
                                 && x.MTRId == MTRId && x.Pro_MTR.Status == "A" && (x.Qty - (x.RecvQty ?? 0)) > 0
                                 select
                 new SINDetailVM
                 {
                     ItemId = x.ItemId,
                     Item = I.Item,
                     Qty = 0,
                     InStock = 0,
                     Remarks = x.Remarks,
                     RequiredQty = x.Qty - (x.RecvQty ?? 0),
                     ItemType = x.ItemType,
                     SINId = x.MTRDtlId
                 }).ToListAsync();
                foreach (var item in lst)
                {
                    item.InStock = await db.Pro_Stock.Where(x => x.LocId == LocId && x.ItemId == item.ItemId).SumAsync(a => (decimal?)(a.Qty - a.OutQty)) ?? 0;
                }
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SINDetailVM>> MTRFarDetailLst(long MTRId, int LocId)
        {
            try
            {
                var lst = await (from x in db.Pro_MTRDetail
                                 join I in db.Pro_Item on x.ItemId equals I.ItemId
                                 where x.MTRId == MTRId && x.ItemType == "P"
                                  && I.Pro_ItemProduct.Pro_ItemCategory.ItemNatureId == 2
                                 && x.Pro_MTR.Status == "A" && (x.Qty - (x.RecvQty ?? 0)) > 0
                                 select
                 new SINDetailVM
                 {
                     ItemId = x.ItemId,
                     Item = I.Item,
                     Qty = 0,
                     InStock = 0,
                     Remarks = x.Remarks,
                     RequiredQty = x.Qty - (x.RecvQty ?? 0),
                     ItemType = "P",
                     SINId = x.MTRDtlId
                 }).ToListAsync();
                foreach (var item in lst)
                {
                    item.InStock = await db.Pro_Stock.Where(x => x.LocId == LocId && x.ItemId == item.ItemId).SumAsync(a => (decimal?)(a.Qty - a.OutQty)) ?? 0;
                }
                var lst1 = await (from x in db.Pro_MTRDetail
                                  join I in db.Itm_Master on x.ItemId equals I.SKUId
                                  where x.MTRId == MTRId && x.ItemType == "S" && x.Pro_MTR.Status == "A" && (x.Qty - (x.RecvQty ?? 0)) > 0
                                  select
                  new SINDetailVM
                  {
                      ItemId = x.ItemId,
                      Item = I.SKUCode,
                      Qty = 0,
                      InStock = 0,
                      Remarks = x.Remarks,
                      RequiredQty = x.Qty - (x.RecvQty ?? 0),
                      ItemType = "S",
                      SINId = x.MTRDtlId
                  }).ToListAsync();
                foreach (var item in lst1)
                {
                    item.InStock = (await db.Inv_Store.Where(x => x.LocId == LocId && x.SKUId == item.ItemId && x.Inv_Status.MFact == 1).SumAsync(a => (decimal?)(a.Qty))) ?? 0;
                }
                lst.AddRange(lst1);
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SupplierVM>> SupplierLst()
        {
            try
            {
                return (await db.Fin_Subsidary.Where(x => x.SubTypeId == 458).ToListAsync()).Select(x => new SupplierVM { SuppId = Convert.ToInt32(x.SubId), SuppName = x.SubsidaryName }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SupplierVM>> SupplierAllLst()
        {
            try
            {
                var lst = (await db.Fin_Subsidary.Where(x => x.SubTypeId == 458).ToListAsync()).Select(x => new SupplierVM { SuppId = Convert.ToInt32(x.SubId), SuppName = x.SubsidaryName, CategoryId = 5 }).ToList();
                lst.AddRange(await db.Inv_Suppliers.Where(x => x.Status).Select(x => new SupplierVM { CategoryId = x.CategoryId, SuppId = x.SuppId, SuppName = x.SuppName }).ToListAsync());
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> GetPONo()
        {
            try
            {
                var docNo = await db.Pro_PO.OrderByDescending(x => x.POId).Select(x => x.PONo).FirstOrDefaultAsync();
                string pre = "PO-" + DateTime.Now.ToString("yyMM");
                if (docNo != null)
                {
                    if (docNo.Substring(0, 7) == pre)
                    {
                        docNo = pre + (Convert.ToInt32(docNo.Substring(7, 4)) + 1).ToString("0000");
                    }
                    else
                    {
                        docNo = pre + "0001";
                    }
                }
                else
                {
                    docNo = pre + "0001";
                }
                return docNo;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public async Task<ResultVM> SavePO(IEnumerable<ProPODetailVM> mod, DateTime RequiredDate, string DeliveryAddress, long MTRId, string Remarks, int SuppId, int NatureId, int UserId)
        {
            ResultVM result = new ResultVM();
            try
            {
                var invNo = await GetPONo();
                //DateTime workingDate = setupBL.GetWorkingDate(LocId);

                //if(MTRId > 0)
                //{
                //    var mtr = await db.Pro_MTR.FindAsync(MTRId);
                //    mtr.Status = "P";
                //}

                List<Pro_PODetail> det = new List<Pro_PODetail>();
                foreach (var v in mod)
                {
                    det.Add(new Pro_PODetail
                    {
                        Disc = v.Disc,
                        Rate = v.Rate,
                        Tax = v.Tax,
                        ItemId = v.ItemId,
                        Qty = v.Qty,
                        LocId = v.LocId,
                        CostTypeId = v.CostTypeId
                    });
                }
                Pro_PO mas = new Pro_PO
                {
                    PODate = DateTime.Now.Date,
                    PONo = invNo,
                    RequiredDate = RequiredDate,
                    Status = "O",
                    TransDate = DateTime.Now,
                    UserId = UserId,
                    DeliveryAddress = DeliveryAddress,
                    MTRId = MTRId,
                    Remarks = Remarks,
                    SuppId = SuppId,
                    NatureId = NatureId,
                    Pro_PODetail = det
                };
                db.Pro_PO.Add(mas);
                await db.SaveChangesAsync();
                result.TransId = mas.POId;
                result.Msg = "OK";
                return result;
            }
            catch (Exception)
            {
                result.Msg = "Error";
                return result;
            }
        }

        #endregion
        #region GRN
        public async Task<List<ProPOVM>> POLst(int LocId)
        {
            try
            {
                return await db.Pro_PODetail.Where(x => x.Pro_PO.Status == "A" && x.LocId == LocId).Select(x => new ProPOVM { POId = x.POId, PONo = x.Pro_PO.PONo, NatureId = x.Pro_PO.NatureId }).Distinct().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ProGRNDetailVM>> GRNReturn(string BillNo, int LocId)
        {
            try
            {
                return await (from GD in db.Pro_GRNDetail
                             join S in db.Pro_Stock on GD.GRNDtlId equals S.RefDocId
                             where S.LocId == LocId && S.DocNo == BillNo && S.Qty > S.OutQty
                              select new ProGRNDetailVM {PODtlId = GD.GRNDtlId,Item = GD.Pro_Item.Item,
                            ItemId = GD.ItemId,OrderQty = S.Qty-S.OutQty,Qty = 0}).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<SINDetailVM>> SIRList(string BillNo)
        {
            try
            {
                var lst = await (from GD in db.Pro_SINDetail
                              join I in db.Pro_Item on GD.ItemId equals I.ItemId
                              where GD.ItemType == "P" && GD.Pro_SIN.TransType == "I" && GD.Pro_SIN.SINNo == BillNo && GD.Pro_SIN.Pro_MTR.NatureId == 1 && GD.Qty > GD.RtnQty
                              select new SINDetailVM
                              {
                                  SINDtlId = GD.SINDtlId,
                                  Item = I.Item,
                                  ItemId = GD.ItemId,
                                  RequiredQty = GD.Qty-GD.RtnQty,
                                  Qty = 0,
                                  CostTypeId = GD.CostTypeId,
                                  ItemType = GD.ItemType
                              }).ToListAsync();
                //var lst1 = await (from GD in db.Pro_SINDetail
                //                 join I in db.Itm_Master on GD.ItemId equals I.SKUId
                //                 where GD.ItemType == "S" && GD.Pro_SIN.SINNo == BillNo && GD.Pro_SIN.Pro_MTR.NatureId == 1
                //                  select new SINDetailVM
                //                 {
                //                     SINDtlId = GD.SINDtlId,
                //                     Item = I.SKUCode,
                //                     ItemId = GD.ItemId,
                //                     RequiredQty = GD.Qty,
                //                     Qty = 0
                //                 }).ToListAsync();
                //lst.AddRange(lst1);
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ResultVM> SaveSIR(IEnumerable<SINDetailVM> mod, string SINNo,int LocId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ResultVM result = new ResultVM();
                try
                {
                    var invNo = await GetSINNo("R");
                    var sin = await db.Pro_SIN.Where(x => x.SINNo == SINNo).FirstOrDefaultAsync();
                    DateTime workingDate = setupBL.GetWorkingDate(LocId);
                    var cc = await db.Fin_CostCenters.FindAsync(sin.CCCode);
                    Pro_SIN mas = new Pro_SIN
                    {
                        SINDate = workingDate,
                        SINNo = invNo,
                        ToLocId = sin.FromLocId,
                        Type = "C",
                        CCCode = sin.CCCode,
                        FromLocId = sin.ToLocId,
                        MTRId = sin.MTRId,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        TransType = "R",
                        ReturnId = sin.SINId
                    };

                    db.Pro_SIN.Add(mas);
                    await db.SaveChangesAsync();

                    List<Pro_SINDetail> det = new List<Pro_SINDetail>();
                    var vrLst = new List<VoucherDetailVM>();
                    AccountBL accountBL = new AccountBL();
                    foreach (var v in mod)
                    {
                        if (v.Qty > 0)
                        {
                            if (v.ItemType == "P")
                            {
                                var sinDetail = await db.Pro_SINDetail.FindAsync(v.SINDtlId);
                                var costType = await db.Fin_CostType.FindAsync(v.CostTypeId);
                                long costTypeAcc = 0;
                                var clearingAcc = await accountBL.GetAcc(459);
                                var storeAcc = await accountBL.GetAcc(471);
                                if (cc.PCCode == 72)
                                {
                                    costTypeAcc = costType.RevenueHOGL ?? 0;
                                }
                                else
                                {
                                    costTypeAcc = costType.RevenueSHGL ?? 0;
                                }
                                var stock = await db.Pro_Stock.Where(x => x.RowId == sinDetail.StoreId).FirstOrDefaultAsync();
                                var dtl = new Pro_SINDetail();
                                if (v.Qty <= stock.OutQty && v.Qty <= sinDetail.Qty-sinDetail.RtnQty)
                                {
                                    dtl = new Pro_SINDetail
                                    {
                                        CPrice = stock.PPrice,
                                        ItemId = sinDetail.ItemId,
                                        Qty = v.Qty,
                                        ItemType = sinDetail.ItemType,
                                        SerialNo = "",
                                        StoreId = stock.RowId,
                                        CostTypeId = sinDetail.CostTypeId,
                                        SINId = mas.SINId
                                    };
                                    db.Pro_SINDetail.Add(dtl);
                                    sinDetail.RtnQty = sinDetail.RtnQty + v.Qty;
                                    stock.OutQty = stock.OutQty - v.Qty;
                                    await db.SaveChangesAsync();
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = costTypeAcc,
                                        CCCode = sin.CCCode,
                                        ChequeNo = "",
                                        Cr = dtl.CPrice * dtl.Qty,
                                        Dr = 0,
                                        Particulars = stock.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                        PCCode = cc.PCCode ?? 0,
                                        RefId = dtl.SINDtlId,
                                        SubId = 0
                                    });
                                    if (cc.PCCode != 72)
                                    {
                                        vrLst.Add(new VoucherDetailVM
                                        {
                                            AccId = clearingAcc,
                                            CCCode = sin.CCCode,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = dtl.CPrice * dtl.Qty,
                                            Particulars = stock.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                            PCCode = cc.PCCode ?? 0,
                                            RefId = dtl.SINDtlId,
                                            SubId = 0
                                        });

                                        vrLst.Add(new VoucherDetailVM
                                        {
                                            AccId = clearingAcc,
                                            CCCode = sin.CCCode,
                                            ChequeNo = "",
                                            Cr = dtl.CPrice * dtl.Qty,
                                            Dr = 0,
                                            Particulars = stock.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                            PCCode = 72,
                                            RefId = dtl.SINDtlId,
                                            SubId = 0
                                        });
                                    }
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = storeAcc,
                                        CCCode = sin.CCCode,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = dtl.CPrice * dtl.Qty,
                                        Particulars = stock.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                        PCCode = 72,
                                        RefId = dtl.SINDtlId,
                                        SubId = 0
                                    });
                                }
                                else
                                {
                                    scop.Dispose();
                                    result.Msg = "Already Return";
                                    return result;
                                }
                            }
                            else
                            {
                                scop.Dispose();
                                result.Msg = "Only Return Consumption SIN";
                                return result;
                            }
                        }
                    }

                    if (vrLst.Count == 0)
                    {
                        scop.Complete();
                    }
                    else
                    {
                        long vrId = await accountBL.PostAutoVoucher(vrLst, "SIR", mas.SINNo, mas.SINDate, UserId);
                        if (vrId > 0)
                        {
                            scop.Complete();
                        }
                    }
                    scop.Dispose();
                    result.TransId = mas.SINId;
                    result.Msg = "OK";
                    return result;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    result.Msg = "Error";
                    return result;
                }
            }
        }
        public async Task<List<ProGRNDetailVM>> PODetailLst(long POId)
        {
            try
            {
                return await db.Pro_PODetail.Where(x => x.Pro_PO.Status == "A" && x.POId == POId && (x.Qty - x.RecvQty) > 0).Select(x => new ProGRNDetailVM { ItemId = x.ItemId, Item = x.Pro_Item.Item, OrderQty = x.Qty - x.RecvQty, Qty = 0, PODtlId = x.PODtlId }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> GetGRNNo(string Type)
        {
            try
            {
                if(Type == "P")
                {
                    var docNo = await db.Pro_GRN.Where(x => x.Type == Type).OrderByDescending(x => x.GRNId).Select(x => x.GRNNo).FirstOrDefaultAsync();
                    string pre = "SSI-" + DateTime.Now.ToString("yyMM");
                    if (docNo != null)
                    {
                        if (docNo.Substring(0, 8) == pre)
                        {
                            docNo = pre + (Convert.ToInt32(docNo.Substring(8, 4)) + 1).ToString("0000");
                        }
                        else
                        {
                            docNo = pre + "0001";
                        }
                    }
                    else
                    {
                        docNo = pre + "0001";
                    }
                    return docNo;
                }
                else
                {
                    var docNo = await db.Pro_GRN.Where(x => x.Type == Type).OrderByDescending(x => x.GRNId).Select(x => x.GRNNo).FirstOrDefaultAsync();
                    string pre = "SSR-" + DateTime.Now.ToString("yyMM");
                    if (docNo != null)
                    {
                        if (docNo.Substring(0, 8) == pre)
                        {
                            docNo = pre + (Convert.ToInt32(docNo.Substring(8, 4)) + 1).ToString("0000");
                        }
                        else
                        {
                            docNo = pre + "0001";
                        }
                    }
                    else
                    {
                        docNo = pre + "0001";
                    }
                    return docNo;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public async Task<ResultVM> SaveGRN(IEnumerable<ProGRNDetailVM> mod, string RefInvNo, string RecvBy, long POId, int LocId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ResultVM result = new ResultVM();
                try
                {
                    var invNo = await GetGRNNo("P");
                    DateTime workingDate = setupBL.GetWorkingDate(LocId);
                    var po = await db.Pro_PO.FindAsync(POId);
                    var vrLst = new List<VoucherDetailVM>();
                    AccountBL accountBL = new AccountBL();
                    var storeAcc = await accountBL.GetAcc(471);
                    var suppAcc = await accountBL.GetAcc(472);

                    Pro_GRN mas = new Pro_GRN
                    {
                        GRNDate = workingDate,
                        GRNNo = invNo,
                        LocId = LocId,
                        POId = POId,
                        RecvBy = RecvBy,
                        RefInvNo = RefInvNo,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        SuppId = po.SuppId,
                        Type = "P"
                    };
                    db.Pro_GRN.Add(mas);
                    await db.SaveChangesAsync();

                    //List<Pro_GRNDetail> det = new List<Pro_GRNDetail>();
                    foreach (var v in mod)
                    {
                        if (v.Qty > 0)
                        {
                            var poDtl = await db.Pro_PODetail.FindAsync(v.PODtlId);
                            poDtl.RecvQty = poDtl.RecvQty + v.Qty;

                            var dtl = new Pro_GRNDetail
                            {
                                Disc = poDtl.Disc,
                                ItemId = v.ItemId,
                                Qty = v.Qty,
                                Rate = poDtl.Rate,
                                Tax = poDtl.Tax,
                                GRNId = mas.GRNId,
                                PODtlId = v.PODtlId
                            };
                            db.Pro_GRNDetail.Add(dtl);
                            await db.SaveChangesAsync();

                            if (po.NatureId != 3)
                            {
                                db.Pro_Stock.Add(new Pro_Stock
                                {
                                    CCode = LocId,
                                    DocType = "SSI",
                                    LocId = LocId,
                                    MFact = 1,
                                    Qty = v.Qty,
                                    RefDocId = dtl.GRNDtlId,
                                    TransDate = DateTime.Now,
                                    UserId = UserId,
                                    WorkingDate = workingDate,
                                    ItemId = v.ItemId,
                                    PPrice = (poDtl.Rate - poDtl.Disc),
                                    SuppId = po.SuppId,
                                    DocNo = mas.GRNNo
                                });
                                await db.SaveChangesAsync();

                                if ((poDtl.Rate - poDtl.Disc) > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = storeAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                        Particulars = "Stock Receive to Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                        PCCode = 72,
                                        RefId = v.ItemId,
                                        SubId = 0
                                    });
                                if ((poDtl.Rate - poDtl.Disc) > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = suppAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                        Dr = 0,
                                        Particulars = "Stock Receive to Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                        PCCode = 72,
                                        RefId = v.ItemId,
                                        SubId = po.SuppId
                                    });
                            }
                            else
                            {
                                var costType = await db.Fin_CostType.FindAsync(poDtl.CostTypeId);
                                long costTypeAcc = 0;
                                var clearingAcc = await accountBL.GetAcc(459);

                                if (LocId == 72)
                                {
                                    costTypeAcc = costType.RevenueHOGL ?? 0;
                                }
                                else
                                {
                                    costTypeAcc = costType.RevenueSHGL ?? 0;
                                }
                                if ((poDtl.Rate - poDtl.Disc) > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = costTypeAcc,
                                        CCCode = LocId,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                        Particulars = "Stock Receive to Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                        PCCode = 72,
                                        RefId = v.ItemId,
                                        SubId = 0
                                    });
                                if (LocId != 72)
                                {
                                    if ((poDtl.Rate - poDtl.Disc) > 0)
                                        vrLst.Add(new VoucherDetailVM
                                        {
                                            AccId = clearingAcc,
                                            CCCode = LocId,
                                            ChequeNo = "",
                                            Cr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                            Dr = 0,
                                            Particulars = "Stock Receive to Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                            PCCode = LocId,
                                            RefId = v.ItemId,
                                            SubId = 0
                                        });

                                    if ((poDtl.Rate - poDtl.Disc) > 0)
                                        vrLst.Add(new VoucherDetailVM
                                        {
                                            AccId = clearingAcc,
                                            CCCode = 72,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                            Particulars = "Stock Receive to Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                            PCCode = 72,
                                            RefId = v.ItemId,
                                            SubId = 0
                                        });
                                }

                                if ((poDtl.Rate - poDtl.Disc) > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = storeAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                        Dr = 0,
                                        Particulars = "Stock Receive to Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                        PCCode = 72,
                                        RefId = v.ItemId,
                                        SubId = 0
                                    });
                            }
                        }
                    }

                    var rem = await db.Pro_PODetail.Where(x => x.POId == POId).SumAsync(x => x.Qty - x.RecvQty);
                    if (rem == 0)
                    {
                        po.Status = "C";
                        await db.SaveChangesAsync();
                    }

                    if (vrLst.Count == 0) 
                    {
                        scop.Complete();
                    }
                    else
                    {
                        long vrId = await accountBL.PostAutoVoucher(vrLst, "SSI", mas.GRNNo, mas.GRNDate, UserId);
                        if (vrId > 0)
                        {
                            scop.Complete();
                        }
                    }
                    scop.Dispose();

                    result.TransId = mas.GRNId;
                    result.Msg = "OK";
                    return result;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    result.Msg = "Error";
                    return result;
                }
            }
        }
        #endregion
        #region SSR
        public async Task<ResultVM> SaveGRNReturn(IEnumerable<ProGRNDetailVM> mod, string RefInvNo, string RecvBy, string GRNNo, int LocId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ResultVM result = new ResultVM();
                try
                {
                    var invNo = await GetGRNNo("R");
                    DateTime workingDate = setupBL.GetWorkingDate(LocId);

                    var vrLst = new List<VoucherDetailVM>();
                    AccountBL accountBL = new AccountBL();
                    var storeAcc = await accountBL.GetAcc(471);
                    var suppAcc = await accountBL.GetAcc(472);

                    var grn = await db.Pro_GRN.Where(x => x.GRNNo == GRNNo).FirstOrDefaultAsync();
                    var po = await db.Pro_PO.FindAsync(grn.POId);

                    Pro_GRN mas = new Pro_GRN
                    {
                        GRNDate = workingDate,
                        GRNNo = invNo,
                        LocId = LocId,
                        POId = po.POId,
                        RecvBy = RecvBy,
                        RefInvNo = RefInvNo,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        SuppId = po.SuppId,
                        Type = "R",
                        ReturnId = grn.GRNId
                    };
                    db.Pro_GRN.Add(mas);
                    await db.SaveChangesAsync();

                    //List<Pro_GRNDetail> det = new List<Pro_GRNDetail>();
                    foreach (var v in mod)
                    {
                        if (v.Qty > 0)
                        {
                            var grnDtl = await db.Pro_GRNDetail.FindAsync(v.PODtlId);
                            var poDtl = await db.Pro_PODetail.FindAsync(grnDtl.PODtlId);
                            var dtl = new Pro_GRNDetail
                            {
                                Disc = poDtl.Disc,
                                ItemId = v.ItemId,
                                Qty = v.Qty,
                                Rate = poDtl.Rate,
                                Tax = poDtl.Tax,
                                GRNId = mas.GRNId,
                                PODtlId = poDtl.PODtlId
                            };
                            db.Pro_GRNDetail.Add(dtl);
                            await db.SaveChangesAsync();

                            if (po.NatureId != 3)
                            {
                                var store = await db.Pro_Stock.Where(x => x.RefDocId == v.PODtlId).FirstOrDefaultAsync();
                                store.OutQty = store.OutQty + v.Qty;
                                await db.SaveChangesAsync();

                                if ((poDtl.Rate - poDtl.Disc) > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = storeAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                        Dr = 0,
                                        Particulars = "Stock Return from Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                        PCCode = 72,
                                        RefId = v.ItemId,
                                        SubId = 0
                                    });

                                if ((poDtl.Rate - poDtl.Disc) > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = suppAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                        Particulars = "Stock Return from Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                        PCCode = 72,
                                        RefId = v.ItemId,
                                        SubId = po.SuppId
                                    });
                            }
                            else
                            {
                                var costType = await db.Fin_CostType.FindAsync(poDtl.CostTypeId);
                                long costTypeAcc = 0;
                                var clearingAcc = await accountBL.GetAcc(459);

                                if (LocId == 72)
                                {
                                    costTypeAcc = costType.RevenueHOGL ?? 0;
                                }
                                else
                                {
                                    costTypeAcc = costType.RevenueSHGL ?? 0;
                                }

                                if ((poDtl.Rate - poDtl.Disc) > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = costTypeAcc,
                                        CCCode = LocId,
                                        ChequeNo = "",
                                        Cr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                        Dr = 0,
                                        Particulars = "Stock Return from Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                        PCCode = 72,
                                        RefId = v.ItemId,
                                        SubId = 0
                                    });
                                if (LocId != 72)
                                {
                                    if ((poDtl.Rate - poDtl.Disc) > 0)
                                        vrLst.Add(new VoucherDetailVM
                                        {
                                            AccId = clearingAcc,
                                            CCCode = LocId,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                            Particulars = "Stock Return from Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                            PCCode = LocId,
                                            RefId = v.ItemId,
                                            SubId = 0
                                        });

                                    if ((poDtl.Rate - poDtl.Disc) > 0)
                                        vrLst.Add(new VoucherDetailVM
                                        {
                                            AccId = clearingAcc,
                                            CCCode = 72,
                                            ChequeNo = "",
                                            Cr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                            Dr = 0,
                                            Particulars = "Stock Return from Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                            PCCode = 72,
                                            RefId = v.ItemId,
                                            SubId = 0
                                        });
                                }

                                if ((poDtl.Rate - poDtl.Disc) > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = storeAcc,
                                        CCCode = 72,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = (poDtl.Rate - poDtl.Disc) * v.Qty,
                                        Particulars = "Stock Return from Store " + poDtl.Pro_Item.Item + " Qty " + v.Qty + " @" + (poDtl.Rate - poDtl.Disc),
                                        PCCode = 72,
                                        RefId = v.ItemId,
                                        SubId = 0
                                    });
                            }
                        }
                    }
                    if (vrLst.Count == 0)
                    {
                        scop.Complete();
                    }
                    else
                    {
                        long vrId = await accountBL.PostAutoVoucher(vrLst, "SSR", mas.GRNNo, mas.GRNDate, UserId);
                        if (vrId > 0)
                        {
                            scop.Complete();
                        }
                    }
                    scop.Dispose();
                    result.TransId = mas.GRNId;
                    result.Msg = "OK";
                    return result;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    result.Msg = "Error";
                    return result;
                }
            }
        }
        #endregion
        #region SIN
        public async Task<List<ItemVM>> ItemByLocList(int LocId)
        {
            try
            {
                return await (from S in db.Pro_Stock
                              join I in db.Pro_Item on S.ItemId equals I.ItemId
                              join B in db.Pro_ItemBrand on I.ItemBrandId equals B.ItemBrandId
                              join P in db.Pro_ItemProduct on I.ItemProductId equals P.ItemProductId
                              where S.LocId == LocId
                              group S by new { I.ItemId, I.ItemCode, I.Item, I.Spec, B.ItemBrand, P.ItemProduct } into G
                              where G.Sum(a => (a.Qty - a.OutQty)) > 0
                              select new ItemVM
                              {
                                  Item = G.Key.Item,
                                  ItemId = G.Key.ItemId,
                                  ItemBrand = G.Key.ItemBrand,
                                  ItemProduct = G.Key.ItemProduct,
                                  ItemCode = G.Key.ItemCode,
                                  Spec = G.Key.Spec,
                                  Qty = G.Sum(a => (a.Qty - a.OutQty))
                              }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ItemVM>> ItemStockByLocList(int LocId, int ItemId, string ItemType)
        {
            try
            {
                if (ItemType == "P")
                {
                    return await db.Pro_Stock.Where(x => x.LocId == LocId && x.ItemId == ItemId).GroupBy(x => x.ItemId).Where(x => x.Sum(a => (a.Qty * a.OutQty)) > 0).Select(x =>
                     new ItemVM
                     {
                         Item = x.FirstOrDefault().Pro_Item.Item,
                         ItemId = x.Key,
                         //ItemBrand = x.FirstOrDefault().Pro_Item.Pro_ItemBrand.ItemBrand,
                         //ItemProduct = x.FirstOrDefault().Pro_Item.Pro_ItemProduct.ItemProduct,
                         //ItemCode = x.FirstOrDefault().Pro_Item.ItemCode,
                         //Spec = x.FirstOrDefault().Pro_Item.Spec,
                         //Qty = x.Sum(a => (a.Qty * a.MFact))
                     }).ToListAsync();
                }
                else
                {
                    return await db.Inv_Store.Where(x => x.LocId == LocId && x.SKUId == ItemId && x.Inv_Status.MFact == 1).Select(x =>
                     new ItemVM
                     {
                         Item = x.Itm_Master.SKUCode,
                         ItemId = x.SKUId,
                         //ItemBrand = x.FirstOrDefault().Pro_Item.Pro_ItemBrand.ItemBrand,
                         //ItemProduct = x.FirstOrDefault().Pro_Item.Pro_ItemProduct.ItemProduct,
                         //ItemCode = x.FirstOrDefault().Pro_Item.ItemCode,
                         //Spec = x.FirstOrDefault().Pro_Item.Spec,
                         //Qty = x.Sum(a => (a.Qty * a.MFact))
                     }).ToListAsync();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> GetSINNo(string Type)
        {
            try
            {
                if(Type == "I")
                {
                    var docNo = await db.Pro_SIN.Where(x => x.TransType == Type).OrderByDescending(x => x.SINId).Select(x => x.SINNo).FirstOrDefaultAsync();
                    string pre = "SIN-" + DateTime.Now.ToString("yyMM");
                    if (docNo != null)
                    {
                        if (docNo.Substring(0, 8) == pre)
                        {
                            docNo = pre + (Convert.ToInt32(docNo.Substring(8, 4)) + 1).ToString("0000");
                        }
                        else
                        {
                            docNo = pre + "0001";
                        }
                    }
                    else
                    {
                        docNo = pre + "0001";
                    }
                    return docNo;
                }
                else
                {
                    var docNo = await db.Pro_SIN.Where(x => x.TransType == Type).OrderByDescending(x => x.SINId).Select(x => x.SINNo).FirstOrDefaultAsync();
                    string pre = "SIR-" + DateTime.Now.ToString("yyMM");
                    if (docNo != null)
                    {
                        if (docNo.Substring(0, 8) == pre)
                        {
                            docNo = pre + (Convert.ToInt32(docNo.Substring(8, 4)) + 1).ToString("0000");
                        }
                        else
                        {
                            docNo = pre + "0001";
                        }
                    }
                    else
                    {
                        docNo = pre + "0001";
                    }
                    return docNo;
                }
                
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public async Task<ResultVM> SaveSIN(IEnumerable<SINDetailVM> mod, int CCCode, long MTRId, int LocId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ResultVM result = new ResultVM();
                try
                {
                    var invNo = await GetSINNo("I");
                    DateTime workingDate = setupBL.GetWorkingDate(LocId);
                    var cc = await db.Fin_CostCenters.FindAsync(CCCode);
                    Pro_SIN mas = new Pro_SIN
                    {
                        SINDate = workingDate,
                        SINNo = invNo,
                        ToLocId = cc.PCCode ?? 0,
                        Type = "C",
                        CCCode = CCCode,
                        FromLocId = LocId,
                        MTRId = MTRId,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        TransType = "I"
                    };

                    db.Pro_SIN.Add(mas);
                    await db.SaveChangesAsync();

                    List<Pro_SINDetail> det = new List<Pro_SINDetail>();
                    var vrLst = new List<VoucherDetailVM>();
                    AccountBL accountBL = new AccountBL();
                    foreach (var v in mod)
                    {
                        if (v.Qty > 0)
                        {
                            if (v.ItemType == "P")
                            {
                                var mtr = await db.Pro_MTRDetail.FindAsync(v.SINId);
                                mtr.RecvQty = v.Qty + (mtr.RecvQty ?? 0);
                                var costType = await db.Fin_CostType.FindAsync(v.CostTypeId);
                                long costTypeAcc = 0;
                                var clearingAcc = await accountBL.GetAcc(459);
                                var storeAcc = await accountBL.GetAcc(471);
                                if (cc.PCCode == 72)
                                {
                                    costTypeAcc = costType.RevenueHOGL ?? 0;
                                }
                                else
                                {
                                    costTypeAcc = costType.RevenueSHGL ?? 0;
                                }
                                var stock = await db.Pro_Stock.Where(x => x.LocId == LocId && x.ItemId == v.ItemId && x.Qty > x.OutQty).OrderBy(x => x.RowId).ToListAsync();
                                if (stock.Sum(x => x.Qty - x.OutQty) >= mod.Where(x => x.ItemId == v.ItemId).Sum(x => x.Qty))
                                {
                                    decimal qty = v.Qty;
                                    foreach (var item in stock)
                                    {
                                        var dtl = new Pro_SINDetail();
                                        if (item.Qty - item.OutQty < qty)
                                        {
                                            dtl = new Pro_SINDetail
                                            {
                                                CPrice = item.PPrice,
                                                ItemId = v.ItemId,
                                                Qty = item.Qty - item.OutQty,
                                                ItemType = v.ItemType,
                                                SerialNo = "",
                                                StoreId = item.RowId,
                                                CostTypeId = v.CostTypeId,
                                                SINId = mas.SINId
                                            };
                                            db.Pro_SINDetail.Add(dtl);
                                            item.OutQty = item.Qty;
                                            qty = qty - (item.Qty - item.OutQty);
                                            await db.SaveChangesAsync();
                                            vrLst.Add(new VoucherDetailVM
                                            {
                                                AccId = costTypeAcc,
                                                CCCode = CCCode,
                                                ChequeNo = "",
                                                Cr = 0,
                                                Dr = dtl.CPrice * dtl.Qty,
                                                Particulars = item.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                                PCCode = cc.PCCode ?? 0,
                                                RefId = dtl.SINDtlId,
                                                SubId = 0
                                            });
                                            if (cc.PCCode != 72)
                                            {
                                                vrLst.Add(new VoucherDetailVM
                                                {
                                                    AccId = clearingAcc,
                                                    CCCode = CCCode,
                                                    ChequeNo = "",
                                                    Cr = dtl.CPrice * dtl.Qty,
                                                    Dr = 0,
                                                    Particulars = item.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                                    PCCode = cc.PCCode ?? 0,
                                                    RefId = dtl.SINDtlId,
                                                    SubId = 0
                                                });

                                                vrLst.Add(new VoucherDetailVM
                                                {
                                                    AccId = clearingAcc,
                                                    CCCode = CCCode,
                                                    ChequeNo = "",
                                                    Cr = 0,
                                                    Dr = dtl.CPrice * dtl.Qty,
                                                    Particulars = item.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                                    PCCode = 72,
                                                    RefId = dtl.SINDtlId,
                                                    SubId = 0
                                                });
                                            }
                                            vrLst.Add(new VoucherDetailVM
                                            {
                                                AccId = storeAcc,
                                                CCCode = CCCode,
                                                ChequeNo = "",
                                                Cr = dtl.CPrice * dtl.Qty,
                                                Dr = 0,
                                                Particulars = item.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                                PCCode = 72,
                                                RefId = dtl.SINDtlId,
                                                SubId = 0
                                            });
                                        }
                                        else
                                        {
                                            //qty = item.Qty - item.OutQty;
                                            item.OutQty = item.OutQty + qty;
                                            dtl = new Pro_SINDetail
                                            {
                                                CPrice = item.PPrice,
                                                ItemId = v.ItemId,
                                                Qty = qty,
                                                ItemType = v.ItemType,
                                                SerialNo = "",
                                                StoreId = item.RowId,
                                                CostTypeId = v.CostTypeId,
                                                SINId = mas.SINId,
                                                RtnQty = 0
                                            };
                                            db.Pro_SINDetail.Add(dtl);
                                            await db.SaveChangesAsync();

                                            if(dtl.CPrice > 0)
                                            vrLst.Add(new VoucherDetailVM
                                            {
                                                AccId = costTypeAcc,
                                                CCCode = CCCode,
                                                ChequeNo = "",
                                                Cr = 0,
                                                Dr = dtl.CPrice * dtl.Qty,
                                                Particulars = item.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                                PCCode = cc.PCCode ?? 0,
                                                RefId = dtl.SINDtlId,
                                                SubId = 0
                                            });
                                            if (cc.PCCode != 72)
                                            {
                                                if (dtl.CPrice > 0)
                                                    vrLst.Add(new VoucherDetailVM
                                                {
                                                    AccId = clearingAcc,
                                                    CCCode = CCCode,
                                                    ChequeNo = "",
                                                    Cr = dtl.CPrice * dtl.Qty,
                                                    Dr = 0,
                                                    Particulars = item.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                                    PCCode = cc.PCCode ?? 0,
                                                    RefId = dtl.SINDtlId,
                                                    SubId = 0
                                                });

                                                if (dtl.CPrice > 0)
                                                    vrLst.Add(new VoucherDetailVM
                                                {
                                                    AccId = clearingAcc,
                                                    CCCode = CCCode,
                                                    ChequeNo = "",
                                                    Cr = 0,
                                                    Dr = dtl.CPrice * dtl.Qty,
                                                    Particulars = item.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                                    PCCode = 72,
                                                    RefId = dtl.SINDtlId,
                                                    SubId = 0
                                                });
                                            }

                                            if (dtl.CPrice > 0)
                                                vrLst.Add(new VoucherDetailVM
                                            {
                                                AccId = storeAcc,
                                                CCCode = CCCode,
                                                ChequeNo = "",
                                                Cr = dtl.CPrice * dtl.Qty,
                                                Dr = 0,
                                                Particulars = item.Pro_Item.Item + " Qty" + dtl.Qty + " @" + dtl.CPrice,
                                                PCCode = 72,
                                                RefId = dtl.SINDtlId,
                                                SubId = 0
                                            });
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }
                    }

                    var rem = await db.Pro_MTRDetail.Where(x => x.MTRId == MTRId).SumAsync(x => x.Qty - (x.RecvQty ?? 0));
                    if (rem == 0)
                    {
                        var mtr = await db.Pro_MTR.FindAsync(MTRId);
                        mtr.Status = "C";
                        await db.SaveChangesAsync();
                    }

                    if (vrLst.Count == 0)
                    {
                        scop.Complete();
                    }
                    else
                    {
                        long vrId = await accountBL.PostAutoVoucher(vrLst, "SIN", mas.SINNo, mas.SINDate, UserId);
                        if (vrId > 0)
                        {
                            scop.Complete();
                        }
                    }
                    scop.Dispose();
                    result.TransId = mas.SINId;
                    result.Msg = "OK";
                    return result;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    result.Msg = "Error";
                    return result;
                }
            }
        }

        public async Task<ResultVM> SaveSINFar(IEnumerable<SINDetailVM> mod, int CCCode, long MTRId, int LocId, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                ResultVM result = new ResultVM();
                try
                {
                    var invNo = await GetSINNo("I");
                    DateTime workingDate = setupBL.GetWorkingDate(LocId);
                    var cc = await db.Fin_CostCenters.FindAsync(CCCode);
                    Pro_SIN mas = new Pro_SIN
                    {
                        SINDate = workingDate,
                        SINNo = invNo,
                        ToLocId = cc.PCCode ?? 0,
                        Type = "F",
                        CCCode = CCCode,
                        FromLocId = LocId,
                        MTRId = MTRId,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        TransType = "I"
                    };
                    db.Pro_SIN.Add(mas);
                    await db.SaveChangesAsync();

                    List<Pro_SINDetail> det = new List<Pro_SINDetail>();
                    var vrLst = new List<VoucherDetailVM>();
                    AccountBL accountBL = new AccountBL();
                    foreach (var v in mod)
                    {
                        var mtr = await db.Pro_MTRDetail.FindAsync(v.SINId);
                        mtr.RecvQty = v.Qty + (mtr.RecvQty ?? 0);
                        if (v.ItemType == "P")
                        {
                            var stock = await db.Pro_Stock.Where(x => x.LocId == LocId && x.ItemId == v.ItemId && x.Qty > x.OutQty).OrderBy(x => x.RowId).FirstOrDefaultAsync();
                            if (stock != null)
                            {
                                stock.OutQty = stock.OutQty + 1;
                                var dtl = new Pro_SINDetail
                                {
                                    CPrice = stock.PPrice,
                                    ItemId = v.ItemId,
                                    Qty = v.Qty,
                                    ItemType = v.ItemType,
                                    SerialNo = v.SerialNo,
                                    StoreId = stock.RowId,
                                    CostTypeId = v.CostTypeId,
                                    SINId = mas.SINId,
                                    RtnQty = 0
                                };
                                db.Pro_SINDetail.Add(dtl);


                                db.Far_Store.Add(new Far_Store
                                {
                                    CCCode = CCCode,
                                    Condition = "New",
                                    CurrentValue = stock.PPrice,
                                    DepreciationMethod = 0,
                                    DepreciationPercent = 0,
                                    EmpId = 0,
                                    InstallationDate = DateTime.Now,
                                    ItemId = v.ItemId,
                                    ItemType = v.ItemType,
                                    LocId = cc.PCCode ?? 0,
                                    PPrice = stock.PPrice,
                                    PurchaseDate = stock.TransDate,
                                    Remarks = "",
                                    SerialNo = v.SerialNo,
                                    Status = 1,
                                    SuppId = stock.SuppId,
                                    TransDate = DateTime.Now,
                                    UserId = UserId,
                                    CostTypeId = v.CostTypeId
                                });
                                await db.SaveChangesAsync();

                                var costType = await db.Fin_CostType.FindAsync(v.CostTypeId);
                                long costTypeAcc = 0;
                                var storeAcc = await accountBL.GetAcc(471);
                                if (cc.PCCode == 72)
                                {
                                    costTypeAcc = costType.CapitalHOGL ?? 0;
                                }
                                else
                                {
                                    costTypeAcc = costType.CapitalSHGL ?? 0;
                                }
                                if (stock.PPrice > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = costTypeAcc,
                                        CCCode = CCCode,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = stock.PPrice,
                                        Particulars = "Transfer to Fixed Asset " + v.SerialNo,
                                        PCCode = 72,
                                        RefId = dtl.SINDtlId,
                                        SubId = 0
                                    });
                                if (stock.PPrice > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = storeAcc,
                                        CCCode = CCCode,
                                        ChequeNo = "",
                                        Cr = stock.PPrice,
                                        Dr = 0,
                                        Particulars = "Transfer to Fixed Asset " + v.SerialNo,
                                        PCCode = stock.LocId,
                                        RefId = dtl.SINDtlId,
                                        SubId = 0
                                    });
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            var stock = await db.Inv_Store.Where(x => x.LocId == LocId && x.SKUId == v.ItemId && x.SerialNo == v.CSerialNo && x.Inv_Status.MFact == 1).FirstOrDefaultAsync();
                            if (stock != null)
                            {
                                stock.StatusID = 13;
                                Inv_StoreHistory tbl = new Inv_StoreHistory
                                {
                                    DocDate = mas.SINDate,
                                    ItemId = stock.ItemId,
                                    LocId = stock.LocId,
                                    MFact = -1,
                                    MRP = stock.MRP,
                                    PPrice = stock.PPrice,
                                    Qty = 1,
                                    SKUId = stock.SKUId,
                                    SMPrice = 0,
                                    SPrice = 0,
                                    TransDate = mas.TransDate,
                                    Type = "Transfer To Asset",
                                    UserId = UserId,
                                    RefId = mas.SINId
                                };
                                db.Inv_StoreHistory.Add(tbl);
                                //stock.OutQty = stock.OutQty + 1;
                                var dtl = new Pro_SINDetail
                                {
                                    CPrice = stock.PPrice,
                                    ItemId = v.ItemId,
                                    Qty = v.Qty,
                                    ItemType = v.ItemType,
                                    SerialNo = v.SerialNo,
                                    StoreId = stock.ItemId,
                                    CostTypeId = v.CostTypeId,
                                    SINId = mas.SINId,
                                    RtnQty = 0
                                };
                                db.Pro_SINDetail.Add(dtl);

                                db.Far_Store.Add(new Far_Store
                                {
                                    CCCode = CCCode,
                                    Condition = "New",
                                    CurrentValue = stock.PPrice,
                                    DepreciationMethod = 0,
                                    DepreciationPercent = 0,
                                    EmpId = 0,
                                    InstallationDate = DateTime.Now,
                                    ItemId = v.ItemId,
                                    ItemType = v.ItemType,
                                    LocId = cc.PCCode ?? 0,
                                    PPrice = stock.PPrice,
                                    PurchaseDate = stock.TrxDate ?? DateTime.Now,
                                    Remarks = "",
                                    SerialNo = v.SerialNo,
                                    CSerialNo = v.CSerialNo,
                                    InvId = stock.ItemId,
                                    Status = 1,
                                    SuppId = stock.SuppId ?? 0,
                                    TransDate = DateTime.Now,
                                    UserId = UserId,
                                    CostTypeId = v.CostTypeId
                                });
                                await db.SaveChangesAsync();

                                var costType = await db.Fin_CostType.FindAsync(v.CostTypeId);
                                long costTypeAcc = 0;
                                var invAcc = await accountBL.GetAcc(7, stock.Inv_Suppliers.CategoryId);
                                var invClr = await accountBL.GetAcc(448);
                                if (cc.PCCode == 72)
                                {
                                    costTypeAcc = costType.CapitalHOGL ?? 0;
                                }
                                else
                                {
                                    costTypeAcc = costType.CapitalSHGL ?? 0;
                                }
                                if (stock.PPrice > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = costTypeAcc,
                                        CCCode = CCCode,
                                        ChequeNo = "",
                                        Cr = 0,
                                        Dr = stock.PPrice,
                                        Particulars = "Transfer to Fixed Asset " + v.SerialNo + " (" + v.CSerialNo + ")",
                                        PCCode = 72,
                                        RefId = dtl.SINDtlId,
                                        SubId = 0
                                    });
                                if (stock.LocId != 72)
                                {
                                    if (stock.PPrice > 0)
                                        vrLst.Add(new VoucherDetailVM
                                        {
                                            AccId = invClr,
                                            CCCode = CCCode,
                                            ChequeNo = "",
                                            Cr = stock.PPrice,
                                            Dr = 0,
                                            Particulars = "Transfer to Fixed Asset " + v.SerialNo + " (" + v.CSerialNo + ")",
                                            PCCode = 72,
                                            RefId = dtl.SINDtlId,
                                            SubId = 0
                                        });

                                    if (stock.PPrice > 0)
                                        vrLst.Add(new VoucherDetailVM
                                        {
                                            AccId = invClr,
                                            CCCode = CCCode,
                                            ChequeNo = "",
                                            Cr = 0,
                                            Dr = stock.PPrice,
                                            Particulars = "Transfer to Fixed Asset " + v.SerialNo + " (" + v.CSerialNo + ")",
                                            PCCode = stock.LocId,
                                            RefId = dtl.SINDtlId,
                                            SubId = 0
                                        });
                                }
                                if (stock.PPrice > 0)
                                    vrLst.Add(new VoucherDetailVM
                                    {
                                        AccId = invAcc,
                                        CCCode = CCCode,
                                        ChequeNo = "",
                                        Cr = stock.PPrice,
                                        Dr = 0,
                                        Particulars = "Transfer to Fixed Asset " + v.SerialNo + " (" + v.CSerialNo + ")",
                                        PCCode = stock.LocId,
                                        RefId = dtl.SINDtlId,
                                        SubId = 0
                                    });
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    var rem = await db.Pro_MTRDetail.Where(x => x.MTRId == MTRId).SumAsync(x => x.Qty - (x.RecvQty ?? 0));
                    if (rem == 0)
                    {
                        var mtr = await db.Pro_MTR.FindAsync(MTRId);
                        mtr.Status = "C";
                        await db.SaveChangesAsync();
                    }
                    if (vrLst.Count == 0)
                    {
                        scop.Complete();
                    }
                    else
                    {
                        long vrId = await accountBL.PostAutoVoucher(vrLst, "SIN", mas.SINNo, mas.SINDate, UserId);
                        if (vrId > 0)
                        {
                            scop.Complete();
                        }
                    }
                    scop.Dispose();
                    result.TransId = mas.SINId;
                    result.Msg = "OK";
                    return result;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    result.Msg = "Error";
                    return result;
                }
            }
        }
        #endregion
        #region GeneralFunctions

        public async Task<List<UOMVM>> UOMList()
        {
            try
            {
                return await db.Itm_UOM.Select(x =>
                new UOMVM
                {
                    UOMId = x.UOMId,
                    UOM = x.UOM
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> StockOpeningCreate(List<FarOpeningAVM> model)
        {
            try
            {
                foreach (var item in model)
                {
                    var invstoremodel = new Far_Opening()
                    {
                        LocId = item.LocId,
                        PPrice = 0,
                        Remarks = item.Remarks,
                        SerialNo = item.SerialNo,
                        TransDate = DateTime.Now,
                        UserId = item.UserId,
                        CCCode = item.LocId,
                        Condition = "",
                        CurrentValue = 0,
                        DepreciationMethod = 0,
                        DepreciationPercent = 0,
                        DocDate = DateTime.Now.Date,
                        EmpId = 0,
                        InstallationDate = DateTime.Now,
                        PurchaseDate = DateTime.Now,
                        ItemId = item.ItemId,
                        Status = 1,
                        StoreId = 0,
                        SuppId = 0,
                        ItemType = item.ItemType,
                        CostTypeId = 0,
                        CSerialNo = item.CSerialNo
                    };
                    db.Far_Opening.Add(invstoremodel);
                    await db.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(item.UploadedFiles))
                    {
                        string file = item.UploadedFiles;
                        file = file.Replace("data:image/jpeg;base64,", "");
                        var imgByte = Convert.FromBase64String(file);
                        string sourcePath = "";
                        string newGuid = invstoremodel.DocId.ToString();
                        sourcePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/FarImg"), newGuid + ".jpg");
                        System.IO.File.WriteAllBytes(sourcePath, imgByte);
                    }
                    item.DocId = invstoremodel.DocId;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<FarDepreciationVM>> CalcDepreciation()
        {
            try
            {
                var lst = await (from m in db.Far_Store
                                 join S in db.Itm_Master on m.ItemId equals S.SKUId
                                 join CT in db.Fin_CostType on m.CostTypeId equals CT.CostTypeId
                                 //join SU in db.Inv_Suppliers on m.SuppId equals SU.SuppId
                                 join CC in db.Fin_CostCenters on m.CCCode equals CC.CCCode
                                 //join L in db.Comp_Locations on m.LocId equals L.LocId
                                 //join e in db.Pay_EmpMaster on m.EmpId equals e.EmpId into g
                                 //from e in g.DefaultIfEmpty()
                                 where m.ItemType == "S" && m.Status == 1
                                 select new FarDepreciationVM
                                 {
                                     CCCode = m.CCCode,
                                     ItemName = S.SKUCode,
                                     SerialNo = m.SerialNo,
                                     StoreId = m.StoreId,
                                     CCCodeDesc = CC.CCDesc,
                                     PCCode = CC.PCCode ?? 0,
                                     CostType = CT.CostType,
                                     CurrentValue = m.CurrentValue,
                                     PPrice = m.PPrice,
                                     CostTypeId = m.CostTypeId,
                                     DeprAmt = Math.Round((m.PPrice * CT.DeprRate / 100) / 12)
                                 }).ToListAsync();
                lst.AddRange(await (from m in db.Far_Store
                                    join S in db.Pro_Item on m.ItemId equals S.ItemId
                                    join CT in db.Fin_CostType on m.CostTypeId equals CT.CostTypeId
                                    //join SU in db.Fin_Subsidary on m.SuppId equals SU.SubId
                                    join CC in db.Fin_CostCenters on m.CCCode equals CC.CCCode
                                    //join L in db.Comp_Locations on m.LocId equals L.LocId
                                    //join e in db.Pay_EmpMaster on m.EmpId equals e.EmpId into g
                                    //from e in g.DefaultIfEmpty()
                                    where m.ItemType == "P" && m.Status == 1
                                    select new FarDepreciationVM
                                    {
                                        CCCode = m.CCCode,
                                        ItemName = S.Item,
                                        SerialNo = m.SerialNo,
                                        StoreId = m.StoreId,
                                        CCCodeDesc = CC.CCDesc,
                                        CostType = CT.CostType,
                                        CurrentValue = m.CurrentValue,
                                        PPrice = m.PPrice,
                                        CostTypeId = m.CostTypeId,
                                        PCCode = CC.PCCode ?? 0,
                                        DeprAmt = Math.Round((m.PPrice * CT.DeprRate / 100) / 12)
                                    }).ToListAsync());
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> PostDepreciation(int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    AccountBL accountBL = new AccountBL();
                    var lst = await CalcDepreciation();
                    var vrLst = new List<VoucherDetailVM>();
                    var clearingAcc = await accountBL.GetAcc(459);
                    DateTime Month = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    foreach (var v in lst)
                    {
                        var isExist = await db.Far_Depreciation.Where(x => x.StoreId == v.StoreId && x.Month.Month == Month.Month && x.Month.Year == Month.Year).AnyAsync();
                        if (isExist)
                        {
                            continue;
                        }
                        var store = await db.Far_Store.FindAsync(v.StoreId);
                        store.CurrentValue = store.CurrentValue - v.DeprAmt;

                        v.DeprAmt = decimal.Round(v.DeprAmt, 2, MidpointRounding.AwayFromZero);
                        store.CurrentValue = decimal.Round(store.CurrentValue, 2, MidpointRounding.AwayFromZero);
                        var dpr = new Far_Depreciation()
                        {
                            CCCode = v.CCCode,
                            DeprAmt = v.DeprAmt,
                            Month = Month,
                            StoreId = v.StoreId,
                            TransDate = DateTime.Now,
                            UserId = UserId
                        };
                        db.Far_Depreciation.Add(dpr);
                        await db.SaveChangesAsync();

                        var costType = await db.Fin_CostType.FindAsync(v.CostTypeId);
                        long deprAcc = 0;
                        long AccDeprAcc = 0;

                        AccDeprAcc = costType.AccDeprHOGL ?? 0;
                        if (v.PCCode == 72)
                        {
                            deprAcc = costType.DeprHOGL ?? 0;

                        }
                        else
                        {
                            deprAcc = costType.DeprSHGL ?? 0;
                            //AccDeprAcc = costType.AccDeprSHGL ?? 0;
                        }
                        if (v.DeprAmt > 0)
                            vrLst.Add(new VoucherDetailVM
                            {
                                AccId = deprAcc,
                                CCCode = v.CCCode,
                                ChequeNo = "",
                                Cr = 0,
                                Dr = v.DeprAmt,
                                Particulars = "Depreciation FMO-" + Month.ToString("MMM-yyyy") + " " + v.SerialNo,
                                PCCode = v.PCCode,
                                RefId = dpr.RowId,
                                SubId = 0
                            });
                        if (v.PCCode != 72)
                        {
                            if (v.DeprAmt > 0)
                                vrLst.Add(new VoucherDetailVM
                                {
                                    AccId = clearingAcc,
                                    CCCode = v.CCCode,
                                    ChequeNo = "",
                                    Cr = v.DeprAmt,
                                    Dr = 0,
                                    Particulars = "Depreciation FMO-" + Month.ToString("MMM-yyyy") + " " + v.SerialNo,
                                    PCCode = v.PCCode,
                                    RefId = dpr.RowId,
                                    SubId = 0
                                });

                            if (v.DeprAmt > 0)
                                vrLst.Add(new VoucherDetailVM
                                {
                                    AccId = clearingAcc,
                                    CCCode = 72,
                                    ChequeNo = "",
                                    Cr = 0,
                                    Dr = v.DeprAmt,
                                    Particulars = "Depreciation FMO-" + Month.ToString("MMM-yyyy") + " " + v.SerialNo,
                                    PCCode = 72,
                                    RefId = dpr.RowId,
                                    SubId = 0
                                });
                        }

                        if (v.DeprAmt > 0)
                            vrLst.Add(new VoucherDetailVM
                            {
                                AccId = AccDeprAcc,
                                CCCode = 72,
                                ChequeNo = "",
                                Cr = v.DeprAmt,
                                Dr = 0,
                                Particulars = "Depreciation FMO-" + Month.ToString("MMM-yyyy") + " " + v.SerialNo,
                                PCCode = 72,
                                RefId = dpr.RowId,
                                SubId = 0
                            });

                    }
                    if (vrLst.Count > 0)
                    {
                        var vrId = await accountBL.PostAutoVoucher(vrLst, "DPR", "", Month, UserId);
                        if (vrId > 0)
                        {
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
                    return false;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }
        public async Task<List<FarStoreVM>> GetAsset()
        {
            try
            {
                var lst = await (from m in db.Far_Store
                                 join S in db.Itm_Master on m.ItemId equals S.SKUId
                                 join SU in db.Inv_Suppliers on m.SuppId equals SU.SuppId
                                 join CC in db.Fin_CostCenters on m.CCCode equals CC.CCCode
                                 join L in db.Comp_Locations on m.LocId equals L.LocId
                                 join e in db.Pay_EmpMaster on m.EmpId equals e.EmpId into g
                                 from e in g.DefaultIfEmpty() 
                                 where m.ItemType == "S"
                                 select new FarStoreVM
                                 {
                                     CCCode = m.CCCode,
                                     Condition = m.Condition,
                                     ItemName = S.SKUCode,
                                     CurrentValue = m.CurrentValue,
                                     DepreciationMethod = m.DepreciationMethod,
                                     DepreciationPercent = m.DepreciationPercent,
                                     EmpId = m.EmpId,
                                     InstallationDate = m.InstallationDate,
                                     ItemId = m.ItemId,
                                     PPrice = m.PPrice,
                                     PurchaseDate = m.PurchaseDate,
                                     Remarks = m.Remarks,
                                     SerialNo = m.SerialNo,
                                     Status = m.Status,
                                     UserId = m.UserId,
                                     SuppId = m.SuppId,
                                     StoreId = m.StoreId,
                                     ItemType = m.ItemType,
                                     CCCodeDesc = CC.CCDesc,
                                     EmpName = e.EmpName ?? "",
                                     LocId = m.LocId,
                                     LocName = L.LocName,
                                     SuppName = SU.SuppName,
                                     TransDate = m.TransDate
                                 }).ToListAsync();
                lst.AddRange(await (from m in db.Far_Store
                                    join S in db.Pro_Item on m.ItemId equals S.ItemId
                                    join SU in db.Fin_Subsidary on m.SuppId equals SU.SubId
                                    join CC in db.Fin_CostCenters on m.CCCode equals CC.CCCode
                                    join L in db.Comp_Locations on m.LocId equals L.LocId
                                    join e in db.Pay_EmpMaster on m.EmpId equals e.EmpId into g
                                    from e in g.DefaultIfEmpty()
                                    where m.ItemType == "P"
                                    select new FarStoreVM
                                    {
                                        CCCode = m.CCCode,
                                        Condition = m.Condition,
                                        ItemName = S.Item,
                                        CurrentValue = m.CurrentValue,
                                        DepreciationMethod = m.DepreciationMethod,
                                        DepreciationPercent = m.DepreciationPercent,
                                        EmpId = m.EmpId,
                                        InstallationDate = m.InstallationDate,
                                        ItemId = m.ItemId,
                                        PPrice = m.PPrice,
                                        PurchaseDate = m.PurchaseDate,
                                        Remarks = m.Remarks,
                                        SerialNo = m.SerialNo,
                                        Status = m.Status,
                                        UserId = m.UserId,
                                        SuppId = m.SuppId,
                                        StoreId = m.StoreId,
                                        ItemType = m.ItemType,
                                        CCCodeDesc = CC.CCDesc,
                                        EmpName = e.EmpName ?? "",
                                        LocId = m.LocId,
                                        LocName = L.LocName,
                                        SuppName = SU.SubsidaryName,
                                        TransDate = m.TransDate
                                    }).ToListAsync());
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> SaveAssetAllocation(FarStoreVM mod, int UserId)
        {
            try
            {
                var store = await db.Far_Store.FindAsync(mod.StoreId);
                Far_Allocation allo = new Far_Allocation()
                {
                    FromCCCode = store.CCCode,
                    FromEmpId = store.EmpId,
                    StoreId = store.StoreId,
                    ToCCCode = mod.ToCCCode,
                    ToEmpId = mod.ToEmpId,
                    TransDate = DateTime.Now,
                    UserId = UserId,
                    WorkingDate = DateTime.Now.Date
                };
                db.Far_Allocation.Add(allo);
                store.CCCode = mod.ToCCCode;
                store.EmpId = mod.ToEmpId;
                await db.SaveChangesAsync();
                return "OK";
            }
            catch (Exception)
            {
                return "Error";
            }
        }
        public async Task<FarStoreVM> GetAsset(long Id)
        {
            try
            {
                var lst = await (from m in db.Far_Store
                                 join S in db.Itm_Master on m.ItemId equals S.SKUId
                                 join SU in db.Inv_Suppliers on m.SuppId equals SU.SuppId
                                 join CC in db.Fin_CostCenters on m.CCCode equals CC.CCCode
                                 join L in db.Comp_Locations on m.LocId equals L.LocId
                                 join e in db.Pay_EmpMaster on m.EmpId equals e.EmpId into g
                                 from e in g.DefaultIfEmpty()
                                 where m.ItemType == "S" && m.StoreId == Id
                                 select new FarStoreVM
                                 {
                                     CCCode = m.CCCode,
                                     Condition = m.Condition,
                                     ItemName = S.SKUCode,
                                     CurrentValue = m.CurrentValue,
                                     DepreciationMethod = m.DepreciationMethod,
                                     DepreciationPercent = m.DepreciationPercent,
                                     EmpId = m.EmpId,
                                     InstallationDate = m.InstallationDate,
                                     ItemId = m.ItemId,
                                     PPrice = m.PPrice,
                                     PurchaseDate = m.PurchaseDate,
                                     Remarks = m.Remarks,
                                     SerialNo = m.SerialNo,
                                     Status = m.Status,
                                     UserId = m.UserId,
                                     SuppId = m.SuppId,
                                     StoreId = m.StoreId,
                                     ItemType = m.ItemType,
                                     CCCodeDesc = CC.CCDesc,
                                     EmpName = e.EmpName ?? "",
                                     LocId = m.LocId,
                                     LocName = L.LocName,
                                     SuppName = SU.SuppName,
                                     TransDate = m.TransDate,
                                     ToCCCode = m.CCCode,
                                     ToEmpId = m.EmpId,
                                     CSerialNo = m.CSerialNo
                                 }).ToListAsync();
                lst.AddRange(await (from m in db.Far_Store
                                    join S in db.Pro_Item on m.ItemId equals S.ItemId
                                    join SU in db.Fin_Subsidary on m.SuppId equals SU.SubId
                                    join CC in db.Fin_CostCenters on m.CCCode equals CC.CCCode
                                    join L in db.Comp_Locations on m.LocId equals L.LocId
                                    join e in db.Pay_EmpMaster on m.EmpId equals e.EmpId into g
                                    from e in g.DefaultIfEmpty()
                                    where m.ItemType == "P" && m.StoreId == Id
                                    select new FarStoreVM
                                    {
                                        CCCode = m.CCCode,
                                        Condition = m.Condition,
                                        ItemName = S.Item,
                                        CurrentValue = m.CurrentValue,
                                        DepreciationMethod = m.DepreciationMethod,
                                        DepreciationPercent = m.DepreciationPercent,
                                        EmpId = m.EmpId,
                                        InstallationDate = m.InstallationDate,
                                        ItemId = m.ItemId,
                                        PPrice = m.PPrice,
                                        PurchaseDate = m.PurchaseDate,
                                        Remarks = m.Remarks,
                                        SerialNo = m.SerialNo,
                                        Status = m.Status,
                                        UserId = m.UserId,
                                        SuppId = m.SuppId,
                                        StoreId = m.StoreId,
                                        ItemType = m.ItemType,
                                        CCCodeDesc = CC.CCDesc,
                                        EmpName = e.EmpName ?? "",
                                        LocId = m.LocId,
                                        LocName = L.LocName,
                                        SuppName = SU.SubsidaryName,
                                        TransDate = m.TransDate,
                                        ToCCCode = m.CCCode,
                                        ToEmpId = m.EmpId,
                                        CSerialNo = m.CSerialNo
                                    }).ToListAsync());
                return lst.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<FarOpeningVM>> GetStockOpening(int LocId)
        {
            try
            {
                var lst = await (from m in db.Far_Opening
                                 join S in db.Itm_Master on m.ItemId equals S.SKUId
                                 where m.LocId == LocId && m.ItemType == "S" && m.Status == 1
                                 select new FarOpeningVM
                                 {
                                     CCCode = m.CCCode,
                                     Condition = m.Condition,
                                     ItemName = S.SKUCode,
                                     CurrentValue = m.CurrentValue,
                                     DepreciationMethod = m.DepreciationMethod,
                                     DepreciationPercent = m.DepreciationPercent,
                                     EmpId = m.EmpId,
                                     InstallationDate = m.InstallationDate,
                                     ItemId = m.ItemId,
                                     PPrice = m.PPrice,
                                     PurchaseDate = m.PurchaseDate,
                                     Remarks = m.Remarks,
                                     SerialNo = m.SerialNo,
                                     Status = m.Status,
                                     UserId = m.UserId,
                                     SuppId = m.SuppId,
                                     DocId = m.DocId,
                                     ItemType = m.ItemType,
                                     CostTypeId = m.CostTypeId,
                                     CSerialNo = m.CSerialNo
                                 }).ToListAsync();
                lst.AddRange(await (from m in db.Far_Opening
                                    join S in db.Pro_Item on m.ItemId equals S.ItemId
                                    where m.LocId == LocId && m.ItemType == "P" && m.Status == 1
                                    select new FarOpeningVM
                                    {
                                        CCCode = m.CCCode,
                                        Condition = m.Condition,
                                        ItemName = S.Item,
                                        CurrentValue = m.CurrentValue,
                                        DepreciationMethod = m.DepreciationMethod,
                                        DepreciationPercent = m.DepreciationPercent,
                                        EmpId = m.EmpId,
                                        InstallationDate = m.InstallationDate,
                                        ItemId = m.ItemId,
                                        PPrice = m.PPrice,
                                        PurchaseDate = m.PurchaseDate,
                                        Remarks = m.Remarks,
                                        SerialNo = m.SerialNo,
                                        Status = m.Status,
                                        UserId = m.UserId,
                                        SuppId = m.SuppId,
                                        DocId = m.DocId,
                                        ItemType = m.ItemType,
                                        CostTypeId = m.CostTypeId,
                                        CSerialNo = m.CSerialNo
                                    }).ToListAsync());
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateStockOpening(FarOpeningVM m, int UserId)
        {
            try
            {
                var tbl = await db.Far_Opening.SingleOrDefaultAsync(x => x.DocId == m.DocId);
                if (tbl != null)
                {
                    tbl.CCCode = m.CCCode;
                    tbl.Condition = m.Condition;
                    tbl.CurrentValue = m.CurrentValue;
                    tbl.DepreciationMethod = m.DepreciationMethod;
                    tbl.DepreciationPercent = m.DepreciationPercent;
                    tbl.EmpId = m.EmpId;
                    tbl.InstallationDate = m.InstallationDate;
                    tbl.ItemId = m.ItemId;
                    tbl.PPrice = m.PPrice;
                    tbl.PurchaseDate = m.PurchaseDate;
                    tbl.Remarks = m.Remarks;
                    tbl.SerialNo = m.SerialNo;
                    tbl.Status = m.Status;
                    tbl.UserId = UserId;
                    tbl.SuppId = m.SuppId;
                    tbl.ItemType = m.ItemType;
                    tbl.CostTypeId = m.CostTypeId;
                    tbl.CSerialNo = m.CSerialNo;
                    await db.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DestroyStockOpening(FarOpeningVM m, int UserId)
        {
            try
            {
                var tbl = await db.Far_Opening.SingleOrDefaultAsync(x => x.DocId == m.DocId);
                if (tbl != null)
                {
                    tbl.Status = 0;
                    tbl.UserId = UserId;
                    await db.SaveChangesAsync();
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
            try
            {
                foreach (var v in TransLst)
                {
                    var item = await db.Far_Opening.Where(x => x.DocId == v).FirstOrDefaultAsync();
                    if (item.IsPosted)
                        continue;

                    var oldItem = await db.Far_Store.FirstOrDefaultAsync(x => x.SerialNo == item.SerialNo);
                    if (oldItem != null)
                        continue;

                    var itm = await db.Inv_Store.FirstOrDefaultAsync(x => x.SerialNo == item.CSerialNo);
                    if (itm != null)
                        continue;

                    var invstoremodel = new Far_Store()
                    {
                        LocId = item.LocId,
                        PPrice = item.PPrice,
                        Remarks = item.Remarks,
                        SerialNo = item.SerialNo,
                        ItemId = item.ItemId,
                        SuppId = item.SuppId,
                        CCCode = item.CCCode,
                        Condition = item.Condition,
                        CurrentValue = item.CurrentValue,
                        DepreciationMethod = item.DepreciationMethod,
                        DepreciationPercent = item.DepreciationPercent,
                        EmpId = item.EmpId,
                        InstallationDate = item.InstallationDate,
                        ItemType = item.ItemType,
                        PurchaseDate = item.PurchaseDate,
                        Status = item.Status,
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        CSerialNo = item.CSerialNo
                    };
                    db.Far_Store.Add(invstoremodel);

                    item.IsPosted = true;
                    await db.SaveChangesAsync();
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