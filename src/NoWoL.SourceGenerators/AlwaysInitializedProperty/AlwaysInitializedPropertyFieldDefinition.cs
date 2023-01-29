using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NoWoL.SourceGenerators.AlwaysInitializedProperty
{
    internal struct AlwaysInitializedPropertyFieldDefinition : IEquatable<AlwaysInitializedPropertyFieldDefinition>
    {
        public string? Namespace { get; set; }

        public List<ClassDefinition>? ParentClasses { get; set; }

        public ClassDefinition ClassDef { get; set; }

        public ImmutableArray<string> UsingStatements { get; set; }

        public string? Name { get; set; }

        public string? Type { get; set; }

        public string? LeadingTrivia { get; set; }

        public DiagnosticDefinition DiagnosticDef { get; set; }

        public bool Equals(AlwaysInitializedPropertyFieldDefinition other)
        {
            return Namespace == other.Namespace
                   && ComparisonHelpers.AreCollectionEquals(ParentClasses, other.ParentClasses)
                   && ComparisonHelpers.AreImmutableArrayEquals(UsingStatements, other.UsingStatements)
                   && ClassDef.Equals(other.ClassDef)
                   && Name == other.Name
                   && Type == other.Type
                   && LeadingTrivia == other.LeadingTrivia
                   && DiagnosticDef.Equals(other.DiagnosticDef);
        }

        public override bool Equals(object? obj)
        {
            return obj is AlwaysInitializedPropertyFieldDefinition other && Equals(other);
        }

        public override int GetHashCode()
        {
            HashCode hash = default;

            hash.Add(Namespace);

            if (ParentClasses != null)
            {
                foreach (var parentClassDef in ParentClasses)
                {
                    hash.Add(parentClassDef);
                }
            }

            if (!UsingStatements.IsDefaultOrEmpty)
            {
                foreach (var usingStatement in UsingStatements)
                {
                    hash.Add(usingStatement);
                }
            }

            hash.Add(ClassDef);
            hash.Add(Name);
            hash.Add(Type);
            hash.Add(LeadingTrivia);
            hash.Add(DiagnosticDef);

            return hash.ToHashCode();
        }

        public void SetDiagnostic(DiagnosticDefinition definition)
        {
            if (DiagnosticDef.Initialized)
            {
                return;
            }

            DiagnosticDef = definition;
        }
    }
}
