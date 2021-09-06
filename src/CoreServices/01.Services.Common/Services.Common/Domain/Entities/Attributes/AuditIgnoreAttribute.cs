using System;

namespace Services.Common.Domain.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AuditIgnoreAttribute : Attribute
    {
    }
}