using Microsoft.AspNetCore.Mvc;
using UPFleet.Models;
using UPFleet.Repositories;
using UPFleet.ViewModels;

namespace UPFleet.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;

        public HomeController(IRepository repository)
        {
            _repository = repository;
        }
        public IActionResult HomePage()
        {
            ViewBag.Ownerlist = _repository.GetOwnerList().Where(m => _repository.GetBargeList().Any(b => string.Compare(b.Owner, m.OwnerName, StringComparison.Ordinal) == 0)).OrderBy(m => m.OwnerName).ToList();            
            return View();
        }

        //Getting Barges list after Selecting any Owner in Home Page Filtering..
        public IActionResult GetBargesByOwner(string owner)
        {
            if (string.Compare(owner, "All", StringComparison.Ordinal) == 0)
            {
                var barges = _repository.GetBargeList().OrderBy(m => m.Barge_Name).ToList(); 
                barges.Insert(0,new Barge{Barge_Name = "Select Barge"});
                return Json(barges);
            }
            else
            {
                var barges = _repository.GetBargeList().Where(m => string.Compare(m.Owner, owner, StringComparison.Ordinal) == 0).OrderBy(m => m.Barge_Name).ToList();
                barges.Insert(0, new Barge { Barge_Name = "Select Barge" });
                return Json(barges);
            }

        }

        public IActionResult IndexPage(string? BargeName = null, double? Transactionno = null)
        {
            var bargeList = _repository.GetBargeList().OrderBy(m => m.Barge_Name).ToList();
            bargeList.Insert(0, new Barge { Barge_Name = "Select Barge" });
            ViewBag.Bargelist = bargeList;

            var locationlist=_repository.GetLocationList();
            locationlist.Insert(0, new Location { LocationName = "Location" });
            ViewBag.location = locationlist;

            var data = _repository.GetTransactionList()
                .Where(m => m.Barge == BargeName && _repository.GetTransferList().Any(tr => tr.Transaction == m.TransactionNo &&
                    (tr.From != null || tr.To != null)))
                .OrderBy(m => m.TransactionNo)
                .ToList();
            if (BargeName != null)
            {
                TempData["BargeName"] = BargeName;
                var selectedData = data.FirstOrDefault();
                if (selectedData == null)
                {
                    var viewModel = new UPFleetViewModel
                    {
                        Barge = _repository.GetBargeList().FirstOrDefault(m => m.Barge_Name == BargeName)
                    };
                    return View(viewModel);
                }
                else
                {
                    var viewModel = new UPFleetViewModel
                    {
                        TransferList = _repository.GetTransferList().Where(m => m.Transaction == selectedData.TransactionNo && (m.From != null || m.To != null)).ToList(),
                        Transaction = selectedData,
                        Transactionslist = data,
                        Barge = _repository.GetBargeList().FirstOrDefault(m => m.Barge_Name == BargeName)
                    };
                    TempData["tranactionNo"] = selectedData.TransactionNo.ToString();
                    return View(viewModel);
                }

            }
            else if (Transactionno != null)
            {
                var bargename = TempData["BargeName"]?.ToString();
                TempData["tranactionNo"] = Transactionno.ToString();
                TempData.Keep("BargeName");
                TempData.Keep("tranactionNo");
                
                if (_repository.GetTransferList().Any(m => m.Transaction == Transactionno && (m.From != null || m.To != null)))
                {
                    var viewModel = new UPFleetViewModel
                    {
                        TransferList = _repository.GetTransferList().Where(m => m.Transaction == Transactionno && (m.From != null || m.To != null)).ToList(),
                        Transaction = _repository.GetTransactionList().FirstOrDefault(m => m.TransactionNo == Transactionno),
                        Transactionslist = _repository.GetTransactionList()
                            .Where(m => m.Barge == bargename && _repository.GetTransferList().Any(tr =>
                                tr.Transaction == m.TransactionNo &&
                                (tr.From != null || tr.To != null)))
                            .OrderBy(m => m.TransactionNo)
                            .ToList(),
                        Barge = _repository.GetBargeList().FirstOrDefault(m => m.Barge_Name == bargename)
                    };
                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("IndexPage", "Home", new { BargeName = bargename });
                }
            }

            return View();
        }

    }
}