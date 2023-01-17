using Microsoft.CodeAnalysis;

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
}
