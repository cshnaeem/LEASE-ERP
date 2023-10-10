using AGEERP.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;

namespace AGEERP
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Session["rptName"] != null)
                {

                    ReportViewer1.ServerReport.ReportPath = Session["rptName"] as string;
                    if (Session["rptIsExport"] != null)
                    {
                        if (Session["rptIsExport"] as string == "false")
                        {
                            ReportViewer1.ShowExportControls = false;
                        }
                    }

                    //if (para != null)
                    //    ReportViewer1.ServerReport.SetParameters(para);


                    Session["rptName"] = null;
                    Session["rptIsExport"] = null;
                }
            }
        }
    }
}