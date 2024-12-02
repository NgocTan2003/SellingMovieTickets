using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.News;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;
using System.Security.Claims;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class NewsController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public NewsController(DataContext context, IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            IEnumerable<NewsModel> news = await _context.News.OrderByDescending(x => x.Id).ToListAsync();

            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                news = news.Where(x =>
                    (x.Title.Contains(searchText) ||
                    x.Description.Contains(searchText) ||
                    x.Detail.Contains(searchText)));
            }

            int recsCount = news.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = news.Skip(resSkip).Take(pager.PageSize).ToList();
            var newsVM = _mapper.Map<List<NewsViewModel>>(data);

            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(newsVM);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsModel news)
        {
            if (ModelState.IsValid)
            {
                var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
                news.SeoTitle = news.Title.Replace(" ", "-");
                var seoTitle = await _context.News.FirstOrDefaultAsync(p => p.SeoTitle == news.SeoTitle);
                if (seoTitle != null)
                {
                    TempData["Error"] = "Title tin tức đã có trong cơ sở dữ liệu";
                    return View(news);
                }

                if (news.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/news");
                    string imageName = Guid.NewGuid().ToString() + "_" + news.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await news.ImageUpload.CopyToAsync(fs);
                    }
                    news.Image = imageName;
                }

                news.CreateDate = DateTime.Now;
                news.ModifiedDate = DateTime.Now;
                news.CreateBy = nameEditor;
                _context.Add(news);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm tin tức thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(news);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                TempData["Error"] = "Tin tức không tồn tại";
                return RedirectToAction("Index");
            }
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewsModel news)
        {
            var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
            var existingNews = await _context.News.FindAsync(id);
            if (news.ImageUpload == null)
            {
                news.Image = existingNews.Image;
            }

            if (existingNews == null)
            {
                TempData["Error"] = "Tin tức không tồn tại";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingNews.Title = news.Title;
                existingNews.Description = news.Description;
                existingNews.Detail = news.Detail;
                existingNews.SeoDescription = news.SeoDescription;
                existingNews.SeoKeywords = news.SeoKeywords;
                existingNews.Status = news.Status;
                existingNews.StartDate = news.StartDate;
                existingNews.ModifiedBy = nameEditor;
                existingNews.ModifiedDate = DateTime.Now;

                // Tạo SeoTitle từ title tin tức và kiểm tra trùng lặp
                existingNews.SeoTitle = news.Title.Replace(" ", "-").ToLower();
                var seoTitle = await _context.News.FirstOrDefaultAsync(p => p.SeoTitle == existingNews.SeoTitle && p.Id != id);
                if (seoTitle != null)
                {
                    TempData["Error"] = "Tin tức đã có trong cơ sở dữ liệu";
                    return View(news);
                }

                if (news.ImageUpload != null)
                {
                    if (existingNews.Image != null)
                    {
                        // xóa ảnh cũ đi trc khi update ảnh ms
                        string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/news");
                        string oldFileImage = Path.Combine(uploadsDir, existingNews.Image);
                        if (System.IO.File.Exists(oldFileImage))
                        {
                            System.IO.File.Delete(oldFileImage);
                        }
                    }

                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/news");
                    string imageName = Guid.NewGuid().ToString() + "_" + news.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await news.ImageUpload.CopyToAsync(fs);
                    }
                    existingNews.Image = imageName;
                }

                _context.Update(existingNews);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật tin tức thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(news);
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            NewsModel news = await _context.News.FindAsync(Id);
            if (news.Image != null)
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/news");
                string oldFileImage = Path.Combine(uploadsDir, news.Image);
                if (System.IO.File.Exists(oldFileImage))
                {
                    System.IO.File.Delete(oldFileImage);
                }
            }
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa tin tức thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, Status status)
        {
            var newsItem = _context.News.FirstOrDefault(n => n.Id == id);
            if (newsItem == null)
            {
                return Json(new { success = false });
            }

            newsItem.Status = status;
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
                        var obj = _context.News.Find(Convert.ToInt32(item));
                        if (obj.Image != null)
                        {
                            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/news");
                            string oldFileImage = Path.Combine(uploadsDir, obj.Image);
                            if (System.IO.File.Exists(oldFileImage))
                            {
                                System.IO.File.Delete(oldFileImage);
                            }
                        }
                        _context.News.Remove(obj);
                        _context.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
