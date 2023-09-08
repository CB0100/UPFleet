using UPFleet.Models;
using UPFleet.ViewModels;

namespace UPFleet.Repositories
{
    public interface IRepository
    {
        public List<Barge> GetBargeList();
        public List<string?> GetBargeNameList(string term);
        public List<Owner> GetOwnerList();
        public List<Transfer> GetTransferList();
        public List<Transaction> GetTransactionList();
        public double? GetTransactionCount();
        public List<Transaction> GetTransactionListforNotBilled();
        public List<Transaction> GetTransactionListforBilled();
        public List<Transaction> GetTransactionListforToBill();
        public List<PeachtreeExportedArchive> GetPeachtreeExportedArchiveList();
        public List<Location> GetLocationList();
        public List<UPFleetViewModel> GetStatusData(string Status);
        public bool AddBarge(Barge barge);
        public bool AddOwner(Owner owner);
        public bool AddTransaction(Transaction transaction);
        public bool AddTransfer(Transfer transfer, string transaction);
        public bool UpdateBarge(Barge barge);
        public bool UpdateOwner(Owner owner);
        public bool UpdateTransfer(Transfer? transfer);
        public bool UpdateTransaction(double transactionInput, string status, double Rate);
        public bool DeleteTransaction(double transactionInput);
    }
}
