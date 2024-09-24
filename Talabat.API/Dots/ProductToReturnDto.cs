using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Talabat.DAL.Entities;

namespace Talabat.API.Dots
{
    //DTO : Data Transfer Object
    public class ProductToReturnDto
    {
        public int Id { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public decimal Price { get; set; }

        public String PictureUrl { get; set; }

        public IFormFile Url { get; set; }

        public List<ImageToReturnDto> Images { get; set; } = new List<ImageToReturnDto>();

        public List<IFormFile> ImagesUrl { get; set; }

        public string ProductBrand { get; set; }  // Navigational Property

        public int ProductBrandId { get; set; }

        public string ProductType { get; set; }   // Navigational Property

        public int ProductTypeId { get; set; }

        public bool Available { get; set; }

        public double Rating { get; set; }

        public List<commentToReturnDto> comments { get; set; } = new List<commentToReturnDto>();

        public string productOwner { get; set; }




    }
}
