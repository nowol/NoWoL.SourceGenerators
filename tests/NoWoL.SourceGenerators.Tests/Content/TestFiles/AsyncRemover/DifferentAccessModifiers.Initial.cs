using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        internal async Task InternalMainMethodAsync()
        {
            await TheMethodAsync();
        }

        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        private async Task PrivateMainMethodAsync()
        {
            await TheMethodAsync();
        }

        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        private static async Task PrivateStaticMainMethodAsync()
        {
            await TheMethodAsync();
        }

        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        protected async Task ProtectedMainMethodAsync()
        {
            await TheMethodAsync();
        }

        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public async Task PublicMainMethodAsync()
        {
            await TheMethodAsync();
        }

        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
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