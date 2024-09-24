using Microsoft.AspNetCore.Http;
using System;

namespace Talabat.API.Dots
{
    public class ProductsToReturnDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string PictureUrl { get; set; }

        public IFormFile Url { get; set; }

        public bool Available { get; set; }

        public double Rating { get; set; }

        public string ProductBrand { get; set; }  // Navigational Property

        public int ProductBrandId { get; set; }

        public string ProductType { get; set; }   // Navigational Property

        public int ProductTypeId { get; set; }

        public string productOwner { get; set; }

    }
}
