using SellingMovieTickets.Areas.Admin.Models.Users;
using SellingMovieTickets.Models.Entities;

namespace SellingMovieTickets.Areas.Admin.Models.Accounts
{
    public class UpdateAccount
    {
        public UpdateUser User { get; set; }
        public string Role { get; set; }
    }
}
