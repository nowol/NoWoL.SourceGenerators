using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task<int> MainMethodAsync()
        {
            return await TheMethodAsync();
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