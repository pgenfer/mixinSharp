
#pragma warning disable 169

/// <summary>
/// this file contains dummy classes
/// that are used to validate the
/// documentation option of mixinSharp.
/// The documentation code of the mixins
/// should also be copied when the mixins
/// are included in the child.
/// </summary>
namespace MixinRefactoring.Test
{
    public class MixinWithOneProperty
    {
        /// <summary>
        /// this is a property inside the mixin
        /// </summary>
        public string Property { get; set; }
    }

    public class MixinWithOneMethod
    {
        /// <summary>
        /// A method with a parameter
        /// </summary>
        /// <param name="parameter">the parameter is a string</param>
        /// <returns>the method always returns true</returns>
        public bool Predicate(string parameter) { return true; }
    }

    public class MixinWithRemark
    {
        /// <summary>
        /// a simple action method
        /// </summary>
        /// <remarks>
        /// The action has an additional remark
        /// </remarks>
        public void Action() { }
    }

    public class MixinWithSeeAlso
    {
        /// <summary>
        /// method with a string parameter
        /// </summary>
        /// <param name="parameter">Parameter is of type string</param>
        /// <seealso cref="System.String">
        /// There is an additional seealso comment here here
        /// </seealso>
        public void Action(string parameter) { }
    }

    public class MixinWithOneLine
    {
        /// <summary>This summary is in one line</summary>
        public void OneMethod() { }
    }

    /// <summary>
    /// child class with all possible mixins.
    /// Every test case can choose the mixin
    /// it wants to generate
    /// </summary>
    public class Child
    {
        private MixinWithOneProperty _mixinWithProperty;
        private MixinWithOneMethod _mixinWithOneMethod;
        private MixinWithRemark _mixinWithRemark;
        private MixinWithSeeAlso _mixinWithSeeAlso;
        private MixinWithOneLine _mixinWithOneLine;
    }
}
