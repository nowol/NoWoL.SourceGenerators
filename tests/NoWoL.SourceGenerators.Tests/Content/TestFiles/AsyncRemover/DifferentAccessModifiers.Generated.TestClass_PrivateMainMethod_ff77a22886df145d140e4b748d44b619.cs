using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [System.CodeDom.Compiler.AsyncToSyncConverterGenerator("ExceptionGenerator", "1.0.0.0")]
        private void PrivateMainMethod()
        {
            TheMethod();
        }
    }
}