using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Repository.Repository;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IChatBotRepository<User> _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IChatBotRepository<User> userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<List<UserDTO>> GetAllUser()
        {
            try
            {
                var userDto = new List<UserDTO>();
                var users = (await _userRepository.GetAsync()).ToList();

                foreach (var user in users)
                {
                    userDto.Add(DataToDtoUsers(user));
                }
                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAllUser Error: {ex.Message}");
                return null;
            }
        }

        public async Task CreateUser(UserDTO user)
        {
            try
            {
                var userData = DtoToDataUsers(user);
                await _userRepository.CreateOrUpdateAsync(userData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateUser Error: {ex.Message}");
            }
        }

        private UserDTO DataToDtoUsers(User users)
        {
            var messages = new List<MessageDTO>();
            foreach (var message in users.Messages)
            {
                messages.Add(new MessageDTO()
                {
                    ChatId = message.ChatId,
                    Id = message.Id,
                    MessageId = message.MessageId,                    
                    Text = message.Text,
                    UserId = message.UserId
                });
            }
            return new UserDTO()
            {                
                FirstName = users.FirstName,
                IsBlocked = users.IsBlocked,
                IsBot = users.IsBot,
                LanguageCode = users.LanguageCode,
                LastName = users.LastName,                
                UserId = users.UserId,
                Username = users.Username
            };
        }

        private User DtoToDataUsers(UserDTO users)
        {            
            return new User()
            {
                FirstName = users.FirstName,
                IsBlocked = users.IsBlocked,
                IsBot = users.IsBot,
                LanguageCode = users.LanguageCode,
                LastName = users.LastName,
                UserId = users.UserId,
                Username = users.Username                
            };
        }

    }
}
