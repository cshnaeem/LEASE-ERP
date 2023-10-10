using AGEERP.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Transports;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotifSystem.Web.Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, UserHubModels> Users =
        new ConcurrentDictionary<string, UserHubModels>(StringComparer.InvariantCultureIgnoreCase);
        NotificationBL data = new NotificationBL();

        public void GetNotification(string id)
        {
            try
            {
                int UserId = Convert.ToInt32(Context.User.Identity.Name);
                var NotifData = LoadNotifData(UserId);
                if (NotifData != null)
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    context.Clients.Client(id).broadcaastNotif(NotifData);

                }

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public void GetUserMsg(string id)
        {
            try
            {
                int UserId = Convert.ToInt32(Context.User.Identity.Name);
                var NotifData = data.GetUnseenChat(UserId);
                if (NotifData != null)
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    context.Clients.Client(id).updateUserList(NotifData);

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public void Send(senderVM sender, string message, string usr, bool IsFile)
        {
            var id = data.SendChat(Convert.ToInt32((sender).name), Convert.ToInt32(usr), message, IsFile);
            var NotifData = data.GetUnseenChat(Convert.ToInt32(usr));
            UserHubModels receiver;
            if (Users.TryGetValue(usr, out receiver))
            {
                var connLst = receiver.ConnectionIds.ToList();
                foreach (var cid in connLst)
                {
                    Clients.Client(cid).broadcastMessage(sender, message, id, IsFile);
                    Clients.Client(cid).updateChatList();
                    Clients.Client(cid).updateUserList(NotifData);
                }
            }

        }
        public class senderVM
        {
            public string name { get; set; }
            public string id { get; set; }
            public string iconUrl { get; set; }
        }
        public void SendTyping(object sender, string usr)
        {
            UserHubModels receiver;
            if (Users.TryGetValue(usr, out receiver))
            {
                var connLst = receiver.ConnectionIds.ToList();
                foreach (var cid in connLst)
                {
                    Clients.Client(cid).sendTyping(sender);
                }
            }
            // Broadcast the typing notification to all clients except the sender.

        }
        public void SendNotification(int SentTo)
        {
            try
            {
              
                UserHubModels receiver;
                if (Users.TryGetValue(SentTo.ToString(), out receiver))
                {
                    var NotifData = LoadNotifDataLatest(SentTo);
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    var connLst = receiver.ConnectionIds.ToList();
                    foreach (var cid in connLst)
                    {
                        context.Clients.Client(cid).broadcaastNotif(NotifData);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        private List<UsersNotificationVM> LoadNotifData(int userId)
        {
            var NotifData = data.GetUserNotifications(userId);
            return NotifData;
        }
        private List<UsersNotificationVM> LoadNotifDataLatest(int userId)
        {
            var NotifData = data.GetUserNotificationLatest(userId);
            return NotifData;
        }
        public override Task OnConnected()
        {
            var UserId = Convert.ToInt32(Context.User.Identity.Name);
            var UserObj = data.GetUserLogin(UserId);
            if (UserObj != null)
            {
                var user = Users.GetOrAdd(Context.User.Identity.Name, _ => new UserHubModels
                {
                    UserName = Context.User.Identity.Name,
                    Branch = UserObj.DeptName,
                    Name = UserObj.EmpName,
                    ConnectionIds = new HashSet<string>()
                });

                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.Add(Context.ConnectionId);
                    if (user.ConnectionIds.Count == 1)
                    {
                        
                        Clients.Others.userConnected(Context.User.Identity.Name);
                    }
                }

              
            }
            return base.OnConnected();
        }

      
        public ConcurrentDictionary<string, UserHubModels> GetOnlineUsers()
        {
            return Users;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserHubModels user;
            Users.TryGetValue(Context.User.Identity.Name, out user);

            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(Context.ConnectionId));
                    if (!user.ConnectionIds.Any())
                    {
                        UserHubModels removedUser;
                        Users.TryRemove(Context.User.Identity.Name, out removedUser);
                        //SendUserCount();
                        Clients.Others.userDisconnected(Context.User.Identity.Name);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}