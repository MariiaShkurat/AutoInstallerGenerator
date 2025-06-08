using AutoInstallerGeneratorWebAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;

namespace AutoInstallerGeneratorWebAPI.Infrastructure
{
    public class InstallerRepository
    {
        private readonly IMongoCollection<InstallerDocument> _meta;
        private readonly IGridFSBucket _bucket;

        public InstallerRepository(IOptions<MongoSettings> opt)
        {
            var client = new MongoClient(opt.Value.ConnectionString);
            var db = client.GetDatabase(opt.Value.Database);

            _meta = db.GetCollection<InstallerDocument>("installers");
            _bucket = new GridFSBucket(db, new GridFSBucketOptions
            {
                BucketName = "msiFiles",
                ChunkSizeBytes = 4 * 1024 * 1024
            });
        }

        public async Task<string> SaveAsync(
            Stream msi, string fileName, InstallerDocument doc)
        {
            var fileId = await _bucket.UploadFromStreamAsync(fileName, msi);
            doc.FileId = fileId.ToString();
            doc.BuiltAt = DateTime.UtcNow;

            await _meta.InsertOneAsync(doc);
            return doc.Id.ToString();
        }

        public Task<List<InstallerDocument>> GetAllAsync() =>
            _meta.Find(FilterDefinition<InstallerDocument>.Empty)
                 .SortByDescending(d => d.BuiltAt)
                 .ToListAsync();

        public Task<InstallerDocument?> GetAsync(string id) =>
            _meta.Find(d => d.Id == id).FirstOrDefaultAsync();

        public Task DownloadAsync(string fileId, Stream dest) =>
            _bucket.DownloadToStreamAsync(ObjectId.Parse(fileId), dest);
    }

}
