using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{
    public class EmbeddedResourceLoaderTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetShouldReturnEmbeddedContent()
        {
            var resourceName = "NoWoL.SourceGenerators.Tests.Content.TestFiles.ClassModifiersShouldBePreserved.Generated.TestClassPublic_8ac28b09dc2dc04896bf6fc0e1c8b8b0.cs";
            var content = EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoaderTests).Assembly,
                                                     resourceName);
            Assert.NotNull(content);
            Assert.NotEmpty(content);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetShouldReturnNullForUnknownResource()
        {
            var resourceName = "unknown";
            var content = EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoaderTests).Assembly,
                                                     resourceName);
            Assert.Null(content);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetFilesFromPartialNameShouldReturnEmptyListForUnknownSearchPattern()
        {
            var contents = EmbeddedResourceLoader.GetFilesFromPartialName(typeof(EmbeddedResourceLoaderTests).Assembly,
                                                                         "Folder",
                                                                         "Unknown");
            Assert.Empty(contents);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetFilesFromPartialNameShouldReturnTheirContentAndFileName()
        {
            var contents = EmbeddedResourceLoader.GetFilesFromPartialName(typeof(EmbeddedResourceLoaderTests).Assembly,
                                                                         "NoWoL.SourceGenerators.Tests.Content.TestFiles",
                                                                         "TwoExceptionsInDifferentScopeWithSameNameShouldBeGenerated.Generated");
            Assert.NotEmpty(contents);
            Assert.Equal(2,
                         contents.Count);
            var first = contents[0];
            Assert.Equal("TestClass_622e170805df79b45c9e35700714460b",
                         first.FileName);
            Assert.NotNull(first.Content);
            Assert.NotEmpty(first.Content!);

            var second = contents[1];
            Assert.Equal("TestClass_ff77a22886df145d140e4b748d44b619",
                         second.FileName);
            Assert.NotNull(second.Content);
            Assert.NotEmpty(second.Content!);
        }
    }
}
