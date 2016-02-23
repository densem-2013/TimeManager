namespace Infocom.TimeManager.Core.Tests.DomainModel
{
    using System;

    using Infocom.TimeManager.Core.DomainModel;

    using NUnit.Framework;

    public class TaskTests : BaseTest<Task>
    {
        #region Public Methods

        [Test]
        public void Project_SettingProjectWithNullTasks_ForceInitialization()
        {
            // arrange
            var project = new Project();

            // act
            this.Sut.Project = project; // no exception here 

            // assert
            Assert.IsNotNull(project.Tasks);
            Assert.IsTrue(project.Tasks.Contains(this.Sut), "Task should be added to the project's task collection.");
        }

        [Test]
        public void Project_SettingProjectTwice_NoDoubling()
        {
            // arrange
            var project = new Project();

            // act
            this.Sut.Project = project;
            this.Sut.Project = project;

            // assert
            Assert.IsTrue(project.Tasks.Contains(this.Sut), "Task should be added to the project's task collection.");
            Assert.AreEqual(1, project.Tasks.Count, "Exact one task should be in the project's collection.");
        }

        [Test, Ignore("todo")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Project_SettingNull_Exceptions()
        {
            // act
            this.Sut.Project = null; // no exceptions here
        }

        #endregion

        #region Methods

        protected override Task InitializeSut()
        {
            return new Task();
        }

        #endregion
    }
}