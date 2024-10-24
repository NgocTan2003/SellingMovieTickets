﻿using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace SellingMovieTickets.Models.Entities
{
    public class SeatModel: CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        public string SeatNumber { get; set; }
        public bool IsAvailable { get; set; }

        public int RoomId { get; set; }
        public RoomModel Room { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
