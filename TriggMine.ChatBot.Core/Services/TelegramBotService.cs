﻿using Microsoft.Extensions.Configuration;
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
        private readonly string _apiKey;

        public TelegramBotService(ILogger<TelegramBotService> logger
            , IConfiguration configuration
            , IUserService userService
            , IMessageService messageService
            , IResolverUrlService resolverUrlService
            )
        {
            _logger = logger;
            _userService = userService;
            _messageService = messageService;
            _resolverUrlService = resolverUrlService;
            _telegramBot = new TelegramBotClient(configuration["TelegramBotToken"]);
            _apiKey = configuration["YandexApiKey"];
        }

        public async Task GetBot()
        {

            var me = await _telegramBot.GetMeAsync();
            _logger.LogInformation($"Name bot: {me.FirstName}");

            _telegramBot.OnUpdate += ReadMessage;

            _telegramBot.StartReceiving();

            await _telegramBot.SetWebhookAsync("");

            _logger.LogInformation(me.FirstName);
        }



        async void ReadMessage(object sender, UpdateEventArgs updateEvent)
        {
            if (updateEvent.Update.Message?.Text == null)
                return;
            try
            {
                switch (updateEvent.Update.Message.Text.Split(' ').First())
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
                        await _telegramBot.TranslateMessage(updateEvent, _apiKey);
                        break;
                    case "/help":
                        await _telegramBot.GetHelp(updateEvent);
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error in running commands chat: {e.Message}");
            }

            await AddUser(updateEvent);

            //Check isBlocked User
            if ((await _userService.FindUser(c => c.UserId == updateEvent.Update.Message.From.Id)).IsBlocked == true)
            {
                await DeleteMessage(updateEvent);
            }

            await AddMessage(updateEvent);


            //Check URLS
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

            if (updateEvent.Update.Message.Text.Contains("хуй"))
            {
                await DeleteMessage(updateEvent);
                // await BlockUser(updateEvent.Update.Message.From.Id);
            }
        }

        public List<string> GetLinks(string message)
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
            await _telegramBot.DeleteMessageAsync(updateEvent.Update.Message.Chat.Id, updateEvent.Update.Message.MessageId);
            await _telegramBot.SendTextMessageAsync(updateEvent.Update.Message.Chat.Id, $"{updateEvent.Update.Message.From.FirstName} {updateEvent.Update.Message.From.LastName}, Ваше сообщение было удалено из-за нарушения политики безопастности!");
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
                ChatTitle = updateEvent.Update.Message.Chat.Title
            });
        }

        private async Task BlockUser(int userId)
        {
            await _userService.BlockUser(userId);
        }

    }
}
