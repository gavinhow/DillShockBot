using DillShock.Discord.Bot.EventHandlers.MessageReceived;
using DillShock.Discord.Bot.Settings;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace DillShock.Discord.Bot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly OnlyLinksAllowed _onlyLinksAllowed;
    private readonly DiscordSocketClient _client;
    private readonly DiscordOptions _options;

    public Worker(ILogger<Worker> logger, IOptions<DiscordOptions> options, OnlyLinksAllowed onlyLinksAllowed)
    {
        _client = new DiscordSocketClient();
        _options = options.Value;
        _logger = logger;
        _onlyLinksAllowed = onlyLinksAllowed;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.Log += Log;
        await _client.LoginAsync(TokenType.Bot, _options.token);
        await _client.StartAsync();
            
        _client.MessageReceived += _onlyLinksAllowed.MessageReceived;
        _client.Ready += () =>
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        };
            
        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}