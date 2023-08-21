using Microsoft.AspNetCore.Mvc;
using UPfleet.Data;
using UPFleet.ViewModels;

namespace UPfleet.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ReportsController(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        //show owner table data 
        public IActionResult Owner_reports()
        {
            var obj = _dbContext.Owners.ToList();
            return View(obj);
        }
        public IActionResult Barge_By_Owner()
        {
            var obj = _dbContext.Barges.ToList();

            return View(obj);
        }


        public IActionResult View_Exported_Archive()
        {
            var obj = _dbContext.peachtreeExportedArchives.ToList();
            return View(obj);
        }

        //transfer detail table view
        public IActionResult Transfer_Details()
        {
            var Bargelist = _dbContext.Barges.ToList();
            var Ownerlist = _dbContext.Owners.ToList();
            var transferlist = _dbContext.Transfers.ToList();
            var transactionlist = _dbContext.Transactions.ToList();
            var obj = (
                from tr in transferlist
                join t in transactionlist on tr.Transaction equals t.TransactionNo
                join b in Bargelist on t.Barge equals b.Barge_Name
                join o in Ownerlist on b.Owner equals o.OwnerName
                select new TransferDetail_View_Model
                {
                    GetBarge = b,
                    GetOwner = o,
                    GetTransaction = t,
                    GetTransfer = tr
                }
            ).ToList();
            return View(obj);
        }
        public IActionResult TransferSummary()
        {
            var bargeslist = _dbContext.Barges.ToList();
            var transactionslist = _dbContext.Transactions.Where(m => m.Status != "Billed").ToList();
            var transferlist = _dbContext.Transfers.Where(m => m.Status != "Billed" && (m.From != null || m.To != null)).ToList();

            var viewmodelobj = (from tr in transactionslist
                                join b in bargeslist on tr.Barge equals b.Barge_Name
                                where tr is not null
                                select new View_Model()
                                {
                                    Barge = b,
                                    Transaction = tr,
                                    TransferList = transferlist.Where(m => m.Transaction == tr.TransactionNo).ToList()
                                }).ToList().OrderBy(m => m.Barge.Barge_Name);

            return View(viewmodelobj);
        }

        public IActionResult PreviewToExport_Page()
        {
            var bargeslist = _dbContext.Barges.ToList();
            var transactionslist = _dbContext.Transactions.ToList();
            var transferlist = _dbContext.Transfers.Where(m => m.Status == "To Bill" && (m.From != null || m.To != null)).ToList();

            var viewmodelobj = (
                from tr in transactionslist
                join b in bargeslist on tr.Barge equals b.Barge_Name
                select new View_Model()
                {
                    Barge = b,
                    Transaction = tr,
                    TransferList = transferlist.Where(m => m.Transaction == tr.TransactionNo).ToList()
                }
            ).ToList();

            return View(viewmodelobj);

        }

        public IActionResult Not_Billed_TransferSummary_reportpage()
        {
            var bargeslist = _dbContext.Barges.ToList();
            var ownerlist = _dbContext.Owners.ToList();
            var transactionslist = _dbContext.Transactions.Where(m =>
                    m.Status != "Billed" && !string.IsNullOrEmpty(m.Status) && _dbContext.Transfers.Any(t =>
                        t.Transaction != null && t.Transaction == m.TransactionNo && t.Status != "Billed" &&
                        !string.IsNullOrEmpty(t.Status)))
                .ToList();

            var transferlist = _dbContext.Transfers
            .Where(m => m.Status != "Billed" &&
                        !string.IsNullOrEmpty(m.Status) &&
                        (m.From != null || m.To != null))
            .ToList();

            var Viewmodelobj = (
                from o in ownerlist
                join b in bargeslist on o.OwnerName equals b.Owner into ownerBarges
                where o != null && ownerBarges.Any() && ownerBarges.Any(b => transactionslist.Any(t => t.Barge == b.Barge_Name))
                orderby o.OwnerName
                select new View_Model
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

            return View(Viewmodelobj);
        }

        public IActionResult Billed_TransferSummary_reportpage()
        {
            var bargeslist = _dbContext.Barges.ToList();
            var ownerlist = _dbContext.Owners.ToList();
            var transactionslist = _dbContext.Transactions.Where(m =>
                    m.Status == "Billed" &&
                    _dbContext.Transfers.Any(t => t.Transaction != null && t.Transaction == m.TransactionNo && t.Status == "Billed"))
                .ToList();

            var transferlist = _dbContext.Transfers.Where(m => m.Status == "Billed" && (m.From != null || m.To != null)).ToList();


            var Viewmodelobj = (
                from o in ownerlist
                join b in bargeslist on o.OwnerName equals b.Owner into ownerBarges
                where o != null && ownerBarges.Any() && ownerBarges.Any(b => transactionslist.Any(t => t.Barge == b.Barge_Name && _dbContext.Transfers.Any(m => m.Transaction == t.TransactionNo)))
                orderby o.OwnerName
                select new View_Model
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
            return View(Viewmodelobj);
        }
    }
}
