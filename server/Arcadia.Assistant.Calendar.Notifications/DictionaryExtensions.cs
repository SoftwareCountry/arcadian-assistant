namespace Arcadia.Assistant.Calendar.Notifications
{
    using System.Collections.Generic;
    using System.Linq;

    public static class DictionaryExtensions
    {
        public static IDictionary<T1, T2> Merge<T1, T2>(
            this IDictionary<T1, T2> target,
            IEnumerable<KeyValuePair<T1, T2>> source)
        {
            var sourceUnique = source?.Where(kvp => !target.ContainsKey(kvp.Key));

            return target
                ?.Concat(sourceUnique ?? Enumerable.Empty<KeyValuePair<T1, T2>>())
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}