﻿using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("AsyncToSyncConverterGenerator", "1.0.0.0")]
        public int MainMethod(System.Func<string> param2)
        {
            string str = param2();

            return 3;
        }
    }
}