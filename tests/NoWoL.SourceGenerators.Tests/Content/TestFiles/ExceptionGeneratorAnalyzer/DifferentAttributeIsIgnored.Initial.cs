namespace AnotherNs
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class AnotherAttribute : System.Attribute
    { }
}

namespace Test
{
    [AnotherNs.Another]
    public class TestClass
    {
    }
}