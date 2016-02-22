namespace Infocom.TimeManager.Core.DomainModel.Repositories
{
    using System;
    using System.Collections.Generic;

    using Infocom.TimeManager.Core.Extensions;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;
    using NHibernate.Linq;

    public class RepositoryBase<T> : IRepository<T>
        where T : DomainObject
    {
        #region Implemented Interfaces

        #region IRepository<T>

        public virtual void Add(T newItem)
        {
            this.GetSession().ExecuteAction((s, t) => { s.Save(newItem); });
        }

        public virtual void Add(IEnumerable<T> items)
        {
            this.GetSession().ExecuteAction((s, t) => items.ForEach(i => s.Save(i)));
        }

        public virtual void Delete(long id)
        {
            this.GetSession().ExecuteAction(
                (s, t) =>
                    {
                        var persistedObject = s.Get<T>(id);
                        if (persistedObject == null)
                        {
                            throw new ObjectNotExistException(String.Format("Object with id '{0}' does not exist.", id));
                        }

                        s.Delete(persistedObject);
                    });
        }

        public virtual bool Exists(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return this.GetSession().ExecuteAction((s, t) => s.Get<T>(item.ID) != null);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return this.GetSession().ExecuteAction((s, t) => s.Query<T>());
        }

        public virtual T GetById(long id)
        {
            var result = this.GetSession().ExecuteAction((s, t) => s.Get<T>(id));
            if (result == null)
            {
                throw new ObjectNotExistException(string.Format("Object with '{0}' id is not found", id));
            }

            return result;
        }

        public virtual bool TryGetById(long id, out T item)
        {
            item = this.GetSession().ExecuteAction((s, t) => s.Get<T>(id));
            return item != null;
        }

        public virtual void Update(T item)
        {
            this.GetSession().ExecuteAction((s, t) => s.Update(item));
        }

        public virtual void Update(IEnumerable<T> items)
        {
            this.GetSession().ExecuteAction((s, t) => items.ForEach(i => s.Update(i)));
        }

        #endregion

        #endregion

        #region Methods

        protected ISession GetSession()
        {
            return ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession();
        }

        #endregion
    }
}