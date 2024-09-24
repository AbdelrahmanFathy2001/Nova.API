using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities;

namespace Talabat.BLL.Specifications
{
    public class ProductWithFiltersForCountSpecification:BaseSpecification<Product>
    {
        public ProductWithFiltersForCountSpecification(ProductSpecParams productSpecParams)
            :base(p =>
            (string.IsNullOrEmpty(productSpecParams.Search) || p.Name.ToLower().Contains(productSpecParams.Search)) && // Search
            (!productSpecParams.TypeId.HasValue || p.ProductTypeId == productSpecParams.TypeId.Value) &&  // Filteration
            (!productSpecParams.BrandId.HasValue || p.ProductBrandId == productSpecParams.BrandId.Value)
            )
        {
                
        }
    }
}
