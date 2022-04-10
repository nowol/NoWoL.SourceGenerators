using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("ExceptionGenerator", "1.0.0.0")]
        public void MainMethod()
        {
            TheMethod(() => SimulateWork(3000));
            int SimulateWork(int value)
            {
                System.Threading.Thread.Sleep(3000);
                return 3;
            }
        }
    }
}