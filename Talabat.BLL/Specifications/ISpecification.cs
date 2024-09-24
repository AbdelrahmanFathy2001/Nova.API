using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.BLL.Specifications
{
    //   ده عباره عن انترفيث ال جواه تعريف ال فانكشن ال ممكن استخدمها لل كويري ال انا ببنيه
    public interface ISpecification<T>  // هنا بعرف الحاجات ال ممكن استخدمها 
    {
        public Expression<Func<T , bool>> Criteria { get; set; }
          
        public List<Expression<Func<T , object>>> Includes { get; set; }

        public Expression<Func<T , object>> OrderBy { get; set; } // p => p.Name  p => p.Price 

        public Expression<Func<T , object>> OrderByDescending { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }

        public bool IsPaginationEnable { get; set; }

    }
}
