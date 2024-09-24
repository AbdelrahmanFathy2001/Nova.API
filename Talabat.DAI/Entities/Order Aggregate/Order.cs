using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.DAL.Entities.Order_Aggregate
{
    public class Order:BaseEntity
    {

        // Empty parameterless constructor
        public Order()
        {

        }
        public Order(string buyerEmail, DeliveryMethod deliveryMethod, Address shipToAddress, List<OrderItem> items, decimal subTotal , string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            DeliveryMethod = deliveryMethod;
            ShipToAddress = shipToAddress;
            Items = items;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; }

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

        public DeliveryMethod DeliveryMethod { get; set; }  // Navigation Property [Eager Loading]

        public Address ShipToAddress { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public List<OrderItem> Items { get; set; }  // Navigation Property  [Eager Loading]

        public String PaymentIntentId { get; set; }

        public decimal SubTotal { get; set; }

        public decimal GetTotal()
            => SubTotal + DeliveryMethod.Cost;
    }
}
