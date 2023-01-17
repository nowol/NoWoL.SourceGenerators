namespace NoWoL.SourceGenerators
{
    /// <summary>
    /// Use this attribute to generate the boiler plate for a property that is always initialized
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("NoWoL.SourceGenerators", null)]
    public class AlwaysInitializedPropertyAttribute : System.Attribute
    {
        /// <summary>
        /// Create an instance of <see cref="AlwaysInitializedPropertyAttribute"/>
        /// </summary>
        public AlwaysInitializedPropertyAttribute()
        {}
    }
}