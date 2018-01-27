namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.ComponentModel.DataAnnotations;

    public class WorktimeChangeWithIdModel : WorktimeChangeModel
    {
        [Required]
        public string WorktimeChangeId { get; set; }
    }
}