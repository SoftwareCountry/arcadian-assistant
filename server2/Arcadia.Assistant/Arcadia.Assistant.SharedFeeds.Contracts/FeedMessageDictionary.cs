using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.SharedFeeds.Contracts
{
    public class FeedMessageDictionary : Dictionary<eFeedType, ICollection<FeedMessage>>
    {
    }
}
