using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility;

namespace SunridgeWEB.Pages.Admin.Invoices
{
    public class DetailsModel : PageModel
    {
        private readonly UserManager<Owner> _userManager;
        private readonly UnitOfWork _unitOfWork;
        public Invoice objInvoice { get; set; }
        public IEnumerable<InvoiceFile> objInvoiceFileList { get; set; }
        public IEnumerable<Payment> Payments { get; set; }

        public DetailsModel(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, UserManager<Owner> userManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            objInvoice = new Invoice();
            objInvoiceFileList = new List<InvoiceFile>();
            Payments = new List<Payment>();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!User.IsInRole(SD.AdminRole))
            {
                return RedirectToPage("/Index");
            }

            objInvoice = await _unitOfWork.Invoice.GetAsync(i => i.Id == id, false, "Ownership.Owner");
            objInvoiceFileList = await _unitOfWork.InvoiceFile.GetAllAsync(f => f.InvoiceId == id && !f.InvoiceFileArchived);
            Payments = await _unitOfWork.Payment.GetAllAsync(p => p.InvoiceId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, Invoice invoice)
        {
            if (!User.IsInRole(SD.AdminRole))
            {
                return RedirectToPage("/Index");
            }

            var invoiceToUpdate = await _unitOfWork.Invoice.GetAsync(i => i.Id == id, false, "Ownership");

            if (invoiceToUpdate == null)
            {
                return NotFound();
            }

            invoiceToUpdate.InvoiceNumber = invoice.InvoiceNumber;
            invoiceToUpdate.InvoiceAmount = invoice.InvoiceAmount;
            invoiceToUpdate.InvoiceDate = invoice.InvoiceDate;
            invoiceToUpdate.InvoiceDueDate = invoice.InvoiceDueDate;
            invoiceToUpdate.InvoicePaid = invoice.InvoicePaid;
            invoiceToUpdate.InvoiceTitle = invoice.InvoiceTitle;
            invoiceToUpdate.InvoiceNotes = invoice.InvoiceNotes;

            _unitOfWork.Invoice.Update(invoiceToUpdate);
            _unitOfWork.Commit();

            return RedirectToPage("/Admin/Invoices/Details", new { id = invoiceToUpdate.Id });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!User.IsInRole(SD.AdminRole))
            {
                return RedirectToPage("/Index");
            }

            var invoiceToDelete = await _unitOfWork.Invoice.GetAsync(i => i.Id == id, false);

            if (invoiceToDelete == null)
            {
                return NotFound();
            }

            _unitOfWork.Invoice.Delete(invoiceToDelete);
            _unitOfWork.Commit();

            return RedirectToPage("/Admin/Invoices/Index");
        }
    }
}
