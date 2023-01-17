using System.Collections.Generic;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        private readonly List<int> _field1;
    }
}