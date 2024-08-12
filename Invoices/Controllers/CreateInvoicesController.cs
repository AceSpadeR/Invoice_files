using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SunridgeWEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateInvoicesController : Controller
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<Owner> _userManager;
        public IEnumerable<Ownership> objOwnerships { get; private set; }



        public CreateInvoicesController(UnitOfWork unitOfWork, UserManager<Owner> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            objOwnerships = new List<Ownership>();
        }

        [HttpGet]
        public IActionResult Get()
        {

            objOwnerships = _unitOfWork.Ownership.GetAll(o => !o.OwnershipArchived, null, "Lot.LotPrefix,Owner");

            
            var ownerDTOs = new List<OwnerDTO>();

            foreach (var ownership in objOwnerships)
            {
                var owner = ownership.Owner;
                
                var lot = ownership.Lot;
                var email = ownership.Owner.Email;
                var phoneNumber = _userManager.GetPhoneNumberAsync(owner).Result;
                var mobilePhoneNumber = ownership.Owner.OwnerMobilePhone;

                var lotWithPrefix = lot.LotPrefix.Prefix + "-" + lot.LotNumber;
                var primary = "";
                if (ownership.OwnershipPrimaryOwner)
                {
                    primary = "Primary";
                }
                else
                {
                    primary = "";
                }
                if (primary == "Primary")
                {

                    ownerDTOs.Add(new OwnerDTO
                    {
                        Id = ownership.Id,
                        FirstName = owner.OwnerFirstName,
                        LastName = owner.OwnerLastName,
                        LotNumber = lotWithPrefix,
                        Email = email,
                        PhoneNumber = phoneNumber,
                        MobilePhoneNumber = mobilePhoneNumber,


                    });
                }
            }

            return Json(new { data = ownerDTOs });
        }

        public class OwnerDTO
        {
            public int? Id { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }

            public string? LotNumber { get; set; }

            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string? MobilePhoneNumber { get; set; }



        }
    }
}

