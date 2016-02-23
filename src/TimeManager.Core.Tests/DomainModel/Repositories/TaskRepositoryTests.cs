namespace Infocom.TimeManager.Core.Tests.DomainModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.Core.DomainModel.Repositories;

    using NHibernate.Validator.Exceptions;

    using Ninject;

    using NUnit.Framework;

    using Castle.Core;

    using SharpTestsEx;

    [TestFixture, Ignore("todo")]
    public class TaskRepositoryTests : RepositoryBaseTests<TaskRepository, Task>
    {
        #region Public Methods

        [Test]
        public override void Add_ItemSeveralItems_ShouldBeAdded()
        {
            base.Add_ItemSeveralItems_ShouldBeAdded();
        }

        [Test]
        public override void GetAll_LazyLoading_NoError()
        {
            base.GetAll_LazyLoading_NoError();
        }

        [Test]
        public void BookTime_CorrectValues_NewRecordInDB()
        {
            // arrange
            var timeToBook = new TimeRecord
                {
                    StartDate = new DateTime(2011, 1, 11),
                    SpentTime = TimeSpan.FromHours(1),
                    Description = "Test Registration",
                    Employee = this.MockingKernel.Get<Employee>(),
                    Task = MockingKernel.Get<Task>()
                };

            // act
            this.Sut.BookTime(timeToBook);

            // assert
            Assert.IsFalse(timeToBook.ID == default(int), "ID should be set, mean the data is inserted.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BookTime_NullArgTimeToBook_Exception()
        {
            // arrange
            TimeRecord timeToBook = null;

            // act
            this.Sut.BookTime(timeToBook); // expected exception here
        }

        [Test]
        public void GetTasksAssignedToLogin_CorrectLogin_RetrievedAssignedTasks()
        {
            // arrange
            var employee = this.MockingKernel.Get<Employee>();
            var employeeRepo = new EmployeeRepository();
            IList<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var task = this.MockingKernel.Get<Task>();
                employee.Tasks.Add(task);
                tasks.Add(task);
            }

            // act
            employeeRepo.Add(employee);
            var assignedTasks = this.Sut.GetTasksAssignedToLogin(employee.Login);

            // assert
            Assert.IsNotNull(assignedTasks);
            Assert.AreEqual(tasks.Count, assignedTasks.Count(), "Exact 1 task should be assigned.");
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void GetAssignedEmployees_NullArg_Exception()
        {
            // arrange
            Task task = null;

            // act
            this.Sut.GetAssignedEmployees(task); // expected exception here
        }

        [Test]
        public void GetAssignedEmployees_CorrectResults_NoMissedItems()
        {
            // arrange
            var task = MockingKernel.Get<Task>();
            var employees = new[] { MockingKernel.Get<Employee>(), MockingKernel.Get<Employee>() };
            employees.ForEach(e => e.Tasks.Add(task));
            var empRepo = new EmployeeRepository();

            // act
            empRepo.Add(employees);
            var assignedEmps = this.Sut.GetAssignedEmployees(task);

            // assert
            assignedEmps.Should().Not.Be.Null();
            assignedEmps.Should().Have.Count.EqualTo(employees.Count());
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void GetTimeSheets_NullArg_Exception()
        {
            // arrange
            Task task = null;

            // act
            this.Sut.GetTimeSheets(task); // expected exception here
        }

        [Test]
        public void GetTimeSheets_CorrectResults_NoMissedItems()
        {
            // arrange
            var task = MockingKernel.Get<Task>();
            var timeSheets = new[] { MockingKernel.Get<TimeSheet>(), MockingKernel.Get<TimeSheet>() };
            timeSheets.ForEach(e => e.Tasks.Add(task));
            var tsRepo = new TimeSheetRepository();

            // act
            tsRepo.Add(timeSheets);
            var assignedTimeSheets = this.Sut.GetTimeSheets(task);

            // assert
            assignedTimeSheets.Should().Not.Be.Null();
            assignedTimeSheets.Should().Have.Count.EqualTo(timeSheets.Count());
        }

        [Test]
        public void Add_ManyToMany_ShouldBeAdded()
        {
            // arrange
            var timeSheets = new[] { this.MockingKernel.Get<TimeSheet>(), this.MockingKernel.Get<TimeSheet>() };
            var tasks = new[] { this.MockingKernel.Get<Task>(), this.MockingKernel.Get<Task>() };

            foreach (var timeSheet in timeSheets)
            {
                foreach (var task in tasks)
                {
                    timeSheet.Tasks.Add(task);
                }
            }

            // act
            try
            {
                this.Sut.Add(tasks);
            }
            catch (InvalidStateException ex)
            {
                var invalidValues = ex.GetInvalidValues();
                var errorMessage = ex.Message;
                foreach (var invalidValue in invalidValues)
                {
                    errorMessage += "\n" + invalidValue.Message;
                }

                throw new ApplicationException(errorMessage);
            }

            // assert
            foreach (var timeSheet in timeSheets)
            {
                var actualTasks = Sut.GetById(timeSheet.ID);
                Assert.AreEqual(tasks.Count(), actualTasks.TimeSheets.Count());
                foreach (var task in timeSheet.Tasks)
                {
                    Assert.AreEqual(timeSheets.Count(), task.AssignedEmployees.Count());
                }
            }
        }

        #endregion

        #region Methods

        [Test]
        public override void Delete_ExistedId_DeletedObject()
        {
            base.Delete_ExistedId_DeletedObject();
        }

        protected override Task CreateSimpleItem()
        {
            return MockingKernel.Get<Task>();
        }

        protected override Task CreateComplexItem()
        {
            var result = this.CreateSimpleItem();
            result.Project = this.MockingKernel.Get<Project>();

            return result;
        }

        protected override object GetLinkedComplexObject(Task source)
        {
            return source.Project;
        }

        protected override TaskRepository InitializeSut()
        {
            return new TaskRepository();
        }

        #endregion
    }
}