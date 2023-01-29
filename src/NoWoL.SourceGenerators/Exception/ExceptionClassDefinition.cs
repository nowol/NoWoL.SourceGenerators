using System;
using System.Collections.Generic;

namespace NoWoL.SourceGenerators
{
    internal struct ExceptionClassDefinition : IEquatable<ExceptionClassDefinition>
    {
        public string? Namespace { get; set; }

        public List<ClassDefinition>? ParentClasses { get; set; }

        public ClassDefinition ClassDef { get; set; }

        public List<string>? Messages { get; set; }

        public DiagnosticDefinition DiagnosticDef { get; set; }

        public bool Equals(ExceptionClassDefinition other)
        {
            var result = Namespace == other.Namespace
                         && ClassDef.Equals(other.ClassDef)
                         && DiagnosticDef.Equals(other.DiagnosticDef)
                         && ComparisonHelpers.AreCollectionEquals(ParentClasses, other.ParentClasses)
                         && ComparisonHelpers.AreCollectionEquals(Messages, other.Messages);

            // Uncomment for diagnostic
            //System.Diagnostics.Trace.WriteLine($"[nwl]----- {result}");
            //System.Diagnostics.Trace.WriteLine($"ns: {Namespace} -- {other.Namespace} ** {Namespace == other.Namespace}");
            //System.Diagnostics.Trace.WriteLine($"ClassDef name: {ClassDef.Name} -- {other.ClassDef.Name} ** {ClassDef.Equals(other.ClassDef)}");
            //System.Diagnostics.Trace.WriteLine($"ClassDef mod: {ClassDef.Modifier} -- {other.ClassDef.Modifier}");
            //System.Diagnostics.Trace.WriteLine($"ParentClasses: {string.Join(", ", (ParentClasses?.Select(x => x.Name + x.Modifier)) ?? new List<string>())} -- {string.Join(", ", other.ParentClasses?.Select(x => x.Name + x.Modifier) ?? new List<string>())} ** {ComparisonHelpers.AreCollectionEquals(ParentClasses, other.ParentClasses)}");
            //System.Diagnostics.Trace.WriteLine($"Messages: {string.Join(", ", (Messages) ?? new List<string>())} -- {string.Join(", ", other.Messages ?? new List<string>())} ** {ComparisonHelpers.AreCollectionEquals(Messages, other.Messages)}");
            //System.Diagnostics.Trace.WriteLine($"DiagnosticDef: {DiagnosticDef.Equals(other.DiagnosticDef)} ** {DiagnosticDef.Equals(other.DiagnosticDef)}");

            return result;
        }

        public override bool Equals(object? obj)
        {
            return obj is ExceptionClassDefinition other && Equals(other);
        }

        public override int GetHashCode()
        {
            HashCode hash = default;

            hash.Add(Namespace);

            if (ParentClasses != null)
            {
                foreach (var parentDef in ParentClasses)
                {
                    hash.Add(parentDef);
                }
            }

            hash.Add(ClassDef);

            if (Messages != null)
            {
                foreach (var message in Messages)
                {
                    hash.Add(message);
                }
            }

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