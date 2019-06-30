using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Models.Tags.Exceptions;
using MongoDB.Driver;

namespace Models.Tags.Repositories
{
    public class MongoTagRepository : ITagRepository
    {
        private readonly IMongoCollection<Tag> tags;

        public MongoTagRepository()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("SampleTextDb");
            tags = database.GetCollection<Tag>("Tags");
        }
        
        public Task<Tag> CreateAsync(TagCreationInfo creationInfo, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (creationInfo == null)
            {
                throw new ArgumentNullException(nameof(creationInfo));
            }

            var tagWithSameId = tags.Find(item => item.Id == creationInfo.Id).FirstOrDefault();

            if (tagWithSameId != null)
            {
                throw new TagDuplicationException(creationInfo.Id);
            }

            var tag = new Tag
            {
                Id = creationInfo.Id,
                Name = creationInfo.Name
            };

            tags.InsertOne(tag, cancellationToken: cancellationToken);
            return Task.FromResult(tag);
        }

        public Task<Tag> GetAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            
            var tag = tags.Find(item => item.Id == id).FirstOrDefault();
            
            if (tag == null)
            {
                throw new TagNotFoundException(id);
            }

            return Task.FromResult(tag);
        }

        public Task<IReadOnlyList<Tag>> SearchAsync(TagSearchInfo searchInfo, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (searchInfo == null)
            {
                throw new ArgumentNullException(nameof(searchInfo));
            }
            
            var search = tags.Find(item => true).ToEnumerable();
            
            if (searchInfo.Tags != null)
            {
                search = search.Where(item => searchInfo.Tags.Contains(item.Id));
            }

            if (searchInfo.Offset != null)
            {
                search = search.Skip(searchInfo.Offset.Value);
            }

            if (searchInfo.Limit != null)
            {
                search = search.Take(searchInfo.Limit.Value);
            }
            
            var result = search.ToList();
            return Task.FromResult<IReadOnlyList<Tag>>(result);
        }

        public Task<Tag> PatchAsync(TagPatchInfo patchInfo, CancellationToken cancellationToken)
        {
            if (patchInfo == null)
            {
                throw new ArgumentNullException(nameof(patchInfo));
            }

            cancellationToken.ThrowIfCancellationRequested();
            var tag = tags.Find(item => item.Id == patchInfo.Id).FirstOrDefault();

            if (tag == null)
            {
                throw new TagNotFoundException(patchInfo.Id);
            }

            var updated = false;

            if (patchInfo.Name != null)
            {
                tag.Name = patchInfo.Name;
                updated = true;
            }
            
            if (updated)
            {
                tags.ReplaceOne(item => item.Id == patchInfo.Id, tag);
            }

            return Task.FromResult(tag);
        }

        public Task RemoveAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            cancellationToken.ThrowIfCancellationRequested();
            var deleteResult = tags.DeleteOne(type => type.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                throw new TagNotFoundException(id);
            }

            return Task.CompletedTask;
        }
    }
}