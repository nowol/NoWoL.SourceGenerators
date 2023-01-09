using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NoWoL.SourceGenerators
{
    internal interface IAsyncToSyncAnalysisContext
    {
        Compilation Compilation { get; }
        SemanticModel SemanticModel { get; }
        IndentedStringBuilder Builder { get; }
        CancellationToken CancellationToken { get; }
        bool ContainsDiagnosticErrors { get; set; }
        SourceProductionContext SourceProductionContext { get; }

        void AddDiagnostic(AsyncToSyncErrorCode errorCode, string message, Location location);
    }

    internal class AsyncToSyncAsyncToSyncAnalysisContext : IAsyncToSyncAnalysisContext
    {
        internal const string DiagnosticCategory = "Async/Await remover generator";

        public AsyncToSyncAsyncToSyncAnalysisContext(SourceProductionContext sourceProductionContext, Compilation compilation, SemanticModel semanticModel, IndentedStringBuilder builder)
        {
            Compilation = compilation;
            SemanticModel = semanticModel;
            Builder = builder;
            SourceProductionContext = sourceProductionContext;
        }

        public SourceProductionContext SourceProductionContext { get; }

        public Compilation Compilation { get; }

        public SemanticModel SemanticModel { get; }

        public IndentedStringBuilder Builder { get; }

        public CancellationToken CancellationToken => SourceProductionContext.CancellationToken;

        public bool ContainsDiagnosticErrors { get; set; }

        public void AddDiagnostic(AsyncToSyncErrorCode errorCode, string message, Location location)
        {
            ContainsDiagnosticErrors = true;
            SourceProductionContext.ReportDiagnostic(Diagnostic.Create(GenerationHelpers.ConvertErrorCode(errorCode),
                                                                       DiagnosticCategory,
                                                                       message,
                                                                       defaultSeverity: DiagnosticSeverity.Error,
                                                                       severity: DiagnosticSeverity.Error,
                                                                       isEnabledByDefault: true,
                                                                       warningLevel: 0,
                                                                       location: location));
        }
    }

    //internal class TransientAsyncToSyncAnalysisContext : IAsyncToSyncAnalysisContext
    //{
    //    private readonly IAsyncToSyncAnalysisContext _baseContext;
    //    private StringBuilder? _builder = null;

    //    public TransientAsyncToSyncAnalysisContext(IAsyncToSyncAnalysisContext baseContext)
    //    {
    //        _baseContext = baseContext;
    //    }

    //    public Compilation Compilation => _baseContext.Compilation;

    //    public SemanticModel SemanticModel => _baseContext.SemanticModel;

    //    public StringBuilder Builder
    //    {
    //        get { return _builder ??= new StringBuilder(); }
    //    }

    //    public CancellationToken CancellationToken => _baseContext.CancellationToken;

    //    public void AddDiagnostic(AsyncToSyncErrorCode errorCode, string message, Location location)
    //    {
    //        _baseContext.AddDiagnostic(errorCode,
    //                                   message,
    //                                   location);
    //    }

    //    public void ClearBuilder()
    //    {
    //        _builder?.Clear();
    //    }
    //}
}
