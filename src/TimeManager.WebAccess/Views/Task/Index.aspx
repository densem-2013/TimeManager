<%@ Page Title="" UICulture="ru-RU" Culture="ru-RU" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" 
    Inherits="ViewPage<Infocom.TimeManager.WebAccess.ViewModels.TaskViewModel>" %>

<%@ Import Namespace="Infocom.TimeManager.WebAccess.Models" %>

<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Задачи
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.ValidationSummary(true, "Пожалуйста, исправьте ошибки и повторите.")%>
    <hr />
    <div class="project_left_info">
        <table class="menucontainer">
            <tr>
                <td style="width:15%;">
                    <%: Html.LabelFor(model=>model.Task.Name)%>:
                </td>
                <td>
                    <%: Model.Task.Name%>
                </td>
            </tr>
            <tr>
                <td>
                    <%: Html.LabelFor(model=>model.Task.Description)%>:
                </td>
                <td>
                    <%: Model.Task.Description%>
                </td>
            </tr>
            <tr>
                <td>
                    <%: Html.LabelFor(model=>model.Task.CurrentStatusName)%>:
                </td>
                <td>
                    <%: Model.Task.CurrentStatusName%>
                    <%if (Model.Task.TaskStatusID != 131075)
                      {
                       %><input type="button" value="Закрыть" onclick='TaskCloseConfirm("<%:  Model.Task.ID %>")'><%   
                      } %>
                    
                </td>
            </tr>
            </table>
         <table class="menucontainer">
            <tr>
                <td>
                    <%: Html.LabelFor(model=>model.Task.AssignedEmployeeIDs)%>:
                </td>
                <td>
                    <% 
                        if (Model.AssignedEmployees.Count() > 0)
                        {
                            foreach (EmployeeModel employee in Model.AssignedEmployees)
                            {
                    %>
                    <div id="<%: employee.ID%>">
                        <%: employee.ShortName%>
                    </div>
                    <%
                            }
                        }
                        else
                        {%>
                    <div>
                        нет назначеных
                    </div>
                    <%
                        }
                    %>
                </td>
                <td>
                    Затрачено времени (мной/всеми):
                </td>
                <td>
                     <%: string.Format("{0} / {1}", Model.SpentTimeByMe.ToFormatedTime(), Model.SpentTimeByAll.ToFormatedTime())%>
                </td>
            </tr>
        </table>
    </div>
    <hr />
    <br />
    <span class="project_left_info">Отработанное время: </span>
    <%
        Html.Telerik().Grid(Model.TimeSheetOverviews)
            .Name("TimeSheet")
            .DataKeys(kf => kf.Add(tr => tr.Title))
            //.ToolBar(commands =>
            //    commands.Template(string.Format(
            //        @"<a class=""t-button-custom"" href=""/Task/Index?TimeRecords-mode=insert&TaskID={0}"">" +
            //        @"<span class=""t-add t-icon""></span>Создать</a>", Model.Task.ID)))
            .Columns(c =>
                {
                    c.Bound(ts => ts.Title).Title("Неделя").Template(
                        ts => {%><a href='/TimeRegistration/Index/?selecteddate=<%:string.Format("{0:dd.MM.yyyy}",ts.Date) %>'>
                        <%: ts.Title%></a><%});
                    
                    
                    c.Bound(tr => tr.SpentTime).Title("Затрачено времени").Template(st => st.SpentTime.ToFormatedTime());
  
                })
                .Pageable(p=>p.PageSize(15))
                .Sortable()
                .Render();
    %>
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server">
    <span class="navlink">
        <%: Html.ActionLink("Проекты", MVC.ProjectsOverview.Index())%></span> <span class="navlink">
            ></span> <span class="navlink">
                <%: Html.ActionLink(Model.Task.ProjectName.ReduceTo(40), MVC.ProjectDetails.Index(Model.Task.ProjectID))%></span>
    <span class="navlink">></span> <span class="navlink">
        <%:Model.Task.Name.ReduceTo(30)%></span>
</asp:Content>
