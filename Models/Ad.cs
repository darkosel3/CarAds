using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace CarAds.Models
{
    public class Ad
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("Brand")]
        [Required(ErrorMessage = "Brand is required.")]
        public  string Brand { get; set; }

        [BsonElement("Model")]
        [Required(ErrorMessage = "Model is required.")]
        public  string Model { get; set; }

        [BsonElement("Year")]
        [Required(ErrorMessage = "Year is required.")]
        public int Year { get; set; }

        [BsonElement("Fuel")]
        [Required(ErrorMessage = "Fuel type is required.")]
        public string Fuel { get; set; }

        [BsonElement("Price")]
        [Required(ErrorMessage = "Price is required.")]
        public  decimal Price { get; set; }

        [BsonElement("Kilometers")]
        [Required(ErrorMessage = "The amount of kilometers the vehicle passed is required.")]
        public int Kilometers { get; set; }

        [BsonElement("Description")]
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [BsonElement("UserId")]
        [BsonRequired]
        public ObjectId UserId { get; set; }  // Referenca na User-a

        [BsonElement("Images")]
        public List<BsonBinaryData> Images { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("Comments")]
        public List<Comment> Comments { get; set; }

        public Ad()
        {
            Comments = new List<Comment>(); // inicijalizacija liste komenatara
            Images = new List<BsonBinaryData>(); // inicijalizacija liste slika 
            CreatedAt = DateTime.UtcNow; // postavljanje trenutnog vremena prilikom kreiranja novog automobila 
        }
    }
}
