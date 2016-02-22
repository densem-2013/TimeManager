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
    public partial class ProjectDetailsController {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected ProjectDetailsController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Index() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Index);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult TaskInsert() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.TaskInsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult TaskUpdate() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.TaskUpdate);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult TaskDelete() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.TaskDelete);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ProjectDetailsController Actions { get { return MVC.ProjectDetails; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "ProjectDetails";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass {
            public readonly string Index = "Index";
            public readonly string TaskInsert = "TaskInsert";
            public readonly string TaskUpdate = "TaskUpdate";
            public readonly string TaskDelete = "TaskDelete";
        }


        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames {
            public readonly string Employees = "~/Views/ProjectDetails/Employees.ascx";
            public readonly string Index = "~/Views/ProjectDetails/Index.aspx";
            public readonly string Status = "~/Views/ProjectDetails/Status.ascx";
            public readonly string Status_ascx = "~/Views/ProjectDetails/Status.ascx";
            static readonly _DisplayTemplates s_DisplayTemplates = new _DisplayTemplates();
            public _DisplayTemplates DisplayTemplates { get { return s_DisplayTemplates; } }
            public partial class _DisplayTemplates{
                public readonly string ProjectManager = "ProjectManager";
                public readonly string Status = "Status";
            }
            static readonly _EditorTemplates s_EditorTemplates = new _EditorTemplates();
            public _EditorTemplates EditorTemplates { get { return s_EditorTemplates; } }
            public partial class _EditorTemplates{
                public readonly string TaskEditor = "TaskEditor";
            }
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_ProjectDetailsController: Infocom.TimeManager.WebAccess.Controllers.ProjectDetailsController {
        public T4MVC_ProjectDetailsController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index(long projectID) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Index);
            callInfo.RouteValueDictionary.Add("projectID", projectID);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult TaskInsert(Infocom.TimeManager.WebAccess.Models.TaskModel taskModel) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.TaskInsert);
            callInfo.RouteValueDictionary.Add("taskModel", taskModel);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult TaskUpdate(Infocom.TimeManager.WebAccess.Models.TaskModel taskModel) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.TaskUpdate);
            callInfo.RouteValueDictionary.Add("taskModel", taskModel);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult TaskDelete(Infocom.TimeManager.WebAccess.Models.TaskModel taskModel) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.TaskDelete);
            callInfo.RouteValueDictionary.Add("taskModel", taskModel);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
