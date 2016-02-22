namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections.Generic;

    using Infocom.TimeManager.WebAccess.Models;

    public class RequestDetailModel : BaseViewModel
    {
        #region Properties

        public long RequestDetailTypeID { get; set; }

        public string CheckedValue 
        { get
            {
                if(this._checked)
                {
                    return "on";
                }
                else
                {
                    return "none";
                }
            }
          set
            {
                if (value == "on")
                {
                    _checked = true;
                }
                else
                { 
                    _checked = false; 
                }
            }
        }

        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set { _checked = value; }
        }
        #endregion
    }
}