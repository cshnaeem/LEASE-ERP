using AGEERP.CrReports;
using AGEERP.Models;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AGEERP.Controllers
{
    [RBAC]
    public class ReportController : Controller
    {
        #region Purchase
        public ActionResult PurchaseInvoice()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchaseInvoice(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptRep_PurchaseInvoiceList";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("SuppCatId", (frm["SuppCatId"] == "" ? "0" : frm["SuppCatId"]).ToString()));
            if (frm.AllKeys.Contains("SuppId"))
                para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            else
                para.Add(new ReportParameter("SuppId", "0"));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("Status", (frm["Status"] == "" ? "0" : frm["Status"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult PurchasePrice()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchasePrice(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptStockPurchaseRate";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("Dt", frm["Date"].ToString()));
            if (frm.AllKeys.Contains("SuppId"))
                para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            else
                para.Add(new ReportParameter("SuppId", "0"));
            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProductId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult BackendIncentiveReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BackendIncentiveReport(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptBackendIncentiveReport";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm["SuppId"] != "")
                para.Add(new ReportParameter("SupId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            else
                para.Add(new ReportParameter("SupId", "0"));

            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult PendingPOLocationWise()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PendingPOLocationWise(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptPurchaseLoc";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptPOLocWise_V1";
                    break;
            }
            para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            para.Add(new ReportParameter("CompanyId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProductId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            if (frm.AllKeys.Contains("SKUId"))
                para.Add(new ReportParameter("SKUId", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
            else
                para.Add(new ReportParameter("SKUId", "0"));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult Purchase()
        {
            return View();
        }
        public ActionResult POLocationWise()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult POLocationWise(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptPOLocWise";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("IsPending", (frm["IsPending"] == "true,false" ? "true" : frm["IsPending"])));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("PONo", (frm["PONo"]).ToString()));
            para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("CompanyId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProductId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            if (frm.AllKeys.Contains("SKUId"))
                para.Add(new ReportParameter("SKUId", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
            else
                para.Add(new ReportParameter("SKUId", "0"));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptPurBillWise";
                    para.Add(new ReportParameter("TransId", (frm["BillNo"] == "" ? "0" : frm["BillNo"]).ToString()));
                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptPurBillWise-2";
                    para.Add(new ReportParameter("TransId", (frm["BillNo"] == "" ? "0" : frm["BillNo"]).ToString()));
                    break;
                case 3:
                    Session["rptName"] = "/AGEReports/rptPurDtlBillWise";
                    para.Add(new ReportParameter("TransId", (frm["BillNo"] == "" ? "0" : frm["BillNo"]).ToString()));
                    break;
                case 4:
                    Session["rptName"] = "/AGEReports/rptPurOverall";
                    para.Add(new ReportParameter("TransId", (frm["BillNo"] == "" ? "0" : frm["BillNo"]).ToString()));
                    break;
                case 5:
                    Session["rptName"] = "/AGEReports/rptPurchaseReturn";
                    para.Add(new ReportParameter("TransId", (frm["BillNo"] == "" ? "0" : frm["BillNo"]).ToString()));
                    break;
                case 6:
                    Session["rptName"] = "/AGEReports/rptCompWisePurchase";
                    break;
            }

            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("CompanyId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("TypeId"))
                para.Add(new ReportParameter("TypeId", (frm["TypeId"] == "" ? "0" : frm["TypeId"]).ToString()));
            else
                para.Add(new ReportParameter("TypeId", "0"));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProductId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult POSummary()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult POSummary(FormCollection frm)
        {
            ReportBL reportBL = new ReportBL();
            //int LocId = 0;
            int SuppId = 0;
            int CatId = 0;
            int ComId = 0;
            int TypeId = 0;
            int ProductId = 0;
            int ModelId = 0;
            string criteria = frm["Criteria"];
            //int AgingDays = 0;

            DateTime fromDate = Convert.ToDateTime(frm["FromDate"]);
            DateTime toDate = Convert.ToDateTime(frm["ToDate"]);
            CatId = Convert.ToInt32((frm["CatId"] == "" ? "0" : frm["CatId"]).ToString());
            SuppId = Convert.ToInt32((frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString());
            //int CityId = Convert.ToInt32((frm["CityId"] == "" ? "0" : frm["CityId"]).ToString());
            //if (frm.AllKeys.Contains("LocId"))
            //    LocId = Convert.ToInt32((frm["LocId"] == "" ? "0" : frm["LocId"]).ToString());
            if (frm.AllKeys.Contains("ComId"))
                ComId = Convert.ToInt32((frm["ComId"] == "" ? "0" : frm["ComId"]).ToString());
            if (frm.AllKeys.Contains("TypeId"))
                TypeId = Convert.ToInt32((frm["TypeId"] == "" ? "0" : frm["TypeId"]).ToString());
            if (frm.AllKeys.Contains("ProductId"))
                ProductId = Convert.ToInt32((frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString());
            if (frm.AllKeys.Contains("ModelId"))
                ModelId = Convert.ToInt32((frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString());
            //if (frm.AllKeys.Contains("AgingDays"))
            //    AgingDays = Convert.ToInt32((frm["AgingDays"] == "" ? "0" : frm["AgingDays"]).ToString());

            ReportDocument rrpt = new ReportDocument();
            switch (frm["rpt"])
            {
                case "1":
                    {
                        List<SalesReportRVM> lst = reportBL.GetPOSummary(CatId, SuppId, ComId, ProductId, TypeId, ModelId, fromDate, toDate);
                        rptSalesCompanyWise rpt = new rptSalesCompanyWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "PO Company Product Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Order");
                        rpt.SetParameterValue("H2", "Recv");
                        rpt.SetParameterValue("H3", "Remain");
                        rrpt = rpt;
                        break;
                    }
                case "2":
                    {
                        List<SalesReportRVM> lst = reportBL.GetPOSummary(CatId, SuppId, ComId, ProductId, TypeId, ModelId, fromDate, toDate).ToList();
                        lst = lst.Select(x => new SalesReportRVM
                        {
                            CompName = x.ItemName,
                            ItemName = x.CompName,
                            AQty = x.AQty,
                            Model = x.Model,
                            RQty = x.RQty,
                            SQty = x.SQty
                        }).ToList();
                        rptSalesCompanyWise rpt = new rptSalesCompanyWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "PO Product Company Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Order");
                        rpt.SetParameterValue("H2", "Recv");
                        rpt.SetParameterValue("H3", "Remain");
                        rrpt = rpt;
                        break;
                    }
                case "3":
                    {
                        List<SalesReportRVM> lst = reportBL.GetPOSummary(CatId, SuppId, ComId, ProductId, TypeId, ModelId, fromDate, toDate);
                        lst = lst.Select(x => new SalesReportRVM
                        {
                            CompName = x.ItemName,
                            ItemName = x.CompName,
                            AQty = x.AQty,
                            Model = x.Model,
                            RQty = x.RQty,
                            SQty = x.SQty
                        }).ToList();
                        rptSalesProductWiseNet rpt = new rptSalesProductWiseNet();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "PO Company Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Order");
                        rpt.SetParameterValue("H2", "Recv");
                        rpt.SetParameterValue("H3", "Remain");
                        rrpt = rpt;
                        break;
                    }
                case "4":
                    {
                        List<SalesReportRVM> lst = reportBL.GetPOSummary(CatId, SuppId, ComId, ProductId, TypeId, ModelId, fromDate, toDate);
                        rptSalesProductWiseNet rpt = new rptSalesProductWiseNet();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "PO Product Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Order");
                        rpt.SetParameterValue("H2", "Recv");
                        rpt.SetParameterValue("H3", "Remain");
                        rrpt = rpt;
                        break;
                    }
                case "5":
                    {
                        List<SalesReportRVM> lst = reportBL.GetPOSummary(CatId, SuppId, ComId, ProductId, TypeId, ModelId, fromDate, toDate);
                        rptSalesModelWise rpt = new rptSalesModelWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "PO Company Product Model Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Order");
                        rpt.SetParameterValue("H2", "Recv");
                        rpt.SetParameterValue("H3", "Remain");
                        rrpt = rpt;
                        break;
                    }
                case "6":
                    {
                        List<SalesReportRVM> lst = reportBL.GetPOSummary(CatId, SuppId, ComId, ProductId, TypeId, ModelId, fromDate, toDate);
                        lst = lst.Select(x => new SalesReportRVM
                        {
                            CompName = x.ItemName,
                            ItemName = x.CompName,
                            AQty = x.AQty,
                            Model = x.Model,
                            RQty = x.RQty,
                            SQty = x.SQty
                        }).ToList();
                        rptSalesModelWise rpt = new rptSalesModelWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "PO Product Company Model Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Order");
                        rpt.SetParameterValue("H2", "Recv");
                        rpt.SetParameterValue("H3", "Remain");
                        rrpt = rpt;
                        break;
                    }
            }
            rrpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
            rrpt.Close();
            rrpt.Dispose();
            return Json("");
            //return Redirect("~/CReport.aspx");
        }

        #endregion
        #region Sale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaleStockCityWise(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptSaleStockCityWise";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("ComId"))
                para.Add(new ReportParameter("CompanyId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            else
                para.Add(new ReportParameter("CompanyId", "0"));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProductId", "0"));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult StockComparison()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StockComparison(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptSaleStockComparison";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("SaleType", (frm["SaleType"] == "" ? "0" : frm["SaleType"]).ToString()));
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            if (frm.AllKeys.Contains("SuppId"))
                para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            else
                para.Add(new ReportParameter("SuppId", "0"));
            if (frm.AllKeys.Contains("ComId"))
                para.Add(new ReportParameter("CompanyId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            else
                para.Add(new ReportParameter("CompanyId", "0"));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProductId", "0"));
            if (frm.AllKeys.Contains("TypeId"))
                para.Add(new ReportParameter("TypeId", (frm["TypeId"] == "" ? "0" : frm["TypeId"]).ToString()));
            else
                para.Add(new ReportParameter("TypeId", "0"));

            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            if (frm.AllKeys.Contains("SKUId"))
                para.Add(new ReportParameter("SKUId", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
            else
                para.Add(new ReportParameter("SKUId", "0"));
            para.Add(new ReportParameter("Row", frm["Row"].ToString()));
            para.Add(new ReportParameter("Col", frm["Column"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            //para.Add(new ReportParameter("Days", frm["Days"].ToString()));

            para.Add(new ReportParameter("IsSale", (frm["isSale"] == "true,false" ? "true" : frm["isSale"])));
            para.Add(new ReportParameter("IsStock", (frm["isStock"] == "true,false" ? "true" : frm["isStock"])));
            para.Add(new ReportParameter("IsPurchase", (frm["isPurchase"] == "true,false" ? "true" : frm["isPurchase"])));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SaleStockAging()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaleStockAging(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "1":
                    {
                        Session["rptName"] = "/AGEReports/rptSaleStockAging";
                        if (frm.AllKeys.Contains("CategoryId"))
                            para.Add(new ReportParameter("SuppCatId", (frm["CategoryId"] == "" ? "0" : frm["CategoryId"]).ToString()));
                        else
                            para.Add(new ReportParameter("SuppCatId", "0"));
                        if (frm.AllKeys.Contains("CompanyId"))
                            para.Add(new ReportParameter("CompanyId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
                        else
                            para.Add(new ReportParameter("CompanyId", "0"));
                        if (frm.AllKeys.Contains("SKUId"))
                            para.Add(new ReportParameter("SKUId", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
                        else
                            para.Add(new ReportParameter("SKUId", "0"));
                        break;
                    }
                case "2":
                    {
                        Session["rptName"] = "/AGEReports/rptSaleComProductWiseAging";
                        if (frm.AllKeys.Contains("CategoryId"))
                            para.Add(new ReportParameter("CategoryId", (frm["CategoryId"] == "" ? "0" : frm["CategoryId"]).ToString()));
                        else
                            para.Add(new ReportParameter("CategoryId", "0"));
                        if (frm.AllKeys.Contains("CompanyId"))
                            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
                        else
                            para.Add(new ReportParameter("ComId", "0"));
                        break;
                    }
            }

            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));

            if (frm.AllKeys.Contains("SuppId"))
                para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            else
                para.Add(new ReportParameter("SuppId", "0"));

            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProductId", "0"));
            if (frm.AllKeys.Contains("TypeId"))
                para.Add(new ReportParameter("TypeId", (frm["TypeId"] == "" ? "0" : frm["TypeId"]).ToString()));
            else
                para.Add(new ReportParameter("TypeId", "0"));

            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));

            para.Add(new ReportParameter("Aging", frm["Aging"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult BranchLogReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BranchLogReport(FormCollection frm)
        {
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptInstallmentLog";
                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptOutstandLog";
                    break;
            }
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult MonthlyComparison()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyComparison(FormCollection frm)
        {
            Session.Remove("rptName");
            List<ReportParameter> para = new List<ReportParameter>();
            Session["rptName"] = "/AGEReports/rptMonthlyCompSummary";
            para.Add(new ReportParameter("Location", frm["LocId"].ToString()));
            para.Add(new ReportParameter("DATEMONTH", frm["DATEMONTH"].ToString()));
            para.Add(new ReportParameter("PrevDate", frm["PrevDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult BankLetterPosted()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankLetterPosted(FormCollection frm)
        {
            Session.Remove("rptName");
            List<ReportParameter> para = new List<ReportParameter>();
            Session["rptName"] = "/AGEReports/rptBankLetterPosted";
            para.Add(new ReportParameter("BlId", frm["BlId"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult BankLetter()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankLetter(FormCollection frm)
        {
            Session.Remove("rptName");
            List<ReportParameter> para = new List<ReportParameter>();
            Session["rptName"] = "/AGEReports/rptPay_BankLetter";
            para.Add(new ReportParameter("SalaryMonth", frm["Month"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SaleVoucherVerification()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaleVoucherVerification(FormCollection frm)
        {
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptCashSaleVoucherVerification";
                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptInstSaleVoucherVerification";
                    break;
                case 3:
                    Session["rptName"] = "/AGEReports/rptInstVoucherVerification";
                    break;
            }

            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SaleRegister()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaleRegister(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptSaleRegister";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("CategoryId", (frm["CatId"] == "" ? "0" : frm["CatId"]).ToString()));
            para.Add(new ReportParameter("CityID", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("TypeId"))
                para.Add(new ReportParameter("TypeId", (frm["TypeId"] == "" ? "0" : frm["TypeId"]).ToString()));
            else
                para.Add(new ReportParameter("TypeId", "0"));
            if (frm.AllKeys.Contains("ProdId"))
                para.Add(new ReportParameter("ProdId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProdId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            if (frm["rpt"] == "CashSale")
                para.Add(new ReportParameter("SaleType", "Cash".ToString()));
            else
                para.Add(new ReportParameter("SaleType", "Lease".ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SaleComparison()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SaleComparison(FormCollection frm)
        {
            int CityId = 0, LocId = 0, SuppId = 0, CompanyId = 0, ProductId = 0, TypeId = 0, ModelId = 0, SKUId = 0;
            string SaleType = "";
            DateTime FromDate, ToDate;
            if (frm.AllKeys.Contains("CityId"))
                CityId = Convert.ToInt32((frm["CityId"] == "" ? "0" : frm["CityId"]).ToString());
            if (frm.AllKeys.Contains("LocId"))
                LocId = Convert.ToInt32(((frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            if (frm.AllKeys.Contains("SuppId"))
                SuppId = Convert.ToInt32(((frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            if (frm.AllKeys.Contains("CompanyId"))
                CompanyId = Convert.ToInt32(((frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("ProductId"))
                ProductId = Convert.ToInt32(((frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            if (frm.AllKeys.Contains("TypeId"))
                TypeId = Convert.ToInt32(((frm["TypeId"] == "" ? "0" : frm["TypeId"]).ToString()));
            if (frm.AllKeys.Contains("ModelId"))
                ModelId = Convert.ToInt32(((frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            if (frm.AllKeys.Contains("SKUId"))
                SKUId = Convert.ToInt32(((frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
            if (frm.AllKeys.Contains("SaleType"))
                SaleType = frm["SaleType"].ToString();

            FromDate = Convert.ToDateTime((frm["FromDate"].ToString()));
            ToDate = Convert.ToDateTime(frm["ToDate"].ToString());

            ReportBL reportBL = new ReportBL();
            ViewBag.dat = reportBL.GetSaleAll(CityId, LocId, 0, SuppId, CompanyId, ProductId, TypeId, ModelId, SKUId, FromDate, ToDate, SaleType);
            return View("_SaleComparison");
        }
        public ActionResult Sale()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sale(FormCollection frm)
        {
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptSalesDualPkg";
                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptSalesMorethanOne";
                    break;
                case 3:
                    Session["rptName"] = "/AGEReports/rptSalesProfit";
                    break;
                case 4:
                    Session["rptName"] = "/AGEReports/rptSalesRatio";
                    break;
                case 5:
                    Session["rptName"] = "/AGEReports/rptSalesReportDetailWise";
                    break;
                case 6:
                    Session["rptName"] = "/AGEReports/rptSalesReportLocationWise";
                    break;
                case 7:
                    Session["rptName"] = "/AGEReports/rptSaleProductWise_Value";
                    break;
                case 8:
                    Session["rptName"] = "/AGEReports/rptInstallmentAndCashSaleComp";
                    break;
                case 9:
                    Session["rptName"] = "/AGEReports/rptSalesReturnReportDetailWise";
                    break;
                case 10:
                    Session["rptName"] = "/AGEReports/rptSaleModelWise_Value";
                    break;
                case 11:
                    Session["rptName"] = "/AGEReports/rptLocWiseProductSale";
                    break;
                case 12:
                    Session["rptName"] = "/AGEReports/rptSaleCompanyWise_Value";
                    break;
                case 13:
                    Session["rptName"] = "/AGEReports/rptSaleAnalysisCityWise";
                    break;
                case 14:
                    Session["rptName"] = "/AGEReports/rptSaleAnalysisLocWise";
                    break;
            }

            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            //para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("CompanyId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("TypeId"))
                para.Add(new ReportParameter("TypeId", (frm["TypeId"] == "" ? "0" : frm["TypeId"]).ToString()));
            else
                para.Add(new ReportParameter("TypeId", "0"));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProductId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            if (!(Convert.ToInt32(frm["rpt"]) == 13 || Convert.ToInt32(frm["rpt"]) == 14))
            {
                if (Convert.ToInt32(frm["rpt"]) == 7 || Convert.ToInt32(frm["rpt"]) == 8 || Convert.ToInt32(frm["rpt"]) == 9 || Convert.ToInt32(frm["rpt"]) == 10 || Convert.ToInt32(frm["rpt"]) == 11 || Convert.ToInt32(frm["rpt"]) == 12)
                {
                    para.Add(new ReportParameter("SaleType", (frm["SaleType"] == "" ? "" : frm["SaleType"]).ToString()));
                }
                else
                {
                    para.Add(new ReportParameter("TransId", (frm["BillNo"] == "" ? "0" : frm["BillNo"]).ToString()));
                }
            }



            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SaleSummary()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaleSummary(FormCollection frm)
        {
            ReportBL reportBL = new ReportBL();
            int LocId = 0;
            int ComId = 0;
            int TypeId = 0;
            int ProductId = 0;
            int ModelId = 0;
            string criteria = frm["Criteria"];
            int AgingDays = 0;
            int SuppId = 0;
            int CatId = 0;

            DateTime fromDate = Convert.ToDateTime(frm["FromDate"]);
            DateTime toDate = Convert.ToDateTime(frm["ToDate"]);
            //para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            int CityId = Convert.ToInt32((frm["CityId"] == "" ? "0" : frm["CityId"]).ToString());
            if (frm.AllKeys.Contains("LocId"))
                LocId = Convert.ToInt32((frm["LocId"] == "" ? "0" : frm["LocId"]).ToString());
            if (frm.AllKeys.Contains("ComId"))
                ComId = Convert.ToInt32((frm["ComId"] == "" ? "0" : frm["ComId"]).ToString());
            if (frm.AllKeys.Contains("TypeId"))
                TypeId = Convert.ToInt32((frm["TypeId"] == "" ? "0" : frm["TypeId"]).ToString());
            if (frm.AllKeys.Contains("ProductId"))
                ProductId = Convert.ToInt32((frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString());
            if (frm.AllKeys.Contains("ModelId"))
                ModelId = Convert.ToInt32((frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString());
            if (frm.AllKeys.Contains("AgingDays"))
                AgingDays = Convert.ToInt32((frm["AgingDays"] == "" ? "0" : frm["AgingDays"]).ToString());

            if (frm.AllKeys.Contains("SuppId"))
                SuppId = Convert.ToInt32((frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString());
            if (frm.AllKeys.Contains("CatId"))
                CatId = Convert.ToInt32((frm["CatId"] == "" ? "0" : frm["CatId"]).ToString());

            ReportDocument rrpt = new ReportDocument();
            switch (frm["rpt"])
            {
                case "1":
                    {
                        List<SalesReportRVM> lst = reportBL.GetSaleAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate, "").Select(x => new SalesReportRVM
                        {
                            AQty = x.Qty,
                            SQty = x.Qty > 0 ? x.Qty : 0,
                            RQty = x.Qty < 0 ? x.Qty * -1 : 0,
                            CompName = x.Company,
                            EasyName = x.LocCode,
                            ItemName = x.Product
                        }).ToList();
                        rptSalesCompanyWise rpt = new rptSalesCompanyWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Sale Company Product Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Sales");
                        rpt.SetParameterValue("H2", "S-Ret");
                        rpt.SetParameterValue("H3", "Actual");
                        rrpt = rpt;
                        break;
                    }
                case "2":
                    {
                        List<SalesReportRVM> lst = reportBL.GetSaleAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate, "").Select(x => new SalesReportRVM
                        {
                            AQty = x.Qty,
                            SQty = x.Qty > 0 ? x.Qty : 0,
                            RQty = x.Qty < 0 ? x.Qty * -1 : 0,
                            CompName = x.Company,
                            EasyName = x.LocCode,
                            ItemName = x.Product
                        }).ToList();
                        rptSalesProductWiseNet rpt = new rptSalesProductWiseNet();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Sale Product Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Sales");
                        rpt.SetParameterValue("H2", "S-Ret");
                        rpt.SetParameterValue("H3", "Actual");
                        rrpt = rpt;

                        break;
                    }
                case "3":
                    {
                        List<SalesReportRVM> lst = reportBL.GetSaleAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate, "").Select(x => new SalesReportRVM
                        {
                            AQty = x.Qty,
                            SQty = x.Qty > 0 ? x.Qty : 0,
                            RQty = x.Qty < 0 ? x.Qty * -1 : 0,
                            CompName = x.Product,
                            EasyName = x.LocCode,
                            ItemName = x.Company
                        }).ToList();
                        rptSalesProductWiseNet rpt = new rptSalesProductWiseNet();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Sale Company Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Sales");
                        rpt.SetParameterValue("H2", "S-Ret");
                        rpt.SetParameterValue("H3", "Actual");
                        rrpt = rpt;
                        break;
                    }
                case "4":
                    {
                        List<SalesReportRVM> lst = reportBL.GetSaleAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate, "").Select(x => new SalesReportRVM
                        {
                            AQty = x.Qty,
                            SQty = x.Qty > 0 ? x.Qty : 0,
                            RQty = x.Qty < 0 ? x.Qty * -1 : 0,
                            CompName = x.Product,
                            EasyName = x.LocCode,
                            ItemName = x.Company
                        }).ToList();
                        rptSalesCompanyWise rpt = new rptSalesCompanyWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Sale Product Company Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Sales");
                        rpt.SetParameterValue("H2", "S-Ret");
                        rpt.SetParameterValue("H3", "Actual");
                        rrpt = rpt;
                        break;
                    }
                case "5":
                    {
                        List<SalesReportRVM> lst = reportBL.GetSaleAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate, "").Select(x => new SalesReportRVM
                        {
                            AQty = x.Qty,
                            SQty = x.Qty > 0 ? x.Qty : 0,
                            RQty = x.Qty < 0 ? x.Qty * -1 : 0,
                            CompName = x.Product,
                            EasyName = x.LocCode,
                            ItemName = x.Company,
                            Model = x.Model
                        }).ToList();
                        rptSalesModelWise rpt = new rptSalesModelWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Sale Product Company Model Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Sales");
                        rpt.SetParameterValue("H2", "S-Ret");
                        rpt.SetParameterValue("H3", "Actual");
                        rrpt = rpt;
                        break;
                    }
                case "6":
                    {
                        List<SalesReportRVM> lst = reportBL.GetSaleAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate, "").Select(x => new SalesReportRVM
                        {
                            AQty = x.Qty,
                            SQty = x.Qty > 0 ? x.Qty : 0,
                            RQty = x.Qty < 0 ? x.Qty * -1 : 0,
                            CompName = x.Company,
                            EasyName = x.LocCode,
                            ItemName = x.Product,
                            Model = x.Model
                        }).ToList();
                        rptSalesModelWise rpt = new rptSalesModelWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Sale Company Product Model Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("H1", "Sales");
                        rpt.SetParameterValue("H2", "S-Ret");
                        rpt.SetParameterValue("H3", "Actual");
                        rrpt = rpt;
                        break;
                    }
                case "11":
                    {
                        List<PurchaseReportRVM> lst = reportBL.GetPurchaseAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate).Select(x => new PurchaseReportRVM
                        {
                            Qty = x.Qty,
                            Company = x.Company,
                            ItemName = x.Product,
                            Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            SRCode = x.LocCode
                        }).ToList();
                        rptPurchaseOverall rpt = new rptPurchaseOverall();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Purchase Company Product Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rrpt = rpt;
                        break;
                    }
                case "12":
                    {
                        List<PurchaseReportRVM> lst = reportBL.GetPurchaseAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate).Select(x => new PurchaseReportRVM
                        {
                            Qty = x.Qty,
                            Company = x.Product,
                            ItemName = x.Company,
                            Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            SRCode = x.LocCode
                        }).ToList();
                        rptPurchaseOverallNet rpt = new rptPurchaseOverallNet();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Purchase Product Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rrpt = rpt;

                        break;
                    }
                case "13":
                    {
                        List<PurchaseReportRVM> lst = reportBL.GetPurchaseAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate).Select(x => new PurchaseReportRVM
                        {
                            Qty = x.Qty,
                            Company = x.Company,
                            ItemName = x.Product,
                            Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            SRCode = x.LocCode
                        }).ToList();
                        rptPurchaseOverall rpt = new rptPurchaseOverall();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Purchase Company Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rrpt = rpt;
                        break;
                    }
                case "14":
                    {
                        List<PurchaseReportRVM> lst = reportBL.GetPurchaseAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate).Select(x => new PurchaseReportRVM
                        {
                            Qty = x.Qty,
                            Company = x.Product,
                            ItemName = x.Company,
                            Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            SRCode = x.LocCode
                        }).ToList();
                        rptPurchaseOverall rpt = new rptPurchaseOverall();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Purchase Product Company Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rrpt = rpt;
                        break;
                    }
                case "15":
                    {
                        List<PurchaseReportRVM> lst = reportBL.GetPurchaseAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate).Select(x => new PurchaseReportRVM
                        {
                            Qty = x.Qty,
                            Company = x.Product,
                            ItemName = x.Company,
                            Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            SRCode = x.LocCode
                        }).ToList();
                        rptPurchaseOverallRegion rpt = new rptPurchaseOverallRegion();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Purchase City Product Company Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rrpt = rpt;
                        break;
                    }
                case "16":
                    {
                        List<PurchaseReportRVM> lst = reportBL.GetPurchaseAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate).Select(x => new PurchaseReportRVM
                        {
                            Qty = x.Qty,
                            Company = x.Company,
                            ItemName = x.Product,
                            Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            SRCode = x.LocCode
                        }).ToList();
                        rptPurchaseOverallRegion rpt = new rptPurchaseOverallRegion();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Purchase City Company Product Model Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rrpt = rpt;
                        break;
                    }
                case "21":
                    {
                        DateTime dt = DateTime.Now.Date;
                        var lst = reportBL.GetStockAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, 0).Select(x => new StockReportRVM
                        {
                            Qty = x.Qty,
                            CompName = x.Company,
                            ItemName = x.Product,
                            //Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            EasyName = x.LocCode,
                            PPrice = x.PPrice,
                            SrNo = x.SerialNo,
                            SuppName = x.Supplier,
                            TypeName = x.Type,
                            AgeDate = x.TrxDate,
                            AgeDays = (x.TrxDate - dt).Days
                        }).ToList();
                        rptStockCompanyWise rpt = new rptStockCompanyWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", "");
                        rpt.SetParameterValue("rptName", "Stock Company Product Wise Report");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rrpt = rpt;
                        break;
                    }
                case "22":
                    {
                        DateTime dt = DateTime.Now.Date;
                        var lst = reportBL.GetStockAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, 0).Select(x => new StockReportRVM
                        {
                            Qty = x.Qty,
                            CompName = x.Product,
                            ItemName = x.Company,
                            //Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            EasyName = x.LocCode,
                            PPrice = x.PPrice,
                            SrNo = x.SerialNo,
                            SuppName = x.Supplier,
                            TypeName = x.Type,
                            AgeDate = x.TrxDate,
                            AgeDays = (x.TrxDate - dt).Days
                        }).ToList();
                        rptStockCompanyWise rpt = new rptStockCompanyWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", "");
                        rpt.SetParameterValue("rptName", "Stock Product Company Wise Report");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rrpt = rpt;
                        break;
                    }
                case "23":
                    {
                        DateTime dt = DateTime.Now.Date;
                        var lst = reportBL.GetStockAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, 0).Select(x => new StockReportRVM
                        {
                            Qty = x.Qty,
                            CompName = x.Company,
                            ItemName = x.Product,
                            //Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            EasyName = x.LocCode,
                            PPrice = x.PPrice,
                            SrNo = x.SerialNo,
                            SuppName = x.Supplier,
                            TypeName = x.Type,
                            AgeDate = x.TrxDate,
                            AgeDays = (x.TrxDate - dt).Days
                        }).ToList();
                        rptStockModelWise rpt = new rptStockModelWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", "");
                        rpt.SetParameterValue("rptName", "Stock Company Product Model Wise Report");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rrpt = rpt;
                        break;
                    }
                case "24":
                    {
                        DateTime dt = DateTime.Now.Date;
                        var lst = reportBL.GetStockAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, 0).Select(x => new StockReportRVM
                        {
                            Qty = x.Qty,
                            CompName = x.Product,
                            ItemName = x.Company,
                            //Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            EasyName = x.LocCode,
                            PPrice = x.PPrice,
                            SrNo = x.SerialNo,
                            SuppName = x.Supplier,
                            TypeName = x.Type,
                            AgeDate = x.TrxDate,
                            AgeDays = (x.TrxDate - dt).Days
                        }).ToList();
                        rptStockModelWise rpt = new rptStockModelWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", "");
                        rpt.SetParameterValue("rptName", "Stock Product Company Model Wise Report");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rrpt = rpt;
                        break;
                    }
                case "25":
                    {
                        DateTime dt = DateTime.Now.Date;

                        var lst = reportBL.GetStockAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, AgingDays).Select(x => new StockReportRVM
                        {
                            Qty = x.Qty,
                            CompName = x.Company,
                            ItemName = x.Product,
                            //Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            EasyName = x.LocCode,
                            PPrice = x.PPrice,
                            SrNo = x.SerialNo,
                            SuppName = x.Supplier,
                            TypeName = x.Type,
                            AgeDate = x.TrxDate,
                            AgeDays = (dt - x.TrxDate).Days
                        }).ToList();
                        rptStockAging rpt = new rptStockAging();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", "");
                        rpt.SetParameterValue("rptName", "Stock Company Product Wise Aging Report");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("rptAging", "Aging Days - " + AgingDays);
                        rrpt = rpt;
                        break;
                    }
                case "26":
                    {
                        DateTime dt = DateTime.Now.Date;
                        var lst = reportBL.GetStockAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, AgingDays).Select(x => new StockReportRVM
                        {
                            Qty = x.Qty,
                            CompName = x.Product,
                            ItemName = x.Company,
                            //Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            EasyName = x.LocCode,
                            PPrice = x.PPrice,
                            SrNo = x.SerialNo,
                            SuppName = x.Supplier,
                            TypeName = x.Type,
                            AgeDate = x.TrxDate,
                            AgeDays = (dt - x.TrxDate).Days
                        }).ToList();
                        rptStockAging rpt = new rptStockAging();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", "");
                        rpt.SetParameterValue("rptName", "Stock Product Company Wise Aging Report");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("rptAging", "Aging Days - " + AgingDays);
                        rrpt = rpt;
                        break;
                    }
                case "27":
                    {
                        DateTime dt = DateTime.Now.Date;
                        var lst = reportBL.GetStockAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, 0).Select(x => new PurchaseReportRVM
                        {
                            Qty = x.Qty,
                            Company = x.Company,
                            ItemName = x.Type,
                            //Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            SRCode = x.LocCode,
                            Amount = x.PPrice,
                            //SrNo = x.SerialNo,
                            //SuppName = x.Supplier,
                            //TypeName = x.Type,
                            //AgeDate = x.TrxDate,
                            //AgeDays = (x.TrxDate - dt).Days
                        }).ToList();
                        //List<PurchaseReportRVM> lst = reportBL.GetPurchaseAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, fromDate, toDate).Select(x => new PurchaseReportRVM
                        //{
                        //    Qty = x.Qty,
                        //    Company = x.Company,
                        //    ItemName = x.Product,
                        //    Amount = x.PPrice,
                        //    City = x.CityCode,
                        //    Model = x.Model,
                        //    SRCode = x.LocCode
                        //}).ToList();
                        rptPurchaseOverall rpt = new rptPurchaseOverall();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", btwDate);
                        rpt.SetParameterValue("rptName", "Stock Product Type Wise");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rrpt = rpt;
                        break;
                    }
                case "28":
                    {
                        DateTime dt = DateTime.Now.Date;

                        var lst = reportBL.GetStockAll(CityId, LocId, CatId, SuppId, ComId, ProductId, TypeId, ModelId, 0, AgingDays).Select(x => new StockReportRVM
                        {
                            Qty = x.Qty,
                            CompName = x.Company,
                            ItemName = x.Product,
                            //Amount = x.PPrice,
                            City = x.CityCode,
                            Model = x.Model,
                            EasyName = x.LocCode,
                            PPrice = x.PPrice,
                            SrNo = x.SerialNo,
                            SuppName = x.Supplier,
                            TypeName = x.Type,
                            AgeDate = x.TrxDate,
                            AgeDays = (dt - x.TrxDate).Days
                        }).ToList();
                        rptStockAgingQtyWise rpt = new rptStockAgingQtyWise();
                        rpt.SetDataSource(lst);
                        string btwDate = fromDate.ToString("dd-MM-yyyy") + " - " + toDate.ToString("dd-MM-yyyy");
                        rpt.SetParameterValue("btwDate", "");
                        rpt.SetParameterValue("rptName", "Stock Company Product Wise Aging Report");
                        rpt.SetParameterValue("rptCriteria", criteria);
                        rpt.SetParameterValue("rptAging", "Aging Days - " + AgingDays);
                        rrpt = rpt;
                        break;
                    }
            }
            rrpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
            rrpt.Close();
            rrpt.Dispose();
            return Json("");
            //return Redirect("~/CReport.aspx");
        }
        public ActionResult SaleRateList()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaleRateList(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();

            Session["rptName"] = "/AGEReports/rptSaleRateList";

            para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SaleRateAllList()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaleRateAllList(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();

            Session["rptName"] = "/AGEReports/rptCashSaleRateAll";

            para.Add(new ReportParameter("Dt", frm["Dt"].ToString()));
            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SKUPlanList()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SKUPlanList(FormCollection frm)
        {
            Session.Remove("rptName");
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptInstPlanList";
                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptStockInstPlanList";
                    break;
            }
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            para.Add(new ReportParameter("Duration", (frm["Duration"] == "" ? "0" : frm["Duration"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult InstSale()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InstSale(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "1":
                    Session["rptName"] = "/AGEReports/rptLeaseAccDetail";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("Status", frm["Status"].ToString()));
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptLeaseReturnDetail";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    break;
                case "3":
                    Session["rptName"] = "/AGEReports/rptLeaseAccOfficerWise";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("Status", frm["Status"].ToString()));
                    if (frm.AllKeys.Contains("RecoveryId"))
                        para.Add(new ReportParameter("MktOfficerId", (frm["RecoveryId"] == "" ? "0" : frm["RecoveryId"]).ToString()));
                    else
                        para.Add(new ReportParameter("MktOfficerId", "0"));
                    para.Add(new ReportParameter("InqOfficerId", "0"));
                    para.Add(new ReportParameter("OffType", "2"));
                    break;
                case "4":
                    Session["rptName"] = "/AGEReports/rptLeaseAccOfficerWise";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("Status", frm["Status"].ToString()));
                    if (frm.AllKeys.Contains("RecoveryId"))
                        para.Add(new ReportParameter("InqOfficerId", (frm["RecoveryId"] == "" ? "0" : frm["RecoveryId"]).ToString()));
                    else
                        para.Add(new ReportParameter("InqOfficerId", "0"));
                    para.Add(new ReportParameter("MktOfficerId", "0"));
                    para.Add(new ReportParameter("OffType", "1"));
                    break;
                case "5":
                    Session["rptName"] = "/AGEReports/rptInstallmentDetailExcessAdv";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    break;
                case "6":
                    Session["rptName"] = "/AGEReports/rptInstallmentDetailWithoutGruntars";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    break;
            }
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ContentResult> PrintCrSlip(FormCollection frm)
        {
            ReportBL reportBL = new ReportBL();
            switch (frm["rptC"])
            {
                case "Installment":
                    {
                        using (rptInstallmentSlip rpt = new rptInstallmentSlip())
                        {
                            List<InstallmentRVM> lst = reportBL.GetInstallmentReport(Convert.ToInt64(frm["TransId"]));
                            rpt.SetDataSource(lst);
                            rpt.SetParameterValue("IsDuplicate", false);
                            rpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                            rpt.Close();
                            rpt.Dispose();
                            break;
                        }
                    }
                case "dInstallment":
                    {
                        using (rptInstallmentSlip rpt = new rptInstallmentSlip())
                        {
                            List<InstallmentRVM> lst = reportBL.GetInstallmentReport(Convert.ToInt64(frm["TransId"]));
                            rpt.SetDataSource(lst);
                            rpt.SetParameterValue("IsDuplicate", true);
                            rpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                            rpt.Close();
                            rpt.Dispose();
                            break;
                        }
                    }
                case "Customer":
                    {
                        using (rptCustInfoDetail rpt = new rptCustInfoDetail())
                        {
                            var IsAccLocked = new SaleBL().IsAccLocked(Convert.ToInt64(frm["TransIdC"]));
                            if (IsAccLocked)
                            {
                                var hasRights = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.LockAccountPrint);
                                if (hasRights)
                                {
                                    List<InstDetailVM> lst = await reportBL.GetInstByAcc(Convert.ToInt64(frm["TransIdC"]));
                                    List<CustomerDetailRVM> lst1 = await reportBL.GetCustomerInfo4Lock(Convert.ToInt64(frm["TransIdC"]));
                                    if (lst1 == null || lst1.Count == 0)
                                    {
                                        return Content("No Data Found");
                                    }
                                    var crcRemarks = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.CRCRemarks);
                                    if (!crcRemarks)
                                    {
                                        lst1[0].CRCRemarks = "CRC remarks hidden by HO";
                                    }
                                    rpt.Database.Tables["AGEERP_Models_InstDetailVM"].SetDataSource(lst);
                                    rpt.Database.Tables["AGEERP_CrReports_CustomerDetailRVM"].SetDataSource(lst1);
                                }
                                else
                                    return Content("Account Locked by HO");
                            }
                            else
                            {
                                List<InstDetailVM> lst = await reportBL.GetInstByAcc(Convert.ToInt64(frm["TransIdC"]));
                                List<CustomerDetailRVM> lst1 = await reportBL.GetCustomerInfo(Convert.ToInt64(frm["TransIdC"]));
                                if (lst1 == null || lst1.Count == 0)
                                {
                                    return Content("No Data Found");
                                }
                                var crcRemarks = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.CRCRemarks);
                                if (!crcRemarks)
                                {
                                    lst1[0].CRCRemarks = "CRC remarks hidden by HO";
                                }
                                rpt.Database.Tables["AGEERP_Models_InstDetailVM"].SetDataSource(lst);
                                rpt.Database.Tables["AGEERP_CrReports_CustomerDetailRVM"].SetDataSource(lst1);
                            }
                            rpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                            rpt.Close();
                            rpt.Dispose();
                            break;
                        }
                    }
                case "CustomerL":
                    {
                        using (rptCustInfoDetail rpt = new rptCustInfoDetail())
                        {
                            var IsAccLocked = new SaleBL().IsAccLocked(Convert.ToInt64(frm["TransIdC"]));
                            if (IsAccLocked)
                            {
                                var hasRights = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.LockAccountPrint);
                                if (hasRights)
                                {
                                    List<InstDetailVM> lst = await reportBL.GetInstByAcc(Convert.ToInt64(frm["TransIdC"]));
                                    List<CustomerDetailRVM> lst1 = await reportBL.GetCustomerInfo4Lock(Convert.ToInt64(frm["TransIdC"]));
                                    if (lst1 == null || lst1.Count == 0)
                                    {
                                        return Content("No Data Found");
                                    }
                                    var crcRemarks = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.CRCRemarks);
                                    if (!crcRemarks)
                                    {
                                        lst1[0].CRCRemarks = "CRC remarks hidden by HO";
                                    }
                                    rpt.Database.Tables["AGEERP_Models_InstDetailVM"].SetDataSource(lst);
                                    rpt.Database.Tables["AGEERP_CrReports_CustomerDetailRVM"].SetDataSource(lst1);
                                }
                                else
                                    return Content("Account Locked by HO");
                            }
                            else
                            {
                                List<InstDetailVM> lst = await reportBL.GetInstByAcc(Convert.ToInt64(frm["TransIdC"]));
                                List<CustomerDetailRVM> lst1 = await reportBL.GetCustomerInfo(Convert.ToInt64(frm["TransIdC"]));
                                if (lst1 == null || lst1.Count == 0)
                                {
                                    return Content("No Data Found");
                                }
                                var crcRemarks = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.CRCRemarks);
                                if (!crcRemarks)
                                {
                                    lst1[0].CRCRemarks = "CRC remarks hidden by HO";
                                }
                                rpt.Database.Tables["AGEERP_Models_InstDetailVM"].SetDataSource(lst);
                                rpt.Database.Tables["AGEERP_CrReports_CustomerDetailRVM"].SetDataSource(lst1);
                            }
                            rpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.Excel, System.Web.HttpContext.Current.Response, false, "");
                            rpt.Close();
                            rpt.Dispose();
                            break;
                        }
                    }
            }
            return Content("No Data Found");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PrintSlip(FormCollection frm)
        {
            Session["rptIsExport"] = "false";
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "Installment":
                    Session["rptName"] = "/AGEReports/rptInstallment";
                    para.Add(new ReportParameter("InstId", frm["TransId"].ToString()));
                    para.Add(new ReportParameter("IsDuplicate", true.ToString()));
                    break;
                case "dInstallment":
                    Session["rptName"] = "/AGEReports/rptInstallment";
                    para.Add(new ReportParameter("InstId", frm["TransId"].ToString()));
                    para.Add(new ReportParameter("IsDuplicate", false.ToString()));
                    break;
                case "DeliverySlip":
                    {
                        var IsAccLocked = new SaleBL().IsAccLocked(Convert.ToInt64(frm["TransId"]));
                        if (IsAccLocked)
                        {
                            return Content("Account Locked by HO");
                        }
                        Session["rptName"] = "/AGEReports/rptDeliverySlip";
                        para.Add(new ReportParameter("AccNo", frm["TransId"].ToString()));
                    }
                    break;
                case "CashSale":
                    {
                        var transId = Convert.ToInt64(frm["TransId"]);
                        var rpType = new SaleBL().IsFBRInvoice(transId, "C");
                        if (rpType == "OK")
                        {
                            Session["rptName"] = "/AGEReports/rptSalesInvoiceTax";
                        }
                        else if (rpType == "Above MRP")
                        {
                            Session["rptName"] = "/AGEReports/rptSalesInvoiceTaxA";
                        }
                        else
                        {
                            Session["rptName"] = "/AGEReports/rptCashSaleInvoice";
                        }
                        para.Add(new ReportParameter("TransId", frm["TransId"].ToString()));
                    }
                    break;
                case "InstSaleTax":
                    {
                        var transId = Convert.ToInt64(frm["TransId"]);
                        var rpType = new SaleBL().IsFBRInvoice(transId, "I");
                        if (rpType == "OK")
                        {
                            Session["rptName"] = "/AGEReports/rptInstInvoiceTax";
                        }
                        else
                        {
                            Session["rptName"] = "/AGEReports/rptDeliverySlip";
                        }
                        para.Add(new ReportParameter("AccNo", frm["TransId"].ToString()));
                    }
                    break;
                case "InstSaleReturnTax":
                    {
                        var transId = Convert.ToInt64(frm["TransId"]);
                        var rpType = new SaleBL().IsFBRInvoice(transId, "R");
                        if (rpType == "OK")
                        {
                            Session["rptName"] = "/AGEReports/rptInstReturnInvoiceTax";
                        }
                        else
                        {
                            Session["rptName"] = "/AGEReports/rptInstReturnInvoiceTax";
                        }
                        para.Add(new ReportParameter("TransId", frm["TransId"].ToString()));
                    }
                    break;
                case "CashSaleTax":
                    {
                        var transId = Convert.ToInt64(frm["TransId"]);
                        var rpType = new SaleBL().IsFBRInvoice(transId, "C");
                        if (rpType == "OK")
                        {
                            Session["rptName"] = "/AGEReports/rptSalesInvoiceTax";
                        }
                        else if (rpType == "Above MRP")
                        {
                            Session["rptName"] = "/AGEReports/rptSalesInvoiceTaxA";
                        }
                        else
                        {
                            Session["rptName"] = "/AGEReports/rptCashSaleInvoice";
                        }
                        para.Add(new ReportParameter("TransId", frm["TransId"].ToString()));
                    }
                    break;
                case "Processing":
                    Session["rptName"] = "/AGEReports/rptProcessingSlip";
                    para.Add(new ReportParameter("AccNo", frm["TransId"].ToString()));
                    break;
                case "Customer":
                    {
                        var IsAccLocked = new SaleBL().IsAccLocked(Convert.ToInt64(frm["TransId"]));
                        if (IsAccLocked)
                        {
                            var hasRights = await new SecurityBL().HasApprovalRight(UserInfo.UserId, UserInfo.GroupId, (int)RightMenuApproval.LockAccountPrint);
                            if (!hasRights)
                            {
                                return Content("Account Locked by HO");
                            }
                        }
                        Session["rptName"] = "/AGEReports/rptCustAccInfoDetail";
                        para.Add(new ReportParameter("AccNo", frm["TransId"].ToString()));
                    }
                    break;
                case "PO":
                    Session["rptName"] = "/AGEReports/rptPO";
                    para.Add(new ReportParameter("POId", frm["POId"].ToString()));
                    Session["rptIsExport"] = "true";
                    break;
                case "POPlanCity":
                    Session["rptName"] = "/AGEReports/rptPoPlanCityWise";
                    para.Add(new ReportParameter("PlanId", frm["PlanId"].ToString()));
                    Session["rptIsExport"] = "true";
                    break;
                case "POSchedule":
                    Session["rptName"] = "/AGEReports/rptPOSchedule";
                    para.Add(new ReportParameter("POId", frm["POId"].ToString()));
                    Session["rptIsExport"] = "true";
                    break;
                case "POSch":
                    Session["rptName"] = "/AGEReports/rptPOSch";
                    para.Add(new ReportParameter("SchMasterId", frm["TransId"].ToString()));
                    Session["rptIsExport"] = "true";
                    break;
                case "GRN":
                    Session["rptName"] = "/AGEReports/rptGRN";
                    para.Add(new ReportParameter("GRNId", frm["TransId"].ToString()));
                    break;
                case "POInvoice":
                    Session["rptName"] = "/AGEReports/rptPOInvoice";
                    para.Add(new ReportParameter("POInvId", frm["TransId"].ToString()));
                    break;
                case "Voucher":
                    Session["rptName"] = "/AGEReports/rptVoucher";
                    para.Add(new ReportParameter("pVrId", frm["TransId"].ToString()));
                    Session["rptIsExport"] = "true";
                    break;
                case "StockIssue":
                    Session["rptName"] = "/AGEReports/rptStockIssueSlip";
                    para.Add(new ReportParameter("TransId", frm["TransId"].ToString()));
                    break;
                case "CashPaymentSlip":
                    Session["rptName"] = "/AGEReports/rptCashPaymentSlip";
                    para.Add(new ReportParameter("TransId", frm["TransId"].ToString()));
                    break;
                case "CashReceiveSlip":
                    Session["rptName"] = "/AGEReports/rptCashReceiveSlip";
                    para.Add(new ReportParameter("TransId", frm["TransId"].ToString()));
                    break;
                case "CashDepositSlip":
                    Session["rptName"] = "/AGEReports/rptCashDepositSlip";
                    para.Add(new ReportParameter("TransId", frm["TransId"].ToString()));
                    break;
                case "BankDepositSlip":
                    Session["rptName"] = "/AGEReports/rptBankDepositSlip";
                    var TransId = await new CashBL().GetBankSlipCounter();
                    para.Add(new ReportParameter("TransId", TransId.ToString()));
                    break;
                case "CashTransferSlip":
                    Session["rptName"] = "/AGEReports/rptCashTransferSlip";
                    para.Add(new ReportParameter("TransId", frm["TransId"].ToString()));
                    break;
                case "ProductIncentivePolicy":
                    Session["rptIsExport"] = "true";
                    Session["rptName"] = "/AGEReports/rptProductIncentiveReport";
                    para.Add(new ReportParameter("ProcessId", frm["TransId"].ToString()));
                    break;
                case "SaleOrderSlip":
                    Session["rptName"] = "/AGEReports/rptCashSaleOrderInv";
                    para.Add(new ReportParameter("TransId", frm["TransId"].ToString()));
                    break;
                case "MTR":
                    Session["rptName"] = "/AGEReports/rptMTR";
                    para.Add(new ReportParameter("MTRId", frm["TransId"].ToString()));
                    break;
                case "ProPO":
                    Session["rptName"] = "/AGEReports/rptProPO";
                    para.Add(new ReportParameter("POId", frm["TransId"].ToString()));
                    break;
                case "ProGRN":
                    Session["rptName"] = "/AGEReports/rptProGRN";
                    para.Add(new ReportParameter("GRNId", frm["TransId"].ToString()));
                    break;
                case "ProSIN":
                    Session["rptName"] = "/AGEReports/rptProSIN";
                    para.Add(new ReportParameter("SINId", frm["TransId"].ToString()));
                    break;
                case "ProSIR":
                    Session["rptName"] = "/AGEReports/rptProSIR";
                    para.Add(new ReportParameter("SINId", frm["TransId"].ToString()));
                    break;
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult CashSale()
        {
            return View();
        }
        public ActionResult CashSaleAll()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashSaleAll(FormCollection frm)
        {
            Session.Remove("rptName");
            List<ReportParameter> para = new List<ReportParameter>();
            Session["rptName"] = "/AGEReports/rptCashSaleAll";
            para.Add(new ReportParameter("PaymentModeId", (frm["ModeId"] == "" ? "0" : frm["ModeId"]).ToString()));
            //para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SalesInvoices()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalesInvoices(FormCollection frm)
        {
            Session.Remove("rptName");
            List<ReportParameter> para = new List<ReportParameter>();
            Session["rptName"] = "/AGEReports/rptTaxSalesBranch";
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashSale(FormCollection frm)
        {
            Session.Remove("rptName");
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "1":
                    Session["rptName"] = "/AGEReports/rptCashSaleReport";
                    para.Add(new ReportParameter("PaymentModeId", (frm["ModeId"] == "" ? "0" : frm["ModeId"]).ToString()));
                    if (frm.AllKeys.Contains("Salesman"))
                        para.Add(new ReportParameter("SalesmanId", (frm["Salesman"] == "" ? "0" : frm["Salesman"]).ToString()));
                    else
                        para.Add(new ReportParameter("SalesmanId", "0"));
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptReferenceSaleReport";
                    break;
                case "3":
                    Session["rptName"] = "/AGEReports/rptCashSaleWOS";
                    para.Add(new ReportParameter("PaymentModeId", (frm["ModeId"] == "" ? "0" : frm["ModeId"]).ToString()));
                    //if (frm.AllKeys.Contains("Salesman"))
                    //    para.Add(new ReportParameter("SalesmanId", (frm["Salesman"] == "" ? "0" : frm["Salesman"]).ToString()));
                    //else
                    para.Add(new ReportParameter("SalesmanId", "0"));
                    break;
                case "4":
                    Session["rptName"] = "/AGEReports/rptSalemanWiseCashSaleSummary";
                    break;
                case "5":
                    Session["rptName"] = "/AGEReports/rptCashSaleOrderReport";
                    break;
            }
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult RegionWiseExpense()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegionWiseExpense(FormCollection frm)
        {

            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("locId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("locId", "0"));
            para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
            if (frm["rpt"] == "A")
            {
                Session["rptName"] = "/AGEReports/rptExpenseReportRegionWise";
                if (Request.Url.Host.Contains("192.168.77"))
                {
                    para.Add(new ReportParameter("IsLive", "false"));
                }
                else
                {
                    para.Add(new ReportParameter("IsLive", "true"));
                }
                para.Add(new ReportParameter("ExpHeadId", (frm["ExpHeadId"] == "" ? "0" : frm["ExpHeadId"]).ToString()));
            }
            else
            {
                Session["rptName"] = "/AGEReports/rptBillExpense";
            }
            if (frm["IsPosted"].ToString() != "2")
            {
                para.Add(new ReportParameter("IsPosted", frm["IsPosted"].ToString()));
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SaleOrderAdvanceDeposition()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaleOrderAdvanceDeposition(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptCustWiseOrderAdvance";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult ExpenseReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExpenseReport(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptExpenseReport";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("locId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
            if (Request.Url.Host.Contains("192.168.77"))
            {
                para.Add(new ReportParameter("IsLive", "false"));
            }
            else
            {
                para.Add(new ReportParameter("IsLive", "true"));
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult CashPaymentReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashPaymentReport(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptCashPaymentDetail";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SupplierPaymentReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierPaymentReport(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptSupplierPayments";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult StockSaleRateList()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StockSaleRateList(FormCollection frm)
        {
            Session.Remove("rptName");
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "StockSaleRate":
                    Session["rptName"] = "/AGEReports/rptStockSaleRate";
                    break;
                case "StockInstPlan":
                    Session["rptName"] = "/AGEReports/rptStockInstPlanList";
                    para.Add(new ReportParameter("Duration", "0"));
                    break;
                case "InstPlanHO":
                    Session["rptName"] = "/AGEReports/rptInstPlanHOList";
                    para.Add(new ReportParameter("Duration", "0"));
                    break;
            }

            para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        #endregion
        #region Accounts
        public async Task<ActionResult> SupplierGL()
        {
            var lst = await new AccountBL().SubCodeList();
            var suppArr = new long[] { 20010100010, 20011000010, 20012000010, 20013000010 };
            ViewBag.COA4 = lst.Where(x => suppArr.Contains(x.Id)).Select(x => new { Name = x.Name, Code = x.Code, Id = Convert.ToInt64(x.Code.Replace("-", "")) }).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierGL(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptSuppGeneralLedger";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("pYrCode", frm["YrCode"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            var subId = (frm["SubId"] == "" ? "0" : frm["SubId"]).ToString();
            para.Add(new ReportParameter("AccId", (frm["Code"] == "" ? "0" : frm["Code"]).ToString()));
            para.Add(new ReportParameter("SuppId", subId));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public async Task<ActionResult> SupplierGLPrevious()
        {
            var lst = await new AccountBL().SubCodeList();
            var suppArr = new long[] { 20010100010, 20011000010, 20012000010, 20013000010 };
            ViewBag.COA4 = lst.Where(x => suppArr.Contains(x.Id)).Select(x => new { Name = x.Name, Code = x.Code, Id = Convert.ToInt64(x.Code.Replace("-", "")) }).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierGLPrevious(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptSuppGeneralLedgerOld";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            var subId = (frm["SubId"] == "" ? "0" : frm["SubId"]).ToString();
            para.Add(new ReportParameter("AccId", (frm["Code"] == "" ? "0" : frm["Code"]).ToString()));
            para.Add(new ReportParameter("SuppId", subId));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult BranchProfitability()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BranchProfitability(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("pYrCode", frm["YrCode"].ToString()));
            switch (frm["rpt"])
            {
                case "1":
                    {
                        Session["rptName"] = "/AGEReports/rptPNLAll2";
                        break;
                    }
                case "2":
                    {
                        Session["rptName"] = "/AGEReports/rptPNLAll3";
                        break;
                    }
                case "3":
                    {
                        Session["rptName"] = "/AGEReports/rptPNLAll4";
                        break;
                    }
                case "4":
                    {
                        Session["rptName"] = "/AGEReports/rptPNLAll5";
                        if (frm.AllKeys.Contains("RegionId"))
                            para.Add(new ReportParameter("Region", frm["RegionId"].ToString()));
                        else
                            para.Add(new ReportParameter("Region", ""));
                        if (frm.AllKeys.Contains("PCCode"))
                            para.Add(new ReportParameter("ProfitCenter", frm["PCCode"].ToString()));
                        else
                            para.Add(new ReportParameter("ProfitCenter", ""));
                        para.Add(new ReportParameter("SaleType", frm["SaleType"].ToString()));
                        para.Add(new ReportParameter("PRType", frm["PRType"].ToString()));
                        break;
                    }
            }


            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            //para.Add(new ReportParameter("PCCode", (frm["PCCode"] == "" ? "0" : frm["PCCode"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult PNL()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PNL(FormCollection frm)
        {
            switch (frm["rpt"])
            {
                case "1":
                    {
                        Session["rptName"] = "/AGEReports/rptPNL1";
                        break;
                    }
                case "2":
                    {
                        Session["rptName"] = "/AGEReports/rptPNL";
                        break;
                    }
            }

            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("pYrCode", frm["YrCode"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("PCCode", (frm["PCCode"] == "" ? "0" : frm["PCCode"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult BalanceSheet()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BalanceSheet(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("pYrCode", frm["YrCode"].ToString()));
            switch (frm["rpt"])
            {
                case "1":
                    {
                        Session["rptName"] = "/AGEReports/rptBalanceSheet1";
                        para.Add(new ReportParameter("Date", frm["ToDate"].ToString()));
                        break;
                    }
                case "2":
                    {
                        Session["rptName"] = "/AGEReports/rptBalanceSheet";
                        para.Add(new ReportParameter("FromDate", "2021-07-01"));
                        para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                        break;
                    }
            }



            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult SalarySheetPromoDemo()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalarySheetPromoDemo(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptPay_SalarySheet_PD";
            List<ReportParameter> para = new List<ReportParameter>();
            DateTime dt = Convert.ToDateTime(frm["Date"]);
            dt = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
            para.Add(new ReportParameter("SalaryMonth", dt.ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult EmpPromoDemo()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmpPromoDemo(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptPay_PromotionDemotion_List";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("PDMonth", frm["Date"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public async Task<ActionResult> GL()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GL(FormCollection frm)
        {
            var AccId = Convert.ToInt64(frm["Code"] == "" ? "0" : frm["Code"]);
            var hasRight = await new SecurityBL().FinHasApprovalRight(UserInfo.UserId, UserInfo.GroupId, AccId);
            if (!hasRight)
            {
                return Content("You are not authorized!");
            }
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("pYrCode", frm["YrCode"].ToString()));
            para.Add(new ReportParameter("pStartDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("pEndDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("pPCCode", (frm["PCCode"] == "" ? "0" : frm["PCCode"]).ToString()));

            var subId = (frm["SubId"] == "" ? "0" : frm["SubId"]).ToString();
            var isMulti = frm["IsMulti"].ToString();
            if (subId == "0")
            {
                para.Add(new ReportParameter("pGLCode", (frm["Code"] == "" ? "0" : frm["Code"]).ToString()));
                Session["rptName"] = "/AGEReports/rptGeneralLedger";
            }
            else
            {
                if (isMulti == "M")
                {
                    Session["rptName"] = "/AGEReports/rptGeneralLedgerDetailMulti";
                    para.Add(new ReportParameter("pSubId", subId));
                    para.Add(new ReportParameter("pSubsidiaryCode", frm["SubsidiaryCode"].ToString()));
                }
                else
                {
                    para.Add(new ReportParameter("pGLCode", (frm["Code"] == "" ? "0" : frm["Code"]).ToString()));
                    Session["rptName"] = "/AGEReports/rptGeneralLedgerDetail";
                    para.Add(new ReportParameter("pSubId", subId));
                }
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        //public async Task<ActionResult> GLU()
        //{
        //    //ViewBag.COA4 = (await new AccountBL().SubCodeList()).Select(x => new { Name = x.Name, Code = x.Code }).ToList();
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> GLU(FormCollection frm)
        //{
        //    var AccId = Convert.ToInt64(frm["Code"] == "" ? "0" : frm["Code"]);
        //    var hasRight = await new SecurityBL().FinHasApprovalRight(UserInfo.UserId, UserInfo.GroupId, AccId);
        //    if (!hasRight)
        //    {
        //        return View();
        //    }
        //    List<ReportParameter> para = new List<ReportParameter>();
        //    para.Add(new ReportParameter("pStartDate", frm["FromDate"].ToString()));
        //    para.Add(new ReportParameter("pEndDate", frm["ToDate"].ToString()));
        //    para.Add(new ReportParameter("pPCCode", (frm["PCCode"] == "" ? "0" : frm["PCCode"]).ToString()));

        //    var subId = (frm["SubId"] == "" ? "0" : frm["SubId"]).ToString();
        //    var isMulti = frm["IsMulti"].ToString();
        //    if (subId == "0")
        //    {
        //        para.Add(new ReportParameter("pGLCode", (frm["Code"] == "" ? "0" : frm["Code"]).ToString()));
        //        Session["rptName"] = "/AGEReports/rptGeneralLedger";
        //    }
        //    else
        //    {
        //        if (isMulti == "M")
        //        {
        //            Session["rptName"] = "/AGEReports/rptGeneralLedgerDetailMulti";
        //            para.Add(new ReportParameter("pSubId", subId));
        //            para.Add(new ReportParameter("pSubsidiaryCode", frm["SubsidiaryCode"].ToString()));
        //        }
        //        else
        //        {
        //            para.Add(new ReportParameter("pGLCode", (frm["Code"] == "" ? "0" : frm["Code"]).ToString()));
        //            Session["rptName"] = "/AGEReports/rptGeneralLedgerDetail";
        //            para.Add(new ReportParameter("pSubId", subId));
        //        }
        //    }
        //    Session["rptParameter"] = para;
        //    return Redirect("~/Report.aspx");
        //}
        public ActionResult TrialBalanceAllProfitCenter()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TrialBalanceAllProfitCenter(FormCollection frm)
        {
            var AccId = Convert.ToInt64(frm["Code"] == "" ? "0" : frm["Code"]);
            var hasRight = await new SecurityBL().FinHasApprovalRight(UserInfo.UserId, UserInfo.GroupId, AccId);
            if (!hasRight)
            {
                return Content("You are not authorized!");
            }
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("pYrCode", frm["YrCode"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            if (frm["rpt"] == "P")
            {
                para.Add(new ReportParameter("AccId", (frm["Code"] == "" ? "0" : frm["Code"]).ToString()));
                Session["rptName"] = "/AGEReports/rptTrialBLLoc";
            }
            else
            {
                para.Add(new ReportParameter("Code", (frm["Code"] == "" ? "0" : frm["Code"]).ToString()));
                para.Add(new ReportParameter("PCCode", "0"));
                Session["rptName"] = "/AGEReports/rptTrialBL5";
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult TrialBalanceAllProfitCenters()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrialBalanceAllProfitCenters(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("pYrCode", frm["YrCode"].ToString()));

            para.Add(new ReportParameter("Lvl", frm["Lvl"]));
            para.Add(new ReportParameter("IsOpening", frm["Opening"] == "true,false" ? "false" : "true"));
            para.Add(new ReportParameter("ForPeriod", frm["ForPeriod"] == "true,false" ? "false" : "true"));
            para.Add(new ReportParameter("Closing", frm["Closing"] == "true,false" ? "false" : "true"));

            if (frm["rpt"] == "C")
            {
                para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                Session["rptName"] = "/AGEReports/rptTrialBLLocAll";
            }
            else if (frm["rpt"] == "A")
            {
                para.Add(new ReportParameter("NonZero", frm["NonZero"] == "true,false" ? "false" : "true"));
                para.Add(new ReportParameter("PCCode", (frm["PCCode"] == "" ? "0" : frm["PCCode"]).ToString()));
                Session["rptName"] = "/AGEReports/rptTrialBLComparison";
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult TrialBalance()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrialBalance(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("pYrCode", frm["YrCode"].ToString()));
            if (frm["rpt"] == "C")
            {
                Session["rptName"] = "/AGEReports/rptTrialBL";
                para.Add(new ReportParameter("NonZero", frm["NonZero"]));
                para.Add(new ReportParameter("Lvl", frm["Lvl"]));
            }
            else
            {
                if (frm.AllKeys.Contains("COA4") && Convert.ToInt64(frm["COA4"] == "" ? "0" : frm["COA4"]) > 0)
                {
                    Session["rptName"] = "/AGEReports/rptTrialBL5";
                    para.Add(new ReportParameter("Code", (frm["COA4"] == "" ? "0" : frm["COA4"]).ToString()));
                }
                else if (frm.AllKeys.Contains("COA3") && Convert.ToInt64(frm["COA3"] == "" ? "0" : frm["COA3"]) > 0)
                {
                    Session["rptName"] = "/AGEReports/rptTrialBL4";
                    para.Add(new ReportParameter("Code", (frm["COA3"] == "" ? "0" : frm["COA3"]).ToString()));
                }
                else if (frm.AllKeys.Contains("COA2") && Convert.ToInt64(frm["COA2"] == "" ? "0" : frm["COA2"]) > 0)
                {
                    Session["rptName"] = "/AGEReports/rptTrialBL3";
                    para.Add(new ReportParameter("Code", (frm["COA2"] == "" ? "0" : frm["COA2"]).ToString()));
                }
                else if (frm.AllKeys.Contains("COA1") && Convert.ToInt64(frm["COA1"] == "" ? "0" : frm["COA1"]) > 0)
                {
                    Session["rptName"] = "/AGEReports/rptTrialBL2";
                    para.Add(new ReportParameter("Code", (frm["COA1"] == "" ? "0" : frm["COA1"]).ToString()));
                }
                else
                {
                    Session["rptName"] = "/AGEReports/rptTrialBL1";
                }

            }

            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("PCCode", (frm["PCCode"] == "" ? "0" : frm["PCCode"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult VoucherList()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VoucherList(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptVoucherList";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("pYrCode", frm["YrCode"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("VrTypeId", (frm["VrTypeId"] == "" ? "All" : frm["VrTypeId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult PostingPendancy()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostingPendancy(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptPostingPendancy";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("Tdate", frm["ToDate"].ToString()));
            //para.Add(new ReportParameter("VrTypeId", (frm["VrTypeId"] == "" ? "All" : frm["VrTypeId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult BankContra()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankContra(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptBankContra";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("pYrCode", frm["YrCode"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("AccId", (frm["AccId"] == "" ? "0" : frm["AccId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult BankReconciliation()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankReconciliation(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptBankReconciliation";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("Dt", frm["DocDate"].ToString()));
            para.Add(new ReportParameter("AccId", (frm["AccId"] == "" ? "0" : frm["AccId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        #endregion
        #region PayrollReports
        public ActionResult LeaveStatus()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LeaveStatus(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptPay_LeaveApplications_Status";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("HDeptId", (frm["HDeptId"] == "" ? "0" : frm["HDeptId"]).ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult ProductIncentiveReportAudit()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductIncentiveReportAudit(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptProductIncentiveReport_4_Audit";
            var CityId = Convert.ToInt32(frm["CityId"]);
            var PeriodId = Convert.ToInt32(frm["PeriodId"]);
            var calendar = new EmployeeBL().GetProductIncTime(PeriodId);
            var ProcessId = new EmployeeBL().GetProductProcessId(CityId, calendar.StartDate, calendar.EndDate);
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("ProcessId", (ProcessId.ProcessId).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult ProductIncentiveReport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductIncentiveReport(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptProductIncentivePolicy";
            DateTime Month = Convert.ToDateTime(frm["Month"]);
            var StartDate = new DateTime(Month.Year, Month.Month, 1);
            var EndDate = StartDate.AddMonths(1).AddDays(-1);
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FromDate", StartDate.ToShortDateString()));
            para.Add(new ReportParameter("ToDate", EndDate.ToShortDateString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult ProductIncentiveReportDaily()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductIncentiveReportDaily(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptProductIncentiveReport_Daily";

            List<ReportParameter> para = new List<ReportParameter>();
            string userid = UserInfo.UserId.ToString();
            para.Add(new ReportParameter("UserId", userid));
            para.Add(new ReportParameter("AIncDate", frm["Date"]));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult ProductInentivePolicy()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductInentivePolicy(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptProductIncentiveReport";
            var CityId = Convert.ToInt32(frm["CityId"]);
            var PeriodId = Convert.ToInt32(frm["PeriodId"]);
            var calendar = new EmployeeBL().GetProductIncTime(PeriodId);
            var ProcessId = new EmployeeBL().GetProductProcessId(CityId, calendar.StartDate, calendar.EndDate);
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("ProcessId", (ProcessId.ProcessId).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult BranchAttendance()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BranchAttendance(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "1":
                    Session["rptName"] = "/AGEReports/rptPay_InOutDetail_V4";
                    para.Add(new ReportParameter("AttnDate", frm["Date"].ToString()));
                    //para.Add(new ReportParameter("EmpId", "0"));
                    para.Add(new ReportParameter("LocId", frm["LocId"]));
                    //para.Add(new ReportParameter("HDeptId", "2"));
                    //para.Add(new ReportParameter("DesgId", "0"));
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptPay_MonthlyAttendance";
                    para.Add(new ReportParameter("Month", frm["Date"].ToString()));
                    para.Add(new ReportParameter("DeptId", frm["LocId"]));
                    para.Add(new ReportParameter("HDeptId", "2"));
                    break;
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult DailyAttendance()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DailyAttendance(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "1":
                    Session["rptName"] = "/AGEReports/rptPay_InOutDetail_V2";
                    para.Add(new ReportParameter("AttnDate", frm["FromDate"].ToString()));
                    //para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    if (frm.AllKeys.Where(x => x == "EmpId").Any())
                        para.Add(new ReportParameter("EmpId", (frm["EmpId"] == "" ? "0" : frm["EmpId"]).ToString()));
                    else
                        para.Add(new ReportParameter("EmpId", "0"));
                    para.Add(new ReportParameter("DeptId", (frm["DeptId"] == "" ? "0" : frm["DeptId"]).ToString()));
                    para.Add(new ReportParameter("HDeptId", (frm["HDeptId"] == "" ? "0" : frm["HDeptId"]).ToString()));
                    para.Add(new ReportParameter("DesgId", (frm["DesgId"] == "" ? "0" : frm["DesgId"]).ToString()));
                    para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptPay_InOutDetail";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    if (frm.AllKeys.Where(x => x == "EmpId").Any())
                        para.Add(new ReportParameter("EmpId", (frm["EmpId"] == "" ? "0" : frm["EmpId"]).ToString()));
                    else
                        para.Add(new ReportParameter("EmpId", "0"));
                    para.Add(new ReportParameter("DeptId", (frm["DeptId"] == "" ? "0" : frm["DeptId"]).ToString()));
                    para.Add(new ReportParameter("HDeptId", (frm["HDeptId"] == "" ? "0" : frm["HDeptId"]).ToString()));
                    para.Add(new ReportParameter("DesgId", (frm["DesgId"] == "" ? "0" : frm["DesgId"]).ToString()));
                    break;
            }

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult MonthlyAttendance()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyAttendance(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptPay_MonthlyAttendance";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("Month", frm["Month"].ToString()));
            para.Add(new ReportParameter("DeptId", (frm["DeptId"] == "" ? "0" : frm["DeptId"]).ToString()));
            para.Add(new ReportParameter("HDeptId", (frm["HDeptId"] == "" ? "0" : frm["HDeptId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult EmployeeStatusLog()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeStatusLog(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptEmpStatusLog";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("StartDate", (frm["StartDate"] == "" ? "0" : frm["StartDate"]).ToString()));
            para.Add(new ReportParameter("EndDate", (frm["EndDate"] == "" ? "0" : frm["EndDate"]).ToString()));
            para.Add(new ReportParameter("DesgId", (frm["DesgId"] == "" ? "0" : frm["DesgId"]).ToString()));
            para.Add(new ReportParameter("HDeptId", (frm["HDeptId"] == "" ? "0" : frm["HDeptId"]).ToString()));
            para.Add(new ReportParameter("DeptId", (frm["DeptId"] == "" ? "0" : frm["DeptId"]).ToString()));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult EmployeeInfo()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeInfo(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptEmployeeInfo";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("deptid", (frm["DeptId"] == "" ? "0" : frm["DeptId"]).ToString()));
            if (frm.AllKeys.Where(x => x == "EmpId").Any())
                para.Add(new ReportParameter("empid", (frm["EmpId"] == "" ? "0" : frm["EmpId"]).ToString()));
            else
                para.Add(new ReportParameter("empid", "0"));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        #endregion
        #region Salary
        public ActionResult MonthlySalarySheet()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlySalarySheet(FormCollection frm)
        {
            if (frm["HDeptId"] == "2")
                Session["rptName"] = "/AGEReports/rptPay_SalarySheet";
            else if (frm["HDeptId"] == "3")
                Session["rptName"] = "/AGEReports/rptPay_SalarySheet_FIELD";
            else
                Session["rptName"] = "/AGEReports/rptPay_SalarySheet_HO";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("SalaryMonth", frm["Month"].ToString()));
            para.Add(new ReportParameter("HDeptId", (frm["HDeptId"] == "" ? "0" : frm["HDeptId"]).ToString()));
            para.Add(new ReportParameter("DeptId", (frm["DeptId"] == "" ? "0" : frm["DeptId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }



        public ActionResult MonthlySalarySheetAudit()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlySalarySheetAudit(FormCollection frm)
        {

            Session["rptName"] = "/AGEReports/rptPay_SalarySheet_4_Audit";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("SalaryMonth", frm["Month"].ToString()));
            para.Add(new ReportParameter("HDeptId", (frm["HDeptId"] == "" ? "0" : frm["HDeptId"]).ToString()));
            para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult SalaryDisbursement()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalaryDisbursement(FormCollection frm)
        {
            //if (frm["rpt"] == "dailyAttendance")
            Session["rptName"] = "/AGEReports/rptSalaryDisbursement";
            //else if (frm["rpt"] == "lateCommers")
            //    Session["rptName"] = "/AGEReports/rptPay_Attendance";

            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("SalaryMonth", frm["FromDate"].ToString()));
            if (frm.AllKeys.Contains("DeptId"))
            {
                para.Add(new ReportParameter("DeptID", (frm["DeptId"] == "" ? "0" : frm["DeptId"]).ToString()));

            }
            else
            {
                para.Add(new ReportParameter("DeptID", "0"));
            }
            para.Add(new ReportParameter("HDeptID", (frm["HDeptId"] == "" ? "0" : frm["HDeptId"]).ToString()));
            para.Add(new ReportParameter("DesgID", (frm["DesgId"] == "" ? "0" : frm["DesgId"]).ToString()));
            para.Add(new ReportParameter("DisbTypeId", (frm["DisbursementTypeId"] == "" ? "0" : frm["DisbursementTypeId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        #endregion
        #region Installment
        public ActionResult AuditObservation()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuditObservation(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            Session["rptName"] = "/AGEReports/rptCustomerDataCheck_V2";
            para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            if (frm.AllKeys.Contains("WrongCase"))
                para.Add(new ReportParameter("WrongCase", frm["WrongCase"].ToString()));
            else
                para.Add(new ReportParameter("WrongCase", (string)null));
            if (frm.AllKeys.Contains("WrongProduct"))
                para.Add(new ReportParameter("WrongProduct", frm["WrongProduct"].ToString()));
            else
                para.Add(new ReportParameter("WrongProduct", (string)null));
            if (frm.AllKeys.Contains("InvolvementCase"))
                para.Add(new ReportParameter("InvolvementCase", frm["InvolvementCase"].ToString()));
            else
                para.Add(new ReportParameter("InvolvementCase", (string)null));
            if (frm.AllKeys.Contains("WrongPTO"))
                para.Add(new ReportParameter("WrongPTO", frm["WrongPTO"].ToString()));
            else
                para.Add(new ReportParameter("WrongPTO", (string)null));
            if (frm.AllKeys.Contains("PTOCase"))
                para.Add(new ReportParameter("PTOCase", frm["PTOCase"].ToString()));
            else
                para.Add(new ReportParameter("PTOCase", (string)null));
            if (frm.AllKeys.Contains("HomeFake"))
                para.Add(new ReportParameter("HomeFake", frm["HomeFake"].ToString()));
            else
                para.Add(new ReportParameter("HomeFake", (string)null));
            if (frm.AllKeys.Contains("OfficialFake"))
                para.Add(new ReportParameter("OfficialFake", frm["OfficialFake"].ToString()));
            else
                para.Add(new ReportParameter("OfficialFake", (string)null));
            if (frm.AllKeys.Contains("LoseGuarantee"))
                para.Add(new ReportParameter("LoseGuarantee", frm["LoseGuarantee"].ToString()));
            else
                para.Add(new ReportParameter("LoseGuarantee", (string)null));
            if (frm.AllKeys.Contains("FakeGuarantee"))
                para.Add(new ReportParameter("FakeGuarantee", frm["FakeGuarantee"].ToString()));
            else
                para.Add(new ReportParameter("FakeGuarantee", (string)null));
            if (frm.AllKeys.Contains("ManageGuarantee"))
                para.Add(new ReportParameter("ManageGuarantee", frm["ManageGuarantee"].ToString()));
            else
                para.Add(new ReportParameter("ManageGuarantee", (string)null));
            if (frm.AllKeys.Contains("WithoutVerification"))
                para.Add(new ReportParameter("WithoutVerification", frm["WithoutVerification"].ToString()));
            else
                para.Add(new ReportParameter("WithoutVerification", (string)null));
            if (frm.AllKeys.Contains("HomeRental"))
                para.Add(new ReportParameter("HomeRental", frm["HomeRental"].ToString()));
            else
                para.Add(new ReportParameter("HomeRental", (string)null));
            if (frm.AllKeys.Contains("PhotoCHQ"))
                para.Add(new ReportParameter("PhotoCHQ", frm["PhotoCHQ"].ToString()));
            else
                para.Add(new ReportParameter("PhotoCHQ", (string)null));
            if (frm.AllKeys.Contains("Guarantor1"))
                para.Add(new ReportParameter("Guarantor1", frm["Guarantor1"].ToString()));
            else
                para.Add(new ReportParameter("Guarantor1", (string)null));
            if (frm.AllKeys.Contains("Guarantor2"))
                para.Add(new ReportParameter("Guarantor2", frm["Guarantor2"].ToString()));
            else
                para.Add(new ReportParameter("Guarantor2", (string)null));
            if (frm.AllKeys.Contains("Mobile"))
                para.Add(new ReportParameter("Mobile", frm["Mobile"].ToString()));
            else
                para.Add(new ReportParameter("Mobile", (string)null));
            if (frm.AllKeys.Contains("NIC"))
                para.Add(new ReportParameter("NIC", frm["NIC"].ToString()));
            else
                para.Add(new ReportParameter("NIC", (string)null));
            if (frm.AllKeys.Contains("Cheque"))
                para.Add(new ReportParameter("Cheque", frm["Cheque"].ToString()));
            else
                para.Add(new ReportParameter("Cheque", (string)null));
            if (frm.AllKeys.Contains("Pic"))
                para.Add(new ReportParameter("Pic", frm["Pic"].ToString()));
            else
                para.Add(new ReportParameter("Pic", (string)null));
            if (frm.AllKeys.Contains("Thumb"))
                para.Add(new ReportParameter("Thumb", frm["Thumb"].ToString()));
            else
                para.Add(new ReportParameter("Thumb", (string)null));
            if (frm.AllKeys.Contains("Affidavit"))
                para.Add(new ReportParameter("Affidavit", frm["Affidavit"].ToString()));
            else
                para.Add(new ReportParameter("Affidavit", (string)null));
            if (frm.AllKeys.Contains("BMSign"))
                para.Add(new ReportParameter("BMSign", frm["BMSign"].ToString()));
            else
                para.Add(new ReportParameter("BMSign", (string)null));
            if (frm.AllKeys.Contains("RMSign"))
                para.Add(new ReportParameter("RMSign", frm["RMSign"].ToString()));
            else
                para.Add(new ReportParameter("RMSign", (string)null));
            if (frm.AllKeys.Contains("VisitStatus"))
                para.Add(new ReportParameter("VisitStatus", frm["VisitStatus"].ToString()));
            else
                para.Add(new ReportParameter("VisitStatus", (string)null));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }



        public ActionResult CustomerDataChecking()
        {
            ViewBag.rptName = "rptCustomerDataCheck";
            ViewBag.rptTitle = "Customer Data Checking Report";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerDataChecking(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rptName"])
            {
                case "rptCustomerDataCheck":
                    //Session["rptName"] = "/AGEReports/rptCustomerDataCheck";
                    Session["rptName"] = "/AGEReports/rptCustomerDataCheckfroBranch";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    break;
                case "rptRecoveryOfficerPerformance":
                    Session["rptName"] = "/AGEReports/rptRecoveryOfficerPerformance";
                    para.Add(new ReportParameter("Category", frm["Category"].ToString()));
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    break;
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult RecoveryPerformance()
        {
            ViewBag.rptName = "rptRecoveryOfficerPerformance";
            ViewBag.rptTitle = "Recovery Officer Performance Report";
            return View("CustomerDataChecking");
        }
        public ActionResult InstalmentDetail()
        {
            ViewBag.rptName = "rptInstallmentDetail";
            ViewBag.rptTitle = "Installment Detail Report";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InstalmentDetail(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "1":
                    Session["rptName"] = "/AGEReports/rptInstallmentDetail";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptVoucherRecoveryRecWise";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    break;
                case "3":
                    Session["rptName"] = "/AGEReports/rptVoucherRecoveryInqWise";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    break;
                case "4":
                    Session["rptName"] = "/AGEReports/rptInstallmentDetailDiscountAllow";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    break;
                case "5":
                    Session["rptName"] = "/AGEReports/rptInstallmentDetailAdvPartial";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    break;
                case "6":
                    Session["rptName"] = "/AGEReports/rptInstallmentDetailRecWise";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    if (frm.AllKeys.Contains("RecoveryId"))
                        para.Add(new ReportParameter("RecoveryId", (frm["RecoveryId"] == "" ? "0" : frm["RecoveryId"]).ToString()));
                    else
                        para.Add(new ReportParameter("RecoveryId", "0"));
                    break;
                case "7":
                    Session["rptName"] = "/AGEReports/rptInstRcvOther";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    break;
                case "8":
                    Session["rptName"] = "/AGEReports/rptInstRcvAtOther";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    break;
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerAccMulti(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            Session["rptName"] = "/AGEReports/rptCustomerDetailMulti";
            para.Add(new ReportParameter("AccNos", frm["AccNos"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CustomerAccMulti(FormCollection frm)
        //{
        //    List<ReportParameter> para = new List<ReportParameter>();
        //    Session["IsAutoPrint"] = "All";
        //    Session["rptC"] = "Customer";
        //    //Session["AccNos"] = reportBL.GetAccfromOSId(frm["AccNos"]);
        //    return Redirect("~/CrReport.aspx");
        //}
        public async Task<JsonResult> ReportPDF(long TransId)
        {
            using (rptCustInfoDetail rpt = new rptCustInfoDetail())
            {
                ReportBL reportBL = new ReportBL();
                var accNo = await reportBL.GetAccfromOSId(TransId);
                List<InstDetailVM> lst = await reportBL.GetInstByAcc(accNo);
                List<CustomerDetailRVM> lst1 = await reportBL.GetCustomerInfo(accNo);
                lst1[0].CRCRemarks = "CRC remarks hidden by HO";
                rpt.Database.Tables["AGEERP_Models_InstDetailVM"].SetDataSource(lst);
                rpt.Database.Tables["AGEERP_CrReports_CustomerDetailRVM"].SetDataSource(lst1);
                rpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                rpt.Close();
                rpt.Dispose();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> CustReportPDF(long accNo)
        {
            using (rptCustInfoDetail rpt = new rptCustInfoDetail())
            {
                ReportBL reportBL = new ReportBL();
                //var accNo = reportBL.GetAccfromOSId(TransId);
                List<InstDetailVM> lst = await reportBL.GetInstByAcc(accNo);
                List<CustomerDetailRVM> lst1 = await reportBL.GetCustomerInfo(accNo);
                lst1[0].CRCRemarks = "CRC remarks hidden by HO";
                rpt.Database.Tables["AGEERP_Models_InstDetailVM"].SetDataSource(lst);
                rpt.Database.Tables["AGEERP_CrReports_CustomerDetailRVM"].SetDataSource(lst1);
                rpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                rpt.Close();
                rpt.Dispose();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult OutStand()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OutStand(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm["rpt"] == "1")
            {
                switch (frm["ReportType"])
                {
                    case "R":
                        if (frm["IsLandscape"] == "true,false")
                        {
                            Session["rptName"] = "/AGEReports/rptOutstandingReportRecWiseLS";
                        }
                        else
                        {
                            Session["rptName"] = "/AGEReports/rptOutstandingReportRecWise";
                        }

                        para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                        para.Add(new ReportParameter("Category", (frm["Category"]).ToString()));
                        para.Add(new ReportParameter("Status", (frm["Status"]).ToString()));
                        para.Add(new ReportParameter("Type", (frm["Assign"]).ToString()));
                        para.Add(new ReportParameter("Month", frm["Month"].ToString()));
                        para.Add(new ReportParameter("RecoveryId", frm["RecoveryId"] == "" ? "0" : frm["RecoveryId"].ToString()));
                        para.Add(new ReportParameter("ReportType", "R"));
                        para.Add(new ReportParameter("IsProduct", (frm["IsProduct"] == "true,false" ? "true" : frm["IsProduct"]).ToString()));
                        para.Add(new ReportParameter("IsGuarantor", (frm["IsGuarantor"] == "true,false" ? "true" : frm["IsGuarantor"]).ToString()));
                        para.Add(new ReportParameter("IsMarketing", (frm["IsMarketing"] == "true,false" ? "true" : frm["IsMarketing"]).ToString()));
                        para.Add(new ReportParameter("IsAddress", (frm["IsAddress"] == "true,false" ? "true" : frm["IsAddress"]).ToString()));
                        para.Add(new ReportParameter("IsRecInq", (frm["IsRecInq"] == "true,false" ? "true" : frm["IsRecInq"]).ToString()));
                        //para.Add(new ReportParameter("IsLandscape", (frm["IsLandscape"] == "true,false" ? "true" : frm["IsLandscape"]).ToString()));
                        break;
                    case "I":
                        if (frm["IsLandscape"] == "true,false")
                        {
                            Session["rptName"] = "/AGEReports/rptOutstandingReportInqWiseLS";
                        }
                        else
                        {
                            Session["rptName"] = "/AGEReports/rptOutstandingReportInqWise";
                        }
                        para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                        para.Add(new ReportParameter("Category", (frm["Category"]).ToString()));
                        para.Add(new ReportParameter("Status", (frm["Status"]).ToString()));
                        para.Add(new ReportParameter("Type", (frm["Assign"]).ToString()));
                        para.Add(new ReportParameter("Month", frm["Month"].ToString()));
                        para.Add(new ReportParameter("RecoveryId", frm["RecoveryId"] == "" ? "0" : frm["RecoveryId"].ToString()));
                        para.Add(new ReportParameter("ReportType", "I"));
                        para.Add(new ReportParameter("IsProduct", (frm["IsProduct"] == "true,false" ? "true" : frm["IsProduct"]).ToString()));
                        para.Add(new ReportParameter("IsGuarantor", (frm["IsGuarantor"] == "true,false" ? "true" : frm["IsGuarantor"]).ToString()));
                        para.Add(new ReportParameter("IsMarketing", (frm["IsMarketing"] == "true,false" ? "true" : frm["IsMarketing"]).ToString()));
                        para.Add(new ReportParameter("IsAddress", (frm["IsAddress"] == "true,false" ? "true" : frm["IsAddress"]).ToString()));
                        para.Add(new ReportParameter("IsRecInq", (frm["IsRecInq"] == "true,false" ? "true" : frm["IsRecInq"]).ToString()));
                        //para.Add(new ReportParameter("IsLandscape", (frm["IsLandscape"] == "true,false" ? "true" : frm["IsLandscape"]).ToString()));
                        break;
                    case "M":
                        if (frm["IsLandscape"] == "true,false")
                        {
                            Session["rptName"] = "/AGEReports/rptOutstandingReportManagerWiseLS";
                        }
                        else
                        {
                            Session["rptName"] = "/AGEReports/rptOutstandingReportManagerWise";
                        }
                        para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                        para.Add(new ReportParameter("Category", (frm["Category"]).ToString()));
                        para.Add(new ReportParameter("Status", (frm["Status"]).ToString()));
                        para.Add(new ReportParameter("Type", (frm["Assign"]).ToString()));
                        para.Add(new ReportParameter("Month", frm["Month"].ToString()));
                        para.Add(new ReportParameter("RecoveryId", frm["RecoveryId"] == "" ? "0" : frm["RecoveryId"].ToString()));
                        para.Add(new ReportParameter("ReportType", "M"));
                        para.Add(new ReportParameter("IsProduct", (frm["IsProduct"] == "true,false" ? "true" : frm["IsProduct"]).ToString()));
                        para.Add(new ReportParameter("IsGuarantor", (frm["IsGuarantor"] == "true,false" ? "true" : frm["IsGuarantor"]).ToString()));
                        para.Add(new ReportParameter("IsMarketing", (frm["IsMarketing"] == "true,false" ? "true" : frm["IsMarketing"]).ToString()));
                        para.Add(new ReportParameter("IsAddress", (frm["IsAddress"] == "true,false" ? "true" : frm["IsAddress"]).ToString()));
                        para.Add(new ReportParameter("IsRecInq", (frm["IsRecInq"] == "true,false" ? "true" : frm["IsRecInq"]).ToString()));
                        //para.Add(new ReportParameter("IsLandscape", (frm["IsLandscape"] == "true,false" ? "true" : frm["IsLandscape"]).ToString()));
                        break;
                }
            }
            else
            {
                Session["rptName"] = "/AGEReports/rptRemOSBranchWise";
                para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                para.Add(new ReportParameter("Category", (frm["Category"]).ToString()));
                para.Add(new ReportParameter("FromDate", (frm["FromDate"]).ToString()));
                para.Add(new ReportParameter("ToDate", (frm["ToDate"]).ToString()));
                para.Add(new ReportParameter("Month", (frm["Month"]).ToString()));
                para.Add(new ReportParameter("RecoveryId", frm["RecoveryId"] == "" ? "0" : frm["RecoveryId"].ToString()));
                para.Add(new ReportParameter("ReportType", "I"));
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult BranchMTDReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BranchMTDReport(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptBranchSaleAndBalanceSummary";
            List<ReportParameter> para = new List<ReportParameter>();
            //para.Add(new ReportParameter("CityId", frm["CityId"] == "" ? "0": frm["CityId"]));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("locId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("locId", "0"));
            para.Add(new ReportParameter("Date", frm["WorkingDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult DailyReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DailyReport(FormCollection frm)
        {
            Session["rptIsExport"] = "false";
            var dt = new SetupBL().GetWorkingDate(Convert.ToInt32(frm["LocId"]));
            switch (frm["rpt"])
            {
                case "1":
                    if (Convert.ToDateTime(frm["WorkingDate"]) == dt)
                    {
                        Session["rptName"] = "/AGEReports/rptDailyReport";
                    }
                    else
                    {
                        Session["rptName"] = "/AGEReports/rptDailyReportH";
                    }
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptDailyReportRM";
                    break;
                case "3":
                    Session["rptName"] = "/AGEReports/rptDailyReport1";
                    break;
                case "4":
                    Session["rptName"] = "/AGEReports/rptDailyReportOS";
                    break;
            }

            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("WorkingDate", frm["WorkingDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        #endregion
        #region Stock Report
        public ActionResult StockOutReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StockOutReport(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptProductWiseStock";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("fDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("tDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("CityID", frm["CityId"] == "" ? "0" : frm["CityId"].ToString()));
            para.Add(new ReportParameter("ProductId", frm["ProductId"] == "" ? "0" : frm["ProductId"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SKUReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SKUReport(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptSkuList";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("ComId", frm["ComId"] == "" ? "0" : frm["ComId"].ToString()));
            para.Add(new ReportParameter("ProductId", frm["ProductId"] == "" ? "0" : frm["ProductId"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult StockVerificationReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StockVerificationReport(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptStockVerification";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("DocumentNo", frm["DocId"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public async Task<JsonResult> StockVerificationList(int LocId, DateTime dateTime)
        {
            ReportBL reportBL = new ReportBL();
            var lst = await reportBL.StockVerificationList(LocId, dateTime);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StockReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StockReport(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptStockReport_V4";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("fDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("tDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult StockReportAll()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StockReportAll(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptStockReportAll";
                    para.Add(new ReportParameter("fDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("tDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("CompanyId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
                    para.Add(new ReportParameter("CatId", (frm["CatId"] == "" ? "0" : frm["CatId"]).ToString()));
                    if (frm.AllKeys.Contains("SuppId"))
                        para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
                    else
                        para.Add(new ReportParameter("SuppId", "0"));
                    if (frm.AllKeys.Contains("ProductId"))
                        para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
                    else
                        para.Add(new ReportParameter("ProductId", "0"));
                    if (frm.AllKeys.Contains("ModelId"))
                        para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
                    else
                        para.Add(new ReportParameter("ModelId", "0"));
                    if (frm.AllKeys.Contains("SKUId"))
                        para.Add(new ReportParameter("SKUId", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
                    else
                        para.Add(new ReportParameter("SKUId", "0"));
                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptStoreStatus_V3";
                    para.Add(new ReportParameter("fDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("CompanyId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
                    para.Add(new ReportParameter("CatId", (frm["CatId"] == "" ? "0" : frm["CatId"]).ToString()));
                    if (frm.AllKeys.Contains("SuppId"))
                        para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
                    else
                        para.Add(new ReportParameter("SuppId", "0"));
                    if (frm.AllKeys.Contains("ProductId"))
                        para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
                    else
                        para.Add(new ReportParameter("ProductId", "0"));
                    if (frm.AllKeys.Contains("ModelId"))
                        para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
                    else
                        para.Add(new ReportParameter("ModelId", "0"));
                    if (frm.AllKeys.Contains("SKUId"))
                        para.Add(new ReportParameter("SKUId", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
                    else
                        para.Add(new ReportParameter("SKUId", "0"));

                    para.Add(new ReportParameter("Exempt", (frm["Exempted"]).ToString()));
                    break;

                case 3:
                    Session["rptName"] = "/AGEReports/rptSrockReporAllProductWise";
                    para.Add(new ReportParameter("fDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("tDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("CompanyId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
                    para.Add(new ReportParameter("CatId", (frm["CatId"] == "" ? "0" : frm["CatId"]).ToString()));
                    if (frm.AllKeys.Contains("SuppId"))
                        para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
                    else
                        para.Add(new ReportParameter("SuppId", "0"));
                    if (frm.AllKeys.Contains("ProductId"))
                        para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
                    else
                        para.Add(new ReportParameter("ProductId", "0"));
                    if (frm.AllKeys.Contains("ModelId"))
                        para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
                    else
                        para.Add(new ReportParameter("ModelId", "0"));
                    break;
                case 4:
                    Session["rptName"] = "/AGEReports/rptProductWiseStockAll";
                    para.Add(new ReportParameter("fDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("tDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("RegionId", (frm["RegionId"] == "" ? "0" : frm["RegionId"]).ToString()));
                    para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
                    para.Add(new ReportParameter("CityID", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
                    para.Add(new ReportParameter("SaleType", (frm["SaleType"] == "" ? "0" : frm["SaleType"]).ToString()));
                    para.Add(new ReportParameter("Net", (frm["Net"] == "true,false" ? "1" : "0").ToString()));
                    break;
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult OpeningStockMobileReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OpeningStockMobileReport(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptStockOpeningMobile";
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptOpeningStock";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    break;
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult BranchStockSummary()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BranchStockSummary(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            Session["rptName"] = "/AGEReports/rptBranchWiseStock";
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult DamageReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DamageReport(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm["rpt"] == "1")
            {
                Session["rptName"] = "/AGEReports/rptStockTransferToDamage";
            }
            else if (frm["rpt"] == "2")
            {
                Session["rptName"] = "/AGEReports/rptStockTransferToSC";
            }

            para.Add(new ReportParameter("FromDate", (frm["FromDate"]).ToString()));
            para.Add(new ReportParameter("ToDate", (frm["ToDate"]).ToString()));
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));

            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProductId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            if (frm.AllKeys.Contains("SKUId"))
                para.Add(new ReportParameter("SKUId", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
            else
                para.Add(new ReportParameter("SKUId", "0"));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult DamageStockReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DamageStockReport(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm["rpt"] == "1")
            {
                Session["rptName"] = "/AGEReports/rptDamageStockHistory";
            }
            else if (frm["rpt"] == "2")
            {
                Session["rptName"] = "/AGEReports/rptDamageStock";
            }
            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            para.Add(new ReportParameter("StatusID", (frm["StatusID"] == "" ? "0" : frm["StatusID"]).ToString()));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult StockDetailReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StockDetailReport(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm["IsAbid"] == "3")
            {
                Session["rptName"] = "/AGEReports/rptStoreStatus_V2";
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            }
            else if (frm["IsAbid"] == "1")
            {
                Session["rptName"] = "/AGEReports/rptStoreStatus_V1";
                if (frm.AllKeys.Contains("CityId"))
                    para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
                else
                    para.Add(new ReportParameter("CityId", "0"));
            }
            else
            {
                Session["rptName"] = "/AGEReports/rptStoreStatus_V1";
                para.Add(new ReportParameter("CityId", "50"));
            }
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("SuppId"))
                para.Add(new ReportParameter("SupId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            else
                para.Add(new ReportParameter("SupId", "0"));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProdId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProdId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            if (frm.AllKeys.Contains("SKUId"))
                para.Add(new ReportParameter("Skuid", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
            else
                para.Add(new ReportParameter("Skuid", "0"));

            para.Add(new ReportParameter("StatusID", (frm["StatusID"] == "" ? "0" : frm["StatusID"]).ToString()));

            para.Add(new ReportParameter("Exempted", (frm["Exempted"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }



        public ActionResult CurrentStockReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CurrentStockReport(FormCollection frm)
        {
            ReportBL reportBL = new ReportBL();
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("ComId"))
                para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            else
                para.Add(new ReportParameter("ComId", "0"));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProdId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProdId", "0"));
            if (frm.AllKeys.Contains("SuppId"))
                para.Add(new ReportParameter("SupId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            else
                para.Add(new ReportParameter("SupId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            if (frm.AllKeys.Contains("SKUId"))
                para.Add(new ReportParameter("Skuid", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
            else
                para.Add(new ReportParameter("Skuid", "0"));
            switch (frm["rpt"])
            {
                case "StockDetailWise":
                    Session["rptName"] = "/AGEReports/rptStoreStatus";
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    Session["rptParameter"] = para;
                    return Redirect("~/Report.aspx");
                case "StockTypeWise":
                    rptStockTypeWise rpt = new rptStockTypeWise();
                    int locId = Convert.ToInt32(frm["LocId"]);
                    rpt.SetDataSource(reportBL.StockTypeWiseReport(locId));
                    rpt.SetParameterValue("LocName", frm["LocName"]);
                    //ReportDocument rptDoc = rpt;
                    rpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                    rpt.Close();
                    rpt.Dispose();
                    return Json("");
                    //Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //stream.Seek(0, SeekOrigin.Begin);
                    //Response.AddHeader("Content-Disposition", "inline; filename=StockTypeWise.pdf");
                    //return File(stream, "application/pdf", "StockTypeWise.pdf");
            }
            return View();
        }
        #endregion
        #region Cash
        public ActionResult CashDeposit()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashDeposit(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptCashDeposit";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("Tdate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        #endregion

        public ActionResult MonthlyCreditSalary()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyCreditSalary(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptPay_SalarySheet_Credit";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("SalaryMonth", frm["Month"].ToString()));
            para.Add(new ReportParameter("HDeptId", ("0").ToString()));
            //para.Add(new ReportParameter("DeptId", ("0").ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult MonthlyClosing()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyClosing(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptPay_Branches_Month_Closing";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("ClosingMonth", frm["FromDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult PurchaseAudit()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchaseAudit(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptLocalPurchaseAudit";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            if (frm.AllKeys.Contains("ComId"))
                para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            else
                para.Add(new ReportParameter("ComId", "0"));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProdId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProdId", "0"));
            //if (frm.AllKeys.Contains("SuppId"))
            //    para.Add(new ReportParameter("SupId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            //else
            //    para.Add(new ReportParameter("SupId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            if (frm.AllKeys.Contains("SKUId"))
                para.Add(new ReportParameter("SkuId", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
            else
                para.Add(new ReportParameter("Skuid", "0"));






            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult CRCFineDetail()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CRCFineDetail(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptCRCFineDetail";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("CRC", (frm["CRC"] == "" ? "0" : frm["CRC"]).ToString()));
            para.Add(new ReportParameter("Month", frm["Month"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult CashTransfer()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashTransfer(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptCashTransfer";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult RegionWiseCashReceive()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegionWiseCashReceive(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptCashReceiveRegionWise";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            if (frm["IsPosted"].ToString() != "2")
            {
                para.Add(new ReportParameter("IsPosted", frm["IsPosted"].ToString()));
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult RegionWiseCashDeposit()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegionWiseCashDeposit(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptCashDepositRegionWise";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            if (frm["IsPosted"].ToString() != "2")
            {
                para.Add(new ReportParameter("IsPosted", frm["IsPosted"].ToString()));
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult CashReceive()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashReceive(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptCashReceive";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult Modifications()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Modifications(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptModification";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("ModType", frm["ModType"]));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SaleDetailCustomized()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaleDetailCustomized(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();

            Session["rptName"] = "/AGEReports/rptSalesReportCustomised";
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("SuppId"))
                para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            else
                para.Add(new ReportParameter("SuppId", "0"));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProductId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SaleDetail()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaleDetail(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptSalesReportRateWise";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptInstaSaleWithPlan";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("CategoryId", (frm["CategoryId"] == "" ? "0" : frm["CategoryId"]).ToString()));
                    break;
                case 3:
                    Session["rptName"] = "/AGEReports/rptCashSaleSummary";
                    para.Add(new ReportParameter("Date", frm["FromDate"].ToString()));
                    break;
                case 4:
                    Session["rptName"] = "/AGEReports/rptSalesReportDiscWise";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    break;
            }
            if (Convert.ToInt32(frm["rpt"]) != 3)
            {
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
                if (frm.AllKeys.Contains("LocId"))
                    para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
                else
                    para.Add(new ReportParameter("LocId", "0"));
                para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
                if (frm.AllKeys.Contains("SuppId"))
                    para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
                else
                    para.Add(new ReportParameter("SuppId", "0"));
                if (frm.AllKeys.Contains("ProductId"))
                    para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
                else
                    para.Add(new ReportParameter("ProductId", "0"));
                if (frm.AllKeys.Contains("ModelId"))
                    para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
                else
                    para.Add(new ReportParameter("ModelId", "0"));
            }


            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult Implementation()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Implementation(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptImplementation";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("pLocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("pLocId", "0"));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult SalevsTarget()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalevsTarget(FormCollection frm)
        {
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptSaleVsTarget";
                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptSaleVsTarget_PR";
                    break;
            }

            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityID", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityID", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            if (frm.AllKeys.Contains("GMId"))
                para.Add(new ReportParameter("GMId", (frm["GMId"] == "" ? "0" : frm["GMId"]).ToString()));
            else
                para.Add(new ReportParameter("GMId", "0"));
            para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("TransType", frm["Type"].ToString()));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult ReceivableProjectionCW()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReceivableProjectionCW(FormCollection frm)
        {
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptReceivablesProjectionCustomerWise";
                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptReceivablesProjectionAll";
                    break;
            }

            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LociD", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult EmployeePerformance()
        {
            ViewBag.rptTitle = "Employee Performance";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeePerformance(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptEmployeePerformance";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("PerformanceMonth", frm["Month"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult RecoveryPerformanceDayWise()
        {

            ViewBag.rptTitle = "Recovery Performance DayWise";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecoveryPerformanceDayWise(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptRecvPerf";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("Month", frm["Month"].ToString()));
            para.Add(new ReportParameter("Category", frm["Category"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult ItemLedger()
        {
            ViewBag.rptTitle = "Item Ledger";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemLedger(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptItemLedger";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("fdate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("tdate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("SKUID", frm["SKUId"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult SaleRateLog()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaleRateLog(FormCollection frm)
        {

            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "SaleRateLog":
                    Session["rptName"] = "/AGEReports/rptStockSaleRateLog";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    Session["rptParameter"] = para;
                    return Redirect("~/Report.aspx");
                case "InstPlanLog":
                    Session["rptName"] = "/AGEReports/rptStockInstPlanLog";
                    para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("Type", frm["Type"].ToString()));
                    Session["rptParameter"] = para;
                    return Redirect("~/Report.aspx");
            }
            return View();
        }


        public ActionResult EmployeeHierarchy()
        {
            return View();
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeHierarchy(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptEmployeeHierarchy_2";
            List<ReportParameter> para = new List<ReportParameter>();
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }



        public ActionResult ReceiveableProjection()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReceiveableProjection(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (Convert.ToInt32(frm["rpt"]))
            {
                case 1:
                    Session["rptName"] = "/AGEReports/rptReceivablesProjectionBranchWise";
                    para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));

                    break;
                case 2:
                    Session["rptName"] = "/AGEReports/rptReceivablesProjectionRegionWise";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    Session["rptParameter"] = para;
                    break;
            }

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult StockIssueReceive()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StockIssueReceive(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            Session["rptName"] = "/AGEReports/rptIssueReceivedStock";
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            para.Add(new ReportParameter("FDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("TDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("Status", frm["Sta"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult OfficerPerformance()
        {

            ViewBag.rptTitle = "Officer Performance Month Wise";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OfficerPerformance(FormCollection frm)
        {
            switch (frm["rpt"])
            {
                case "1":
                    Session["rptName"] = "/AGEReports/rptOfficersPerformanceMinimum";
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptOfficersPerformanceMaximum";
                    break;
            }

            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult PurchasePayments()
        {
            ViewBag.rptTitle = "Purchase Payments Report";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchasePayments(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptPurchasePayments";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            para.Add(new ReportParameter("SuppName", frm["SuppName"].ToString()));
            para.Add(new ReportParameter("SuppMobile", frm["SuppMobile"].ToString()));
            para.Add(new ReportParameter("SuppAddress", frm["SuppAddress"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult CashClosingAll()
        {
            ViewBag.rptTitle = "Cash Closing Report";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashClosingAll(FormCollection frm)
        {
            if (frm["rpt"] == "1")
            {
                Session["rptName"] = "/AGEReports/rptCashClosingAll";
            }
            else
            {
                Session["rptName"] = "/AGEReports/rptCashClosingAll4CCC";
            }

            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("WorkingDate", frm["Date"].ToString()));
            para.Add(new ReportParameter("CashCenter", frm["LocId"] == "" ? "0" : frm["LocId"].ToString()));
            para.Add(new ReportParameter("WithOpening", frm["WithOpening"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult CashClosing()
        {
            ViewBag.rptTitle = "Cash Closing Report";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashClosing(FormCollection frm)
        {
            Session["rptIsExport"] = "false";
            Session["rptName"] = "/AGEReports/rptCashClosing";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("WorkingDate", frm["Month"].ToString()));
            para.Add(new ReportParameter("LocId", frm["LocId"].ToString()));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult TaxSale()
        {
            ViewBag.rptTitle = "Tax Sale Report";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaxSale(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptTaxSales";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("Category", frm["Category"].ToString()));
            para.Add(new ReportParameter("SyncStatus", frm["SyncStatus"].ToString()));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult POList()
        {
            ViewBag.rptTitle = "PO List Report";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult POList(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptRep_POList";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm["POTypeId"].ToString() == "1")
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));

            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            para.Add(new ReportParameter("POTypeId", frm["POTypeId"].ToString()));
            para.Add(new ReportParameter("CompanyId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProductId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProductId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            if (frm.AllKeys.Contains("SKUId"))
                para.Add(new ReportParameter("SKUId", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
            else
                para.Add(new ReportParameter("SKUId", "0"));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult TargetIncentive()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TargetIncentive(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptTarget_Incentive_Summary";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));

            para.Add(new ReportParameter("TIMonth", frm["Month"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult SMSAll()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMSAll(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptSMSAll";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult AttendanceMap()
        {
            return View();
        }
        public JsonResult GetAttendanceMap(DateTime dt)
        {
            var lst = new ReportBL().GetAttMap(dt);

            return Json(lst, JsonRequestBehavior.AllowGet);

        }
        public ActionResult SupplierAging()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierAging(FormCollection frm)
        {
            Session.Remove("rptName");
            List<ReportParameter> para = new List<ReportParameter>();

            switch (frm["rpt"])
            {
                case "1":
                    Session["rptName"] = "/AGEReports/rptGRNDetailAging";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
                    para.Add(new ReportParameter("Type", frm["Type"].ToString()));
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptGRNSummaryAging";
                    para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("SuppCatId", (frm["CatId"] == "" ? "0" : frm["CatId"]).ToString()));
                    para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
                    para.Add(new ReportParameter("Type", frm["Type"].ToString()));
                    break;
            }

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult GMWiseSrm()
        {
            ViewBag.rptTitle = "GM Wise SRM Report";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GMWiseSrm(FormCollection frm)
        {
            Session.Remove("rptName");
            List<ReportParameter> para = new List<ReportParameter>();

            switch (frm["rpt"])
            {
                case "1":
                    Session["rptName"] = "/AGEReports/rptOSSRMWise";
                    para.Add(new ReportParameter("GMId", (frm["GMId"] == "" ? "0" : frm["GMId"]).ToString()));
                    para.Add(new ReportParameter("Month", frm["Month"].ToString()));
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptGMWiseOS";
                    para.Add(new ReportParameter("Date", frm["Month"].ToString()));
                    break;
                case "3":
                    Session["rptName"] = "/AGEReports/rptRMWiseOSMonthlyComparison";
                    para.Add(new ReportParameter("GMId", (frm["GMId"] == "" ? "0" : frm["GMId"]).ToString()));
                    para.Add(new ReportParameter("FromMonth", frm["FromDate"].ToString()));
                    para.Add(new ReportParameter("ToMonth", frm["ToDate"].ToString()));
                    para.Add(new ReportParameter("Type", frm["Category"].ToString()));

                    break;
                case "4":
                    Session["rptName"] = "/AGEReports/rptRemaininfAndRecoveredOS";
                    para.Add(new ReportParameter("GMId", (frm["GMId"] == "" ? "0" : frm["GMId"]).ToString()));
                    para.Add(new ReportParameter("WorkingDate", frm["WorkingDate"].ToString()));
                    para.Add(new ReportParameter("SortBy", frm["SortBy"].ToString()));
                    break;
                case "5":
                    Session["rptName"] = "/AGEReports/rptRMWiseOSSummary";
                    para.Add(new ReportParameter("GMId", (frm["GMId"] == "" ? "0" : frm["GMId"]).ToString()));
                    para.Add(new ReportParameter("Month", frm["Month"].ToString()));
                    break;
            }

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }




        public ActionResult GMWiseInstSale()
        {
            ViewBag.rptTitle = "Installment Sale Summary GM Wise";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GMWiseInstSale(FormCollection frm)
        {
            Session.Remove("rptName");
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "1":
                    Session["rptName"] = "/AGEReports/rptGMWiseInstSale";
                    para.Add(new ReportParameter("GMId", (frm["GMId"] == "" ? "0" : frm["GMId"]).ToString()));
                    para.Add(new ReportParameter("WorkingDate", frm["Month"].ToString()));
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptInstSaleSummary";
                    para.Add(new ReportParameter("WorkingDate", frm["Month"].ToString()));
                    para.Add(new ReportParameter("Type", frm["Designation"].ToString()));
                    break;
                case "3":
                    Session["rptName"] = "/AGEReports/rptMonthWiseSaleCompSum";
                    para.Add(new ReportParameter("GMId", (frm["GMId"] == "" ? "0" : frm["GMId"]).ToString()));
                    para.Add(new ReportParameter("PreDate", frm["Month"].ToString()));
                    para.Add(new ReportParameter("WorkingDate", frm["ToMonth"].ToString()));
                    break;
            }
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult BikeLetter()
        {
            ViewBag.rptTitle = "Bike Letter Report";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BikeLetter(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptBikeLetter";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("TypeId"))
                para.Add(new ReportParameter("TypeId", (frm["TypeId"] == "" ? "0" : frm["TypeId"]).ToString()));
            else
                para.Add(new ReportParameter("TypeId", "0"));

            if (frm.AllKeys.Contains("SuppId"))
                para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            else
                para.Add(new ReportParameter("SuppId", "0"));

            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult BikeLetterSale()
        {
            ViewBag.rptTitle = "Bike Letter Sale Report";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BikeLetterSale(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptBikeLetterSale";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            if (frm.AllKeys.Contains("TypeId"))
                para.Add(new ReportParameter("TypeId", (frm["TypeId"] == "" ? "0" : frm["TypeId"]).ToString()));
            else
                para.Add(new ReportParameter("TypeId", "0"));
            if (frm.AllKeys.Contains("SKUId"))
                para.Add(new ReportParameter("SKUId", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
            else
                para.Add(new ReportParameter("SKUId", "0"));

            if (frm.AllKeys.Contains("SuppId"))
                para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            else
                para.Add(new ReportParameter("SuppId", "0"));

            if (frm.AllKeys.Contains("SaleType"))
                para.Add(new ReportParameter("SaleType", (frm["SaleType"] == "" ? "B" : frm["SaleType"]).ToString()));
            else
                para.Add(new ReportParameter("SaleType", "B"));



            para.Add(new ReportParameter("SerialNo", frm["SerialNo"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }



        public ActionResult StockAdjustment()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StockAdjustment(FormCollection frm)
        {
            Session.Remove("rptName");
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "1":
                    Session["rptName"] = "/AGEReports/rptStockAdjustment";
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptStockAdjustmentOut";
                    break;
            }

            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult StoreVerificationAll()
        {
            ViewBag.rptTitle = "Region Wise Store Verification All";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreVerificationAll(FormCollection frm)
        {


            Session["rptName"] = "/AGEReports/rptStockVerificationAll";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocID", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocID", "0"));


            para.Add(new ReportParameter("Status", frm["Status"].ToString()));
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }



        public ActionResult AccWiseremBal()
        {
            ViewBag.rptTitle = "Region Wise Customer Balances";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccWiseremBal(FormCollection frm)
        {


            Session["rptName"] = "/AGEReports/rptAccWiseremBal";
            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));

            para.Add(new ReportParameter("WorkingDate", frm["WorkingDate"].ToString()));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }



        public ActionResult GrnVsInv()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GrnVsInv(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptGrnVsInv";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));

            if (frm.AllKeys.Contains("CategoryId"))
                para.Add(new ReportParameter("CategoryId", (frm["CategoryId"] == "" ? "0" : frm["CategoryId"]).ToString()));
            else
                para.Add(new ReportParameter("CategoryId", "0"));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult InvoicePendancy()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvoicePendancy(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptInvoicePendancy";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));

            if (frm.AllKeys.Contains("CategoryId"))
                para.Add(new ReportParameter("CategoryId", (frm["CategoryId"] == "" ? "0" : frm["CategoryId"]).ToString()));
            else
                para.Add(new ReportParameter("CategoryId", "0"));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult ReturnStockReport()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReturnStockReport(FormCollection frm)
        {

            List<ReportParameter> para = new List<ReportParameter>();

            Session["rptName"] = "/AGEReports/rptOldPieceReturnStock";

            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));

            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("ComId", (frm["ComId"] == "" ? "0" : frm["ComId"]).ToString()));
            if (frm.AllKeys.Contains("SuppId"))
                para.Add(new ReportParameter("SupId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            else
                para.Add(new ReportParameter("SupId", "0"));
            if (frm.AllKeys.Contains("ProductId"))
                para.Add(new ReportParameter("ProdId", (frm["ProductId"] == "" ? "0" : frm["ProductId"]).ToString()));
            else
                para.Add(new ReportParameter("ProdId", "0"));
            if (frm.AllKeys.Contains("ModelId"))
                para.Add(new ReportParameter("ModelId", (frm["ModelId"] == "" ? "0" : frm["ModelId"]).ToString()));
            else
                para.Add(new ReportParameter("ModelId", "0"));
            if (frm.AllKeys.Contains("SKUId"))
                para.Add(new ReportParameter("Skuid", (frm["SKUId"] == "" ? "0" : frm["SKUId"]).ToString()));
            else
                para.Add(new ReportParameter("Skuid", "0"));

            para.Add(new ReportParameter("Aging", (frm["Aging"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }



        public ActionResult TicketDetail()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TicketDetail(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptCrmTicketDetail";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));

            if (frm.AllKeys.Contains("CategoryId"))
                para.Add(new ReportParameter("CategoryId", (frm["CategoryId"] == "" ? "0" : frm["CategoryId"]).ToString()));
            else
                para.Add(new ReportParameter("CategoryId", "0"));
            if (frm.AllKeys.Contains("MCategoryId"))
                para.Add(new ReportParameter("MCategoryId", (frm["MCategoryId"] == "" ? "0" : frm["MCategoryId"]).ToString()));
            else
                para.Add(new ReportParameter("MCategoryId", "0"));
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));



            para.Add(new ReportParameter("Status", frm["Status"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult UserRight()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserRight(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptUserRightsDetail";
            List<ReportParameter> para = new List<ReportParameter>();

            if (frm.AllKeys.Contains("GroupId"))
                para.Add(new ReportParameter("GroupId", (frm["GroupId"] == "" ? "0" : frm["GroupId"]).ToString()));
            else
                para.Add(new ReportParameter("GroupId", "0"));
            if (frm.AllKeys.Contains("EmployeeId"))
                para.Add(new ReportParameter("UserId", (frm["EmployeeId"] == "" ? "0" : frm["EmployeeId"]).ToString()));
            else
                para.Add(new ReportParameter("EmployeeId", "0"));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult CrcRemarks()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrcRemarks(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptCrcRemarks";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            if (frm.AllKeys.Contains("CityId"))
                para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            else
                para.Add(new ReportParameter("CityId", "0"));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult RMTargetIncentive()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RMTargetIncentive(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptRMTargetIncentive";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            para.Add(new ReportParameter("SalaryMonth", frm["Month"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult CashFlowStatement()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashFlowStatement(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptCashFlowStatement";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult CharityDonationReport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CharityDonationReport(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/CharityDonationReport";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("Month", frm["Month"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        public ActionResult EmpAllowDeduction()
        {
            ViewBag.AllowancesDeductionsType = SelectListVM.AllowancesDeductionsType;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmpAllowDeduction(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptEmpAllowDeduction";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));

            if (frm.AllKeys.Contains("Type"))
                para.Add(new ReportParameter("Type", (frm["Type"] == "" ? "0" : frm["Type"]).ToString()));
            else
                para.Add(new ReportParameter("Type", "0"));

            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult ChequeRegister()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChequeRegister(FormCollection frm)
        {
            List<ReportParameter> para = new List<ReportParameter>();
            switch (frm["rpt"])
            {
                case "1":
                    Session["rptName"] = "/AGEReports/rptChequeRegister";
                    para.Add(new ReportParameter("ChequeType", frm["ChequeType"].ToString()));
                    break;
                case "2":
                    Session["rptName"] = "/AGEReports/rptSecurityCheque";
                    para.Add(new ReportParameter("ChequeType", "Security"));
                    break;
                default:
                    Session["rptName"] = "/AGEReports/rptPDC";
                    para.Add(new ReportParameter("ChequeType", "PDC"));
                    break;
            }

            para.Add(new ReportParameter("FromDate", frm["FromDate"].ToString()));
            para.Add(new ReportParameter("ToDate", frm["ToDate"].ToString()));
            para.Add(new ReportParameter("PaymentType", frm["PaymentType"].ToString()));
            para.Add(new ReportParameter("Status", frm["ChequeStatus"].ToString()));
            para.Add(new ReportParameter("AccNo", frm["AccId"].ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }


        public ActionResult BackendIncentivePolicies()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BackendIncentivePolicies(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptBackendIncentive_Policies";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("SupId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }

        #region Procurement
        public ActionResult MTR()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MTR(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptProMTR";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("CityId", (frm["CityId"] == "" ? "0" : frm["CityId"]).ToString()));
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            para.Add(new ReportParameter("Status", (frm["Status"]).ToString()));
            para.Add(new ReportParameter("FromDate", (frm["FromDate"]).ToString()));
            para.Add(new ReportParameter("ToDate", (frm["ToDate"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult ProPO()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProPO(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptProPODetail";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("Status", (frm["Status"]).ToString()));
            para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            para.Add(new ReportParameter("NatureId", (frm["NatureId"] == "" ? "0" : frm["NatureId"]).ToString()));
            para.Add(new ReportParameter("FromDate", (frm["FromDate"]).ToString()));
            para.Add(new ReportParameter("ToDate", (frm["ToDate"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult ProSSI()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProSSI(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptProSSI";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("NatureId", (frm["NatureId"] == "" ? "0" : frm["NatureId"]).ToString()));
            para.Add(new ReportParameter("Status", (frm["Status"]).ToString()));
            para.Add(new ReportParameter("SuppId", (frm["SuppId"] == "" ? "0" : frm["SuppId"]).ToString()));
            para.Add(new ReportParameter("FromDate", (frm["FromDate"]).ToString()));
            para.Add(new ReportParameter("ToDate", (frm["ToDate"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult ProSIN()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProSIN(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptProSINDetail";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("NatureId", (frm["NatureId"] == "" ? "0" : frm["NatureId"]).ToString()));
            para.Add(new ReportParameter("CCCode", (frm["CCCode"] == "" ? "0" : frm["CCCode"]).ToString()));
            para.Add(new ReportParameter("FromDate", (frm["FromDate"]).ToString()));
            para.Add(new ReportParameter("ToDate", (frm["ToDate"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult ProStock()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProStock(FormCollection frm)
        {
            Session["rptName"] = "/AGEReports/rptProStock";
            List<ReportParameter> para = new List<ReportParameter>();
            para.Add(new ReportParameter("ItemCategoryId", (frm["ItemCategoryId"] == "" ? "0" : frm["ItemCategoryId"]).ToString()));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        public ActionResult FAR()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FAR(FormCollection frm)
        {
            switch (frm["rpt"])
            {
                case "1":
                    {
                        Session["rptName"] = "/AGEReports/rptFAR";
                        break;
                    }
                case "2":
                    {
                        Session["rptName"] = "/AGEReports/rptFAROpening";
                        break;
                    }
            }

            List<ReportParameter> para = new List<ReportParameter>();
            if (frm.AllKeys.Contains("LocId"))
                para.Add(new ReportParameter("LocId", (frm["LocId"] == "" ? "0" : frm["LocId"]).ToString()));
            else
                para.Add(new ReportParameter("LocId", "0"));
            Session["rptParameter"] = para;
            return Redirect("~/Report.aspx");
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            GC.Collect(0);
        }
    }
}
