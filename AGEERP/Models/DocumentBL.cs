using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace AGEERP.Models
{
    public class DocumentBL
    {
        AGEEntities db = new AGEEntities();

        public async Task<List<DocumentTypesVM>> GetDocumentTypes()
        {
            try
            {
                return await db.Comp_DocumentTypes.Select(x => new DocumentTypesVM()
                {
                    Id = x.Id,
                    DocType = x.DocType,
                    Path = x.Path
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DocumentTypesVM> GetDocumentTypes(int DocTypeId)
        {
            try
            {
                return await db.Comp_DocumentTypes.Where(x => x.Id == DocTypeId).Select(x => new DocumentTypesVM()
                {
                    Id = x.Id,
                    DocType = x.DocType,
                    Path = x.Path
                }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Upload(Stream sourceStream, string path)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("administrator", "Izah@2486@$*^");
                //var sourceStream = file.InputStream;
                var requestStream = request.GetRequestStream();
                //request.ContentLength = sourceStream.Length;

                sourceStream.CopyTo(requestStream);

                //byte[] buffer = new byte[1000000];
                //int bytesRead = sourceStream.Read(buffer, 0, 1000000);
                //do
                //{
                //    requestStream.Write(buffer, 0, bytesRead);
                //    bytesRead = sourceStream.Read(buffer, 0, 1000000);
                //} while (bytesRead > 0);
                sourceStream.Close();
                requestStream.Close();
                //var response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        


        //public async Task<List<BankBookVM>> GetAllDocuments(int doctypeid)
        //{
        //    try
        //    {
        //        return await db.Fin_BankBook.Where(x => x.ActiveStatus).Select(x =>
        //        new BankBookVM
        //        {
        //            CurrentChqNo = x.CurrentChqNo,
        //            EndChqNo = x.EndChqNo,
        //            StartChqNo = x.StartChqNo,
        //            AccId = x.AccId,
        //            TransID = x.TransID,
        //            SubCodeDesc = ""//x.Fin_Accounts.SubCodeDesc          To be discussed
        //        }).ToListAsync();
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public async Task<DocumentVM> GetFile(int id)
        {
            try
            {
                if (id > 0)
                {
                    return await (from x in db.Comp_Documents
                                  where x.DocId == id
                                  select new DocumentVM()
                                  {
                                      Created = x.Created,
                                      Id = x.DocId,
                                      Name = x.DocName,
                                      Path = x.DocPath,
                                      DocTypeId = x.DocTypeId,
                                      RefObjId = x.RefObjId,
                                      Remarks = x.Remarks,
                                      Status = x.Status,
                                      UserId = x.UserId,
                                      Extension = x.Extension
                                  }).FirstOrDefaultAsync();
                }
                else
                {
                    return new DocumentVM();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<DocumentVM>> GetAllDocuments(long id, long refobjid)
        {
            try
            {
                if (id > 0 && refobjid > 0)
                {
                    //Documents For Employees
                    if (refobjid == 1)
                    {
                        return await (from x in db.Comp_Documents
                                      join docemp in db.Pay_EmpMaster on x.RefObjId equals docemp.EmpId
                                      where x.DocTypeId == refobjid && x.RefObjId == id
                                      select new DocumentVM()
                                      {
                                          Created = x.Created,
                                          Id = x.DocId,
                                          Name = x.DocName,
                                          Path = x.DocPath,
                                          DocTypeId = x.DocTypeId,
                                          Size = x.Size,
                                          Extension = x.Extension,
                                          HasDirectories = false,
                                          IsDirectory = false,
                                          Modified = x.Modified,
                                          RefObjId = x.RefObjId,
                                          Remarks = x.Remarks,
                                          Status = x.Status,
                                          UserId = x.UserId,
                                          documentowner = docemp.EmpName
                                      }).ToListAsync();
                    }
                    //Documents for Suppliers
                    else if (refobjid == 2)
                    {
                        return await (from x in db.Comp_Documents
                                      join supp in db.Inv_Suppliers on x.RefObjId equals supp.SuppId
                                      where x.DocTypeId == refobjid && x.RefObjId == id
                                      select new DocumentVM()
                                      {
                                          Created = x.Created,
                                          Id = x.DocId,
                                          Name = x.DocName,
                                          Path = x.DocPath,
                                          DocTypeId = x.DocTypeId,
                                          Size = x.Size,
                                          Extension = x.Extension,
                                          HasDirectories = false,
                                          IsDirectory = false,
                                          Modified = x.Modified,
                                          RefObjId = x.RefObjId,
                                          Remarks = x.Remarks,
                                          Status = x.Status,
                                          UserId = x.UserId
                                      }).ToListAsync();
                    }
                    //Documents for Customers -- Have to correct the query
                    else if (refobjid == 3)
                    {
                        return await (from x in db.Comp_Documents
                                      join user in db.Pay_EmpMaster on x.UserId equals user.EmpId
                                      where x.DocTypeId == refobjid && x.RefObjId == id
                                      select new DocumentVM()
                                      {
                                          Created = x.Created,
                                          Id = x.DocId,
                                          Name = x.DocName,
                                          Path = x.DocPath,
                                          DocTypeId = x.DocTypeId,
                                          Size = x.Size,
                                          Extension = x.Extension,
                                          HasDirectories = false,
                                          IsDirectory = false,
                                          Modified = x.Modified,
                                          RefObjId = x.RefObjId,
                                          Remarks = x.Remarks,
                                          Status = x.Status,
                                          UserId = x.UserId

                                      }).ToListAsync();
                    }
                    else if (refobjid == 4)
                    {
                        return await (from x in db.Comp_Documents
                                      join user in db.Inv_GRN on x.RefObjId equals user.GRNId
                                      where x.DocTypeId == refobjid && x.RefObjId == id
                                      select new DocumentVM()
                                      {
                                          Created = x.Created,
                                          Id = x.DocId,
                                          Name = x.DocName,
                                          Path = x.DocPath,
                                          DocTypeId = x.DocTypeId,
                                          Size = x.Size,
                                          Extension = x.Extension,
                                          HasDirectories = false,
                                          IsDirectory = false,
                                          Modified = x.Modified,
                                          RefObjId = x.RefObjId,
                                          Remarks = x.Remarks,
                                          Status = x.Status,
                                          UserId = x.UserId

                                      }).ToListAsync();
                    }
                    else if (refobjid == 5)
                    {
                        return await (from x in db.Comp_Documents
                                      join user in db.Inv_PO on x.RefObjId equals user.POId
                                      where x.DocTypeId == refobjid && x.RefObjId == id
                                      select new DocumentVM()
                                      {
                                          Created = x.Created,
                                          Id = x.DocId,
                                          Name = x.DocName,
                                          Path = x.DocPath,
                                          DocTypeId = x.DocTypeId,
                                          Size = x.Size,
                                          Extension = x.Extension,
                                          HasDirectories = false,
                                          IsDirectory = false,
                                          Modified = x.Modified,
                                          RefObjId = x.RefObjId,
                                          Remarks = x.Remarks,
                                          Status = x.Status,
                                          UserId = x.UserId

                                      }).ToListAsync();
                    }
                    else if (refobjid == 6)
                    {
                        return await (from x in db.Comp_Documents
                                      join user in db.Lse_ExpenseTransaction on x.RefObjId equals user.TransId
                                      where x.DocTypeId == refobjid && x.RefObjId == id
                                      select new DocumentVM()
                                      {
                                          Created = x.Created,
                                          Id = x.DocId,
                                          Name = x.DocName,
                                          Path = x.DocPath,
                                          DocTypeId = x.DocTypeId,
                                          Size = x.Size,
                                          Extension = x.Extension,
                                          HasDirectories = false,
                                          IsDirectory = false,
                                          Modified = x.Modified,
                                          RefObjId = x.RefObjId,
                                          Remarks = x.Remarks,
                                          Status = x.Status,
                                          UserId = x.UserId

                                      }).ToListAsync();
                    }
                    else
                    {
                        return await (from x in db.Comp_Documents
                                      where x.DocTypeId == refobjid && x.RefObjId == id
                                      select new DocumentVM()
                                      {
                                          Created = x.Created,
                                          Id = x.DocId,
                                          Name = x.DocName,
                                          Path = x.DocPath,
                                          DocTypeId = x.DocTypeId,
                                          Size = x.Size,
                                          Extension = x.Extension,
                                          HasDirectories = false,
                                          IsDirectory = false,
                                          Modified = x.Modified,
                                          RefObjId = x.RefObjId,
                                          Remarks = x.Remarks,
                                          Status = x.Status,
                                          UserId = x.UserId
                                      }).ToListAsync();
                    }
                }
                else
                {
                    return new List<DocumentVM>();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<DocumentVM> AddDocument(DocumentVM model, int UserId)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(model.Path))
                {
                    Comp_Documents document = new Comp_Documents()
                    {
                        Created = DateTime.Now,
                        DocName = model.Name,
                        DocPath = model.Path,
                        DocTypeId = model.DocTypeId,
                        RefObjId = model.RefObjId,
                        Remarks = model.Remarks,
                        Status = true,
                        UserId = UserId,
                        Extension = model.Extension,
                        Size = model.Size
                    };
                    db.Comp_Documents.Add(document);
                    await db.SaveChangesAsync();
                    model.Id = document.DocId;
                }
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<bool> RenameDocument(long docid, string filename)
        {
            if (docid > 0)
            {
                var doc = db.Comp_Documents.Where(x => x.DocId == docid).FirstOrDefault();
                if (doc != null)
                {
                    doc.DocName = filename;
                    doc.Modified = DateTime.Now;
                    await db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public async Task<bool> UpdateRemarksDocument(long docid, string remarks)
        {
            if (docid > 0)
            {
                var doc = db.Comp_Documents.Where(x => x.DocId == docid).FirstOrDefault();
                if (doc != null)
                {
                    doc.Remarks = remarks;
                    doc.Modified = DateTime.Now;
                    await db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RemoveDocument(long docid)
        {
            if (docid > 0)
            {
                var doc = db.Comp_Documents.Where(x => x.DocId == docid).FirstOrDefault();
                if (doc != null)
                {
                    doc.Status = false;
                    await db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> UpdateDocRef(List<long> lst, long refobjId)
        {
            try
            {
                if (lst != null)
                {
                    foreach (var v in lst)
                    {
                        var doc = await db.Comp_Documents.Where(x => x.DocId == v).FirstOrDefaultAsync();
                        if (doc != null)
                        {
                            doc.RefObjId = refobjId;
                        }
                    }
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        #region DocTransfer
        public async Task<int> SaveDocTransfer(DocTransferVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    Comp_DocTransfer tbl = new Comp_DocTransfer();
                    tbl.FromLocId = mod.FromLocId;
                    tbl.ToLocId = mod.ToLocId;
                    tbl.WorkingDate = mod.WorkingDate;
                    tbl.Title = mod.Title;
                    tbl.TransDate = DateTime.Now;
                    tbl.UserId = UserId;
                    tbl.Remarks = mod.Remarks;
                    tbl.Status = "I";
                    db.Comp_DocTransfer.Add(tbl);
                    await db.SaveChangesAsync();
                    if (!String.IsNullOrWhiteSpace(mod.UploadedFiles))
                    {
                        List<long> files = mod.UploadedFiles.Split(',').Select(long.Parse).ToList();
                        var IsSave = await UpdateDocRef(files, tbl.TransId);
                        if (!IsSave)
                            scop.Dispose();
                    }
                    scop.Complete();
                    scop.Dispose();
                    return tbl.TransId;
                }
                catch (Exception ex)
                {
                    scop.Dispose();
                    return 0;
                }
            }
        }


        public async Task<List<DocTransferVM>> GetDocTransfer(int LocId)
        {
            try
            {
                return await (from C in db.Comp_DocTransfer
                              join L in db.Comp_Locations on C.ToLocId equals L.LocId
                              where C.ToLocId == LocId && C.Status == "I"
                              select
                 new DocTransferVM
                 {
                     TransId = C.TransId,
                     Location = L.LocName,
                     Title = C.Title,
                     Remarks = C.Remarks,
                     FromLocId = C.FromLocId,
                     Status = C.Status
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<bool> SaveDocReceive(DocTransferVM item, int UserId)
        {
            try
            {
                var tbl = await db.Comp_DocTransfer.FindAsync(item.TransId);
                if (tbl.RecvBy == null)
                {
                    tbl.RecvBy = UserId;
                    tbl.RecvDate = DateTime.Now;
                    tbl.Remarks = item.Remarks;
                    tbl.Status = item.Status;
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion
        public async Task UploadTicketDoc(Crm_TicketVM mod)
        {
            try
            {
                int refobjid = 17;
                string file = mod.UploadedFiles;
                file = file.Replace("data:image/jpeg;base64,", "");
                var imgByte = Convert.FromBase64String(file);
                string sourcePath = "";
                string destinationPath = "";
                var newGuid = Guid.NewGuid();
                sourcePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/TempImg"), newGuid + ".jpg");
                var path = await GetDocumentTypes(refobjid);

                destinationPath = path.Path + newGuid + ".pdf";
                System.IO.File.WriteAllBytes(sourcePath, imgByte);
                string result = JPGToPDF(sourcePath, destinationPath);
                if (result == "")
                {
                    return;
                }
                string dst = sourcePath.Replace(".jpg", ".pdf");
                FileInfo fil = new FileInfo(dst);
                var filename = Path.GetFileNameWithoutExtension(dst);
                DocumentVM docs = new DocumentVM();
                docs.Created = DateTime.Now;
                docs.DocTypeId = refobjid;
                docs.Extension = ".pdf";
                docs.Path = destinationPath;
                docs.Name = filename;
                docs.Size = fil.Length;
                docs.Status = true;
                docs.RefObjId = mod.TicketId;
                var TransStatus = await AddDocument(docs, mod.UserId);
                return;
            }
            catch (Exception e)
            {
                return;
            }
        }

        public string JPGToPDF(string srcFilename, string dstFilename)
        {
            try
            {
                iTextSharp.text.Rectangle pageSize = null;
                string src = srcFilename.Replace(".jpg", ".pdf");
                using (var srcImage = new Bitmap(srcFilename))
                {
                    pageSize = new iTextSharp.text.Rectangle(0, 0, srcImage.Width, srcImage.Height);
                }
                using (var ms = new MemoryStream())
                {
                    var document = new iTextSharp.text.Document(pageSize, 0, 0, 0, 0);
                    iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
                    document.Open();
                    var image = iTextSharp.text.Image.GetInstance(srcFilename);
                    document.Add(image);
                    document.Close();
                    File.WriteAllBytes(src, ms.ToArray());
                }

                FileStream file = File.OpenRead(src);
                Upload(file, dstFilename);
                return dstFilename;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}