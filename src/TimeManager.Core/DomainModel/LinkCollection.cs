namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
    using System.Collections.Generic;

    public class LinkCollection<T> : AbstractFlexibleCollection<T>
    {
        #region Constants and Fields

        private readonly Action<T> _addAction;

        private readonly Action<T> _removeAction;

        #endregion

        #region Constructors and Destructors

        public LinkCollection(ICollection<T> target, Action<T> addAction, Action<T> removeAction)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (addAction == null)
            {
                throw new ArgumentNullException("addAction");
            }

            if (removeAction == null)
            {
                throw new ArgumentNullException("removeAction");
            }

            this.Items = target;
            this._addAction = addAction;
            this._removeAction = removeAction;
        }

        #endregion

        #region Implemented Interfaces

        #region ICollection<T>

        public override void Add(T assignedEmployees)
        {
            if (!this.Items.Contains(assignedEmployees))
            {
                this.Items.Add(assignedEmployees);
                this._addAction(assignedEmployees);
            }
        }

        public override bool Remove(T assignedEmployees)
        {
            if (this.Items.Contains(assignedEmployees))
            {
                var result = this.Items.Remove(assignedEmployees);
                this._removeAction(assignedEmployees);
                return result;
            }

            return true;
        }

        #endregion

        #endregion
    }
}