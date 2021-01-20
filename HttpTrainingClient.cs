using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AITest
{
    public class HttpTrainingClient : ITrainingClient
    {
        private readonly HttpClient _httpClient;
        private readonly Configuration _configuration;
        public HttpTrainingClient(IHttpClientFactory httpClientFactory, Configuration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient("");
        }

        public async Task<bool> UploadImageAsync(Stream data, string fileName, string tagName)
        {
            HttpResponseMessage tagsResponse = await _httpClient.GetAsync($"https://westeurope.api.cognitive.microsoft.com/customvision/v3.3/Training/projects/{_configuration.ProjectId}/tags");
            string tagsJson = await tagsResponse.Content.ReadAsStringAsync();
            IEnumerable<Tag> tags = JsonConvert.DeserializeObject<IEnumerable<Tag>>(tagsJson);
            Tag exisitngTag = tags.SingleOrDefault(t => t.Name == tagName);
            if (exisitngTag == null)
                exisitngTag = await CreateTagAsync(tagName);

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(data), "file", fileName);
            HttpResponseMessage postImageResponse = await _httpClient.PostAsync($"https://westeurope.api.cognitive.microsoft.com/customvision/v3.3/Training/projects/{_configuration.ProjectId}/images?tagIds={exisitngTag.Id}", content);
            return postImageResponse.IsSuccessStatusCode;
        }

        private async Task<Tag> CreateTagAsync(string tagName)
        {
            HttpResponseMessage tagResponse = await _httpClient.PostAsync($"https://westeurope.api.cognitive.microsoft.com/customvision/v3.3/Training/projects/8409a583-31c8-4225-8ceb-d229603ebde3/tags?projectId={_configuration.ProjectId}&name={tagName}", null);
            string tagResponseJson = await tagResponse.Content.ReadAsStringAsync();
            Tag createdTag = JsonConvert.DeserializeObject<Tag>(tagResponseJson);
            return createdTag;
        }
    }
}