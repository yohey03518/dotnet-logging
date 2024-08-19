using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace UserApi;

public class UserManagementDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
    { }

    public DbSet<UserProfile> UserProfiles { get; set; }
}

public class UserProfile : BaseEntity
{
    [StringLength(50)]
    public string FirstName { get; set; }

    [StringLength(50)]
    public string LastName { get; set; }

    [StringLength(50)]
    public string Email { get; set; }
}

public class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedOn { get; set; }

    [Required]
    [StringLength(50)]
    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    [StringLength(50)]
    public string? ModifiedBy { get; set; }
}