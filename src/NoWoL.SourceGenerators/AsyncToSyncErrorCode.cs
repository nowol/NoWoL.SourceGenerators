using System;
using System.Collections.Generic;
using System.Text;

namespace NoWoL.SourceGenerators
{
    internal enum AsyncToSyncErrorCode
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
