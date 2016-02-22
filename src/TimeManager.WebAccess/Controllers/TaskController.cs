namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Extensions;
    using Infocom.TimeManager.WebAccess.Infrastructure;

    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;

    [HandleError]
    public partial class TaskController : TimeManagerBaseController
    {
        #region Public Methods

        public virtual ActionResult Index(long taskID)
        {
            var model = this.CreateModel(taskID);

            return View(model);
        }

        public virtual ActionResult CloseTask(string taskID)
        {
            long result = 0;
            if (long.TryParse(taskID,out result))
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    var taskForUpdate = this.PersistenceSession.Get<Task>(long.Parse(taskID));

                    taskForUpdate.CurrentStatus = this.PersistenceSession.Get<Status>((long)131075);
                    taskForUpdate.FinishDate = DateTime.Now; 
                    this.PersistenceSession.Update(taskForUpdate);
                    tx.Commit();
                }
            }
            return Redirect("~");
        }

        #endregion

        #region Methods

        private TaskViewModel CreateModel(long taskID)
        {
            var model = new TaskViewModel(this.CurrentEmployee.Value.ID);

            var allEmployees = this.PersistenceSession.QueryOver<Employee>().Future();

            var assembler = new ModelAssembler();

            var persistedTask = this.PersistenceSession.Load<Task>(taskID);

            if (persistedTask == null)
            {
                this.ModelState.AddModelError(
                    String.Empty,
                    String.Format("Задача с ID = {0} не найдена", taskID));
                return model;
            }

            IEnumerable<TimeSheet> timeSheets =
                PersistenceSession.QueryOver<TimeSheet>()
                .Where(ts => ts.Employee == this.CurrentEmployee.Value )
                .Future();

            var persistedTimeRecordsForTask = PersistenceSession.QueryOver<TimeRecord>().Where(tr => tr.Task == persistedTask);

            model.SpentTimeByAll = new TimeSpan(persistedTimeRecordsForTask.Future().Sum(tr => tr.SpentTime.Value.Ticks));
           
            var persistedMyTimeRecords =
                persistedTimeRecordsForTask.Where(tr => tr.Task == persistedTask
                                                                           && tr.Employee == this.CurrentEmployee.Value).Future();

            model.Task = assembler.CreateTaskModel(persistedTask);
            model.AssignedEmployees = persistedTask.AssignedEmployees.Select(assembler.CreateEmployeeModel);
            model.TimeSheetOverviews =
                timeSheets.Where(ts => ts.Tasks.Contains(persistedTask)).Select(
                    ts =>
                    new TimeSheetOverviewModel
                        {
                            Title =
                                string.Format(
                                    "Неделя: {0:00} (c {1:dd.MM} по {2:dd.MM}) {1:yyyy}",
                                    ts.Date.WeekNumber(),
                                    ts.Date.FirstDayOfWeek(),
                                    ts.Date.FirstDayOfWeek().DateByWeekDayNumber(7)),
                            SpentTime =
                                new TimeSpan(
                                persistedMyTimeRecords.ToList().Where(
                                    tr =>
                                    tr.StartDate >= ts.Date.FirstDayOfWeek() &&
                                    tr.StartDate <= ts.Date.FirstDayOfWeek().DateByWeekDayNumber(7)).Sum(
                                        tr => tr.SpentTime.Value.Ticks)),
                            Date = ts.Date
                        }).OrderBy(ts => ts.Title);

            model.Task = assembler.CreateTaskModel(persistedTask);
            model.AssignedEmployees = persistedTask.AssignedEmployees.Select(assembler.CreateEmployeeModel);

            var notAssignedEmployees = allEmployees.Select(assembler.CreateEmployeeModel).Except(model.AssignedEmployees);

            model.NotAssignedEmployees = notAssignedEmployees;
            model.SpentTimeByMe = new TimeSpan(persistedMyTimeRecords.Sum(tr => tr.SpentTime.Value.Ticks));

            return model;
        }

        #endregion
    }
}