#nullable enable
namespace UniT.Initializables
{
    using System;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class InitializableManager : IInitializableManager
    {
        private readonly IEarlyInitializable[]      earlyInitializables;
        private readonly IAsyncEarlyInitializable[] asyncEarlyInitializables;
        private readonly IInitializable[]           initializables;
        private readonly IAsyncInitializable[]      asyncInitializables;
        private readonly ILateInitializable[]       lateInitializables;
        private readonly IAsyncLateInitializable[]  asyncLateInitializables;

        [Preserve]
        public InitializableManager(
            IEarlyInitializable[]      earlyInitializables,
            IAsyncEarlyInitializable[] asyncEarlyInitializables,
            IInitializable[]           initializables,
            IAsyncInitializable[]      asyncInitializables,
            ILateInitializable[]       lateInitializables,
            IAsyncLateInitializable[]  asyncLateInitializables
        )
        {
            this.earlyInitializables      = earlyInitializables;
            this.asyncEarlyInitializables = asyncEarlyInitializables;
            this.initializables           = initializables;
            this.asyncInitializables      = asyncInitializables;
            this.lateInitializables       = lateInitializables;
            this.asyncLateInitializables  = asyncLateInitializables;
        }

        #if UNIT_UNITASK
        async UniTask IInitializableManager.InitializeAsync(IProgress<float>? progress, CancellationToken cancellationToken)
        {
            var subProgresses = progress.CreateSubProgresses(3).ToArray();
            this.earlyInitializables.ForEach(service => service.Initialize());
            await this.asyncEarlyInitializables.ForEachAsync(
                (service, progress, cancellationToken) => service.InitializeAsync(progress, cancellationToken),
                subProgresses[0],
                cancellationToken
            );
            subProgresses[0]?.Report(1);
            this.initializables.ForEach(service => service.Initialize());
            await this.asyncInitializables.ForEachAsync(
                (service, progress, cancellationToken) => service.InitializeAsync(progress, cancellationToken),
                subProgresses[1],
                cancellationToken
            );
            subProgresses[1]?.Report(1);
            this.lateInitializables.ForEach(service => service.Initialize());
            await this.asyncLateInitializables.ForEachAsync(
                (service, progress, cancellationToken) => service.InitializeAsync(progress, cancellationToken),
                subProgresses[2],
                cancellationToken
            );
            subProgresses[2]?.Report(1);
        }
        #else
        IEnumerator IInitializableManager.InitializeAsync(Action? callback, IProgress<float>? progress)
        {
            var subProgresses = progress.CreateSubProgresses(3).ToArray();
            this.earlyInitializables.ForEach(service => service.Initialize());
            yield return this.asyncEarlyInitializables.ForEachAsync(
                (service, progress) => service.InitializeAsync(progress: progress),
                progress: subProgresses[0]
            );
            subProgresses[0]?.Report(1);
            this.initializables.ForEach(service => service.Initialize());
            yield return this.asyncInitializables.ForEachAsync(
                (service, progress) => service.InitializeAsync(progress: progress),
                progress: subProgresses[1]
            );
            subProgresses[1]?.Report(1);
            this.lateInitializables.ForEach(service => service.Initialize());
            yield return this.asyncLateInitializables.ForEachAsync(
                (service, progress) => service.InitializeAsync(progress: progress),
                progress: subProgresses[2]
            );
            subProgresses[2]?.Report(1);
            callback?.Invoke();
        }
        #endif
    }
}