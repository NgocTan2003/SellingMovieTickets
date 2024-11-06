using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellingMovieTickets.Models.Entities
{
    public class OrderModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ngày đặt không được bỏ trống")]
        public DateTime OrderDate { get; set; }
        public string PaymentType { get; set; }
        public int NumberOfTickets { get; set; }
        [Column(TypeName = "decimal(10, 3)")]
        public decimal TotalAmount { get; set; }

        public int TicketId { get; set; }
        public TicketModel Ticket { get; set; }

        public int? CinemaShowTimeId { get; set; }
        public CinemaShowTimeModel CinemaShowTime { get; set; }

        public int CustomerManagementId { get; set; }
        public CustomerManagementModel CustomerManagement { get; set; }

        public int? PromotionId { get; set; }
        public PromotionModel? Promotion { get; set; }

        public int? OtherServicesOrderId { get; set; }
        public OtherServicesOrderModel? OtherServicesOrder { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<OrderDetailModel> OrderDetails { get; set; }
    }
}
