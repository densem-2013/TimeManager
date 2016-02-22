namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Web.Security;

    public abstract class BaseViewModel
    {
        #region Properties

        public bool IsAdmin
        {
            get
            {
                return Roles.IsUserInRole("TimeManagerAdministrators");
            }
        }

        #endregion
    }
}