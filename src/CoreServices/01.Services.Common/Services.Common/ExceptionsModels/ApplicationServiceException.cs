using Services.Common.MethodResultUtils;
using System.Collections.Generic;
using NetHttpStatusCode = System.Net.HttpStatusCode;

namespace Services.Common.ExceptionsModels
{
    public class ApplicationServiceException : BaseException
    {
        public ApplicationServiceException(ErrorResult errorResult, int httpStatusCode = (int)NetHttpStatusCode.BadRequest) : base(errorResult, httpStatusCode)
        {
        }

        public ApplicationServiceException(IReadOnlyCollection<ErrorResult> errorResultList, int httpStatusCode = (int)NetHttpStatusCode.BadRequest) : base(errorResultList, httpStatusCode)
        {
        }
    }
}