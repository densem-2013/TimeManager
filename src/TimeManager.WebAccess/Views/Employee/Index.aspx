<%@ Page Language="C#" UICulture="ru-RU" Culture="ru-RU" MasterPageFile="~/Views/Shared/Site.Master"
    Inherits="System.Web.Mvc.ViewPage<Infocom.TimeManager.WebAccess.ViewModels.EmployeesViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Сотрудники
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server">
    <span class="navlink">Сотрудники</span>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        th.t-header.t-last-header
        {
            width: 20px;
        }         
    </style>
    <% 
          
        var isHasErrorText = !String.IsNullOrEmpty((string)this.ViewBag.OverviewErrorMessage);
        if (isHasErrorText)
        {
    %><span style="color: Red;"> Ошибка:
        <%: this.ViewBag.OverviewErrorMessage%>.</span><%
        }
        else
        {

            if (Roles.IsUserInRole("TimeManagerAdministrators"))
            {


                Html.Telerik().Grid(Model.Employees).Name("Employees")
                    .DataKeys(dataKeys => dataKeys.Add(c => c.ID))
                    // .ToolBar(commands => commands.
                    //Insert().ImageHtmlAttributes(new { style = "margin-left:0" }))
                    //Template(string.Format(@"<a class=""t-button-custom"" onclick=""AddEmployee()""><span class=""t-add t-icon""></span>Добавить сотрудника</a>")))
                    // Template(string.Format(@"<a class=""t-button-custom""  href=""/Employee?Employees-mode=insert""><span class=""t-add t-icon""></span>Добавить сотрудника</a>")))
                    .DataBinding(b => b.Server()
                                          .Select("Index", "Employee")
                                          .Insert("EmployeeInsert", "Employee")
                                          .Update("EmployeeUpdate", "Employee")
                    ).Columns(
                        c =>
                        {
                            c.Bound(p => p.LastName).Template(
                                p =>
                                Html.ActionLink((p.LastName) ?? "Empty", "../EmployeeRate/Index", new { ID = p.ID }))
                                .
                                Width(150).ReadOnly(true);
                            //.Template(p => Html.ActionLink((p.LastName) ?? "Empty", "../EmployeeRate/Index", new { ID = p.ID }))
                            c.Bound(p => p.FirstName).Width(150).ReadOnly(true);
                            c.Bound(p => p.PatronymicName).Width(150).ReadOnly(true);
                            c.Bound(p => p.Rate).Width(170).ReadOnly(true); ;
                            c.Bound(p => p.HireDate).Format("{0:dd.MM.yyyy}").Width(170).EditorTemplateName(
                                "DateTimeNullable").ReadOnly(true); 
                            c.Bound(p => p.FireDate).Format("{0:dd.MM.yyyy}").Width(170).EditorTemplateName(
                                "DateTimeNullable").ReadOnly(true); 

                            c.Bound(p => p.Login)
                                .Template(
                                    p =>
                                    Html.ActionLink((p.Login) ?? "Empty", "../Employee/LogonAs",
                                                    new { Login = p.Login }))
                                .HeaderTemplate(() =>
                                                    {
        %><a class='t-grid-header' href="/TimeRegistration?TimeRecords-orderBy=spentTimeInMonday-asc">
            <div>
                Выполнить вход от имени
            </div>
        </a>
    <%

                                                        }).ReadOnly(true);


                             
                            })
                    .Resizable(res => res.Columns(false))
                    .Pageable(p => p.PageSize(25))
                    .Sortable()
                    .Filterable()
                    .Groupable()
                    .Render();
    %>
    <% }
        }
        if ((bool)this.Session["HumanResource"])
        {
            Html.Telerik().Grid(Model.Employees).Name("Employees")
                             .DataKeys(dataKeys => dataKeys.Add(c => c.ID))
                              .ToolBar(commands => commands.
                             Template(@"<a class=""t-button-custom"" href=""/Employee?Employees-mode=insert""><span class=""t-add t-icon""></span>Добавить нового сотрудника</a>"))

                             .DataBinding(b => b.Server()
                                                   .Select("Index", "Employee")
                                                   .Insert("EmployeeInsert", "Employee")
                                                   .Update("EmployeeUpdate", "Employee")
                             ).Columns(
                                 c =>
                                 {
                                     c.Bound(p => p.LastName).
                                         Width(120).ReadOnly(false);
                                     c.Bound(p => p.FirstName).Width(120).ReadOnly(false);
                                     c.Bound(p => p.PatronymicName).Width(150).ReadOnly(false);
                                     c.Bound(p => p.DepartmentID).Hidden(true);
                                     c.Bound(p => p.DepartmentShortName).Width(120);
                                     c.Bound(p => p.Rate).Width(120);
                                     c.Bound(p => p.HireDate).Format("{0:dd.MM.yyyy}").Width(150).EditorTemplateName(
                                         "DateTimeNullable");
                                     // c.Bound(p => p.FireDate).Format("{0:dd.MM.yyyy}").Width(150).EditorTemplateName(
                                     //"DateTimeNullable");

                                     c.Bound(p => p.Login)
                                        .ReadOnly(false).Width(180);
                                     c.Bound(p => p.Email).Width(100);


                                     c.Command(
                                         s =>
                                         {
                                             s.Edit().ButtonType(GridButtonType.Image).HtmlAttributes(new { style = "width:20px;" });
                                         });
                                 })
                             .Editable(e => e.Mode(GridEditMode.PopUp)
                                                .TemplateName("EmployeeEditor").Window(
                                                    wb => wb.Title("Информация о сотруднике")
                                                              .ClientEvents(eb => eb.OnLoad("onEmployeeEditorLoad"))
                                                              .ClientEvents(eb => eb.OnClose("onEmployeesClose"))))
                             .Resizable(res => res.Columns(false))
                             .Pageable(p => p.PageSize(25))
                             .Sortable()
                             .Filterable()
                             .Groupable()
                             .Render();
        } %>
    <script type="text/javascript">
        function onEmployeeEditorLoad(e) {
            var element = document.getElementById("EmployeesPopUp");
            element.style.cssText = "top:20%;left:35%; width:600px;";

            $('div.t-overlay')[0].attributes[1].value = '';
            var txt = $('#HireDate');
            
            var date = Date.now();
            txt.value= dateToYmd(date);
        }

        function onEmployeesClose(e) {
            window.location = "http://localhost:50334/Employee";
        }
    </script>
</asp:Content>
