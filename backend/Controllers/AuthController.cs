using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Google.Apis.Auth;
using Facebook;
using Newtonsoft.Json;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly MyAppContext _context;
        private readonly IConfiguration _config;

        public AuthController(MyAppContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Đăng ký/Đăng nhập bằng Google
        [HttpPost("google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginRequest request)
        {
            try
            {
                // Xác minh token từ Google
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _config["Google:ClientId"] }
                });

                // Kiểm tra người dùng đã tồn tại
                var account = await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Email == payload.Email);
                if (account == null)
                {
                    // Nếu chưa tồn tại, đăng ký tài khoản mới
                    var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
                    if (userRole == null) return StatusCode(500, "Role mặc định không tồn tại.");

                    account = new Account
                    {
                        Email = payload.Email,
                        Password = string.Empty, // Không cần mật khẩu vì dùng Google
                        RoleId = userRole.RoleId
                    };

                    var customer = new Customer
                    {
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Gender = "Other", // Hoặc bạn có thể lấy thông tin từ payload nếu có.
                        DateOfBirth = DateTime.MinValue,
                        Account = account,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();

                    // Gửi email chào mừng
                    var emailBody = $"Chào {payload.GivenName} {payload.FamilyName},\n\nCảm ơn bạn đã đăng ký tài khoản tại MyApp qua Google!";
                    if (!SendEmail(payload.Email, "Welcome to MyApp", emailBody))
                        Console.WriteLine("Cảnh báo: Lỗi khi gửi email chào mừng.");
                }

                // Tạo JWT token
                var token = GenerateJwtToken(account);

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message); // Ghi chi tiết lỗi
                return BadRequest("Đăng nhập Google thất bại: " + ex.InnerException?.Message ?? ex.Message);
            }
        }

        // Đăng ký
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Accounts.AnyAsync(a => a.Email == request.Email))
                return BadRequest("Email đã tồn tại.");
            if (await _context.Accounts.AnyAsync(a => a.Username == request.Username))
                return BadRequest("Username đã tồn tại.");

            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
            if (userRole == null) return StatusCode(500, "Role mặc định không tồn tại.");

            var account = new Account
            {
                Email = request.Email,
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = userRole.RoleId
            };

            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Gender = "Other", // Hoặc bạn có thể lấy thông tin từ payload nếu có.
                Account = account,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Gửi email chào mừng
            var emailBody = $"Chào {request.FirstName} {request.LastName},\n\nCảm ơn bạn đã đăng ký tài khoản tại MyApp!";
            if (!SendEmail(request.Email, "Welcome to MyApp", emailBody))
                return StatusCode(500, "Đăng ký thành công nhưng lỗi khi gửi email.");

            return Ok("Đăng ký thành công.");
        }

        // Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a =>
                    a.Email == request.EmailOrUsername ||
                    a.Username == request.EmailOrUsername);

            if (account == null || !BCrypt.Net.BCrypt.Verify(request.Password, account.Password))
                return Unauthorized("Thông tin đăng nhập không chính xác.");

            var token = GenerateJwtToken(account);

            // Kiểm tra xem đây có phải là lần đăng nhập đầu tiên
            if (account.IsFirstLogin)
            {
                // Gửi email thông báo đăng nhập
                var emailBody = $"Chào {request.EmailOrUsername},\n\nBạn vừa đăng nhập vào tài khoản MyApp lần đầu tiên lúc {DateTime.UtcNow}. Chúc bạn sử dụng MyApp vui vẻ!";
                if (!SendEmail(request.EmailOrUsername, "Welcome to MyApp", emailBody))
                    Console.WriteLine("Cảnh báo: Lỗi khi gửi email thông báo đăng nhập.");

                // Cập nhật trường IsFirstLogin thành false sau lần đăng nhập đầu tiên
                account.IsFirstLogin = false;
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
            }

            return Ok(new { Token = token });
        }


        // Quên mật khẩu
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == request.Email);
            if (account == null) return NotFound("Email không tồn tại.");

            // Tạo mã xác thực 6 số
            var resetToken = new PasswordResetToken
            {
                Token = GenerateResetToken(), // Tạo mã xác thực 6 số
                ExpiryDate = DateTime.UtcNow.AddHours(1),
                AccountId = account.AccountId
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            // Gửi Email
            if (!SendEmail(account.Email, "Password Reset", $"Your reset token: {resetToken.Token}"))
                return StatusCode(500, "Lỗi khi gửi email.");

            return Ok("Mã đặt lại mật khẩu đã được gửi qua email.");
        }

        // Đặt lại mật khẩu
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var tokenEntry = await _context.PasswordResetTokens
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.Token == request.Token && t.ExpiryDate > DateTime.UtcNow);

            if (tokenEntry == null) return BadRequest("Mã không hợp lệ hoặc đã hết hạn.");

            tokenEntry.Account.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _context.PasswordResetTokens.Remove(tokenEntry);

            await _context.SaveChangesAsync();
            return Ok("Mật khẩu đã được đặt lại.");
        }

        // Tạo mã xác thực 6 số
        private string GenerateResetToken()
        {
            Random random = new Random();
            string token = random.Next(100000, 999999).ToString(); // Tạo mã ngẫu nhiên từ 100000 đến 999999
            return token;
        }


        // Tạo JWT Token
        private string GenerateJwtToken(Account account)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, account.Email),
        new Claim("Role", account.Role.RoleName),  // Role claim is added here
        new Claim("id", account.AccountId.ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // Gửi Email
        private bool SendEmail(string toEmail, string subject, string bodyContent)
        {
            try
            {
                // Tạo nội dung HTML của email
                var emailBody = $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>
                        body {{
                            font-family: 'Arial', sans-serif;
                            margin: 0;
                            padding: 0;
                            background-color: #e7eff6;
                            color: #333;
                        }}
                        .email-container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            border-radius: 8px;
                            overflow: hidden;
                            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                        }}
                        .header {{
                            background-color: #1d72b8;
                            color: #fff;
                            padding: 25px;
                            text-align: center;
                            font-size: 28px;
                            font-weight: bold;
                        }}
                        .content {{
                            padding: 30px;
                            font-size: 16px;
                            line-height: 1.6;
                            color: #333;
                        }}
                        .content h2 {{
                            color: #1d72b8;
                            font-size: 22px;
                        }}
                        .content p {{
                            margin: 10px 0;
                            font-size: 16px;
                        }}
                        .footer {{
                            background-color: #f4f4f9;
                            color: #555;
                            text-align: center;
                            padding: 20px;
                            font-size: 14px;
                            border-top: 1px solid #ddd;
                        }}
                        .footer a {{
                            color: #1d72b8;
                            text-decoration: none;
                        }}
                    </style>
                </head>
                <body>
                    <div class='email-container'>
                        <div class='header'>
                            Welcome to MyApp
                        </div>
                        <div class='content'>
                            <h2>Hello,</h2>
                            <p>{bodyContent}</p>
                        </div>
                        <div class='footer'>
                            &copy; {DateTime.Now.Year} MyApp. All rights reserved.<br>
                            <small>If you have any questions, feel free to contact us at <a href='mailto:support@myapp.com'>support@myapp.com</a></small>
                        </div>
                    </div>
                </body>
                </html>";

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("MyApp Support", _config["Email:From"]));
                email.To.Add(new MailboxAddress("", toEmail));
                email.Subject = subject;

                // Thiết lập email gửi với định dạng HTML
                email.Body = new TextPart("html") { Text = emailBody };

                using var smtp = new SmtpClient();
                smtp.Connect(_config["Email:SmtpServer"], 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(_config["Email:Username"], _config["Email:Password"]);
                smtp.Send(email);
                smtp.Disconnect(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }

}
