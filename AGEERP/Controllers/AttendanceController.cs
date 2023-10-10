using AGEERP.Models;
using DPUruNet;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AGEERP.Controllers
{
    [RBAC]
    public class AttendanceController : Controller
    {

        AttendanceBL _AttenBL = new AttendanceBL();

        [HttpGet]
        public async Task<ActionResult> Enrollment()
        {
            //ViewBag.EmpLst = await employeeBL.EmployeeList();
            return View();
        }

        /// <summary>
        /// Send the Base64 string from the client side processing will be done at the client side it will only convet base64 to bytes and then bytes to the FMD
        /// </summary>
        /// <param name="EmployeeEnrolledStr"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Enrollment(int EmpId, List<string> EmployeeEnrolledStr)
        {
            try
            {
                List<Fmd> preenrollmentFmds = new List<Fmd>();
                foreach (var itme in EmployeeEnrolledStr)
                {
                    Bitmap bmp = _AttenBL.Base64StringToBitmap(itme);
                    Fmd _EmpTemplate = _AttenBL.ExtractFmdfromBmp(bmp);
                    if (_EmpTemplate != null)
                    {
                        preenrollmentFmds.Add(_EmpTemplate);
                    }
                    else
                    {
                        return Json("Invalid Template", JsonRequestBehavior.AllowGet);
                    }
                }
                DataResult<Fmd> _Template = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.DP_REGISTRATION, preenrollmentFmds);
                preenrollmentFmds.Clear();
                if (_Template.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    string templ = Fmd.SerializeXml(_Template.Data);
                    var result = await _AttenBL.SaveTemplate(EmpId, templ, UserInfo.UserId);
                    return Json("Enroll Successfully", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(_Template.ResultCode.ToString(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Server Error", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> EnrollCustomer(long AccNo, List<string> CustEnrolledStr)
        {
            try
            {
                List<Fmd> preenrollmentFmds = new List<Fmd>();
                foreach (var itme in CustEnrolledStr)
                {
                    Bitmap bmp = _AttenBL.Base64StringToBitmap(itme);
                    Fmd _EmpTemplate = _AttenBL.ExtractFmdfromBmp(bmp);
                    if (_EmpTemplate != null)
                    {
                        preenrollmentFmds.Add(_EmpTemplate);
                    }
                    else
                    {
                        return Json("Invalid Template", JsonRequestBehavior.AllowGet);
                    }
                }
                DataResult<Fmd> _Template = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.DP_REGISTRATION, preenrollmentFmds);
                preenrollmentFmds.Clear();
                if (_Template.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    string templ = Fmd.SerializeXml(_Template.Data);
                    var result = await (new SaleBL()).SaveCustTemplate(AccNo, templ, UserInfo.UserId);
                    new SaleBL().ConvertURItoImage(CustEnrolledStr[0], "thumb_" + AccNo.ToString());
                    return Json("Enroll Successfully", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(_Template.ResultCode.ToString(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Server Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Verification()
        {
            return View();
        }

        /// <summary>
        /// Will recieve the base64 string template of the employee and it will be verified from the database
        /// </summary>
        /// <param name="empid"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Verification(int EmpId, string EmployeeEnrolledStr)
        {
            try
            {
                Bitmap img = _AttenBL.Base64StringToBitmap(EmployeeEnrolledStr);
                Fmd _EmpTemplate = null;
                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                byte[] imageByte = ms.ToArray();
                DataResult<Fmd> fmd = DPUruNet.FeatureExtraction.CreateFmdFromRaw(imageByte, 0, 1, img.Width, img.Height, 700, Constants.Formats.Fmd.DP_VERIFICATION);
                if (fmd.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    _EmpTemplate = fmd.Data;
                }
                else
                {
                    return Json(fmd.ResultCode.ToString(), JsonRequestBehavior.AllowGet);
                }
                var dbTemplate = await _AttenBL.GetEmpTemplate(EmpId);
                if (dbTemplate == null)
                {
                    return Json("Not Enrolled", JsonRequestBehavior.AllowGet);
                }
                var _dbFmd = Fmd.DeserializeXml(dbTemplate);
                CompareResult compareResult = Comparison.Compare(_EmpTemplate, 0, _dbFmd, 0);
                if (compareResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    if (compareResult.Score < 2147)
                        return Json("Verified Successfully", JsonRequestBehavior.AllowGet);
                    else
                        return Json("Not Verified", JsonRequestBehavior.AllowGet);
                    //var req =   compareResult.Score.ToString();

                }
                else
                {
                    return Json(compareResult.ResultCode.ToString(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Server Error", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Attendance()
        {
            return View();
        }


        /// <summary>
        /// Will recieve the base64 string template of the employee and it will be verified from the database
        /// </summary>
        /// <param name="empid"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Attendance(string str)
        {
            try
            {
                Bitmap img = _AttenBL.Base64StringToBitmap(str);
                Fmd _EmpTemplate = null;
                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                byte[] imageByte = ms.ToArray();
                DataResult<Fmd> fmd = DPUruNet.FeatureExtraction.CreateFmdFromRaw(imageByte, 0, 1, img.Width, img.Height, 700, Constants.Formats.Fmd.DP_VERIFICATION);
                if (fmd.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    _EmpTemplate = fmd.Data;
                }
                else
                {
                    return Json(fmd.ResultCode.ToString(), JsonRequestBehavior.AllowGet);
                }

                var _dbFmd = await _AttenBL.GetTemplateByDept(UserInfo.LocId);
                var dbFmd = _dbFmd.Select(x => Fmd.DeserializeXml(x.Template));
                IdentifyResult compareResult = Comparison.Identify(_EmpTemplate, 0, dbFmd, 2147, 1);
                if (compareResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    if (compareResult.Indexes.Length > 0)
                    {
                        var ind = compareResult.Indexes[0].FirstOrDefault();
                        var wish = await _AttenBL.IsBirthday(_dbFmd[ind].EmpId);
                        var empl = _AttenBL.MarkAttendance(_dbFmd[ind].EmpId, UserInfo.LocId, IP(), UserInfo.UserId, "B");
                        var emp = new
                        {
                            EmpId = empl.CardNo,
                            DeptName = empl.Department,
                            DesgName = empl.Designation,
                            EmpName = empl.Name,
                            InTime = empl.InTime.ToString("hh:mm:ss tt"),
                            OutTime = empl.OutTime.HasValue ? empl.OutTime.Value.ToString("hh:mm:ss tt") : "",
                            Status = empl.Status,
                            IsBirthday = wish.IsBirthday,
                            IsWorkAnniversary = wish.IsWorkAnniversary
                        };
                        return Json(new { Status = "OK", Data = emp }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Status = "Not Found" }, JsonRequestBehavior.AllowGet);
                }
                return Json(compareResult.ResultCode.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }
        public string IP()
        {
            try
            {
                string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = Request.UserHostAddress;
                }
                return ip;
            }
            catch (Exception)
            {
                return "";
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////
        public ActionResult AttendanceFace()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AttendanceFace(string str)
        {
            string result = "Error";
            try
            {
                str = str.Replace("data:image/jpeg;base64,", "");
                string rand = Path.GetRandomFileName();
                rand = rand.Replace(".", "");
                var imgByte = Convert.FromBase64String(str);
                string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~\\Content\\AttImg\\") + rand + ".jpg";

                System.IO.File.WriteAllBytes(filePath, imgByte);
                result = await _AttenBL.MarkAttendFace(filePath, UserInfo.LocId);
                int EmpId = 0;
                if (Int32.TryParse(result, out EmpId))
                {
                    var empl = _AttenBL.MarkAttendance(EmpId, UserInfo.LocId, IP(), UserInfo.UserId, "F");
                    var wish = new BirthdayVM();
                    if (empl.AttnId > 0)
                    {
                        wish = await _AttenBL.IsBirthday(EmpId);
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
                        Status = empl.Status,
                        IsBirthday = wish.IsBirthday,
                        IsWorkAnniversary = wish.IsWorkAnniversary
                    };
                    return Json(new { Status = "OK", Data = emp }, JsonRequestBehavior.AllowGet);
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
            return Json(new { Status = "Not Found", Data = data }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EnrollFace()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> EnrollFace(string str, int EmpId)
        {
            var result = await _AttenBL.EnrollFace(str, EmpId, UserInfo.UserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AttendanceFaceVerification()
        {
            return View();
        }
        public ActionResult AttendanceFaceVerification_Read([DataSourceRequest] DataSourceRequest request, DateTime FromDate, DateTime ToDate, int LocId)
        {
            var lst = _AttenBL.faceAttnList(FromDate, ToDate, LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}