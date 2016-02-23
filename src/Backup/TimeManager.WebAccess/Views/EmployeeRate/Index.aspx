<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Infocom.TimeManager.WebAccess.ViewModels.EmployeeRateViewModel>" %>

<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    История ставок сотрудника
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server">
    <asp:HyperLink ID="HyperLink1" CssClass="navlink" ForeColor="#3F3F3F" NavigateUrl="~/Employee/Index"
        runat="server">Сотрудники</asp:HyperLink><span class="navlink"> > История ставок сотрудника</span>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% if (Roles.IsUserInRole("TimeManagerAdministrators"))
       { %>
    <table width="100%">
        <tr>
            <td style="width: 120px;">
                <%= Html.Label("Имя: ") %>
            </td>
            <td>
                <%= Html.LabelFor(m => m.Employee.FirstName, Model.Employee.FirstName) %>
            </td>
        </tr>
        <tr>
            <td>
                <%= Html.Label("Фамилия: ") %>
            </td>
            <td>
                <%= Html.LabelFor(m => m.Employee.LastName, Model.Employee.LastName) %>
            </td>
        </tr>
        <tr>
            <td>
                <%= Html.Label("Отчество: ") %>
            </td>
            <td>
                <%= Html.LabelFor(m => m.Employee.PatronymicName, Model.Employee.PatronymicName) %>
            </td>
        </tr>
        <tr>
            <td>
                <%= Html.Label("Дата поступления: ") %>
            </td>
            <td>
            <span style="font-size: 12px; font-weight: bold">
                    <%=
    Model.Employee.HireDate.HasValue
        ? Model.Employee.HireDate.Value.ToString("dd.MM.yyyy")
        : "Не известна" %>
                </span>
            </td>
        </tr>
        <tr>
            <td style="width: 120px;">
                <%= Html.Label("Текущая ставка: ") %>
            </td>
            <td>
                <%= Html.LabelFor(m => m.Employee.Rate, Math.Round(Model.Employee.Rate.Value, 2).ToString()) %>
            </td>
        </tr>
    </table>
    <br />
    <%
        bool isLast = false;

%>
    <%=
    Html.Telerik().Grid(Model.EmployeeRates.OrderBy(m => m.RateNumber)).Name("EmployeeRates").DataKeys(
        datakeys => datakeys.Add(c => c.ID))
        .DataBinding(b => b.Server()
                              .Select("SelectIndex", "EmployeeRate", new {employeeId = Model.Employee.ID})
                              .Insert("EmployeeRateInsert", "EmployeeRate", new {employeeId = Model.Employee.ID, Model})
                              .Update("EmployeeRateUpdate", "EmployeeRate",
                                      new {employeeId = Model.Employee.ID, employeeRateId = ID})
                              .Delete("EmployeeRateDelete", "EmployeeRate",
                                      new {employeeId = Model.Employee.ID, employeeRateId = ID})
        )
        .TableHtmlAttributes(new {id = "EmployeeRatesTable"})
        .ToolBar(commands =>
                 commands.Insert()
                     .ButtonType(GridButtonType.ImageAndText)
                     .ImageHtmlAttributes(new {m = Model.Employee.ID}))
        .Columns(
            c =>
                {
                    c.Bound(p => p.Employee.ID).Hidden(true);
                    c.Bound(p => p.RateNumber).Width(200).ReadOnly(true).Template(
                        t => string.Format("{0}", t.RateNumber == 0 ? Model.EmployeeRates.Count() + 1 : t.RateNumber));
                    c.Bound(p => p.Rate).Width(200);

                    c.Bound(p => p.StartDate).Format("{0:dd.MM.yyyy}").Width(270).EditorTemplateName("DateTimeNullable");
                    c.Bound(p => p.FinishDate).Format("{0:dd.MM.yyyy}").Width(270).EditorTemplateName("DateTimeNullable");
                    c.Command(
                        s =>
                            {
                                s.Edit().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(
                                    new {style = "margin: 0 3px"});
                                s.Delete().ButtonType(GridButtonType.ImageAndText).HtmlAttributes(
                                    new {style = "display: none", name = "deleteButton"});
                            }).Width(165);

                })
        .Editable(e => e.Mode(GridEditMode.InLine)
                           .TemplateName("EmployeeRateEditor").Window(
                               wb => wb.Title("Редактирование ставки сотрудника")
                                         .ClientEvents(eb => eb.OnLoad("onEmployeeRateLoad"))
                                         .ClientEvents(eb => eb.OnClose("onEmployeeRateClose"))))

        .Resizable(res => res.Columns(false))
        .Pageable(p => p.PageSize(25))
        .Sortable()
        .Filterable()
        .Groupable()

    %>
       <% }
          else
       {
           %><span style="color:Red;"> Ошибка: <%: this.ViewBag.OverviewErrorMessage%>.</span><%
       } %>
    
    <style type="text/css">
        .t-grid-add
        {
            background: #E8E8E8 !important;
            border-color: #A7BAC5;
            padding: 0 !important;
            font-family: Tahoma,Verdana,Arial,sans-serif;
            font-weight: bold;
            font-size: 12px;
            color: #628EC0;
            border-bottom: 1px solid #628EC0;
            height: 18px;
            width: 169px;
        }
        
    </style>
    <script type="text/javascript">
        function onEmployeeRateLoad(e) {
            var element = document.getElementById("EmployeeRatesPopUp");
            element.style.cssText = "top:20%;left:14%";
            if (windowType.search("edit") > 0) {
                $('span.t-window-title').append("<b>Редактирование информации о сотруднике</b>");
            }
            else {
                $('span.t-window-title').append("<b>Добавить ставку</b>");
            }
            $('div.t-overlay')[0].attributes[1].value = '';
        }

        function onEmployeeRateClose(e) {
            window.location = "http://localhost:50334/EmployeeRate/Index" + Model.employeeId;
        }
    </script>
    <script type="text/javascript" src="/Scripts/2011.2.712/jquery-1.5.1.min.js"></script>
    <script>
        jQuery(document).ready(function () {
            // Get array with all last columns
            var table = document.getElementById("EmployeeRatesTable");
            var lastRowIndex = table.rows.length - 1;
            var lastCellIndex = table.rows[lastRowIndex].cells.length - 1;

            var cntls = table.rows[lastRowIndex].cells[lastCellIndex].getElementsByTagName('button');
            for (var j = 0; j < cntls.length; j++) {
                if (cntls[j].name == 'deleteButton') {
                    cntls[j].setAttribute("style", "display:block");
              }
            }
        })
    </script>
    
</asp:Content>
