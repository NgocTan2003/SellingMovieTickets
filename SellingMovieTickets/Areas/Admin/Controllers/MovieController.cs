﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;
using System.Security.Claims;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class MovieController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MovieController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            var moviesQuery = _context.Movies
                .Include(m => m.MovieCategoryMappings)
                    .ThenInclude(mcm => mcm.MovieCategory)
                .OrderByDescending(x => x.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                moviesQuery = moviesQuery.Where(x =>
                    x.Name.Contains(searchText) ||
                    x.Description.Contains(searchText) ||
                    x.ReleaseDate.ToString().Contains(searchText) ||
                    x.Duration.ToString().Contains(searchText) ||
                    x.MovieLanguageFormat.Contains(searchText) ||
                    x.MovieFormat.Contains(searchText) ||
                    x.TrailerUrl.Contains(searchText));
            }

            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = await moviesQuery.CountAsync();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;

            var movies = await moviesQuery.Skip(resSkip).Take(pager.PageSize).ToListAsync();
            var movieViewModel = movies.Select(x => new MovieViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ReleaseDate = x.ReleaseDate,
                Duration = x.Duration,
                MovieLanguageFormat = x.MovieLanguageFormat,
                MovieFormat = x.MovieFormat,
                Director = x.Director,
                Actor = x.Actor,
                Rating = x.Rating,
                TrailerUrl = x.TrailerUrl,
                Status = x.Status,
                Image = x.Image,
                CreateBy = x.CreateBy,
                CreateDate = x.CreateDate,
                ModifiedBy = x.ModifiedBy,
                ModifiedDate = x.ModifiedDate,
                Genres = string.Join(", ", x.MovieCategoryMappings.Select(mcm => mcm.MovieCategory.CategoryName))
            }).ToList();
            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(movieViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new CreateMovie
            {
                MovieLanguageFormats = new SelectList(new List<string>
                {
                    MovieLanguageFormat.Vietsub,
                    MovieLanguageFormat.Narration,
                    MovieLanguageFormat.Voiceover
                }),
                MovieFormats = new SelectList(new List<string>
                {
                    MovieFormat.TwoD,
                    MovieFormat.ThreeD,
                    MovieFormat.IMAX,
                    MovieFormat.FourD
                }),
                MovieCategoryList = new MultiSelectList(_context.MovieCategories, "Id", "CategoryName")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMovie movie)
        {
            movie.MovieLanguageFormats = new SelectList(new List<string>
                {
                    MovieLanguageFormat.Vietsub,
                    MovieLanguageFormat.Narration,
                    MovieLanguageFormat.Voiceover
                });
            movie.MovieFormats = new SelectList(new List<string>
                {
                    MovieFormat.TwoD,
                    MovieFormat.ThreeD,
                    MovieFormat.IMAX,
                    MovieFormat.FourD
                });
            movie.MovieCategoryList = new MultiSelectList(_context.MovieCategories, "Id", "CategoryName");

            if (ModelState.IsValid)
            {
                var nameEditor = User.FindFirstValue(ClaimUserLogin.UserName);
                var existingMovie = await _context.Movies.FirstOrDefaultAsync(p => p.Name == movie.Name);

                if (existingMovie != null)
                {
                    TempData["Error"] = "Tên phim đã có trong cơ sở dữ liệu";
                    return View(movie);
                }

                string imageName = null;
                if (movie.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/movies");
                    imageName = Guid.NewGuid().ToString() + "_" + movie.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await movie.ImageUpload.CopyToAsync(fs);
                    }
                }

                var movieModel = new MovieModel
                {
                    Name = movie.Name,
                    Description = movie.Description,
                    ReleaseDate = movie.ReleaseDate,
                    Duration = movie.Duration,
                    MovieLanguageFormat = movie.SelectedMovieLanguageFormat,
                    MovieFormat = movie.SelectedMovieFormat,
                    Director = movie.Director,
                    Actor = movie.Actor,
                    TrailerUrl = movie.TrailerUrl,
                    Status = movie.Status,
                    Image = imageName,
                    CreateDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    CreateBy = nameEditor
                };
                _context.Movies.Add(movieModel);
                await _context.SaveChangesAsync();
                var movieId = movieModel.Id;

                // Lưu các thể loại đã chọn
                foreach (var categoryId in movie.SelectedCategories)
                {
                    var movieCategory = new MovieCategoryMappingModel
                    {
                        MovieId = movieId,
                        MovieCategoryId = categoryId
                    };
                    _context.MovieCategoryMappings.Add(movieCategory);
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm phim thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(movie);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies
               .Include(m => m.MovieCategoryMappings)
               .ThenInclude(mc => mc.MovieCategory)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                TempData["Error"] = "Phim không tồn tại";
                return RedirectToAction("Index");
            }

            var updateMovie = MapToUpdateMovie(movie);
            var categories = await _context.MovieCategories.ToListAsync();
            updateMovie.MovieCategoryList = new MultiSelectList(categories, "Id", "CategoryName", movie.MovieCategoryMappings.Select(mc => mc.MovieCategoryId));
            updateMovie.MovieLanguageFormats = new SelectList(new List<string>
                {
                    MovieLanguageFormat.Vietsub,
                    MovieLanguageFormat.Narration,
                    MovieLanguageFormat.Voiceover
                });
            updateMovie.MovieFormats = new SelectList(new List<string>
                {
                    MovieFormat.TwoD,
                    MovieFormat.ThreeD,
                    MovieFormat.IMAX,
                    MovieFormat.FourD
                });
            return View(updateMovie);
        }

        private UpdateMovie MapToUpdateMovie(MovieModel movie)
        {
            return new UpdateMovie
            {
                Name = movie.Name,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Duration = movie.Duration,
                SelectedMovieLanguageFormat = movie.MovieLanguageFormat,
                SelectedMovieFormat = movie.MovieFormat,
                Director = movie.Director,
                Actor = movie.Actor,
                TrailerUrl = movie.TrailerUrl,
                Status = movie.Status,
                Image = movie.Image,
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateMovie movie)
        {
            movie.MovieLanguageFormats = new SelectList(new List<string>
                {
                    MovieLanguageFormat.Vietsub,
                    MovieLanguageFormat.Narration,
                    MovieLanguageFormat.Voiceover
                });
            movie.MovieFormats = new SelectList(new List<string>
                {
                    MovieFormat.TwoD,
                    MovieFormat.ThreeD,
                    MovieFormat.IMAX,
                    MovieFormat.FourD
                });
            movie.MovieCategoryList = new MultiSelectList(_context.MovieCategories, "Id", "CategoryName");

            var nameEditor = User.FindFirstValue(ClaimUserLogin.UserName);
            var existingMovie = await _context.Movies.FindAsync(id);
            if (movie.ImageUpload == null)
            {
                movie.Image = existingMovie.Image;
            }

            if (existingMovie == null)
            {
                TempData["Error"] = "Phim không tồn tại";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingMovie.Name = movie.Name;
                existingMovie.Description = movie.Description;
                existingMovie.ReleaseDate = movie.ReleaseDate;
                existingMovie.Duration = movie.Duration;
                existingMovie.MovieLanguageFormat = movie.SelectedMovieLanguageFormat;
                existingMovie.MovieFormat = movie.SelectedMovieFormat;
                existingMovie.TrailerUrl = movie.TrailerUrl;
                existingMovie.Status = movie.Status;
                existingMovie.ModifiedBy = nameEditor;
                existingMovie.ModifiedDate = DateTime.Now;

                if (movie.ImageUpload != null)
                {
                    if (movie.Image != null)
                    {
                        string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/movies");
                        string oldFileImage = Path.Combine(uploadsDir, existingMovie.Image);
                        if (System.IO.File.Exists(oldFileImage))
                        {
                            System.IO.File.Delete(oldFileImage);
                        }
                    }

                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/movies");
                    string imageName = Guid.NewGuid().ToString() + "_" + movie.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await movie.ImageUpload.CopyToAsync(fs);
                    }
                    existingMovie.Image = imageName;
                }

                // Xóa các thể loại cũ
                var existingCategories = _context.MovieCategoryMappings.Where(m => m.MovieId == id).ToList();
                _context.MovieCategoryMappings.RemoveRange(existingCategories);

                // Thêm các thể loại mới được chọn
                foreach (var categoryId in movie.SelectedCategories)
                {
                    var movieCategory = new MovieCategoryMappingModel
                    {
                        MovieId = existingMovie.Id,
                        MovieCategoryId = categoryId
                    };
                    _context.MovieCategoryMappings.Add(movieCategory);
                }

                _context.Update(existingMovie);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật phim thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(movie);
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            MovieModel movie = await _context.Movies.FindAsync(Id);
            if (movie.Image != null)
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/movies");
                string oldFileImage = Path.Combine(uploadsDir, movie.Image);
                if (System.IO.File.Exists(oldFileImage))
                {
                    System.IO.File.Delete(oldFileImage);
                }
            }
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa phim thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, Status status)
        {
            var movieItem = _context.Movies.FirstOrDefault(n => n.Id == id);
            if (movieItem == null)
            {
                return Json(new { success = false });
            }

            movieItem.Status = status;
            _context.SaveChanges();

            return Json(new { success = true });
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
                        var obj = _context.Movies.Find(Convert.ToInt32(item));
                        if (obj.Image != null)
                        {
                            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/movies");
                            string oldFileImage = Path.Combine(uploadsDir, obj.Image);
                            if (System.IO.File.Exists(oldFileImage))
                            {
                                System.IO.File.Delete(oldFileImage);
                            }
                        }
                        _context.Movies.Remove(obj);
                        _context.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
