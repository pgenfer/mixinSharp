using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    /// <summary>
    /// interface allows storing all implementations 
    /// in the same list
    /// </summary>
    public interface IImplementMemberForwarding
    {
        /// <summary>
        /// only untyped method of the public interface
        /// </summary>
        /// <param name = "member"></param>
        /// <param name="positionOfClassInSourceFile"></param>
        /// <returns></returns>
        MemberDeclarationSyntax ImplementMember(Member member, int positionOfClassInSourceFile);
    }
}