using System;
using System.Collections.Immutable;
using System.Text;
using System.Xml.Linq;

namespace NoWoL.SourceGenerators.AlwaysInitializedProperty
{
    internal struct AlwaysInitializedPropertyClassDefinition : IEquatable<AlwaysInitializedPropertyClassDefinition>
    {
        public ImmutableArray<AlwaysInitializedPropertyFieldDefinition> Fields { get; set; }
        
        public ClassWithParentAndNamespaceDefinition AdvClassDef { get; set; }

        public bool Equals(AlwaysInitializedPropertyClassDefinition other)
        {
            return AdvClassDef.Equals(other.AdvClassDef)
                   && ComparisonHelpers.AreImmutableArrayEquals(Fields, other.Fields);
        }

        public override bool Equals(object? obj)
        {
            return obj is AlwaysInitializedPropertyClassDefinition other && Equals(other);
        }

        public override int GetHashCode()
        {
            HashCode hash = default;

            hash.Add(AdvClassDef);

            if (!Fields.IsDefaultOrEmpty)
            {
                foreach (var field in Fields)
                {
                    hash.Add(field);
                }
            }

            return hash.ToHashCode();
        }
    }
}
