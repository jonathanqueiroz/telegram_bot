using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    internal class Program
    {
        private static readonly string BotToken = "token";
        private static readonly TelegramBotClient botClient = new TelegramBotClient(BotToken);

        static void Main(string[] args)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message, UpdateType.CallbackQuery }
            };

            botClient.StartReceiving(
                updateHandler: Bot_OnUpdate,
                errorHandler: Bot_OnError,
                receiverOptions: receiverOptions
            );

            Console.WriteLine("Bot iniciado!");
            Console.ReadLine(); // Manter o bot rodando
        }

        private static async Task Bot_OnUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message != null)
            {
                await Bot_OnMessage(update.Message);
            }
            else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                await Bot_OnCallbackQuery(update.CallbackQuery);
            }
        }

        private static async Task Bot_OnMessage(Message message)
        {
            if (message == null || message.From == null)
            {
                return;
            }

            Console.WriteLine($"Mensagem recebida do User ID: {message.From.Id}");
            Console.WriteLine($"Username: {message.From.Username}");
            Console.WriteLine($"Nome: {message.From.FirstName} {message.From.LastName}");
            Console.WriteLine($"Mensagem: {message.Text}");

            if (message.Text == "/start")
            {
                await SendMessage(message, "Olá seja bem-vindo ao bot do Distrito, o que você deseja saber hoje?");
            }
            else if (message.Text == "/opcao")
            {
                string responseMessage = "Vou te dar algumas opções, favor clicar na que melhor te atenda:";

                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Relatório", "report"),
                        InlineKeyboardButton.WithCallbackData("Gráfico", "graph"),
                        InlineKeyboardButton.WithCallbackData("Texto", "text")
                    }
                });

                await botClient.SendMessage(
                    chatId: message.Chat,
                    text: responseMessage,
                    replyMarkup: keyboard
                );
            }
            else if (message.Text == "/imagem")
            {
                string imageUrl = "https://picsum.photos/480/300";
                await SendPhoto(message, imageUrl, "Aqui está a imagem que você pediu!");
            }
            else
            {
                await SendMessage(message, "Sua mensagem é: " + message.Text);
            }
        }

        private static async Task Bot_OnCallbackQuery(CallbackQuery callbackQuery)
        {
            Console.WriteLine($"Callback recebido do User ID: {callbackQuery.From.Id}");
            Console.WriteLine($"Callback: {callbackQuery.Message}");
            Console.WriteLine($"Data: {callbackQuery.Data}");

            if (callbackQuery.Message == null)
            {
                return;
            }

            if (callbackQuery.Data == "report")
            {
                await SendMessage(callbackQuery.Message, "Buscando seu relatório...");
                await SendDocument(callbackQuery.Message, "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf", "Aqui está o relatório que você pediu!");
            }
            else if (callbackQuery.Data == "graph")
            {
                await SendMessage(callbackQuery.Message, "Buscando seu gráfico...");
                await SendPhoto(callbackQuery.Message, "https://picsum.photos/480/300", "Aqui está o gráfico que você pediu!");
            }
            else if (callbackQuery.Data == "text")
            {
                await SendMessage(callbackQuery.Message, "Pesquisando...");
                await SendMessage(callbackQuery.Message, "Aqui está sua resposta!");
            }
        }

        private static async Task<Telegram.Bot.Types.Message> SendMessage(Message message, string responseMessage)
        {
            return await botClient.SendMessage(
                chatId: message.Chat,
                text: responseMessage
            );
        }

        private static async Task SendPhoto(Message message, string imageUrl, string caption)
        {
            await botClient.SendPhoto(
                chatId: message.Chat.Id,
                photo: imageUrl,
                caption: caption
            );
        }

        private static async Task SendDocument(Message message, string documentUrl, string caption)
        {
            await botClient.SendDocument(
                chatId: message.Chat.Id,
                document: documentUrl,
                caption: caption
            );
        }

        private static Task Bot_OnError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"An error occurred: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}