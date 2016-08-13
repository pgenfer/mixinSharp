using NUnit.Framework;
using MixinRefactoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring.Test
{
    public class EventSyntaxReaderTest : IntegrationTestBase
    {
        [TestDescription("Read event syntax")]
        public void VisitEventDeclaration()
        {
            WithSourceFiles(Files.ChildClass);
            var childClass = CreateClass(nameof(ChildClassWithEvent));

            var eventList = new EventList();
            var eventSyntaxReader = new EventSyntaxReader(eventList, Semantic);

            eventSyntaxReader.Visit(childClass.SourceCode);

            Assert.IsNotEmpty(eventList);
            Assert.AreEqual("SomethingHappened", eventList.Single().Name);
        }
    }
}