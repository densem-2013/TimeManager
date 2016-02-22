

namespace Infocom.TimeManager.Core.Tests.DomainModel.Repositories
{
    using System;
    using System.Linq;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.Core.DomainModel.Repositories;

    using NHibernate.Exceptions;
    using NHibernate.Validator.Engine;
    using NHibernate.Validator.Exceptions;

    using Ninject;

    using NUnit.Framework;

    using SharpTestsEx;

    using Castle.Core;

    [TestFixture]
    public class TimeSheetRepositoryTest : RepositoryBaseTests<TimeSheetRepository, TimeSheet>
    {
        #region Public Methods

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
            Assert.AreEqual(itemsBeforeAdding, itemsAfterAdding - 1);
        }

        #endregion

        #region Methods
        protected override TimeSheet CreateSimpleItem()
        {
            return this.MockingKernel.Get<TimeSheet>();
        }

        protected override TimeSheet CreateComplexItem()
        {
            var result = this.CreateSimpleItem();
            result.Employee = this.MockingKernel.Get<Employee>();

            return result;
        }

        protected override object GetLinkedComplexObject(TimeSheet source)
        {
            throw new NotImplementedException();
        }

        protected override TimeSheetRepository InitializeSut()
        {
            return new TimeSheetRepository();
        }

        #endregion
    }
}
