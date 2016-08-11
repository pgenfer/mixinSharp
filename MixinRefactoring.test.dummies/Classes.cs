using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    /// <summary>
    /// mixin with internal accessor.
    /// When included, the setter should not be visible
    /// </summary>
    public class MixinWithInternalSetter
    {
        public string Property { get; internal set; }
    }

    /// <summary>
    /// mixin with internal getter. 
    /// When included, the getter should not be visible
    /// </summary>
    public class MixinWithInternalGetter
    {
        public string Property { internal get; set; }
    }

    /// <summary>
    /// this is just a marker class used to load the assembly
    /// where this class is placed in
    /// </summary>
    public class ExternalClass
    {
        
    }
}
