using System.Text.RegularExpressions;
using DillShock.Discord.Bot.Settings;
using Discord;
using Microsoft.Extensions.Options;

namespace DillShock.Discord.Bot.EventHandlers.MessageReceived;

public class OnlyLinksAllowed : IMessageReceived
{
    public static int DELETE_MESSAGE_DELAY = 10000;
    private readonly OnlyLinksAllowedOptions _onlyLinksAllowedOptions;

    public OnlyLinksAllowed(IOptions<OnlyLinksAllowedOptions> options)
    {
        _onlyLinksAllowedOptions = options.Value;
    }
    
    public async Task MessageReceived(IMessage message)
    {
        // TODO: Restrict to certain channels
        if (!message.Author.IsBot && _onlyLinksAllowedOptions.ChannelNames.Contains(message.Channel.Name) && !IsUrl(message
        .Content) )
        {
            await message.DeleteAsync();
            SendRichEmbedAsyncAndDelete(message.Channel);
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