namespace Arcadia.Assistant.CSP.Cache
{
    using System.Collections.Concurrent;

    public class MemoryCache
    {
        private readonly ConcurrentDictionary<string, object> storage = new ConcurrentDictionary<string, object>();

        public bool TryGetValue<T>(string cacheKey, out T value)
        {
            if (!this.storage.TryGetValue(cacheKey, out var temp))
            {
                value = default(T);
                return false;
            }

            value = (T)temp;
            return true;
        }

        public void Set(string cacheKey, object value)
        {
            this.storage.AddOrUpdate(cacheKey, value, (s, o) => value);
        }
    }
}
