namespace Infocom.TimeManager.WebAccess.Tests.Controllers
{
    using System;
    using System.Web.Mvc;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Controllers;
    using Infocom.TimeManager.WebAccess.Exceptions;
    using Infocom.TimeManager.WebAccess.Infrastructure;

    using Moq;

    using NHibernate;

    using NUnit.Framework;

    using System.Linq;

    using NHibernate.Linq;

    using SharpTestsEx;

    [TestFixture]
    public class ProjectDetailsControllerTests : BasePersistenceMockTest<ProjectDetailsController>
    {
        protected static ModelAssembler _assambler = new ModelAssembler();

        #region Methods

        public override void SetUp()
        {
            base.SetUp();

            this.InitializeDBSchema();
        }

        protected override Mock<ProjectDetailsController> InitializeSutMock()
        {
            return new Mock<ProjectDetailsController>() { CallBase = true };
        }

        [Test]
        public void CreateModel_ProjectWithTasks_CorrectViewModel()
        {
            // Arrange
            var date = DateTime.Now;

            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var project = Controllers.Model.CreateProject(user, date);
            // Act
            var actualViewModel = Sut.CreateModel(project.ID);

            // Assert
            Assert.IsNotNull(actualViewModel, "Actual shouldn't be null.");
            Assert.IsNotNull(actualViewModel.Project, "Actual shouldn't be null.");
            Assert.AreEqual(project.ID, actualViewModel.Project.ID, "Project ID should be the same");
            Assert.AreEqual(project.Name, actualViewModel.Project.Name, "Project ID should be the same");
            Assert.IsNotEmpty(actualViewModel.Tasks.ToArray(), "Some tasks should be retrieved .");
            Assert.AreEqual(project.Tasks.Count, actualViewModel.Tasks.Count(), "Expected and actual items count should be the same.");
            foreach (var actualTask in actualViewModel.Tasks)
            {
                Assert.IsTrue(project.Tasks.Where(t => t.ID == actualTask.ID).Count() == 1, "Not expected Actual task found.");
            }
        }

        [Test]
        public void CreateModel_ProjectWithoutTasks_CorrectViewModel()
        {
            // Arrange
            var date = DateTime.Now;

            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var project = new Project();
            project.Name = "TestProject";
            project.ProjectType = Controllers.Model.CreateProjectTypes().First();
            project.Code = "TestCode";
            project.CurrentStatus = Controllers.Model.CreateProjectStatuses().First();

            using (var tx = CurrentSession.BeginTransaction())
            {
                CurrentSession.Save(project);

                tx.Commit();
            }

            // Act
            var actualViewModel = Sut.CreateModel(project.ID);

            // Assert
            Assert.IsNotNull(actualViewModel, "Actual shouldn't be null.");
            Assert.IsNotNull(actualViewModel.Project, "Actual shouldn't be null.");
            Assert.AreEqual(project.ID, actualViewModel.Project.ID, "Project ID should be the same");
            Assert.AreEqual(project.Name, actualViewModel.Project.Name, "Project ID should be the same");
            Assert.IsEmpty(actualViewModel.Tasks.ToArray(), "Some tasks should be retrieved .");
        }

        [Test]
        public void CreateModel_ProjectWithWrongId_ErrorViewModel()
        {
            // Arrange
            var date = DateTime.Now;

            var department = Controllers.Model.CreateDepartment();
            var projectID = 0;

            // Act
            var actualViewModel = Sut.CreateModel(projectID);

            // Assert
            Assert.IsNotNull(actualViewModel, "Actual shouldn't be null.");
            Assert.AreEqual(1, Sut.ModelState.Count, "Wrong task. ModelState should contain 1 exception.");
        }

        [Test]
        public void TaskDelete_TaskModel_CorrectDeleting()
        {
            // Arrange
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            this._SetupUserFeature(user);

            var project = Controllers.Model.CreateProject(user, date);
            var taskModel =  project.Tasks.Select(_assambler.CreateTaskModel).First();
            var taskID = taskModel.ID;

            var taskBeforeDeleteAction = Sut.PersistenceSession.Get<Task>(taskID);

            // Assert
            Assert.IsNotNull(taskBeforeDeleteAction, "Actual shouldn't be null.");
            // Act
            Sut.TaskDelete(taskModel);

            var isTaskExists = Sut.PersistenceSession.Get<Task>(taskID) != null;
            // Assert
            Assert.IsFalse(isTaskExists, "Task should be deleted.");
        }


        [Test, ExpectedException(typeof(InfrastructureException))]
        public void Index_AssesingWithNotExistedID_CorrectObjectRetrieved()
        {
            // arrange
            var notExistedProjectID = 0;
            Sut.ModelState.IsValid.Should().Be.True();

            // act
            var viewResult = Sut.Index(notExistedProjectID) as ViewResult;

            // assert
            Sut.ModelState.IsValid.Should().Be.False();
            viewResult.Should().Not.Be.Null();
            viewResult.ViewName.Should().Be.Empty();
        }

        #endregion

        private void _SetupUserFeature(Employee user)
        {
            var userFeature = new Mock<IFutureValue<Employee>>();
            userFeature.Setup(uf => uf.Value).Returns(user);
            SutMock.Setup(c => c.CurrentEmployee).Returns(userFeature.Object);
        }

    }
}