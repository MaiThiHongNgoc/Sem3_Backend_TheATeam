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
                        DateOfBirth = DateTime.MinValue,
                        Account = account
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
        // Đăng nhập/Đăng ký bằng Facebook
        // [HttpPost("facebook")]
        // public async Task<IActionResult> LoginWithFacebook([FromBody] FacebookLoginRequest request)
        // {
        //     try
        //     {
        //         // Kiểm tra nếu AccessToken không có giá trị
        //         if (string.IsNullOrEmpty(request.AccessToken))
        //         {
        //             return BadRequest("Access token không hợp lệ.");
        //         }

        //         // Gửi yêu cầu tới Facebook để lấy thông tin người dùng
        //         using var client = new HttpClient();
        //         var url = $"https://graph.facebook.com/me?fields=id,first_name,last_name,email&access_token={request.AccessToken}";
        //         var responseMessage = await client.GetAsync(url);

        //         // Kiểm tra phản hồi của Facebook API
        //         if (!responseMessage.IsSuccessStatusCode)
        //         {
        //             return StatusCode((int)responseMessage.StatusCode, "Lỗi khi kết nối với Facebook API.");
        //         }

        //         var responseContent = await responseMessage.Content.ReadAsStringAsync();
        //         Console.WriteLine($"Facebook API response: {responseContent}");

        //         // Deserialize phản hồi JSON từ Facebook
        //         var facebookUser = JsonConvert.DeserializeObject<FacebookUser>(responseContent);
        //         if (facebookUser == null || string.IsNullOrEmpty(facebookUser.Email))
        //         {
        //             return BadRequest("Không thể xác thực tài khoản Facebook. Email không hợp lệ.");
        //         }

        //         // Kiểm tra người dùng đã tồn tại trong hệ thống
        //         var account = await _context.Accounts
        //             .Include(a => a.Role)
        //             .FirstOrDefaultAsync(a => a.Email == facebookUser.Email);

        //         if (account == null)
        //         {
        //             // Tạo tài khoản mới nếu người dùng chưa tồn tại
        //             account = new Account
        //             {
        //                 Email = facebookUser.Email,
        //                 Username = $"{facebookUser.FirstName} {facebookUser.LastName}",
        //                 Password = string.Empty, // Không cần mật khẩu khi đăng nhập qua Facebook
        //                 RoleId = userRole.RoleId, // Giả định RoleId = 2 là User
        //                 IsActive = true
        //             };

        //             _context.Accounts.Add(account);
        //             await _context.SaveChangesAsync();

        //             // Tạo đối tượng Customer
        //             var customer = new Customer
        //             {
        //                 FirstName = facebookUser.FirstName,
        //                 LastName = facebookUser.LastName,
        //                 DateOfBirth = DateTime.MinValue, // Không có thông tin về ngày sinh
        //                 Account = account
        //             };

        //             _context.Customers.Add(customer);
        //             await _context.SaveChangesAsync();

        //             // Gửi email chào mừng người dùng
        //             var emailBody = $"Chào {facebookUser.FirstName} {facebookUser.LastName},\n\nCảm ơn bạn đã đăng ký tài khoản tại MyApp qua Facebook!";
        //             if (!SendEmail(facebookUser.Email, "Chào mừng đến với MyApp", emailBody))
        //             {
        //                 Console.WriteLine("Cảnh báo: Lỗi khi gửi email chào mừng.");
        //             }

        //             // Tạo JWT token cho tài khoản mới
        //             var token = GenerateJwtToken(account);
        //             return Ok(new { Token = token, AccountId = account.AccountId });
        //         }

        //         // Nếu người dùng đã tồn tại, tạo JWT token và trả về
        //         var existingToken = GenerateJwtToken(account);
        //         return Ok(new { Token = existingToken, AccountId = account.AccountId });
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log lỗi chi tiết
        //         Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
        //         return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}");
        //     }
        // }

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
                Account = account
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




            // Gửi email thông báo đăng nhập
            var emailBody = $"Chào {request.EmailOrUsername},\n\nBạn vừa đăng nhập vào tài khoản MyApp lúc {DateTime.UtcNow}. Nếu không phải bạn, hãy liên hệ với chúng tôi ngay lập tức.";
            if (!SendEmail(request.EmailOrUsername, "Login Notification", emailBody))
                Console.WriteLine("Cảnh báo: Lỗi khi gửi email thông báo đăng nhập."); // Không chặn người dùng tiếp tục sử dụng ứng dụng.

            return Ok(new { Token = token });
        }


        // Quên mật khẩu
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == request.Email);
            if (account == null) return NotFound("Email không tồn tại.");

            var resetToken = new PasswordResetToken
            {
                Token = Guid.NewGuid().ToString(),
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
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        margin: 0;
                        padding: 0;
                    }}
                    .header {{
                        background-color: #007BFF;
                        color: white;
                        padding: 10px;
                        text-align: center;
                        font-size: 24px;
                        font-weight: bold;
                    }}
                    .footer {{
                        background-color: #f1f1f1;
                        color: #555;
                        padding: 10px;
                        text-align: center;
                        font-size: 14px;
                    }}
                    .content {{
                        padding: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class='header'>MyApp</div>
                <div class='content'>
                    {bodyContent}
                </div>
                <div class='footer'>
                    &copy; {DateTime.Now.Year} MyApp. All rights reserved.
                </div>
            </body>
            </html>";

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("MyApp", _config["Email:From"]));
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
    public class RegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class LoginRequest
    {
        public string EmailOrUsername { get; set; }
        public string Password { get; set; }
    }
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
    public class UpdateCustomerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }
    }
    public class FacebookUser
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    // Mô hình yêu cầu từ frontend
    public class FacebookLoginRequest
    {
        public string AccessToken { get; set; }
    }
}
