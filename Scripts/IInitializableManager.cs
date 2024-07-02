#nullable enable
namespace UniT.Initializables
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IInitializableManager
    {
        #if UNIT_UNITASK
        public UniTask InitializeAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator InitializeAsync(IProgress<float>? progress = null);
        #endif
    }
}