using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using GamesDiscounts;
using GameInfo = GamesDiscounts.GameInfo;

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient("8002657900:AAEu1_d3RQ2ZJS15stf23n3ywJqYNdZRYIY", cancellationToken: cts.Token);
var me = await bot.GetMe();
bot.OnError += OnError;
bot.OnMessage += OnMessage;
Console.ReadLine();
cts.Cancel();

// method to handle errors in polling or in your OnMessage/OnUpdate code
async Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine(exception);
}

// method that handle messages received by the bot:
async Task OnMessage(Message msg, UpdateType type)
{
    if (msg.Text == "/Start")
    {
        await bot.SendMessage(msg.Chat, "Write me the name of the game and I will show you a discount on it:");
    }
    //else if (msg.Text == "/Sale")
    //{
    //    // send a message with the current sale games waht`s player searching befor. I need serialization of all games waht's player searching and then send it to player
    //}
    else
    {
        GameInfo gameInfo = new GameInfo();
        await bot.SendMessage(msg.Chat, "Seaching Game");
        string gameName = msg.Text;

        await gameInfo.FoundGameAppIdAsync(gameName);
        await bot.SendPhoto(chatId: msg.Chat.Id, photo: gameInfo.HeaderImage.Media, caption: $"Final price: {gameInfo.FinalPrice}\nDiscount:{gameInfo.Discount}%");
    }

}