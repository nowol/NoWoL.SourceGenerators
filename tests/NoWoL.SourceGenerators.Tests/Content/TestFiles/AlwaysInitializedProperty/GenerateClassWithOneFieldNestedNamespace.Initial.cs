using System.Collections.Generic;

namespace Test
{
    namespace Test2
    {
        public partial class TestClass
        {
            public partial class TestClass2
            {
                public partial class TestClass3
                {
                    [NoWoL.SourceGenerators.AlwaysInitializedProperty]
                    private List<int> _field1;
                }
            }
        }
    }
}