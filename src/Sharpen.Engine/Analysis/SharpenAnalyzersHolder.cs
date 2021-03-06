﻿using Sharpen.Engine.SharpenSuggestions.CSharp30.ImplicitlyTypedLocalVariables;
using Sharpen.Engine.SharpenSuggestions.CSharp50.AsyncAwait;
using Sharpen.Engine.SharpenSuggestions.CSharp60.ExpressionBodiedMembers;
using Sharpen.Engine.SharpenSuggestions.CSharp60.NameofExpressions;
using Sharpen.Engine.SharpenSuggestions.CSharp70.ExpressionBodiedMembers;
using Sharpen.Engine.SharpenSuggestions.CSharp70.OutVariables;
using Sharpen.Engine.SharpenSuggestions.CSharp71.DefaultExpressions;
using Sharpen.Engine.SharpenSuggestions.CSharp80.AsyncStreams.Analyzers;
using Sharpen.Engine.SharpenSuggestions.CSharp80.NullableReferenceTypes.Analyzers;
using Sharpen.Engine.SharpenSuggestions.CSharp80.SwitchExpressions.Analyzers;
using Sharpen.Engine.SharpenSuggestions.CSharp80.UsingDeclarations.Analyzers;

namespace Sharpen.Engine.Analysis
{
    // There is no need to complicate here. We do not need any complex collector so far.
    // No need for MEF based instance searches or similar. New analyzers and suggestions are
    // added rather rarely and at the moment it is very easy to keep this list manually up-to-date.
    internal static class SharpenAnalyzersHolder
    {
        public static readonly ISingleSyntaxTreeAnalyzer[] Analyzers =
        {
            // C# 3.0
            UseVarKeywordInVariableDeclarationWithObjectCreation.Instance,

            // C# 5.0.
            ConsiderAwaitingEquivalentAsynchronousMethodAndMakingTheCallerAsynchronous.Instance,
            AwaitEquivalentAsynchronousMethod.Instance,
            AwaitTaskDelayInsteadOfCallingThreadSleep.Instance,
            AwaitTaskInsteadOfCallingTaskWait.Instance,
            AwaitTaskInsteadOfCallingTaskResult.Instance,
            AwaitTaskWhenAllInsteadOfCallingTaskWaitAll.Instance,
            AwaitTaskWhenAnyInsteadOfCallingTaskWaitAny.Instance,

            // C# 6.0.
            UseExpressionBodyForGetOnlyProperties.Instance,
            UseExpressionBodyForGetOnlyIndexers.Instance,

            UseNameofExpressionForThrowingArgumentExceptions.Instance,
            UseNameofExpressionInDependencyPropertyDeclarations.Instance,

            // C# 7.0.
            UseExpressionBodyForConstructors.Instance,
            UseExpressionBodyForDestructors.Instance,
            UseExpressionBodyForGetAccessorsInProperties.Instance,
            UseExpressionBodyForGetAccessorsInIndexers.Instance,
            UseExpressionBodyForSetAccessorsInProperties.Instance,
            UseExpressionBodyForSetAccessorsInIndexers.Instance,
            UseExpressionBodyForLocalFunctions.Instance,

            UseOutVariablesInMethodInvocations.Instance,
            UseOutVariablesInObjectCreations.Instance,
            DiscardOutVariablesInMethodInvocations.Instance,
            DiscardOutVariablesInObjectCreations.Instance,

            // C# 7.1.
            UseDefaultExpressionInReturnStatements.Instance,
            UseDefaultExpressionInOptionalMethodParameters.Instance,
            UseDefaultExpressionInOptionalConstructorParameters.Instance,

            // C# 8.0
            EnableNullableContextAndDeclareReferenceIdentifierAsNullableAnalyzer.Instance,
            ConsiderAwaitingEquivalentAsynchronousMethodAndYieldingIAsyncEnumerableAnalyzer.Instance,
            ReplaceUsingStatementWithUsingDeclarationAnalyzer.Instance,
            ReplaceSwitchStatementWithSwitchExpressionAnalyzer.Instance
        };
    }
}