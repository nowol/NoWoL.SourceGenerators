using System;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("AsyncToSyncConverterGenerator", "1.0.0.0")]
        public void MainMethod()
        {
            System.Threading.Thread.Sleep(123);
            System.Threading.Thread.Sleep(456);
            System.Threading.Thread.Sleep(TimeSpan.MaxValue);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(7));
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(8));
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(9));
        }
    }
}