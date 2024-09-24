using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Talabat.API.Dots;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Specifications;
using Talabat.DAL.Data;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.API.Controllers.Dashboard
{
    public class DashboardOrder : BaseApiController
    {

        private readonly IOrderService _orderService;
        private readonly StoreContext _context;
        private readonly IMapper _mapper;

        public DashboardOrder(IOrderService orderService, StoreContext context, IMapper mapper )
        {
            _orderService = orderService;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]  // api/orders/deliveryMethods
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrder()
        {
            var orders = await _orderService.GetAllOrderAsync();
            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [HttpGet("GetOrdersForUser")] // api/orders
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser(string buyerEmail)
        {
            var orders = await _orderService.GetOrderForUserAsync(buyerEmail);
            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [HttpGet("{id}")] // api/orders/{id}
        public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser([FromRoute] int id)
        {
            var order = await _orderService.GetOrderByIdForUserDashboardAsync(id);
            return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }
            // Check if the order is more than a day old
            var timeDifference = DateTime.Now - order.OrderDate;
            if (timeDifference.TotalDays > 1)
            {
                return BadRequest("You cannot delete an order that is more than a day old.");
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
