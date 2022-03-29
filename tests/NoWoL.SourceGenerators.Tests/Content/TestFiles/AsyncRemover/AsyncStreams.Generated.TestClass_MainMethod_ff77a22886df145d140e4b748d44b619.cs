using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [System.CodeDom.Compiler.AsyncToSyncConverterGenerator("ExceptionGenerator", "1.0.0.0")]
        public void MainMethod()
        {
            foreach (var n in SimulateWork())
            {
            }

            foreach (var n in new string[]{"hello"})
            {
            }
        }
    }
}