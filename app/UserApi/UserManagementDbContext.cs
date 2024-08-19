using Microsoft.EntityFrameworkCore;
using UserApi.Entities;

namespace UserApi;

public class UserManagementDbContext : DbContext
{
    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
    { }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<ActionLog> ActionLogs { get; set; }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        RecordActionLog();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        RecordActionLog();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void RecordActionLog()
    {
        var entityEntries = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).ToList();

        foreach (var modifiedEntity in entityEntries)
        {
            var tableName = modifiedEntity.Metadata.GetTableName();
            var baseEntity = (BaseEntity)modifiedEntity.Entity;
            var entityId = baseEntity.Id;

            var modifiedBy = baseEntity.ModifiedBy;

            foreach (var property in modifiedEntity.Properties)
            {
                var columnName = property.Metadata.Name;
                var oldValue = property.OriginalValue?.ToString() ?? string.Empty;
                var newValue = property.CurrentValue?.ToString() ?? string.Empty;

                if (oldValue == newValue)
                {
                    continue;
                }

                var hasAttr = property.HasLogAttr(modifiedEntity);

                if (hasAttr)
                {
                    ActionLogs.Add(new ActionLog
                    {
                        CreatedOn = DateTime.Now,
                        CreatedBy = modifiedBy ?? string.Empty,
                        TargetTable = tableName,
                        TargetId = entityId,
                        TargetColumn = columnName,
                        OldValue = oldValue,
                        NewValue = newValue
                    });
                }
            }
        }
    }
}