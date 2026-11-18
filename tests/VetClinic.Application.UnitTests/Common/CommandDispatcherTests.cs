using VetClinic.Application.Common.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace VetClinic.Application.UnitTests.Common;
public class CommandDispatcherTests
{
    private sealed record PingCommand(string Text) : ICommand<string>;
    private sealed class PingCommandHandler : ICommandHandler<PingCommand, string>
    {
        public Task<string> Handle(PingCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult($"ping: {command.Text}");
        }
    }

    [Fact]
    public async Task Send_WithRegisteredHandler_ShouldReturnHandlerResponse()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ICommandHandler<PingCommand, string>, PingCommandHandler>();
        var dispatcher = new CommandDispatcher(serviceCollection.BuildServiceProvider());

        var response = await dispatcher.Send(new PingCommand("Hello"));

        // Assert
        Assert.Equal("ping: Hello", response);
    }

    [Fact]
    public async Task Send_WithoutRegisteredHandler_ShouldThrow()
    {
        var dispatcher = new CommandDispatcher(new ServiceCollection().BuildServiceProvider());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            dispatcher.Send(new PingCommand("Hello")));

        Assert.Contains(nameof(PingCommand), ex.Message);
    }

    [Fact]
    public async Task Send_WithNullCommand_ShouldThrow()
    {
        var dispatcher = new CommandDispatcher(new ServiceCollection().BuildServiceProvider());

        await Assert.ThrowsAsync<ArgumentNullException>(() => dispatcher.Send<string>(null!));
    }

}