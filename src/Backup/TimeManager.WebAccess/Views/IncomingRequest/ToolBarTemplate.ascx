<%@ Control Language="C#"  
Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.ViewModels.IncomingRequestViewModel>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Filters" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>
 <%
     //var requestEditorView = (RequestEditorViewModel) ViewBag.RequestEditorViewModel;
     //var requestTypes = requestEditorView.RequestFilterTypes;
     //var statuses = requestEditorView.RequestFilterStatuses;
     var filter = new ContractsFilter();

     if (Session["IncomingRequestFilter"] != null)
     {
         filter = (ContractsFilter)Session["IncomingRequestFilter"];
     }
     using (Html.BeginForm("Index", "IncomingRequest", FormMethod.Post))
     {
%>
 <table width='100%'>
    <tr>
     <%if (Model.IsAdmin)
       {%>
        <td>
            <a class='t-button-custom' href='/IncomingRequest?IncomingRequests-mode=insert'>
            <span class='t-add t-icon'></span>Создать новую заявку</a>
       </td>
       <%}%>
        <td>
                    <div>Поиск:</div>
        </td>
       <td>
            <%: Html.TextBox("FilteringBySearchWord", filter.FilteringBySearchWord, new { style = "width:250px;" })%>
           <input type='submit' value='Найти' /> 
       </td>
    </tr>
</table>
<%}%>
       