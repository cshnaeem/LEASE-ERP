using AGEERP.Models;
using DPUruNet;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AGEERP.Controllers
{
    [RBAC]
    public class SalaryController : Controller
    {

        AttendanceBL _AttenBL = new AttendanceBL();


        // GET: Salary
        public ActionResult SalaryDisbursment()
        {
            return View();
        }

        public async Task<JsonResult> GetSalaryDisbursementTypes()
        {
            var Types = await _AttenBL.GetSalaryDisbursementTypes(true);
            return Json(Types, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> MSalaryDisbursment(string cnic, DateTime month, int DistTypeId, int PeriodId)
        {
            try
            {
                var emp = await new EmployeeBL().GetEmployeeByCNIC(cnic);
                if (emp != null)
                {
                    var empid = Convert.ToInt32(emp.EmpId);
                    EmpSalaryDataVM salary = new EmpSalaryDataVM();
                    if (empid > 0)
                    {
                        salary = await _AttenBL.GetEmpSalary(empid, month, DistTypeId, UserInfo.UserId, "M", PeriodId);
                    }
                    return Json(new { Status = "OK", Data = salary }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Status = "Not Found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                new AttendanceBL().AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                {
                    EmpId = 0,
                    Exception = ex.Message.ToString() + "Stack Trace : " + ex.StackTrace.ToString(),
                    SalaryMonth = month,
                    TransDate = DateTime.Now,
                    UserId = UserInfo.UserId,
                    DisbursementSource = "M",
                    DisbursementTypeId = DistTypeId
                });
                return Json(new { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> SalaryDisbursment(string str, DateTime month, int DistTypeId, int PeriodId)
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
                var _dbFmd = await _AttenBL.GetTemplateEmpTemplates(month, UserInfo.LocId);
                var dbFmd = _dbFmd.Select(x => Fmd.DeserializeXml(x.Template));
                IdentifyResult compareResult = Comparison.Identify(_EmpTemplate, 0, dbFmd, 2147, 1);
                if (compareResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    if (compareResult.Indexes.Length > 0)
                    {
                        var ind = compareResult.Indexes[0].FirstOrDefault();
                        var empid = _dbFmd[ind].EmpId;
                        EmpSalaryDataVM salary = new EmpSalaryDataVM();
                        if (empid > 0)
                        {
                            salary = await _AttenBL.GetEmpSalary(empid, month, DistTypeId, UserInfo.UserId, "B", PeriodId);
                        }
                        return Json(new { Status = "OK", Data = salary }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Status = "Not Found" }, JsonRequestBehavior.AllowGet);
                }
                return Json(compareResult.ResultCode.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                new AttendanceBL().AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                {
                    EmpId = 0,
                    Exception = ex.Message.ToString() + "Stack Trace : " + ex.StackTrace.ToString(),
                    SalaryMonth = month,
                    TransDate = DateTime.Now,
                    UserId = UserInfo.UserId,
                    DisbursementSource = "B",
                    DisbursementTypeId = DistTypeId
                });
                return Json(new { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> PayEmpSalary(int EmpId, DateTime month, int DistId, string source, int PeriodId)
        {
            var PaidSal = await _AttenBL.PayEmpSalary(EmpId, month, UserInfo.UserId, UserInfo.LocId, DistId, source, PeriodId);
            if (PaidSal.Status == true)
            {
                return Json(new { Status = "OK", Message = PaidSal.Msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = "Error", Message = "" }, JsonRequestBehavior.AllowGet);
            }
        }


       
    }
}