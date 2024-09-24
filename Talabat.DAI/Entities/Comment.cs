using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Identity;

namespace Talabat.DAL.Entities
{
    public class Comment:BaseEntity
    {
        public string Text { get; set; }

        public string BuyerEmail { get; set; }

        public int ProductId { get; set; }

        //public Product Product { get; set; }
    }
}
