using System.Threading.Tasks;
using DillShock.Discord.Bot.EventHandlers.MessageReceived;
using Discord;
using Moq;
using NUnit.Framework;

namespace DillShock.Discord.Bot.Test.EventHandlers.MessageReceived;

[TestFixture]
public class OnlyLinksAllowedTests
{
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
        Mock<IMessage> socketMessageMoq = new Mock<IMessage>();
        socketMessageMoq.SetupGet(msg => msg.Content).Returns(message);
        
        // Act
        await OnlyLinksAllowed.MessageReceived(socketMessageMoq.Object);
        
        // Assert
        socketMessageMoq.Verify(service => service.DeleteAsync(null), Times.Never());
    }
    
    [TestCase("hello world")]
    public async Task ShouldDeleteMessagesWithoutUrl(string message)
    {
        // Arrange
        Mock<IUserMessage> socketMessageMoq = new Mock<IUserMessage>();
        
        Mock<IUser> currentUserMoq = new Mock<IUser>();
        currentUserMoq.SetupGet(user => user.IsBot).Returns(false);
        
        Mock<IMessageChannel> messageChannelMoq = new Mock<IMessageChannel>();
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
        await OnlyLinksAllowed.MessageReceived(socketMessageMoq.Object);

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