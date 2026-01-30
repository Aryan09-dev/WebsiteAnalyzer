using WebsiteAnalyzer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using WebsiteAnalyzer.Application.Interfaces;
using WebsiteAnalyzer.Infrastructure.Repositories;
using WebsiteAnalyzer.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Add Services
// =========================
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddHttpClient<IWebsiteScanRepository, WebsiteScanRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
