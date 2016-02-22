<%@ Control Language="C#"  
Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.ViewModels.ContractViewModel>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Filters" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>
 <%
     var filter = new ContractsFilter();

     if (Session["ContractsFilter"] != null)
     {
         filter = (ContractsFilter)Session["ContractsFilter"];
     }
     using (Html.BeginForm("Index", "Contracts", FormMethod.Post))
     {
%>
 <table width='100%'>
    <tr>
     <%--<%if (Model.IsAdmin)
       {%>--%>
        <td>
            <a class='t-button-custom' href='/Contracts?Contracts-mode=insert'>
            <span class='t-add t-icon'></span>Создать новый договор</a>
       </td>
      <%-- <%}%>--%>
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
       