using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class CustomerPointsHistoryModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public CustomerManagementModel Customer { get; set; }

        public int PointsChanged { get; set; } // Điểm tăng thêm hoặc giảm đi
        public int BalanceAfterTransaction { get; set; } // Số điểm sau giao dịch
        public DateTime TransactionDate { get; set; } // Ngày giao dịch
        public PointChangeStatus PointChangeStatus { get; set; } // Mô tả giao dịch ("Mua vé", "Sử dụng điểm thưởng")

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
