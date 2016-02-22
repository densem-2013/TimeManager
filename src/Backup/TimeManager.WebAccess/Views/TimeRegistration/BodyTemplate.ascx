<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.TimeSpan>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<%var spentTime = Model.ToFormatedTime();
    if (spentTime != "00:00")
    {
        %><div class="timeregistration-editing-nonzerovalue"><%:
        spentTime
        %></div><%
    }
    else
    {
        %><div class="timeregistration-editing-zerovalue"><%:
        spentTime
        %></div><%
    }
 %>