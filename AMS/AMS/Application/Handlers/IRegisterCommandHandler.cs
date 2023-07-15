using AMS.Application.Accounts.Commands;
using AMS.Application.HandlerModels;

namespace AMS.Application.Handlers
{
    public interface IRegisterCommandHandler
    {
        Task<RegisterResult> Handle(RegisterCommand command);
    }
}
