using RepositoryLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLib.Repository.Impl
{
    public class UserRepository : IUserRepository
    {
        public IEnumerable<FileboxUser> All => throw new NotImplementedException();

        public long Count()
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync()
        {
            throw new NotImplementedException();
        }

        public void Delete(FileboxUser t)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync(FileboxUser t)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool ExistsById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxUser>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxUser> FindByFilter(Expression<Func<FileboxUser, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxUser>> FindByFilterAsync(Expression<Func<FileboxUser, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public FileboxUser FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<FileboxUser> FindByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxUser> FindByIds(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxUser>> FindByIdsAsync(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public FileboxUser Save(FileboxUser t)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxUser> Save(IEnumerable<FileboxUser> entities)
        {
            throw new NotImplementedException();
        }

        public Task<FileboxUser> SaveAsync(FileboxUser t)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxUser>> SaveAsync(IEnumerable<FileboxUser> entities)
        {
            throw new NotImplementedException();
        }
    }
}
