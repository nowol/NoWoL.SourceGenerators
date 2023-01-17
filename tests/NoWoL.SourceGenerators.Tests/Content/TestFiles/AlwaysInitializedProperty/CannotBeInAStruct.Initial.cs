using System.Collections.Generic;

namespace Test
{
    public struct TestClass
    {
        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        private List<int> _field1;
    }
}