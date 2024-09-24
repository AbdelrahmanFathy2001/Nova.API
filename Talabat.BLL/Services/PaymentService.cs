using Microsoft.Extensions.Configuration;
using Stripe;
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
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUniteOfWork _uniteOfWork;

        public PaymentService(IConfiguration configuration , IBasketRepository basketRepository , IUniteOfWork uniteOfWork)
        {
            _configuration = configuration;
            _basketRepository = basketRepository;
            _uniteOfWork = uniteOfWork;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var basket = await _basketRepository.GetCustomerBasket(basketId);

            if (basket == null) return null;

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _uniteOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Cost;
            }

            foreach(var item in basket.BasketItems)
            {
                var product = await _uniteOfWork.Repository<DAL.Entities.Product>().GetByIdAsync(item.Id);
                if(item.Price != product.Price)
                    item.Price = product.Price;
            }

            // start create paymentIntent 

            var service = new PaymentIntentService();

            PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var option = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(basket.BasketItems.Sum(i => i.Quantity * (i.Price * 100))) + ((long)(shippingPrice * 100)),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };

                intent = await service.CreateAsync(option);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var option = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(basket.BasketItems.Sum(i => i.Quantity * (i.Price * 100))) + ((long)(shippingPrice * 100))
                };

                await service.UpdateAsync(basket.PaymentIntentId , option);
            }

            basket.ShippingPrice = shippingPrice;

            await _basketRepository.UpdateCustomerBasket(basket);

            return basket;
        }

        public async Task<Order> UpdateOrderPaymentSucceded(string paymentIntentId)
        {
            var spec = new OrderWithItemByPaymentIntentSpecifications(paymentIntentId);

            var order = await _uniteOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);

            if(order == null) return null;

            order.Status = OrderStatus.PaymentReceived;

            _uniteOfWork.Repository<Order>().Update(order);

            await _uniteOfWork.Complete();

            return order;
        }

        public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderWithItemByPaymentIntentSpecifications(paymentIntentId);

            var order = await _uniteOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);

            if (order == null) return null;

            order.Status = OrderStatus.PaymentFailed;

            _uniteOfWork.Repository<Order>().Update(order);

            await _uniteOfWork.Complete();

            return order;
        }
    }
}
