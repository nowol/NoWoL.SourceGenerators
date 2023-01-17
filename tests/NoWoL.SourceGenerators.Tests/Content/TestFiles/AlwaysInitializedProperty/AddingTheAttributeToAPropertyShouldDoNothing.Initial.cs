using System.Collections.Generic;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        public List<int> Field1 { get; set; }
    }
}