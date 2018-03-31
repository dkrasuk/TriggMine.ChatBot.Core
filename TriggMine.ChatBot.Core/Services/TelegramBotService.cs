using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Repository.Repository;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private ILogger<TelegramBotService> _logger;
        private readonly TelegramBotClient _telegramBot;
        private readonly IChatBotRepository<User> _userRepository;

        public TelegramBotService(ILogger<TelegramBotService> logger
            , IConfiguration configuration
            , IChatBotRepository<User> userRepository
            )
        {
            _logger = logger;
            _userRepository = userRepository;
            _telegramBot = new TelegramBotClient(configuration["TelegramBotToken"]);
        }

        public async Task GetBot()
        {

            var me = await _telegramBot.GetMeAsync();
            _logger.LogInformation($"Name bot: {me.FirstName}");

            //   _telegramBot.OnMessage += HandleMessage;
            _telegramBot.OnUpdate += ReadMessage;
            _telegramBot.StartReceiving();


            await _telegramBot.SetWebhookAsync("");



            _logger.LogInformation(me.FirstName);
        }

        void ReadMessage(object sender, UpdateEventArgs updateEvent)
        {



            //_messageService.CreateMessage(new MessageDTO()
            //{
            //    ChatId = updateEvent.Update.Message.Chat.Id,
            //    MessageId = updateEvent.Update.Message.MessageId,
            //    Text = updateEvent.Update.Message.Text,
            //    UserId = updateEvent.Update.Message.From.Id                
            //});


            Get(updateEvent);


            var update = updateEvent.Update;
            var message = update.Message;

            var chatId = updateEvent.Update.Message.Chat.Id;
            var messageId = updateEvent.Update.Message.MessageId;
            var userId = updateEvent.Update.Message.From.Id;


            if (message.Text == null)
                return;

            if (message.Text.Contains("хуй"))
            {

                DeleteMessage(updateEvent);

            }

            if (userId == 213417044)
            {
                DeleteMessage(updateEvent);

            }

            //else
            //{
            //    _telegramBot.OnMessage += HandleMessage;
            //}

        }


        async void DeleteMessage(UpdateEventArgs updateEvent)
        {
            var chatId = updateEvent.Update.Message.Chat.Id;
            var messageId = updateEvent.Update.Message.MessageId;
            var userName = $"{updateEvent.Update.Message.From.FirstName} {updateEvent.Update.Message.From.LastName}";
            await _telegramBot.SendTextMessageAsync(chatId, $"Пользователь {userName} заблокирован");
            await _telegramBot.DeleteMessageAsync(chatId, messageId);

        }

        private void Get(UpdateEventArgs updateEvent)
        {

            _userRepository.CreateOrUpdateAsync(new User()
            {
                FirstName = updateEvent.Update.Message.From.FirstName,
                LastName = updateEvent.Update.Message.From.LastName,
                IsBot = updateEvent.Update.Message.From.IsBot,
                LanguageCode = updateEvent.Update.Message.From.LanguageCode,
                Username = updateEvent.Update.Message.From.Username,
                UserId = updateEvent.Update.Message.From.Id
            });
        }
    }
}
