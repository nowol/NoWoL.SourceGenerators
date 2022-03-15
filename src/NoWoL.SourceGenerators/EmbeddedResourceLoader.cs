using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NoWoL.SourceGenerators
{
    internal static class EmbeddedResourceLoader
    {
        public const string ExceptionGeneratorAttributeFileName = "NoWoL.SourceGenerators.Content.ExceptionGeneratorAttribute.cs";

        public static string? Get(Assembly assembly, string resourceName)
        {
            var name = assembly.GetManifestResourceNames().FirstOrDefault(x => String.Equals(x,
                                                                                             resourceName,
                                                                                             StringComparison.Ordinal));

            if (name == null)
            {
                return null;
            }

            return GetEmbeddedContent(assembly,
                                      resourceName);
        }

        private static string GetEmbeddedContent(Assembly assembly, string resourceName)
        {
            using Stream stream = assembly.GetManifestResourceStream(resourceName)!;

            using StreamReader reader = new(stream!);

            return reader.ReadToEnd();
        }

        public static List<EmbeddedFile> GetFilesFromPartialName(Assembly assembly, string folder, string partialResourceName)
        {
            var files = new List<EmbeddedFile>();

            var dotPath = folder.Replace(@"\", ".").TrimEnd('.') + ".";
            var partialNameWithPath = dotPath + partialResourceName;

            foreach (var resourceName in assembly.GetManifestResourceNames()
                                                 .Where(x => x.StartsWith(partialNameWithPath,
                                                                                     StringComparison.OrdinalIgnoreCase)))
            {
                var content = GetEmbeddedContent(assembly,
                                                 resourceName);

                files.Add(new EmbeddedFile
                          {
                              Content = content,
                              FileName = GetFileName(resourceName)
                          });
            }

            return files;

            static string GetFileName(string resourceName)
            {
                var parts = resourceName.Split('.');

                return parts[parts.Length - 2];
            }
        }
    }

    internal class EmbeddedFile
    {
        public string? FileName { get; set; }

        public string? Content { get; set; }
    }
}