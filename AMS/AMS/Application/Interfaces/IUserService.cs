using AMS.Application.HandlerModels;
using AMS.Domain;
using AMS.Domain.Models;
using AMS.Web.ViewModels.Requests;
using AMS.Web.ViewModels.Results;

namespace AMS.Application.Interfaces
{
    public interface IUserService : IGenericRepository<User>
    {
        Task<User> GetUserById(int userId);
        Task<User> GetUserByEmail(string email);
        IEnumerable<User> GetUsers();
        Task<User> UpdateUser(int userId, UserUpdateRequest request);
        Task<DeleteResult> DeleteUser(int userId);
        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);
        Task<bool> ResetPassword(string email, string newPassword);
        Task<IEnumerable<Account>> GetUserAccounts(int userId);
        Task<bool> AddAccountToUser(int userId, Account account);
        Task<bool> RemoveAccountFromUser(int userId, int accountId);

        Task<LoginResult> LoginAsync(LoginRequest request);
        Task<RegisterResult> RegisterAsync(RegisterRequest request);
    }
}
