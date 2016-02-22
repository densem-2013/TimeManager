<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.Models.EmployeeRateModel>" %>

 <% 
        var isEditingMode = Model.ID != 0;  
    %>
    
   <%   if (isEditingMode)
        { %>
        <div  style=" font-weight:bold; font-size:12px;" >
    <%: Html.ValidationSummary(true, "Пожалуйста, исправьте ошибки и повторите.") %>
    <%--FirstName--%>
    <div ><%: Html.Label("", "ФИО сотрудника:") %></div>
   <div >
 <%--  <%:Html.Label("", string.Format("{0} {1}.{2}.", Model.LastName, Model.FirstName.First(), Model.PatronymicName.First()))%>--%>
   </div>
    <%--Rate--%>
    <br/>
   <div ><%: Html.LabelFor(model => model.Rate) %></div>
   <div><%: Html.TextBoxFor(model => model.Rate) %>
</div>
    <% } else
        { %>
     
     
        <%
    }
%>


