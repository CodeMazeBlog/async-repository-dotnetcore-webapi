using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public Task<IEnumerable<Account>> AccountsByOwner(Guid ownerId)
        {
            return FindByConditionAync(a => a.OwnerId.Equals(ownerId));
        }
    }
}
