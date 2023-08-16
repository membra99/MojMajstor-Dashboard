using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;
using Services;
using Entities.Universal.MainData;

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
        [HttpGet]
        public async Task<ActionResult<Product>> test1()
        {
            var result = _mainDataServices.test(); // call the main
            return result;
        }
    }
}