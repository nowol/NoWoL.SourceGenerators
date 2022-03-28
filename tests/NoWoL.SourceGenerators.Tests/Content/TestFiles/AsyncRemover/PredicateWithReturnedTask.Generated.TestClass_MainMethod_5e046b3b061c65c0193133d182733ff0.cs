using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        public void MainMethod()
        {
            TheMethod(() => SimulateWork(3000));
        }
    }
}