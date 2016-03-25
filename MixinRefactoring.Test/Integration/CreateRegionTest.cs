using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using NSubstitute;
using MixinRefactoring;
using Microsoft.CodeAnalysis.Text;

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
            var newClassDeclaration = (ClassDeclarationSyntax)mixinCommand.Execute(settings);

            // assert: the method must be between the region
            // get begin and end region
            var beginRegion = newClassDeclaration.FindRegionByText("mixin _name");
            var endRegion = newClassDeclaration.FindEndRegion("mixin _name");
            // get all nodes between the regions
            var span = new TextSpan(beginRegion.Span.End, endRegion.Span.Start);
            var nodesBetweenRegion = newClassDeclaration.DescendantNodes(span);
            // check that a property declaration for a "Name" property is there
            var nameProperty = nodesBetweenRegion
                .OfType<PropertyDeclarationSyntax>()
                .FirstOrDefault(x => x.Identifier.ToString() == "Name");
            Assert.IsNotNull(nameProperty);
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
