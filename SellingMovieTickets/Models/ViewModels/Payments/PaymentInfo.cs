using SellingMovieTickets.Models.Entities;

namespace SellingMovieTickets.Models.ViewModels.Payments
{
    public class PaymentInfo
    {
        public int ShowTimeId { get; set; }
        public string PaymentType { get; set; }
        public double TotalAmount { get; set; }
        public List<InforSeat> OrderDetails { get; set; }
    }

    public class InforSeat
    {
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }
    }   
}
