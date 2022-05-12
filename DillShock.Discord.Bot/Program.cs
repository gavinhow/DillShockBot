using DillShock.Discord.Bot;
using DillShock.Discord.Bot.Settings;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((hostContext, services) =>
{
    IConfiguration configuration = hostContext.Configuration;
    services.AddHostedService<Worker>();

    services.Configure<DiscordOptions>(configuration.GetSection(DiscordOptions.Discord));
});



IHost host= builder.Build();


await host.RunAsync();