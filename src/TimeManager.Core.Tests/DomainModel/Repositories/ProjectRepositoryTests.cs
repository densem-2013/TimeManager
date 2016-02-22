namespace Infocom.TimeManager.Core.Tests.DomainModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.Core.DomainModel.Repositories;

    using Ninject;

    using NUnit.Framework;

    [TestFixture, Ignore("todo")]
    public class ProjectRepositoryTests : RepositoryBaseTests<ProjectRepository, Project>
    {
        #region Methods

        [Test]
        public override void TryGetById_ExistedId_TrueAndCorrectObject()
        {
            base.TryGetById_ExistedId_TrueAndCorrectObject();
        }

        [Test]
        public override void Add_ItemWithRef_ShouldBeAdded()
        {
            // arrange
            var itemToAdd = this.CreateComplexItem();
            var itemsBeforeAdding = this.Sut.GetAll().Count();

            // act
            this.Sut.Add(itemToAdd);
            var itemsAfterAdding = this.Sut.GetAll().Count();

            // assert
            Assert.AreEqual(itemsBeforeAdding, itemsAfterAdding - 2);
        }

        protected override Project CreateSimpleItem()
        {
            return this.MockingKernel.Get<Project>();
        }

        protected override Project CreateComplexItem()
        {
            var result = this.CreateSimpleItem();
            //result.Tasks = new List<Task> { this.MockingKernel.Get<Task>() };

            return result;
        }

        protected override object GetLinkedComplexObject(Project source)
        {
            return source.Tasks;
        }

        protected override ProjectRepository InitializeSut()
        {
            return new ProjectRepository();
        }

        #endregion
    }
}