using System;
using System.Threading;
using System.Threading.Tasks;
using Models.Secrets.Exceptions;
using MongoDB.Driver;

namespace Models.Secrets.Repositories
{
    public class MongoSecretRepository : ISecretRepository
    {
        private readonly IMongoCollection<Secret> secrets;

        public MongoSecretRepository()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("SampleTextDb");
            secrets = database.GetCollection<Secret>("Secrets");
        }
        
        public Task<Secret> CreateAsync(SecretCreationInfo creationInfo, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (creationInfo == null)
            {
                throw new ArgumentNullException(nameof(creationInfo));
            }

            var id = Guid.NewGuid();
            var secret = new Secret
            {
                Id = id,
                ImageUrl = creationInfo.ImageUrl,
                Description = creationInfo.Description
            };

            secrets.InsertOne(secret, cancellationToken: cancellationToken);
            return Task.FromResult(secret);
        }

        public Task<Secret> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            
            var secret = secrets.Find(item => item.Id == id).FirstOrDefault();
            
            if (secret == null)
            {
                throw new SecretNotFoundException(id.ToString());
            }

            return Task.FromResult(secret);
        }
    }
}