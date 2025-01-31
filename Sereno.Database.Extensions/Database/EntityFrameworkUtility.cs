using Microsoft.EntityFrameworkCore;

namespace Sereno.Database
{
    public class EntityFrameworkUtility
    {

        /// <summary>
        /// Präfixe vor die Spalten setzen (z.B. vTitle, ...)
        /// </summary>
        public static void SetDatabaseColumnPrefixes(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    var columnName = property.Name;
                    var storeType = property.GetColumnType()
                        ?? (Nullable.GetUnderlyingType(property.ClrType)?.Name ?? property.ClrType.Name);

                    // Präfix basierend auf Datentyp hinzufügen
                    if (storeType.StartsWith("nvarchar") || storeType.StartsWith("varchar"))
                    {
                        property.SetColumnName($"v{columnName}");
                    }
                    else if (storeType.ToLower().StartsWith("datetime"))
                    {
                        property.SetColumnName($"d{columnName}");
                    }
                    else if (storeType == "int" || storeType == "bigint" || storeType == "smallint" ||
                             storeType == "tinyint" || storeType == "decimal" || storeType == "float" ||
                             storeType == "numeric" || storeType == "real")
                    {
                        property.SetColumnName($"n{columnName}");
                    }
                }
            }
        }
    }
}
