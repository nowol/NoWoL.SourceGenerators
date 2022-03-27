using System.Threading.Tasks;

namespace Test
{
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false)]
    public class AnotherAttr1Attribute : System.Attribute
    {

    }

    public partial class TestClass
    {
        [Test.AnotherAttr1Attribute()]
        [NoWoL.AnotherNamespace.ExperimentalAsyncRemover()]
        public async Task MainMethodAsync()
        {
            await TheMethodAsync().ConfigureAwaitWithCulture(false);
        }

        public async Task TheMethodAsync()
        {
            await Task.Delay(3).ConfigureAwait(false);
        }

        public int TheMethod()
        {
            return 3;
        }
    }
}