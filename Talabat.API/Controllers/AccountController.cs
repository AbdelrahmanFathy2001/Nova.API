using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Talabat.API.Dots;
using Talabat.API.Errors;
using Talabat.API.Extensions;
using Talabat.API.Helpers;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Entities.Identity;
using Talabat.DAL.Identity;

namespace Talabat.API.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly AppIdentityDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenServices _tokenServices;
        private readonly IMapper _mapper;

        public AccountController(AppIdentityDbContext dbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, SignInManager<AppUser> signInManager , ITokenServices tokenServices , IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _tokenServices = tokenServices;
            _mapper = mapper;
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if(user == null) return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if(!result.Succeeded)return Unauthorized(new ApiResponse(401));
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token =await _tokenServices.CreateToken(user, _userManager)
            }) ; 
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (CheckEmailExists(registerDto.Email).Result.Value)
                return BadRequest(new ApiValidationErrorResponse() { Errors = new[] { "This email is already in use !!" } });
            if (_userManager.Users.Any(u => u.PhoneNumber == registerDto.PhoneNumber))
                return BadRequest(new ApiValidationErrorResponse() { Errors = new[] { "This Phone is already in use !!" } });
            var user = new AppUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.Email.Split("@")[0],
                DisplayName = registerDto.DisplayName,
                PhoneNumber = registerDto.PhoneNumber,
                Address = new Address()
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    City = registerDto.City,
                    Country = registerDto.Country,
                    Street = registerDto.Street

                }
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if(!result.Succeeded) return BadRequest( new ApiResponse(400));
            var role = await _roleManager.FindByNameAsync(registerDto.role);
            if (role == null)
            {
                return BadRequest($"Role '{registerDto.role}' not found.");
            }
            if(registerDto.role == "seller" || registerDto.role == "buyer")
            {
                var userRole = await _userManager.AddToRoleAsync(user, registerDto.role);

            }
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token =await _tokenServices.CreateToken(user, _userManager)
            });
        }



        [HttpPost("forgetPassword")]
        public async Task<ActionResult<UserDto>> ForgetPassword(ForgetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    //var token = await _tokenServices.CreateToken(user, _userManager);
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPasswordLink = Url.Action("ResetPassword","Account",new{Token = token , Email = model.Email },Request.Scheme);
                    resetPasswordLink = resetPasswordLink.Replace("https://localhost:5001", "http://localhost:3000");
                    resetPasswordLink = resetPasswordLink.Replace("api/Account/", "");
                    var email = new Email()
                    {
                        Title = "Reset Password",
                        To = model.Email,
                        Body = resetPasswordLink
                    };
                    EmailSettings.SendEmail(email);
                    return Ok(model);
                }
                ModelState.AddModelError(string.Empty, "Email is not Exist!!");
            }
            return Ok(model);

        }

        [Route("resetpassword", Name = "ResetPassword")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                        return Ok(model);
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Ok(model);

                }
                ModelState.AddModelError(string.Empty, "This email is not existed");
            }
            return Ok(model);
        }


        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwordDto)
        {
            // Get the user 
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.FindByEmailAsync(email);

            // Check if the old password matches
            var passwordMatch = await _userManager.CheckPasswordAsync(user, passwordDto.oldPassword);
            if (!passwordMatch)
            {
                return BadRequest("The old password is incorrect.");
            }

            // Generate a new password hash
            var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, passwordDto.newPassword);

            // Update the user's password hash
            user.PasswordHash = newPasswordHash;
            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                return Ok("Password has been updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update the password.");
            }
        }


        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> DeleteUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.FindByEmailAsync(email);
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }



        [Authorize]
        [HttpGet]
        public async Task<ActionResult<AcountUserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            return Ok(new AcountUserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            });
        }


        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<UserDto>> GetUserAddress()
        {
            var user = await _userManager.FindUserWithAddressByEmailAsync(User);

            return Ok( _mapper.Map<Address , AddressDto>( user.Address));
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<UserDto>> UpdateUserAddress(AddressDto newaddress)
        {
            var user = await _userManager.FindUserWithAddressByEmailAsync(User);

            user.Address = _mapper.Map<AddressDto, Address>(newaddress);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest(new ApiValidationErrorResponse() { Errors = new[] { "an Error Occured With Updating User Address" } });

            return Ok(newaddress);
            
        }

        [Authorize]
        [HttpPut("UpdateUser")]
        public async Task<ActionResult<UpdateUserDto>> UpdateUser(UpdateUserDto userDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            user.DisplayName = userDto.DisplayName;
            user.PhoneNumber = userDto.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest(new ApiValidationErrorResponse() { Errors = new[] { "an Error Occured With Updating User " } });

            return Ok(result);

        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExists([FromQuery]string email)
        {
            return await _userManager.FindByEmailAsync(email) != null; 
        }

    }
}
