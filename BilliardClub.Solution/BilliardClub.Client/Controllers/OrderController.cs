using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Client.DataReceivers;
using BilliardClub.Client.Models;
using BilliardClub.Client.Utils;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BilliardClub.Client.Controllers
{
    public class OrderController : Controller
    {
        private INotyfService _notyf;
        private ILayoutDataReceiver _layoutDataReceiver;
        private IPaymentRepository _paymentRepository;
        private IOrderRepository _orderRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IProductRepository _productRepository;
        private IUserRepository _userRepository;
        public const string CARTKEY = "cart";
        public OrderController(IPaymentRepository paymentRepository,IUserRepository userRepository, INotyfService notyf, ILayoutDataReceiver layoutDataReceiver, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IProductRepository productRepository)
        {
            _notyf = notyf;
            _layoutDataReceiver = layoutDataReceiver;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository; 
        }
        private void GetData()
        {
            HomeViewModel model = _layoutDataReceiver.GetData();
            ViewBag.ArticleCategories = model.ArticleCategories;
            ViewBag.ProductCategories = model.ProductCategories;
            ViewBag.ContactInformations = model.ContactInformations;
            ViewBag.Payments = _paymentRepository.GetMulti(x => x.IsApproved);
            ViewBag.Cart = GetCartItems();
        }
        List<CartItemViewModel> GetCartItems()
        {

            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItemViewModel>>(jsoncart);
            }
            return new List<CartItemViewModel>();
        }
        public IActionResult Checkout()
        {
            if (GetCartItems().Count() == 0)
            {
                _notyf.Information("Giỏ hàng của bạn đang trống! Hãy tiếp tục mua sắm rồi thanh toán nhé");
                return RedirectToAction("Index", "Product");
            }
            if (HttpContext.Session.GetInt32("_id") == null)
            {
                _notyf.Information("Hãy tiến hành đăng nhập để thanh toán nhé!");
                return RedirectToAction("Login", "Auth");
            }
            GetData();
            return View(_userRepository.GetSingleById((int)HttpContext.Session.GetInt32("_id")));
        }
        public IActionResult PaymentProceed(string fullname,string email, string phone, string address, string note, int payment)
        {
            Order order = new Order()
            {
                OrderId = Guid.NewGuid(),
                Fullname = fullname,
                Email = email,
                Phone = phone,
                Address = address,
                Note = note,
                PaymentId = payment,
                OrderAt = DateTime.Now,
                OrderBy = (int)HttpContext.Session.GetInt32("_id"),
                IsPaid = false,
                TotalPrice = GetTotalPrice(),
            };
            InsertOrderToDb(order);
            InsertOrderDetailsToDb(order);
            return PaymentProcessing(order, payment);
        }

        private IActionResult PaymentProcessing(Order order, int payment)
        {
            HttpContext.Session.SetString("_orderId", order.OrderId.ToString());
            switch (payment)
            {
                case 2:
                    return MomoPayment(order.TotalPrice);
                case 1:
                    return VnPayPayment(order.TotalPrice);
                default:
                    DeleteOrder(order.OrderId);
                    _notyf.Information("Chức năng hiện đang phát triển nên chưa thể sử dụng! Vui lòng chọn phương thức thanh toán khác", 5);
                    return RedirectToAction("Checkout", "Order");
            }
        }
        public ActionResult MomoPayment(int total)
        {
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toán sản phẩm từ 84 Billiard";
            string returnUrl = $"https://localhost:7117/Order/MomoResult";
            string notifyurl = "http://ba1adf48beba.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount = total.ToString();
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
            notifyurl + "&extraData=" +
            extraData;

            MomoSecurity crypto = new MomoSecurity();
            //sign signature SHA256
            string signature = crypto.SignSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }
            };

            string responseFromMomo = MomoPaymentRequest.SendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }
        public ActionResult MomoResult()
        {
            //hiển thị thông báo cho người dùng
            string errorCode = HttpContext.Request.Query["errorCode"].ToString();
            Guid orderId = new Guid((string)HttpContext.Session.GetString("_orderId"));

            if (errorCode == "0")
            {
                OrderAdjustment(orderId);
                ViewBag.Message = "Thanh toán thành công qua ví điện tử Momo, cảm ơn bạn đã lựa chọn mua hàng từ 84 Billiard";
            }
            else
            {
                DeleteOrder(orderId);
                ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn, thanh toán thất bại";
            }
            GetData();
            return View();
        }
        public ActionResult VnPayPayment(int total)
        {
            string url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string returnUrl = "https://localhost:7117/Order/VnPayResult";
            string tmnCode = "GHHNT2HB";
            string hashSecret = "BAGAOHAPRHKQZASKQZASVPRSAKPXNYXS";

            VnPayLib pay = new VnPayLib();

            pay.AddRequestData("vnp_Version", "2.0.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.0.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", (total * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", "127.0.0.1"); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toán sản phẩm từ 84 Billiard"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }
        public ActionResult VnPayResult()
        {
            if (HttpContext.Request.Query.Count() > 0)
            {
                string hashSecret = "BAGAOHAPRHKQZASKQZASVPRSAKPXNYXS"; //Chuỗi bí mật
                var vnpayData = HttpContext.Request.Query;
                VnPayLib pay = new VnPayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (var s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s.Key) && s.Key.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s.Key, s.Value);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = HttpContext.Request.Query["vnp_SecureHash"].ToString(); //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?
                
                Guid id = new Guid((string)HttpContext.Session.GetString("_orderId"));

                if (checkSignature)
                {
                    string errorCode = HttpContext.Request.Query["vnp_ResponseCode"].ToString();
                    if (vnp_ResponseCode == "00")
                    {
                        OrderAdjustment(id);
                        ViewBag.Message = "Thanh toán thành công qua VNPAY hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + ", cảm ơn bạn đã mua hàng từ 84 Billiard";
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        DeleteOrder(id);
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn VNPAY: " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    DeleteOrder(id);
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn VNPAY: " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                }
            }
            GetData();
            return View();
        }
        public void OrderAdjustment(Guid id)
        {
            MoneyPaid(id);
            StockMinus(id);
        }

        private void StockMinus(Guid id)
        {
            var orderDetails = _orderDetailRepository.GetMulti(x => x.OrderId == id);
            foreach (var item in orderDetails)
            {
                var product = _productRepository.GetSingleById(item.ProductId);
                product.Stock -= item.Quantity;
                _productRepository.Update(product);
            }
            _productRepository.SaveChanges();
        }

        private void MoneyPaid(Guid id)
        {
            var order = _orderRepository.GetSingleByCondition(x => x.OrderId == id);
            order.IsPaid = true;
            _orderRepository.Update(order);
            _orderRepository.SaveChanges();
        }

        private void DeleteOrder(Guid id)
        {
            var order = _orderRepository.GetSingleByCondition(x => x.OrderId == id);
            _orderRepository.Delete(order);
            _orderRepository.SaveChanges();
        }
        private void InsertOrderToDb(Order order)
        {
            _orderRepository.Add(order);
            _orderRepository.SaveChanges();
        }
        private void InsertOrderDetailsToDb(Order order)
        {
            var cart = GetCartItems();
            foreach (var item in cart)
            {
                int price = 0;
                if (item.Product.DiscountPercent != 0)
                {
                    price = item.Quantity * (item.Product.Price / 100 * (100 - item.Product.DiscountPercent));
                }
                else
                {
                    price = item.Quantity * item.Product.Price;
                }
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = order.OrderId,
                    ProductId = item.Product.ProductId,
                    Quantity = item.Quantity,
                    SinglePrice = price,
                };
                _orderDetailRepository.Add(orderDetail);
                _orderDetailRepository.SaveChanges();
            }
        }
        private int GetTotalPrice()
        {
            var cart = GetCartItems();
            int total = 0;
            foreach (var item in cart)
            {
                if (item.Product.DiscountPercent != 0)
                {
                    int price = item.Quantity * (item.Product.Price / 100 * (100 - item.Product.DiscountPercent));
                    total += price;
                }
                else
                {
                    int price = item.Quantity * item.Product.Price;
                    total += price;
                }
            }
            return total;
        }
    }
}
