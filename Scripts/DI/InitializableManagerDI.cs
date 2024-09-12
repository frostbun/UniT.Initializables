#if UNIT_DI
#nullable enable
namespace UniT.Initializables.DI
{
    using UniT.DI;

    public static class InitializableManagerDI
    {
        public static void AddInitializableManager(this DependencyContainer container)
        {
            if (container.Contains<IInitializableManager>()) return;
            container.AddInterfaces<InitializableManager>();
        }
    }
}
#endif