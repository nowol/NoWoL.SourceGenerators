using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        /// <summary>
        /// The summary
        /// </summary>
        /// <returns>The return</returns>
        [AnotherAttribute()]
        public void MainMethod()
        {
            TheMethod();
        }
    }
}