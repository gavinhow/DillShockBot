using DillShock.Discord.Bot;
using DillShock.Discord.Bot.EventHandlers.MessageReceived;
using DillShock.Discord.Bot.Settings;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((hostContext, services) =>
{
    IConfiguration configuration = hostContext.Configuration;
    services.AddHostedService<Worker>();
    services.AddSingleton<OnlyLinksAllowed, OnlyLinksAllowed>();

    services.Configure<DiscordOptions>(configuration.GetSection(DiscordOptions.Discord));
    services.Configure<OnlyLinksAllowedOptions>(configuration.GetSection(OnlyLinksAllowedOptions.OnlyLinksAllowed));
});



IHost host= builder.Build();


await host.RunAsync();