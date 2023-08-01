using AutoMapper;
using RepositoryLib.DTO;
using RepositoryLib.Models;

namespace File_Box_App.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig() 
        {
            CreateMap<FolderViewDto, FileboxFolder>();   
            CreateMap<FolderViewDto, FileboxFolder>().ReverseMap();
            CreateMap<FileboxFile, FileViewDto>().ReverseMap();
           
            
        }
    }
}
