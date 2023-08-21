using UPfleet.Models;

namespace UPFleet.ViewModels
{
    public class TransferDetail_View_Model
    {
        public Barge GetBarge { get; set; }
        public Owner GetOwner { get; set; }
        public Transfer GetTransfer { get; set; }
        public Transaction GetTransaction { get; set; }
    }
}
