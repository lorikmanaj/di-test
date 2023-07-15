using AMS.Web.ViewModels.Accounts.Commands;
using AMS.Web.ViewModels.HandlerModels;

namespace AMS.Application.Handlers.Contracts
{
    public interface ILoginCommandHandler
    {
        Task<LoginResult> Handle(LoginCommand command);
        Task<string> AuthenticateUser(string email, string password);
    }
}
