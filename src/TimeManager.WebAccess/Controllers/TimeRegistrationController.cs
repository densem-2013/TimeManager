namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Telerik.Web.Mvc.Extensions;

    using Castle.Components.DictionaryAdapter;
    using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Extensions;
    using Infocom.TimeManager.WebAccess.Infrastructure;
    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;

    using NHibernate.Linq;

    using System.Linq;
    using System.Security.Principal;

    [HandleError]
    public partial class TimeRegistrationController : TimeManagerBaseController
    {
        //todo: there is a problem with selectedDate converting... possible reason - Globalization. To solve this issue I've used string type instead 
        public virtual ActionResult Index(string selectedDate)
        {
            var parsedDateTime = DateTime.Parse(selectedDate ?? DateTime.Now.Date.ToString());

            if (Session["SelectedDate"] == null || selectedDate != null)
            {
                Session["SelectedDate"] = parsedDateTime;
            }

            return this.ViewIndex();
        }

        internal ActionResult ViewIndex()
        {
            //IIdentity WinId = System.Web.HttpContext.Current.User.Identity;
            //WindowsIdentity wi = (WindowsIdentity)WinId;
            //System.Security.Principal.WindowsPrincipal p = System.Threading.Thread.CurrentPrincipal as System.Security.Principal.WindowsPrincipal;

            //string strName = p.Identity.Name;
            if ((bool)this.Session["RequestPermission"])
            {
                
                return this.RedirectToAction("Index", "Requests");
            }
            var firstDayOfWeek = ((DateTime)Session["SelectedDate"]).FirstDayOfWeek();
            var model = this.CreateModel(firstDayOfWeek);
            this.InitializeViewData(firstDayOfWeek);
            return this.IndexView(model);
        }

        private ActionResult IndexView(TimeRegistrationViewModel model)
        { 
            return this.View("Index", model);
        }

        internal TimeRegistrationViewModel CreateModel(DateTime date)
        {
            var model = new TimeRegistrationViewModel();
            var timeSheet = !this.IsTimeSheetExists(date) ? this.CreateDefaultTimesheetForCurrentEmployee(date) : this.GetTimeSheetForCurrentEmployee(date);
            InfocomServer server;

         
            server = InfocomServer.SrvOm;
           

            //my
            var closedTasks = timeSheet.Tasks.Where(t => t.CurrentStatus.ID == 131075);
            foreach (Task task in closedTasks.ToArray())
            {
                if (!(task.TimeRecords.Where(t => t.Employee == CurrentEmployee.Value && t.SpentTime.Value.TotalHours >= 0 && t.StartDate >= date && t.StartDate < date.AddDays(7)).Any()))
                {
                    timeSheet.Tasks.Remove(task);
                }
                    
             }
            //my


            model.SelectedDate = date;
            var assembler = new ModelAssembler();
            model.TimeSheet = assembler.CreateTimeSheetModel(timeSheet,this.CurrentEmployee.Value.ID);
            model.RealSpentTime = InfocomTimeSheet.GetSpentTimeOfWeekByDay(server,CurrentEmployee.Value.Login,date);
            model.TotalSpentTimeInMonth = GetTotalSpentTimeInMonth(date);
            model.TotalSpentTimeInMonthFromTimeSheet = InfocomTimeSheet.GetSpentTimeOfMonth(server,CurrentEmployee.Value.Login, date);
            DeleteExtraTimeRecords(date, model);

            return model;
        }

       
        internal TimeSpan GetTotalSpentTimeInMonth(DateTime date)
        {
           
            var fromDate = date.FirstDayOfMonth();
            var toDate = date.LastDayOfMonth().AddDays(1).AddMilliseconds(-1);
            var employee = this.CurrentEmployee.Value;
            //note: for current implement of NH need to use .ToArray
            return new TimeSpan(
                this.PersistenceSession.Query<TimeRecord>().Where(
                    tr =>
                    tr.StartDate >= fromDate &&
                    tr.StartDate < toDate &&
                    tr.Employee == employee &&
                    tr.SpentTime != null).ToArray().Sum(tr => tr.SpentTime.Value.Ticks));
        }

        internal void DeleteExtraTimeRecords(DateTime date, TimeRegistrationViewModel model)
        {
            var fistDayOfWeek = date.FirstDayOfWeek();
            foreach (var task in model.TimeSheet.Tasks)
            {
                var timeRecords = task.TimeRecords.Where(
                    tr =>
                    tr.EmployeeID == this.CurrentEmployee.Value.ID
                    &&
                    !(tr.StartDate >= fistDayOfWeek
                    && 
                    tr.StartDate < fistDayOfWeek.AddDays(7))
                    );

                if (timeRecords.Any())
                {
                    timeRecords.ToArray().ForEach(tr => task.TimeRecords.Remove(tr));
                }

            }
        }

       
        private bool IsTimeSheetExists(DateTime date)
        {
            return this.GetTimeSheetForCurrentEmployee(date) != null;
        }

        public TimeSheet CreateDefaultTimesheetForCurrentEmployee(DateTime date)
        {
            var newTimeSheet = new TimeSheet();

            using (var tx = this.PersistenceSession.BeginTransaction())
            {
                var currentEmployee = this.CurrentEmployee.Value;
                var fromDate = date.FirstDayOfWeek();

                // by the default we add tasks which are 
                // - not closed for the beginning of the week
                // - started at least on this week (or earlier)
                var tasksForWeek = this.GetNotFinishedTasksForWeek(fromDate, currentEmployee).Where(t => t.CurrentStatus.ID == 131072 || t.CurrentStatus.ID == 131071);

                newTimeSheet.Employee = currentEmployee;
                tasksForWeek.OrderBy(ts=>ts.Project.Name).ForEach(newTimeSheet.Tasks.Add);
                newTimeSheet.Type = TimeSheetType.Weekly;
                newTimeSheet.Date = date;

                this.PersistenceSession.Save(newTimeSheet);

                tx.Commit();
            }

            return newTimeSheet;
        }

        // todo: refactor this method with "Extract method",
        // see: http://fabiomaulo.blogspot.com/2010/07/nhibernate-linq-provider-extension.html
        // for details on how to register LINQ extensions
        public IEnumerable<Task> GetTasksForWeek(DateTime startDate, Employee user)
        {
            var endDate = startDate.AddDays(7).AddMilliseconds(-1);
            return
                this.AllTastsForEmployee(user).Where(
                    t =>
                    (t.StartDate == null || (t.StartDate >= startDate && t.StartDate < endDate))); 
        }

        // todo: refactor this method with "Extract method",
        // see: http://fabiomaulo.blogspot.com/2010/07/nhibernate-linq-provider-extension.html
        // for details on how to register LINQ extensions
        public IEnumerable<Task> GetNotFinishedTasksForWeek(DateTime startDate, Employee user)
        {
            var endDate = startDate.AddDays(7).AddMilliseconds(-1);
            return
                this.AllTastsForEmployee(user).Where(
                    t =>
                    (t.StartDate == null || t.StartDate < endDate) &&
                    (t.FinishDate == null || (t.FinishDate >= startDate && t.FinishDate < endDate)));
        }

        private IQueryable<Task> AllTastsForEmployee(Employee employee)
        {
            return this.PersistenceSession.Query<Task>().Where(t => t.AssignedEmployees.Contains(employee) && (t.CurrentStatus.ID == 131072 || t.CurrentStatus.ID == 131071)); //!!!

        }

        internal TimeSheet GetTimeSheetForCurrentEmployee(DateTime date)
        {
            
            var firstDayOfWeek = date.FirstDayOfWeek();
            return
                this.PersistenceSession.Query<TimeSheet>().Where(
                    ts =>
                    ts.Date == firstDayOfWeek
                    && ts.Type == TimeSheetType.Weekly
                    && ts.Employee == this.CurrentEmployee.Value).
                    SingleOrDefault();


        }

        internal void InitializeViewData(DateTime selectedDate)
        {
            var fistDayOfWeek = selectedDate.FirstDayOfWeek();
            this.ViewBag.RegistrationDate = fistDayOfWeek;
            // for every day in week
            var daysToEdit = new List<DateTime>();
            for (int i = 0; i < 7; i++)
            {
                daysToEdit.Add(fistDayOfWeek.AddDays(i));
            }

            this.ViewBag.DaysToEdit = daysToEdit;

            var assembler = new ModelAssembler();

            var taskStatuses = this.PersistenceSession.QueryOver<Status>()
                    .Where(s => s.ApplicableTo == DomailEntityType.Task)
                    .Future()
                    .Select(assembler.CreateStatusModel);

            this.ViewBag.TaskStatuses = taskStatuses;
            /* not used ???
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

            ViewBag.WorkingWeeks = timeSheetOverviews;
            */
            if (Session["EditErrorMessage"] != null)
            {
                this.ViewBag.OverviewErrorMessage = Session["EditErrorMessage"].ToString();
                Session["EditErrorMessage"] = null;
            } 
        }

        [HttpPost]
        public virtual ActionResult TimeSheetAddTask(long timeSheetID,  TaskModel model, DateTime selectedDate, long taskID=0)
        {
            using (var tx = this.PersistenceSession.BeginTransaction())
            {
                if (taskID==0)
                {
                    return RedirectToAction("Index");
                }
                var taskToAdd = this.PersistenceSession.Get<Task>(taskID);
                if (taskToAdd == null)
                {
                    this.CreateOverviewFromEditErrorMessage(string.Format("Задача c ID = {0} не найдена", taskID));
                    return RedirectToAction("Index");
                }

                var timeSheetToUpdate = this.PersistenceSession.Get<TimeSheet>(timeSheetID);
                if (timeSheetToUpdate == null)
                {
                    this.CreateOverviewFromEditErrorMessage(string.Format("Расписание c ID = {0} не найдено", timeSheetToUpdate));
                    return RedirectToAction("Index");
                }

                if (timeSheetToUpdate.Tasks.Contains(taskToAdd))
                {
                    this.CreateOverviewFromEditErrorMessage("Задача существует в расписании");
                   return RedirectToAction("Index");
                }

                timeSheetToUpdate.Tasks.Add(taskToAdd);
                this.PersistenceSession.Update(timeSheetToUpdate);

                tx.Commit();

                return RedirectToIndex(selectedDate);
            }
        }

        private void CreateOverviewFromEditErrorMessage(string message)
        {
            this.Session["EditErrorMessage"] = message;
        }

        public virtual ActionResult GetProjectForEmployee()
        {
            var projects = this.PersistenceSession.Query<Project>().Where(
                         p => (p.CurrentStatus.ID == 131070 || p.CurrentStatus.ID == 131073) && p.Tasks.Where(t => t.AssignedEmployees.Contains(this.CurrentEmployee.Value)).Any()).OrderByDescending(o => o.ID).ToFuture();
            var result = new List<NamedItem>();
            projects.ForEach(p => result.Add(new NamedItem { ID = p.ID, Name = string.Format("{0} {1}",p.Code,p.Name) }));
            return new JsonResult { Data = new SelectList(result, "ID", "Name") };
        }

        public virtual ActionResult GetTasksForProject(long projectID)
        {
            var tasks =
                this.PersistenceSession.Query<Task>().Where(
                    t => (t.CurrentStatus.ID == 131071 || t.CurrentStatus.ID == 131072) && t.Project.ID == projectID
                        && t.AssignedEmployees.Contains(this.CurrentEmployee.Value)).OrderByDescending(o => o.ID).ToFuture();
            var result = new List<NamedItem>();
            tasks.ForEach(t => result.Add(new NamedItem { ID = t.ID, Name = t.Name }));
            return new JsonResult { Data = new SelectList(result, "ID", "Name") };
        }

        [HttpPost]
       public virtual ActionResult TimeSheetDeleteTaskssss(long timeSheetID/*, long ID*/, DateTime selectedDate)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {

                    var timeSheetToUpdate = this.PersistenceSession.Get<TimeSheet>(timeSheetID);
                    if (timeSheetToUpdate == null)
                    {
                        this.CreateOverviewFromEditErrorMessage(string.Format("Расписание c ID = {0} не найдено", timeSheetID));
                        return RedirectToAction("Index");
                        //this.View();
                    }

                    var someTasks = timeSheetToUpdate.Tasks;

                    foreach (var taskToDelete in someTasks.ToArray())
                    {
                        var employee = CurrentEmployee.Value;
                        var fromDate = selectedDate.Date;
                        var toDate = selectedDate.Date.AddDays(7).AddMilliseconds(-1);
                        taskToDelete.TimeRecords.Where(
                            tr => tr.Employee == employee
                                && tr.StartDate >= fromDate
                                && tr.StartDate < toDate)
                            .DeleteIn(PersistenceSession, taskToDelete.TimeRecords);

                        timeSheetToUpdate.Tasks.Remove(taskToDelete);
                        this.PersistenceSession.Update(timeSheetToUpdate);

                    }

                    tx.Commit();

                    return RedirectToIndex(selectedDate);
                      
                    
                }
            }
            return ViewIndex();
        }
                
        [HttpPost]
        public virtual ActionResult TimeSheetDeleteTask(long timeSheetID, long ID,  DateTime selectedDate)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    var timeSheetToUpdate = this.PersistenceSession.Get<TimeSheet>(timeSheetID);
                    if (timeSheetToUpdate == null)
                    {
                        this.CreateOverviewFromEditErrorMessage(string.Format("Расписание c ID = {0} не найдено", timeSheetID));
                        return RedirectToAction("Index");
                        //this.View();
                    }

                   
                    var taskToDelete = this.PersistenceSession.Get<Task>(ID);
                    if (taskToDelete == null)
                    {
                        this.CreateOverviewFromEditErrorMessage(string.Format("Задача c ID = {0} не найдена", ID));
                        return RedirectToAction("Index");
                    }

                    var employee = CurrentEmployee.Value;
                    var fromDate = selectedDate.Date;
                    var toDate = selectedDate.Date.AddDays(7).AddMilliseconds(-1);
                    taskToDelete.TimeRecords.Where(
                        tr => tr.Employee == employee
                            && tr.StartDate >= fromDate
                            && tr.StartDate < toDate)
                        .DeleteIn(PersistenceSession, taskToDelete.TimeRecords);

                    timeSheetToUpdate.Tasks.Remove(taskToDelete);
                    this.PersistenceSession.Update(timeSheetToUpdate);

                    tx.Commit();

                    return RedirectToIndex(selectedDate);
                }
            }

            return ViewIndex();
        }

        [HttpPost]
        public virtual ActionResult TimeSheetUpdateTask(long taskID, DateTime selectedDate, IEnumerable<TimeSpan> timeSpantByDays)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    var employee = CurrentEmployee.Value;
                    var taskToUpdate = PersistenceSession.Get<Task>(taskID);
                    if (taskToUpdate == null)
                    {
                        this.CreateOverviewFromEditErrorMessage(string.Format("Задача c ID = {0} не найдена", taskID));
                        return RedirectToAction("Index");
                    }

                    if (timeSpantByDays == null)
                    {
                        this.CreateOverviewFromEditErrorMessage(string.Format("Значение затраченного времени не инициализированно"));
                        return RedirectToAction("Index");
                    }

                    var timeRecordsForEmployee =
                        taskToUpdate.TimeRecords.Where(
                                record => record.Employee == employee && record.StartDate.Value.Date >= selectedDate
                                && record.StartDate.Value.Date<= selectedDate.AddDays(timeSpantByDays.Count()-1));

                    for (int i = 0; i < timeSpantByDays.Count(); i++)
                    {
                        var dayToUpdate = selectedDate.AddDays(i);
                        
                        var timeRecordsForUpdate =
                            timeRecordsForEmployee.Where(record => record.StartDate.Value.Date == dayToUpdate);
                        
                        if (timeRecordsForUpdate.Count() > 0)
                        {
                            timeRecordsForUpdate.First().SpentTime = timeSpantByDays.ElementAt(i);
                            timeRecordsForUpdate
                                .Where(tr=>tr.ID!=timeRecordsForUpdate.First().ID)
                                .ForEach(tr=>tr.SpentTime=new TimeSpan(0));
                        }
                        else
                        {
                            taskToUpdate.TimeRecords.Add(
                                new TimeRecord
                                    {
                                        Employee = employee,
                                        StartDate = dayToUpdate,
                                        SpentTime = timeSpantByDays.ElementAt(i),
                                        Task = taskToUpdate
                                    });
                        }
                    }

                    // if task status is 'New' then change status to Open
                    if (taskToUpdate.CurrentStatus.ID == 131071)
                    {
                        taskToUpdate.CurrentStatus = PersistenceSession.Get<Status>(Int64.Parse("131072"));
                    }

                    this.PersistenceSession.Update(taskToUpdate);

                    tx.Commit();
                    RouteValueDictionary routeValues = this.GridRouteValues();
                    routeValues.Add("selectedDate", selectedDate.ToString());
                    // add the editing mode to the route values

                    return RedirectToAction("Index", routeValues);
                    //return RedirectToIndex(selectedDate);
                }
            }

            return this.Index(selectedDate.ToString());
        }

        private ActionResult RedirectToIndex(DateTime selectedDate)
        {
            return this.RedirectToAction("Index", "TimeRegistration", new { selectedDate = selectedDate.ToString() });
        }
    }
}