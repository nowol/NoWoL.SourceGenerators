using System;
using System.Collections.Generic;
using System.Text;

namespace NoWoL.SourceGenerators
{
    internal enum ExceptionGeneratorErrorCode
    {
        UnexpectedException,
        MustBeInParentPartialClass,
        MethodClassMustBeInNamespace,
        MethodMustBePartial
    }
}
