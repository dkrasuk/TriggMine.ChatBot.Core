using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetAllUser(Expression<Func<User, bool>> predicate);
        Task CreateUser(UserDTO user);
        Task BlockUser(int userId);
        Task<UserDTO> FindUser(Expression<Func<User, bool>> predicate);
    }
}
