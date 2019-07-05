namespace Arcadia.Assistant.WorkHoursCredit.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Approval : StatusChange
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ApprovalId { get; set; }
    }
}