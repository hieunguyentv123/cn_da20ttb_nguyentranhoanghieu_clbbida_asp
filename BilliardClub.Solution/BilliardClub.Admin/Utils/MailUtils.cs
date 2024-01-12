using System.Net;
using System.Net.Mail;

namespace BilliardClub.Admin.Utils
{
    public static class MailUtils
    {
        /// <summary>
        /// Gửi Email
        /// </summary>
        /// <param name="_from">Địa chỉ email gửi</param>
        /// <param name="_to">Địa chỉ email nhận</param>
        /// <param name="_subject">Chủ đề của email</param>
        /// <param name="_body">Nội dung (hỗ trợ HTML) của email</param>
        /// <param name="client">SmtpClient - kết nối smtp để chuyển thư</param>
        /// <returns>Task</returns>
        public static async Task<bool> SendMail(string _from, string _to, string _subject, string _body, SmtpClient client)
        {
            // Tạo nội dung Email
            MailMessage message = new MailMessage(
                from: _from,
                to: _to,
                subject: _subject,
                body: _body
            );
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.ReplyToList.Add(new MailAddress(_from));
            message.Sender = new MailAddress(_from);


            try
            {
                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Gửi email sử dụng máy chủ SMTP Google (smtp.gmail.com)
        /// </summary>
        public static async Task<bool> SendMailGoogleSmtp(string _to,  string _body)
        {

            MailMessage message = new MailMessage(
                from: "84billiarddemoemail@gmail.com",
                to: _to,
                subject: "Phản hồi về yêu cầu tư vấn",
                body: _body
            );
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.ReplyToList.Add(new MailAddress("84billiarddemoemail@gmail.com"));
            message.Sender = new MailAddress("84billiarddemoemail@gmail.com");

            // Tạo SmtpClient kết nối đến smtp.gmail.com
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential("84billiarddemoemail@gmail.com", "Hoanghieu1@");
                client.EnableSsl = true;
                return await SendMail("84billiarddemoemail@gmail.com", _to, "Phản hồi về yêu cầu tư vấn", _body, client);
            }

        }
    }
}
