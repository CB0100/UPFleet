using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using UPFleet.Data;
using UPFleet.Models;
using UPFleet.ViewModels;

namespace UPFleet.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public MaintenanceController(ApplicationDbContext context)
        {
            this._dbContext = context;
        }

        public IActionResult OwnerUpdate(int Id)
        {
            int minid = _dbContext.Owners.Min(m => m.ID);
            int maxid = _dbContext.Owners.Max(m => m.ID);
            Id = (Id < minid) ? minid : Id;
            Id = (Id > maxid) ? maxid : Id;
            Owner? obj = new Owner();
            obj = _dbContext.Owners.FirstOrDefault(m => m.ID == Id);
            TempData["data"] = _dbContext.Owners.Max(m => m.ID);
            if (obj != null)
            {
                return View(obj);
            }
            else
            {
                return RedirectToAction("OwnerUpdate", new { Id = Id + 1 });
            }
        }

        [HttpPost]
        public IActionResult OwnerUpdate(Owner model)
        {
            if (ModelState.IsValid)
            {
                var data = _dbContext.Owners.FirstOrDefault(m => m.ID == model.ID && m.OwnerName == model.OwnerName);
                if (data != null)
                {
                    data.OwnerName = model.OwnerName;
                    data.Company = model.Company;
                    data.Account = model.Account;
                    _dbContext.SaveChanges();
                    return RedirectToAction("OwnerUpdate", new { Id = model.ID });
                }
                else
                {
                    Owner obj = new Owner()
                    {
                        OwnerName = model.OwnerName,
                        Company = model.Company,
                        Account = model.Account
                    };
                    _dbContext.Add(obj);
                    _dbContext.SaveChanges();
                    int maxid = _dbContext.Owners.Max(m => m.ID);
                    return RedirectToAction("OwnerUpdate", new { Id = maxid });
                }
            }
            return RedirectToAction("OwnerUpdate", new { Id = model.ID });
        }

        public ActionResult AutocompleteBarge(string term)
        {
            List<string> suggestions = new List<string>();

                suggestions = _dbContext.Barges.Where(b => b.Barge_Name.Contains(term)).Select(b => b.Barge_Name).ToList();

            return Json(suggestions);
        }
        public ActionResult AutocompleteOwner(string term)
        {
            List<string> suggestions = new List<string>();

                suggestions = _dbContext.Owners.Where(b => b.OwnerName.Contains(term)).Select(b => b.OwnerName).ToList();

            return Json(suggestions);
        }

        public ActionResult CreateBarge()
        {
            var Ownerslist = _dbContext.Owners.OrderBy(m=>m.OwnerName).ToList();
            ViewBag.message = Ownerslist;
            return View();
        }
        [HttpPost]
        public ActionResult CreateBarge(Barge? model)
        {
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    Barge obj = new Barge()
                    {
                        Barge_Name = model.Barge_Name,
                        Size = model.Size,
                        Description = model.Description,
                        Rate = model.Rate,
                        Owner = model.Owner
                    };
                    _dbContext.Add(obj);
                    _dbContext.SaveChanges();
                    return RedirectToAction("IndexPage", "Home", new { BargeName = model.Barge_Name });
                }
            }
            return View();
        }
        public IActionResult BargeUpdate(int Id)
        {
            var minid = _dbContext.Barges.Min(m => m.ID);
            var maxid = _dbContext.Barges.Max(m => m.ID);
            Id = (Id < minid) ? minid : Id;
            TempData["MaxID"] = maxid;
            Id = (Id > maxid) ? maxid : Id;
            var obj = _dbContext.Barges.FirstOrDefault(m => m.ID == Id);
            var Ownerslist = _dbContext.Owners.OrderBy(m => m.OwnerName).ToList();
            ViewBag.message = Ownerslist;
            return View(obj);
        }

        [HttpPost]
        public IActionResult BargeUpdate(Barge model)
        {
            if (ModelState.IsValid)
            {
                var data = _dbContext.Barges.FirstOrDefault(m => m.Barge_Name == model.Barge_Name);
                if (data != null)
                {
                    data.Barge_Name = model.Barge_Name;
                    data.Size = model.Size;
                    data.Description = model.Description;
                    data.Rate = model.Rate;
                    data.Owner = model.Owner;
                    _dbContext.SaveChanges();
                    return RedirectToAction("BargeUpdate", new { Id = data.ID });
                }
                else
                {
                    Barge obj = new Barge()
                    {
                        Barge_Name = model.Barge_Name,
                        Size = model.Size,
                        Description = model.Description,
                        Rate = model.Rate,
                        Owner = model.Owner
                    };
                    _dbContext.Add(obj);
                    _dbContext.SaveChanges();
                    int maxid = _dbContext.Barges.Max(m => m.ID);
                    return RedirectToAction("BargeUpdate", new { Id = maxid });
                }
            }

            return RedirectToAction("BargeUpdate", new { Id = model.ID });
        }

        public IActionResult GoBack()
        {
            var bargename = TempData["BargeName"]?.ToString();
            TempData.Keep("BargeName");
            TempData.Keep("tranactionNo");        

            return RedirectToAction("IndexPage", "Home", new { BargeName = bargename});
        }
        public IActionResult SaveTransfers()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SaveTransfers(List<UPFleetViewModel> transferlist)
        {
            var transaction = TempData["tranactionNo"]?.ToString();
            TempData.Keep("BargeName");
            TempData.Keep("tranactionNo");
            foreach (var transfer in transferlist)
            {
                if (transfer != null)
                {
                    if (transfer.Transfer?.ID == 0)
                    {
                        Transfer? data = new Transfer();
                        if (transaction != null)
                        {
                            if (double.TryParse(transaction.ToString(), out var doubleValue))
                            {
                                data.Transaction = doubleValue;
                            }
                        }
                        else
                        {
                            data.Transaction = _dbContext.Transactions.Max(m => m.TransactionNo);
                        }
                        double maxTransfer = _dbContext.Transfers.Max(m => m.TransferNO);
                        data.TransferNO = maxTransfer;
                        data.From = transfer.Transfer.From;
                        data.To = transfer.Transfer.To;
                        data.Status = transfer.Transfer.Status;
                        data.InsuranceDays = transfer.Transfer.InsuranceDays;
                        data.FromIns = transfer.Transfer.FromIns;
                        if (transfer.Transfer is { From: not null, To: not null })
                        {
                            TimeSpan duration = (TimeSpan)(transfer.Transfer.To - transfer.Transfer.From);
                            data.DaysIn = (int)duration.TotalDays;
                        }

                        if (transfer.Transfer is { To: not null, FromIns: not null })
                        {
                            DateTime toDateTime = transfer.Transfer.To.Value;
                            if (DateTime.TryParse(transfer.Transfer.FromIns, out var fromInsDateTime))
                            {
                                int insuranceDays = (int)(toDateTime - fromInsDateTime).TotalDays;
                                data.InsuranceDays = insuranceDays.ToString();
                            }
                        }
                        _dbContext.Transfers.Add(data);
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        Transfer? data =
                            _dbContext.Transfers.FirstOrDefault(m => m.ID == transfer.Transfer.ID);
                        if (data != null)
                        {
                            data.From = transfer.Transfer.From;
                            data.To = transfer.Transfer.To;
                            data.Status = transfer.Transfer.Status;
                            data.DaysIn = transfer.Transfer.DaysIn;
                            data.InsuranceDays = transfer.Transfer.InsuranceDays;
                            data.FromIns = transfer.Transfer.FromIns;
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
                            _dbContext.SaveChanges();
                        }
                    }
                }
            }

            return RedirectToAction("IndexPage", "Home", new { Transactionno = transaction });
        }
        [HttpGet]
        public IActionResult GetBargeDetails(string barge, string status)
        {
            var bargeDetails = _dbContext.Barges.FirstOrDefault(b => b.Barge_Name == barge);

            if (bargeDetails == null)
            {
                return NotFound();
            }

            var count = _dbContext.Transactions.Count(m => m.Barge == barge&&_dbContext.Transfers.Any(tr=>tr.Transaction==m.TransactionNo));
            var TransId = _dbContext.Transactions.Max(m => m.TransactionNo) + 1;
            var response = new
            {
                Rate = bargeDetails.Rate,
                Owner = bargeDetails.Owner,
                Transaction = TransId,
                record = count + 1
            };

            // Save Transaction
            Transaction data = new Transaction()
            {
                TransactionNo = TransId,
                Rate = (double?)response.Rate,
                Barge = barge,
                Status = status
            };
            _dbContext.Transactions.Add(data);
            _dbContext.SaveChanges();
            TempData["tranactionNo"] =TransId.ToString() ;
            TempData["BargeName"] = barge;
            return Json(response);
        }


        [HttpGet]
        public IActionResult GetDetails(string barge = null, string owner = null)
        {
           
           if(barge != null)
            {
                var bargeDetails = _dbContext.Barges.FirstOrDefault(b => b.Barge_Name == barge);
                if (bargeDetails != null)
                {
                    var response = new
                    {
                        bargeid = bargeDetails.ID
                    };
                    return Json(response);
                }
            }
            else if (owner!=null)
            {
                var ownerdetails = _dbContext.Owners.FirstOrDefault(b => b.OwnerName == owner);
                if (ownerdetails != null)
                {
                    var response = new
                    {
                        ownerid = ownerdetails.ID
                    };
                    return Json(response);
                }
            }
           return Json(null);
        }


            [HttpGet]
        public IActionResult Update_transaction(double transactionInput,string status, double Rate)
        {
            var transaction = _dbContext.Transactions.FirstOrDefault(m => m.TransactionNo == transactionInput);
            if (transaction != null)
            {
                transaction.Status = status;
                transaction.Rate = Rate;
                _dbContext.SaveChanges();
            }
            if (_dbContext.Transfers.Any(m => m.Transaction == transactionInput))
            {
                var response = new
                {
                    currentTransactionType = "Update"
                };
                return Json(response);
            }
            else
            {
                var response = new
                {
                    currentTransactionType = "New"
                };
                return Json(response);
            }
        }

        //delete transfer
        [HttpGet]
        public ActionResult Delete_transaction(double transactionInput)
        {
            var transaction =
                _dbContext.Transactions.FirstOrDefault(t => Equals(t.TransactionNo, (double)(transactionInput)));
            var transfers = _dbContext.Transfers.Where(m => m.Transaction == transactionInput).ToList();
            if (transaction != null)
            {
                foreach (var transfer in transfers)
                {
                    _dbContext.Transfers.Remove(transfer);
                }
                _dbContext.Transactions.Remove(transaction);
                _dbContext.SaveChanges();
            }

            var response = "Data saved Successfully.";
            return Json(response);

        }
    }
}
