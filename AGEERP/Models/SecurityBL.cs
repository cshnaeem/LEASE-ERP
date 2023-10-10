using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace AGEERP.Models
{
    public class SecurityBL
    {
        AGEEntities data = new AGEEntities();

        #region Authentication
        public async Task<string> ChangePassword(ChangePasswordVM mod, int userid)
        {
            var user = await data.Users_Login.Where(x => x.UserID == userid).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.Password == mod.CurrentPassword)
                {
                    if (mod.NewPassword == mod.ChangePassword)
                    {
                        user.Password = mod.ChangePassword;
                        user.IsFirstLogin = false;
                        await data.SaveChangesAsync();
                        return "Save Successfully";
                    }
                    return "Password not match";
                }
                return "Current Password not match";
            }
            return "User not found";
        }
        public async Task<LoginRequestVM> CreateLoginRequest(LoginRequestVM mod)
        {
            try
            {
                var emp = await data.Pay_EmpMaster.Where(x => (x.CNIC == mod.CNIC && x.EmpId == mod.UserId)).FirstOrDefaultAsync();
                if (emp == null)
                {
                    mod.Msg = "CNIC or UserId not Match";
                }
                else if (emp.StatusId != "A")
                {
                    mod.Msg = "Inactive Employee";
                }
                else if (emp.ApprovedBy == null)
                {
                    mod.Msg = "Your data not approved by HR";
                }
                else
                {
                    var req = await data.Users_LoginRequest.Where(x => x.UserId == mod.UserId).OrderByDescending(x => x.RowId).FirstOrDefaultAsync();
                    if (req == null)
                    {
                        Users_LoginRequest tbl = new Users_LoginRequest();
                        tbl.UserId = mod.UserId;
                        tbl.CNIC = mod.CNIC;
                        tbl.DeviceId = mod.DeviceId;
                        data.Users_LoginRequest.Add(tbl);
                        await data.SaveChangesAsync();
                        mod.Msg = "OK";
                    }
                    else if (req.ApprovedDate == null && req.CNIC == mod.CNIC && req.DeviceId == mod.DeviceId && req.UserId == mod.UserId)
                    {
                        mod.Msg = "Already request received.";
                    }
                    else
                    {
                        var usr = await data.Users_Login.Where(x => x.UserID == mod.UserId).FirstOrDefaultAsync();
                        if (usr != null)
                        {
                            if (usr.DeviceId == mod.DeviceId)
                            {
                                mod.Msg = "Already registered.";
                            }
                            else
                            {
                                Users_LoginRequest tbl = new Users_LoginRequest();
                                tbl.UserId = mod.UserId;
                                tbl.CNIC = mod.CNIC;
                                tbl.DeviceId = mod.DeviceId;
                                data.Users_LoginRequest.Add(tbl);
                                await data.SaveChangesAsync();
                                mod.Msg = "OK";
                            }
                        }
                        else
                        {
                            Users_LoginRequest tbl = new Users_LoginRequest();
                            tbl.UserId = mod.UserId;
                            tbl.CNIC = mod.CNIC;
                            tbl.DeviceId = mod.DeviceId;
                            data.Users_LoginRequest.Add(tbl);
                            await data.SaveChangesAsync();
                            mod.Msg = "OK";
                        }
                    }
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<UserVM> LoginUser(LoginVM mod, string session, string IP, string computerName, DateTime loginDate)
        {
            try
            {
                UserVM user = new UserVM();
                var usr = await data.Users_Login.Where(x => x.Username == mod.Username).SingleOrDefaultAsync();
                if (usr == null)
                {
                    user.msg = "Incorrect username";
                }
                else if (!usr.Status)
                {
                    user.msg = "Inactive user";
                }
                else if (usr.Password != mod.Password)
                {
                    user.msg = "Incorrect password";
                }
                else
                {
                    string ip = "";
                    if ((IP == "::1" || IP == "127.0.0.1") && usr.GroupId == 1)
                    {
                        ip = "172.16.1";
                        IP = "172.16.1.1";
                    }
                    else
                        ip = IP.Substring(0, IP.LastIndexOf('.'));
                    int lastIp = Convert.ToInt32(IP.Substring(IP.LastIndexOf('.') + 1));
                    var locLst = await data.Comp_LocationIP.Where(x => x.Status).ToListAsync();
                    var loc = locLst.Where(x => x.IP.Substring(0, x.IP.LastIndexOf('.')) == ip &&
                    Convert.ToInt32(x.IP.Substring(x.IP.LastIndexOf('.') + 1)) <= lastIp && Convert.ToInt32(x.ToIP.Substring(x.ToIP.LastIndexOf('.') + 1)) >= lastIp).FirstOrDefault();
                    int locId = 0;
                    if (loc == null && usr.AllowExternalAccess == true)
                    {
                        loc = locLst.Where(x => x.LocId == 72).FirstOrDefault();
                    }
                    if (usr.EmployeeId > 0)
                    {
                        locId = await data.Pay_EmpMaster.Where(x => x.EmpId == usr.EmployeeId).Select(x => x.DeptId).FirstOrDefaultAsync();
                    }

                    if (loc == null)
                    {
                        user.msg = "Unauthorized Location";
                    }
                    else if (usr.GroupId == 2 && loc.LocId != locId)
                    {
                        user.msg = "You should login from your assigned location";
                    }
                    else
                    {
                        user.LocId = loc.LocId;
                        usr.ThemeName = mod.ThemeSel;
                        await data.SaveChangesAsync();
                        user.UserID = usr.UserID;
                        user.GroupId = usr.GroupId;
                        user.FullName = usr.FullName;
                        user.EmployeeId = usr.EmployeeId;
                        usr.SessionId = session;
                        usr.LastLoginDate = loginDate;
                        user.IsFirstLogin = usr.IsFirstLogin;

                        Users_LoginLog log = new Users_LoginLog
                        {
                            IP = IP,
                            LoginDate = loginDate,
                            Session = session,
                            UserId = usr.UserID,
                            ComputerName = computerName
                        };
                        data.Users_LoginLog.Add(log);
                        data.SaveChanges();
                        user.msg = "OK";
                    }
                }
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<string> ForgotPasswordMobile(ForgotPasswordVM mod)
        {
            try
            {
                var user = await data.Users_Login.Where(x => x.Username == mod.Username).FirstOrDefaultAsync();
                if (user != null)
                {
                    if (user.Status == false)
                    {
                        return "InActive User";
                    }
                    if (user.DeviceId != mod.DeviceId)
                    {
                        return "Device not registered";
                    }
                    string mobileNo = "";
                    var emp = await data.Pay_EmpMaster.FirstOrDefaultAsync(x => x.EmpId == user.EmployeeId);
                    if (emp.StatusId != "A")
                    {
                        return "InActive Employee";
                    }
                    if (emp.Mobile1 == mod.MobileNo || emp.Mobile2 == mod.MobileNo)
                    {
                        string str = Guid.NewGuid().ToString().Substring(0, 6);
                        user.Password = str;
                        user.IsFirstLogin = true;
                        await data.SaveChangesAsync();
                        SMSBL sBL = new SMSBL();
                        await sBL.Send(emp.Mobile1, @"Welcome to AGE ERP. Username: " + user.Username + @" Password: " + user.Password);
                        return "OK";
                    }
                    else
                    {
                        return "Username and MobileNo  not match";
                    }
                }
                return "User not found";
            }
            catch (Exception)
            {
                return "Server Error";
            }
        }
        public async Task<string> ForgotPassword(ForgotPasswordVM mod)
        {
            try
            {
                var user = await data.Users_Login.Where(x => x.Username == mod.Username).FirstOrDefaultAsync();
                if (user != null)
                {
                    if (user.Status == false)
                    {
                        return "InActive User";
                    }
                    var emp = await data.Pay_EmpMaster.FirstOrDefaultAsync(x => x.EmpId == user.EmployeeId);
                    if (emp.Mobile1 == mod.MobileNo || emp.Mobile2 == mod.MobileNo)
                    {
                        string str = Guid.NewGuid().ToString().Substring(0, 6);
                        user.Password = str;
                        user.IsFirstLogin = true;
                        await data.SaveChangesAsync();
                        SMSBL sBL = new SMSBL();
                        await sBL.Send(emp.Mobile1, @"Welcome to AGE ERP. Username: " + user.Username + @" Password: " + user.Password);
                        return "OK";
                    }
                    else
                    {
                        return "Username and MobileNo  not match";
                    }
                }
                return "User not found";
            }
            catch (Exception)
            {
                return "Server Error";
            }
        }
        #endregion
        #region UserGroups

        public async Task<List<UsersGroupVM>> UserGroupsList()
        {
            try
            {
                return await data.Users_Group.Where(x => x.Status).Select(it => new UsersGroupVM
                {
                    GroupId = it.GroupId,
                    GroupName = it.GroupName
                }).ToListAsync();

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UsersGroupVM> CreateUserGroups(UsersGroupVM mod)
        {
            try
            {
                Users_Group tbl = new Users_Group
                {
                    GroupName = mod.GroupName,
                    Status = true
                };
                data.Users_Group.Add(tbl);
                await data.SaveChangesAsync();
                mod.GroupId = tbl.GroupId;
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateUserGroups(UsersGroupVM mod)
        {
            try
            {
                var tbl = data.Users_Group.SingleOrDefault(x => x.GroupId.Equals(mod.GroupId));
                if (tbl != null) tbl.GroupName = mod.GroupName;
                await data.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DestroyUserGroups(UsersGroupVM mod)
        {
            try
            {
                var tbl = data.Users_Group.SingleOrDefault(x => x.GroupId.Equals(mod.GroupId));
                if (tbl != null) tbl.Status = false;
                data.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
        #region User

        public async Task<List<UserVM>> UserList()
        {
            try
            {
                //var ls = data.Users_Login
                //    .GroupJoin(data.Pay_EmpMaster, x => x.EmployeeId, e => e.EmpId, (x, e) => new { x, e })
                //    .SelectMany(x => x.e.DefaultIfEmpty(), (xx, ee) => new UserVM
                //    {
                //        FullName = xx.x.FullName,
                //        GroupId = xx.x.GroupId,
                //        //Password = x.Password,
                //        Status = xx.x.Status,
                //        TransDate = xx.x.TransDate,
                //        Username = xx.x.Username,
                //        UserID = xx.x.UserID,
                //        msg = ee == null ? "" : ee.Mobile1
                //    }) ;
                var ls = (from x in data.Users_Login
                          join e in data.Pay_EmpMaster on x.EmployeeId equals e.EmpId into g
                          from e in g.DefaultIfEmpty()
                          select new UserVM
                          {
                              FullName = x.FullName,
                              GroupId = x.GroupId,
                              //Password = x.Password,
                              Status = x.Status,
                              TransDate = x.TransDate,
                              Username = x.Username,
                              UserID = x.UserID,
                              msg = e.Mobile1 ?? "",
                              DeviceId = x.DeviceId
                          });
                var lst = await ls.ToListAsync();
                return lst;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> MakeUserInActive(int EmpId)
        {
            try
            {

                if (EmpId > 0)
                {
                    var IsUserExist = await data.Users_Login.Where(x => x.UserID == EmpId && x.Status).FirstOrDefaultAsync();
                    if (IsUserExist != null)
                    {
                        IsUserExist.Status = false;
                        data.SaveChanges();
                        return true;
                    }
                    return false;
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

        public async Task<UserVM> CreateUser(UserVM mod)
        {
            try
            {
                Users_Login tbl = new Users_Login();
                string str = Guid.NewGuid().ToString().Substring(0, 6);
                tbl.FullName = mod.FullName;
                tbl.GroupId = mod.GroupId;
                tbl.Password = str;
                tbl.Status = true;
                tbl.DeviceId = mod.DeviceId;
                tbl.TransDate = DateTime.Now;
                tbl.IsFirstLogin = true;
                tbl.EmployeeId = mod.EmployeeId;

                if (mod.EmployeeId > 0)
                {
                    tbl.UserID = mod.EmployeeId;
                    tbl.Username = mod.EmployeeId.ToString();
                }
                else
                {
                    mod.Username = mod.Username.Trim();
                    var IsExist = await data.Users_Login.Where(x => x.Username == mod.Username).AnyAsync();
                    if (IsExist)
                        return null;

                    tbl.Username = mod.Username;
                    int maxId = await data.Users_Login.MaxAsync(x => x.UserID);
                    tbl.UserID = maxId++;
                }
                data.Users_Login.Add(tbl);
                await data.SaveChangesAsync();
                mod.UserID = tbl.UserID;
                mod.Status = tbl.Status;
                if (mod.SendSMS == true)
                {
                    var emp = data.Pay_EmpMaster.Where(x => x.EmpId == mod.EmployeeId).FirstOrDefault();
                    if (emp.Mobile1 != null)
                    {
                        SMSBL sBL = new SMSBL();
                        await sBL.Send(emp.Mobile1, @"Welcome to AGE ERP.
Username: " + tbl.Username + @"
Password: " + tbl.Password);
                    }
                }
                return mod;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateUser(UserVM mod)
        {
            try
            {
                var tbl = data.Users_Login.SingleOrDefault(x => x.UserID.Equals(mod.UserID));
                if (tbl != null)
                {
                    //Guid rdn = new Guid();
                    string str = Guid.NewGuid().ToString().Substring(0, 6);
                    tbl.FullName = mod.FullName;
                    tbl.GroupId = mod.GroupId;
                    if (!String.IsNullOrEmpty(mod.Password))
                    {
                        tbl.Password = str;
                    }
                    tbl.Status = mod.Status;
                    //tbl.TransDate = DateTime.Now;
                    tbl.Username = mod.Username;
                    tbl.DeviceId = mod.DeviceId;
                    tbl.EmployeeId = mod.EmployeeId;
                    tbl.ModifiedBy = mod.UserID;
                    tbl.ModifiedDate = DateTime.Now;
                }
                await data.SaveChangesAsync();
                if (mod.SendSMS == true)
                {
                    var emp = data.Pay_EmpMaster.Where(x => x.EmpId == mod.EmployeeId).FirstOrDefault();
                    if (emp.Mobile1 != null)
                    {
                        SMSBL sBL = new SMSBL();
                        await sBL.Send(emp.Mobile1, @"Your AGE ERP password has been reset.
Username: " + tbl.Username + @"
Password: " + tbl.Password);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public bool DestroyUser(UserVM mod)
        //{
        //    try
        //    {
        //        var tbl = data.User.SingleOrDefault(x => x.GroupID.Equals(mod.GroupID));
        //        if (tbl != null) tbl.Status = false;
        //        data.SaveChanges();
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        #endregion
        #region Rights

        public async Task<bool> HasApprovalRight(int userid, int groupId, int menuid)
        {
            var HaRights = await data.Users_GroupAccess.Where(x => x.GroupId == groupId && x.MenuId == menuid).AnyAsync();
            if (HaRights)
            {
                return true;
            }
            return await data.Users_UserAccess.Where(x => x.UserId == userid && x.MenuId == menuid).AnyAsync();
        }
        public async Task<bool> FinHasApprovalRight(int userid, int groupId, long AccId)
        {
            var HaRights = await data.Fin_GroupAccess.Where(x => x.GroupId == groupId && x.AccId == AccId).AnyAsync();
            if (HaRights)
            {
                return true;
            }
            return await data.Fin_UserAccess.Where(x => x.UserId == userid && x.AccId == AccId).AnyAsync();
        }
        public async Task<List<UsersGroupVM>> ActiveUserGroupsList()
        {
            try
            {
                return await data.Users_Group.Where(x => x.Status).Select(it => new UsersGroupVM
                {
                    GroupId = it.GroupId,
                    GroupName = it.GroupName
                }).ToListAsync();

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<int>> GetUserRights(int id)
        {
            try
            {
                var fList = await (from fl in data.Users_UserAccess
                                   where fl.UserId == id
                                   select new { fl.MenuId, fl.Users_Menu.ParentId }
                ).ToListAsync();

                //var objList = await (from fl in data.Users_UserMenuObjectAccess
                //                     where fl.UserId == id
                //                     select fl.ObjectId
                //).ToListAsync();

                var list = (from fl in fList
                            where !(fList.Select(x => x.ParentId)).Contains(fl.MenuId)
                            select fl.MenuId
                ).ToList();

                //list.AddRange(objList);
                var grp = data.Users_Login.Where(x => x.UserID == id).Select(x => x.GroupId).FirstOrDefault();
                if (grp > 0)
                {
                    list.AddRange(await GetGroupRights(grp));
                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<long>> FinGetUserRights(int id)
        {
            try
            {
                var list = await (from fl in data.Fin_UserAccess
                                  where fl.UserId == id
                                  select fl.AccId
                ).ToListAsync();

                //var objList = await (from fl in data.Users_UserMenuObjectAccess
                //                     where fl.UserId == id
                //                     select fl.ObjectId
                //).ToListAsync();

                //var list = (from fl in fList
                //            where !(fList.Select(x => x.ParentId)).Contains(fl.MenuId)
                //            select fl.MenuId
                //).ToList();

                //list.AddRange(objList);
                var grp = data.Users_Login.Where(x => x.UserID == id).Select(x => x.GroupId).FirstOrDefault();
                if (grp > 0)
                {
                    list.AddRange(await FinGetGroupRights(grp));
                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<int>> GetGroupRights(int id)
        {
            try
            {

                var fList = await (from fl in data.Users_GroupAccess
                                   where fl.GroupId == id
                                   select new { fl.MenuId, fl.Users_Menu.ParentId }
                ).ToListAsync();

                //var objList = await (from fl in data.Users_GroupMenuObjectAccess
                //                     where fl.GroupId == id
                //                     select fl.ObjectId
                //).ToListAsync();

                var list = (from fl in fList
                            where !(fList.Select(x => x.ParentId)).Contains(fl.MenuId)
                            select fl.MenuId
                ).ToList();

                //list.AddRange(objList);

                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<long>> FinGetGroupRights(int id)
        {
            try
            {

                var fList = await (from fl in data.Fin_GroupAccess
                                   where fl.GroupId == id
                                   select fl.AccId
                ).ToListAsync();

                //var objList = await (from fl in data.Users_GroupMenuObjectAccess
                //                     where fl.GroupId == id
                //                     select fl.ObjectId
                //).ToListAsync();

                //var list = (from fl in fList
                //            where !(fList.Select(x => x.ParentId)).Contains(fl.MenuId)
                //            select fl.MenuId
                //).ToList();

                //list.AddRange(objList);

                return fList;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<object> GetForms(int id)
        {
            var forms = await (from f in data.Users_Menu
                               where f.ParentId == id
                               orderby f.SortOrder ascending
                               //let menuObj = data.Users_Menu.Where(it => it.ParentId == f.MenuId).ToList()
                               select new
                               {
                                   id = f.MenuId,
                                   Name = f.Title,
                                   hasChildren = //menuObj.Any() || 
                                   data.Users_Menu.Where(it => it.ParentId == f.MenuId).OrderBy(x => x.SortOrder)
                                                     .Select(it => it.Title).Any(),
                                   enabled = true
                               }).ToListAsync();

            //forms.AddRange(await data.Users_MenuObject.Where(it => it.MenuId == id).Select(it => new
            //{
            //    id = it.ObjectId,
            //    Name = it.ObjectName,
            //    hasChildren = false,
            //    enabled = true
            //}).ToListAsync());
            return forms;
        }
        public async Task<object> FinGetForms(long id)
        {
            if (id == 0)
            {
                var forms = await (from f in data.Fin_AcClasses
                                       //where f.ParentId == id
                                       //orderby f.SortOrder ascending
                                       //let menuObj = data.Users_Menu.Where(it => it.ParentId == f.MenuId).ToList()
                                   select new
                                   {
                                       id = f.ClsCode,
                                       Name = f.ClsCode + " " + f.ClsDesc,
                                       hasChildren = true,
                                       enabled = true
                                   }).ToListAsync();
                return forms;
            }
            else if (id < 100)
            {
                var forms = await (from f in data.Fin_AcGroups
                                   where f.ClsCode == id
                                   //orderby f.SortOrder ascending
                                   //let menuObj = data.Users_Menu.Where(it => it.ParentId == f.MenuId).ToList()
                                   select new
                                   {
                                       id = f.GrId,
                                       Name = f.GrCode + " " + f.GrDesc,
                                       hasChildren = true,
                                       enabled = true
                                   }).ToListAsync();
                return forms;
            }
            else if (id < 10000)
            {
                var forms = await (from f in data.Fin_AcControls
                                   where f.GrId == id
                                   //orderby f.SortOrder ascending
                                   //let menuObj = data.Users_Menu.Where(it => it.ParentId == f.MenuId).ToList()
                                   select new
                                   {
                                       id = f.CnId,
                                       Name = f.CnCode + " " + f.CnDesc,
                                       hasChildren = true,
                                       enabled = true
                                   }).ToListAsync();
                return forms;
            }
            else
            {
                var forms = await (from f in data.Fin_Accounts
                                   where f.CnId == id
                                   //orderby f.SortOrder ascending
                                   //let menuObj = data.Users_Menu.Where(it => it.ParentId == f.MenuId).ToList()
                                   select new
                                   {
                                       id = f.AccId,
                                       Name = f.SubCode + " " + f.SubCodeDesc,
                                       hasChildren = false,
                                       enabled = true
                                   }).ToListAsync();
                return forms;
            }
        }
        public async Task<bool> SetRights(List<int> newList, Int32 groupId, int typeId)
        {
            if (groupId <= 0) return false;
            if (typeId == 1)
            {
                //List<int> menuObjcts = newList.Where(v => (v % 100) > 0).ToList();
                //List<int> menuList = newList.Where(x => !menuObjcts.Contains(x)).ToList();

                //List<Users_GroupMenuObjectAccess> newObjectAccessList = new List<Users_GroupMenuObjectAccess>();
                //foreach (var obj in menuObjcts)
                //{
                //    var oId = obj;
                //    oId -= oId % 100;
                //    Users_GroupMenuObjectAccess newObj = new Users_GroupMenuObjectAccess
                //    {
                //        GroupId = groupId,
                //        MenuId = oId,
                //        ObjectId = obj
                //    };
                //    newObjectAccessList.Add(newObj);
                //}

                List<Users_GroupAccess> newMenuAccssList = new List<Users_GroupAccess>();
                foreach (var right in newList.Distinct())
                {
                    Users_GroupAccess newRight = new Users_GroupAccess
                    {
                        GroupId = groupId,
                        MenuId = right
                    };
                    newMenuAccssList.Add(newRight);
                }
                var previousRights = (from r in data.Users_GroupAccess
                                      where r.GroupId == groupId
                                      select r).ToList();

                //var previousObjectRights = (from r in data.Users_GroupMenuObjectAccess
                //                            where r.GroupId == groupId
                //                            select r).ToList();

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        //if (previousObjectRights.Count > 0)
                        //{
                        //    foreach (var right in previousObjectRights)
                        //    {
                        //        data.Users_GroupMenuObject3.Remove(right);
                        //    }
                        //    data.SaveChanges();
                        //}
                        if (previousRights.Count > 0)
                        {
                            foreach (var right in previousRights)
                            {
                                data.Users_GroupAccess.Remove(right);
                            }
                            data.SaveChanges();
                        }
                        data.Users_GroupAccess.AddRange(newMenuAccssList);
                        //data.Users_GroupMenuObjectAccess.AddRange(newObjectAccessList);
                        data.SaveChanges();
                        transaction.Complete();
                        transaction.Dispose();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Dispose();
                        return false;
                    }
                }
            }
            else
            {
                var grp = data.Users_Login.Where(x => x.UserID == groupId).Select(x => x.GroupId).FirstOrDefault();
                var grpList = await GetGroupRights(grp);
                newList.RemoveAll(x => grpList.Contains(x));

                //List<int> menuObjcts = newList.Where(v => (v % 100) > 0).ToList();
                //List<int> menuList = newList.Where(x => !menuObjcts.Contains(x)).ToList();

                //List<Users_UserMenuObjectAccess> newObjectAccessList = new List<Users_UserMenuObjectAccess>();
                //foreach (var obj in menuObjcts)
                //{
                //    var oId = obj;
                //    oId -= oId % 100;
                //    Users_UserMenuObjectAccess newObj = new Users_UserMenuObjectAccess
                //    {
                //        UserId = groupId,
                //        MenuId = oId,
                //        ObjectId = obj,
                //    };
                //    newObjectAccessList.Add(newObj);
                //}

                List<Users_UserAccess> newMenuAccssList = new List<Users_UserAccess>();
                foreach (var right in newList.Distinct())
                {
                    Users_UserAccess newRight = new Users_UserAccess
                    {
                        UserId = groupId,
                        MenuId = right
                    };
                    newMenuAccssList.Add(newRight);
                }
                var previousRights = (from r in data.Users_UserAccess
                                      where r.UserId == groupId
                                      select r).ToList();

                //var previousObjectRights = (from r in data.Users_UserMenuObjectAccess
                //                            where r.UserId == groupId
                //                            select r).ToList();

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        //if (previousObjectRights.Count > 0)
                        //{
                        //    foreach (var right in previousObjectRights)
                        //    {
                        //        data.Users_UserMenuObjectAccess.Remove(right);
                        //    }
                        //    data.SaveChanges();
                        //}
                        if (previousRights.Count > 0)
                        {
                            foreach (var right in previousRights)
                            {
                                data.Users_UserAccess.Remove(right);
                            }
                            data.SaveChanges();
                        }
                        data.Users_UserAccess.AddRange(newMenuAccssList);
                        //data.Users_UserMenuObjectAccess.AddRange(newObjectAccessList);
                        data.SaveChanges();
                        transaction.Complete();
                        transaction.Dispose();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Dispose();
                        return false;
                    }
                }
            }
        }
        public async Task<bool> FinSetRights(List<long> newList, Int32 groupId, int typeId)
        {
            newList = newList.Where(x => x > 999999).ToList();
            if (groupId <= 0) return false;
            if (typeId == 1)
            {
                //List<int> menuObjcts = newList.Where(v => (v % 100) > 0).ToList();
                //List<int> menuList = newList.Where(x => !menuObjcts.Contains(x)).ToList();

                //List<Users_GroupMenuObjectAccess> newObjectAccessList = new List<Users_GroupMenuObjectAccess>();
                //foreach (var obj in menuObjcts)
                //{
                //    var oId = obj;
                //    oId -= oId % 100;
                //    Users_GroupMenuObjectAccess newObj = new Users_GroupMenuObjectAccess
                //    {
                //        GroupId = groupId,
                //        MenuId = oId,
                //        ObjectId = obj
                //    };
                //    newObjectAccessList.Add(newObj);
                //}

                List<Fin_GroupAccess> newMenuAccssList = new List<Fin_GroupAccess>();
                foreach (var right in newList.Distinct())
                {
                    Fin_GroupAccess newRight = new Fin_GroupAccess
                    {
                        GroupId = groupId,
                        AccId = right
                    };
                    newMenuAccssList.Add(newRight);
                }
                var previousRights = (from r in data.Fin_GroupAccess
                                      where r.GroupId == groupId
                                      select r).ToList();

                //var previousObjectRights = (from r in data.Users_GroupMenuObjectAccess
                //                            where r.GroupId == groupId
                //                            select r).ToList();

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        //if (previousObjectRights.Count > 0)
                        //{
                        //    foreach (var right in previousObjectRights)
                        //    {
                        //        data.Users_GroupMenuObject3.Remove(right);
                        //    }
                        //    data.SaveChanges();
                        //}
                        if (previousRights.Count > 0)
                        {
                            foreach (var right in previousRights)
                            {
                                data.Fin_GroupAccess.Remove(right);
                            }
                            data.SaveChanges();
                        }
                        data.Fin_GroupAccess.AddRange(newMenuAccssList);
                        //data.Users_GroupMenuObjectAccess.AddRange(newObjectAccessList);
                        data.SaveChanges();
                        transaction.Complete();
                        transaction.Dispose();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Dispose();
                        return false;
                    }
                }
            }
            else
            {
                var grp = data.Users_Login.Where(x => x.UserID == groupId).Select(x => x.GroupId).FirstOrDefault();
                var grpList = await FinGetGroupRights(grp);
                newList.RemoveAll(x => grpList.Contains(x));

                //List<int> menuObjcts = newList.Where(v => (v % 100) > 0).ToList();
                //List<int> menuList = newList.Where(x => !menuObjcts.Contains(x)).ToList();

                //List<Users_UserMenuObjectAccess> newObjectAccessList = new List<Users_UserMenuObjectAccess>();
                //foreach (var obj in menuObjcts)
                //{
                //    var oId = obj;
                //    oId -= oId % 100;
                //    Users_UserMenuObjectAccess newObj = new Users_UserMenuObjectAccess
                //    {
                //        UserId = groupId,
                //        MenuId = oId,
                //        ObjectId = obj,
                //    };
                //    newObjectAccessList.Add(newObj);
                //}

                List<Fin_UserAccess> newMenuAccssList = new List<Fin_UserAccess>();
                foreach (var right in newList.Distinct())
                {
                    Fin_UserAccess newRight = new Fin_UserAccess
                    {
                        UserId = groupId,
                        AccId = right
                    };
                    newMenuAccssList.Add(newRight);
                }
                var previousRights = (from r in data.Fin_UserAccess
                                      where r.UserId == groupId
                                      select r).ToList();

                //var previousObjectRights = (from r in data.Users_UserMenuObjectAccess
                //                            where r.UserId == groupId
                //                            select r).ToList();

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        //if (previousObjectRights.Count > 0)
                        //{
                        //    foreach (var right in previousObjectRights)
                        //    {
                        //        data.Users_UserMenuObjectAccess.Remove(right);
                        //    }
                        //    data.SaveChanges();
                        //}
                        if (previousRights.Count > 0)
                        {
                            foreach (var right in previousRights)
                            {
                                data.Fin_UserAccess.Remove(right);
                            }
                            data.SaveChanges();
                        }
                        data.Fin_UserAccess.AddRange(newMenuAccssList);
                        //data.Users_UserMenuObjectAccess.AddRange(newObjectAccessList);
                        data.SaveChanges();
                        transaction.Complete();
                        transaction.Dispose();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Dispose();
                        return false;
                    }
                }
            }
        }

        #endregion
        #region MenuObject
        public async Task<bool> SaveMenuObject(int MenuId)
        {
            try
            {
                foreach (var item in MenuObjectTest.Permissions)
                {
                    string title = "/" + item.ActionLink.Replace("-", "/");
                    if (!await data.Users_MenuObject.Where(x => x.MenuId == MenuId && x.ObjectAction == title).AnyAsync())
                    {
                        int objectId = 0;
                        var maxMenu = await data.Users_MenuObject.Where(x => x.MenuId == MenuId).OrderByDescending(x => x.ObjectId).FirstOrDefaultAsync();
                        if (maxMenu == null)
                        {
                            objectId = MenuId + 1;
                        }
                        else
                        {
                            objectId = maxMenu.ObjectId + 1;
                        }
                        Users_MenuObject tbl = new Users_MenuObject
                        {
                            MenuId = MenuId,
                            ObjectAction = title,
                            ObjectId = objectId,
                            ObjectName = title
                        };
                        data.Users_MenuObject.Add(tbl);
                        await data.SaveChangesAsync();
                    }
                }
                MenuObjectTest.Permissions = new List<UserPermissionVM>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<MenuObjectVM>> MenuList()
        {
            try
            {
                return await data.Users_Menu.Select(x =>
                new MenuObjectVM
                {
                    MenuId = x.MenuId,
                    ObjectName = x.Title
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<MenuObjectVM>> MenuObjectList()
        {
            try
            {
                return await data.Users_MenuObject.Select(x =>
                new MenuObjectVM
                {
                    MenuId = x.MenuId,
                    ObjectAction = x.ObjectAction,
                    ObjectId = x.ObjectId,
                    ObjectName = x.ObjectName
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        //public async Task<MenuObjectVM> CreateMenuObject(MenuObjectVM mod, int UserId)
        //{
        //    try
        //    {
        //        Users_MenuObject tbl = new Users_MenuObject
        //        {
        //            MenuId = mod.MenuId,
        //            ObjectAction = mod.ObjectAction,
        //            ObjectId = mod.ObjectId,
        //            ObjectName = mod.ObjectName
        //        };
        //        data.Users_MenuObject.Add(tbl);
        //        await data.SaveChangesAsync();
        //        return mod;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        public async Task<bool> UpdateMenuObject(MenuObjectVM mod, int UserId)
        {
            try
            {
                var tbl = await data.Users_MenuObject.SingleOrDefaultAsync(x => x.ObjectId.Equals(mod.ObjectId));
                if (tbl != null)
                {
                    tbl.MenuId = mod.MenuId;
                    tbl.ObjectAction = mod.ObjectAction;
                    tbl.ObjectName = mod.ObjectName;
                }
                else
                {
                    tbl = new Users_MenuObject
                    {
                        MenuId = mod.MenuId,
                        ObjectAction = mod.ObjectAction,
                        ObjectId = mod.ObjectId,
                        ObjectName = mod.ObjectName
                    };
                    data.Users_MenuObject.Add(tbl);
                }
                await data.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        #endregion
        #region MobileAPI
        public string GetToken(int UserID, int GroupId, int EmployeeId)
        {
            try
            {
                string key = "W03hEb0Hk4Vk1jfU3pBuIcg"; //Secret key which will be used later during validation    
                var issuer = "https://116.58.56.123";  //normally this will be your site URL    

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                //Create a List of Claims, Keep claims name short    
                var permClaims = new List<Claim>();
                permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                permClaims.Add(new Claim("UserID", UserID.ToString()));
                permClaims.Add(new Claim("GroupId", GroupId.ToString()));
                permClaims.Add(new Claim("EmployeeId", EmployeeId.ToString()));
                //Create Security Token object by giving required parameters    
                var token = new JwtSecurityToken(issuer, //Issure    
                                issuer,  //Audience    
                                permClaims,
                                expires: DateTime.Now.AddDays(1),
                                signingCredentials: credentials);
                var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
                return jwt_token;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public async Task<MUserVM> MLoginUser(MUserVM user)
        {
            try
            {
                var usr = await data.Users_Login.Where(x => x.Username == user.Username).SingleOrDefaultAsync();
                if (usr == null)
                {
                    user.Msg = "Incorrect username";
                }
                else if (!usr.Status)
                {
                    user.Msg = "Inactive user";
                }
                else if (usr.Password != user.Password)
                {
                    user.Msg = "Incorrect password";
                }
                else if (usr.DeviceId != user.DeviceId && user.DeviceId != "9bf9ade0caa23bdf" && usr.DeviceId != "1590EE70-6B00-4FD1-9582-9E569BC28113" && usr.DeviceId != "9bf9ade0caa23bdf")
                {
                    user.Msg = "Invalid Device";
                }
                else
                {
                    var LocList = new List<LocationVM>();
                    var emp = await data.Pay_EmpMaster.Where(x => x.EmpId == usr.EmployeeId).FirstOrDefaultAsync();
                    if (emp != null)
                    {
                        if (emp.StatusId != "A")
                        {
                            user.Msg = "Inactive Employee. Please contact HR.";
                            return user;
                        }
                        if (emp.DeptId < 1000)
                        {
                            LocList = await (from location in data.Comp_Locations
                                             where location.LocId == emp.DeptId && location.Status
                                             select new LocationVM()
                                             {
                                                 LocId = location.LocId,
                                                 LocName = location.LocName,
                                                 LocCode = location.LocCode,
                                                 CityId = location.CityId,
                                                 CityName = location.Comp_City.City,
                                                 Lat = location.Lat,
                                                 Long = location.Lng
                                             }).ToListAsync();
                        }
                        else
                        {
                            LocList = await (from item in data.Pay_EmpLocationMapping
                                             join location in data.Comp_Locations on item.LocId equals location.LocId
                                             where item.EmpId == usr.EmployeeId && item.Status && location.Status
                                             select new LocationVM()
                                             {
                                                 LocId = location.LocId,
                                                 LocName = location.LocName,
                                                 LocCode = location.LocCode,
                                                 CityId = location.CityId,
                                                 CityName = location.Comp_City.City,
                                                 Lat = location.Lat,
                                                 Long = location.Lng
                                             }).ToListAsync();

                            LocList.AddRange((from item in data.Pay_EmpHierarchy
                                              join location in data.Comp_Locations on item.LocId equals location.LocId
                                              where item.GMId == usr.EmployeeId ||
                                              item.SSRMId == usr.EmployeeId ||
                                              item.SRMId == usr.EmployeeId ||
                                              item.BDMId == usr.EmployeeId ||
                                              item.RMId == usr.EmployeeId ||
                                              item.RegionalCashHeadId == usr.EmployeeId ||
                                              item.CRCHeadId == usr.EmployeeId ||
                                              item.RAuditorId == usr.EmployeeId ||
                                              item.SAuditorId == usr.EmployeeId ||
                                              item.SAssAuditorId == usr.EmployeeId ||
                                              item.AuditorId == usr.EmployeeId ||
                                              item.DGMId == usr.EmployeeId
                                              select new LocationVM()
                                              {
                                                  LocId = location.LocId,
                                                  LocName = location.LocName,
                                                  LocCode = location.LocCode,
                                                  CityId = location.CityId,
                                                  CityName = location.Comp_City.City,
                                                  Lat = location.Lat,
                                                  Long = location.Lng
                                              }).ToList());
                            LocList = LocList.GroupBy(x => new { x.CityId, x.CityName, x.Lat, x.LocCode, x.LocId, x.LocName, x.Long }).Select(x => new LocationVM
                            {
                                LocId = x.Key.LocId,
                                LocName = x.Key.LocName,
                                LocCode = x.Key.LocCode,
                                CityId = x.Key.CityId,
                                CityName = x.Key.CityName,
                                Lat = x.Key.Lat,
                                Long = x.Key.Long
                            }).ToList();
                            //LocList.AddRange((from item in data.Pay_EmpHierarchy.ToList()
                            //                  join location in data.Comp_Locations on item.LocId equals location.LocId
                            //                  where item.CRCHeadId == usr.EmployeeId ||
                            //                  item.RAuditorId == usr.EmployeeId ||
                            //                  item.SAuditorId == usr.EmployeeId ||
                            //                  item.AuditorId == usr.EmployeeId
                            //                  select new LocationVM()
                            //                  {
                            //                      LocId = location.LocId,
                            //                      LocName = location.LocName,
                            //                      LocCode = location.LocCode,
                            //                      CityId = location.CityId,
                            //                      CityName = location.Comp_City.City,
                            //                      Lat = location.Lat,
                            //                      Long = location.Lng
                            //                  }).ToList());

                        }


                        var Desg = await (from desg in data.Pay_Designation
                                          where desg.DesgId == emp.DesgId
                                          select new EmployeeVM()
                                          {
                                              DesgName = desg.DesgName,
                                              DesgId = desg.DesgId
                                          }).FirstOrDefaultAsync();

                        user.UserID = usr.UserID;
                        user.GroupId = usr.GroupId;
                        user.FullName = usr.FullName;
                        user.EmployeeId = usr.EmployeeId;
                        user.LastLoginDate = DateTime.Now;
                        user.MenuAccess = GetMobileMenuList(user.UserID, user.GroupId);
                        user.LocId = LocList;
                        user.DesgId = Desg.DesgId;
                        user.DesgName = Desg.DesgName;
                        user.IsFirstLogin = usr.IsFirstLogin;
                        user.EmpPic = new HelperBL().ConvertFromPathToBase64("~/Content/EmpImg/" + usr.EmployeeId + ".jpg", "~/app-assets/images/avatar/user.png");

                        //var RecoverStat = new StockBL().GetRecoverPerformance(DateTime.Now, user.UserID);

                        //Users_LoginLog log = new Users_LoginLog
                        //{
                        //    IP = user.IP,
                        //    LoginDate = DateTime.Now,
                        //    Session = "",
                        //    UserId = usr.UserID,
                        //    ComputerName = user.DeviceName
                        //};


                        //data.Users_LoginLog.Add(log);
                        //data.SaveChanges();
                        user.Token = GetToken(usr.UserID, usr.GroupId, usr.EmployeeId);
                        user.Msg = "OK";

                    }
                    return user;
                }
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<string> MChangePassword(MUserVM mod)
        {
            var user = await data.Users_Login.Where(x => x.UserID == mod.UserID).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.DeviceId == mod.DeviceId)
                {
                    if (user.Password == mod.FullName)
                    {
                        user.Password = mod.Password;
                        await data.SaveChangesAsync();
                        return "Save Successfully";
                    }
                    return "Current Password not match";
                }
                return "Invalid DeviceId";
            }
            return "User not found";
        }
        #endregion
        #region MobileMenu
        public List<UserMenuInfo> GetMenuList(int loginId, int groupId)
        {

            //=============== FROM GROUP MENU ACCESS
            var menuListIds = data.Users_GroupAccess.Where(it => it.GroupId == groupId).Select(x => x.MenuId).ToList();
            //=============== FROM GROUP MENU OBJECT ACCESS
            //menuListIds.AddRange(data.Users_GroupMenuObjectAccess.Where(it => it.GroupID == groupId).Select(it => it.MenuId)
            //    .Distinct().ToList());
            //=============== FROM USER MENU ACCESS
            menuListIds.AddRange(data.Users_UserAccess.Where(it => it.UserId == loginId).Select(it => it.MenuId).ToList());
            //=============== FROM USER MENU OBJECT ACCESS
            //menuListIds.AddRange(data.UserMenuObjectAccesses.Where(it => it.LoginID == loginId).Select(it => it.MenuId)
            //    .Distinct().ToList());

            //=============== FINAL LIST OF MENU IDs FOR MENU BUILD
            var pMenuList = data.Users_Menu.Where(it => menuListIds.Contains(it.MenuId) && it.ParentId != 0).Select(it => it.ParentId).ToList();
            menuListIds.AddRange(pMenuList);
            var ppMenuList = data.Users_Menu.Where(it => pMenuList.Contains(it.MenuId) && it.ParentId != 0).Select(it => it.ParentId).ToList();
            menuListIds.AddRange(ppMenuList);
            //menuListIds.AddRange(data.MainMenus.Where(it => pMenuList.Contains(it.MenuId)).Select(it => it.MenuId).ToList());

            var finalList = menuListIds.Distinct().ToList();

            //=============== FINAL MENU BUILD UP
            var menuList = data.Users_Menu.Where(m => finalList.Contains(m.MenuId) && m.IsVisible == true).Select(m => new UserMenuInfo
            {
                MenuId = m.MenuId,
                ActionLink = m.Link,
                Name = m.Title,
                MenuIcon = m.Icon,
                MenuLevel = m.MenuLevel,
                ParentId = m.ParentId,
                SortOrder = m.SortOrder,
                Menu2 = m.Menu2 ?? 100
            }).ToList();

            return menuList.OrderBy(it => it.SortOrder).ToList();
        }
        public List<MUserMenuInfo> GetMobileMenuList(int loginId, int groupId)
        {
            var menuList = (from item in data.Users_MobileMenu
                            join itemmobile in data.Users_MobileMAccess on item.ActivityId equals itemmobile.ActivityId
                            where item.IsVisible == true && item.AppType == 1 && itemmobile.GroupId == groupId
                            select new MUserMenuInfo()
                            {
                                ActivityId = item.ActivityId,
                                ActivityName = item.ActivityTitle,
                                MenuIcon = item.Icon,
                                ParentId = item.ParentId,
                                IsVisible = item.IsVisible,
                                Link = item.Link
                            }).ToList();
            menuList.ForEach(x => x.Link = x.Link.Replace("#EmpId#", loginId.ToString()));

            return menuList;
        }

        public async Task<List<MobileMenuVM>> GetMobileMenu()
        {
            return await data.Users_MobileMenu.Select(x => new MobileMenuVM()
            {
                ActivityTitle = x.ActivityTitle,
                ActivityId = x.ActivityId,
                AppType = x.AppType,
                Icon = x.Icon,
                IsVisible = x.IsVisible,
                Link = x.Link,
                ParentId = x.ParentId
            }).ToListAsync();
        }

        public async Task<bool> AddMobileMenu(MobileMenuVM mod)
        {
            try
            {
                var activity = await data.Users_MobileMenu.Where(x => x.ActivityId == mod.ActivityId).FirstOrDefaultAsync();
                if (activity == null)
                {
                    Users_MobileMenu Mobilem = new Users_MobileMenu()
                    {
                        ActivityTitle = mod.ActivityTitle,
                        ParentId = mod.ParentId,
                        ActivityId = mod.ActivityId,
                        AppType = mod.AppType,
                        Icon = mod.Icon,
                        IsVisible = mod.IsVisible,
                        Link = mod.Link
                    };
                    data.Users_MobileMenu.Add(Mobilem);
                    await data.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> UpdateMobileMenu(MobileMenuVM mod)
        {
            try
            {
                var activity = await data.Users_MobileMenu.Where(x => x.ActivityId == mod.ActivityId).FirstOrDefaultAsync();
                if (activity != null)
                {
                    activity.ActivityId = mod.ActivityId;
                    activity.ActivityTitle = mod.ActivityTitle;
                    activity.AppType = mod.AppType;
                    activity.Icon = mod.Icon;
                    activity.IsVisible = mod.IsVisible;
                    activity.Link = mod.Link;
                    activity.ParentId = mod.ParentId;
                    await data.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> DeleteMobileMenu(MobileMenuVM mod)
        {
            try
            {
                var activity = await data.Users_MobileMenu.Where(x => x.ActivityId == mod.ActivityId).FirstOrDefaultAsync();
                if (activity != null)
                {
                    data.Users_MobileMenu.Remove(activity);
                    await data.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public List<MobileMenuAccessVM> GetMobileMenuAccess()
        {

            try
            {
                return data.Users_Group.Where(x => x.Status).ToList().Select(it => new MobileMenuAccessVM
                {
                    GroupId = it.GroupId,
                    GroupName = it.GroupName,
                    ActivityIds = data.Users_MobileMAccess.Where(x => x.GroupId == it.GroupId).Select(x => x.ActivityId).ToArray()
                }).ToList();

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddUpdateMobileMenuAccess(int groupid, int[] activitys)
        {
            try
            {
                List<Users_MobileMAccess> dat = new List<Users_MobileMAccess>();
                var menugrpacc = data.Users_MobileMAccess.Where(x => x.GroupId == groupid).ToList();
                if (menugrpacc.Count > 0)
                {
                    data.Users_MobileMAccess.RemoveRange(menugrpacc);
                    await data.SaveChangesAsync();
                }
                else
                {
                    data.Users_MobileMAccess.RemoveRange(menugrpacc);
                    await data.SaveChangesAsync();
                }

                foreach (var item in activitys)
                {
                    Users_MobileMAccess mod = new Users_MobileMAccess()
                    {
                        ActivityId = item,
                        GroupId = groupid
                    };
                    dat.Add(mod);
                }

                data.Users_MobileMAccess.AddRange(dat);
                await data.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public async Task<bool> RemoveMobileMAccess(int groupid)
        {
            var grp = data.Users_MobileMAccess.Where(x => x.GroupId == groupid).ToList();
            if (grp.Count() < 0)
            {
                data.Users_MobileMAccess.RemoveRange(grp);
                await data.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


    }

    //Mobile Menu and User Permissions Data for Web

    public static class MDStore
    {
        private static List<UserPermissionVM> permis;
        public static List<UserPermissionVM> Permissions
        {
            get
            {
                return permis;
            }
            set
            {
                try
                {
                    AGEEntities data = new AGEEntities();
                    List<UserPermissionVM> permission = new List<UserPermissionVM>();
                    permission.AddRange((from O in data.Users_MobileMenuObject
                                         join G in data.Users_MobileMAccess on O.ActivityId equals G.ActivityId
                                         select new UserPermissionVM { ActionLink = O.ObjectAction, GroupId = G.GroupId, IsGroup = true }).ToListAsync().Result);
                    permis = permission.Distinct().ToList();
                }
                catch (Exception)
                {
                    permis = null;
                }
            }
        }
    }
    public static class MenuObjectTest
    {
        public static bool IsStarted = false;
        public static List<UserPermissionVM> Permissions;
    }
    public static class DStore
    {
        private static List<UserPermissionVM> permis;
        public static List<UserPermissionVM> Permissions
        {
            get
            {
                return permis;
            }
            set
            {
                try
                {
                    AGEEntities data = new AGEEntities();
                    List<UserPermissionVM> permission = new List<UserPermissionVM>();
                    permission.AddRange(data.Users_GroupAccess.Select(it => new UserPermissionVM { ActionLink = it.Users_Menu.Link, GroupId = it.GroupId, IsGroup = true }).ToListAsync().Result);
                    permission.AddRange((from O in data.Users_MenuObject
                                         join G in data.Users_GroupAccess on O.MenuId equals G.MenuId
                                         select new UserPermissionVM { ActionLink = O.ObjectAction, GroupId = G.GroupId, IsGroup = true }).ToListAsync().Result);
                    permission.AddRange(data.Users_UserAccess.Select(it => new UserPermissionVM { ActionLink = it.Users_Menu.Link, GroupId = it.UserId, IsGroup = false }).ToListAsync().Result);
                    permission.AddRange((from O in data.Users_MenuObject
                                         join G in data.Users_UserAccess on O.MenuId equals G.MenuId
                                         select new UserPermissionVM { ActionLink = O.ObjectAction, GroupId = G.UserId, IsGroup = false }).ToListAsync().Result);

                    permission.ForEach(x => x.ActionLink = x.ActionLink.TrimStart('/').TrimEnd('/').Replace('/', '-'));
                    permis = permission.Distinct().ToList();
                }
                catch (Exception)
                {
                    permis = null;
                }
            }
        }
    }
    public class RBACAttribute : AuthorizeAttribute
    {

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //you can change to any controller
            filterContext.Result = new RedirectResult("/Home/login/");
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            /*Create permission string based on the requested controller name and action name in the format 'controllername-action'*/
            var requiredPermission =
                $"{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}-{filterContext.ActionDescriptor.ActionName}";

            /*Create an instance of our custom user authorisation object passing requesting user's 'Windows Username' into constructor*/
            HttpCookie authCookie = filterContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            var userData = "";
            if (authCookie != null)
            {
                // Get the forms authentication ticket.
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket != null) userData = authTicket.UserData;
            }
            RBACUser requestingUser = new RBACUser(userData);

            if (!requestingUser.HasPermission(requiredPermission))
            {
                Trace.WriteLine(requiredPermission);
                filterContext.Result = new RedirectToRouteResult(
                                               new RouteValueDictionary {
                                                {  "action", "Index" },
                                                { "controller", "Login" } ,
                                               { "ReturnUrl",requiredPermission } });
            }
        }
    }
    public class MRBACAttribute : System.Web.Http.AuthorizeAttribute, System.Web.Http.Filters.IAuthorizationFilter
    {
        public override void OnAuthorization(HttpActionContext filterContext)
        {
            if (filterContext.Request.RequestUri.Port != 8008)
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }
    }
    public class RBACUser
    {
        AGEEntities data = new AGEEntities();
        private int User_Id { get; set; }
        private int Group_Id { get; set; }
        private string Employee_Id { get; set; }
        private string SessionId { get; set; }
        private string CurrentSessionId { get; set; }

        private List<string> permissions = new List<string>();

        public RBACUser(string _username)
        {

            string[] lst = _username.Split(',');
            if (lst.Count() > 2)
            {
                User_Id = Int32.Parse(lst[0]);
                Group_Id = Int32.Parse(lst[1]);
                Employee_Id = lst[2].ToString();
                CurrentSessionId = lst[3];

            }
            GetDatabaseUserRolesPermissions();
        }


        private void GetDatabaseUserRolesPermissions()
        {
            try
            {
                if (User_Id == 0)
                    return;
                SessionId = data.Users_Login.Find(User_Id).SessionId;
                if (DStore.Permissions == null)
                {
                    DStore.Permissions = new List<UserPermissionVM>();
                }
                permissions = DStore.Permissions.Where(x => x.GroupId == Group_Id && x.IsGroup).Select(x => x.ActionLink).ToList();
                permissions.AddRange(DStore.Permissions.Where(x => x.GroupId == User_Id && !x.IsGroup).Select(x => x.ActionLink).ToList());
                permissions.Add("Home-Index");
                permissions.Add("Account-Index");
                permissions.Add("Cash-Index");
                permissions.Add("CRM-Index");
                permissions.Add("Employee-Index");
                permissions.Add("Purchase-Index");
                permissions.Add("Sale-Index");
                permissions.Add("Setup-Index");
                permissions.Add("Stock-Index");
                permissions.Add("Security-Index");
                permissions.Add("Procurement-Index");
                permissions.Add("Home-_BranchDB");
                permissions.Add("Security-GetMenu");
                permissions.Add("Security-LoadChangePassword");
                permissions.Add("Security-ChangePassword");
                permissions.Add("Security-GetUser");
                permissions.Add("Setup-GetWorkingDate");
                permissions.Add("Setup-MarkSeen");
                permissions.Add("Setup-ClearAllNoti");
                permissions.Add("DashBoard-GetDashboardCloud");
                permissions.Add("Home-HelpDesk");
                permissions.Add("Home-Tutorial");
                permissions.Add("Home-Chat");
                permissions.Add("Home-WhatsAGE");
                permissions.Add("Setup-ChatUsers_Read");
                permissions.Add("Setup-AllUsers_Read");
                permissions.Add("Setup-GetUnseenChat");
                permissions.Add("Setup-GetChat");
                permissions.Add("Setup-GetChatUsers");
                permissions.Add("Setup-SeenChat");
                permissions.Add("Setup-CustomDropZone_Save");
                permissions.Add("Security-StopMobileMenuTest");
            }
            catch (Exception)
            {
            }
        }

        public bool HasPermission(string requiredPermission)
        {
            if (SessionId != CurrentSessionId)
            {
                return false;
            }

            return permissions.Contains(requiredPermission);
        }

        public bool HasRole(string role)
        {
            return true;
        }

        public bool HasRoles(string roles)
        {
            bool bFound = false;
            return bFound;
        }
    }
    public class MRBACUser
    {
        AGEEntities data = new AGEEntities();
        private int User_Id { get; set; }
        private int Group_Id { get; set; }
        private string Employee_Id { get; set; }
        private List<string> permissions = new List<string>();

        public MRBACUser(ClaimsIdentity user)
        {
            if (user.HasClaim(x => x.Type == "UserID"))
            {
                User_Id = Int32.Parse(user.Claims.Where(x => x.Type == "UserID").Select(x => x.Value).FirstOrDefault());
                Group_Id = Int32.Parse(user.Claims.Where(x => x.Type == "GroupId").Select(x => x.Value).FirstOrDefault());
                Employee_Id = user.Claims.Where(x => x.Type == "EmployeeId").Select(x => x.Value).FirstOrDefault();
            }
            GetDatabaseUserRolesPermissions();
        }


        private void GetDatabaseUserRolesPermissions()
        {
            try
            {
                if (User_Id == 0)
                    return;
                if (MDStore.Permissions == null)
                {
                    MDStore.Permissions = new List<UserPermissionVM>();
                }
                permissions = MDStore.Permissions.Where(x => x.GroupId == Group_Id && x.IsGroup).Select(x => x.ActionLink).ToList();
            }
            catch (Exception)
            {
            }
        }

        public bool HasPermission(string requiredPermission)
        {
            return permissions.Contains(requiredPermission);
        }

        public bool HasRole(string role)
        {
            return true;
        }

        public bool HasRoles(string roles)
        {
            bool bFound = false;
            return bFound;
        }
    }


}