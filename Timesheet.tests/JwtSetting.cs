using Timesheet.Helper;

internal class JwtSetting : Jwt
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public TimeSpan TokenExpiryTime { get; set; }
    public RefreshTokenSetting RefreshToken { get; set; }
}