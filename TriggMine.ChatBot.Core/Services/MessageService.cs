using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TriggMine.ChatBot.Core.Services.Interfaces;
using TriggMine.ChatBot.Repository.Interfaces;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Repository.Repository;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services
{
    public class MessageService : IMessageService
    {
        private readonly ILogger<MessageService> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public MessageService(ILogger<MessageService> logger, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
        }

        public async Task<List<MessageDTO>> GetMessageAsync()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                try
                {
                    var messagesDto = new List<MessageDTO>();
                    var messages = uow.MessageRepository.Query();
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
        }

        public async Task CreateMessage(MessageDTO messageDTO)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                try
                {
                    var message = DtoToDataMessage(messageDTO);
                    await uow.MessageRepository.AddAsync(message);
                    await uow.SaveChangeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"CreateMessage idUser {messageDTO.UserId} Error: {ex.Message}");
                }
            }
        }

        public async Task<List<MessageDTO>> GetMessagesByUserId(int userId)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                try
                {
                    var messages = await uow.MessageRepository.Query().Where(i => i.UserId == userId).ToListAsync();
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
