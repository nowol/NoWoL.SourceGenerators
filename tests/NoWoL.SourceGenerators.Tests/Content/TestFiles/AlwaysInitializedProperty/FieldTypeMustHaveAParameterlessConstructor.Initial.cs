using System.Collections.Generic;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        private RefType _field1;
    }

    public class RefType
    {
        public RefType(int i)
        { }
    }
}