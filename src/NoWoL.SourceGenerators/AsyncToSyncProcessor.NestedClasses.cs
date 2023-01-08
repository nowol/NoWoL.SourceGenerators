using Microsoft.CodeAnalysis;

namespace NoWoL.SourceGenerators
{
    internal partial class AsyncToSyncProcessor // NestedClasses
    {
        internal struct Symbolizer
        {
            private readonly SemanticModel _sm;
            private readonly SyntaxNode _node;
            private SymbolInfo? symbol = null;

            public Symbolizer(SemanticModel sm, SyntaxNode node)
            {
                _sm = sm;
                _node = node;
            }

            public SymbolInfo SymbolInfo
            {
                get
                {
                    return symbol ??= _sm.GetSymbolInfo(_node);
                }
            }
        }
    }
}
