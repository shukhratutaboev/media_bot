using System.Runtime.InteropServices;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace media_bot.Services
{
    public class BotHandlers
    {
        private readonly ILogger<BotHandlers> _logger;
        private readonly IStorageService _storage;
        private readonly PixabayClient _client;

        public BotHandlers(ILogger<BotHandlers> logger, IStorageService storage, PixabayClient client)
        {
            _logger = logger;
            _storage = storage;
            _client = client;
        }
        public Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken ctoken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException => $"Error occured with Telegram Client: {exception.Message}",
                _ => exception.Message
            };
            _logger.LogCritical(errorMessage);
            return Task.CompletedTask;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken ctoken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageRecieved(client, update.Message),
                _ => UnknownUpdateHandler(client, update)
            };
            try
            {
                await handler;
            }
            catch(Exception e)
            {
                _logger.LogWarning(e.Message);
            }
        }

        private Task UnknownUpdateHandler(ITelegramBotClient client, Update update)
        {
            throw new Exception("This type update can't be handled.");
        }

        private async Task BotOnMessageRecieved(ITelegramBotClient client, Message? message)
        {
            var user = new Entities.User(){};
            if(_storage.ExistsAsync(message.Chat.Id).Result)
            {
                user = (await _storage.GetAsync(message.Chat.Id)).user;
            }
            if(!user.InProcess)
            {
                if(message.Text == "/start")
                {
                    if(!(await _storage.ExistsAsync(message.Chat.Id)))
                    {
                        var newuser = new Entities.User(
                            message.Chat.Id,
                            message.From.Username,
                            message.From.FirstName + " " + message.From.LastName){};
                        await _storage.InsertAsync(newuser);
                    }
                    await client.SendTextMessageAsync(
                        message.Chat.Id,
                        "Salom, bu bot siz izlagan rasm yoki videoni topib beradi.",
                        replyMarkup: Buttons.Choices()
                    );
                }
                if(message.Text == "Video")
                {
                    user.InProcess = true;
                    user.ContentType = "video";
                    await _storage.UpdateAsync(user);
                    await client.SendTextMessageAsync(
                        user.ChatId,
                        "Siz izlayotgan mavzuni ingliz tilida kiriting:"
                    );
                }
                if(message.Text == "Photo")
                {
                    user.InProcess = true;
                    user.ContentType = "photo";
                    await _storage.UpdateAsync(user);
                    await client.SendTextMessageAsync(
                        user.ChatId,
                        "Siz izlayotgan mavzuni ingliz tilida kiriting:"
                    );
                }
            }
            // _client Pixabay API client
            // client Telegram API client
            else
            {
                user.InProcess = false;
                await _storage.UpdateAsync(user);
                if(user.ContentType == "video")
                {
                    try
                    {
                        var video = await _client.GetVideoAsync(message.Text.ToLower());
                        int i = 0;
                        foreach(var v in video.video.Hits)
                        {
                            if(i == 10) break;
                            await client.SendAnimationAsync(
                                user.ChatId,
                                v.Videos.Tiny.Url
                            );
                            i++;
                        }
                    }
                    catch(Exception e)
                    {
                        _logger.LogWarning(e.Message);
                        await client.SendTextMessageAsync(
                            user.ChatId,
                            "Topilmadi("
                        );
                    }
                }
                else
                {
                    try
                    {
                        var photo = await _client.GetPhotoAsync(message.Text.ToLower());
                        int i = 0;
                        foreach(var p in photo.photo.Hits)
                        {
                            if(i == 10) break;
                            await client.SendPhotoAsync(
                                user.ChatId,
                                p.LargeImageURL
                            );
                            i++;
                        }
                    }
                    catch(Exception e)
                    {
                        _logger.LogWarning(e.Message);
                        await client.SendTextMessageAsync(
                            user.ChatId,
                            "Topilmadi("
                        );
                    }
                }
            }
        }
    }
}