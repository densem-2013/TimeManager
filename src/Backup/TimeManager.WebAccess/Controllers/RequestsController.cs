using System.Globalization;

namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Filters;
    using Infocom.TimeManager.WebAccess.Infrastructure;
    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;

    using Telerik.Web.Mvc.Extensions;
    using NHibernate;
    using NHibernate.Criterion;
    using System.Web;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Net;
    using System.Threading;
    using System.Web.Security;
    using System.Text;

    [HandleError]
    public partial class RequestsController : TimeManagerBaseController
    {
        private readonly string _requestsFilterKey = "RequestsFilter";


        #region Public Methods

        public virtual ActionResult ViewReports()
        {
            var redirectUrl = string.Format("/Requests");
            return Redirect(redirectUrl);
        }


        [HttpPost]
        public virtual ActionResult Index(long filteringByRequestTypeID, string filteringBySearchWord, long filteringByStatusID)
        {
            var filter = new RequestsFilter();
            filter.FilteringByRequestTypeID = filteringByRequestTypeID;
            filter.FilteringBySearchWord = filteringBySearchWord;
            filter.FilteringByStatusID = filteringByStatusID;
            Session[this._requestsFilterKey] = filter;

            var model = this.CreateModel();
            this.InitializeViewData();
            var modelAssembler = new ModelAssembler();
            ViewData["Departments"] = new SelectList(PersistenceSession.QueryOver<Department>().Future(), "ID", "ShortName");
            var view = View(model);
            return view;
        }

        public virtual ActionResult Index()
        {
            var model = this.CreateModel();

            this.InitializeViewData();
            ViewData["Departments"] = new SelectList(PersistenceSession.QueryOver<Department>().Future(), "ID", "ShortName");
            return View(model);
        }

        internal void InitializeViewData()
        {
            var modelAssembler = new ModelAssembler();
            var requestEditorModel = new RequestEditorViewModel();
            var requestTypes = this.PersistenceSession.QueryOver<RequestType>().Future();
            var department = this.PersistenceSession.QueryOver<Department>().Future();
            var requestFilterTypes = new List<RequestTypeModel>();

            requestFilterTypes.Add(new RequestTypeModel() { Name = "Все" });
            requestFilterTypes.AddRange(requestTypes.Select(modelAssembler.CreateRequestTypeModel));

            var requestDetailTypes = this.PersistenceSession.QueryOver<RequestDetailType>().OrderBy(r => r.ID).Asc.Future();
            requestEditorModel.RequestDetailTypes = requestDetailTypes.Select(modelAssembler.CreateRequestDetailTypeModel);
            var managers = new List<EmployeeViewModel>();
            if (CurrentEmployee.Value.Department.ID == 1)
            {
                managers.Add(new EmployeeViewModel() { ID = CurrentEmployee.Value.ID, ShortName = CurrentEmployee.Value.ShortName });
                managers.AddRange(PersistenceSession.QueryOver<Employee>().Future()
                    .Where(e => e.Department.ID == 1 && e.ID != CurrentEmployee.Value.ID && e.FireDate == null)
                    .OrderBy(m => m.ShortName).Select(e => new EmployeeViewModel() { ID = e.ID, ShortName = e.ShortName }));
            }
            else
            {
                //managers.AddRange(PersistenceSession.QueryOver<Employee>().Future()
                //    .Where(e => e.Department.ID == 1 && e.FireDate == null)
                //    .OrderBy(m => m.ShortName).Select(e => new EmployeeViewModel() { ID = e.ID, ShortName = e.ShortName }));
                managers.AddRange(PersistenceSession.QueryOver<Employee>().Future()
                  .Where(e => e.FireDate == null)
                  .OrderBy(m => m.ShortName).Select(e => new EmployeeViewModel() { ID = e.ID, ShortName = e.ShortName }));
            }

            requestEditorModel.Managers = managers;
            requestEditorModel.RequestTypes = requestTypes./*Where(rt => rt.ID != 1).*/Select(modelAssembler.CreateRequestTypeModel); //этот кусок убирал тип "Административные работы" у заявок
            requestEditorModel.RequestNumber = CreateRequestNumber(requestEditorModel.RequestTypes.First().ID);
            requestEditorModel.Departments = department.Select(modelAssembler.CreateDepartmentModel);
            
            ViewBag.RequestEditorViewModel = requestEditorModel;
            if (this.Session != null)
            {
                if (this.Session["EditErrorMessage"] != null)
                {
                    this.ViewBag.OverviewErrorMessage = this.Session["EditErrorMessage"].ToString();
                    this.Session["EditErrorMessage"] = null;
                }
            }
        }

        private void CreateOrderEditFormErrorMessage(string message)
        {
            this.Session["EditErrorMessage"] = message;
        }

        internal RequestsViewModel CreateModel()
        {
            var filter = new RequestsFilter();

            if (this.Session != null)
            {
                if (Session[this._requestsFilterKey] != null)
                {
                    filter = (RequestsFilter)Session[this._requestsFilterKey];
                }
            }
            var persistedRequests = GetPersistedRequests(filter);

            var modelAssembler = new ModelAssembler();
            var model = new RequestsViewModel();
            
            var previewRequest = persistedRequests.OrderByDescending(r => r.ID).OrderBy(r => r.IsValidated).Select(modelAssembler.CreatevwRequestOverviewModel);

            if (CurrentEmployee.Value.Department.ID == 1)
            {
                var requests = new List<RequestOverviewModel>();
                var requestWithProcessState = previewRequest;

                var assignedRequests = requestWithProcessState.Where(r => r.ProjectManagerId == CurrentEmployee.Value.ID);
                var notAssignedRequests = requestWithProcessState.Where(r => r.ProjectManagerId != CurrentEmployee.Value.ID);
                requests.AddRange(assignedRequests);
                requests.AddRange(notAssignedRequests);
              
                model.Requests = GetRequestWithFiltringByStatus(requests, filter);
            }
            else
            {
                model.Requests = GetRequestWithFiltringByStatus(previewRequest, filter);
            }
            //foreach (var item in model.Requests)
            //{
            //    var contract = PersistenceSession.Get<Contract>(item.ContractId);
            //    if (contract!=null && contract.ContractType.ID>3)
            //    {
            //        item.isCustomersWithoutContracts = true;
            //    }
                
            //}

            return model;
        }

        private IEnumerable<vwRequest> GetPersistedRequests(RequestsFilter filter)
        {
            var result = new List<vwRequest>();
            var persistedRequests = this.PersistenceSession.QueryOver<vwRequest>().Future();
            // if the user use the filter, the '0' means that the filter is not applied 
            // '4' means "New and Opened" projects
            if (filter.FilteringByRequestTypeID != 0)
            {
                persistedRequests =
                    persistedRequests.Where(p => p.TypeId == filter.FilteringByRequestTypeID);
            }

            if (!string.IsNullOrEmpty(filter.FilteringBySearchWord))
            {
                persistedRequests =
                   persistedRequests.Where(r =>
                       r.Number.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower()) >= 0
                       ||
                       r.CustomerName.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower()) >= 0
                           //||
                           //r.ContractName.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower()) >= 0
                       ||
                       r.ProjectManagerShortName.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower()) >= 0
                       );
            }

            result.AddRange(persistedRequests);
            return result;
        }

        private IEnumerable<RequestOverviewModel> GetRequestWithFiltringByStatus(IEnumerable<RequestOverviewModel> requests, RequestsFilter filter)
        {
            var result = requests;

            // if the user use the filter, the '2' means that the filter is not applied 
            if (filter.FilteringByStatusID > 0)
                if (filter.FilteringByStatusID == 1 || filter.FilteringByStatusID == 2)
                {
                    result = result
                     .Where(r => r.StatusId == 131076 && r.ProcessStateId == filter.FilteringByStatusID).ToList();   //статусы процессов "Необработанный и В работе" может быть только у Открытых
                }

            if (filter.FilteringByStatusID == 3)
            {
                result = result
                    .Where(r => r.StatusId == 131076).ToList();  // Opened
            }

            if (filter.FilteringByStatusID == 4)
            {
                result = result
                    .Where(r => r.StatusId == 131078).ToList(); //Changed
            }
            if (filter.FilteringByStatusID == 5)
            {
                result = result
                    .Where(r => r.StatusId == 131077).ToList(); // Closed
            }
            return result;
        }


        public virtual ActionResult RequestStatusClose(string requestID) // закрытие заявки,проекта и задач +отправка письма с уведомлением
        {
            long result = 0;
            if (long.TryParse(requestID, out result))
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {

                    var requestCloseStatus = this.PersistenceSession.Get<Status>((long)131077);

                    var requestForUpdate = this.PersistenceSession.Get<Request>(long.Parse(requestID));


                    StringBuilder projectList = new StringBuilder();

                    if (requestForUpdate.ProjectManager.ID == CurrentEmployee.Value.ID
                   || ((bool)this.Session["ApprovePermission"])                                 // на случай эксикома сделать проверку на отдел
                   )
                    {

                        var projectCloseStatus = this.PersistenceSession.Get<Status>((long)131074);
                        var taskCloseStatus = this.PersistenceSession.Get<Status>((long)131075);


                        foreach (var phase in requestForUpdate.Phases)
                        {
                            foreach (var project in phase.Projects)
                            {
                                foreach (var task in project.Tasks)
                                {
                                    var taskForUpdate = this.PersistenceSession.Get<Task>(task.ID);
                                    taskForUpdate.CurrentStatus = taskCloseStatus;
                                    this.PersistenceSession.Update(taskForUpdate);

                                }
                                var projectForUpdate = this.PersistenceSession.Get<Project>(project.ID);
                                projectForUpdate.CurrentStatus = projectCloseStatus;
                                this.PersistenceSession.Update(projectForUpdate);

                                projectList.AppendFormat("{0}<br/>", projectForUpdate.Name);
                            }
                        }


                        requestForUpdate.Status = requestCloseStatus;
                        this.PersistenceSession.Update(requestForUpdate);
                        tx.Commit();

                        /*var message = string.Format("<b>Закрыта" + " заявка №</b> {0}."
                        + System.Environment.NewLine + "Заказчик: {1}"
                        + System.Environment.NewLine + "Руководитель проекта: {2} "
                      //  + System.Environment.NewLine + "{3}\n"
                        + System.Environment.NewLine + "Закрыты проекты {4} и закрыты все задачи по проекту."
                        + "Детали заявки находятся по адресу: http://dit.ia.ua:9010/RequestDetails/Index?RequestID={3} .",
                        requestForUpdate.Number, requestForUpdate.Contract.Customer.Name,
                        requestForUpdate.ProjectManager.ShortName, /*contractNamerequestForUpdate.Contract.Name* requestForUpdate.ID, projectList.ToString());*/
                        string mailTemplate = @"
                            <p>Уважаемые коллеги,закрыта заявка <b>№{0}</b></p>
                            <table>
                            <tr>
                            <td><b>Заказчик:</b></td><td>{1}</br></td>
                            </tr>
                            <tr>
                            <td><b>Руководитель проекта:</b></td><td>{2}</br> </td>
                            </tr>
                            <tr>
                            <td><b>Закрыт проект(и все задачи):</b></br></td><td>{4}</td>
                            </tr>
                            <tr>
                            <td><b>Детали заявки находятся по адресу:</b></td><td>http://timemanager.infocom-ltd.com/RequestDetails/Index?RequestID={3}</td>
                            </tr>
                            </table>
                            ";

                        string message = string.Format(mailTemplate, requestForUpdate.Number, requestForUpdate.Contract.Customer.Name,
                        requestForUpdate.ProjectManager.ShortName, requestForUpdate.ID, projectList.ToString());
                        var subject = string.Format("Закрыта" + " заявка № {0}.", requestForUpdate.Number);
                        var subscribers = new List<string>();

                        foreach (var budgetAllocation in requestForUpdate.Phases.ToList()[0].Budget.BudgetAllocations) // вот эта часть отвечает за рассылку на участвующие отделы
                        {
                            if (!string.IsNullOrEmpty(budgetAllocation.Department.Email))
                            {
                                subscribers.Add(budgetAllocation.Department.Email);
                            }
                            else
                            {

                                subscribers.Add("alina_alinichenko@dit.ia.ua");
                            }
                        }
                        this.WatchdogMailMessageHtml(subscribers, message, subject);
                    }
                }
            }

            var redirectUrl = string.Format("/RequestDetails/Index?RequestID={0}", requestID);
            return Redirect(redirectUrl);
        }

        [HttpPost]

        internal RequestDetailsViewModel CreateModel(long requestID)
        {
            var model = new RequestDetailsViewModel();
            var assembler = new ModelAssembler();
            var persistedRequest = this.PersistenceSession.Get<Request>(requestID);
            if (persistedRequest != null)
            {
                var projects = this.PersistenceSession.QueryOver<Project>().Future();
                var persistedPhases = this.PersistenceSession.QueryOver<Phase>()
                    .Where(p => p.Request.ID == requestID).Future();
                var persistedProjects = persistedPhases
                    .Select(persistedPhase => this.PersistenceSession.QueryOver<Project>()
                        .Where(p => p.Phase.ID == persistedPhase.ID)
                        .Future().FirstOrDefault())
                        .Where(persistedProject => persistedProject != null)
                        .ToList();


                if (persistedRequest == null)
                {
                    this.ModelState.AddModelError(String.Empty,
                        string.Format("Заявка с ID='{0}' не найдена.", requestID));
                }
                //else
                //{
                //    var requestModel = assembler.CreateRequestModel(persistedRequest);
                //    model.Request = requestModel;
                //    var contact = PersistenceSession.Get<Contract>(model.Request.ContractID);
                //    if (contact != null)
                //    {
                //        model.Request.isCustomersWithoutContracts = contact.ContractType.ID > 3 ? true : false;
                //    }

                //}

                if (persistedRequest != null)
                {
                    foreach (var phase in model.Request.Phases)
                    {
                        var phaseCount = 0;
                        if (projects != null)
                        {
                            phaseCount = projects.Where(p => p.Phase.ID == phase.ID).Count();
                        }

                        phase.ProcessState = phaseCount > 0 ? "В работе" : RequestProcessState.Необработанная.ToString();
                    }
                }

                if (persistedProjects.Count() > 0)
                {
                    model.Projects = persistedProjects.Select(assembler.CreateProjectModel);
                }

                return model;
            }
            else
            {
                return model;
            }
        }


        private void InitializeViewData(RequestDetailsViewModel model)
        {
            var modelAssembler = new ModelAssembler();
            var requestDetailTypes = this.PersistenceSession.QueryOver<RequestDetailType>().OrderBy(r => r.ID).Asc.Future();
            model.RequestDetailTypes = requestDetailTypes.Select(modelAssembler.CreateRequestDetailTypeModel);
        }

        [HttpPost]
        public virtual ActionResult RequestStatusReOpen(string requestID) // открытие заявки и проекта +отправка письма с уведомлением
        {
            long result = 0;
            if (long.TryParse(requestID, out result))
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {

                    var requestOpenStatus = this.PersistenceSession.Get<Status>((long)131076);

                    var requestOpenForUpdate = this.PersistenceSession.Get<Request>(long.Parse(requestID));
                    requestOpenForUpdate.Status = requestOpenStatus;
                    // var model = this.CreateModel(long.Parse(requestID));
                    StringBuilder projectList = new StringBuilder();

                    if (requestOpenForUpdate.ProjectManager.ID == CurrentEmployee.Value.ID
                   || ((bool)this.Session["ApprovePermission"]))
                    {

                        var projectOpenStatus = this.PersistenceSession.Get<Status>((long)131073);
                        var taskOpenStatus = this.PersistenceSession.Get<Status>((long)131072);


                        foreach (var phase in requestOpenForUpdate.Phases)
                        {
                            foreach (var project in phase.Projects)
                            {

                                var projectOpenForUpdate = this.PersistenceSession.Get<Project>(project.ID);
                                projectOpenForUpdate.CurrentStatus = projectOpenStatus;
                                this.PersistenceSession.Update(projectOpenForUpdate);

                                projectList.AppendFormat("{0}<br/>", projectOpenForUpdate.Name);

                            }
                        }


                        this.PersistenceSession.SaveOrUpdate(requestOpenForUpdate);
                        tx.Commit();

                        string mailTemplate = @"
                            <p>Уважаемые коллеги,открыта заявка<b>№{0}</b></p>
                            <table>
                            <tr>
                            <td><b>Заказчик:</b></td><td>{1}</br></td>
                            </tr>
                            <tr>
                            <td><b>Руководитель проекта:</b></td><td>{2}</br> </td>
                            </tr>
                            <tr>
                            <td><b>Открыт проект:</b></br></td><td>{4}</td>
                            </tr>
                            <tr>
                            <td><b>Детали заявки находятся по адресу:</b></td><td>http://timemanager.infocom-ltd.com/RequestDetails/Index?RequestID={3}</td>
                            </tr>
                            </table>
                            ";

                        string message = string.Format(mailTemplate, requestOpenForUpdate.Number, requestOpenForUpdate.Contract.Customer.Name,
                        requestOpenForUpdate.ProjectManager.ShortName, requestOpenForUpdate.ID, projectList.ToString());
                        var subject = string.Format("Открыта" + " заявка № {0}.", requestOpenForUpdate.Number);
                        var subscribers = new List<string>();

                        foreach (var budgetAllocation in requestOpenForUpdate.Phases.ToList()[0].Budget.BudgetAllocations) // вот эта часть отвечает за рассылку на участвующие отделы
                        {
                            if (!string.IsNullOrEmpty(budgetAllocation.Department.Email))
                            {
                                subscribers.Add(budgetAllocation.Department.Email);
                                subscribers.Add("ksusha@ia.ua");
                            }
                            else
                            {
                                subscribers.Add("alina_alinichenko@dit.ia.ua");

                            }
                        }
                        this.WatchdogMailMessageHtml(subscribers, message, subject);

                    }
                }
            }

            var redirectUrl = string.Format("/RequestDetails/Index?RequestID={0}", requestID);
            return Redirect(redirectUrl);
        }

        [HttpPost]
        public virtual ActionResult RequestCheck(long requestID)
        {
            if ((bool)this.Session["ApprovePermission"])
            {

                using (var tx = this.PersistenceSession.BeginTransaction())
                {

                    var requestToUpdate = PersistenceSession.Get<Request>(requestID);
                    var statusValue = requestToUpdate.Status.ID;

                    if (requestToUpdate.IsValidated != true)
                    {

                        requestToUpdate.IsValidated = true;
                        requestToUpdate.Status = PersistenceSession.Get<Status>((long)131076); // Status Open
                        requestToUpdate.LastUpdateUser = CurrentEmployee.Value;
                        requestToUpdate.LastUpdateDate = DateTime.Now;

                        this.PersistenceSession.SaveOrUpdate(requestToUpdate);

                        StringBuilder projectList = new StringBuilder();

                        tx.Commit();
                        var contractName = "";
                        if (requestToUpdate.Type.ID != 3)
                        {
                            contractName = string.Format("Наименование договора: {0}", requestToUpdate.Contract.Name);
                        }
                        else
                        {
                            //               foreach (var item in requestToUpdate.RequestDetails.Where(r=>r.Checked==true) )
                            //{
                            // ЗАКОНЧИТЬ!!!!
                            //} 
                        }

                        var statusName = string.Empty;
                        if (statusValue == 131076)
                        {
                            statusName = " создана";
                        }
                        else
                        {
                            statusName = " изменена";
                        }

                        string mailTemplate = @"
                                   <p>Уважаемые коллеги, <b>{5}</b> заявка <b>№{0}</b></p>
                                   <table>
                                   <tr>
                                   <td><b>Наименование:</b></td><td>{6}</br></td>
                                   </tr>
                                   <tr>
                                   <td><b>Заказчик:</b></td><td>{1}</br></td>
                                   </tr>
                                   <tr>
                                   <td><b>Руководитель проекта:</b></td><td>{2}</br> </td>
                                   </tr>
                                   <tr>
                                   <td><b>Наименование договора:</b></br></td><td>{3}</td>
                                   </tr>
                                   <tr>
                                   <td><b>Детали заявки находятся по адресу:</b></td><td>http://timemanager.infocom-ltd.com/RequestDetails/Index?RequestID={4}</td>
                                   </tr>
                                   </table>
                                   ";

                        string message = string.Format(mailTemplate, requestToUpdate.Number, requestToUpdate.Contract.Customer.Name,
                        requestToUpdate.ProjectManager.ShortName, requestToUpdate.Contract.Name, requestToUpdate.ID, statusName, requestToUpdate.Phases.First().Name);
                        var subject = string.Format(statusName + " заявка № {0}.", requestToUpdate.Number);
                        var subscribers = new List<string>();

                        foreach (var budgetAllocation in requestToUpdate.Phases.ToList()[0].Budget.BudgetAllocations)
                        {
                            if (!string.IsNullOrEmpty(budgetAllocation.Department.Email))
                            {
                                subscribers.Add(budgetAllocation.Department.Email);
                            }
                            else
                            {
                                //subscribers.Add("dron@dit.ia.ua");
                                subscribers.Add("alina_alinichenko@dit.ia.ua");
                            }
                        }
                        this.WatchdogMailMessageHtml(subscribers, message, subject);
                    }
                }
            }
            return RedirectToIndex();

        }

        [HttpPost]
        public virtual ActionResult TenderResult(long requestId, string tenderRemark, string tenderResult)
        {
            using (var tx = this.PersistenceSession.BeginTransaction())
            {
                var requestToUpdate = this.PersistenceSession.Get<Request>(requestId);
                requestToUpdate.TenderResult = tenderResult;
                requestToUpdate.TenderRemark = tenderRemark;
                this.PersistenceSession.SaveOrUpdate(requestToUpdate);
                tx.Commit();

            }
            var redirectUrl = string.Format("/RequestDetails/Index?RequestID={0}", requestId);
            return Redirect(redirectUrl);
        }

        [HttpPost]
        public virtual ActionResult RequestUpdate(RequestModel requestModel)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    try
                    {

                        var listOfChanges = new List<string>();

                        var requestToUpdate = PersistenceSession.Get<Request>(requestModel.ID);

                        if (requestToUpdate.Status.ID != (long)131077)
                        {


                            var requestType = this.PersistenceSession.Get<RequestType>(requestModel.Type.ID);
                            var projectManager = this.PersistenceSession.Get<Employee>(requestModel.ProjectManagerId);
                            var phasesToUpdate =
                                this.PersistenceSession.QueryOver<Phase>().Where(
                                    phase => phase.Request.ID == requestModel.ID).Future().ToList();
                            int phaseNumber = 0;
                            foreach (var phase in requestModel.Phases)
                            {
                                if (phasesToUpdate.Count - 1 >= phaseNumber)
                                {

                                    if (phasesToUpdate[phaseNumber].DeadLine != phase.DeadLine)
                                    {
                                        listOfChanges.Add(
                                            string.Format("Дата окончания проекта изменена с '{0}' на '{1}'",
                                                          phasesToUpdate[phaseNumber].DeadLine, phase.DeadLine));
                                        phasesToUpdate[phaseNumber].DeadLine = phase.DeadLine;
                                    }

                                    if (phasesToUpdate[phaseNumber].Name != phase.Name)
                                    {
                                        listOfChanges.Add(string.Format("Наименование проекта с '{0}' на '{1}'",
                                                                        phasesToUpdate[phaseNumber].Name, phase.Name));
                                        phasesToUpdate[phaseNumber].Name = phase.Name;
                                    }

                                    phasesToUpdate[phaseNumber].Number = phaseNumber + 1;

                                    var bugget = new ProjectBudget();
                                    var oldBudgetAllocations = phasesToUpdate[phaseNumber].Budget.BudgetAllocations;
                                    var oldBudget = oldBudgetAllocations.ToList();
                                    foreach (var allocation in phase.Budget.BudgetAllocations)
                                    {
                                        if (allocation.AmountOfHours != 0 &&
                                            allocation.AmountOfMoney.ToString(CultureInfo.InvariantCulture) != "0,00")
                                        {

                                            var department = PersistenceSession.Get<Department>(allocation.Department.ID);
                                            var oldDepartmentAllocation =
                                                oldBudget.Where(b => b.Department.ID == department.ID).FirstOrDefault();
                                            if (oldDepartmentAllocation != null)
                                            {
                                                if (oldDepartmentAllocation.AmountOfHours != allocation.AmountOfHours)
                                                {
                                                    listOfChanges.Add(
                                                        string.Format(
                                                            "Распределение бюджетного времени для департамента {0} изменено с '{1}' на '{2}'",
                                                            department.Name, oldDepartmentAllocation.AmountOfHours,
                                                            allocation.AmountOfHours));
                                                }
                                                if (oldDepartmentAllocation.AmountOfMoney != allocation.AmountOfMoney)
                                                {
                                                    listOfChanges.Add(
                                                        string.Format(
                                                            "Распределение бюджетных средств для департамента {0} изменено с '{1}' на '{2}'",
                                                            department.Name, oldDepartmentAllocation.AmountOfMoney,
                                                            allocation.AmountOfMoney));
                                                }
                                            }
                                            else
                                            {
                                                listOfChanges.Add(
                                                    string.Format(
                                                        "Добавлен новый департамент {0} в раздел распределения бюджетных средств.",
                                                        department.ShortName));
                                            }

                                            var newAllocation = new ProjectBudgetAllocation
                                                {
                                                    AmountOfHours = allocation.AmountOfHours,
                                                    AmountOfMoney = allocation.AmountOfMoney,
                                                    Department = department,
                                                    DateEndWorkByDepartment = allocation.DateEndWorkByDepartment
                                                };
                                            bugget.BudgetAllocations.Add(newAllocation);
                                        }
                                    }
                                    if (bugget.BudgetAllocations.Count == 0)
                                    {
                                        throw new Exception("Загрузка данных прервана из-за ошибки: Не был выбран ни один департамент. Повторите попытку с указанием хотя бы одного департамента");
                                    }


                                    var phaseId = requestToUpdate.Phases.First().ID;
                                    foreach (var allocation in oldBudgetAllocations)
                                    {
                                        if (!phase.Budget.BudgetAllocations
                                                 .Where(ba => ba.Department.ID == allocation.Department.ID).Any())
                                        {
                                            var projectOfPhase = this.PersistenceSession
                                                .QueryOver<Project>().Where(p => p.Phase.ID == phaseId).Future().
                                                FirstOrDefault();
                                            if (projectOfPhase != null)
                                            {
                                                if (
                                                    projectOfPhase.Employees.Where(
                                                        e => e.Department.ID == allocation.Department.ID).Any())
                                                {
                                                    var department =
                                                        PersistenceSession.Get<Department>(allocation.Department.ID);
                                                    var newAllocation = new ProjectBudgetAllocation
                                                        {
                                                            AmountOfHours = allocation.AmountOfHours,
                                                            AmountOfMoney = allocation.AmountOfMoney,
                                                            Department = department,
                                                            DateEndWorkByDepartment = allocation.DateEndWorkByDepartment
                                                        };
                                                    bugget.BudgetAllocations.Add(newAllocation);
                                                }
                                            }
                                        }
                                    }
                                    if (bugget.BudgetAllocations.Count == 0)
                                    {
                                        throw new Exception("Не был выбран ни один департамент. Повторите попытку с указанием хотя бы одного департамента");
                                    }


                                    phasesToUpdate[phaseNumber].Request = requestToUpdate;
                                    phasesToUpdate[phaseNumber].Budget = bugget;
                                }
                                if (phasesToUpdate.Count <= phaseNumber)
                                {
                                    var bugget = new ProjectBudget();
                                    foreach (var allocation in phase.Budget.BudgetAllocations)
                                    {
                                        var department =
                                            this.PersistenceSession.Get<Department>(allocation.Department.ID);
                                        var newAllocation = new ProjectBudgetAllocation
                                            {
                                                AmountOfHours = allocation.AmountOfHours,
                                                AmountOfMoney = allocation.AmountOfMoney,
                                                Department = department,
                                                DateEndWorkByDepartment = allocation.DateEndWorkByDepartment
                                            };
                                        bugget.BudgetAllocations.Add(newAllocation);
                                    }
                                    if (bugget.BudgetAllocations.Count == 0)
                                    {
                                        throw new Exception("Не был выбран ни один департамент. Повторите попытку с указанием хотя бы одного департамента");
                                    }

                                    var newPhase = new Phase
                                        {
                                            Budget = bugget,
                                            DeadLine = phase.DeadLine,
                                            Name = phase.Name,
                                            Request = requestToUpdate,
                                            Number = phaseNumber + 1
                                        };
                                    phasesToUpdate.Add(newPhase);
                                    this.PersistenceSession.SaveOrUpdate(newPhase);

                                    phaseNumber++;
                                }

                                requestToUpdate.Phases.Clear();
                                requestToUpdate.Phases.AddRange(phasesToUpdate);

                                if (requestToUpdate.Contract.ID != requestModel.ContractID)
                                {

                                    var contract = new Contract();
                                    if (requestType.ID != 3)
                                    {
                                        if (requestModel.isCustomersWithoutContracts) //проверяет установку CheckBox Без договора 
                                        {
                                            var customer = PersistenceSession.Get<Customer>(requestModel.CustomerID);
                                            if (customer == null)
                                            {
                                                var newCustomer = new Customer { Name = requestModel.CustomerName };
                                                this.PersistenceSession.SaveOrUpdate(newCustomer);

                                                customer = PersistenceSession.QueryOver<Customer>()
                                                    .Where(c => c.Name == requestModel.CustomerName).SingleOrDefault();
                                            }
                                            var newContract = new Contract();
                                            newContract.Number = CreateRequestNumber(requestType.ID);
                                            switch (requestType.ID)
                                            {
                                                case 1:
                                                    newContract.Name = string.Format("Административные работы {0}", newContract.Number);
                                                    break;
                                                case 2:
                                                    newContract.Name = string.Format("Проект {0}", newContract.Number);
                                                    break;
                                                case 4:
                                                    newContract.Name = string.Format("Производство {0}", newContract.Number);
                                                    break;
                                                case 5:
                                                    newContract.Name = string.Format("Обучение {0}", newContract.Number);
                                                    break;
                                                case 6:
                                                    newContract.Name = string.Format("Сервисное обслуживание {0}", newContract.Number);
                                                    break;
                                                case 7:
                                                    newContract.Name = string.Format("Вызов специалиста {0}", newContract.Number);
                                                    break;
                                            }
                                            newContract.ContractType = this.PersistenceSession.Get<ContractType>((long)(requestModel.ContractID));
                                            newContract.Customer = customer;
                                            newContract.SigningDate = requestModel.Date;
                                            this.PersistenceSession.SaveOrUpdate(newContract);

                                            contract = PersistenceSession.QueryOver<Contract>()
                                            .Where(c => c.Number == newContract.Number && c.Customer.ID == customer.ID)
                                            .Future().FirstOrDefault();
                                        }
                                        else
                                        {
                                            contract = PersistenceSession.Get<Contract>(requestModel.ContractID);
                                        }
                                    }
                                    else
                                    {
                                        var customer = PersistenceSession.Get<Customer>(requestModel.CustomerID);
                                        if (customer == null)
                                        {
                                            var newCustomer = new Customer { Name = requestModel.CustomerName };
                                            this.PersistenceSession.SaveOrUpdate(newCustomer);

                                            customer = PersistenceSession.QueryOver<Customer>()
                                                .Where(c => c.Name == requestModel.CustomerName).SingleOrDefault();
                                        }

                                        var newContract = new Contract();

                                        newContract.Number = CreateRequestNumber(requestType.ID);
                                        newContract.Name = string.Format("Проработка ТП {0}", newContract.Number);
                                        newContract.ContractType = this.PersistenceSession.Get<ContractType>((long)3);
                                        newContract.Customer = customer;
                                        newContract.SigningDate = requestModel.Date;
                                        this.PersistenceSession.SaveOrUpdate(newContract);

                                        contract = PersistenceSession.QueryOver<Contract>()
                                            .Where(c => c.Number == newContract.Number && c.Customer.ID == customer.ID)
                                            .SingleOrDefault();
                                    }
                                    requestToUpdate.Contract = contract;
                                }

                                requestToUpdate.Status = this.PersistenceSession.Get<Status>((long)131078);
                                if (requestToUpdate.Number != requestModel.Number)
                                {
                                    listOfChanges.Add(string.Format("Номер заявки с '{0}' на '{1}'", requestToUpdate.Number,
                                                                    requestModel.Number));
                                    requestToUpdate.Number = requestModel.Number;
                                }

                                if (requestToUpdate.StartDate != requestModel.StartDate)
                                {
                                    listOfChanges.Add(string.Format("Дата начала работ с '{0}' на '{1}'",
                                                                    requestToUpdate.StartDate, requestModel.StartDate));
                                    requestToUpdate.StartDate = requestModel.StartDate;
                                }

                                if (requestToUpdate.Type.ID != requestType.ID)
                                {
                                    listOfChanges.Add(string.Format("Тип заявки с '{0}' на '{1}'", requestToUpdate.Type.Name,
                                                                    requestType.Name));
                                    requestToUpdate.Type = requestType;
                                }

                                if (requestToUpdate.ProjectManager != projectManager)
                                {
                                    listOfChanges.Add(string.Format("Менеджер проекта с '{0}' на '{1}'",
                                                                    requestToUpdate.ProjectManager.ShortName,
                                                                    projectManager.ShortName));
                                    requestToUpdate.ProjectManager = projectManager;
                                }
                                if (requestToUpdate.Date != requestModel.Date)
                                {
                                    listOfChanges.Add(string.Format("Дата с '{0}' на '{1}'", requestToUpdate.Date,
                                                                    requestModel.Date));
                                    requestToUpdate.Date = requestModel.Date;
                                }

                                var requestDetails =
                                    this.PersistenceSession.QueryOver<RequestDetail>().Where(
                                        r => r.Request.ID == requestModel.ID);

                                if (requestToUpdate.Detail != requestModel.Detail)
                                {
                                    listOfChanges.Add(string.Format("Детали заявки с '{0}' на '{1}'", requestToUpdate.Detail,
                                                                    requestModel.Detail));
                                    requestToUpdate.Detail = requestModel.Detail;
                                }

                                if (requestToUpdate.Documentation != requestModel.Documentation)
                                {
                                    listOfChanges.Add(string.Format("Количество экземпляров проекта с '{0}' на '{1}'",
                                                                    requestToUpdate.Documentation,
                                                                    requestModel.Documentation));
                                    requestToUpdate.Documentation = requestModel.Documentation;
                                }

                                requestToUpdate.IsValidated = false;
                                requestToUpdate.LastUpdateDate = DateTime.Now;
                                requestToUpdate.LastUpdateUser = CurrentEmployee.Value;
                                this.PersistenceSession.SaveOrUpdate(requestToUpdate);
                                this.SetRequestDetails(requestModel, requestToUpdate.ID, listOfChanges);
                                tx.Commit();

                                var message = string.Format("Изменены детали заявки № {0}. "
                                                            + System.Environment.NewLine +
                                                            "Пользователь изменивший заявку: {5}"
                                                            + System.Environment.NewLine +
                                                            "Заявку необходимо повторно утвердить. "
                                                            + System.Environment.NewLine + "Руководитель проекта: {1} "
                                                            + System.Environment.NewLine + "Заказчик: {3}"
                                                            + System.Environment.NewLine + "Наименование заявки: {2}. "
                                                            + System.Environment.NewLine +
                                                            "Детали заявки находятся по адресу: http://timemanager.infocom-ltd.com/RequestDetails/Index?RequestID={4} .",
                                                            requestToUpdate.Number,
                                                            requestToUpdate.ProjectManager.ShortName,
                                                            requestToUpdate.Phases.First().Name,
                                                            requestToUpdate.Contract.Customer.Name,
                                                            requestToUpdate.ID,
                                                            CurrentEmployee.Value.ShortName);

                                if (listOfChanges.Count > 0)
                                {
                                    message += System.Environment.NewLine + "Были внесенны следующие изменения:" +
                                               System.Environment.NewLine;
                                    foreach (var changeMessage in listOfChanges)
                                    {
                                        message += changeMessage + System.Environment.NewLine;
                                    }
                                }
                                var subject = string.Format("Изменены детали заявки № {0}.", requestToUpdate.Number);
                                var subscribers = new List<string>();



                                subscribers.Add("alina_alinichenko@dit.ia.ua");
                                subscribers.Add("ksusha@ia.ua");
                                subscribers.Add("dron@dit.ia.ua");

                                this.WatchdogMailMessage(subscribers, message, subject);
                            }
                        }
                        else
                        {
                            this.CreateOrderEditFormErrorMessage(string.Format("Заявка № {0} была закрыта. Изменения без разрешения начальника КД не возможны.", requestModel.Number));
                        }
                    }
                    catch (Exception ex)
                    {
                        this.CreateOrderEditFormErrorMessage(string.Format("Загрузка данных прервана из-за ошибки: {0}", ex.Message));
                    }
                    return RedirectToIndex();
                }
            }
            else
            {
                var value = this.ModelState.Values.Where(v => v.Errors.Any() == true);
                this.CreateOrderEditFormErrorMessage(string.Format("Загрузка данных прервана из-за ошибки: {0}", value.First().Errors.First().ErrorMessage));
            }
            return RedirectToIndex();
        }

        private void WatchdogMailMessage(List<string> messageTo, string body, string subject)
        {
            //var watchdogSettings = new WatchdogSettings();
            var mailMessage = new MailMessage();
            try
            {
                var smtpServerName = Properties.Settings.Default.SMTPServerName;
                using (var smtpClient = new SmtpClient(smtpServerName))
                {
                    smtpClient.Port = 25;
                    mailMessage.From = new MailAddress("NoReply@dit.infocom-ltd.com");
                    foreach (var address in messageTo.Cast<string>().Where(address => !string.IsNullOrEmpty(address)))
                    {
                        mailMessage.To.Add(new MailAddress(address));
                    }
                    mailMessage.Subject = "[Time Manager] " + subject;
                    mailMessage.IsBodyHtml = false;
                    mailMessage.Body = body;
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                var exception = ex.InnerException;
                var message = string.Empty;
                while (exception != null)
                {
                    message += exception.Message + ";";
                    exception = exception.InnerException;
                }
                this.CreateOrderEditFormErrorMessage(string.Format("Отправка сообщения уведомлений прервана по причине: {0}", message));
            }
        }

        private void WatchdogMailMessageHtml(List<string> messageTo, string body, string subject) //html-шаблон для отправки писем по открытию-закрытию заявок и проектов
        {
            //var watchdogSettings = new WatchdogSettings();
            var mailMessage = new MailMessage();
            try
            {
                var smtpServerName = Properties.Settings.Default.SMTPServerName;
                using (var smtpClient = new SmtpClient(smtpServerName))
                {
                    smtpClient.Port = 25;
                    mailMessage.From = new MailAddress("NoReply@dit.infocom-ltd.com");
                    foreach (var address in messageTo.Cast<string>().Where(address => !string.IsNullOrEmpty(address)))
                    {
                        mailMessage.To.Add(new MailAddress(address));
                    }
                    mailMessage.Subject = "[Time Manager]" + subject;

                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = body;
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                var exception = ex.InnerException;
                var message = string.Empty;
                while (exception != null)
                {
                    message += exception.Message + ";";
                    exception = exception.InnerException;
                }
                this.CreateOrderEditFormErrorMessage(string.Format("Отправка сообщения уведомлений прервана по причине: {0}", message));
            }
        }

        [HttpPost]
        public virtual ActionResult RequestInsert(RequestModel requestModel)
        {
            if (ModelState["CustomerID"].Errors.Count > 0)
            {
                try
                {
                    using (var tx = this.PersistenceSession.BeginTransaction())
                    {
                        var newCustomer = new Customer { Name = requestModel.CustomerName };
                        this.PersistenceSession.SaveOrUpdate(newCustomer);
                        tx.Commit();
                    }
                    ModelState["CustomerID"].Errors.Clear();
                }
                catch (Exception ex)
                {

                    CreateOrderEditFormErrorMessage(ex.Message + " " + ex.InnerException.Message);
                }
            }
            if ( (ModelState["isCustomersWithoutContracts"].Errors.Count > 0))
            {
                ModelState["isCustomersWithoutContracts"].Errors.Clear();
            }
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    try
                    {
                        var modelAssembler = new ModelAssembler();

                        var newRequest = new Request();
                        var requestType = PersistenceSession.Get<RequestType>(requestModel.Type.ID);
                        var manager = PersistenceSession.Get<Employee>(requestModel.ProjectManagerId);
                        var customer = new Customer();
                        var LastUser = CurrentEmployee.Value;
                        var LastUpdateDate = DateTime.Now;
                        var requestDate = newRequest.Date != DateTime.MinValue ? newRequest.Date : DateTime.Now;
                        customer = PersistenceSession.Get<Customer>(requestModel.CustomerID);
                        if (customer == null)
                        {
                            customer = PersistenceSession.QueryOver<Customer>()
                                .Where(c => c.Name == requestModel.CustomerName).SingleOrDefault();
                            if (customer == null)
                            {
                                var newCustomer = new Customer { Name = requestModel.CustomerName };
                                this.PersistenceSession.SaveOrUpdate(newCustomer);
                                customer = PersistenceSession.QueryOver<Customer>()
                                    .Where(c => c.Name == requestModel.CustomerName).SingleOrDefault();
                            }
                        }

                        var contract = new Contract();
                        if (requestType.ID != 3)
                        {
                            if (requestModel.isCustomersWithoutContracts) //проверяет установку CheckBox Без договора 
                            {
                                var newContract = new Contract();
                                newContract.Number = CreateRequestNumber(requestType.ID);
                                switch (requestType.ID)
                                {
                                    case 1:
                                        newContract.Name = string.Format("Административные работы {0}", newContract.Number);
                                        break;
                                    case 2:
                                        newContract.Name = string.Format("Проект {0}", newContract.Number);
                                        break;
                                    case 4:
                                        newContract.Name = string.Format("Производство {0}", newContract.Number);
                                        break;
                                    case 5:
                                        newContract.Name = string.Format("Обучение {0}", newContract.Number);
                                        break;
                                    case 6:
                                        newContract.Name = string.Format("Сервисное обслуживание {0}", newContract.Number);
                                        break;
                                    case 7:
                                        newContract.Name = string.Format("Вызов специалиста {0}", newContract.Number);
                                        break;
                                }
                                newContract.ContractType = this.PersistenceSession.Get<ContractType>((long)(requestModel.ContractID));
                                newContract.Customer = customer;
                                newContract.SigningDate = requestModel.Date;
                                this.PersistenceSession.SaveOrUpdate(newContract);

                                contract = PersistenceSession.QueryOver<Contract>()
                                .Where(c => c.Number == newContract.Number && c.Customer.ID == customer.ID)
                                .Future().FirstOrDefault();
                            }
                            else
                            {
                                contract = PersistenceSession.Get<Contract>(requestModel.ContractID);
                            }
                        }
                        else
                        {
                            var newContract = new Contract();
                            newContract.Number = CreateRequestNumber(requestType.ID);
                            newContract.Name = string.Format("Проработка ТП {0}", newContract.Number);
                            newContract.ContractType = this.PersistenceSession.Get<ContractType>((long)3);
                            newContract.Customer = customer;
                            newContract.SigningDate = requestModel.Date;
                            this.PersistenceSession.SaveOrUpdate(newContract);


                            contract = PersistenceSession.QueryOver<Contract>()
                                .Where(c => c.Number == newContract.Number && c.Customer.ID == customer.ID)
                                .Future().FirstOrDefault();
                        }

                        var phases = new List<Phase>();
                        var bugget = new ProjectBudget();

                        var phase = requestModel.Phases[0];
                        foreach (var allocation in phase.Budget.BudgetAllocations)
                        {
                            if (allocation.AmountOfHours != 0 &&
                                allocation.AmountOfMoney.ToString(CultureInfo.InvariantCulture) != "0,00")
                            {
                                var department = PersistenceSession.Get<Department>(allocation.Department.ID);
                                var newAllocation = new ProjectBudgetAllocation
                                    {
                                        AmountOfHours = allocation.AmountOfHours,
                                        AmountOfMoney = allocation.AmountOfMoney,
                                        Department = department,
                                        DateEndWorkByDepartment = allocation.DateEndWorkByDepartment
                                    };
                                bugget.BudgetAllocations.Add(newAllocation);
                            }
                        }
                        if (bugget.BudgetAllocations.Count == 0)
                        {
                            CreateOrderEditFormErrorMessage(
                                "Загрузка данных прервана. Не был выбран ни один департамент. Повторите попытку с указанием хотя бы одного департамента");
                        }
                        else
                        {
                            var newPhase = new Phase
                               {
                                   Budget = bugget,
                                   DeadLine = phase.DeadLine,
                                   Name = phase.Name,
                                   Number = 1,
                                   Request = newRequest
                               };

                            phases.Add(newPhase);

                            newRequest.Status = this.PersistenceSession.Get<Status>((long)131076);
                            newRequest.Detail = requestModel.Detail;
                            newRequest.Documentation = requestModel.Documentation; // количество экземпляров проекта
                            newRequest.Date = requestModel.Date;
                            newRequest.Number = CreateRequestNumber(requestType.ID);
                            newRequest.Phases = phases;
                            newRequest.Contract = contract;
                            newRequest.Type = requestType;
                            newRequest.ProjectManager = manager;
                            newRequest.LastUpdateDate = LastUpdateDate;
                            newRequest.LastUpdateUser = LastUser;
                            newRequest.IsValidated = false;
                            newRequest.StartDate = requestModel.StartDate;
                            this.SetRequestDetails(requestModel, newRequest);
                            this.PersistenceSession.SaveOrUpdate(newRequest);

                            tx.Commit();

                            this.SendMailToOmManagers(newRequest);
                        }

                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("not-null"))
                        {
                            var message = "Загрузка данных прервана. ";
                            if (ex.Message.Contains("Phases[0].Name"))
                            {
                                message += "Поле 'Наименование проекта' не было заполнено";
                            }
                            else
                            {
                                message += "Не все поля были заполнены";
                            }
                            this.CreateOrderEditFormErrorMessage(message);
                        }
                        else
                        {
                            this.CreateOrderEditFormErrorMessage("Загрузка данных прервана, т.к. произошла непредвиденная ошибка");
                        }
                    }
                    return RedirectToIndex();
                }
            }
            else
            {
                this.CreateOrderEditFormErrorMessage(string.Format("Загрузка данных прервана из-за ошибки."));
            }

            return this.ViewIndex();
        }

        private void SendMailToOmManagers(Request requestToUpdate)
        {
            var contractName = "";
            if (requestToUpdate.Type.ID != 3)
            {
                contractName = string.Format("Наименование договора: {0}" /*+ System.Environment.NewLine*/, requestToUpdate.Contract.Name);
            }

            string mailTemplate = @"
                               <p>Создана заявка <b>№{0}</b></p>
                               <table>
                               <tr>
                               <td><b>Заказчик:</b></td><td>{1}</br></td>
                               </tr>
                               <tr>
                               <td><b>Руководитель проекта:</b></td><td>{2}</br> </td>
                               </tr>
                               <tr>
                               <td><b>Наименование договора:</b></br></td><td>{3}</td>
                               </tr>
                               <tr>
                               <td><b>Детали заявки:</b></br></td><td>{4}</td>
                               </tr>
                               <tr>
                               <td><b>Детали заявки находятся по адресу:</b></td><td>http://timemanager.infocom-ltd.com/RequestDetails/Index?RequestID={5}</td>
                               </tr>
                               </table>
                               ";

            string message = string.Format(mailTemplate, requestToUpdate.Number, requestToUpdate.Contract.Customer.Name,
            requestToUpdate.ProjectManager.ShortName, requestToUpdate.Contract.Name, requestToUpdate.Phases.First().Name, requestToUpdate.ID);
            var subject = string.Format("Создана заявка № {0}.", requestToUpdate.Number);
            var subscribers = new List<string>();


            /* var message = string.Format("Создана заявка № {0}."
                 + System.Environment.NewLine + "Заказчик: {1}"
                 + System.Environment.NewLine + "Руководитель проекта: {2} "
                 + System.Environment.NewLine + "{3}"
                 + System.Environment.NewLine + "{4}"
                 + System.Environment.NewLine + "Детали заявки находятся по адресу: http://timemanager.infocom-ltd.com/RequestDetails/Index?RequestID={5} .",
                 requestToUpdate.Number, requestToUpdate.Contract.Customer.Name, requestToUpdate.ProjectManager.ShortName,
                 contractName, requestToUpdate.Phases.First().Name, requestToUpdate.ID);

             var subject = string.Format("Создана заявка № {0}.", requestToUpdate.Number);
             var subscribers = new List<string>();*/

            subscribers.Add("ksusha@ia.ua");
            subscribers.Add("dron@dit.ia.ua");
            subscribers.Add("alina_alinichenko@dit.ia.ua");

            this.WatchdogMailMessageHtml(subscribers, message, subject);
        }

        private void SetRequestDetails(RequestModel requestModel, Core.DomainModel.Request newRequest)
        {
            foreach (var requestDetail in requestModel.RequestDetails)
            {
                var deteilType = PersistenceSession.Get<RequestDetailType>(requestDetail.RequestDetailTypeID);

                newRequest.RequestDetails.Add(
                        new RequestDetail
                        {
                            Request = newRequest,
                            RequestDetailType = deteilType,
                            Checked = requestDetail.Checked
                        });
            }
        }

        private void SetRequestDetails(RequestModel requestModel, long requestId, List<string> listOfChanges)
        {
            var requestDetails = new List<RequestDetail>();
            var requestDetailsCount = this.PersistenceSession.QueryOver<RequestDetail>().Where(rd => rd.Request.ID == requestId).Future().Count();
            if (requestDetailsCount == 6)
            {
                foreach (var requestDetail in requestModel.RequestDetails)
                {
                    var requestDetailToUpdate =
                        this.PersistenceSession.QueryOver<RequestDetail>()
                        .Where(rd => rd.RequestDetailType.ID == requestDetail.RequestDetailTypeID && rd.Request.ID == requestId)
                        .Future().Single();
                    if (requestDetailToUpdate.Checked != requestDetail.Checked)
                    {

                        listOfChanges.Add(string.Format("Детали заявки ({0}) с '{1}' на '{2}'",
                            requestDetailToUpdate.RequestDetailType.Name, requestDetailToUpdate.Checked, requestDetail.Checked));

                        requestDetailToUpdate.Checked = requestDetail.Checked;
                        this.PersistenceSession.SaveOrUpdate(requestDetailToUpdate);
                    }

                }
            }
            else
            {
                var request = this.PersistenceSession.Get<Request>(requestId);
                this.SetRequestDetails(requestModel, request);
                this.PersistenceSession.SaveOrUpdate(request);
            }

        }

        [HttpPost]
        public virtual IHtmlString GetRequestNumber(long typeID)
        {
            var requestNumer = this.CreateRequestNumber(typeID);
            return new MvcHtmlString(requestNumer);
        }

        [HttpPost]
        public virtual ActionResult GetCustomer(string text)
        {
            Thread.Sleep(1000);
            var preview = this.PersistenceSession.QueryOver<Customer>()
                .Where(c => c.Name != string.Empty).Future().ToList();
            var result = preview
                .Where(c => c.Name.ToLower().IndexOf(text.ToLower()) >= 0);//.CreateRequestNumber(typeID);
            return new JsonResult { Data = new SelectList(result, "ID", "Name") };
        }

        private string CreateRequestNumber(long typeID)
        {
            var year = DateTime.Now.Year.ToString().Substring(2, 2);
            var dateFrom = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
            var dateTo = dateFrom.AddYears(1);
            var requests = this.PersistenceSession.QueryOver<Request>()
                    .Where(r => r.Type.ID == typeID && r.Date > dateFrom && r.Date < dateTo).Future().OrderBy(r => r.Number);
            var requestTypeShortName = this.PersistenceSession.QueryOver<RequestType>()
                    .Where(r => r.ID == typeID).Future().Single().Description;
            var requestNumer = "";
           
            if (requests.Count() == 0)
            {
                requestNumer = string.Format("{0}-0001{1}", year, requestTypeShortName);
            }
            else
            {
                var number = int.Parse(requests.Last().Number.Replace(year.ToString() + "-", "").Replace(requestTypeShortName, ""));

                requestNumer = string.Format("{0}-{1:0000}{2}", year, number + 1, requestTypeShortName);
            }

            return requestNumer;
        }

        [HttpPost]
        public virtual ActionResult RequestDelete(long id)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    var NeedToDelete = true;

                    var phasesToDelete = this.PersistenceSession.QueryOver<Phase>()
                                        .Where(ph => ph.Request.ID == id).Future();

                    foreach (var phase in phasesToDelete)
                    {
                        var projectToDelete = this.PersistenceSession.QueryOver<Project>().Where(p => p.Phase.ID == phase.ID).SingleOrDefault();
                        if (projectToDelete != null)
                        {
                            var amountOfUsesTask =
                                projectToDelete.Tasks.Where(task => task.TimeRecords.Count() > 0).Count();
                            if (amountOfUsesTask > 0)
                            {
                                NeedToDelete = false;
                            }
                        }
                    }

                    if (NeedToDelete)
                    {
                        foreach (var phase in phasesToDelete)
                        {
                            var projectToDelete = this.PersistenceSession.QueryOver<Project>().Where(p => p.Phase.ID == phase.ID).SingleOrDefault();
                            if (projectToDelete != null)
                            {
                                this.PersistenceSession.Delete(projectToDelete);
                            }

                            var phaseToDelete = this.PersistenceSession.Get<Phase>(phase.ID);
                            this.PersistenceSession.Delete(phaseToDelete);
                        }

                        var requestDetails = this.PersistenceSession.QueryOver<RequestDetail>().Where(rd => rd.Request.ID == id).Future();
                        foreach (var requestDetail in requestDetails)
                        {
                            this.PersistenceSession.Delete(requestDetail);
                        }

                        var requestToDelete = this.PersistenceSession.Get<Request>(id);
                        this.PersistenceSession.Delete(requestToDelete);

                        if (requestToDelete.Type.ID == 3)
                        {
                            var contractToDelete = this.PersistenceSession.Get<Contract>(requestToDelete.Contract.ID);
                            this.PersistenceSession.Delete(contractToDelete);
                        }

                        tx.Commit();


                        string mailTemplate = @"
                               <p>Удалена заявка <b>№{0}</b></p>
                               <table>
                               <tr>
                               <td><b>Руководитель проекта:</b></td><td>{2}</br> </td>
                               </tr>
                               <tr>
                               <td><b>Наименование заявки:</b></br></td><td>{3}</td>
                               </tr>
                               <tr>
                               <td><b>Пользователь удаливший заявку:</b></td><td>{1}</br></td>
                               </tr>
                               </table>
                               ";

                        string message = string.Format(mailTemplate, requestToDelete.Number, CurrentEmployee.Value.ShortName,
                        requestToDelete.ProjectManager.ShortName, requestToDelete.Phases.First().Name);
                        var subject = string.Format("Удалена заявка № {0}.", requestToDelete.Number);
                        var subscribers = new List<string>();

                        subscribers.Add("ksusha@ia.ua");
                        subscribers.Add("dron@dit.ia.ua");
                        subscribers.Add("alina_alinichenko@dit.ia.ua");

                        this.WatchdogMailMessageHtml(subscribers, message, subject);
                    }
                    else
                    {
                        this.CreateOrderEditFormErrorMessage(string.Format("Удаление не возможно. По заявке уже существуют задания и зарегистрированное время по заданиям."));
                    }
                }

                return RedirectToIndex();
            }

            return this.ViewIndex();
        }

        private ActionResult RedirectToIndex()
        {
            return this.RedirectToAction("Index");
        }

        private ViewResult ViewIndex()
        {
            this.InitializeViewData();
            return View("Index");
        }

        #endregion
    }
}