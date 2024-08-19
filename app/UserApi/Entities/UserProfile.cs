using System.ComponentModel.DataAnnotations;

namespace UserApi.Entities;

public class UserProfile : BaseEntity
{
    [StringLength(50)]
    [LogChange]
    public string FirstName { get; set; }

    [StringLength(50)]
    [LogChange]
    public string LastName { get; set; }

    [StringLength(50)]
    public string Email { get; set; }
}