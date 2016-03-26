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

            members[0] = members[0]
                .WithLeadingTrivia(EndOfLine(NewLine), Trivia(regionBegin));
            members[memberCount - 1] = members[memberCount - 1]
                .WithTrailingTrivia(Trivia(regionEnd), EndOfLine(NewLine));
            return members;
        }
    }
}
