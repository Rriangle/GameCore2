using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GameCore.Tests
{
    public class TestProgram
    {
        public static WebApplication CreateTestApplication()
        {
            var builder = WebApplication.CreateBuilder();
            
            // 加入基本服務
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            
            // 加入健康檢查
            builder.Services.AddHealthChecks();
            
            var app = builder.Build();
            
            // 配置中間件
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
            
            return app;
        }
    }
}