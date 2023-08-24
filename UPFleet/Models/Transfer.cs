using System.ComponentModel.DataAnnotations.Schema;

namespace UPFleet.Models
{
    [Table("Transfer")]
    public class Transfer
    {
        public int ID { get; set; }
        [Column("Transfer")]
        public double TransferNO { get; set; }
        public double? Transaction { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Status { get; set; }
        public string? FromIns { get; set; } 
        public double? DaysIn { get; set; }
        public string? InsuranceDays { get; set; }
        
    }
}
