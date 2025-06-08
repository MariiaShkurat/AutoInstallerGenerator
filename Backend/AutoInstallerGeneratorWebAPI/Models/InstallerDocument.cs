using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AutoInstallerGeneratorWebAPI.Models
{
    public class InstallerDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        [BsonElement("project")] public string ProjectName { get; set; } = "";
        public DateTime BuiltAt { get; set; }
        public string FileId { get; set; } = "";
        public string FileName { get; set; } = "";
        public string Log { get; set; } = "";
        public string Culture { get; set; } = "";
    }
}