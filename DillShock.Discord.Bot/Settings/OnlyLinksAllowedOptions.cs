namespace DillShock.Discord.Bot.Settings;

public class OnlyLinksAllowedOptions
{
    public const string OnlyLinksAllowed = "OnlyLinksAllowed";

    public string[] ChannelNames { get; set; } = Array.Empty<string>();
}