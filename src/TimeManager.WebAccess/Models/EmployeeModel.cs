namespace Infocom.TimeManager.WebAccess.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System;

    public class EmployeeModel : BaseModel
    {
        #region Constructors and Destructors

        public EmployeeModel()
        {
            this.Tasks = new List<long>();

        }


        #endregion

        #region Properties

        [DisplayName(@"Имя")]
        public string FirstName { get; set; }

        [DisplayName(@"Фамилия")]
        public string LastName { get; set; }

        [DisplayName(@"Отчество")]
        public string PatronymicName { get; set; }

        public string ShortName { get; set; }

        [DisplayName(@"Логин")]
        public string Login { get; set; }

        [DisplayName(@"Email")]
        public string Email { get; set; }

        [RegularExpression(@"^\d+(.\d+){0,1}$", ErrorMessage = "Допустимое значение 0-999")]
        [DisplayName(@"Часовая ставка в грн.")]
        public decimal? Rate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName(@"Дата поступления")]
        public DateTime? HireDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName(@"Дата увольнения")]
        public DateTime? FireDate { get; set; }

        [DisplayName(@"Название отдела")]
        public string DepartmentShortName { get; set; }

        public long ManagerID { get; set; }

        public IEnumerable<long> Tasks { get; set; }

        public int ApprovePermission { get; set; }
        public int HumanResource { get; set; }
        public bool IsHumanResource { get; set; }
        public long DepartmentID { get; set; }

        #endregion
    }
}