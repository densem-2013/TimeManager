namespace Infocom.TimeManager.Core.Tests.Service
{
    using System;
    using System.Collections.Generic;
    using System.Security.Authentication;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.Core.DomainModel.Repositories;
    using Infocom.TimeManager.Core.Services;

    using Microsoft.Practices.ServiceLocation;

    using Moq;

    using NHibernate;

    using NUnit.Framework;

    using System.Linq;

    [TestFixture]
    public class TaskManagementTests : BasePersistenceTest<TaskManagement>
    {
        private class Model
        {
            public Model(Project project, IEnumerable<Employee> employees)
            {
                this.Project = project;
                this.Employees = employees;
            }

            public Project Project { get; private set; }

            public IEnumerable<Employee> Employees { get; private set; }
        }

        #region Properties

        private Mock<TaskManagement> _sutMock;

        private Model _model;

        protected override TaskManagement InitializeSut()
        {
            this._sutMock = new Mock<TaskManagement> { CallBase = true };

            return this._sutMock.Object;
        }

        #endregion

        public override void SetUp()
        {
            base.SetUp();

            InitializeDBSchema();
            CreateTestModel();
        }

        private void CreateTestModel()
        {
            var emp_manager = new Employee { FirstName = "Алексей", LastName = "Светличный", PatronymicName = "Николаевич", Login = "INFOCOM-LTD\asvetlic" };
            var emp_amukas = new Employee { FirstName = "Алексей", LastName = "Мукась", PatronymicName = "Петрович", Manager = emp_manager, Login = "INFOCOM-LTD\amukas" };
            emp_manager.Subordinates.Add(emp_amukas);

            var openedStatus_project = new Status { Name = "Opened", ApplicableTo = DomailEntityType.Project };
            var openedStatus_task = new Status { Name = "Opened", ApplicableTo = DomailEntityType.Task };
            var type = new ProjectType { Name = "Проекты" };
            var project = new Project
                {
                    Name = "TimeManager",
                    Code = "ProjectCode",
                    CurrentStatus = openedStatus_project,
                    Description = "Internal project",
                    Rate = 1000,
                    ProjectType =type
                };

            var task_1 = new Task();
            task_1.AssignedEmployees.Add(emp_amukas);
            project.Tasks.Add(task_1);

            task_1.CurrentStatus = openedStatus_task;
            task_1.Name = "Task1";
            task_1.Description = "Test task";
            task_1.RegistrationDate = DateTime.Now;
            task_1.StartDate = DateTime.Now.AddDays(1);
            task_1.TimeBudget = TimeSpan.FromHours(10);

            var session = ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(project);

                tx.Commit();
            }

            this._model = new Model(project, new[] { emp_amukas, emp_manager });
        }

        #region Public Methods

        [Test]
        public void GetWorkItems_RegistredUserWithNoWorkItems_EmptyListRetrieved()
        {
            // arrange
            this.SetContextToRegistredUser(_model.Employees.Where(e => e.Tasks.Count == 0).Single());

            //// act
            var workItems = this.Sut.GetWorkItems();

            //// assert
            Assert.IsNotNull(workItems);
            Assert.IsEmpty(workItems);
        }

        [Test]
        public void GetWorkItems_RegistredUserWithWorkItems_ListOfTasksRetrieved()
        {
            // arrange
            var testEmp = this._model.Employees.Where(e => e.Tasks.Count > 0).Single();
            this.SetContextToRegistredUser(testEmp);

            // act
            var workItems = this.Sut.GetWorkItems();

            // assert
            Assert.IsNotNull(workItems);
            Assert.AreEqual(testEmp.Tasks.Count(), workItems.Count());
            for (int index = 0; index < testEmp.Tasks.Count; index++)
            {
                var assignedTask = workItems[index];
                Assert.AreEqual(assignedTask.Name, workItems[index].Name);
            }
        }

        [Test]
        [ExpectedException(typeof(AuthenticationException))]
        public void GetWorkItems_NotRegistredUser_Exception()
        {
            // arrange
            this.SetContextToNotRegistredUser();

            // act
            this.Sut.GetWorkItems(); // exception here
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BookTime_WrongTaskID_Exception()
        {
            // arrange
            var taskID = 0; // wrong value
            var startDate = new DateTime();
            var spentTime = TimeSpan.FromHours(1);

            // act
            this.Sut.BookTime(taskID, startDate, spentTime);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BookTime_WrongStartDate_Exception()
        {
            // arrange
            var taskID = this._model.Project.Tasks.First().ID;
            var startDate = new DateTime(); // wrong
            var spentTime = TimeSpan.FromHours(1);

            // act
            this.Sut.BookTime(taskID, startDate, spentTime);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BookTime_WrongSpentDate_Exception()
        {
            // arrange
            var taskID = this._model.Project.Tasks.First().ID;
            var startDate = new DateTime(2011, 1, 11);
            var spentTime = new TimeSpan(); // wrong

            // act
            this.Sut.BookTime(taskID, startDate, spentTime);
        }

        [Test]
        [ExpectedException(typeof(ObjectNotExistException))]
        public void BookTime_NotExistedId_Exception()
        {
            // arrange
            var testEmp = this._model.Employees.Where(e => e.Tasks.Count > 0).Single();
            this.SetContextToRegistredUser(testEmp);
            var taskID = long.MaxValue; // wrong
            var startDate = new DateTime(2011, 10, 10);
            var spentTime = TimeSpan.FromHours(1);

            // act
            this.Sut.BookTime(taskID, startDate, spentTime);
        }

        [Test]
        public void BookTime_CorrectValues_ShouldBeBooked()
        {
            // arrange
            var testEmp = this._model.Employees.Where(e => e.Tasks.Count > 0).Single();
            var taskID = testEmp.Tasks.First().ID;
            var startDate = new DateTime(2011, 1, 1);
            var spentTime = TimeSpan.FromHours(10);
            SetContextToRegistredUser(testEmp);

            // act 
            var timeRecord = Sut.BookTime(taskID, startDate, spentTime);

            // assert
            Assert.IsNotNull(timeRecord, "Retrieved timerecord should not be null.");
            Assert.IsTrue(timeRecord.ID != default(int), "New time record should be inserted.");
        }

        #endregion

        #region Methods

        private void SetContextToNotRegistredUser()
        {
            this._sutMock.Setup(tm => tm.CheckForUserRole()).Throws(new AuthenticationException());
        }

        private void SetContextToRegistredUser(Employee employee)
        {
            this._sutMock.Setup(tm => tm.CheckForUserRole());
            this._sutMock.Setup(tm => tm.GetLogin()).Returns(employee.Login);
        }

        #endregion
    }
}