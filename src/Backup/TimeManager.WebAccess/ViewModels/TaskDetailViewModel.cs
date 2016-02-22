namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System;
    using System.ComponentModel;

    public class TaskDetailViewModel : BaseViewModel
    {
        #region Properties

        [DisplayName(@"Суммарное затраченое время")]
        public TimeSpan TotalSpentTime { get; set; }

        #endregion
    }
}