using System.Net;
using System.Text.Json;

namespace GameCore.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = "未授權的訪問";
                    errorResponse.ErrorCode = "UNAUTHORIZED";
                    break;

                case ArgumentException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = argEx.Message;
                    errorResponse.ErrorCode = "INVALID_ARGUMENT";
                    break;

                case InvalidOperationException invOpEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = invOpEx.Message;
                    errorResponse.ErrorCode = "INVALID_OPERATION";
                    break;

                case KeyNotFoundException keyEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = "請求的資源不存在";
                    errorResponse.ErrorCode = "NOT_FOUND";
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = _environment.IsDevelopment()
                        ? exception.Message
                        : "發生內部伺服器錯誤";
                    errorResponse.ErrorCode = "INTERNAL_ERROR";
                    break;
            }

            // 記錄錯誤
            _logger.LogError(exception, "處理請求時發生錯誤: {Method} {Path} - {ErrorCode}",
                context.Request.Method, context.Request.Path, errorResponse.ErrorCode);

            // 在開發環境中提供詳細錯誤資訊
            if (_environment.IsDevelopment())
            {
                errorResponse.Details = new
                {
                    ExceptionType = exception.GetType().Name,
                    StackTrace = exception.StackTrace,
                    InnerException = exception.InnerException?.Message
                };
            }

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(result);
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public object? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
