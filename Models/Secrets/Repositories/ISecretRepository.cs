using System;
using System.Threading;
using System.Threading.Tasks;

namespace Models.Secrets.Repositories
{
    public interface ISecretRepository
    {
        Task<Secret> CreateAsync(SecretCreationInfo creationInfo, CancellationToken cancellationToken);
        Task<Secret> GetAsync(Guid id, CancellationToken cancellationToken);
    }
}