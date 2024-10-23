using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace SellingMovieTickets.Models.Entities
{
    public class PaymentModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }

        public int TicketId { get; set; }
        public TicketModel Ticket { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
