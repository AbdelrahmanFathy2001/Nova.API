using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.DAL.Entities.Identity
{
    public class Email
    {
        public int Id { get; set; }

        public String Title { get; set; }

        public String Body { get; set; }

        public String To { get; set; } //Receiver Email
    }
}
