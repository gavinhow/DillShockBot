using System.Threading.Tasks;
using DillShock.Discord.Bot.EventHandlers.MessageReceived;
using DillShock.Discord.Bot.Settings;
using Discord;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace DillShock.Discord.Bot.Test.EventHandlers.MessageReceived;

[TestFixture]
public class OnlyLinksAllowedTests
{
    private const string DefaultChannelName = "channel_name";

    [SetUp]
    public void SetUp()
    {
        OnlyLinksAllowed.DELETE_MESSAGE_DELAY = 5;
    }
    
    [TestCase("https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [TestCase("What do you think of this Dil? https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    public async Task ShouldIgnoreMessagesWithUrl(string message)
    {
        // Arrange
        IOptions<OnlyLinksAllowedOptions> options = Options.Create(new OnlyLinksAllowedOptions()
        {
            ChannelNames = new[] {DefaultChannelName}
        });
        OnlyLinksAllowed onlyLinksAllowed = new OnlyLinksAllowed(options);
        
        Mock<IUser> currentUserMoq = new Mock<IUser>();
        currentUserMoq.SetupGet(user => user.IsBot).Returns(false);
        
        Mock<IMessageChannel> messageChannelMoq = new Mock<IMessageChannel>();
        messageChannelMoq.Setup(msgChannel => msgChannel.Name).Returns(DefaultChannelName);

        Mock<IMessage> socketMessageMoq = new Mock<IMessage>();
        socketMessageMoq.SetupGet(msg => msg.Channel).Returns(messageChannelMoq.Object);
        socketMessageMoq.SetupGet(msg => msg.Content).Returns(message);
        socketMessageMoq.SetupGet(msg => msg.Author).Returns(currentUserMoq.Object);

        // Act
        await onlyLinksAllowed.MessageReceived(socketMessageMoq.Object);
        
        // Assert
        socketMessageMoq.Verify(service => service.DeleteAsync(null), Times.Never());
    }
    
    [TestCase("https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [TestCase("What do you think of this Dil? https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [TestCase("message without url")]
    public async Task ShouldIgnoreMessagesFromBot(string message)
    {
        // Arrange
        IOptions<OnlyLinksAllowedOptions> options = Options.Create(new OnlyLinksAllowedOptions()
        {
            ChannelNames = new[] {DefaultChannelName}
        });
        OnlyLinksAllowed onlyLinksAllowed = new OnlyLinksAllowed(options);
        
        Mock<IUser> currentUserMoq = new Mock<IUser>();
        currentUserMoq.SetupGet(user => user.IsBot).Returns(true);
        
        Mock<IMessageChannel> messageChannelMoq = new Mock<IMessageChannel>();
        messageChannelMoq.Setup(msgChannel => msgChannel.Name).Returns(DefaultChannelName);

        Mock<IMessage> socketMessageMoq = new Mock<IMessage>();
        socketMessageMoq.SetupGet(msg => msg.Channel).Returns(messageChannelMoq.Object);
        socketMessageMoq.SetupGet(msg => msg.Content).Returns(message);
        socketMessageMoq.SetupGet(msg => msg.Author).Returns(currentUserMoq.Object);

        // Act
        await onlyLinksAllowed.MessageReceived(socketMessageMoq.Object);
        
        // Assert
        socketMessageMoq.Verify(service => service.DeleteAsync(null), Times.Never());
    }
    
    [TestCase("https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [TestCase("What do you think of this Dil? https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    public async Task ShouldDeleteMessagesFromSpecifiedChannelNames(string message)
    {
        // Arrange
        IOptions<OnlyLinksAllowedOptions> options = Options.Create(new OnlyLinksAllowedOptions()
        {
            ChannelNames = new[] {DefaultChannelName}
        });
        OnlyLinksAllowed onlyLinksAllowed = new OnlyLinksAllowed(options);
        
        Mock<IUser> currentUserMoq = new Mock<IUser>();
        currentUserMoq.SetupGet(user => user.IsBot).Returns(false);
        
        Mock<IMessageChannel> messageChannelMoq = new Mock<IMessageChannel>();
        messageChannelMoq.Setup(msgChannel => msgChannel.Name).Returns(DefaultChannelName);

        Mock<IMessage> socketMessageMoq = new Mock<IMessage>();
        socketMessageMoq.SetupGet(msg => msg.Channel).Returns(messageChannelMoq.Object);
        socketMessageMoq.SetupGet(msg => msg.Content).Returns(message);
        socketMessageMoq.SetupGet(msg => msg.Author).Returns(currentUserMoq.Object);

        // Act
        await onlyLinksAllowed.MessageReceived(socketMessageMoq.Object);
        
        // Assert
        socketMessageMoq.Verify(service => service.DeleteAsync(null), Times.Never());
    }
    
    [TestCase("https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [TestCase("What do you think of this Dil? https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    public async Task ShouldDeleteMessagesFromSpecifiedChannelIds(string message)
    {
        // Arrange
        IOptions<OnlyLinksAllowedOptions> options = Options.Create(new OnlyLinksAllowedOptions()
        {
            ChannelNames = new[] {"966442808384815165"}
        });
        OnlyLinksAllowed onlyLinksAllowed = new OnlyLinksAllowed(options);
        
        Mock<IUser> currentUserMoq = new Mock<IUser>();
        currentUserMoq.SetupGet(user => user.IsBot).Returns(false);
        
        Mock<IMessageChannel> messageChannelMoq = new Mock<IMessageChannel>();
        messageChannelMoq.Setup(msgChannel => msgChannel.Id).Returns(966442808384815165);

        Mock<IMessage> socketMessageMoq = new Mock<IMessage>();
        socketMessageMoq.SetupGet(msg => msg.Channel).Returns(messageChannelMoq.Object);
        socketMessageMoq.SetupGet(msg => msg.Content).Returns(message);
        socketMessageMoq.SetupGet(msg => msg.Author).Returns(currentUserMoq.Object);

        // Act
        await onlyLinksAllowed.MessageReceived(socketMessageMoq.Object);
        
        // Assert
        socketMessageMoq.Verify(service => service.DeleteAsync(null), Times.Never());
    }

    [TestCase("https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [TestCase("What do you think of this Dil? https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [TestCase("message without url")]
    public async Task ShouldIgnoreMessagesFromNotSpecifiedChannels(string message)
    {
        // Arrange
        IOptions<OnlyLinksAllowedOptions> options = Options.Create(new OnlyLinksAllowedOptions()
        {
            ChannelNames = new[] {"Other channel name"}
        });
        OnlyLinksAllowed onlyLinksAllowed = new OnlyLinksAllowed(options);
        
        Mock<IUser> currentUserMoq = new Mock<IUser>();
        currentUserMoq.SetupGet(user => user.IsBot).Returns(false);
        
        Mock<IMessageChannel> messageChannelMoq = new Mock<IMessageChannel>();
        messageChannelMoq.Setup(msgChannel => msgChannel.Name).Returns(DefaultChannelName);

        Mock<IMessage> socketMessageMoq = new Mock<IMessage>();
        socketMessageMoq.SetupGet(msg => msg.Channel).Returns(messageChannelMoq.Object);
        socketMessageMoq.SetupGet(msg => msg.Content).Returns(message);
        socketMessageMoq.SetupGet(msg => msg.Author).Returns(currentUserMoq.Object);

        // Act
        await onlyLinksAllowed.MessageReceived(socketMessageMoq.Object);
        
        // Assert
        socketMessageMoq.Verify(service => service.DeleteAsync(null), Times.Never());
    }
    
    [TestCase("hello world")]
    public async Task ShouldDeleteMessagesWithoutUrl(string message)
    {
        // Arrange
        IOptions<OnlyLinksAllowedOptions> options = Options.Create(new OnlyLinksAllowedOptions()
        {
            ChannelNames = new[] {DefaultChannelName}
        });
        OnlyLinksAllowed onlyLinksAllowed = new OnlyLinksAllowed(options);
        
        // Arrange
        Mock<IUserMessage> socketMessageMoq = new Mock<IUserMessage>();
        
        Mock<IUser> currentUserMoq = new Mock<IUser>();
        currentUserMoq.SetupGet(user => user.IsBot).Returns(false);
        
        Mock<IMessageChannel> messageChannelMoq = new Mock<IMessageChannel>();
        messageChannelMoq.Setup(msgChannel => msgChannel.Name).Returns(DefaultChannelName);
        messageChannelMoq.Setup(msgChan => msgChan.SendMessageAsync(null,
            false,
            It.IsAny<Embed>(),
            null,
            null,
            null,
            null,
            null,
            null,
            MessageFlags.None)).ReturnsAsync(socketMessageMoq.Object);
        
        socketMessageMoq.SetupGet(msg => msg.Content).Returns(message);
        socketMessageMoq.SetupGet(msg => msg.Author).Returns(currentUserMoq.Object);
        socketMessageMoq.SetupGet(msg => msg.Channel).Returns(messageChannelMoq.Object);

        // Act
        await onlyLinksAllowed.MessageReceived(socketMessageMoq.Object);

        // Assert
        socketMessageMoq.Verify(service => service.DeleteAsync(null), Times.AtLeastOnce());
        messageChannelMoq.Verify(service => 
            service.SendMessageAsync(
                null, 
                false, 
                It.IsAny<Embed>(),
                null,
                null,
                null,
                null,
                null,
                null,
                MessageFlags.None
                ), Times.Once
        ());
    }
}