using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        internal partial class InnerTestClass
        {
            /// <summary>
            /// The summary
            /// </summary>
            /// <param name="param">The param</param>
            /// <param name="param2Async">The 2nd param</param>
            /// <param name="param3">The 3rd param</param>
            /// <param name="token">The cancel token</param>
            /// <returns>The return</returns>
            [NoWoL.SourceGenerators.AsyncToSyncConverter()]
            public async Task<string> MainMethodAsync(int param, Func<int, string, string> param2Async, Func<int, string, Task> param3, System.Threading.CancellationToken token)
            {
                string str = await param2Async(1, "").ConfigureAwait(true);

                for (int i = 0; i < 24; i++)
                {
                    int someInt = 3498 * i;
                    Console.WriteLine("Hello");
                }

                await foreach (var n in SimulateWorkStreamAsync())
                {
                    await TheMethodAsync(async () => await SimulateWorkAsync(3000).ConfigureAwait(false)).ConfigureAwait(false);
                    await TheMethodAsync(async () => await Task.Delay(3000, token).ConfigureAwait(false)).ConfigureAwait(false);

                    await AnotherMethodAsync(SimulateWorkAsync).ConfigureAwait(false);

                    async Task AnotherMethodAsync(Func<int, Task> funkyAsync)
                    {
                        await funkyAsync(3);
                    }
                }

                foreach (var n in new string[] { "hello" })
                { }

                async Task TheMethodAsync(Func<Task> funkyAsync)
                {
                    await funkyAsync().ConfigureAwait(false);
                }

                async Task<int> SimulateWorkAsync(int value)
                {
                    await Task.Delay(3000, token).ConfigureAwait(false);

                    return 3;

                    ValueTask TheMethodValueAsync(Func<ValueTask> funkyAsync)
                    {
                        return funkyAsync();
                    }

                    async ValueTask AnotherMethodValueAsync(Func<int, ValueTask> funkyAsync)
                    {
                        await funkyAsync(3).ConfigureAwait(false);
                    }

                    async ValueTask<int> SimulateWorkValueAsync(int value)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), token).ConfigureAwait(false);

                        return 3;
                    }

                    Task ReturnNonGenericTaskAsync(Func<Task> funkyAsync)
                    {
                        return funkyAsync();
                    }

                    Task<int> ReturnGenericTaskAsync(Func<Task<int>> funkyAsync)
                    {
                        return funkyAsync();
                    }

                    int NonAsyncMethod(int nonAsyncParam)
                    {
                        return 4;
                    }
                }

                return await param2Async(1, "").ConfigureAwaitWithCulture(true);
            }

            public async IAsyncEnumerable<string> SimulateWorkStreamAsync()
            {
                await Task.Delay(3);

                yield return "Hello";
            }

            public IEnumerable<string> SimulateWorkStream()
            {
                yield return "Hello";
            }
        }
    }
}