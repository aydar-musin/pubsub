using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.Server.Storage
{
    interface IUserStorage
    {
        void AddConnection(string userName, string connectionId);
        void RemoveConnection(string connectionId);
        void Subscribe(string userName, string groupName);
        void UnSubscribe(string userName, string groupName);

        IEnumerable<string> GetUserGroups(string userName);

        IEnumerable<string> GetUserConnections(string userName);

        void RemoveUser(string userName);
    }
}
