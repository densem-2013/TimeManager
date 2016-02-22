namespace Infocom.TimeManager.Core.Services.Dto
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public class TimeRecordDto
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public EmployeeDto Employee { get; set; }

        [DataMember]
        public TaskDto Task { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public TimeSpan SpentTime { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}
