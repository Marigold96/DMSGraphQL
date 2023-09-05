namespace DMSGraphQL;

public class JwtSettings
{
    public string Secret { set; get; }
    public int AccessTokenExpMinute { set; get; }
    public int RefreshTokenExpMinute { set; get; }
    public string Issuer { set; get; }
    public string Audience { set; get; }
}
