using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [AnotherAttribute()]
        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        public Task MainMethodAsync()
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