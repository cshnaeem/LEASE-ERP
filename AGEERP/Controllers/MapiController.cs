using AGEERP.CrReports;
using AGEERP.Models;
using NotifSystem.Web.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AGEERP.Controllers
{
    [MRBAC]
    public class MapiController : ApiController
    {

        #region UserLogin
        [System.Web.Http.Route("api/Mapi/Login")]
        [System.Web.Http.HttpPost]

        public async Task<HttpResponseMessage> Login([FromBody] MUserVM User)
        {
            if (!String.IsNullOrWhiteSpace(User.Username) && !String.IsNullOrWhiteSpace(User.Password) && !String.IsNullOrWhiteSpace(User.DeviceId))
            {
                var UserObj = await new SecurityBL().MLoginUser(User);
                if (UserObj.Msg != "OK")
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, UserObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, UserObj);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new MUserVM() { Status = false, Msg = "Required Fields Are Missing." });
            }

        }

        [System.Web.Http.Route("api/Mapi/ForgotPasswordMobile")]
        [System.Web.Http.HttpPost]

        public async Task<HttpResponseMessage> ForgotPasswordMobile([FromBody] ForgotPasswordVM User)
        {
            if (!String.IsNullOrWhiteSpace(User.Username) && !String.IsNullOrWhiteSpace(User.MobileNo) && !String.IsNullOrWhiteSpace(User.DeviceId))
            {
                var UserObj = await new SecurityBL().ForgotPasswordMobile(User);
                if (UserObj != "OK")
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, UserObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, UserObj);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new MUserVM() { Status = false, Msg = "Required Fields Are Missing." });
            }

        }
        [System.Web.Http.Route("api/Mapi/LoginRequest")]
        [System.Web.Http.HttpPost]

        public async Task<HttpResponseMessage> LoginRequest([FromBody] LoginRequestVM User)
        {
            if (!String.IsNullOrWhiteSpace(User.CNIC) && User.UserId != 0)
            {
                var UserObj = await new SecurityBL().CreateLoginRequest(User);
                if (UserObj.Msg != "OK")
                {
                    try
                    {
                        var notiLst = new NotificationBL().PostNotiLoc(10, 0, "You have new Login request from " + UserObj.UserId + " (" + UserObj.EmpName + ")", 100003);
                        //foreach (var item in notiLst)
                        //{
                        //    objNotifHub.SendNotification(item);
                        //}
                    }
                    catch (Exception)
                    {
                    }
                    return Request.CreateResponse(HttpStatusCode.NotFound, UserObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, UserObj);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new MUserVM() { Status = false, Msg = "Required Fields Are Missing." });
            }

        }
        [System.Web.Http.Route("api/Mapi/ChangePassword")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> ChangePassword([FromBody] MUserVM User)
        {
            User.Password = User.Password.Trim();
            if (!String.IsNullOrWhiteSpace(User.Username) && !String.IsNullOrWhiteSpace(User.Password) && !String.IsNullOrWhiteSpace(User.DeviceId))
            {
                var UserObj = await new SecurityBL().MChangePassword(User);
                if (UserObj != "Save Successfully")
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, UserObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, UserObj);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new MUserVM() { Status = false, Msg = "Required Fields Are Missing." });
            }
        }
        [System.Web.Http.Route("api/Mapi/MobileAppVersion")]
        [System.Web.Http.HttpGet]
        //[MRBAC]
        public async Task<HttpResponseMessage> MobileAppVersion()
        {
            var UserObj = await new SetupBL().GetMobileAppVersion();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        #endregion
        #region LeaseCustomer
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetCashCollList")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> GetCashCollList(long EmpId)
        {
            var UserObj = await new CashBL().GetCashCollList(EmpId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/SaveCashColl")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> SaveCashColl(CashCollectionListVM mod)
        {
            var UserObj = await new CashBL().SaveCashColl(mod);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }


        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetAudit")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> GetAudit(long AccNo)
        {
            var UserObj = await new SaleBL().GetAudit(AccNo);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/SaveAudit")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> SaveAudit(LseAuditVM mod, int LocId, int UserId)
        {
            var UserObj = await new SaleBL().SaveAudit(mod, LocId, UserId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/PlanPolicyList")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> PlanPolicyList(int LocId, int SKUId, bool IsLocal, int Duration)
        {
            var lst = await new SaleBL().PlanPolicyList(LocId, SKUId, IsLocal, Duration);
            var data = lst.Select(x => new { MarkUp = x.MarkUp, RowId = x.PolicyId }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/InstPriceBySKUAuto")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> InstPriceBySKUAuto(int LocId, int SKUId, decimal Advance, decimal PPrice, bool IsLocal, int PolicyId)
        {
            var x = await new SaleBL().InstPlanBySKU(LocId, SKUId, Advance, PPrice, IsLocal, PolicyId);
            if (x != null)
            {
                var data = new
                {
                    InstPrice = x.InstPrice,
                    Advance = x.Advance,
                    Inst = x.Inst,
                    //Duration = x.Duration,
                    //RowId = x.RowId,
                    Msg = x.Remarks
                };
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            else
            {
                var data = new
                {
                    InstPrice = 0,
                    Advance = 0,
                    Inst = 0,
                    //Duration = x.Duration,
                    //RowId = x.RowId,
                    Msg = "No Plan Found"
                };
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/VPNSearch")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> VPNSearch(int LocId, int Crit1, string CritVal1, int Status, int UserId)
        {
            var UserObj = new SaleBL().GetVPNSearch(LocId, Crit1, CritVal1, Status, UserId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetBlockCustomer")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> GetBlockCustomer(int Crit1, string CritVal1)
        {
            var UserObj = await new SaleBL().GetBlockCustomer(Crit1, CritVal1);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/SaleManWiseCashSaleSummary")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage GetCashSaleManWiseSummary(int LocId, DateTime FromDate, DateTime ToDate)
        {
            var UserObj = new SaleBL().GetCashSaleSummarySaleManWise(LocId, FromDate, ToDate);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/UpdateLseCustomer")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> UpdateLseCustomer(long AccNo, string ResAddress, string OffAddress)
        {
            var UserObj = await new SaleBL().UpdateLseCustomer(AccNo, ResAddress, OffAddress);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/UpdateLseCustomerLatLng")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> UpdateLatLng(long AccNo, string Type, decimal Lat, decimal Lng)
        {
            var UserObj = await new SaleBL().UpdateLatLng(AccNo, Type, Lat, Lng);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetLseCustomers")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetLseCustomers(int LocId, int Status)
        {
            var UserObj = await new SaleBL().GetLeaseCustomer(LocId, Status);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetLseOutstand")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetLseOutstand(int LocId, string Category, string Status)
        {
            var UserObj = await new SaleBL().GetLeaseOutstand(LocId, Category, Status);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetLocByIp")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetLocByIp(string IP)
        {
            var UserObj = await new SaleBL().GetLocByIp(IP);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        [System.Web.Http.Route("api/Mapi/GetUtilityVersion")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetUtilityVersion()
        {
            var UserObj = await new SaleBL().GetUtilityVersion();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/ReportPDF")]
        [System.Web.Http.HttpGet]
        public async Task<string> ReportPDF(long TransId)
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
            return ""; //Request.CreateResponse(HttpStatusCode.OK, "");
        }

        [System.Web.Http.Route("api/Mapi/PostPendTax")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> PostPendTax()
        {
            var UserObj = await new TaxBL().PostPend();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetOfficerPerf")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetOfficerPerf(int EmpId)
        {
            var UserObj = new DashboardBL().GetRecOffPerfForMobile(EmpId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetOfficerPerfLoc")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetOfficerPerfLoc(int LocId)
        {
            var UserObj = new DashboardBL().GetEmployeePerf(LocId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetSaleVsTarget")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetSaleVsTarget(int EmpId)
        {
            var UserObj = new DashboardBL().GetSaleVsTarget(EmpId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetLeaseCustomers")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetLeaseCustomers(int RecoveryId, string Category = "")
        {
            var UserObj = await new SaleBL().GetLeaseCustomer(RecoveryId, Category);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/UpdateOutstandRemarks")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> UpdateOutstandRemarks(long AccNo, string Remarks)
        {
            var UserObj = await new SaleBL().UpdateOutstandRemarks(AccNo, Remarks);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        [System.Web.Http.Route("api/Mapi/LoadAllOutStand")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> LoadAllOutStand(int M)
        {
            var UserObj = await new SaleBL().LoadAllOutStand(M);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetLeaseCustomer")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> GetLeaseCustomer(long AccNo)
        {
            var UserObj = await new SaleBL().GetLeaseCustomer(AccNo);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        [System.Web.Http.Route("api/Mapi/GetLeaseCustomerName")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> GetLeaseCustomerName(long AccNo)
        {
            var UserObj = await new SaleBL().GetLeaseCustomerName(AccNo);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        [System.Web.Http.Route("api/Mapi/LocationList")]
        [System.Web.Http.HttpPost]
        //[MRBAC]
        public async Task<HttpResponseMessage> GetLocationList()
        {
            var UserObj = await new SetupBL().LocationList();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetAuditCRCSummary")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> GetAuditCRCSummary(DateTime FromDate, DateTime ToDate, int SAuditorId)
        {
            var UserObj = await new SaleBL().GetAuditCRCSummary(FromDate, ToDate, SAuditorId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetAuditCRC")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> GetAuditCRC(DateTime FromDate, DateTime ToDate, int LocId)
        {
            var UserObj = await new SaleBL().GetAuditCRC(FromDate, ToDate, LocId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]

        [System.Web.Http.Route("api/Mapi/GetCRCPolicyList")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetCRCPolicyList()
        {
            var UserObj = await new SaleBL().GetPolicyDetail();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }


        //[System.Web.Http.Route("api/Mapi/GetPolicyDetail")]
        //[System.Web.Http.HttpGet]
        //public async Task<HttpResponseMessage> GetPolicyDetail(int id)
        //{
        //    var UserObj = await new SaleBL().CrcFinesList(UserId);
        //    return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        //}

        [System.Web.Http.Route("api/Mapi/GetCrcFines")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetCrcFines(int UserId)
        {
            var UserObj = await new SaleBL().CrcFinesList(UserId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        [System.Web.Http.Route("api/Mapi/SaveCrcFines")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> SaveCrcFines(CrcFinesVM mod, int UserId)
        {
            var UserObj = await new SaleBL().SaveCrcFines(mod, UserId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        [System.Web.Http.Route("api/Mapi/GetEmpListCRCFine")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage GetEmpListCRCFine(long accno, int PolicyId, int locid)
        {
            var UserObj = new EmployeeBL().EmpListByPolicy(accno, PolicyId, locid);

            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetEmpByCNIC")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetEmpByCNIC(string CNIC)
        {
            var UserObj = await new EmployeeBL().GetEmployeeByCNIC(CNIC);
            var data = new { EmpId = UserObj.EmpId, EmpName = UserObj.EmpName };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetBranchLeaves")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetBranchLeaves(int UserId, int LocId, DateTime date)
        {
            var obj = await new EmployeeBL().GetBranchLeaves(UserId, LocId, date);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }
        [System.Web.Http.Route("api/Mapi/ApproveRejectLeave")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> ApproveRejectLeave(int LeaveId, bool IsApproved, int UserId, string remarks)
        {
            var obj = await new EmployeeBL().ApproveRejectLeave(LeaveId, IsApproved, UserId, remarks);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetAssignAcc")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetAssignAcc(int LocId)
        {
            var UserObj = await new SaleBL().GetAssignAcc(LocId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/LSECERAddRemove")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> LSECERAddRemove(LSECERVM mod)
        {
            var UserObj = await new SaleBL().LSECERAddRemove(mod);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        #endregion
        #region StockTaking
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/StockAtBranch")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> StockAtBranch(int LocId, int SKUId)
        {
            var UserObj = new StockBL().GetStockAtBranch(LocId, 0, SKUId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/SKUList")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> SKUList()
        {
            var UserObj = await new SaleBL().GetSKUList();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        [System.Web.Http.Route("api/Mapi/FarItemList")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> FarItemList()
        {
            var UserObj = await new ProcurementBL().ItemAllList();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/CurrentStockList")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> CurrentStockList(int LocId)
        {
            var UserObj = await new SaleBL().GetCurrentStockList(LocId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetItemBySrNo")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> GetItemBySrNo(int LocId, string SrNo)
        {
            var UserObj = await new SaleBL().GetItemBySrNo(LocId, SrNo);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/SaveSaleCart")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> SaveSaleCart(SaleCartVM mod)
        {
            var UserObj = await new SaleBL().SaveSaleCart(mod);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/StockInTransit")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> StockInTransit(int LocId, string Type)
        {
            var UserObj = await new StockBL().StockInTransit(LocId, Type);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/SaveStockVerification")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> SaveStockVerification(List<StockVerificationAVM> mod)
        {
            var UserObj = await new StockBL().SaveStockVerification(mod);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/SaveStockOpening")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> SaveStockOpening(List<InvOpeningStockMobileVM> mod)
        {
            var UserObj = await new StockBL().MobileStockOpeningCreate(mod);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        [System.Web.Http.Route("api/Mapi/FarStockOpeningCreate")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> FarStockOpeningCreate(List<FarOpeningAVM> mod)
        {
            var UserObj = await new ProcurementBL().StockOpeningCreate(mod);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        #endregion
        #region Suppliers
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/GetSuppliers")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetSuppliers()
        {
            var UserObj = await new SetupBL().SupplierList();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        [System.Web.Http.Route("api/Mapi/FarSuppliers")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> FarSuppliers()
        {
            var UserObj = await new ProcurementBL().SupplierAllLst();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        #endregion
        #region Attendance
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/FaceAttendance")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> FaceAttendance([FromBody] AttendanceVM att)
        {
            string result = "Error";
            try
            {
                if (!String.IsNullOrWhiteSpace(att.EmpPic)
                    && !String.IsNullOrWhiteSpace(att.Long)
                    && !String.IsNullOrWhiteSpace(att.Lat)
                    //&& !String.IsNullOrWhiteSpace(att.IP)
                    && att.EmpId != 0
                    && att.AttLocId != 0)
                {
                    string str = att.EmpPic;
                    str = str.Replace("data:image/jpeg;base64,", "");
                    string rand = Path.GetRandomFileName();
                    rand = rand.Replace(".", "");
                    var imgByte = Convert.FromBase64String(str);
                    string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~\\Content\\AttImg\\") + rand + ".jpg";
                    System.IO.File.WriteAllBytes(filePath, imgByte);
                    //result = await new AttendanceBL().MarkAttendFaceMobile(filePath, att.EmpId);
                    int EmpId = att.EmpId;
                    //if (Int32.TryParse(result, out EmpId))
                    //{
                    decimal lat = 0;
                    decimal lng = 0;
                    Decimal.TryParse(att.Lat, out lat);
                    Decimal.TryParse(att.Long, out lng);

                    var empl = new AttendanceBL().MarkAttendance(EmpId, att.AttLocId, att.IP, att.UserId, "M", lat, lng);
                    if (empl.AttnId > 0)
                    {
                        System.IO.File.Move(filePath, System.Web.Hosting.HostingEnvironment.MapPath("~\\Content\\AttImg\\") + empl.AttnId + "_" + empl.Status + ".jpg");
                    }
                    var emp = new
                    {
                        EmpId = empl.CardNo,
                        DeptName = empl.Department,
                        DesgName = empl.Designation,
                        EmpName = empl.Name,
                        InTime = empl.InTime.ToString("hh:mm:ss tt"),
                        OutTime = empl.OutTime.HasValue ? empl.OutTime.Value.ToString("hh:mm:ss tt") : "",
                        Status = empl.Status
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = "OK", Data = emp });

                    //}
                }
            }
            catch (Exception ex)
            {
            }
            var data = new
            {
                EmpId = "",
                DeptName = "",
                DesgName = "",
                EmpName = "",
                InTime = "",
                OutTime = "",
                Status = result
            };
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = "OK", Data = data });
        }
        #endregion
        #region AttendanceReport

        [System.Web.Http.Route("api/Mapi/DailyAttendance")]
        [System.Web.Http.HttpGet]

        public async Task<HttpResponseMessage> GetDailyAttendance(int EmpId)
        {
            var UserObj = await new AttendanceBL().GetDailyAttendance(EmpId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        [System.Web.Http.Route("api/Mapi/DailyBranchAttendance")]
        [System.Web.Http.HttpGet]

        public async Task<HttpResponseMessage> GetDailyBranchAttendance(int DeptId)
        {
            var UserObj = await new AttendanceBL().GetDailyBranchAttendance(DeptId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }


        #endregion
        #region Tax
        [System.Web.Http.Route("api/Mapi/PostPendingTax")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> PostPendingTax()
        {
            var UserObj = await new TaxBL().PostPendingFBR();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        #endregion
        #region Suppliers
        [System.Web.Http.Route("api/Mapi/GetDashboardTicket")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> GetDashboardTicket()
        {
            var UserObj = new DashboardBL().GetTableauStringAsync();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        #endregion
        #region SMS
        [System.Web.Http.Route("api/Mapi/SendAllSMS")]
        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> SendAllSMS(int LocId, string Category, string Message)
        {
            var UserObj = await new SetupBL().SendAllSMS(LocId, Category, Message, false);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/GetAllBranchesDailySale")]
        public async Task<HttpResponseMessage> GetAllBranchesDailySale()
        {
            var UserObj = new DashboardBL().GetAllBranchesDailySale(DateTime.Now.Date);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        #endregion
        #region Accounts

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/AutoVoucherPosting")]
        public async Task<HttpResponseMessage> AutoVoucherPosting()
        {
            var UserObj = await new AccountBL().AutoVoucherPosting();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        #endregion
        #region PurchaseOrder
        //[MRBAC]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/GetPOForGRN")]
        public async Task<HttpResponseMessage> GetPOForGRN(int LocId)
        {
            var UserObj = (await new PurchaseBL().GetPOForGRN(LocId)).Select(x => new { POId = x.POId, PONo = x.PONo, SuppId = x.SuppId, SuppName = x.Inv_Suppliers.SuppName }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/OrderDetailForGRN")]
        public async Task<HttpResponseMessage> OrderDetailForGRN(long POId, int LocId)
        {
            var UserObj = await new PurchaseBL().GetOrderDetailForGRN(POId, LocId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        //[MRBAC]
        [System.Web.Http.Route("api/Mapi/SavePOPocket")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> SavePOPocket(List<POPocketVM> mod)
        {
            var UserObj = await new PurchaseBL().SavePOPocket(mod);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        #endregion
        #region CRCApproval

        [System.Web.Http.Route("api/Mapi/GetCRCFines")]
        [System.Web.Http.HttpGet]

        public HttpResponseMessage GetCRCFines(int EmpId, int GroupId, int LocId)
        {
            var UserObj = new SaleBL().CrcFinesListForApproval(EmpId, GroupId, LocId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }


        [System.Web.Http.Route("api/Mapi/ApproveFine")]
        [System.Web.Http.HttpPost]

        public async Task<HttpResponseMessage> ApproveCRCFine(int UserId, int GroupId, int FineId)
        {
            var UserObj = await new SaleBL().ApproveCRCFine(GroupId, UserId, FineId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        [System.Web.Http.Route("api/Mapi/RejectFine")]
        [System.Web.Http.HttpPost]

        public async Task<HttpResponseMessage> RejectCRCFine(int UserId, int GroupId, int FineId)
        {
            var UserObj = await new SaleBL().RejectCRCFine(UserId, FineId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        #endregion
        #region GeneralFunctions
        [System.Web.Http.Route("api/Mapi/ExceptionLog")]
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> ExceptionLog(string msg)
        {
            var UserObj = await new SaleBL().WriteLog(msg, "");
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }


        //[MRBAC]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/CrmCategoryList")]
        public async Task<HttpResponseMessage> CrmCategoryList()
        {
            var UserObj = await new SetupBL().CrmCategoryList();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/CrmTicketsList")]
        public async Task<HttpResponseMessage> CrmTicketsList(int LocId)
        {
            var UserObj = await new CRMBL().GetTicketsMobile(LocId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/CrmAllTicketsList")]
        public async Task<HttpResponseMessage> CrmAllTicketsList(int CityId, int LocId, int CategoryId, DateTime fromdate, DateTime todate, string Status)
        {
            var UserObj = await new CRMBL().GetAllTicketsMoblie(CityId, LocId, CategoryId, fromdate, todate, Status);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }



        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/CloseTicket")]
        public async Task<HttpResponseMessage> CloseTicket(int TicketId, string Response, string Status, int UserId)
        {
            var UserObj = await new CRMBL().CloseTicketMobile(TicketId, Response, Status, UserId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        //[MRBAC]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Mapi/AddTickets")]
        public async Task<HttpResponseMessage> AddTickets(Crm_TicketVM mod)
        {
            //mod.WorkingDate = new SetupBL().GetWorkingDate(UserInfo.LocId);
            var TicketId = await new CRMBL().AddTicketsMobile(mod, mod.UserId);
            if (TicketId > 0)
            {
                mod.TicketId = TicketId;
                if (mod.UploadedFiles != "")
                {
                    await new DocumentBL().UploadTicketDoc(mod);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, mod);
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/SendCashRiskAlert")]
        public async Task<HttpResponseMessage> SendCashRiskAlert()
        {
            var UserObj = await new CashBL().CashRiskAlert();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/SendWishEmail")]
        public async Task<HttpResponseMessage> SendWishEmail()
        {
            var UserObj = await new EmployeeBL().SendWishEmail();
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/UserNotifications")]
        public HttpResponseMessage UserNotifications(int UserId)
        {
            var UserObj = new NotificationBL().GetUserNotifications(UserId);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Mapi/SeenMessage")]
        public async Task<HttpResponseMessage> SeenMessage(long Id)
        {
            var UserObj = await new NotificationBL().SeenMessage(Id);
            return Request.CreateResponse(HttpStatusCode.OK, UserObj);
        }

        #endregion

    }





}

