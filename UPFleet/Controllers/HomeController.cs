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
            IOrderedQueryable<Barge?> BargeList = _dbContext.Barges.OrderBy(m => m.Barge_Name);
            ViewBag.Bargelist = BargeList;
            return View();
        }
        public IActionResult IndexPage(string? BargeName = null, double? Transactionno = null)
        {
            List<Barge> BargeList = new List<Barge>(_dbContext.Barges.ToList().OrderBy(m => m.Barge_Name));
            BargeList.Insert(0, new Barge { Barge_Name = "Select Barge" });
            ViewBag.Bargelist = BargeList;
            if (BargeName != null)
            {
                View_Model viewmodelobj = new()
                {
                    Transactionslist = _dbContext.Transactions.Where(m => m.Barge == BargeName).ToList(),
                    Barge = _dbContext.Barges?.Where(m => m.Barge_Name == BargeName).FirstOrDefault()
                };

                for (int i = viewmodelobj.Transactionslist.Count - 1; i >= 0; i--)
                {
                    var datalist = _dbContext.Transfers
                        .Where(m => m.Transaction != null && m.Transaction == viewmodelobj.Transactionslist[i].TransactionNo).ToList();
                    if (datalist.Count == 0)
                    {
                        viewmodelobj.Transactionslist.RemoveAt(i);
                    }
                }

                TempData["BargeName"] = BargeName;
                return View(viewmodelobj);
            }
            else if (Transactionno != null)
            {
                string bargename = TempData["BargeName"].ToString();
                TempData["tranactionNo"] = Transactionno.ToString();
                TempData.Keep("BargeName");
                TempData.Keep("tranactionNo");
                View_Model viewmodelobj = new View_Model()
                {
                    TransferList = _dbContext.Transfers.Where(m => m.Transaction == Transactionno).ToList(),
                    Transaction = _dbContext.Transactions.FirstOrDefault(m => m.TransactionNo == Transactionno),
                    Transactionslist = _dbContext.Transactions.Where(m => m.Barge == bargename).ToList(),
                    Barge = _dbContext.Barges?.Where(m => m.Barge_Name == bargename).FirstOrDefault()
                };
                for (int i = viewmodelobj.Transactionslist.Count - 1; i >= 0; i--)
                {
                    var datalist = _dbContext.Transfers
                        .Where(m => m.Transaction == viewmodelobj.Transactionslist[i].TransactionNo).ToList();
                    if (datalist.Count == 0)
                    {
                        viewmodelobj.Transactionslist.RemoveAt(i);
                    }
                }

                return View(viewmodelobj);
            }

            return View();
        }
    }
}