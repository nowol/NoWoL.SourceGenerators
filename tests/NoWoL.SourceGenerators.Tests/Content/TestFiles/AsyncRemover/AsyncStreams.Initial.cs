using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.ExperimentalAsyncRemover()]
        public async Task MainMethodAsync()
        {
            await foreach (var n in SimulateWorkAsync())
            {
            }

            foreach (var n in new string[] { "hello" })
            {
            }
        }

        public async IAsyncEnumerable<string> SimulateWorkAsync()
        {
            await Task.Delay(3);

            yield return "Hello";
        }

        public IEnumerable<string> SimulateWorkAsync()
        {
            yield return "Hello";
        }
    }
}