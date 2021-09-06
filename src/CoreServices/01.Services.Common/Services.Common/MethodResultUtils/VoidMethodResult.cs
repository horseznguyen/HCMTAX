using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services.Common.MethodResultUtils
{
    public class VoidMethodResult
    {
        private readonly List<WarningResult> _warningResults = new List<WarningResult>();

        private readonly List<ErrorResult> _errorMessages = new List<ErrorResult>();

        #region error message

        public void AddErrorMessage(ErrorResult errorResult) => _errorMessages.Add(errorResult);

        public void AddErrorMessages(IEnumerable<ErrorResult> errorResults) => _errorMessages.AddRange(errorResults);

        public void AddErrorMessage(string exceptionErrorMessage, string exceptionStackTrace = "")
        {
            AddErrorMessage(CommonErrors.APIServerError, ErrorHelpers.GetCommonErrorMessage(CommonErrors.APIServerError), new string[] { }, exceptionErrorMessage, exceptionStackTrace);
        }

        public void AddErrorMessage(string errorCode, string errorMessage, string[] errorValues)
        {
            var errorResult = new ErrorResult
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };

            if (errorValues?.Length > 0)
            {
                foreach (var errorValue in errorValues)
                    errorResult.ErrorValues.Add(errorValue);
            }

            AddErrorMessage(errorResult);
        }

        private void AddErrorMessage(
            string errorCode,
            string errorMessage,
            string[] errorValues,
            string exceptionErrorMessage,
            string exceptionStackTrace)
        {
            _errorMessages.Add(new ErrorResult
            {
                ErrorCode = errorCode,
                ErrorMessage = $"Error: {errorMessage}, Exception Message: {exceptionErrorMessage}, Stack Trace: {exceptionStackTrace}",
                ErrorValues = new List<string>(errorValues)
            });
        }

        #endregion error message

        #region warning message

        public void AddWarningMessage(WarningResult warningResult) => _warningResults.Add(warningResult);

        public void AddWarningMessages(IEnumerable<WarningResult> warningResults) => _warningResults.AddRange(warningResults);

        #endregion warning message

        [JsonPropertyName("warningMessages")]
        public IReadOnlyCollection<WarningResult> WarningResults => _warningResults;

        [JsonPropertyName("errorMessages")]
        public IReadOnlyCollection<ErrorResult> ErrorMessages => _errorMessages;

        [JsonPropertyName("isOk")] public bool IsOk => _errorMessages.Count == 0;

        public string ToJsonString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}