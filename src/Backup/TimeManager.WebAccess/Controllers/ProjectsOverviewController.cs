namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Linq.SqlClient;
    using System.Web.Mvc;

    using System.Net.Mail;
    using System.Net.Mime;
    using System.Net;
    using System.Web.Security;
    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Filters;
    using Infocom.TimeManager.WebAccess.Infrastructure;

    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;

    using NHibernate.Linq;
    using Infocom.TimeManager.Core;

    [HandleError]
    public partial class ProjectsOverviewController : TimeManagerBaseController
    {
        private readonly string _projectsOverviewFilterKey = "ProjectsOverviewFilter";

        #region Public Methods

       
        public virtual ActionResult Index()
        {
            var c = CurrentEmployee;
           
         if(  
             !(((bool)this.Session["RequestPermission"])  ||  Roles.IsUserInRole("TimeManagerAdministrators")) 
             )              
                        
            {
                var filter = new ProjectsOverviewFilter();
                if (Session[this._projectsOverviewFilterKey] == null)
                {
                    filter.FilteringByAssignedProjects = true;
                    Session[this._projectsOverviewFilterKey] = filter;
                }
            }
           
            var model = this.CreateModel();
            this.InitializeViewData();


            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Index(long filteringByProjectTypeID, string filteringByAssignedProjects, string filteringBySearchWord,long filteringByStatusID)
        {
            var filteringByAssignedProjectsAsBoolean = bool.Parse(filteringByAssignedProjects);
            var filter = new ProjectsOverviewFilter();
            filter.FilteringByProjectTypeID = filteringByProjectTypeID;
            filter.FilteringByAssignedProjects = filteringByAssignedProjectsAsBoolean;
            filter.FilteringBySearchWord = filteringBySearchWord;
            filter.FilteringByStatusID = filteringByStatusID;
            Session[this._projectsOverviewFilterKey] = filter;

            var model = this.CreateModel();
            this.InitializeViewData();
            var view = View(model);
            return view;
        }

        [HttpPost]
        public virtual ActionResult ProjectUpdate(ProjectModel projectModel)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    try
                    {
                        var projectToUpdate = this.PersistenceSession.Load<Project>(projectModel.ID);
                        this.TryUpdateModel(projectToUpdate);
                        var projectType = this.PersistenceSession.Load<ProjectType>(projectModel.ProjectTypeID);
                        /* var projectCurrentSatatus = this.PersistenceSession.Load<Status>(projectModel.CurrentStatusID);*/  //запрет редактирования закрытой заявки
                        var projectPriority = this.PersistenceSession.Load<ProjectPriority>(projectModel.ProjectPiorityID);

                        var subscribers = new List<string>();
                                             
                        var assignedEmployees =
                            PersistenceSession.QueryOver<Employee>().WhereRestrictionOn(e => e.ID).IsIn(
                                projectModel.AssignedEmployeeIDs.ToArray()).Future().ToList();

                        var currentEmployees = projectToUpdate.Employees.ToList();
                       // var newe = (from i in assignedEmployees select i.ID).Distinct().Except((from k in currentEmployees select k.ID).Distinct());
                       var newEmployees = assignedEmployees.Except(currentEmployees);
                        
                        projectToUpdate.SetEmployees(assignedEmployees);
                        projectToUpdate.ProjectType = projectType;
                      /*  projectToUpdate.CurrentStatus = projectCurrentSatatus;*/ //запрет редактирования закрытой заявки
                        projectToUpdate.ProjectPriority = projectPriority;

                        this.PersistenceSession.SaveOrUpdate(projectToUpdate);

                        var email_list = new List<string>();

                        foreach (var employeeId in newEmployees.ToList())
                        {
                          //  var userLogin = this.PersistenceSession.Get<Employee>(employeeId).Email; // тут ошибка
                            var userLogin = employeeId.Email;
                            email_list.Add(userLogin);
                            subscribers.Add(userLogin);
                            
                        }

                        tx.Commit();

                        string mailTemplate = @"
                            <p><b>Вы назначены на проект.</b></p>
                            <table>
                                <tr>
                                    <td><b>Проект:</b></br></td><td>{3}</td>
                                </tr>
                                <tr>
                                    <td><b>Заказчик:</b></td><td>{0}</br></td>
                                </tr>
                                <tr>
                                    <td><b>Руководитель проекта:</b></td><td>{1}</br> </td>
                                </tr>
                                <tr>
                                    <td><b>Детали проекта находятся по адресу:</b></td><td>http://timemanager.infocom-ltd.com/ProjectDetails/Index?ProjectID={2}</td>
                                </tr>
                            </table>
                            ";

                        string message = string.Format(mailTemplate, projectToUpdate.Phase.Request.Contract.Customer.Name,
                         projectToUpdate.Phase.Request.ProjectManager.ShortName, projectToUpdate.ID, projectToUpdate.Name);
                        var subject = string.Format("Вы назначены на проект: {0}.", projectToUpdate.Name);
                                              

                        this.WatchdogMailMessageHtml(subscribers, message, subject);

                        
                    }
                    catch (Exception ex)
                    {
                        this.ModelState.AddModelError(
                        String.Empty, string.Format("Загрузка данных прервана из-за ошибки: {0}",ex.Message));
                    }
                    return RedirectToIndex();
                }
            }

            return this.ViewIndex();
        }

        private ActionResult RedirectToIndex()
        {
            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public virtual ActionResult ProjectInsert(
            ProjectModel projectModel)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    try
                    {
                        var modelAssembler = new ModelAssembler();
                        var newProject = modelAssembler.CreateProject(projectModel);

                        
                        var phase= this.PersistenceSession.Load<Request>(projectModel.RequestID).Phases.First();
                        var project = this.PersistenceSession.QueryOver<Project>().Where(p => p.Phase.ID == phase.ID).Future().FirstOrDefault();
                        if (project==null)
                        {
                            var projectType = this.PersistenceSession.Load<ProjectType>(projectModel.ProjectTypeID);
                            var currentSatatus = this.PersistenceSession.Load<Status>((Int64)131073); //новый проект
                            var projectPriority = this.PersistenceSession.Load<ProjectPriority>(projectModel.ProjectPiorityID);
                        //    var request = this.PersistenceSession.Load<Request>(projectModel.Request); // тут добавила

                            // var phase = this.PersistenceSession.Load<Phase>(phaseId);

                            var subscribers = new List<string>();

                            var assignedEmployees =
                               PersistenceSession.QueryOver<Employee>().WhereRestrictionOn(e => e.ID).IsIn(
                                   projectModel.AssignedEmployeeIDs.ToArray()).Future().ToList();

                            var currentEmployees = newProject.Employees.ToList();
                            var newEmployees = assignedEmployees.Except(currentEmployees);

                            newProject.SetEmployees(assignedEmployees);
                            newProject.Phase = phase;
                            newProject.ProjectType = projectType;
                            newProject.CurrentStatus = currentSatatus;
                            newProject.ProjectPriority = projectPriority;
                           // newProject.Phase.Request.StartDate = request.StartDate; //  и тут

                            this.PersistenceSession.Save(newProject);

                            var email_list = new List<string>();

                            foreach (var employeeId in newEmployees.ToList())
                            {
                                var userLogin = employeeId.Email;
                                email_list.Add(userLogin);
                                subscribers.Add(userLogin);

                            }

                            tx.Commit();

                            string mailTemplate = @"
                            <p><b>Вы назначены на проект.</b></p>
                            <table>
                                <tr>
                                    <td><b>Проект:</b></br></td><td>{3}</td>
                                </tr>
                                <tr>
                                    <td><b>Заказчик:</b></td><td>{0}</br></td>
                                </tr>
                                <tr>
                                    <td><b>Руководитель проекта:</b></td><td>{1}</br> </td>
                                </tr>
                                <tr>
                                    <td><b>Детали проекта находятся по адресу:</b></td><td>http://timemanager.infocom-ltd.com/ProjectDetails/Index?ProjectID={2}</td>
                                </tr>
                            </table>
                            ";

                            string message = string.Format(mailTemplate, newProject.Phase.Request.Contract.Customer.Name,
                            newProject.Phase.Request.ProjectManager.ShortName, newProject.ID, newProject.Name);
                            var subject = string.Format("Вы назначены на проект: {0}.", newProject.Name);


                            this.WatchdogMailMessageHtml(subscribers, message, subject);

                        }
                        else
                        {
                           var redirectUrl = string.Format("/ProjectDetails/Index?ProjectID={0}", project.ID);
                           return Redirect(redirectUrl);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.CreateOrederEditFormErrorMessage(string.Format("Загрузка данных прервана из-за ошибки: {0}", ex.Message));
                    }
                    return RedirectToIndex();
                }
            }

            return this.ViewIndex();
        }

        [HttpPost]
        public virtual ActionResult CreateProjectFromRequest(long requestId)
        {
            long projectId = 0;
            using (var tx = this.PersistenceSession.BeginTransaction())
            {
                var request = this.PersistenceSession.QueryOver<Request>().Where(r => r.ID == requestId).Future().FirstOrDefault();
                if (request.Type.ID!=3)
                {
                    string redirectUrl = string.Format("~/ProjectsOverview?Projects-mode=insert&RequestId={0}", request.ID);
                   return this.Redirect(redirectUrl);  
                }
                var phaseId = request.Phases.First().ID;
                var project = this.PersistenceSession.QueryOver<Project>()
                        .Where(ph => ph.Phase.ID == phaseId).Future().FirstOrDefault();
                if (project != null)
                {
                    projectId = project.ID;
                }
                else
                {
                    // var request = this.PersistenceSession.Get<Request>(requestId);
                    var newProject = new Project();
                    newProject.Name = this.CreateProjectName(requestId);
                    newProject.Code = this.CreateProjectCode(requestId);
                    long projectTypeId = request.Type.ID;

                    if (projectTypeId == 4)
                    {
                        projectTypeId = 2;
                    }
                    newProject.ProjectType = this.PersistenceSession.Get<ProjectType>(projectTypeId);
                    newProject.Phase = request.Phases.First();
                    newProject.CurrentStatus = this.PersistenceSession.Get<Status>((long)131073);
                    newProject.ProjectPriority = this.PersistenceSession.Get<ProjectPriority>((long)ProjectPriorityVariant.Средний);
                  //  newProject.Phase.Request.StartDate = request.StartDate; // и тут

                    this.PersistenceSession.Save(newProject);
                    tx.Commit();

                    projectId = this.PersistenceSession.QueryOver<Project>()
                        .Where(ph => ph.Phase.ID == phaseId).Future().First().ID;
                }
            }

            string url = string.Format("~/ProjectDetails/Index?ProjectID={0}", projectId);
            return this.Redirect(url);
        }

        private string CreateProjectCode(long requestId)
        {
            string result = string.Empty;
            var request = PersistenceSession.Get<Request>(requestId);
            if (request != null)
            {
                result = string.Format("{0} {1}",request.Type.Description,request.Number.Substring(0,7));
            }

            return result; 
        }

        private string CreateProjectName(long requestId)
        {
            string result = string.Empty;
            var phase = PersistenceSession.Get<Request>(requestId)
                .Phases.FirstOrDefault();
            if (phase != null)
	        {
		        result = string.Format("{0}: {1}",phase.Name, phase.Request.Contract.Name);
	        }
             
            return result; 
        }

        [HttpPost]
        public virtual ActionResult ProjectDelete(long id)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                        using (var tx = this.PersistenceSession.BeginTransaction())
                        {
                            var projectToDelete = this.PersistenceSession.Get<Project>(id);
                            if (projectToDelete != null)
                            {
                                var taskWithRecords = projectToDelete.Tasks.Where(t => t.TimeRecords.Sum(tr => tr.SpentTime.Value.Ticks)>0).Count();
                                if (taskWithRecords==0)
                                {
                                     this.PersistenceSession.Delete(projectToDelete);
                                    tx.Commit();
                                }
                                else
                                {
                                    this.CreateOrederEditFormErrorMessage("Удаление проекта не возможно, т.к. по проекту велись работы.");
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    this.CreateOrederEditFormErrorMessage(string.Format("Загрузка данных прервана из-за ошибки: {0}", ex.Message));
                }
                return RedirectToIndex();
            }

            return this.ViewIndex();
        }

        private void CreateOrederEditFormErrorMessage(string message)
        {
            this.Session["EditErrorMessage"] = message;
        }

        [HttpPost]
        public virtual ActionResult GetNewRequests()
        {
            var modelAssembler = new ModelAssembler();
            IEnumerable<RequestShortModel> result = new List<RequestShortModel>();
            var allRequests = this.PersistenceSession.QueryOver<Request>().Future();
            var requestsWithAssignedProjects = this.PersistenceSession.QueryOver<Project>()
                    .Future().Select(p => p.Phase.Request);
            result = allRequests.Except(requestsWithAssignedProjects).Where(r => r.IsValidated==true)
                            .Select(modelAssembler.CreateRequestShortModel).ToList().OrderByDescending(p => p.ID);
            return new JsonResult { Data = new SelectList(result, "ID", "Name") };
        }

        [HttpPost]
        public virtual ActionResult GetRequests(string variant)
        {
            var modelAssembler = new ModelAssembler();
            var allRequests = this.PersistenceSession.QueryOver<Request>().Future().Where(r=>r.IsValidated==true);
            var requestsWithAssignedProjects = this.PersistenceSession.QueryOver<Project>()
                    .Future().Select(p => p.Phase.Request).Where(r => r.IsValidated == true);
            var newRequests = allRequests.Except(requestsWithAssignedProjects);
            IEnumerable<RequestShortModel> result = new List<RequestShortModel>();
            switch (variant)
            {
                case "all":
                    result = allRequests.Select(modelAssembler.CreateRequestShortModel);
                    break;
                case "new":
                    result = newRequests.Select(modelAssembler.CreateRequestShortModel);
                    break;
                case "old":
                    result = requestsWithAssignedProjects.Select(p => modelAssembler.CreateRequestShortModel(p));
                    break;
            }
            return new JsonResult { Data = new SelectList(result, "ID", "Name") };
        }

        [HttpPost]
        public virtual ActionResult GetPhaseShortModel(long requestID)
        {
            var modelAssembler = new ModelAssembler();
            IEnumerable<PhaseShortModel> result = new List<PhaseShortModel>();
            result = PersistenceSession.Get<Request>(requestID).Phases.Select(modelAssembler.CreatePhaseShortModel);
            return new JsonResult { Data = new SelectList(result, "ID", "Name") };
        }

        [HttpPost]
        public virtual ActionResult GetPhaseShortModelForNameList(long requestID)
        {
            var modelAssembler = new ModelAssembler();
            IEnumerable<PhaseShortModel> result = new List<PhaseShortModel>();
            result = PersistenceSession.Get<Request>(requestID)
                .Phases.Select(ph => new PhaseShortModel { ID = ph.ID, Name =ph.Name +": "+ ph.Request.Contract.Name });
            return new JsonResult { Data = new SelectList(result, "ID", "Name") };
        }

        [HttpPost]
        public virtual ActionResult GetProjectCode(long requestID)
        {
            var modelAssembler = new ModelAssembler();
            IEnumerable<PhaseShortModel> result = new List<PhaseShortModel>();
            result = PersistenceSession.QueryOver<Request>().Where(r=>r.ID==requestID).Future()
                .Select(r => new PhaseShortModel { 
                    ID = r.ID, 
                    Name = string.Format("{0} {1}",r.Type.Description,r.Number.Substring(0,7)) });
            return new JsonResult { Data = new SelectList(result, "ID", "Name") };
        }

        [HttpPost]
        public virtual ActionResult SearchWord(string searchWord)
        {
            var model = this.CreateModel();
            var projects = model.Projects.Where(p => p.Name.IndexOf(searchWord) >= 0);
            model.Projects = projects;   
            this.InitializeViewData();
            return View("Index",string.Empty,model); 
        }

        #endregion

        #region Methods

        private ViewResult ViewIndex()
        {
            this.InitializeViewData();
            return View("Index");
        }

        internal void InitializeViewData()
        {
            // to be used in edit-template of the grid
            // the model of the view is not available in the edit-template
            // so need to use ViewData, We will use ViewBag as "magic-string" safe
            var modelAssembler = new ModelAssembler();
            // Department 'КД' has ID = 1
            var departmentID = 1;

            var employees = this.PersistenceSession.QueryOver<Employee>().Future();
            IEnumerable<Department> department = this.PersistenceSession.QueryOver<Department>().Future();
            var status = this.PersistenceSession.QueryOver<Status>().Future();
            var types = this.PersistenceSession.QueryOver<ProjectType>().Future();
            var allRequests = this.PersistenceSession.QueryOver<Request>().Future();
            var phases = this.PersistenceSession.QueryOver<Phase>().Future();
            var requestsWithAssignedProjects = this.PersistenceSession.QueryOver<Project>()
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

            var projectEditorViewModel = new ProjectEditorViewModel
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

            var projectPriorities = this.PersistenceSession.QueryOver<ProjectPriority>().Future().Select(modelAssembler.CreateProjectPriorityModel);

            projectEditorViewModel.ProjectPriorities = projectPriorities;

            this.ViewBag.ProjectEditorViewModel = projectEditorViewModel;

            if (this.Session != null)
            {
                if (Session["EditErrorMessage"] != null)
                {
                    this.ViewBag.OverviewErrorMessage = Session["EditErrorMessage"].ToString();
                    Session["EditErrorMessage"] = null;
                }
            }
        }

        private void WatchdogMailMessageHtml(List<string> messageTo, string body, string subject) //html-шаблон для отправки создании задачи
        {
            
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
                /* var exception = ex.InnerException;
                 var message = string.Empty;
                 while (exception != null)
                 {
                     message += exception.Message + ";";
                     exception = exception.InnerException;
                 }
                 this.CreateOrderEditFormErrorMessage(string.Format("Отправка сообщения уведомлений прервана по причине: {0}", message));*/
            }
        }




        internal ProjectsOverviewViewModel CreateModel()
        {
            var filter = new ProjectsOverviewFilter();

            if (this.Session != null)
            {
                if (Session[this._projectsOverviewFilterKey] != null)
                {
                    filter = (ProjectsOverviewFilter)Session[this._projectsOverviewFilterKey];
                }
            }

            var model = new ProjectsOverviewViewModel();
            var assembler = new ModelAssembler();
           
            IEnumerable<Project> persistedProjects;

            persistedProjects = GetPersistedProjects(filter);
            
            var persistedEmployees = this.PersistenceSession.QueryOver<Employee>().Future();

            var projectOverviews = persistedProjects.OrderByDescending(p => p.Phase.Request.Number).Select(assembler.CreateProjectModel);
            var employees = persistedEmployees.Select(e=> new EmployeeViewModel {ID = e.ID, ShortName=e.ShortName});

          
            model.Projects = projectOverviews;
            model.AllEmployees = employees;

                       
            return model;
        }

        private IEnumerable<Project> GetPersistedProjects(ProjectsOverviewFilter filter)
        {
            IEnumerable<Project> persistedProjects;
            if (this.RouteData.Values.ContainsKey("id") && this.Request.Params.AllKeys.Contains("directEdit"))
            {
                Int64 id = Int64.Parse(this.RouteData.Values["id"].ToString());
                persistedProjects =
                this.PersistenceSession.Query<Project>().Where(
                    p => p.ID==id);
                return persistedProjects;
            }

            if (filter.FilteringByAssignedProjects)
            {
                persistedProjects =
                    this.PersistenceSession.Query<Project>().Where(
                        p =>
                        p.Employees.Contains(this.CurrentEmployee.Value) ||
                        p.Tasks.Where(t => t.AssignedEmployees.Contains(this.CurrentEmployee.Value)).Any());
            }
            else
            {
                persistedProjects = this.PersistenceSession.QueryOver<Project>().Future();
            }

            // if the user use the filter, the '0' means that the filter is not applied 
            // '4' means "New and Opened" projects
            if (filter.FilteringByProjectTypeID != 0 )
               
            {
                persistedProjects =
                    persistedProjects.Where(p => p.ProjectType.ID == filter.FilteringByProjectTypeID);
            }
           
            // if the user use the filter, the '0' means that the filter is not applied 
            if (filter.FilteringByStatusID == 131070 || filter.FilteringByStatusID == 131073 ||
                 filter.FilteringByStatusID == 131074)
            {
                persistedProjects =
                    persistedProjects.Where(p => p.CurrentStatus.ID == filter.FilteringByStatusID);
            }

            if (filter.FilteringByStatusID == 4)
            {
                persistedProjects =
                    persistedProjects.Where(p =>
                        p.CurrentStatus.ID != 131074);
            }
            if (!string.IsNullOrEmpty(filter.FilteringBySearchWord))
            {
                persistedProjects =
                   persistedProjects.Where(p => p.Name.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower())>=0 
                       || p.Code.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower())>=0 
                       || p.Phase.Request.Contract.Customer.Name.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower())>=0 );
            }
            return persistedProjects;
        }

       #endregion
    }
}