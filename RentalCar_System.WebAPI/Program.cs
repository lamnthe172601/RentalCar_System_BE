using RentalCar_System.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RentalCar_System.Business.RentalCarService;
using RentalCar_System.Data.RentalContractRepository;
using RentalCar_System.Models.Entity;
using RentalCar_System.Business.AuthService;
using RentalCar_System.Business.UserService;
using RentalCar_System.Data;
using RentalCar_System.Business.CarService; // Add this line
using RentalCar_System.Data.UserRepository;
using System.Text;
using RentalCar_System.Data.CarRepository;
using RentalCar_System.Business.CarService;
using RentalCar_System.Business.NotificationService;
using RentalCar_System.Business.Background;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RentalCarDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarService, CarService>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region repository
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IRentalContractRepository, RentalContractRepository>();
builder.Services.AddScoped<IRentalContractService, RentalContractService>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHostedService<ExpiringContractsBackgroundService>();


#endregion

#region JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("5TgbNHY6UjmKL4pMdV9G7RbQ2wXA8Zc3Hty7Nr9Lq5l3PMi2Ue4hYvS7w3jKl9Pb"))
            };
            options.Events = new JwtBearerEvents { OnAuthenticationFailed = context => { context.NoResult(); context.Response.StatusCode = 401; context.Response.ContentType = "text/plain"; return context.Response.WriteAsync("Invalid token"); } };
        });

builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field. Example: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

#endregion

#region CORS
// Thêm dịch vụ CORS vào DI container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
#endregion

builder.Services.AddControllers();
var app = builder.Build();
// Cấu hình để phục vụ các tệp tĩnh từ thư mục Images

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "Images")),
    RequestPath = "/api/cars/images"
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Thêm middleware CORS.
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
