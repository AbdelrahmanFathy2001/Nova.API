using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Identity;

namespace Talabat.DAL.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Abdalrhman Fathy",
                    UserName = "abdalrhman fathy",
                    Email = "abdalrhmanfathy170@gmail.com",
                    PhoneNumber = "01143210112",
                    Address = new Address()
                    {
                        FirstName = "Abdalrhman",
                        LastName = "Fathy",
                        Country = "Egypt",
                        City = "Beni Suef",
                        Street = "Elabasire"
                    }
                };
                await userManager.CreateAsync(user , "Pa$$w0rd");
            }
        }
    }
}
