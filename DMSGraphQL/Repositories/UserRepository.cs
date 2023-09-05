using Common.Models;
using DMSGraphQL.Data;
using DMSGraphQL.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DMSGraphQL.Repositories;

public class UserRepository : IUserRepository
{

    private readonly ApplicationDbContext _appDbContext;
    private readonly JwtSettings _jwtSettings;
    private DbSet<UserModel> Users => _appDbContext.Users;

    public UserRepository(
        ApplicationDbContext appDbContext,
        IOptions<JwtSettings> jwtSettings)
    {
        _appDbContext = appDbContext;
        _jwtSettings = jwtSettings.Value;
    }

    public UserModel ById(Guid Id)
    {
        return Users.Single(u => u.Id == Id);
    }

    public UserModel? ByName(string name)
    {
        return Users.SingleOrDefault(u => u.Name == name);
    }

    public LoginData Login(LoginInput loginInput)
    {
        if (string.IsNullOrEmpty(loginInput.UserName)
            || string.IsNullOrEmpty(loginInput.Password))
        {
            return new LoginData();
        }

        var user = ByName(loginInput.UserName);
        if (user == null)
        {
            return new LoginData();
        }

        if (!ValidatePasswordHash(loginInput.Password, user.Password))
        {
            return new LoginData();
        }

        return LoginDataGenerate(user);
    }

    public bool ChangePassword(string username, ChangePasswordData passwordData)
    {
        if (string.IsNullOrEmpty(username))
        {
            return false;
        }

        var user = ByName(username);
        if (user == null)
        {
            return false;
        }

        if (!ValidatePasswordHash(passwordData.OldPassword, user.Password))
        {
            return false;
        }

        user.Password = HashPassword(passwordData.NewPassword);

        _appDbContext.Update(user);
        _appDbContext.SaveChanges();

        return true;
    }

    public LoginData RenewAccessToken(LoginData renewTokenData)
    {
        if (string.IsNullOrEmpty(renewTokenData.AccessToken)
        || string.IsNullOrEmpty(renewTokenData.RefreshToken))
        {
            return new LoginData();
        }

        ClaimsPrincipal principal = GetClaimsFromExpiredToken(renewTokenData.AccessToken);

        if (principal == null)
        {
            return new LoginData();
        }

        string userName = principal.Claims.Where(_ => _.Type == "UserName").Select(_ => _.Value).FirstOrDefault();
        if (string.IsNullOrEmpty(userName))
        {
            return new LoginData();
        }

        var user = Users.SingleOrDefault(x => x.Name == userName && x.RefreshToken == renewTokenData.RefreshToken && x.RefreshTokenExpiration > DateTime.Now);
        if (user == null)
        {
            return new LoginData();
        }

        return LoginDataGenerate(user);
    }

    public UserModel Add(UserModel userModel)
    {
        userModel.Password = HashPassword(userModel.Password);
        _appDbContext.Add(userModel);
        _appDbContext.SaveChanges();
        return userModel;
    }

    public bool Remove(Guid userId)
    {
        var userModel = ById(userId);
        _appDbContext.Remove(userModel);
        _appDbContext.SaveChanges();
        return true;
    }

    public UserModel Update(UserModel userModel)
    {
        userModel.Password = HashPassword(userModel.Password);
        _appDbContext.Update(userModel);
        _appDbContext.SaveChanges();
        return userModel;
    }

    private LoginData LoginDataGenerate(UserModel user)
    {
        var response = new LoginData(true, GenerateToken(user), GenerateRefreshToken());

        user.RefreshToken = response.RefreshToken;
        user.RefreshTokenExpiration = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpMinute);

        _appDbContext.Update(user);
        _appDbContext.SaveChanges();

        return response;
    }

    private string HashPassword(string password)
    {
        byte[] salt;
        RandomNumberGenerator.Create().GetBytes(salt = new byte[16]);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
        byte[] hash = pbkdf2.GetBytes(20);

        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }

    private bool ValidatePasswordHash(string password, string dbPassword)
    {
        byte[] hashBytes = Convert.FromBase64String(dbPassword);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
        byte[] hash = pbkdf2.GetBytes(20);

        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hash[i])
            {
                return false;
            }
        }

        return true;
    }
    private string GenerateToken(UserModel user)
    {
        var securtityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(securtityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
            {
                new Claim("UserName", user.Name),
                new Claim(ClaimTypes.Name, user.Name)
            };

        if(user.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
        }

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpMinute),
            signingCredentials: credentials,
            claims: claims
        );
        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
    private ClaimsPrincipal GetClaimsFromExpiredToken(string accessToken)
    {
        var tokenValidationParameter = new TokenValidationParameters
        {
            ValidIssuer = _jwtSettings.Issuer,
            ValidateIssuer = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            ValidateLifetime = false // ignore expiration
        };

        var jwtHandler = new JwtSecurityTokenHandler();
        var principal = jwtHandler.ValidateToken(accessToken, tokenValidationParameter, out SecurityToken securityToken);

        var jwtScurityToken = securityToken as JwtSecurityToken;
        if (jwtScurityToken == null)
        {
            return null;
        }

        return principal;
    }
}
