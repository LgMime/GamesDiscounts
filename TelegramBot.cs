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
       for (int i = 0; i < 3; i++)
       {
            await bot.SendMessage(
                chatId: msg.Chat.Id,
                text: "Hello, World! " + i,
                cancellationToken: cts.Token
            );
       }
    }
    else if (msg.Text == "/Stop")
    {
        // somehow stop the bot
    }
    else
    {

        GameInfo gameInfo = new GameInfo(); 
        await bot.SendMessage(msg.Chat, "Seaching Game");
        string gameName = msg.Text;

        await gameInfo.FoundGameAppId(gameName);
        await bot.SendMessage(msg.Chat, "Final price: " + gameInfo.finalPrice + "\n Discount: " + gameInfo.discount);
    }
}