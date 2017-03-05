using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using PubSub.Server.Storage;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;

namespace PubSub.Server
{
    public class PubSubHub:Hub
    {
        // here we store information about subscriptions
        private static IUserStorage _storage = new UserStorage();

        // here we store information about subscriptions with expiring when user disconected
        private static IExpiringStorage<IEnumerable<string>> _subscriptionExpStorage = new ExpiringStorage<IEnumerable<string>>();

        // last messages for channels with expiring
        private static IExpiringStorage<string> _lastMessageExpStorage = new ExpiringStorage<string>();

        private static long SessionDuration = long.Parse(System.Configuration.ConfigurationManager.AppSettings["SessionDuration"]);
        private static string Key = System.Configuration.ConfigurationManager.AppSettings["AuthKey"];

        public override Task OnConnected()
        {
            var userName = GetUserName(Context);

            _storage.AddConnection(userName,Context.ConnectionId);


            var userGroups = _subscriptionExpStorage.Get(userName); // session not expired
            if (userGroups != null)
            {
                foreach (var group in userGroups)
                    Groups.Add(Context.ConnectionId, group); // add again to subscribed groups

                foreach (var group in userGroups)
                {
                    var lastMsg = _lastMessageExpStorage.Get(group);
                    if (!string.IsNullOrEmpty(lastMsg))
                        Clients.Caller.SendMessage(lastMsg); // send last message from group
                }
            }
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            var userName = GetUserName(Context);

            
            IEnumerable<string> subs = _storage.GetUserGroups(userName);
            if (subs.Any())
                _subscriptionExpStorage.Add(userName, subs, DateTimeOffset.UtcNow.AddSeconds(SessionDuration));

            _storage.RemoveUser(GetUserName(Context));

            return base.OnDisconnected(stopCalled);
        }

        public void Publish(string data)
        {
            string userName = GetUserName(Context);

            Groups.Add(Context.ConnectionId, userName);

            Clients.OthersInGroup(userName).SendMessage(data);

            _lastMessageExpStorage.Add(userName, data, DateTimeOffset.UtcNow.AddSeconds(SessionDuration));
        }

        public void Subscribe(string groupName)
        {
            string userName = GetUserName(Context);

            Groups.Add(Context.ConnectionId, groupName);
            _storage.Subscribe(userName, groupName);
        }

        public void UnSubscribe(string groupName)
        {
            string userName = GetUserName(Context);
            var userConnections = _storage.GetUserConnections(userName);

            foreach (var connection in userConnections)
                Groups.Remove(connection, groupName);

            _storage.UnSubscribe(userName, groupName);
        }


        private string GetUserName(HubCallerContext context)
        {
            try
            {
                string headerText = Crypto.Decrypt(context.Headers["CustomAuthorization"], Key);
                if (headerText.Contains("username="))
                    return headerText.Replace("username=", "");
                else
                    throw new NotAuthorizedException();
            }
            catch
            {
                throw new NotAuthorizedException();
            }
        }
    }
}