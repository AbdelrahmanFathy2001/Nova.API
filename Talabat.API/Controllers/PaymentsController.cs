using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using System.IO;
using System.Threading.Tasks;
using Talabat.API.Errors;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private const string _whSecret = "whsec_8b4f0c426e384b9c6053311f6b313f7827e91d85530c46960e448212e1bb057e";

        public PaymentsController(IPaymentService paymentService , ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }
        [Authorize]
        [HttpPost("{basketId}")] //api/payments/{basketId}
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null) return BadRequest(new ApiResponse(400, "Problem With Your Basket"));

            return Ok(basket);
        }

        [HttpPost("webhook")] // api/payments/webhook
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], _whSecret);

                PaymentIntent intent;
                Order order;
                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        _logger.LogInformation("Payment Succeded ya Abdo");
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        order = await _paymentService.UpdateOrderPaymentSucceded(intent.Id);
                        break;
                    case Events.PaymentIntentPaymentFailed:
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                        _logger.LogInformation("Payment Failed ya Abdo" , order.Id);
                        _logger.LogInformation("Payment Failed ya Abdo" , intent.Id);

                        break;
                }
                return new EmptyResult();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
