using System.Threading.Tasks;

public partial class TestClass
{
    [NoWoL.SourceGenerators.AsyncToSyncConverter()]
    public async Task MainMethodAsync()
    {
        await TheMethodAsync();
    }

    public async Task TheMethodAsync()
    {
        await Task.Delay(3).ConfigureAwait(false);
    }

    public void TheMethod()
    {
    }
}