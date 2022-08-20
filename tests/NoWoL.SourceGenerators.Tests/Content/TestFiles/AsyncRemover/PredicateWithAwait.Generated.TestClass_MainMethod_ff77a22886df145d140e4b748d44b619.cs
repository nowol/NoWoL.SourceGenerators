using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("AsyncToSyncConverterGenerator", "1.0.0.0")]
        public void MainMethod()
        {
            TheMethod(() => SimulateWork(3000));
        }
    }
}