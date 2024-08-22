using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserApi.Entities;

public class ActionLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(50)]
    public required string TargetTable { get; set; }

    [StringLength(50)]
    public required string TargetColumn { get; set; }

    public int TargetId { get; set; }

    [StringLength(1000)]
    public required string OldValue { get; set; }

    [StringLength(1000)]
    public required string NewValue { get; set; }

    public DateTime CreatedOn { get; set; }

    [StringLength(1000)]
    public required string CreatedBy { get; set; }

    [StringLength(100)]
    public required string RequestId { get; set; }
}