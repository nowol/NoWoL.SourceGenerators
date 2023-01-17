using System;
using System.Collections.Generic;
using System.Text;

namespace NoWoL.SourceGenerators
{
    internal enum AlwaysInitializedPropertyGeneratorErrorCode
    {
        MustBeInParentPartialClass,
        FieldMustBePrivate,
        FieldCannotBeStatic,
        ClassMustBeInNamespace,
        FieldCannotBeReadOnly,
        FieldMustBeInClass,
        FieldTypeMustBeAReferenceType,
        FieldTypeMustHaveParameterlessConstructor,
        FieldTypeMustExist,
        OnlyOneFieldCanBeDeclared,
    }
}
