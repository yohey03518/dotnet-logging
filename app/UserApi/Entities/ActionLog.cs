using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserApi.Entities;

public class ActionLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(50)]
    public string TargetTable { get; set; }

    [StringLength(50)]
    public string TargetColumn { get; set; }

    public int TargetId { get; set; }

    [StringLength(1000)]
    public string OldValue { get; set; }

    [StringLength(1000)]
    public string NewValue { get; set; }

    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }

    [StringLength(100)]
    public string RequestId { get; set; }
}