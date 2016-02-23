<%@ Page Title="" UICulture="ru-RU" Culture="ru-RU" Language="C#" 
MasterPageFile="~/Views/Shared/Site.Master"
Inherits="ViewPage<Infocom.TimeManager.WebAccess.ViewModels.ProjectsOverviewViewModel>" %>

<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Filters" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Проекты
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server">
    <asp:HyperLink ID="HyperLink1" CssClass="navlink" NavigateUrl="~/ProjectsOverview/Index"
        runat="server">Проекты</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
         var isHasErrorText = !String.IsNullOrEmpty((string)this.ViewBag.OverviewErrorMessage);
        if (isHasErrorText)
       {
           %><span style="color:Red;"> Ошибка: <%: this.ViewBag.OverviewErrorMessage%></span><%
       } 
    %>  
    <% Html.Telerik().Grid(Model.Projects).Name("Projects")
            .DataKeys(dataKeys => dataKeys.Add(c => c.ID))
            .ToolBar(commands =>
                {
                   commands.Template(() => Html.RenderPartial("ToolBarTemplate", Model));
                })//                        onclick='onChangeSearchWord()'
            .DataBinding(b => b.Server()
                .Select("Index", "ProjectsOverview")
                .Insert("ProjectInsert", "ProjectsOverview")
                .Update("ProjectUpdate", "ProjectsOverview")
                .Delete("ProjectDelete", "ProjectsOverview")
                ).Columns(
                c =>
                {
                    c.Bound(p => p.CurrentStatusName).Width(80).HeaderHtmlAttributes(new { style = "text-align:center" });
                    if (Model.IsAdmin)
                    {
                        c.Bound(p => p.OpenedTasks).Width(110).Title("Задачи").Template(
                            p =>
                                {
%>
                              <div>
                              <%:string.Format("Открытых: {0}", p.OpenedTasks)%>
                              </div>
                              <div>
                              <%:string.Format("Закрытых: {0}", p.ClosedTasks)%>
                              </div>
                              <div>
                              <%:string.Format("Всего: {0}", p.AmountTasks)%>
                              </div>
                              <%
                                }).HeaderHtmlAttributes(new { style = "text-align:center" });
                    }
                    c.Bound(p => p.ProjectTypeName).Width(90).HeaderHtmlAttributes(new { style = "text-align:center" });
                    c.Bound(p => p.Code).Width(100).HeaderHtmlAttributes(new { style = "text-align:center" });
                    c.Bound(p => p.Name).Width(200).Template(p => Html.ActionLink(p.Name.ReduceTo(50) ?? "Empty", "../ProjectDetails/Index", new { ProjectID = p.ID }, new { title = p.Name })).HeaderHtmlAttributes(new { style = "text-align:center" });
                    c.Bound(p => p.ProjectPiorityName).Width(100).HeaderHtmlAttributes(new { style = "text-align:center" });
                    c.Bound(p => p.Request.StartDate).Format("{0:dd.MM.yyyy}").Title("Дата начала работ").Width(100).HeaderHtmlAttributes(new { style = "text-align:center" });
                    c.Bound(p => p.DeadLine).Format("{0:dd.MM.yyyy}").Title("Дата окончания работ").Width(100).HeaderHtmlAttributes(new { style = "text-align:center" });
                    c.Bound(p => p.Contract.Customer.Name).Width(150).HeaderHtmlAttributes(new { style = "text-align:center" });
                    c.Bound(p => p.RequestNumber).Width(90).Template(p => Html.ActionLink(p.RequestNumber.ToString(), "../RequestDetails/Index", new { RequestID = p.RequestID }, new { title = p.RequestNumber })).HeaderHtmlAttributes(new { style = "text-align:center" });

                    if (Model.IsAdmin)
                    {
                        c.Command(
                            s =>
                            {
                                s.Edit().ButtonType(GridButtonType.Image);
                                s.Delete().ButtonType(GridButtonType.Image);
                            }).Width(70);
                    }
                    
                })
            .Editable(e => e.Mode(GridEditMode.PopUp).TemplateName("ProjectsEditor").Window(wb => wb.Title(" ")
            .ClientEvents(eb => eb.OnLoad("onProjectsEditorLoad"))
            .ClientEvents(eb => eb.OnClose("onProjectsEditorClose"))))
            .Resizable(res => res.Columns(false))
            .Pageable(p => p.PageSize(15))
            .Sortable(sorting => sorting.Enabled(true).SortMode(Telerik.Web.Mvc.UI.GridSortMode.SingleColumn)
            .OrderBy(o => o.Add(m => m.Request.StartDate).Descending()))
            //.Filterable()
            .Groupable()
            .Render();
    %>
    <script type="text/javascript">
        function onProjectsEditorLoad(e) {
            var element = document.getElementById("ProjectsPopUp");
            element.style.cssText = "top:20%;left:14%";
            var windowType = document.location.search;
            if (windowType.search("edit") > 0) {
                $('span.t-window-title').append("<b>Редактирование проекта</b>");
            }
            else {
                $('span.t-window-title').append("<b>Создание проекта</b>");
            }
            $('div.t-overlay')[0].attributes[1].value = '';

            if ($('#ErrorMessage').length) {
                alert("Выполнен переход по неверной ссылке.");
                document.location.href = 'http://' + document.location.host + '/ProjectsOverview';
            }
        }
        function onProjectsEditorClose(e) {
            window.location = document.referrer;
        }

        function onChangeSearchWord() {
            var searchWord = document.getElementById("searchWord").value;
            var _location = location.pathname + "/SearchWord?Value=" + searchWord;
            var params = "menubar=yes,location=yes,resizable=yes,scrollbars=yes,status=yes";
            window.open(_location, "Yandex", params);

        }

        function onChangePhase(e) {
            var element = document.getElementById("Name");
            var text = "";
            for (var key in phases) {
                if (phases[key].Value == e.value) {
                    var phaseName = phases[key].Text;
                    text = phaseName;
                }
            }
            element.value = text;
        }
    </script>
  
</asp:Content>
