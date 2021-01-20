using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AITest
{
    public class Function
    {
        private readonly ITrainingClient _trainingClient;
        public Function(ITrainingClient trainingClient)
        {
            _trainingClient = trainingClient;
        }

        [FunctionName("SendImageToCognitiveServiceWhenNewImageInBlob")]
        public async Task RunAsync([BlobTrigger("images-ai/{name}", Connection = "imagestorageai_STORAGE")] Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            string tag = name.Split(' ').First();

            await _trainingClient.UploadImageAsync(myBlob, name, tag);
        }
    }
}
