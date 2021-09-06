using System;

namespace Services.Common.Domain.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class AuditIncludeAttribute : Attribute
    {
    }
}