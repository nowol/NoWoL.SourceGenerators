using System;
using System.Collections.Generic;
using System.Linq;

namespace NoWoL.SourceGenerators
{
    public static class ComparisonHelpers
    {
        public static bool AreEquatable<T>(IEquatable<T>? item, IEquatable<T>? other)
        {
            if (item == null
                && other == null)
            {
                return true;
            }

            if (item == null)
            {
                return false;
            }

            return item.Equals(other);
        }

        public static bool BothNull<T>(T? item, T? other)
        {
            var isNull = item == null;
            var isOtherNull = other == null;

            if (isNull == isOtherNull && isNull) // both null
            {
                return true;
            }

            return false;
        }

        public static bool AreCollectionEquals<T>(IList<T>? col, IList<T>? colOther)
        {
            var isNull = col == null;
            var isOtherNull = colOther == null;

            if (!isNull
                && !isOtherNull)
            {
                return col!.SequenceEqual(colOther!);
            }

            return isNull == isOtherNull;
        }
    }
}