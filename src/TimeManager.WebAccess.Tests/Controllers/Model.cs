using System;
using System.Collections.Generic;
using System.Linq;

namespace Infocom.TimeManager.WebAccess.Tests.Controllers
{
    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Extensions;
    using Infocom.TimeManager.WebAccess.Infrastructure;
    using Infocom.TimeManager.WebAccess.Models;

    using Microsoft.Practices.ServiceLocation;

    using Moq;

    using NHibernate;

    public class Model
    {
        protected static ISession CurrentSession
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession();
            }
        }

        protected static ModelAssembler _assambler = new ModelAssembler();

        internal static List<Task> CreateTasks(Employee user, DateTime startDate)
        {
            var tasks = new List<Task>();
            var status = CreateTaskStatuses().First();
            for (int i = 0; i < 5; i++)
            {
                var newTask = new Task();
                newTask.Description = "Some task";
                newTask.Name = "Task" + i;
                newTask.StartDate = startDate;
                newTask.FinishDate = startDate.AddMilliseconds(1);
                newTask.AssignedEmployees.Add(user); //associating task with user
                newTask.CurrentStatus = status;
                tasks.Add(newTask);
            }

            using (var tx = CurrentSession.BeginTransaction())
            {
                tasks.ForEach(task => CurrentSession.Save(task));

                tx.Commit();
            }
            return tasks;
        }

        internal static List<Task> CreateTasks(Employee user, DateTime startDate,Project project)
        {
            var tasks = CreateTasks(user, startDate);
            tasks.ForEach(t => t.Project = project);
            
            using (var tx = CurrentSession.BeginTransaction())
            {
                tasks.ForEach(task => CurrentSession.Update(task));

                tx.Commit();
            }
            return tasks;
        }
       
        internal static Employee CreateUser(Department department)
        {
            var user = new Employee();
            user.FirstName = "Alexey";
            user.LastName = "Mukas";
            user.PatronymicName = "Petrovich";
            user.Login = "amukas";
            user.Department = department;
            using (var tx = CurrentSession.BeginTransaction())
            {
                CurrentSession.Save(user);
                tx.Commit();
            }
            return user;
        }

        internal static Department CreateDepartment()
        {
            var department = new Department();
            department.Name = "DIT";
            department.ShortName = "DIT";
            using (var tx = CurrentSession.BeginTransaction())
            {
                CurrentSession.Save(department);
                tx.Commit();
            }
            return department;
        }

        internal static  IEnumerable<StatusModel> CreateTaskStatusesModel()
        {
            return CreateTaskStatuses().Select(_assambler.CreateStatusModel);
        }

        internal static IEnumerable<Status> CreateTaskStatuses()
        {
            var statuses = new List<Status>();
            statuses.Add(new Status { ApplicableTo = DomailEntityType.Task, Name = "Новая" });
            statuses.Add(new Status { ApplicableTo = DomailEntityType.Task, Name = "Открытая" });
            statuses.Add(new Status { ApplicableTo = DomailEntityType.Task, Name = "Закрытая" });

            using (var tx = CurrentSession.BeginTransaction())
            {
                statuses.ForEach(s => CurrentSession.Save(s));
                tx.Commit();
            }
            return statuses;
        }

        internal static Task CreateNewTask(Employee user, DateTime date)
        {
            var task = new Task();
            task.Description = "Some task";
            task.Name = "TaskForAdd";
            task.StartDate = date;
            task.AssignedEmployees.Add(user); //associating task with user

            using (var tx = CurrentSession.BeginTransaction())
            {
                CurrentSession.Save(task);
                tx.Commit();
            }
            return task;
        }

        internal static Project CreateProject(Employee user, DateTime date)
        {
            var project = new Project();
            var timeSheet = Model.CreateTimeSheet(user);
            project.Name = "TestProject";
            project.ProjectType = CreateProjectTypes().First();
            project.Code = "TestCode";
            CreateTasks(user, date).ForEach(project.Tasks.Add);
            project.Tasks.ToList().ForEach(t => t.TimeSheets.Add(timeSheet));
            project.Tasks.ToList().ForEach(t => t.TimeRecords.Add(new TimeRecord{Employee = user,SpentTime = new TimeSpan(8,0,0),StartDate = date,Task = t}));
            project.CurrentStatus = CreateProjectStatuses().First();


            using (var tx = CurrentSession.BeginTransaction())
            {
                CurrentSession.Save(project);

                tx.Commit();
            }

            return project;
        }

        internal static IEnumerable<Status> CreateProjectStatuses()
        {
            var statuses = new List<Status>();
            statuses.Add(new Status { ApplicableTo = DomailEntityType.Project, Name = "Новый" });
            statuses.Add(new Status { ApplicableTo = DomailEntityType.Project, Name = "Открытый" });
            statuses.Add(new Status { ApplicableTo = DomailEntityType.Project, Name = "Закрытый" });
            using (var tx = CurrentSession.BeginTransaction())
            {
                statuses.ForEach(status => CurrentSession.Save(status));

                tx.Commit();
            }
            return statuses;
        }

        internal static IEnumerable<ProjectType> CreateProjectTypes()
        {
            var types = new List<ProjectType>();
            types.Add(new ProjectType { Name = "Проекты" });
            types.Add(new ProjectType { Name = "ТКП" });
            using (var tx = CurrentSession.BeginTransaction())
            {
                types.ForEach(type => CurrentSession.Save(type));

                tx.Commit();
            }
            return types;
        }

        internal static TimeSheet CreateTimeSheet(Employee employee)
        {
            var result = new TimeSheet();
            var date = DateTime.Now;

            var tasksWithRecords = CreateTasksWithRecords(employee, date);
            result.Date = date.FirstDayOfWeek();
            result.Employee = employee;
            tasksWithRecords.ToList().ForEach(t => result.Tasks.Add(t));
            result.Type = TimeSheetType.Weekly;

            using (var tx = CurrentSession.BeginTransaction())
            {
                CurrentSession.Save(result);

                tx.Commit();
            }

            return result;
        }

        internal static TimeSheetModel CreateTimeSheetModel(Employee employee)
        {
            var result = new TimeSheetModel();
            var date = DateTime.Now;

            var tasksWithRecords = CreateTasksWithRecords(employee, date);

            result.Date = date;
            result.Employee = _assambler.CreateEmployeeModel(employee);
            tasksWithRecords.ToList().ForEach(task => result.Tasks.Add(_assambler.CreateTaskModel(task)));
            result.Type = TimeSheetType.Weekly;
            return result;
        }

        internal static IEnumerable<Task> CreateTasksWithRecords(Employee employee, DateTime date)
        {
            var tasks = CreateTasks(employee, date);
            var resultTasks = tasks;
            foreach (var task in tasks)
            {
                var currentDate = DateTime.Now;
                var lastWeekTimeRecord = new TimeRecord
                {
                    Employee = employee,
                    SpentTime = new TimeSpan(8, 0, 0),
                    StartDate = currentDate.AddDays(-7),
                    Task = task
                };
                var currentWeekTimeRecord = new TimeRecord
                {
                    Employee = employee,
                    SpentTime = new TimeSpan(8, 0, 0),
                    StartDate = currentDate,
                    Task = task
                };
                var nextWeekTimeRecord = new TimeRecord
                {
                    Employee = employee,
                    SpentTime = new TimeSpan(8, 0, 0),
                    StartDate = currentDate.AddDays(7),
                    Task = task
                };

                task.TimeRecords.Add(currentWeekTimeRecord);
                task.TimeRecords.Add(lastWeekTimeRecord);
                task.TimeRecords.Add(nextWeekTimeRecord);
            }
            return resultTasks;
        }

        internal static Request CreateRequest()
        {
            var request = new Request();
            using (var tx = CurrentSession.BeginTransaction())
            {
                var department = CreateDepartment();
                var manager = new Employee
                    {
                        Department = department,
                        FirstName = "Andrii",
                        LastName = "Sokoliuk",
                        PatronymicName = "Sergeevich",
                        Login = "dron",
                    };

                var dept1 = new Department
                {
                    Name = "DIT",
                    ShortName = "DIT",
                };

                var dept2 = new Department
                {
                    Name = "PKD",
                    ShortName = "PKD",
                };

                CurrentSession.SaveOrUpdate(dept1);
                CurrentSession.SaveOrUpdate(dept2);

                var phase = new Phase();
                phase.DeadLine = new DateTime(2011, 1, 1);
                phase.Name = "TestPhase";

                var allocation1 = new ProjectBudgetAllocation();
                allocation1.AmountOfHours = 10;
                allocation1.AmountOfMoney = 1000;
                allocation1.Department = dept1;

                var allocation2 = new ProjectBudgetAllocation();
                allocation2.AmountOfHours = 99;
                allocation2.AmountOfMoney = 99;
                allocation2.Department = dept2;

                phase.Budget.BudgetAllocations.Add(allocation1);
                phase.Budget.BudgetAllocations.Add(allocation2);

                CurrentSession.SaveOrUpdate(phase);



                request.Contract = new Contract
                {
                    Customer = new Customer { Name = "TestCustomer" },
                    Name = "TestContract",
                    Number = "12",
                    SigningDate = new DateTime(2011, 1, 1)
                };
                request.Date = new DateTime(2011, 5, 5);
                request.Number = "123";
                request.ProjectManager = manager;
                request.Phases.Add(phase);
                request.Type = new RequestType { Description = "Project", Name = "Project" };
                CurrentSession.SaveOrUpdate(request);
                tx.Commit();
            }

            return request;
        }

        internal static IEnumerable<TaskModel> CreateTasksModelWithRecords(Employee employee, DateTime date)
        {
            var resultTasks = CreateTasksWithRecords(employee, date).Select(_assambler.CreateTaskModel);

            return resultTasks;
        }

        internal static RequestType CreateRequestType()
        {
            var requestType = new RequestType();
            requestType.Name = "Проект";
            requestType.Description = "Проект";

            using (var tx = CurrentSession.BeginTransaction())
            {
                CurrentSession.Save(requestType);
                tx.Commit();
            }

            return requestType;
        }
    }
}
