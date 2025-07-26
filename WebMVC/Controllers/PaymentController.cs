using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Success(string txnRef, double amount)
        {
            ViewBag.TxnRef = txnRef;
            ViewBag.Amount = amount;
            return View();
        }
    }
}