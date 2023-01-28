using System;

namespace NoWoL.SourceGenerators
{
    internal struct ClassDefinition : IEquatable<ClassDefinition>
    {
        public string Name { get; set; }

        public string Modifier { get; set; }

        public bool Equals(ClassDefinition other)
        {
            return Name == other.Name
                   && Modifier == other.Modifier;
        }

        public override bool Equals(object? obj)
        {
            return obj is ClassDefinition other && Equals(other);
        }

        public override int GetHashCode()
        {
            HashCode hash = default;

            hash.Add(Name);
            hash.Add(Modifier);

            return hash.ToHashCode();
        }
    }
}