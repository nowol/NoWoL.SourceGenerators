namespace Test
{
    // This is generated code
    [System.Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("ExceptionClassGenerator", "1.0.0.0")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class TestClass : System.Exception
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Creates an instance of the <see cref="TestClass"/> class.
        /// </summary>
        public TestClass()
        {}

        /// <summary>
        /// Creates an instance of the <see cref="TestClass"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public TestClass(string message)
            : base(message)
        {}

        /// <summary>
        /// Creates an instance of the <see cref="TestClass"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        /// <param name="innerException">Optional inner exception</param>
        public TestClass(string message, System.Exception innerException)
            : base(message, innerException)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClass"/> class.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected TestClass(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Helper method to create the exception
        /// </summary>
        /// <param name="innerException">Optional inner exception</param>
        /// <returns>An instance of the <see cref="TestClass"/> exception</returns>
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public static TestClass Create(int nb, string str, int[] numbers, System.Exception innerException = null)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return new TestClass($"This is a number {nb} and this a string {str} and a transformed param {NoWoL.SourceGenerators.Tests.ValueToStringTransformer.ConvertToCsv(numbers)}", innerException);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        /// <summary>
        /// Helper method to create the exception's message
        /// </summary>
        /// <returns>An string with the message of the <see cref="TestClass"/> exception</returns>
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public static string CreateMessage(int nb, string str, int[] numbers)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return new $"This is a number {nb} and this a string {str} and a transformed param {NoWoL.SourceGenerators.Tests.ValueToStringTransformer.ConvertToCsv(numbers)}";
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}