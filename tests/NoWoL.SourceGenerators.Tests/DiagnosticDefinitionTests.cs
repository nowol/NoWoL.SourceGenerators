using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{
    public class DiagnosticDefinitionTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void BlanksAreEquals()
        {
            var def1 = new DiagnosticDefinition();
            var def2 = new DiagnosticDefinition();

            Assert.Equal(def1,
                         def2);

            Assert.Equal(def1.GetHashCode(),
                         def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsObjectsReturnsTrue()
        {
            var def1 = new DiagnosticDefinition();
            var def2 = new DiagnosticDefinition();

            Assert.True(def1.Equals((object?)def2));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsObjectsReturnsFalseForDiffType()
        {
            var def1 = new DiagnosticDefinition();
            var def2 = new object();

            Assert.False(def1.Equals((object?)def2));
        }

        private DiagnosticDefinition Create()
        {
            var def = new DiagnosticDefinition
                      {
                           Parameter = "param",
                           Initialized = true,
                           Location = Location.Create("File", TextSpan.FromBounds(0, 64), new LinePositionSpan(LinePosition.Zero, new LinePosition(3, 5))),
                           Diagnostic = new DiagnosticDescriptor("id", "title", "msgformat", "category", DiagnosticSeverity.Error, true)
                      };

            return def;
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsWithEveryPropertiesSame()
        {
            var def1 = Create();
            var def2 = Create();

            Assert.Equal(def1,
                         def2);

            Assert.Equal(def1.GetHashCode(),
                         def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsWithNullLocation()
        {
            var def1 = Create();
            def1.Location = null;
            var def2 = Create();
            def2.Location = null;

            Assert.Equal(def1,
                         def2);

            Assert.Equal(def1.GetHashCode(),
                         def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsWithNullParameter()
        {
            var def1 = Create();
            def1.Parameter = null;
            var def2 = Create();
            def2.Parameter = null;

            Assert.Equal(def1,
                         def2);

            Assert.Equal(def1.GetHashCode(),
                         def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsWithNullDiagnostic()
        {
            var def1 = Create();
            def1.Diagnostic = null;
            var def2 = Create();
            def2.Diagnostic = null;

            Assert.Equal(def1,
                         def2);

            Assert.Equal(def1.GetHashCode(),
                         def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DifferentBecauseOfLocation()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Location = Location.Create("File", TextSpan.FromBounds(11, 22), new LinePositionSpan(LinePosition.Zero, new LinePosition(3, 5)));

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DifferentBecauseOfLocationOneNull()
        {
            var def1 = Create();
            def1.Location = null;
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DifferentBecauseOfLocationOtherOneNull()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Location = null;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DifferentBecauseOfParameter()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Parameter = "another name";

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DifferentBecauseOfParameterOneNull()
        {
            var def1 = Create();
            def1.Parameter = null;
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DifferentBecauseOfParameterOtherOneNull()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Parameter = null;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DifferentBecauseOfInitialized()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Initialized = false;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DifferentBecauseOfDiagnosticOneNull()
        {
            var def1 = Create();
            def1.Diagnostic = null;
            var def2 = Create();

            def2.Initialized = false;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DifferentBecauseOfDiagnosticOtherOneNull()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Diagnostic = null;

            def2.Initialized = false;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DifferentBecauseOfDiagnostic()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Diagnostic = new DiagnosticDescriptor("another id", "title", "msgformat", "category", DiagnosticSeverity.Error, true);

            def2.Initialized = false;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }
    }
}
