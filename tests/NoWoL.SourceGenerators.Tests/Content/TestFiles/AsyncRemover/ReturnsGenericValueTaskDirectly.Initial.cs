using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        public async ValueTask<int> MainMethodAsync()
        {
            return await TheMethodAsync();
        }

        public async ValueTask<int> TheMethodAsync()
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