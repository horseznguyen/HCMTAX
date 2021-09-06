using System.Collections.Generic;
using Services.Common.MethodResultUtils;
using NetHttpStatusCode = System.Net.HttpStatusCode;

namespace Services.Common.ExceptionsModels
{
    public class DomainServiceException : BaseException
    {
        public DomainServiceException(ErrorResult errorResult, int httpStatusCode = (int)NetHttpStatusCode.BadRequest) : base(errorResult, httpStatusCode)
        {
        }
        public DomainServiceException(IReadOnlyCollection<ErrorResult> errorResultList, int httpStatusCode = (int)NetHttpStatusCode.BadRequest) : base(errorResultList, httpStatusCode)
        {
        }
    }
}