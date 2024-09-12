#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Initializables.DI
{
    using VContainer;

    public static class InitializableManagerVContainer
    {
        public static void RegisterInitializableManager(this IContainerBuilder builder)
        {
            if (builder.Exists(typeof(IInitializableManager), true)) return;
            builder.Register<InitializableManager>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif