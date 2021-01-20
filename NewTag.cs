using Newtonsoft.Json;

namespace AITest
{
    public class NewTag
    {
        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}