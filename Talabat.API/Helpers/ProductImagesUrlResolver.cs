using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Talabat.API.Dots;
using Talabat.DAL.Entities;

namespace Talabat.API.Helpers
{
    public class ProductImagesUrlResolver: IValueResolver<Product, ProductToReturnDto, List<ImageToReturnDto>>
    {
        private readonly IConfiguration _configuration;

        public ProductImagesUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<ImageToReturnDto> Resolve(Product source, ProductToReturnDto destination, List<ImageToReturnDto> destMember, ResolutionContext context)
        {
            return source.Images?.Select(img => new ImageToReturnDto
            {
                Id = img.Id,
                Name = $"{_configuration["ApiUrl"]}{img.Name}"
            }).ToList();
        }
    }
}
