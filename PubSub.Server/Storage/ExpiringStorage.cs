using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace PubSub.Server.Storage
{
    public class ExpiringStorage<ValueType> : IExpiringStorage<ValueType>
    {
        private MemoryCache _cache;

        public ExpiringStorage()
        {
            _cache = new MemoryCache("ExpiringStorageCache");
        }
        public void Add(string key, ValueType value, DateTimeOffset expireAfter)
        {
            if (_cache.Contains(key))
                _cache.Remove(key);

            _cache.Add(key, value, expireAfter);
        }

        public ValueType Get(string key)
        {
            var value = _cache.Get(key);
            return value!=null?(ValueType) value: default(ValueType);
        }
    }
}