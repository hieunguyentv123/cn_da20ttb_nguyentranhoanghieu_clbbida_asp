using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using X.PagedList;

namespace BilliardClub.Admin.Controllers
{
    public class SlideController : Controller
    {
        private INotyfService _notyf;
        private ISlideRepository _slideRepository;

        public SlideController(INotyfService notyf, ISlideRepository slideRepository)
        {
            _notyf = notyf;
            _slideRepository = slideRepository;
        }

        public IActionResult Index(int? page, string? key)
        {
            var list = _slideRepository.GetAll();
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(list.Reverse().ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormFile img)
        {
            Slide slide = new Slide();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "..\\shared\\slide");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (img != null && img.Length > 0)
            {
                string fileName = Path.GetFileName(img.FileName);
                fileName = Guid.NewGuid().ToString() + fileName;
                slide.SlidePath = fileName;
                slide.IsDisplay = true;
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    img.CopyTo(stream);
                    _notyf.Success($"Upload ảnh slide thành công!", 5);
                }
                try
                {
                    _slideRepository.Add(slide);
                    _slideRepository.SaveChanges();
                    _notyf.Success("Thêm mới slide thành công");
                    return RedirectToAction("Index", "Slide");
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            else
            {
                _notyf.Information("Hãy chọn ảnh để tạo slide mới", 5);
            }
            return View(slide);
        }

        public IActionResult Edit(int id)
        {
            return View(_slideRepository.GetSingleById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slide slide, IFormFile img)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "..\\shared\\slide");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (img != null && img.Length > 0)
            {
                if (!String.IsNullOrEmpty(slide.SlidePath))
                {
                    var oldFilePath = Path.Combine(path, slide.SlidePath);
                    if (Directory.Exists(oldFilePath))
                    {
                        Directory.Delete(oldFilePath);
                    }
                }
                string fileName = Path.GetFileName(img.FileName);
                fileName = Guid.NewGuid().ToString() + fileName;
                slide.SlidePath = fileName;
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    img.CopyTo(stream);
                    _notyf.Success($"Cập nhật ảnh slide thành công!", 5);
                }
                try
                {
                    _slideRepository.Update(slide);
                    _slideRepository.SaveChanges();
                    _notyf.Success("Cập nhật ảnh slide thành công");
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            else
            {
                _notyf.Information("Hãy chọn ảnh để thay đổi", 5);
            }
            return View(slide);
        }
        public IActionResult Delete(int id)
        {
            var slide = _slideRepository.GetSingleById(id);
            return View(slide);
        }
        [HttpPost]
        public IActionResult Delete(int id, string confirm)
        {
            var slide = _slideRepository.GetSingleById(id);
            if (confirm == "OK")
            {
                try
                {
                    _slideRepository.Delete(slide);
                    _slideRepository.SaveChanges();
                    _notyf.Success("Xóa thành công", 5);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "..\\shared\\slide");
                    if (!String.IsNullOrEmpty(slide.SlidePath))
                    {
                        var oldFilePath = Path.Combine(path, slide.SlidePath);
                        if (Directory.Exists(oldFilePath))
                        {
                            Directory.Delete(oldFilePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            else
            {
                _notyf.Error("Xác nhận tên danh mục không đúng!", 5);
                return View(slide);
            }
            return RedirectToAction("Index", "Slide");
        }
        public IActionResult ChangeStatus(int id)
        {
            var slide = _slideRepository.GetSingleById(id);
            if (slide.IsDisplay)
            {
                slide.IsDisplay = false;
            }
            else
            {
                slide.IsDisplay = true;
            }
            try
            {
                _slideRepository.Update(slide);
                _slideRepository.SaveChanges();
                _notyf.Success("Cập nhật trạng thái slide thành công!", 5);

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return RedirectToAction("Index", "Slide");
        }
    }
}
