using AMS.Application.Exceptions;
using AMS.Application.HandlerModels;
using AMS.Application.Interfaces;
using AMS.Config;
using AMS.Data;
using AMS.Domain.Models;
using AMS.Infrastructure.Repositories;
using AMS.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AMS.Application.Services
{
    public class UserService : GenericRepository<User>, IUserService
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly AppSetings _appSettings;
        private readonly AMSDb _dbContext;

        public UserService(IUserService userService, UserManager<User> userManager, IOptions<AppSetings> appSettings, AMSDb dbContext)
            : base(dbContext)
        {
            _userService = userService;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
        }

        public async Task<RegisterResult> RegisterAsync(RegisterRequest request)
        {
            // Perform any necessary validations before registering the user

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                Age = request.Age,
                Accounts = new List<Account>()
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
                return new RegisterResult { Success = true, Message = "User registered successfully" };
            else
                return new RegisterResult { Success = false, Message = "Failed to register user" };
        }

        public User GetUserById(int userId)
        {
            var user = _userService.GetById(userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            return user;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            // Perform any necessary validations before login

            var user = await _userManager.FindByNameAsync(request.Username);

            if (user == null)
                return new LoginResult { Success = false, Message = "Invalid username" };

            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (result)
            {
                var token = GenerateJwtToken(user);
                return new LoginResult { Success = true, Message = "Login successful", Token = token };
            }

            return new LoginResult { Success = false, Message = "Invalid password" };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(_appSettings.TokenExpirationHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
