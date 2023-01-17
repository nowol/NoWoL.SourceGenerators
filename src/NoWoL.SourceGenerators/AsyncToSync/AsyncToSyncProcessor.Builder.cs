using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators
{
    internal partial class AsyncToSyncProcessor // Builder
    {
        public void AppendWithTrivia(SyntaxNode? n)
        {
            if (n != null)
            {
                _analysisContext.Builder.AddRaw(n.ToFullString());
            }
        }

        public void AddLeadingTrivia(SyntaxNode n)
        {
            if (n!.HasLeadingTrivia)
            {
                var triviaList = n.GetLeadingTrivia();

                foreach (var trivia in triviaList)
                {
                    var isProcessed = false;
                    if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                        && trivia.HasStructure)
                    {
                        var structure = trivia.GetStructure();

                        if (structure != null)
                        {
                            ProcessTriviaStructure(structure);
                            isProcessed = true;
                        }
                    }
                    //else if (trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                    //{ }

                    if (!isProcessed)
                    {
                        _analysisContext.Builder.AddRaw(trivia.ToFullString());
                    }
                }
            }
        }

        private void ProcessTriviaStructure(SyntaxNode structure)
        {
            foreach (var childNodesAndToken in structure.ChildNodesAndTokens())
            {
                if (childNodesAndToken.IsToken)
                {
                    _analysisContext.Builder.AddRaw(childNodesAndToken.ToFullString());
                }
                else
                {
                    var node = childNodesAndToken.AsNode()!;
                    bool ignoreLine = false;

                    if (node is XmlElementSyntax xes)
                    {
                        if (xes.StartTag.Name.ToString() == "param")
                        {
                            if (xes.StartTag.Attributes.FirstOrDefault(x => x is XmlNameAttributeSyntax) is XmlNameAttributeSyntax nameAttr)
                            {
                                if (nameAttr.Identifier.Identifier.ValueText == "cancellationToken")
                                {
                                    ignoreLine = true;
                                }
                                else if (nameAttr.Identifier.Identifier.ValueText.EndsWith("Async") == true)
                                {
                                    var newNameAttr = nameAttr.WithIdentifier(AsyncToSyncProcessor.RemoveAsyncFrom(nameAttr.Identifier));
                                    var newStartTag = xes.StartTag.WithAttributes(xes.StartTag.Attributes.Replace(nameAttr,
                                                                                                                  newNameAttr));
                                    var newXes = xes.WithStartTag(newStartTag);

                                    AppendWithTrivia(newXes);
                                    ignoreLine = true;
                                }
                            }
                        }
                    }

                    if (!ignoreLine)
                    {
                        _analysisContext.Builder.AddRaw(node.ToFullString());
                    }
                }
            }
        }

        public void AddTrailingTrivia(SyntaxNode n)
        {
            if (n.HasTrailingTrivia)
            {
                _analysisContext.Builder.AddRaw(n.GetTrailingTrivia().ToFullString());
            }
        }

        public void AddLeadingTrivia(SyntaxToken token)
        {
            if (token.HasLeadingTrivia)
            {
                _analysisContext.Builder.AddRaw(token.LeadingTrivia.ToFullString());
            }
        }

        [ExcludeFromCodeCoverage]
        public void AddTrailingTrivia(SyntaxToken token)
        {
            if (token.HasTrailingTrivia)
            {
                _analysisContext.Builder.AddRaw(token.TrailingTrivia.ToFullString());
            }
        }
    }
}
