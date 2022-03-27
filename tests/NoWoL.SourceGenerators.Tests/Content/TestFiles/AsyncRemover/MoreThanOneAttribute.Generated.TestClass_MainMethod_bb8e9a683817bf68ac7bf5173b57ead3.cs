using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [AnotherAttribute()]
        public void MainMethod()
        {
            TheMethod();
        }
    }
}