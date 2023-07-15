using AMS.Application.Accounts.Commands;
using AMS.Application.HandlerModels;

namespace AMS.Application.Handlers
{
    public interface ILoginCommandHandler
    {
        Task<LoginResult> Handle(LoginCommand command);
    }
}
