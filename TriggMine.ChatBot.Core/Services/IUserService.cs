using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetAllUser();
        Task CreateUser(UserDTO user);
        Task BlockUser(int userId);
        Task<UserDTO> FindUser(int userId);
    }
}
