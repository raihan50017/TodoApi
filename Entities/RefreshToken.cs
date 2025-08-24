namespace TodoApi.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = default!;
        public DateTime ExpireOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public Guid UserId { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpireOn;
        public bool IsRevoked => RevokedOn != null;
        public bool IsActive => RevokedOn == null && !IsExpired;
        public User User { get; set; } = default!;
    }
}
