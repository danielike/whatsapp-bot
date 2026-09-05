namespace WhatsappBotTests;

using System.Net;
using WhatsappBot.Command;
using WhatsappBot.ExternalApis.Evolution;
using WhatsappBot.Options;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public sealed class BuzzServiceTests
{
    [Fact]
    public async Task StartBuzz_ShouldSendConfiguredBuzzMessage()
    {
        // Arrange
        const string id = "group-123";
        const string mention = "@user";
        const string buzzMessage = "Buzz message";

        var messageSent = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        var evolutionApiService = new Mock<IEvolutionApiService>();

        evolutionApiService
            .Setup(x => x.SendMessage(
                id,
                $"{mention}\n{buzzMessage}",
                mention))
            .Callback(() => messageSent.TrySetResult(true))
            .Returns(Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            }));

        var scope = CreateScope(evolutionApiService.Object);

        var scopeFactory = new Mock<IServiceScopeFactory>();

        scopeFactory
            .Setup(x => x.CreateScope())
            .Returns(scope.Object);

        var configuration = new Mock<IOptionsMonitor<ConfigurationOptions>>();

        configuration
            .SetupGet(x => x.CurrentValue)
            .Returns(new ConfigurationOptions
            {
                BuzzMessage = buzzMessage
            });

        var sut = new BuzzService(
            scopeFactory.Object,
            configuration.Object);

        // Act
        var buzzId = sut.StartBuzz(
            TimeSpan.FromMilliseconds(20),
            id,
            mention);

        // Assert
        Assert.NotEqual(Guid.Empty, buzzId);

        var completedTask = await Task.WhenAny(
            messageSent.Task,
            Task.Delay(TimeSpan.FromSeconds(2)));

        Assert.Same(messageSent.Task, completedTask);

        evolutionApiService.Verify(
            x => x.SendMessage(
                id,
                $"{mention}\n{buzzMessage}",
                mention),
            Times.AtLeastOnce);

        scopeFactory.Verify(
            x => x.CreateScope(),
            Times.Once);

        // Cleanup the background operation started by StartBuzz.
        sut.Stop(buzzId);
    }

    [Fact]
    public async Task StopAll_ShouldCancelAllActiveBuzzes()
    {
        // Arrange
        var evolutionApiService = new Mock<IEvolutionApiService>();

        evolutionApiService
            .Setup(x => x.SendMessage(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            }));

        var scope = CreateScope(evolutionApiService.Object);

        var scopeFactory = new Mock<IServiceScopeFactory>();

        scopeFactory
            .Setup(x => x.CreateScope())
            .Returns(scope.Object);

        var configuration = new Mock<IOptionsMonitor<ConfigurationOptions>>();

        configuration
            .SetupGet(x => x.CurrentValue)
            .Returns(new ConfigurationOptions
            {
                BuzzMessage = "Buzz message"
            });

        var sut = new BuzzService(
            scopeFactory.Object,
            configuration.Object);

        var interval = TimeSpan.FromMilliseconds(100);

        sut.StartBuzz(interval, "id-1", "@user1");
        sut.StartBuzz(interval, "id-2", "@user2");
        sut.StartBuzz(interval, "id-3", "@user3");

        // Act
        sut.StopAll();

        // Wait longer than the timer interval. If StopAll did not cancel
        // the buzzes, messages would be sent during this period.
        await Task.Delay(TimeSpan.FromMilliseconds(250));

        // Assert
        evolutionApiService.Verify(
            x => x.SendMessage(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    private static Mock<IServiceScope> CreateScope(
        IEvolutionApiService evolutionApiService)
    {
        var serviceProvider = new Mock<IServiceProvider>();

        serviceProvider
            .Setup(x => x.GetService(typeof(IEvolutionApiService)))
            .Returns(evolutionApiService);

        var scope = new Mock<IServiceScope>();

        scope
            .SetupGet(x => x.ServiceProvider)
            .Returns(serviceProvider.Object);

        return scope;
    }
}
