namespace Infocom.TimeManager.WebAccess.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Extensions;
    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;

    public class ModelAssembler
    {
        #region Constructors and Destructors

        static ModelAssembler()
        {
            
            Mapper.CreateMap<Project, ProjectModel>()
                .ForMember(src => src.CurrentStatusID, opt => opt.MapFrom(dest => dest.CurrentStatus.ID))
                .ForMember(src => src.CurrentStatusName, opt => opt.MapFrom(dest => dest.CurrentStatus.Name))
                .ForMember(src => src.ProjectPiorityID, opt => opt.MapFrom(dest => dest.ProjectPriority.ID))
                .ForMember(src => src.ProjectPiorityName, opt => opt.MapFrom(dest => dest.ProjectPriority.Name));
            Mapper.CreateMap<EmployeeRate, EmployeeRateModel>(); 
   
            Mapper.CreateMap<ProjectModel, Project>();
            Mapper.CreateMap<ProjectType, ProjectTypeModel>();
            Mapper.CreateMap<ProjectPriority, ProjectPriorityModel>();
            Mapper.CreateMap<Status, StatusModel>();
            Mapper.CreateMap<StatusModel, Status>();
            Mapper.CreateMap<RequestDetail, RequestDetailModel>();
            Mapper.CreateMap<RequestDetailModel, RequestDetail>();
            Mapper.CreateMap<RequestDetailType, RequestDetailTypeModel>();
            Mapper.CreateMap<RequestDetailTypeModel, RequestDetailType>();
            Mapper.CreateMap<TimeSheet, TimeSheetModel>();
            Mapper.CreateMap<TimeSheetModel, TimeSheet>();
            Mapper.CreateMap<Customer, CustomerModel>();
            Mapper.CreateMap<CustomerModel, Customer>();
            Mapper.CreateMap<Contract, ContractModel>();
            Mapper.CreateMap<ContractModel, Contract>();
            Mapper.CreateMap<Contract, IncomingRequestModel>();
            Mapper.CreateMap<IncomingRequestModel, Contract>();
            Mapper.CreateMap<ContractTypeModel, ContractType>();
            Mapper.CreateMap<ContractType, ContractTypeModel>();
            Mapper.CreateMap<Department, DepartmentModel>();
            Mapper.CreateMap<DepartmentModel, Department>();
            Mapper.CreateMap<BudgetAllocationModel, ProjectBudgetAllocation>();
            Mapper.CreateMap<ProjectBudgetAllocation, BudgetAllocationModel>();
            Mapper.CreateMap<EmployeeModel, Employee>();
            Mapper.CreateMap<Employee, EmployeeModel>().ForMember(
                dest => dest.Tasks, opt => opt.MapFrom(dest => dest.Tasks.Select(t => t.ID)));
            Mapper.CreateMap<Task, TaskModel>()
                .ForMember(src => src.TaskStatusID, opt => opt.MapFrom(dest => dest.CurrentStatus.ID))
                .ForMember(src => src.CurrentStatusName, opt => opt.MapFrom(dest => dest.CurrentStatus.Name))
                .ForMember(src => src.AssignedEmployeeIDs, opt => opt.MapFrom(dest => dest.AssignedEmployees.Select(t => t.ID)))
                .ForMember(src => src.TotalSpentTime, opt => opt.MapFrom(dest => new TimeSpan(dest.TimeRecords.Sum(tr => tr.SpentTime.Value.Ticks))))
                .ForMember(src => src.AssignedEmployeeNames, opt => opt.MapFrom(dest => String.Join(", ", dest.AssignedEmployees.Select(t => t.ShortName).ToArray())));

            // .ForMember(src => src.TimeBudget, opt => opt.MapFrom(dest => new DateTime(2000, 1, 1, dest.TimeBudget.Value.Hours, dest.TimeBudget.Value.Minutes, 0)));
            Mapper.CreateMap<TimeRecord, TimeRecordModel>();
            Mapper.CreateMap<RequestModel, Request>();
            //Mapper.CreateMap<Request, RequestOverviewModel>()
            //   .ForMember(dest => dest.ContractID, opt => opt.MapFrom(src => src.Contract.ID))
            //   .ForMember(dest => dest.ContractName, opt => opt.MapFrom(src => src.Contract.Name))
            //   .ForMember(dest => dest.ProjectManagerId, opt => opt.MapFrom(src => src.ProjectManager.ID))
            //   .ForMember(dest => dest.ProjectManagerShortName, opt => opt.MapFrom(src => src.ProjectManager.ShortName))
            //   .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.Contract.Customer.ID))
            //   .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Contract.Customer.Name));
            Mapper.CreateMap<vwRequest, RequestOverviewModel>();
             
            Mapper.CreateMap<Request, RequestModel>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Status.ID))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.ContractID, opt => opt.MapFrom(src => src.Contract.ID))
                .ForMember(dest => dest.ContractName, opt => opt.MapFrom(src => src.Contract.Name))
                .ForMember(dest => dest.ProjectManagerId, opt => opt.MapFrom(src => src.ProjectManager.ID))
                .ForMember(dest => dest.ProjectManagerShortName, opt => opt.MapFrom(src => src.ProjectManager.ShortName))
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.Contract.Customer.ID))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Contract.Customer.Name));
            Mapper.CreateMap<RequestTypeModel, RequestType>();
            Mapper.CreateMap<RequestType, RequestTypeModel>();
            Mapper.CreateMap<Phase, PhaseModel>();
            Mapper.CreateMap<PhaseModel, Phase>();
            Mapper.CreateMap<ProjectBudget, BudgetModel>();
            Mapper.CreateMap<BudgetModel, ProjectBudget>();
            Mapper.CreateMap<Request, RequestShortModel>();
            
        }

        #endregion

        #region Public Methods

        public ProjectModel CreateProjectModel(Project source)
        {
            var result = new ProjectModel();
            result = Mapper.Map<Project, ProjectModel>(source);

            if (result.AssignedEmployeeIDs == null)
            {
                result.AssignedEmployeeIDs = new List<long>();
            }

            if (source.Employees != null)
            {
                result.AssignedEmployeeIDs = source.Employees.Select(e => e.ID);
            }
            
            if(source.Phase.Request!=null)
            {
                result.ProjectManager = this.CreateEmployeeModel(source.Phase.Request.ProjectManager);
            }

            result.OpenedTasks = source.Tasks.Where(t => t.CurrentStatus.Name == "Открытая").Count();

            result.ClosedTasks = source.Tasks.Where(t => t.CurrentStatus.Name == "Закрытая").Count();

            result.AmountTasks = source.Tasks.Count;

            result.RequestID = source.Phase.Request.ID;
            
            result.RequestNumber = source.Phase.Request.Number;

            if ( source.Phase.DeadLine.HasValue)
            {
                 result.DeadLine = source.Phase.DeadLine.Value;
            }

            result.Contract = CreateContractModel(source.Phase.Request.Contract);
            result.Request = CreateRequestModel(source.Phase.Request);

            return result;
        }

        public Project CreateProject(ProjectModel source)
        {
            var result = Mapper.Map<ProjectModel, Project>(source);

           // result.Contract.Number = string.Format("{0}/{1}",source.Contract.NumberBeforeSlash,source.Contract.NumberAfterSlash);

            return result;
        }

        public EmployeeModel CreateEmployeeModel(Employee source)
        {
            return Mapper.Map<Employee, EmployeeModel>(source);
        }

        public TaskModel CreateTaskModel(Task source)
        {
            return Mapper.Map<Task, TaskModel>(source);
        }

        public TimeRecordModel CreateTimeRecordModel(TimeRecord source)
        {
            return Mapper.Map<TimeRecord, TimeRecordModel>(source);
        }

        public ProjectTypeModel CreateProjectTypeModel(ProjectType source)
        {
            return Mapper.Map<ProjectType, ProjectTypeModel>(source);
        }

        public StatusModel CreateStatusModel(Status source)
        {
            return Mapper.Map<Status, StatusModel>(source);
        }

        public DepartmentModel CreateDepartment(Department source)
        {
            return Mapper.Map<Department, DepartmentModel>(source);
        }

        public BudgetAllocationModel CreateProjectBudgetAllocation(ProjectBudgetAllocation source)
        {
            return Mapper.Map<ProjectBudgetAllocation, BudgetAllocationModel>(source);
        }

        public TimeSheetModel CreateTimeSheetModel(TimeSheet source,long employeeID)
        {
            var result = Mapper.Map<TimeSheet, TimeSheetModel>(source);
            result.Tasks.ToList().ForEach(t => t.SetEmployeeID(employeeID));
            return result;
        }

        public Contract CreateContract(ContractModel source)
        {
            return Mapper.Map<ContractModel, Contract>(source);
        }

        public Request CreateRequest(RequestModel source)
        {
            Request result;
            result = Mapper.Map<RequestModel, Request>(source);

            return result;
        }

        public RequestTypeModel CreateRequestTypeModel(RequestType source)
        {
            return Mapper.Map<RequestType, RequestTypeModel>(source);
        }

        public DepartmentModel CreateDepartmentModel(Department source)
        {
            return Mapper.Map<Department, DepartmentModel>(source);
        }

        public ProjectBudgetAllocation CreateBudgetAllocation(BudgetAllocationModel source)
        {
            return Mapper.Map<BudgetAllocationModel, ProjectBudgetAllocation>(source);
        }

        public Phase CreatePhase(PhaseModel source)
        {
            return Mapper.Map<PhaseModel, Phase>(source);
        }

        public PhaseModel CreatePhaseModel(Phase source)
        {
            return Mapper.Map<Phase, PhaseModel>(source);
        }

        public ProjectBudget CreateProjectBudget(BudgetModel source)
        {
            return Mapper.Map<BudgetModel, ProjectBudget>(source);
        }

        public RequestModel CreateRequestModel(Request source)
        {
            var requestModel = new RequestModel();
            requestModel = Mapper.Map<Request, RequestModel>(source);

            //if (source.IsInWork)
            //{
            //    requestModel.ProcessState = RequestProcessState.Вработе;
            //}
            //else
            //{
            //    requestModel.ProcessState = RequestProcessState.Необработанная;
            //}

            if (requestModel.Phases.FirstOrDefault() !=null)
            {
                if (requestModel.Phases[0].DeadLine.HasValue)
                {
                    requestModel.FinishDate = requestModel.Phases[0].DeadLine.Value;
                }

                if (source.Phases.First().Projects.Any())
                {
                    requestModel.ProcessState = RequestProcessState.Вработе;
                }
                else
                {
                    requestModel.ProcessState = RequestProcessState.Необработанная;
                }
            }

            if (source.IsValidated)
            {
                requestModel.IsValidatedValue = "Подтверждена" ;
            }
            else
            {
                requestModel.IsValidatedValue = "Не подтверждена";
            }
            return requestModel;
        } 

        public RequestShortModel CreateRequestShortModel(Request source)
        {
            var result = new RequestShortModel
                {
                    ID = source.ID,
                    Date = source.Date,
                    Name = string.Format("№{1},{0: dd.MM.yyyy}, {2}, {3}", source.Date, source.Number, source.Contract.Customer.Name.ReduceTo(20), source.Contract.Name.ReduceTo(40))
                };
            return result;
        }

        public PhaseShortModel CreatePhaseShortModel(Phase source)
        {
            var result = new PhaseShortModel
            {
                ID = source.ID,
                Name = string.Format("№{0}, {1}, {2}", source.Number,source.Request.Contract.Name.ReduceTo(20), source.Name).ReduceTo(45)
            };
            return result;
        }


        public ContractModel CreateContractModel(Contract source)
        {
            return Mapper.Map<Contract, ContractModel>(source);
        }

        public CustomerModel CreateCustomerModel(Customer source)
        {
            return Mapper.Map<Customer, CustomerModel>(source);
        }

        public ContractTypeModel CreateContractTypeModel(ContractType source)
        {
            return Mapper.Map<ContractType, ContractTypeModel>(source);
        }

        public Contract CreateContractFromRequest(IncomingRequestModel source)
        {
            return Mapper.Map<IncomingRequestModel, Contract>(source);
        }

        public RequestDetailTypeModel CreateRequestDetailTypeModel(RequestDetailType source)
        {
            return Mapper.Map<RequestDetailType, RequestDetailTypeModel>(source);
        }

        public RequestOverviewModel CreateRequestOverviewModel(Request source)
        {
            var requestModel = new RequestOverviewModel();
            requestModel = Mapper.Map<Request, RequestOverviewModel>(source);

            //if (source.IsInWork)
            //{
            //    requestModel.ProcessState = RequestProcessState.Вработе;
            //}
            //else
            //{
            //    requestModel.ProcessState = RequestProcessState.Необработанная;
            //}

            //if (requestModel.Phases.FirstOrDefault() != null)
            //{
            //    if (requestModel.Phases[0].DeadLine.HasValue)
            //    {
            //        requestModel.FinishDate = requestModel.Phases[0].DeadLine.Value;
            //    }

            //    if (source.Phases.First().Projects.Any())
            //    {
            //        requestModel.ProcessState = RequestProcessState.Вработе;
            //    }
            //    else
            //    {
            //        requestModel.ProcessState = RequestProcessState.Необработанная;
            //    }
            //}

            if (source.IsValidated)
            {
                requestModel.IsValidatedValue = "Подтверждена";
            }
            else
            {
                requestModel.IsValidatedValue = "Не подтверждена";
            }
            return requestModel;
        }

        public RequestOverviewModel CreatevwRequestOverviewModel(vwRequest source)
        {
            var requestModel = new RequestOverviewModel();
            requestModel = Mapper.Map<vwRequest, RequestOverviewModel>(source);

            if (source.IsValidated)
            {
                requestModel.IsValidatedValue = "Подтверждена";
            }
            else
            {
                requestModel.IsValidatedValue = "Не подтверждена";
            }
            return requestModel;
        } 

        public ProjectPriorityModel CreateProjectPriorityModel(ProjectPriority source)
        {
            return Mapper.Map<ProjectPriority, ProjectPriorityModel>(source);
        }

        public Employee ConvertToEmployee(EmployeeModel source)
        {
            var result = Mapper.Map<EmployeeModel, Employee>(source);
            return result;
        }

        public EmployeeRateModel CreateEmployeeRateModel(EmployeeRate source)
        {
            var result = Mapper.Map< EmployeeRate, EmployeeRateModel>(source);

            return result;
        }
        
        #endregion




     
    }
}