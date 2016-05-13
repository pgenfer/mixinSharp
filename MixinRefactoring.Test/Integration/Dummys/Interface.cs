using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    /// <summary>
    /// some interfaces used for a mixin
    /// </summary>
    public interface IFirstInterface
    {
    }

    public interface ISecondInterface
    { }

    public interface IThirdInterface : IFirstInterface
    { }

    /// <summary>
    /// mixin has only one interface
    /// </summary>
    public class MixinWithOneInterface : IFirstInterface
    { }

    /// <summary>
    /// mixin has two interfaces
    /// </summary>
    public  class MixinWithTwoInterfaces : IFirstInterface,ISecondInterface
    { }

    /// <summary>
    /// mixin has three interfaces, one is derived from one
    /// </summary>
    public class MixinWithManyInterfaces : IThirdInterface,ISecondInterface
    { }

    /// <summary>
    /// mixin has two interfaces, the second one is already included in the first one
    /// </summary>
    public class MixinWithRedundantInterfaces : IThirdInterface,IFirstInterface
    { }

    /// <summary>
    /// a child class that does not implement any interfaces
    /// </summary>
    public class ChildWithoutInterface
    {
        private IFirstInterface _firstInterface;
    }

    /// <summary>
    /// special case here: The interface derives
    /// already from other interfaces,
    /// but the child should only use the interface itself
    /// result should be:
    /// ChildWithInterfaceHierarchy : IThirdInterface
    /// </summary>
    public class ChildWithInterfaceHierarchy
    {
        private IThirdInterface _mixin;
    }

    /// <summary>
    /// the mixin is an implementation, but
    /// the child should only implement 
    /// the interfaces of the mixin class,
    /// not the mixin class itself (because it is an implementation)
    /// </summary>
    public class ChildWithMixinImplementation
    {
        private MixinWithManyInterfaces _mixin;
    }

    /// <summary>
    /// child class that already implements an interface
    /// </summary>
    public class ChildWithInterface : IFirstInterface
    {
        private IFirstInterface _firstInterface;
    }
}
