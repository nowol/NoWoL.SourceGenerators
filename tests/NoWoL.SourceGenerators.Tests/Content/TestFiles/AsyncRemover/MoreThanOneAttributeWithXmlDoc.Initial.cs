using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        /// <summary>
        /// The summary
        /// </summary>
        /// <returns>The return</returns>
        [AnotherAttribute()]
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
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