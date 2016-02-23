namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using NHibernate.Linq;
    
    using System.Data.SqlClient;
    using System.Data;
    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Infrastructure;
    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;
    using Infocom.TimeManager.WebAccess.Filters;


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
    using System.Collections.Generic;    
    

    using Castle.Components.DictionaryAdapter;
    using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
    using System.DirectoryServices;


    [HandleError]
    public partial class ProjectDetailsController : TimeManagerBaseController
    {
        #region Constants and Fields

        private RoleManagment _userRoleService;

        #endregion

        #region Constructors and Destructors

        public ProjectDetailsController()
        {
            this._userRoleService = new RoleManagment(this);
        }

        #endregion

        #region Public Methods

        public virtual ActionResult Index(long projectID)
        {
            var model = this.CreateModel(projectID);
            this.InitializeViewData(model);

            return View(model);
        }

        public virtual ActionResult CloseProject(string projectID)
        {
            long result = 0;
            if (long.TryParse(projectID, out result))
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    var projectCloseStatus = this.PersistenceSession.Get<Status>((long)131074);
                    var taskCloseStatus = this.PersistenceSession.Get<Status>((long)131075);

                    var projectForUpdate = this.PersistenceSession.Get<Project>(long.Parse(projectID));
                    projectForUpdate.CurrentStatus = projectCloseStatus;
                    projectForUpdate.Tasks.ForEach(t => t.CurrentStatus = taskCloseStatus);
                    projectForUpdate.Tasks.ForEach(t => t.FinishDate = DateTime.Now);
                    this.PersistenceSession.Update(projectForUpdate);
                    tx.Commit();
                  }
            }
            return this.ViewIndex(result);
        }


        /*[HttpPost]
        public virtual ActionResult UpdateProject(ProjectModel project)
        {
            string redirectUrl = string.Format("~/ProjectsOverview/Index/{0}?Projects-mode=edit", project.ID);
            return this.Redirect(redirectUrl);  
        }*/

        [HttpPost]
        public virtual ActionResult TaskInsert(TaskModel taskModel)
        {
            if (this.ModelState.IsValid)
            {
                var newTask = new Task();
                if (this.TryUpdateModel(newTask))
                {
                    using (var tx = this.PersistenceSession.BeginTransaction())
                    {
                        var persistedProject = this.PersistenceSession.Get<Project>(taskModel.ProjectID);
                        if (persistedProject == null)
                        {
                            this.ModelState.AddModelError(
                                String.Empty,
                                String.Format(
                                    "Проект с идентификатором '{0}' не найден в базе данных.", taskModel.ProjectID));

                            return ViewIndex(taskModel);
                        }
                        newTask.Project = persistedProject;

                        var currentStatus = this.PersistenceSession.Get<Status>(taskModel.TaskStatusID);
                        if (currentStatus.ApplicableTo != DomailEntityType.Task)
                        {
                            var errMessage = string.Format(
                                "Статус '{0}' не пременим к задаче '{1}.", taskModel.TaskStatusID, taskModel.ID);
                            this.ModelState.AddModelError(string.Empty, errMessage);
                            return ViewIndex(taskModel);
                        }
                        newTask.CurrentStatus = currentStatus;

                        var subscribers = new List<string>();

                        var assignedEmployees =
                            this.PersistenceSession.QueryOver<Employee>().WhereRestrictionOn(e => e.ID).IsIn(
                                taskModel.AssignedEmployeeIDs.ToArray()).Future().ToList();
                        newTask.SetEmployees(assignedEmployees);

                         var email_list = new List<string>();

                       
                        
                        this.PersistenceSession.Save(newTask);
                        
                        var projectToUpdate = this.PersistenceSession.Get<Project>(taskModel.ProjectID);
                        foreach (var assignedEmployeeID in taskModel.AssignedEmployeeIDs)
                        {
                            if (!persistedProject.Employees.Where(e => e.ID == assignedEmployeeID).Any())
                            {          
                                var employee =  this.PersistenceSession.Get<Employee>(assignedEmployeeID);
                                var userLogin = this.PersistenceSession.Get<Employee>(assignedEmployeeID).Email;
                                email_list.Add(userLogin);
                                subscribers.Add(userLogin);
                                projectToUpdate.Employees.Add(employee);
                            }
                        }

                        this.PersistenceSession.Save(projectToUpdate);
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
                                              
                        //
                        this.WatchdogMailMessageHtml(subscribers, message, subject);


                        
                                                                    
                        return this.RedirectToAction("Index", new { taskModel.ProjectID });
                                            
                    }

                }

                //todo: find better solution
                this.AddErrorMessagesFromUpdateTaskModel(newTask);
                return this.ViewIndex(taskModel);
            }

            return this.ViewIndex(taskModel.ProjectID);
        }

        private ActionResult ViewIndex(List<string> email_list)
        {
            throw new NotImplementedException();
        }

              
                
        [HttpPost]
        public virtual ActionResult TaskUpdate(TaskModel taskModel)
        {
            if (this.ModelState.IsValid)
            {
                var taskToUpdate = this.PersistenceSession.Get<Task>(taskModel.ID);
                if (this.TryUpdateModel(taskToUpdate))
                {
                    using (var tx = this.PersistenceSession.BeginTransaction())
                    {
                        var subscribers = new List<string>();
                        var assignedEmployees =
                            this.PersistenceSession.QueryOver<Employee>().WhereRestrictionOn(e => e.ID).IsIn(
                                taskModel.AssignedEmployeeIDs.ToArray()).Future().ToList();
                        taskToUpdate.SetEmployees(assignedEmployees);

                        var newStatus = this.PersistenceSession.Get<Status>(taskModel.TaskStatusID);
                        if (newStatus.ApplicableTo != DomailEntityType.Task)
                        {
                            var errMessage = string.Format(
                                "Статус '{0}' не пременим к задаче '{1}.", taskModel.TaskStatusID, taskModel.ID);
                            this.ModelState.AddModelError(string.Empty, errMessage);
                            return ViewIndex(taskModel);
                        }

                        if (newStatus.Name == "Открытая" && taskToUpdate.StartDate == null)
                        {
                            taskToUpdate.StartDate = DateTime.Now;
                        }

                        if (newStatus.ID == 131075)
                        {
                            taskToUpdate.FinishDate = DateTime.Now;
                        }
                        var email_list = new List<string>();

                        taskToUpdate.CurrentStatus = newStatus;
                        this.PersistenceSession.SaveOrUpdate(taskToUpdate);
                        var projectToUpdate = this.PersistenceSession.Get<Project>(taskModel.ProjectID);
                        foreach (var assignedEmployeeID in taskModel.AssignedEmployeeIDs)
                        {
                            if (!projectToUpdate.Employees.Where(e => e.ID == assignedEmployeeID).Any())
                            {
                                var employee = this.PersistenceSession.Get<Employee>(assignedEmployeeID);
                                var userLogin = this.PersistenceSession.Get<Employee>(assignedEmployeeID).Email;
                                email_list.Add(userLogin);
                                subscribers.Add(userLogin);
                                projectToUpdate.Employees.Add(employee);
                            }
                        }

                        this.PersistenceSession.Save(projectToUpdate);

                        tx.Commit();
                    //
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

                        return this.RedirectToAction("Index", new { taskModel.ProjectID });
                    }
                }

                this.AddErrorMessagesFromUpdateTaskModel(taskToUpdate);
                return this.ViewIndex(taskModel);
            }

            return this.ViewIndex(taskModel.ProjectID);
        }

        [HttpPost]
        public virtual ActionResult TaskDelete(TaskModel taskModel)
        {
            using (var tx = this.PersistenceSession.BeginTransaction())
            {
                var persistedTaskToDelete = this.PersistenceSession.Get<Task>(taskModel.ID);
                if (persistedTaskToDelete == null)
                {
                    this.ModelState.AddModelError(
                        String.Empty,
                        string.Format("Задача с идентификатором '{0}' не найдена в базе данных.", taskModel.ID));
                    return this.View("Index");
                }

                var isOwner = persistedTaskToDelete.AssignedEmployees.Contains(this.CurrentEmployee.Value);
                if (isOwner || this._userRoleService.IsAdmin())
                {
                    
                    if (persistedTaskToDelete.TimeRecords.Sum(x=>x.SpentTime.Value.Ticks)>0)
                    {                   
                        Session["ErrorMessage"] = string.Format("Задача {0} не может быть удалена, по причине списанного времени."
                         + "Сначала очистите время по задаче.", taskModel.Name);                        
                        this.ViewBag.OverviewErrorMessage = Session["ErrorMessage"].ToString();
                        Session["ErrorMessage"] = null;
                        var model = this.CreateModel(taskModel.ProjectID);
                        this.InitializeViewData(model);
                        return View("Index", model);
                    }
                    else
                    {
                        SqlConnection con = new SqlConnection(PersistenceSession.Connection.ConnectionString);
                        SqlCommand cmd = new SqlCommand("prDeleteTaskRecordsFromEmployeeMonthCost", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        cmd.Parameters.Add("@taskId", SqlDbType.BigInt);
                        cmd.Parameters["@taskId"].Value = persistedTaskToDelete.ID;
                        cmd.ExecuteNonQuery();
                        con.Close();
                        this.PersistenceSession.Delete(persistedTaskToDelete);
                    }             
                    tx.Commit();
                }
            }

            return this.RedirectToAction("Index", new { taskModel.ProjectID });
        }

        #endregion

        #region Methods

        private void AddErrorMessagesFromUpdateTaskModel(Task newTask)
        {
            try
            {
                this.UpdateModel(newTask);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(String.Empty, string.Format("Ошибки валидации: {0}", ex.Message));
            }
        }

        private ViewResult ViewIndex(long projectID)
        {
            var model = this.CreateModel(projectID);
            this.InitializeViewData(model);
            return View("Index", model);
        }

        private ViewResult ViewIndex(TaskModel model)
        {
            return View("Index", model);
        }

        internal ProjectDetailsViewModel CreateModel(long projectID)
        {
            var model = new ProjectDetailsViewModel();
            var assembler = new ModelAssembler();
            Project persistedProject = new Project();

            // Use Get project, because has problem with Load checking  
            persistedProject = this.PersistenceSession.Get<Project>(projectID);
            if (persistedProject == null)
            {
                this.ModelState.AddModelError(String.Empty, string.Format("Проект с ID='{0}' не найден.", projectID));
            }
            else
            {
                var projectModel =  assembler.CreateProjectModel(persistedProject);
                var contract = persistedProject.Phase.Request.Contract;
                if (contract != null)
                {
                    projectModel.Contract = assembler.CreateContractModel(contract);
                }
                var request = persistedProject.Phase.Request; // присоединение Даты начала работ
                if (request != null)
                {
                    projectModel.Request = assembler.CreateRequestModel(request);
                }                                             //присоединение Даты начала работ

                model.Project = projectModel;
                model.Tasks = persistedProject.Tasks.Select(assembler.CreateTaskModel).OrderByDescending(o=>o.ID);
            }

            return model;
        }

        private void WatchdogMailMessageHtml(List<string> messageTo, string body, string subject) //html-шаблон для отправки создании задачи
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

    
        private void InitializeViewData(ProjectDetailsViewModel model)
        {
            var assembler = new ModelAssembler();
            var currentProject = this.PersistenceSession.Get<Project>(model.Project.ID);
            var allEmployees = this.PersistenceSession.QueryOver<Employee>().Future();
            var taskStatuses =
                this.PersistenceSession.QueryOver<Status>().Where(s => s.ApplicableTo == DomailEntityType.Task).Future();
            var phases = this.PersistenceSession.QueryOver<Phase>().Future().Select(assembler.CreatePhaseModel);
            
            var projectDetailsEdit = new ProjectDetailsEditViewModel
                {
                    AllEmployees = allEmployees.OrderBy(e => e.ShortName).Select(assembler.CreateEmployeeModel),
                    Phases = phases,
                    TaskStatuses = taskStatuses.Select(assembler.CreateStatusModel)

                };

            this.ViewBag.ProjectDetailsEditViewModel = projectDetailsEdit;
        }

              
        #endregion
    }
}