using System.ComponentModel.DataAnnotations.Schema;

namespace UPFleet.Models
{
    [Table("Transaction")]
    public class Transaction
    {
        public int ID { get; set; }
        [Column("Transaction")]
        public double? TransactionNo { get; set; }
        public string? Barge { get; set; }
        public string? Status { get; set; }
        public double? Rate { get; set; }
        [NotMapped]
        public string? rateShow { get; set; }
    }
}
