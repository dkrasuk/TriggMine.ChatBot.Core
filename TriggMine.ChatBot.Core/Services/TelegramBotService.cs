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
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public TelegramBotService(ILogger<TelegramBotService> logger
            , IConfiguration configuration
            , IUserService userService
            , IMessageService messageService
            )
        {
            _logger = logger;
            _userService = userService;
            _messageService = messageService;
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

        async void ReadMessage(object sender, UpdateEventArgs updateEvent)
        {
            if (updateEvent.Update.Message.Text == null)
                return;

            await AddUser(updateEvent);

            if ((await _userService.FindUser(c=>c.UserId == updateEvent.Update.Message.From.Id)).IsBlocked == true)
            {
               await DeleteMessage(updateEvent);
            }
           
            await AddMessage(updateEvent);

            //var update = updateEvent.Update;
            //var message = update.Message;

            //var chatId = updateEvent.Update.Message.Chat.Id;
            //var messageId = updateEvent.Update.Message.MessageId;
            //var userId = updateEvent.Update.Message.From.Id;

            if (updateEvent.Update.Message.Text.Contains("хуй"))
            {
                await DeleteMessage(updateEvent);
                await BlockUser(updateEvent.Update.Message.From.Id);                
            }

        }


        private async Task DeleteMessage(UpdateEventArgs updateEvent)
        {
            var chatId = updateEvent.Update.Message.Chat.Id;
            var messageId = updateEvent.Update.Message.MessageId;
            await _telegramBot.DeleteMessageAsync(chatId, messageId);
            await _telegramBot.SendTextMessageAsync(chatId, $"Пользователь {updateEvent.Update.Message.From.FirstName} {updateEvent.Update.Message.From.LastName} заблокирован");
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
                UserId = updateEvent.Update.Message.From.Id
            });
        }

        private async Task BlockUser(int userId)
        {
            await _userService.BlockUser(userId);
        }
    }
}
