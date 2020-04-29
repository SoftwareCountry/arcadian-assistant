using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.NotificationTemplates
{
    public class EventParameter
    {
        public EventParameter(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; }

        public string Value { get; }
    }
}
