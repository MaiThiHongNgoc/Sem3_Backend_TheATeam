"# Sem3-backend" 
Project ATM Bank
1. Các thực thể(Entity): User, Account, Transaction
2. Sử dụng ASP.Net Core, Entity Framwork core, ORM, Swagger
3. Cài đặt EF Core và MySql
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Pomelo.EntityFrameworkCore.MySql
4. Cài đặt Swagger để tích hợp api
dotnet add package Swashbuckle.AspNetCore
5. Migrations đồng bộ hóa entity với mysql
dotnet ef migrations add InitialCreate
6. Áp dụng Migration để tạo Database
dotnet ef database update
