using AutoMapper;
using Entities.Universal.MainData;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;

namespace Entities.Mapping
{
    public class UniversalMappingProfile : Profile
    {
        public UniversalMappingProfile()
        {
            CreateMap<Product, ProductIDTO>();
            CreateMap<ProductODTO, Product>();
        }
    }
}
