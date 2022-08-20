namespace Test
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ExceptionGeneratorAttribute : System.Attribute
    {

    }

    [ExceptionGenerator]
    public class TestClass
    {
    }
}