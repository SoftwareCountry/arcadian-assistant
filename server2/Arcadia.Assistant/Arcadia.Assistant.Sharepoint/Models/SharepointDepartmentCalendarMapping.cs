namespace Arcadia.Assistant.Sharepoint.Models
{
    using System.Text.Json.Serialization;

    public class SharepointDepartmentCalendarMapping
    {
        [JsonPropertyName("id")]
        public string DepartmentId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Calendar { get; set; } = string.Empty;
    }
}