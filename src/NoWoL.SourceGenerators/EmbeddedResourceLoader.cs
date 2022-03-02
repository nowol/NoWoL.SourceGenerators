using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NoWoL.SourceGenerators
{
    internal static class EmbeddedResourceLoader
    {
        public const string ExceptionGeneratorAttributeFileName = "NoWoL.SourceGenerators.Content.ExceptionGeneratorAttribute.cs";

        public static string Get(string resourceName)
        {
            return Get(typeof(EmbeddedResourceLoader).Assembly,
                       resourceName);
        }

        public static string Get(Assembly assembly, string resourceName)
        {
            var name = assembly.GetManifestResourceNames().FirstOrDefault(x => String.Equals(x,
                                                                                             resourceName,
                                                                                             StringComparison.Ordinal));

            if (name == null)
            {
                return null;
            }

            using Stream stream = assembly.GetManifestResourceStream(resourceName);

            using StreamReader reader = new(stream!);

            return reader.ReadToEnd();
        }
    }
}