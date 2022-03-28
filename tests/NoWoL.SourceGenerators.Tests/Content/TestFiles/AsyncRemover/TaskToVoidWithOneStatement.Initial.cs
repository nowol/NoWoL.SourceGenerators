using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task MainMethodAsync()
        {
            await TheMethodAsync();
            //await TheMethodAsync().ConfigureAwait(false);
        }

        public async Task TheMethodAsync()
        {
            await Task.Delay(3).ConfigureAwait(false);
        }

        public void TheMethod()
        {
        }
    }
}