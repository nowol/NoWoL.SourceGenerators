namespace AnotherNs.NoWoL.SourceGenerators
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ExceptionGeneratorAttribute : System.Attribute
    { }
}

namespace Test
{
    [AnotherNs.NoWoL.SourceGenerators.ExceptionGenerator]
    public class TestClass
    {
    }

    public enum SSS
    {
        J
    }
}