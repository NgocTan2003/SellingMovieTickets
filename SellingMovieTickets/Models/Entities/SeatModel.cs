using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace SellingMovieTickets.Models.Entities
{
    public class SeatModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        public string SeatNumber { get; set; }
        public bool IsAvailable { get; set; } // trạng thái ghế đã được mua chưa
        public bool IsHeld { get; set; } // trạng thái ghế đang được giữ
        public int HoldUntil { get; set; } // thời gian được giữ ghế

        public int CinemaShowTimeId { get; set; } 
        public CinemaShowTimeModel CinemaShowTime { get; set; }  

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
