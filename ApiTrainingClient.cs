using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

namespace AITest
{
    public class ApiTrainingClient : ITrainingClient
    {
        private readonly Configuration _configuration;

        public ApiTrainingClient(Configuration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> UploadImageAsync(Stream data, string fileName, string tagName)
        {
            CustomVisionTrainingClient trainingApi = AuthenticateTraining(_configuration.TrainingEndpoint, _configuration.TrainingKey);
            Guid projectIdGuid = Guid.Parse(_configuration.ProjectId);
            Project project = await trainingApi.GetProjectAsync(projectIdGuid);
            IList<Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models.Tag> tags = await trainingApi.GetTagsAsync(projectIdGuid);
            Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models.Tag existedTag = tags.SingleOrDefault(t => t.Name == tagName);
            if(existedTag == null)
                existedTag = trainingApi.CreateTag(projectIdGuid, tagName);
            ImageCreateSummary result = trainingApi.CreateImagesFromData(projectIdGuid, data, new List<Guid>() { existedTag.Id });
            return result.IsBatchSuccessful;
        }

        private static CustomVisionTrainingClient AuthenticateTraining(string endpoint, string trainingKey)
        {
            CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(trainingKey))
            {
                Endpoint = endpoint
            };
            return trainingApi;
        }
    }
}