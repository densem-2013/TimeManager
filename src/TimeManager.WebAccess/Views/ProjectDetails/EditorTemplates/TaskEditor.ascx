<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.Models.TaskModel>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Models" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>

<div class="project-detail-edit-form">
    <%
        var projectDetailsEdit = (ProjectDetailsEditViewModel)ViewBag.ProjectDetailsEditViewModel; %>
    <table class="edit-form">
        <tr>
            <td class="project-detail-edit-form-left-cell" valign='top'>
                <%--Name--%>
                <div>
                    <%: Html.LabelFor(model => model.Name) %>
                </div>
                <div>
                    <%: Html.TextAreaFor(model => model.Name)%>
                    <%: Html.ValidationMessageFor(model => model.Name)%>
                </div>
            </td>
            <td class="project-detail-edit-form-right-cell" valign="top">
                <div>
                    <%: Html.LabelFor(model => model.RegistrationDate)%>
                </div>
                <div>
                    <%: Html.Telerik().DatePickerFor(model => model.RegistrationDate).Value(Model.RegistrationDate)%>
                    <%: Html.ValidationMessageFor(model => model.RegistrationDate)%>
                </div>
                <div>
                    <%: Html.LabelFor(model => model.TaskStatusID)%>
                </div>
                <div>
                    <%:  Html.Telerik().DropDownList().Name("taskStatusID")
                                                              .BindTo(new SelectList(projectDetailsEdit.TaskStatuses, "ID", "Name", Model.TaskStatusID)).SelectedIndex(1)%>
                    <%: Html.ValidationMessageFor(model => model.TaskStatusID)%>
                </div>
            </td>
        </tr>
    </table>
    <%--Description--%>
    <div class="footer-description">
        <%: Html.LabelFor(model => model.Description)%>
    </div>
    <div class="footer-description">
        <%: Html.TextAreaFor(model => model.Description)%>
        <%: Html.ValidationMessageFor(model => model.Description)%>
    </div>
    <table>
            <tr>
                <td class="project-detail-edit-form-sides">
                                     <a id="DepartmentEmployeeListLabel" href="#" onclick="ShowDepartmentEmployeeList()" style="font-weight:bold;font-size:12px;">Отдел</a>
                                     <a id="AllEmployeeListLabel" href="#" onclick="ShowAllEmployeeList()" style="font-size:12px;">Все</a>&nbsp;
                
                 </td>
               <td></td>
                
                <td class="project-detail-edit-form-sides">
                    <%: Html.Label("", "Назначенные сотрудники")%>
                </td>
            </tr>
            <tr>
                <td valign='top'>
                    <select id="notAssignedEmployees" name="D1" size="10" ondblclick="MoveEmployeeProjectListToAssignedList('notAssignedEmployees','assignedEmployeeList')"   style="display:none">
                        <%
                            var assignedEmployees =
                                (projectDetailsEdit.AllEmployees).Where( 
                                    pe => Model.AssignedEmployeeIDs.Contains(pe.ID));

                            var notAssignedEmployees =
                                (projectDetailsEdit.AllEmployees).Where(pe => pe.FireDate==null).Except(assignedEmployees);
                                      
                            foreach (EmployeeModel employees in notAssignedEmployees)
                            {
                               
                                    %><option id="<%: employees.ID %>">
                            <%: employees.ShortName %></option>
                        <%
                               
                            };
                        %>
                    </select>
                    <select id="notAssignedEmployeesDepartment" name="D5" size="10" ondblclick="MoveEmployeeProjectListToAssignedList('notAssignedEmployeesDepartment','assignedEmployeeList')">
                                        <%
                               foreach (EmployeeModel item in notAssignedEmployees)
                               if (item.DepartmentID == (long)this.Session["userDepartmentId"])
                                   
                               {                           
                                        %><option id="<%: item.ID%>">
                                            <%: item.ShortName%></option>
                                        <%                          
                               };
                                        %>
                                    </select>
                </td>
               <td align=center>
                    <div>
                        <input id="addEmployeeButton" onclick="MoveEmployeeProjectListToAssignedList('notAssignedEmployeesDepartment','assignedEmployeeList')"
                                            type="button" value="&gt;&gt;" /></div>
                    <div>
                        <input id="removeEmployeeButton" onclick="MoveEmployeeAssignedListToProjectList('assignedEmployeeList','notAssignedEmployeesDepartment')"
                                            type="button" value="&lt;&lt;" /></div>
                </td>
                <td>
                    <div id="emploeeListDiv">
                        <select id="assignedEmployeeList" name="Employees" size="10" multiple="multiple" ondblclick="MoveEmployeeAssignedListToProjectList('assignedEmployeeList','notAssignedEmployeesDepartment')">
                            <%
                                foreach (EmployeeModel employee in assignedEmployees)  
                                {
                                %>
                                <option id="<%: employee.ID%>">
                                    <%: employee.ShortName%></option>
                                <%
                                };
                            %>
                        </select>
                    </div>
                    <div id="div_hidden">
                    <% 
                        foreach (EmployeeModel employee in assignedEmployees)  
                                {
                                %>
                                <div id='<%: "employee"+employee.ID%>'><input type='hidden' name='AssignedEmployeeIDs' value='<%: employee.ID%>' /></div>
                                <%
                                };
                         %>
                    </div>
                </td>
            </tr>
        </table>
    </div>
