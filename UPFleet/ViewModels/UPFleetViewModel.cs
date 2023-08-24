using UPFleet.Models;

namespace UPFleet.ViewModels
{
    public class UPFleetViewModel
    {
        public Barge? Barge { get; set; }
        public Owner? Owner { get; set; }
        public Transaction? Transaction { get; set; }
        public Transfer? Transfer { get; set; }
        public List<Transaction>? Transactionslist { get; init; }
        public List<Transfer?>? TransferList { get; set; }
        public List<Owner?>? OwnerList { get; set; }
        public List<Barge?>? BargeList { get; set; }
    }
}