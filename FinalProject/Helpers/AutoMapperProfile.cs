using AutoMapper;
using FinalProject.Data;
using FinalProject.ViewModels;

namespace FinalProject.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterVM, User>();
            CreateMap<UpdatePersonalInformationVM, User>();
            CreateMap<User, UpdatePersonalInformationVM>(); 
        }
    }
}
