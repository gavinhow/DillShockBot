using DillShock.Discord.Bot.EventHandlers.MessageReceived;
using DillShock.Discord.Bot.Settings;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace DillShock.Discord.Bot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    
    private DiscordSocketClient _client;

    private DiscordOptions _options;

    public Worker(ILogger<Worker> logger, IOptions<DiscordOptions> options)
    {
        _client = new DiscordSocketClient();
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.Log += Log;
            
        //  You can assign your bot token to a string, and pass that in to connect.
        //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
        
            
        // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
        // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
        // var token = File.ReadAllText("token.txt");
        // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

        await _client.LoginAsync(TokenType.Bot, _options.token);
        await _client.StartAsync();
            
        _client.MessageReceived += OnlyLinksAllowed.MessageReceived;
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