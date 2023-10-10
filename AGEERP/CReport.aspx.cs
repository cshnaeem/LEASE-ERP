using AGEERP.CrReports;
using AGEERP.Models;
using CrystalDecisions.CrystalReports.Engine;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace AGEERP
{
    public partial class CReport : System.Web.UI.Page
    {
        ReportDocument rpt;
        protected void Page_PreInit(object sender, EventArgs e)
        {
            //if (!this.IsPostBack)
            //{

            //    if (Session["rptC"] != null)
            //    {
            //        ReportDocument rpt = (ReportDocument)Session["rptC"];
            //        rpt.PrintToPrinter(1, true, 0, 0);
            //        CrystalReportViewer1.ReportSource = rpt;
            //    }
            //}
            if (Session["rptC"] != null)
            {
                //if (Session["IsAutoPrint"] == "All")
                //{
                //    ReportBL reportBL = new ReportBL();
                //    long[] arr = (long[])Session["AccNos"];
                //    foreach (var item in arr)
                //    {
                //        rptCustInfoDetail rpt = new rptCustInfoDetail();
                //        List<InstDetailVM> lst = reportBL.GetInstByAccNo(Convert.ToInt64(item));
                //        List<CustomerDetailRVM> lst1 = reportBL.GetCustomerInfo(Convert.ToInt64(item));
                //        rpt.Database.Tables["AGEERP_Models_InstDetailVM"].SetDataSource(lst);
                //        rpt.Database.Tables["AGEERP_CrReports_CustomerDetailRVM"].SetDataSource(lst1);
                //        rpt.PrintToPrinter(1, true, 0, 0);
                //    }
                //    ClientScript.RegisterStartupScript(typeof(Page), "closePage", "window.close();", true);
                //}
                //else 
                //if (Session["IsAutoPrint"] == "Yes")
                //{
                //    ReportDocument rpt = (ReportDocument)Session["rptC"];
                //    rpt.PrintToPrinter(1, true, 0, 0);
                //    CrystalReportViewer1.ReportSource = rpt;
                //}
                //else
                //{
                rpt = (ReportDocument)Session["rptC"];
                CrystalReportViewer1.ReportSource = rpt;
                //}

            }
        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            CrystalReportViewer1.Dispose();
        }
    }
}