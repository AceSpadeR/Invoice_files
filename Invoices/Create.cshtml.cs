using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Utility;

namespace SunridgeWEB.Pages.Admin.Invoices
{
    public class CreateModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<Owner> _userManager;
        private readonly IEmailSender _emailSender;

        public CreateModel(UnitOfWork unitOfWork, UserManager<Owner> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public Invoice Invoice { get; set; }

        public Payment Payment { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string ownershipIds)
        {
            List<int> selectedOwnershipIds = JsonConvert.DeserializeObject<List<int>>(ownershipIds);
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            ModelState.Remove("Invoice.Ownership");
            ModelState.Remove("Invoice.OwnershipId");

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Data Incomplete";
                return Page();
            }

            // Get the currently signed-in admin
            var currentUser = await _userManager.GetUserAsync(User);
            //var domain = "https://localhost:7152/";
            var domain = SD.domainURL;
            var invoiceUrl = $"{domain}User/Invoice/Index";
            int invoiceNumber = _unitOfWork.Invoice.GetAll().Max(i => i.InvoiceNumber) + 1;
            foreach (var ownershipId in selectedOwnershipIds)
            {

                var invoice = new Invoice
                {
                    InvoiceNumber = invoiceNumber,
                    InvoiceAmount = Invoice.InvoiceAmount,
                    InvoiceDate = DateTime.Now, // Set current date and time
                    InvoiceDueDate = Invoice.InvoiceDueDate,
                    InvoicePaid = false, // Set the InvoicePaid to false
                    InvoiceModifiedDate = DateTime.Now, // Set current date and time
                    InvoiceModifiedBy = currentUser.UserName, // Set the InvoiceModifiedBy to the currently signed-in admin
                    InvoiceTitle = Invoice.InvoiceTitle,
                    InvoiceNotes = Invoice.InvoiceNotes,
                    OwnershipId = ownershipId
                };

                _unitOfWork.Invoice.Add(invoice);
                var ownership = _unitOfWork.Ownership.Get(o => o.Id == invoice.OwnershipId, false, "Owner");
                var owner = ownership.Owner;
                invoiceNumber++;

                if (owner.Email != null && owner.Email != "") { 
                // Send email to owner 
                var emailContent = $"<h2>Sunridge</h2> </hr> <p>Dear {owner.OwnerFirstName} {owner.OwnerLastName},<br/>New Invoice has been created for your Sunridge lot. Please click on the link below to make a payment:<br/><a href='{invoiceUrl}'>Pay Now</a></p>";

                await _emailSender.SendEmailAsync(owner.Email, "Invoice Payment Link:", emailContent);
                }
            }

            _unitOfWork.Commit();
            TempData["success"] = "Invoices created and emails sent successfully";
            return RedirectToPage("./Index");
        }


    }
}
