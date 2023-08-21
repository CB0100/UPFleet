using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPfleet.Models
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
