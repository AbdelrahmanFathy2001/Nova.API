using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.DAL.Entities.Identity
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string SenderUserId { get; set; }
        public string RecipientUserId { get; set; }
        public string Message { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ConnectionId { get; set; }
        public AppUser SenderUser { get; set; }
        public AppUser RecipientUser { get; set; }
    }
}
