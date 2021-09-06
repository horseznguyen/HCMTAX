using Services.Common.MethodResultUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Common.ExceptionsModels
{
    public class BaseException : Exception
    {
        public int HttpStatusCode { get; }
        private readonly List<ErrorResult> _errorResultList;

        private BaseException(int httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
            _errorResultList ??= new List<ErrorResult>();
        }

        public BaseException(ErrorResult errorResult, int httpStatusCode) : this(httpStatusCode)
        {
            if (errorResult != null)
            {
                _errorResultList.Add(errorResult);
            }
        }

        public BaseException(IReadOnlyCollection<ErrorResult> errorResultList, int httpStatusCode) : this(httpStatusCode)
        {
            if (errorResultList != null && errorResultList.Any())
            {
                _errorResultList.AddRange(errorResultList);
            }
        }

        public IReadOnlyCollection<ErrorResult> ErrorResultList => _errorResultList;
    }
}