namespace Infocom.TimeManager.WebAccess.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.ViewModels;

    public class RequestModel: BaseModel
    {
        public RequestModel()
        {
            this.Phases = new List<PhaseModel>();
            this.Type = new RequestTypeModel();
            this.RequestDetails = new List<RequestDetailModel>();
            
        }
        public bool isCustomersWithoutContracts { get; set; }

        [DisplayName(@"Номер")]
        public string Number { get; set; }

        [DisplayName(@"Дата")]
        [Required(ErrorMessage = @"Не задано")]
        public DateTime Date { get; set; }

        [DisplayName(@"Дата начала работ")]
        [Required(ErrorMessage = @"Не задано")]
        public DateTime StartDate { get; set; }

        [DisplayName(@"Тип")]
        public RequestTypeModel Type { get; set; }

        [DisplayName(@"Наименование договора")]
        [Required(ErrorMessage = @"Поле 'Наименование договора' осталось не заполненым")]
        public long ContractID { get; set; }

        [DisplayName(@"Наименование заказчика")]
        public long CustomerID { get; set; }

        [DisplayName(@"Этапы")]
        public List<PhaseModel> Phases { get; set; }

        [DisplayName(@"Руководитель проекта в КД")]
        public long ProjectManagerId { get; set; }

        [DisplayName(@"Руководитель проекта в КД")]
        public string ProjectManagerShortName { get; set; }

        [DisplayName(@"Статус заявки")]
        public RequestProcessState ProcessState { get; set; }

        [DisplayName(@"Договор")]
        public string ContractName { get; set; }

        [DisplayName(@"Последняя редакция")]
        public DateTime LastUpdateDate { get; set; }

        [DisplayName(@"Изменил")]
        public string LastUpdateUserShortName { get; set; }

        [DisplayName(@"Заказчик")]
        public string CustomerName { get; set; }

        [DisplayName(@"Номер контракта")]
        public string ContractNumber { get; set; }
        
        [DisplayName(@"Номер контракта")]
        public DateTime ContractSigningDate { get; set; }

        [DisplayName(@"Проверена")]
        public Boolean IsValidated { get; set; }

        [DisplayName(@"Проверена")]
        public string IsValidatedValue { get; set; }

        [DisplayName(@"Наименование и объем передаваемой технической документации")]
        public string Detail { get; set; }

        [DisplayName(@"Количество экземпляров проекта")]
        public string Documentation { get; set; }

        [DisplayName(@"")]
        public List<RequestDetailModel> RequestDetails { get; set; }

        [DisplayName(@"Дата окончания")]
        [Required(ErrorMessage = @"Не задано")]
        public DateTime? FinishDate { get; set; }

        [DisplayName(@"Статус")]
        public long StatusId { get; set; }

        [DisplayName(@"Статус")]
        public string StatusName { get; set; }

        [DisplayName(@"Проверена")]
        public string TenderResult { get; set; }

        [DisplayName(@"Проверена")]
        public string TenderRemark { get; set; }
    }
}