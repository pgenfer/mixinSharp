using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    /// <summary>
    /// extension of NUnit test attribute
    /// let's the user enter a description directly 
    /// as attribute argument
    /// </summary>
    public class TestDescriptionAttribute : TestAttribute
    {
        public TestDescriptionAttribute(string description = "")
        {
            Description = description;
        }
    }
}
