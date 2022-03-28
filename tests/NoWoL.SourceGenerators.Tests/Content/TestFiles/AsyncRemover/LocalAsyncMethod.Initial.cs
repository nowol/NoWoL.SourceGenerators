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
            await TheMethodAsync(() => SimulateWorkAsync(5000)).ConfigureAwait(false);

            await AnotherMethodAsync(SimulateWorkAsync).ConfigureAwait(false);

            async Task TheMethodAsync(Func<Task> funkyAsync)
            {
                await funkyAsync().ConfigureAwait(false);
            }

            async Task AnotherMethodAsync(Func<int, Task> funkyAsync)
            {
                await funkyAsync(3).ConfigureAwait(false);
            }

            async Task<int> SimulateWorkAsync(int value)
            {
                await Task.Delay(3000).ConfigureAwait(false);
                return 3;
            }

            async ValueTask TheMethodValueAsync(Func<ValueTask> funkyAsync)
            {
                await funkyAsync().ConfigureAwait(false);
            }

            async ValueTask AnotherMethodValueAsync(Func<int, ValueTask> funkyAsync)
            {
                await funkyAsync(3).ConfigureAwait(false);
            }

            async ValueTask<int> SimulateWorkValueAsync(int value)
            {
                await Task.Delay(3000).ConfigureAwait(false);
                return 3;
            }

            Task ReturnNonGenericTaskAsync(Func<Task> funkyAsync)
            {
                return funkyAsync();
            }

            Task<int> ReturnGenericTaskAsync(Func<Task<int>> funkyZAsync)
            {
                return funkyZAsync();
            }

            Task<int> ReturnGenericTaskWhereTheReturnShouldBeKept(Func<Task<int>> funkyAsync)
            {
                return funkyAsync();
            }
        }
	}
}