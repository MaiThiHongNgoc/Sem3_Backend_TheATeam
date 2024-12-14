namespace backend.Config
{
    public class JwtConfig
    {
        public string Key { get; set; } // Khóa bí mật dùng để tạo và kiểm tra JWT
        public string Issuer { get; set; } // Issuer (người cấp JWT)
        public string Audience { get; set; } // Audience (khách hàng mong muốn nhận JWT)
    }
}
