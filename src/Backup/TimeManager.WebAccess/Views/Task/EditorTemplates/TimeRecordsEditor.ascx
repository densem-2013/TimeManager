<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl< Infocom.TimeManager.WebAccess.Models.TimeRecordModel>" %>
<%--<%: Html.Hidden("EmployeeLogin", Request.LogonUserIdentity.Name)%>--%>
<%: Html.Hidden("EmployeeLogin", Session["userLogin"])%>
<%--StartDate--%>
<div style="font-weight: bold;">
    <%: Html.LabelFor(model => model.StartDate) %>
</div>
<div class="edit-form-longer">
    <%: Html.EditorFor(model => model.StartDate)%>
    <%: Html.ValidationMessageFor(model => model.StartDate)%>
</div>
<%--SpentTime--%>
<div style="font-weight: bold;">
    <%: Html.LabelFor(model => model.SpentTime) %>
</div>
<div class="edit-form-longer">
    <%: Html.Telerik().TimePickerFor(model => model.SpentTime)%>
    <%: Html.ValidationMessageFor(model => model.SpentTime)%>
</div>
