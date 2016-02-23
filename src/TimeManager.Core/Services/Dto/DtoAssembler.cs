namespace Infocom.TimeManager.Core.Services.Dto
{

    using AutoMapper;

    using Infocom.TimeManager.Core.DomainModel;

    public class DtoAssembler
    {
        #region Constructors and Destructors

        static DtoAssembler()
        {
            Mapper.CreateMap<Task, TaskDto>();
            Mapper.CreateMap<Employee, EmployeeDto>();
            Mapper.CreateMap<TimeRecord, TimeRecordDto>();
        }

        #endregion

        #region Public Methods

        public TaskDto CreateTaskDto(Task source)
        {
            return Mapper.Map<Task, TaskDto>(source);
        }

        public TimeRecordDto CreateTimeRecordDto(TimeRecord source)
        {
            return Mapper.Map<TimeRecord, TimeRecordDto>(source);
        }

        public EmployeeDto CreateEmployeeDto(Employee source)
        {
            return Mapper.Map<Employee, EmployeeDto>(source);
        }

        #endregion
    }
}