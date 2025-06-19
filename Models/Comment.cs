using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CarAds.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string UserId { get; set; } // ID korisnika koji je ostavio komentar 

        [BsonRequired]
        public string AdId { get; set; } // ID oglasa na koji se komentar odnosi    

        [BsonRequired]
        public string Content { get; set; } // Sadržaj komentara

        [BsonRequired]
        public DateTime CreatedAt { get; set; } // Datum i vreme kada je komentar ostavljen
    }
}
