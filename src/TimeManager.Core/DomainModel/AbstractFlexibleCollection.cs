namespace Infocom.TimeManager.Core.DomainModel
{
    using System.Collections;
    using System.Collections.Generic;

    public abstract class AbstractFlexibleCollection<T> : ICollection<T>
    {
        #region Constants and Fields

        private ICollection<T> _items;

        #endregion

        #region Constructors and Destructors

        public AbstractFlexibleCollection()
        {
            this._items = new List<T>();
        }

        #endregion

        #region Properties

        public virtual int Count
        {
            get
            {
                return this.Items.Count;
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return this.Items.IsReadOnly;
            }
        }

        protected virtual ICollection<T> Items
        {
            get
            {
                return this._items;
            }

            set
            {
                this._items = value;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region ICollection<T>

        public virtual void Add(T item)
        {
            this.Items.Add(item);
        }

        public virtual void Clear()
        {
            this.Items.Clear();
        }

        public virtual bool Contains(T item)
        {
            return this.Items.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            this.Items.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(T item)
        {
            return this.Items.Remove(item);
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IEnumerable<T>

        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #endregion
    }
}