using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

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
            var task = (updateEvent.Update.Message.ReplyToMessage == null) ? (
                    await telegramBot.KickChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                        updateEvent.Update.Message.Entities.Where(c => c.User != null).Select(c => c.User.Id).FirstOrDefault())
                ) :
                (await telegramBot.KickChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.ReplyToMessage.From.Id)
                );
        }

        /// <summary>
        /// Добавляет все права пользователю
        /// </summary>
        /// <param name="telegramBot"></param>
        /// <param name="updateEvent"></param>
        /// <returns></returns>
        public static async Task PromoteUserChatAsync(this TelegramBotClient telegramBot, UpdateEventArgs updateEvent)
        {
            var task = (updateEvent.Update.Message.ReplyToMessage == null) ? (
                    await telegramBot.PromoteChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                        updateEvent.Update.Message.Entities.Where(c => c.User != null).Select(c => c.User.Id).FirstOrDefault())
                ) :
                (await telegramBot.PromoteChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.ReplyToMessage.From.Id)
                );
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
            var task = (updateEvent.Update.Message.ReplyToMessage == null) ? (
                    await telegramBot.RestrictChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                        updateEvent.Update.Message.Entities.Where(c => c.User != null).Select(c => c.User.Id).FirstOrDefault(), DateTime.Now.AddHours(1), false, false, false, false)
                ) :
                (await telegramBot.RestrictChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.ReplyToMessage.From.Id, DateTime.Now.AddHours(1), false, false, false, false)
                );
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
            var task = (updateEvent.Update.Message.ReplyToMessage == null) ? (
                    await telegramBot.UnbanChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                        updateEvent.Update.Message.Entities.Where(c => c.User != null).Select(c => c.User.Id).FirstOrDefault())
                ) :
                (await telegramBot.RestrictChatMemberAsync(updateEvent.Update.Message.Chat.Id,
                    updateEvent.Update.Message.ReplyToMessage.From.Id)
                );
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
            int randomUrl = rnd.Next(0, 20);
            string luckyUrl = urls[randomUrl];
            var fileToSend = new FileToSend(luckyUrl);

            await telegramBot.SendPhotoAsync(updateEvent.Update.Message.Chat.Id, fileToSend, updateEvent.Update.Message.Text.Replace('#', ' '));
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
