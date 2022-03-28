using System.Threading.Tasks;

namespace Test
{
    public class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task MainMethodAsync()
        {
            await TheMethodAsync();
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