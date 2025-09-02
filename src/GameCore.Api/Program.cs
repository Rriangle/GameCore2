using GameCore.Api.Middleware;
using GameCore.Domain.Interfaces;
using GameCore.Api.Services;
using GameCore.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using GameCore.Infrastructure.Data;
using Serilog;
using GameCore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Serialization;
// 新增：回應壓縮與 Brotli/Gzip 相關命名空間（效能優化）
using Microsoft.AspNetCore.ResponseCompression; // 目的：啟用回應壓縮中介軟體，提高回應傳輸效率
using System.IO.Compression; // 目的：設定壓縮等級

namespace GameCore.Api;

public partial class Program
{
    public static async Task Main(string[] args)
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
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                // 效能優化：配置 JSON 序列化選項，提升序列化效能
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // 忽略 null 值，減少回應大小
                options.JsonSerializerOptions.WriteIndented = false; // 生產環境不需要格式化，提升效能
                options.JsonSerializerOptions.MaxDepth = 64; // 限制序列化深度，防止無限遞迴
            });
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    
        // 配置資料庫
        builder.Services.AddDbContext<GameCoreDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            // 效能優化：在開發環境啟用詳細日誌，生產環境關閉
            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // 效能優化：配置記憶體快取，提升熱點資料查詢效能
        builder.Services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024; // 限制快取大小為 1024 個項目
            options.ExpirationScanFrequency = TimeSpan.FromMinutes(5); // 每 5 分鐘掃描過期項目
        });

        // 效能優化（新增）：配置回應壓縮（Brotli/Gzip）
        // 目的：降低 JSON 等回應傳輸大小 60-80%，提升下載速度
        // 前後差異：原本無壓縮 → 啟用壓縮（含 HTTPS）
        // 風險與回滾：低風險，僅影響傳輸層；問題時可移除 UseResponseCompression()
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            // 僅壓縮常見的 API 回應 MIME 類型，避免壓縮影像等已壓縮格式
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
        });
        builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
        builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

        // 效能優化（新增）：回應快取設定
        // 目的：讓標記了 [ResponseCache] 的端點可被中介軟體快取，減少重複計算
        // 前後差異：原本未啟用 → 啟用 UseResponseCaching()
        // 風險與回滾：需注意授權端點不得被快取；若有問題可移除 UseResponseCaching()
        builder.Services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 1024 * 1024; // 限制快取回應大小 1MB
            options.UseCaseSensitivePaths = false; // 路徑不區分大小寫，有助於提升命中率
        });

        // 效能優化：配置 HTTP 客戶端工廠，避免 HttpClient 洩漏問題
        builder.Services.AddHttpClient("default", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30); // 設定 30 秒逾時
            client.DefaultRequestHeaders.Add("User-Agent", "GameCore-API/1.0");
        });

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

        // 註冊服務 - Stage 1: 只註冊認證相關服務
        builder.Services.AddScoped<IAuthService, AuthService>();
        // 暫時註解掉其他服務，專注於 Stage 1 認證功能
        // builder.Services.AddScoped<IWalletService, WalletService>();
        // builder.Services.AddScoped<IMarketService, MarketService>();
        // builder.Services.AddScoped<IJwtService, JwtService>();
        // builder.Services.AddScoped<IValidationService, ValidationService>();

        // 註冊 Repository - Stage 1: 只註冊認證相關 Repository
        builder.Services.AddScoped<IUserRepository, GameCore.Infrastructure.Repositories.UserRepository>();
        builder.Services.AddScoped<IUserWalletRepository, GameCore.Infrastructure.Repositories.UserWalletRepository>();
        // 暫時註解掉其他 Repository，專注於 Stage 1 認證功能
        // builder.Services.AddScoped<IMarketRepository, GameCore.Infrastructure.Repositories.MarketRepository>();
        // builder.Services.AddScoped<IProductRepository, GameCore.Infrastructure.Repositories.ProductRepository>();

        // 註冊服務 - Stage 2: 註冊其他服務
        builder.Services.AddScoped<IWalletService, WalletService>();
        builder.Services.AddScoped<ISalesService, SalesService>();
        builder.Services.AddScoped<IStoreService, StoreService>();
        builder.Services.AddScoped<GameCore.Domain.Interfaces.IUserService, UserService>();
        builder.Services.AddScoped<GameCore.Api.Services.IValidationService, ValidationService>();

        // 註冊 Repository - Stage 2: 註冊其他 Repository
        builder.Services.AddScoped<IMemberSalesProfileRepository, MemberSalesProfileRepository>();
        builder.Services.AddScoped<IUserSalesInformationRepository, UserSalesInformationRepository>();
        builder.Services.AddScoped<IProductInfoRepository, ProductInfoRepository>();
        builder.Services.AddScoped<IOrderInfoRepository, OrderInfoRepository>();
        builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
        builder.Services.AddScoped<IUserIntroduceRepository, UserIntroduceRepository>();
        builder.Services.AddScoped<IUserRightsRepository, UserRightsRepository>();

        // 註冊服務 - Stage 3: 市場和排行榜服務
        builder.Services.AddScoped<MarketService>();
        builder.Services.AddScoped<LeaderboardService>();

        // 註冊 Repository - Stage 3: 市場和排行榜 Repository
        // 暫時註解掉不存在的 Repository，等實體和 Repository 完成後再啟用
        // builder.Services.AddScoped<IPlayerMarketProductInfoRepository, PlayerMarketProductInfoRepository>();
        // builder.Services.AddScoped<IPlayerMarketOrderInfoRepository, PlayerMarketOrderInfoRepository>();
        // builder.Services.AddScoped<IGameRepository, GameRepository>();
        // builder.Services.AddScoped<IMetricRepository, MetricRepository>();
        builder.Services.AddScoped<IGameMetricDailyRepository, GameMetricDailyRepository>();
        // builder.Services.AddScoped<IPopularityIndexDailyRepository, PopularityIndexDailyRepository>();
        // builder.Services.AddScoped<ILeaderboardSnapshotRepository, LeaderboardSnapshotRepository>();

        // 註冊服務 - Stage 4: 社交功能服務
        // builder.Services.AddScoped<INotificationService, NotificationService>();
        // builder.Services.AddScoped<IChatService, ChatService>();
        // builder.Services.AddScoped<IGroupService, GroupService>();

        // 註冊 Repository - Stage 4: 社交功能 Repository
        // builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
        // builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        // builder.Services.AddScoped<IGroupRepository, GroupRepository>();
        // builder.Services.AddScoped<IReactionRepository, ReactionRepository>();
        // builder.Services.AddScoped<IForumThreadRepository, ForumThreadRepository>();
        // builder.Services.AddScoped<IThreadPostRepository, ThreadPostRepository>();

        // 註冊服務 - Stage 4: 每日簽到和虛擬寵物服務
        builder.Services.AddScoped<ISignInService, SignInService>();
        builder.Services.AddScoped<IPetService, PetService>();
        // builder.Services.AddScoped<IMiniGameService, MiniGameService>();

        // 註冊 Repository - Stage 4: 每日簽到和虛擬寵物相關
        builder.Services.AddScoped<IUserSignInStatsRepository, UserSignInStatsRepository>();
        builder.Services.AddScoped<IPetRepository, PetRepository>();
        // builder.Services.AddScoped<IMiniGameRepository, MiniGameRepository>();

        // 註冊 JWT 認證
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "your-secret-key-here"))
                };
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        // 配置 HTTP 請求管道
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // 暫時註解掉有問題的中間件，專注於 Stage 1 認證功能
        // app.UseErrorHandling();
        // app.UseRateLimiting();
        // app.UseMiddleware<ResponseWrapperMiddleware>();

        // 新增：回應壓縮中介軟體需放在靠前位置（在回應產生前）
        // 目的：確保所有後續回應都能被壓縮
        app.UseResponseCompression();

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");

        // 新增：回應快取中介軟體
        // 目的：使 [ResponseCache] 生效，降低重複請求計算
        // 風險：需避免對授權資料做共享快取（本案未對授權端點加上 ResponseCache 屬性）
        app.UseResponseCaching();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // 健康檢查端點
        app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

        // 初始化種子資料 - Stage 1
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<GameCoreDbContext>();
            try
            {
                await SeedData.InitializeAsync(context);
                Log.Information("資料庫種子資料初始化完成");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "資料庫種子資料初始化失敗");
            }
        }

        try
        {
            Log.Information("啟動 GameCore API...");
            await app.RunAsync();
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
