using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.DAL.Entities
{
    public class Rating:BaseEntity
    {
        public double Value { get; set; }

        public string BuyerEmail { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}
