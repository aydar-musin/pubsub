using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.Server.Storage
{
    interface IExpiringStorage<VALUEType>
    {
        void Add(string key, VALUEType value, DateTimeOffset expireAfter);

        VALUEType Get(string key);
    }
}
