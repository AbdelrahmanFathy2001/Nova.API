using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Identity;

namespace Talabat.DAL.Identity
{
    public class AppIdentityDbContext:IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options):base(options)
        {

        }

        public DbSet<Address> Address { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<UserConnection> UserConnections { get; set; }
    }
}
