using Microsoft.Extensions.DependencyInjection;
using GamesDiscounts.Bot;
using GamesDiscounts.Bot.Commands;
using GamesDiscounts.Models;
using GamesDiscounts.Services;
using Telegram.Bot;

namespace GamesDiscounts
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient("8002657900:AAEu1_d3RQ2ZJS15stf23n3ywJqYNdZRYIY"));


            services.AddSingleton<BotMessage>();
            services.AddSingleton<GameAlerts>();
            services.AddSingleton<GameInfo>();
            services.AddSingleton<SavedGamesService>();
            services.AddSingleton<AlertNotifier>();
            services.AddSingleton<SqlSaveDB>();

            services.AddSingleton<IAlert, GameAlerts>();
            services.AddSingleton<ISavedGamesService, SavedGamesService>();
            services.AddSingleton<IGameInfo, GameInfo>();
            services.AddSingleton<IDataBase, SqlSaveDB>();
            services.AddSingleton<IHttpService, HttpService>();

            services.AddSingleton<ICommandHandler, Start>();
            services.AddSingleton<ICommandHandler, Search>();
            services.AddSingleton<ICommandHandler, ExactSearch>();
            services.AddSingleton<ICommandHandler, Save>();
            services.AddSingleton<ICommandHandler, Sale>();
            services.AddSingleton<ICommandHandler, Delete>();
            services.AddSingleton<ICommandHandler, Alerts>();
            services.AddSingleton<ICommandHandler, AlertsOff>();

            services.AddSingleton<Alerts>();
            services.AddSingleton<AlertsOff>();
            services.AddSingleton<Delete>();
            services.AddSingleton<ExactSearch>();
            services.AddSingleton<Sale>();
            services.AddSingleton<Save>();
            services.AddSingleton<Search>();
            services.AddSingleton<Start>();




            services.AddSingleton<CommandDispatcher>();
            services.AddSingleton<TgBot>();

            var provider = services.BuildServiceProvider();

            var bot = provider.GetRequiredService<TgBot>();
            await bot.RunAsync();
        }
    }
}
