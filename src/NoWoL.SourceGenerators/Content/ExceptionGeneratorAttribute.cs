namespace NoWoL.SourceGenerators
{
    /// <summary>
    /// Use this attribute to generate the boiler plate for an exception
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("NoWoL.SourceGenerators", null)]
    internal class ExceptionGeneratorAttribute : System.Attribute
    {
        /// <summary>
        /// Message of the exception
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Create an instance of <see cref="ExceptionGeneratorAttribute"/>
        /// </summary>
        public ExceptionGeneratorAttribute()
            : this(System.String.Empty)
        {
        }

        /// <summary>
        /// Create an instance of <see cref="ExceptionGeneratorAttribute"/> with a message
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public ExceptionGeneratorAttribute(string message)
        {
            Message = message;
        }
    }
}