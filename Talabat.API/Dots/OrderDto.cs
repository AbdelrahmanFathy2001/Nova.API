namespace Talabat.API.Dots
{
    public class OrderDto
    {
        public string BasketId { get; set; }

        public int DeliveryMethodId { get; set; }

        public AddressDto ShipTOAddress { get; set; }
    }
}
