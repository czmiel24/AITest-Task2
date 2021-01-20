using System.IO;
using System.Threading.Tasks;

namespace AITest
{
    public interface ITrainingClient
    {
         Task<bool> UploadImageAsync(Stream data, string fileName, string tagName);
    }
}