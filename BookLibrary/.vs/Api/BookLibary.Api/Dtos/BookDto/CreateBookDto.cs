using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookLibary.Api.Dtos.BookDto
{
    public class CreateBookDto
    {
        public ObjectId Id { get; set; }


        public string Name { get; set; } = string.Empty;

        [BsonElement("Yazar")]

         public string? Yazar { get; set; }

         [BsonElement("Durum")]

         public bool Durum { get; set; }
     }
}