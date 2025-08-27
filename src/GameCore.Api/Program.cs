using GameCore.Api.Middleware;
using GameCore.Domain.Interfaces;
using GameCore.Api.Services;
using GameCore.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using GameCore.Infrastructure.Data;
using Serilog;

namespace GameCore.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
var builder = WebApplication.CreateBuilder(args);

        // 配置 Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
            .WriteTo.File("logs/gamecore-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

        // 添加服務到容器
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    
        // 配置資料庫
builder.Services.AddDbContext<GameCoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // 配置記憶體快取
        builder.Services.AddMemoryCache();

        // 配置 CORS
builder.Services.AddCors(options =>
{
            options.AddPolicy("AllowAll", policy =>
    {
                policy.AllowAnyOrigin()
              .AllowAnyMethod()
                      .AllowAnyHeader();
    });
});

        // 註冊服務
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IWalletService, WalletService>();
        builder.Services.AddScoped<IMarketService, MarketService>();
        builder.Services.AddScoped<IJwtService, JwtService>();

        // 註冊 Repository
        builder.Services.AddScoped<IUserRepository, GameCore.Infrastructure.Repositories.UserRepository>();
        builder.Services.AddScoped<IUserWalletRepository, GameCore.Infrastructure.Repositories.UserWalletRepository>();
        builder.Services.AddScoped<IMarketRepository, GameCore.Infrastructure.Repositories.MarketRepository>();
        builder.Services.AddScoped<IProductRepository, GameCore.Infrastructure.Repositories.ProductRepository>();

var app = builder.Build();

        // 配置 HTTP 請求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
            app.UseSwaggerUI();
        }

        // 使用自定義中間件
        app.UseErrorHandling();
        app.UseRateLimiting();

app.UseHttpsRedirection();
        app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

        // 健康檢查端點
        app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

        try
        {
            Log.Information("啟動 GameCore API...");
app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "應用程式啟動失敗");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
