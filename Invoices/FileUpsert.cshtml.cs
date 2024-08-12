using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utility;

namespace SunridgeWEB.Pages.Admin.Invoices
{
	public class FileUpsertModel : PageModel
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly UnitOfWork _unitOfWork;
		private readonly UserManager<Owner> _userManager;

		[BindProperty]
		public Invoice objInvoice { get; set; }

		public IEnumerable<InvoiceFile> objInvoiceFileList { get; set; }

		public string? objInvoiceFileComment { get; set; }

		public InvoiceFile objInvoiceFile { get; set; }

		public FileUpsertModel(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, UserManager<Owner> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_webHostEnvironment = webHostEnvironment;
			objInvoice = new Invoice();
			objInvoiceFileList = new List<InvoiceFile>();
			objInvoiceFile = new InvoiceFile();
		}

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if ((!User.IsInRole(SD.AdminRole)) && (!User.IsInRole(SD.OwnerRole)))
			{
				return RedirectToPage("/Index");
			}

			var user = await _userManager.GetUserAsync(User);

			if (string.IsNullOrWhiteSpace(user.OwnerStreetAddress))
			{
				// Street address is null, empty, or consists only of white-space characters
				return RedirectToPage("../OwnerInformation/Index");
			}

			// Retrieve Invoice and InvoiceFile data from database
			objInvoice = _unitOfWork.Invoice.Get(i => i.Id == id, false, "Ownership.Owner");
			objInvoiceFileList = _unitOfWork.InvoiceFile.GetAll(f => f.InvoiceId == id && !f.InvoiceFileArchived);

			if (objInvoice == null)
			{
				return NotFound();
			}

			return Page();
		}

		public IActionResult OnPost(int? id)
		{
			//determine Root Path of wwwroot
			string webRootPath = _webHostEnvironment.WebRootPath;

			string successMessage = "No new files added";

			//retrieve the files (this is an array)
			var files = HttpContext.Request.Form.Files;

			string? Comment = HttpContext.Request.Form["objInvoiceFileComment"];

			// A file exists and the delete button was clicked
			if (id != 0)
			{
				// Get the InvoiceFile object
				InvoiceFile fileToRemove = _unitOfWork.InvoiceFile.Get(f => f.Id == id);
				// Set the InvoiceFileArchived attribute to True
				fileToRemove.InvoiceFileArchived = true;

				// update the database
				_unitOfWork.InvoiceFile.Update(fileToRemove);

				successMessage = "File Removed";
			}

			// Adding new invoice files
			else
			{
				// were there files uploaded?
				if (files.Count > 0)
				{
					int counter = 0;
					foreach (var file in files)
					{
						InvoiceFile newFile = new InvoiceFile();
						// create a unique identifier for the image name
						string fileName = Guid.NewGuid().ToString();

						// create a variable to hold a path to the \Invoice\ folder
						var uploads = Path.Combine(webRootPath, @"Invoice\");

						// get and preserve the extension type
						var extension = Path.GetExtension(files[counter].FileName);

						// create full path of the file to stream
						var fullPath = uploads + fileName + extension;

						// stream the binary files to the server
						using var fileStream = System.IO.File.Create(fullPath);
						files[counter].CopyTo(fileStream);

						// assign URL path and save to database
						newFile.InvoiceFileURL = @"\Invoice\" + fileName + extension;
						newFile.InvoiceFileAccessLevel = "User";
						newFile.InvoiceFileArchived = false;
						newFile.InvoiceFileType = extension;
						newFile.InvoiceFileUploadDate = DateTime.Now;
						newFile.InvoiceFileDescription = files[counter].FileName;
						newFile.InvoiceFileComment = Comment;
						newFile.InvoiceId = objInvoice.Id;

						//add file to database
						_unitOfWork.InvoiceFile.Add(newFile);

						counter++;
					}
					successMessage = "Files Added Successfully";
				}
			}

			// Save changes to database
			_unitOfWork.Commit();
			TempData["success"] = successMessage;

			return RedirectToPage("/Admin/Invoices/Details", new { id = objInvoice.Id });
		}
	}
}
