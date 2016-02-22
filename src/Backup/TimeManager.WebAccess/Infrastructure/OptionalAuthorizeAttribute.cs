namespace Infocom.TimeManager.WebAccess.Infrastructure
{
    using System.Web;
    using System.Web.Mvc;

    public class OptionalAuthorizeAttribute : AuthorizeAttribute
    {
        #region Constants and Fields

        private readonly bool _authorize;

        #endregion

        #region Constructors and Destructors

        public OptionalAuthorizeAttribute()
        {
            this._authorize = true;
        }

        public OptionalAuthorizeAttribute(bool authorize)
        {
            this._authorize = authorize;
        }

        #endregion

        #region Methods

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!this._authorize)
            {
                return true;
            }

            return base.AuthorizeCore(httpContext);
        }

        #endregion
    }
}