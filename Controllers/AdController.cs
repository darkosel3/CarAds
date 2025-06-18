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

namespace CarAds.Controllers
{
    //[Authorize]
    public class AdController : Controller
    {
        private readonly IMongoCollection<Ad> _ads;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdController(IMongoDatabase database, UserManager<ApplicationUser> userManager)
        {
            _ads = database.GetCollection<Ad>("Ads");
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var allAds = _ads.Find(ad => true).ToList();
            return View(allAds);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Ad ad, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                //var userId = _userManager.GetUserId(User);
                //ad.UserId = new ObjectId(userId);
                ad.UserId = new ObjectId("000000000000000000000001");

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
            return View(ad); // DARKO Comment  -> nema potrebe da se vraca nad add car, vec samo moze da se vraca view sa trenutnim autom View(ad); 
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

            if (ModelState.IsValid)
            {
                var existingAd = await _ads.Find(c => c.Id == objectId).FirstOrDefaultAsync();
                if (existingAd == null)
                    return NotFound();

                // Update fields
                existingAd.Brand = updatedAd.Brand;
                existingAd.Model = updatedAd.Model;
                existingAd.Year = updatedAd.Year;
                existingAd.Price = updatedAd.Price;
                existingAd.Description = updatedAd.Description;


                await _ads.ReplaceOneAsync(c => c.Id == objectId, existingAd);
                return RedirectToAction("Index");
            }

            return View(updatedAd);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return BadRequest();

            var ad = await _ads.Find(c => c.Id == objectId && c.UserId == ObjectId.Parse(_userManager.GetUserId(User))).FirstOrDefaultAsync();
            if (ad == null)
                return NotFound();

            return View(ad); // Shows confirmation page
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
    }
}