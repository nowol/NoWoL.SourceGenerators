﻿using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        /// <summary>
        /// The summary
        /// </summary>
        /// <returns>The return</returns>
        [AnotherAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("AsyncToSyncConverterGenerator", "1.0.0.0")]
        public void MainMethod()
        {
            TheMethod();
        }
    }
}