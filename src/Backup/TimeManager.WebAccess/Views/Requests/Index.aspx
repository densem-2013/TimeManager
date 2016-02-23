<%@ Page Title="" UICulture="ru-RU" Culture="ru-RU" Language="C#" MasterPageFile="~/Views/Shared/Site.Master"
    Inherits="ViewPage<Infocom.TimeManager.WebAccess.ViewModels.RequestsViewModel>" %>

<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Заявки
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%   var isHasErrorText = !String.IsNullOrEmpty((string)this.ViewBag.OverviewErrorMessage);
         if (isHasErrorText)
         {
    %><span style="color: Red;"> Ошибка:
        <%: this.ViewBag.OverviewErrorMessage%></span><%
       }
        Html.Telerik().Grid(User.IsInRole("TimeManagerAdministrators") == true ? Model.Requests.OrderByDescending(r => r.ID) : Model.Requests).Name("Requests")
             .DataKeys(dataKeys => dataKeys.Add(c => c.ID))
             .ToolBar(commands =>
                 {
                     commands.Template(() => Html.RenderPartial("ToolBarTemplate", Model));
                 })
             .DataBinding(b => b.Server()
                 .Select("Index", "Requests")
                 .Insert("RequestInsert", "Requests")
                 .Update("RequestUpdate", "Requests")
                 .Delete("RequestDelete", "Requests")
                 ).Columns(
                 c =>
                 {
                     c.Bound(p => p.Number).Width(100).Template(r => Html.ActionLink(r.Number ?? "Empty", "../RequestDetails/Index", new { RequestID = r.ID }, new { title = r.Number })).HeaderHtmlAttributes(new { style = "text-align:center" });
                     c.Bound(p => p.Date).Format("{0:dd.MM.yyyy}").Title("Дата заявки").HeaderHtmlAttributes(new { style = "text-align:center" });
                     c.Bound(p => p.StartDate).Format("{0:dd.MM.yyyy}").Title("Начало работ").HeaderHtmlAttributes(new { style = "text-align:center" });
                     c.Bound(p => p.FinishDate).Format("{0:dd.MM.yyyy}").Title("Окончание работ").HeaderHtmlAttributes(new { style = "text-align:center" });
                     c.Bound(p => p.IsValidatedValue).Title("П").HeaderHtmlAttributes(new { style = "text-align:center;" }).Filterable(false).HtmlAttributes(new { style = "text-align:center;" })
                     .Template(p =>
                     {
                         if (p.IsValidatedValue == "Подтверждена")
                         {
        %>
    <img alt="<%= p.IsValidatedValue %>" title="<%= p.IsValidatedValue %>" src="<%= Url.Content("~/Content/Images/approved.gif") %>" />
    <%
                                                                                                                  }
                                                                                                                  else
                                                                                                                  {
    %>
    <img alt="<%= p.IsValidatedValue %>" title="<%= p.IsValidatedValue %>" src="<%= Url.Content("~/Content/Images/not-approved.gif") %>" />
    <%
                                                                                                                  }
                                                                                                              });
                                                                                                              c.Bound(p => p.CustomerName).Width(120).HeaderHtmlAttributes(new { style = "text-align:center" });
                                                                                                              c.Bound(p => p.PhaseName).Width("100%").Title("Наименование проекта").HeaderHtmlAttributes(new { style = "text-align:center" });
                                                                                                              c.Bound(p => p.ProjectManagerShortName).Title("Руководитель проекта в КД").Width(130).HeaderHtmlAttributes(new { style = "text-align:center" });
                                                                                                              c.Bound(p => p.TypeName).Title("Тип заявки").Width(150).HeaderHtmlAttributes(new { style = "text-align:center" });

                                                                                                              c.Bound(p => p.ProcessState).Width(140)
                                                                                                                  .Template(ps =>
                                                                                                                  {
                                                                                                                      if (ps.StatusName != "Закрытая")
                                                                                                                      {
    %>
    <div>
        <%: ps.StatusName%>
    </div>
    <div>
        (<%: ps.ProcessState%>)
    </div>
    <%  
                                                                                                                      }
                                                                                                                      else
                                                                                                                      { 
    %>
    <div>
        <%: ps.StatusName%>
    </div>
    <%
                                                                                                                      }

                                                                                                                  }

                                                                                                              ).HeaderHtmlAttributes(new { style = "text-align:center" });
                                                                                                              if ((bool)this.Session["RequestPermission"] || Model.IsAdmin)
                                                                                                              {
                                                                                                                  c.Command(
                                                                                                                       s =>
                                                                                                                       {
                                                                                                                           s.Edit().ButtonType(GridButtonType.Image);
                                                                                                                           s.Delete().ButtonType(GridButtonType.Image);
                                                                                                                       }).Width(80);
                                                                                                              }
                                                                                                          }
                                                                                                          )
                                                                                                       .RowAction(row =>
                                                                                                  {
                                                                                                      if (row.DataItem.IsValidatedValue != "Подтверждена")
                                                                                                      {
                                                                                                          row.HtmlAttributes["style"] = "background:#FCC;";
                                                                                                      }
                                                                                                  })
                                                                                                      .Editable(e => e.Mode(GridEditMode.PopUp).TemplateName("RequestEditor").Window(wb => wb.Title(" ")
                                                                                                      .ClientEvents(eb => eb.OnLoad("onRequestEditorLoad"))
                                                                                                      .ClientEvents(eb => eb.OnClose("onRequestEditorClose"))
                                                                                                      ))
                                                                                                      .Resizable(res => res.Columns(false))
                                                                                                      .Pageable(p => p.PageSize(20))
                                                                                                      .Sortable(sorting => sorting.Enabled(true).SortMode(Telerik.Web.Mvc.UI.GridSortMode.SingleColumn)
                                                                                                        .OrderBy(o => o.Add(m => m.Date).Descending()))
                                                                                                      .Filterable()
                                                                                                      .Groupable()
                                                                                                      .Render();
    %>
    <script type="text/javascript">
        function onRequestEditorLoad(e) {
            var element = document.getElementById("RequestsPopUp");
            var windowType = document.location.search;
            element.style.cssText = "top:0%;left:25%;";
            if (windowType.search("edit") > 0) {
                $('span.t-window-title').append("<b>Редактирование заявки</b>");
            }
            else {
                $('span.t-window-title').append("<b>Создание заявки</b>");
            }
            // $('div.t-overlay')[0].attributes[1].value = '';

            var theClass = "t-overlay";

            var allTags = document.getElementsByTagName("div");

            for (i = 0; i < allTags.length; i++) {

                if (allTags[i].className == theClass) {

                    allTags[i].attributes[1].value = '';

                }
            }

            if ($('#ErrorMessage').length) {
                alert("Выполнен переход по неверной ссылке.");
                document.location.href = 'http://' + document.location.host + '/Requests';
            }
            //document.getElementById("Number").autocomplete="off";
            //document.getElementById("Contract_Name").autocomplete="off";
            if (document.getElementById("Phases_0__Name") != null) {
                document.getElementById("Phases_0__Name").autocomplete = "off";
            }

        }
        function onRequestEditorClose(e) {
            window.location = document.referrer;
        }


        
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server">
    <span class="navlink">Заявки</span>
</asp:Content>
