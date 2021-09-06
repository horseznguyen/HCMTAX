using System;

namespace Services.Common.DI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SkipAutoBindAttribute : Attribute
    {
    }
}