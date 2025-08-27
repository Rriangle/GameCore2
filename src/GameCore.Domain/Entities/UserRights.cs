namespace GameCore.Domain.Entities
{
    public class UserRights
    {
        public int UserRightsId { get; set; }
        public int UserId { get; set; }
        public bool SalesAuthority { get; set; }
        public bool PurchaseAuthority { get; set; }
        public bool AdminAuthority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}
