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