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
            var mixinReference = personClass.FindMixinReference("_name");
            var settings = new Settings(createRegions: true);
            var mixinCommand = new MixinCommand(sourceCode.Semantic, mixinReference);

            // act
            var newClassDeclaration = mixinCommand.Execute(settings);

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
    }
}
