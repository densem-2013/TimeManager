<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.DateTime>" %>
<a class='t-grid-header' href="">
        <div><%: string.Format("{0:dd.MM}",Model)%></div>
        <div><%: string.Format("{0:ddd}", Model)%></div>
</a>