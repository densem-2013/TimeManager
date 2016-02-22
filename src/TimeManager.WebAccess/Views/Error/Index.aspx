<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Infocom.TimeManager.WebAccess.Extensions.ErrorDisplay>"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Ошибка обработки запроса
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Внимание!</h2>

<div style="color:Red;">
      <p> Произошла ошибка при обработке запроса.</p>
       Данные об ошибке отправлены службе поддержки Timemanager.</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server">
</asp:Content>
