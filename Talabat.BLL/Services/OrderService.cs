using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Specifications.Order_Specifications;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUniteOfWork _uniteOfWork;
        private readonly IPaymentService _paymentService;
        //private readonly IGenericRepository<Product> _productsRepo;
        //private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
        //private readonly IGenericRepository<Order> _ordersRepo;

        public OrderService(IBasketRepository basketRepository , 
            IUniteOfWork uniteOfWork ,
            IPaymentService paymentService
            //IGenericRepository<Product> productsRepo , 
            //IGenericRepository<DeliveryMethod> deliveryMethodRepo , 
            //IGenericRepository<Order> ordersRepo
            )
        {
            _basketRepository = basketRepository;
            _uniteOfWork = uniteOfWork;
            _paymentService = paymentService;
            //_productsRepo = productsRepo;
            //_deliveryMethodRepo = deliveryMethodRepo;
            //_ordersRepo = ordersRepo;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shipToAddress)
        {
            // 1- Get Basket From Basket Repo
            var basket = await _basketRepository.GetCustomerBasket(basketId);

            // 2- Get Selected Items at Basket From Product Repo
            var orderItems = new List<OrderItem>();

            foreach(var item in basket.BasketItems)
            {
                var product = await _uniteOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                var orderItem = new OrderItem(productItemOrdered , product.Price , item.Quantity);
                orderItems.Add(orderItem);
            }

            // 3- Get Delivery Method From DeliveryMethods Repo
            var deliveryMethod = await _uniteOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // 4- Calculate SubTotal
            var subTotal = orderItems.Sum(item => item.Price * item.Quantity);

            //check if Order is Existed or Not

            var spec = new OrderWithItemByPaymentIntentSpecifications(basket.PaymentIntentId);
            var existingOrder = await _uniteOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);
            if (existingOrder != null)
            {
                 _uniteOfWork.Repository<Order>().Delete(existingOrder);

                await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);
            }

            // 5- Create Order
            var order = new Order(buyerEmail , deliveryMethod, shipToAddress, orderItems ,subTotal, basket.PaymentIntentId);
            await _uniteOfWork.Repository<Order>().Add(order);

            // 6- Save TO Database

            int result = await _uniteOfWork.Complete();
            if (result <= 0) return null;


            return order;

        }


        public Task<IReadOnlyList<Order>> GetAllOrderAsync()
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecifications();

            var orders = _uniteOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            return orders;

        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _uniteOfWork.Repository<DeliveryMethod>().GetAllAsync();
        }

        public async Task<Order> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecifications(orderId , buyerEmail);
            var orders = await _uniteOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);

            return orders;
        }

        public async Task<Order> GetOrderByIdForUserDashboardAsync(int orderId)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecifications(orderId);
            var orders = await _uniteOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);

            return orders;
        }

        public async Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecifications(buyerEmail);
            var orders = await _uniteOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            return orders;
        }
    }
}
