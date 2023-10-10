using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AGEERP.Models
{
    public class DashboardBL
    {
        AGEEntities db = new AGEEntities();

        public List<spRep_AllBranchesDailySale_Result> GetAllBranchesDailySale(DateTime dt)
        {
            try
            {
                return db.spRep_AllBranchesDailySale(dt).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<spDASH_LocSaleVsTarget_Result> GetSaleVsTarget(int EmpId)
        {
            try
            {
                return db.spDASH_LocSaleVsTarget(EmpId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<spRep_RecOffPerfForMobile_Result> GetRecOffPerfForMobile(int EmpId)
        {
            try
            {
                var LocId = db.Pay_EmpMaster.Where(x => x.EmpId == EmpId).Select(x => x.DeptId).Single();
                return db.spRep_RecOffPerfForMobile(DateTime.Now.Date,LocId,EmpId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<spRep_EmployeePerf_V1_Result> GetEmployeePerf(int LocId)
        {
            try
            {
                var dt = Convert.ToDateTime(DateTime.Now.Year.ToString()+ "-"+DateTime.Now.Month.ToString()+ "-01");
                return db.spRep_EmployeePerf_V1(dt, DateTime.Now.Date, LocId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool IsBranch(int LocId)
        {
            try
            {
                var locType = db.Comp_Locations.Where(x => x.LocId == LocId).Select(x => x.LocTypeId).First();
                if (locType == 1)
                    return true;
                else
                   return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public dynamic GetBranchDashboard(int LocId)
        {
            try
            {
                //SetupBL setupBL = new SetupBL();
                DateTime dt = new SetupBL().GetWorkingDate(LocId);
                var lst = db.spDash_TwoMonthSale(LocId).ToList();
                var policy = db.Comp_Locations.Find(LocId).FreshMonth;
                var tdCashSale = lst.Where(x => x.Date == dt).Sum(x => x.Sale);
                var mnCashSale = lst.Where(x => x.Date.Value.Month == dt.Month && x.Date.Value.Year == dt.Year).Sum(x => x.Sale);
                var tdInstSale = lst.Where(x => x.Date == dt).Sum(x => x.DeliveryAmt);
                var mnInstSale = lst.Where(x => x.Date.Value.Month == dt.Month && x.Date.Value.Year == dt.Year).Sum(x => x.DeliveryAmt);

                var cDay = dt.Day;
                var PMonth = dt.AddMonths(-1).Month;
                var CMonth = dt.Month;
                var PYear = dt.AddYears(-1).Year;
                var CYear = dt.Year;
                List<TwoMonthSaleVM> ls = new List<TwoMonthSaleVM>();
                decimal ps = 0;
                decimal? cs = 0;
                decimal? pys = 0;
                for (int i = 1; i <= 31; i++)
                {
                    var p = lst.Where(x => x.Date.Value.Month == PMonth && x.Date.Value.Day == i).Select(x => x.Sale).FirstOrDefault();
                    ps += (p ?? 0);
                    var py = lst.Where(x => x.Date.Value.Month == CMonth && x.Date.Value.Year == PYear && x.Date.Value.Day == i).Select(x => x.Sale).FirstOrDefault();
                    pys += (py ?? 0);
                    if (cDay < i)
                    {
                        cs = null;
                    }
                    else
                    {
                        var c = lst.Where(x => x.Date.Value.Month == CMonth && x.Date.Value.Year == CYear && x.Date.Value.Day == i).Select(x => x.Sale).FirstOrDefault();
                        cs += (c ?? 0);
                    }
                    ls.Add(new TwoMonthSaleVM
                    {
                        Day = i,
                        PSale = ps,
                        CSale = cs,
                        PYSale = pys
                    });
                }
                var tmCashSale = ls;
                ls = new List<TwoMonthSaleVM>();
                ps = 0;
                cs = 0;
                pys = 0;
                for (int i = 1; i <= 31; i++)
                {
                    var p = lst.Where(x => x.Date.Value.Month == PMonth && x.Date.Value.Day == i).Select(x => x.DeliveryAmt).FirstOrDefault();
                    ps += (p ?? 0);
                    var py = lst.Where(x => x.Date.Value.Month == CMonth && x.Date.Value.Year == PYear && x.Date.Value.Day == i).Select(x => x.DeliveryAmt).FirstOrDefault();
                    pys += (py ?? 0);
                    if (cDay < i)
                    {
                        cs = null;
                    }
                    else
                    {
                        var c = lst.Where(x => x.Date.Value.Month == CMonth && x.Date.Value.Year == CYear && x.Date.Value.Day == i).Select(x => x.DeliveryAmt).FirstOrDefault();
                        cs += (c ?? 0);
                    }
                    ls.Add(new TwoMonthSaleVM
                    {
                        Day = i,
                        PSale = ps,
                        CSale = cs,
                        PYSale = pys
                    });
                }
                var tmInstSale = ls;

                var outs = db.Lse_Outstand.Where(x => x.LocId == LocId && x.OutstandDate.Month == dt.Month && x.OutstandDate.Year == dt.Year).GroupBy(x => x.LocId).Select(x => new { OutstandAmt = x.Sum(a => a.OutstandAmt), RecvAmt = x.Sum(a => a.RecvAmt ?? 0) }).FirstOrDefault();
                decimal recovery = 0;
                if(outs != null)
                {
                    recovery = Math.Round(outs.RecvAmt/ outs.OutstandAmt * 100,2);
                }
                var pendingGrn = (from PO in db.Inv_PO
                          join POD in db.Inv_PODetail on PO.POId equals POD.POId
                          join POS in db.Inv_POSchedule on POD.PODtlId equals POS.PODtlId
                          where PO.Status >= 3 && PO.Status <= 5 && PO.ApprovedBy > 0 && POS.OrderQty > POS.ReceivedQty
                          && POS.LocId == LocId
                          select (int?)(POS.OrderQty - POS.ReceivedQty)).Sum() ?? 0;

                var pendingStockReceive = (from PO in db.Inv_Issue
                                  join POD in db.Inv_IssueDetail on PO.TransId equals POD.TransId
                                  where PO.ToLocId == LocId && POD.Status == "I" && PO.Status == "I"
                                  select (int?)POD.Qty).Sum() ?? 0;

                var pendingStockIssue = (from PO in db.Inv_Issue
                                    join POD in db.Inv_IssueDetail on PO.TransId equals POD.TransId
                                    where PO.FromLocId == LocId && POD.Status == "I" && PO.Status == "I"
                                    select (int?)POD.Qty).Sum() ?? 0;

                var pendingCashReceive = (from PO in db.Lse_CashTransfer
                                    where PO.ToLocId == LocId && PO.Status == "T"
                                    select (decimal?)PO.TransferedCash).Sum() ?? 0;

                var pendingCashIssue = (from PO in db.Lse_CashTransfer
                                        where PO.LocId == LocId && PO.Status == "T"
                                        select (decimal?)PO.TransferedCash).Sum() ?? 0;


                return new
                {
                    tdCashSale = string.Format("{0:#,0}", tdCashSale), 
                    mnCashSale = string.Format("{0:#,0}", mnCashSale), 
                    tdInstSale = string.Format("{0:#,0}", tdInstSale), 
                    mnInstSale = string.Format("{0:#,0}", mnInstSale), 
                    tmCashSale, 
                    tmInstSale, 
                    policy, 
                    recovery,
                    pendingGrn,
                    pendingStockReceive,
                    pendingStockIssue,
                    pendingCashReceive,
                    pendingCashIssue
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public dynamic GetLocDashboard(int LocId)
        {
            var lstMonth = GetSaleMonth(LocId);
            var lstYear = GetSaleYear(LocId);
            var instSaleMonth = GetTwoMonthInstSale(lstMonth);
            var instMonth = GetTwoMonthInst(lstMonth);
            var cashSaleMonth = GetTwoMonthSale(lstMonth);
            var instSaleYear = GetTwoYearInstSale(lstYear);
            var instYear = GetTwoYearInst(lstYear);
            var cashSaleYear = GetTwoYearSale(lstYear);
            return new
            {
                instSaleMonth,
                instMonth,
                cashSaleMonth,
                instSaleYear,
                instYear,
                cashSaleYear
            };
        }
        public async Task<dynamic> GetHDashboard(int LocId)
        {
            try
            {
                //SetupBL setupBL = new SetupBL();
                DateTime dt = DateTime.Now.Date;
                if(LocId > 0)
                {
                    dt = new SetupBL().GetWorkingDate(LocId);
                }
                var lst = db.spDash_TwoMonthSale(LocId).ToList();
                //var policy = db.Comp_Locations.Find(LocId).FreshMonth;
                var tdCashSale = lst.Where(x => x.Date == dt).Sum(x => x.Sale);
                var mnCashSale = lst.Where(x => x.Date.Value.Month == dt.Month && x.Date.Value.Year == dt.Year).Sum(x => x.Sale);
                var tdInstSale = lst.Where(x => x.Date == dt).Sum(x => x.DeliveryAmt);
                var mnInstSale = lst.Where(x => x.Date.Value.Month == dt.Month && x.Date.Value.Year == dt.Year).Sum(x => x.DeliveryAmt);

                var cDay = dt.Day;
                var PMonth = dt.AddMonths(-1).Month;
                var CMonth = dt.Month;
                var PYear = dt.AddYears(-1).Year;
                var CYear = dt.Year;
                List<TwoMonthSaleVM> ls = new List<TwoMonthSaleVM>();
                decimal ps = 0;
                decimal? cs = 0;
                decimal? pys = 0;
                for (int i = 1; i <= 31; i++)
                {
                    var p = lst.Where(x => x.Date.Value.Month == PMonth && x.Date.Value.Day == i).Select(x => x.Sale).FirstOrDefault();
                    ps += (p ?? 0);
                    var py = lst.Where(x => x.Date.Value.Month == CMonth && x.Date.Value.Year == PYear && x.Date.Value.Day == i).Select(x => x.Sale).FirstOrDefault();
                    pys += (py ?? 0);
                    if (cDay < i)
                    {
                        cs = null;
                    }
                    else
                    {
                        var c = lst.Where(x => x.Date.Value.Month == CMonth && x.Date.Value.Year == CYear && x.Date.Value.Day == i).Select(x => x.Sale).FirstOrDefault();
                        cs += (c ?? 0);
                    }
                    ls.Add(new TwoMonthSaleVM
                    {
                        Day = i,
                        PSale = ps,
                        CSale = cs,
                        PYSale = pys
                    });
                }
                var tmCashSale = ls;
                ls = new List<TwoMonthSaleVM>();
                ps = 0;
                cs = 0;
                pys = 0;
                for (int i = 1; i <= 31; i++)
                {
                    var p = lst.Where(x => x.Date.Value.Month == PMonth && x.Date.Value.Day == i).Select(x => x.DeliveryAmt).FirstOrDefault();
                    ps += (p ?? 0);
                    var py = lst.Where(x => x.Date.Value.Month == CMonth && x.Date.Value.Year == PYear && x.Date.Value.Day == i).Select(x => x.DeliveryAmt).FirstOrDefault();
                    pys += (py ?? 0);
                    if (cDay < i)
                    {
                        cs = null;
                    }
                    else
                    {
                        var c = lst.Where(x => x.Date.Value.Month == CMonth && x.Date.Value.Year == CYear && x.Date.Value.Day == i).Select(x => x.DeliveryAmt).FirstOrDefault();
                        cs += (c ?? 0);
                    }
                    ls.Add(new TwoMonthSaleVM
                    {
                        Day = i,
                        PSale = ps,
                        CSale = cs,
                        PYSale = pys
                    });
                }
                var tmInstSale = ls;

                var outs = await db.Lse_Outstand.Where(x => x.OutstandDate.Month == dt.Month && x.OutstandDate.Year == dt.Year && (LocId == 0 || x.LocId == LocId)).GroupBy(x => 1).Select(x => new { OutstandAmt = x.Sum(a => a.OutstandAmt), RecvAmt = x.Sum(a => a.RecvAmt ?? 0) }).FirstOrDefaultAsync();
                decimal recovery = 0;
                if (outs != null)
                {
                    recovery = Math.Round(outs.RecvAmt / outs.OutstandAmt * 100, 2);
                }
              


                return new
                {
                    tdCashSale = string.Format("{0:#,0}", tdCashSale),
                    mnCashSale = string.Format("{0:#,0}", mnCashSale),
                    tdInstSale = string.Format("{0:#,0}", tdInstSale),
                    mnInstSale = string.Format("{0:#,0}", mnInstSale),
                    tmCashSale,
                    tmInstSale,
                    recovery
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<spDash_TwoMonthSale_Result> GetSaleMonth(int LocID)
        {
            try
            {
                return db.spDash_TwoMonthSale(LocID).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<spDash_TwoYearSale_Result> GetSaleYear(int LocID)
        {
            try
            {
                return db.spDash_TwoYearSale(LocID).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<DashboardVM>> GetDashboard(int TypeId)
        {
            try
            {
                return await db.Comp_Dashboard.Where(x => x.TypeId == TypeId && x.Status).Select(x =>
                new DashboardVM
                {
                    ImgPath = x.ImgPath,
                    Link = x.Link,
                    Title = x.Title,
                    LinkCode = x.LinkCode
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> GetDash(string LinkCode)
        {
            try
            {
                return await db.Comp_Dashboard.Where(x => x.LinkCode == LinkCode && x.Status).Select(x => x.Link).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<DashboardCloudVM>> GetDashboardCloud()
        {
            try
            {
                string[] arr = new string[] { "#FF0000", "#00FFFF", "#C0C0C0", "#0000FF", "#808080", "#0000A0",
            "#000000","#ADD8E6","#FFA500","#800080","#A52A2A","#DE9A04","#800000","#00FF00","#008000","#FF00FF","#808000","#98AFC7","#43C6DB","#89C35C",};

                var lst = await (from item in db.Inv_Store
                                 join comloc in db.Comp_Locations on item.LocId equals comloc.LocId
                                 where comloc.Status == true && item.Inv_Status.MFact == 1 && comloc.LocId != 191
                                 group item by new { comloc.CityId, comloc.LocCode } into g
                                 select new
                                 {
                                     text = g.Key.LocCode,
                                     weight = Math.Round(g.Sum(pc => pc.Qty * pc.MRP) / 1000000, 3).ToString(),
                                     color = g.Key.CityId - 1
                                 }).ToListAsync();
                return lst.Select(x => new DashboardCloudVM()
                {
                    text = x.text,
                    weight = x.weight,
                    color = arr[x.color]
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string GetTableauStringAsync()
        {
            string postData = "username=Administrator";
            byte[] data = System.Text.Encoding.ASCII.GetBytes(postData);
            var myTicket = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://192.168.77.38/trusted");

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = postData.Length;

                Stream outStream = req.GetRequestStream();
                outStream.Write(data, 0, data.Length);
                outStream.Close();

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader inStream = new StreamReader(res.GetResponseStream());
                string resString = inStream.ReadToEnd();
                inStream.Close();

                myTicket = resString;
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
                string innerException = ex.InnerException.Message;

                myTicket = "ERROR";
            }

            return myTicket;
        }

        /////////////////////////////////////////////////////////////////////////////
        ///
        public List<TwoMonthSaleVM> GetTwoMonthSale(List<spDash_TwoMonthSale_Result> lst)
        {
            var cDay = DateTime.Now.Day;
            //var lst = dashboardBL.GetSaleMonth(LocID);
            var PMonth = DateTime.Now.AddMonths(-1).Month;
            var CMonth = DateTime.Now.Month;
            var CYear = DateTime.Now.Year;
            List<TwoMonthSaleVM> ls = new List<TwoMonthSaleVM>();
            decimal ps = 0;
            decimal? cs = 0;
            for (int i = 1; i <= 31; i++)
            {
                var p = lst.Where(x => x.Date.Value.Month == PMonth && x.Date.Value.Day == i).Select(x => x.Sale).FirstOrDefault();
                ps += (p ?? 0);
                if (cDay < i)
                {
                    cs = null;
                }
                else
                {
                    var c = lst.Where(x => x.Date.Value.Month == CMonth && x.Date.Value.Day == i && x.Date.Value.Year == CYear).Select(x => x.Sale).FirstOrDefault();
                    cs += (c ?? 0);
                }
                ls.Add(new TwoMonthSaleVM
                {
                    Day = i,
                    PSale = ps,
                    CSale = cs
                });
            }
            return ls;
        }
        public List<TwoMonthSaleVM> GetTwoMonthInstSale(List<spDash_TwoMonthSale_Result> lst)
        {
            var cDay = DateTime.Now.Day;
            //var lst = dashboardBL.GetSaleMonth(LocID);
            var PMonth = DateTime.Now.AddMonths(-1).Month;
            var CMonth = DateTime.Now.Month;
            var CYear = DateTime.Now.Year;
            List<TwoMonthSaleVM> ls = new List<TwoMonthSaleVM>();
            decimal ps = 0;
            decimal? cs = 0;
            for (int i = 1; i <= 31; i++)
            {
                var p = lst.Where(x => x.Date.Value.Month == PMonth && x.Date.Value.Day == i).Select(x => x.DeliveryAmt).FirstOrDefault();
                ps += (p ?? 0);
                if (cDay < i)
                {
                    cs = null;
                }
                else
                {
                    var c = lst.Where(x => x.Date.Value.Month == CMonth && x.Date.Value.Day == i && x.Date.Value.Year == CYear).Select(x => x.DeliveryAmt).FirstOrDefault();
                    cs += (c ?? 0);
                }
                ls.Add(new TwoMonthSaleVM
                {
                    Day = i,
                    PSale = ps,
                    CSale = cs
                });
            }
            return ls;
        }
        public List<TwoMonthSaleVM> GetTwoMonthInst(List<spDash_TwoMonthSale_Result> lst)
        {
            var cDay = DateTime.Now.Day;
            //var lst = dashboardBL.GetSaleMonth(LocID);
            var PMonth = DateTime.Now.AddMonths(-1).Month;
            var CMonth = DateTime.Now.Month;
            var CYear = DateTime.Now.Year;
            List<TwoMonthSaleVM> ls = new List<TwoMonthSaleVM>();
            decimal ps = 0;
            decimal? cs = 0;
            for (int i = 1; i <= 31; i++)
            {
                var p = lst.Where(x => x.Date.Value.Month == PMonth && x.Date.Value.Day == i).Select(x => x.InstallmentAmt).FirstOrDefault();
                ps += (p ?? 0);
                if (cDay < i)
                {
                    cs = null;
                }
                else
                {
                    var c = lst.Where(x => x.Date.Value.Month == CMonth && x.Date.Value.Day == i && x.Date.Value.Year == CYear).Select(x => x.InstallmentAmt).FirstOrDefault();
                    cs += (c ?? 0);
                }
                ls.Add(new TwoMonthSaleVM
                {
                    Day = i,
                    PSale = ps,
                    CSale = cs
                });
            }
            return ls;
        }
        public List<TwoMonthSaleVM> GetTwoYearSale(List<spDash_TwoYearSale_Result> lst)
        {
            var cMonth = DateTime.Now.Month;

            var PYear = DateTime.Now.AddYears(-1).Year.ToString("0000");
            var CYear = DateTime.Now.Year.ToString("0000");
            List<TwoMonthSaleVM> ls = new List<TwoMonthSaleVM>();
            decimal ps = 0;
            decimal? cs = 0;
            for (int i = 1; i <= 12; i++)
            {
                var p = lst.Where(x => x.Month.Substring(0, 4) == PYear && x.Month.Substring(4, 2) == i.ToString("00")).Select(x => x.Sale).FirstOrDefault();
                ps = (p ?? 0);
                if (cMonth < i)
                {
                    cs = null;
                }
                else
                {
                    var c = lst.Where(x => x.Month.Substring(0, 4) == CYear && x.Month.Substring(4, 2) == i.ToString("00")).Select(x => x.Sale).FirstOrDefault();
                    cs = (c ?? 0);
                }
                ls.Add(new TwoMonthSaleVM
                {
                    Day = i,
                    PSale = ps,
                    CSale = cs
                });
            }
            return ls;
        }
        public List<TwoMonthSaleVM> GetTwoYearInstSale(List<spDash_TwoYearSale_Result> lst)
        {
            var cMonth = DateTime.Now.Month;
            var PYear = DateTime.Now.AddYears(-1).Year.ToString("0000");
            var CYear = DateTime.Now.Year.ToString("0000");
            List<TwoMonthSaleVM> ls = new List<TwoMonthSaleVM>();
            decimal ps = 0;
            decimal? cs = 0;
            for (int i = 1; i <= 12; i++)
            {
                var p = lst.Where(x => x.Month.Substring(0, 4) == PYear && x.Month.Substring(4, 2) == i.ToString("00")).Select(x => x.DeliveryAmt).FirstOrDefault();
                ps = (p ?? 0);
                if (cMonth < i)
                {
                    cs = null;
                }
                else
                {
                    var c = lst.Where(x => x.Month.Substring(0, 4) == CYear && x.Month.Substring(4, 2) == i.ToString("00")).Select(x => x.DeliveryAmt).FirstOrDefault();
                    cs = (c ?? 0);
                }
                ls.Add(new TwoMonthSaleVM
                {
                    Day = i,
                    PSale = ps,
                    CSale = cs
                });
            }
            return ls;
        }
        public List<TwoMonthSaleVM> GetTwoYearInst(List<spDash_TwoYearSale_Result> lst)
        {
            var cMonth = DateTime.Now.Month;
            var PYear = DateTime.Now.AddYears(-1).Year.ToString("0000");
            var CYear = DateTime.Now.Year.ToString("0000");
            List<TwoMonthSaleVM> ls = new List<TwoMonthSaleVM>();
            decimal ps = 0;
            decimal? cs = 0;
            for (int i = 1; i <= 12; i++)
            {
                var p = lst.Where(x => x.Month.Substring(0, 4) == PYear && x.Month.Substring(4, 2) == i.ToString("00")).Select(x => x.InstallmentAmt).FirstOrDefault();
                ps = (p ?? 0);
                if (cMonth < i)
                {
                    cs = null;
                }
                else
                {
                    var c = lst.Where(x => x.Month.Substring(0, 4) == CYear && x.Month.Substring(4, 2) == i.ToString("00")).Select(x => x.InstallmentAmt).FirstOrDefault();
                    cs = (c ?? 0);
                }
                ls.Add(new TwoMonthSaleVM
                {
                    Day = i,
                    PSale = ps,
                    CSale = cs
                });
            }
            return ls;
        }



    }


}