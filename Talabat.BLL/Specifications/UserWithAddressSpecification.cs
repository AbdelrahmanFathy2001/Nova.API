using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Identity;

namespace Talabat.BLL.Specifications
{
    public class UserWithAddressSpecification : BaseSpecification<AppUser>
    {

        public UserWithAddressSpecification()
        {
            AddInclude(p => p.Address);
        }
        public UserWithAddressSpecification( string id ):base(p => p.Id == id)
        {
            AddInclude(p => p.Address);
        }
    }
}
