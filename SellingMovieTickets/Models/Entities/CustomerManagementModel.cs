using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class CustomerManagementModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public AppUserModel AppUser { get; set; }

        public int TotalTicketsPurchased { get; set; } // Tổng số vé đã mua
        public decimal TotalSpent { get; set; } // Tổng số tiền đã chi tiêu
        public int CurrentPointsBalance { get; set; } // Điểm khách hàng hiện tại

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<OrderModel> OrderModels { get; set; }
        public ICollection<CustomerPointsHistoryModel> PointsHistory { get; set; }  
    }
}
