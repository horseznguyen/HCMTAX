using Microsoft.Extensions.Configuration;

namespace Services.Common
{
    public class AppSettings
    {
        private static AppSettings _instance;

        private static readonly object ObjLocked = new object();

        private IConfiguration _configuration;

        protected AppSettings()
        {
        }

        public static AppSettings Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (ObjLocked)
                    {
                        if (null == _instance)
                        {
                            _instance = new AppSettings();
                        }
                    }
                }
                return _instance;
            }
        }

        public void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T Get<T>(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return _configuration.Get<T>(opt => opt.BindNonPublicProperties = true);
            }

            return _configuration.GetSection(key).Get<T>(opt => opt.BindNonPublicProperties = true);
        }

        public T Get<T>(string key, T defaultValue)
        {
            if (_configuration.GetSection(key) == null)
            {
                return defaultValue;
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return _configuration.Get<T>(opt => opt.BindNonPublicProperties = true);
            }

            return _configuration.GetSection(key).Get<T>(opt => opt.BindNonPublicProperties = true);
        }

        public static T GetObject<T>(string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Instance._configuration.Get<T>(opt => opt.BindNonPublicProperties = true);
            }

            var section = Instance._configuration.GetSection(key);

            return section.Get<T>(opt => opt.BindNonPublicProperties = true);
        }

        public static T GetObject<T>(string key, T defaultValue)
        {
            if (Instance._configuration.GetSection(key) == null)
            {
                return defaultValue;
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return Instance._configuration.Get<T>(opt => opt.BindNonPublicProperties = true);
            }

            return Instance._configuration.GetSection(key).Get<T>(opt => opt.BindNonPublicProperties = true);
        }
    }
}