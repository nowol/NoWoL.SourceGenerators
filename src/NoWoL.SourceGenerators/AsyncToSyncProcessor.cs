using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static NoWoL.SourceGenerators.AsyncToSyncConverterGenerator;

namespace NoWoL.SourceGenerators
{
    internal partial class AsyncToSyncProcessor
    {
        private readonly IAsyncToSyncAnalysisContext _analysisContext;

        public AsyncToSyncProcessor(Compilation compilation, SourceProductionContext context, SyntaxNode node, IndentedStringBuilder builder)
        {
            var sm = compilation.GetSemanticModel(node.SyntaxTree);
            _analysisContext = new AsyncToSyncAsyncToSyncAnalysisContext(context, compilation, sm, builder);

        }

        public bool ContainsDiagnosticErrors => _analysisContext.ContainsDiagnosticErrors;

        public string GetBuilderContent()
        {
            return _analysisContext.Builder.ToString();
        }
    }

    //internal partial class TransientAsyncToSyncProcessor
    //{
    //    private readonly IAsyncToSyncAnalysisContext _analysisContext;

    //    public TransientAsyncToSyncProcessor(IAsyncToSyncAnalysisContext analysisContext)
    //    {
    //        _analysisContext = analysisContext;
    //    }

    //    public bool ContainsDiagnosticErrors => _analysisContext.ContainsDiagnosticErrors;
    //}
}
