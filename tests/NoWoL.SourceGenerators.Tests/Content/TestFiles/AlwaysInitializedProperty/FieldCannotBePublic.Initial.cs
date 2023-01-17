using System.Collections.Generic;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        public List<int> _field1;
    }
}