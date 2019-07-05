namespace Arcadia.Assistant.WorkHoursCredit.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Rejection : StatusChange
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid RejectionId { get; set; }
    }
}