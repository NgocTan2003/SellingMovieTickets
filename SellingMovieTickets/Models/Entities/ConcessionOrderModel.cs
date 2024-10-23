using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    // bảng đơn đồ ăn/ đồ uống đi kèm khi mua vé
    public class ConcessionOrderModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }

        public int UserId { get; set; }
        public AppUserModel User { get; set; }

        public int ConcessionId { get; set; }
        public ConcessionModel Concession { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
