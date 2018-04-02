using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Services.Interfaces
{
    public interface IResolverUrlService
    {
        Task<List<ResolvedUrlDTO>> GetResolvedUrlsListAsync();
        Task AddResolvedUrl(ResolvedUrlDTO resolvedUrlDto);
        Task DeleteResolvedUrl(int resolvedUrlId);
    }
}
