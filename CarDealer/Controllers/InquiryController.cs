using CarDealer.Data;
using CarDealer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.Controllers
{
    public class InquiryController : Controller
    {
        private readonly CarDealerDbContext _context;
        public InquiryController(CarDealerDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Inquiry>> CreateInquiry(InquiryDto inquiry)
        {
            var inquiryEntity = new Inquiry
            {
                CarId = inquiry.CarId,
                Name = inquiry.Name,
                Email = inquiry.Email,
                Message = inquiry.Message,
                CreatedAt = DateTime.UtcNow
            };
            _context.Inquiries.Add(inquiryEntity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetInquiry), 
                new { id = inquiryEntity.Id }, 
                inquiryEntity);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Inquiry>> GetInquiry(int id)
        {
            var inquiry = await _context.Inquiries.FindAsync(id);
            if (inquiry == null)
            {
                return NotFound();
            }
            return inquiry;
        }

        [HttpGet]
        [Route("/Inquiries/Create/{carId}")]
        public IActionResult Create(int carId)
        {
            return View(new Inquiry { CarId = carId });
        }

        [HttpPost]
        [Route("Inquiries/Create")]
        public async Task<IActionResult> Create(Inquiry inquiry)
        {
            if (ModelState.IsValid)
            {
                inquiry.CreatedAt = DateTime.UtcNow;
                _context.Inquiries.Add(inquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Cars", new { id = inquiry.CarId });
            }
            return View(inquiry);
        }
    }
}
