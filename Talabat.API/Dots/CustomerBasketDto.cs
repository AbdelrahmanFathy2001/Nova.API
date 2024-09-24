using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Talabat.DAL.Entities;

namespace Talabat.API.Dots
{
    public class CustomerBasketDto
    {
        [Required]
        public string Id { get; set; }

        public List<BasketItemDto> BasketItems { get; set; } = new List<BasketItemDto>();


        public int? DeliveryMethodId { get; set; }

        public string PaymentIntentId { get; set; }

        public string ClientSecret { get; set; }

        public decimal ShippingPrice { get; set; }

    }
}
