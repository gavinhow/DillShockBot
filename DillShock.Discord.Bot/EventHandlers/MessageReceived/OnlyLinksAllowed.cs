using System.Text.RegularExpressions;
using Discord;

namespace DillShock.Discord.Bot.EventHandlers.MessageReceived;

public static class OnlyLinksAllowed
{
    public static int DELETE_MESSAGE_DELAY = 10000;
    public static async Task MessageReceived(IMessage message)
    {
        // TODO: Restrict to certain channels
        if (!IsUrl(message.Content) && !message.Author.IsBot)
        {
            await message.DeleteAsync();
            await SendRichEmbedAsyncAndDelete(message.Channel);
        }
    }

    private static bool IsUrl(string text)
    {
        Regex r = new Regex(@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*");
        // Match the regular expression pattern against a text string.
        return r.IsMatch(text);
    }

    private static async Task SendRichEmbedAsyncAndDelete(IMessageChannel channel)
    {
        EmbedBuilder embed = new EmbedBuilder
        {
            Title = "Message Deleted!",
            Description = "Sorry pickle, you can only post URLs here."
        };

        CancellationTokenSource source = new CancellationTokenSource();
        IUserMessage? message = await channel.SendMessageAsync(embed: embed.Build());
        await Task.Delay(DELETE_MESSAGE_DELAY, source.Token);
        await message.DeleteAsync();
    }
}