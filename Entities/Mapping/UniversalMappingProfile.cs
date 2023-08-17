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
            CreateMap<Product, ProductODTO>();
            CreateMap<ProductIDTO, Product>();

            CreateMap<Categories, CategoriesODTO>();
            CreateMap<CategoriesIDTO, Categories>();

            CreateMap<ProductAttributes, ProductAttributesODTO>();
            CreateMap<ProductAttributesIDTO, ProductAttributes>();
        }
    }
}