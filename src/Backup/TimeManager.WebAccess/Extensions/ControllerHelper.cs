namespace Infocom.TimeManager.WebAccess.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using NHibernate.Linq;
    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.Core.Services;
    using Infocom.TimeManager.WebAccess.ViewModels;
    using Infocom.TimeManager.WebAccess.Infrastructure;

    public class ControllerHelper
    {
        #region Public Methods

        public static string DateToString(DateTime dateTime)
        {
            return string.Format("{0:dd.MM.yyyy}", dateTime);
        }

        public static DateTime GetCorrectDate(string date)
        {
            DateTime result = DateTime.Now;
            if (date.Contains("/"))
            {
                var dateArray = date.Substring(0, 10).Split('/');
                var tempDate = new DateTime(int.Parse(dateArray[2]), int.Parse(dateArray[0]), int.Parse(dateArray[1]));
                result = tempDate;
            }
            else
            {
                result = DateTime.Parse(date);
            }

            return result;
        }

        public static DateTime GetLastDayOfMonth(DateTime fistDayOfWeek)
        {
            return new DateTime(
                fistDayOfWeek.Year, fistDayOfWeek.Month, GetFirstDayOfMonth(fistDayOfWeek).AddMonths(1).AddDays(-1).Day);
        }

        public static DateTime GetFirstDayOfMonth(DateTime fistDayOfWeek)
        {
            return new DateTime(fistDayOfWeek.Year, fistDayOfWeek.Month, 1);
        }

       

        public static int WeekNumber(DateTime date)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Returns the first day of the week that the specified date
        /// is in.
        /// </summary>
        /// <returns>
        /// The get first day of week.
        /// </returns>
        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            DayOfWeek firstDay = defaultCultureInfo.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
            {
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }

            return firstDayInWeek;
        }

        public static string GetCombinatedRequestName(long requestId)
        {
            var combinatedName= string.Empty;
            using (var tx = NHibernateContext.Current().Session.BeginTransaction())
            {
                var phase = NHibernateContext.Current().Session.QueryOver<Phase>()
                    .Where(ph=>ph.Request.ID == requestId).Future().First();
                combinatedName = string.Format("№ {0} от {1:dd.MM.yy}, {2}, {3}",phase.Request.Number
                    ,phase.Request.Date,phase.Request.Contract.Customer.Name,phase.Name);
            }
            return combinatedName;
        }

        public static ProjectModel GetProjectModel(long requestId)
        {
            var modelAssembler = new ModelAssembler();
            var newProjectModel = new ProjectModel();
            using (var tx = NHibernateContext.Current().Session.BeginTransaction())
            {
                var request = NHibernateContext.Current().Session.Get<Request>(requestId);
                var phase = NHibernateContext.Current().Session.Get<Request>(requestId)
                            .Phases.FirstOrDefault();
                if (phase != null)
	            {
                    newProjectModel.Name = string.Format("{0}: {1}", phase.Name, phase.Request.Contract.Name);
	            }
                    
                newProjectModel.Code = string.Format("{0} {1}", request.Type.Description, request.Number.Substring(0, 7));
                long projectTypeId = request.Type.ID;

                if (projectTypeId == 4)
                {
                    projectTypeId = 2;
                }

                newProjectModel.ProjectTypeID = projectTypeId;
                newProjectModel.PhaseID = request.Phases.First().ID;
                newProjectModel.CurrentStatusID = 131073; //this.PersistenceSession.Get<Status>((long)131073);
                newProjectModel.RequestID = requestId;
                newProjectModel.Request = modelAssembler.CreateRequestModel(request);  //тут дописала
            }
            return newProjectModel;
        }

        public static ProjectEditorViewModel GetProjectEditorViewModel()
        {
            var modelAssembler = new ModelAssembler();
            var model = new ProjectEditorViewModel();
            using (var tx = NHibernateContext.Current().Session.BeginTransaction())
            {
                // Department 'КД' has ID = 1
                var departmentID = 1;

                var employees = NHibernateContext.Current().Session.QueryOver<Employee>().Future();
                var department = NHibernateContext.Current().Session.QueryOver<Department>().Future();
                var status = NHibernateContext.Current().Session.QueryOver<Status>().Future();
                var types = NHibernateContext.Current().Session.QueryOver<ProjectType>().Future();
                var allRequests = NHibernateContext.Current().Session.QueryOver<Request>().Future();
                var phases = NHibernateContext.Current().Session.QueryOver<Phase>().Future();
                var requestsWithAssignedProjects = NHibernateContext.Current().Session.QueryOver<Project>()
                        .Future().Select(p => p.Phase.Request);
                var newRequests = allRequests.Except(requestsWithAssignedProjects);
                var viewStatuses = new List<StatusModel>();
                var statuses = new List<StatusModel>();
                viewStatuses.Add(new StatusModel
                {
                    ID = 0,
                    ApplicableTo = DomailEntityType.Project,
                    Name = "Все"
                });
                viewStatuses.Add(new StatusModel
                {
                    ID = 4,
                    ApplicableTo = DomailEntityType.Project,
                    Name = "Новые и Открытые"
                });
                status.Select(modelAssembler.CreateStatusModel)
                    .Where(s => s.ApplicableTo == DomailEntityType.Project)
                    .ForEach(viewStatuses.Add);

                status.Select(modelAssembler.CreateStatusModel)
                     .Where(s => s.ApplicableTo == DomailEntityType.Project)
                     .ForEach(statuses.Add);
           
               model = new ProjectEditorViewModel
                {
                    AllEmployees =
                        employees.Select(
                            modelAssembler.CreateEmployeeModel).OrderBy(e => e.ShortName),
                    BudgetAllocations =
                        department.Select(
                            d => new BudgetAllocationModel { Department = modelAssembler.CreateDepartment(d) }).ToList(),
                    ProjectManagers =
                        employees.Where(e => e.Department.ID == departmentID).Select(modelAssembler.CreateEmployeeModel)
                        .OrderBy(e => e.ShortName),
                    Statuses = statuses,
                    ViewStatuses = viewStatuses,
                    Types = types.Select(modelAssembler.CreateProjectTypeModel),
                    NewRequests = newRequests.Select(modelAssembler.CreateRequestShortModel),
                    AllRequests = allRequests.Select(modelAssembler.CreateRequestShortModel),
                    Phases = phases.Select(modelAssembler.CreatePhaseShortModel),
                };
             }
            return model;
                
        }

        public static RequestModel GetRequestOverviewModel(long requestId)
        {
            var modelAssembler = new ModelAssembler();
            var result = new RequestModel();
            using (var tx = NHibernateContext.Current().Session.BeginTransaction())
            {
                var request = NHibernateContext.Current().Session.Get<Request>(requestId);
                if (request != null)
                {
                    result = modelAssembler.CreateRequestModel(request);
                }
            }
            return result;
        }

        //добавленное
        public static ContractModel GetContractModel(long contractId)
        {
            var modelAssembler = new ModelAssembler();
            var result = new ContractModel();
            using (var tx = NHibernateContext.Current().Session.BeginTransaction())
            {
                var contract = NHibernateContext.Current().Session.Get<Contract>(contractId);
                if (contract != null)
                {
                    result = modelAssembler.CreateContractModel(contract);
                }
            }
            return result;
        }
        #endregion

        public static RequestDetailTypesViewModel GetRequestDetailsViewModel(long requestId)
        {
            var modelAssembler = new ModelAssembler();
            var result = new RequestDetailTypesViewModel();
            using (var tx = NHibernateContext.Current().Session.BeginTransaction())
            {
                var request = NHibernateContext.Current().Session.Get<Request>(requestId);
                foreach (var item in request.RequestDetails)
	            {
		           result.RequestDetailTypes.Add(new RequestDetailModel{RequestDetailTypeID=item.ID,Checked=item.Checked});
	            }   
            }
            return result;
        }

        public static BudgetViewModel GetBudgetModelBy(long departmentId, long projectId)
        {
            BudgetViewModel result = new BudgetViewModel();
            using (var tx = NHibernateContext.Current().Session.BeginTransaction())
            {
               // var request = NHibernateContext.Current().Session.Get<Request>(requestId);
                var budgetResource = NHibernateContext.Current()
                    .Session.QueryOver<vwGetBudgetBy>()
                    .Where(t => t.ProjectId == projectId && t.DepartmentId == departmentId)
                    .Future().FirstOrDefault();
                if (budgetResource!=null)
	            {
		            result.SpentTime = budgetResource.SpentTime;
                    result.SpentMoney = budgetResource.SpentMoney;
	            }
                
            }
            return result;
        }

    }
}