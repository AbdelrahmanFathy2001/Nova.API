using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.BLL.Specifications
{
    public class ProductSpecParams
    {

        // clean code  (GetProducts)

        private const int MaxPageSize = 50;

        public int PageIndex { get; set; } = 1;

        private int pageSize = 10;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }


        public string Sort { get; set; }

        public int? TypeId { get; set; }

        public int? BrandId { get; set; }

        private String search;

        public String Search
        {
            get { return search; }
            set { search = value.ToLower(); }
        }


    }
}
