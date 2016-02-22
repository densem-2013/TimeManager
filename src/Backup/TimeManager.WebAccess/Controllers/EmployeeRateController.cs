using System.Web.Security;

namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Infrastructure;
    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;

    [HandleError]
    public class EmployeeRateController : TimeManagerBaseController
    {

        public virtual ActionResult SelectIndex(long employeeId)
        {
            // var employeeRate = PersistenceSession.Get<EmployeeRate>(id);
            var modelAssembler = new ModelAssembler();
            var model = new EmployeeRateViewModel();

            model.Employee = modelAssembler.CreateEmployeeModel(PersistenceSession.Get<Employee>(employeeId));
            IEnumerable<EmployeeRate> employeeRates =
                PersistenceSession.QueryOver<EmployeeRate>().Where(er => er.Employee.ID == employeeId).Future();
            List<EmployeeRateModel> rates = new List<EmployeeRateModel>();
            foreach (var employeeRate in employeeRates)
            {
                EmployeeRateModel modelnew = modelAssembler.CreateEmployeeRateModel(employeeRate);
                rates.Add(modelnew);
            }

            model.EmployeeRates = rates;

            return View("Index", model);
        }

        [HttpPost]
        public virtual ActionResult EmployeeRateDelete(EmployeeRateModel model)
        {
            long employeeId;
            using (var tx = this.PersistenceSession.BeginTransaction())
            {
                EmployeeRate updateRate = PersistenceSession.Get<EmployeeRate>(model.ID);
                employeeId = updateRate.Employee.ID;
                IEnumerable<EmployeeRate> employeeRates =
               PersistenceSession.QueryOver<EmployeeRate>().Where(er => er.Employee.ID == employeeId).Future();
              
                foreach (var employeeRate in employeeRates)
                {
                    if (employeeRate.RateNumber == MaxRateNumberForEmployee(employeeId)-1)
                    {
                        Employee emp = PersistenceSession.Get<Employee>(employeeId);
                        emp.Rate = employeeRate.Rate;

                        PersistenceSession.Update(emp);
                    }
                }
                PersistenceSession.Delete(updateRate);
                tx.Commit();
                return RedirectToAction("Index", "EmployeeRate", new { id = employeeId });
            }
        }

        [HttpPost]
        public virtual ActionResult EmployeeRateUpdate(EmployeeRateModel model)
        {
            using (var tx = this.PersistenceSession.BeginTransaction())
            {
                EmployeeRate updateRate = PersistenceSession.Get<EmployeeRate>(model.ID);
                updateRate.Rate = model.Rate;
                updateRate.StartDate = model.StartDate;
                updateRate.FinishDate = model.FinishDate;
                Employee employee = PersistenceSession.Get<Employee>(model.Employee.ID);
                employee.Rate = model.Rate;
                PersistenceSession.Update(employee);
                PersistenceSession.Update(updateRate);
                tx.Commit();
               return RedirectToAction("Index", "EmployeeRate", new { id = model.Employee.ID });
            }
        }

        [HttpPost]
        public virtual ActionResult EmployeeRateInsert(long employeeId, EmployeeRateModel newRateModel)
        {
            using (var tx = this.PersistenceSession.BeginTransaction())
            {
                var modelAssembler = new ModelAssembler();
                EmployeeRate newEmployeeRateViewModel = new EmployeeRate();
                EmployeeModel EmployeeModel = modelAssembler.CreateEmployeeModel(PersistenceSession.Get<Employee>(employeeId));
                newEmployeeRateViewModel.Employee = modelAssembler.ConvertToEmployee(EmployeeModel);
                newEmployeeRateViewModel.Rate = newRateModel.Rate;
                newEmployeeRateViewModel.Employee.Rate = newRateModel.Rate;
                newEmployeeRateViewModel.RateNumber = MaxRateNumberForEmployee(employeeId) + 1;
                newEmployeeRateViewModel.StartDate = newRateModel.StartDate;
                newEmployeeRateViewModel.FinishDate = newRateModel.FinishDate;
                Employee employee = PersistenceSession.Get<Employee>(employeeId);
                employee.Rate = newRateModel.Rate;
                ChangeDateForForwardRate(employeeId, newRateModel.StartDate.Value.AddDays(-1));
                PersistenceSession.Update(employee);
                PersistenceSession.Save(newEmployeeRateViewModel);
                tx.Commit();
                return RedirectToAction("Index", "EmployeeRate", new { id = employeeId });
            }
        }

        private int MaxRateNumberForEmployee(long employeeId)
        {
            return PersistenceSession.QueryOver<EmployeeRate>()
                .Where(er => er.Employee.ID == employeeId)
                .Future()
                .Select(i => i.RateNumber)
                .Max();
        }

        private void ChangeDateForForwardRate(long employeeId, DateTime finishDate)
        {
            EmployeeRate employeeRate = PersistenceSession.QueryOver<EmployeeRate>()
                .Where(er => er.Employee.ID == employeeId)
                .Where(er => er.RateNumber == this.MaxRateNumberForEmployee(employeeId))
                .Future()
                .ToList().Single();

            employeeRate.FinishDate = finishDate;
            PersistenceSession.Update(employeeRate);
        }

        public virtual ActionResult Index(long id = 0)
        {
            
                var modelAssembler = new ModelAssembler();
                var model = new EmployeeRateViewModel();
                if (Roles.IsUserInRole("TimeManagerAdministrators"))
                {
                    if (id != 0)
                    {
                        model.Employee = modelAssembler.CreateEmployeeModel(PersistenceSession.Get<Employee>(id));
                        IEnumerable<EmployeeRate> employeeRates =
                            PersistenceSession.QueryOver<EmployeeRate>().Where(er => er.Employee.ID == id).Future();
                        List<EmployeeRateModel> rates = new List<EmployeeRateModel>();
                        foreach (var employeeRate in employeeRates.OrderBy(er => er.RateNumber))
                        {
                            EmployeeRateModel modeltest = modelAssembler.CreateEmployeeRateModel(employeeRate);
                            rates.Add(modeltest);
                        }

                        model.EmployeeRates = rates;
                    }
                }
                else
                {
                    this.Session["ErrorMessage"] = "Для доступа на данную страницу недостаточно прав. Вы не являетесь администратором для просмотра этой страницы.";
                    ViewBag.OverviewErrorMessage = Session["ErrorMessage"].ToString();
                    return View(model);
                }

            if (Session["ErrorMessage"] != null)
                    {
                        this.ViewBag.OverviewErrorMessage = Session["ErrorMessage"].ToString();
                        Session["ErrorMessage"] = null;
                    }
                
              
            return View(model);
        }
    }
}
