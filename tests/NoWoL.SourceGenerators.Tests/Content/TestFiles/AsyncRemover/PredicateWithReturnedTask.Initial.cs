using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task MainMethodAsync()
        {
            await TheMethodAsync(() => SimulateWorkAsync(3000)).ConfigureAwait(false);
        }

        public async Task TheMethodAsync(Func<Task> funky)
        {
            await funky().ConfigureAwait(false);
        }

        public async Task SimulateWorkAsync(int value)
        {
            await Task.Delay(3000).ConfigureAwait(false);
        }

        public int TheMethod()
        {
            return 3;
        }
    }
}