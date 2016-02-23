<%@ Control Language="C#"  Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.ViewModels.ProjectsOverviewViewModel>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Filters" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>
 <%
     var projectTypes = ((ProjectEditorViewModel)ViewBag.ProjectEditorViewModel).Types;
     var statuses = ((ProjectEditorViewModel)ViewBag.ProjectEditorViewModel).ViewStatuses;
     var filter = new ProjectsOverviewFilter();

     if (Session["ProjectsOverviewFilter"] != null)
     {
         filter = (ProjectsOverviewFilter)Session["ProjectsOverviewFilter"];
     }
     using (Html.BeginForm("Index", "ProjectsOverview", FormMethod.Post))
     {
%>
 <table width='100%'>
    <tr>
     <%if (this.Session["userDepartmentId"] != null)
           if (Model.IsAdmin || (bool)this.Session["RequestPermission"])
       {%>
      <td>
           <!--   <a class='t-button-custom' href='/ProjectsOverview?Projects-mode=insert'>
            <span class='t-add t-icon'></span>������� ����� ������</a>-->
            <span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </span>
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
                    .Name("filteringByProjectTypeID")
                    .BindTo(new SelectList(projectTypes, "ID", "Name", filter.FilteringByProjectTypeID)).Value(filter.FilteringByProjectTypeID.ToString())%>
            <div>
             <%:Html.Telerik()
                    .DropDownList()
                    .Name("FilteringByStatusID")
                    .BindTo(new SelectList(statuses, "ID", "Name", filter.FilteringByStatusID)).Value(filter.FilteringByStatusID.ToString())%>

            </div>
         </td>
                
                <td>
                    <div>�������� �����������</div>
                        <div>��� ���� �������</div>
                   </td>
                   <td>
                 <%: Html.CheckBox("filteringByAssignedProjects", filter.FilteringByAssignedProjects)%>
                </td>
                   <td>
                    <input type="submit" value="���������"  />
                   </td>
        <td>
                    <div>�����:</div>
        </td>
       <td style='text-align:right'>
            <%: Html.TextBox("FilteringBySearchWord", filter.FilteringBySearchWord)%>
           <input type='submit' value='�����' /> 
       </td>
    </tr>
</table>
<%}%>
       