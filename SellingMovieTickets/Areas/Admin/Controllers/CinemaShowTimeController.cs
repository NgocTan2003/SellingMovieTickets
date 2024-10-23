﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.CinemaShowTime;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;
using System.Security.Claims;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CinemaShowTimeController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CinemaShowTimeController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            var cinemaShowTimes = _context.CinemaShowTimes.Include(x => x.Room).Include(x => x.Movie).OrderByDescending(x => x.Id).AsQueryable();
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                cinemaShowTimes = cinemaShowTimes.Where(x =>
                    (x.Room.RoomNumber.Contains(searchText) ||
                    x.Movie.Name.Contains(searchText) ||
                    x.StartShowTime.Equals(searchText)));
            }

            int recsCount = cinemaShowTimes.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = cinemaShowTimes.Skip(resSkip).Take(pager.PageSize).ToList();

            var CinemaShowTimeViewModel = data.Select(x => new CinemaShowTimeViewModel
            {
                Id = x.Id,
                StartShowTime = x.StartShowTime,
                EndShowTime = x.EndShowTime,
                NumberRoom = x.Room.RoomNumber,
                NameMovie = x.Movie.Name,
                CreateBy = x.CreateBy,
                CreateDate = x.CreateDate,
                ModifiedBy = x.ModifiedBy,
                ModifiedDate = x.ModifiedDate
            }).ToList();

            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(CinemaShowTimeViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var movies = await _context.Movies
                .Select(m => new { m.Id, m.Name })
                .ToListAsync();

            var rooms = await _context.Rooms
                .Select(r => new { r.Id, r.RoomNumber })
                .ToListAsync();

            var viewModel = new CreateCinemaShowTime
            {
                MovieIds = new SelectList(movies, "Id", "Name"),
                NumberRooms = new SelectList(rooms, "Id", "RoomNumber")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCinemaShowTime cinemaShowTime)
        {
            var movies = await _context.Movies.Select(m => new { m.Id, m.Name }).ToListAsync();
            var rooms = await _context.Rooms.Select(r => new { r.Id, r.RoomNumber }).ToListAsync();
            cinemaShowTime.MovieIds = new SelectList(movies, "Id", "Name");
            cinemaShowTime.NumberRooms = new SelectList(rooms, "Id", "RoomNumber");

            if (ModelState.IsValid)
            {
                var selectedMovie = await _context.Movies.FindAsync(cinemaShowTime.SelectedMovieId);
                if (selectedMovie == null)
                {
                    TempData["Error"] = "Phim đã chọn không tồn tại.";
                    return View(cinemaShowTime);
                }

                DateTime endShowTime = cinemaShowTime.StartShowTime.AddMinutes(selectedMovie.Duration);
                // Kiểm tra xem có suất chiếu nào trùng thời gian trong cùng phòng hay không
                var overlappingShowTimes = await _context.CinemaShowTimes
                    .Where(s => s.RoomId == cinemaShowTime.SelectedNumberRoomId &&
                                s.StartShowTime < endShowTime &&  // Thời gian bắt đầu của suất chiếu mới sau khi kết thúc của suất chiếu hiện tại
                                s.StartShowTime.AddMinutes(s.Movie.Duration) > cinemaShowTime.StartShowTime)  // Thời gian kết thúc của suất chiếu hiện tại sau khi bắt đầu của suất chiếu mới
                    .ToListAsync();

                if (overlappingShowTimes.Any())
                {
                    TempData["Error"] = "Phòng đã có suất chiếu vào thời gian này. Vui lòng chọn phòng hoặc thời gian khác.";
                    return View(cinemaShowTime);
                }
                else
                {
                    var showTime = new CinemaShowTimeModel
                    {
                        StartShowTime = cinemaShowTime.StartShowTime,
                        EndShowTime = endShowTime,
                        MovieId = cinemaShowTime.SelectedMovieId,
                        RoomId = cinemaShowTime.SelectedNumberRoomId,
                        CreateDate = DateTime.Now
                    };

                    _context.CinemaShowTimes.Add(showTime);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            return View(cinemaShowTime);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cinemaShowTime = await _context.CinemaShowTimes.FindAsync(id);
            var movies = await _context.Movies.Select(m => new { m.Id, m.Name }).ToListAsync();
            var rooms = await _context.Rooms.Select(r => new { r.Id, r.RoomNumber }).ToListAsync();

            if (cinemaShowTime == null)
            {
                TempData["Error"] = "Suất chiếu không tồn tại";
                return RedirectToAction("Index");
            }

            var updateCinemaShowTime = MapToUpdateCinemaShowTime(cinemaShowTime);
            updateCinemaShowTime.MovieIds = new SelectList(movies, "Id", "Name");
            updateCinemaShowTime.NumberRooms = new SelectList(rooms, "Id", "RoomNumber");
            return View(updateCinemaShowTime);
        }

        private UpdateCinemaShowTime MapToUpdateCinemaShowTime(CinemaShowTimeModel cinemaShowTime)
        {
            return new UpdateCinemaShowTime
            {
                StartShowTime = cinemaShowTime.StartShowTime,
                SelectedMovieId = cinemaShowTime.MovieId,
                SelectedNumberRoomId = cinemaShowTime.RoomId
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCinemaShowTime updateCinemaST)
        {
            var movies = await _context.Movies.Select(m => new { m.Id, m.Name }).ToListAsync();
            var rooms = await _context.Rooms.Select(r => new { r.Id, r.RoomNumber }).ToListAsync();
            updateCinemaST.MovieIds = new SelectList(movies, "Id", "Name");
            updateCinemaST.NumberRooms = new SelectList(rooms, "Id", "RoomNumber");

            var nameEditor = User.FindFirstValue(ClaimUserLogin.UserName);
            var existingCinemaShowTimes = await _context.CinemaShowTimes.FindAsync(id);
            if (existingCinemaShowTimes == null)
            {
                TempData["Error"] = "Suất chiếu không tồn tại";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var selectedMovie = await _context.Movies.FindAsync(updateCinemaST.SelectedMovieId);
                if (selectedMovie == null)
                {
                    TempData["Error"] = "Phim đã chọn không tồn tại.";
                    return View(updateCinemaST);
                }

                DateTime endShowTime = updateCinemaST.StartShowTime.AddMinutes(selectedMovie.Duration);
                // Kiểm tra xem có suất chiếu nào trùng thời gian trong cùng phòng hay không
                var overlappingShowTimes = await _context.CinemaShowTimes
                    .Where(s => s.RoomId == updateCinemaST.SelectedNumberRoomId &&
                                s.StartShowTime < endShowTime &&  // Thời gian bắt đầu của suất chiếu mới sau khi kết thúc của suất chiếu hiện tại
                                s.StartShowTime.AddMinutes(s.Movie.Duration) > updateCinemaST.StartShowTime)  // Thời gian kết thúc của suất chiếu hiện tại sau khi bắt đầu của suất chiếu mới
                    .ToListAsync();

                if (overlappingShowTimes.Any())
                {
                    TempData["Error"] = "Phòng đã có suất chiếu vào thời gian này. Vui lòng chọn phòng hoặc thời gian khác.";
                    return View(updateCinemaST);
                }
                else
                {
                    existingCinemaShowTimes.StartShowTime = updateCinemaST.StartShowTime;
                    existingCinemaShowTimes.EndShowTime = endShowTime;
                    existingCinemaShowTimes.MovieId = updateCinemaST.SelectedMovieId;
                    existingCinemaShowTimes.RoomId = updateCinemaST.SelectedNumberRoomId;
                }
                _context.Update(existingCinemaShowTimes);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật suất chiếu thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(updateCinemaST);
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            CinemaShowTimeModel cinemaShowTime = await _context.CinemaShowTimes.FindAsync(Id);
            _context.CinemaShowTimes.Remove(cinemaShowTime);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa suất chiếu thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = _context.CinemaShowTimes.Find(Convert.ToInt32(item));
                        _context.CinemaShowTimes.Remove(obj);
                        _context.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
