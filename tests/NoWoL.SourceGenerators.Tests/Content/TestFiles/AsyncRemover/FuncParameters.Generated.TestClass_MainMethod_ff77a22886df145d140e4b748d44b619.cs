using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("ExceptionGenerator", "1.0.0.0")]
        public void MainMethod(int param, Func<int, string, string> param2Async, System.Action<int, string> param3, System.Func<int, string, Test.TestClass> param4, Func<string> param5, System.Action param6, System.Func<Test.TestClass> param7)
        {
            param2Async(1, "a");
            param3(1, "a");
            param4(1, "a");
            string s = param5();
            param6();
            TestClass tc = param7();
            TheMethod(param2Async, param3, param4, param5, param6, param7);
            void TheMethod(Func<int, string, string> paramAAsync, System.Action<int, string> paramB, System.Func<int, string, Test.TestClass> paramC, Func<string> paramD, System.Action paramE, System.Func<Test.TestClass> paramF)
            {
                System.Threading.Thread.Sleep(3);
                paramAAsync(1, "a");
                paramB(1, "a");
                paramC(1, "a");
                string s2 = paramD();
                paramE();
                TestClass tc2 = paramF();
            }
        }
    }
}