using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities;

namespace Talabat.BLL.Specifications
{
    public class ProductWithTypeAndBrandSpecification:BaseSpecification<Product>
    {
        // this Constructor is used for get all products
        public ProductWithTypeAndBrandSpecification(ProductSpecParams productSpecParams)
            :base(p =>
            (string.IsNullOrEmpty(productSpecParams.Search) || p.Name.ToLower().Contains(productSpecParams.Search))&&
            (!productSpecParams.TypeId.HasValue || p.ProductTypeId == productSpecParams.TypeId.Value) &&  // Filteration
            (!productSpecParams.BrandId.HasValue || p.ProductBrandId == productSpecParams.BrandId.Value)
            )
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
            AddInclude(p => p.Images);
            AddorderBy(p => p.Name);

            ApplyPagination(productSpecParams.PageSize * (productSpecParams.PageIndex - 1), productSpecParams.PageSize);

            if (!string.IsNullOrEmpty(productSpecParams.Sort))
            {
                switch (productSpecParams.Sort)
                {
                    case "priceAsc":
                        AddorderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddorderBy(p => p.Name);
                        break;
                }
            }
        }

        public ProductWithTypeAndBrandSpecification(ProductSpecParameters productSpecParams)
            : base(p =>
            (string.IsNullOrEmpty(productSpecParams.Search) || p.Name.ToLower().Contains(productSpecParams.Search)) &&
            (!productSpecParams.TypeId.HasValue || p.ProductTypeId == productSpecParams.TypeId.Value) &&  // Filteration
            (!productSpecParams.BrandId.HasValue || p.ProductBrandId == productSpecParams.BrandId.Value)
            )
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
            AddInclude(p => p.Images);
            AddorderBy(p => p.Name);

            if (!string.IsNullOrEmpty(productSpecParams.Sort))
            {
                switch (productSpecParams.Sort)
                {
                    case "priceAsc":
                        AddorderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddorderBy(p => p.Name);
                        break;
                }
            }
        }


        // this Constructor is used for get a specifiv product With Id

        public ProductWithTypeAndBrandSpecification(int id):base(p => p.Id == id)
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
            AddInclude(p => p.Images);
            AddInclude(p => p.comments);

        }

        public ProductWithTypeAndBrandSpecification()
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
            AddInclude(p => p.Images);
            AddInclude(p => p.comments);

        }
    }
}
