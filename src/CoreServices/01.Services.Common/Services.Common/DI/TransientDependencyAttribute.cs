using Microsoft.Extensions.DependencyInjection;

namespace Services.Common.DI
{
    public class TransientDependencyAttribute : DependencyAttribute
    {
        public TransientDependencyAttribute() : base(ServiceLifetime.Transient)
        {
        }
    }
}