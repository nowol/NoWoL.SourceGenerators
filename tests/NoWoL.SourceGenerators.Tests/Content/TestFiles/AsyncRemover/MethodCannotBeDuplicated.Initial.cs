using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task MainMethodAsync()
        {
            await TheMethodAsync(async () => await SimulateWorkAsync(3000).ConfigureAwait(false)).ConfigureAwait(false);
            
            async Task<int> SimulateWorkAsync(int value)
            {
                await Task.Delay(3000).ConfigureAwait(false);
                return 3;
            }
        }

        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task MainMethodAsync()
        {
            await TheMethodAsync(async () => await SimulateWorkAsync(3000).ConfigureAwait(false)).ConfigureAwait(false);
            
            async Task<int> SimulateWorkAsync(int value)
            {
                await Task.Delay(3000).ConfigureAwait(false);
                return 3;
            }
        }
    }
}