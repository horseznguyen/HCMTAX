using Services.Common.Modules;

namespace Services.Common
{
    /// <summary>
    /// Kernel (core) module of the FW system.
    /// No need to depend on this, it's automatically the first module always.
    /// </summary>
    public sealed class KernelModule : Module
    {
    }
}