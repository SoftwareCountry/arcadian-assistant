namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.SharepointApiModels
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class SharepointListItemsResponse
    {
        [JsonPropertyName("value")]
        public IEnumerable<JsonElement>? Value { get; set; }
    }
}