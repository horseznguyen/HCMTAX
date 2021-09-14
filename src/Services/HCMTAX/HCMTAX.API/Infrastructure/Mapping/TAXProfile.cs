using AutoMapper;
using HCMTAX.API.Domain.Entities;
using HCMTAX.API.ViewModels.PNNVDMKBNN;

namespace HCMTAX.API.Infrastructure.Mapping
{
    public class TAXProfile : Profile
    {
        public TAXProfile()
        {
            CreateMap<PNNVDMKBNN, PNNVDMKBNNResponseDto>();
        }
    }
}