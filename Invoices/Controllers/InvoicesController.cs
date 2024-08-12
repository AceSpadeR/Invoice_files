using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SunridgeWEB.Controllers
{

    [Route("api/[controller]")]
	[ApiController]
	public class InvoicesController : Controller
	{
		private readonly UnitOfWork _unitOfWork;
		private readonly UserManager<Owner> _userManager;
		public IEnumerable<Invoice> invoicesWithOwners { get; private set; }

		public InvoicesController(UnitOfWork unitOfWork, UserManager<Owner> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
            invoicesWithOwners = new List<Invoice>();
		}

		[HttpGet]
		public IActionResult Get(bool viewArchived)
		{

            invoicesWithOwners = _unitOfWork.Invoice.GetAllJoin(i => true, null, i => i.Ownership.Owner);



            var invoiceDTOs = new List<InvoiceDTO>();

			foreach (var invoice in invoicesWithOwners)
			{
                var owner = invoice.Ownership.Owner;
                invoiceDTOs.Add(new InvoiceDTO
				{
					Id = invoice.Id,
					InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceTitle = invoice.InvoiceTitle,
                    OwnerFirstName = owner.OwnerFirstName,
					OwnerLastName = owner.OwnerLastName,
					InvoiceAmount = invoice.InvoiceAmount,
					InvoiceDate = invoice.InvoiceDate,
					InvoiceDueDate = invoice.InvoiceDueDate,
					InvoicePaid = invoice.InvoicePaid,
					InvoiceModifiedDate = invoice.InvoiceModifiedDate,
					InvoiceModifiedBy = invoice.InvoiceModifiedBy,

                    InvoiceNotes = invoice.InvoiceNotes

				});
			}
			ViewData["viewArchived"] = viewArchived;

			return Json(new { data = invoiceDTOs });
		}

		public class InvoiceDTO
		{
			public int Id { get; set; }

			public int InvoiceNumber { get; set; }
			public string OwnerFirstName { get; set; }
			public string OwnerLastName { get; set; }
			public decimal InvoiceAmount { get; set; }
			public DateTime InvoiceDate { get; set; }
			public DateTime InvoiceDueDate { get; set; }
			public bool InvoicePaid { get; set; }
			public DateTime InvoiceModifiedDate { get; set; }
			public string InvoiceModifiedBy { get; set; } = string.Empty;

            public string InvoiceTitle { get; set; } = string.Empty;
            public string InvoiceNotes { get; set; } = string.Empty;
			public int OwnershipId { get; set; }

		}
	}
}


