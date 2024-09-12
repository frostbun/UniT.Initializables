#nullable enable
namespace UniT.Initializables
{
    using System;
    using System.Collections.Generic;
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
        private readonly IReadOnlyList<IEarlyInitializable>      earlyInitializables;
        private readonly IReadOnlyList<IAsyncEarlyInitializable> asyncEarlyInitializables;
        private readonly IReadOnlyList<IInitializable>           initializables;
        private readonly IReadOnlyList<IAsyncInitializable>      asyncInitializables;
        private readonly IReadOnlyList<ILateInitializable>       lateInitializables;
        private readonly IReadOnlyList<IAsyncLateInitializable>  asyncLateInitializables;

        [Preserve]
        public InitializableManager(
            IEnumerable<IEarlyInitializable>      earlyInitializables,
            IEnumerable<IAsyncEarlyInitializable> asyncEarlyInitializables,
            IEnumerable<IInitializable>           initializables,
            IEnumerable<IAsyncInitializable>      asyncInitializables,
            IEnumerable<ILateInitializable>       lateInitializables,
            IEnumerable<IAsyncLateInitializable>  asyncLateInitializables
        )
        {
            this.earlyInitializables      = earlyInitializables.ToArray();
            this.asyncEarlyInitializables = asyncEarlyInitializables.ToArray();
            this.initializables           = initializables.ToArray();
            this.asyncInitializables      = asyncInitializables.ToArray();
            this.lateInitializables       = lateInitializables.ToArray();
            this.asyncLateInitializables  = asyncLateInitializables.ToArray();
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