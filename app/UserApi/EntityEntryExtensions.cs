using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UserApi;

public static class EntityEntryExtensions
{
    // tableName -> column Name -> boolean of whether record action log
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> CacheMap = new();

    public static bool NeedLogChange(this EntityEntry modifiedEntity, PropertyEntry property)
    {
        var tableName = modifiedEntity.Metadata.GetTableName()!;
        var propertyName = property.Metadata.Name;

        if (CacheMap.ContainsKey(tableName) && CacheMap[tableName].ContainsKey(propertyName))
        {
            return CacheMap[tableName][propertyName];
        }

        var prop = modifiedEntity.Metadata.ClrType.GetProperties()
            .FirstOrDefault(p => p.Name == propertyName);

        var hasAttr = prop != null && Attribute.IsDefined(prop, typeof(LogChangeAttribute));

        CacheMap.TryAdd(tableName, new ConcurrentDictionary<string, bool>());
        CacheMap[tableName].TryAdd(propertyName, hasAttr);

        return hasAttr;
    }
}