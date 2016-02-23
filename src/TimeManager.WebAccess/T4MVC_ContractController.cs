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
    public partial class ContractsController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ContractsController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected ContractsController(Dummy d) { }

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
            return new T4MVC_ActionResult(Area, Name, ActionNames.ContractUpdate);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult ContractInsert()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.ContractInsert);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult ContractDelete()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.ContractDelete);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ContractsController Actions { get { return MVC.Contracts; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Contracts";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string ContractUpdate = "ContractUpdate";
            public readonly string ContractInsert = "ContractInsert";
            public readonly string ContractDelete = "ContractDelete";
        }


        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames
        {
            public readonly string Index = "~/Views/Contracts/Index.aspx";
            static readonly _EditorTemplates s_EditorTemplates = new _EditorTemplates();
            public _EditorTemplates EditorTemplates { get { return s_EditorTemplates; } }
            public partial class _EditorTemplates
            {
                public readonly string ContractEditor = "ContractEditor";
            }
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_ContractController: Infocom.TimeManager.WebAccess.Controllers.ContractsController {
        public T4MVC_ContractController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ContractUpdate(Infocom.TimeManager.WebAccess.Models.ContractModel contractModel)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.ContractUpdate);
            callInfo.RouteValueDictionary.Add("requestModel", contractModel);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ContractInsert(Infocom.TimeManager.WebAccess.Models.ContractModel contractModel)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.ContractInsert);
            callInfo.RouteValueDictionary.Add("requestModel", contractModel);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ContractDelete(long id)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.ContractDelete);
            callInfo.RouteValueDictionary.Add("id", id);
            return callInfo;
        }



    }
}
