using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using UPFleet.Data;
using UPFleet.Models;
using UPFleet.ViewModels;

namespace UPFleet.Repositories
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _dbcontext;
        public Repository(ApplicationDbContext context)
        {
            _dbcontext = context;
        }

        public List<Barge> GetBargeList()
        {
            return _dbcontext.Barges.ToList();
        }
        public List<string?> GetBargeNameList(string term)
        {
            return _dbcontext.Barges.Where(b => b.Barge_Name!.Contains(term)).Select(b => b.Barge_Name).ToList();
        }
        public List<Owner> GetOwnerList()
        {
            return _dbcontext.Owners.ToList();
        }
        public List<Transfer> GetTransferList()
        {
            return _dbcontext.Transfers.Where(m => (m.From != null || m.To != null) && !string.IsNullOrEmpty(m.Status)).ToList();
        }
        public List<Transaction> GetTransactionList()
        {
            return _dbcontext.Transactions.Where(m => _dbcontext.Transfers.Any(tr => tr.Transaction == m.TransactionNo)).ToList();
        }
        public double? GetTransactionCount()
        {
            return _dbcontext.Transactions.Max(m => m.TransactionNo);
        }
        public List<Transaction> GetTransactionListforNotBilled()
        {
            return _dbcontext.Transactions.Where(m =>
                    m.Status != "Billed" && !string.IsNullOrEmpty(m.Status) && _dbcontext.Transfers.Any(t =>
                        t.Transaction != null && t.Transaction == m.TransactionNo && t.Status != "Billed" &&
                        !string.IsNullOrEmpty(t.Status)))
                .ToList();
        }
        public List<Transaction> GetTransactionListforBilled()
        {
            return _dbcontext.Transactions.Where(m =>
                    m.Status == "Billed" &&
                     _dbcontext.Transfers.Any(t => t.Transaction != null && t.Transaction == m.TransactionNo && t.Status == "Billed"))
                .ToList();
        }
        public List<Transaction> GetTransactionListforToBill()
        {
            return _dbcontext.Transactions.Where(m => _dbcontext.Transfers.Any(t => t.Transaction != null && t.Transaction == m.TransactionNo && t.Status == "To Bill"))
                .ToList();
        }
        public List<PeachtreeExportedArchive> GetPeachtreeExportedArchiveList()
        {
            return _dbcontext.peachtreeExportedArchives.ToList();
        }
        public List<Location> GetLocationList()
        {
            return _dbcontext.Locations.ToList();
        }
        public List<UPFleetViewModel> GetStatusData(string Status)
        {
            var bargeslist = GetBargeList();
            var ownerlist = GetOwnerList();
            var transactionslist = Status == "Billed" ? GetTransactionListforBilled() : GetTransactionListforNotBilled();
            var transferlist = _dbcontext.Transfers.Where(m => Status == "Billed" ? m.Status == "Billed" : m.Status != "Billed").ToList();

            var Viewmodelobj = (
                from o in ownerlist
                join b in bargeslist on o.OwnerName equals b.Owner into ownerBarges
                where o != null && ownerBarges.Any() && ownerBarges.Any(b => transactionslist.Any(t => t.Barge == b.Barge_Name))
                orderby o.OwnerName
                select new UPFleetViewModel
                {
                    Owner = o,
                    BargeList = ownerBarges.ToList(),
                    Transactionslist = (
                        from tr in transactionslist
                        join b in ownerBarges on tr.Barge equals b.Barge_Name
                        select tr
                    ).ToList(),
                    TransferList = (
                        from tr in transferlist
                        join t in transactionslist on tr.Transaction equals t.TransactionNo
                        join b in ownerBarges on t.Barge equals b.Barge_Name
                        select tr
                    ).ToList()
                }
            ).ToList();

            return Viewmodelobj;
        }


        public bool AddBarge(Barge barge)
        {
            try
            {
                _dbcontext.Barges.Add(barge);
                _dbcontext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateBarge(Barge barge)
        {
            try
            {
                var data = _dbcontext.Barges.FirstOrDefault(m => m.Barge_Name == barge.Barge_Name);
                if (data != null)
                {
                    data.Barge_Name = barge.Barge_Name;
                    data.Size = barge.Size;
                    data.Description = barge.Description;
                    data.Rate = barge.Rate;
                    data.Owner = barge.Owner;
                    _dbcontext.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateOwner(Owner owner)
        {
            try
            {
                var data = _dbcontext.Owners.FirstOrDefault(m => m.ID == owner.ID && m.OwnerName == owner.OwnerName);
                if (data != null)
                {
                    data.OwnerName = owner.OwnerName;
                    data.Company = owner.Company;
                    data.Account = owner.Account;
                    _dbcontext.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddOwner(Owner owner)
        {
            try
            {
                _dbcontext.Owners.Add(owner);
                _dbcontext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddTransaction(Transaction transaction)
        {
            try
            {
                _dbcontext.Transactions.Add(transaction);
                _dbcontext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddTransfer(Transfer transfer, string transaction)
        {
            try
            {
                Transfer? data = new();
                if (transaction != null)
                {
                    if (double.TryParse(transaction.ToString(), out var doubleValue))
                    {
                        data.Transaction = doubleValue;
                    }
                }
                else
                {
                    data.Transaction = _dbcontext.Transactions.Max(m => m.TransactionNo);
                }
                double maxTransfer = _dbcontext.Transfers.Max(m => m.TransferNO);
                data.TransferNO = maxTransfer;
                data.LocationFrom = transfer.LocationFrom;
                data.LocationTo = transfer.LocationTo;
                data.From = transfer.From;
                data.To = transfer.To;
                data.Status = transfer.Status;
                data.InsuranceDays = transfer.InsuranceDays;
                data.FromIns = transfer.FromIns;
                if (transfer is { From: not null, To: not null })
                {
                    TimeSpan duration = (TimeSpan)(transfer.To - transfer.From);
                    data.DaysIn = (int)duration.TotalDays;
                }

                if (transfer is { To: not null, FromIns: not null })
                {
                    DateTime toDateTime = transfer.To.Value;
                    if (DateTime.TryParse(transfer.FromIns, out var fromInsDateTime))
                    {
                        int insuranceDays = (int)(toDateTime - fromInsDateTime).TotalDays;
                        data.InsuranceDays = insuranceDays.ToString();
                    }
                }
                _dbcontext.Transfers.Add(data);
                _dbcontext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateTransfer(Transfer? transfer)
        {
            try
            {
                if (transfer != null)
                {
                    Transfer? data = _dbcontext.Transfers.FirstOrDefault(m => m.ID == transfer.ID);
                    if (data != null)
                    {
                        data.LocationFrom = transfer?.LocationFrom;
                        data.LocationTo = transfer?.LocationTo;
                        data.From = transfer?.From;
                        data.To = transfer?.To;
                        data.Status = transfer?.Status;
                        data.DaysIn = transfer?.DaysIn;
                        data.InsuranceDays = transfer?.InsuranceDays;
                        data.FromIns = transfer?.FromIns;
                        if (data is { From: not null, To: not null })
                        {
                            TimeSpan duration = (TimeSpan)(data.To - data.From);
                            data.DaysIn = (int)duration.TotalDays;
                        }

                        if (data is { To: not null, FromIns: not null })
                        {
                            DateTime toDateTime = data.To.Value;
                            if (DateTime.TryParse(data.FromIns, out var fromInsDateTime))
                            {
                                int insuranceDays = (int)(toDateTime - fromInsDateTime).TotalDays;
                                data.InsuranceDays = insuranceDays.ToString();
                            }
                        }
                        _dbcontext.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateTransaction(double transactionInput, string status, double Rate)
        {
            var transaction = _dbcontext.Transactions.FirstOrDefault(m => m.TransactionNo == transactionInput);
            if (transaction != null)
            {
                transaction.Status = status;
                transaction.Rate = Rate;
                _dbcontext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteTransaction(double transactionInput)
        {
            try
            {
                var transaction = _dbcontext.Transactions.FirstOrDefault(t => Equals(t.TransactionNo, (double)(transactionInput)));
                var transfers = _dbcontext.Transfers.Where(m => m.Transaction == transactionInput).ToList();
                if (transaction != null)
                {
                    foreach (var transfer in transfers)
                    {
                        _dbcontext.Transfers.Remove(transfer);
                    }
                    _dbcontext.Transactions.Remove(transaction);
                    _dbcontext.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
