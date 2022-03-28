using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task MainMethodAsync(int param, Func<int, string, string> param2Async, Func<int, string, Task> param3, Func<int, string, Task<TestClass>> param4, Func<string> param5, Func<Task> param6, System.Func<Task<TestClass>> param7)
        {
            param2Async(1, "a");
            await param3(1, "a");
            await param4(1, "a").ConfigureAwait(false);
            string s = param5();
            await param6();
            TestClass tc = await param7();
            await TheMethodAsync(param2Async, param3, param4, param5, param6, param7).ConfigureAwait(false);

            async Task TheMethodAsync(Func<int, string, string> paramAAsync, Func<int, string, Task> paramB, System.Func<int, string, Task<TestClass>> paramC, Func<string> paramD, Func<Task> paramE, Func<Task<TestClass>> paramF)
            {
                await Task.Delay(3).ConfigureAwait(false);

                paramAAsync(1, "a");
                await paramB(1, "a");
                await paramC(1, "a").ConfigureAwait(false);
                string s2 = paramD();
                await paramE();
                TestClass tc2 = await paramF();
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