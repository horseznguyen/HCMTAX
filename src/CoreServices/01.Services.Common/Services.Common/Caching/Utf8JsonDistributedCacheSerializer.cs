using Newtonsoft.Json;
using Services.Common.Caching.Interfaces;
using Services.Common.DI;
using System.Text;

namespace Services.Common.Caching
{
    [TransientDependency(ServiceType = typeof(IDistributedCacheSerializer))]
    public class Utf8JsonDistributedCacheSerializer : IDistributedCacheSerializer
    {
        public byte[] Serialize<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return (T)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), typeof(T));
        }
    }
}