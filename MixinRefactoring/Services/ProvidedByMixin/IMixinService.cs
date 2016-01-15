using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// Interface for all services provided by mixin classes.
    /// </summary>
    public interface IMixinService
    {
        /// <summary>
        /// used for double dispatching
        /// </summary>
        /// <param name="implementation"></param>
        /// <returns></returns>
        bool IsImplementedBy(PropertyServiceBase implementation);
        /// <summary>
        /// used for double dispatching
        /// </summary>
        /// <param name="implementation"></param>
        /// <returns></returns>
        bool IsImplementedBy(MethodServiceBase implementation);

        /// <summary>
        /// generates a syntax tree for a member declaration
        /// that will call this service on the given mixin
        /// </summary>
        /// <param name="nameOfMixinVariable"></param>
        /// <returns></returns>
        MemberDeclarationSyntax ToMemberDeclaration(string nameOfMixinVariable);

        /// <summary>
        /// used to identify the service
        /// </summary>
        string Name { get; }
    }
}
