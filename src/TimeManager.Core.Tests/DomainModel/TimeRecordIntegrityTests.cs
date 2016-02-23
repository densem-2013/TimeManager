namespace Infocom.TimeManager.Core.Tests.DomainModel
{
    using System;

    using Infocom.TimeManager.Core.DomainModel;

    using Ninject;

    using NUnit.Framework;

    [TestFixture]
    public class TimeRecordIntegrityTests : IntegrityTestBase<TimeRecord>
    {
        #region Public Methods

        [Test]
        public void Employee_NotNull()
        {
            CheckProperty(t => t.Employee).For.NotNull();
        }

        [Test]
        public void Task_NotNull()
        {
            CheckProperty(t => t.Task).For.NotNull();
        }

        [Test]
        public void Description_Null()
        {
            CheckProperty(t => t.Description).For.Null();
        }

        [Test]
        public void StartDate_MinValue()
        {
            CheckProperty(t => t.StartDate).For.MinimumDateTime(new DateTime(2000, 1, 1));
        }

        [Test]
        public void StartDate_MaxValue()
        {
            CheckProperty(t => t.StartDate).For.MaximumDateTime(new DateTime(2100, 1, 1));
        }


        #endregion

        #region Methods

        protected override TimeRecord InitializeSut()
        {
            return this.MockingKernel.Get<TimeRecord>();
        }

        #endregion
    }
}