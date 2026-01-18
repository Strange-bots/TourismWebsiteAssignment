using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models;

namespace TourismWebsiteAssignment.Controllers
{
    public class PaymentTransactionsController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: PaymentTransactions
        public async Task<ActionResult> Index()
        {
            var paymentTransactions = db.PaymentTransactions.Include(p => p.Booking);
            return View(await paymentTransactions.ToListAsync());
        }

        // GET: PaymentTransactions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PaymentTransactions paymentTransactions = await db.PaymentTransactions.FindAsync(id);
            if (paymentTransactions == null)
            {
                return HttpNotFound();
            }

            return View(paymentTransactions);
        }

        // GET: PaymentTransactions/Create
        public ActionResult Create()
{
    // Get bookings with 'PendingPayment' status and load BookingStatus
    ViewBag.BookingId = new SelectList(db.Bookings.Where(b => b.BookingStatus.StatusName == "PendingPayment"), "BookingId", "SpecialStatus");
    return View();
}

// POST: PaymentTransactions/Create
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> Create([Bind(Include = "TransactionId,BookingId,TransactionDate,Amount,PaymentMethod,TransactionStatus,TransactionReference,Currency")] PaymentTransactions paymentTransactions, int Paid)
{
    if (ModelState.IsValid)
    {
        // Validate booking exists and is in "PendingPayment" state
        var booking = await db.Bookings
            .Include(b => b.BookingStatus)  // Ensure BookingStatus is loaded
            .FirstOrDefaultAsync(b => b.BookingId == paymentTransactions.BookingId);

        if (booking == null || booking.BookingStatus.StatusName != "PendingPayment")
        {
            ModelState.AddModelError("BookingId", "Invalid booking or booking is not in 'PendingPayment' state.");
            return View(paymentTransactions);
        }

        // Prevent duplicate successful payments
        bool duplicateTransaction = db.PaymentTransactions.Any(p => p.BookingId == paymentTransactions.BookingId && p.TransactionStatus == "Success");
        if (duplicateTransaction)
        {
            ModelState.AddModelError("", "This booking already has a successful payment.");
            return View(paymentTransactions);
        }

        // Update transaction details
        paymentTransactions.TransactionReference = Guid.NewGuid().ToString(); // Unique reference
        paymentTransactions.TransactionDate = DateTime.Now;
        paymentTransactions.TransactionStatus = "Success"; // Assuming the payment was successful
        paymentTransactions.Currency = "AUD"; // Example currency

        // Add payment transaction to the database
        db.PaymentTransactions.Add(paymentTransactions);
        await db.SaveChangesAsync();

        // Update booking status to 'Paid'
        booking.BookingStatus.BookingStatusId = Paid; // Update to "Paid"
        db.Entry(booking).State = EntityState.Modified;
        await db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "SpecialStatus", paymentTransactions.BookingId);
    return View(paymentTransactions);
}



        // GET: PaymentTransactions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PaymentTransactions paymentTransactions = await db.PaymentTransactions.FindAsync(id);
            if (paymentTransactions == null)
            {
                return HttpNotFound();
            }

            ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "SpecialStatus", paymentTransactions.BookingId);
            return View(paymentTransactions);
        }

        // POST: PaymentTransactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TransactionId,BookingId,TransactionDate,Amount,PaymentMethod,TransactionStatus,TransactionReference,Currency")] PaymentTransactions paymentTransactions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paymentTransactions).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "SpecialStatus", paymentTransactions.BookingId);
            return View(paymentTransactions);
        }

        // GET: PaymentTransactions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PaymentTransactions paymentTransactions = await db.PaymentTransactions.FindAsync(id);
            if (paymentTransactions == null)
            {
                return HttpNotFound();
            }

            return View(paymentTransactions);
        }

        // POST: PaymentTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PaymentTransactions paymentTransactions = await db.PaymentTransactions.FindAsync(id);
            db.PaymentTransactions.Remove(paymentTransactions);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
