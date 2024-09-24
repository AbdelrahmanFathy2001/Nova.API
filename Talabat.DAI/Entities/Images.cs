using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.DAL.Entities
{
    public class Image:BaseEntity
    {
        public string Name { get; set; }

        public int ProductId { get; set; }
    }
}
