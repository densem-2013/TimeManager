using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infocom.TimeManager.WebAccess.Infrastructure
{
    using System.Web.Mvc;

    public class RoleManagment : IRoleManagment
    {
        private readonly Controller controller;

        public RoleManagment(Controller controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            this.controller = controller;
        }

        #region IRoleManagment Members

        public bool IsAdmin()
        {
            return controller.User.IsInRole("TimeManagerAdministrators");
        }

        public bool IsInRole(string role)
        {
            return controller.User.IsInRole(role);
        }

        #endregion
    } 
}