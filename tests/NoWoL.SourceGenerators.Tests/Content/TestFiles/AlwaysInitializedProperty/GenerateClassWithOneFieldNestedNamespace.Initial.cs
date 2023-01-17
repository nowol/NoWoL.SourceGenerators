using System.Collections.Generic;

namespace Test
{
    namespace Test2
    {
        public partial class TestClass
        {
            [NoWoL.SourceGenerators.AlwaysInitializedProperty]
            private List<int> _field1;
        }
    }
}