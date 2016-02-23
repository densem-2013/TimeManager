namespace Infocom.TimeManager.Core.Services.Dto
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public class ProjectDto
    {
        #region Properties

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string ProjectOrder { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime DeadLine { get; set; }

        [DataMember]
        public EmployeeDto ProjectManager { get; set; }

        [DataMember]
        public decimal Budget { get; set; }

        [DataMember]
        public string CurrentStatusName { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public decimal Rate { get; set; }

        [DataMember]
        public TaskDto[] Tasks { get; set; }

        #endregion
    }
}