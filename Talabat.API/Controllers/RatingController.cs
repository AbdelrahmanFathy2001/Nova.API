using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Talabat.API.Dots;
using Talabat.DAL.Data;
using Talabat.DAL.Entities;

namespace Talabat.API.Controllers
{
    [Authorize]
    public class RatingController :BaseApiController
    {

        private readonly StoreContext _context;
        //private readonly UserManager<AppUser> _userManager;

        public RatingController(StoreContext context)
        {
            _context = context;
        }

        [HttpPost("{productId}")]
        public async Task<ActionResult> CreateComment([FromRoute]int? productId, [FromForm]RatingDto ratingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var existingRating = _context.Rating.FirstOrDefault(r => r.ProductId == productId && r.BuyerEmail == buyerEmail);

            if (existingRating != null)
            {
                existingRating.Value = ratingDto.Value; 
                _context.Rating.Update(existingRating);
            }
            else
            {
                var rating = new Rating
                {
                    Value = ratingDto.Value,
                    ProductId = ratingDto.ProductId,
                    BuyerEmail = buyerEmail
                };
                _context.Rating.Add(rating);
            }
            await _context.SaveChangesAsync();

            var product = _context.Products.Find(productId);
            var ratings = _context.Rating.Where(r => r.ProductId == productId);
            if (ratings.Any())
            {
                var averageRating = ratings.Average(r => r.Value);
                var score = Math.Round(averageRating, 2);
                product.Rating = score;
                _context.SaveChanges();
            }

            return Ok();
        }
    }
}
