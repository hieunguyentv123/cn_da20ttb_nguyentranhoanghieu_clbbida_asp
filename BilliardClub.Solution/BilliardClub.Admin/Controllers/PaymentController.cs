using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BilliardClub.Admin.Controllers
{
    public class PaymentController : Controller
    {
        private INotyfService _notyf;
        private IPaymentRepository _paymentRepository;

        public PaymentController(INotyfService notyf, IPaymentRepository paymentRepository)
        {
            _notyf = notyf;
            _paymentRepository = paymentRepository;
        }

        public IActionResult Index(int? page, string? key)
        {
            var list = _paymentRepository.GetAll();
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(key))
            {
                list = list.Where(x => x.Method.ToLower().Contains(key.ToLower().Trim()));
            }
            return View(list.Reverse().ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Method")] Payment payment)
        {
            payment.IsApproved = true;
            if (ModelState.IsValid)
            {
                try
                {
                    _paymentRepository.Add(payment);
                    _paymentRepository.SaveChanges();
                    _notyf.Success("Thêm mới thành công phương thức thanh toán!", 5);
                    return RedirectToAction("Index", "Payment");
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            return View(payment);
        }

        public IActionResult Edit(int id)
        {
            return View(_paymentRepository.GetSingleById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Payment payment)
        {
            try
            {
                _paymentRepository.Update(payment);
                _paymentRepository.SaveChanges();
                _notyf.Success("Cập nhật thành công phương thức thanh toán!", 5);
                return RedirectToAction("Index", "Payment");
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return View(payment);
        }
        public IActionResult Delete(int id)
        {
            var payment = _paymentRepository.GetSingleById(id);
            return View(payment);
        }
        [HttpPost]
        public IActionResult Delete(int id, string confirm)
        {
            var payment = _paymentRepository.GetSingleById(id);
            if (confirm == payment.Method)
            {
                try
                {
                    _paymentRepository.Delete(payment);
                    _paymentRepository.SaveChanges();
                    _notyf.Success("Xóa thành công", 5);
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            else
            {
                _notyf.Error("Xác nhận tên phương thức không đúng!", 5);
                return View(payment);
            }
            return RedirectToAction("Index", "Payment");
        }
        public IActionResult ChangeStatus(int id)
        {
            var payment = _paymentRepository.GetSingleById(id);
            if (payment.IsApproved)
            {
                payment.IsApproved = false;
            }
            else
            {
                payment.IsApproved = true;
            }
            try
            {
                _paymentRepository.Update(payment);
                _paymentRepository.SaveChanges();
                _notyf.Success("Cập nhật trạng thái thành công!", 5);

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return RedirectToAction("Index", "Payment");
        }
    }
}
