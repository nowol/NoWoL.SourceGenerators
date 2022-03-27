using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        public async Task MainMethodAsync()
        {
            await TheMethodAsync().ConfigureAwait(false);
        }

        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        public async Task MainMethod2Async()
        {
            await Task.Delay(333).ConfigureAwait(false);
        }

        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        public async Task<int> MainMethodAsync()
        {
            await TheMethodAsync().ConfigureAwait(false);

            return 3;
        }

        public async Task TheMethodAsync()
        {
            await Task.Delay(3).ConfigureAwait(false);
        }

        public void TheMethod()
        {
        }

        public Task Return3Async()
        {
            return Task.FromResult(3);
        }

        public void Return3()
        {
            return 3;
        }
    }
}