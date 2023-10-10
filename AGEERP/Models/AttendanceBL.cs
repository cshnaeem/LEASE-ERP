using DPUruNet;
using Luxand;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AGEERP.Models
{
    public struct TFaceRecord
    {
        public byte[] Template; //Face Template;
        public FSDK.TFacePosition FacePosition;
        public FSDK.TPoint[] FacialFeatures; //Facial Features;

        public string ImageFileName;

        public FSDK.CImage image;
        public FSDK.CImage faceImage;
    }
    public class AttendanceBL
    {
        AGEEntities db = new AGEEntities();

        #region Digital Persona Conversion 
        //public byte[] ExtractByteArray(Bitmap img)
        //{
        //    byte[] rawData = null;
        //    byte[] bitData = null;
        //    //ToDo: CreateFmdFromRaw only works on 8bpp bytearrays. As such if we have an image with 24bpp then average every 3 values in Bitmapdata and assign it to bitdata
        //    if (img.PixelFormat == PixelFormat.Format8bppIndexed)
        //    {
        //        //Lock the bitmap's bits
        //        BitmapData bitmapdata = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, img.PixelFormat);
        //        //Declare an array to hold the bytes of bitmap
        //        byte[] imgData = new byte[bitmapdata.Stride * bitmapdata.Height]; //stride=360, height 392
        //        //Copy bitmapdata into array
        //        Marshal.Copy(bitmapdata.Scan0, imgData, 0, imgData.Length);//imgData.length =141120
        //        bitData = new byte[bitmapdata.Width * bitmapdata.Height];//ditmapdata.width =357, height = 392
        //        for (int y = 0; y < bitmapdata.Height; y++)
        //        {
        //            for (int x = 0; x < bitmapdata.Width; x++)
        //            {
        //                bitData[bitmapdata.Width * y + x] = imgData[y * bitmapdata.Stride + x];
        //            }
        //        }
        //        rawData = new byte[bitData.Length];
        //        for (int i = 0; i < bitData.Length; i++)
        //        {
        //            int avg = (img.Palette.Entries[bitData[i]].R + img.Palette.Entries[bitData[i]].G + img.Palette.Entries[bitData[i]].B) / 3;
        //            rawData[i] = (byte)avg;
        //        }
        //    }
        //    else
        //    {
        //        bitData = new byte[img.Width * img.Height];//ditmapdata.width =357, height = 392, bitdata.length=139944
        //        for (int y = 0; y < img.Height; y++)
        //        {
        //            for (int x = 0; x < img.Width; x++)
        //            {
        //                Color pixel = img.GetPixel(x, y);
        //                bitData[img.Width * y + x] = (byte)((Convert.ToInt32(pixel.R) + Convert.ToInt32(pixel.G) + Convert.ToInt32(pixel.B)) / 3);
        //            }
        //        }
        //    }
        //    return bitData;
        //}
        public Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);
            memoryStream.Position = 0;
            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);
            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;
            return bmpReturn;
        }
        public Fmd ExtractFmdfromBmp(Bitmap img)
        {
            //byte[] imageByte =  //ExtractByteArray(img);
            MemoryStream ms = new MemoryStream();
            img.Save(ms, ImageFormat.Bmp);
            byte[] imageByte = ms.ToArray();
            DataResult<Fmd> fmd = DPUruNet.FeatureExtraction.CreateFmdFromRaw(imageByte, 0, 1, img.Width, img.Height, 700, Constants.Formats.Fmd.DP_PRE_REGISTRATION);
            if (fmd.ResultCode == Constants.ResultCode.DP_SUCCESS)
            {
                return fmd.Data;
            }
            else
            {
                return null;
            }
        }

        #endregion

        public async Task<bool> SaveTemplate(int EmpId, string template, int UserId)
        {
            try
            {
                Pay_EmpTemplate tbl = await db.Pay_EmpTemplate.FirstOrDefaultAsync(x => x.EmpId == EmpId);
                if (tbl == null)
                {
                    tbl = new Pay_EmpTemplate();
                    tbl.EmpId = EmpId;
                    tbl.Status = true;
                    tbl.Template = template;
                    tbl.TransDate = DateTime.Now;
                    tbl.UserId = UserId;
                    db.Pay_EmpTemplate.Add(tbl);
                }
                else
                {
                    tbl.Status = true;
                    tbl.Template = template;
                    tbl.TransDate = DateTime.Now;
                    tbl.UserId = UserId;
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<string> GetEmpTemplate(int EmpId)
        {
            try
            {
                return await db.Pay_EmpTemplate.Where(x => x.EmpId == EmpId).Select(x => x.Template).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<EmpTemplateVM>> GetTemplateByDept(int LocId)
        {
            try
            {
                var empLst = db.spget_Pay_EmpTemplateByLoc(LocId).ToList();
                return await db.Pay_EmpTemplate.Where(x => empLst.Contains(x.EmpId)).Select(T => new EmpTemplateVM
                {
                    EmpId = T.EmpId,
                    Template = T.Template
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<EmpTemplateVM>> GetTemplateEmpTemplates(DateTime SalMonth, int LocId)
        {
            try
            {
                var empLst = db.spGet_TemplateForSalary(SalMonth, LocId);
                return await db.Pay_EmpTemplate.Where(x => empLst.Contains(x.EmpId)).Select(T => new EmpTemplateVM
                {
                    EmpId = T.EmpId,
                    Template = T.Template
                }).ToListAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<GetDailyAttendanceVM>> GetDailyAttendance(int EmpId)
        {
            return db.spRep_Pay_DailyAttendanceMobile(EmpId).
            Select(A => new GetDailyAttendanceVM
            {
                DeptName = A.DeptName,
                DesgName = A.DesgName,
                EmpId = A.EmpId,
                EmpName = A.EmpName,
                Status = A.Status
            }).ToList();
        }

        public List<GetDailyAttendanceVM> GetDailyBranchAttendance(int DeptId)
        {
            return db.spRep_Pay_DailyBranchAttendanceMobile(DeptId).
            Select(A => new GetDailyAttendanceVM
            {
                DeptName = A.DeptName,
                DesgName = A.DesgName,
                EmpId = A.EmpId,
                EmpName = A.EmpName,
                Status = A.Status
            }).ToList();
        }
        public spget_MarkAttendance_Result MarkAttendance(int EmpId, int LocId, string IP, int UserId, string AttType, decimal? Lat = null, decimal? Lng = null)
        {
            try
            {
                return db.spget_MarkAttendance(EmpId, LocId, UserId, IP, AttType, Lat, Lng).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<BirthdayVM> IsBirthday(int EmpId)
        {
            try
            {
                BirthdayVM mod = new BirthdayVM();
                var dt = DateTime.Now.Date;
                var emp = await db.Pay_EmpMaster.Where(x => x.EmpId == EmpId).FirstOrDefaultAsync();
                if (emp.DOB.HasValue)
                {
                    if (emp.DOB.Value.Day == dt.Day && emp.DOB.Value.Month == dt.Month)
                    {
                        mod.IsBirthday = true;
                    }
                }
                if (emp.DOJ.HasValue)
                {
                    if (emp.DOJ.Value.Day == dt.Day && emp.DOJ.Value.Month == dt.Month && emp.DOJ.Value != dt)
                    {
                        mod.IsWorkAnniversary = true;
                    }
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        ///////////////////////////////////////FACE////////////////////////////////////////////////////
        public float FaceDetectionThreshold = 5;
        public float FARValue = 1f;
        //public List<TFaceRecord> FaceList;

        public async Task<string> MarkAttendFace(string filePath, int LocId)
        {
            try
            {
               
                var EmpLst = db.spget_Pay_EmpTemplateByLocFace(LocId).ToList();
                var lst = await db.Pay_EmpTemplateFace.Where(x => EmpLst.Contains(x.EmpId)).ToListAsync();
                List<TFaceRecord> FaceList = new List<TFaceRecord>();
                foreach (var v in lst)
                {
                    if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\EmpImg\") + "img_" + v.EmpId.ToString() + ".jpg"))
                    {
                        TFaceRecord fr = new TFaceRecord();
                        fr.ImageFileName = v.EmpId.ToString();

                        fr.FacePosition = new FSDK.TFacePosition();
                        fr.FacePosition.xc = v.FacePositionXc;
                        fr.FacePosition.yc = v.FacePositionYc;
                        fr.FacePosition.w = v.FacePositionW;
                        fr.FacePosition.angle = v.FacePositionAngle;

                        fr.FacialFeatures = new FSDK.TPoint[2];
                        fr.FacialFeatures[0] = new FSDK.TPoint();
                        fr.FacialFeatures[0].x = v.Eye1X;
                        fr.FacialFeatures[0].y = v.Eye1Y;
                        fr.FacialFeatures[1] = new FSDK.TPoint();
                        fr.FacialFeatures[1].x = v.Eye2X;
                        fr.FacialFeatures[1].y = v.Eye2Y;

                        fr.Template = v.Template;

                        Image img = Image.FromFile(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\EmpImg\") + "img_" + v.EmpId.ToString() + ".jpg");
                        Image img_face = Image.FromFile(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\EmpImg\") + "img_face_" + v.EmpId.ToString() + ".jpg");
                        fr.image = new FSDK.CImage(img);
                        fr.faceImage = new FSDK.CImage(img_face);
                        FaceList.Add(fr);

                        img.Dispose();
                        img_face.Dispose();
                    }
                }

                try
                {
                    string fn = filePath;
                    TFaceRecord fr = new TFaceRecord();
                    fr.ImageFileName = fn;
                    fr.FacePosition = new FSDK.TFacePosition();
                    fr.FacialFeatures = new FSDK.TPoint[FSDK.FSDK_FACIAL_FEATURE_COUNT];
                    fr.Template = new byte[FSDK.TemplateSize];

                    fr.image = new FSDK.CImage(fn);

                    fr.FacePosition = fr.image.DetectFace();
                    if (0 == fr.FacePosition.w)
                        return "No faces found";
                    else
                    {
                        fr.faceImage = fr.image.CopyRect((int)(fr.FacePosition.xc - Math.Round(fr.FacePosition.w * 0.5)), (int)(fr.FacePosition.yc - Math.Round(fr.FacePosition.w * 0.5)), (int)(fr.FacePosition.xc + Math.Round(fr.FacePosition.w * 0.5)), (int)(fr.FacePosition.yc + Math.Round(fr.FacePosition.w * 0.5)));
                        fr.FacialFeatures = fr.image.DetectEyesInRegion(ref fr.FacePosition);
                        fr.Template = fr.image.GetFaceTemplateInRegion(ref fr.FacePosition);
                        var SearchFace = fr;
                        Image img = SearchFace.faceImage.ToCLRImage();


                        float Threshold = 0.0f;
                        FSDK.GetMatchingThresholdAtFAR(FARValue / 100, ref Threshold);

                        int MatchedCount = 0;
                        int FaceCount = FaceList.Count;
                        float[] Similarities = new float[FaceCount];
                        int[] Numbers = new int[FaceCount];
                        string multiFace = "";
                        for (int i = 0; i < FaceList.Count; i++)
                        {
                            float Similarity = 0.0f;
                            TFaceRecord CurrentFace = FaceList[i];
                            FSDK.MatchFaces(ref SearchFace.Template, ref CurrentFace.Template, ref Similarity);
                            if (Similarity >= Threshold)
                            {
                                Similarities[MatchedCount] = Similarity;
                                Numbers[MatchedCount] = i;
                                ++MatchedCount;
                                multiFace += FaceList[i].ImageFileName + ",";
                                //break;
                            }
                        }

                        if (MatchedCount == 0)
                            return "No matches found";
                        else if (MatchedCount > 1)
                            return "Multiple matches found " + multiFace;
                        else
                        {
                            floatReverseComparer cmp = new floatReverseComparer();
                            Array.Sort(Similarities, Numbers, 0, MatchedCount, (IComparer<float>)cmp);


                            int i = 0;
                            var result = FaceList[Numbers[i]].ImageFileName;
                         
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return "Error 3";

                }
            }

            catch (Exception ex)
            {

                return "Error 4";
            }
        }

        public async Task<string> MarkAttendFaceMobile(string filePath, int EmpId)
        {
            try
            {
                var lst = await db.Pay_EmpTemplateFace.Where(x => x.EmpId == EmpId).ToListAsync();
                List<TFaceRecord> FaceList = new List<TFaceRecord>();
                foreach (var v in lst)
                {
                    TFaceRecord fr = new TFaceRecord();
                    fr.ImageFileName = v.EmpId.ToString();

                    fr.FacePosition = new FSDK.TFacePosition();
                    fr.FacePosition.xc = v.FacePositionXc;
                    fr.FacePosition.yc = v.FacePositionYc;
                    fr.FacePosition.w = v.FacePositionW;
                    fr.FacePosition.angle = v.FacePositionAngle;

                    fr.FacialFeatures = new FSDK.TPoint[2];
                    fr.FacialFeatures[0] = new FSDK.TPoint();
                    fr.FacialFeatures[0].x = v.Eye1X;
                    fr.FacialFeatures[0].y = v.Eye1Y;
                    fr.FacialFeatures[1] = new FSDK.TPoint();
                    fr.FacialFeatures[1].x = v.Eye2X;
                    fr.FacialFeatures[1].y = v.Eye2Y;

                    fr.Template = v.Template;
                    Image img = Image.FromFile(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\EmpImg\") + "img_" + v.EmpId.ToString() + ".jpg");
                    Image img_face = Image.FromFile(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\EmpImg\") + "img_face_" + v.EmpId.ToString() + ".jpg");
                    fr.image = new FSDK.CImage(img);
                    fr.faceImage = new FSDK.CImage(img_face);
                    FaceList.Add(fr);

                    img.Dispose();
                    img_face.Dispose();
                }

                try
                {
                    string fn = filePath;
                    TFaceRecord fr = new TFaceRecord();
                    fr.ImageFileName = fn;
                    fr.FacePosition = new FSDK.TFacePosition();
                    fr.FacialFeatures = new FSDK.TPoint[FSDK.FSDK_FACIAL_FEATURE_COUNT];
                    fr.Template = new byte[FSDK.TemplateSize];

                    fr.image = new FSDK.CImage(fn);

                    fr.FacePosition = fr.image.DetectFace();
                    if (0 == fr.FacePosition.w)
                        return "No faces found";
                    else
                    {
                        fr.faceImage = fr.image.CopyRect((int)(fr.FacePosition.xc - Math.Round(fr.FacePosition.w * 0.5)), (int)(fr.FacePosition.yc - Math.Round(fr.FacePosition.w * 0.5)), (int)(fr.FacePosition.xc + Math.Round(fr.FacePosition.w * 0.5)), (int)(fr.FacePosition.yc + Math.Round(fr.FacePosition.w * 0.5)));
                        fr.FacialFeatures = fr.image.DetectEyesInRegion(ref fr.FacePosition);
                        fr.Template = fr.image.GetFaceTemplateInRegion(ref fr.FacePosition);
                        var SearchFace = fr;
                        Image img = SearchFace.faceImage.ToCLRImage();


                        float Threshold = 0.0f;
                        FSDK.GetMatchingThresholdAtFAR(FARValue / 100, ref Threshold);

                        int MatchedCount = 0;
                        int FaceCount = FaceList.Count;
                        float[] Similarities = new float[FaceCount];
                        int[] Numbers = new int[FaceCount];

                        for (int i = 0; i < FaceList.Count; i++)
                        {
                            float Similarity = 0.0f;
                            TFaceRecord CurrentFace = FaceList[i];
                            FSDK.MatchFaces(ref SearchFace.Template, ref CurrentFace.Template, ref Similarity);
                            if (Similarity >= 0.97)
                            {
                                Similarities[MatchedCount] = Similarity;
                                Numbers[MatchedCount] = i;
                                ++MatchedCount;
                                //break;
                            }
                        }

                        if (MatchedCount == 0 || MatchedCount > 1)
                            return "No matches found";
                        else
                        {
                            floatReverseComparer cmp = new floatReverseComparer();
                            Array.Sort(Similarities, Numbers, 0, MatchedCount, (IComparer<float>)cmp);


                            int i = 0;
                            var result = FaceList[Numbers[i]].ImageFileName;

                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return "Error 3";

                }
            }

            catch (Exception ex)
            {

                return "Error 4";
            }
        }

        public async Task<string> EnrollFace(string str, int EmpId, int UserId)
        {
            try
            {
                str = str.Replace("data:image/jpeg;base64,", "");
                //string rand = Path.GetRandomFileName();
                //rand = rand.Replace(".", "");
                var imgByte = Convert.FromBase64String(str);
                string fn = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\EmpImg\") + EmpId + ".jpg";
                System.IO.File.WriteAllBytes(fn, imgByte);
                //if (FSDK.FSDKE_OK != FSDK.ActivateLibrary("fVrFCzYC5wOtEVspKM/zfLWVcSIZA4RNqx74s+QngdvRiCC7z7MHlSf2w3+OUyAZkTFeD4kSpfVPcRVIqAKWUZzJG975b/P4HNNzpl11edXGIyGrTO/DImoZksDSRs6wktvgr8lnNCB5IukIPV5j/jBKlgL5aqiwSfyCR8UdC9s="))
                //{
                //    return "Error 1";
                //}
                //FSDK.InitializeLibrary();
                //if (FSDK.InitializeLibrary() != FSDK.FSDKE_OK)
                //    return "Error 2";
                TFaceRecord fr = new TFaceRecord();
                fr.ImageFileName = fn;
                fr.FacePosition = new FSDK.TFacePosition();
                fr.FacialFeatures = new FSDK.TPoint[2];
                fr.Template = new byte[FSDK.TemplateSize];

                fr.image = new FSDK.CImage(fn);

                fr.FacePosition = fr.image.DetectFace();
                if (0 == fr.FacePosition.w)
                    return ("No faces found");
                else
                {
                    fr.faceImage = fr.image.CopyRect((int)(fr.FacePosition.xc - Math.Round(fr.FacePosition.w * 0.5)), (int)(fr.FacePosition.yc - Math.Round(fr.FacePosition.w * 0.5)), (int)(fr.FacePosition.xc + Math.Round(fr.FacePosition.w * 0.5)), (int)(fr.FacePosition.yc + Math.Round(fr.FacePosition.w * 0.5)));
                    fr.FacialFeatures = fr.image.DetectEyesInRegion(ref fr.FacePosition);
                    fr.Template = fr.image.GetFaceTemplateInRegion(ref fr.FacePosition); // get template with higher precision

                    return await SaveFaceInDB(fr, EmpId, UserId);
                }
            }
            catch (Exception)
            {
                return "Error 3";
            }
        }
        private async Task<string> SaveFaceInDB(TFaceRecord fr, int EmpId, int UserId)
        {
            try
            {
                Image img = fr.image.ToCLRImage();
                Image img_face = fr.faceImage.ToCLRImage();
                img.Save(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\EmpImg\") + "img_" + EmpId + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                img_face.Save(System.Web.Hosting.HostingEnvironment.MapPath(@"~\Content\EmpImg\") + "img_face_" + EmpId + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                var tbl = await db.Pay_EmpTemplateFace.FirstOrDefaultAsync(x => x.EmpId == EmpId);
                if (tbl == null)
                {
                    tbl = new Pay_EmpTemplateFace();
                    tbl.EmpId = EmpId;
                    tbl.FacePositionXc = fr.FacePosition.xc;
                    tbl.FacePositionYc = fr.FacePosition.yc;
                    tbl.FacePositionW = fr.FacePosition.w;
                    tbl.FacePositionAngle = (float)fr.FacePosition.angle;
                    tbl.Eye1X = fr.FacialFeatures[0].x;
                    tbl.Eye1Y = fr.FacialFeatures[0].y;
                    tbl.Eye2X = fr.FacialFeatures[1].x;
                    tbl.Eye2Y = fr.FacialFeatures[1].y;
                    tbl.Template = fr.Template;
                    tbl.TransDate = DateTime.Now;
                    tbl.UserId = UserId;
                    tbl.Status = true;
                    db.Pay_EmpTemplateFace.Add(tbl);
                }
                else
                {
                    tbl.FacePositionXc = fr.FacePosition.xc;
                    tbl.FacePositionYc = fr.FacePosition.yc;
                    tbl.FacePositionW = fr.FacePosition.w;
                    tbl.FacePositionAngle = (float)fr.FacePosition.angle;
                    tbl.Eye1X = fr.FacialFeatures[0].x;
                    tbl.Eye1Y = fr.FacialFeatures[0].y;
                    tbl.Eye2X = fr.FacialFeatures[1].x;
                    tbl.Eye2Y = fr.FacialFeatures[1].y;
                    tbl.Template = fr.Template;
                    tbl.TransDate = DateTime.Now;
                    tbl.UserId = UserId;
                    tbl.Status = true;
                }
                await db.SaveChangesAsync();

                img.Dispose();
                img_face.Dispose();
                return "Enroll Successfully";
            }
            catch (Exception ex)
            {
                return "Error 4";
            }
        }


        #region Salary

        public async Task<List<SalaryDisbursementTypeVM>> GetSalaryDisbursementTypes(bool Status)
        {
            return await db.Pay_SalaryDisbursementTypes.Where(x => x.IsActive == Status).Select(x => new SalaryDisbursementTypeVM()
            {
                DisbursementType = x.DisbursementType,
                DisbursementTypeId = x.DisbursementTypeId
            }).ToListAsync();
        }

        public bool AddSalaryDisbursementLog(SalaryDisbursementLogVM mod)
        {
            try
            {
                Pay_SalaryDisbursementLog SalMod = new Pay_SalaryDisbursementLog();
                SalMod.EmpId = mod.EmpId;
                SalMod.Exception = mod.Exception;
                SalMod.SalaryMonth = mod.SalaryMonth;
                SalMod.TransDate = DateTime.Now;
                SalMod.UserId = mod.UserId;
                SalMod.DisbursementTypeId = mod.DisbursementTypeId;
                SalMod.DisbursementSource = mod.DisbursementSource;
                db.Pay_SalaryDisbursementLog.Add(SalMod);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<EmpSalaryDataVM> GetEmpSalary(int EmpId, DateTime month, int DistId, int UserId, string source, int PeriodId)
        {
            try
            {
                var emp = await new EmployeeBL().GetEmployeeById(EmpId);
                var hdept = await new EmployeeBL().HDepartmentList();
                EmpSalaryDataVM dat;
                var Mon = month.Month;
                var Year = month.Year;
                
                Pay_SalaryDisbursement AlreadyPaid = new Pay_SalaryDisbursement();
                if(DistId == 8)
                {
                    
                    var perd = await db.Pay_ProductIncCalendar.Where(x => x.RowId == PeriodId).FirstOrDefaultAsync();
                    var dst = perd.ToDate.Date;
                    AlreadyPaid = db.Pay_SalaryDisbursement.Where(x => x.SalaryMonth == dst && x.SalaryMonth.Month == Mon && x.SalaryMonth.Year == Year && x.EmpId == EmpId && x.DisbursementTypeId == 8).FirstOrDefault();
                }
                else
                {
                    AlreadyPaid = db.Pay_SalaryDisbursement.Where(x => x.SalaryMonth.Month == Mon && x.SalaryMonth.Year == Year && x.EmpId == EmpId && (x.DisbursementTypeId == DistId || DistId == 8)).FirstOrDefault();
                }
                 
                if (AlreadyPaid == null)
                {
                    if (emp.ApprovedBy == null)
                    {
                        dat = new EmpSalaryDataVM()
                        {
                            CNIC = "",
                            Department = "",
                            Designation = "",
                            EmpName = "",
                            EmpId = 0,
                            EmpSal = null,
                            Title = "Error",
                            Msg = "Employee Not Approved",
                            Location = ""
                        };
                        return dat;
                    }
                    if (source == "M" && emp.MSalary == false)
                    {
                        dat = new EmpSalaryDataVM()
                        {
                            CNIC = "",
                            Department = "",
                            Designation = "",
                            EmpName = "",
                            EmpId = 0,
                            EmpSal = null,
                            Title = "Error",
                            Msg = "MANUAL SALARY NOT ALLOWED",
                            Location = ""
                        };
                        return dat;
                    }
                    List<EmpSalaryVM> empsal = new List<EmpSalaryVM>();
                    if (DistId == 1)
                    {
                        empsal = db.spPay_GetEmpSalary_4_Disbursement(EmpId, month, DistId, source).ToList().Select(x => new EmpSalaryVM()
                        {
                            Amount = x.Amount,
                            Title = x.Title,
                            Type = x.Type
                        }).ToList();
                    }
                    else if (DistId == 2)
                    {
                        empsal = db.spPay_GetEmpAdvance_4_Disbursement(EmpId, month, DistId).ToList().Select(x => new EmpSalaryVM()
                        {
                            Amount = x.Amount,
                            Title = x.Title,
                            Type = x.Type
                        }).ToList();
                    }
                    else if (DistId == 3 || DistId == 4 || DistId == 5)
                    {
                        empsal = db.spPay_GetEmpIncentives_4_Disbursement(EmpId, month, DistId).ToList().Select(x => new EmpSalaryVM()
                        {
                            Amount = x.Amount,
                            Title = x.Title,
                            Type = x.Type
                        }).ToList();
                    }
                    else if (DistId == 6 || DistId == 9)
                    {
                        empsal = db.spPay_GetEmpZakat_4_Disbursement(EmpId, month, DistId).ToList().Select(x => new EmpSalaryVM()
                        {
                            Amount = x.Amount,
                            Title = x.Title,
                            Type = x.Type
                        }).ToList();
                    }
                    else if (DistId == 7)
                    {
                        empsal = db.spPay_GetEmpIftari_4_Disbursement(EmpId, month, DistId).ToList().Select(x => new EmpSalaryVM()
                        {
                            Amount = x.Amount,
                            Title = x.Title,
                            Type = x.Type
                        }).ToList();
                    }
                    else if (DistId == 8)
                    {
                        empsal = db.spPay_GetEmpProdIncentives_4_Disbursement(EmpId, DistId, PeriodId).ToList().Select(x => new EmpSalaryVM()
                        {
                            Amount = x.Amount,
                            Title = x.Title,
                            Type = x.Type
                        }).ToList();
                    }
                    else if (DistId == 10)
                    {
                        empsal = db.spPay_GetRMIncentives_4_Disbursement(EmpId, month, DistId).ToList().Select(x => new EmpSalaryVM()
                        {
                            Amount = x.Amount,
                            Title = x.Title,
                            Type = x.Type
                        }).ToList();
                    }
                    else if (DistId == 11)
                    {
                        empsal = db.spPay_GetEmpGeneralPayment_4_Disbursement(EmpId, month, DistId).ToList().Select(x => new EmpSalaryVM()
                        {
                            Amount = x.Amount,
                            Title = x.Title,
                            Type = x.Type
                        }).ToList();
                    }


                    var ErrorExist = empsal.Where(x => x.Title == "Error" || x.Type == "Error").FirstOrDefault();
                    if (ErrorExist == null)
                    {
                        dat = new EmpSalaryDataVM()
                        {
                            CNIC = emp.CNIC,
                            Department = emp.DeptName,
                            Designation = emp.DesgName,
                            EmpName = emp.EmpName,
                            EmpId = Convert.ToInt32(emp.EmpId),
                            Msg = "NOT PAID",
                            EmpSal = empsal,
                            Location = hdept.Where(x => x.HDeptId == emp.HDeptId).FirstOrDefault().HDeptName
                        };
                    }
                    else
                    {
                        AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                        {
                            EmpId = EmpId,
                            Exception = ErrorExist.Title,
                            SalaryMonth = month,
                            TransDate = DateTime.Now,
                            UserId = UserId,
                            DisbursementSource = source,
                            DisbursementTypeId = DistId
                        });

                        dat = new EmpSalaryDataVM()
                        {
                            CNIC = "",
                            Department = "",
                            Designation = "",
                            EmpName = "",
                            EmpId = 0,
                            EmpSal = null,
                            Title = "Error",
                            Msg = ErrorExist.Title,
                            Location = ""
                        };
                    }
                }
                else
                {
                    dat = new EmpSalaryDataVM()
                    {
                        CNIC = "",
                        Department = "",
                        Designation = "",
                        EmpName = "",
                        EmpId = 0,
                        EmpSal = null,
                        Title = "Error",
                        Msg = "ALREADY PAID",
                        Location = ""
                    };
                }


                return dat;
            }
            catch (Exception ex)
            {
                AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                {
                    EmpId = 0,
                    Exception = ex.Message.ToString() + "Stack Trace : " + ex.StackTrace.ToString(),
                    SalaryMonth = month,
                    TransDate = DateTime.Now,
                    UserId = UserId,
                    DisbursementSource = source,
                    DisbursementTypeId = DistId
                });
                return null;
            }
        }

        public async Task<TransStatus> PayEmpSalary(int EmpId, DateTime month, int userid, int LocId, int DistId, string source, int PeriodId)
        {
            TransStatus stat = new TransStatus();
            try
            {
                var Mon = month.Month;
                var Year = month.Year;
                Pay_SalaryDisbursement AlreadyPaid = new Pay_SalaryDisbursement();
                if (DistId == 8)
                {

                    var perd = await db.Pay_ProductIncCalendar.Where(x => x.RowId == PeriodId).FirstOrDefaultAsync();
                    var dst = perd.ToDate.Date;
                    AlreadyPaid = db.Pay_SalaryDisbursement.Where(x => x.SalaryMonth == dst && x.SalaryMonth.Month == Mon && x.SalaryMonth.Year == Year && x.EmpId == EmpId && x.DisbursementTypeId == 8).FirstOrDefault();
                }
                else
                {
                    AlreadyPaid = db.Pay_SalaryDisbursement.Where(x => x.SalaryMonth.Month == Mon && x.SalaryMonth.Year == Year && x.EmpId == EmpId && (x.DisbursementTypeId == DistId || DistId == 8)).FirstOrDefault();
                }

                if (AlreadyPaid == null)
                {
                    if (DistId == 1)
                    {
                        var status = db.spPay_EmpSalaryDisbursement(EmpId, month, userid, LocId, DistId, source, userid).FirstOrDefault();
                        if (status != null)
                        {

                            stat.Msg = status;
                            if (status == "SALARY PAID")
                            {
                                stat.Status = true;
                            }
                            else
                            {
                                AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                                {
                                    EmpId = EmpId,
                                    Exception = status,
                                    SalaryMonth = month,
                                    TransDate = DateTime.Now,
                                    UserId = userid,
                                    DisbursementSource = source,
                                    DisbursementTypeId = DistId
                                });
                                stat.Status = false;
                            }
                        }
                        else
                        {
                            AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                            {
                                EmpId = EmpId,
                                Exception = "Empty From Database",
                                SalaryMonth = month,
                                TransDate = DateTime.Now,
                                UserId = userid,
                                DisbursementTypeId = DistId,
                                DisbursementSource = source
                            });
                            stat.Status = false;
                        }

                    }
                    else if (DistId == 2)
                    {
                        var status = db.spPay_SalaryAdvanceDisbursement(EmpId, month, userid, LocId, DistId, source).FirstOrDefault();
                        if (!String.IsNullOrWhiteSpace(status))
                        {
                            stat.Msg = status;
                            if (status == "SALARY ADVANCE PAID")
                            {
                                stat.Status = true;
                            }
                            else
                            {
                                AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                                {
                                    EmpId = EmpId,
                                    Exception = status,
                                    SalaryMonth = month,
                                    TransDate = DateTime.Now,
                                    UserId = userid,
                                    DisbursementSource = source,
                                    DisbursementTypeId = DistId
                                });
                                stat.Status = false;
                            }
                        }
                        else
                        {
                            AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                            {
                                EmpId = EmpId,
                                Exception = "Empty From Database",
                                SalaryMonth = month,
                                TransDate = DateTime.Now,
                                UserId = userid,
                                DisbursementTypeId = DistId,
                                DisbursementSource = source
                            });
                            stat.Status = false;
                        }
                    }
                    else if (DistId == 3 || DistId == 4 || DistId == 5 || DistId == 10)
                    {
                        var status = db.spPay_EmpTargetInc_Disbursement(EmpId, month, userid, LocId, DistId, source).FirstOrDefault();
                        if (!String.IsNullOrWhiteSpace(status))
                        {
                            stat.Msg = status;
                            if (status == "INCENTIVES PAID")
                            {
                                stat.Status = true;
                            }
                            else
                            {
                                AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                                {
                                    EmpId = EmpId,
                                    Exception = status,
                                    SalaryMonth = month,
                                    TransDate = DateTime.Now,
                                    UserId = userid,
                                    DisbursementSource = source,
                                    DisbursementTypeId = DistId
                                });
                                stat.Status = false;
                            }
                        }
                        else
                        {
                            AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                            {
                                EmpId = EmpId,
                                Exception = "Empty From Database",
                                SalaryMonth = month,
                                TransDate = DateTime.Now,
                                UserId = userid,
                                DisbursementTypeId = DistId,
                                DisbursementSource = source
                            });
                            stat.Status = false;
                        }
                    }
                    else if (DistId == 6 || DistId == 9)
                    {
                        var status = db.spPay_EmpZakat_Disbursement(EmpId, month, userid, LocId, DistId, source).FirstOrDefault();
                        if (!String.IsNullOrWhiteSpace(status))
                        {
                            stat.Msg = status;
                            if (status == "ZAKAT ALREADY PAID")
                            {
                                stat.Status = true;
                            }
                            else
                            {
                                AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                                {
                                    EmpId = EmpId,
                                    Exception = status,
                                    SalaryMonth = month,
                                    TransDate = DateTime.Now,
                                    UserId = userid,
                                    DisbursementSource = source,
                                    DisbursementTypeId = DistId
                                });
                                stat.Status = false;
                            }
                        }
                        else
                        {
                            AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                            {
                                EmpId = EmpId,
                                Exception = "Empty From Database",
                                SalaryMonth = month,
                                TransDate = DateTime.Now,
                                UserId = userid,
                                DisbursementTypeId = DistId,
                                DisbursementSource = source
                            });
                            stat.Status = false;
                        }
                    }
                    else if (DistId == 7)
                    {
                        var status = db.spPay_EmpIftari_Disbursement(EmpId, month, userid, LocId, DistId, source).FirstOrDefault();
                        if (!String.IsNullOrWhiteSpace(status))
                        {
                            stat.Msg = status;
                            if (status == "IFTARI ALREADY PAID")
                            {
                                stat.Status = true;
                            }
                            else
                            {
                                AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                                {
                                    EmpId = EmpId,
                                    Exception = status,
                                    SalaryMonth = month,
                                    TransDate = DateTime.Now,
                                    UserId = userid,
                                    DisbursementSource = source,
                                    DisbursementTypeId = DistId
                                });
                                stat.Status = false;
                            }
                        }
                        else
                        {
                            AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                            {
                                EmpId = EmpId,
                                Exception = "Empty From Database",
                                SalaryMonth = month,
                                TransDate = DateTime.Now,
                                UserId = userid,
                                DisbursementTypeId = DistId,
                                DisbursementSource = source
                            });
                            stat.Status = false;
                        }
                    }
                    else if (DistId == 8)
                    {
                        var status = db.spPay_ProductIncentiveDisbursement(EmpId, PeriodId, userid, LocId, DistId, source, userid).FirstOrDefault();
                        if (!String.IsNullOrWhiteSpace(status))
                        {
                            stat.Msg = status;
                            if (status == "INCENTIVES PAID")
                            {
                                stat.Status = true;
                            }
                            else
                            {
                                AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                                {
                                    EmpId = EmpId,
                                    Exception = status,
                                    SalaryMonth = month,
                                    TransDate = DateTime.Now,
                                    UserId = userid,
                                    DisbursementSource = source,
                                    DisbursementTypeId = DistId
                                });
                                stat.Status = false;
                            }
                        }
                        else
                        {
                            AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                            {
                                EmpId = EmpId,
                                Exception = "Empty From Database",
                                SalaryMonth = month,
                                TransDate = DateTime.Now,
                                UserId = userid,
                                DisbursementTypeId = DistId,
                                DisbursementSource = source
                            });
                            stat.Status = false;
                        }
                    }
                    else if (DistId == 11)
                    {
                        var status = db.spPay_EmpGeneralPayment_Disbursement(EmpId, month, userid, LocId, DistId, source).FirstOrDefault();
                        if (!String.IsNullOrWhiteSpace(status))
                        {
                            stat.Msg = status;
                            if (status == "AMOUNT ALREADY PAID")
                            {
                                stat.Status = true;
                            }
                            else
                            {
                                AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                                {
                                    EmpId = EmpId,
                                    Exception = status,
                                    SalaryMonth = month,
                                    TransDate = DateTime.Now,
                                    UserId = userid,
                                    DisbursementSource = source,
                                    DisbursementTypeId = DistId
                                });
                                stat.Status = false;
                            }
                        }
                        else
                        {
                            AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                            {
                                EmpId = EmpId,
                                Exception = "Empty From Database",
                                SalaryMonth = month,
                                TransDate = DateTime.Now,
                                UserId = userid,
                                DisbursementTypeId = DistId,
                                DisbursementSource = source
                            });
                            stat.Status = false;
                        }
                    }

                    else
                    {
                        stat.Status = false;
                    }
                }
                else
                {
                    stat.Status = false;
                }

            }
            catch (Exception ex)
            {
                AddSalaryDisbursementLog(new SalaryDisbursementLogVM()
                {
                    EmpId = 0,
                    Exception = ex.Message.ToString() + "Stack Trace : " + ex.StackTrace.ToString(),
                    SalaryMonth = month,
                    TransDate = DateTime.Now,
                    UserId = userid,
                    DisbursementSource = source,
                    DisbursementTypeId = DistId
                });
                stat.Status = false;
            }
            return stat;
        }
        #endregion

        public List<AttendanceFaceVM> faceAttnList(DateTime fromDate, DateTime toDate, int LocId)
        {
            try
            {
                return db.Pay_Attendance.Where(x => x.LocId == LocId && x.AttnDate >= fromDate && x.AttnDate <= toDate && (x.AttnType == "F" || x.AttnTypeOut == "F")).Select(x =>
                new AttendanceFaceVM
                {
                    AttendanceDate = x.AttnDate,
                    EmpPic = @"../../Content/EmpImg/" + x.EmpId.ToString() + ".jpg",
                    IN = x.AttnType == "F" ? @"../../Content/AttImg/" + x.AttnId.ToString() + "_IN.jpg" : x.AttnType == "B" ? @"../../Content/Attendance/scan.png" : "",
                    OUT = x.AttnTypeOut == "F" ? @"../../Content/AttImg/" + x.AttnId.ToString() + "_OUT.jpg" : x.AttnTypeOut == "B" ? @"../../Content/Attendance/scan.png" : "",
                    EmpId = x.EmpId,
                    EmpName = x.Pay_EmpMaster.EmpName
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    public class floatReverseComparer : IComparer<float>
    {
        public int Compare(float x, float y)
        {
            return y.CompareTo(x);
        }
    }
}