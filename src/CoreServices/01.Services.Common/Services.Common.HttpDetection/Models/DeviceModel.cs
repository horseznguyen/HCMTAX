using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Http;
using Shyjus.BrowserDetection;
using System;
using System.IO;

namespace Services.Common.HttpDetection.Models
{
    public class DeviceModel : IDisposable
    {
        #region fields

        private static readonly object Lock = new object();
        private bool IsDisposed { get; set; }

        #endregion fields

        #region constructors

        public DeviceModel()
        {
        }

        public DeviceModel(HttpRequest request, IBrowser browser)
        {
            if (browser != null)
            {
                Type = browser.DeviceType;
                OsName = browser.OS;
                BrowserName = browser.Name;
                BrowserVersion = browser.Version;
            }
            OsVersion = HttpRequestHelpers.GetOsVersion(request);
            EngineName = HttpRequestHelpers.GetEngineName(request);
            EngineVersion = HttpRequestHelpers.GetEngineVersion(request);
            UpdateLocation(request);
            UserAgent = HttpRequestHelpers.GetUserAgent(request);
            DeviceHash = GetDeviceHash();
        }

        #endregion constructors

        #region Properties

        public string Type { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
        public string EngineName { get; set; }
        public string EngineVersion { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string IpAddress { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string CountryIsoCode { get; set; }
        public string TimeZone { get; set; }
        public string PostalCode { get; set; }
        public string UserAgent { get; set; }
        public string DeviceHash { get; set; }

        #endregion Properties

        #region Methods

        private string GetDeviceHash()
        {
            var ipAddress = ""; // TODO

            var identityDevice = $"{OsName}|{OsVersion}_{EngineName}|{EngineVersion}_{BrowserName}|{BrowserVersion}_{ipAddress}";

            var deviceHash = ""; // TODO

            return deviceHash;
        }

        private void UpdateLocation(HttpRequest request)
        {
            IpAddress = ""; // TODO
            var geoDbAbsolutePath = Path.Combine(Bootstrapper.Instance.WorkingFolder, HttpDetectionConstants.DbName);
            if (!File.Exists(geoDbAbsolutePath))
            {
                throw new FileNotFoundException($"{geoDbAbsolutePath} not found", geoDbAbsolutePath);
            }

            lock (Lock)
            {
                using var reader = new DatabaseReader(geoDbAbsolutePath);
                if (!reader.TryCity(IpAddress, out var city))
                {
                    return;
                }

                if (city == null)
                {
                    return;
                }

                IpAddress = city.Traits.IPAddress;
                CityName = city.City.Names.TryGetValue("en", out var cityName) ? cityName : city.City.Name;
                CountryName = city.Country.Names.TryGetValue("en", out var countryName) ? countryName : city.Country.Name;
                CountryIsoCode = city.Country.IsoCode;
                PostalCode = city.Postal.Code;
                TimeZone = city.Location.TimeZone;
            }
        }

        #endregion Methods

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (isDisposing)
            {
                DisposeUnmanagedResources();
            }
            IsDisposed = true;
        }

        protected virtual void DisposeUnmanagedResources()
        {
        }

        ~DeviceModel()
        {
            Dispose(false);
        }

        #endregion Dispose
    }
}