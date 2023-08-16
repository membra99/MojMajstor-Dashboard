using AutoMapper;
using Entities.Context;
using Entities.Universal.MainData;
using Universal.DTO.IDTO;

namespace Services
{
    public class MainDataServices : BaseServices
    {
        public MainDataServices(MainContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Product test(ProductIDTO prod)
        {
            
            var x = _mapper.Map<Product>(prod);
            return x;
        }
    }
}