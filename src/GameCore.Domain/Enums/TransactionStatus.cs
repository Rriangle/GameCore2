namespace GameCore.Domain.Enums;

public enum TransactionStatus
{
    Pending = 1,      // 待處理
    Processing = 2,   // 處理中
    Completed = 3,    // 已完成
    Failed = 4,       // 失敗
    Cancelled = 5,    // 已取消
    Refunded = 6      // 已退款
}
