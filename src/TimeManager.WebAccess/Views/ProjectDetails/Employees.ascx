<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.ViewModels.TaskViewModel>" %>
<%
    if(Model.AssignedEmployees!=null)
    if (Model.AssignedEmployees.Count() > 0)
        foreach (var employee in Model.AssignedEmployees)
        {
            if (employee != null)
            {
             %>
                <span><%: employee.ShortName%>;</span>
            <%	 
}
            else
            { %>
                <span><%: "Empty"%>;</span>
            <%
             }
	 
    }
    else
    { 
    %>
    <span>Не назначены</span>
    <%
    }
%>
