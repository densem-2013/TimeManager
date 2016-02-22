<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.TimeSpan>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<div class="timeregistration-editing-nonzerovalue">
    <%:
        Model.ToFormatedTime()
    %>
</div>
