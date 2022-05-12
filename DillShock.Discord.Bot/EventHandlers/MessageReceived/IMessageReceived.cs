using Discord;

namespace DillShock.Discord.Bot.EventHandlers.MessageReceived;

public interface IMessageReceived
{
    Task MessageReceived(IMessage message);
}