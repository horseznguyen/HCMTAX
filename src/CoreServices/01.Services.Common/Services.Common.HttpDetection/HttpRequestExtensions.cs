using Microsoft.AspNetCore.Http;
using Services.Common.HttpDetection.Models;
using Shyjus.BrowserDetection;

namespace Services.Common.HttpDetection
{
    public static class HttpRequestExtensions
    {
        public static DeviceModel GetDeviceInformation(this HttpRequest request, IBrowser browserDetector)
        {
            return new DeviceModel(request, browserDetector);
        }
    }
}