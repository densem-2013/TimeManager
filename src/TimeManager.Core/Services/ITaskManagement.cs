namespace Infocom.TimeManager.Core.Services
{
    using System;
    using System.ServiceModel;

    using Infocom.TimeManager.Core.Services.Dto;

    [ServiceContract]
    public interface ITaskManagement
    {
        [OperationContract]
        TaskDto[] GetWorkItems();

        [OperationContract]
        TimeRecordDto BookTime(long taskId, DateTime startDate, TimeSpan spentTime);
    }
}
