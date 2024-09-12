#if UNIT_ZENJECT
#nullable enable
namespace UniT.Initializables.DI
{
    using Zenject;
    using InitializableManager = InitializableManager;

    public static class InitializableManagerZenject
    {
        public static void BindInitializableManager(this DiContainer container)
        {
            if (container.HasBinding<IInitializableManager>()) return;
            container.BindInterfacesTo<InitializableManager>().AsSingle();
        }
    }
}
#endif