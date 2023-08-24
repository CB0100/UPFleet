using System.ComponentModel.DataAnnotations.Schema;

namespace UPFleet.Models
{
    [Table("Barge")]
    public class Barge
    {
        public int ID { get; set; }
        [Column("Barge")]
        public string? Barge_Name { get; set; }
       
        public string? Size { get; set; }
        
        public double? Rate { get; set; }
        
        public string? Owner { get; set; }
       
        public string? Description { get; set; }
    }
}
