<%@ Page Title="" UICulture="ru-RU" Culture="ru-RU" Language="C#"
 MasterPageFile="~/Views/Shared/Site.Master"
    Inherits="ViewPage<Infocom.TimeManager.WebAccess.ViewModels.ContractViewModel>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   Договора
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% var isHasErrorText = !String.IsNullOrEmpty((string)this.ViewBag.OverviewErrorMessage);
        if (isHasErrorText)
       {
           %><span style="color:Red;"> Ошибка: <%: this.ViewBag.OverviewErrorMessage%>. Пожалуйста, исправьте ошибки и повторите действие снова.</span><%
       } 
 Html.Telerik().Grid(Model.Contracts).Name("Contracts")
            .DataKeys(dataKeys => dataKeys.Add(c => c.ID))
            .ToolBar(commands =>
                {
                    commands.Template(() => Html.RenderPartial("ToolBarTemplate", Model));
                })
            .DataBinding(b => b.Server()
                .Select("Index", "Contracts")
                .Insert("ContractInsert", "Contracts")
                .Update("ContractUpdate", "Contracts")
                .Delete("ContractDelete", "Contracts")
                ).Columns(
                c =>
                {
                    c.Bound(p => p.Number).Width(100);//.Template(r => Html.ActionLink(r.Number ?? "Empty", "../RequestDetails/Index", new { RequestID = r.ID }, new { title = r.Number }));
                    c.Bound(p => p.SigningDate).Format("{0:dd.MM.yyyy}").Width(100);
                    c.Bound(p => p.CompletionDate).Format("{0:dd.MM.yyyy}").Width(100);
                    c.Bound(p => p.Customer.Name).Width(200);
                    c.Bound(p => p.Name).Width(200).Title("Наименование договора").Template(p => p.Name.ReduceTo(50) );;
                    c.Command(
                           s =>
                           {
                               s.Edit().ButtonType(GridButtonType.Image);
                               s.Delete().ButtonType(GridButtonType.Image);
                           }).Width(80);
                    }
                )
            .Editable(e => e.Mode(GridEditMode.PopUp).TemplateName("ContractEditor").Window(wb => wb.Title(" ")
            .ClientEvents(eb => eb.OnLoad("onContractEditorLoad"))
            .ClientEvents(eb => eb.OnClose("onContractEditorClose"))
            ))
            .Resizable(res => res.Columns(false))
            .Pageable(p => p.PageSize(20))
            .Sortable()
           // .Filterable()
            .Groupable()
            .Render();
    %>
     <script type="text/javascript">
         function onContractEditorLoad(e) {
             var element = document.getElementById("ContractsPopUp");
             var windowType = document.location.search;
             //document.getElementById("Number").autocomplete="off";
             //document.getElementById("Contract_Name").autocomplete="off";
             //document.getElementById("Contract_Number").autocomplete="off";
             element.style.cssText = "top:30%;left:30%;";
             if (windowType.search("edit") > 0) {
                 $('span.t-window-title').append("<b>Редактирование договора</b>");
             }
             else {
                 $('span.t-window-title').append("<b>Создание договора</b>");
             }
             $('div.t-overlay')[0].attributes[1].value = '';
         }
         function onContractEditorClose(e) {
             window.location = document.referrer;
         }
         function onChangeContractName(e) {
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

<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server">
<span class="navlink">Договора</span>
</asp:Content>
