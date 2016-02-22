namespace Infocom.TimeManager.Core
{
    using System;
    using System.Linq;

    using CommonServiceLocator.NinjectAdapter;

    using ConfOrm;
    using ConfOrm.Mappers;
    using ConfOrm.NH;
    using ConfOrm.Shop.CoolNaming;

    using Infocom.TimeManager.Core.DomainModel;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Validator.Cfg;
    using NHibernate.Validator.Engine;

    using Ninject;
    using Ninject.Modules;

    using Infocom.IT.ILibrary.Extensions;

    public class PersistenceInjection : NinjectModule
    {
        #region Constants and Fields

        private Configuration _config;

        private ISessionFactory _factory;

        #endregion

        #region Properties

        private ISessionFactory Factory
        {
            get
            {
                return this._factory ?? (this._factory = this.GetNHibernateConfig().BuildSessionFactory());
            }
        }

        #endregion

        #region Public Methods

        public static IServiceLocator SetServiceLocator()
        {
            var kernel = new NinjectServiceLocator(new StandardKernel(new PersistenceInjection()));
            ServiceLocator.SetLocatorProvider(() => kernel);

            return kernel;
        }

        public Configuration GetNHibernateConfig(Action<Configuration> interceptor = null)
        {
            if (this._config == null)
            {
                var persistedEntities =
                    typeof(PersistenceInjection).Assembly.GetTypes().Where(
                        t => t.GetCustomAttributes(typeof(PersistedEntityAttribute), true).Any());

                var orm = new ObjectRelationalMapper();
                orm.TablePerClass(persistedEntities);
                orm.ManyToMany<Task, Employee>();
                orm.ManyToMany<Project, Employee>();
                orm.ManyToMany<Task, TimeSheet>();

                var anyButOrphans = Cascade.Persist | Cascade.Refresh | Cascade.Merge | Cascade.Detach | Cascade.ReAttach;

                orm.Cascade<Contract, Customer>(anyButOrphans);

                orm.Cascade<RequestDetail, RequestDetailType>(anyButOrphans);
                orm.Cascade<RequestDetail, Request>(anyButOrphans);

                orm.Cascade<Request, Contract>(anyButOrphans);
                orm.Cascade<Request, Employee>(anyButOrphans);
                orm.Cascade<Request, RequestType>(anyButOrphans);
                orm.Cascade<Request, RequestDetail>(Cascade.All);
                orm.Cascade<Request, Phase>(Cascade.All);

                orm.Cascade<Phase, Request>(anyButOrphans);
                orm.Cascade<Phase, ProjectBudget>(anyButOrphans);
                orm.Cascade<Phase, Project>(Cascade.All);

                orm.Cascade<Project, Task>(Cascade.All);
                orm.Cascade<Project, Employee>(anyButOrphans);
                orm.Cascade<Project, Status>(anyButOrphans);
                orm.Cascade<Project, Phase>(anyButOrphans);  // под вопросом
                orm.Cascade<Project, ProjectType>(anyButOrphans);
                orm.Cascade<Project, ProjectPriority>(anyButOrphans);

                orm.Cascade<ProjectBudget, ProjectBudgetAllocation>(Cascade.All);

                orm.Cascade<Task, Employee>(anyButOrphans);
                orm.Cascade<Task, Project>(anyButOrphans);
                orm.Cascade<Task, Task>(anyButOrphans);
                orm.Cascade<Task, TimeRecord>(Cascade.All);
                orm.Cascade<Task, Status>(anyButOrphans);
                orm.Cascade<Task, TimeSheet>(anyButOrphans);

                orm.Cascade<Employee, Employee>(anyButOrphans);
                orm.Cascade<Employee, Task>(anyButOrphans);
                orm.Cascade<Employee, Project>(anyButOrphans);
                orm.Cascade<Employee, Department>(anyButOrphans);
                orm.Cascade<Employee, EmployeeRate>(anyButOrphans);

                orm.Cascade<TimeRecord, Employee>(anyButOrphans);
                orm.Cascade<TimeRecord, Task>(anyButOrphans);

                orm.Cascade<Department, Employee>(anyButOrphans);
                orm.Cascade<ProjectBudgetAllocation, Department>(anyButOrphans);

                orm.Cascade<TimeSheet, Employee>(anyButOrphans);
                orm.Cascade<TimeSheet, Task>(Cascade.All);

                

                this._config = new Configuration().Configure();
                if (interceptor != null)
                {
                    interceptor(this._config);
                }
                else
                {
                    this.ConfigurationInterceptor(this._config);
                }

                var mapper = new Mapper(orm, new CoolPatternsAppliersHolder(orm));

                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.ContractName, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.CustomerName, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.Date, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.Detail, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.FinishDate, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.IsValidated, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.Number, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.ProjectManagerShortName, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.TypeName, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.ProjectManagerId, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.ProcessState, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.ProcessStateId, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.ContractId, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.CustomerId, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.StatusId, pm => pm.NotNullable(true)));
                mapper.Customize<vwRequest>(pcc => pcc.Property(n => n.StatusName, pm => pm.NotNullable(true)));

                mapper.Customize<vwGetBudgetBy>(pcc => pcc.Property(n => n.DepartmentId, pm => pm.NotNullable(true)));
                mapper.Customize<vwGetBudgetBy>(pcc => pcc.Property(n => n.ProjectId, pm => pm.NotNullable(true)));
                mapper.Customize<vwGetBudgetBy>(pcc => pcc.Property(n => n.SpentTime, pm => pm.NotNullable(true)));

                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.AmountOfHours, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.AmountOfMoney, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.ContractCustomer, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.ContractNumberAndSigningDate, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.DepartmentID, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.DepartmentName, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.EmployeeRate, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.EmployeeShortName, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.ID, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.PorjectTypeName, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.ProjectCode, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.ProjectDeadLine, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.ProjectID, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.ProjectName, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.ProjectOrder, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.SpentTime, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.StatusName, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.TaskName, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.TimeRecordStartDate, pm => pm.NotNullable(true)));
                mapper.Customize<vwProjectDetailsWithSpentTimeByBudgetAllocation>(pcc => pcc.Property(n => n.DateEndWorkByDepartment, pm => pm.NotNullable(false)));
                
                mapper.Class<Customer>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<Customer>(pcc => pcc.Property(n => n.Name, pm => pm.NotNullable(true)));

                mapper.Class<Contract>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<Contract>(pcc => pcc.Property(n => n.ContractType, pm => pm.NotNullable(true)));

                mapper.Class<TimeSheet>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<TimeSheet>(pcc => pcc.Collection(n => n.Tasks, pm => pm.Access(Accessor.Field)));
                mapper.Customize<TimeSheet>(pcc => pcc.ManyToOne(n => n.Employee, pm => pm.Access(Accessor.Property)));
                mapper.Customize<TimeSheet>(pcc => pcc.Property(n => n.Employee, pm => pm.Access(Accessor.Property)));
                mapper.Customize<TimeSheet>(pcc => pcc.Property(n => n.Date, pm => pm.Access(Accessor.Property)));

                mapper.Class<Project>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<Project>(pcc => pcc.ManyToOne(n => n.CurrentStatus, pm => pm.Access(Accessor.Field)));
                mapper.Customize<Project>(pcc => pcc.Property(n => n.Name, pm => pm.NotNullable(true)));
                mapper.Customize<Project>(pcc => pcc.Collection(n => n.Tasks, pm => pm.Access(Accessor.Field)));
                mapper.Customize<Project>(pcc => pcc.Collection(n => n.Employees, pm => pm.Access(Accessor.Field)));

                mapper.Customize<Project>(pcc => pcc.OneToOne(n => n.Phase, pm => pm.Access(Accessor.Property)));
                mapper.Customize<Project>(pcc => pcc.ManyToOne(n => n.ProjectType, pm => pm.Access(Accessor.Property)));
                mapper.Customize<Project>(pcc => pcc.Property(n => n.Name, pm => pm.NotNullable(true)));
                mapper.Customize<Project>(pcc => pcc.Property(n => n.Code, pm => pm.NotNullable(true)));
                mapper.Customize<Project>(pcc => pcc.Property(n => n.ProjectPriority, pm => pm.NotNullable(true)));
                mapper.Customize<Project>(pcc => pcc.Property(n => n.Phase, pm => pm.NotNullable(true)));


                mapper.Class<Department>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<Department>(pcc => pcc.Property(n => n.Name, pm => pm.NotNullable(true)));
                mapper.Customize<Department>(pcc => pcc.Property(n => n.ShortName, pm => pm.NotNullable(true)));
                mapper.Customize<Department>(pcc => pcc.ManyToOne(n => n.Manager, pm => pm.Access(Accessor.Property)));
                mapper.Customize<Department>(pcc => pcc.Collection(n => n.Employees, pm => pm.Access(Accessor.Property)));

                mapper.Class<Task>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<Task>(pcc => pcc.ManyToOne(n => n.CurrentStatus, pm => pm.Access(Accessor.Field)));
                mapper.Customize<Task>(pcc => pcc.ManyToOne(n => n.Project, pm => pm.Access(Accessor.Field)));
                mapper.Customize<Task>(pcc => pcc.Collection(n => n.AssignedEmployees, pm => pm.Access(Accessor.Field)));
                mapper.Customize<Task>(pcc => pcc.Collection(n => n.TimeSheets, pm => pm.Access(Accessor.Field)));

                mapper.Customize<Task>(pcc => pcc.Collection(n => n.TimeRecords, pm => pm.Access(Accessor.Property)));
                mapper.Customize<Task>(pcc => pcc.Property(n => n.Name, pm => pm.NotNullable(true)));
                mapper.Customize<Task>(pcc => pcc.OneToOne(n => n.ParentTask, pm => pm.Access(Accessor.Property)));

                mapper.Class<TimeRecord>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<TimeRecord>(pcc => pcc.Property(n => n.Employee, pm => pm.NotNullable(true)));
                mapper.Customize<TimeRecord>(pcc => pcc.Property(n => n.Task, pm => pm.NotNullable(true)));
                mapper.Customize<TimeRecord>(pcc => pcc.ManyToOne(n => n.Employee, pm => pm.Access(Accessor.Property)));
                mapper.Customize<TimeRecord>(pcc => pcc.ManyToOne(n => n.Task, pm => pm.Access(Accessor.Property)));

                mapper.Class<Employee>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<Employee>(pcc => pcc.Property(n => n.Login, pm => pm.Unique(true)));
                mapper.Customize<Employee>(pcc => pcc.Collection(n => n.Tasks, pm => pm.Access(Accessor.Field)));
                mapper.Customize<Employee>(pcc => pcc.Collection(n => n.Projects, pm => pm.Access(Accessor.Field)));

                mapper.Customize<Employee>(pcc => pcc.Property(n => n.Login, pm => pm.NotNullable(true)));
                mapper.Customize<Employee>(pcc => pcc.Property(n => n.FirstName, pm => pm.NotNullable(true)));
                mapper.Customize<Employee>(pcc => pcc.Property(n => n.LastName, pm => pm.NotNullable(true)));
                mapper.Customize<Employee>(pcc => pcc.Property(n => n.PatronymicName, pm => pm.NotNullable(true)));
                mapper.Customize<Employee>(pcc => pcc.Property(n => n.HumanResource, pm => pm.NotNullable(false)));
                mapper.Customize<Employee>(pcc => pcc.ManyToOne(n => n.Manager, pm => pm.Access(Accessor.Property)));
                mapper.Customize<Employee>(pcc => pcc.ManyToOne(n => n.Department, pm => pm.Access(Accessor.Property)));
                mapper.Customize<Employee>(pcc => pcc.Collection(n => n.Subordinates, pm => pm.Access(Accessor.Property)));
                mapper.Customize<Employee>(pcc => pcc.Collection(n => n.EmployeeRates, pm => pm.Access(Accessor.Property)));

                mapper.Class<EmployeeRate>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<EmployeeRate>(pcc => pcc.Property(n => n.ID, pm => pm.Unique(true)));
                mapper.Customize<EmployeeRate>(pcc => pcc.Property(n => n.Rate, pm => pm.NotNullable(true)));
                mapper.Customize<EmployeeRate>(pcc => pcc.Property(n => n.RateNumber, pm => pm.NotNullable(true)));
                mapper.Customize<EmployeeRate>(pcc => pcc.Property(n => n.StartDate, pm => pm.NotNullable(true)));
                mapper.Customize<EmployeeRate>(pcc => pcc.ManyToOne(n => n.Employee, pm => pm.Access(Accessor.Property)));

                mapper.Class<Request>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<Request>(pcc => pcc.Property(n => n.Number, pm => pm.NotNullable(true)));
                mapper.Customize<Request>(pcc => pcc.Property(n => n.Date, pm => pm.NotNullable(true)));
                mapper.Customize<Request>(pcc => pcc.Property(n => n.Type, pm => pm.Access(Accessor.Field)));
                mapper.Customize<Request>(pcc => pcc.Property(n => n.ProjectManager, pm => pm.NotNullable(true)));
                mapper.Customize<Request>(pcc => pcc.Collection(n => n.Phases, pm => pm.Access(Accessor.Property)));
                mapper.Customize<Request>(pcc => pcc.Property(n => n.IsValidated, pm => pm.NotNullable(true)));
                mapper.Customize<Request>(pcc => pcc.Collection(n => n.RequestDetails, pm => pm.Access(Accessor.Field)));
                mapper.Customize<Request>(pcc => pcc.Property(n => n.Type, pm => pm.NotNullable(true)));
                mapper.Customize<Request>(pcc => pcc.ManyToOne(n => n.Contract, pm => pm.Access(Accessor.Property)));
                mapper.Customize<Request>(pcc => pcc.Property(n => n.Status, pm => pm.Access(Accessor.Field)));

                mapper.Customize<RequestType>(pcc => pcc.Property(n => n.Name, pm => pm.NotNullable(true)));

                mapper.Class<RequestDetail>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
               // mapper.Customize<RequestDetail>(pcc => pcc.Property(n => n.RequestDetailType, pm => pm.NotNullable(true)));
                mapper.Customize<RequestDetail>(pcc => pcc.ManyToOne(n => n.Request, pm => pm.Access(Accessor.Field)));
               // mapper.Customize<RequestDetail>(pcc => pcc.Property(n => n.Request, pm => pm.NotNullable(true)));
                mapper.Customize<RequestDetail>(pcc => pcc.Property(n => n.Checked, pm => pm.NotNullable(true)));

                //mapper.Class<RequestDetailType>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<RequestDetailType>(pcc => pcc.Property(n => n.Name, pm => pm.NotNullable(true)));

                mapper.Class<Phase>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<Phase>(pcc => pcc.Property(n => n.Budget, pm => pm.Access(Accessor.Field)));
                mapper.Customize<Phase>(pcc => pcc.Property(n => n.Name, pm => pm.NotNullable(true)));
                mapper.Customize<Phase>(pcc => pcc.Property(n => n.Number, pm => pm.NotNullable(true)));
                mapper.Customize<Phase>(pcc => pcc.Property(n => n.DeadLine, pm => pm.NotNullable(true)));
                mapper.Customize<Phase>(pcc => pcc.Collection(n => n.Projects, pm => pm.Access(Accessor.Property)));


                mapper.Customize<Phase>(pcc => pcc.OneToOne(n => n.Request, pm => pm.Access(Accessor.Property)));

                mapper.Class<ProjectBudget>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<ProjectBudget>(pcc => pcc.Collection(n => n.BudgetAllocations, pm => pm.Access(Accessor.Property)));

                mapper.Class<ProjectBudgetAllocation>(pm => pm.Id(id => id.Generator(new NativeGeneratorDef())));
                mapper.Customize<ProjectBudgetAllocation>(pcc => pcc.ManyToOne(n => n.Department, pm => pm.Access(Accessor.Property)));

                mapper.Customize<ProjectType>(pcc => pcc.Property(n => n.Name, pm => pm.NotNullable(true)));

                mapper.Customize<ProjectPriority>(pcc => pcc.Property(n => n.Name, pm => pm.NotNullable(true)));

                mapper.Customize<Status>(pcc => pcc.Property(n => n.Name, pm => pm.NotNullable(true)));
                mapper.Customize<Status>(pcc => pcc.Property(n => n.ApplicableTo, pm => pm.NotNullable(true)));

                var mapping = mapper.CompileMappingFor(persistedEntities);

                this._config.AddDeserializedMapping(mapping, "FullDomain");
            }

            return this._config;
        }

        public override void Load()
        {
            var validatorEngine = new ValidatorEngine();
            validatorEngine.Configure();
            var nhConfig = this.GetNHibernateConfig();
            ValidatorInitializer.Initialize(nhConfig, validatorEngine);
            this.Bind<Configuration>().ToConstant(nhConfig).InSingletonScope();
            this.Bind<ISessionFactory>().ToConstant(this.Factory).InSingletonScope();
        }

        #endregion

        #region Methods

        protected virtual void ConfigurationInterceptor(Configuration configuration)
        {
        }

        #endregion
    }
}