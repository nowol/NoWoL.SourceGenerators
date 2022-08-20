using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace NoWoL.SourceGenerators
{
    internal class ExceptionGeneratorDescriptors
    {
        public static readonly DiagnosticDescriptor MethodMustBePartial = GetMethodMustBePartialDescriptor();
        public static readonly DiagnosticDescriptor MethodClassMustBeInNamespace = GetMethodClassMustBeInNamespaceDescriptor();
        public static readonly DiagnosticDescriptor MustBeInParentPartialClass = GetMustBeInParentPartialClassDescriptor();
        private const string ExceptionGeneratorCategory = "Exception generator";

        private static DiagnosticDescriptor GetMethodMustBePartialDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(ExceptionGeneratorErrorCode.MethodMustBePartial),
                                            "Class must be partial",
                                            "The class '{0}' must be partial to use [ExceptionGenerator]",
                                            ExceptionGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }

        private static DiagnosticDescriptor GetMethodClassMustBeInNamespaceDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(ExceptionGeneratorErrorCode.MethodClassMustBeInNamespace),
                                            "Class must be in namespace",
                                            "The class '{0}' must be contained in a namespace",
                                            ExceptionGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }

        private static DiagnosticDescriptor GetMustBeInParentPartialClassDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(ExceptionGeneratorErrorCode.MustBeInParentPartialClass),
                                            "Class must be in namespace",
                                            "The parent classes of class '{0}' must also be partial",
                                            ExceptionGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }
    }
}
