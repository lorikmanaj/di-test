using AMS.Application.Exceptions;
using AMS.Application.HandlerModels;
using AMS.Application.Interfaces;
using AMS.Config;
using AMS.Data;
using AMS.Domain.Models;
using AMS.Infrastructure.Repositories;
using AMS.Web.ViewModels.Requests;
using AMS.Web.ViewModels.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

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
            var existingUser = await _userService.GetUserByEmail(request.Email);
            if (existingUser != null)
                return new RegisterResult { Success = false, Message = "Email is already registered" };

            if (!IsValidEmail(request.Email))
                return new RegisterResult { Success = false, Message = "Invalid email format" };

            if (!IsValidPassword(request.Password))
                return new RegisterResult { Success = false, Message = "Invalid password format" };

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

        private bool IsValidEmail(string email)
        {
            const string EmailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(email, EmailPattern);
        }

        private bool IsValidPassword(string password)
        {
            const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{3,}$";
            return Regex.IsMatch(password, PasswordPattern);
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username))
                return new LoginResult { Success = false, Message = "Username is required" };

            if (string.IsNullOrEmpty(request.Password))
                return new LoginResult { Success = false, Message = "Password is required" };

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

        public async Task<User> GetUserById(int userId)
        {
            var user = _userService.GetById(userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userService.GetFirstOrDefaultAsync(u => u.Email == email);
        }

        public IEnumerable<User> GetUsers()
        {
            return _userService.GetAll();
        }

        public async Task<User> UpdateUser(int userId, UserUpdateRequest request)
        {
            var user = await _userService.GetUserById(userId) ?? throw new NotFoundException("User not found");

            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.Email = request.Email ?? user.Email;
            user.Password = request.Password ?? user.Password;
            user.Age = request.Age;

            _userService.Update(user);
            _userService.SaveChanges();

            return user;
        }

        public async Task<DeleteResult> DeleteUser(int userId)
        {
            var user = _userService.GetById(userId);

            if (user == null)
                return new DeleteResult { Success = false, Message = "User not found" };

            _userService.Delete(user);
            _userService.SaveChanges();

            return new DeleteResult { Success = true, Message = "User deleted successfully" };
        }

        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _userService.GetUserById(userId) ?? throw new NotFoundException("User not found.");
            
            var passwordHasher = new PasswordHasher<User>();

            var verifyResult = passwordHasher.VerifyHashedPassword(user, user.Password, currentPassword);

            if (verifyResult == PasswordVerificationResult.Failed)
                return false;

            var newHashedPassword = passwordHasher.HashPassword(user, newPassword);
            user.Password = newHashedPassword;

            _userService.Update(user);
            _userService.SaveChanges();

            return true;
        }

        public async Task<bool> ResetPassword(string email, string newPassword)
        {
            var user = await _userService.GetFirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return false;

            var passwordHasher = new PasswordHasher<User>();

            var newPasswordHash = passwordHasher.HashPassword(user, newPassword);
            user.PasswordHash = newPasswordHash;

            _userService.Update(user);
            _userService.SaveChanges();

            return true;
        }

        public async Task<IEnumerable<Account>> GetUserAccounts(int userId)
        {
            var user = await _userService.GetUserById(userId);

            if (user == null)
                return Enumerable.Empty<Account>();

            return user.Accounts;
        }

        public async Task<bool> AddAccountToUser(int userId, Account account)
        {
            var user = await _userService.GetUserById(userId);

            if (user == null)
                return false;

            user.Accounts.Add(account);

            _userService.Update(user);
            _userService.SaveChanges();

            return true;
        }

        public async Task<bool> RemoveAccountFromUser(int userId, int accountId)
        {
            var user = await _userService.GetUserById(userId);

            if (user == null)
                return false;

            var account = user.Accounts.FirstOrDefault(a => a.Id == accountId);

            if (account == null)
                return false;

            user.Accounts.Remove(account);

            _userService.Update(user);
            _userService.SaveChanges();

            return true;
        }
    }
}
