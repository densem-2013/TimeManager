namespace Infocom.TimeManager.Core.Tests.DomainModel.Repositories
{
    using System;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.Core.DomainModel.Repositories;

    using Ninject;

    using NUnit.Framework;

    [TestFixture]
    public class TimeRecordRepositoryTests : RepositoryBaseTests<TimeRecordRepository, TimeRecord>
    {
        #region Public Methods

        [Test, Ignore()]
        public override void Add_ItemWithNoRef_ShouldBeAdded()
        {
            base.Add_ItemWithNoRef_ShouldBeAdded();
        }

        [Test, Ignore()]
        public override void Add_ItemWithRef_ShouldBeAdded()
        {
            base.Add_ItemWithRef_ShouldBeAdded();
        }
        
        #endregion

        #region Methods

        protected override TimeRecord CreateSimpleItem()
        {
            return new TimeRecord(
                this.MockingKernel.Get<Employee>(), 
                this.MockingKernel.Get<Task>(), 
                new DateTime(2011, 1, 1), 
                TimeSpan.FromHours(1));
        }

        protected override TimeRecord CreateComplexItem()
        {
            return new TimeRecord(
                this.MockingKernel.Get<Employee>(), 
                this.MockingKernel.Get<Task>(), 
                new DateTime(2011, 1, 1), 
                TimeSpan.FromHours(1));
        }

        protected override object GetLinkedComplexObject(TimeRecord source)
        {
            return source.Employee;
        }

        protected override TimeRecordRepository InitializeSut()
        {
            return new TimeRecordRepository();
        }

        #endregion
    }
}