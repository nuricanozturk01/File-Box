using AutoMapper;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using Service.Services.DownloadService;

namespace FileBoxTest.DownloadTest
{
    public class DownloadTest
    {
        private readonly IDownloadService m_downloadService;

        public DownloadTest()
        {
            var context = new FileBoxDbContext();
            var userRepoDal = new UserRepositoryDal(new CrudRepository<FileboxUser, Guid>(context));
            var folderRepoDal = new FolderRepositoryDal(new CrudRepository<FileboxFolder, long>(context));

            var fileRepoDal = new FileRepositoryDal(new CrudRepository<FileboxFile, long>(context));

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FolderViewDto, FileboxFolder>();
                cfg.CreateMap<FolderViewDto, FileboxFolder>().ReverseMap();
                cfg.CreateMap<FileboxFile, FileViewDto>().ReverseMap();
            }).CreateMapper();

            m_downloadService = new DownloadService(fileRepoDal, folderRepoDal);
        }

    }
}
