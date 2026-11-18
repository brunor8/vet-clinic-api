using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.Application.Common.Messaging;
public interface ICommandDispatcher
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
}