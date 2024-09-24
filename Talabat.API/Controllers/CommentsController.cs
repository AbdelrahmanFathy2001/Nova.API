using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Talabat.API.Dots;
using Talabat.DAL.Data;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Identity;

namespace Talabat.API.Controllers
{
    
    public class CommentsController : BaseApiController
    {

        private readonly StoreContext _context;
        private readonly SentimentsModel _model;

        //private readonly UserManager<AppUser> _userManager;

        public CommentsController(StoreContext context , SentimentsModel model)
        {
            _context = context;
            _model = model;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateComment([FromForm]CommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var comment = new Comment
            {
                Text = commentDto.Text,
                BuyerEmail = buyerEmail,
                ProductId = commentDto.ProductId
            };
            _context.Comment.Add(comment);

            var modelInput = new SentimentsModel.ModelInput() { Col0 = comment.Text};
            var result = SentimentsModel.Predict(modelInput);
            if (result.Prediction == 1)
            {
                var existingRating = _context.Rating.FirstOrDefault(r => r.ProductId == comment.ProductId && r.BuyerEmail == buyerEmail);

                if (existingRating != null)
                {
                    existingRating.Value = 4;
                    _context.Rating.Update(existingRating);
                }
                else
                {
                    var rating = new Rating
                    {
                        Value = 4,
                        ProductId = comment.ProductId,
                        BuyerEmail = buyerEmail
                    };
                    _context.Rating.Add(rating);
                }
            }
            else
            {
                var existingRating = _context.Rating.FirstOrDefault(r => r.ProductId == comment.ProductId && r.BuyerEmail == buyerEmail);

                if (existingRating != null)
                {
                    existingRating.Value = 2;
                    _context.Rating.Update(existingRating);
                }
                else
                {
                    var rating = new Rating
                    {
                        Value = 2,
                        ProductId = comment.ProductId,
                        BuyerEmail = buyerEmail
                    };
                    _context.Rating.Add(rating);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllCommentsByProductId(int productId)
        {
            var comments = _context.Comment.Where(c => c.ProductId == productId).ToList();
            if (comments != null && comments.Any())
            {
                return Ok(comments);
            }
            else
            {
                return NotFound();
            }
        }
    }
};
