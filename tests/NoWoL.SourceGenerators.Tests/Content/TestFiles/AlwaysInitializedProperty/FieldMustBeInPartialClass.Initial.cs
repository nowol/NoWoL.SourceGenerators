using System.Collections.Generic;

namespace Test
{
    public class TestClass
    {
        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        private List<int> _field1;
    }
}