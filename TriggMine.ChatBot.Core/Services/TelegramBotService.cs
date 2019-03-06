using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Telegram.Bot;
using Telegram.Bot.Args;
using TriggMine.ChatBot.Core.Services.Interfaces;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Repository.Repository;
using TriggMine.ChatBot.Shared.DTO;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace TriggMine.ChatBot.Core.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ILogger<TelegramBotService> _logger;
        private readonly TelegramBotClient _telegramBot;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly IResolverUrlService _resolverUrlService;
        private readonly IAzureMachineLearningService _azureMachineLearningService;
        private readonly string _apiKey;
        private readonly string _basePathImageFolder;
        private readonly bool _IsEnableAML;

        public TelegramBotService(ILogger<TelegramBotService> logger
            , IConfiguration configuration
            , IUserService userService
            , IMessageService messageService
            , IResolverUrlService resolverUrlService
            , IAzureMachineLearningService azureMachineLearningService
            )
        {
            _logger = logger;
            _userService = userService;
            _messageService = messageService;
            _resolverUrlService = resolverUrlService;
            _azureMachineLearningService = azureMachineLearningService;
            _telegramBot = new TelegramBotClient(configuration["TelegramBotToken"]);           
            _basePathImageFolder = configuration["BasePathImageFolder"];
            _IsEnableAML = Boolean.Parse(configuration["IsEnableAML"]);
            _apiKey = configuration["YandexApiKey"];
        }

        public async Task GetBot()
        {
            var me = await _telegramBot.GetMeAsync();

            _logger.LogInformation($"Name bot: {me.FirstName}");

            _telegramBot.OnUpdate += ReadMessage;
            _telegramBot.StartReceiving();
            await _telegramBot.SetWebhookAsync("");          
        }

        async void ReadMessage(object sender, UpdateEventArgs updateEvent)
        {
            if (updateEvent.Update.Message == null)
                return;

            await AddUser(updateEvent);
            await AddMessage(updateEvent);
          
            if ((await _userService.FindUser(c => c.UserId == updateEvent.Update.Message.From.Id)).IsBlocked == true)
            {
                await DeleteMessage(updateEvent);
                return;
            }

            if (updateEvent.Update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo && _IsEnableAML)
                await CheckPhotoMessageMachineLearning(updateEvent);

            if (!string.IsNullOrEmpty(updateEvent.Update.Message.Text))
            {
                await CheckTextMessage(updateEvent);
                await CheckResolvedUrls(updateEvent);
            }

            ////Delete ServiceMessage ALL
            //if (updateEvent.Update.Message.Type == Telegram.Bot.Types.Enums.MessageType.ServiceMessage)
            //{
            //    await DeleteMessage(updateEvent);
            //}            
        }

        #region Private

        private async Task CheckPhotoMessageMachineLearning(UpdateEventArgs updateEvent)
        {
            var path = DownloadFileFromChat(updateEvent.Update.Message.Photo[updateEvent.Update.Message.Photo.Length - 1].FileId);

            var analysisResult = await _azureMachineLearningService.AnalyzePhotoMLAzureAsync(path);

            if (!string.IsNullOrEmpty(analysisResult))
            {
                var resultTranslate = await CommandChatExtMethods.TranslateMessage(analysisResult, _apiKey);
                await _telegramBot.SendTextMessageAsync(updateEvent.Update.Message.Chat.Id, $"Я думаю, что на фото изображено: <b>{resultTranslate}</b>", Telegram.Bot.Types.Enums.ParseMode.Html);
            }
        }

        private async Task CheckTextMessage(UpdateEventArgs updateEvent)
        {
            switch (updateEvent.Update.Message.Text?.Split(' ').First())
            {
                case "/kick":
                    await _telegramBot.KickUserChatAsync(updateEvent);
                    break;
                case "/promote":
                    await _telegramBot.PromoteUserChatAsync(updateEvent);
                    break;
                case "/ban":
                    await _telegramBot.BanUserChatAsync(updateEvent);
                    break;
                case "/unban":
                    await _telegramBot.UnBanUserChatAsync(updateEvent);
                    break;
                case var someVal when new Regex(@"[#]+").IsMatch(someVal):
                    await _telegramBot.GetImageAndSentToChat(updateEvent);
                    break;
                case var someVal when new Regex(@"[*]+").IsMatch(someVal):
                    await _telegramBot.TranslateMessageAndSend(updateEvent, _apiKey);
                    break;
                case "/help":
                    await _telegramBot.GetHelp(updateEvent);
                    break;
                case "/menu":
                    await _telegramBot.GetMenuButton(updateEvent);
                    break;
                case var expression when ((updateEvent.Update.Message.Text?.Split(' ').First().ToString()).Contains("Сиськи")):
                    await _telegramBot.GetImageAndSentToChat(updateEvent);
                    break;
                case var expression when ((updateEvent.Update.Message.Text?.Split(' ').First().ToString()).Contains("Попки")):
                    await _telegramBot.GetImageAndSentToChat(updateEvent);
                    break;
                case var expression when ((updateEvent.Update.Message.Text?.Split(' ').First().ToString()).Contains("Порно")):
                    await _telegramBot.GetImageAndSentToChat(updateEvent);
                    break;
                case var expression when ((updateEvent.Update.Message.Text?.Split(' ').First().ToString()).Contains("Шлюхи")):
                    await _telegramBot.GetImageAndSentToChat(updateEvent);
                    break;
                default:                  
                    break;
            }
        }

        private async Task CheckResolvedUrls(UpdateEventArgs updateEvent)
        {
            var urls = GetLinks(updateEvent.Update.Message.Text);
            if (urls.Count > 0)
            {
                var resolverUrls = (await _resolverUrlService.GetResolvedUrlsListAsync()).Select(c => c.Url).ToList();

                //  var isIncluded = resolverUrls.Intersect(urls).Any();
                var isIncluded = urls.Except(resolverUrls).Any();

                if (!isIncluded)
                {
                    await DeleteMessage(updateEvent);
                }
                //  await _telegramBot.SendTextMessageAsync(updateEvent.Update.Message.Chat.Id, $"isIncluded: {isIncluded}");
            }
            if (updateEvent.Update.Message.Text.ToLower().Contains("хуй"))
            {
                await DeleteMessage(updateEvent);
                // await BlockUser(updateEvent.Update.Message.From.Id);
            }
        }

        private string DownloadFileFromChat(string fileId)
        {
            var filePath = _telegramBot.GetFileAsync(fileId).Result.FilePath;
            //var imageUid = $"d:\\{filePath}".Replace('/', '\\');
            var imageUid = $"{_basePathImageFolder}\\{filePath}".Replace('/', '\\');

            try
            {
                using (var saveImageStream = new FileStream(imageUid, FileMode.Create))
                {
                    var file = _telegramBot.GetInfoAndDownloadFileAsync(fileId, saveImageStream).Result;
                    if (file.FilePath != null)
                    {
                        return imageUid;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error downloading: " + ex.Message);
                return null;
            }
        }

        private List<string> GetLinks(string message)
        {
            List<string> list = new List<string>();
            string pattern = @"\(?(?:(http|https|ftp):\/\/)?(?:((?:[^\W\s]|\.|-|[:]{1})+)@{1})?((?:www.)?(?:[^\W\s]|\.|-)+[\.][^\W\s]{2,4}|localhost(?=\/)|\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})(?::(\d*))?([\/]?[^\s\?]*[\/]{1})*(?:\/?([^\s\n\?\[\]\{\}\#]*(?:(?=\.)){1}|[^\s\n\?\[\]\{\}\.\#]*)?([\.]{1}[^\s\?\#]*)?)?(?:\?{1}([^\s\n\#\[\]]*))?([\#][^\s\n]*)?\)?";
            Regex urlRx = new Regex(pattern, RegexOptions.IgnoreCase);

            MatchCollection matches = urlRx.Matches(message);
            foreach (Match match in matches)
            {
                list.Add($"{match.Groups[1]}://{ match.Groups[3].Value.Replace("www.", "").Trim()}");
            }
            return list;
        }

        private async Task DeleteMessage(UpdateEventArgs updateEvent)
        {
            try
            {
                await _telegramBot.DeleteMessageAsync(updateEvent.Update.Message.Chat.Id, updateEvent.Update.Message.MessageId);
                //  await _telegramBot.SendTextMessageAsync(updateEvent.Update.Message.Chat.Id, $"{updateEvent.Update.Message.From.FirstName} {updateEvent.Update.Message.From.LastName}, Ваше сообщение было удалено из-за нарушения политики безопастности!");
                await _telegramBot.SendTextMessageAsync(updateEvent.Update.Message.Chat.Id, $"{updateEvent.Update.Message.From.FirstName} {updateEvent.Update.Message.From.LastName}, стапэ... или пойдешь на хуй с мопеда!");
            }
            catch (Exception)
            {
                _logger.LogError($"An error occurred while deleting the message or it was deleted earlier!");
            }

        }

        private async Task AddUser(UpdateEventArgs updateEvent)
        {

            await _userService.CreateUser(new UserDTO()
            {
                FirstName = updateEvent.Update.Message.From.FirstName,
                LastName = updateEvent.Update.Message.From.LastName,
                IsBot = updateEvent.Update.Message.From.IsBot,
                LanguageCode = updateEvent.Update.Message.From.LanguageCode,
                Username = updateEvent.Update.Message.From.Username,
                UserId = updateEvent.Update.Message.From.Id
            });
        }

        private async Task AddMessage(UpdateEventArgs updateEvent)
        {
            await _messageService.CreateMessage(new MessageDTO()
            {
                ChatId = updateEvent.Update.Message.Chat.Id,
                MessageId = updateEvent.Update.Message.MessageId,
                Text = updateEvent.Update.Message.Text,
                UserId = updateEvent.Update.Message.From.Id,
                ChatTitle = updateEvent.Update.Message.Chat.Title,
                Type = updateEvent.Update.Message.Type.ToString()
            });
        }

        private async Task BlockUser(int userId)
        {
            await _userService.BlockUser(userId);
        }

        #endregion

    }
}
