using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace AGEERP.Models
{
    public class CRMBL
    {
        AGEEntities db = new AGEEntities();

        #region Complain
        public async Task<long> AddTickets(Crm_TicketVM mod, int UserId)
        {

            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    mod.WorkingDate = new SetupBL().GetWorkingDate(mod.LocId);
                    Crm_Ticket tbl = new Crm_Ticket
                    {
                        WorkingDate = mod.WorkingDate,
                        CategoryId = mod.CategoryId,
                        Complain = mod.Complain,
                        ContactPerson = mod.ContactPerson,
                        ContactNo = mod.ContactNo,
                        LocId = mod.LocId,
                        Status = "O",
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        Priority = "Normal"
                    };
                    db.Crm_Ticket.Add(tbl);
                    await db.SaveChangesAsync();

                    var respondent = await (from M in db.Crm_ComplainMapping
                                            join E in db.Pay_EmpMaster on M.EmpId equals E.EmpId
                                            where M.CategoryId == mod.CategoryId && E.StatusId == "A"
                                            select new { EmpId = M.EmpId, TicketId = tbl.TicketId }).ToListAsync();
                    foreach (var item in respondent)
                    {
                        db.Crm_Respondent.Add(new Crm_Respondent { EmpId = item.EmpId, TicketId = item.TicketId });
                    }
                    await db.SaveChangesAsync();

                    if (!String.IsNullOrWhiteSpace(mod.UploadedFiles))
                    {
                        List<long> files = mod.UploadedFiles.Split(',').Select(long.Parse).ToList();
                        var IsSave = await new DocumentBL().UpdateDocRef(files, tbl.TicketId);
                        if (!IsSave)
                            scop.Dispose();
                    }

                    scop.Complete();
                    scop.Dispose();
                    return tbl.TicketId;
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<long> AddTicketsMobile(Crm_TicketVM mod, int UserId)
        {

            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    mod.WorkingDate = new SetupBL().GetWorkingDate(mod.LocId);
                    Crm_Ticket tbl = new Crm_Ticket
                    {
                        WorkingDate = mod.WorkingDate,
                        CategoryId = mod.CategoryId,
                        Complain = mod.Complain,
                        ContactPerson = mod.ContactPerson,
                        ContactNo = mod.ContactNo,
                        LocId = mod.LocId,
                        Status = "O",
                        TransDate = DateTime.Now,
                        UserId = UserId,
                        Priority = "Normal"
                    };
                    db.Crm_Ticket.Add(tbl);
                    await db.SaveChangesAsync();

                    var respondent = await (from M in db.Crm_ComplainMapping
                                            join E in db.Pay_EmpMaster on M.EmpId equals E.EmpId
                                            where M.CategoryId == mod.CategoryId && E.StatusId == "A"
                                            select new { EmpId = M.EmpId, TicketId = tbl.TicketId }).ToListAsync();

                    foreach (var item in respondent)
                    {
                        db.Crm_Respondent.Add(new Crm_Respondent { EmpId = item.EmpId, TicketId = item.TicketId });
                    }
                    await db.SaveChangesAsync();

                    scop.Complete();
                    scop.Dispose();
                    return tbl.TicketId;
                }
                catch (Exception ex)
                {
                    await WriteLog(ex.StackTrace, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    scop.Dispose();
                    return 0;
                }
            }
        }
        public async Task<string> CloseTicketMobile(int TicketId, string Response, string Status, int UserId)
        {
            try
            {
                var tbl = new Crm_Response
                {
                    TicketId = TicketId,
                    Response = Response,
                    TransDate = DateTime.Now,
                    UserId = UserId
                };
                db.Crm_Response.Add(tbl);
                await db.SaveChangesAsync();

                if (Status == "C")
                {
                    var tblticket = await db.Crm_Ticket.SingleOrDefaultAsync(x => x.TicketId.Equals(TicketId));
                    if (tblticket.Status == "O")
                    {
                        tblticket.Status = "C";
                        tblticket.CloseDate = DateTime.Now;
                        tblticket.CloseBy = UserId;
                    }
                    await db.SaveChangesAsync();

                }

                return "Success";

            }
            catch (Exception)
            {
                return "Server Error";
            }
        }


        public async Task<List<Crm_TicketVM>> GetAllTicketsMoblie(int CityId, int LocId, int CategoryId, DateTime fromdate, DateTime todate, string Status)
        {
            try
            {
                if (Status == "O")
                {
                    var lst = await (from x in db.Crm_Ticket
                                     join l in db.Comp_Locations on x.LocId equals l.LocId
                                     where (x.LocId == LocId || LocId == 0) &&
                                     (x.CategoryId == CategoryId || CategoryId == 0) &&
                                     (l.CityId == CityId || CityId == 0) &&
                                     (x.TransDate >= fromdate && x.TransDate <= todate) &&
                                      (x.Status == "O" || x.Status == "H")
                                     select new { x, l }).ToListAsync();

                    return (from t in lst
                            select
                          new Crm_TicketVM
                          {
                              LocCode = t.l.LocCode,
                              TicketId = t.x.TicketId,
                              CategoryId = t.x.CategoryId,
                              Complain = t.x.Complain,
                              Category = t.x.Crm_Category.Category,
                              // Response = r.Response,
                              Response = string.Join(",", t.x.Crm_Response.Select(a => a.Response).ToList()),
                              Status = "O",
                              City = t.l.Comp_City.City,
                              Priority = t.x.Priority
                          }).ToList();
                }
                else
                {
                    var lst = await (from x in db.Crm_Ticket
                                     join l in db.Comp_Locations on x.LocId equals l.LocId
                                     where (x.LocId == LocId || LocId == 0) &&
                                     (x.CategoryId == CategoryId || CategoryId == 0) &&
                                     (l.CityId == CityId || CityId == 0) &&
                                     (x.TransDate >= fromdate && x.TransDate <= todate) &&
                                      x.Status == Status
                                     select new { x, l }).ToListAsync();

                    return (from t in lst
                            select
                          new Crm_TicketVM
                          {
                              LocCode = t.l.LocCode,
                              TicketId = t.x.TicketId,
                              CategoryId = t.x.CategoryId,
                              Complain = t.x.Complain,
                              Category = t.x.Crm_Category.Category,
                              // Response = r.Response,
                              Response = string.Join(",", t.x.Crm_Response.Select(a => a.Response).ToList()),
                              Status = t.x.Status,
                              City = t.l.Comp_City.City,
                              Priority = t.x.Priority
                          }).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Crm_TicketVM>> GetTicketsMobile(int LocId)
        {
            try
            {

                var lst = await (from x in db.Crm_Ticket
                                 join l in db.Comp_Locations on x.LocId equals l.LocId
                                 // join r in db.Crm_Response on x.TicketId equals r.TicketId
                                 where (x.LocId == LocId || LocId == 0) &&
                                 //(x.CategoryId == CategoryId || CategoryId == 0) &&
                                 //(l.CityId == CityId || CityId == 0) &&
                                 (x.Status == "O" || x.Status == "H")

                                 select new { x, l }).ToListAsync();

                return (from t in lst
                        select
                          new Crm_TicketVM
                          {
                              LocCode = t.l.LocCode,
                              TicketId = t.x.TicketId,
                              CategoryId = t.x.CategoryId,
                              Complain = t.x.Complain,
                              Category = t.x.Crm_Category.Category,
                                          // Response = r.Response,
                                          Response = string.Join(",", t.x.Crm_Response.Select(a => a.Response).ToList()),
                              Status = t.x.Status,
                              City = t.l.Comp_City.City,
                              Priority = t.x.Priority
                          }).ToList();


            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> WriteLog(string trace, string strMessage)
        {
            try
            {
                FileStream objFilestream = new FileStream("D:\\Web\\logs.txt", FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                await objStreamWriter.WriteLineAsync(DateTime.Now.ToString() + "--" + trace + "--" + strMessage);
                objStreamWriter.Close();
                objFilestream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<List<Crm_TicketVM>> GetTickets(int CityId, int LocId, int CategoryId, int UserId)
        {
            try
            {

                var lst = await (from x in db.Crm_Ticket
                                 join l in db.Comp_Locations on x.LocId equals l.LocId
                                 join m in db.Crm_Respondent on x.TicketId equals m.TicketId
                                 where (x.LocId == LocId || LocId == 0) &&
                                 (x.CategoryId == CategoryId || CategoryId == 0) &&
                                 (l.CityId == CityId || CityId == 0) &&
                                 (x.Status == "O" || x.Status == "H") &&
                                 m.EmpId == UserId
                                 select new { x, l }).ToListAsync();

                return (from t in lst
                        select
                                      new Crm_TicketVM
                                      {
                                          LocCode = t.l.LocCode,
                                          TicketId = t.x.TicketId,
                                          CategoryId = t.x.CategoryId,
                                          Complain = t.x.Complain,
                                          Category = t.x.Crm_Category.Category,
                                          // Response = r.Response,
                                          Response = string.Join(",", t.x.Crm_Response.Select(a => a.Response).ToList()),
                                          Status = t.x.Status,
                                          City = t.l.Comp_City.City,
                                          WorkingDate = t.x.WorkingDate,
                                          Priority = t.x.Priority
                                      }).ToList();


            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Crm_DamageStockTicketVM>> GetDamageStockTickets(int CityId, int LocId, int UserId, DateTime FromDate, DateTime ToDate)
        {
            try
            {

                var lst = await (from x in db.Crm_Ticket
                                 join l in db.Comp_Locations on x.LocId equals l.LocId
                                 //join m in db.Crm_Respondent on x.TicketId equals m.TicketId
                                 join c in db.Crm_InvComplain on x.TicketId equals c.TicketId
                                 join s in db.Inv_Store on c.ItemId equals s.ItemId
                                 join ma in db.Itm_Master on s.SKUId equals ma.SKUId
                                 join mo in db.Itm_Model on ma.ModelId equals mo.ModelId
                                 join t in db.Itm_Type on mo.TypeId equals t.TypeId
                                 join p in db.Itm_Products on t.ProductId equals p.ProductId
                                 join co in db.Itm_Company on t.ComId equals co.ComId
                                 where (x.LocId == LocId || LocId == 0) &&
                                 (l.CityId == CityId || CityId == 0) &&
                                 (x.Status == "O" || x.Status == "H") &&
                                 (x.WorkingDate >= FromDate && x.WorkingDate <= ToDate) 
                                 //&&
                                 //m.EmpId == UserId
                                 select new { x, l.LocCode,l.Comp_City.City,co.ComName,p.ProductName,mo.Model,ma.SKUName,s.SerialNo,c.Type }).ToListAsync();

                return (from t in lst
                        select
                                      new Crm_DamageStockTicketVM
                                      {
                                          LocCode = t.LocCode,
                                          TicketId = t.x.TicketId,
                                          CategoryId = t.x.CategoryId,
                                          Complain = t.x.Complain,
                                          Category = t.x.Crm_Category.Category,
                                          // Response = r.Response,
                                          Response = string.Join(",", t.x.Crm_Response.Select(a => a.Response).ToList()),
                                          Status = t.x.Status,
                                          City = t.City,
                                          WorkingDate = t.x.WorkingDate,
                                          Priority = t.x.Priority,
                                          CompName = t.ComName,
                                          ProductName = t.ProductName,
                                          Model = t.Model,
                                          SerialNo = t.SerialNo,
                                          Type = t.Type == "D" ? "Damage": "Serivce Center"

                                      }).ToList();


            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateTicket(Crm_TicketVM mod, int UserId)
        {
            try
            {
                var tbl = await db.Crm_Ticket.Where(x => x.TicketId == mod.TicketId).FirstOrDefaultAsync();
                if (tbl != null)
                {
                    if (tbl.CategoryId != mod.CategoryId)
                    {
                        tbl.CategoryId = mod.CategoryId;
                    }
                    if ((tbl.Status == "O" || tbl.Status == "H") && tbl.Status != mod.Status)
                    {
                        tbl.Status = mod.Status;
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
        public async Task<List<Crm_TicketVM>> GetTickets(int CityId, int LocId, int CategoryId)
        {
            try
            {

                var lst = await (from x in db.Crm_Ticket
                                 join l in db.Comp_Locations on x.LocId equals l.LocId
                                 // join r in db.Crm_Response on x.TicketId equals r.TicketId
                                 where (x.LocId == LocId || LocId == 0) &&
                                 (x.CategoryId == CategoryId || CategoryId == 0) &&
                                 (l.CityId == CityId || CityId == 0) &&
                                 (x.Status == "O" || x.Status == "H")

                                 select new { x, l }).ToListAsync();

                return (from t in lst
                        select
                                      new Crm_TicketVM
                                      {
                                          LocCode = t.l.LocCode,
                                          TicketId = t.x.TicketId,
                                          CategoryId = t.x.CategoryId,
                                          Complain = t.x.Complain,
                                          Category = t.x.Crm_Category.Category,
                                          // Response = r.Response,
                                          Response = string.Join(",", t.x.Crm_Response.Select(a => a.Response).ToList()),
                                          Status = t.x.Status == "O" ? "Open" : (t.x.Status == "H" ? "Hold" : "Close"),
                                          City = t.l.Comp_City.City,
                                          Priority = t.x.Priority
                                      }).ToList();


            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Crm_TicketVM>> GetTicketsByTicketId(int TicketId)
        {
            try
            {

                var lst = await (from x in db.Crm_Ticket
                                 join l in db.Comp_Locations on x.LocId equals l.LocId
                                 // join r in db.Crm_Response on x.TicketId equals r.TicketId
                                 where (x.TicketId == TicketId) && (x.Status == "O" || x.Status == "H")

                                 select new { x, l }).ToListAsync();

                return (from t in lst
                        select
                                      new Crm_TicketVM
                                      {
                                          LocCode = t.l.LocCode + " (" + t.l.Ext + ")",
                                          TicketId = t.x.TicketId,
                                          CategoryId = t.x.CategoryId,
                                          Complain = t.x.Complain,
                                          ContactPerson = t.x.ContactPerson,
                                          ContactNo = t.x.ContactNo,
                                          Category = t.x.Crm_Category.Category,
                                          UserId = t.x.UserId,
                                          TransDate = t.x.TransDate,
                                          // Response = r.Response,
                                          Response = string.Join(",", t.x.Crm_Response.Select(a => a.Response).ToList()),
                                          Status = t.x.Status,
                                          Priority = t.x.Priority
                                      }).ToList();


            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Crm_TicketVM>> GetTicketsByLoc(int LocId)
        {
            try
            {
                var lst = await (from x in db.Crm_Ticket
                                 where (x.LocId == LocId)
                                 select new Crm_TicketVM
                                 {
                                     TicketId = x.TicketId,
                                     //CategoryId = x.CategoryId,
                                     Complain = x.Complain,
                                     //ContactPerson = x.ContactPerson,
                                     //ContactNo = x.ContactNo,
                                     Category = x.Crm_Category.Category,
                                     TransDate = x.TransDate,
                                     Status = x.Status == "O" ? "Open" : (x.Status == "H" ? "Hold" : "Close")
                                 }).OrderByDescending(x => x.TicketId).ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CloseTicket(int TicketId, int UserId)
        {
            try
            {

                var tbl = await db.Crm_Ticket.SingleOrDefaultAsync(x => x.TicketId.Equals(TicketId));
                if (tbl != null)
                {
                    tbl.Status = "C";
                    tbl.CloseDate = DateTime.Now;
                    tbl.CloseBy = UserId;
                }
                await db.SaveChangesAsync();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        //public async Task<bool> HoldTicket(int TicketId, int UserId)
        //{
        //    try
        //    {

        //        var tbl = await db.Crm_Ticket.SingleOrDefaultAsync(x => x.TicketId.Equals(TicketId));
        //        if (tbl.Status == "O")
        //        {
        //            tbl.Status = "H";
        //            //tbl.CloseDate = DateTime.Now;
        //            //tbl.CloseBy = UserId;
        //        }
        //        await db.SaveChangesAsync();
        //        return true;

        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        //public async Task<bool> OpenTicket(int TicketId, int UserId)
        //{
        //    try
        //    {

        //        var tbl = await db.Crm_Ticket.SingleOrDefaultAsync(x => x.TicketId.Equals(TicketId));
        //        if (tbl.Status == "H")
        //        {
        //            tbl.Status = "O";
        //            //tbl.CloseDate = DateTime.Now;
        //            //tbl.CloseBy = UserId;
        //        }
        //        await db.SaveChangesAsync();
        //        return true;

        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}


        public async Task<string> AddResponse(int TicketId, string Response, int UserId)
        {
            try
            {
                var tbl = new Crm_Response
                {
                    TicketId = TicketId,
                    Response = Response,
                    TransDate = DateTime.Now,
                    UserId = UserId
                };
                db.Crm_Response.Add(tbl);
                await db.SaveChangesAsync();
                return "Success";

            }
            catch (Exception)
            {
                return "Server Error";
            }
        }


        public async Task<List<Crm_TicketVM>> GetResponseList(int TicketId)
        {
            try
            {
                return await db.Crm_Response.Where(x => x.TicketId == TicketId).Select(x =>
                                 new Crm_TicketVM
                                 {
                                     TicketId = x.TicketId,
                                     Response = x.Response,
                                     UserId = x.UserId,
                                     TransDate = x.TransDate
                                 }).ToListAsync();


            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Inventroy Complain

        public async Task<bool> UpdateInvComplain(InvCompalinVM mod, int UserId)
        {
            using (TransactionScope scop = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var tbl = await db.Inv_Store.SingleOrDefaultAsync(x => x.ItemId == mod.ItemId);
                    var EmpName = await db.Pay_EmpMaster.Where(x => x.EmpId == mod.Salesman).Select(x => x.EmpName).FirstOrDefaultAsync();
                    var WorkingDate = new SetupBL().GetWorkingDate(mod.LocId);
                    long TransId = 0;
                    string remarks = tbl.Itm_Master.SKUCode + " " + tbl.SerialNo + " Transfered to " + (mod.Type == "D" ? "Damage Stock (" : "Service Center (") + mod.Remarks+")";
                    if (tbl != null)
                    {
                        if (tbl.Inv_Status.MFact == 1 && tbl.StatusID != 5 && tbl.StatusID != 11)
                        {
                            Crm_Ticket tic = new Crm_Ticket
                            {
                                WorkingDate = WorkingDate,
                                CategoryId = 28,
                                Complain = remarks,
                                ContactPerson = EmpName,
                                ContactNo = mod.CellNo,
                                LocId = mod.LocId,
                                Status = "H",
                                TransDate = DateTime.Now,
                                UserId = UserId,
                                Priority = "Normal"
                            };
                            db.Crm_Ticket.Add(tic);
                            await db.SaveChangesAsync();

                            var respondent = await (from M in db.Crm_ComplainMapping
                                                    join E in db.Pay_EmpMaster on M.EmpId equals E.EmpId
                                                    where M.CategoryId == 28 && E.StatusId == "A"
                                                    select new { EmpId = M.EmpId, TicketId = tic.TicketId }).ToListAsync();
                            foreach (var item in respondent)
                            {
                                db.Crm_Respondent.Add(new Crm_Respondent { EmpId = item.EmpId, TicketId = item.TicketId });
                            }
                            await db.SaveChangesAsync();

                            //var ticketId = await AddTickets(new Crm_TicketVM
                            //{
                            //    CategoryId = 28,
                            //    Complain = mod.Remarks,
                            //    ContactPerson = CP.EmpName,
                            //    ContactNo = mod.CellNo,
                            //    LocId = mod.LocId
                            //},UserId);


                            if (mod.Type == "D")
                            {
                                int Status = tbl.StatusID;
                                tbl.StatusID = 5;
                                await db.SaveChangesAsync();
                                Inv_StoreLog obj = new Inv_StoreLog
                                {
                                    LocId = tbl.LocId,
                                    ItemId = mod.ItemId,
                                    FromStatus = Status,
                                    ToStatus = 5,
                                    UserId = UserId,
                                    TransDate = DateTime.Now,
                                    Reason = mod.Remarks,
                                    IsSerialChange = false
                                };
                                db.Inv_StoreLog.Add(obj);
                                await db.SaveChangesAsync();
                                TransId = obj.RowId;
                            }
                            else if (mod.Type == "S")
                            {
                                Inv_Issue mas = new Inv_Issue()
                                {
                                    FromLocId = mod.LocId,
                                    IssueDate = WorkingDate,
                                    ToLocId = 213,
                                    Status = "I",
                                    TransDate = DateTime.Now,
                                    Remarks = mod.Remarks,
                                    UserId = UserId,
                                };
                                db.Inv_Issue.Add(mas);
                                await db.SaveChangesAsync();
                                TransId = mas.TransId;
                                int counter = 0;

                                if (mod.ItemId > 0)
                                {
                                    var item = await db.Inv_Store.FindAsync(mod.ItemId);
                                    if (item.StatusID != 3 && item.Inv_Status.MFact == 1)
                                    {
                                        item.StatusID = 3;

                                        Inv_IssueDetail det = new Inv_IssueDetail
                                        {
                                            ItemId = mod.ItemId,
                                            Status = "I",
                                            Qty = 1,
                                            TransId = mas.TransId,
                                            PPrice = item.PPrice
                                        };
                                        db.Inv_IssueDetail.Add(det);
                                        await db.SaveChangesAsync();
                                        counter++;
                                    }
                                }

                            }

                            Crm_InvComplain cmp = new Crm_InvComplain
                            {
                                LocId = mod.LocId,
                                Type = mod.Type,
                                ItemId = mod.ItemId,
                                DocDate = WorkingDate,
                                Communicator = mod.Salesman,
                                ContactNo = mod.CellNo,
                                Fault = mod.Fault,
                                Remarks = mod.Remarks,
                                UserId = UserId,
                                TransDate = DateTime.Now,
                                TicketId = tic.TicketId,
                                IssueId = TransId
                            };
                            db.Crm_InvComplain.Add(cmp);
                            await db.SaveChangesAsync();
                            scop.Complete();
                            scop.Dispose();
                            return true;
                        }
                    }
                    scop.Dispose();
                    return false;
                }
                catch (Exception)
                {
                    scop.Dispose();
                    return false;
                }
            }
        }


        #endregion


    }
}