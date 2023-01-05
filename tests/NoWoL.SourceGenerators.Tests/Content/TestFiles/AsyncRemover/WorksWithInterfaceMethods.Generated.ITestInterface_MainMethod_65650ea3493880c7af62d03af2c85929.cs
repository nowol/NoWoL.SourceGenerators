using System;
using System.Threading.Tasks;

namespace Test
{
    public partial interface ITestInterface
    {
        /// <summary>
        /// The summary
        /// </summary>
        /// <param name = "param">The param</param>
        /// <param name = "param2Async">The 2nd param</param>
        /// <param name = "param3">The 3rd param</param>
        /// <param name = "token">The cancel token</param>
        /// <returns>The return</returns>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("AsyncToSyncConverterGenerator", "1.0.0.0")]
        string MainMethod(int param, Func<int, string, string> param2Async, System.Action<int, string> param3, System.Threading.CancellationToken token);
    }
}