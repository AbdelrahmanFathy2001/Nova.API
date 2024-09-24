using System.Collections.Generic;
using Talabat.DAL.Entities.Identity;

namespace Talabat.API.Dots
{
    public class UserDashboardDto
    {
        public string UserId { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public AddressDto Address { get; set; }

        public List<ProductToReturnDto> products { get; set; } = new List<ProductToReturnDto>();


    }
}
