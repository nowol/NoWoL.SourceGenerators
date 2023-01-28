namespace Test
{
    public partial class Level1
    {
        private partial class Level2
        {
            private partial class Level3
            {
                [NoWoL.SourceGenerators.ExceptionGenerator()]
                public partial class TestClass
                {
                }
            }
        }
    }
}
namespace Test
{
    public partial class Level1B
    {
        private static partial class Level2Static
        {
            [NoWoL.SourceGenerators.ExceptionGenerator()]
            public partial class TestClassStatic
            {
            }
        }
    }
}