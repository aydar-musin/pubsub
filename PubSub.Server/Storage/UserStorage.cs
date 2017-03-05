using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;

namespace PubSub.Server.Storage
{
    public class UserStorage : IUserStorage
    {
        private ConcurrentDictionary<string, HashSet<string>> _userConnections = new ConcurrentDictionary<string, HashSet<string>>();
        private ConcurrentDictionary<string, HashSet<string>> _userGroups = new ConcurrentDictionary<string, HashSet<string>>();

        public void AddConnection(string userName, string connectionId)
        {
            HashSet<string> currentConnections = null;
            if(_userConnections.TryGetValue(userName, out currentConnections))
            {
                currentConnections.Add(connectionId);
            }
            else
            {
                if (!_userConnections.TryAdd(userName, new HashSet<string>() { connectionId }))
                    AddConnection(userName, connectionId); // if cannot add, then just try again
            }
        }

        public void Subscribe(string userName, string groupName)
        {
            HashSet<string> currentGroups = null;
            if(_userGroups.TryGetValue(userName, out currentGroups))
            {
                currentGroups.Add(groupName);
            }
            else
            {
                if (!_userGroups.TryAdd(userName, new HashSet<string>() { groupName }))
                    Subscribe(userName, groupName); // if cannot add, just try again
            }
        }

        public void UnSubscribe(string userName, string groupName)
        {
            HashSet<string> userGroups = null;
            if (_userGroups.TryGetValue(userName, out userGroups))
                userGroups.Remove(groupName);
        }

        public void RemoveConnection(string connectionId)
        {
#warning optimization may be required

            _userConnections.AsParallel().ForAll(u =>
            {
                u.Value.Remove(connectionId);
            });
        }

        public IEnumerable<string> GetUserGroups(string userName)
        {
            HashSet<string> userGroups = null;
            if (!_userGroups.TryGetValue(userName, out userGroups))
                userGroups = new HashSet<string>();

            return userGroups;
        }

        public IEnumerable<string> GetUserConnections(string userName)
        {
            HashSet<string> userConnections = null;
            if (!_userConnections.TryGetValue(userName, out userConnections))
                userConnections = new HashSet<string>();

            return userConnections;
        }

        public void RemoveUser(string userName)
        {
            HashSet<string> result = null;
            _userGroups.TryRemove(userName,out result);

            _userConnections.TryRemove(userName, out result);
        }
    }
}