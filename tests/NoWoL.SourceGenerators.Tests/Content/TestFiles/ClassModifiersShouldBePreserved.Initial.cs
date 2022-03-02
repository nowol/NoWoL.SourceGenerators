namespace Test
{
    [NoWoL.SourceGenerators.ExceptionGenerator()]
    public partial class TestClassPublic
    {
    }

    [NoWoL.SourceGenerators.ExceptionGenerator()]
    partial class TestClassNone
    {
    }

    [NoWoL.SourceGenerators.ExceptionGenerator()]
    internal partial class TestClassInternal
    {
    }

    [NoWoL.SourceGenerators.ExceptionGenerator()]
    internal abstract partial class TestClassInternalAbstract
    {
    }
}