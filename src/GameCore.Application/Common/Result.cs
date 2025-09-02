using System.Collections.Generic;

namespace GameCore.Application.Common
{
    /// <summary>
    /// 統一的操作結果封裝
    /// </summary>
    /// <typeparam name="T">結果資料類型</typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Value { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public List<ValidationError> ValidationErrors { get; private set; }

        private Result()
        {
            ValidationErrors = new List<ValidationError>();
        }

        /// <summary>
        /// 建立成功結果
        /// </summary>
        /// <param name="value">成功結果值</param>
        /// <returns>成功結果</returns>
        public static Result<T> Success(T value)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Value = value,
                ErrorMessage = string.Empty
            };
        }

        /// <summary>
        /// 建立失敗結果
        /// </summary>
        /// <param name="error">錯誤訊息</param>
        /// <returns>失敗結果</returns>
        public static Result<T> Failure(string error)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Value = default,
                ErrorMessage = error
            };
        }

        /// <summary>
        /// 建立驗證失敗結果
        /// </summary>
        /// <param name="validationErrors">驗證錯誤列表</param>
        /// <returns>驗證失敗結果</returns>
        public static Result<T> ValidationFailure(List<ValidationError> validationErrors)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Value = default,
                ErrorMessage = "驗證失敗",
                ValidationErrors = validationErrors
            };
        }

        /// <summary>
        /// 隱式轉換運算子：從 T 轉換為 Result<T>
        /// </summary>
        /// <param name="value">值</param>
        public static implicit operator Result<T>(T value) => Success(value);
    }

    /// <summary>
    /// 無返回值的操作結果
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public List<ValidationError> ValidationErrors { get; private set; }

        private Result()
        {
            ValidationErrors = new List<ValidationError>();
        }

        /// <summary>
        /// 建立成功結果
        /// </summary>
        /// <param name="message">成功訊息</param>
        /// <returns>成功結果</returns>
        public static Result Success(string message = "操作成功")
        {
            return new Result
            {
                IsSuccess = true,
                Message = message
            };
        }

        /// <summary>
        /// 建立失敗結果
        /// </summary>
        /// <param name="message">失敗訊息</param>
        /// <returns>失敗結果</returns>
        public static Result Failure(string message = "操作失敗")
        {
            return new Result
            {
                IsSuccess = false,
                Message = message
            };
        }

        /// <summary>
        /// 建立驗證失敗結果
        /// </summary>
        /// <param name="validationErrors">驗證錯誤列表</param>
        /// <returns>驗證失敗結果</returns>
        public static Result ValidationFailure(List<ValidationError> validationErrors)
        {
            return new Result
            {
                IsSuccess = false,
                Message = "驗證失敗",
                ValidationErrors = validationErrors
            };
        }
    }

    /// <summary>
    /// 驗證錯誤
    /// </summary>
    public class ValidationError
    {
        public string PropertyName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }

        public ValidationError(string propertyName, string errorMessage, string? errorCode = null)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }
    }
} 