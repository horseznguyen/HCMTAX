using Services.Common.MethodResultUtils;
using System.Collections.Generic;
using NetHttpStatusCode = System.Net.HttpStatusCode;

namespace Services.Common.ExceptionsModels
{
    public class DomainException : BaseException
    {
        public DomainException(ErrorResult errorResult, int httpStatusCode = (int)NetHttpStatusCode.BadRequest) : base(errorResult, httpStatusCode)
        {
        }

        public DomainException(IReadOnlyCollection<ErrorResult> errorResultList, int httpStatusCode = (int)NetHttpStatusCode.BadRequest) : base(errorResultList, httpStatusCode)
        {
        }
    }
}