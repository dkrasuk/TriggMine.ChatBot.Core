using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriggMine.ChatBot.Core.Services.Interfaces;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Repository.Repository;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services
{
    public class MessageService : IMessageService
    {
        private readonly IChatBotRepository<Message> _messageRepository;
        private readonly ILogger<MessageService> _logger;
        public MessageService(IChatBotRepository<Message> messageRepository, ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public async Task<List<MessageDTO>> GetMessageAsync()
        {
            try
            {
                var messagesDto = new List<MessageDTO>();
                var messages = (await _messageRepository.GetAsyncList(c=>true)).ToList();
                foreach (var message in messages)
                {
                    messagesDto.Add(DataToDtoMessage(message));
                }
                return messagesDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetMessageAsync Error: {ex.Message}");
                return null;
            }

        }

        public async Task CreateMessage(MessageDTO messageDTO)
        {
            try
            {
                var message = DtoToDataMessage(messageDTO);
                await _messageRepository.CreateOrUpdateAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateMessage idUser {messageDTO.UserId} Error: {ex.Message}");
            }

        }

        public async Task<List<MessageDTO>> GetMessagesByUserId(int userId)
        {
            try
            {
                var messages = await _messageRepository.GetAsyncList(c => c.UserId == userId);               

                var messagesDto = new List<MessageDTO>();
                foreach (var message in messages)
                {
                    messagesDto.Add(DataToDtoMessage(message));
                }
                return messagesDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetMessagesByUserId idUser {userId} Error: {ex.Message}");
                return null;
            }
        }

        private MessageDTO DataToDtoMessage(Message message)
        {
            return new MessageDTO()
            {
                ChatId = message.ChatId,
                Id = message.Id,
                MessageId = message.MessageId,
                Text = message.Text,
                UserId = message.UserId,
                SendMessageDate = message.SendMessageDate,
                ChatTitle = message.ChatTitle,
                Type = message.Type
            };
        }

        private Message DtoToDataMessage(MessageDTO message)
        {
            return new Message()
            {
                ChatId = message.ChatId,
                Id = message.Id,
                MessageId = message.MessageId,
                Text = message.Text,
                UserId = message.UserId,
                ChatTitle = message.ChatTitle,
                Type = message.Type
            };
        }
    }
}
