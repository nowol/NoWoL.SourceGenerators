using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NoWoL.SourceGenerators.AlwaysInitializedProperty
{
    internal struct ClassWithParentAndNamespaceDefinition : IEquatable<ClassWithParentAndNamespaceDefinition>
    {
        public string? Namespace { get; set; }

        public List<ClassDefinition>? ParentClasses { get; set; }

        public ImmutableArray<string> UsingStatements { get; set; }

        public ClassDefinition ClassDef { get; set; }

        public bool Equals(ClassWithParentAndNamespaceDefinition other)
        {
            return Namespace == other.Namespace
                   && ComparisonHelpers.AreCollectionEquals(ParentClasses, other.ParentClasses)
                   && ComparisonHelpers.AreImmutableArrayEquals(UsingStatements, other.UsingStatements)
                   && ClassDef.Equals(other.ClassDef);
        }

        public override bool Equals(object? obj)
        {
            return obj is ClassWithParentAndNamespaceDefinition other && Equals(other);
        }

        public override int GetHashCode()
        {
            HashCode hash = default;

            if (!UsingStatements.IsDefaultOrEmpty)
            {
                foreach (var usingStatement in UsingStatements)
                {
                    hash.Add(usingStatement);
                }
            }

            hash.Add(Namespace);

            if (ParentClasses != null)
            {
                foreach (var parentClassDef in ParentClasses)
                {
                    hash.Add(parentClassDef);
                }
            }

            hash.Add(ClassDef);

            return hash.ToHashCode();
        }
    }
}