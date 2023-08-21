using System.ComponentModel.DataAnnotations.Schema;

namespace UPfleet.Models
{
    [Table("Owner")]
    public class Owner
    {
        public int ID { get; set; }
        [Column("Owner")]
        public string? OwnerName { get; set; }
        public string? Company { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Account { get; set; }
    }
}
