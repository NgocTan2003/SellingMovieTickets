using SellingMovieTickets.Areas.Admin.Models.Users;
using SellingMovieTickets.Models.Entities;

namespace SellingMovieTickets.Areas.Admin.Models.Accounts
{
    public class CreateAccount
    {
        public RegisterUser User { get; set; }
        public string Role { get; set; }
    }
}
