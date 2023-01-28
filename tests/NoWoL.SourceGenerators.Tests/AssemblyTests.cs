using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{
    public class AssemblyTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void EnsureThatOnlyRequiredTypesArePublic()
        {
            var publicTypes = typeof(AlwaysInitializedPropertyGenerator).Assembly.GetTypes().Where(x => x.IsPublic).ToList();

            var whiteList = new List<Type>
                            {
                                typeof(AlwaysInitializedPropertyGenerator),
                                typeof(AsyncToSyncConverterGenerator),
                                typeof(ExceptionClassGenerator)
                            };

            var typesWhichShouldNotBePublic = publicTypes.Except(whiteList).ToList();

            if (typesWhichShouldNotBePublic.Count > 0)
            {
                Assert.Fail($"These types should not be public: {String.Join(", ", typesWhichShouldNotBePublic.Select(x => x.Name))}");
            }
        }
    }
}
