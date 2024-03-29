﻿using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AsyncToSyncConverter()]
        public Task MainMethodAsync()
        {
            return TheMethod();
        }

        public async Task TheMethod()
        {
            await Task.Delay(3).ConfigureAwait(false);
        }
    }
}