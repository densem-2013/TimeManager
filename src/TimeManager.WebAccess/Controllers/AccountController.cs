namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Infrastructure;

    using Infocom.TimeManager.WebAccess.Models;

    using NHibernate.Linq;
    using System.Web;

    [OptionalAuthorize(false)]
    public partial class AccountController : TimeManagerBaseController
    {
        #region Properties

        public IFormsAuthenticationService FormsService { get; set; }

        public IMembershipService MembershipService { get; set; }

        #endregion

        // **************************************
        // URL: /Account/LogOn
        // **************************************
        #region Public Methods

        public virtual ActionResult LogOn()
        {
            return this.View();
        }

        [HttpPost]
        public virtual ActionResult LogOn(LogOnModel model, string returnUrl="~")
        {
            if (this.ModelState.IsValid)
            {
                if (this.MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    this.FormsService.SignIn(model.UserName, model.RememberMe);
                    var userName = "INFOCOM-LTD\\" + model.UserName.ToLower();
                    this.Session["userName"] = userName;
                    this.Session["authenticatedUserName"] = userName;

                    var isRegistered = PersistenceSession.Query<Employee>().Where(e => e.Login == userName).Any();

                    if (!isRegistered)
                    {
                        this.ModelState.AddModelError(
                            String.Empty,
                            "Пользователь не зарегистрирован в системе. Обратитесь к администратору системы");
                        return View(model);
                    }
                    var user = PersistenceSession.Query<Employee>().Where(e => e.Login == userName).Single();
                    this.Session["userDepartmentId"] = user.Department.ID;
                    this.Session["ApprovePermission"] = (bool)(user.ApprovePermission == 1);
                    this.Session["RequestPermission"] = (bool)(user.Department.RequestPermission == 1);
                    this.Session["TimeregPermission"] = (bool)(user.Department.TimeregPermission == 1);
                    this.Session["HumanResource"] = (bool) (user.HumanResource == 1);
                    string userFullName = user.LastName;
                    if (user.FirstName != null) userFullName += " " + user.FirstName.Substring(0,1) + ".";
                    if (user.PatronymicName != null) userFullName += " " + user.PatronymicName.Substring(0, 1) + ".";
                    this.Session["userFullName"] = userFullName;
                    return this.Redirect(returnUrl);
                }

                this.ModelState.AddModelError(string.Empty, "Имя учетной записи или пароль введены неверно.");
            }
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public virtual ActionResult LogOff()
        {
            this.FormsService.SignOut();
            return this.RedirectToAction("Index", "ProjectsOverview");
        }

        #endregion

        #region Methods

        protected override void Initialize(RequestContext requestContext)
        {
            if (this.FormsService == null)
            {
                this.FormsService = new FormsAuthenticationService();
            }

            if (this.MembershipService == null)
            {
                this.MembershipService = new AccountMembershipService();
            }

            base.Initialize(requestContext);
        }

        #endregion
    }
}