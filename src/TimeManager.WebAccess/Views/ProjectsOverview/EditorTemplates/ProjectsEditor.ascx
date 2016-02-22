<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.Models.ProjectModel>" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Models" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>

<div class="project-edit-form">
   <%
       var projectEditorViewModel = new ProjectEditorViewModel();
           projectEditorViewModel =   (ProjectEditorViewModel)ViewBag.ProjectEditorViewModel;
          
        var projectModel = new ProjectModel();
       if (!string.IsNullOrEmpty(Request.Params["RequestId"]))
       {
           projectModel = ControllerHelper.GetProjectModel(long.Parse(Request.Params["RequestId"]));
           //projectEditorViewModel = ControllerHelper.GetProjectEditorViewModel();
       }       
   %>
   <% if (Model == null)
      {
           %>
           <script type="text/javascript">
               $('button.t-button t-button-icon t-grid-update').remove();
           </script>
                <div id='ErrorMessage' style="text-align:center;"><label></label></div> 
           <% 
      }
      else
      {
       %>
    <table>
        <tr>
            <td valign="top" >
                <fieldset>
                    <legend>
                        <%: Html.Label("", "Проект")%></legend> 
                        <%--Name--%>
                        <div class="edit-form">
                            <%: Html.LabelFor(model => model.Name)%>
                        </div>
                        <div>
                        <% if (string.IsNullOrEmpty(projectModel.Name))
                           {
                           %>
                                <%: Html.TextAreaFor(model => model.Name, new { style = "height:90px; " })%>
                                <%: Html.ValidationMessageFor(model => model.Name)%>
                           <%
                           }
                           else 
                           { 
                               %>
                                    <textarea name="Name"><%: projectModel.Name %></textarea>
                               <%
                           } %>
                            
                        </div>
                    <table class="edit-form">

                          <tr>
                            <td valign="top">
                               
                                 <%--ProjectCode--%>
                                <div>
                                    <%: Html.LabelFor(model => model.Code)%>
                                </div>
                                <div class="edit-form-longer">
                                 <% if (string.IsNullOrEmpty(projectModel.Code))
                           {
                           %>
                                    <%: Html.TextBoxFor(model => model.Code, new { @class = "projectOvervieweEditLeftTextBox", })%>
                                    <%: Html.ValidationMessageFor(model => model.Code)%>
                                     <%
                           }
                           else 
                           { 
                               %>
                                    <input name="Code" value='<%: projectModel.Code %>' />
                               <%
                           } %>
                                </div>
                            </td>
                            <td valign="top">
                                 <%--TypeId--%>
                                <div>
                                    <%: Html.LabelFor(model => model.ProjectTypeID)%>
                                </div>
                                <div>
                                    <%
                                      var selectedIndex = Model.ProjectTypeID.ToString();
                                      if (Model.ProjectTypeID == 0)
                                      {
                                          selectedIndex = "2";
                                      }
                                      if (projectModel.ProjectTypeID!=0)
                                      {
                                          selectedIndex = projectModel.ProjectTypeID.ToString();
                                      }
                                      %>
                                        <%:Html.Telerik().DropDownList().Name("projectTypeID").BindTo(
                                            new SelectList(projectEditorViewModel.Types.Where(pt => pt.ID != 0), "ID", "Name", Model.ProjectTypeID)).
                                            Value(selectedIndex)%>
                                </div>
                                </td>
                        </tr>
                        <tr>
                                <td>
                                 <%--ProjectPriority--%>
                                <div>
                                    <%: Html.LabelFor(model => model.ProjectPiorityID)%>
                                </div>
                               <div>
                                    <%                                        
                                      var selectedProjectPiorityID = Model.ProjectPiorityID.ToString();
                                      if (Model.ProjectPiorityID == 0)
                                      {
                                          selectedProjectPiorityID = "2";
                                      }
                                     
                                      %>
                                    <%:Html.Telerik().DropDownList().Name("ProjectPiorityID").BindTo(
                                        new SelectList(projectEditorViewModel.ProjectPriorities
                                            .Where(pt => pt.ID != 0), "ID", "Name", Model.ProjectPiorityID))
                                            .Value(selectedProjectPiorityID)%>
                                </div>
                                </td>
                               <td valign="top">
                                <%--Status--%>
                                <div>
                                    <%: Html.LabelFor(model => model.CurrentStatusID)%>
                                </div>
                                <div class="edit-form-longer">
                                    <%: Model.CurrentStatusName %>
                                    <%-- Создать hiden поля для всех--%>
                                </div>
                            </td>
                        </tr>

                        <tr>
                        <td>

                           <%--StartDate--%>
                                                     
                           <div>
                           <%: Html.LabelFor(model => projectModel.Request.StartDate)%>
                           </div>
                           <div>

                            <% if (string.IsNullOrEmpty(Request.Params["RequestId"]))
                           {
                           %>
                                <%: Model.Request.StartDate.GetDateTimeFormats('d').First()%>
                           <%
                           }
                           else 
                           { 
                               %>
                                      <%: projectModel.Request.StartDate.ToShortDateString() %>
                               <%
                           } %>
                                                                                                            
                           </div>
                           </td>

                            <td>
                                <%--Documentation--%>
                                                     
                                   <div>
                                   <%: Html.LabelFor(model => projectModel.Request.Documentation)%>
                                   </div>
                                   <div>

                                    <% if (string.IsNullOrEmpty(Request.Params["RequestId"]))
                                   {
                                   %>
                                        <%: Model.Request.Documentation%>
                                   <%
                                   }
                                   else 
                                   { 
                                       %>
                                              <%: projectModel.Request.Documentation %>
                                       <%
                                   } %>

                                   </div>

                            </td>
                        </tr>

                    </table>
                    <%--Description--%>
                    <div class="project-detail-edit-form-sides">
                        <%: Html.LabelFor(model => model.Description)%>
                    </div>
                    <div>
                        <%: Html.TextAreaFor(model => model.Description, new { style = "height:100px;" })%>
                        <%: Html.ValidationMessageFor(model => model.Description)%>
                    </div>
                </fieldset>
            </td>
            <td valign="top">
                <div class="footer-description">
                     <fieldset>
                    <legend>
                        <%: Html.Label("", "Заявка")%></legend>
                        <div id="request">
                       
                    <table class="edit-form" style="margin-top: -15px;">
                        
                            <% if ((Model.RequestID == 0 || Model.PhaseID == 0)&&projectModel.RequestID==0)
                               {	
                              %>
                              <tr>
                                <td class="project-edit-form-left-cell" valign="top" align="left">
                                <div>
                                    <input style=" width:10%;" checked="checked" name="requestVariant" type="radio" value="new" onclick="ChangeRequestVariant('new')" /> <%: Html.Label("", "Не обработанные")%> 
                                    <input style=" width:10%;" name="requestVariant" type="radio" value="old" onclick="ChangeRequestVariant('old')"/>  <%: Html.Label("", "Обработанные")%> 
                                    <input style=" width:10%;" name="requestVariant" type="radio" value="all" onclick="ChangeRequestVariant('all')"/>  <%: Html.Label("", "Все")%> 
                                </div>
                                </td>
                            </tr>
                              <%
                               } %>
                                
                        <tr class="project-edit-form-left-cell" valign='top'>
                        <td>
                        <div style = "width:600px;" >
                       
                        <table>
                        <tr>
                            <td  style = "width:10%;">
                               <div>
                                <%: Html.Label("", "Заявка:")%>
                               </div>
                            </td>
                            <td>
                             <%if (Model.RequestID == 0 && projectModel.RequestID == 0)
                               {%>
                               <%:
                                       Html.Telerik().DropDownList().Name("RequestID").DataBinding(
                                           b => b.Ajax().Select("GetNewRequests", "ProjectsOverview"))
                                           .ClientEvents(eb => eb.OnChange("GetPhase"))
                                           .HtmlAttributes(
                                                   new { style = "width:100%;" })%>
                              <%:Html.ValidationMessageFor(model => model.RequestID)%>
                              <%}
                               else
                               {
                                   long requestID    = 0;
                                   if (Model.RequestID == 0 && projectModel.RequestID>0)
                                   {
                                        requestID = projectModel.RequestID;
                                   }
                                   else 
                                   { 
                                        requestID = Model.RequestID;
                                   }
                                   var requestName = ControllerHelper.GetCombinatedRequestName(requestID);
                                   %>
                               <%: Html.Label("", requestName)%>
                                 <input type='hidden' name='requestId' value='<%: requestID%>' />
                               <%
                               }%>
                            </td>
                         </tr>
                        </table>
                        </div>
                        </td>   
                        </tr>
                        </table>
                           <%-- <input type="button" value="Изменить" onclick="EditRequest()">--%>
                        </div>
                        <div id="requestForEdit">
                        </div>
                </fieldset>
                </div>
                <div class="project-overview-edit-form">
                    <fieldset>
                        <legend>
                            <%: Html.Label("", "Сотрудники")%></legend>
                        <table>
                            <tr>
                                <td class="project-detail-edit-form-sides">
                                     <a id="DepartmentEmployeeListLabel" href="#" onclick="ShowDepartmentEmployeeList()" style="font-weight:bold;font-size:12px;">Отдел</a>
                                     <a id="AllEmployeeListLabel" href="#" onclick="ShowAllEmployeeList()" style="font-size:12px;">Все</a>&nbsp;
                                   
                                </td>
                                <td>
                                </td>
                                <td class="project-detail-edit-form-sides">
                                    <%: Html.Label("", "Назначенные сотрудники")%>
                                </td>
                            </tr>
                            <tr>
                                <td valign='top'>
                                    <select id="notAssignedEmployees" name="D1" size="10" ondblclick="MoveEmployeeProjectListToAssignedList('notAssignedEmployees','assignedEmployeeList')"  style="display:none">
                                        <%
                               var assignedEmployees =
                                   projectEditorViewModel.AllEmployees.Where(pe => Model.AssignedEmployeeIDs.Contains(pe.ID));

                               var notAssignedEmployees =
                                   projectEditorViewModel.AllEmployees.Where(pe => pe.FireDate == null).Except(assignedEmployees);

                               foreach (EmployeeModel item in notAssignedEmployees)
                               {                           
                                        %><option id="<%: item.ID%>">
                                            <%: item.ShortName%></option>
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
                                <td align="center">
                                    <div>
                                        <input id="addEmployeeButton" onclick="MoveEmployeeProjectListToAssignedList('notAssignedEmployeesDepartment','assignedEmployeeList')"
                                            type="button" value="&gt;&gt;" /></div>
                                    <div>
                                        <input id="removeEmployeeButton" onclick="MoveEmployeeAssignedListToProjectList('assignedEmployeeList','notAssignedEmployeesDepartment')"
                                            type="button" value="&lt;&lt;" /></div>
                                </td>
                                <td>
                                    <div id="emploeeListDiv">
                                        <select id="assignedEmployeeList" name="Employees" size="10" multiple="multiple" ondblclick="MoveEmployeeAssignedListToProjectList('assignedEmployeeList','notAssignedEmployees')">
                                            <%
                               foreach (EmployeeModel item in assignedEmployees)
                               {
                                            %>
                                            <option id="<%: item.ID%>">
                                                <%: item.ShortName%></option>
                                            <%
                               };
                                            %>
                                        </select>
                                    </div>
                                    <div id="div_hidden">
                                        <%
                               foreach (EmployeeModel item in assignedEmployees)
                               {
                                        %>
                                        <div id='<%: "employee"+item.ID%>'>
                                            <input type='hidden' name='AssignedEmployeeIDs' value='<%: item.ID%>' /></div>
                                        <%
                               };
                                        %>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
            </td>
        </tr>
    </table>
    <br />
    <%} %>
</div>
