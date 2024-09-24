using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Specifications;
using Talabat.DAL.Identity;

namespace Talabat.BLL.Repositories
{
    public class UserRepository<T> : IUserRepository<T> where T : class
    {
        private readonly AppIdentityDbContext context;

        public UserRepository(AppIdentityDbContext context)
        {
            this.context = context;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {

            //if (typeof(T) == typeof(Product))
            //    return (IReadOnlyList<T>) await context.Set<Product>().Include(P => P.ProductType).ToListAsync();
            return await context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
            => await context.Set<T>().FindAsync(id);



        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).ToListAsync();
            //_context.Set<Product>()
            //      .orderBy(p => p.Name)
            //      .Include(p => p.ProductType)
            //      .Include(p => p.ProductBrand)
            //      .ToListAsync()

        }

        public async Task<T> GetByIdWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).FirstOrDefaultAsync();

        }



        private IQueryable<T> ApplySpecifications(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(context.Set<T>(), spec);
        }

        public async Task<int> GetCountAsync(ISpecification<T> spec)
            => await ApplySpecifications(spec).CountAsync();


        public async Task<int> Add(T spec)
        {
            context.Set<T>().Add(spec);
            return await context.SaveChangesAsync();
        }

        public async Task<int> Update(T entity)
        {
            context.Set<T>().Update(entity);
            return await context.SaveChangesAsync();
        }

        public async Task<int> Delete(T spec)
        {
            context.Set<T>().Remove(spec);
            return await context.SaveChangesAsync();
        }
    }
}
