using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task MainMethodAsync(int param, Func<int, string, string> param2Async, Func<int, string, Task> param3Async, Func<int, string, Task<TestClass>> param4Async, Func<string> param5, Func<Task> param6Async, System.Func<Task<TestClass>> param7Async)
        {
            param2Async(1, "a");
            await param3Async(1, "a");
            await param4Async(1, "a").ConfigureAwait(false);
            string s = param5();
            await param6Async();
            TestClass tc = await param7Async();
            await TheMethodAsync(param2Async, param3Async, param4Async, param5, param6Async, param7Async).ConfigureAwait(false);

            async Task TheMethodAsync(Func<int, string, string> paramAAsync, Func<int, string, Task> paramBAsync, System.Func<int, string, Task<TestClass>> paramCAsync, Func<string> paramDAsync, Func<Task> paramEAsync, Func<Task<TestClass>> paramFAsync)
            {
                await Task.Delay(3).ConfigureAwait(false);

                paramAAsync(1, "a");
                await paramBAsync(1, "a");
                await paramCAsync(1, "a").ConfigureAwait(false);
                string s2 = paramDAsync();
                await paramEAsync();
                TestClass tc2 = await paramFAsync();
            }
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