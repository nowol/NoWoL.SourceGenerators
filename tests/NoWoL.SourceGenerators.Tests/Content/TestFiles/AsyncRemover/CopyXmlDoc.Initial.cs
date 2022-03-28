using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        /// <summary>
        /// The summary
        /// </summary>
        /// <param name="param">The param</param>
        /// <param name="param2">The 2nd param</param>
        /// <returns>The return</returns>
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task MainMethodAsync(int param, Func<int, string, Task<string>> param2)
        {
            await TheMethodAsync().ConfigureAwait(false);
        }

        public async Task TheMethodAsync()
        {
            await Task.Delay(3).ConfigureAwait(false);
        }

        public int TheMethod()
        {
            return 3;
        }
    }
}