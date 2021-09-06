using Services.Common.DI;
using System;

namespace Services.Common.GuidUtils
{
    [SingletonDependency(ServiceType = typeof(IGuidGenerator))]
    public class RegularGuidGenerator : IGuidGenerator
    {
        public virtual Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}