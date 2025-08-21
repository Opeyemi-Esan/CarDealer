using CarDealer.Data;
using CarDealer.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.Controllers
{
    public class AdminController : Controller
    {
        private readonly CarDealerDbContext _contex;
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;

        public AdminController(CarDealerDbContext contex, Cloudinary cloudinary, IConfiguration configuration)
        {
            _contex = contex;
            _cloudinary = cloudinary;
            _configuration = configuration;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var adminUsername = _configuration["AdminCredentials:Username"];
            var adminPassword = _configuration["AdminCredentials:Password"];

            if (username == adminUsername && password == adminPassword)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("UploadCar");
            }

            ViewBag.ErrorMessage = "Invalid username or password.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            return RedirectToAction("Login");
        }

        public IActionResult UploadCar()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadCar(CarDto carDto)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            var car = new Car
            {
                Make = carDto.Make,
                Model = carDto.Model,
                Year = carDto.Year,
                Price = carDto.Price,
                Description = carDto.Description,
                CreatedAt = DateTime.Now,
            };

            if (carDto.Image != null && carDto.Image.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(carDto.Image.FileName, carDto.Image.OpenReadStream()),
                    Folder = "car_dealer"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    ModelState.AddModelError("", "Image upload failed: " + uploadResult.Error.Message);
                    return View(carDto);
                }

                car.ImageUrl = uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString();
            }

            _contex.Cars.Add(car);
            _contex.SaveChanges();
            return RedirectToAction("RetrieveCars", "Admin");
        }

        [HttpGet("/Retrieve")]
        public IActionResult RetrieveCars()
        {
            var result = _contex.Cars.ToList();
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {

            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }
            var car = _contex.Cars.Find(id);
            if (car == null)
            {
                return NotFound();
            }
            _contex.Cars.Remove(car);
            _contex.SaveChanges();
            return RedirectToAction("RetrieveCars", "Admin");
        }
    }    
}
