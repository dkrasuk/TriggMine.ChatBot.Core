using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services
{
    public static class CommandChatExtMethods
    {
        /// <summary>
        /// Удаляет пользователя из чата
        /// </summary>
        /// <param name="telegramBot"></param>
        /// <param name="updateEvent"></param>
        /// <returns></returns>
        public static async Task KickUserChatAsync(this TelegramBotClient telegramBot, UpdateEventArgs updateEvent)
        {
            if (updateEvent.Update.Message.ReplyToMessage == null)
            {
                await telegramBot.KickChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.Entities.Where(c => c.User != null).Select(c => c.User.Id)
                        .FirstOrDefault());
            }
            else
            {
                await telegramBot.KickChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.ReplyToMessage.From.Id);
            }
        }

        /// <summary>
        /// Добавляет все права пользователю
        /// </summary>
        /// <param name="telegramBot"></param>
        /// <param name="updateEvent"></param>
        /// <returns></returns>
        public static async Task PromoteUserChatAsync(this TelegramBotClient telegramBot, UpdateEventArgs updateEvent)
        {
            if (updateEvent.Update.Message.ReplyToMessage == null)
            {
                await telegramBot.PromoteChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.Entities.Where(c => c.User != null).Select(c => c.User.Id)
                        .FirstOrDefault());
            }
            else
            {
                await telegramBot.PromoteChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.ReplyToMessage.From.Id);
            }
            await telegramBot.SendTextMessageAsync(updateEvent.Update.Message.Chat.Id, $"{updateEvent.Update.Message.From.FirstName} promote { updateEvent.Update.Message.Entities.Where(c => c.User != null).Select(c => c.User.FirstName).FirstOrDefault()}");
        }

        /// <summary>
        /// Банит пользователя, позволяет только читать сообщения
        /// </summary>
        /// <param name="telegramBot"></param>
        /// <param name="updateEvent"></param>
        /// <returns></returns>
        public static async Task BanUserChatAsync(this TelegramBotClient telegramBot, UpdateEventArgs updateEvent)
        {
            if (updateEvent.Update.Message.ReplyToMessage == null)
            {
                await telegramBot.RestrictChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.Entities.Where(c => c.User != null).Select(c => c.User.Id)
                        .FirstOrDefault(), DateTime.Now.AddHours(1), false, false, false, false);
            }
            else
            {
                await telegramBot.RestrictChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.ReplyToMessage.From.Id, DateTime.Now.AddHours(1), false, false, false,
                    false);
            }
            await telegramBot.SendTextMessageAsync(updateEvent.Update.Message.Chat.Id, $"{updateEvent.Update.Message.From.FirstName} baned { updateEvent.Update.Message.Entities.Where(c => c.User != null).Select(c => c.User.FirstName).FirstOrDefault()}");
        }

        /// <summary>
        /// Удаляет пользователя, вернуться может только по приглашению
        /// </summary>
        /// <param name="telegramBot"></param>
        /// <param name="updateEvent"></param>
        /// <returns></returns>

        public static async Task UnBanUserChatAsync(this TelegramBotClient telegramBot, UpdateEventArgs updateEvent)
        {
            if (updateEvent.Update.Message.ReplyToMessage == null)
            {
                await telegramBot.UnbanChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.Entities.Where(c => c.User != null).Select(c => c.User.Id).FirstOrDefault());
            }
            else
            {
                await telegramBot.RestrictChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.ReplyToMessage.From.Id);
            }
            await telegramBot.SendTextMessageAsync(updateEvent.Update.Message.Chat.Id, $"{updateEvent.Update.Message.From.FirstName} unbaned { updateEvent.Update.Message.Entities.Where(c => c.User != null).Select(c => c.User.FirstName).FirstOrDefault()}");
        }

        /// <summary>
        /// По ключу # возращает картинку из google images
        /// </summary>
        /// <param name="telegramBot"></param>
        /// <param name="updateEvent"></param>
        /// <returns></returns>
        public static async Task GetImageAndSentToChat(this TelegramBotClient telegramBot, UpdateEventArgs updateEvent)
        {
            string html = await GetHtmlCode(updateEvent.Update.Message.Text.Replace('#', ' '));
            List<string> urls = await GetUrls(html);
            var rnd = new Random();
            int randomUrl = rnd.Next(0, 10);
            string luckyUrl = urls[randomUrl];
            var fileToSend = new InputOnlineFile(luckyUrl);

            await telegramBot.SendPhotoAsync(updateEvent.Update.Message.Chat.Id, fileToSend, updateEvent.Update.Message.Text.Replace('#', ' '));
        }

        public static async Task TranslateMessageAndSend(this TelegramBotClient telegramBot, UpdateEventArgs updateEvent, string apiKey)
        {
            string sourceText = updateEvent.Update.Message.Text.Replace('*', ' ').Trim();
            string response = string.Empty;
            await Task.Run(() =>
            {
                using (var client = new WebClient())
                {
                    //Определение языка
                    var detectTranslate = JsonConvert.DeserializeObject<Translate>(client.DownloadString($"https://translate.yandex.net/api/v1.5/tr.json/detect?key={apiKey}&text={sourceText}"));
                    switch (detectTranslate.Lang)
                    {
                        case "ru":
                        case "uk":
                            response = client.DownloadString($"https://translate.yandex.net/api/v1.5/tr.json/translate?key={apiKey}&text={sourceText}&lang=en");
                            break;
                        case "en":
                            response = client.DownloadString($"https://translate.yandex.net/api/v1.5/tr.json/translate?key={apiKey}&text={sourceText}&lang=ru");
                            break;
                        default:
                            response = client.DownloadString($"https://translate.yandex.net/api/v1.5/tr.json/translate?key={apiKey}&text={sourceText}&lang=ru");
                            break;
                    }

                    var translateText = JsonConvert.DeserializeObject<Translate>(response);
                    telegramBot.SendTextMessageAsync(updateEvent.Update.Message.Chat.Id, translateText.Text.FirstOrDefault());
                }
            });
        }
        public static async Task<string> TranslateMessage(string messageText, string apiKey)
        {
            string response = string.Empty;
            string translateText = string.Empty;
            await Task.Run(() =>
            {
                using (var client = new WebClient())
                {
                    //Определение языка
                    var detectTranslate = JsonConvert.DeserializeObject<Translate>(client.DownloadString($"https://translate.yandex.net/api/v1.5/tr.json/detect?key={apiKey}&text={messageText}"));
                    switch (detectTranslate.Lang)
                    {
                        case "ru":
                        case "uk":
                            response = client.DownloadString($"https://translate.yandex.net/api/v1.5/tr.json/translate?key={apiKey}&text={messageText}&lang=en");
                            break;
                        case "en":
                            response = client.DownloadString($"https://translate.yandex.net/api/v1.5/tr.json/translate?key={apiKey}&text={messageText}&lang=ru");
                            break;
                        default:
                            response = client.DownloadString($"https://translate.yandex.net/api/v1.5/tr.json/translate?key={apiKey}&text={messageText}&lang=ru");
                            break;
                    }

                    translateText = JsonConvert.DeserializeObject<Translate>(response).Text.FirstOrDefault();                    
                }
            });
            return translateText;
        }

        public static async Task GetHelp(this TelegramBotClient telegramBot, UpdateEventArgs updateEvent)
        {
            string helpMessage = $"Hi, I'm a bot my name is {telegramBot.GetMeAsync().Result.FirstName} \n \n" +
                                 $"-------------------------------------------------------------- \n" +
                                 $"I can execute the following commands: \n \n" +
                                 $"#text - I will find a picture from Google; \n" +
                                 $"*text - translate autodetect [RU]=>[EN] or [EN]=>[RU]; \n" +
                                 $"/help - Tell what I'm cool; \n" +
                                 $"/ban @user - User ban, allows only to read messages; \n" +
                                 $"/unban - Deletes a user, can only return by invitation; \n" +
                                 $"/kick - Removes a user from chat; \n" +
                                 $"/promote - Adds all rights to the user; \n";
            await telegramBot.SendTextMessageAsync(updateEvent.Update.Message.Chat.Id, helpMessage);
        }

        #region PrivateMethots
        private static async Task<string> GetHtmlCode(string searchQuery)
        {
            return await Task.Run(() =>
            {
                string url = "https://www.google.com/search?q=" + searchQuery + "&tbm=isch";
                string data = "";

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

                var response = (HttpWebResponse)request.GetResponse();

                using (Stream dataStream = response.GetResponseStream())
                {
                    if (dataStream == null)
                        return "";
                    using (var sr = new StreamReader(dataStream))
                    {
                        data = sr.ReadToEnd();
                    }
                }
                return data;
            });
        }
        private static async Task<List<string>> GetUrls(string html)
        {
            return await Task.Run(() =>
            {
                var urls = new List<string>();

                int ndx = html.IndexOf("\"ou\"", StringComparison.Ordinal);

                while (ndx >= 0)
                {
                    ndx = html.IndexOf("\"", ndx + 4, StringComparison.Ordinal);
                    ndx++;
                    int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
                    string url = html.Substring(ndx, ndx2 - ndx);
                    urls.Add(url);
                    ndx = html.IndexOf("\"ou\"", ndx2, StringComparison.Ordinal);
                }
                return urls;
            });
        }
        #endregion

    }
}
