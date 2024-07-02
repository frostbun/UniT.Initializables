#if UNIT_DI
#nullable enable
namespace UniT.Initializables
{
    using UniT.DI;

    public static class DIBinder
    {
        public static void AddInitializableManager(this DependencyContainer container)
        {
            if (container.Contains<IInitializableManager>()) return;
            container.AddInterfaces<InitializableManager>();
        }
    }
}
#endif