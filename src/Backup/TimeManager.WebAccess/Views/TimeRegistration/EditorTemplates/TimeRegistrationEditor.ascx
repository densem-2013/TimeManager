<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.Models.TaskModel>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>

    <% 
        var isEditingMode = Model.ProjectName != null;  
    %>
    
    <%
        var isHasErrorText = !String.IsNullOrEmpty((string)this.ViewBag.EditErrorMessage);
        if (isHasErrorText)
       {
           %><div><span style="color:Red;">Ошибка: <%: this.ViewBag.EditErrorMessage%></span></div>
           <div><span style="color:Red;">Пожалуйста, исправьте ошибки и повторите снова.</span></div><%
       }
    if (isEditingMode)
    {
    %>
   <tr>
        <td>
            <%:Html.Label("", Model.ProjectCode)%>
        </td>
        <td class="timeregistration-editing-value2">
            <%:Html.Label("", Model.ProjectName.ReduceTo(70))%>
            <%:Html.HiddenFor(model => model.ProjectID)%>
        </td>
            <td>
                <%:Model.Name.ReduceTo(50)%>
                <%:Html.HiddenFor(model => model.ID)%>
            </td>
        <td><div class="timeregistration-editing-nonzerovalue">&nbsp;<br />&nbsp;</div></td>
            <%: Html.Hidden("taskID", Model.ID) %>
            <%
                var index = 0;
                foreach (var day in (IEnumerable<DateTime>)ViewBag.DaysToEdit)
                {
            %>
            <td><div class="timeregistration-editing-nonzerovalue">
                <%: Html.Telerik().TimePicker().ShowButton(false).Name(string.Format("timeSpantByDays[{0}]", index)).Value(Model.SpendTimeOn(day))
                .HtmlAttributes(new {style = "width:100%; " })
                .InputHtmlAttributes(new { style = "text-align:right; font-weight:700;display:block; " })%>
                 <%: Html.ValidationMessage(string.Format("timeSpantByDays[{0}]", Model.SpendTimeOn(day)))%>
                </div>
            </td>
            <%
                    index++;
                }
            %>
        <td> <%:Html.Label(Model.SpendTimeByWeek((DateTime)ViewBag.DaysToEdit[0]).ToFormatedTime())%></td>
        <td class="t-last" style="margin: 0 auto">
            <button class="t-button t-button-icon t-grid-update t-visible" type="submit"><span class="t-icon t-update"></span></button>
            <a class="t-button t-button-icon t-grid-cancel t-visible" href="javascript:history.back();"><span class="t-icon t-cancel"></span></a>
        </td>
    </tr>
    <style type="text/css">
       .t-picker-wrap
    {
          padding : 0;
    }
    .t-timepicker
    {
        width : 100%;
    }
    button.t-grid-update,
    a.t-grid-cancel
    {
        display:none;
    }
    button.t-visible,
    a.t-visible
    {
        display:inline;
    }
    </style>
 
<%   }
     else //insert task
     {
%>
<div class="timeregistration-editing">
    <table style="width: 100%">
        <tr>
            <%--Project--%>
            <td class="timeregistration-editing-title">
                <%:Html.Label("", "Проект:")%>
            </td>
            <td class="timeregistration-editing-value">
                <%:Html.Telerik().DropDownList().Name("ProjectID")
                    .HtmlAttributes(new { style = string.Format("width:{0}px", 450) })
                    .DataBinding(b => b.Ajax().Select("GetProjectForEmployee", "TimeRegistration"))
                    .ClientEvents(eb => eb.OnChange("onChange"))%>
            </td>
        </tr>
        </tr>
        <tr>
            <%--Task--%>
            <td class="timeregistration-editing-title">
                <%:Html.Label("", "Задача:")%>
            </td>
            <td class="timeregistration-editing-value">
                <%:Html.Telerik().DropDownList().Name("TaskID").HtmlAttributes(new { style = string.Format("width:{0}px", 450) })%>
            </td>
        </tr>
     </table>
</div>


<%
    }
%>

