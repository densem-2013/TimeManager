<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.Models.TaskModel>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
   <% var statusName = Model.CurrentStatusName;
      if (statusName == "��������")
        {
            %><div class="timeregistration-editing-nonzerovalue;" style="text-align:center;"><%:
            statusName
            %><br />
            <a style="white-space:nowrap;" href="#" onclick='TaskCloseConfirm("<%: Model.ID %>")'><span class="t-icon t-close"></span>�������</a>
            </div>
           <%
        }
        else
        {
            %><div class="timeregistration-editing-zerovalue"><%:
            statusName
            %></div><%
        }
 %>