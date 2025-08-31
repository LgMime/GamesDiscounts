using Telegram.Bot;
using Telegram.Bot.Types;
namespace GamesDiscounts.Models
{
    public interface ICommandHandler
    {
        string Command { get; }
       

        Task HandleCommand(ITelegramBotClient bot, Update update, CancellationToken token);
        Task HandleInput(ITelegramBotClient bot, Update update, CancellationToken token);

        bool RequiresInput => true;
    }
}
