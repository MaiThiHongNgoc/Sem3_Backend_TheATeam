namespace backend.Models
{
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
}
