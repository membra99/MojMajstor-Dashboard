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

            CreateMap<Media, MediaODTO>();
            CreateMap<MediaIDTO, Media>();

            CreateMap<MediaType, MediaTypeODTO>();
            CreateMap<MediaTypeIDTO, MediaType>();

            CreateMap<Users, UsersODTO>()
                .ForMember(dest => dest.ImageSrc, source => source.MapFrom(m => m.Media.Src));
            CreateMap<UsersIDTO, Users>();

            CreateMap<Sale, SaleODTO>();
            CreateMap<SaleIDTO, Sale>();

            CreateMap<Seo, SaleODTO>();

            CreateMap<Order, OrderODTO>()
                 .ForMember(dest => dest.Email, source => source.MapFrom(m => m.Users.Email));
            CreateMap<OrderIDTO, Order>();

            CreateMap<Declaration, DeclarationODTO>();
            CreateMap<DeclarationIDTO, Declaration>();
        }
    }
}