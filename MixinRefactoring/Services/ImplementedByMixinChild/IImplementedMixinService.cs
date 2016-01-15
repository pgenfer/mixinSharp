using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// A service of a mixin class that is already implemented
    /// in a child class. The implementation should only
    /// delegate the call to the service of the mixin
    /// </summary>
    public interface  IImplementedMixinService
    {
        /// <summary>
        /// IsImplementationOf and service.IsImplementedBy are 
        /// an implementation of visitor pattern to enable a double dispatch here
        /// between ImplementedService and Service
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        bool IsImplementationOf(IMixinService service);

        /// <summary>
        /// name used to identify this service
        /// </summary>
        string Name { get; }
    }

    

    
}
