﻿namespace NoWoL.SourceGenerators
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class ExceptionGeneratorAttribute : System.Attribute
    {
        public string Message { get; private set; }
        public ExceptionGeneratorAttribute()
            : this(System.String.Empty)
        {
        }

        public ExceptionGeneratorAttribute(string message)
        {
            Message = message;
        }
    }
}