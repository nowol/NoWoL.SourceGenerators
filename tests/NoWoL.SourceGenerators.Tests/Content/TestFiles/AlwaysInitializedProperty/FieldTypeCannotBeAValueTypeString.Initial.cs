using System.Collections.Generic;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        private string _field1;
    }
}