using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace NoWoL.SourceGenerators
{
    internal class AlwaysInitializedPropertyGeneratorDescriptors
    {
        public static readonly DiagnosticDescriptor FieldMustBeInClass = GetFieldMustBeInClassDescriptor();
        public static readonly DiagnosticDescriptor FieldMustBePrivate = GetFieldMustBePrivateDescriptor();
        public static readonly DiagnosticDescriptor FieldCannotBeStatic = GetFieldCannotBeStaticDescriptor();
        public static readonly DiagnosticDescriptor FieldCannotBeReadOnly = GetFieldCannotBeReadOnlyDescriptor();
        public static readonly DiagnosticDescriptor FieldMustBeInNamespace = GetFieldMustBeInNamespaceDescriptor();
        public static readonly DiagnosticDescriptor MustBeInParentPartialClass = GetMustBeInParentPartialClassDescriptor();
        public static readonly DiagnosticDescriptor FieldTypeMustBeAReferenceType = GetFieldTypeMustBeAReferenceTypeDescriptor();
        public static readonly DiagnosticDescriptor FieldTypeMustHaveParameterlessConstructor = GetFieldTypeMustHaveParameterlessConstructorDescriptor();
        public static readonly DiagnosticDescriptor FieldTypeMustExist = GetFieldTypeMustExistDescriptor();
        public static readonly DiagnosticDescriptor OnlyOneFieldCanBeDeclared = GetOnlyOneFieldCanBeDeclaredDescriptor();
        
        private const string AlwaysInitializedPropertyGeneratorCategory = "Always Initialized Property Generator";

        private static DiagnosticDescriptor GetFieldMustBeInClassDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(AlwaysInitializedPropertyGeneratorErrorCode.FieldMustBeInClass),
                                            "Field must be part of a class",
                                            "The field '{0}' must be part of a class to use [AlwaysInitializedPropertyGenerator]",
                                            AlwaysInitializedPropertyGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }

        private static DiagnosticDescriptor GetFieldMustBePrivateDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(AlwaysInitializedPropertyGeneratorErrorCode.FieldMustBePrivate),
                                            "Field must be private",
                                            "The field '{0}' must have a private access modifier to use [AlwaysInitializedPropertyGenerator]",
                                            AlwaysInitializedPropertyGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }

        private static DiagnosticDescriptor GetFieldCannotBeStaticDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(AlwaysInitializedPropertyGeneratorErrorCode.FieldCannotBeStatic),
                                            "Field cannot be static",
                                            "The field '{0}' cannot be static to use [AlwaysInitializedPropertyGenerator]",
                                            AlwaysInitializedPropertyGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }

        private static DiagnosticDescriptor GetFieldCannotBeReadOnlyDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(AlwaysInitializedPropertyGeneratorErrorCode.FieldCannotBeReadOnly),
                                            "Field cannot be readonly",
                                            "The field '{0}' cannot be readonly to use [AlwaysInitializedPropertyGenerator]",
                                            AlwaysInitializedPropertyGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }

        private static DiagnosticDescriptor GetFieldMustBeInNamespaceDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(AlwaysInitializedPropertyGeneratorErrorCode.ClassMustBeInNamespace),
                                            "Class must be in namespace",
                                            "The field '{0}' must be in a class contained in a namespace",
                                            AlwaysInitializedPropertyGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }

        private static DiagnosticDescriptor GetMustBeInParentPartialClassDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(AlwaysInitializedPropertyGeneratorErrorCode.MustBeInParentPartialClass),
                                            "Parent classes must be partial",
                                            "The parent classes of field '{0}' must all be partial",
                                            AlwaysInitializedPropertyGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }

        private static DiagnosticDescriptor GetFieldTypeMustBeAReferenceTypeDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(AlwaysInitializedPropertyGeneratorErrorCode.FieldTypeMustBeAReferenceType),
                                            "Field type must be a reference type",
                                            "The type of field '{0}' must be a reference type",
                                            AlwaysInitializedPropertyGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }

        private static DiagnosticDescriptor GetFieldTypeMustHaveParameterlessConstructorDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(AlwaysInitializedPropertyGeneratorErrorCode.FieldTypeMustHaveParameterlessConstructor),
                                            "Field type must have a parameterless constructor",
                                            "The type of field '{0}' must have a parameterless constructor",
                                            AlwaysInitializedPropertyGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }

        private static DiagnosticDescriptor GetFieldTypeMustExistDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(AlwaysInitializedPropertyGeneratorErrorCode.FieldTypeMustExist),
                                            "Field type must exist",
                                            "The type of field '{0}' must exist",
                                            AlwaysInitializedPropertyGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }

        private static DiagnosticDescriptor GetOnlyOneFieldCanBeDeclaredDescriptor()
        {
            return new DiagnosticDescriptor(GenerationHelpers.ConvertErrorCode(AlwaysInitializedPropertyGeneratorErrorCode.OnlyOneFieldCanBeDeclared),
                                            "Single field declaration",
                                            "Declaration for field '{0}' cannot declare more than one variable",
                                            AlwaysInitializedPropertyGeneratorCategory,
                                            DiagnosticSeverity.Error,
                                            true);
        }
    }
}
