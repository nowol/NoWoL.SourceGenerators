using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        internal async Task InternalMainMethodAsync()
        {
            await TheMethodAsync();
        }

        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        private async Task PrivateMainMethodAsync()
        {
            await TheMethodAsync();
        }

        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        private static async Task PrivateStaticMainMethodAsync()
        {
            await TheMethodAsync();
        }

        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        protected async Task ProtectedMainMethodAsync()
        {
            await TheMethodAsync();
        }

        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        public async Task PublicMainMethodAsync()
        {
            await TheMethodAsync();
        }

        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        public virtual async Task PublicVirtualMainMethodAsync()
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