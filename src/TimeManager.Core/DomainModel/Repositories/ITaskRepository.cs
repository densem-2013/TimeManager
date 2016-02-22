namespace Infocom.TimeManager.Core.DomainModel.Repositories
{
    using System.Collections.Generic;

    public interface ITaskRepository : IRepository<Task>
    {
        #region Public Methods

        IEnumerable<Task> GetTasksAssignedToLogin(string loginName);

        IEnumerable<Employee> GetAssignedEmployees(Task task);

        #endregion
    }
}