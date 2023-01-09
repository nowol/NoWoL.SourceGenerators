using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task<int> MainMethodAsync(Func<int, Task<string>> param1Async, Func<int, object, Task<string>> param2Async, Func<decimal, Task<string>> param3Async, Action<decimal> param4)
        {
            string str = await param1Async(1).ConfigureAwait(true);
            str = await param2Async(1, 48).ConfigureAwait(true);
            str = await param2Async(1, typeof(int));
            str = await param3Async(1.0m).ConfigureAwait(true);

            param4(8);

            return 3;
        }
    }
}