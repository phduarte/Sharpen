﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sharpen.Engine.Analysis;
using Sharpen.Engine.SharpenSuggestions.CSharp80.SwitchExpressions.Suggestions;

namespace Sharpen.Engine.SharpenSuggestions.CSharp80.SwitchExpressions.Analyzers
{
    // TODO-IG: Done quickly for the cool demo at the Weblica conference. Take a good look, implement TODOs, and refactor.
    internal sealed class ReplaceSwitchStatementWithSwitchExpressionAnalyzer : ISingleSyntaxTreeAnalyzer
    {
        private ReplaceSwitchStatementWithSwitchExpressionAnalyzer() { }

        public static readonly ReplaceSwitchStatementWithSwitchExpressionAnalyzer Instance = new ReplaceSwitchStatementWithSwitchExpressionAnalyzer();

        enum SwitchStatementSectionsCategory
        {
            None,
            AllSwitchSectionsAreAssignmentsToTheSameIdentifier,
            AllSwitchSectionsAreReturnStatements
        }

        public IEnumerable<AnalysisResult> Analyze(SyntaxTree syntaxTree, SemanticModel semanticModel, SingleSyntaxTreeAnalysisContext analysisContext)
        {
            // TODO-IG: We can have more than one case section pointing to the same statements ;-)
            //          E.g. case 1: case 2: case 3: ...
            //          Implement also this case properly.
            //          One possibility could be, depending e.g. on the number of the cases pointing to
            //          the same statements to offer a "Consider" suggestion.

            return syntaxTree.GetRoot()
                .DescendantNodes()
                .OfType<SwitchStatementSyntax>()
                .Select(GetSwitchStatementPotentialReplaceabilityInfo)
                .Where(replaceabilityInfo => replaceabilityInfo.switchStatement != null)
                .Select(replaceabilityInfo => new AnalysisResult
                (
                    GetSuggestion(replaceabilityInfo.category, replaceabilityInfo.isSurelyExhaustive),
                    analysisContext,
                    syntaxTree.FilePath,
                    replaceabilityInfo.switchStatement.SwitchKeyword,
                    replaceabilityInfo.switchStatement
                ));

            ISharpenSuggestion GetSuggestion(SwitchStatementSectionsCategory category, bool isSurelyExhaustive)
            {
                // TODO-IG: It will be so nice to switch to switch statement one day we move Sharpen to C# 8.0 :-)
                if (category == SwitchStatementSectionsCategory.AllSwitchSectionsAreAssignmentsToTheSameIdentifier)
                {
                    return isSurelyExhaustive
                        ? (ISharpenSuggestion)ReplaceSwitchStatementContainingOnlyAssignmentsWithSwitchExpression.Instance
                        : ConsiderReplacingSwitchStatementContainingOnlyAssignmentsWithSwitchExpression.Instance;
                }
                else
                {
                    return isSurelyExhaustive
                        ? (ISharpenSuggestion)ReplaceSwitchStatementContainingOnlyReturnsWithSwitchExpression.Instance
                        : ConsiderReplacingSwitchStatementContainingOnlyReturnsWithSwitchExpression.Instance;
                }
            }

            (SwitchStatementSyntax switchStatement, SwitchStatementSectionsCategory category, bool isSurelyExhaustive) GetSwitchStatementPotentialReplaceabilityInfo(SwitchStatementSyntax switchStatement)
            {
                // We have to have at least one switch section (case or default).
                if (switchStatement.Sections.Count <= 0) return (null, SwitchStatementSectionsCategory.None, false);

                // If we have the default section it is surely exhaustive.
                // Otherwise we cannot be sure. We will of course not do any
                // proper check that the compiler does.
                bool isSurelyExhaustive = switchStatement.Sections.Any(switchSection =>
                    switchSection.Labels.Any(label => label.IsKind(SyntaxKind.DefaultSwitchLabel)));

                if (AllSwitchSectionsAreAssignmentsToTheSameIdentifier(switchStatement.Sections))
                    return (switchStatement, SwitchStatementSectionsCategory.AllSwitchSectionsAreAssignmentsToTheSameIdentifier, isSurelyExhaustive);

                if (AllSwitchSectionsAreReturnStatements(switchStatement.Sections))
                    return (switchStatement, SwitchStatementSectionsCategory.AllSwitchSectionsAreReturnStatements, isSurelyExhaustive);

                return (null, SwitchStatementSectionsCategory.None, false);

                bool AllSwitchSectionsAreAssignmentsToTheSameIdentifier(SyntaxList<SwitchSectionSyntax> switchSections)
                {
                    string previousIdentifier = null;
                    foreach (var switchSection in switchSections)
                    {
                        // TODO-IG: Valid case is also throwing an exception instead of assigning to an identifier.			
                        // We expect exactly two statements, the assignment and the break;
                        if (switchSection.Statements.Count != 2) return false;
                        if (!switchSection.Statements[1].IsKind(SyntaxKind.BreakStatement)) return false;

                        if (!(switchSection.Statements[0] is ExpressionStatementSyntax expression)) return false;

                        if (!(expression.Expression is AssignmentExpressionSyntax assignment)) return false;

                        // TODO-IG: Compare by symbol and not by name/text because we can have this._field, or something.Something, or a static access or similar.
                        var currentIdentifier = assignment.Left?.ToString();
                        if (previousIdentifier != null && previousIdentifier != currentIdentifier) return false;

                        previousIdentifier = currentIdentifier;
                    }

                    return true;
                }

                bool AllSwitchSectionsAreReturnStatements(SyntaxList<SwitchSectionSyntax> switchSections)
                {
                    foreach (var switchSection in switchSections)
                    {
                        // TODO-IG: Valid case is also throwing an exception instead of assigning to an identifier.			
                        // We expect exactly one statement, the return statement;
                        if (switchSection.Statements.Count != 1) return false;

                        if (!(switchSection.Statements[0] is ReturnStatementSyntax @return)) return false;

                        if (@return.Expression == null) return false;
                    }

                    return true;
                }
            }
        }
    }
}