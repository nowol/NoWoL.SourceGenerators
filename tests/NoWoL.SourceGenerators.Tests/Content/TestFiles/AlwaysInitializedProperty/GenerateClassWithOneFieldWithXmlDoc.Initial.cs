using System.Collections.Generic;

namespace Test
{
    public partial class TestClass
    {
        /// <summary>
        /// Gets or sets the documentation
        /// </summary>
        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        private List<int> _field1;
    }
}