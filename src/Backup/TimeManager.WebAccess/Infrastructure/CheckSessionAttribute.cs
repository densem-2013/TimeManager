namespace Infocom.TimeManager.WebAccess.Infrastructure
{
    using System.Web.Mvc;
    using System.Web.Security;

    public class CheckSessionAttribute : ActionFilterAttribute 
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session!=null)
            if (filterContext.HttpContext.Session.IsNewSession)
            {
                FormsAuthentication.SignOut();
                
                InitializeSession(filterContext);

                if (filterContext.HttpContext.Request.UrlReferrer != null)
                {
                    filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.UrlReferrer.AbsoluteUri);
                }
                else
                {
                    filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.Url.AbsoluteUri);
                    //filterContext.Result = new RedirectResult("/");
                }
                
            }

        }
        /// <summary>
        /// Initializes a session.
        /// </summary>
        /// <param name="filterContext"></param>
        /// <remarks>Writes dummy data to initialize session.</remarks>
        private void InitializeSession(ActionExecutingContext filterContext)
        {
            filterContext.Controller.TempData["Constants.TEMPDATA_KEYS.TIMEOUT"] = "Your session has timed out.  Please login again to continue.";
        }
    }
}