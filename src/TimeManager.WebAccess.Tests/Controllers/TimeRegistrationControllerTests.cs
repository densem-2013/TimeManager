namespace Infocom.TimeManager.WebAccess.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Controllers;
    using Infocom.TimeManager.WebAccess.Extensions;
    using Infocom.TimeManager.WebAccess.Infrastructure;
    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;

    using Moq;

    using NHibernate;

    using NUnit.Framework;

    using SharpTestsEx;

    [TestFixture]
    public class TimeRegistrationControllerTests : BasePersistenceMockTest<TimeRegistrationController>
    {
        private ModelAssembler _assembler = new ModelAssembler();

        #region Methods

        public override void SetUp()
        {
            base.SetUp();
            this.InitializeDBSchema();
        }

        protected override Mock<TimeRegistrationController> InitializeSutMock()
        {
            return new Mock<TimeRegistrationController>() { CallBase = true };
        }

        [Test, Ignore("check it")]
        public void Index_ModelCheck_NoErrors()
        {

            // act
            var result = this.GetViewResult();

            // assert
            result.Should().Not.Be.Null();
            result.Model.Should().Not.Be.Null();
        }

        private ViewResult GetViewResult()
        {
            return this.Sut.Index(new DateTime(2011, 1, 3).ToString()) as ViewResult;
        }

        //[Test]
        //public void GetWeekDays_NoErrors()
        //{
        //    // act
        //    var controller = new TimeRegistrationController();
        //    var weekDays = controller.GetWeekDays(11, 2011);

        //    weekDays.Should().Be.Equals("Год 2011 Нед.11:14.03 - 20.03");
        //}
        
        #endregion

        [Test]
        public void CreateDefaultTimesheetForCurrentEmployee_NoTimeSheetInDBAndSomeTasksExist_NewTimeSheetWithTasksCreated()
        {
            // Arrange
            var date = DateTime.Now;
            

            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var tasks = Controllers.Model.CreateTasks(user, date);
            this._SetupUserFeature(user);
            // Act
            var actualTimeSheet = Sut.CreateDefaultTimesheetForCurrentEmployee(date);

            // Assert
            Assert.IsNotNull(actualTimeSheet, "Actual TimeSheet shouldn't be null.");
            Assert.AreEqual(tasks.Count, actualTimeSheet.Tasks.Count(), "Expected and actual items count should be the same.");
            foreach (var actualTask in actualTimeSheet.Tasks)
            {
                Assert.IsTrue(tasks.Contains(actualTask), "Not expected Actual task found.");
            }
        }

        [Test]
        public void CreateModel_DateWithTasks_CorrectViewModel()
        {
            // Arrange
            var date = DateTime.Now;

            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            this._SetupUserFeature(user); 
            var tasks = Controllers.Model.CreateTasks(user, date);

            // Act
            var actualViewModel = Sut.CreateModel(date);

            // Assert
            Assert.IsNotNull(actualViewModel, "Actual shouldn't be null.");
            Assert.IsNotEmpty(actualViewModel.TimeSheet.Tasks.ToArray(), "Some tasks should be retrieved .");
            Assert.AreEqual(tasks.Count, actualViewModel.TimeSheet.Tasks.Count(), "Expected and actual items count should be the same.");
            foreach (var actualTask in actualViewModel.TimeSheet.Tasks)
            {
                Assert.IsTrue(tasks.Where(t => t.ID == actualTask.ID).Count() == 1, "Not expected Actual task found.");
            }
        }

        [Test]
        public void CreateModel_DateWithNoRegistration_EmptyTimeSheetInViewModel()
        {
            // Arrange
            var date = DateTime.Now;

            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            this._SetupUserFeature(user);

            // Act
            var actualViewModel = Sut.CreateModel(date);

            // Assert
            Assert.IsNotNull(actualViewModel, "Actual shouldn't be null.");
            Assert.IsEmpty(actualViewModel.TimeSheet.Tasks.ToArray(), "No tasks should be retrieved .");
        }

        [Test]
        public void DeleteExtraTimeRecords_DeleteTimeRecordsUnderWeekInterval_ChangedTimeSheetTimeRecords()
        {
            // Arrange
            var currentDate = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var model = new TimeRegistrationViewModel();
            var timeSheet = Controllers.Model.CreateTimeSheetModel(user);
            _SetupUserFeature(user);

            model.SelectedDate = currentDate;
            model.TimeSheet = timeSheet;

            var startDate = currentDate.FirstDayOfWeek();
            var finishDate = startDate.AddDays(7).AddMilliseconds(-1);
            var TotalSpentTimeByWeek = new TimeSpan();

            foreach (var task in timeSheet.Tasks)
            {
                var records = task.TimeRecords.ToList().Where(tr => tr.StartDate >= startDate && tr.StartDate < finishDate);
                var spentTime = records.Sum(tr => tr.SpentTime.Ticks);
                TotalSpentTimeByWeek += new TimeSpan(spentTime);
            }

            // todo: Move to test time sheet model 
            // Assert
            Assert.AreEqual(TotalSpentTimeByWeek.Ticks, model.TimeSheet.TotalSpentTimeByWeek.Ticks, "Expect TotalSpentTimeOnWeek = 40 hours on currentDate");
        }
        
        [Test]
        public void GetProjectForEmployee_GetJsonResultWithTaskForProjectCollection_GetProjectForCurrentEmployee()
        {
            // Arrange
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            Controllers.Model.CreateProject(user, date);
            _SetupUserFeature(user);

            // Act
            var projectsForEmployee = (JsonResult)Sut.GetProjectForEmployee();
            var projectList = (SelectList) projectsForEmployee.Data;
            // Assert
            Assert.IsTrue(projectList.Any(), "ProjectForEmployee should have 1 project");
            Assert.AreEqual("TestProject", projectList.First().Text, "ProjectForEmployee should have project name 'TestProject'");
        }

        [Test]
        public void GetTasksForProject_GetJsonResultWithTaskForProjectCollection_GetTasksForCurrentEmployee()
        {
            // Arrange
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            Controllers.Model.CreateProject(user, date);
            _SetupUserFeature(user);
            var projectsForEmployee = (JsonResult)Sut.GetProjectForEmployee();
            var projects = (SelectList)projectsForEmployee.Data;
            var projectID = long.Parse(projects.First().Value); 
            // Act
            var tasksForEmployee = (JsonResult)Sut.GetTasksForProject(projectID);
            var taskts = (SelectList)tasksForEmployee.Data;

            // Assert
            Assert.IsTrue(taskts.Any(), "tasksForEmployee should have 1 project");
            Assert.AreEqual("Task0", taskts.First().Text, "Fist element of task list should have name 'TestProject'");
        }
        
        [Test]
        public void GetTimeSheetForCurrentEmployee_GetTimeSheetForCurenWeek_TimeSheetHasTasks()
        {
            // Arrange
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            Controllers.Model.CreateProject(user, date);
            Controllers.Model.CreateTimeSheet(user);
            _SetupUserFeature(user);
           
            // Act
            var timeSheet = Sut.GetTimeSheetForCurrentEmployee(date);

            // Assert
            Assert.IsNotNull(timeSheet, "TimeSheet should be not null.");
            Assert.IsTrue(timeSheet.Tasks.Any() , "TimeSheet for user should have any task.");
            Assert.AreEqual("Task0", timeSheet.Tasks.First().Name, "Fist element of task list should have name 'TestProject'");
        }

        [Test]
        public void GetTotalSpentTimeInMonth_GetTotalTimeOfMonth_ShouldBeSumOfTime()
        {
            // Arrange
            var dateApril = new DateTime(2011,4,5);
            var dateMay = new DateTime(2011, 5, 5);
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            Controllers.Model.CreateProject(user, dateApril);
            _SetupUserFeature(user);
            var task = Controllers.Model.CreateNewTask(user, dateApril);
            // spent time in April 
            task.TimeRecords.Add(new TimeRecord { Task = task, Employee = user, StartDate = dateApril, SpentTime = new TimeSpan(8, 0, 0) });
            task.TimeRecords.Add(new TimeRecord { Task = task, Employee = user, StartDate = dateApril.AddDays(2), SpentTime = new TimeSpan(8, 0, 0) });
            // spent time in May
            task.TimeRecords.Add(new TimeRecord { Task = task, Employee = user, StartDate = dateMay, SpentTime = new TimeSpan(4, 0, 0) });
            task.TimeRecords.Add(new TimeRecord { Task = task, Employee = user, StartDate = dateMay.AddDays(1), SpentTime = new TimeSpan(8, 0, 0) });
            task.TimeRecords.Add(new TimeRecord { Task = task, Employee = user, StartDate = dateMay.AddDays(2), SpentTime = new TimeSpan(8, 0, 0) });

            using (var tx = CurrentSession.BeginTransaction())
            {
                CurrentSession.SaveOrUpdate(task);

                tx.Commit();
            }

            // Act
            var totalSpentTimeInApril = Sut.GetTotalSpentTimeInMonth(dateApril);
            var totalSpentTimeInMay = Sut.GetTotalSpentTimeInMonth(dateMay);

            // Assert
            Assert.IsNotNull(totalSpentTimeInApril, "TimeSheet should be not null.");
            Assert.IsNotNull(totalSpentTimeInMay, "TimeSheet should be not null.");
            Assert.AreEqual(new TimeSpan(16, 0, 0).Ticks, totalSpentTimeInApril.Ticks, "Spent Time in April should equal 16 (or 576000000000 ticks) hours");
            Assert.AreEqual(new TimeSpan(20, 0, 0).Ticks, totalSpentTimeInMay.Ticks, "Spent Time in May should equal 20 (or 720000000000 ticks) hours");
        }

        [Test]
        public void InitializeViewData_RegistrationDateInViewBag_RegistrationDateShouldBeEqualFistDayOfWeek()
        {
            // Arrange    
            var middleOfWeek = new DateTime(2011, 04, 14);
            var firstDayOfWeek = new DateTime(2011, 04, 11);

            // Act 
            Sut.InitializeViewData(middleOfWeek);

            // Assert
            var actualRegistrationDate = ((DateTime)Sut.ViewBag.RegistrationDate);

            Assert.AreEqual(firstDayOfWeek, actualRegistrationDate, "Expected and actual items count should be the same.");
        }

        [Test]
        public void InitializeViewData_DateInTheMiddleOfWeek_7DaysToEdit()
        {
            // Arrange    
            var middleOfWeek = new DateTime(2011, 04, 14);
            var firstDayOfWeek = new DateTime(2011, 04, 11);
            var lastDayOfWeek = new DateTime(2011, 04, 17);

            int expectedDaysToEdit = 7;

            // Act 
            Sut.InitializeViewData(middleOfWeek);

            // Assert
            var actualDaysToEdit = ((List<DateTime>)Sut.ViewBag.DaysToEdit);

            Assert.AreEqual(expectedDaysToEdit, actualDaysToEdit.Count, "Expected and actual items count should be the same.");
            Assert.IsTrue(actualDaysToEdit.Contains(firstDayOfWeek), "FirstDayOfWeek to edit is not found.");
            Assert.IsTrue(actualDaysToEdit.Contains(lastDayOfWeek), "LastDayOfWeek to edit is not found.");

        }

        [Test]
        public void InitializeViewData_TaskStatusesInViewBag_TaskStatusesCollectionShouldBeCreated()
        {
            // Arrange    
            var taskStatuses = Controllers.Model.CreateTaskStatusesModel();
            var middleOfWeek = new DateTime(2011, 04, 14);

            // Act 
            Sut.InitializeViewData(middleOfWeek);

            // Assert
            var actualTaskStatuses = ((IEnumerable<StatusModel>)Sut.ViewBag.TaskStatuses);
            Assert.IsNotNull(actualTaskStatuses, "Actual shouldn't be null");
            Assert.AreEqual(taskStatuses, actualTaskStatuses, "Expected and actual items count should be the same.");
        }

        [Test]
        public void InitializeViewData_WorkingWeeksInViewBag_WorkingWeeksShouldBeCreated()
        {
            // Arrange    
            var middleOfWeek = new DateTime(2011, 04, 14);
            var WorkingWeeks = _WorkingWeeks(middleOfWeek);
           
            // Act 
            Sut.InitializeViewData(middleOfWeek);

            // Assert
            var actualWorkingWeeks = ((List<TimeSheetOverviewModel>)Sut.ViewBag.WorkingWeeks);
            Assert.IsNotNull(actualWorkingWeeks, "Actual shouldn't be null");
            Assert.AreEqual(WorkingWeeks.Count, actualWorkingWeeks.Count, "Expected and actual items count should be the same.");
            Assert.AreEqual(WorkingWeeks.First().Title, actualWorkingWeeks.First().Title, "Expected and actual items title should be the same.");
            Assert.AreEqual(WorkingWeeks.Last().Title, actualWorkingWeeks.Last().Title, "Expected and actual items title should be the same.");
        }
       
        [Test]
        public void GetTasksForWeek_EmployeeHasTasksTodayOnly_RetrieveAllAssociatedTasks()
        {
            //// Arrange

            var startDate = DateTime.Now;

            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var tasks = Controllers.Model.CreateTasks(user, startDate);

            //// Act
            var actualTasks = Sut.GetTasksForWeek(startDate, user);

            //// Assert
            Assert.IsNotNull(actualTasks, "Actual result shouldn't null");
            Assert.AreEqual(tasks.Count, actualTasks.Count(), "Expected and actual items count should be the same.");
            foreach (var actualTask in actualTasks)
            {
                Assert.IsTrue(tasks.Contains(actualTask), "Not expected Actual task found.");
            }
        }

        [Test]
        public void GetTasksForWeek_EmployeeHasTasksForYesterdayTodayAndNextWeek_RetrieveAllAssociatedTasks()
        {
            //// Arrange
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);

            var today = DateTime.Now;
            var yesterday = DateTime.Now.AddDays(-1);
            var dayOfNextWeek = DateTime.Now.AddDays(7);

            var tasksForThisWeek = Controllers.Model.CreateTasks(user, today);
            var tasksForYesterday = Controllers.Model.CreateTasks(user, yesterday);
            var tasksForNextWeek = Controllers.Model.CreateTasks(user, dayOfNextWeek);

            //// Act
            var actualTasks = Sut.GetTasksForWeek(today, user);

            //// Assert
            Assert.IsNotNull(actualTasks, "Actual result shouldn't null");
            Assert.AreEqual(
                tasksForThisWeek.Count, actualTasks.Count(), "Expected and actual items count should be the same.");
            foreach (var actualTask in actualTasks)
            {
                Assert.IsTrue(tasksForThisWeek.Contains(actualTask), "Not expected Actual task found.");
            }
        }

        [Test]
        public void GetNotFinishedTasksForWeek_TaskCreated2WeeksAgoAndFinishedOnThisWeek_TaskRetrieved()
        {
            //// Arrange

            var twoWeeksAgo = DateTime.Now.AddDays(-14);
            var finishDate = DateTime.Now;

            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);

            var task = new Task();
            task.Description = "Some task";
            task.Name = "Task";
            task.StartDate = twoWeeksAgo;
            task.FinishDate = finishDate;
            task.AssignedEmployees.Add(user); //associating task with user

            using (var tx = CurrentSession.BeginTransaction())
            {
                CurrentSession.Save(task);
                tx.Commit();
            }

            //// Act
            var actualTasks = Sut.GetNotFinishedTasksForWeek(DateTime.Now, user);

            //// Assert
            Assert.IsNotNull(actualTasks, "Actual result shouldn't null.");
            Assert.AreEqual(1, actualTasks.Count(), "Exact one item should be found.");
            Assert.IsTrue(task == actualTasks.First(), "Not expected Actual task found.");
        }

        [Test]
        public void GetNotFinishedTasksForWeek_TaskCreatedOnLastWeeksAndFinishedOnNextWeek_TaskRetrieved()
        {
            //// Arrange

            var lastWeeks = DateTime.Now.AddDays(-7);
            var finishDate = DateTime.Now.AddDays(7);

            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);

            var task = new Task();
            task.Description = "Some task";
            task.Name = "Task";
            task.StartDate = lastWeeks;
            task.FinishDate = finishDate;
            task.AssignedEmployees.Add(user); //associating task with user

            using (var tx = CurrentSession.BeginTransaction())
            {
                CurrentSession.Save(task);
                tx.Commit();
            }

            //// Act
            var actualTasks = Sut.GetNotFinishedTasksForWeek(DateTime.Now, user);

            //// Assert
            Assert.IsNotNull(actualTasks, "Actual result shouldn't null.");
            Assert.AreEqual(1, actualTasks.Count(), "Exact one item should be found.");
            Assert.IsTrue(task == actualTasks.First(), "Not expected Actual task found.");
        }

        [Test]
        public void TimeSheetAddTask_AddedTaskInTimeSheetCorrectly_CreateNewTaskAndAddedToTimeSheet()
        {
            //// Arrange 
           
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var timeSheet = Controllers.Model.CreateTimeSheet(user);
            this._SetupUserFeature(user);
            var task = Controllers.Model.CreateNewTask(user, date);
            TaskModel model = _assembler.CreateTaskModel(task);
            //// Assert
            Assert.AreEqual(5,timeSheet.Tasks.Count,"Count of task should be 5");

            //// Act
            Sut.TimeSheetAddTask(timeSheet.ID, model, date, task.ID);

            //// Assert
            Assert.AreEqual(6, timeSheet.Tasks.Count, "After add count of task should be 6");
            Assert.IsTrue(timeSheet.Tasks.Where(t => t.Name == "TaskForAdd").Any(), "TimeSheet contain task with name 'TaskForAdd'");

        }
        
        [Test]
        public void TimeSheetAddTask_ReceivedModuleStateErrorMessage_TestOnNullTask()
        {
            //// Arrange  
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            this._SetupUserFeature(user);
            var timeSheet = Controllers.Model.CreateTimeSheet(user);
            var taskID = 0;

            //// Act
            Sut.TimeSheetAddTask(timeSheet.ID, new TaskModel(),date, taskID);
            
            //// Assert
            Assert.AreEqual(1,Sut.ModelState.Count, "Wrong task. ModelState should contain 1 exception.");

        }

        [Test]
        public void TimeSheetAddTask_ReceivedModuleStateErrorMessage_TestOnNullTimeSheet()
        {
            //// Arrange 
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var timeSheetID = 0;
            this._SetupUserFeature(user);
            var task = Controllers.Model.CreateNewTask(user, date);

            //// Act
            Sut.TimeSheetAddTask(timeSheetID, new TaskModel(), date, task.ID);

            //// Assert
            Assert.AreEqual(1, Sut.ModelState.Count, "Wrong task. ModelState should contain 1 exception.");
        }

        [Test]
        public void TimeSheetAddTask_ReceivedModuleStateErrorMessage_TestOnTaskIsContained()
        {
            //// Arrange
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var timeSheet = Controllers.Model.CreateTimeSheet(user); 
            this._SetupUserFeature(user);
            var task = timeSheet.Tasks.First();

            //// Act
            Sut.TimeSheetAddTask(timeSheet.ID, new TaskModel(), date, task.ID);

            //// Assert
            Assert.AreEqual(1, Sut.ModelState.Count, "Wrong task. ModelState should contain 1 exception.");
        }

        [Test]
        public void TimeSheetUpdateTask_UpdatedTaskInTimeSheetCorrectly_SpentTimeByDaysUpdatedForTask()
        {
            //// Arrange 
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var timeSheet = Controllers.Model.CreateTimeSheet(user);
            this._SetupUserFeature(user);

            var timeSpantByDay = new List<TimeSpan>();
            var taskToUpdate = timeSheet.Tasks.First();
            var addedSpentTime = new TimeSpan(4, 0, 0);
            var dayOfUpdate = date.AddDays(-1);
            timeSpantByDay.Add(addedSpentTime);

            var totalSpentTime = taskToUpdate.TimeRecords.Sum(tr => tr.SpentTime.Value.Ticks);

            //// Act
            Sut.TimeSheetUpdateTask(taskToUpdate.ID, dayOfUpdate, timeSpantByDay);

            //// Assert
            Assert.AreEqual(totalSpentTime + addedSpentTime.Ticks, timeSheet.Tasks.Where(t => t == taskToUpdate).Single().TimeRecords.Sum(tr => tr.SpentTime.Value.Ticks), "After update amount of assigned employees on task should equal 2");

        }

        [Test]
        public void TimeSheetUpdateTask_ReceivedModuleStateErrorMessage_TestOnTaskIsNull()
        {
            //// Arrange
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            this._SetupUserFeature(user);
            var taskID = 0;
            var timeSpantByDays = new List<TimeSpan>();

            //// Act
            Sut.TimeSheetUpdateTask(taskID, date, timeSpantByDays);

            //// Assert
            Assert.AreEqual(1, Sut.ModelState.Count, "Wrong task. ModelState should contain 1 exception.");
        }

        [Test]
        public void TimeSheetUpdateTask_ReceivedModuleStateErrorMessage_TestOnTimeSpantByDaysIsNull()
        {
            //// Arrange
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            this._SetupUserFeature(user);
            var task = Controllers.Model.CreateNewTask(user,date);
            List<TimeSpan> timeSpantByDays = null;

            //// Act
            Sut.TimeSheetUpdateTask(task.ID, date, timeSpantByDays);

            //// Assert
            Assert.AreEqual(1, Sut.ModelState.Count, "Wrong task. ModelState should contain 1 exception.");
        }

        [Test]
        public void TimeSheetDeleteTask_DeleteTaskFromTimeSheet_TaskDeletedCorrectly()
        {
            //// Arrange
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var timeSheet = Controllers.Model.CreateTimeSheet(user);
            this._SetupUserFeature(user);
            var task = timeSheet.Tasks.First();

            //// Assert
            Assert.AreEqual(5, timeSheet.Tasks.Count, "Count of task in time sheet should be equal 5");

            //// Act
            Sut.TimeSheetDeleteTask(timeSheet.ID, task.ID, date);

            //// Assert
            Assert.AreEqual(4, timeSheet.Tasks.Count, "Count of task in time sheet should be equal 4");
            Assert.IsFalse(timeSheet.Tasks.Contains(task), "Time sheet not contained current task");
        }

        [Test]
        public void TimeSheetDeleteTask_ReceivedModuleStateErrorMessage_TestOnTimeSheetIsNull()
        {
            //// Arrange
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var timeSheetID = 0;
            this._SetupUserFeature(user);
            var task = Controllers.Model.CreateNewTask(user,date);

            //// Act
            Sut.TimeSheetDeleteTask(timeSheetID, task.ID, date);

            //// Assert
            Assert.AreEqual(1, Sut.ModelState.Count, "Wrong task. ModelState should contain 1 exception.");
        }

        [Test]
        public void TimeSheetDeleteTask_ReceivedModuleStateErrorMessage_TestOnWrongTaskToDelete()
        {
            //// Arrange
            var date = DateTime.Now;
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var timeSheet = Controllers.Model.CreateTimeSheet(user);
            this._SetupUserFeature(user);
            var taskID = 0;

            //// Act
            Sut.TimeSheetDeleteTask(timeSheet.ID, taskID, date);

            //// Assert
            Assert.AreEqual(1, Sut.ModelState.Count, "Wrong task. ModelState should contain 1 exception.");
        }

       
        private List<TimeSheetOverviewModel> _WorkingWeeks(DateTime selectedDate)
        {
            var timeSheetOverviews = new List<TimeSheetOverviewModel>();

            var firstDayOfYear = new DateTime(selectedDate.Year, 1, 1).FirstDayOfWeek();

            // 52 week in year
            for (int i = 1; i <= 52; i++)
            {
                var date = firstDayOfYear.AddDays(i * 7);
                timeSheetOverviews.Add(new TimeSheetOverviewModel
                {
                    Date = date,
                    Title = string.Format("Неделя: {0:00} (c {1:dd.MM} по {2:dd.MM}) {1:yyyy}",
                                date.WeekNumber(),
                                date,
                                date.DateByWeekDayNumber(7))

                });
            }
            return timeSheetOverviews;
        }

        private void _SetupUserFeature(Employee user)
        {
            var userFeature = new Mock<IFutureValue<Employee>>();
            userFeature.Setup(uf => uf.Value).Returns(user);
            SutMock.Setup(c => c.CurrentEmployee).Returns(userFeature.Object);
        }

    }
}
