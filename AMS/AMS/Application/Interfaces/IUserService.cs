using AMS.Application.HandlerModels;
using AMS.Domain;
using AMS.Domain.Models;
using AMS.Web.ViewModels;

namespace AMS.Application.Interfaces
{
    public interface IUserService : IGenericRepository<User>
    {
        User GetUserById(int userId);
        Task<LoginResult> LoginAsync(LoginRequest request);
        Task<RegisterResult> RegisterAsync(RegisterRequest request);
    }
}
