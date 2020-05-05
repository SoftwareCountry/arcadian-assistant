using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.NotificationTemplates
{
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    public static class DictionaryExtension
    {
        public static Dictionary<T1, T2> Perform<T1, T2>(
            IEnumerable<KeyValuePair<T1, T2>> target,
            IEnumerable<KeyValuePair<T1, T2>> source)
        {
            return target
                .Concat(source)
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Last().Value);
        }

        public static Dictionary<string, string> SerializeObject(object obj)
        {
            return new Dictionary<string, string>()
                .SerializeObject(obj);
        }

        public static Dictionary<string, string> SerializeObject(
            this Dictionary<string, string> target,
            object obj)
        {
            if (obj != null)
            {
                foreach (var member in obj.GetType().GetMembers())
                {
                    if (member.CustomAttributes.Any(x =>
                        x.GetType().GetNestedTypes().Any(t => t == typeof(DataMemberAttribute)))
                        && !target.Keys.Contains(member.Name))
                    {
                        if (member.MemberType == MemberTypes.Field)
                        {
                            target.Add(member.Name, ((FieldInfo)member).GetValue(obj).ToString());
                        } else if (member.MemberType == MemberTypes.Property)
                        {
                            target.Add(member.Name, ((PropertyInfo)member).GetValue(obj).ToString());
                        }
                    }
                }
            }

            return target;
        }
    }
}
