using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Talabat.API.Dots;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Specifications;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Identity;
using Talabat.DAL.Identity;

namespace Talabat.API.Controllers.Dashboard
{
    public class DashboardUser : BaseApiController
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppIdentityDbContext _context;
        private readonly IUserRepository<AppUser> _userRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<Product> _productsRepo;

        public DashboardUser(UserManager<AppUser> userManager, IGenericRepository<Product> productsRepo, RoleManager<IdentityRole> roleManager, AppIdentityDbContext context, IUserRepository<AppUser> userRepo,  IMapper mapper)
        {
            _userManager = userManager;
            _productsRepo = productsRepo;
            _roleManager = roleManager;
            _context = context;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        //[Authorize(Policy = "RequireAdminRole")]
        [HttpPost("AddRole")]
        public async Task<ActionResult> AddRole(string roleName)
        {
            // Check if role already exists
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest($"Role '{roleName}' already exists.");
            }

            // Create new role
            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok($"Role '{roleName}' created successfully.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("AddUserToRole")]
        public async Task<ActionResult> AddUserToRole(string userEmail, string roleName )
        {
            // Get user and role
            var user = await _userManager.FindByEmailAsync(userEmail);
            var role = await _roleManager.FindByNameAsync(roleName);

            if (user == null)
            {
                return BadRequest($"User '{userEmail}' not found.");
            }

            if (role == null)
            {
                return BadRequest($"Role '{roleName}' not found.");
            }

            // Check if user is already in role
            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                return BadRequest($"User '{userEmail}' is already in role '{roleName}'.");
            }

            // Add user to role
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok($"User '{userEmail}' added to role '{roleName}' successfully.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


        [HttpDelete("{email}")]
        public async Task<ActionResult> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }



        [HttpGet]
        public async Task<ActionResult> GetAllUsersWithAddresses()
        {
            var spec = new UserWithAddressSpecification();
            var Users = await _userRepo.GetAllWithSpecAsync(spec);
            var Data = _mapper.Map<IReadOnlyList<AppUser>, IReadOnlyList<UserDashboardDto>>(Users);
            return Ok(Data);
        }


        [HttpGet("phoneNumber")]
        public async Task<ActionResult> GetUserByPhoneNumber(string phoneNumber)
        {
            var user = await _userManager.Users.Include(u => u.Address).SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            var Data = _mapper.Map<AppUser, UserDashboardDto>(user);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(Data);
        }

        [HttpGet("productOwner")]
        public async Task<ActionResult> GetUserByemail(string productOwner)
        {
            //if(productOwner != "nova")
            //{
                var user = await _userManager.Users.Include(u => u.Address).SingleOrDefaultAsync(u => u.Email == productOwner);
                var Data = _mapper.Map<AppUser, UserDashboardDto>(user);
                if (user == null)
                {
                    return NotFound();
                }

                var checkRole = await _userManager.IsInRoleAsync(user, "seller");
                if (checkRole)
                {
                    var spec = new ProductWithTypeAndBrandSpecification();

                    var products = await _productsRepo.GetAllWithSpecAsync(spec);

                    var Novaproduct = products.Where(p => p.productOwner == productOwner).ToList();

                    Data.products = _mapper.Map<List<Product>, List<ProductToReturnDto>>(Novaproduct);
                }
                return Ok(Data);
            //}
            //else
            //{

            //    var spec = new ProductWithTypeAndBrandSpecification();

            //    var products = await _productsRepo.GetAllWithSpecAsync(spec);

            //    var Novaproduct = products.Where(p => p.productOwner == "nova").ToList();
                 
            //    var Data = _mapper.Map<List<Product>, List<ProductToReturnDto>>(Novaproduct);
            //    return Ok(Data);

            //}

        }
    }
}
