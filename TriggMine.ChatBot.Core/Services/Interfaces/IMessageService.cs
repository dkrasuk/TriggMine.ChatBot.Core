using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageDTO>> GetMessageAsync();
        Task CreateMessage(MessageDTO messageDTO);
        Task<List<MessageDTO>> GetMessagesByUserId(int userId);
    }
}
