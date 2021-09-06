using System;
using System.Collections.Generic;

namespace Services.Common.DI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class IocScannerOptions : Attribute
    {
        // TODO Hung
        public List<Func<Type, bool>> Excludes { get; set; }

        public IEnumerable<Type> TypesToExclude { get; set; }

        public string[] CommonPostfixes { get; set; } = { "AppService", "ApplicationService", "DomainService", "Service" };
    }
}