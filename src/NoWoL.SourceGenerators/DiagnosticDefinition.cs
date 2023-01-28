using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace NoWoL.SourceGenerators
{
    public struct DiagnosticDefinition : IEquatable<DiagnosticDefinition>
    {
        public DiagnosticDescriptor? Diagnostic { get; set; }
        public Location? Location { get; set; }
        public string? Parameter { get; set; }
        public bool Initialized { get; set; }

        public bool Equals(DiagnosticDefinition other)
        {
            // uncomment for diagnostics
            //System.Diagnostics.Trace.WriteLine("[nwl2]-----");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Diagnostic {Helpy.AreEquatable(Diagnostic, other.Diagnostic)}");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Diagnostic (null {Diagnostic == null} -- {other.Diagnostic==null}");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Diagnostic.Category {Diagnostic?.Category} -- {other.Diagnostic?.Category}");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Diagnostic.DefaultSeverity {Diagnostic?.DefaultSeverity} -- {other.Diagnostic?.DefaultSeverity}");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Diagnostic.Description {Diagnostic?.Description} -- {other.Diagnostic?.Description}");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Diagnostic.HelpLinkUri {Diagnostic?.HelpLinkUri} -- {other.Diagnostic?.HelpLinkUri}");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Diagnostic.Id {Diagnostic?.Id} -- {other.Diagnostic?.Id}");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Diagnostic.IsEnabledByDefault {Diagnostic?.IsEnabledByDefault} -- {other.Diagnostic?.IsEnabledByDefault}");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Diagnostic.MessageFormat {Diagnostic?.MessageFormat} -- {other.Diagnostic?.MessageFormat}");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Diagnostic.Title {Diagnostic?.Title} -- {other.Diagnostic?.Title}");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Parameter {Parameter} -- {other.Parameter}");
            //System.Diagnostics.Trace.WriteLine($"[nwl2] Location {Location?.SourceSpan.Equals(other.Location?.SourceSpan)}");
            
            if (!ComparisonHelpers.AreEquatable(Diagnostic, other.Diagnostic))
            {
                return false;
            }

            if (Parameter != other.Parameter)
            {
                return false;
            }

            if (Initialized != other.Initialized)
            {
                return false;
            }

            if (ComparisonHelpers.BothNull(Location,
                               other.Location))
            {
                return true;
            }

            return ComparisonHelpers.AreEquatable<TextSpan>(Location?.SourceSpan, other.Location?.SourceSpan);
        }

        public override bool Equals(object? obj)
        {
            return obj is DiagnosticDefinition other && Equals(other);
        }

        public override int GetHashCode()
        {
            HashCode hash = default;

            hash.Add(Diagnostic?.Id);
            hash.Add(Parameter);
            hash.Add(Initialized);
            hash.Add(Location?.SourceSpan);

            return hash.ToHashCode();
        }
    }
}