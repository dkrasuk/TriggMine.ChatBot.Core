using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TriggMine.ChatBot.Core.Services.Interfaces;
using TriggMine.ChatBot.Repository.Interfaces;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Repository.Repository;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWorkFactory unitOfWorkFactory, ILogger<UserService> logger)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
        }

        public async Task<List<UserDTO>> GetAllUser(Expression<Func<User, bool>> predicate)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                try
                {
                    var userDto = new List<UserDTO>();
                    var users = await uow.UserRepository.Query().ToListAsync();

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
        }

        public async Task CreateUser(UserDTO user)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                try
                {
                    var userDto = await uow.UserRepository.Query().FirstOrDefaultAsync(i => i.UserId == user.UserId);

                    if (userDto == null)
                        await uow.UserRepository.AddAsync(DtoToDataUsers(user));

                    await uow.SaveChangeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"CreateUser Error: {ex.Message}");
                }
            }
        }

        public async Task BlockUser(int userId)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                try
                {
                    var userDto = await uow.UserRepository.Query().FirstOrDefaultAsync(i => i.UserId == userId);
                    if (userDto != null)
                    {
                        userDto.IsBlocked = true;
                        userDto.DateBlockedUser = DateTime.Now;
                        uow.UserRepository.Update(userDto);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"BlockUser ID: {userId} Error: {ex.Message}"); ;
                }
            }
        }

        public async Task<UserDTO> FindUser(Expression<Func<User, bool>> predicate)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                try
                {
                    var user = await uow.UserRepository.Query().FirstOrDefaultAsync(predicate);
                    return DataToDtoUsers(user);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"UserId not found! {ex.Message}");
                    return null;
                }
            }
        }

        private UserDTO DataToDtoUsers(User users)
        {
            return new UserDTO()
            {
                FirstName = users.FirstName,
                IsBlocked = users.IsBlocked,
                IsBot = users.IsBot,
                LanguageCode = users.LanguageCode,
                LastName = users.LastName,
                UserId = users.UserId,
                Username = users.Username,
                DateFirstActivity = users.DateFirstActivity,
                DateBlockedUser = users.DateBlockedUser
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
