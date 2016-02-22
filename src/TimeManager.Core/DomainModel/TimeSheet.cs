namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
    using System.Collections.Generic;

    using NHibernate.Validator.Constraints;

    [PersistedEntity]
    public class TimeSheet : DomainObject
    {
        #region Constants and Fields

        private ICollection<Task> _tasks;

        private ICollection<Task> _tasksWrapper;

        #endregion

        public TimeSheet()
        {
            this._tasks = new List<Task>();
        }

        [NotNull]
        public virtual Employee Employee { get; set; }

        public virtual ICollection<Task> Tasks
        {
            get
            {
                return this._tasksWrapper ??
                       (this._tasksWrapper =
                        new LinkCollection<Task>(
                            this._tasks, e => e.TimeSheets.Add(this), e => e.TimeSheets.Remove(this)));
            }
        }

        [NotNull]
        public virtual DateTime Date { get; set; }

        public virtual TimeSheetType Type { get; set; }
    }
}
