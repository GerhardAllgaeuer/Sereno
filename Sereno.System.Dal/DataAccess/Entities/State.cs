using System.ComponentModel.DataAnnotations.Schema;
using Sereno.Database.Logging.TlDb1;

namespace Sereno.System.DataAccess.Entities
{
    [Table("sysState")]
    public class State : ILogging
    {
        [Column(TypeName = "nvarchar(100)")]
        public required string Id { get; set; }


        [Column(TypeName = "nvarchar(500)")]
        public string? Description { get; set; }


        public DateTime? Create { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? CreateUser { get; set; }

        public DateTime? Modify { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? ModifyUser { get; set; }



        // MasterData 

        public static readonly State Active = new() { Id = "Active", Description = "Aktiv" };
        public static readonly State Locked = new() { Id = "Locked", Description = "Gesperrt" };

        public static IEnumerable<State> All =>
        [
            Active,
            Locked, 
        ];

        public override bool Equals(object? obj) => obj is State other && Id == other.Id;
        public override int GetHashCode() => Id.GetHashCode();
        public static bool operator ==(State? a, State? b) => a?.Id == b?.Id;
        public static bool operator !=(State? a, State? b) => !(a == b);
    }
}
