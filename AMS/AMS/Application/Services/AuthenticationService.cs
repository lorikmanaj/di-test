using AMS.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly double _expirationMinutes;

    public AuthenticationService(IUserService userService, string secretKey, string issuer, string audience, double expirationMinutes)
    {
        _userService = userService;
        _secretKey = secretKey;
        _issuer = issuer;
        _audience = audience;
        _expirationMinutes = expirationMinutes;
    }

    public async Task<string> AuthenticateUser(string email, string password)
    {
        var user = await _userService.GetUserByEmail(email);

        if (user != null && VerifyPassword(password, user.Password))
            return GenerateJwtToken(user.Id.ToString(), user.UserName, _secretKey, _issuer, _audience, _expirationMinutes);

        throw new AuthenticationException("Invalid email or password.");
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    private string GenerateJwtToken(string userId, string username, string secretKey, string issuer, string audience, double expirationMinutes)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public async Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        await context.ChallengeAsync(scheme, properties);
    }

    public async Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        await context.ForbidAsync(scheme, properties);
    }

    public async Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
    {
        await context.SignInAsync(scheme, principal, properties);
    }

    public async Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        await context.SignOutAsync(scheme, properties);
    }

    public async Task<AMS.Web.ViewModels.Results.AuthenticateResult> AuthenticateAsync(string email, string password)
    {
        var user = await _userService.GetUserByEmail(email);

        if (user != null && VerifyPassword(password, user.Password))
        {
            string token = GenerateJwtToken(user.Id.ToString(), user.UserName, _secretKey, _issuer, _audience, _expirationMinutes);
            return new AMS.Web.ViewModels.Results.AuthenticateResult { Succeeded = true, UserId = user.Id.ToString(), Username = user.UserName, Token = token };
        }
        else
            return new AMS.Web.ViewModels.Results.AuthenticateResult { Succeeded = false };
    }

    public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
    {
        throw new NotImplementedException();
    }
}
