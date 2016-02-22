namespace Infocom.TimeManager.Core.Services.Dto
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public class TaskDto
    {
        #region Properties

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int ParentTaskID { get; set; }

        [DataMember]
        public string CurrentStatusName { get; set; }

        [DataMember]
        public int ProjectID { get; set; }

        [DataMember]
        public int[] AssignedEmployeesID { get; set; }

        [DataMember]
        public decimal TaskRate { get; set; }

        [DataMember]
        public TimeSpan TimeBudget { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime RegistrationDate { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public TimeSpan SpentTime { get; set; }

        #endregion
    }
}