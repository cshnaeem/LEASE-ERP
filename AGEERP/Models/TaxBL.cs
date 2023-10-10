using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AGEERP.Models
{
    public class TaxBL
    {
        AGEEntities db = new AGEEntities();
        HttpClient client = new HttpClient();

        public TaxBL()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "1298b5eb-b252-3d97-8622-a4a69d5bf818");
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }
        public async Task<string> GetInvoiceNo(int LocId)
        {
            try
            {
                var loc = await db.Comp_Locations.FindAsync(LocId);
                if (loc.FBRPOSID == null)
                {
                    return "";
                }
                var dt = DateTime.Now;
                string startWith = dt.ToString("yyMM") + LocId.ToString("000");
                var docNo = await db.Tax_Invoice.Where(x => x.LocId == loc.LocId && x.USIN.StartsWith(startWith)).OrderByDescending(x => x.USIN).Select(x => x.USIN).FirstOrDefaultAsync();
                if (docNo != null)
                {
                    //if (docNo.Substring(0, 4) == DateTime.Now.ToString("yyMM"))
                    //{
                        docNo = startWith + (Convert.ToInt32(docNo.Substring(7, 6)) + 1).ToString("000000");
                    //}
                    //else
                    //{
                    //    docNo = startWith + "000001";
                    //}
                }
                else
                {
                    docNo = startWith + "000001";
                }
                return docNo;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task<string> GetInvoiceNo2(int LocId)
        {
            try
            {
                var loc = await db.Comp_Locations.FindAsync(LocId);
                if (loc.FBRPOSID == null)
                {
                    return "";
                }
                var dt = Convert.ToDateTime("2022-04-01");
                //var Ndt = Convert.ToDateTime("2021-12-01");
                string startWith = dt.ToString("yyMM") + LocId.ToString("000");
                var docNo = await db.Tax_Invoice.Where(x => x.LocId == loc.LocId && x.USIN.StartsWith(startWith)).OrderByDescending(x => x.TransId).Select(x => x.USIN).FirstOrDefaultAsync();
                if (docNo != null)
                {
                    if (docNo.Substring(0, 4) == dt.ToString("yyMM"))
                    {
                        docNo = dt.ToString("yyMM") + LocId.ToString("000") + (Convert.ToInt32(docNo.Substring(7, 6)) + 1).ToString("000000");
                    }
                    else
                    {
                        docNo = dt.ToString("yyMM") + LocId.ToString("000") + "000001";
                    }
                }
                else
                {
                    docNo = dt.ToString("yyMM") + LocId.ToString("000") + "000001";
                }
                return docNo;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task<bool> PostPend()
        {
            try
            {
                long[] lst = new long[] {
1975780
                };
                foreach (var v in lst)
                {
                    await PostToFBR(v, "I");
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> PostToFBR(long TransId, string Type)
        {
            db = new AGEEntities();
            try
            {
                var IsExist = await db.Tax_Invoice.Where(x => x.RefTransId == TransId && x.Type == Type).AnyAsync();
                if (IsExist)
                {
                    return false;
                }
                if (Type == "C")
                {

                    var mas = await db.Inv_Sale.FindAsync(TransId);
                    if (!(db.spget_Taxable(TransId, Type).FirstOrDefault().Value))
                    {
                        return false;
                    }

                    var loc = await db.Comp_Locations.FindAsync(mas.LocId);
                    if (loc.FBRPOSID == null)
                    {
                        return false;
                    }

                    var dtlLst = mas.Inv_SaleDetail.ToList();

                    decimal totalBillAmount = 0;
                    decimal totalTaxCharged = 0;
                    int totalQuantity = 0;
                    decimal totalSaleValue = 0;
                    decimal totalDisc = 0;

                    var refUSIN = "";
                    if (mas.TransactionTypeId == 2 || mas.TransactionTypeId == 6)
                    {
                        if (mas.ItemType == "O")
                        {
                            return false;
                        }
                        var preSale = await db.Inv_Sale.FindAsync(mas.RefSaleId);
                        if (preSale.TInvoiceNo == null)
                        {
                            return false;
                        }
                        else
                        {
                            refUSIN = preSale.TInvoiceNo;
                        }
                    }

                    List<Tax_InvoiceItems> invDtl = new List<Tax_InvoiceItems>();
                    foreach (var x in dtlLst)
                    {
                        if (x.MRP == 0)
                            continue;

                        Tax_InvoiceItems tbl = new Tax_InvoiceItems();
                        var itm = await db.Inv_Store.FindAsync(x.ItemId);
                        if (itm.Itm_Master.Itm_Model.TypeId == 1001)
                        {
                            tbl.PCTCode = "85171219";
                        }
                        else
                        {
                            tbl.PCTCode = itm.Itm_Master.Itm_Model.Itm_Type.Itm_Products.PCTCode;
                        }

                        tbl.RefTransDtlId = x.TransDtlId;
                        tbl.Discount = (x.SPrice - x.Discount) < x.MRP ? x.MRP - (x.SPrice - x.Discount) : 0;
                        tbl.FurtherTax = 0;
                        tbl.InvoiceType = (new int[] { 1, 5, 11 }).Contains(mas.TransactionTypeId) ? 1 : 3;

                        if (itm.Itm_Master.SKUCode.Length > 50)
                        {
                            tbl.ItemCode = itm.Itm_Master.SKUCode.Substring(0, 50);
                        }
                        else
                        {
                            tbl.ItemCode = itm.Itm_Master.SKUCode;
                        }

                        tbl.Quantity = x.Qty;
                        tbl.RefUSIN = refUSIN;
                        tbl.SaleValue = x.MRP - x.Tax ?? 0;
                        tbl.TaxCharged = x.Tax ?? 0;
                        tbl.TaxRate = (double)(itm.Tax ?? 0);
                        tbl.TotalAmount = x.MRP;
                        tbl.ItemName = itm.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName;

                        totalBillAmount = totalBillAmount + tbl.TotalAmount;
                        totalTaxCharged = totalTaxCharged + tbl.TaxCharged;
                        totalQuantity = totalQuantity + tbl.Quantity;
                        totalSaleValue = totalSaleValue + tbl.SaleValue;
                        totalDisc = totalDisc + tbl.Discount ?? 0;

                        invDtl.Add(tbl);

                    }

                    if(invDtl.Count == 0)
                        return false;

                    string invNo = await GetInvoiceNo(mas.LocId);
                    mas.TInvoiceNo = invNo;
                    mas.IsTaxable = true;
                    Tax_Invoice inv = new Tax_Invoice()
                    {
                        RefTransId = TransId,
                        InvoiceNumber = "",
                        BuyerCNIC = (mas.CustCNIC ?? "").Replace("-", ""),
                        BuyerName = mas.CustName,
                        BuyerNTN = mas.CustNTN,
                        BuyerPhoneNumber = "92" + mas.CustCellNo.Replace("-", "").Substring(1, 10),
                        DateTime = mas.TransDate,
                        Discount = totalDisc,
                        FurtherTax = 0,
                        InvoiceType = (new int[] { 1, 5, 11 }).Contains(mas.TransactionTypeId) ? 1 : 3,//new 1 debit 2 credit 3
                        PaymentMode = mas.PaymentModeId == 1 ? 1 : mas.PaymentModeId == 2 ? 2 : mas.PaymentModeId == 3 ? 6 : 1,//Cash 1, Card 2, Cheque 6
                        POSID = loc.FBRPOSID ?? 0,
                        RefUSIN = refUSIN,
                        TotalBillAmount = totalBillAmount,
                        TotalSaleValue = totalSaleValue,
                        TotalTaxCharged = totalTaxCharged,
                        USIN = invNo,
                        TotalQuantity = totalQuantity,
                        Tax_InvoiceItems = invDtl,
                        SyncStatus = false,
                        Type = "C",
                        LocId = mas.LocId
                    };

                    db.Tax_Invoice.Add(inv);
                    await db.SaveChangesAsync();

                }
                else if (Type == "I")
                {
                    var mas = await db.Lse_Master.FindAsync(TransId);
                    if (!(db.spget_Taxable(TransId, Type).FirstOrDefault().Value))
                    {
                        mas.IsTaxable = false;
                        return false;
                    }

                    var dtlLst = mas.Lse_Detail.ToList();
                    var loc = await db.Comp_Locations.FindAsync(mas.LocId);
                    if (loc.FBRPOSID == null)
                    {
                        return false;
                    }

                    decimal totalBillAmount = 0;
                    decimal totalTaxCharged = 0;
                    int totalQuantity = 0;
                    decimal totalSaleValue = 0;

                    List<Tax_InvoiceItems> invDtl = new List<Tax_InvoiceItems>();
                    foreach (var x in dtlLst)
                    {
                        if (x.MRP == 0)
                            continue;

                        Tax_InvoiceItems tbl = new Tax_InvoiceItems();
                        tbl.RefTransDtlId = x.DtlId;
                        tbl.Discount = x.Discount;
                        tbl.FurtherTax = 0;
                        tbl.InvoiceType = 1;
                        var itm = await db.Inv_Store.FindAsync(x.ItemId);
                        if (itm.Itm_Master.SKUCode.Length > 50)
                        {
                            tbl.ItemCode = itm.Itm_Master.SKUCode.Substring(0, 50);
                        }
                        else
                        {
                            tbl.ItemCode = itm.Itm_Master.SKUCode;
                        }
                        if (itm.Itm_Master.Itm_Model.TypeId == 1001)
                        {
                            tbl.PCTCode = "85171219";
                        }
                        else
                        {
                            tbl.PCTCode = itm.Itm_Master.Itm_Model.Itm_Type.Itm_Products.PCTCode;
                        }
                        tbl.Quantity = x.Qty;
                        tbl.RefUSIN = "";
                        tbl.SaleValue = x.MRP - x.Tax;
                        tbl.TaxCharged = x.Tax;
                        tbl.TaxRate = (double)(itm.Tax ?? 0);
                        tbl.TotalAmount = x.MRP;
                        tbl.ItemName = itm.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName;

                        totalBillAmount = totalBillAmount + tbl.TotalAmount;
                        totalTaxCharged = totalTaxCharged + tbl.TaxCharged;
                        totalQuantity = totalQuantity + tbl.Quantity;
                        totalSaleValue = totalSaleValue + tbl.SaleValue;

                        invDtl.Add(tbl);

                    }

                    //"92" + mas.Mobile1.Replace("-", "").Substring(1, 11),
                    if (invDtl.Count == 0)
                        return false;

                    string invNo = await GetInvoiceNo(mas.LocId);
                    mas.TInvoiceNo = invNo;
                    mas.IsTaxable = true;

                    Tax_Invoice inv = new Tax_Invoice()
                    {
                        RefTransId = TransId,
                        InvoiceNumber = "",
                        BuyerCNIC = mas.NIC.Replace("-", ""),
                        BuyerName = mas.CustName,
                        BuyerNTN = "",
                        BuyerPhoneNumber = "92" + mas.Mobile1.Replace("-", "").Substring(1, 10),
                        DateTime = (DateTime)mas.DeliveryTransDate,
                        Discount = mas.Discount,
                        FurtherTax = 0,
                        InvoiceType = 1,//new 1 debit 2 credit 3
                        PaymentMode = 1,//Cash 1, Card 2, Cheque 6
                        POSID = loc.FBRPOSID ?? 0,
                        RefUSIN = "",
                        TotalBillAmount = totalBillAmount,
                        TotalSaleValue = totalSaleValue,
                        TotalTaxCharged = totalTaxCharged,
                        USIN = invNo,
                        TotalQuantity = totalQuantity,
                        Tax_InvoiceItems = invDtl,
                        SyncStatus = false,
                        Type = "I",
                        LocId = mas.LocId
                    };

                    db.Tax_Invoice.Add(inv);
                    await db.SaveChangesAsync();
                }
                else if (Type == "R")
                {
                    var ret = await db.Lse_Return.FindAsync(TransId);
                    var mas = await db.Lse_Master.FindAsync(ret.AccNo);
                    if (!(db.spget_Taxable(TransId, Type).FirstOrDefault().Value))
                    {
                        ret.IsTaxable = false;
                        return false;
                    }
                    var refUSIN = "";

                    if (ret.ItemType == "O")
                    {
                        return false;
                    }
                    //var preSale = await db.Inv_Sale.FindAsync(mas.RefSaleId);
                    if (mas.TInvoiceNo == null)
                    {
                        return false;
                    }
                    else
                    {
                        refUSIN = mas.TInvoiceNo;
                    }

                    var dtlLst = mas.Lse_Detail.ToList();
                    var loc = await db.Comp_Locations.FindAsync(mas.LocId);
                    if (loc.FBRPOSID == null)
                    {
                        return false;
                    }

                    decimal totalBillAmount = 0;
                    decimal totalTaxCharged = 0;
                    int totalQuantity = 0;
                    decimal totalSaleValue = 0;

                    List<Tax_InvoiceItems> invDtl = new List<Tax_InvoiceItems>();
                    foreach (var x in dtlLst)
                    {
                        if (x.MRP == 0)
                            continue;

                        Tax_InvoiceItems tbl = new Tax_InvoiceItems();
                        tbl.RefTransDtlId = x.DtlId;
                        tbl.Discount = x.Discount;
                        tbl.FurtherTax = 0;
                        tbl.InvoiceType = 3;
                        var itm = await db.Inv_Store.FindAsync(x.ItemId);
                        if (itm.Itm_Master.SKUCode.Length > 50)
                        {
                            tbl.ItemCode = itm.Itm_Master.SKUCode.Substring(0, 50);
                        }
                        else
                        {
                            tbl.ItemCode = itm.Itm_Master.SKUCode;
                        }
                        if (itm.Itm_Master.Itm_Model.TypeId == 1001)
                        {
                            tbl.PCTCode = "85171219";
                        }
                        else
                        {
                            tbl.PCTCode = itm.Itm_Master.Itm_Model.Itm_Type.Itm_Products.PCTCode;
                        }
                        tbl.Quantity = x.Qty;
                        tbl.RefUSIN = refUSIN;
                        tbl.SaleValue = x.MRP - x.Tax;
                        tbl.TaxCharged = x.Tax;
                        tbl.TaxRate = (double)(itm.Tax ?? 0);
                        tbl.TotalAmount = x.MRP;
                        tbl.ItemName = itm.Itm_Master.Itm_Model.Itm_Type.Itm_Products.ProductName;

                        totalBillAmount = totalBillAmount + tbl.TotalAmount;
                        totalTaxCharged = totalTaxCharged + tbl.TaxCharged;
                        totalQuantity = totalQuantity + tbl.Quantity;
                        totalSaleValue = totalSaleValue + tbl.SaleValue;

                        invDtl.Add(tbl);

                    }

                    //"92" + mas.Mobile1.Replace("-", "").Substring(1, 11),

                    if (invDtl.Count == 0)
                        return false;

                    string invNo = await GetInvoiceNo(mas.LocId);
                    ret.TInvoiceNo = invNo;
                    ret.IsTaxable = true;

                    Tax_Invoice inv = new Tax_Invoice()
                    {
                        RefTransId = TransId,
                        InvoiceNumber = "",
                        BuyerCNIC = mas.NIC.Replace("-", ""),
                        BuyerName = mas.CustName,
                        BuyerNTN = "",
                        BuyerPhoneNumber = "92" + mas.Mobile1.Replace("-", "").Substring(1, 10),
                        DateTime = ret.TransDate,
                        Discount = mas.Discount,
                        FurtherTax = 0,
                        InvoiceType = 3,//new 1 debit 2 credit 3
                        PaymentMode = 1,//Cash 1, Card 2, Cheque 6
                        POSID = loc.FBRPOSID ?? 0,
                        RefUSIN = refUSIN,
                        TotalBillAmount = totalBillAmount,
                        TotalSaleValue = totalSaleValue,
                        TotalTaxCharged = totalTaxCharged,
                        USIN = invNo,
                        TotalQuantity = totalQuantity,
                        Tax_InvoiceItems = invDtl,
                        SyncStatus = false,
                        Type = "R",
                        LocId = mas.LocId
                    };

                    db.Tax_Invoice.Add(inv);
                    await db.SaveChangesAsync();
                }
                await PostInvoice(TransId, Type);

                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.Message);
                return false;
            }
        }
        public async Task<bool> WriteLog(string trace, string strMessage)
        {
            try
            {
                FileStream objFilestream = new FileStream("D:\\Web\\logs.txt", FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                await objStreamWriter.WriteLineAsync(DateTime.Now.ToString()+ "--" + trace + "--" + strMessage);
                objStreamWriter.Close();
                objFilestream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> PostInvoice(long RefTransId, string Type)
        {
            try
            {
                var v = await db.Tax_Invoice.Where(x => x.RefTransId == RefTransId && x.Type == Type && x.SyncStatus == false).SingleOrDefaultAsync();
                if (v != null)
                {
                    Invoice objInv = new Invoice()
                    {
                        InvoiceNumber = v.InvoiceNumber,
                        BuyerCNIC = v.BuyerCNIC,
                        BuyerName = v.BuyerName,
                        BuyerNTN = v.BuyerNTN,
                        BuyerPhoneNumber = v.BuyerPhoneNumber,
                        DateTime = v.DateTime,
                        Discount = v.Discount,
                        FurtherTax = v.FurtherTax,
                        InvoiceType = v.InvoiceType,
                        PaymentMode = v.PaymentMode,
                        POSID = v.POSID,
                        RefUSIN = v.RefUSIN,
                        TotalBillAmount = v.TotalBillAmount,
                        TotalSaleValue = v.TotalSaleValue,
                        TotalTaxCharged = v.TotalTaxCharged,
                        USIN = v.USIN,
                        TotalQuantity = v.TotalQuantity
                    };

                    objInv.Items = db.Tax_InvoiceItems.Where(x => x.TransId == v.TransId).Select(x => new InvoiceItems
                    {
                        Discount = x.Discount,
                        FurtherTax = x.FurtherTax,
                        InvoiceType = x.InvoiceType,
                        ItemCode = x.ItemCode,
                        PCTCode = x.PCTCode,
                        Quantity = x.Quantity,
                        RefUSIN = x.RefUSIN,
                        SaleValue = x.SaleValue,
                        TaxCharged = x.TaxCharged,
                        TaxRate = x.TaxRate,
                        TotalAmount = x.TotalAmount,
                        ItemName = x.ItemName
                    }).ToList();

                    StringContent content = new StringContent(JsonConvert.SerializeObject(objInv), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync("https://gw.fbr.gov.pk/imsp/v1/api/Live/PostData", content).Result;
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var str = response.Content.ReadAsStringAsync().Result;
                            FBRResponse resp = JsonConvert.DeserializeObject<FBRResponse>(response.Content.ReadAsStringAsync().Result);

                            if (resp.Code == 100)
                            {
                                v.SyncDate = DateTime.Now;
                                v.SyncStatus = true;
                            }
                            v.InvoiceNumber = resp.InvoiceNumber ?? "";
                            v.Code = resp.Code;
                            v.Response = resp.Response;
                            await db.SaveChangesAsync();

                            if (Type == "C")
                            {
                                var mas = await db.Inv_Sale.FindAsync(RefTransId);
                                mas.FBRInvoiceNo = resp.InvoiceNumber ?? "";
                                mas.Response = resp.Response;
                            }
                            else if (Type == "I")
                            {
                                var mas = await db.Lse_Master.FindAsync(RefTransId);
                                mas.FBRInvoiceNo = resp.InvoiceNumber ?? "";
                            }
                            else if (Type == "R")
                            {
                                var mas = await db.Lse_Return.FindAsync(RefTransId);
                                mas.FBRInvoiceNo = resp.InvoiceNumber ?? "";
                            }
                        }
                        else
                        {
                            v.Response =  response.ReasonPhrase.Substring(0,Math.Min(response.ReasonPhrase.Length,200));
                        }
                    }
                    else
                    {
                        v.Response = "Null Response";
                    }
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.Message);
                return false;
            }
        }

      
        public async Task<bool> PostPending(List<long> Lst)
        {
            try
            {
               
                var lst = await db.Tax_Invoice.Where(x => x.SyncStatus == false && Lst.Contains(x.TransId)).ToListAsync();
                foreach (var v in lst)
                {
                    await PostInvoice(v.RefTransId, v.Type);
                }
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.Message);
                return false;
            }
        }
        public async Task<bool> PostPendingFBR()
        {
            try
            {
                var lst = new List<AdvanceVM> {
                    new AdvanceVM{AccNo = 1962930,CustName = "I" },
                    new AdvanceVM{AccNo = 1964963,CustName = "I" },
                    new AdvanceVM{AccNo = 1969499,CustName = "I" },
                    new AdvanceVM{AccNo = 1963482,CustName = "I" },
                    new AdvanceVM{AccNo = 4791,CustName = "R" }
                };
                foreach (var v in lst)
                {
                    await PostToFBR(v.AccNo, v.CustName);
                }
                return true;
            }
            catch (Exception ex)
            {
                await WriteLog(ex.StackTrace, ex.Message);
                return false;
            }
        }
        public async Task<List<CashSaleVM>> GetPendingInvoices(int locid, DateTime fromdate, DateTime todate)
        {
            try
            {
                todate = todate.AddDays(1);
                var lst = await db.Tax_Invoice.Where(x => (locid == 0 || x.LocId == locid) && x.DateTime >= fromdate.Date && x.DateTime <= todate.Date && !x.SyncStatus
                ).Select(x => new CashSaleVM()
                {
                    TransId = x.TransId,
                    BillNo = x.USIN,
                    Amount = x.TotalBillAmount,
                    Customer = x.BuyerName,
                    FBR = x.InvoiceNumber,
                    IsReturn = x.InvoiceType == 1?"Sale":"Return",
                    USIN = x.USIN,
                    CNIC = x.BuyerCNIC,
                    Mobile = x.BuyerPhoneNumber,
                    Type = x.Type == "C"?"Cash": x.Type == "I" ? "Inst Sale":"Inst Return",
                    DateTime = x.DateTime
                }).ToListAsync();
                return lst;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<bool> UpdatePendingInvoices(CashSaleVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Tax_Invoice.Where(x => x.TransId == mod.TransId).FirstOrDefaultAsync();
                if (!tbl.SyncStatus)
                {
                    tbl.InvoiceNumber = mod.FBR;
                    tbl.SyncStatus = true;
                    tbl.SyncDate = DateTime.Now;
                    tbl.Response = "Modified By " + UserId.ToString();
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

  
}
