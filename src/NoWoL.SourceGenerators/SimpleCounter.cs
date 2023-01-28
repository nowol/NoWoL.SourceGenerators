using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NoWoL.SourceGenerators
{
    [ExcludeFromCodeCoverage]
    internal class SimpleCounter
    {
#pragma warning disable CS0169
        private static int _counter;
#pragma warning restore CS0169

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetValue()
        {
            // uncomment to mark files with a generation count header

            //return $"/* W {Interlocked.Increment(ref _counter)} */ ";
            return null;
        }
    }
}