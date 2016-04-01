using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.Environment;

namespace MixinRefactoring
{
    public static class MemberDeclarationSyntaxExtension
    {
        public static MemberDeclarationSyntax[] AddRegionAround(
            this MemberDeclarationSyntax[] members, string regionName)
        {
            var regionBegin = RegionDirectiveTrivia(true)
                   .WithHashToken(Token(SyntaxKind.HashToken))
                   .WithRegionKeyword(Token(SyntaxKind.RegionKeyword))
                   .WithEndOfDirectiveToken(
                       Token(TriviaList(PreprocessingMessage(regionName)),
                       SyntaxKind.EndOfDirectiveToken,
                       TriviaList()))
                   .WithTrailingTrivia(EndOfLine(NewLine));
            var regionEnd = EndRegionDirectiveTrivia(true);
            // add the region block before the first and after the last member
            var memberCount = members.Length;
            // add new trivias at the beginning of the
            // node, but also use the existing trivias of the node
            var firstMember = members[0];
            members[0] = firstMember
                .WithLeadingTrivia(
                new SyntaxTriviaList()
                    .Add(EndOfLine(NewLine))
                    .Add(Trivia(regionBegin))
                    .AddRange(firstMember.GetLeadingTrivia()));
            // add region after the trivias that already exist
            var lastMember = members[memberCount - 1];
            members[memberCount - 1] = lastMember
                .WithTrailingTrivia(
                new SyntaxTriviaList()
                .AddRange(lastMember.GetTrailingTrivia())
                .Add(Trivia(regionEnd))
                .Add(EndOfLine(NewLine)));
            return members;
        }
    }
}
