<%@ Control Language="C#"  
Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.ViewModels.RequestsViewModel>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Filters" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Models" %>
 <%
     var requestEditorView = (RequestEditorViewModel) ViewBag.RequestEditorViewModel;
     var requestTypes = new List<RequestTypeModel>();
     requestTypes.Add(new RequestTypeModel() { ID=0, Name = "���" });
     requestTypes.Add(new RequestTypeModel() { ID = 1, Name = "���������������� ������" });
     requestTypes.Add(new RequestTypeModel() { ID = 2, Name = "�������" });
     requestTypes.Add(new RequestTypeModel() { ID = 3, Name = "����������� ����������" });
     requestTypes.Add(new RequestTypeModel() { ID = 4, Name = "������������" });
     requestTypes.Add(new RequestTypeModel() { ID = 5, Name = "��������" });
     requestTypes.Add(new RequestTypeModel() { ID = 6, Name = "��������� ������������" });
     requestTypes.Add(new RequestTypeModel() { ID = 7, Name = "����� �����������" });
     
     var statuses = new List<RequestStatusModel>();
     statuses.Add(new RequestStatusModel() { Name = "���" });
     statuses.Add(new RequestStatusModel() { ID = 1, Name = "��������������" });
     statuses.Add(new RequestStatusModel() { ID = 2, Name = "� ������" });
     statuses.Add(new RequestStatusModel() { ID = 3, Name = "��������" });
     statuses.Add(new RequestStatusModel() { ID = 4, Name = "���������"});     
     statuses.Add(new RequestStatusModel() { ID = 5, Name = "��������" });
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
            <span class='t-add t-icon'></span>������� ����� ������</a>
       </td>
       <%}%>
       <td>
        �������:
       </td>
       <td>
                    <div>���:</div>
                    <hr />
                        <div>������:</div>
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
          <input type="submit" value="���������"  />
         </td>
        <td>
                    <div>�����:</div>
        </td>
       <td>
            <%: Html.TextBox("FilteringBySearchWord", filter.FilteringBySearchWord, new { style = "width:250px;"})%>
           <input type='submit' value='�����' /> 
       </td>
    </tr>
</table>
<%}%>
       