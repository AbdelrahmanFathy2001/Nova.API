using System;
using System.ComponentModel.DataAnnotations;

namespace Talabat.API.Dots
{
    public class BasketItemDto
    {
        [Required]
        public int Id { get; set; }

        [Required]

        public String ProductName { get; set; }

        [Required]
        [Range(0.1 , double.MaxValue,ErrorMessage ="Price Must be Greater than zero !!")]
        public decimal Price { get; set; }

        [Required]
        [Range(1 , int.MaxValue , ErrorMessage ="Quantity Must be at least One Item !!")]
        public int Quantity { get; set; }

        [Required]

        public String PictureUrl { get; set; }

        [Required]

        public String Brand { get; set; }

        [Required]

        public String Type { get; set; }

    }
}