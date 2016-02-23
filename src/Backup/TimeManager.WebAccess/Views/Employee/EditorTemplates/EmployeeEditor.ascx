<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.Models.EmployeeModel>" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Web.Helpers" %>
<%@ Import Namespace="Infocom.TimeManager.Core.DomainModel" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Models" %>
<div style="font-weight: bold; font-size: 12px;">
    <%
        var departments = ViewData["Departments"] as List<Department>;%>
    <%: Html.ValidationSummary(true, "Пожалуйста, исправьте ошибки и повторите.")%>
    <table width="450px">
        <tr>
            <td>
                <%:Html.LabelFor(model => model.FirstName)%>
                <br />
                <%:Html.TextBoxFor(model=>model.FirstName, new {style="width:120px;"})%>
                <br />
              
            </td>
            <td>
                <%:Html.LabelFor(model => model.LastName)%>
                <br />
                <%:Html.TextBoxFor(model=>model.LastName,new {style="width:200px;"})%>
                <br />
             
            </td>
            <td>
                <%:Html.LabelFor(model => model.PatronymicName)%>
                <br />
                <%:Html.TextBoxFor(model => model.PatronymicName, new { style = "width:200px;" })%>
                <br />
             
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <div>
                    <div style="display: inline; width: 280px; float: left;">
                        <%:Html.LabelFor(model => model.DepartmentShortName)%>
                        <span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                        <% var dep = new List<DepartmentModel>();
                           foreach (var department in departments)
                           {
                               var d = new DepartmentModel { ID = department.ID, ShortName = department.ShortName };
                               dep.Add(d);

                           } %>
                        <%:Html.Telerik().DropDownList().Name("DepartmentID")
                       .Value(Model.DepartmentID.ToString(CultureInfo.InvariantCulture))
                       .BindTo(new SelectList(dep, "ID", "ShortName")).HtmlAttributes(new { style = "width:100px; align:center;" })%>
                    </div>
                    <div style="width: 300px; margin-left: 40%;">
                        <%:Html.LabelFor(model=>model.Rate) %>
                        <%:Html.TextBoxFor(model => model.Rate, new { style = "width:70px;" })%>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <div>
                    <div style="display: inline; width: 280px; float: left;">
                        <%:Html.LabelFor(model=>model.HireDate)%>
                        <span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                          <%:Html.Telerik().DatePickerFor(model => model.HireDate).HtmlAttributes(new { style = "width:120px;" })%>
                      
                    </div>
                    <div style="width: 320px; margin-left: 40%;">
                    <%:Html.LabelFor(model=>model.FireDate)%>
                       <span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                    <%:Html.Telerik().DatePickerFor(model => model.FireDate).HtmlAttributes(new { style = "width:120px;" })%>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <div>
                    <div style="display: inline; width: 280px; float: left;">
                <%:Html.LabelFor(model=>model.Login) %>
                   <span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                <%:Html.TextBoxFor(model => model.Login, new { style = "width:200px;" })%>
                </div>
                <div style="width: 300px; margin-left: 45%;">
                <%:Html.LabelFor(model=>model.Email) %>
                   <span>&nbsp;&nbsp;&nbsp;&nbsp;</span>
                <%:Html.TextBoxFor(model => model.Email, new { style = "width:200px;" })%>
                </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <%:Html.Label("", "Менеджер по кадрам")%>
           <span>&nbsp;&nbsp;&nbsp;&nbsp;</span>
                <%:Html.CheckBox("IsHumanResource", Model.HumanResource == 1)%>
               
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
</div>
