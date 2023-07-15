﻿using AMS.Web.ViewModels.Accounts.Commands;
using AMS.Web.ViewModels.HandlerModels;

namespace AMS.Application.Handlers
{
    public interface IRegisterCommandHandler
    {
        Task<RegisterResult> Handle(RegisterCommand command);
    }
}
