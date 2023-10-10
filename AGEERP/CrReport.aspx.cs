using AGEERP.CrReports;
using AGEERP.Models;
using CrystalDecisions.CrystalReports.Engine;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace AGEERP
{
    public partial class CrReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                //ReportBL reportBL = new ReportBL();
                //List<long> arr = (List<long>)Session["AccNos"];
                //if (arr.Count() > 0)
                //{
                //    rptCustInfoDetail rpt = new rptCustInfoDetail();
                //    List<InstDetailVM> lst = reportBL.GetInstByAcc(Convert.ToInt64(arr[0]));
                //    List<CustomerDetailRVM> lst1 = reportBL.GetCustomerInfo(Convert.ToInt64(arr[0]));
                //    rpt.Database.Tables["AGEERP_Models_InstDetailVM"].SetDataSource(lst);
                //    rpt.Database.Tables["AGEERP_CrReports_CustomerDetailRVM"].SetDataSource(lst1);
                //    //rpt.PrintToPrinter(1, true, 0, 0);
                //    CrystalReportViewer1.ReportSource = rpt;
                //    ClientScript.RegisterStartupScript(this.GetType(), "Print", "PrintAllP();", true);
                //    arr.RemoveAt(0);
                //    Session["AccNos"] = arr;
                //}

            }
        }
    }
}