using System.Collections.Generic;

namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;
    using System.Web.Routing;
    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Infrastructure;

    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;

    using NHibernate.Linq;

    [HandleError]
    public partial class EmployeeController : TimeManagerBaseController
    {
        #region Public Methods

        public virtual ActionResult Index()
        {
            var modelAssembler = new ModelAssembler();
            EmployeesViewModel model = new EmployeesViewModel();
            if (Roles.IsUserInRole("TimeManagerAdministrators"))
            {
             
                model = new EmployeesViewModel
                               {
                                   Employees =
                                       this.CurrentEmployee.Value.Department.Employees.Select(
                                           modelAssembler.CreateEmployeeModel).OrderBy(e => e.LastName)
                               };
                
            }
            if ((bool)Session["HumanResource"])
            {
                model = null;


                IQueryable<Employee> emloyees = PersistenceSession.Query<Employee>().Where(e=>e.FireDate.HasValue==false);
                IQueryable<Department> departments = PersistenceSession.Query<Department>();
               var newModel = new EmployeesViewModel
                {
                    Employees = emloyees
                       .Select(
                            modelAssembler.CreateEmployeeModel).OrderBy(e => e.LastName)
                
                };
               foreach (var modemp in newModel.Employees)
               {
                   var departmentModel = departments.FirstOrDefault(d => d.ID == modemp.DepartmentID);
                   if (departmentModel != null)
                       modemp.DepartmentShortName = departmentModel.ShortName;
                   modemp.IsHumanResource = modemp.HumanResource == 1;
               }

                IEnumerable<SelectListItem> departmentList = new SelectList(departments.Distinct().ToList(), "ID", "ShortName");

                this.ViewData["Departments"] = departments.ToList();
               return View(newModel);
            }
            else if (!(bool)Session["HumanResource"] || !Roles.IsUserInRole("TimeManagerAdministrators"))
         
            {
                Session["ErrorMessage"] = "Для доступа на данную страницу недостаточно прав. Вы не являетесь администратором для просмотра этой страницы.";
            }
            if (Session["ErrorMessage"] != null)
            {
                this.ViewBag.OverviewErrorMessage = Session["ErrorMessage"].ToString();
                Session["ErrorMessage"] = null;
            }
            return View(model);
        }
        // Начальники отделов могут заходить в систему от имени сотрудников своего отдела
        public virtual ActionResult LogonAs(string Login)
        {
            if (!Roles.IsUserInRole("TimeManagerAdministrators"))
            {
                this.Session["ErrorMessage"] = "Недостаточно прав.";
                return this.RedirectToAction("Index");
            }
            long departmentId = (long)this.Session["userDepartmentId"];
            if (departmentId == 1)
            {
                this.Session["ErrorMessage"] = "Не разрешено"; // Коммерческий департамент
                return this.RedirectToAction("Index");
            }
            if (this.Session["userName"] != this.Session["authenticatedUserName"])
            {
                this.Session["ErrorMessage"] = "Невозможно повторно сменить пользователя. Используйте кнопку 'Выход'"; // 
                return this.RedirectToAction("Index");
            }

            var isValid = PersistenceSession.Query<Employee>().Where((e => (e.Login == Login && e.Department.ID == departmentId))).Any();
            if (isValid)
            {
                var user = PersistenceSession.Query<Employee>().Where(e => e.Login == Login).Single();
                this.Session["userFullName"] = user.LastName + " " + user.FirstName.Substring(0, 1) + ". " + user.PatronymicName.Substring(0, 1) + ".";
                this.Session["userName"] = user.Login;
                FormsAuthentication.SetAuthCookie(user.Login, false);
                return this.RedirectToAction("Index", "TimeRegistration");

            }
            this.Session["ErrorMessage"] = "Пользователь не найден";
            return this.RedirectToAction("Index");

        }
        [HttpPost]
        public virtual ActionResult EmployeeInsert(EmployeeModel employeeModel)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    var modelAssembler = new ModelAssembler();
                    
                    var department = PersistenceSession.Get<Department>(employeeModel.DepartmentID);
                    long managerId = department.Manager.ID;
                    var manager = PersistenceSession.Get<Employee>(managerId);
                    employeeModel.ManagerID = managerId;
                    employeeModel.Login = @"INFOCOM-LTD\"+ employeeModel.Login;
                    Employee newEmloyee = modelAssembler.ConvertToEmployee(employeeModel);
                    newEmloyee.Department = department;
                    newEmloyee.Manager = manager;
                    newEmloyee.HumanResource = employeeModel.IsHumanResource ? 1 : 0;
                    PersistenceSession.Save(newEmloyee);
                    EmployeeRate newEmployeeRate = new EmployeeRate
                                                       {
                                                           Employee = newEmloyee,
                                                           StartDate = newEmloyee.HireDate,
                                                           RateNumber = 1,
                                                           Rate = newEmloyee.Rate,
                                                           FinishDate = DateTime.MaxValue
                                                       };
                    PersistenceSession.Save(newEmployeeRate);

                    tx.Commit();
                }
                return this.RedirectToAction("Index");
            }

         
            this.ModelState.AddModelError(string.Empty, "Страница содержит ошибки.");
            return this.View();
        }

        [HttpPost]
        public virtual ActionResult EmployeeUpdate(EmployeeModel employeeModel)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    var employeeUpdated = this.PersistenceSession.Get<Employee>(employeeModel.ID);
                    var department = PersistenceSession.Get<Department>(employeeModel.DepartmentID);
                    long managerId = department.Manager.ID;
                    var manager = PersistenceSession.Get<Employee>(managerId);
                    employeeUpdated.Department = department;
                    employeeUpdated.ApprovePermission = 0;
                    employeeUpdated.Email = employeeModel.Email;
                    employeeUpdated.LastName = employeeModel.LastName;
                    employeeUpdated.FirstName = employeeModel.FirstName;
                    employeeUpdated.PatronymicName = employeeModel.PatronymicName;
                    employeeUpdated.Manager = manager;
                    employeeUpdated.HumanResource = employeeModel.IsHumanResource ? 1 : 0;
                    if (employeeModel.FireDate.HasValue)
                    {
                        employeeUpdated.FireDate = employeeModel.FireDate;
                    }

                    int newEmployeeRate = PersistenceSession.QueryOver<EmployeeRate>()
                        .Where(er => er.Employee.ID == employeeUpdated.ID)
                        .Future().Count();
              
                    if (newEmployeeRate==1)
                    {
                        EmployeeRate updatedRate = 
                            PersistenceSession.QueryOver<EmployeeRate>()
                            .Future()
                            .FirstOrDefault(er => er.Employee.ID == employeeUpdated.ID);
                        if (updatedRate != null)
                        {
                            updatedRate.Rate = employeeModel.Rate;
                            updatedRate.StartDate = employeeModel.HireDate;
                            employeeUpdated.Rate = employeeModel.Rate;
                        }
                        PersistenceSession.Update(updatedRate);
                    }
                    this.PersistenceSession.Update(employeeUpdated);

                    tx.Commit();
                }

                return this.RedirectToAction("Index");
            }
            this.ModelState.AddModelError(string.Empty, "Страница содержит ошибки.");
            return this.View();
        }

        #endregion
    }
}