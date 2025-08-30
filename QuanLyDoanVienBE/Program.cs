using Microsoft.EntityFrameworkCore;
using QuanLyDoanVienBE.ModelFromDB;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost7058", policy =>
    {
        policy.WithOrigins("https://localhost:7058") // frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // nếu dùng cookie, token...
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


IConfigurationRoot cf = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
builder.Services.AddDbContext<dbQuanLyDoanVien>(opt => opt.UseSqlServer(cf.GetConnectionString("cnn")));

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseStaticFiles(); // m?c ??nh ?ã b?t wwwroot
app.UseCors("AllowLocalhost7058"); // PHẢI đặt trước Authorization

app.UseAuthorization();


app.MapControllers();

app.Run();


