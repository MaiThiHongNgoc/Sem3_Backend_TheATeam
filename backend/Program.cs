using backend.Services;
using backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using backend.Config;
using System.Text;
using backend.Filters;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Read configuration from appsettings.json
var configuration = builder.Configuration;

// Configure DbContext to MySQL
builder.Services.AddDbContext<MyAppContext>(options =>
    options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

// Register your services for DI
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<INGOService, NGOService>();
builder.Services.AddScoped<IGalleryImageService, GalleryImageService>();
builder.Services.AddScoped<IProgram1Service, Program1Service>();
builder.Services.AddScoped<IProgramDonationService, ProgramDonationService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<IQueryService, QueryService>();
builder.Services.AddScoped<ITransactionHistoryService, TransactionHistoryService>();

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
    options.AddPolicy("NGOPolicy", policy => policy.RequireRole("NGO"));
});

// Configure JWT Authentication
var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = "Role"
    };
})
.AddGoogle(options =>
{
    options.ClientId = configuration["Google:ClientId"];
    options.ClientSecret = configuration["Google:ClientSecret"];
})
.AddFacebook(options =>
{
    options.AppId = configuration["Facebook:AppId"];
    options.AppSecret = configuration["Facebook:AppSecret"];
    options.Fields.Add("email");
    options.Scope.Add("public_profile");
    options.Scope.Add("email");
    options.SaveTokens = true;
});

// Configure Email settings
builder.Services.Configure<EmailSettings>(configuration.GetSection("Email"));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Controllers with JSON Options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64;
    });

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<FileUploadOperationFilter>();
    options.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});


// Add Logging
builder.Logging.AddConsole();

var app = builder.Build();

// Configure Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();





// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.AspNetCore.Authentication.Google;
// using Microsoft.AspNetCore.Authentication.Facebook;
// using backend.Data;
// using backend.Config; // Giả sử bạn có một thư mục Config chứa các cấu hình như JwtConfig và EmailSettings
// using System.Text;
// using backend.Services; 

// var builder = WebApplication.CreateBuilder(args);

// // Đọc cấu hình từ appsettings.json
// var configuration = builder.Configuration;

// // Cấu hình DbContext kết nối với MySQL
// builder.Services.AddDbContext<MyAppContext>(options =>
//     options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
//     new MySqlServerVersion(new Version(8, 0, 21))));

// // Cấu hình JWT Authentication
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
//     options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
//     options.AddPolicy("NGOPolicy", policy => policy.RequireRole("NGO"));
// });

// var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = configuration["Jwt:Issuer"],
//             ValidAudience = configuration["Jwt:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(key),
//             RoleClaimType = "Role"
//         };
//     });

// // Cấu hình Google Authentication
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
// })
// .AddGoogle(options =>
// {
//     options.ClientId = configuration["Google:ClientId"];
//     options.ClientSecret = configuration["Google:ClientSecret"];
// })
// // Cấu hình Facebook Authentication
// .AddFacebook(options =>
// {
//     options.AppId = configuration["Facebook:AppId"];
//     options.AppSecret = configuration["Facebook:AppSecret"];
//     options.Fields.Add("email"); // You can add more fields based on the data you want
//     options.Scope.Add("public_profile");
//     options.Scope.Add("email");
//     options.SaveTokens = true; // Save the access token to retrieve later
// });

// // Cấu hình email từ appsettings.json
// builder.Services.Configure<EmailSettings>(configuration.GetSection("Email"));

// // Cấu hình CORS để cho phép tất cả các nguồn gốc
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll", policy =>
//     {
//         policy.AllowAnyOrigin()   // Cho phép tất cả các nguồn gốc
//               .AllowAnyHeader()   // Cho phép tất cả các header
//               .AllowAnyMethod();  // Cho phép tất cả các phương thức HTTP
//     });
// });

// // Thêm Controllers
// builder.Services.AddControllers()
//     .AddJsonOptions(options =>
//     {
//         options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//         options.JsonSerializerOptions.MaxDepth = 64;
//     });

// // Cấu hình Swagger cho API
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Logging.AddConsole();


// var app = builder.Build();

// // Kiểm tra môi trường để bật Swagger khi phát triển
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// // Sử dụng CORS để cho phép tất cả các yêu cầu
// app.UseCors("AllowAll");

// // Bật các middleware cần thiết
// app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();

// // Map các controller
// app.MapControllers();

// // Chạy ứng dụng
// app.Run();
