using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;
using Services;
using Entities.Universal.MainData;
using Universal.DTO.IDTO;

namespace Universal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Test1Controller : ControllerBase
    {
        private readonly MainDataServices _mainDataServices;

        public Test1Controller(MainDataServices mainDataServices)
        {
            _mainDataServices = mainDataServices;
        }

        [Route("test1")]
        [HttpPost]
        public async Task<ActionResult<Product>> test1(ProductIDTO prod)
        {
            var result = _mainDataServices.test(prod); // call the main
            return result;
        }
    }
}