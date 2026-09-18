using GamesDiscounts.Bot;
using GamesDiscounts.Bot.Commands;
using GamesDiscounts.Models;
using GamesDiscounts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace GamesDiscounts
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string botToken = configuration["BotToken"];

            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));


            services.AddSingleton<BotMessage>();
            services.AddSingleton<GameAlerts>();
            services.AddSingleton<IAlert>(sp => sp.GetRequiredService<GameAlerts>()); 
            services.AddSingleton<ISavedGamesService, SavedGamesService>();
            services.AddSingleton<IGameInfo, GameInfo>();
            services.AddSingleton<IDataBase, SqlSaveDB>();
            services.AddSingleton<HttpService>();
            services.AddSingleton<AlertNotifier>();

        
            services.AddSingleton<ICommandHandler, Start>();
            services.AddSingleton<ICommandHandler, Search>();
            services.AddSingleton<ICommandHandler, ExactSearch>();
            services.AddSingleton<ICommandHandler, Save>();
            services.AddSingleton<ICommandHandler, Sale>();
            services.AddSingleton<ICommandHandler, Delete>();
            services.AddSingleton<ICommandHandler, Alerts>();
            services.AddSingleton<ICommandHandler, AlertsOff>();

            services.AddSingleton<CommandDispatcher>();
            services.AddSingleton<TgBot>();




            services.AddSingleton<CommandDispatcher>();
            services.AddSingleton<TgBot>();



            var provider = services.BuildServiceProvider();


            var bot = provider.GetRequiredService<TgBot>();
            await bot.RunAsync();
        }
    }
}
