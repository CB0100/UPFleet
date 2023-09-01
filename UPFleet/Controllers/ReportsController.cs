using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPFleet.Data;
using UPFleet.Models;
using UPFleet.Repositories;
using UPFleet.ViewModels;

namespace UPFleet.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IRepository _repository;

        public ReportsController(IRepository repository)
        {
            _repository = repository;
        }

        //show owner table data 
        public IActionResult Owner_reports()
        {
            var obj = _repository.GetOwnerList();
            return View(obj);
        }

        public IActionResult Barge_By_Owner()
        {
            return View();
        }

        public IActionResult Owner_list()
        {
            var obj = _repository.GetOwnerList().Where(m => _repository.GetBargeList().Any(b => b.Owner == m.OwnerName)).OrderBy(m => m.OwnerName).ToList();
            obj.Insert(0, new Owner { OwnerName = "All" });
            return Json(obj);
        }
        public IActionResult BargeByOwner(string SelectOwner)
        {
            if (SelectOwner == "All")
            {
                var obj = _repository.GetBargeList().OrderBy(m => m.Barge_Name).ToList();

                return Json(obj);
            }
            else
            {
                var obj = _repository.GetBargeList().Where(m => m.Owner == SelectOwner).OrderBy(m => m.Barge_Name).ToList();

                return Json(obj);
            }

        }
        public IActionResult View_Exported_Archive()
        {
            var obj = _repository.GetPeachtreeExportedArchiveList();
            return View(obj);
        }

        //transfer detail table view
        public IActionResult Transfer_Details()
        {
            var Bargelist = _repository.GetBargeList();
            var Ownerlist = _repository.GetOwnerList();
            var transferlist = _repository.GetTransferList();
            var transactionlist = _repository.GetTransactionList();
            var obj = (
                from tr in transferlist
                join t in transactionlist on tr.Transaction equals t.TransactionNo
                join b in Bargelist on t.Barge equals b.Barge_Name
                join o in Ownerlist on b.Owner equals o.OwnerName
                where tr.To > new DateTime(2023, 1, 1) && tr.Status != "Billed" && !string.IsNullOrEmpty(tr.Status)
                select new HoursInFleetViewModel
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
            var bargeslist = _repository.GetBargeList();
            var transactionslist = _repository.GetTransactionList().Where(m => m.Status != "Billed").ToList();
            var transferlist = _repository.GetTransferList().Where(m => m.Status != "Billed" && (m.From != null || m.To != null)).ToList();

            var viewmodelobj = (from tr in transactionslist
                                join b in bargeslist on tr.Barge equals b.Barge_Name
                                where tr is not null
                                select new UPFleetViewModel()
                                {
                                    Barge = b,
                                    Transaction = tr,
                                    TransferList = transferlist.Where(m => m.Transaction == tr.TransactionNo).ToList()
                                }).ToList().OrderBy(m => m.Barge.Barge_Name);

            return View(viewmodelobj);
        }

        public IActionResult PreviewToExport_Page()
        {
            var bargeslist = _repository.GetBargeList();
            var transactionslist = _repository.GetTransactionList().ToList();
            var transferlist = _repository.GetTransferList().Where(m => m.Status == "To Bill" && (m.From != null || m.To != null)).ToList();

            var viewmodelobj = (
                from tr in transactionslist
                join b in bargeslist on tr.Barge equals b.Barge_Name
                select new UPFleetViewModel()
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
            var bargeslist = _repository.GetBargeList();
            var ownerlist = _repository.GetOwnerList();
            var transactionslist = _repository.GetTransactionList().Where(m =>
                    m.Status != "Billed" && !string.IsNullOrEmpty(m.Status) && _repository.GetTransferList().Any(t =>
                        t.Transaction != null && t.Transaction == m.TransactionNo && t.Status != "Billed" &&
                        !string.IsNullOrEmpty(t.Status)))
                .ToList();

            var transferlist = _repository.GetTransferList()
            .Where(m => m.Status != "Billed" &&
                        !string.IsNullOrEmpty(m.Status) &&
                        (m.From != null || m.To != null))
            .ToList();

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

            return View(Viewmodelobj);
        }

        public IActionResult Billed_TransferSummary_reportpage()
        {
            var bargeslist = _repository.GetBargeList();
            var ownerlist = _repository.GetOwnerList();
            var transactionslist = _repository.GetTransactionList().Where(m =>
                    m.Status == "Billed" &&
                    _repository.GetTransferList().Any(t => t.Transaction != null && t.Transaction == m.TransactionNo && t.Status == "Billed"))
                .ToList();

            var transferlist = _repository.GetTransferList().Where(m => m.Status == "Billed" && (m.From != null || m.To != null)).ToList();


            var Viewmodelobj = (
                from o in ownerlist
                join b in bargeslist on o.OwnerName equals b.Owner into ownerBarges
                where o != null && ownerBarges.Any() && ownerBarges.Any(b => transactionslist.Any(t => t.Barge == b.Barge_Name && _repository.GetTransferList().Any(m => m.Transaction == t.TransactionNo)))
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
            return View(Viewmodelobj);
        }
    }
}
