namespace Infocom.TimeManager.Core.Services.Dto
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public class EmployeeDto
    {
        #region Properties

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string PatronymicName { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public int ManagerID { get; set; }

        [DataMember]
        public int DeprtmentID { get; set; }

        [DataMember]
        public EmployeeDto[] Subordinates { get; set; }

        public virtual TaskDto[] Tasks { get; set; }

        #endregion
    }
}