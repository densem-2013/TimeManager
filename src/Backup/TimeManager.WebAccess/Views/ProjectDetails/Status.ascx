<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.Core.DomainModel.Status>" %>
 <%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
 <%@ Import Namespace="Infocom.TimeManager.Core.DomainModel" %>
<%:    Html.Telerik().ComboBox()
        .Name("status")
             .BindTo(
                 new SelectList(
                    (IEnumerable)ViewData["taskStatuses"] ?? new List<Status>(),
                    "Id", 
                    "Name",
                                        Model.SafeGet(t => t.ID, () => ((IEnumerable<Status>)ViewData["taskStatuses"]).First().ID, 0)
                     )).AutoFill(true)
 %>