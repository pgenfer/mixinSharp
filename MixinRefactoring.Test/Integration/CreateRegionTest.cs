using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace MixinRefactoring.Test
{
    /// <summary>
    /// Test class that contains all tests cases for the "Create Regions" feature.
    /// 
    /// </summary>
    [TestFixture]
    public class CreateRegionTest
    {
        /// <summary>
        /// helper method:
        /// Includes the mixin in the given child class
        /// and checks that a region exists where the methods
        /// are added to
        /// </summary>
        /// <typeparam name="T">Type of child class to read from source file</typeparam>
        private void CheckThatMembersAreInRegion<T>()
        {
            // arrange
            var sourceCode = new SourceCode(Files.Person, Files.Name);
            var personClass = sourceCode.Class(typeof(T).Name);
            var mixinReference = 
                new MixinReferenceFactory(sourceCode.Semantic)
                .Create(personClass.FindMixinReference("_name"));
            var settings = new Settings(createRegions: true);
            var mixinCommand = new CreateMixinFromFieldDeclarationCommand(mixinReference);

            // act
            var newClassDeclaration = mixinCommand.Execute(personClass,sourceCode.Semantic,settings);

            // assert: the method must be between the region
            var isPropertyBetweenRegion = 
                ValidationHelpers.IsPropertyBetweenRegion(newClassDeclaration, "mixin _name", "Name");
            Assert.IsTrue(isPropertyBetweenRegion);
        }

        [Test]
        public void NoRegionInClass_CreateRegion_RegionCreated()
        {
            CheckThatMembersAreInRegion<PersonWithoutRegion>();
        }        

        [Test]
        public void RegionInClass_AddMembers_MembersAddedToExistingRegion()
        {
            CheckThatMembersAreInRegion<PersonWithRegion>();
        }

        [Test]
        public void SeveralRegionsInClass_CreateRegion_RegionCreated()
        {
            CheckThatMembersAreInRegion<PersonWithNestedRegionsButWithoutMixinRegion>();
        }

        [Test]
        public void SeveralRegionsInClass_AddMembers_MembersAddedToExistingRegion()
        {
            CheckThatMembersAreInRegion<PersonWithNestedRegions>();
        }

        [Test]
        public void EmptyRegionExists_AddMembers_MembersAddedToExistingRegion()
        {
            CheckThatMembersAreInRegion<PersonWithEmptyRegion>();
        }

        /// <summary>
        /// this test should verify that bug
        /// https://github.com/pgenfer/mixinSharp/issues/9
        /// is fixed (regions of two or more mixins are nested
        /// instead of sequentially)
        /// </summary>
        [Test]
        public void ChildWithTwoMixins_AddMembers_RegionAddedAfterLastRegion()
        {
            // arrange
            var sourceCode = new SourceCode(Files.Person, Files.Name,Files.Worker);
            var personClass = sourceCode.Class(nameof(PersonWithTwoMixins));
            var workerMixin = 
                new MixinReferenceFactory(sourceCode.Semantic)
                .Create(personClass.FindMixinReference("_worker"));
            var settings = new Settings(createRegions: true);
            // act: add the second mixin
            var mixinCommand = new CreateMixinFromFieldDeclarationCommand(workerMixin);
            var newClassDeclaration = mixinCommand.Execute(personClass, sourceCode.Semantic,settings);

            // get the region directive for the second mixin and ensure
            // that it is AFTER the first endregion directive
            var beginRegion = newClassDeclaration.FindRegionByText("mixin _worker");
            var endRegion = newClassDeclaration.FindEndRegion("mixin _name");

            Assert.IsTrue(beginRegion.SpanStart > endRegion.SpanStart);
        }
    }
}
