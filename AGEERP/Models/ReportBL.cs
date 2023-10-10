using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AGEERP.CrReports;

namespace AGEERP.Models
{

    public class ReportBL
    {
        AGEEntities db = new AGEEntities();
        //SaleBL saleBL = new SaleBL();
        public List<SalesReportRVM> GetPOSummary(int CatId, int SuppId, int CompanyId, int ProductId, int TypeId, int ModelId, DateTime FromDate, DateTime ToDate)
        {
            db.Database.CommandTimeout = 3600;
            return db.spRep_POSummary(FromDate, ToDate, ProductId, CompanyId, ModelId, TypeId,SuppId,CatId).
                Select(x => new SalesReportRVM
                {
                    Model = x.SKU,
                    AQty = (x.Ord ?? 0) - (x.Rcv ?? 0),
                    CompName = x.Company,
                    ItemName = x.Product,
                    RQty = x.Rcv ?? 0,
                    SQty = x.Ord ?? 0
                }).ToList();
        }
        public List<StockAllVM> GetSaleAll(int CityId, int LocId,int CatId, int SuppId, int CompanyId, int ProductId, int TypeId, int ModelId, int SKUId, DateTime FromDate, DateTime ToDate, string SaleType)
        {
            try
            {
                List<StockAllVM> lst = new List<StockAllVM>();
                if (SaleType == "" || SaleType == "C")
                {
                    var que = (from S in db.Inv_Sale
                               join D in db.Inv_SaleDetail on S.TransId equals D.TransId
                               join L in db.Comp_Locations on S.LocId equals L.LocId
                               join CT in db.Comp_City on L.CityId equals CT.CityId
                               join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                               join I in db.Itm_Master on ST.SKUId equals I.SKUId
                               join M in db.Itm_Model on I.ModelId equals M.ModelId
                               join T in db.Itm_Type on M.TypeId equals T.TypeId
                               join C in db.Itm_Company on T.ComId equals C.ComId
                               join P in db.Itm_Products on T.ProductId equals P.ProductId
                               join SU in db.Inv_Suppliers on ST.SuppId equals SU.SuppId
                               where S.BillDate >= FromDate && S.BillDate <= ToDate && !I.IsPair
                               select new { S.TransactionTypeId, D.PPrice, D.SPrice, L.CityId, L.LocId, L.LocCode, D.Qty, CT.City, I.SKUId, I.SKUCode,SU.CategoryId, SU.SuppId, SU.SuppName, C.ComId, C.ComName, P.ProductId, P.ProductName, T.TypeId, T.TypeName, M.ModelId, M.Model, ST.SerialNo }).AsQueryable();

                    if (CityId > 0)
                    {
                        que = que.Where(x => x.CityId == CityId);
                    }
                    if (LocId > 0)
                    {
                        que = que.Where(x => x.LocId == LocId);
                    }
                    if (CatId > 0)
                    {
                        que = que.Where(x => x.CategoryId == CatId);
                    }
                    if (SuppId > 0)
                    {
                        que = que.Where(x => x.SuppId == SuppId);
                    }
                    if (CompanyId > 0)
                    {
                        que = que.Where(x => x.ComId == CompanyId);
                    }
                    if (ProductId > 0)
                    {
                        que = que.Where(x => x.ProductId == ProductId);
                    }
                    if (TypeId > 0)
                    {
                        que = que.Where(x => x.TypeId == TypeId);
                    }
                    if (ModelId > 0)
                    {
                        que = que.Where(x => x.ModelId == ModelId);
                    }
                    if (SKUId > 0)
                    {
                        que = que.Where(x => x.SKUId == SKUId);
                    }
                    int[] ttype = new int[] { 1, 5, 11 };
                    var ques = que.Select(x => new StockAllVM
                    {
                        LocCode = x.LocCode,
                        PPrice = ttype.Contains(x.TransactionTypeId) ? x.PPrice : x.PPrice * -1,
                        CityCode = x.City,
                        Company = x.ComName,
                        Model = x.SKUCode,
                        Product = x.ProductName,
                        SerialNo = x.SerialNo,
                        SKU = x.SKUCode,
                        Supplier = x.SuppName,
                        Type = x.TypeName,
                        Qty = ttype.Contains(x.TransactionTypeId) ? x.Qty : x.Qty * -1,
                        SaleType = "Cash",
                        SPrice = ttype.Contains(x.TransactionTypeId) ? x.SPrice : x.SPrice * -1
                    });
                    lst = ques.ToList();
                }
                if (SaleType == "" || SaleType == "I")
                {
                    var quee = (from S in db.Lse_Master
                                join D in db.Lse_Detail on S.AccNo equals D.AccNo
                                join L in db.Comp_Locations on S.LocId equals L.LocId
                                join CT in db.Comp_City on L.CityId equals CT.CityId
                                join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                                join I in db.Itm_Master on ST.SKUId equals I.SKUId
                                join M in db.Itm_Model on I.ModelId equals M.ModelId
                                join T in db.Itm_Type on M.TypeId equals T.TypeId
                                join C in db.Itm_Company on T.ComId equals C.ComId
                                join P in db.Itm_Products on T.ProductId equals P.ProductId
                                join SU in db.Inv_Suppliers on ST.SuppId equals SU.SuppId
                                where S.DeliveryDate >= FromDate && S.DeliveryDate <= ToDate && !I.IsPair
                                select new { D.PPrice, D.InstPrice, L.CityId, L.LocId, L.LocCode, D.Qty, CT.City, I.SKUId, I.SKUCode,SU.CategoryId, SU.SuppId, SU.SuppName, C.ComId, C.ComName, P.ProductId, P.ProductName, T.TypeId, T.TypeName, M.ModelId, M.Model, ST.SerialNo });

                    if (CityId > 0)
                    {
                        quee = quee.Where(x => x.CityId == CityId);
                    }
                    if (LocId > 0)
                    {
                        quee = quee.Where(x => x.LocId == LocId);
                    }
                    if (CatId > 0)
                    {
                        quee = quee.Where(x => x.CategoryId == CatId);
                    }
                    if (SuppId > 0)
                    {
                        quee = quee.Where(x => x.SuppId == SuppId);
                    }
                    if (CompanyId > 0)
                    {
                        quee = quee.Where(x => x.ComId == CompanyId);
                    }
                    if (ProductId > 0)
                    {
                        quee = quee.Where(x => x.ProductId == ProductId);
                    }
                    if (TypeId > 0)
                    {
                        quee = quee.Where(x => x.TypeId == TypeId);
                    }
                    if (ModelId > 0)
                    {
                        quee = quee.Where(x => x.ModelId == ModelId);
                    }
                    if (SKUId > 0)
                    {
                        quee = quee.Where(x => x.SKUId == SKUId);
                    }

                    var quees = quee.Select(x => new StockAllVM
                    {
                        LocCode = x.LocCode,
                        PPrice = x.PPrice,
                        CityCode = x.City,
                        Company = x.ComName,
                        Model = x.SKUCode,
                        Product = x.ProductName,
                        SerialNo = x.SerialNo,
                        SKU = x.SKUCode,
                        Supplier = x.SuppName,
                        Type = x.TypeName,
                        Qty = x.Qty,
                        SaleType = "Inst",
                        SPrice = x.InstPrice,
                    });
                    lst.AddRange(quees.ToList());
                    var queer = (from R in db.Lse_Return
                                 join S in db.Lse_Master on R.AccNo equals S.AccNo
                                 join D in db.Lse_Detail on S.AccNo equals D.AccNo
                                 join L in db.Comp_Locations on S.LocId equals L.LocId
                                 join CT in db.Comp_City on L.CityId equals CT.CityId
                                 join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                                 join I in db.Itm_Master on ST.SKUId equals I.SKUId
                                 join M in db.Itm_Model on I.ModelId equals M.ModelId
                                 join T in db.Itm_Type on M.TypeId equals T.TypeId
                                 join C in db.Itm_Company on T.ComId equals C.ComId
                                 join P in db.Itm_Products on T.ProductId equals P.ProductId
                                 join SU in db.Inv_Suppliers on ST.SuppId equals SU.SuppId
                                 where R.WorkingDate >= FromDate && R.WorkingDate <= ToDate && !I.IsPair
                                 select new { D.PPrice, D.InstPrice, L.CityId, L.LocId, L.LocCode, D.Qty, CT.City, I.SKUId, I.SKUCode,SU.CategoryId, SU.SuppId, SU.SuppName, C.ComId, C.ComName, P.ProductId, P.ProductName, T.TypeId, T.TypeName, M.ModelId, M.Model, ST.SerialNo });

                    if (CityId > 0)
                    {
                        queer = queer.Where(x => x.CityId == CityId);
                    }
                    if (LocId > 0)
                    {
                        queer = queer.Where(x => x.LocId == LocId);
                    }
                    if (CatId > 0)
                    {
                        queer = queer.Where(x => x.CategoryId == CatId);
                    }
                    if (SuppId > 0)
                    {
                        queer = queer.Where(x => x.SuppId == SuppId);
                    }
                    if (CompanyId > 0)
                    {
                        queer = queer.Where(x => x.ComId == CompanyId);
                    }
                    if (ProductId > 0)
                    {
                        queer = queer.Where(x => x.ProductId == ProductId);
                    }
                    if (TypeId > 0)
                    {
                        queer = queer.Where(x => x.TypeId == TypeId);
                    }
                    if (ModelId > 0)
                    {
                        queer = queer.Where(x => x.ModelId == ModelId);
                    }
                    if (SKUId > 0)
                    {
                        queer = queer.Where(x => x.SKUId == SKUId);
                    }

                    var queers = queer.Select(x => new StockAllVM
                    {
                        LocCode = x.LocCode,
                        PPrice = x.PPrice * -1,
                        CityCode = x.City,
                        Company = x.ComName,
                        Model = x.SKUCode,
                        Product = x.ProductName,
                        SerialNo = x.SerialNo,
                        SKU = x.SKUCode,
                        Supplier = x.SuppName,
                        Type = x.TypeName,
                        Qty = x.Qty * -1,
                        SaleType = "Inst",
                        SPrice = x.InstPrice * -1,
                    });
                    lst.AddRange(queers.ToList());
                }
                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<StockAllVM> GetPurchaseAll(int CityId, int LocId,int CatId, int SuppId, int CompanyId, int ProductId, int TypeId, int ModelId, int SKUId, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                List<StockAllVM> lst = new List<StockAllVM>();
                    var que = (from S in db.Inv_GRN
                               join D in db.Inv_GRNDetail on S.GRNId equals D.GRNId
                               join L in db.Comp_Locations on S.LocId equals L.LocId
                               join CT in db.Comp_City on L.CityId equals CT.CityId
                               join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                               join I in db.Itm_Master on ST.SKUId equals I.SKUId
                               join M in db.Itm_Model on I.ModelId equals M.ModelId
                               join T in db.Itm_Type on M.TypeId equals T.TypeId
                               join C in db.Itm_Company on T.ComId equals C.ComId
                               join P in db.Itm_Products on T.ProductId equals P.ProductId
                               join SU in db.Inv_Suppliers on ST.SuppId equals SU.SuppId
                               where S.GRNDate >= FromDate && S.GRNDate <= ToDate && !I.IsPair
                               select new { ST.PPrice, L.CityId, L.LocId, L.LocCode, D.RcvdQty, CT.City, I.SKUId, I.SKUCode,SU.CategoryId, SU.SuppId, SU.SuppName, C.ComId, C.ComName, P.ProductId, P.ProductName, T.TypeId, T.TypeName, M.ModelId, M.Model, ST.SerialNo }).AsQueryable();

                    if (CityId > 0)
                    {
                        que = que.Where(x => x.CityId == CityId);
                    }
                    if (LocId > 0)
                    {
                        que = que.Where(x => x.LocId == LocId);
                    }
                    if (CatId > 0)
                    {
                        que = que.Where(x => x.CategoryId == CatId);
                    }
                    if (SuppId > 0)
                    {
                        que = que.Where(x => x.SuppId == SuppId);
                    }
                    if (CompanyId > 0)
                    {
                        que = que.Where(x => x.ComId == CompanyId);
                    }
                    if (ProductId > 0)
                    {
                        que = que.Where(x => x.ProductId == ProductId);
                    }
                    if (TypeId > 0)
                    {
                        que = que.Where(x => x.TypeId == TypeId);
                    }
                    if (ModelId > 0)
                    {
                        que = que.Where(x => x.ModelId == ModelId);
                    }
                    if (SKUId > 0)
                    {
                        que = que.Where(x => x.SKUId == SKUId);
                    }
                    var ques = que.Select(x => new StockAllVM
                    {
                        LocCode = x.LocCode,
                        PPrice = x.PPrice ,
                        CityCode = x.City,
                        Company = x.ComName,
                        Model = x.SKUCode,
                        Product = x.ProductName,
                        SerialNo = x.SerialNo,
                        SKU = x.SKUCode,
                        Supplier = x.SuppName,
                        Type = x.TypeName,
                        Qty = x.RcvdQty,
                    });
                    lst = ques.ToList();

              var  quee = (from S in db.Inv_POReturn
                           join D in db.Inv_POReturnDtl on S.PORId equals D.PORId
                           join L in db.Comp_Locations on S.LocId equals L.LocId
                           join CT in db.Comp_City on L.CityId equals CT.CityId
                           join ST in db.Inv_Store on D.ItemId equals ST.ItemId
                           join I in db.Itm_Master on ST.SKUId equals I.SKUId
                           join M in db.Itm_Model on I.ModelId equals M.ModelId
                           join T in db.Itm_Type on M.TypeId equals T.TypeId
                           join C in db.Itm_Company on T.ComId equals C.ComId
                           join P in db.Itm_Products on T.ProductId equals P.ProductId
                           join SU in db.Inv_Suppliers on ST.SuppId equals SU.SuppId
                           where S.PORDate >= FromDate && S.PORDate <= ToDate && !I.IsPair
                           select new { ST.PPrice, L.CityId, L.LocId, L.LocCode, D.Qty, CT.City, I.SKUId, I.SKUCode,SU.CategoryId, SU.SuppId, SU.SuppName, C.ComId, C.ComName, P.ProductId, P.ProductName, T.TypeId, T.TypeName, M.ModelId, M.Model, ST.SerialNo }).AsQueryable();

                if (CityId > 0)
                {
                    quee = quee.Where(x => x.CityId == CityId);
                }
                if (LocId > 0)
                {
                    quee = quee.Where(x => x.LocId == LocId);
                }
                if (CatId > 0)
                {
                    quee = quee.Where(x => x.CategoryId == CatId);
                }
                if (SuppId > 0)
                {
                    quee = quee.Where(x => x.SuppId == SuppId);
                }
                if (CompanyId > 0)
                {
                    quee = quee.Where(x => x.ComId == CompanyId);
                }
                if (ProductId > 0)
                {
                    quee = quee.Where(x => x.ProductId == ProductId);
                }
                if (TypeId > 0)
                {
                    quee = quee.Where(x => x.TypeId == TypeId);
                }
                if (ModelId > 0)
                {
                    quee = quee.Where(x => x.ModelId == ModelId);
                }
                if (SKUId > 0)
                {
                    quee = quee.Where(x => x.SKUId == SKUId);
                }
                var quees = quee.Select(x => new StockAllVM
                {
                    LocCode = x.LocCode,
                    PPrice = x.PPrice*-1,
                    CityCode = x.City,
                    Company = x.ComName,
                    Model = x.SKUCode,
                    Product = x.ProductName,
                    SerialNo = x.SerialNo,
                    SKU = x.SKUCode,
                    Supplier = x.SuppName,
                    Type = x.TypeName,
                    Qty = x.Qty*-1,
                });
                lst.AddRange(quees.ToList());


                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<StockAllVM> GetStockAll(int CityId, int LocId,int CatId, int SuppId, int CompanyId, int ProductId, int TypeId, int ModelId, int SKUId,int AgingDays)
        {
            try
            {
                var dt = DateTime.Now.Date;
                var que = (from  ST in db.Inv_Store //on D.ItemId equals ST.ItemId
                           join SS in db.Inv_Status on ST.StatusID equals SS.StatusID
                           join L in db.Comp_Locations on ST.LocId equals L.LocId
                           join CT in db.Comp_City on L.CityId equals CT.CityId
                           join I in db.Itm_Master on ST.SKUId equals I.SKUId
                           join M in db.Itm_Model on I.ModelId equals M.ModelId
                           join T in db.Itm_Type on M.TypeId equals T.TypeId
                           join C in db.Itm_Company on T.ComId equals C.ComId
                           join P in db.Itm_Products on T.ProductId equals P.ProductId
                           join SU in db.Inv_Suppliers on ST.SuppId equals SU.SuppId
                           where SS.MFact == 1 && !I.IsPair && ST.LocId != 191
                           select new { ST.PPrice, L.CityId, L.LocId, L.LocCode, ST.Qty, CT.City, I.SKUId, I.SKUCode,SU.CategoryId, SU.SuppId, SU.SuppName, C.ComId, C.ComName, P.ProductId, P.ProductName, T.TypeId, T.TypeName, M.ModelId, M.Model, ST.SerialNo,ST.TrxDate }).AsQueryable();

                if(AgingDays > 0)
                {
                    DateTime agingdate = DateTime.Now.Date.AddDays(AgingDays * -1);
                    que = que.Where(x => x.TrxDate < agingdate);
                }
                if (CityId > 0)
                {
                    que = que.Where(x => x.CityId == CityId);
                }
                if (LocId > 0)
                {
                    que = que.Where(x => x.LocId == LocId);
                }
                if (CatId > 0)
                {
                    que = que.Where(x => x.CategoryId == CatId);
                }
                if (SuppId > 0)
                {
                    que = que.Where(x => x.SuppId == SuppId);
                }
                if (CompanyId > 0)
                {
                    que = que.Where(x => x.ComId == CompanyId);
                }
                if (ProductId > 0)
                {
                    que = que.Where(x => x.ProductId == ProductId);
                }
                if (TypeId > 0)
                {
                    que = que.Where(x => x.TypeId == TypeId);
                }
                if (ModelId > 0)
                {
                    que = que.Where(x => x.ModelId == ModelId);
                }
                if (SKUId > 0)
                {
                    que = que.Where(x => x.SKUId == SKUId);
                }
                var ques = que.Select(x => new StockAllVM
                {
                    LocCode = x.LocCode,
                    PPrice = x.PPrice,
                    CityCode = x.City,
                    Company = x.ComName,
                    Model = x.SKUCode,
                    Product = x.ProductName,
                    SerialNo = x.SerialNo,
                    SKU = x.SKUCode,
                    Supplier = x.SuppName,
                    Type = x.TypeName,
                    Qty = x.Qty,
                    TrxDate = x.TrxDate.Value
                });
                return ques.ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }
        //public List<StockAllVM> GetSaleReport(int CityId, int LocId, int SuppId, int CompanyId, int ProductId, int TypeId, int ModelId, int SKUId, DateTime FromDate, DateTime ToDate, string SaleType)
        //{
        //    try
        //    {
        //        var lst = db.spRep_Sale(0,FromDate,ToDate,ProductId,CompanyId,ModelId,TypeId,CityId,LocId).Select(x => 
        //        new SalesReportRVM 
        //        {
        //            AQty = x.sa
        //        })
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        public async Task<List<StockVerificationVM>> StockVerificationList(int LocId, DateTime dateTime)
        {
            try
            {
                return await db.Inv_StockVerification.Where(x => x.LocId == LocId && x.DocDate == dateTime).Select(x => new StockVerificationVM
                {
                    DocId = x.DocId,
                    DocDate = x.DocDate
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<StockTypeWiseRVM> StockTypeWiseReport(int LocId)
        {
            try
            {
                return db.spRep_StockTypeWise(LocId).Select(x => new StockTypeWiseRVM
                {
                    Model = x.Model,
                    Qty = x.Qty ?? 0,
                    Type = x.TypeName,
                    Status = x.StatusTitle
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
     
        public async Task<List<CustomerDetailRVM>> GetCustomerInfo4Lock(long AccNo)
        {
            try
            {
                var lst = db.spRep_CustomerInfo_V1_Lock(AccNo).Select(x => new CustomerDetailRVM
                {
                    AccNo = x.AccNo,
                    OldAccNo = x.OldAccNo ?? 0,
                    ActualAdvance = x.ActualAdvance ?? 0,
                    Advance = x.Advance,
                    Affidavit = x.Affidavit == true ? "Yes" : "No",
                    FName = x.FName,
                    FineRecv = x.FineRecv ?? 0,
                    Duration = x.Duration ?? 0,
                    Balance = x.Balance ?? 0,
                    Category = x.Category == "F" ? "Fresh" : x.Category == "R" ? "Regular" : "",
                    Company = x.Company,
                    CRC = x.CRC ?? "",
                    CustId = x.CustId ?? 0,
                    Customer = x.Customer,
                    Defaulter = x.Defaulter == true ? "Yes" : "No",
                    Discount = x.Discount ?? 0,
                    DueAmt = x.DueAmt ?? 0,
                    Gender = x.Gender,
                    Inquiry = x.Inquiry,
                    InstPrice = x.InstPrice,
                    LocCode = x.LocCode,
                    LocName = x.LocName,
                    Manager = x.Manager,
                    Marketing = x.Marketing,
                    Mobile1 = x.Mobile1,
                    Mobile2 = x.Mobile2,
                    Model = x.Model,
                    MonthlyInst = x.MonthlyInst,
                    MonthStatus = x.MonthStatus ?? "",
                    NIC = x.NIC,
                    NoOfFine = x.NoOfFine ?? 0,
                    NoOfFineExempt = x.NoOfFineExempt ?? 0,
                    NoOfInstRec = x.NoOfInstRec ?? 0,
                    NoOfInstRem = x.NoOfInstRem ?? 0,
                    Occupation = x.Occupation,
                    OffAddress = x.OffAddress,
                    OutstandAmt = x.OutstandAmt ?? 0,
                    OutstandDate = x.OutstandDate ?? Convert.ToDateTime("01-01-1900"),
                    PrevAcc = x.PrevAcc,
                    ProcessAt = x.ProcessAt,
                    ProcessFee = x.ProcessFee,
                    Product = x.Product,
                    ProductPrice = x.ProductPrice,
                    PTO = x.PTO == true ? "Yes" : "No",
                    RecOfficer = x.RecOfficer ?? "",
                    RecvAmt = x.RecvAmt ?? 0,
                    Remarks = x.Remarks,
                    RepeatCus = x.RepeatCus ?? 0,
                    RepeatGuar = x.RepeatGuar ?? 0,
                    ResAddress = x.ResAddress,
                    ResidentialStatus = x.ResidentialStatus,
                    RM = x.RM ?? "",
                    Salary = x.Salary ?? 0,
                    SearchStatus = x.SearchStatus == true ? "Yes" : "No",
                    SerialNo = x.SerialNo,
                    SKUCode = x.SKUCode ?? "",
                    SManager = x.SManager ?? "",
                    SRM = x.SRM ?? "",
                    Status = x.Status,
                    TotalRecv = x.TotalRecv ?? 0,
                    TransDate = x.TransDate ?? Convert.ToDateTime("01-01-1900"),
                    Username = x.Username,
                    Worth = x.Worth == true ? "Yes" : "No",
                    G1Name = "",
                    G1NIC = "",
                    G1Occupation = "",
                    G1OffAddress = "",
                    G1Relation = "",
                    G1ResAddress = "",
                    G1SO = "",
                    G1TelOff = "",
                    G1TelRes = "",
                    G2Name = "",
                    G2NIC = "",
                    G2Occupation = "",
                    G2OffAddress = "",
                    G2Relation = "",
                    G2ResAddress = "",
                    G2SO = "",
                    G2TelOff = "",
                    G2TelRes = "",
                    G3Name = "",
                    G3NIC = "",
                    G3Occupation = "",
                    G3OffAddress = "",
                    G3Relation = "",
                    G3ResAddress = "",
                    G3SO = "",
                    G3TelOff = "",
                    G3TelRes = "",
                    G4Name = "",
                    G4NIC = "",
                    G4Occupation = "",
                    G4OffAddress = "",
                    G4Relation = "",
                    G4ResAddress = "",
                    G4SO = "",
                    G4TelOff = "",
                    G4TelRes = "",
                    Column1 = "",
                    CRCRemarks = x.CRCRemarks,
                    DBMRemarks = x.DBMRemarks,
                    EmployeeStatus = x.EmployeeStatus
                }).ToList();

                var gLst = await db.Lse_Guarantor.Where(x => x.AccNo == AccNo && x.Status).OrderBy(x => x.GuarantorId).ToListAsync();
                int gcount = 1;
                foreach (var v in gLst)
                {
                    if (gcount == 1)
                    {
                        lst[0].G1Name = v.Name;
                        lst[0].G1NIC = v.NIC;
                        lst[0].G1Occupation = v.Occupation;
                        lst[0].G1OffAddress = v.OffAddress;
                        lst[0].G1Relation = v.GRelation;
                        lst[0].G1ResAddress = v.ResAddress;
                        lst[0].G1SO = v.FName;
                        lst[0].G1TelOff = v.TelOff;
                        lst[0].G1TelRes = v.TelRes;
                    }
                    else if (gcount == 2)
                    {
                        lst[0].G2Name = v.Name;
                        lst[0].G2NIC = v.NIC;
                        lst[0].G2Occupation = v.Occupation;
                        lst[0].G2OffAddress = v.OffAddress;
                        lst[0].G2Relation = v.GRelation;
                        lst[0].G2ResAddress = v.ResAddress;
                        lst[0].G2SO = v.FName;
                        lst[0].G2TelOff = v.TelOff;
                        lst[0].G2TelRes = v.TelRes;
                    }
                    else if (gcount == 3)
                    {
                        lst[0].G3Name = v.Name;
                        lst[0].G3NIC = v.NIC;
                        lst[0].G3Occupation = v.Occupation;
                        lst[0].G3OffAddress = v.OffAddress;
                        lst[0].G3Relation = v.GRelation;
                        lst[0].G3ResAddress = v.ResAddress;
                        lst[0].G3SO = v.FName;
                        lst[0].G3TelOff = v.TelOff;
                        lst[0].G3TelRes = v.TelRes;
                    }
                    else if (gcount == 4)
                    {
                        lst[0].G4Name = v.Name;
                        lst[0].G4NIC = v.NIC;
                        lst[0].G4Occupation = v.Occupation;
                        lst[0].G4OffAddress = v.OffAddress;
                        lst[0].G4Relation = v.GRelation;
                        lst[0].G4ResAddress = v.ResAddress;
                        lst[0].G4SO = v.FName;
                        lst[0].G4TelOff = v.TelOff;
                        lst[0].G4TelRes = v.TelRes;
                    }
                    gcount++;
                }

                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<CustomerDetailRVM>> GetCustomerInfo(long AccNo)
        {
            try
            {
                var lst = db.spRep_CustomerInfo_V1(AccNo).Select(x => new CustomerDetailRVM
                {
                    AccNo = x.AccNo,
                    OldAccNo = x.OldAccNo ?? 0,
                    ActualAdvance = x.ActualAdvance ?? 0,
                    Advance = x.Advance,
                    Affidavit = x.Affidavit == true ? "Yes" : "No",
                    FName = x.FName,
                    FineRecv = x.FineRecv ?? 0,
                    Duration = x.Duration ?? 0,
                    Balance = x.Balance ?? 0,
                    Category = x.Category == "F" ? "Fresh" : x.Category == "R" ? "Regular" :"" ,
                    Company = x.Company,
                    CRC = x.CRC ?? "",
                    CustId = x.CustId ?? 0,
                    Customer = x.Customer,
                    Defaulter = x.Defaulter == true ? "Yes" : "No",
                    Discount = x.Discount ?? 0,
                    DueAmt = x.DueAmt ?? 0,
                    Gender = x.Gender,
                    Inquiry = x.Inquiry,
                    InstPrice = x.InstPrice,
                    LocCode = x.LocCode,
                    LocName = x.LocName,
                    Manager = x.Manager,
                    Marketing = x.Marketing,
                    Mobile1 = x.Mobile1,
                    Mobile2 = x.Mobile2,
                    Model = x.Model,
                    MonthlyInst = x.MonthlyInst,
                    MonthStatus = x.MonthStatus ?? "",
                    NIC = x.NIC,
                    NoOfFine = x.NoOfFine ?? 0,
                    NoOfFineExempt = x.NoOfFineExempt ?? 0,
                    NoOfInstRec = x.NoOfInstRec ?? 0,
                    NoOfInstRem = x.NoOfInstRem ?? 0,
                    Occupation = x.Occupation,
                    OffAddress = x.OffAddress,
                    OutstandAmt = x.OutstandAmt ?? 0,
                    OutstandDate = x.OutstandDate ?? Convert.ToDateTime("01-01-1900"),
                    PrevAcc = x.PrevAcc,
                    ProcessAt = x.ProcessAt,
                    ProcessFee = x.ProcessFee,
                    Product = x.Product,
                    ProductPrice = x.ProductPrice,
                    PTO = x.PTO == true ? "Yes" : "No",
                    RecOfficer = x.RecOfficer ?? "",
                    RecvAmt = x.RecvAmt ?? 0,
                    Remarks = x.Remarks,
                    RepeatCus = x.RepeatCus ?? 0,
                    RepeatGuar = x.RepeatGuar ?? 0,
                    ResAddress = x.ResAddress,
                    ResidentialStatus = x.ResidentialStatus,
                    RM = x.RM ?? "",
                    Salary = x.Salary ?? 0,
                    SearchStatus = x.SearchStatus == true ? "Yes" : "No",
                    SerialNo = x.SerialNo,
                    SKUCode = x.SKUCode ?? "",
                    SManager = x.SManager ?? "",
                    SRM = x.SRM ?? "",
                    Status = x.Status,
                    TotalRecv = x.TotalRecv ?? 0,
                    TransDate = x.TransDate ?? Convert.ToDateTime("01-01-1900"),
                    Username = x.Username,
                    Worth = x.Worth == true ? "Yes" : "No",
                    G1Name = "",
                    G1NIC = "",
                    G1Occupation = "",
                    G1OffAddress = "",
                    G1Relation = "",
                    G1ResAddress = "",
                    G1SO = "",
                    G1TelOff = "",
                    G1TelRes = "",
                    G2Name = "",
                    G2NIC = "",
                    G2Occupation = "",
                    G2OffAddress = "",
                    G2Relation = "",
                    G2ResAddress = "",
                    G2SO = "",
                    G2TelOff = "",
                    G2TelRes = "",
                    G3Name = "",
                    G3NIC = "",
                    G3Occupation = "",
                    G3OffAddress = "",
                    G3Relation = "",
                    G3ResAddress = "",
                    G3SO = "",
                    G3TelOff = "",
                    G3TelRes = "",
                    G4Name = "",
                    G4NIC = "",
                    G4Occupation = "",
                    G4OffAddress = "",
                    G4Relation = "",
                    G4ResAddress = "",
                    G4SO = "",
                    G4TelOff = "",
                    G4TelRes = "",
                    Column1 = "",
                    CRCRemarks = x.CRCRemarks,
                    EmployeeStatus = x.EmployeeStatus,
                    DBMRemarks = x.DBMRemarks
                }).ToList();

                var gLst = await db.Lse_Guarantor.Where(x => x.AccNo == AccNo && x.Status).OrderBy(x => x.GuarantorId).ToListAsync();
                int gcount = 1;
                foreach (var v in gLst)
                {
                    if(gcount == 1)
                    {
                        lst[0].G1Name = v.Name;
                        lst[0].G1NIC = v.NIC;
                        lst[0].G1Occupation = v.Occupation;
                        lst[0].G1OffAddress = v.OffAddress;
                        lst[0].G1Relation = v.GRelation;
                        lst[0].G1ResAddress = v.ResAddress;
                        lst[0].G1SO = v.FName;
                        lst[0].G1TelOff = v.TelOff;
                        lst[0].G1TelRes = v.TelRes;
                    }
                    else if (gcount == 2)
                    {
                        lst[0].G2Name = v.Name;
                        lst[0].G2NIC = v.NIC;
                        lst[0].G2Occupation = v.Occupation;
                        lst[0].G2OffAddress = v.OffAddress;
                        lst[0].G2Relation = v.GRelation;
                        lst[0].G2ResAddress = v.ResAddress;
                        lst[0].G2SO = v.FName;
                        lst[0].G2TelOff = v.TelOff;
                        lst[0].G2TelRes = v.TelRes;
                    }
                    else if (gcount == 3)
                    {
                        lst[0].G3Name = v.Name;
                        lst[0].G3NIC = v.NIC;
                        lst[0].G3Occupation = v.Occupation;
                        lst[0].G3OffAddress = v.OffAddress;
                        lst[0].G3Relation = v.GRelation;
                        lst[0].G3ResAddress = v.ResAddress;
                        lst[0].G3SO = v.FName;
                        lst[0].G3TelOff = v.TelOff;
                        lst[0].G3TelRes = v.TelRes;
                    }
                    else if (gcount == 4)
                    {
                        lst[0].G4Name = v.Name;
                        lst[0].G4NIC = v.NIC;
                        lst[0].G4Occupation = v.Occupation;
                        lst[0].G4OffAddress = v.OffAddress;
                        lst[0].G4Relation = v.GRelation;
                        lst[0].G4ResAddress = v.ResAddress;
                        lst[0].G4SO = v.FName;
                        lst[0].G4TelOff = v.TelOff;
                        lst[0].G4TelRes = v.TelRes;
                    }
                    gcount++;
                }

                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<long> GetAccfromOSId(long TransId)
        {
            try
            {
                return await db.Lse_Outstand.Where(x => x.TransId == TransId).Select(x => x.AccNo).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //public List<long> GetAccfromOSId(string str)
        //{
        //    try
        //    {
        //        long[] arr = Array.ConvertAll(str.Split(','), x => Convert.ToInt64(x));
        //        return db.Lse_Outstand.Where(x => arr.Contains(x.TransId)).Select(x => x.AccNo).ToList();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //} 
        //public List<InstDetailVM> GetInstByAccNo(long AccNo)
        //{
        //    try
        //    {
        //        var proc = db.Lse_Master.Find(AccNo);
        //        var PreBalance = proc.InstPrice - proc.Advance;
        //        var lst = db.Lse_Installment.Where(x => x.AccNo == AccNo).OrderBy(x => x.InstDate).ToList();
        //        List<InstDetailVM> InstDetLst = new List<InstDetailVM>();
        //        foreach (var x in lst)
        //        {
        //            InstDetLst.Add(new InstDetailVM
        //            {
        //                AccNo = x.AccNo,
        //                ActualInstallment = x.InstCharges,
        //                Discount = x.Discount,
        //                Fine = x.Fine,
        //                FineType = x.FineType == "E" ? "Exempt" : x.FineType == "N" ? "Nothing" : x.FineType == "R" ? "Receive" : "",
        //                InstallDate = x.InstDate,
        //                InstCharges = x.InstCharges,
        //                Remarks = (x.PaidBy == 1 ? "" : "Voucher || ") + x.Remarks ?? "",
        //                TransId = x.InstId,
        //                RecoveryOff = x.IsAIC ? "AIC" : x.RecoveryId == null ? "" : db.Pay_EmpMaster.Where(a => a.EmpId == x.RecoveryId).Select(a => a.EmpName).FirstOrDefault(),
        //                PreBalance = PreBalance,
        //                Balance = PreBalance - x.InstCharges - x.Discount,
        //                PaidBy = x.PaidBy
        //            });
        //            PreBalance = PreBalance - x.InstCharges - x.Discount;
        //        }
        //        return InstDetLst;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public async Task<List<InstDetailVM>> GetInstByAcc(long AccNo)
        {
            try
            {
                //var proc = await db.Lse_Master.FindAsync(AccNo);
                var PreBalance = await db.Lse_Master.Where(x => x.AccNo == AccNo).SumAsync(proc => (proc.InstPrice - proc.Advance));
                var lst = await db.Lse_Installment.Where(x => x.AccNo == AccNo).OrderBy(x => x.InstDate).ToListAsync();
                List<InstDetailVM> InstDetLst = new List<InstDetailVM>();
                foreach (var x in lst)
                {
                    InstDetLst.Add(new InstDetailVM
                    {
                        AccNo = x.AccNo,
                        ActualInstallment = x.InstCharges,
                        Discount = x.Discount,
                        Fine = x.Fine,
                        FineType = x.FineType == "E" ? "Exempt": x.FineType == "N" ? "Nothing" : x.FineType == "R" ? "Receive":"",
                        InstallDate = x.InstDate,
                        InstCharges = x.InstCharges,
                        Remarks = (x.PaidBy == 1 ? "":"Voucher || ")+ x.Remarks ?? "",
                        TransId = x.InstId,
                        RecoveryOff = x.IsAIC ? "AIC": x.RecoveryId == null ? "" : db.Pay_EmpMaster.Where(a => a.EmpId == x.RecoveryId).Select(a => a.EmpName).FirstOrDefault(),
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
        public List<InstallmentRVM> GetInstallmentReport(long InstId)
        {
            try
            {
                return db.spRep_Installment_V1(InstId).Select(x => new InstallmentRVM
                {
                    AccNo = x.AccNo,
                    Advance = x.Advance,
                    Customer = x.Customer,
                    DeliveryDate = (DateTime)x.DeliveryDate,
                    Discount = x.Discount,
                    Duration = x.Duration,
                    Fine = x.Fine,
                    FineType = x.FineType,
                    FName = x.FName,
                    InstCharges = x.InstCharges,
                    InstDate = x.InstDate,
                    Location = x.Location,
                    ReceiptNo = x.ReceiptNo,
                    RecoveryOff = x.RecoveryOff,
                    RecvInst = x.RecvInst ?? 0,
                    RecvInstAmt = x.RecvInstAmt ?? 0,
                    TotalPrice = x.TotalPrice,
                    Username = x.Username
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<AttendanceVM> GetAttMap(DateTime dt)
        {
            try
            {
                return db.Pay_Attendance.Where(x => x.AttnDate == dt && x.Lat > 0).Select(x =>
                new AttendanceVM
                {
                    Lat =  x.Lat.ToString(),
                    Long = x.Lng.ToString(),
                    EmpId = x.EmpId
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}