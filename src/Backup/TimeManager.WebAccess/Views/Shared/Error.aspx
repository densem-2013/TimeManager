<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Ошибка при обработке
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <br />
    <div style="color:Red;">
      <p> Произошла ошибка при обработке запроса.</p>
       Данные об ошибке отправлены службе поддержки Timemanager.</div>
    <br />
   
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server">
</asp:Content>
