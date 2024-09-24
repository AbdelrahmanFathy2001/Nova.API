using AutoMapper;
using Microsoft.Extensions.Configuration;
using Talabat.API.Dots;
using Talabat.DAL.Entities;

namespace Talabat.API.Helpers
{
    public class ProductsPictureUrlResolver : IValueResolver<Product, ProductsToReturnDto, string>
    {
        private readonly IConfiguration _configuration;

        public ProductsPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Product source, ProductsToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
                return $"{_configuration["ApiUrl"]}{source.PictureUrl}";
            return null;
        }
    }
}

