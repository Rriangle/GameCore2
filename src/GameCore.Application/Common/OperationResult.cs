namespace GameCore.Application.Common
{
    /// <summary>
    /// 操作執行結果
    /// </summary>
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public object? Data { get; set; }

        public OperationResult()
        {
            IsSuccess = false;
            Message = string.Empty;
            ErrorCode = string.Empty;
            Data = null;
        }

        public OperationResult(bool isSuccess, string message = "", string errorCode = "", object? data = null)
        {
            IsSuccess = isSuccess;
            Message = message ?? string.Empty;
            ErrorCode = errorCode ?? string.Empty;
            Data = data;
        }

        /// <summary>
        /// 建立成功結果
        /// </summary>
        /// <param name="message">成功訊息</param>
        /// <param name="data">相關資料</param>
        /// <returns>成功結果</returns>
        public static OperationResult Success(string message = "操作成功", object? data = null)
        {
            return new OperationResult(true, message, "", data);
        }

        /// <summary>
        /// 建立失敗結果
        /// </summary>
        /// <param name="message">失敗訊息</param>
        /// <param name="errorCode">錯誤代碼</param>
        /// <param name="data">相關資料</param>
        /// <returns>失敗結果</returns>
        public static OperationResult Failure(string message = "操作失敗", string errorCode = "", object? data = null)
        {
            return new OperationResult(false, message, errorCode, data);
        }

        /// <summary>
        /// 建立警告結果
        /// </summary>
        /// <param name="message">警告訊息</param>
        /// <param name="data">相關資料</param>
        /// <returns>警告結果</returns>
        public static OperationResult Warning(string message = "操作完成但有警告", object? data = null)
        {
            return new OperationResult(true, message, "WARNING", data);
        }

        /// <summary>
        /// 檢查是否為成功結果
        /// </summary>
        /// <returns>是否成功</returns>
        public bool IsSuccessful() => IsSuccess;

        /// <summary>
        /// 檢查是否為失敗結果
        /// </summary>
        /// <returns>是否失敗</returns>
        public bool IsFailed() => !IsSuccess;

        /// <summary>
        /// 檢查是否為警告結果
        /// </summary>
        /// <returns>是否為警告</returns>
        public bool IsWarning() => IsSuccess && ErrorCode == "WARNING";
    }
} 