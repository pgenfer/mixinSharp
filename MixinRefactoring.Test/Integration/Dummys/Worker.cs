using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    /// <summary>
    /// Mixin class that provides a method as service
    /// </summary>
    public class Worker
    {
        public void Work() { /* a worker does work */}
    }

    public class WorkerWithTool
    {
        public void Work(int toolNumber) { /* a worker works with a tool */}
    }

    /// <summary>
    /// a mixin class that overrides a method
    /// from System.Object.
    /// In that special case, the mixin child
    /// should also delegate the methods (but only
    /// if they come from the Object base type)
    /// </summary>
    public class MixinWithToString
    {
        public override string ToString() => "This is a ToString method";
    }

    public class MixinWithStaticMethod
    {
        /// <summary>
        /// a static method that should not be included
        /// </summary>
        public static void Work() { }
    }
}
