﻿using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("AsyncToSyncConverterGenerator", "1.0.0.0")]
        public int MainMethod(System.Func<int, string> param1, System.Func<int, object, string> param2, System.Func<decimal, string> param3, Action<decimal> param4)
        {
            string str = param1(1);
            str = param2(1, 48);
            str = param2(1, typeof(int));
            str = param3(1.0m);

            param4(8);

            return 3;
        }
    }
}