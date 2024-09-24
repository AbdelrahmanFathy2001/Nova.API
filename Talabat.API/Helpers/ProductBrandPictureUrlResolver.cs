using AutoMapper;
using Microsoft.Extensions.Configuration;
using Talabat.API.Dots;
using Talabat.DAL.Entities;

namespace Talabat.API.Helpers
{
    public class ProductBrandPictureUrlResolver : IValueResolver<ProductBrand, ProductBrandOrTypeToReturnDto, string>
    {
        private readonly IConfiguration _configuration;

        public ProductBrandPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(ProductBrand source, ProductBrandOrTypeToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
                return $"{_configuration["ApiUrl"]}{source.PictureUrl}";
            return null;
        }
    }
}
