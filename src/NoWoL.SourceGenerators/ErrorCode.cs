using System;
using System.Collections.Generic;
using System.Text;

namespace NoWoL.SourceGenerators
{
    internal enum ErrorCode
    {
        AwaitedMethodMustEndWithAsync,
        ReturnedMethodMustEndWithAsync,
        AttributeMustBeAppliedToAClassEndingWithAsync,
        AttributeMustBeAppliedToPartialClass,
        AttributeMustBeAppliedInPartialClassHierarchy,
        MethodMustBeInNameSpace,
        MethodMustReturnTask,
    }
}
