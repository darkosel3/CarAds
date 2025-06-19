using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CarAds.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System;
using System.Runtime.ConstrainedExecution;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;

namespace CarAds.Controllers
{
    [Authorize]
    public class AdController : Controller
    {
        private readonly IMongoCollection<Ad> _ads;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdController> _logger;

        public AdController(IMongoDatabase database, UserManager<ApplicationUser> userManager, ILogger<AdController> logger)
        {
            _ads = database.GetCollection<Ad>("Ads");
            _userManager = userManager;
            _logger = logger;
        }
        //existingAd.Brand = updatedAd.Brand;
        //    existingAd.Model = updatedAd.Model;
        //    existingAd.Year = updatedAd.Year;
        //    existingAd.Fuel = updatedAd.Fuel;
        //    existingAd.Price = updatedAd.Price;
            //existingAd.Kilometers = updatedAd.Kilometers;



        public async Task<IActionResult> Index(string brand, string model,string year, string fuel)
        {
            var filterBuilder = Builders<Ad>.Filter;
            var filters = new List<FilterDefinition<Ad>>(); 

            if (!string.IsNullOrEmpty(brand))
                filters.Add(filterBuilder.Eq(ad => ad.Brand, brand));

            if (!string.IsNullOrEmpty(model))
                filters.Add(filterBuilder.Eq(c => c.Model,model));

            if (!string.IsNullOrEmpty(year) && int.TryParse(year, out int yearValue))
                filters.Add(filterBuilder.Eq(c => c.Year, yearValue));

            if(!string.IsNullOrEmpty(fuel))
                filters.Add(filterBuilder.Eq(c => c.Fuel, fuel));

            var filter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;  

            var allAds = await _ads.Find(filter).ToListAsync();
            return View(allAds);
        }

        public IActionResult Create()
        {
            return View();
        }

   
        public async Task<IActionResult> Details(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return BadRequest("Neispravan ID");

            var detailedAd = await _ads.Find(a => a.Id == objectId).FirstOrDefaultAsync();

            if(detailedAd.Comments != null)
            {
                foreach(var comment in detailedAd.Comments)
                {
                    var user = await _userManager.FindByIdAsync(comment.UserId);
                    if (user != null) 
                        comment.User = user; // Popunjavanje korisnika za svaki komentar
                }
            }

            if (detailedAd == null)
                return NotFound("Ad does not exist");

            return View(detailedAd);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string adId, string commentText)
        {
            var ad = await _ads.Find(c => c.Id == new ObjectId(adId)).FirstOrDefaultAsync();
            if (ad == null)
                return NotFound("Ad not found");
            if (string.IsNullOrWhiteSpace(commentText))
                return BadRequest("Comment cannot be empty");

            var comment = new Comment
            {
                Content = commentText,
                AdId = adId,
                CreatedAt = DateTime.UtcNow,
                UserId = _userManager.GetUserId(User) // Assuming the user is logged in
            };

            ad.Comments.Add(comment);
            var result = await _ads.ReplaceOneAsync(c => c.Id == new ObjectId(adId), ad);
            if (result.ModifiedCount > 0)
            {
                return RedirectToAction("Details", new { id = adId });
            }

           return BadRequest("Failed to add comment");
        }


        [HttpPost]
        public async Task<IActionResult> Create(Ad ad, List<IFormFile> images)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            ad.UserId = userId;
            ad.CreatedAt = DateTime.UtcNow;

            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {

                if (images != null && images.Count > 0)
                {
                        var imageList = new List<BsonBinaryData>();

                        foreach (var image in images)
                        {
                            if (image.Length > 0)
                            {
                                using var ms = new MemoryStream();
                                await image.CopyToAsync(ms);
                                imageList.Add(new BsonBinaryData(ms.ToArray()));
                            }
                        }
                        ad.Images = imageList;
                  
                }

                await _ads.InsertOneAsync(ad);
                return RedirectToAction("Index");
            }
            foreach (var error in ModelState.Values)
            {
                foreach (var subError in error.Errors)
                {
                    ModelState.AddModelError(string.Empty, subError.ErrorMessage);
                }
            }
            return View(ad);
        }
        public async Task<IActionResult> Edit(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return BadRequest("Neispravan ID");

            var ad = await _ads.Find(c => c.Id == objectId).FirstOrDefaultAsync();
            if (ad == null)
                return NotFound();

            return View(ad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Ad updatedAd, List<IFormFile> newImages)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return BadRequest();


            var existingAd = await _ads.Find(c => c.Id == objectId).FirstOrDefaultAsync();
            if (existingAd == null)
                return NotFound();

            existingAd.Brand = updatedAd.Brand;
            existingAd.Model = updatedAd.Model;
            existingAd.Year = updatedAd.Year;
            existingAd.Fuel = updatedAd.Fuel;
            existingAd.Price = updatedAd.Price;
            existingAd.Kilometers = updatedAd.Kilometers;
            existingAd.Description = updatedAd.Description;


            await _ads.ReplaceOneAsync(c => c.Id == objectId, existingAd);

            return RedirectToAction("MyAds");
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return BadRequest();

            var userIdString = _userManager.GetUserId(User);
            
            var ad = await _ads.Find(c => c.Id == objectId && c.UserId == userIdString).FirstOrDefaultAsync();
            if (ad == null)
                return NotFound();

            return RedirectToAction("MyAds");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return BadRequest();

            await _ads.DeleteOneAsync(c => c.Id == objectId);
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyAds()
        {

            var userIdString = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }



            var myAds = await _ads.Find(ad => ad.UserId == userIdString).ToListAsync();

            return View(myAds);
        }
    }
}