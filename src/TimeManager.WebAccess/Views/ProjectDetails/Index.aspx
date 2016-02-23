<%@ Page Title="" UICulture="ru-RU" Culture="ru-RU" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<Infocom.TimeManager.WebAccess.ViewModels.ProjectDetailsViewModel>" %>

<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>

<%@ Import Namespace="Infocom.TimeManager.WebAccess.Models" %>

<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>

<%@ Import Namespace="Telerik.Web.Mvc.Extensions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали проекта
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

     <%: Html.ValidationSummary(true, "Пожалуйста, исправьте ошибки и повторите.")%>
     <hr />
     
     <%
         var projectDetailsEdit = (ProjectDetailsEditViewModel)ViewBag.ProjectDetailsEditViewModel;
         var phase = projectDetailsEdit.Phases.Where(p => p.ID == Model.Project.PhaseID).FirstOrDefault();
         var isHasErrorText = !String.IsNullOrEmpty((string)this.ViewBag.OverviewErrorMessage);
         
         
        
         
         
         
        if (isHasErrorText)
           {
               %><span style="color:Red;"> Ошибка: <%: this.ViewBag.OverviewErrorMessage%>. Пожалуйста, исправьте ошибки и повторите действие снова.</span><%
           } 
    %>
    <div class="project_left_info">
        <table class="menucontainer" >
            <tr>
                <td style="width: 10%">
                    Наименование:
                </td>
                <td style="width: 70%">
                    <%: Model.Project.Name%>
                </td>

                <td> <%
                               {%>
                                      <button class='t-button t-button-icontext' style="float:right;"onclick="window.location='<%= string.Format("/ProjectsOverview/Index/{0}?Projects-mode=edit&directEdit=1", Model.Project.ID) %>';" >
                                        <span style="white-space:nowrap;"><span class="t-icon t-edit"></span>
                                        Редактировать
                                       </button> 
                               <% }
                               %>
                </td>
             </tr>
            <tr>
                <td >    Описание:
                </td>
                <td>
                    <%: Model.Project.Description%>
                </td>
            </tr>
        </table>
        <table style="width: 100%;">
           <tr>
              <td style="width: 70%;">
                 <table class="menucontainer">
            <tr>
                <td  style="width: 15%">
                    Руководитель проекта:
                </td>
                <td  style="width: 35%">
                    <%: Model.Project.ProjectManager.ShortName ?? "Не указан"%>
                </td>
                <td  style="width: 15%">
                    Номер заявки:
                </td>
                <td  style="width: 35%">
                    <a href='../RequestDetails/Index?RequestID=<%: Model.Project.RequestID %>' ><%:Model.Project.RequestNumber%></a>
                </td>
            </tr>
            <tr>
                <td>
                    Код проекта:
                </td>
                <td>
                    <%:Model.Project.Code%>
                </td>
                <td >
                   Тип проекта:
                </td>
                <td>
                    <%:Model.Project.ProjectTypeName%>
                </td>
                
            </tr>
            <tr>
                <td>
                    Статус проекта:
                </td>
                <td >
                    <%: Model.Project.CurrentStatusName%>
                 <% if (Model.Project.CurrentStatusID != 131074 && Model.IsAdmin)
                      {
                          %><!--<input type="button" value="Закрыть" onclick='ProjectCloseConfirm("<%: Model.Project.ID%>")'>-->
                     <% } %>
                </td>
                <td >
                  Дата начала работ:  
                </td >
                <td>
                  <%:Model.Project.Request.StartDate.GetDateTimeFormats('d').First()%> 
                </td>
            </tr>
            <tr>
            <td>
                    Приоритет:
                </td>
                <td>
                    <%:Model.Project.ProjectPiorityName%>
                </td>
                <td>
                Дата окончания работ:
                </td>
                <td>
                 <%: phase.DeadLine.Value.GetDateTimeFormats('d').First()%>
                </td>
            </tr>
            <tr>
                <td >
                    Договор:
                </td>
                <td>
                    <%: Model.Project.Contract.Name%>
                </td>
                <td >
                    Номер и дата договора:
                </td>
                <td>
                    <%: string.Format("№ {0} от {1:dd.MM.yyyy}",Model.Project.Contract.Number,Model.Project.Contract.SigningDate)%>
                </td>
            </tr>
            <tr>
                <td>
                    Количество экземпляров проекта:
                </td>
                <td>
                    <%: Model.Project.Request.Documentation %>
                </td>
            </tr>
        </table>
              </td>
                          

              <td valign="top" style="width: 50%">
                  <div>Распределение бюджетов:</div>
                    <% if (Model.IsAdmin)
                    {
                        %><table class="t-widget t-grid">
                         <thead class="t-grid-header">
                         <tr>
                                <td  style="width: 20%">Департамент</td>
                                <td  style="width: 10%">Бюджет, чел.ч</td>
                                <td  style="width: 20%">Бюджет, грн.</td>
                                <td  style="width: 10%">Затраты, чел.ч</td>
                                <td  style="width: 10%">Затраты, грн.</td>
                                <td  style="width: 30%">Дата завершения работ</td>
                            </tr>
                            </thead>
                            <tbody><%
                            bool tAlt = true;
                            foreach (var budgetAllocation in phase.Budget.BudgetAllocations)
                            {
                                tAlt = !tAlt;
                                %>
                            <tr<%:(tAlt ? " class=t-alt" :"") %>>
                            <td  style="width: 10%">
                                <%: string.Format("{0}",budgetAllocation.Department.ShortName)%>
                            </td>
                            <td  style="width: 45%; text-align:right;">
                                <%: string.Format("{0}",budgetAllocation.AmountOfHours)%>
                            </td>
                            <td  style="width: 45%; text-align:right;">
                                <%: string.Format("{0:0.00}",budgetAllocation.AmountOfMoney)%>
                            </td>
                             <% var budget = new Infocom.TimeManager.WebAccess.ViewModels.BudgetViewModel();
                                  budget = ControllerHelper.GetBudgetModelBy(budgetAllocation.Department.ID, Model.Project.ID);
                              %>  
                            <td style="width: 20%; text-align:right;"> 
                                <%: string.Format("{0}", budget.SpentTime)%>
                            </td>
                            <td style="width: 20%; text-align:right;"> 
                                <%: string.Format("{0}", budget.SpentMoney)%>
                            </td>
                            <td style="width: 20%; text-align:right;"> 
                                <%: string.Format("{0:dd.MM.yyyy}", budgetAllocation.DateEndWorkByDepartment)%>
                            </td>
                            </tr><%}
                            %>
                            </tbody>
                            </table><%
                        }%>
              </td>
              <%--<td><button>Кнопка</button></td>--%>
           </tr>
           
           <%-- <tr>

                
                </tr>--%>
        </table>
        <table style='width:100%;'>
          <tr>
             <td style='width:20%;'>
                    <%: Html.LabelFor(model=>model.Project.AssignedEmployeeIDs)%>:
                </td>
             <td>
                    <%
                       var assignedEmployees =
                           (projectDetailsEdit.AllEmployees.Where(
                               e => Model.Project.AssignedEmployeeIDs.Contains(e.ID)));
                        if (Model.Project.AssignedEmployeeIDs.Count() > 0)
                        {
                                       
                            foreach (EmployeeModel employee in assignedEmployees)
                            {
                                RouteValueDictionary routeValues = new RouteValueDictionary();
                                routeValues.Add("ProjectID", Model.Project.ID);
                                routeValues.Add("tasks-filter", "AssignedEmployeeNames~substringof~'"+employee.ShortName+"'");
                                %>
                                <%: Html.ActionLink(employee.ShortName, "Index", routeValues)  %>
                                
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
          </tr>
         
        </table>
    </div>
    <hr />
    <br />
    <span class="project_left_info">Задачи: </span>
    <%
        Html.Telerik().Grid(Model.Tasks.OrderByDescending(t=>t.ID))
            .Name("tasks")
            .DataKeys(k => k.Add(t => t.ID))
            .ToolBar(commands => commands.
                                Template(string.Format(@"<a class=""t-button-custom"" href=""/ProjectDetails/Index?tasks-mode=insert&ProjectID={0}""><span class=""t-add t-icon""></span>Создать задачу</a>", Model.Project.ID)))
                .Columns(c =>
            {
                c.Bound(p => p.Name).Width(300).Title("Наименование")
                    .Template(x =>
                         {
                             if (x != null)
                             {
    %>
    <%: Html.ActionLink(x.Name ?? "Empty", "../Task/Index", new { TaskID = x.ID })%>
    <%
                             }
                         });

                c.Bound(t => t.CurrentStatusName).Template(
                    x =>
                    {
    %><%:Html.Label("", x.CurrentStatusName)%>
    <%
                    }).Title("Статус задания");

                c.Bound(t => t.AssignedEmployeeNames)
                    .Template(x =>
                        {%>
    <%:  Html.Label("", x.AssignedEmployeeNames) %>
                        <% }).Title("Назначеные сотрудники");
                
                 c.Bound(t => t.TotalSpentTime).Template(
                     x =>
                     {
    %><%: x.TotalSpentTime.ToFormatedTime()%>
    <%
                           
                        }).Title("Затрачено времени");
                c.Command(s =>
                {
                    s.Edit().ButtonType(GridButtonType.Image);
                    s.Delete().ButtonType(GridButtonType.Image);
                }).Width(80);
            })
            .DataBinding(b => b.Server()
                    .Select("Index", "ProjectDetails", new { ProjectID = Model.Project.ID })
                    .Insert("TaskInsert", "ProjectDetails", new { ProjectID = Model.Project.ID })
                    .Update("TaskUpdate", "ProjectDetails", new { ProjectID = Model.Project.ID })
                    .Delete("TaskDelete", "ProjectDetails", new { ProjectID = Model.Project.ID })
                    )
                .Editable(e => e.Mode(GridEditMode.PopUp).TemplateName("TaskEditor").Window(wb => wb.Title(" ")
                .ClientEvents(eb => eb.OnLoad("onTasksEditorLoad"))
                .ClientEvents(eb => eb.OnClose("onTasksEditorClose"))).DisplayDeleteConfirmation(true))
                .Resizable(res => res.Columns(false))
                .Pageable(p => p.PageSize(40))
              //  .Sortable()
               .Filterable()
                .Sortable()
                .Render();
    %>
    <script type="text/javascript">
        function onTasksEditorLoad(e) {
            var element = document.getElementById("tasksPopUp");
            element.style.cssText = "top:25%;left:32%";
            var windowType = document.location.search;
            if (windowType.indexOf("-mode=edit")>0) {
                $('span.t-window-title').append("<b>Редактирование задачи</b>");
            }
            else {
                $('span.t-window-title').append("<b>Создание задачи</b>");
            }
            $('div.t-overlay')[0].attributes[1].value = '';
            
        }

        function onTasksEditorClose(e) {        
            window.llocation = document.referrer; 
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server" >
   <span class="navlink">
        <%: Html.ActionLink("Проекты", MVC.ProjectsOverview.Index())%>
    </span>
    <span class="navlink">></span> 
            
    <span class="navlink">

        <%: Model.Project.Name.ReduceTo(30)%>
    </span>
</asp:Content>
  

