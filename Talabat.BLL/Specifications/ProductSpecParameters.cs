using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.BLL.Specifications
{
    public class ProductSpecParameters
    {
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
