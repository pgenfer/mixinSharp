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

    /// <summary>
    /// a class with a generic type
    /// parameter. When including
    /// this mixin, the parameter T should
    /// be specified
    /// </summary>
    /// <typeparam name="T">generic parameter of mixin</typeparam>
    public class GenericWorker<T>
    {
        public T Work(T tool) { return default(T); }
    }

    /// <summary>
    /// base class that already provides a method
    /// </summary>
    public class WorkerBase
    {
        public void Work() { }
    }

    /// <summary>
    /// derived worker, when including the mixin,
    /// the base class method should also be added
    /// </summary>
    public class DerivedWorker : WorkerBase
    {
        public void AdditionalWork() { }
    }
}
