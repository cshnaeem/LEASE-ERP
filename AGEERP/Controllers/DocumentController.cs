using AGEERP.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NotifSystem.Web.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AGEERP.Controllers
{
    [RBAC]
    public class DocumentController : Controller
    {
        DocumentBL docbl = new DocumentBL();
        NotificationHub _notificationhub = new NotificationHub();

        #region DocumentManager
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EDocumentView(long id, long refobjid)
        {
            if (id > 0 && refobjid > 0)
            {
                return View("EDocumentView", new DocumentManagerVM() { id = id, RefObjId = refobjid });
            }
            else
            {
                return Content("Invalid URL");
            }
        }
        public ActionResult EDocumentManager(long id, long refobjid)
        {
            if (id > 0 && refobjid > 0)
            {
                return View(new DocumentManagerVM() { id = id, RefObjId = refobjid });
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult EDocumentManager(FormCollection frm)
        {
            long id = Convert.ToInt64(frm["DocId"]);
            long refobjid = Convert.ToInt64(frm["RefObjId"]);
            if (id > 0 && refobjid > 0)
            {
                return View("EDocumentManagerPop", new DocumentManagerVM() { id = id, RefObjId = refobjid });
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult EDocumentView(FormCollection frm)
        {
            long id = Convert.ToInt64(frm["DocId"]);
            long refobjid = Convert.ToInt64(frm["RefObjId"]);
            if (id > 0 && refobjid > 0)
            {
                return View("EDocumentView", new DocumentManagerVM() { id = id, RefObjId = refobjid });
            }
            else
            {
                return View();
            }
        }

        public ActionResult DocumentManager(long id, long refobjid)
        {
            if (id > 0 && refobjid > 0)
            {
                return PartialView(new DocumentManagerVM() { id = id, RefObjId = refobjid });
            }
            else
            {
                return PartialView();
            }
        }

        public async Task<JsonResult> GetDocumentTypes()
        {
            var lst = await docbl.GetDocumentTypes();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> Document_Read(long id, long refobjid)
        {
            var lst = await docbl.GetAllDocuments(id, refobjid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public async Task<FileResult> Download(int id)
        {
            if (id > 0)
            {
                var FileDetail = await docbl.GetFile(id);
                string path = FileDetail.Path;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                string contentType = "application/pdf";
                FtpWebResponse response = (FtpWebResponse)(await request.GetResponseAsync());
                Stream responseStream = response.GetResponseStream();
                return File(responseStream, contentType, FileDetail.Name);
            }
            else
            {
                return null;
            }
        }

        public async Task<ActionResult> UploadDoc(HttpPostedFileBase file, int id, int refobjid)
        {
            try
            {
                var newGuid = Guid.NewGuid();
                var filename = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                if (ext == ".pdf")
                {
                    DocumentVM docs = new DocumentVM();
                    var FilePaths = await docbl.GetDocumentTypes(refobjid);
                    string physicalpath = physicalpath = FilePaths.Path + newGuid + ext;
                    if (!docbl.Upload(file.InputStream, physicalpath))
                    {
                        return Json(new { ImageUrl = "Error" }, "text/plain");
                    }

                    docs.Created = DateTime.Now;
                    docs.DocTypeId = refobjid;
                    docs.Extension = ext;
                    docs.Path = physicalpath;
                    docs.Name = filename;
                    docs.Size = file.ContentLength;
                    docs.Status = true;
                    docs.RefObjId = id;
                    var TransStatus = await docbl.AddDocument(docs, UserInfo.UserId);
                    return Json(new { ImageUrl = "", FileName = filename }, "text/plain");
                }
                else
                {
                    return Json(new { ImageUrl = "Error" }, "text/plain");
                }
            }
            catch (Exception e)
            {
                return Json(new { ImageUrl = "Error" }, "text/plain");
            }
        }

        public async Task<ActionResult> UploadDocBulk(IEnumerable<HttpPostedFileBase> files, int refobjid)
        {
            List<long> docLst = new List<long>();

            foreach (var item in files)
            {
                try
                {
                    var newGuid = Guid.NewGuid();
                    var filename = Path.GetFileNameWithoutExtension(item.FileName);
                    var ext = Path.GetExtension(item.FileName);
                    if (ext == ".pdf")
                    {
                        DocumentVM docs = new DocumentVM();
                        var FilePaths = await docbl.GetDocumentTypes(refobjid);
                        string physicalpath = FilePaths.Path + newGuid + ext;
                        if (!docbl.Upload(item.InputStream, physicalpath))
                        {
                            return Json(new { ImageUrl = "Error" }, "text/plain");
                        }

                        docs.Created = DateTime.Now;
                        docs.DocTypeId = refobjid;
                        docs.Extension = ext;
                        docs.Path = physicalpath;
                        docs.Name = filename;
                        docs.Size = item.ContentLength;
                        docs.Status = true;
                        docs.RefObjId = 0;
                        docs = await docbl.AddDocument(docs, UserInfo.UserId);
                        docLst.Add(docs.Id);
                    }
                    else
                    {
                        return Json(new { ImageUrl = "Error" }, "text/plain");
                    }
                }
                catch (Exception e)
                {
                    return Json(new { ImageUrl = "Error" }, "text/plain");
                }
            }
            return Json(docLst, "text/plain");

        }

        public async Task<ActionResult> Document_Update(string target, FileManagerEntry entry, long DocId)
        {
            if (entry != null && DocId > 0)
            {
                bool IsUpdate = await docbl.RenameDocument(DocId, entry.Name);
                return Json(entry);
            }
            else
            {
                return Json("");
            }
        }
        public async Task<ActionResult> UpdateRemarks(string remarks, long DocId)
        {
            if (!String.IsNullOrWhiteSpace(remarks) && DocId > 0)
            {
                bool IsUpdate = await docbl.UpdateRemarksDocument(DocId, remarks);
                return Json(IsUpdate);
            }
            else
            {
                return Json("");
            }
        }
        public async Task<ActionResult> Document_Destroy(FileManagerEntry entry, long DocId)
        {
            if (DocId > 0)
            {
                var IsSave = await docbl.RemoveDocument(DocId);
                return Json(IsSave);
            }
            else
            {
                return Json("");
            }
        }


        public async Task<ActionResult> DocumentRemove()
        {
            return Json("");
        }

        #endregion
        #region DocumentSharing

        public async Task<ActionResult> DocShare()
        {
            ViewData["LocVD"] = await new EmployeeBL().Locations();
            return View();
        }
        public async Task<ActionResult> DocumentReceving()
        {
            ViewBag.IsEdit = UserInfo.GroupId == 2 ? true : false;
            ViewData["LocVD"] = await new EmployeeBL().Locations();
            return View();
        }
        public ActionResult DocShareEdit(int id)
        {
            return View();
        }
        public JsonResult GetDocShare(int TransId)
        {
            try
            {
                var DocDetail = new NotificationBL().GetDocDetail(TransId, UserInfo.LocId);
                return Json(DocDetail, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<ActionResult> DocShare_Read([DataSourceRequest] DataSourceRequest request, DateTime StartDate, DateTime EndDate)
        {

            var lst = await new NotificationBL().GetDocumentsSharing(StartDate, EndDate, UserInfo.LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> DocShare_Create([DataSourceRequest] DataSourceRequest request, DocShareVM mod)
        {
            bool Trans = false;
            try
            {

                var workdate = new SetupBL().GetWorkingDate(UserInfo.LocId);
                var TransStatus = await new NotificationBL().ShareDoc(mod, workdate, UserInfo.UserId);
                if (TransStatus)
                {
                    var notiLst = new List<int>();
                    foreach (var item in mod.DocShareDet)
                    {
                        notiLst.AddRange(new NotificationBL().PostNotiLoc(8, item.LocId, mod.DocTitle, UserInfo.UserId));
                    }
                    foreach (var item in notiLst)
                    {
                        _notificationhub.SendNotification(item);
                    }
                    Trans = true;
                    ModelState.AddModelError("Msg", "Save Successfully");
                }
                else
                {
                    Trans = true;
                    ModelState.AddModelError("Msg", "Save Successfully");
                }
            }
            catch (Exception e)
            {
                Trans = false;
                ModelState.AddModelError("Error", "Server Error");
            }
            return Json(new[] { Trans }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> DocShare_Update([DataSourceRequest] DataSourceRequest request, DocShareVM mod)
        {
            bool Trans = false;
            try
            {
                var workdate = new SetupBL().GetWorkingDate(UserInfo.LocId);
                var TransStatus = await new NotificationBL().UpdateDoc(mod.DocDetailId, mod.ReceivedBy1, mod.ReceivedBy2, mod.ReceivedBy3);
                if (TransStatus)
                {
                    Trans = true;
                    ModelState.AddModelError("Msg", "Save Successfully");
                }
            }
            catch (Exception e)
            {
                Trans = false;
                ModelState.AddModelError("Error", "Server Error");
            }
            return Json(new[] { Trans }.ToDataSourceResult(request, ModelState));
        }

        #endregion
        #region DocTransfer
        public ActionResult DocTransfer()
        {
            ViewBag.WorkingDate = new SetupBL().GetWorkingDate(UserInfo.LocId);
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> DocTransfer([Bind] DocTransferVM mod)
        {
            if (ModelState.IsValid)
            {
                var TransId = await docbl.SaveDocTransfer(mod, UserInfo.UserId);
                mod.TransId = TransId;
            }
            return Json(mod, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DocReceive()
        {
            return View();
        }

        public async Task<ActionResult> DocReceive_Read([DataSourceRequest] DataSourceRequest request, int LocId)
        {
            var lst = await docbl.GetDocTransfer(LocId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> DocReceive_Update([DataSourceRequest] DataSourceRequest request, DocTransferVM mod)
        {
            if (mod != null && ModelState.IsValid)
            {
                var tbl = await docbl.SaveDocReceive(mod, UserInfo.UserId);
                if (!tbl)
                    ModelState.AddModelError("", "Server Error");
            }
            return Json(new[] { mod }.ToDataSourceResult(request, ModelState));
        }
        #endregion
    }
}