using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriggMine.ChatBot.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Telegram.Bot.Args;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace TriggMine.ChatBot.Core.Services
{
    public class AzureMachineLearningService : IAzureMachineLearningService
    {
        private readonly ComputerVisionAPI _computerVisionAPI;
        private readonly ILogger<AzureMachineLearningService> _logger;
        private readonly string _azureMLApiKey;
        private readonly string _basePathImageFolder;
        public AzureMachineLearningService(ILogger<AzureMachineLearningService> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _azureMLApiKey = configuration["AzureMLApiKey"];
            _basePathImageFolder = configuration["BasePathImageFolder"];
            _computerVisionAPI = new ComputerVisionAPI(new ApiKeyServiceClientCredentials(_azureMLApiKey),
                new System.Net.Http.DelegatingHandler[] { });
            _computerVisionAPI.AzureRegion = Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models.AzureRegions.Westcentralus;
        }

        private readonly List<VisualFeatureTypes> features =
           new List<VisualFeatureTypes>()
       {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
       };

        public async Task<string> AnalyzePhotoMLAzureAsync(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                _logger.LogError(
                    "\nUnable to open or read localImagePath:\n{0} \n", imagePath);
                return null;
            }

            using (Stream imageStream = File.OpenRead(imagePath))
            {
                ImageAnalysis analysis = await _computerVisionAPI.AnalyzeImageInStreamAsync(imageStream, features);
                if (analysis.Description.Captions.Count() > 0)
                {
                    return analysis.Description.Captions[0].Text;
                }                
            }
            return null;
        }
    }
}
