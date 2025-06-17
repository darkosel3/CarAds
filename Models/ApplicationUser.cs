using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;

namespace AutoOglasi.Models
{
        [CollectionName("Users")]
        public class ApplicationUser:MongoIdentityUser<Guid>
        {
           
        }
}
