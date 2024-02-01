using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;
using System.Net;
using Microsoft.AspNetCore.Cors;

namespace Universal.Admin_Controllers.AdminAPI
{
	[Route("api/[controller]")]
	[ApiController]
	[EnableCors("CorsPolicy")]
	public class OrderController : ControllerBase
	{
		private readonly MainDataServices _mainDataServices;

		public OrderController(MainDataServices mainServices)
		{
			_mainDataServices = mainServices;
		}

		[HttpPost]
		public async Task<ActionResult> PostSiteContent(OrderDetailsIDTO orderIDTO)
		{
			try
			{
				await _mainDataServices.PostOrder(orderIDTO);
				return new OkResult();
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		[HttpGet("GetAll")]
		public async Task<ActionResult<IEnumerable<OrderODTO>>> GetAllOrders()
		{
			try
			{
				return await _mainDataServices.GetAllOrder();
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		[HttpGet("GetFullOrderById")]
		public async Task<ActionResult<FullOrderODTO>> GetFullOrderByIds(int id)
		{
			try
			{
				return await _mainDataServices.GetFullOrderById(id);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		[HttpPost("CreateInvoice")]
		public async Task<ActionResult<bool>> Invoice(InvoiceIDTO htmltable)
		{
			try
			{
				return await _mainDataServices.CreateInvoice(htmltable);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		[HttpPut]
		public async Task<ActionResult<OrderODTO>> PutOrder(int id, string status)
		{
			try
			{
				return await _mainDataServices.EditOrder(id, status);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<OrderODTO>> DeleteOrders(int id)
		{
			try
			{
				return await _mainDataServices.DeleteOrder(id);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}
	}
}
