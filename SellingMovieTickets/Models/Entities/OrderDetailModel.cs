using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class OrderDetailModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public OrderModel Order { get; set; }
        public string SeatNumber { get; set; } 
        public decimal Price { get; set; } 

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

}
