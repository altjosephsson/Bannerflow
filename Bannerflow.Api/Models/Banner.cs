using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bannerflow.Api.Models
{
    public class Banner
    {
        [BsonId]
        public ObjectId InternalId { get; set; }
        public Guid Id { get; set; }
        public string Html { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
