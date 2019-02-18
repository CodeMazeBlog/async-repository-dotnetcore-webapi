using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAccountRepository:IRepositoryBase<Account>
    {
        Task<IEnumerable<Account>> AccountsByOwner(Guid ownerId);
    }
}
