using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        public async Task MainMethodAsync()
        {
            await foreach (var n in SimulateWork())
            {
            }

            foreach (var n in new string[] { "hello" })
            {
            }
        }

        public async IAsyncEnumerable<string> SimulateWork()
        {
            await Task.Delay(3);

            yield return "Hello";
        }
    }
}