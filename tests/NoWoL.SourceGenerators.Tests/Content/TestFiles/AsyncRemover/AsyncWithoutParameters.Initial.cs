using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task<int> MainMethodAsync(Func<Task<string>> param2Async)
        {
            string str = await param2Async().ConfigureAwait(true);

            return 3;
        }
    }
}