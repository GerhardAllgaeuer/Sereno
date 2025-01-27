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

        public static void SetColumnPrefixes(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    var columnName = property.Name;
                    var storeType = property.GetColumnType() ?? property.ClrType.Name; // Datenbanktyp oder CLR-Typ

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
