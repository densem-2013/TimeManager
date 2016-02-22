using Infocom.TimeManager.Core.Services;
using Telerik.Web.Mvc.UI;
using Telerik.Web.Mvc.UI.Fluent;

namespace Infocom.TimeManager.WebAccess.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Infrastructure;

    using Infocom.TimeManager.WebAccess.Models;

    using NHibernate;

    public static class MVCHelpers
    {
        #region Public Methods

        public static TResult SafeGet<TTarget, TResult>(
            this TTarget target, Func<TTarget, TResult> funcToTry, TResult defaultValue)
        {
            try
            {
                return funcToTry(target);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static TResult SafeGet<TTarget, TResult>(
            this TTarget target, Func<TTarget, TResult> funcToTry, Func<TResult> funcToTry2, TResult defaultValue)
        {
            try
            {
                return funcToTry(target);
            }
            catch
            {
                try
                {
                    return funcToTry2();
                }
                catch
                {
                    return defaultValue;
                }
            }
        }

        public static string ReduceTo(this string target, int maxLength)
        {
            const string replacement = " ...";
            if (maxLength < replacement.Length)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "MaxLength should be greater then the replacement. Replacement length is '{0}'", replacement));
            }

            string result = target;
            if (!string.IsNullOrEmpty(target))
            if (target.Length > maxLength)
            {
                result = target.Substring(0, maxLength - replacement.Length) + replacement;
            }

            return result;
        }

        public static IEnumerable<EmployeeModel> GetAssignedEmployees(
            IEnumerable<EmployeeModel> AllEmployees,
            IEnumerable<long> assignedEmployeeIDs,
            IEnumerable<EmployeeModel> assignedEmployees)
        {
            var result = new List<EmployeeModel>();
            if (assignedEmployeeIDs != null)
            {
                IEnumerable<EmployeeModel> employeeList = new List<EmployeeModel>();
                if (assignedEmployeeIDs.Count() > 0)
                {
                    employeeList = AllEmployees.Where(pe => assignedEmployeeIDs.Contains(pe.ID));
                }
                else
                {
                    if (assignedEmployees != null)
                    {
                        employeeList = AllEmployees.Where(assignedEmployees.Contains);
                    }
                }

                result.AddRange(employeeList);
            }

            return result;
        }

        public static IEnumerable<EmployeeModel> GetProjectEmployees(
            IEnumerable<long> assignedEmployeeIDs,
            IEnumerable<EmployeeModel> projectEmployees,
            IEnumerable<EmployeeModel> assignedEmployees)
        {
            var result = new List<EmployeeModel>();

            IEnumerable<EmployeeModel> employeeList = new List<EmployeeModel>();
            if (assignedEmployeeIDs.Count() > 0)
            {
                employeeList = projectEmployees.Where(pe => !assignedEmployeeIDs.Contains(pe.ID));
            }
            else
            {
                if (assignedEmployees != null)
                {
                    employeeList = projectEmployees.Where(pe => !assignedEmployees.Contains(pe));
                }
                else
                {
                    employeeList = projectEmployees;
                }
            }

            foreach (var item in employeeList)
            {
                result.Add(item);
            }

            return result;
        }

        public static IEnumerable<BudgetAllocationModel> GetFillBudgetAllocationsFromSources(
            IEnumerable<BudgetAllocationModel> budgetAllocations, IEnumerable<BudgetAllocationModel> budgetAllocationModels)
        {
            IEnumerable<BudgetAllocationModel> result; // = new List<BudgetAllocationModel>();

            if (budgetAllocations.Count() == 0)
            {
                result = budgetAllocationModels;
            }
            else
            {
                result = budgetAllocations;
            }

            return result;
        }

        public static IEnumerable<EmployeeModel> GetAssignedEmployeesViewState(ISession persistenceSession, long[] AssignedEmployeeIDs)
        {
            var assignedEmployees = new List<EmployeeModel>();
            if (AssignedEmployeeIDs != null)
            {
                var modelAssembler = new ModelAssembler();
                assignedEmployees.AddRange(
                    persistenceSession.QueryOver<Employee>().Future().Select(modelAssembler.CreateEmployeeModel).Where(
                        e => AssignedEmployeeIDs.Contains(e.ID)));
            }

            return assignedEmployees;
        }

        public static List<string> GetBudgetAllocationForViewModel(List<BudgetAllocationModel> allocations)
        {
            var result = new List<string>();
            foreach (var allocation in allocations)
            {
                if (allocation.AmountOfHours > 0 || allocation.AmountOfMoney > 0)
                {
                    result.Add(
                        string.Format("{0} ({1} чел/час)", allocation.Department.ShortName, allocation.AmountOfHours));
                }
            }

            return result;
        }

        public static int MaxRateNumberForEmployee(long employeeId)
        {
            int result = 0;
            var PersistenceSession = NHibernateContext.Current().Session;
           
            result = PersistenceSession.QueryOver<EmployeeRate>()
                .Where(er => er.Employee.ID == employeeId)
                .Future()
                .Select(i => i.RateNumber)
                .Max();
            return result;
        }

       #endregion
    }
}