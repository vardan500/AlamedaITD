using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace API.Data
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan duration);
        void Remove(string key);
    }

    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            _memoryCache.TryGetValue(key, out T value);
            return value ?? default!;
        }

        public void Set<T>(string key, T value, TimeSpan duration)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(duration); // You can customize expiration as needed
            _memoryCache.Set(key, value, cacheOptions);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            _memoryCache.Remove(key);
        }
    }


}