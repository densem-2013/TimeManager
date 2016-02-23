<%@ Control Language="C#"  
Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.ViewModels.RequestsViewModel>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Filters" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Models" %>
 <%
     var requestEditorView = (RequestEditorViewModel) ViewBag.RequestEditorViewModel;
     var requestTypes = new List<RequestTypeModel>();
     requestTypes.Add(new RequestTypeModel() { ID=0, Name = "Все" });
     requestTypes.Add(new RequestTypeModel() { ID = 1, Name = "Административные работы" });
     requestTypes.Add(new RequestTypeModel() { ID = 2, Name = "Проекты" });
     requestTypes.Add(new RequestTypeModel() { ID = 3, Name = "Техническая проработка" });
     requestTypes.Add(new RequestTypeModel() { ID = 4, Name = "Производство" });
     requestTypes.Add(new RequestTypeModel() { ID = 5, Name = "Обучение" });
     requestTypes.Add(new RequestTypeModel() { ID = 6, Name = "Сервисное обслуживание" });
     requestTypes.Add(new RequestTypeModel() { ID = 7, Name = "Вызов специалиста" });
     
     var statuses = new List<RequestStatusModel>();
     statuses.Add(new RequestStatusModel() { Name = "Все" });
     statuses.Add(new RequestStatusModel() { ID = 1, Name = "Необработанная" });
     statuses.Add(new RequestStatusModel() { ID = 2, Name = "В работе" });
     statuses.Add(new RequestStatusModel() { ID = 3, Name = "Открытые" });
     statuses.Add(new RequestStatusModel() { ID = 4, Name = "Изменённые"});     
     statuses.Add(new RequestStatusModel() { ID = 5, Name = "Закрытые" });
     var filter = new RequestsFilter();

     if (Session["RequestsFilter"] != null)
     {
         filter = (RequestsFilter)Session["RequestsFilter"];
     }
     using (Html.BeginForm("Index", "Requests", FormMethod.Post))
     {
%>
 <table width='100%'>
    <tr>
     <%if (this.Session["userDepartmentId"] != null)
           if (Model.IsAdmin || (bool)this.Session["RequestPermission"])
       {%>
        <td>
            <a class='t-button-custom' href='/Requests?Requests-mode=insert'>
            <span class='t-add t-icon'></span>Создать новую заявку</a>
       </td>
       <%}%>
       <td>
        Фильтры:
       </td>
       <td>
                    <div>Тип:</div>
                    <hr />
                        <div>Статус:</div>
        </td>
       <td style=" width:150px;">
            <%:Html.Telerik()
                    .DropDownList()
                    .Name("FilteringByRequestTypeID")
                    .BindTo(new SelectList(requestTypes, "ID", "Name",
                        filter.FilteringByRequestTypeID))
                    .Value(filter.FilteringByRequestTypeID.ToString())%>
            <div>
             <%:Html.Telerik()
                    .DropDownList()
                             .Name("filteringByStatusID")
                    .BindTo(new SelectList(statuses, "ID", "Name", filter.FilteringByStatusID))
                    .Value(filter.FilteringByStatusID.ToString())%>

            </div>
         </td>
         <td>
          <input type="submit" value="Применить"  />
         </td>
        <td>
                    <div>Поиск:</div>
        </td>
       <td>
            <%: Html.TextBox("FilteringBySearchWord", filter.FilteringBySearchWord, new { style = "width:250px;"})%>
           <input type='submit' value='Найти' /> 
       </td>
    </tr>
</table>
<%}%>
       