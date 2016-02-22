namespace Infocom.TimeManager.Core.DomainModel.Repositories
{
    using System.Collections.Generic;

    public interface IRepository<T>
    {
        #region Public Methods

        IEnumerable<T> GetAll();

        T GetById(long id);

        bool TryGetById(long id, out T item);

        void Add(T newItem);

        void Add(IEnumerable<T> newItem);

        void Delete(long id);

        void Update(T project);

        void Update(IEnumerable<T> project);

        bool Exists(T item);

        #endregion
    }
}