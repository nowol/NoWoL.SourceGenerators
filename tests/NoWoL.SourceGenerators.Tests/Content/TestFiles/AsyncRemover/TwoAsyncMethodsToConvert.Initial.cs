using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public Task MainMethod1Async()
        {
            return TheMethodAsync();
        }

        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public Task MainMethod2Async()
        {
            return TheMethodAsync();
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