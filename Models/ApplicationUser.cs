using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;

namespace CarAds.Models
{
        [CollectionName("Users")]
        public class ApplicationUser:MongoIdentityUser<Guid>
        {
           
        }
}
