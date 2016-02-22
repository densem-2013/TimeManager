<%@ Page Title="" UICulture="ru-Ru" Culture="ru-Ru" Language="C#" MasterPageFile="~/Views/Shared/Site.Master"
    Inherits="System.Web.Mvc.ViewPage<Infocom.TimeManager.WebAccess.ViewModels.TimeRegistrationViewModel>" %>

<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<%@ Import Namespace="System.Globalization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Регистрация времени
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%-- Header --%>
    <table style="margin-left: 20px;">
        <tr>
            <td>
                <%: Html.Telerik()
                        .Calendar()
                        .Name("Calendar")
                        .Selection(sb => sb.Action("Index", new {selectedDate = "{0}"}))  
                                            .Value(Model.SelectedDate) 
                %>
            </td>
            <td valign="top">
                <table style="font-weight: bold;">
                    
                    <tr style="padding: 0px;">
                        <td>
                            <h2 style="margin-top: 0px;">
                                Информация о работе пользователя:</h2>
                        </td>
                    </tr>
                   
                    <tr>
                        <td>
                            Рабочая неделя:
                        </td>
                        <td style="text-align: right;">
                            <%: Model.SelectedDate.WeekNumber() %>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Количество отработанного времени:
                        </td>
                    </tr>
                    <tr>
                        <td>
                            - за неделю:
                        </td>
                        <td>
                            <div style="text-align: right;">
                                <%: Model.TimeSheet.TotalSpentTimeByWeek.ToFormatedTime()
                                %></div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            - за месяц (<%: string.Format("{0:MMMM}", Model.SelectedDate) %>):
                        </td>
                        <td>
                            <div style="text-align: right;">
                                <%: Model.TotalSpentTimeInMonth.ToFormatedTime()%></div>
                        </td>
                    </tr>
                     <tr>
                        <td>
                        <div style="text-align: left; color: #A42225;">
                            - за месяц по табелю (<%: string.Format("{0:MMMM}", Model.SelectedDate) %>):
                            </div>
                        </td>
                        <td>
                            <div style="text-align: right; color: #A42225;">
                                <%: Model.TotalSpentTimeInMonthFromTimeSheet.ToFormatedTime()%></div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
     <%
        
         var isHasErrorText = !String.IsNullOrEmpty((string)this.ViewBag.OverviewErrorMessage);
        if (isHasErrorText)
       {
           %><span style="color:Red;"> Ошибка: <%: this.ViewBag.OverviewErrorMessage%>. Пожалуйста, исправьте ошибки и повторите действие снова.</span><%
       } 
    %>
   <% 
       Html.Telerik().Grid(Model.TimeSheet.Tasks.OrderBy(ts => ts.Name).OrderBy(ts => ts.ProjectName))
            .Name("TimeRecords")
            .TableHtmlAttributes(new { @class = "t-grid-timeregistration" })
            .DataKeys(k => k.Add(o => o.ID))
            .DataBinding(b => b.Server()
                .Insert("TimeSheetAddTask", "TimeRegistration", new { selectedDate = Model.SelectedDate, timeSheetID = Model.TimeSheet.ID })
                .Update("TimeSheetUpdateTask", "TimeRegistration", new { selectedDate = Model.SelectedDate, timeSheetID = Model.TimeSheet.ID })
                .Delete("TimeSheetDeleteTask", "TimeRegistration", new { selectedDate = Model.SelectedDate, timeSheetID = Model.TimeSheet.ID, taskID = Model.ID })
                 )
            .ToolBar(commands =>
            {
                commands.Template(@"<a class=""t-button-custom"" href=""/TimeRegistration?TimeRecords-mode=insert""><span class=""t-add t-icon""></span>Добавить задачу</a>");
               
            })
            .Columns(columns =>
                {
                                                               
                    columns.Bound(r => r.ProjectCode)
                        .HeaderTemplate(() =>
                            {
                                %><a class='t-grid-header' href="/TimeRegistration?TimeRecords-orderBy=spentTimeInMonday-asc">
                                    <div>
                                        Код проекта
                                    </div>
                                </a>
                                <%
                        
                            });
                    columns.Bound(r => r.ProjectName)
                        .Template(p => Html.ActionLink(p.ProjectName.ReduceTo(60) ?? "Empty", "../ProjectDetails/Index", new { ProjectID = p.ProjectID }))
                        .HeaderTemplate(() =>
                            {
                                %><a class='t-grid-header' href="/TimeRegistration?TimeRecords-orderBy=spentTimeInMonday-asc">
                                    <div>
                                        Наименование проекта
                                    </div>
                                </a>
                                <%
                        
                            });

                    columns.Bound(r => r.Name)
                        .Template(task => Html.ActionLink(task.Name.ReduceTo(40) ?? "Empty", "../Task/Index", new { TaskID = task.ID }, new { title = task.Name }))
                        .HeaderTemplate(() =>
                            {
                                %><a class='t-grid-header' href="/TimeRegistration?TimeRecords-orderBy=spentTimeInMonday-asc">
                                    <div>
                                        Наименование задачи</div>
                                </a>
                                <%
                        
                            }).FooterTemplate(() =>
                            {%>
                                <div>Суммарное время:</div>
                                <div style="color: #A42225;">Время по табелю:</div>
                                <%});
                    columns.Bound(r => r.CurrentStatusName).HeaderTemplate(() =>
                           {
                                %><a class='t-grid-header' href="/TimeRegistration?TimeRecords-orderBy=spentTimeInMonday-asc">
                                       <div>Статус</div><div>задачи</div>
                                </a>
                                
                                <%
                           }).Template(t => Html.RenderPartial("StateTemplate", t));
                           
                           
                           
                           
                           
                           

                    for (int i = 0; i <= 6; i++)
                    {
                        var dayToProcess = Model.SelectedDate.AddDays(i);

                        columns.Bound("Day" + i).Width("40px")
                               .HtmlAttributes(new { @class = Model.isWeekend(dayToProcess) ? "t-weekend" : "" })
                               .HeaderHtmlAttributes(new { @class = Model.isWeekend(dayToProcess) ? "t-header-weekend" : "" })
                               .HeaderTemplate(() => Html.RenderPartial("HeaderTemplate", dayToProcess))
                               .Template(t => Html.RenderPartial("BodyTemplate", t.SpendTimeOn(dayToProcess)))
                               .FooterTemplate(() =>
                                   {
                                       Html.RenderPartial("FooterTemplate", Model.TimeSheet.SpendTimeOn(dayToProcess));
                                       Html.RenderPartial("TimeSheetTemplate", Model.SpendTimeOn(dayToProcess));
                                   })
                               .FooterHtmlAttributes(new { @class = Model.isWeekend(dayToProcess) ? "t-weekend" : "" });
                    }

                    columns.Bound(r => r.TotalSpentTime).HeaderTemplate(() =>
                                {
                                %><a class='t-grid-header' href="/TimeRegistration?TimeRecords-orderBy=SpentTimeInSunday-asc">
                                    <span >
                                        <div>
                                            Сумма<br/>
                                            в часах</div>
                                    </span></a>
                                                                                                     
                                   <%
                                })
                                
                                .Template(t =>
                                    Html.RenderPartial("BodyTemplate", t.TotalSpentTime)
                                    )
                                .Template(t => Html.RenderPartial("BodyTemplate", t.SpendTimeByWeek(Model.SelectedDate)))
                                .FooterTemplate(() =>
                            {
                                Html.RenderPartial(
                                    "FooterTemplate", new TimeSpan(Model.TimeSheet.TotalSpentTimeByWeek.Ticks));
                                Html.RenderPartial("TimeSheetTemplate", Model.TotalSpentTimeByWeek());
                            });
                                                          
                            //columns.Command(
                            //s =>
                            //{
                             
                                    
                   columns.Command(
                            s =>
                            {

                                s.Edit().ButtonType(GridButtonType.Image);   // кнопка редактирования времени в задаче
                                s.Delete().ButtonType(GridButtonType.Image);
                            }).Width(80).HeaderTemplate(() => {
                          
                                    RouteValueDictionary routValueDictionary = ViewContext.RouteData.Values;
                                routValueDictionary.Add("timeSheetID", Model.TimeSheet.ID);
                                routValueDictionary.Add("selectedDate", Model.TimeSheet.Date); 

                                using (Html.BeginForm("TimeSheetDeleteTaskssss", "TimeRegistration", routValueDictionary)) 
                                {%>
                                      <button class='t-button t-button-icontext' onclick="return confirm('Вы уверены что хотите удалить все задачи?');" >
                                        <span style="white-space:nowrap;"><span class="t-icon t-delete"></span>
                                        Удалить</span><br />
                                        все задачи
                                       </button>
                               <% }

                            }).Title("Удалить все задачи").HtmlAttributes(new { style = "margin: 0 auto" });

                })
                .CellAction(row =>
                {
                    // "DataItem" is the Order object to which the current row is bound to
                    if (row.DataItem.CurrentStatusName == "Закрытая" && row.Column.Title == "Удалить все задачи")
                    {
                        
                        row.HtmlAttributes["style"] = "visibility:hidden";
                    }
                })
            .Editable(e => e.Mode(GridEditMode.InForm).TemplateName("TimeRegistrationEditor").Window(wb => wb.Title(" ")
            .ClientEvents(eb => eb.OnLoad("onProjectsEditorLoad"))))
                .Resizable(res => res.Columns(false))
                .Selectable()
                .Pageable(p => p.PageSize(40))
                .Render();
    %>
    <script type="text/javascript">
      function onProjectsEditorLoad(e) {
           var element = document.getElementById("TimeRecordsPopUp");
            element.style.cssText = "top:30%; left:24%";
            var windowType = document.location.search;
           if (windowType.search("edit") > 0) {
               $('span.t-window-title').append("<b>Редактирование времени по выбранной задаче</b>");
           }            else {
                $('span.t-window-title').append("<b>Добавление задачи в расписание</b>");
           }
            $('div.t-overlay')[0].attributes[1].value = '';
        }

        function onDropDownChange(e) {
           
        }

        function onChange(e) {
            var first = $('#ProjectID').data('tDropDownList');
            var second = $('#TaskID').data('tDropDownList');
            $.ajax(
                 { type: "POST",
                     url: "/TimeRegistration/GetTasksForProject",
                     data: "projectID=" + first.value(),
                     success: function (data) { second.dataBind(data); },
                     error: function (req, status, error) {
                         alert('Data Not Found')
                     }
                 });
             }
             
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server">
    <span class="navlink">Регистрация времени</span>
</asp:Content>
