using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Web.Mvc;
using T4MVC;

namespace Infocom.TimeManager.WebAccess.Controllers
{
    public partial class IncomingRequestController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public IncomingRequestController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected IncomingRequestController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult ContractUpdate()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.IncomingRequestUpdate);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult ContractInsert()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.IncomingRequestInsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult ContractDelete()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.IncomingRequestDelete);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public IncomingRequestController Actions { get { return MVC.IncomingRequest; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "IncomingRequest";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string IncomingRequestUpdate = "IncomingRequestUpdate";
            public readonly string IncomingRequestInsert = "IncomingRequestInsert";
            public readonly string IncomingRequestDelete = "IncomingRequestDelete";
        }


        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames
        {
            public readonly string Index = "~/Views/IncomingRequest/Index.aspx";
            static readonly _EditorTemplates s_EditorTemplates = new _EditorTemplates();
            public _EditorTemplates EditorTemplates { get { return s_EditorTemplates; } }
            public partial class _EditorTemplates
            {
                public readonly string IncomingRequestEditor = "IncomingRequestEditor";
            }
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_IncomingRequestController : Infocom.TimeManager.WebAccess.Controllers.IncomingRequestController
    {
        public T4MVC_IncomingRequestController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult IncomingRequestUpdate(Infocom.TimeManager.WebAccess.Models.IncomingRequestModel incomingRequestModel)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.IncomingRequestUpdate);
            callInfo.RouteValueDictionary.Add("requestModel", incomingRequestModel);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult IncomingRequestInsert(Infocom.TimeManager.WebAccess.Models.IncomingRequestModel incomingRequestModel)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.IncomingRequestInsert);
            callInfo.RouteValueDictionary.Add("requestModel", incomingRequestModel);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult IncomingRequestDelete(long id)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.IncomingRequestDelete);
            callInfo.RouteValueDictionary.Add("id", id);
            return callInfo;
        }



    }
}
