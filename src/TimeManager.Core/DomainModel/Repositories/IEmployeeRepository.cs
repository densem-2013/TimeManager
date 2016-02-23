namespace Infocom.TimeManager.Core.DomainModel.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        #region Public Methods

        Employee GetByLogin(string login);

        bool Exists(string login);

        #endregion
    }
}