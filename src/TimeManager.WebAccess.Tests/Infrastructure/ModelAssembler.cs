namespace Infocom.TimeManager.WebAccess.Tests.Infrastructure
{
    using System.Collections.Generic;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Infrastructure;

    using NUnit.Framework;

    using System.Linq;

    using SharpTestsEx;

    [TestFixture]
    public class ModelAssemblerTests : BaseTest<ModelAssembler>
    {
        private class TestTask : Task
        {
            private static int _index;
            public TestTask()
            {
                this.ID = _index++;
            }
        }

        private class EmployeeTask : Employee
        {
            private static int _index;
            public EmployeeTask()
            {
                this.ID = _index++;
            }
        }

        #region Methods

        [Test]
        public void CreateEmployeeModel_CheckTasksMapping_NoMissedItems()
        {
            // arrange
            var employee = new Employee();
            employee.Tasks.Add(new TestTask()); // ID = 0
            employee.Tasks.Add(new TestTask()); // ID = 1

            // act
            var result = Sut.CreateEmployeeModel(employee);

            // arrange
            result.Tasks.Count().Should().Be.EqualTo(employee.Tasks.Count);
            result.Tasks.ElementAt(0).Should().Be.EqualTo(0);
            result.Tasks.ElementAt(1).Should().Be.EqualTo(1);
        }

        [Test]
        public void CreateEmployeeModel1_CheckTasksMapping_NoMissedItems()
        {
            // arrange
            var task = new Task();
            task.AssignedEmployees.Add(new EmployeeTask()); // ID = 0
            task.AssignedEmployees.Add(new EmployeeTask()); // ID = 1

            // act
            var result = Sut.CreateTaskModel(task);

            // arrange
            result.AssignedEmployeeIDs.Count().Should().Be.EqualTo(task.AssignedEmployees.Count);
            result.AssignedEmployeeIDs.ElementAt(0).Should().Be.EqualTo(0);
            result.AssignedEmployeeIDs.ElementAt(1).Should().Be.EqualTo(1);
        }

        protected override ModelAssembler InitializeSut()
        {
            return new ModelAssembler();
        }

        #endregion
    }
}