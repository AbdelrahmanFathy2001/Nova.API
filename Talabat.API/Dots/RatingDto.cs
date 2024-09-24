namespace Talabat.API.Dots
{
    public class RatingDto
    {
        public int Id { get; set; }

        public double Value { get; set; }

        public string BuyerEmail { get; set; }

        public int ProductId { get; set; }

        public string Product { get; set; }
    }
}
