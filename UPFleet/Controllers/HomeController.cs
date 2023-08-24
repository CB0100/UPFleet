using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UPfleet.Data;
using UPfleet.Models;
using UPFleet.ViewModels;

namespace UPfleet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext context)
        {
            this._dbContext = context;
        }
        public IActionResult HomePage()
        {
            List<Owner?> owners = _dbContext.Owners.Where(m=>_dbContext.Barges.Any(b=>b.Owner==m.OwnerName)).OrderBy(m => m.OwnerName).ToList();
            ViewBag.Ownerlist = owners;
            return View();
        }

        public IActionResult GetBargesByOwner(string owner)
        {
            if (owner == "All")
            {
                var barges = _dbContext.Barges.OrderBy(m => m.Barge_Name).ToList(); ; 
                return Json(barges);
            }
            else
            {
                var barges= _dbContext.Barges.Where(m=>m.Owner==owner).OrderBy(m=>m.Barge_Name).ToList(); ; 
                return Json(barges);
            }
            
        }

        public IActionResult IndexPage(string? BargeName = null, double? Transactionno = null)
        {
            var bargeList = _dbContext.Barges.OrderBy(m => m.Barge_Name).ToList();
            bargeList.Insert(0, new Barge { Barge_Name = "Select Barge" });
            ViewBag.Bargelist = bargeList;

                var data = _dbContext.Transactions
                    .Where(m => m.Barge == BargeName && _dbContext.Transfers.Any(tr => tr.Transaction == m.TransactionNo &&
                        (tr.From != null || tr.To != null)))
                    .OrderBy(m => m.TransactionNo)
                    .ToList();
            if (BargeName != null)
            {

                var selectedData = data.FirstOrDefault();
                var viewModel = new View_Model
                {
                    TransferList = _dbContext.Transfers.Where(m => m.Transaction == selectedData.TransactionNo).ToList(),
                    Transaction = selectedData,
                    Transactionslist = data,
                    Barge = _dbContext.Barges.FirstOrDefault(m => m.Barge_Name == BargeName)
                };

                TempData["BargeName"] = BargeName;
                TempData["tranactionNo"] = selectedData.TransactionNo.ToString();
                return View(viewModel);
            }
            else if (Transactionno != null)
            {
                var bargename = TempData["BargeName"]?.ToString();
                TempData["tranactionNo"] = Transactionno.ToString();
                TempData.Keep("BargeName");
                TempData.Keep("tranactionNo");

                var viewModel = new View_Model
                {
                    TransferList = _dbContext.Transfers.Where(m => m.Transaction == Transactionno).ToList(),
                    Transaction = _dbContext.Transactions.FirstOrDefault(m => m.TransactionNo == Transactionno),
                    Transactionslist = _dbContext.Transactions
                        .Where(m => m.Barge == bargename && _dbContext.Transfers.Any(tr => tr.Transaction == m.TransactionNo &&
                            (tr.From != null || tr.To != null)))
                        .OrderBy(m => m.TransactionNo)
                        .ToList(),
                    Barge = _dbContext.Barges.FirstOrDefault(m => m.Barge_Name == bargename)
                };

                return View(viewModel);
            }

            return View();
        }

    }
}