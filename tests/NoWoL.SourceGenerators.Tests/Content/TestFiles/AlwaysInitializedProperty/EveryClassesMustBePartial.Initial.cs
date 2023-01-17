using System.Collections.Generic;

namespace Test
{
    public class ParentTestClass
    {
        public partial class TestClass
        {
            [NoWoL.SourceGenerators.AlwaysInitializedProperty]
            private List<int> _field1;
        }
    }
}