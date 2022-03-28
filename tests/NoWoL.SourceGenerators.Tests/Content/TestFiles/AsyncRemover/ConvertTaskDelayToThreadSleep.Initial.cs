using System;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
		public async Task MainMethodAsync(CancellationToken ctToken)
        {
            await Task.Delay(123);
            await Task.Delay(456).ConfigureAwait(true);
            await Task.Delay(TimeSpan.MaxValue);
            await Task.Delay(TimeSpan.FromSeconds(7)).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(8), CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(9), ctToken).ConfigureAwait(false);
        }
    }
}