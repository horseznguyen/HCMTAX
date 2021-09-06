using System;

namespace EFCore.Common
{
    public class EfCoreOptions
    {
        public int MaxRetryCount { get; set; }
        public int MaxRetryDelayInSecond { get; set; }
        public int CommandTimeOut { get; set; }
        public string ConnectionStrings { get; set; }
        public DateTimeKind DateTimeKind { get; set; }
    }
}