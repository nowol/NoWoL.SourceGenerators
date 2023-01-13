using System;
using System.Collections.Generic;
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
            /// 
            /// <param name="param">The param</param>
            /// <param name="param2Async">The 2nd param</param>
            /// <param name="param3">The 3rd param</param>
            /// <param name="cancellationToken">The cancel token</param>
            /// <returns>The return</returns>
            [NoWoL.SourceGenerators.AsyncToSyncConverter()]
            public async Task<string> MainMethodAsync(int param, Func<int, string, Task<string>> param2Async, Func<int, string, Task> param3, System.Threading.CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string str = await param2Async(1, "").ConfigureAwait(true);

                await foreach (var n in SimulateWorkStreamAsync())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await TheMethodAsync(async () => await SimulateWorkAsync(3000, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
                    await TheMethodAsync(async () => await Task.Delay(3000, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
                    await TheMethodAsync(async () => await Task.Delay(3001, cancellationToken)).ConfigureAwait(false);
                    await TheMethodAsync(async () => await Task.Delay(3002, cancellationToken));
                    await TheMethodAsync(async () => await Task.Delay(3003));

                    await AnotherMethodAsync(SimulateWorkAsync).ConfigureAwait(false);

                    async Task AnotherMethodAsync(Func<int, System.Threading.CancellationToken, Task> funkyAsync)
                    {
                        await funkyAsync(3, System.Threading.CancellationToken.None);
                    }
                }

                foreach (var n in new string[] { "hello" })
                { }

                async Task TheMethodAsync(Func<Task> funky9000Async)
                {
                    await funky9000Async().ConfigureAwait(false);
                }

                async Task<int> SimulateWorkAsync(int value, System.Threading.CancellationToken anotherToken)
                {
                    anotherToken.ThrowIfCancellationRequested();
                    await Task.Delay(3000, cancellationToken).ConfigureAwait(false);

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
                        await Task.Delay(TimeSpan.FromSeconds(3), anotherToken).ConfigureAwait(false);
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

                    Task ReturnTaskWithNormalReturnStatementAsync(Func<Task> funkyAsync)
                    {
                        await funkyAsync();
                        return;
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