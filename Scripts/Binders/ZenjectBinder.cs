#if UNIT_ZENJECT
#nullable enable
namespace UniT.Initializables
{
    using Zenject;

    public static class ZenjectBinder
    {
        public static void BindInitializableManager(this DiContainer container)
        {
            if (container.HasBinding<IInitializableManager>()) return;
            container.BindInterfacesTo<InitializableManager>().AsSingle();
        }
    }
}
#endif