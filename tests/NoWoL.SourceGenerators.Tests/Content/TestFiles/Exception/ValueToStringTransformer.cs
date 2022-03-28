namespace NoWoL.SourceGenerators.Tests
{
    public static class ValueToStringTransformer
    {
        public static string ConvertToCsv(System.Collections.Generic.IEnumerable<int> numbers)
        {
            if (numbers == null)
            {
                return "";
            }

            return System.String.Join(", ",
                                      numbers);
        }
    }
}