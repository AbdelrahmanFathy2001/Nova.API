using Microsoft.AspNetCore.Http;
using System;

namespace Talabat.API.Dots
{
    public class ProductBrandOrTypeToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public IFormFile Url { get; set; }
    }
}
