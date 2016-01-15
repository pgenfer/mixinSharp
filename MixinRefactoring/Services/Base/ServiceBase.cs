using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// Base functionality for all services (mixins and implementations in mixin children)
    /// </summary>
    public abstract class ServiceBase
    {
        protected ServiceBase(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override string ToString() => Name;
    }
}


