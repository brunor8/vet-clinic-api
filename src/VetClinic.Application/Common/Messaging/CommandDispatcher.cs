namespace VetClinic.Application.Common.Messaging;

public sealed class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var wrapperType = typeof(HandlerWrapper<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        var wrapper = (HandlerWrapper<TResponse>)Activator.CreateInstance(wrapperType)!;

        return wrapper.Handle(command, serviceProvider, cancellationToken);
    }

    private abstract class HandlerWrapper<TResponse>
    {
        public abstract Task<TResponse> Handle(
            ICommand<TResponse> command, IServiceProvider provider, CancellationToken ct);
    }

    private sealed class HandlerWrapper<TCommand, TResponse> : HandlerWrapper<TResponse>
        where TCommand : ICommand<TResponse>
    {
        public override Task<TResponse> Handle(
            ICommand<TResponse> command, IServiceProvider provider, CancellationToken ct)
        {
            var handler = provider.GetService(typeof(ICommandHandler<TCommand, TResponse>))
                as ICommandHandler<TCommand, TResponse>
                ?? throw new InvalidOperationException(
                    $"Nenhum handler registrado para {typeof(TCommand).Name}.");

            return handler.Handle((TCommand)command, ct);
        }
    }
}