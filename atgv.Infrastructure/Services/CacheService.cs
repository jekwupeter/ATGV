using atgv.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace atgv.Infrastructure.Services
{
    public class CacheService(IMemoryCache _memoryCache) : ICacheService
    {
        public T? Get<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out T? value))
            {
                return value;
            }
            return default(T);
        }

        public bool Remove(string key)
        {
            if (_memoryCache.TryGetValue(key, out _))
            {
                _memoryCache.Remove(key);
            }
            return true;
        }

        public bool Set<T>(string key, T value, int expirationInMins)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationInMins)
            };
            _memoryCache.Set(key, value, cacheEntryOptions);
            return true;
        }
    }
}
