using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RepositoryLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLib.Repository.Impl
{
    public class FolderRepository : IFolderRepository
    {
        private readonly FileBoxDbContext m_dbContext;

        public FolderRepository(FileBoxDbContext dbContext)
        {
            m_dbContext = dbContext;
        }
        public IEnumerable<FileboxFolder> All => throw new NotImplementedException();

        public void InsertFolder(long parentFolder, Guid userId, string folderName, string folderPath)
        {
            var parentFolderParam = new SqlParameter("@parent_folder", parentFolder);
            var userIdParam = new SqlParameter("@user_id", userId);
            var folderNameParam = new SqlParameter("@folder_name", folderName);
            var folderPathParam = new SqlParameter("@folder_path", folderPath);


            m_dbContext.InsertFolderProcedure.FromSqlRaw(
                "exec filebox_insert_folder @parent_folder, @user_id, @folder_name, @folder_path",
                parentFolderParam, userIdParam, folderNameParam, folderPathParam);
        }
        public long Count()
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync()
        {
            throw new NotImplementedException();
        }

        public void Delete(FileboxFolder t)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync(FileboxFolder t)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(long id)
        {
            throw new NotImplementedException();
        }

        public void DeleteByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public bool ExistsById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxFolder>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxFolder> FindByFilter(Expression<Func<FileboxFolder, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxFolder>> FindByFilterAsync(Expression<Func<FileboxFolder, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public FileboxFolder FindById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<FileboxFolder> FindByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxFolder> FindByIds(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxFolder>> FindByIdsAsync(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }

        public FileboxFolder Save(FileboxFolder t)
        {
            return m_dbContext.Add(t).Entity;
        }

        public IEnumerable<FileboxFolder> Save(IEnumerable<FileboxFolder> entities)
        {
            throw new NotImplementedException();
        }

        public Task<FileboxFolder> SaveAsync(FileboxFolder t)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxFolder>> SaveAsync(IEnumerable<FileboxFolder> entities)
        {
            throw new NotImplementedException();
        }
    }
}
