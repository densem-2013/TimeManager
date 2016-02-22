namespace Infocom.TimeManager.Core.DomainModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Infocom.TimeManager.Core.Extensions;

    using NHibernate.Linq;

    public class TaskRepository : RepositoryBase<Task>, ITaskRepository
    {
        #region Public Methods

        public void BookTime(TimeRecord timeToBook)
        {
            if (timeToBook == null)
            {
                throw new ArgumentNullException("timeToBook");
            }

            this.GetSession().ExecuteAction((s, t) => s.Save(timeToBook));
        }

        #endregion

        #region Implemented Interfaces

        #region IRepository<Task>

        public override void Delete(long id)
        {
            this.GetSession().ExecuteAction(
                (s, t) =>
                    {
                        var task = s.Get<Task>(id);
                        if (task == null)
                        {
                            throw new ObjectNotExistException(String.Format("Object with id '{0}' does not exist.", id));
                        }

                        if (task.Project != null)
                        {
                            task.Project.Tasks.Remove(task);
                        }

                        s.Delete(task);
                    });
        }

        #endregion

        #region ITaskRepository

        public IEnumerable<Employee> GetAssignedEmployees(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            return this.GetSession().Query<Employee>().Where(e => e.Tasks.Contains(task));
        }

        public IEnumerable<Task> GetTasksAssignedToLogin(string loginName)
        {
            var employee = this.GetSession().Query<Employee>().Where(e => e.Login == loginName).Single();

            return employee.Tasks;
        }

        public IEnumerable<TimeSheet> GetTimeSheets(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            return this.GetSession().Query<TimeSheet>().Where(e => e.Tasks.Contains(task));
        }


        #endregion

        #endregion
    }
}