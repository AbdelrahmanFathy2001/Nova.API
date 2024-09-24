using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities;

namespace Talabat.BLL.Specifications
{
    public class SpecificationEvaluator<TEntity> where TEntity : class
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery , ISpecification<TEntity> spec)
        {
            var query = inputQuery;

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            //_context.Set<Product>()

            if(spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);

            //_context.Set<Product>().orderBy(p => p.Name)


            if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if(spec.IsPaginationEnable)
                query = query.Skip(spec.Skip).Take(spec.Take);

            query = spec.Includes.Aggregate(query, (currentQuery, include) => currentQuery.Include(include));


            //_context.Set<Product>().orderBy(p => p.Name).Include(p => p.ProductType)

            //_context.Set<Product>().orderBy(p => p.Name).Include(p => p.ProductType).Include(p => p.ProductBrand)

            return query;
        }
    }
}
