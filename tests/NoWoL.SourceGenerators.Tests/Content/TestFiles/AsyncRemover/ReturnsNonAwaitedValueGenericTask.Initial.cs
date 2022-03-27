using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        public Task<int> MainMethodAsync()
        {
            return TheMethodAsync();
        }

        public async Task<int> TheMethodAsync()
        {
            await Task.Delay(3).ConfigureAwait(false);

            return 3;
        }

        public int TheMethod()
        {
            return 3;
        }
    }
}