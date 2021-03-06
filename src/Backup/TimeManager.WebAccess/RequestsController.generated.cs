// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace Infocom.TimeManager.WebAccess.Controllers {
    public partial class RequestsController {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public RequestsController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RequestsController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult RequestUpdate() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.RequestUpdate);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult RequestInsert() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.RequestInsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult RequestDelete() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.RequestDelete);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public RequestsController Actions { get { return MVC.Requests; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Requests";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass {
            public readonly string Index = "Index";
            public readonly string RequestUpdate = "RequestUpdate";
            public readonly string RequestInsert = "RequestInsert";
            public readonly string RequestDelete = "RequestDelete";
        }


        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames {
            public readonly string Index = "~/Views/Requests/Index.aspx";
            static readonly _EditorTemplates s_EditorTemplates = new _EditorTemplates();
            public _EditorTemplates EditorTemplates { get { return s_EditorTemplates; } }
            public partial class _EditorTemplates{
                public readonly string RequestEditor = "RequestEditor";
            }
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_RequestsController: Infocom.TimeManager.WebAccess.Controllers.RequestsController {
        public T4MVC_RequestsController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index() {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult RequestUpdate(Infocom.TimeManager.WebAccess.Models.RequestModel requestModel) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.RequestUpdate);
            callInfo.RouteValueDictionary.Add("requestModel", requestModel);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult RequestInsert(Infocom.TimeManager.WebAccess.Models.RequestModel requestModel) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.RequestInsert);
            callInfo.RouteValueDictionary.Add("requestModel", requestModel);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult RequestDelete(long id) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.RequestDelete);
            callInfo.RouteValueDictionary.Add("id", id);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
