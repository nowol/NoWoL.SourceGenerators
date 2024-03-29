﻿// <auto-generated/>
#pragma warning disable
#nullable enable

namespace Test
{
    [System.Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("ExceptionClassGenerator", "1.0.0.0")]
    public partial class TestClass : System.Exception
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
        public static TestClass Create(System.Exception innerException = null)
        {
            return new TestClass($"The message", innerException);
        }

        /// <summary>
        /// Helper method to create the exception's message
        /// </summary>
        /// <returns>An string with the message of the <see cref="TestClass"/> exception</returns>
        public static string CreateMessage()
        {
            return $"The message";
        }
    }
}