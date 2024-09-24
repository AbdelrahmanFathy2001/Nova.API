 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shipToAddress);

        Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail);

        Task<IReadOnlyList<Order>> GetAllOrderAsync();

        Task<Order> GetOrderByIdForUserAsync(int orderId, string buyerEmail);

        Task<Order> GetOrderByIdForUserDashboardAsync(int orderId);

        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();

    }
}
