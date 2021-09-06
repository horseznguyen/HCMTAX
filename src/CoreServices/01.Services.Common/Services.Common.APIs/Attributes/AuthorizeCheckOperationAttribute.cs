using Microsoft.AspNetCore.Mvc;
using Services.Common.APIs.Filters;
using Services.Common.Authorization;

namespace Services.Common.APIs.Attributes
{
    public class AuthorizeCheckOperationAttribute : TypeFilterAttribute
    {
        public AuthorizeCheckOperationAttribute(EAuthorizeType authorizeType, string crudName = "") : base(typeof(AuthorizeCheckOperationFilter))
        {
            Arguments = new object[] { authorizeType, crudName };
        }
    }
}