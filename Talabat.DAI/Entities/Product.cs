using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.DAL.Entities
{
    public class Product:BaseEntity
    {
        public String Name { get; set; }

        public String Description { get; set; }

        public decimal Price { get; set; }

        public string PictureUrl { get; set; }

        public List<Image> Images { get; set; }

        public ProductBrand ProductBrand { get; set; }  // Navigational Property

        public int ProductBrandId { get; set; }

        public ProductType ProductType { get; set; }   // Navigational Property

        public int ProductTypeId { get; set; }

        public double Rating { get; set; }

        public bool Available { get; set; } = true;

        public List<Comment> comments { get; set; }

        public string productOwner { get; set; }



    }
}
