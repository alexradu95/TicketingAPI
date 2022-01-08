using System.Collections.Generic;
using WatersTicketingAPI.Models;

namespace WatersTicketingAPI.DTO
{
    public class UserDTO
    {
        public string Username { get; set; }

        public string Role { get; set; }

        public List<Ticket> Tickets { get; set; }
    }
}
