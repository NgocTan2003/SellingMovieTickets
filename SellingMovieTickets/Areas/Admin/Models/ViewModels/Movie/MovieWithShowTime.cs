using SellingMovieTickets.Areas.Admin.Models.ViewModels.Room;
using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie
{
    public class MovieWithShowTime
    {
        public int Id { get; set; }
        public MovieViewModel MovieVM { get; set; }
        public RoomViewModel RoomVM { get; set; }
        public DateTime StartShowTime { get; set; }
    }
}
