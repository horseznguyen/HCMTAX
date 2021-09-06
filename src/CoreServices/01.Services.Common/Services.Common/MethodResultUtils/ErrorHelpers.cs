using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Services.Common.MethodResultUtils
{
    public static class ErrorHelpers
    {
        private static ConcurrentDictionary<string, Dictionary<string, string>> _warningMessage;

        private static ConcurrentDictionary<string, Dictionary<string, string>> _errorMessages;

        public static string GetExceptionMessage(Exception ex) => $"Message: {ex.Message}, InnerMessage: {ex.InnerException?.Message}";

        public static string GenerateErrorResult(string propertyName, object propertyValue)
        {
            return $"{propertyName.Substring(0, 1).ToLower(CultureInfo.InvariantCulture)}{propertyName.Substring(1, propertyName.Length - 1)}: {propertyValue}";
        }

        public static string GetCommonErrorMessage(string errorCode)
        {
            return GetErrorMessage(errorCode, ref _errorMessages, typeof(ErrorHelpers).Assembly);
        }

        public static string GetErrorMessage(string errorCode, Assembly resourceAssembly)
        {
            return GetErrorMessage(errorCode, ref _errorMessages, resourceAssembly);
        }

        public static string GetWarningMessage(string warningCode, Assembly resourceAssembly)
        {
            return GetWarningMessage(warningCode, ref _warningMessage, resourceAssembly);
        }

        public static string GetErrorMessage(string errorCode, ref ConcurrentDictionary<string, Dictionary<string, string>> errorMessages, Assembly resourceAssembly)
        {
            string defaultErrorMessage = "No pre-defined error message";

            if (resourceAssembly == null) return defaultErrorMessage;

            Dictionary<string, string> messages = null;

            var currentLanguage = CultureInfo.DefaultThreadCurrentCulture.TwoLetterISOLanguageName;

            var dictionaryKey = $"{resourceAssembly.GetName().Name}@@{currentLanguage}";

            #region get list of errors

            try
            {
                if (errorMessages != null)
                {
                    messages = errorMessages[dictionaryKey];
                }
            }
            catch { }

            if (messages == null)
            {
                try
                {
                    string jsonErrorFilePath = $"{Settings.ResourceFolderName}.{Settings.ErrorsFileName}-{currentLanguage}.json";

                    var fileData = GetFromResources(jsonErrorFilePath, resourceAssembly);

                    messages = JsonSerializer.Deserialize<Dictionary<string, string>>(fileData);
                }
                catch
                {
                    messages = new Dictionary<string, string>();
                }

                if (messages != null)
                {
                    errorMessages ??= new ConcurrentDictionary<string, Dictionary<string, string>>();

                    errorMessages[dictionaryKey] = messages;
                }
            }

            #endregion get list of errors

            defaultErrorMessage = messages.Keys.Contains(errorCode) ? messages[errorCode] : defaultErrorMessage;

            return defaultErrorMessage;
        }

        public static string GetWarningMessage(string errorCode, ref ConcurrentDictionary<string, Dictionary<string, string>> warningMessages, Assembly resourceAssembly)
        {
            string defaultWarningMessage = "No pre-defined warning message";

            if (resourceAssembly == null) return defaultWarningMessage;

            Dictionary<string, string> messages = null;

            var currentLanguage = CultureInfo.DefaultThreadCurrentCulture.TwoLetterISOLanguageName;

            var dictionaryKey = $"{resourceAssembly.GetName().Name}@@{currentLanguage}";

            try
            {
                if (_warningMessage != null)
                {
                    messages = _warningMessage[dictionaryKey];
                }
            }
            catch (Exception)
            {
                throw;
            }

            if (messages == null)
            {
                try
                {
                    string jsonErrorFilePath = $"{Settings.ResourceFolderName}.{Settings.WarningsFileName}-{currentLanguage}.json";

                    var fileData = GetFromResources(jsonErrorFilePath, resourceAssembly);

                    messages = JsonSerializer.Deserialize<Dictionary<string, string>>(fileData);
                }
                catch
                {
                    messages = new Dictionary<string, string>();
                }

                if (messages != null)
                {
                    warningMessages ??= new ConcurrentDictionary<string, Dictionary<string, string>>();

                    warningMessages[dictionaryKey] = messages;
                }
            }

            defaultWarningMessage = messages.Keys.Contains(errorCode) ? messages[errorCode] : defaultWarningMessage;

            return defaultWarningMessage;
        }

        public static string GetFromResources(string resourceName, Assembly resourceAssembly)
        {
            using Stream stream = resourceAssembly.GetManifestResourceStream(resourceAssembly.GetName().Name + '.' + resourceName);

            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}