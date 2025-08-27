namespace GameCore.Domain.Enums;

public enum TransactionType
{
    Deposit = 1,      // 存款
    Withdrawal = 2,   // 提款
    Transfer = 3,     // 轉帳
    Purchase = 4,     // 購買
    Sale = 5,         // 銷售
    Refund = 6,       // 退款
    Bonus = 7,        // 獎勵
    Fee = 8           // 手續費
}
