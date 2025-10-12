namespace SurveyBasket.Domain.Entities;


[Owned]
public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedOn { get; set; }
    public string UserId { get; set; }//its guid
    bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsExpired && RevokedOn is null;
}
