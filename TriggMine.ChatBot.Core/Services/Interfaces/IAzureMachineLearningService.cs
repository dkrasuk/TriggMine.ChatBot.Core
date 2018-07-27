using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriggMine.ChatBot.Core.Services.Interfaces
{
    public interface IAzureMachineLearningService
    {
        Task<string> AnalyzePhotoMLAzureAsync(string imagePath);
    }
}
