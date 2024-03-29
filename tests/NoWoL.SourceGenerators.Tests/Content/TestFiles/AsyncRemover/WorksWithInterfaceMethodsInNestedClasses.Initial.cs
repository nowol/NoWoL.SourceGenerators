﻿using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        public partial class TestClass2
        {
            public partial interface ITestInterface
            {
                /// <summary>
                /// The summary
                /// </summary>
                /// <param name="param">The param</param>
                /// <param name="param2Async">The 2nd param</param>
                /// <param name="param3">The 3rd param</param>
                /// <param name="token">The cancel token</param>
                /// <returns>The return</returns>
                [NoWoL.SourceGenerators.AsyncToSyncConverter()]
                Task<string> MainMethodAsync(int param, Func<int, string, string> param2Async, Func<int, string, Task> param3, System.Threading.CancellationToken token);
            }
        }
    }
}