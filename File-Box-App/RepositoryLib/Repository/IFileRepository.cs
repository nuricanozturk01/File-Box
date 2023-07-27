using RepositoryLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLib.Repository
{
    public interface IFileRepository : ICrudRepository<FileboxFile, long>
    {
    }
}
