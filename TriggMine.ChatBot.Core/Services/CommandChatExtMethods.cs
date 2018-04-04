using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

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
    }
}
