using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TriggMine.ChatBot.Core.Services.Interfaces;
using TriggMine.ChatBot.Repository.Interfaces;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Repository.Repository;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services
{
    public class ResolverUrlService : IResolverUrlService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger<ResolverUrlService> _logger;

        public ResolverUrlService(IUnitOfWorkFactory unitOfWorkFactory, ILogger<ResolverUrlService> logger)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
        }

        public async Task<List<ResolvedUrlDTO>> GetResolvedUrlsListAsync()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                try
                {
                    var resolvedUrlsDto = new List<ResolvedUrlDTO>();
                    var resolvedUrls = await uow.ResolvedUrlRepository.Query().ToListAsync();
                    foreach (var resolvedUrl in resolvedUrls)
                    {
                        resolvedUrlsDto.Add(DataToDtoResolvedUrl(resolvedUrl));
                    }
                    return resolvedUrlsDto;
                }
                catch (Exception e)
                {
                    _logger.LogError($"GetResolvedUrlsListAsync Error: {e.Message}");
                    return null;
                }
            }
        }

        public async Task AddResolvedUrl(ResolvedUrlDTO resolvedUrlDto)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                try
                {
                    var resolvedUrl = await uow.ResolvedUrlRepository.AddAsync(DtoToDataResolvedUrl(resolvedUrlDto));
                    await uow.SaveChangeAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError($"AddResolvedUrl Error: {e.Message}");
                }
            }
        }

        public async Task DeleteResolvedUrl(int resolvedUrlId)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                try
                {
                    var resolvedUrl = await uow.ResolvedUrlRepository.Query().FirstOrDefaultAsync(i => i.Id == resolvedUrlId);
                    if (resolvedUrl == null)
                        return;

                    uow.ResolvedUrlRepository.Delete(resolvedUrl);
                }
                catch (Exception e)
                {
                    _logger.LogError($"DeleteResolvedUrl Error: {e.Message}");
                }
            }
        }

        private ResolvedUrl DtoToDataResolvedUrl(ResolvedUrlDTO resolvedUrlDto)
        {
            return new ResolvedUrl()
            {
                Id = resolvedUrlDto.Id,
                Url = resolvedUrlDto.Url
            };
        }

        private ResolvedUrlDTO DataToDtoResolvedUrl(ResolvedUrl resolvedUrlDto)
        {
            return new ResolvedUrlDTO()
            {
                Id = resolvedUrlDto.Id,
                Url = resolvedUrlDto.Url
            };
        }
    }
}
