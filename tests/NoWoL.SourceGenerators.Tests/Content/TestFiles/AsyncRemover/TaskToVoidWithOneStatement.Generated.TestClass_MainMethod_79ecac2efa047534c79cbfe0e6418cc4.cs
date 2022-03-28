using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        public void MainMethod()
        {
            TheMethod();
        //await TheMethodAsync().ConfigureAwait(false);
        }
    }
}