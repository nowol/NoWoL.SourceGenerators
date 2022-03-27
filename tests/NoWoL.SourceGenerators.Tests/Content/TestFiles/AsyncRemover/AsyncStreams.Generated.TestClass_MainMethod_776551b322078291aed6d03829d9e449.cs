using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
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