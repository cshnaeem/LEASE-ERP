using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AGEERP.Models
{
    public class NotificationBL
    {
        private AGEEntities db = new AGEEntities();

        public EmployeeVM GetUserLogin(int userid)
        {
            try
            {
                return db.Pay_EmpMaster.Where(x => x.EmpId == userid).Select(x => 
                new EmployeeVM 
                {
                    DeptName = x.Pay_Department.DeptName,
                    EmpName = x.EmpName
                }).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public long SendChat(int from, int to, string msg, bool IsFile)
        {
            try
            {
                Users_Chat cht = new Users_Chat()
                {
                    FromId = from,
                    IsSeen = false,
                    Message = msg,
                    SendDate = DateTime.Now,
                    ToId = to,
                    IsFile = IsFile
                };
                db.Users_Chat.Add(cht);
                db.SaveChanges();
                return cht.RowId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<List<UsersNotificationVM>> GetChat(int userid, int CUserId)
        {
            try
            {
                return await db.Users_Chat.Where(x => (x.ToId == userid && x.FromId == CUserId) || (x.ToId == CUserId && x.FromId == userid)).OrderBy(x => x.RowId).Select(x =>
                 new UsersNotificationVM()
                 {
                     Id = x.RowId,
                     UserId = x.FromId,
                     Notification = x.Message,
                     NotificationDate = x.SendDate,
                     IsSeen = x.IsFile
                 }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string RelativeDate(DateTime theDate)
        {
            try
            {
                Dictionary<long, string> thresholds = new Dictionary<long, string>();
                int minute = 60;
                int hour = 60 * minute;
                int day = 24 * hour;
                thresholds.Add(60, "{0} seconds ago");
                thresholds.Add(minute * 2, "a minute ago");
                thresholds.Add(45 * minute, "{0} minutes ago");
                thresholds.Add(120 * minute, "an hour ago");
                thresholds.Add(day, "{0} hours ago");
                thresholds.Add(day * 2, "yesterday");
                thresholds.Add(day * 30, "{0} days ago");
                thresholds.Add(day * 365, "{0} months ago");
                thresholds.Add(long.MaxValue, "{0} years ago");
                long since = (DateTime.Now.Ticks - theDate.Ticks) / 10000000;
                foreach (long threshold in thresholds.Keys)
                {
                    if (since < threshold)
                    {
                        TimeSpan t = new TimeSpan((DateTime.Now.Ticks - theDate.Ticks));
                        return string.Format(thresholds[threshold], (t.Days > 365 ? t.Days / 365 : (t.Days > 0 ? t.Days : (t.Hours > 0 ? t.Hours : (t.Minutes > 0 ? t.Minutes : (t.Seconds > 0 ? t.Seconds : 0))))).ToString());
                    }
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task<bool> AddUserNotification(int UserId, string message, int SentBy)
        {
            try
            {
                Users_Notifications obj = new Users_Notifications()
                {
                    IsSeen = false,
                    Notification = message,
                    UserId = UserId,
                    NotificationDate = DateTime.Now,
                    SentBy = SentBy
                };
                db.Users_Notifications.Add(obj);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<List<int>> CustomNotification(int HDeptId, int DeptId, int DesgId, int EmpId, string message, int SentBy)
        {
            try
            {
                var lst = await (from x in db.Pay_EmpMaster
                                 join l in db.Users_Login on x.EmpId equals l.EmployeeId
                                 where (x.StatusId == "A") && l.Status
                                 && (x.Pay_Department.HDeptId == HDeptId || HDeptId == 0)
                                 && (x.DeptId == DeptId || DeptId == 0)
                                 && (x.DesgId == DesgId || DesgId == 0)
                                 && (x.EmpId == EmpId || EmpId == 0)
                                 select l.UserID).ToListAsync();

                foreach (var item in lst)
                {
                    Users_Notifications obj = new Users_Notifications()
                    {
                        IsSeen = false,
                        Notification = message,
                        UserId = item,
                        NotificationDate = DateTime.Now,
                        SentBy = SentBy
                    };
                    db.Users_Notifications.Add(obj);
                    await db.SaveChangesAsync();
                }
                return lst;
            }
            catch (Exception e)
            {
                return new List<int>();
            }
        }
        public List<int> PostNotiLoc(int NotiTypeId, int LocId, string message, int SentBy)
        {
            List<int> lst = new List<int>();
            try
            {
                var usr = (from L in db.Users_Login
                           join E in db.Pay_EmpMaster on L.EmployeeId equals E.EmpId
                           join N in db.Users_NotificationMapping on L.GroupId equals N.GroupId
                           where (E.DeptId == LocId || LocId == 0) && E.StatusId == "A" && L.Status && N.NotiTypeId == NotiTypeId
                           select L.UserID).Distinct().ToList();

                foreach (var v in usr)
                {
                    Users_Notifications obj = new Users_Notifications()
                    {
                        IsSeen = false,
                        Notification = message,
                        UserId = v,
                        NotificationDate = DateTime.Now,
                        SentBy = SentBy
                    };
                    db.Users_Notifications.Add(obj);
                    db.SaveChanges();
                    lst.Add(v);
                }
                return lst;
            }
            catch (Exception)
            {
                return lst;
            }
        }

        public bool UpdateUserConnection(int userid, string connectionid)
        {
            try
            {
                var userlog = db.Users_Login.Where(x => x.UserID == userid).FirstOrDefault();
                if (userlog != null)
                {
                    userlog.ConnectionId = connectionid;
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> SeenChatMessage(long rowId)
        {
            try
            {
                var notification = await db.Users_Chat.Where(x => x.RowId == rowId && !x.IsSeen).FirstOrDefaultAsync();
                if (notification != null)
                {
                    notification.IsSeen = true;
                    notification.SeenDate = DateTime.Now;
                    await db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> SeenAllChatMessage(long userId,int cUserId)
        {
            try
            {
                var lst = await db.Users_Chat.Where(x => x.ToId == cUserId && x.FromId == userId && !x.IsSeen).ToListAsync();
                lst.ForEach(x =>
                {
                    x.IsSeen = true;
                    x.SeenDate = DateTime.Now;
                });
                await db.SaveChangesAsync();
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<ChatVM>> AllChatUsers(int userId)
        {
            try
            {
                var lst = await (from E in db.Pay_EmpMaster
                           join U in db.Users_Login on E.EmpId equals U.EmployeeId
                           where U.Status && E.StatusId == "A" && U.UserID != userId
                           select new ChatVM
                           {
                               UserId = U.UserID,
                               Name = U.FullName,
                               UserName = U.Username,
                               Branch = E.Pay_Department.DeptName,
                               IsOnline = false,
                               LastMsg = "",
                               NoOfMsg = 0,
                               LastMsgTime = "",
                               MaxId = 0
                           }).ToListAsync();

                //var chat = db.Users_Chat.Where(x => x.ToId == userId || x.FromId == userId).GroupBy(x => (x.ToId == userId ? x.FromId : x.ToId)).Select(x => new
                //{
                //    UserId = x.Key,
                //    MaxId = x.Max(a => a.RowId),
                //    //SendDate = x.Max(a => a.SendDate),
                //    NoOfMsg = x.Sum(a => (!a.IsSeen && a.ToId == userId) ? 1 : 0)
                //}).ToList();

                //var lst = (from c in chat
                //           join lm in db.Users_Chat on c.MaxId equals lm.RowId
                //           join e in db.Pay_EmpMaster on c.UserId equals e.EmpId
                //           select new ChatVM
                //           {
                //               UserId = e.EmpId,
                //               Name = e.EmpName,
                //               UserName = e.EmpId.ToString(),
                //               Branch = e.Pay_Department.DeptName,
                //               IsOnline = false,
                //               LastMsg = lm.Message,
                //               NoOfMsg = c.NoOfMsg,
                //               LastMsgTime = lm.SendDate.ToString("hh:mm ttt"),
                //               MaxId = c.MaxId
                //           }).OrderByDescending(x => x.MaxId).ToList();

                //foreach (var v in chat)
                //{
                //    var c = db.Pay_EmpMaster.Where(x => x.EmpId == v.UserId).FirstOrDefault();
                //    if(c != null)
                //    {
                //        c.LastMsgTime = v.SendDate.ToString("hh:mm ttt");
                //        c.NoOfMsg = v.NoOfMsg;
                //        c.MaxId = v.MaxId;
                //        c.LastMsg = db.Users_Chat.Where(x => x.RowId == v.MaxId).Select(x => x.Message).FirstOrDefault();
                //    }
                //}

                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<ChatVM> GetUnseenChat(int userId)
        {
            try
            {
                var chat = db.Users_Chat.Where(x => !x.IsSeen && x.ToId == userId).GroupBy(x => x.FromId).Select(x => new
                {
                    UserId = x.Key,
                    MaxId = x.Max(a => a.RowId),
                    //SendDate = x.Max(a => a.SendDate),
                    NoOfMsg = x.Count()
                }).ToList();
                var lst = new List<ChatVM>();
                //if (chat.Count > 0)
                //{
                //    lst = (from c in chat
                //               join lm in db.Users_Chat on c.MaxId equals lm.RowId
                //               join e in db.Pay_EmpMaster on lm.FromId equals e.EmpId
                //               where lm.ToId == userId
                //               select new ChatVM
                //               {
                //                   UserId = e.EmpId,
                //                   Name = e.EmpName,
                //                   UserName = e.EmpId.ToString(),
                //                   Branch = e.Pay_Department.DeptName,
                //                   IsOnline = false,
                //                   LastMsg = lm.IsFile ? "Photo" : lm.Message,
                //                   NoOfMsg = c.NoOfMsg,
                //                   LastMsgTime = lm.SendDate.ToString("hh:mm ttt"),
                //                   MaxId = c.MaxId,
                //                   IsFile = lm.IsFile
                //               }).OrderByDescending(x => x.MaxId).ToList();
                //}
                foreach (var v in chat)
                {
                    var x = (from lm in db.Users_Chat //on c.MaxId equals lm.RowId
                              join e in db.Pay_EmpMaster on lm.FromId equals e.EmpId
                              where lm.RowId == v.MaxId
                              select new
                              {
                                  UserId = e.EmpId,
                                  Name = e.EmpName,
                                  UserName = e.EmpId,
                                  Branch = e.Pay_Department.DeptName,
                                  //IsOnline = false,
                                  LastMsg = lm.IsFile ? "Photo" : lm.Message,
                                  //NoOfMsg = v.NoOfMsg,
                                  LastMsgTime = lm.SendDate,
                                  //MaxId = v.MaxId,
                                  IsFile = lm.IsFile
                              }).FirstOrDefault();
                    lst.Add(new ChatVM {
                               UserId = x.UserId,
                               Name = x.Name,
                               UserName = x.UserName.ToString(),
                               Branch = x.Branch,
                               IsOnline = false,
                               LastMsg = x.LastMsg,
                               NoOfMsg = v.NoOfMsg,
                               LastMsgTime = x.LastMsgTime.ToString("hh:mm ttt"),
                               MaxId = v.MaxId,
                               IsFile = x.IsFile
                           });
                }

                return lst.OrderByDescending(x => x.MaxId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<ChatVM>> GetChatUsers(int userId)
        {
            try
            {
                //var chat = db.Users_Chat.Where(x => x.ToId == userId || x.FromId == userId).GroupBy(x =>  (x.ToId == userId ? x.FromId : x.ToId)).Select(x => new
                //{
                //    UserId = x.Key,
                //    MaxId = x.Max(a => a.RowId),
                //    NoOfMsg = x.Sum(a => (!a.IsSeen && a.ToId == userId) ? 1 : 0)
                //}).ToList();

                //var lst = (from c in chat
                //          join lm in db.Users_Chat on c.MaxId equals lm.RowId
                //          join e in db.Pay_EmpMaster on c.UserId equals e.EmpId
                //          select new ChatVM
                //          {
                //              UserId = e.EmpId,
                //              Name = e.EmpName,
                //              UserName = e.EmpId.ToString(),
                //              Branch = e.Pay_Department.DeptName,
                //              IsOnline = false,
                //              LastMsg = lm.Message,
                //              NoOfMsg = c.NoOfMsg,
                //              LastMsgTime = lm.SendDate.ToString("hh:mm ttt"),
                //              MaxId = c.MaxId
                //          }).OrderByDescending(x => x.MaxId).ToList();


                return db.spRep_ChatList(userId).Select(x => new ChatVM
                {
                    UserId = x.UserId,
                    Name = x.EmpName,
                    UserName = x.UserId.ToString(),
                    Branch = x.DeptName,
                    IsOnline = false,
                    LastMsg = x.Message,
                    NoOfMsg = x.UnSeen ?? 0,
                    LastMsgTime = x.SendDate.ToString("hh:mm ttt"),
                    MaxId = x.Maxid ?? 0
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> ClearAllNoti(int UserId)
        {
            try
            {
                var notification = await db.Users_Notifications.Where(x => x.IsSeen == false && x.UserId == UserId).ToListAsync();
                notification.ForEach(x => { x.IsSeen = true; x.SeenDate = DateTime.Now; });
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> SeenMessage(long notid)
        {
            try
            {
                var notification = await db.Users_Notifications.Where(x => x.Id == notid).FirstOrDefaultAsync();
                if (notification != null)
                {
                    notification.IsSeen = true;
                    notification.SeenDate = DateTime.Now;
                    await db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<UsersNotificationVM> GetUserNotifications(int userid)
        {
            try
            {
                return (db.Users_Notifications.Where(x => x.IsSeen == false && x.UserId == userid).ToList().Select(x =>
                 new UsersNotificationVM()
                 {
                     Id = x.Id,
                     UserId = x.UserId,
                     IsSeen = x.IsSeen,
                     Notification = x.Notification,
                     RefId = x.RefId,
                     RefLink = x.RefLink,
                     NotificationDate = x.NotificationDate,
                     RelativeNotificationDate = RelativeDate(x.NotificationDate),
                     SeenDate = x.SeenDate,
                     SentBy = x.SentBy,
                     SentByUsername = x.Users_Login.FullName
                 })).OrderByDescending(x => x.NotificationDate).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<UsersNotificationVM> GetUserNotificationLatest(int userid)
        {
            try
            {
                List<UsersNotificationVM> notiflst = new List<UsersNotificationVM>();
                var Notifi = (db.Users_Notifications.Where(x => x.IsSeen == false && x.UserId == userid).ToList().Select(x =>
                 new UsersNotificationVM()
                 {
                     Id = x.Id,
                     UserId = x.UserId,
                     IsSeen = x.IsSeen,
                     Notification = x.Notification,
                     RefId = x.RefId,
                     RefLink = x.RefLink,
                     NotificationDate = x.NotificationDate,
                     RelativeNotificationDate = RelativeDate(x.NotificationDate),
                     SeenDate = x.SeenDate,
                     SentBy = x.SentBy,
                     SentByUsername = x.Users_Login.FullName
                 })).OrderByDescending(x => x.NotificationDate).FirstOrDefault();
                notiflst.Add(Notifi);
                return notiflst;
            }
            catch (Exception)
            {
                return null;
            }
        }


        #region Document Sharing 

        public async Task<DocShareVM> GetDocDetail(int TransId, int LocId)
        {
            try
            {
                return await (from item in db.Comp_DocShare
                              join det in db.Comp_DocShareDetail on item.TransId equals det.TransId
                              where item.TransId == TransId && det.LocId == LocId
                              select new DocShareVM()
                              {
                                  Description = item.Description,
                                  DocTitle = item.Title,
                                  TransId = item.TransId,
                                  LocId = det.LocId

                              }).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public async Task<List<DocShareVM>> GetDocumentsSharing(DateTime StartDate, DateTime EndDate, int LocId)
        {
            try
            {
                if (LocId == 72)
                {
                    return await (from item in db.Comp_DocShare
                                  join det in db.Comp_DocShareDetail on item.TransId equals det.TransId
                                  where item.TransDate >= StartDate && item.TransDate <= EndDate
                                  select new DocShareVM()
                                  {
                                      Description = item.Description,
                                      DocTitle = item.Title,
                                      DocId = item.TransId,
                                      TransId = item.TransId,
                                      LocId = det.LocId,
                                      DocDetailId = det.TransDtlId,
                                      ReceivedBy1 = det.ReceivedBy1,
                                      ReceivedBy2 = det.ReceivedBy2,
                                      ReceivedBy3 = det.ReceivedBy3
                                  }).Distinct().ToListAsync();
                }
                else
                {
                    return await (from item in db.Comp_DocShare
                                  join det in db.Comp_DocShareDetail on item.TransId equals det.TransId
                                  where item.TransDate >= StartDate && item.TransDate <= EndDate && det.LocId == LocId
                                  select new DocShareVM()
                                  {
                                      Description = item.Description,
                                      DocTitle = item.Title,
                                      DocId = item.TransId,
                                      TransId = item.TransId,
                                      LocId = det.LocId,
                                      DocDetailId = det.TransDtlId,
                                      ReceivedBy1 = det.ReceivedBy1,
                                      ReceivedBy2 = det.ReceivedBy2,
                                      ReceivedBy3 = det.ReceivedBy3
                                  }).Distinct().ToListAsync();
                }

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> UpdateDoc(long DocDetailId, string RecOne, string RecTwo, string RecThree)
        {
            try
            {
                var DocDetail = await db.Comp_DocShareDetail.Where(x => x.TransDtlId == DocDetailId).FirstOrDefaultAsync();
                DocDetail.ReceivedBy1 = RecOne;
                DocDetail.ReceivedBy2 = RecTwo;
                DocDetail.ReceivedBy3 = RecThree;
                DocDetail.TransDate = DateTime.Now;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<bool> ShareDoc(DocShareVM mod, DateTime WorkDate, int UserId)
        {
            try
            {
                Comp_DocShare md = new Comp_DocShare()
                {
                    Title = mod.DocTitle,
                    Description = mod.Description,
                    TransDate = DateTime.Now,
                    WorkingDate = WorkDate,
                    UserId = UserId
                };
                db.Comp_DocShare.Add(md);
                await db.SaveChangesAsync();
                foreach (var item in mod.DocShareDet)
                {
                    Comp_DocShareDetail mddetail = new Comp_DocShareDetail()
                    {
                        LocId = item.LocId,
                        TransDate = DateTime.Now,
                        TransId = md.TransId,
                        UserId = UserId
                    };
                    db.Comp_DocShareDetail.Add(mddetail);
                    await db.SaveChangesAsync();
                }

                if (!String.IsNullOrWhiteSpace(mod.UploadedFiles))
                {
                    List<long> files = mod.UploadedFiles.Split(',').Select(long.Parse).ToList();
                     await new DocumentBL().UpdateDocRef(files, md.TransId);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion


    }
}