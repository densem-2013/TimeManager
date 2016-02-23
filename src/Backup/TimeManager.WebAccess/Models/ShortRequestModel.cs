using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Infocom.TimeManager.WebAccess.Models;

    public class ShortRequestModel : DynamicBaseModel
    {
        [DisplayName(@"Наименование")]
        [Required(ErrorMessage = @"Не задано")]
        [StringLength(255)]
        public string Name { get; set; }

        [DisplayName(@"Номер")]
        [Required(ErrorMessage = @"Не задано")]
        [StringLength(255)]
        public string Number { get; set; }

        [DisplayName(@"Номер")]
        [Required(ErrorMessage = @"Не задано")]
        [StringLength(255)]
        public string Title
        {
            get
            {
                return string.Format("{0} {1}", Number, Name);
            }
        }
    }
}
