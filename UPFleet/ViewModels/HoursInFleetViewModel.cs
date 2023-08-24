using UPFleet.Models;

namespace UPFleet.ViewModels
{
    public class HoursInFleetViewModel
    {
        public Barge GetBarge { get; set; }
        public Owner GetOwner { get; set; }
        public Transfer GetTransfer { get; set; }
        public Transaction GetTransaction { get; set; }
    }
}
