using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SellingMovieTickets.Models.Entities;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SellingMovieTickets.Repository
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<MovieModel> Movies { get; set; }
        public DbSet<CinemaShowTimeModel> CinemaShowTimes { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<SeatModel> Seats { get; set; }
        public DbSet<AppUserModel> Users { get; set; }
        public DbSet<TicketModel> Tickets { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<PromotionModel> Promotions { get; set; }
        public DbSet<OtherServicesModel> OtherServices { get; set; }
        public DbSet<OtherServicesOrderModel> OtherServicesOrders { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }
        public DbSet<MovieCategoryModel> MovieCategories { get; set; }
        public DbSet<MovieCategoryMappingModel> MovieCategoryMappings { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<AdvModel> Advs { get; set; }
        public DbSet<NewsModel> News { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public DbSet<CustomerManagementModel> CustomerManagements { get; set; }
        public DbSet<CustomerPointsModel> CustomerPoints { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Thiết lập khóa chính cho bảng MovieCategoryMapping
            modelBuilder.Entity<MovieCategoryMappingModel>()
                .HasKey(mc => new { mc.MovieId, mc.MovieCategoryId });

            // Thiết lập quan hệ nhiều-nhiều giữa Movie và MovieCategory
            modelBuilder.Entity<MovieCategoryMappingModel>()
                .HasOne(mc => mc.Movie)
                .WithMany(m => m.MovieCategoryMappings)
                .HasForeignKey(mc => mc.MovieId);

            modelBuilder.Entity<MovieCategoryMappingModel>()
                .HasOne(mc => mc.MovieCategory)
                .WithMany(c => c.MovieCategoryMappings)
                .HasForeignKey(mc => mc.MovieCategoryId);

            // Cấu hình mối quan hệ giữa SeatModel và RoomModel với hành vi xóa Cascade
            modelBuilder.Entity<SeatModel>()
                .HasOne(s => s.Room)               // Mối quan hệ một-nhiều
                .WithMany(r => r.Seats)            // Một Room có nhiều Seats
                .HasForeignKey(s => s.RoomId)      // Khóa ngoại RoomId
                .OnDelete(DeleteBehavior.Cascade); // Hành vi xóa: Cascade

            base.OnModelCreating(modelBuilder);
        }

    }
}
