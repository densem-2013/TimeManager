namespace Infocom.TimeManager.Core.Tests
{
    using System;

    using Infocom.TimeManager.Core.DomainModel;

    using Ninject.MockingKernel;
    using Ninject.MockingKernel.Moq;

    using NUnit.Framework;

    using Ninject;

    public abstract class BaseTest<T> : BaseTest
        where T : class
    {
        #region Properties

        protected T Sut { get; private set; }

        #endregion

        #region Public Methods

        public override void SetUp()
        {
            base.SetUp();

            this.Sut = this.InitializeSut();
        }

        #endregion

        #region Methods

        protected abstract T InitializeSut();

        #endregion
    }

    public abstract class BaseTest
    {
        #region Constructors and Destructors

        public BaseTest()
        {
            this.MockingKernel = new MoqMockingKernel();
        }

        #endregion

        #region Properties

        protected MockingKernel MockingKernel { get; private set; }

        #endregion

        #region Public Methods

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
        }

        [TestFixtureTearDown]
        public virtual void FixtureTearDown()
        {
        }

        [SetUp]
        public virtual void SetUp()
        {
            this.MockInit();
        }

        [TearDown]
        public virtual void TearDown()
        {
        }

        #endregion

        #region Methods

        protected virtual void MockInit()
        {
            this.MockingKernel.Rebind<Employee>().ToMethod(
                c =>
                new Employee
                    {
                        FirstName = this.GenerateUniqueString(),
                        LastName = this.GenerateUniqueString(),
                        PatronymicName = this.GenerateUniqueString(),
                        Login = this.GenerateUniqueString()
                    });
            this.MockingKernel.Rebind<Project>().ToMethod(
                c =>
                new Project
                    {
                        Name = this.GenerateUniqueString(),
                        Code = this.GenerateUniqueString(),
                        Description = "Desc",
                        Rate = 100,
                        ProjectType = MockingKernel.Get<ProjectType>()
                    });
            this.MockingKernel.Rebind<TimeRecord>().ToMethod(
                c =>
                new TimeRecord
                    {
                        Description = this.GenerateUniqueString(),
                        Employee = this.MockingKernel.Get<Employee>(),
                        StartDate = new DateTime(2011, 1, 1),
                        SpentTime = TimeSpan.FromHours(1),
                        Task = this.MockingKernel.Get<Task>()
                    });
            this.MockingKernel.Rebind<Task>().ToMethod(
                c => new Task { Name = this.GenerateUniqueString(), Project = MockingKernel.Get<Project>() });
            this.MockingKernel.Rebind<TimeSheet>().ToMethod(
                c =>
                new TimeSheet
                {
                    Employee = this.MockingKernel.Get<Employee>()
                });
        }

        private string GenerateUniqueString()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion
    }
}