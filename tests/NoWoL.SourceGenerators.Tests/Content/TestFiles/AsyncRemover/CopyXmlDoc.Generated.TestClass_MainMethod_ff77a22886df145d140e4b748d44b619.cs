﻿using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        /// <summary>
        /// The summary
        /// </summary>
        /// <param name = "param">The param</param>
        /// <param name = "param2">The 2nd param</param>
        /// <returns>The return</returns>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("AsyncToSyncConverterGenerator", "1.0.0.0")]
        public void MainMethod(int param, System.Func<int, string, string> param2)
        {
            TheMethod();
        }
    }
}