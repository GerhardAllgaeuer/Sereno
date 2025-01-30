using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Database
{
    public class EntityFrameworkUtility
    {

        public static void EnableTriggersOnTables(ModelBuilder modelBuilder)
        {
            // Für alle Entitäten im Modell durchlaufen
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                //// Setze die ID-Eigenschaft auf ValueGeneratedNever, wenn eine ID existiert
                //var primaryKey = entityType.FindPrimaryKey();
                //if (primaryKey != null)
                //{
                //    foreach (var keyProperty in primaryKey.Properties)
                //    {
                //        modelBuilder.Entity(entityType.ClrType)
                //            .Property(keyProperty.Name)
                //            .ValueGeneratedNever();
                //    }
                //}

                //// Deaktiviere die OUTPUT-Klausel für alle Tabellen mit Triggern
                //modelBuilder.Entity(entityType.ClrType)
                //    .ToTable(tb => tb.UseSqlOutputClause(false));
            }
        }

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
