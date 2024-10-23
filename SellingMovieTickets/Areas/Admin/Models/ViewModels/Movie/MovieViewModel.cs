using Microsoft.AspNetCore.Mvc.Rendering;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Duration { get; set; }
        public string MovieLanguageFormat { get; set; }
        public string MovieFormat { get; set; }
        public string Rating { get; set; }
        public string Director { get; set; }
        public string Actor { get; set; }
        public string TrailerUrl { get; set; }
        public Status Status { get; set; }
        public string? Image { get; set; }
        public string Genres { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
