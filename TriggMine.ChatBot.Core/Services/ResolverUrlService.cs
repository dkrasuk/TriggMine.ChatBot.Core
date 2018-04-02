using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TriggMine.ChatBot.Core.Services.Interfaces;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Repository.Repository;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services
{
    public class ResolverUrlService : IResolverUrlService
    {
        private readonly IChatBotRepository<ResolvedUrl> _resolvedUrlRepository;
        private readonly ILogger<ResolverUrlService> _logger;

        public ResolverUrlService(IChatBotRepository<ResolvedUrl> resolvedUrlRepository, ILogger<ResolverUrlService> logger)
        {
            _resolvedUrlRepository = resolvedUrlRepository;
            _logger = logger;
        }

        public async Task<List<ResolvedUrlDTO>> GetResolvedUrlsListAsync()
        {
            try
            {
                var resolvedUrlsDto = new List<ResolvedUrlDTO>();
                var resolvedUrls = (await _resolvedUrlRepository.GetAsyncList(c => true)).ToList();
                foreach (var resolvedurl in resolvedUrls)
                {
                    resolvedUrlsDto.Add(DataToDtoResolvedUrl(resolvedurl));
                }
                return resolvedUrlsDto;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetResolvedUrlsListAsync Error: {e.Message}");
                return null;
            }
        }

        public async Task AddResolvedUrl(ResolvedUrlDTO resolvedUrlDto)
        {
            try
            {
                await _resolvedUrlRepository.CreateOrUpdateAsync(DtoToDataResolvedUrl(resolvedUrlDto));
            }
            catch (Exception e)
            {
                _logger.LogError($"AddResolvedUrl Error: {e.Message}");
            }
        }

        public async Task DeleteResolvedUrl(int resolvedUrlId)
        {
            try
            {
                await _resolvedUrlRepository.DeleteRecord(resolvedUrlId);
            }
            catch (Exception e)
            {
                _logger.LogError($"DeleteResolvedUrl Error: {e.Message}");
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
