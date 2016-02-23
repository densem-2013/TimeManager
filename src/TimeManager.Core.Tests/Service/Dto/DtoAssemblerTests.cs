namespace Infocom.TimeManager.Core.Tests.Service.Dto
{
    using System;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.Core.Services.Dto;

    using NUnit.Framework;

    [TestFixture]
    public class DtoAssemblerTests
    {
        private DtoAssembler _sut;

        [SetUp]
        public void SetUp()
        {
            this._sut = new DtoAssembler();
        }

        [Test]
        public void CreateTaskDto_SimpleTypeMapping_AllMemmabersMapped()
        {
            var task = new Task
                {
                    Name = "Task1",
                    Description = "Desc",
                    RegistrationDate = DateTime.Now,
                    StartDate = DateTime.Now.AddDays(1)
                };

            var dto = this._sut.CreateTaskDto(task);

            Assert.AreEqual(task.ID, dto.ID);
            Assert.AreEqual(task.Name, dto.Name);
            Assert.AreEqual(task.Description, dto.Description);
            Assert.AreEqual(task.RegistrationDate, dto.RegistrationDate);
            Assert.AreEqual(task.StartDate, dto.StartDate);
        }

        [Test, Ignore("todo")]
        public void CreateTaskDto_MappingParentTask_AllMemmabersMapped()
        {
            //var task = new Task { Name = "Root", ParentTask = new Task { Name = "Task1" } };

            //var dto = this._sut.CreateTaskDto(task);

            //Assert.IsNotNull(dto);
            //Assert.IsNotNull(dto.ParentTaskID);
            //Assert.AreEqual(task.ParentTask.ID, dto.ParentTask.Id);
            //Assert.AreEqual(task.ParentTask.Name, dto.ParentTask.Name);
        }

        [Test]
        public void CreateTaskDto_StatusMapping_CorrectValue()
        {
            var task = new Task { CurrentStatus = new Status { ApplicableTo = DomailEntityType.Task, Name = "Opened" } };

            var dto = this._sut.CreateTaskDto(task);

            Assert.IsNotNull(dto);
            Assert.AreEqual(task.CurrentStatus.Name, dto.CurrentStatusName);
        }

        //todo: add more tests
    }
}
