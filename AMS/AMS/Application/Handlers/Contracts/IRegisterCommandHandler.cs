using AMS.Web.ViewModels.Accounts.Commands;
using AMS.Web.ViewModels.HandlerModels;

namespace AMS.Application.Handlers.Contracts
{
    public interface IRegisterCommandHandler
    {
        Task<RegisterResult> Handle(RegisterCommand command);
    }
}
