using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace uwpPlatformer.Utilities
{
    public class TiledLoader
    {
        public T LoadResource<T>(Stream stream)
        {
            var jsonSerializer = new JsonSerializer();
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return jsonSerializer.Deserialize<T>(jsonTextReader);
            }
        }

        public async Task<T> LoadResourceAsync<T>(Uri uri)
        {
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);

            using (var randomAccessStream = await storageFile.OpenReadAsync())
            {
                return LoadResource<T>(randomAccessStream.AsStreamForRead());
            }
        }
    }
}