using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.NotificationTemplates
{
    using System.Linq;

    public class DictionaryMerge
    {
        public Dictionary<T1, T2> Perform<T1, T2>(
            IEnumerable<KeyValuePair<T1, T2>> target,
            IEnumerable<KeyValuePair<T1, T2>> source)
        {
            return target
                .Concat(source)
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Last().Value);
        }
    }
}
