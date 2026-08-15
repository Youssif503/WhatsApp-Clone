namespace Whatsapp.API.Helpers;
public class JWToptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
    public string DurationInHour { get; set; }
    public string RefreshTokenDurationInDays { get; set; }
}