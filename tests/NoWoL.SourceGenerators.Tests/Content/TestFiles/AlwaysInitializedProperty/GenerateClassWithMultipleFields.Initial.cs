﻿using System.Collections.Generic;

namespace Test
{
    public partial class TestClass
    {
        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        private List<int> _field1;

        /// <summary>
        /// Some documentation
        /// </summary>
        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        private List<int> _field2;

        private List<int> _field3;

        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        private List<int> _field4;

        [NoWoL.SourceGenerators.AlwaysInitializedProperty]
        private List<int> _field5;
    }
}